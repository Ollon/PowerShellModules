// -----------------------------------------------------------------------
// <copyright file="WebUtilities.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Net;

namespace PowerShell.Infrastructure.Utilities
{
    public static class WebUtilities
    {
        public static string GetWebFileString(string uri)
        {
            using (WebClient wc = new WebClient())
            {
                return wc.DownloadString(uri);
            }
        }
    }
}
