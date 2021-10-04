// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

namespace TinMonkey.HL7
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// HL7 Segment.
    /// </summary>
    public class HL7Segment : HL7Element
    {
        /// <summary>Initializes a new instance of the <see cref="HL7Segment"/> class.</summary>
        /// <param name="encoding">The encoding.</param>
        public HL7Segment(HL7Encoding encoding)
            : base(encoding)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HL7Segment" /> class.</summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="value">The value.</param>
        public HL7Segment(HL7Encoding encoding, string? value)
            : base(encoding, value)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HL7Segment" /> class.</summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="value">The value.</param>
        /// <param name="fields">The fields.</param>
        public HL7Segment(HL7Encoding encoding, string? value, IEnumerable<HL7Element> fields)
            : base(encoding, value, fields)
        {
        }

        /// <summary>Gets the inner text.</summary>
        /// <value>The value.</value>
        public override string? InnerText =>
            $"{this.Value}{this.Encoding.FieldDelimiter}{string.Join(this.Encoding.FieldDelimiter, this.Fields.Select(x => x?.InnerText))}";

        /// <summary>Gets the fields.</summary>
        /// <value>The fields.</value>
        public IEnumerable<HL7Field?> Fields => this.Children.Cast<HL7Field?>();

        /// <summary>Gets the delimiter.</summary>
        /// <value>The delimiter.</value>
        protected override char Delimiter => '\r';

        /// <summary>Creates the child.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The child.</returns>
        public override HL7Element CreateChild(ReadOnlySpan<char> value)
        {
            return new HL7Field(this.Encoding, value.ToString());
        }
    }
}
