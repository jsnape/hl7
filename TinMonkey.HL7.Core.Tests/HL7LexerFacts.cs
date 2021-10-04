// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

namespace TinMonkey.HL7.Tests
{
    using System.Diagnostics;
    using System.Text;
    using Xunit;
    using Token = TinMonkey.HL7.HL7Lexer.Token;

    /// <summary>HL7 lexer facts.</summary>
    public static class HL7LexerFacts
    {
        private const string Pid = "PID|E||PATID1234^^M11&ADT1^MR^GOOD HEALTH HOSPITAL~123456789^^^USSSA^SS||EVERYMAN^ADAM^A^III||19610615|M||2106-3|2222 HOME STREET^^GREENSBORO";

        /// <summary>Lexers should return token sequence.</summary>
        [Fact]
        public static void LexerShouldReturnTokenSequence()
        {
            var tokens = new[]
            {
                Token.Value, // PID
                Token.FieldSeparator,
                Token.Value, // 1
                Token.FieldSeparator,
                Token.FieldSeparator,
                Token.Value, // PATID1234
                Token.ComponentSeparator,
                Token.ComponentSeparator,
                Token.Value, // M11
                Token.SubcomponentSeparator,
                Token.Value, // ADT1
                Token.ComponentSeparator,
                Token.Value, // MR
                Token.ComponentSeparator,
                Token.Value, // GOOD HEALTH HOSPITAL
                Token.RepeatSeparator,
                Token.Value, // 123456789
                Token.ComponentSeparator,
                Token.ComponentSeparator,
                Token.ComponentSeparator,
                Token.Value, // USSSA
                Token.ComponentSeparator,
                Token.Value, // SS
                Token.FieldSeparator,
                Token.FieldSeparator,
                Token.Value, // EVERYMAN
                Token.ComponentSeparator,
                Token.Value, // ADAM
                Token.ComponentSeparator,
                Token.Value, // A
                Token.ComponentSeparator,
                Token.Value, // III
                Token.FieldSeparator,
                Token.FieldSeparator,
                Token.Value, // 19610615
                Token.FieldSeparator,
                Token.Value, // M
                Token.FieldSeparator,
                Token.FieldSeparator,
                Token.Value, // 2106-3
                Token.FieldSeparator,
                Token.Value, // 2222 HOME STREET
                Token.ComponentSeparator,
                Token.ComponentSeparator,
                Token.Value, // GREENSBORO
                Token.EndOfBuffer,
            };

            var lexer = new HL7Lexer(Pid, HL7Encoding.Default);
            int i = 0;

            while (lexer.MoveNext())
            {
                var expected = tokens[i++];
                var actual = lexer.CurrentToken;

                Assert.Equal(expected, actual);
            }
        }

        /// <summary>Lexers should return value sequence.</summary>
        [Fact]
        public static void LexerShouldReturnValueSequence()
        {
            var values = new[]
            {
                "PID",
                "E",
                "PATID1234",
                "M11",
                "ADT1",
                "MR",
                "GOOD HEALTH HOSPITAL",
                "123456789",
                "USSSA",
                "SS",
                "EVERYMAN",
                "ADAM",
                "A",
                "III",
                "19610615",
                "M",
                "2106-3",
                "2222 HOME STREET",
                "GREENSBORO",
            };

            var lexer = new HL7Lexer(Pid, HL7Encoding.Default);
            int i = 0;

            while (lexer.MoveNext())
            {
                if (lexer.CurrentToken != Token.Value)
                {
                    continue;
                }

                var expected = values[i++];
                var actual = lexer.CurrentValue.ToString();

                Assert.Equal(expected, actual);
            }
        }

        /// <summary>Lexer peek next should return token sequence.</summary>
        [Fact]
        public static void LexerPeekNextShouldReturnNextTokenSequence()
        {
            var tokens = new[]
            {
                Token.FieldSeparator, // PID

                Token.FieldSeparator, // |E
                Token.FieldSeparator, // E

                Token.FieldSeparator,

                Token.ComponentSeparator, // |PATID1234
                Token.ComponentSeparator, // PATID1234
                Token.ComponentSeparator,

                Token.SubcomponentSeparator, // ^M11
                Token.SubcomponentSeparator, // M11

                Token.ComponentSeparator, // &ADT1
                Token.ComponentSeparator, // ADT1

                Token.ComponentSeparator, // ^MR
                Token.ComponentSeparator, // MR

                Token.RepeatSeparator, // ^GOOD HEALTH HOSPITAL
                Token.RepeatSeparator, // GOOD HEALTH HOSPITAL

                Token.ComponentSeparator, // ~123456789
                Token.ComponentSeparator, // 123456789
                Token.ComponentSeparator,
                Token.ComponentSeparator,

                Token.ComponentSeparator, // ^USSSA
                Token.ComponentSeparator, // USSSA

                Token.FieldSeparator, // ^SS
                Token.FieldSeparator, // SS
                Token.FieldSeparator,

                Token.ComponentSeparator, // ^EVERYMAN
                Token.ComponentSeparator, // EVERYMAN

                Token.ComponentSeparator, // ^ADAM
                Token.ComponentSeparator, // ADAM

                Token.ComponentSeparator, // ^A
                Token.ComponentSeparator, // A

                Token.FieldSeparator, // ^III
                Token.FieldSeparator, // III
                Token.FieldSeparator,

                Token.FieldSeparator, // |19610615
                Token.FieldSeparator, // 19610615
                Token.FieldSeparator, // |M
                Token.FieldSeparator, // M
                Token.FieldSeparator,
                Token.FieldSeparator, // ^2106-3
                Token.FieldSeparator, // 2106-3
                Token.ComponentSeparator, // ^2222 HOME STREET
                Token.ComponentSeparator, // 2222 HOME STREET
                Token.ComponentSeparator,
                Token.EndOfBuffer, // GREENSBORO
                Token.EndOfBuffer,
            };

            var lexer = new HL7Lexer(Pid, HL7Encoding.Default);
            int i = 0;

            while (lexer.MoveNext() && lexer.CurrentToken != Token.EndOfBuffer)
            {
                var expected = tokens[i++];
                var actual = lexer.PeekNext();

                Debug.WriteLine(actual);
                Assert.Equal(expected, actual);
            }
        }

        /// <summary>The Lexer loop should rebuild original.</summary>
        [Fact]
        public static void LexerLoopShouldRebuildOriginal()
        {
            var output = new StringBuilder(Pid.Length);
            var encoding = HL7Encoding.Default;

            var lexer = new HL7Lexer(Pid, encoding);
            {
                while (lexer.MoveNext())
                {
                    switch (lexer.CurrentToken)
                    {
                        case Token.Value:
                            output.Append(lexer.CurrentValue);
                            break;
                        case Token.FieldSeparator:
                            output.Append(encoding.FieldDelimiter);
                            break;
                        case Token.ComponentSeparator:
                            output.Append(encoding.ComponentDelimiter);
                            break;
                        case Token.SubcomponentSeparator:
                            output.Append(encoding.SubcomponentDelimiter);
                            break;
                        case Token.RepeatSeparator:
                            output.Append(encoding.RepeatDelimiter);
                            break;
                        default:
                            break;
                    }
                }

                Assert.Equal(Pid, output.ToString());
            }
        }
    }
}
