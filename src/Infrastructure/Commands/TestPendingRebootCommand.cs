using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;
using PowerShell.Infrastructure.Utilities;

namespace PowerShell.Infrastructure.Commands
{
    public partial class TestPendingRebootCommand
    {
        private const uint HKLM = 0x80000002;

        private const string Confirm = nameof(Confirm);
        private const string ComputerNameName = nameof(ComputerName);
        private const string RebootPending = nameof(RebootPending);
        private const string RebootRequired = nameof(RebootRequired);
        private const string PendingFileRenameOperations = nameof(PendingFileRenameOperations);



        /// <summary>
        /// When overridden in the derived class, performs initialization
        /// of command execution.
        /// Default implementation in the base class just returns.
        /// </summary>
        /// <exception cref="T:System.Exception">
        /// This method is overridden in the implementation of
        /// individual Cmdlets, and can throw literally any exception.
        /// </exception>
        protected override void BeginProcessing()
        {
            if (string.IsNullOrEmpty(ComputerName))
            {
                ComputerName = Environment.MachineName;
            }
        }

        /// <summary>
        /// When overridden in the derived class, performs execution
        /// of the command.
        /// </summary>
        /// <exception cref="T:System.Exception">
        /// This method is overridden in the implementation of
        /// individual Cmdlets, and can throw literally any exception.
        /// </exception>
        protected override void ProcessRecord()
        {
            RestartInfo info = new RestartInfo();
            CimSession session = CimSession.Create(ComputerName);
            CimInstance instance = session.EnumerateInstances("ROOT\\cimV2", "Win32_OperatingSystem")?.FirstOrDefault();
            CimProperty buildNumber = instance.CimInstanceProperties["BuildNumber"];
            CimProperty csName = instance.CimInstanceProperties["CSName"];
            CimClass cimRegistryClass = session.GetClass("ROOT\\default", "StdRegProv");
            if ((int.Parse(buildNumber.Value.ToString()) > 6001))
            {
                info.ComputerName = CimGetStringValue(ComputerName, RegistryPaths.ComputerName, ComputerNameName);
                info.ActiveComputerName = CimGetStringValue(ComputerName, RegistryPaths.ActiveComputerName, ComputerNameName);
                info.ComputerPendingRename = info.ComputerName != info.ActiveComputerName;
                info.CBSRebootPend = CimEnumKey(ComputerName, RegistryPaths.ComponentBasedServicing).Contains(RebootPending);
                info.PendingFileRenameOperations =
                    CimEnumKey(ComputerName, RegistryPaths.SessionManager).Contains(PendingFileRenameOperations);
                info.WindowsAutoUpdate = CimEnumKey(ComputerName, RegistryPaths.WindowsAutoUpdate).Contains(RebootRequired);
                if (info.CBSRebootPend || info.PendingFileRenameOperations || info.WindowsAutoUpdate || info.ComputerPendingRename)
                {
                    if (Force || ShouldProcess("Restart computer " + ComputerName))
                    {
                        RestartMachine(ComputerName);
                    }
                }
                WriteObject(info);
            }
            session.Dispose();
        }

        private static string CimGetMultiStringValue(string computerName, string subKey)
        {
            using (CimSession session = CimSession.Create(computerName))
            {
                CimClass cimRegistryClass = session.GetClass("ROOT\\default", "StdRegProv");
                CimInstance regInstance = new CimInstance(cimRegistryClass.CimSystemProperties.ClassName, cimRegistryClass.CimSystemProperties.Namespace);
                var result1 = session.InvokeMethod(regInstance, "GetMultiStringValue", new CimMethodParametersCollection
                {
                    CimMethodParameter.Create("hDefKey", HKLM, CimFlags.Indication | CimFlags.In ),
                    CimMethodParameter.Create("sSubKeyName",subKey, CimFlags.Indication | CimFlags.In ),
                    CimMethodParameter.Create("sValueName", "ComputerName",  CimFlags.Indication | CimFlags.In )
                });

                return (string)result1.OutParameters["sValue"].Value;
            }
        }


        private static string CimGetStringValue(string computerName, string subKey, string valueName)
        {
            using (CimSession session = CimSession.Create(computerName))
            {
                CimClass cimRegistryClass = session.GetClass("ROOT\\default", "StdRegProv");
                CimInstance regInstance = new CimInstance(cimRegistryClass.CimSystemProperties.ClassName, cimRegistryClass.CimSystemProperties.Namespace);
                var result1 = session.InvokeMethod(regInstance, "GetStringValue", new CimMethodParametersCollection
                {
                    CimMethodParameter.Create("hDefKey", HKLM, CimFlags.Indication | CimFlags.In ),
                    CimMethodParameter.Create("sSubKeyName",subKey, CimFlags.Indication | CimFlags.In ),
                    CimMethodParameter.Create("sValueName", valueName,  CimFlags.Indication | CimFlags.In )
                });

                return (string)result1.OutParameters["sValue"].Value;
            }
        }

        private static string[] CimEnumKey(string computerName, string subKey)
        {
            using (CimSession session = CimSession.Create(computerName))
            {
                CimClass cimRegistryClass = session.GetClass("ROOT\\default", "StdRegProv");
                CimInstance regInstance = new CimInstance(cimRegistryClass.CimSystemProperties.ClassName, cimRegistryClass.CimSystemProperties.Namespace);
                var result1 = session.InvokeMethod(regInstance, "EnumKey", new CimMethodParametersCollection
                {
                    CimMethodParameter.Create("hDefKey", HKLM, CimFlags.Indication | CimFlags.In ),
                    CimMethodParameter.Create("sSubKeyName",subKey, CimFlags.Indication | CimFlags.In ),
                });

                return (string[])result1.OutParameters["sNames"].Value;
            }

        }

        private void RestartMachine(string computerName)
        {
            if (Credential != null)
            {
                CimSessionOptions options = GetSessionOptionsFromPSCredential();
                using (CimSession session = CimSession.Create(computerName, options))
                {
                    IEnumerable<CimInstance> instances =
                        session.EnumerateInstances(CimNamespaces.CimV2, CimClassNames.OperatingSystem);
                    CimInstance instance = instances.FirstOrDefault();
                    session.InvokeMethod(instance, CimMethodNames.Reboot, null);
                }
            }
            else
            {
                using (CimSession session = CimSession.Create(computerName))
                {
                    IEnumerable<CimInstance> instances =
                        session.EnumerateInstances(CimNamespaces.CimV2, CimClassNames.OperatingSystem);
                    CimInstance instance = instances.FirstOrDefault();
                    session.InvokeMethod(instance, CimMethodNames.Reboot, null);
                }
            }

        }

        private CimSessionOptions GetSessionOptionsFromPSCredential()
        {
            string domain = Credential.GetNetworkCredential().Domain;
            string userName = Credential.GetNetworkCredential().UserName;
            SecureString password = Credential.GetNetworkCredential().SecurePassword;
            CimCredential credential = new CimCredential(PasswordAuthenticationMechanism.NtlmDomain, domain, userName, password);
            CimSessionOptions options = new CimSessionOptions();
            options.AddDestinationCredentials(credential);
            return options;
        }
    }

    public struct RestartInfo
    {
        public string ComputerName { get; set; }
        public string ActiveComputerName { get; set; }
        public bool ComputerPendingRename { get; set; }
        public bool PendingFileRenameOperations { get; set; }
        public bool WindowsAutoUpdate { get; set; }
        public bool CBSRebootPend { get; set; }
    }
}
