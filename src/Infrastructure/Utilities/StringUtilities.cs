// -----------------------------------------------------------------------
// <copyright file="StringUtilities.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace PowerShell.Infrastructure.Utilities
{
    internal static class StringUtilities
    {
        public static string ConvertToUncPath(string str, string computerName)
        {
            string newString = str.Replace(":", "$");
            return string.Format("\\\\{0}\\{1}", computerName, newString);
        }

        public static string MakePascal(string inString, bool firstCharacterOnly = false)
        {
            int count = inString.Length;
            string firstChar = inString[0].ToString().ToUpper();
            StringBuilder builder = new StringBuilder();
            builder.Append(firstChar);
            string restOfString = inString.Substring(1, count - 1);
            builder.Append(!firstCharacterOnly ? restOfString.ToLower() : restOfString);
            return builder.ToString();
        }

        public static string MakeCamel(string inString, bool firstCharacterOnly = false)
        {
            int count = inString.Length;
            string firstChar = inString[0].ToString().ToLower();
            StringBuilder builder = new StringBuilder();
            builder.Append(firstChar);
            string restOfString = inString.Substring(1, count - 1);
            builder.Append(!firstCharacterOnly ? restOfString.ToLower() : restOfString);
            return builder.ToString();
        }

        public static string WriteLine(string pre, string line, string post, EscapeStyle pattern = EscapeStyle.PowerShell)
        {
            switch (pattern)
            {
                //case EscapePattern.PowerShell:
                //    break;
                //case EscapePattern.Xml:
                //    line = SecurityElement.Escape(line);
                //    break;
                //default:
                //    line = line.Replace("\"", "\"\"");
                //    break;
                case EscapeStyle.PowerShell:
                    line = line.Replace("`", "``");
                    line = line.Replace("\"", "`\"");
                    line = line.Replace("$", "`$");
                    break;
                case EscapeStyle.Xml:
                    line = SecurityElement.Escape(line);
                    break;
                case EscapeStyle.CSharpString:
                    line = line.Replace("\\", "\\\\");
                    line = line.Replace("\"", "\\\"");
                    break;
                case EscapeStyle.CSharpVerbatimString:
                    line = line.Replace("\"", "\"\"");
                    break;
                case EscapeStyle.CSharpChar:
                    line = line.Replace("\\", "\\\\");
                    line = line.Replace("'", "\\'");
                    break;
                case EscapeStyle.CSharpInterpolation:
                    line = line.Replace("{", "{{");
                    line = line.Replace("}", "}}");
                    line = line.Replace("\\", "\\\\");
                    line = line.Replace("\"", "\\\"");
                    break;
                case EscapeStyle.CSharpVerbatimInterpolation:
                    line = line.Replace("\"", "\"\"");
                    line = line.Replace("{", "{{");
                    line = line.Replace("}", "}}");
                    break;
                case EscapeStyle.CSharpFormat:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pattern), pattern, null);
            }

            return string.Format($"{pre}{line}{post}");
        }

        public static string RemoveQuotes(this string InputTxt)
        {
            if (!InputTxt.StartsWith("\"", StringComparison.Ordinal) ||
                !InputTxt.EndsWith("\"", StringComparison.Ordinal))
            {
                return InputTxt;
            }

            int len = InputTxt.Length;
            int stop = len - 2;
            return InputTxt.Substring(1, stop);
        }

        /// <summary>
        ///     Un-quotes a quoted string
        /// </summary>
        /// <param name="InputTxt">Text string need to be escape with slashes</param>
        public static string UnEscape(this string InputTxt)
        {
            // List of characters handled:
            // \000 null
            // \010 backspace
            // \011 horizontal tab
            // \012 new line
            // \015 carriage return
            // \032 substitute
            // \042 double quote
            // \047 single quote
            // \134 backslash
            // \140 grave accent
            string Result = InputTxt;
            try
            {
                Result = Regex.Replace(InputTxt, @"(\\)([\000\010\011\012\015\032\042\047\134\140])", "$2");
            }
            catch (Exception Ex)
            {
                // handle any exception here
                Console.WriteLine(Ex.Message);
            }

            return Result;
        }

        /// <summary>
        ///     Returns a string with backslashes before characters that need to be quoted
        /// </summary>
        /// <param name="line"></param>
        /// <param name="style">Text string need to be escape with slashes</param>
        public static string Escape(this string line, EscapeStyle style)
        {
            switch (style)
            {
                //case EscapePattern.PowerShell:
                //    break;
                //case EscapePattern.Xml:
                //    line = SecurityElement.Escape(line);
                //    break;
                //default:
                //    line = line.Replace("\"", "\"\"");
                //    break;
                case EscapeStyle.PowerShell:
                    line = line.Replace("`", "``");
                    line = line.Replace("\"", "`\"");
                    line = line.Replace("$", "`$");
                    break;
                case EscapeStyle.Xml:
                    line = SecurityElement.Escape(line);
                    break;
                case EscapeStyle.CSharpString:
                    line = line.Replace("\\", "\\\\");
                    line = line.Replace("\"", "\\\"");
                    break;
                case EscapeStyle.CSharpVerbatimString:
                    line = line.Replace("\"", "\"\"");
                    break;
                case EscapeStyle.CSharpChar:
                    line = line.Replace("\\", "\\\\");
                    line = line.Replace("'", "\\'");
                    break;
                case EscapeStyle.CSharpInterpolation:
                    line = line.Replace("{", "{{");
                    line = line.Replace("}", "}}");
                    line = line.Replace("\\", "\\\\");
                    line = line.Replace("\"", "\\\"");
                    break;
                case EscapeStyle.CSharpVerbatimInterpolation:
                    line = line.Replace("\"", "\"\"");
                    line = line.Replace("{", "{{");
                    line = line.Replace("}", "}}");
                    break;
                case EscapeStyle.CSharpFormat:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }

            return line;
        }
    }

}
