// -----------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace PowerShell.Infrastructure.Fusion.WrappedFusion
{
    /// <summary>
    ///     The <see cref="AssemblyInfo" /> structure contains information about an assembly in the side-by-side assembly
    ///     store.
    ///     The information is used by the <see cref="IAssemblyCache.QueryAssemblyInfo" /> method.
    /// </summary>
    /// <remarks>
    ///     Native name: ASSEMBLY_INFO
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct AssemblyInfo
    {
        /// <summary>
        ///     Size of the structure in bytes. Permits additions to the structure in future version of the .NET Framework.
        /// </summary>
        public uint cbAssemblyInfo;
        /// <summary>
        ///     The flags set when querying info using <see cref="IAssemblyCache.QueryAssemblyInfo" />.
        /// </summary>
        public AssemblyInfoFlags assemblyFlags;
        /// <summary>
        ///     The size of the files that make up the assembly in kilobytes (KB).
        /// </summary>
        public ulong assemblySize;
        /// <summary>
        ///     The current path of the directory that contains the files that make up the assembly. The path must end with a zero.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)] public string currentAssemblyPath;
        /// <summary>
        ///     Size of the buffer that the currentAssemblyPath field points to.
        /// </summary>
        public uint currentAssemblyPathSize;
    }
}
