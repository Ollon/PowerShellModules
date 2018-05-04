// -----------------------------------------------------------------------
// <copyright file="AbstractPSCmdlet.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security;
using Microsoft.Management.Infrastructure.Options;

namespace PowerShell.Infrastructure.Commands
{
    /// <summary>
    ///     Represents the abstract base for PowerShell Cmdlets.
    /// </summary>
    public abstract partial class AbstractPSCmdlet : PSCmdlet
    {
        protected RuntimeDefinedParameterDictionary _dynamicParameters;

        protected AbstractPSCmdlet()
        {
            ParameterEval = new ParameterEvaluator(this);
            PathEval = new PathEvaluator(this);
        }

        protected ParameterEvaluator ParameterEval { get; }

        protected PathEvaluator PathEval { get; }

        protected void PageResults<T>(params T[] items)
        {
            PageResults(items.AsEnumerable());
        }

        protected void PageResults<T>(IEnumerable<T> items)
        {
            T[] array = items.ToArray();
            int resultsCount = array.Length;
            if (PagingParameters.IncludeTotalCount)
            {
                const double accuracy = 1.0;
                PSObject totalCountResult = PagingParameters.NewTotalCount((ulong) resultsCount, accuracy);
                WriteObject(totalCountResult);
            }
            if (resultsCount > 0)
            {
                if (PagingParameters.Skip >= (ulong) resultsCount)
                {
                    WriteWarning("No results satisfy the paging parameters");
                }
                else
                {
                    ulong firstNumber = PagingParameters.Skip;
                    ulong lastNumber = firstNumber +
                                       Math.Min(
                                           PagingParameters.First,
                                           (ulong) resultsCount - PagingParameters.Skip
                                       );
                    for (ulong i = firstNumber; i < lastNumber; i++)
                    {
                        WriteObject(array[i]);
                    }
                }
            }
            else
            {
                WriteWarning("No results generated. The query returned 0 items.");
            }
        }

        protected CimSessionOptions GetSessionOptionsFromPSCredential(PSCredential pscredential)
        {
            string domain = pscredential.GetNetworkCredential().Domain;
            string userName = pscredential.GetNetworkCredential().UserName;
            SecureString password = pscredential.GetNetworkCredential().SecurePassword;
            CimCredential credential = new CimCredential(PasswordAuthenticationMechanism.NtlmDomain, domain, userName, password);
            CimSessionOptions options = new CimSessionOptions();
            options.AddDestinationCredentials(credential);
            return options;
        }
    }
}
