// -----------------------------------------------------------------------
// <copyright file="AbstractCmdlet.PathEvaluator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Management.Automation;

namespace PowerShell.Infrastructure.Commands
{
    public abstract partial class AbstractPSCmdlet : PSCmdlet
    {
        public class PathEvaluator
        {
            private readonly PSCmdlet _cmdlet;

            public PathEvaluator(PSCmdlet cmdlet)
            {
                _cmdlet = cmdlet;
            }

            public string CurrentDirectory
            {
                get
                {
                    return _cmdlet.SessionState.Path.CurrentLocation.Path;
                }
            }
        }
    }
}
