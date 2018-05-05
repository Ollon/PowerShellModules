// -----------------------------------------------------------------------
// <copyright file="AbstractCmdlet.PathEvaluator.cs" company="Ollon, LLC">
//     Copyright (c) 2017 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
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


        protected static class ErrorFactory
        {

            public static ErrorRecord NotSpecified(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.NotSpecified, target);
            }

            public static ErrorRecord OpenError(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.OpenError, target);
            }

            public static ErrorRecord CloseError(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.CloseError, target);
            }

            public static ErrorRecord DeviceError(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.DeviceError, target);
            }

            public static ErrorRecord DeadlockDetected(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.DeadlockDetected, target);
            }

            public static ErrorRecord InvalidArgument(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.InvalidArgument, target);
            }

            public static ErrorRecord InvalidData(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.InvalidData, target);
            }

            public static ErrorRecord InvalidOperation(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.InvalidOperation, target);
            }

            public static ErrorRecord InvalidResult(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.InvalidResult, target);
            }

            public static ErrorRecord InvalidType(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.InvalidType, target);
            }

            public static ErrorRecord MetadataError(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.MetadataError, target);
            }

            public static ErrorRecord NotImplemented(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.NotImplemented, target);
            }

            public static ErrorRecord NotInstalled(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.NotInstalled, target);
            }

            public static ErrorRecord ObjectNotFound(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.ObjectNotFound, target);
            }

            public static ErrorRecord OperationStopped(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.OperationStopped, target);
            }

            public static ErrorRecord OperationTimeout(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.OperationTimeout, target);
            }

            public static ErrorRecord SyntaxError(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.SyntaxError, target);
            }

            public static ErrorRecord ParserError(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.ParserError, target);
            }

            public static ErrorRecord PermissionDenied(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.PermissionDenied, target);
            }

            public static ErrorRecord ResourceBusy(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.ResourceBusy, target);
            }

            public static ErrorRecord ResourceExists(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.ResourceExists, target);
            }

            public static ErrorRecord ResourceUnavailable(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.ResourceUnavailable, target);
            }

            public static ErrorRecord ReadError(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.ReadError, target);
            }

            public static ErrorRecord WriteError(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.WriteError, target);
            }

            public static ErrorRecord FromStdErr(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.FromStdErr, target);
            }

            public static ErrorRecord SecurityError(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.SecurityError, target);
            }

            public static ErrorRecord ProtocolError(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.ProtocolError, target);
            }

            public static ErrorRecord ConnectionError(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.ConnectionError, target);
            }

            public static ErrorRecord AuthenticationError(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.AuthenticationError, target);
            }

            public static ErrorRecord LimitsExceeded(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.LimitsExceeded, target);
            }

            public static ErrorRecord QuotaExceeded(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.QuotaExceeded, target);
            }

            public static ErrorRecord NotEnabled(Exception exception, object target = null)
            {
                return new ErrorRecord(exception, exception.HResult.ToString(), ErrorCategory.NotEnabled, target);
            }

            public static ErrorRecord Create(Exception e, ErrorCategory c = ErrorCategory.WriteError, object target = null)
            {
                return new ErrorRecord(e, e.Source, c, target);
            }
        }
    }
}
