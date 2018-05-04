// -----------------------------------------------------------------------
// <copyright file="InstallerTypeExt.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace PowerShell.Infrastructure.Fusion
{
    /// <summary>
    ///     Holds extension methods for <see cref="InstallerType" />.
    /// </summary>
    internal static class InstallerTypeExt
    {
        #region Constants

        private const string InstallerGuid = "8cedc215-ac4b-488b-93c0-a50a49cb2fb8";
        private const string FileGuid = "b02f9d65-fb77-4f7a-afa5-b391309f11c9";
        private const string OpaqueStringGuid = "2ec93463-b0c3-45e1-8364-327e96aea856";

        // These GUID cannot be used for installing into GAC.
        private const string MsiGuid = "25df0fc1-7f97-4070-add7-4b13bbfd7cb8";
        private const string OsInstallGuid = "d16d444c-56d8-11d5-882d-0080c847b195";

        #endregion

        #region Public Methods

        /// <summary>
        ///     Converts the defined <see cref="Guid" /> to its equivalent <see cref="InstallerType" /> representation.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static InstallerType FromGuid(Guid guid)
        {
            switch (guid.ToString())
            {
                case FileGuid:
                    return InstallerType.File;
                case InstallerGuid:
                    return InstallerType.Installer;
                case OpaqueStringGuid:
                    return InstallerType.OpaqueString;
                case MsiGuid:
#pragma warning disable 618
                    return InstallerType.WindowsInstaller;
#pragma warning restore 618
                case OsInstallGuid:
#pragma warning disable 618
                    return InstallerType.OperatingSystem;
#pragma warning restore 618
                default:
                    throw new ArgumentException("ProjectGuid \"" + guid + "\" does not define an InstallerType.");
            }
        }

        #endregion

        #region Extension Methods

        /// <summary>
        ///     Converts the value of this instance to its equivalent <see cref="Guid" /> representation.
        /// </summary>
        /// <param name="installerType"></param>
        /// <returns></returns>
        public static Guid AsGuid(this InstallerType installerType)
        {
            switch (installerType)
            {
                case InstallerType.Installer:
                    return new Guid(InstallerGuid);
                case InstallerType.File:
                    return new Guid(FileGuid);
                case InstallerType.OpaqueString:
                    return new Guid(OpaqueStringGuid);
#pragma warning disable 618
                case InstallerType.WindowsInstaller:
#pragma warning restore 618
                    return new Guid(MsiGuid);
#pragma warning disable 618
                case InstallerType.OperatingSystem:
#pragma warning restore 618
                    return new Guid(OsInstallGuid);
                default:
                    throw new NotSupportedException("InstallerType " +
                                                    installerType +
                                                    " is not implemented by the ToGuid() method.");
            }
        }

        #endregion
    }
}
