﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerShell.Infrastructure.Utilities
{
    internal static class UnicodeChars
    {
        public static string[] Whitespace = { "\u0009", "\u000A", "\u000B", "\u000C", "\u000D", "\u0020" };

        public static string[] Punctuation = { "\u0021", "\u0022", "\u0023", "\u0025", "\u0026", "\u0027", "\u0028", "\u0029", "\u002A", "\u002C", "\u002D", "\u002E", "\u002F", "\u003A", "\u003B", "\u003F", "\u0040", "\u005B", "\u005C", "\u005D", "\u005F", "\u007B", "\u007D" };

        public static string[] Digits = { "\u0030", "\u0031", "\u0032", "\u0033", "\u0034", "\u0035", "\u0036", "\u0037", "\u0038", "\u0039" };

        public static string[] LowerCaseLetters = { "\u0061", "\u0062", "\u0063", "\u0064", "\u0065", "\u0066", "\u0067", "\u0068", "\u0069", "\u006A", "\u006B", "\u006C", "\u006D", "\u006E", "\u006F", "\u0070", "\u0071", "\u0072", "\u0073", "\u0074", "\u0075", "\u0076", "\u0077", "\u0078", "\u0079", "\u007A" };

        public static string[] UpperCaseLetters = { "\u0041", "\u0042", "\u0043", "\u0044", "\u0045", "\u0046", "\u0047", "\u0048", "\u0049", "\u004A", "\u004B", "\u004C", "\u004D", "\u004E", "\u004F", "\u0050", "\u0051", "\u0052", "\u0053", "\u0054", "\u0055", "\u0056", "\u0057", "\u0058", "\u0059", "\u005A" };

        public static string[] Letters = { "\u0041", "\u0042", "\u0043", "\u0044", "\u0045", "\u0046", "\u0047", "\u0048", "\u0049", "\u004A", "\u004B", "\u004C", "\u004D", "\u004E", "\u004F", "\u0050", "\u0051", "\u0052", "\u0053", "\u0054", "\u0055", "\u0056", "\u0057", "\u0058", "\u0059", "\u005A", "\u0061", "\u0062", "\u0063", "\u0064", "\u0065", "\u0066", "\u0067", "\u0068", "\u0069", "\u006A", "\u006B", "\u006C", "\u006D", "\u006E", "\u006F", "\u0070", "\u0071", "\u0072", "\u0073", "\u0074", "\u0075", "\u0076", "\u0077", "\u0078", "\u0079", "\u007A" };
    }

    internal static class UnicodeEscapes
    {
        public static string[] Whitespace = { "\\u0009", "\\u000A", "\\u000B", "\\u000C", "\\u000D", "\\u0020" };

        public static string[] Punctuation = { "\\u0021", "\\u0022", "\\u0023", "\\u0025", "\\u0026", "\\u0027", "\\u0028", "\\u0029", "\\u002A", "\\u002C", "\\u002D", "\\u002E", "\\u002F", "\\u003A", "\\u003B", "\\u003F", "\\u0040", "\\u005B", "\\u005C", "\\u005D", "\\u005F", "\\u007B", "\\u007D" };

        public static string[] Digits = { "\\u0030", "\\u0031", "\\u0032", "\\u0033", "\\u0034", "\\u0035", "\\u0036", "\\u0037", "\\u0038", "\\u0039" };

        public static string[] LowerCaseLetters = { "\\u0061", "\\u0062", "\\u0063", "\\u0064", "\\u0065", "\\u0066", "\\u0067", "\\u0068", "\\u0069", "\\u006A", "\\u006B", "\\u006C", "\\u006D", "\\u006E", "\\u006F", "\\u0070", "\\u0071", "\\u0072", "\\u0073", "\\u0074", "\\u0075", "\\u0076", "\\u0077", "\\u0078", "\\u0079", "\\u007A" };

        public static string[] UpperCaseLetters = { "\\u0041", "\\u0042", "\\u0043", "\\u0044", "\\u0045", "\\u0046", "\\u0047", "\\u0048", "\\u0049", "\\u004A", "\\u004B", "\\u004C", "\\u004D", "\\u004E", "\\u004F", "\\u0050", "\\u0051", "\\u0052", "\\u0053", "\\u0054", "\\u0055", "\\u0056", "\\u0057", "\\u0058", "\\u0059", "\\u005A" };

        public static string[] Letters = { "\\u0041", "\\u0042", "\\u0043", "\\u0044", "\\u0045", "\\u0046", "\\u0047", "\\u0048", "\\u0049", "\\u004A", "\\u004B", "\\u004C", "\\u004D", "\\u004E", "\\u004F", "\\u0050", "\\u0051", "\\u0052", "\\u0053", "\\u0054", "\\u0055", "\\u0056", "\\u0057", "\\u0058", "\\u0059", "\\u005A", "\\u0061", "\\u0062", "\\u0063", "\\u0064", "\\u0065", "\\u0066", "\\u0067", "\\u0068", "\\u0069", "\\u006A", "\\u006B", "\\u006C", "\\u006D", "\\u006E", "\\u006F", "\\u0070", "\\u0071", "\\u0072", "\\u0073", "\\u0074", "\\u0075", "\\u0076", "\\u0077", "\\u0078", "\\u0079", "\\u007A" };
    }

    internal static class AsciiChars
    {
        public static string[] Whitespace = { "\x0009", "\x000A", "\x000B", "\x000C", "\x000D", "\x0020" };

        public static string[] Punctuation = { "\x0021", "\x0022", "\x0023", "\x0025", "\x0026", "\x0027", "\x0028", "\x0029", "\x002A", "\x002C", "\x002D", "\x002E", "\x002F", "\x003A", "\x003B", "\x003F", "\x0040", "\x005B", "\x005C", "\x005D", "\x005F", "\x007B", "\x007D" };

        public static string[] Digits = { "\x0030", "\x0031", "\x0032", "\x0033", "\x0034", "\x0035", "\x0036", "\x0037", "\x0038", "\x0039" };

        public static string[] LowerCaseLetters = { "\x0061", "\x0062", "\x0063", "\x0064", "\x0065", "\x0066", "\x0067", "\x0068", "\x0069", "\x006A", "\x006B", "\x006C", "\x006D", "\x006E", "\x006F", "\x0070", "\x0071", "\x0072", "\x0073", "\x0074", "\x0075", "\x0076", "\x0077", "\x0078", "\x0079", "\x007A" };

        public static string[] UpperCaseLetters = { "\x0041", "\x0042", "\x0043", "\x0044", "\x0045", "\x0046", "\x0047", "\x0048", "\x0049", "\x004A", "\x004B", "\x004C", "\x004D", "\x004E", "\x004F", "\x0050", "\x0051", "\x0052", "\x0053", "\x0054", "\x0055", "\x0056", "\x0057", "\x0058", "\x0059", "\x005A" };

        public static string[] Letters = { "\x0041", "\x0042", "\x0043", "\x0044", "\x0045", "\x0046", "\x0047", "\x0048", "\x0049", "\x004A", "\x004B", "\x004C", "\x004D", "\x004E", "\x004F", "\x0050", "\x0051", "\x0052", "\x0053", "\x0054", "\x0055", "\x0056", "\x0057", "\x0058", "\x0059", "\x005A", "\x0061", "\x0062", "\x0063", "\x0064", "\x0065", "\x0066", "\x0067", "\x0068", "\x0069", "\x006A", "\x006B", "\x006C", "\x006D", "\x006E", "\x006F", "\x0070", "\x0071", "\x0072", "\x0073", "\x0074", "\x0075", "\x0076", "\x0077", "\x0078", "\x0079", "\x007A", };
    }

    internal static class AsciiEscapes
    {
        public static string[] Whitespace = { "\\x0009", "\\x000A", "\\x000B", "\\x000C", "\\x000D", "\\x0020" };

        public static string[] Punctuation = { "\\x0021", "\\x0022", "\\x0023", "\\x0025", "\\x0026", "\\x0027", "\\x0028", "\\x0029", "\\x002A", "\\x002C", "\\x002D", "\\x002E", "\\x002F", "\\x003A", "\\x003B", "\\x003F", "\\x0040", "\\x005B", "\\x005C", "\\x005D", "\\x005F", "\\x007B", "\\x007D" };

        public static string[] Digits = { "\\x0030", "\\x0031", "\\x0032", "\\x0033", "\\x0034", "\\x0035", "\\x0036", "\\x0037", "\\x0038", "\\x0039" };

        public static string[] LowerCaseLetters = { "\\x0061", "\\x0062", "\\x0063", "\\x0064", "\\x0065", "\\x0066", "\\x0067", "\\x0068", "\\x0069", "\\x006A", "\\x006B", "\\x006C", "\\x006D", "\\x006E", "\\x006F", "\\x0070", "\\x0071", "\\x0072", "\\x0073", "\\x0074", "\\x0075", "\\x0076", "\\x0077", "\\x0078", "\\x0079", "\\x007A" };

        public static string[] UpperCaseLetters = { "\\x0041", "\\x0042", "\\x0043", "\\x0044", "\\x0045", "\\x0046", "\\x0047", "\\x0048", "\\x0049", "\\x004A", "\\x004B", "\\x004C", "\\x004D", "\\x004E", "\\x004F", "\\x0050", "\\x0051", "\\x0052", "\\x0053", "\\x0054", "\\x0055", "\\x0056", "\\x0057", "\\x0058", "\\x0059", "\\x005A" };

        public static string[] Letters = { "\\x0041", "\\x0042", "\\x0043", "\\x0044", "\\x0045", "\\x0046", "\\x0047", "\\x0048", "\\x0049", "\\x004A", "\\x004B", "\\x004C", "\\x004D", "\\x004E", "\\x004F", "\\x0050", "\\x0051", "\\x0052", "\\x0053", "\\x0054", "\\x0055", "\\x0056", "\\x0057", "\\x0058", "\\x0059", "\\x005A", "\\x0061", "\\x0062", "\\x0063", "\\x0064", "\\x0065", "\\x0066", "\\x0067", "\\x0068", "\\x0069", "\\x006A", "\\x006B", "\\x006C", "\\x006D", "\\x006E", "\\x006F", "\\x0070", "\\x0071", "\\x0072", "\\x0073", "\\x0074", "\\x0075", "\\x0076", "\\x0077", "\\x0078", "\\x0079", "\\x007A" };
    }
}
