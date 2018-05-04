// -----------------------------------------------------------------------
// <copyright file="AssemblyInfoFlags.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace PowerShell.Infrastructure.Fusion.WrappedFusion
{
    /// <summary>
    ///     Provides extra information on the <see cref="AssemblyInfo" />.
    /// </summary>
    internal enum AssemblyInfoFlags
    {
        /// <summary>
        ///     No flags set.
        /// </summary>
        None = 0x0,
        /// <summary>
        ///     Indicates that the assembly is actually installed.
        ///     Always set in current version of the .NET Framework.
        /// </summary>
        Installed = 0x01,
        /// <summary>
        ///     Never set in the current version of the .NET Framework.
        /// </summary>
        PayloadResident = 0x02
    }
}
