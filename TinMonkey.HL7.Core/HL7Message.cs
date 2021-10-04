// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

namespace TinMonkey.HL7
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>HL7 message.</summary>
    public class HL7Message
    {
        /// <summary>Initializes a new instance of the <see cref="HL7Message" /> class.</summary>
        /// <param name="encoding">The encoding.</param>
        /// <exception cref="System.ArgumentNullException">If encoding is null.</exception>
        public HL7Message(HL7Encoding encoding)
        {
            this.Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            this.Segments = new List<HL7Segment>();
        }

        /// <summary>Initializes a new instance of the <see cref="HL7Message" /> class.</summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="children">The segments.</param>
        /// <exception cref="System.ArgumentNullException">If encoding is null.</exception>
        public HL7Message(HL7Encoding encoding, IEnumerable<HL7Segment> children)
        {
            this.Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            this.Segments = children?.ToList() ?? throw new ArgumentNullException(nameof(children));
        }

        /// <summary>Gets the encoding.</summary>
        /// <value>The encoding.</value>
        public HL7Encoding Encoding { get; }

        /// <summary>Gets the segments.</summary>
        /// <value>The segments.</value>
        public IList<HL7Segment> Segments { get; }

        /// <summary>Gets the inner text.</summary>
        /// <value>The inner text.</value>
        public string? InnerText => string.Join("\r\n", this.Segments.Select(x => x.InnerText));
    }
}
