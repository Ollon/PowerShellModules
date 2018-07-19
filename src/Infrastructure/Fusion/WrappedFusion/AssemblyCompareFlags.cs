// -----------------------------------------------------------------------
// <copyright file="AssemblyCompareFlags.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace PowerShell.Infrastructure.Fusion.WrappedFusion
{
    /// <summary>
    ///     Indicates the attributes to be compared.
    /// </summary>
    /// <remarks>
    ///     Native name: AssemblyCompareFlags
    /// </remarks>
    [Flags]
    internal enum AssemblyCompareFlags
    {
        NAME = 0x1,
        MAJOR_VERSION = 0x2,
        MINOR_VERSION = 0x4,
        BUILD_NUMBER = 0x8,
        REVISION_NUMBER = 0x10,
        PUBLIC_KEY_TOKEN = 0x20,
        CULTURE = 0x40,
        CUSTOM = 0x80,
        ALL = NAME
              | MAJOR_VERSION
              | MINOR_VERSION
              | REVISION_NUMBER
              | BUILD_NUMBER
              | PUBLIC_KEY_TOKEN
              | CULTURE
              | CUSTOM,
        DEFAULT = 0x100
    }
}
