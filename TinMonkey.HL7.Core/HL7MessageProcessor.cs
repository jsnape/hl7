// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

namespace TinMonkey.HL7
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>HL7 message processor.</summary>
    /// <seealso cref="IHL7StreamProcessor" />
    public class HL7MessageProcessor : IHL7StreamProcessor
    {
        /// <summary>The MSH segment label bytes.</summary>
        private static readonly byte[] MshSegmentLabelBytes = Encoding.ASCII.GetBytes(HL7Constants.MshSegmentLabel);

        /// <summary>The segments.</summary>
        private readonly List<HL7Segment> segments = new ();

        /// <summary>The messages.</summary>
        private readonly List<HL7Message> messages = new ();

        /// <summary>The encoding.</summary>
        private HL7Encoding encoding;

        /// <summary>Initializes a new instance of the <see cref="HL7MessageProcessor"/> class.</summary>
        public HL7MessageProcessor()
            : this(HL7Encoding.Default)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HL7MessageProcessor"/> class.</summary>
        /// <param name="initialEncoding">The initial encoding.</param>
        /// <exception cref="System.ArgumentNullException">If initialEncoding is null.</exception>
        public HL7MessageProcessor(HL7Encoding initialEncoding)
        {
            this.encoding = initialEncoding ?? throw new ArgumentNullException(nameof(initialEncoding));
        }

        /// <summary>Gets the messages.</summary>
        /// <value>The messages.</value>
        public IList<HL7Message> Messages => this.messages;

        /// <summary>Called when a [comment] is read.</summary>
        /// <param name="line">The line.</param>
        /// <param name="lineNumber">The line number.</param>
        public void OnComment(in ReadOnlySpan<byte> line, int lineNumber)
        {
            // Method intentionally left empty.
        }

        /// <summary>Called when [complete].</summary>
        public void OnComplete()
        {
            this.MessageComplete();
        }

        /// <summary>Called when an [error] is raised.</summary>
        /// <param name="line">The line.</param>
        /// <param name="error">The error.</param>
        /// <param name="lineNumber">The line number.</param>
        public void OnError(in ReadOnlySpan<byte> line, Exception error, int lineNumber)
        {
            // Method intentionally left empty.
        }

        /// <summary>Called when [next] line is read.</summary>
        /// <param name="line">The line.</param>
        /// <param name="lineNumber">The line number.</param>
        public void OnNext(in ReadOnlySpan<byte> line, int lineNumber)
        {
            if (line.StartsWith(MshSegmentLabelBytes))
            {
                this.MessageComplete();

                var delimiters = Encoding.UTF8.GetString(line[3..8]);
                this.encoding = HL7Encoding.Create(delimiters);
            }

            var segmentParser = new HL7SegmentParser(line, this.encoding);
            var fields = segmentParser.Parse();

            var segment = new HL7Segment(
                this.encoding, Encoding.UTF8.GetString(segmentParser.Label), fields);

            this.segments.Add(segment);
        }

        /// <summary>Called when a message is complete.</summary>
        private void MessageComplete()
        {
            if (this.segments.Count == 0)
            {
                return;
            }

            var message = new HL7Message(this.encoding, this.segments);
            this.messages.Add(message);

            this.segments.Clear();
        }
    }
}
