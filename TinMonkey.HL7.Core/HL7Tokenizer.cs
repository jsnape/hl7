// <copyright file="HL7Tokenizer.cs" company="TinMonkey">
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>

namespace TinMonkey.HL7
{
    using System;
    using TinMonkey.HL7.Properties;

    /// <summary>The HL7 tokenizer.</summary>
    internal ref struct HL7Tokenizer
    {
        /// <summary>The encoding.</summary>
        private readonly HL7Encoding encoding;

        /// <summary>The buffer.</summary>
        private readonly ReadOnlySpan<char> buffer;

        /// <summary>The buffer offset.</summary>
        private int offset;

        /// <summary>Initializes a new instance of the <see cref="HL7Tokenizer" /> struct.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="encoding">The encoding.</param>
        public HL7Tokenizer(ReadOnlySpan<char> buffer, HL7Encoding encoding)
        {
            this.encoding = encoding;

            this.buffer = buffer;
            this.offset = 0;
        }

        /// <summary>Gets the segment.</summary>
        /// <value>The segment.</value>
        /// <exception cref="HL7ParseException">If the segment label is too short.</exception>
        public ReadOnlySpan<char> Segment =>
            this.buffer.Length > 2 ? this.buffer[..3] : throw new HL7ParseException(Resources.ParseErrorSegmentLabelTooShort);
    }

    internal enum SpanType
    {
        None,
        Field,
        Repeat,
        Component,
        Subcomponent,
    }

    internal ref struct HL7Span
    {
        /// <summary>The buffer subspan.</summary>
        private readonly ReadOnlySpan<char> span;

        /// <summary>The span type.</summary>
        private readonly SpanType spanType;

        /// <summary>The buffer offset.</summary>
        private int offset;

        /// <summary>Initializes a new instance of the <see cref="HL7Span"/> struct.</summary>
        /// <param name="spanType">Type of the span.</param>
        /// <param name="span">The span.</param>
        /// <param name="offset">The offset.</param>
        public HL7Span(SpanType spanType, ReadOnlySpan<char> span, int offset)
        {
            this.spanType = spanType;
            this.span = span;
            this.offset = offset;
        }
    }
}
