// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

namespace TinMonkey.HL7
{
    using System;

    /// <summary>The HL7 stream processor interface.</summary>
    public interface IHL7StreamProcessor
    {
        /// <summary>Called when [next] line is read.</summary>
        /// <param name="line">The line.</param>
        /// <param name="lineNumber">The line number.</param>
        void OnNext(in ReadOnlySpan<byte> line, int lineNumber);

        /// <summary>Called when a [comment] is read.</summary>
        /// <param name="line">The line.</param>
        /// <param name="lineNumber">The line number.</param>
        void OnComment(in ReadOnlySpan<byte> line, int lineNumber);

        /// <summary>Called when an [error] is raised.</summary>
        /// <param name="line">The line.</param>
        /// <param name="error">The error.</param>
        /// <param name="lineNumber">The line number.</param>
        void OnError(in ReadOnlySpan<byte> line, Exception error, int lineNumber);

        /// <summary>Called when [complete].</summary>
        void OnComplete();
    }
}
