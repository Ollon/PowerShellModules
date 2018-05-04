using System;
using System.Runtime.Serialization;

namespace PowerShell.EditorFeatures.Core.Parsing
{

    /// <summary>
    /// Happens during parse when a conversion becomes exceptional.
    /// </summary>
    [Serializable]
    internal class FailedParseConversionException : Exception
    {
        /// <summary>
        /// Initializes a new Instance of the <see cref="FailedParseConversionException"/> class.
        /// </summary>
        public FailedParseConversionException()
        {
        }

        /// <summary>
        /// Initializes a new Instance of the <see cref="FailedParseConversionException"/> class.
        /// </summary>
        /// <param name="message"></param>
        public FailedParseConversionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new Instance of the <see cref="FailedParseConversionException"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public FailedParseConversionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new Instance of the <see cref="FailedParseConversionException"/> class.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected FailedParseConversionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
