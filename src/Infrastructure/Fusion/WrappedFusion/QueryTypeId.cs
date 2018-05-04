// -----------------------------------------------------------------------
// <copyright file="QueryTypeId.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace PowerShell.Infrastructure.Fusion.WrappedFusion
{
    /// <summary>
    ///     Specifies the information to retrieve when querying assembly information.
    /// </summary>
    internal enum QueryTypeId
    {
        /// <summary>
        ///     No information type specified.
        /// </summary>
        None = 0x0,
        /// <summary>
        ///     Validates the assembly files in the side-by-side assembly store against the assembly manifest.
        ///     This includes the verification of the assembly's hash and strong name signature.
        /// </summary>
        Validate = 0x1,
        /// <summary>
        ///     Returns the size of all files in the assembly.
        /// </summary>
        Size = 0x2
    }
}
