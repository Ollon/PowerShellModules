// -----------------------------------------------------------------------
// <copyright file="IInstallReferenceItem.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace PowerShell.Infrastructure.Fusion.WrappedFusion
{
    /// <summary>
    ///     The IInstallReferenceItem interface represents a reference that has been set on an assembly in the GAC.
    ///     Instances of IInstallReferenceItem are returned by the <see cref="IInstallReferenceEnum" /> interface.
    /// </summary>
    [ComImport]
    [Guid("582dac66-e678-449f-aba6-6faaec8a9394")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IInstallReferenceItem
    {
        /// <summary>
        ///     The IInstallReferenceItem::GetReference method returns a <see cref="FusionInstallReference" /> structure.
        /// </summary>
        /// <param name="ppRefData">
        ///     A pointer to a <see cref="FusionInstallReference" /> structure.
        ///     The memory is allocated by the GetReference method and is freed when IInstallReferenceItem is released.
        ///     Callers must not hold a reference to this buffer after the IInstallReferenceItem object is released.
        /// </param>
        /// <param name="dwFlags">Must be zero.</param>
        /// <param name="pvReserved">Must be null.</param>
        /// <returns></returns>
        [PreserveSig]
        int GetReference(
            out IntPtr ppRefData,
            uint dwFlags,
            IntPtr pvReserved);
    }
}
