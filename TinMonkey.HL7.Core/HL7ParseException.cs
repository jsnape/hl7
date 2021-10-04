// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

namespace TinMonkey.HL7
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>Thrown by HL7 parsers when the message cannot be read.</summary>
    [Serializable]
    public class HL7ParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HL7ParseException"/> class.
        /// </summary>
        public HL7ParseException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HL7ParseException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public HL7ParseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HL7ParseException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public HL7ParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HL7ParseException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected HL7ParseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
