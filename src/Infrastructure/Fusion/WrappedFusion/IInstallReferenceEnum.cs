// -----------------------------------------------------------------------
// <copyright file="IInstallReferenceEnum.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace PowerShell.Infrastructure.Fusion.WrappedFusion
{
    /// <summary>
    ///     The IInstallReferenceEnum interface enumerates all references that are set on an assembly in the GAC.
    /// </summary>
    /// <remarks>
    ///     References that belong to the assembly are locked for changes while those references are being enumerated.
    /// </remarks>
    [ComImport]
    [Guid("56b1a988-7c0c-4aa2-8639-c3eb5a90226f")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IInstallReferenceEnum
    {
        /// <summary>
        ///     IInstallReferenceEnum::GetNextInstallReferenceItem returns the next reference information for an assembly.
        /// </summary>
        /// <param name="ppRefItem">Pointer to a memory location that receives the <see cref="IInstallReferenceItem" /> pointer.</param>
        /// <param name="dwFlags">Must be zero.</param>
        /// <param name="pvReserved">Must be null.</param>
        /// <returns></returns>
        [PreserveSig]
        int GetNextInstallReferenceItem(
            out IInstallReferenceItem ppRefItem,
            uint dwFlags,
            IntPtr pvReserved);
    }
}
