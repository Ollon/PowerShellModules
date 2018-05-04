// -----------------------------------------------------------------------
// <copyright file="Restarter.cs" company="Ollon, LLC">
//     Copyright (c) 2018 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using PowerShell.EditorFeatures.Core.Host;

namespace PowerShell.EditorFeatures.Core.Features.Restart
{
    public static class Restarter
    {
        public static void Restart(IEditorFeaturesHostObject hostObject)
        {

           

            string powerShellISEPath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.Windows),
                "System32\\WindowsPowerShell\\v1.0\\powershell_ise.exe");
            if (File.Exists(powerShellISEPath))
            {
                ProcessStartInfo Info = new ProcessStartInfo
                {
                    Arguments = "/C ping 127.0.0.1 -n 2 && \"" + powerShellISEPath + "\"",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    FileName = "cmd.exe"
                };
                Process.Start(Info);
                Process.GetCurrentProcess().Kill();
            }
        }
    }
}
