// -----------------------------------------------------------------------
// <copyright file="AssemblyEnumerator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using PowerShell.Infrastructure.Fusion.Helpers;
using PowerShell.Infrastructure.Fusion.WrappedFusion;

namespace PowerShell.Infrastructure.Fusion
{
    internal class AssemblyEnumerator : IEnumerator<AssemblyName>
    {
        /// <summary>
        ///     To obtain an instance of the CreateAssemblyEnum API, call the CreateAssemblyNameObject API.
        /// </summary>
        /// <param name="pEnum">Pointer to a memory location that contains the IAssemblyEnum pointer.</param>
        /// <param name="pUnkReserved">Must be null.</param>
        /// <param name="pName">
        ///     An assembly name that is used to filter the enumeration. Can be null to enumerate all assemblies in
        ///     the GAC.
        /// </param>
        /// <param name="dwFlags">Exactly one bit from the ASM_CACHE_FLAGS enumeration.</param>
        /// <param name="pvReserved">Must be NULL.</param>
        [DllImport("fusion.dll", SetLastError = true, PreserveSig = false)]
        private static extern void CreateAssemblyEnum(out IAssemblyEnum pEnum,
            IntPtr pUnkReserved,
            IAssemblyName pName,
            AssemblyCacheFlags dwFlags,
            IntPtr pvReserved);

        private readonly IAssemblyEnum _enum;
        private AssemblyName _current;

        public AssemblyEnumerator()
        {
            CreateAssemblyEnum(out _enum, (IntPtr)0, null, AssemblyCacheFlags.GAC, (IntPtr)0);
        }

        public AssemblyName Current
        {
            get
            {
                return _current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return _current;
            }
        }

        public bool MoveNext()
        {
            int hResult = _enum.GetNextAssembly(out IApplicationContext context, out IAssemblyName ae, 0);
            _current = ae == null || !HResult.IsSuccess(hResult) ? null : ae.ToAssemblyName();
            return _current != null;
        }

        public void Reset()
        {
            _enum.Reset();
        }

        public void Dispose()
        {
        }
    }
}
