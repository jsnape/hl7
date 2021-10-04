// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

namespace TinMonkey.HL7
{
    using System;

    /// <summary>HL7 Subcomponent.</summary>
    /// <seealso cref="TinMonkey.HL7.HL7Element" />
    public class HL7Subcomponent : HL7Element
    {
        /// <summary>Initializes a new instance of the <see cref="HL7Subcomponent" /> class.</summary>
        /// <param name="encoding">The encoding.</param>
        public HL7Subcomponent(HL7Encoding encoding)
            : base(encoding)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HL7Subcomponent" /> class.</summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="value">The value.</param>
        public HL7Subcomponent(HL7Encoding encoding, string? value)
            : base(encoding, value)
        {
        }

        /// <summary>Gets the delimiter.</summary>
        /// <value>The delimiter.</value>
        protected override char Delimiter => this.Encoding.SubcomponentDelimiter;

        /// <summary>Creates the child.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The child.</returns>
        /// <exception cref="System.InvalidOperationException">Subcomponents don't have children.</exception>
        public override HL7Element CreateChild(ReadOnlySpan<char> value)
        {
            throw new InvalidOperationException();
        }
    }
}
