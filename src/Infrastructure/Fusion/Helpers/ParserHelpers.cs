// -----------------------------------------------------------------------
// <copyright file="ParserHelpers.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace PowerShell.Infrastructure.Fusion.Helpers
{
    /// <summary>
    ///     Helper class for generic parser functions.
    /// </summary>
    public static class ParserHelper
    {
        #region Public Methods

        /// <summary>
        ///     Tries to parse a string to a value of the specified <typeparamref name="EnumType" />.
        ///     The operation is case insensitive.
        /// </summary>
        /// <typeparam name="EnumType">The type of enumeration to parse to.</typeparam>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseEnum<EnumType>(string value, out EnumType result)
        {
            result = default(EnumType);
            Type type = typeof(EnumType);
            if (!type.IsEnum)
            {
                return false;
            }
            value = value.ToUpperInvariant();
            string[] names = Enum.GetNames(type);
            foreach (string name in names)
            {
                if (name.ToUpperInvariant() != value)
                {
                    continue;
                }
                result = (EnumType) Enum.Parse(type, value, true);
                return true;
            }
            return false;
        }

        #endregion
    }
}
