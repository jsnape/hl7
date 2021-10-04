// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

namespace TinMonkey.HL7
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>The HL7 lexer.</summary>
    /// <remarks>
    /// This code isn't used in the current parser but remains in case anyone
    /// wishes to implement a traditional lexer/parser combination.
    /// </remarks>
    [DebuggerDisplay("{this.CurrentToken} {this.CurrentValue.ToString(),nq}")]
    internal ref struct HL7Lexer
    {
        /// <summary>The encoding.</summary>
        private readonly HL7Encoding encoding;

        /// <summary>The buffer.</summary>
        private readonly ReadOnlySpan<char> buffer;

        /// <summary>The buffer offset.</summary>
        private int offset;

        /// <summary>Initializes a new instance of the <see cref="HL7Lexer" /> struct.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="encoding">The encoding.</param>
        public HL7Lexer(ReadOnlySpan<char> buffer, HL7Encoding encoding)
        {
            this.encoding = encoding;

            this.buffer = buffer;
            this.offset = 0;

            this.CurrentToken = Token.Segment;
            this.DelimiterToken = Token.Segment;
            this.CurrentValue = ReadOnlySpan<char>.Empty;
        }

        /// <summary>Lexer Tokens.</summary>
        /// <remarks>
        /// Don't reorder these enums. The relative value of each is
        /// used in the parser to determine end of sequence processing.
        /// </remarks>
        internal enum Token
        {
            /// <summary>None Token.</summary>
            None,

            /// <summary>The value.</summary>
            Value,

            /// <summary>The subcomponent separator.</summary>
            SubcomponentSeparator,

            /// <summary>The component separator.</summary>
            ComponentSeparator,

            /// <summary>Repeat separator.</summary>
            RepeatSeparator,

            /// <summary>Field separator.</summary>
            FieldSeparator,

            /// <summary>The end of buffer.</summary>
            EndOfBuffer,

            /// <summary>The segment.</summary>
            Segment,
        }

        /// <summary>Gets the current token.</summary>
        /// <value>The current token.</value>
        public Token CurrentToken { get; private set; }

        /// <summary>Gets the delimiter token.</summary>
        /// <value>The delimiter token.</value>
        public Token DelimiterToken { get; private set; }

        /// <summary>Gets the current value.</summary>
        /// <value>The current value.</value>
        public ReadOnlySpan<char> CurrentValue { get; private set; }

        /// <summary>Gets the current character.</summary>
        /// <value>The current character.</value>
        public char CurrentChar => this.buffer[this.offset];

        /// <summary>Gets the offset.</summary>
        /// <value>The offset.</value>
        public int Offset => this.offset;

#if DEBUG
        /// <summary>Gets the previous.</summary>
        /// <value>The previous.</value>
        internal string Previous => this.buffer[.. this.offset].ToString();

        /// <summary>Gets the remainder.</summary>
        /// <value>The remainder.</value>
        internal string Remainder => this.buffer[this.offset ..].ToString();

        /// <summary>Gets the progress.</summary>
        /// <value>The progress.</value>
        internal string Progress => $"{this.Previous}«•»{this.Remainder}";

        /// <summary>Gets the next.</summary>
        /// <value>The next.</value>
        internal Token Next => this.PeekNext();
#endif

        /// <summary>Peeks the next token without moving the offset.</summary>
        /// <returns>The next token.</returns>
        /// <remarks>Be careful, this returns the next token both for values and separators.</remarks>
        public Token PeekNext()
        {
            int next = this.offset;

            while (next < this.buffer.Length)
            {
                char c = this.buffer[next++];

                if (this.encoding.IsSeparator(c))
                {
                    return this.ToToken(c);
                }
            }

            return Token.EndOfBuffer;
        }

        /// <summary>Moves to the next token.</summary>
        /// <returns>True if there is more data to be processed.</returns>
        public bool MoveNext()
        {
            int start = this.offset;

            while (this.offset < this.buffer.Length)
            {
                char c = this.buffer[this.offset++];

                if (this.encoding.IsSeparator(c))
                {
                    this.DelimiterToken = this.CurrentToken = this.ToToken(c);
                    this.CurrentValue = ReadOnlySpan<char>.Empty;
                    return true;
                }

                char n = this.offset == this.buffer.Length ? '\0' : this.buffer[this.offset];

                if (this.encoding.IsSeparator(n))
                {
                    this.CurrentToken = Token.Value;
                    this.CurrentValue = this.buffer[start..this.offset];
                    return true;
                }
            }

            this.CurrentToken = Token.EndOfBuffer;

            return false;
        }

        /// <summary>Spans the matching.</summary>
        /// <returns>The matching.</returns>
        public ReadOnlySpan<char> SpanMatching()
        {
            int next = this.offset;

            while (next < this.buffer.Length)
            {
                char c = this.buffer[next++];

                if (this.ToToken(c) == this.CurrentToken)
                {
                    return this.buffer[this.offset..next];
                }
            }

            return this.buffer[this.offset..];
        }

        /// <summary>Moves the by.</summary>
        /// <param name="offset">The offset.</param>
        public void MoveBy(int offset)
        {
            this.offset += offset;
        }

        /// <summary>Converts to token.</summary>
        /// <param name="c">The character to convert.</param>
        /// <returns>The token.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Token ToToken(char c)
        {
            if (c == this.encoding.FieldDelimiter)
            {
                return Token.FieldSeparator;
            }

            if (c == this.encoding.RepeatDelimiter)
            {
                return Token.RepeatSeparator;
            }

            if (c == this.encoding.ComponentDelimiter)
            {
                return Token.ComponentSeparator;
            }

            Debug.Assert(c == this.encoding.SubcomponentDelimiter, "Invalid delimiter character");
            return Token.SubcomponentSeparator;
        }
    }
}
