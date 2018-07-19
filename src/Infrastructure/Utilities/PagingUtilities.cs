// -----------------------------------------------------------------------
// <copyright file="PagingUtilities.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Management.Automation;

namespace PowerShell.Infrastructure.Utilities
{


    public static class PagingUtilities
    {

        public static void Page<T>(PSCmdlet cmdlet, params T[] items)
        {
            int resultsCount = items.Length;
            if (cmdlet.PagingParameters.IncludeTotalCount)
            {
                const double accuracy = 1.0;
                PSObject totalCountResult = cmdlet.PagingParameters.NewTotalCount((ulong)resultsCount, accuracy);
                cmdlet.WriteObject(totalCountResult);
            }
            if (resultsCount > 0)
            {
                if (cmdlet.PagingParameters.Skip >= (ulong)resultsCount)
                {
                    cmdlet.WriteWarning("No results satisfy the paging parameters");
                }
                else
                {
                    ulong firstNumber = cmdlet.PagingParameters.Skip;
                    ulong lastNumber = firstNumber
                                       + Math.Min(
                                           cmdlet.PagingParameters.First,
                                           (ulong)resultsCount - cmdlet.PagingParameters.Skip
                                       );
                    for (ulong i = firstNumber; i < lastNumber; i++)
                    {
                        cmdlet.WriteObject(items[i]);
                    }
                }
            }
            else
            {
                cmdlet.WriteWarning("No results generated. The query returned 0 items.");
            }
        }
    }
}
