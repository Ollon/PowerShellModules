// -----------------------------------------------------------------------
// <copyright file="AbstractCmdlet.ParameterEvaluator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using PowerShell.Infrastructure.Utilities;

namespace PowerShell.Infrastructure.Commands
{
    public abstract partial class AbstractPSCmdlet : PSCmdlet
    {
        public class ParameterEvaluator : IParameterEvaluator
        {
            private readonly PSCmdlet cmdlet;

            public bool Contains(string parameterName) => PSBoundParameters.ContainsKey(parameterName);

            public object this[string parameterName]
            {
                get
                {
                    if (Contains(parameterName))
                    {
                        return PSBoundParameters[parameterName];
                    }
                    return null;
                }
            }

            internal ParameterEvaluator(PSCmdlet cmdlet)
            {
                this.cmdlet = cmdlet;
            }

            public ActionPreference VerbosePreference
            {
                get
                {
                    return (ActionPreference) cmdlet.GetVariableValue("VerbosePreference");
                }
            }

            public ActionPreference WarningPreference
            {
                get
                {
                    return (ActionPreference) cmdlet.GetVariableValue("WarningPreference");
                }
            }

            public bool WhatIfPreference
            {
                get
                {
                    return (bool) cmdlet.GetVariableValue("WhatIfPreference");
                }
            }

            public bool ShouldPageResults
            {
                get
                {
                    return cmdlet.MyInvocation.BoundParameters.ContainsKey("Skip") ||
                           cmdlet.MyInvocation.BoundParameters.ContainsKey("IncludeTotalCount") ||
                           cmdlet.MyInvocation.BoundParameters.ContainsKey("First");
                }
            }

            public bool UseTransaction
            {
                get
                {
                    return cmdlet.MyInvocation.BoundParameters.ContainsKey("UseTransaction");
                }
            }

            public bool WhatIf
            {
                get
                {
                    return cmdlet.MyInvocation.BoundParameters.ContainsKey("WhatIf");
                }
            }

            public bool Confirm
            {
                get
                {
                    return cmdlet.MyInvocation.BoundParameters.ContainsKey("Confirm");
                }
            }

            public bool Verbose
            {
                get
                {
                    return cmdlet.MyInvocation.BoundParameters.ContainsKey("Verbose");
                }
            }

            public bool Debug
            {
                get
                {
                    if (cmdlet.MyInvocation.BoundParameters.ContainsKey("Debug"))
                    {
                        return (bool) cmdlet.MyInvocation.BoundParameters["Debug"];
                    }
                    return false;
                }
            }

            public ActionPreference ErrorAction
            {
                get
                {
                    if (cmdlet.MyInvocation.BoundParameters.ContainsKey("ErrorAction"))
                    {
                        return (ActionPreference) cmdlet.MyInvocation.BoundParameters["ErrorAction"];
                    }
                    return default(ActionPreference);
                }
            }

            public ActionPreference WarningAction
            {
                get
                {
                    if (cmdlet.MyInvocation.BoundParameters.ContainsKey("WarningAction"))
                    {
                        return (ActionPreference) cmdlet.MyInvocation.BoundParameters["WarningAction"];
                    }
                    return default(ActionPreference);
                }
            }

            public ActionPreference InformationAction
            {
                get
                {
                    if (cmdlet.MyInvocation.BoundParameters.ContainsKey("InformationAction"))
                    {
                        return (ActionPreference) cmdlet.MyInvocation.BoundParameters["InformationAction"];
                    }
                    return default(ActionPreference);
                }
            }

            public string ErrorVariable
            {
                get
                {
                    if (cmdlet.MyInvocation.BoundParameters.ContainsKey("ErrorVariable"))
                    {
                        return (string) cmdlet.MyInvocation.BoundParameters["ErrorVariable"];
                    }
                    return string.Empty;
                }
            }

            public string WarningVariable
            {
                get
                {
                    if (cmdlet.MyInvocation.BoundParameters.ContainsKey("WarningVariable"))
                    {
                        return (string) cmdlet.MyInvocation.BoundParameters["WarningVariable"];
                    }
                    return string.Empty;
                }
            }

            public string InformationVariable
            {
                get
                {
                    if (cmdlet.MyInvocation.BoundParameters.ContainsKey("InformationVariable"))
                    {
                        return (string) cmdlet.MyInvocation.BoundParameters["InformationVariable"];
                    }
                    return string.Empty;
                }
            }

