// -----------------------------------------------------------------------
// <copyright file="UninstallDisposition.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace PowerShell.Infrastructure.Fusion
{
    /// <summary>
    ///     Represents the result of an attempt to uninstall an assembly from the global assembly cache.
    /// </summary>
    public enum UninstallDisposition
    {
        None = 0x00,
        /// <summary>
        ///     The assembly files have been removed from the GAC.
        /// </summary>
        Uninstalled = 0x01,
        /// <summary>
        ///     [Windows 9x] An application is using the assembly.
        /// </summary>
        StillInUse = 0x02,
        /// <summary>
        ///     The assembly does not exist in the GAC.
        /// </summary>
        AlreadyUninstalled = 0x03,
        [Obsolete("This value is not used by the Fusion API")] DeletePending = 0x04,
        /// <summary>
        ///     The assembly has not been removed from the GAC because another application reference exists.
        /// </summary>
        HasReferences = 0x05,
        /// <summary>
        ///     The reference is not found in the GAC.
        /// </summary>
        ReferenceNotFound = 0x06
    }
}
