// -----------------------------------------------------------------------
// <copyright file="FusionInstallReference.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace PowerShell.Infrastructure.Fusion.WrappedFusion
{
    /// <summary>
    ///     The <see cref="FusionInstallReference" /> structure represents a reference that is made
    ///     when an application has installed an assembly in the GAC.
    /// </summary>
    /// <remarks>
    ///     Native name: FUSION_INSTALL_REFERENCE
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct FusionInstallReference
    {
        #region Variables

        /// <summary>
        ///     The size of the structure in bytes.
        /// </summary>
        /// <remarks>
        ///     <see cref="Marshal.SizeOf(System.Type)" /> returns 32 when passing the type of
        ///     <see cref="FusionInstallReference" />,
        ///     but after extensive debugging it is determined that the real size in unmanaged memory is 40 bytes.
        ///     Testing is done on Windows XP x86 and on Windows Vista x64.
        /// </remarks>
        private readonly uint _size;
        /// <summary>
        ///     Reserved, must be zero.
        /// </summary>
        private readonly uint _flags;
        /// <summary>
        ///     The entity that adds the reference.
        /// </summary>
        public Guid GuidScheme;
        /// <summary>
        ///     A unique string that identifies the application that installed the assembly.
        /// </summary>
        public string Identifier;
        /// <summary>
        ///     A string that is only understood by the entity that adds the reference. The GAC only stores this string.
        /// </summary>
        public string NonCannonicalData;

        #endregion

        #region Constructors

        public FusionInstallReference(Guid entity, string uniqueId, string nonCannonicalData)
        {
            // The following line sets _size to 32
            //    _size = (uint)Marshal.SizeOf(typeof (FusionInstallReference));
            // When the following code is used the size is returned as 40:
            //    int size = Marshal.SizeOf(new FusionInstallReference(...));
            // After extensive testing it is determined that _size must always be 40.
            // _size is not made a constant because then it would be impossible to marshal instances of FusionInstallReference
            _size = 40;
            _flags = 0;
            GuidScheme = entity;
            Identifier = uniqueId;
            NonCannonicalData = nonCannonicalData;
        }

        #endregion
    }
}
