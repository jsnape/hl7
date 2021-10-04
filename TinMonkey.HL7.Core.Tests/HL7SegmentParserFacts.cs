// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

namespace TinMonkey.HL7.Tests
{
    using System.Linq;
    using System.Text;
    using Xunit;

    /// <summary>HL7 segment parser facts.</summary>
    public class HL7SegmentParserFacts
    {
        /// <summary>Segments the only should parse.</summary>
        [Fact]
        public void SegmentOnlyShouldParse()
        {
            const string line = "PV1|";
            var bytes = Encoding.UTF8.GetBytes(line);

            var target = new HL7SegmentParser(bytes, HL7Encoding.Default);
            var fields = target.Parse();

            Assert.Equal("PV1", Encoding.UTF8.GetString(target.Label));
            Assert.Empty(fields);
        }

        /// <summary>Segments the only should parse.</summary>
        [Fact]
        public void SegmentOneFieldShouldParse()
        {
            const string line = "PV1|ABC";
            var bytes = Encoding.UTF8.GetBytes(line);

            var target = new HL7SegmentParser(bytes, HL7Encoding.Default);
            var fields = target.Parse();

            var firstField = fields.Single();

            Assert.Equal("PV1", Encoding.UTF8.GetString(target.Label));
            Assert.Equal("ABC", firstField.Value);
        }

        /// <summary>Segments the only should parse.</summary>
        [Fact]
        public void SegmentOneBlankFieldShouldParse()
        {
            const string line = "PV1||";
            var bytes = Encoding.UTF8.GetBytes(line);

            var target = new HL7SegmentParser(bytes, HL7Encoding.Default);
            var fields = target.Parse();

            var firstField = fields.Single();

            Assert.Equal("PV1", Encoding.UTF8.GetString(target.Label));
            Assert.Null(firstField.Value);
        }

        /// <summary>Segments the only should parse.</summary>
        [Fact]
        public void SegmentMultipleBlankFieldShouldParse()
        {
            const string line = "PV1|||";
            var bytes = Encoding.UTF8.GetBytes(line);

            var target = new HL7SegmentParser(bytes, HL7Encoding.Default);
            var fields = target.Parse();

            var firstField = fields[0];
            var secondField = fields[1];

            Assert.Equal("PV1", Encoding.UTF8.GetString(target.Label));

            Assert.Null(firstField.Value);
            Assert.Null(secondField.Value);
        }

        /// <summary>Segment with multiple non blank fields should parse.</summary>
        [Fact]
        public void SegmentMultipleNonBlankFieldsShouldParse()
        {
            const string line = "PV1|AA|BB|CC";
            var bytes = Encoding.UTF8.GetBytes(line);

            var target = new HL7SegmentParser(bytes, HL7Encoding.Default);
            var fields = target.Parse();

            var firstField = fields[0];
            var secondField = fields[1];

            Assert.Equal("PV1", Encoding.UTF8.GetString(target.Label));
            Assert.Equal("AA", firstField.Value);
            Assert.Equal("BB", secondField.Value);

            Assert.Equal("CC", fields[2].Value);
        }

        /// <summary>Segment with component field should parse.</summary>
        [Fact]
        public void SegmentComponentFieldShouldParse()
        {
            const string line = "PV1|AA|BB^CC";
            var bytes = Encoding.UTF8.GetBytes(line);

            var target = new HL7SegmentParser(bytes, HL7Encoding.Default);
            var fields = target.Parse();

            var firstField = fields[0];
            var secondField = fields[1];

            Assert.Equal("PV1", Encoding.UTF8.GetString(target.Label));
            Assert.Equal("AA", firstField.Value);

            Assert.Equal(2, secondField.Children.Count);
            Assert.Equal("BB", secondField.Children[0].Value);
            Assert.Equal("CC", secondField.Children[1].Value);
        }

        /// <summary>Segment with subcomponent field should parse.</summary>
        [Fact]
        public void SegmentSubcomponentFieldShouldParse()
        {
            const string line = "PV1|AA|BB&CC^DD&EE";
            var bytes = Encoding.UTF8.GetBytes(line);

            var target = new HL7SegmentParser(bytes, HL7Encoding.Default);
            var fields = target.Parse();

            var secondField = fields[1];

            Assert.Equal(2, secondField.Children[0].Children.Count);
            Assert.Equal("BB", secondField.Children[0].Children[0].Value);
            Assert.Equal("CC", secondField.Children[0].Children[1].Value);

            Assert.Equal(2, secondField.Children[1].Children.Count);
            Assert.Equal("DD", secondField.Children[1].Children[0].Value);
            Assert.Equal("EE", secondField.Children[1].Children[1].Value);
        }

        /// <summary>Basic PID should parse.</summary>
        [Fact]
        public void BasicPidShouldParse()
        {
            const string line = "PID||E|2000^2012^01||004777^ATTEND&AARON^A&11111^FOO&BAR^A||SUR|||7|A0|";
            var bytes = Encoding.UTF8.GetBytes(line);

            var target = new HL7SegmentParser(bytes, HL7Encoding.Default);
            var fields = target.Parse();

            Assert.Equal(11, fields.Count);
            Assert.Equal(3, fields[2].Children.Count);
            Assert.Equal(2, fields[4].Children[1].Children.Count);
        }

        /// <summary>Basic PID should parse.</summary>
        [Fact]
        public void RepeatShouldParse()
        {
            const string line = "PID|AAAA~BBBB~CCCC";
            var bytes = Encoding.UTF8.GetBytes(line);

            var target = new HL7SegmentParser(bytes, HL7Encoding.Default);
            var fields = target.Parse();

            Assert.Single(fields);

            var repeats = fields.Single().Repeats;

            Assert.Equal(3, repeats.Count());

            var repeatValues = repeats.Select(x => x.Value);

            Assert.Equal(new[] { "AAAA", "BBBB", "CCCC" }, repeatValues);
        }
    }
}
