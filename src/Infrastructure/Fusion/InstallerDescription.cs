// -----------------------------------------------------------------------
// <copyright file="InstallerDescription.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using PowerShell.Infrastructure.Fusion.WrappedFusion;

namespace PowerShell.Infrastructure.Fusion
{
    /// <summary>
    ///     Descriptor class for applications manipulating the GAC using an instance of <see cref="GlobalAssemblyCache" />.
    /// </summary>
    public class InstallerDescription
    {
        private readonly InstallerType _installerType;
        private readonly string _uniqueId;
        private readonly string _applicationDescription;

        /// <summary>
        ///     Gets the type of the installer described by the current <see cref="InstallerDescription" />.
        /// </summary>
        public InstallerType Type
        {
            get
            {
                return _installerType;
            }
        }

        /// <summary>
        ///     Gets the unique identifier of the installer described by the current <see cref="InstallerDescription" />.
        /// </summary>
        public string Id
        {
            get
            {
                return _uniqueId;
            }
        }

        /// <summary>
        ///     Gets the description of the installer described by the current <see cref="InstallerDescription" />.
        /// </summary>
        public string Description
        {
            get
            {
                return _applicationDescription;
            }
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="InstallerDescription" /> from the values specified.
        /// </summary>
        /// <param name="installerType"></param>
        /// <param name="applicationDescription"></param>
        /// <param name="uniqueId"></param>
        private InstallerDescription(InstallerType installerType, string uniqueId, string applicationDescription)
        {
            _installerType = installerType;
            _uniqueId = uniqueId;
            _applicationDescription = applicationDescription;
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="InstallerDescription" /> using the data specified in the given
        ///     <see cref="FusionInstallReference" />.
        /// </summary>
        /// <param name="fusionInstallReference"></param>
        internal InstallerDescription(FusionInstallReference fusionInstallReference)
        {
            _installerType = InstallerTypeExt.FromGuid(fusionInstallReference.GuidScheme);
            _applicationDescription = fusionInstallReference.NonCannonicalData;
            _uniqueId = fusionInstallReference.Identifier;
        }

        public static InstallerDescription CreateForInstaller(string installerName, string installerIdentifier)
        {
            return new InstallerDescription(InstallerType.Installer, installerIdentifier, installerName);
        }

        public static InstallerDescription CreateForFile(string description, string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("An instance of InstallReference can only be created for an existing file.", fileName);
            }
            return new InstallerDescription(InstallerType.File, fileName, description);
        }

        public static InstallerDescription CreateForOpaqueString(string description, string opaqueString)
        {
            return new InstallerDescription(InstallerType.OpaqueString, opaqueString, description);
        }

        public override string ToString()
        {
            return "[" + _installerType + "] " + _uniqueId;
        }

        internal FusionInstallReference ToFusionStruct()
        {
            FusionInstallReference result = new FusionInstallReference(
                _installerType.AsGuid(),
                _uniqueId,
                _applicationDescription);
            return result;
        }
    }
}
