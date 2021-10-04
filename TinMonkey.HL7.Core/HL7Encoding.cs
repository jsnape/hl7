// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

namespace TinMonkey.HL7
{
    using System;
    using System.Globalization;
    using System.Text;
    using TinMonkey.HL7.Core.Properties;

    /// <summary>HL7 Character Encoding.</summary>
    public class HL7Encoding
    {
        /// <summary>The default encoding.</summary>
        public static readonly HL7Encoding Default = new (HL7Constants.DefaultDelimiters);

        /// <summary>The field delimiter.</summary>
        private readonly string fieldDelimiter;

        /// <summary>The component delimiter.</summary>
        private readonly string componentDelimiter;

        /// <summary>The repeat delimiter.</summary>
        private readonly string repeatDelimiter;

        /// <summary>The escape character.</summary>
        private readonly string escapeCharacter;

        /// <summary>The subcomponent delimiter.</summary>
        private readonly string subcomponentDelimiter;

        /// <summary>
        /// Initializes a new instance of the <see cref="HL7Encoding"/> class.
        /// </summary>
        /// <param name="delimiters">The delimiters.</param>
        public HL7Encoding(ReadOnlySpan<char> delimiters)
        {
            if (delimiters.IsEmpty)
            {
                throw new ArgumentNullException(nameof(delimiters));
            }

            this.Delimiters = delimiters.ToString();

            this.fieldDelimiter = delimiters[0].ToString();
            this.componentDelimiter = delimiters[1].ToString();
            this.repeatDelimiter = delimiters[2].ToString();
            this.escapeCharacter = delimiters[3].ToString();
            this.subcomponentDelimiter = delimiters[4].ToString();
        }

        /// <summary>Gets the delimiters.</summary>
        /// <value>The delimiters.</value>
        public string Delimiters { get; }

        /// <summary>Gets the field delimiter.</summary>
        /// <value>The field delimiter.</value>
        public char FieldDelimiter => this.fieldDelimiter[0];

        /// <summary>Gets the component delimiter.</summary>
        /// <value>The component delimiter.</value>
        public char ComponentDelimiter => this.componentDelimiter[0];

        /// <summary>Gets the repeat delimiter.</summary>
        /// <value>The repeat delimiter.</value>
        public char RepeatDelimiter => this.repeatDelimiter[0];

        /// <summary>Gets the escape character.</summary>
        /// <value>The escape character.</value>
        public char EscapeCharacter => this.escapeCharacter[0];

        /// <summary>Gets the subcomponent delimiter.</summary>
        /// <value>The subcomponent delimiter.</value>
        public char SubcomponentDelimiter => this.subcomponentDelimiter[0];

        /// <summary>Gets the present but null.</summary>
        /// <value>The present but null.</value>
        public string PresentButNull { get; } = "\"\"";

        /// <summary>Creates the specified delimiters.</summary>
        /// <param name="delimiters">The delimiters.</param>
        /// <returns>Either the default encoding or a specific one.</returns>
        public static HL7Encoding Create(ReadOnlySpan<char> delimiters)
        {
            if (delimiters.CompareTo(Default.Delimiters, StringComparison.Ordinal) == 0)
            {
                return Default;
            }

            return new HL7Encoding(delimiters);
        }

        /// <summary>Decodes the specified encoded value.</summary>
        /// <param name="encoded">The encoded value.</param>
        /// <returns>A decoded string.</returns>
        /// <exception cref="HL7ParseException">Invalid escape sequence in HL7 string.</exception>
        public ReadOnlySpan<char> Decode(ReadOnlySpan<char> encoded)
        {
            if (encoded.IndexOf(this.EscapeCharacter) == -1)
            {
                return encoded;
            }

            var decoded = new StringBuilder();

            for (int i = 0; i < encoded.Length; ++i)
            {
                char c = encoded[i];

                if (c != this.EscapeCharacter)
                {
                    decoded.Append(c);
                    continue;
                }

                ++i; // Swallow the escape character

                int nextEscape = encoded[i..].IndexOf(this.EscapeCharacter);

                if (nextEscape == -1)
                {
                    throw new HL7ParseException(Resources.ParseErrorIncompleteEscapeSequence);
                }

                nextEscape += i; // Add the offset back after checking for not-found.
                var escapeSequence = encoded[i..nextEscape];
                i = nextEscape;

                ReadOnlySpan<char> decodedSequence = escapeSequence[0] switch
                {
                    'H' => "<B>",
                    'N' => "</B>",
                    '.' => ToProcessingCommand(escapeSequence),

                    'F' => this.fieldDelimiter,
                    'S' => this.componentDelimiter,
                    'T' => this.subcomponentDelimiter,
                    'R' => this.repeatDelimiter,
                    'E' => this.escapeCharacter,

                    'X' => ToHexString(escapeSequence),

                    _ => Escape(escapeSequence, this.escapeCharacter),
                };

                decoded.Append(decodedSequence);
            }

            return decoded.ToString();

            static string Escape(ReadOnlySpan<char> value, string escapeCharacter) =>
                string.Concat(escapeCharacter, value, escapeCharacter);

            static string ToHexString(ReadOnlySpan<char> value) =>
                int.Parse(value[1..], NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture)
                   .ToString(CultureInfo.InvariantCulture);

            static ReadOnlySpan<char> ToProcessingCommand(ReadOnlySpan<char> value) =>
                value.CompareTo(".br", StringComparison.Ordinal) == 0 ? "<BR>" : value;
        }
    }
}
