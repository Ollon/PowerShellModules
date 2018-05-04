using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;
using PowerShell.Infrastructure.Utilities;

namespace PowerShell.Infrastructure.Commands
{
    public partial class RestartWorkstationCommand
    {
        private bool _confirmed = false;

        protected override void BeginProcessing()
        {
            if (MyInvocation.BoundParameters.ContainsKey("Confirm"))
            {
                _confirmed = (SwitchParameter)MyInvocation.BoundParameters["Confirm"];
            }

            if (string.IsNullOrEmpty(ComputerName))
            {
                ComputerName = Environment.MachineName;
            }

            if (Credential == null)
            {
                Credential = PSCredential.Empty;
            }
        }

        protected override void ProcessRecord()
        {
            WriteVerbose($"   Force       = {Force}");
            WriteVerbose($"   Confirm     = {_confirmed}");
            WriteVerbose($"   ComputerName = {ComputerName}");

            if (Force && !_confirmed)
            {
                RestartMachine(ComputerName);
            }
            else
            {
                if (ShouldContinue($"Do you want to restart computer {ComputerName}?",
                    $"Restart Computer {ComputerName}"))
                {
                    RestartMachine(ComputerName);
                }
            }
        }

        private void RestartMachine(string computerName)
        {
            using (CimSession session = CimSession.Create(computerName, GetSessionOptionsFromPSCredential(Credential)))
            {
                IEnumerable<CimInstance> instances =
                    session.EnumerateInstances(CimNamespaces.CimV2, CimClassNames.OperatingSystem);
                CimInstance instance = instances.FirstOrDefault();
                if (instance != null)
                {
                    session.InvokeMethod(instance, CimMethodNames.Reboot, null);
                }
            }
        }
    }
}
