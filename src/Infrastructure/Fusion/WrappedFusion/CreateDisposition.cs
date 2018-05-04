// -----------------------------------------------------------------------
// <copyright file="CreateDisposition.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace PowerShell.Infrastructure.Fusion.WrappedFusion
{
    /// <summary>
    ///     Specifies the attributes of an <see cref="IAssemblyName" /> object
    ///     when it is constructed by the CreateAssemblyNameObject function.
    /// </summary>
    internal enum CreateDisposition
    {
        /// <summary>
        ///     Indicates that the parameter passed is a textual identity.
        /// </summary>
        /// <remarks>
        ///     If this flag is specified, the szAssemblyName parameter is a full assembly name and is parsed to
        ///     the individual properties. If the flag is not specified, szAssemblyName is the "Name" portion of the assembly name.
        /// </remarks>
        ParseDisplayName = 0x1,
        /// <summary>
        ///     Sets a few default values.
        /// </summary>
        /// <remarks>
        ///     If this flag is specified, certain properties, such as processor architecture, are set to
        ///     their default values.
        /// </remarks>
        SetDefaultValues = 0x2
    }
}
