// -----------------------------------------------------------------------
// <copyright file="IHistoryReader.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.InteropServices;
using System.Text;
using FileTime = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace PowerShell.Infrastructure.Fusion.WrappedFusion
{
    /// <summary>
    ///     Undocumented interface. Declared for potential future implementations.
    /// </summary>
    /// <remarks>
    ///     An instance is retrieved with the CreateHistoryReader function.
    ///     See <see cref="http://msdn.microsoft.com/en-us/library/ms231201.aspx" />,
    ///     <see cref="http://www.koders.com/csharp/fid9779AA56CDC41CCC0356DD0D4C5DEAB13F5E2495.aspx?s=button#L165" />,
    ///     <see cref="http://www.koders.com/csharp/fid9779AA56CDC41CCC0356DD0D4C5DEAB13F5E2495.aspx?s=button#L224" />,
    ///     <see cref="http://www.koders.com/csharp/fid9779AA56CDC41CCC0356DD0D4C5DEAB13F5E2495.aspx?s=button#L232" />.
    /// </remarks>
    [ComImport]
    [Guid("1D23DF4D-A1E2-4B8B-93D6-6EA3DC285A54")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IHistoryReader
    {
        [PreserveSig]
        int GetFilePath(
            [MarshalAs(UnmanagedType.LPWStr)] out StringBuilder wzFilePath,
            ref uint pdwSize);

        [PreserveSig]
        int GetApplicationName(
            [MarshalAs(UnmanagedType.LPWStr)] out StringBuilder wzAppName,
            ref uint pdwSize);

        [PreserveSig]
        int GetEXEModulePath(
            [MarshalAs(UnmanagedType.LPWStr)] out StringBuilder wzExePath,
            ref uint pdwSize);

        void GetNumActivations(
            out uint pdwNumActivations);

        void GetActivationDate(
            uint dwIdx, // One based.
            out FileTime pftDate);

        [PreserveSig]
        int GetRunTimeVersion(
            ref FileTime pftActivationDate,
            [MarshalAs(UnmanagedType.LPWStr)] out StringBuilder wzRunTimeVersion,
            ref uint pdwSize);

        void GetNumAssemblies(
            ref FileTime pftActivationDate,
            out uint pdwNumAsms);

        void GetHistoryAssembly(
            ref FileTime pftActivationDate,
            uint dwIdx, // One based.
            [MarshalAs(UnmanagedType.IUnknown)] out object ppHistAsm);
    }
}
