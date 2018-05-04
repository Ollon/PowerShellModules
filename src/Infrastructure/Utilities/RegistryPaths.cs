// -----------------------------------------------------------------------
// <copyright file="RegistryPaths.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------


namespace PowerShell.Infrastructure.Utilities
{
    public static class RegistryPaths
    {
        public const string ComputerName = @"SYSTEM\ControlSet001\Control\ComputerName\ComputerName";
        public const string ActiveComputerName = @"SYSTEM\ControlSet001\Control\ComputerName\ActiveComputerName";


        public const string SessionManager = @"SYSTEM\CurrentControlSet\Control\Session Manager";
        public const string SessionManager001 = @"SYSTEM\ControlSet001\Control\Session Manager";
        public const string SessionManager002 = @"SYSTEM\ControlSet002\Control\Session Manager";

        public const string WindowsAutoUpdate = @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update";

        public const string ComponentBasedServicing = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing";

    }


    public static class HKLM
    {
        public static class SYSTEM
        {
            public static class CurrentControlSet
            {
                public static class Control
                {
                    public static class Session_Manager
                    {
                        public const string SessionManager = @"SYSTEM\CurrentControlSet\Control\Session Manager";
                    }
                }
            }

            public static class ControlSet001
            {
                public static class Control
                {
                    public const string SessionManager = "SYSTEM\\ControlSet001\\Control\\Session Manager";
                }
            }

            public static class ControlSet002
            {
                public static class Control
                {
                    public const string SessionManager = "SYSTEM\\ControlSet002\\Control\\Session Manager";
                }
            }
        }
    }
}
