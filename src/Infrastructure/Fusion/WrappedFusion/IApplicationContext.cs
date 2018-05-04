// -----------------------------------------------------------------------
// <copyright file="IApplicationContext.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace PowerShell.Infrastructure.Fusion.WrappedFusion
{
    /// <summary>
    ///     Undocumented.
    /// </summary>
    [ComImport]
    [Guid("7C23FF90-33AF-11D3-95DA-00A024A85B51")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IApplicationContext
    {
        void SetContextNameObject(
            IAssemblyName pName);

        void GetContextNameObject(
            out IAssemblyName ppName);

        void Set(
            [MarshalAs(UnmanagedType.LPWStr)] string szName,
            int pvValue,
            uint cbValue,
            uint dwFlags);

        void Get(
            [MarshalAs(UnmanagedType.LPWStr)] string szName,
            out int pvValue,
            ref uint pcbValue,
            uint dwFlags);

        void GetDynamicDirectory(
            [Out] out int wzDynamicDir,
            ref uint pdwSize);
    }
}
