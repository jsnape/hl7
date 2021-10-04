// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

namespace TinMonkey.HL7.Tests
{
    using System;
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// HL7Encoding Facts.
    /// </summary>
    public class HL7EncodingFacts
    {
        /// <summary>The test target.</summary>
        private readonly HL7Encoding target = HL7Encoding.Default;

        /// <summary>Creating the default encoding should return default.</summary>
        [Fact]
        public static void CreatingDefaultEncodingShouldReturnDefault()
        {
            var actual = HL7Encoding.Create(HL7Constants.DefaultDelimiters);
            Assert.Same(HL7Encoding.Default, actual);
        }

        /// <summary>Creating different encoding should return unique.</summary>
        [Fact]
        public static void CreatingDifferentEncodingShouldReturnUnique()
        {
            var actual = HL7Encoding.Create(".@!#*");
            Assert.NotSame(HL7Encoding.Default, actual);
        }

        /// <summary>Null constructor delimiters should throw.</summary>
        [Fact]
        public static void NullDelimitersShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new HL7Encoding(null!));
        }

        /// <summary>Unterminated escape should throw.</summary>
        [Fact]
        public void UnterminatedEscapeShouldThrow()
        {
            Assert.Throws<HL7ParseException>(() => this.target.Decode(@"bar\foo"));
        }

        /// <summary>Unregognized escape should pass through.</summary>
        [Fact]
        public void UnregognizedEscapeShouldPassThrough()
        {
            const string encoded = @"bar\L\zoo";
            var decoded = this.target.Decode(encoded);

            Assert.Equal(encoded, decoded.ToString());
        }

        /// <summary>Default field delimiter is correct.</summary>
        [Fact]
        public void DefaultFieldDelimiterIsCorrect()
        {
            Assert.Equal('|', this.target.FieldDelimiter);
        }

        /// <summary>Default component delimiter is correct.</summary>
        [Fact]
        public void DefaultComponentDelimiterIsCorrect()
        {
            Assert.Equal('^', this.target.ComponentDelimiter);
        }

        /// <summary>Default subcomponent delimiter is correct.</summary>
        [Fact]
        public void DefaultSubcomponentDelimiterIsCorrect()
        {
            Assert.Equal('&', this.target.SubcomponentDelimiter);
        }

        /// <summary>Default repeat delimiter is correct.</summary>
        [Fact]
        public void DefaultRepeatDelimiterIsCorrect()
        {
            Assert.Equal('~', this.target.RepeatDelimiter);
        }

        /// <summary>Default escape character is correct.</summary>
        [Fact]
        public void DefaultEscapeCharacterIsCorrect()
        {
            Assert.Equal('\\', this.target.EscapeCharacter);
        }

        /// <summary>Default present but null is correct.</summary>
        [Fact]
        public void DefaultPresentButNullValueIsCorrect()
        {
            Assert.Equal("\"\"", this.target.PresentButNull);
        }

        /// <summary>Decodeds the strings should be correct.</summary>
        /// <param name="encoded">The encoded value.</param>
        /// <param name="decoded">The decoded value.</param>
        [Theory]
        [MemberData(nameof(EncodeDecodeTestPairs))]
        public void DecodedStringsShouldBeCorrect(string encoded, string decoded)
        {
            var result = this.target.Decode(encoded).ToString();
            Assert.Equal(decoded, result);
        }

        /// <summary>Encode and decode test pairs.</summary>
        /// <returns>A sequence of test values.</returns>
        private static IEnumerable<object[]> EncodeDecodeTestPairs()
        {
            yield return new[] { "no encode", "no encode" };

            yield return new[]
            {
                @"Not bold \H\This text is bold\N\ also normal\.br\",
                "Not bold <B>This text is bold</B> also normal<BR>",
            };

            yield return new[]
            {
                @"\F\ \S\ \T\ \R\ \E\",
                @"| ^ & ~ \",
            };

            yield return new[] { @"\X12\", "18", };

            yield return new[] { @"\XFFFF\", "65535", };
        }
    }
}
