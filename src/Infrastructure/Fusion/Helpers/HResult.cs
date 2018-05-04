// -----------------------------------------------------------------------
// <copyright file="HResult.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace PowerShell.Infrastructure.Fusion.Helpers
{
    public static class HResult
    {
        public static bool IsSuccess(int hResult)
        {
            return hResult == 0;
        }
    }
}
