// -----------------------------------------------------------------------
// <copyright file="IAssemblyEnum.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace PowerShell.Infrastructure.Fusion.WrappedFusion
{
    /// <summary>
    ///     The IAssemblyEnum interface enumerates the assemblies in the GAC.
    /// </summary>
    [ComImport]
    [Guid("21b8916c-f28e-11d2-a473-00c04f8ef448")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAssemblyEnum
    {
        /// <summary>
        ///     The IAssemblyEnum::GetNextAssembly method enumerates the assemblies in the GAC.
        /// </summary>
        /// <param name="ppAppCtx">Must be null.</param>
        /// <param name="ppName">
        ///     Pointer to a memory location that is to receive the interface pointer to the assembly
        ///     name of the next assembly that is enumerated.
        /// </param>
        /// <param name="dwFlags">Must be zero.</param>
        /// <returns></returns>
        [PreserveSig]
        int GetNextAssembly(
            out IApplicationContext ppAppCtx,
            out IAssemblyName ppName,
            uint dwFlags);

        /// <summary>
        ///     Reset the enumeration to the first assembly.
        /// </summary>
        /// <returns></returns>
        [PreserveSig]
        int Reset();

        /// <summary>
        ///     Create a copy of the assembly enum that is independently enumerable.
        /// </summary>
        /// <param name="ppEnum"></param>
        /// <returns></returns>
        [PreserveSig]
        int Clone(
            out IAssemblyEnum ppEnum);
    }
}
