// -----------------------------------------------------------------------
// <copyright file="HexUtilities.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;

namespace PowerShell.Infrastructure.Utilities
{
    public static class HexUtilities
    {
        // converts number to hex digit. Does not do any range checks.
        private static char HexDigit(int num)
        {
            return (char)(num < 10 ? num + '0' : num + ('A' - 10));
        }

        public static string EncodeHexString(byte[] sArray)
        {
            string result = null;
            if (sArray != null)
            {
                char[] hexOrder = new char[sArray.Length * 2];
                for (int i = 0, j = 0; i < sArray.Length; i++)
                {
                    int digit = (sArray[i] & 0xf0) >> 4;
                    hexOrder[j++] = HexDigit(digit);
                    digit = sArray[i] & 0x0f;
                    hexOrder[j++] = HexDigit(digit);
                }
                result = new string(hexOrder);
            }
            return result;
        }

        internal static string EncodeHexStringFromInt(byte[] sArray)
        {
            string result = null;
            if (sArray != null)
            {
                char[] hexOrder = new char[sArray.Length * 2];
                int i = sArray.Length;
                int j = 0;
                while (i-- > 0)
                {
                    int digit = (sArray[i] & 0xf0) >> 4;
                    hexOrder[j++] = HexDigit(digit);
                    digit = sArray[i] & 0x0f;
                    hexOrder[j++] = HexDigit(digit);
                }
                result = new string(hexOrder);
            }
            return result;
        }

        public static int ConvertHexDigit(char val)
        {
            if (val <= '9' && val >= '0')
            {
                return val - '0';
            }
            if (val >= 'a' && val <= 'f')
            {
                return val - 'a' + 10;
            }
            if (val >= 'A' && val <= 'F')
            {
                return val - 'A' + 10;
            }
            throw new ArgumentException();
        }

        public static byte[] DecodeHexString(string hexString)
        {
            if (hexString == null)
            {
                throw new ArgumentNullException("hexString");
            }
            Contract.EndContractBlock();
            bool spaceSkippingMode = false;
            int i = 0;
            int length = hexString.Length;
            if (length >= 2
                && hexString[0] == '0'
                && (hexString[1] == 'x' || hexString[1] == 'X'))
            {
                length = hexString.Length - 2;
                i = 2;
            }

            // Hex strings must always have 2N or (3N - 1) entries.
            if (length % 2 != 0 && length % 3 != 2)
            {
                throw new ArgumentException();
            }
            byte[] sArray;
            if (length >= 3 && hexString[i + 2] == ' ')
            {
                spaceSkippingMode = true;

                // Each hex digit will take three spaces, except the first (hence the plus 1).
                sArray = new byte[length / 3 + 1];
            }
            else
            {
                // Each hex digit will take two spaces
                sArray = new byte[length / 2];
            }
            for (int j = 0; i < hexString.Length; i += 2, j++)
            {
                int rawdigit = ConvertHexDigit(hexString[i]);
                int digit = ConvertHexDigit(hexString[i + 1]);
                sArray[j] = (byte)(digit | (rawdigit << 4));
                if (spaceSkippingMode)
                {
                    i++;
                }
            }
            return sArray;
        }
    }
}
