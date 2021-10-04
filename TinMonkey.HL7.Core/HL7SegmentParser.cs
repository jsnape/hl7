// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

namespace TinMonkey.HL7
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using TinMonkey.HL7.Core.Properties;

    /// <summary>The HL7 segment parser.</summary>
    internal ref struct HL7SegmentParser
    {
        /// <summary>The encoding.</summary>
        private readonly HL7Encoding encoding;
        private readonly ReadOnlySpan<byte> buffer;

        /// <summary>Initializes a new instance of the <see cref="HL7SegmentParser" /> struct.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="encoding">The encoding.</param>
        public HL7SegmentParser(ReadOnlySpan<byte> buffer, HL7Encoding encoding)
        {
            this.encoding = encoding;

            if (buffer.Length < HL7Constants.SegmentLabelLength)
            {
                throw new HL7ParseException(Resources.ParseErrorSegmentLabelTooShort);
            }

            this.Label = buffer[0..HL7Constants.SegmentLabelLength];
            this.buffer = buffer;
        }

        /// <summary>Gets the segment label.</summary>
        /// <value>The segment label.</value>
        public ReadOnlySpan<byte> Label { get; }

        /// <summary>Parses this instance.</summary>
        /// <returns>A list of fields.</returns>
        public List<HL7Field> Parse()
        {
            var fields = new List<HL7Field>();
            var localBuffer = this.buffer.Slice(HL7Constants.SegmentLabelLength + 1);

            if (this.Label.StartsWith(HL7Constants.MshSegmentLabelBytes))
            {
                var delimiters = Encoding.UTF8.GetString(
                    this.buffer.Slice(HL7Constants.SegmentLabelLength, HL7Constants.DelimiterLength));

                var delimiterField = new HL7Field(this.encoding, delimiters);

                fields.Add(delimiterField);

                localBuffer = localBuffer[HL7Constants.DelimiterLength..];
            }

            while (Next((byte)this.encoding.FieldDelimiter, ref localBuffer, out var field))
            {
                HL7Field? head = null;
                HL7Field? next = null;

                while (Next((byte)this.encoding.RepeatDelimiter, ref field, out var repeat))
                {
                    var hl7Field = this.ParseField(repeat);

                    if (next == null)
                    {
                        head = next = hl7Field;
                    }
                    else
                    {
                        next.Next = hl7Field;
                        next = hl7Field;
                    }
                }

                fields.Add(head ?? new HL7Field(this.encoding));
            }

            return fields;
        }

        /// <summary>Nexts the specified buffer.</summary>
        /// <param name="delimiter">The delimiter.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="field">The field.</param>
        /// <returns>True if there is more data to be read.</returns>
        private static bool Next(byte delimiter, ref ReadOnlySpan<byte> buffer, out ReadOnlySpan<byte> field)
        {
            if (buffer.IsEmpty)
            {
                field = ReadOnlySpan<byte>.Empty;
                return false;
            }

            var nextDelimiter = buffer.IndexOf(delimiter);

            if (nextDelimiter == -1)
            {
                field = buffer;
                buffer = ReadOnlySpan<byte>.Empty;
            }
            else
            {
                field = buffer[..nextDelimiter];
                buffer = buffer.Slice(nextDelimiter + 1);
            }

            return true;
        }

        private HL7Field ParseField(ReadOnlySpan<byte> field)
        {
            if (field.IsEmpty)
            {
                return new HL7Field(this.encoding);
            }

            if (field.IndexOfAny((byte)this.encoding.ComponentDelimiter, (byte)this.encoding.SubcomponentDelimiter) == -1)
            {
                return new HL7Field(this.encoding, Encoding.UTF8.GetString(field));
            }

            var components = new List<HL7Component>();

            while (Next((byte)this.encoding.ComponentDelimiter, ref field, out var componentSpan))
            {
                HL7Component? component;

                if (componentSpan.IndexOf((byte)this.encoding.SubcomponentDelimiter) == -1)
                {
                    component = new HL7Component(this.encoding, Encoding.UTF8.GetString(componentSpan));
                }
                else
                {
                    var subcomponents = new List<HL7Subcomponent>();

                    while (Next((byte)this.encoding.SubcomponentDelimiter, ref componentSpan, out var subcomponentSpan))
                    {
                        var subcomponent = new HL7Subcomponent(this.encoding, Encoding.UTF8.GetString(subcomponentSpan));
                        subcomponents.Add(subcomponent);
                    }

                    component = new HL7Component(this.encoding, subcomponents);
                }

                components.Add(component);
            }

            return new HL7Field(this.encoding, components);
        }
    }
}
