// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

namespace TinMonkey.HL7
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>HL7 Field.</summary>
    /// <seealso cref="TinMonkey.HL7.HL7Element" />
    public class HL7Field : HL7Element
    {
        /// <summary>Initializes a new instance of the <see cref="HL7Field" /> class.</summary>
        /// <param name="encoding">The encoding.</param>
        public HL7Field(HL7Encoding encoding)
            : base(encoding)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HL7Field" /> class.</summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="value">The value.</param>
        public HL7Field(HL7Encoding encoding, string? value)
            : base(encoding, value)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HL7Field" /> class.</summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="components">The components.</param>
        public HL7Field(HL7Encoding encoding, IEnumerable<HL7Element> components)
            : base(encoding, components)
        {
        }

        /// <summary>Gets or sets the next repetition.</summary>
        /// <value>The next repetition.</value>
        public HL7Field? Next { get; set; }

        /// <summary>Gets the value.</summary>
        /// <value>The value.</value>
        public override string? InnerText
        {
            get
            {
                if (this.Next != null)
                {
                    return string.Join(this.Encoding.RepeatDelimiter, this.Repeats.Select(x => x.ChildText));
                }

                if (this.Children.Count != 0)
                {
                    return this.ChildText;
                }

                if (this.Value == null)
                {
                    return null;
                }

                if (this.Value.StartsWith(this.Encoding.FieldDelimiter))
                {
                    // The encoding field already has a delimeter attached.
                    return this.Value[1..];
                }

                return this.Value;
            }
        }

        /// <summary>Gets the repeats.</summary>
        /// <value>The repeats.</value>
        public IEnumerable<HL7Field> Repeats
        {
            get
            {
                var next = this;

                while (next != null)
                {
                    yield return next;
                    next = next.Next;
                }
            }
        }

        /// <summary>Gets the components.</summary>
        /// <value>The components.</value>
        public IEnumerable<HL7Component?> Components => this.Children.Cast<HL7Component?>();

        /// <summary>Gets the delimiter.</summary>
        /// <value>The delimiter.</value>
        protected override char Delimiter => this.Encoding.FieldDelimiter;

        /// <summary>Gets the child text.</summary>
        /// <value>The child text.</value>
        private string ChildText =>
            string.Join(this.Encoding.ComponentDelimiter, this.Components.Select(x => x?.InnerText));

        /// <summary>Creates the child.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The child.</returns>
        public override HL7Element CreateChild(ReadOnlySpan<char> value)
        {
            return new HL7Component(this.Encoding, value.ToString());
        }
    }
}