            public string OutVariable
            {
                get
                {
                    if (cmdlet.MyInvocation.BoundParameters.ContainsKey("OutVariable"))
                    {
                        return (string) cmdlet.MyInvocation.BoundParameters["OutVariable"];
                    }
                    return string.Empty;
                }
            }

            public int OutBuffer
            {
                get
                {
                    if (cmdlet.MyInvocation.BoundParameters.ContainsKey("OutBuffer"))
                    {
                        return (int) cmdlet.MyInvocation.BoundParameters["OutBuffer"];
                    }
                    return default(int);
                }
            }

            public string PipelineVariable
            {
                get
                {
                    if (cmdlet.MyInvocation.BoundParameters.ContainsKey("PipelineVariable"))
                    {
                        return (string) cmdlet.MyInvocation.BoundParameters["PipelineVariable"];
                    }
                    return string.Empty;
                }
            }

            public object Args
            {
                get
                {
                    return cmdlet.GetVariableValue("args");
                }
            }

            public string ConsoleFileName
            {
                get
                {
                    return (string) cmdlet.GetVariableValue("ConsoleFileName");
                }
            }

            public ArrayList Error
            {
                get
                {
                    return (ArrayList) cmdlet.GetVariableValue("Error");
                }
            }

            public EngineIntrinsics ExecutionContext
            {
                get
                {
                    return (EngineIntrinsics) cmdlet.GetVariableValue("ExecutionContext");
                }
            }

            public string Home
            {
                get
                {
                    return (string) cmdlet.GetVariableValue("HOME");
                }
            }

            public object Host
            {
                get
                {
                    return cmdlet.GetVariableValue("Host");
                }
            }

            public object Input
            {
                get
                {
                    return cmdlet.GetVariableValue("input");
                }
            }

            public int MaximumAliasCount
            {
                get
                {
                    return (int) cmdlet.GetVariableValue("MaximumAliasCount");
                }
            }

            public int MaximumDriveCount
            {
                get
                {
                    return (int) cmdlet.GetVariableValue("MaximumDriveCount");
                }
            }

            public int MaximumErrorCount
            {
                get
                {
                    return (int) cmdlet.GetVariableValue("MaximumErrorCount");
                }
            }

            public int MaximumFunctionCount
            {
                get
                {
                    return (int) cmdlet.GetVariableValue("MaximumFunctionCount");
                }
            }

            public int MaximumVariableCount
            {
                get
                {
                    return (int) cmdlet.GetVariableValue("MaximumVariableCount");
                }
            }

            public InvocationInfo MyInvocation
            {
                get
                {
                    return (InvocationInfo) cmdlet.GetVariableValue("MyInvocation");
                }
            }

            public int PID
            {
                get
                {
                    return (int) cmdlet.GetVariableValue("PID");
                }
            }

            public Dictionary<string, object> PSBoundParameters
            {
                get
                {
                    return (Dictionary<string, object>) cmdlet.GetVariableValue("PSBoundParameters");
                }
            }

            public string PSCommandPath
            {
                get
                {
                    return (string) cmdlet.GetVariableValue("PSCommandPath");
                }
            }

            public string PSCulture
            {
                get
                {
                    return (string) cmdlet.GetVariableValue("PSCulture");
                }
            }

            public string PSEdition
            {
                get
                {
                    return (string) cmdlet.GetVariableValue("PSEdition");
                }
            }

            public string PSHome
            {
                get
                {
                    return (string) cmdlet.GetVariableValue("PSHOME");
                }
            }

            public object PsISE
            {
                get
                {
                    return cmdlet.GetVariableValue("psISE");
                }
            }

            public string PSScriptRoot
            {
                get
                {
                    return (string) cmdlet.GetVariableValue("PSScriptRoot");
                }
            }

            public string PSUICulture
            {
                get
                {
                    return (string) cmdlet.GetVariableValue("PSUICulture");
                }
            }

            public PSObject[] PsUnsupportedConsoleApplications
            {
                get
                {
                    return (PSObject[]) cmdlet.GetVariableValue("psUnsupportedConsoleApplications");
                }
            }

            public Hashtable PSVersionTable
            {
                get
                {
                    return (Hashtable) cmdlet.GetVariableValue("PSVersionTable");
                }
            }

            public string ShellId
            {
                get
                {
                    return (string) cmdlet.GetVariableValue("ShellId");
                }
            }
        }
    }
}
