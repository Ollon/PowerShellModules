// -----------------------------------------------------------------------
// <copyright file="GuidUtilities.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace PowerShell.Infrastructure.Utilities
{
    public static class GuidUtilities
    {
        private static string GetFormat(GuidOptions options)
        {
            switch (options)
            {
                case GuidOptions.Digits:
                    return "N";
                case GuidOptions.Hyphens:
                    return "D";
                case GuidOptions.Braces:
                    return "B";
                case GuidOptions.Parens:
                    return "P";
                case GuidOptions.Hex:
                    return "X";
                default:
                    throw new ArgumentOutOfRangeException(nameof(options), options, null);
            }
        }

        /// <summary>
        /// Generates an upper case guid string with options.
        /// </summary>
        /// <param name="options">The options for customizing the output</param>
        /// <returns>a string representing a Guid.</returns>
        public static string UpperCaseGenerate(GuidOptions options = GuidOptions.Braces)
        {
            return Guid.NewGuid().ToString(GetFormat(options)).ToUpper();
        }

        /// <summary>
        /// Generates a lower case guid string with options.
        /// </summary>
        /// <param name="options">The options for customizing the output</param>
        /// <returns>a string representing a Guid.</returns>
        public static string LowerCaseGenerate(GuidOptions options = GuidOptions.Braces)
        {
            return Guid.NewGuid().ToString(GetFormat(options)).ToLower();
        }
    }
}
