// -----------------------------------------------------------------------
// <copyright file="AssemblyNameExt.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using PowerShell.Infrastructure.Fusion.Helpers;

namespace PowerShell.Infrastructure.Fusion.WrappedFusion
{
    /// <summary>
    ///     Extension methods for <see cref="AssemblyName" /> and for <see cref="IAssemblyName" />.
    /// </summary>
    internal static class AssemblyNameExt
    {
        #region Public Methods

        /// <summary>
        ///     Converts the value of this instance to its equivalent <see cref="AssemblyName" />.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static AssemblyName ToAssemblyName(this IAssemblyName assemblyName)
        {
            AssemblyName result = new AssemblyName
            {
                Name = assemblyName.GetName(),
                Version = assemblyName.GetVersion(),
                CultureInfo = new CultureInfo(assemblyName.GetProperty<string>(AssemblyNamePropertyId.Culture)),
                CodeBase = assemblyName.GetProperty<string>(AssemblyNamePropertyId.CodebaseUrl)
            };
            result.SetPublicKey(assemblyName.GetProperty<byte[]>(AssemblyNamePropertyId.PublicKey));
            result.SetPublicKeyToken(assemblyName.GetProperty<byte[]>(AssemblyNamePropertyId.PublicKeyToken));
            string tmp = assemblyName.GetDisplayName(DisplayNameFlags.ProcessArchitecture);
            tmp = tmp.Substring(tmp.LastIndexOf('=') + 1);
            if (ParserHelper.TryParseEnum(tmp, out ProcessorArchitecture architecture))
            {
                result.ProcessorArchitecture = architecture;
            }
            return result;
        }

        /// <summary>
        ///     Converts the value of this instance to its equivalent <see cref="IAssemblyName" />.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static IAssemblyName ToIAssemblyName(this AssemblyName assemblyName)
        {
            GlobalAssemblyCache.CreateAssemblyNameObject(out IAssemblyName result,
                assemblyName.FullName,
                CreateDisposition.ParseDisplayName,
                IntPtr.Zero);
            result.SetProperty(AssemblyNamePropertyId.CodebaseUrl, assemblyName.CodeBase);
            result.SetProperty(AssemblyNamePropertyId.Culture, assemblyName.CultureInfo?.ToString());
            result.SetProperty(AssemblyNamePropertyId.MajorVersion,
                assemblyName.Version == null ? null : (object)(short)assemblyName.Version.Major);
            result.SetProperty(AssemblyNamePropertyId.MinorVersion,
                assemblyName.Version == null ? null : (object)(short)assemblyName.Version.Minor);
            result.SetProperty(AssemblyNamePropertyId.Name, assemblyName.Name);
            result.SetProperty(AssemblyNamePropertyId.PublicKey, assemblyName.GetPublicKey());
            result.SetProperty(AssemblyNamePropertyId.PublicKeyToken, assemblyName.GetPublicKeyToken());
            return result;
        }

        /// <summary>
        ///     Returns the most specific available name, which is still compatible with the fusion API.
        /// </summary>
        /// <remarks>
        ///     <see cref="AssemblyName.FullName" /> is not compatible with fusion because of "PublicKeyToken" being specified.
        /// </remarks>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static string GetFusionCompatibleFullName(this AssemblyName assemblyName)
        {
            return assemblyName.Name +
                   (assemblyName.Version == null ? "" : ", Version=" + assemblyName.Version) +
                   (string.IsNullOrEmpty(assemblyName.CultureInfo?.Name) ? "" : ", Culture=" + assemblyName.CultureInfo.Name);
        }

        #endregion

        #region Private Methods - IAssemblyName

        private static string GetName(this IAssemblyName name)
        {
            uint bufferSize = 255;
            StringBuilder buffer = new StringBuilder((int)bufferSize);
            name.GetName(ref bufferSize, buffer);
            return buffer.ToString();
        }

        private static string GetDisplayName(this IAssemblyName name, DisplayNameFlags which)
        {
            uint bufferSize = 255;
            StringBuilder buffer = new StringBuilder((int)bufferSize);
            name.GetDisplayName(buffer, ref bufferSize, which);
            return buffer.ToString();
        }

        private static Version GetVersion(this IAssemblyName name)
        {
            name.GetVersion(out uint versionHi, out uint versionLow);
            return new Version((int)(versionHi >> 16),
                (int)(versionHi & 0xFFFF),
                (int)(versionLow >> 16),
                (int)(versionLow & 0xFFFF));
        }

        private static T GetProperty<T>(this IAssemblyName name, AssemblyNamePropertyId propertyId)
        {
            uint bufferSize = 512;
            IntPtr bufferPointer = Marshal.AllocHGlobal((int)bufferSize);
            try
            {
                int hResult = name.GetProperty(propertyId, bufferPointer, ref bufferSize);
                if (!HResult.IsSuccess(hResult))
                {
                    Marshal.ThrowExceptionForHR(hResult);
                }
                return bufferSize > 0 // IAssemblyName.GetProperty() will always return a bufferSize greater than 0
                    ?
                    bufferPointer.Read<T>(bufferSize) :
                    default(T);
            }
            finally
            {
                Marshal.FreeHGlobal(bufferPointer);
            }
        }

        private static void SetProperty(this IAssemblyName name, AssemblyNamePropertyId propertyId, object value)
        {
            int allocatedBytes = 0;
            IntPtr ptr = value?.ToPointer(out allocatedBytes) ?? IntPtr.Zero;
            try
            {
                // First clear the property
                int hResult = name.SetProperty(propertyId, IntPtr.Zero, 0);
                Marshal.ThrowExceptionForHR(hResult);

                // Now set the property
                hResult = name.SetProperty(propertyId, ptr, (uint)allocatedBytes);
                Marshal.ThrowExceptionForHR(hResult);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        #endregion
    }
}
