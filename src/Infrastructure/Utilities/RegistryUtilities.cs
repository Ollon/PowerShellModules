// -----------------------------------------------------------------------
// <copyright file="RegistryUtilities.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace PowerShell.Infrastructure.Utilities
{
    public static class RegistryUtilities
    {
        private const string CommandStoreKeyString = @"Software\Microsoft\Windows\CurrentVersion\Explorer\CommandStore";

        public static RegistryKey CommandStore
        {
            get
            {
                return Registry.LocalMachine.OpenSubKey(CommandStoreKeyString, true);
            }
        }

        public static IEnumerable<string> ShellKeyNames
        {
            get
            {
                return CommandStoreShell.GetSubKeyNames().AsEnumerable();
            }
        }

        public static RegistryKey CommandStoreShell
        {
            get
            {
                return CommandStore.OpenSubKey("Shell", true);
            }
        }

        public static RegistryKey ComponentBasedServicing(string computerName)
        {
            return RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, computerName, RegistryView.Registry32)
                .OpenSubKey(RegistryPaths.ComponentBasedServicing);
        }

        public static RegistryKey WindowsUpdateAutoUpdate()
        {
            return Registry.LocalMachine.OpenSubKey(RegistryPaths.WindowsAutoUpdate);
        }

        public static RegistryKey SessionManager(string computerName)
        {
            return RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, computerName, RegistryView.Registry32)
                .OpenSubKey(RegistryPaths.SessionManager);
        }

        public static string RemoteMachineName(string computerName)
        {
            RegistryKey registryKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, computerName, RegistryView.Registry32);
            RegistryKey subKey = registryKey.OpenSubKey(PowerShellResources.RegistryKeyComputerName);
            object value = subKey.GetValue("ComputerName");
            return value.ToString();
        }
    }
}
