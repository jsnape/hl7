// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

namespace TinMonkey.HL7
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// HL7Element base class.
    /// </summary>
    [DebuggerDisplay("{DisplayValue,nq}")]
    public abstract class HL7Element
    {
        /// <summary>Initializes a new instance of the <see cref="HL7Element" /> class.</summary>
        /// <param name="encoding">The encoding.</param>
        /// <exception cref="System.ArgumentNullException">If encoding is null.</exception>
        protected HL7Element(HL7Encoding encoding)
        {
            this.Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            this.Children = new List<HL7Element>();
        }

        /// <summary>Initializes a new instance of the <see cref="HL7Element" /> class.</summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="value">The value.</param>
        protected HL7Element(HL7Encoding encoding, string? value)
            : this(encoding, value, Enumerable.Empty<HL7Element>())
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HL7Element" /> class.</summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="children">The children.</param>
        /// <exception cref="System.ArgumentNullException">If encoding is null.</exception>
        protected HL7Element(HL7Encoding encoding, IEnumerable<HL7Element> children)
        {
            this.Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            this.Children = children.ToList();
        }

        /// <summary>Initializes a new instance of the <see cref="HL7Element" /> class.</summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="value">The value.</param>
        /// <param name="children">The children.</param>
        /// <exception cref="System.ArgumentNullException">If encoding is null.</exception>
        protected HL7Element(HL7Encoding encoding, string? value, IEnumerable<HL7Element> children)
        {
            this.Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            this.Value = value;
            this.Children = children.ToList();
        }

        /// <summary>Gets or sets the value.</summary>
        /// <value>The value.</value>
        public string? Value { get; set; }

        /// <summary>Gets the inner text.</summary>
        /// <value>The value.</value>
        public virtual string? InnerText
        {
            get
            {
                if (this.Value != null)
                {
                    return this.Value;
                }

                if (this.Children.Count == 0)
                {
                    return string.Empty;
                }

                var delimiter = this.Children[0].Delimiter;

                return this.Value ?? string.Join(delimiter, this.Children.Select(x => x.InnerText));
            }
        }

        /// <summary>Gets the display value.</summary>
        /// <value>The display value.</value>
        public string DisplayValue => $"{this.GetType().Name}({this.InnerText})";

        /// <summary>Gets the encoding.</summary>
        /// <value>The encoding.</value>
        public HL7Encoding Encoding { get; }

        /// <summary>Gets or sets the index.</summary>
        /// <value>The index.</value>
        public int Index { get; set; }

        /// <summary>Gets the children.</summary>
        /// <value>The children.</value>
        public IList<HL7Element> Children { get; }

        /// <summary>Gets the delimiter.</summary>
        /// <value>The delimiter.</value>
        protected abstract char Delimiter { get; }

        /// <summary>Creates the child.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The child.</returns>
        public abstract HL7Element CreateChild(ReadOnlySpan<char> value);

        /// <summary>Created and adds a new child.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The child.</returns>
        public HL7Element AddChild(ReadOnlySpan<char> value)
        {
            var child = this.CreateChild(value);
            this.Children.Add(child);
            return child;
        }
    }
}
