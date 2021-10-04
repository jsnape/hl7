// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

namespace TinMonkey.HL7
{
    using System;

    /// <summary>Span extensions.</summary>
    public static class SpanExtensions
    {
        /// <summary>Counts the numer of occurences of <c>value</c> in the <c>span</c>.</summary>
        /// <typeparam name="T">The type of items in the ReadOnlySpan.</typeparam>
        /// <param name="span">The span.</param>
        /// <param name="value">The value.</param>
        /// <returns>A count of the value occurences or zero if there are none.</returns>
        internal static int CountOf<T>(this ReadOnlySpan<T> span, T value)
            where T : struct
        {
            var count = 0;

            for (int i = 1; i < span.Length; ++i)
            {
                if (value.Equals(span[i]))
                {
                    ++count;
                }
            }

            return count;
        }
    }
}
