using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using System.Security.Principal;

namespace PowerShell.Infrastructure.Utilities
{
    internal static class UACUtilities
    {
        public static void EnsureElevation(PSCmdlet cmdlet)
        {
            if (!IsElevated())
            {
                Elevate(cmdlet);
            }
        }
        public static bool IsElevated()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void Elevate(PSCmdlet cmdlet)
        {
            string commandLine = cmdlet.MyInvocation.InvocationName;
            List<object> unboundArguments = cmdlet.MyInvocation.UnboundArguments;

            string currentPath = cmdlet.SessionState.Path.CurrentLocation.Path;


            Process powerShell = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Environment.SystemDirectory + "\\WindowsPowerShell\\v1.0\\powerShell.exe",
                    Arguments = $"-NoExit -Command Set-Location \"{currentPath}\"",
                    Verb = "RunAs"
                }
            };

            powerShell.Start();
        }
    }
}
