// -----------------------------------------------------------------------
// <copyright file="PathUtilities.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;

namespace PowerShell.Infrastructure.Utilities
{
    public static class PathUtilities
    {
        public static void CreateDirectoryIfNecessary(params string[] paths)
        {
            foreach (string path in paths)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }
    }
}
