// -----------------------------------------------------------------------
// <copyright file="IParameterEvaluator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace PowerShell.Infrastructure.Utilities
{
    public interface IParameterEvaluator
    {
        object this[string parameterName] { get; }

        object Args { get; }

        bool Confirm { get; }

        string ConsoleFileName { get; }

        bool Debug { get; }

        ArrayList Error { get; }

        ActionPreference ErrorAction { get; }

        string ErrorVariable { get; }

        EngineIntrinsics ExecutionContext { get; }

        string Home { get; }

        object Host { get; }

        ActionPreference InformationAction { get; }

        string InformationVariable { get; }

        object Input { get; }

        int MaximumAliasCount { get; }

        int MaximumDriveCount { get; }

        int MaximumErrorCount { get; }

        int MaximumFunctionCount { get; }

        int MaximumVariableCount { get; }

        InvocationInfo MyInvocation { get; }

        int OutBuffer { get; }

        string OutVariable { get; }

        int PID { get; }

        string PipelineVariable { get; }

        Dictionary<string, object> PSBoundParameters { get; }

        string PSCommandPath { get; }

        string PSCulture { get; }

        string PSEdition { get; }

        string PSHome { get; }

        object PsISE { get; }

        string PSScriptRoot { get; }

        string PSUICulture { get; }

        PSObject[] PsUnsupportedConsoleApplications { get; }

        Hashtable PSVersionTable { get; }

        string ShellId { get; }

        bool ShouldPageResults { get; }

        bool UseTransaction { get; }

        bool Verbose { get; }

        ActionPreference VerbosePreference { get; }

        ActionPreference WarningAction { get; }

        ActionPreference WarningPreference { get; }

        string WarningVariable { get; }

        bool WhatIf { get; }

        bool WhatIfPreference { get; }

        bool Contains(string parameterName);
    }
}
