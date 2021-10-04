// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

namespace TinMonkey.HL7
{
    /// <summary>HL7 parser options.</summary>
    public class HL7ParserOptions
    {
        /// <summary>The default parser options.</summary>
        public static readonly HL7ParserOptions Default = new ()
        {
            AllowComments = true,
            IgnoreBlankLines = true,
        };

        /// <summary>Gets or sets a value indicating whether to ignore blank lines or not.</summary>
        /// <value><c>true</c> if [ignore blank lines]; otherwise, <c>false</c>.</value>
        public bool IgnoreBlankLines { get; set; }

        /// <summary>Gets or sets a value indicating whether to allow comments or not.</summary>
        /// <value><c>true</c> if [allow comments]; otherwise, <c>false</c>.</value>
        public bool AllowComments { get; set; }
    }
}
