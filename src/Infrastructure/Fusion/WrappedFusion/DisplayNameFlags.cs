// -----------------------------------------------------------------------
// <copyright file="DisplayNameFlags.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace PowerShell.Infrastructure.Fusion.WrappedFusion
{
    /// <summary>
    ///     Indicates the version, build, culture, signature, and so on, of the assembly whose display name will be retrieved.
    /// </summary>
    [Flags]
    public enum DisplayNameFlags
    {
        All = 0x0,
        /// <summary>
        ///     Includes the version number as part of the display name.
        /// </summary>
        Version = 0x1,
        /// <summary>
        ///     Includes the culture.
        /// </summary>
        Culture = 0x2,
        /// <summary>
        ///     Includes the internal key token.
        /// </summary>
        PublicKeyToken = 0x4,
        /// <summary>
        ///     Includes the internal key.
        /// </summary>
        PublicKey = 0x8,
        /// <summary>
        ///     Includes the custom part of the assembly name.
        /// </summary>
        Custom = 0x10,
        /// <summary>
        ///     Includes the processor architecture.
        /// </summary>
        ProcessArchitecture = 0x20,
        /// <summary>
        ///     Includes the language ID.
        /// </summary>
        LanguageId = 0x40
    }
}
