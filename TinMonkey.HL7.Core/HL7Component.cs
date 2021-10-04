// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

namespace TinMonkey.HL7
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>HL7 Component.</summary>
    /// <seealso cref="TinMonkey.HL7.HL7Element" />
    public class HL7Component : HL7Element
    {
        /// <summary>Initializes a new instance of the <see cref="HL7Component" /> class.</summary>
        /// <param name="encoding">The encoding.</param>
        public HL7Component(HL7Encoding encoding)
            : base(encoding)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HL7Component" /> class.</summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="value">The value.</param>
        public HL7Component(HL7Encoding encoding, string? value)
            : base(encoding, value)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HL7Component" /> class.</summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="subcomponents">The subcomponents.</param>
        public HL7Component(HL7Encoding encoding, IEnumerable<HL7Element> subcomponents)
            : base(encoding, subcomponents)
        {
        }

        /// <summary>Gets the subcomponents.</summary>
        /// <value>The subcomponents.</value>
        public IEnumerable<HL7Subcomponent?> Subcomponents => this.Children.Cast<HL7Subcomponent?>();

        /// <summary>Gets the delimiter.</summary>
        /// <value>The delimiter.</value>
        protected override char Delimiter => this.Encoding.ComponentDelimiter;

        /// <summary>Creates the child.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The child.</returns>
        public override HL7Element CreateChild(ReadOnlySpan<char> value)
        {
            return new HL7Subcomponent(this.Encoding, value.ToString());
        }
    }
}
