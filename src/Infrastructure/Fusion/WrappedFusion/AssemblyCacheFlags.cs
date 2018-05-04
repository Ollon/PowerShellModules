// -----------------------------------------------------------------------
// <copyright file="AssemblyCacheFlags.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace PowerShell.Infrastructure.Fusion.WrappedFusion
{
    /// <summary>
    ///     Indicates the source of an assembly represented by <see cref="IAssemblyCacheItem" /> in the global assembly cache.
    /// </summary>
    /// <remarks>
    ///     Native name: ASM_CACHE_FLAGS
    /// </remarks>
    [Flags]
    internal enum AssemblyCacheFlags
    {
        /// <summary>
        ///     Enumerates the cache of precompiled assemblies by using Ngen.exe.
        /// </summary>
        /// <remarks>
        ///     Native name: ASM_CACHE_ZAP
        /// </remarks>
        ZAP = 0x1,
        /// <summary>
        ///     Enumerates the GAC.
        /// </summary>
        /// <remarks>
        ///     Native name: ASM_CACHE_GAC
        /// </remarks>
        GAC = 0x2,
        /// <summary>
        ///     Enumerates the assemblies that have been downloaded on-demand or that have been shadow-copied.
        /// </summary>
        /// <remarks>
        ///     Native name: ASM_CACHE_DOWNLOAD
        /// </remarks>
        Download = 0x4
    }
}
