// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

namespace TinMonkey.HL7
{
    using System.Runtime.CompilerServices;

    /// <summary>Encoding extensions.</summary>
    public static class EncodingExtensions
    {
        /// <summary>Determines whether the specified c is separator.</summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="c">The character to check.</param>
        /// <returns><c>true</c> if the specified c is separator; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSeparator(this HL7Encoding encoding,  char c)
        {
            if (c == encoding.FieldDelimiter)
            {
                return true;
            }

            if (c == encoding.RepeatDelimiter)
            {
                return true;
            }

            if (c == encoding.ComponentDelimiter)
            {
                return true;
            }

            if (c == encoding.SubcomponentDelimiter)
            {
                return true;
            }

            if (c == '\0')
            {
                return true;
            }

            return false;
        }
    }
}
