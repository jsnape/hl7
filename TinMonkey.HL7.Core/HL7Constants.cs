// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

namespace TinMonkey.HL7
{
    using System.Text;

    /// <summary>
    /// HL7 Constants.
    /// </summary>
    public static class HL7Constants
    {
        /// <summary>The MSH segment label.</summary>
        public const string MshSegmentLabel = "MSH";

        /// <summary>The default HL7 delimiters.</summary>
        public const string DefaultDelimiters = @"|^~\&";

        /// <summary>The segment label length.</summary>
        public const int SegmentLabelLength = 3;

        /// <summary>
        /// The minimum MSH segment length.
        /// </summary>
        public const int MinMshSegmentLength = 8; //// MshSegmentLabel.Length + DefaultEncoding.Length;

        /// <summary>The delimiter length.</summary>
        public const int DelimiterLength = 5;
    }
}
