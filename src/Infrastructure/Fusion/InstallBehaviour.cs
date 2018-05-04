// -----------------------------------------------------------------------
// <copyright file="InstallBehaviour.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace PowerShell.Infrastructure.Fusion
{
    /// <summary>
    ///     Defines the rules determing when an assembly must be installed in the GAC.
    /// </summary>
    public enum InstallBehaviour
    {
        /// <summary>
        ///     Install the assembly only if there it has not been installed already in the GAC.
        /// </summary>
        Default = 0,
        /// <summary>
        ///     If the assembly is already installed in the GAC and the file version numbers of the assembly being
        ///     installed are the same or later, the files are replaced.
        /// </summary>
        Refresh = 1,
        /// <summary>
        ///     The files of an existing assembly are overwritten regardless of their version number.
        /// </summary>
        ForceRefresh = 2
    }
}
