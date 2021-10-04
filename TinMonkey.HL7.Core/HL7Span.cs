// <copyright file="HL7Span.cs" company="TinMonkey">
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>

namespace TinMonkey.HL7
{
    using System;

    internal ref struct HL7Span
    {
        private readonly ReadOnlySpan<byte> buffer;
        private readonly int[] offsets;

        public HL7Span(ReadOnlySpan<byte> buffer, byte delimiter)
        {
            this.buffer = buffer;

            var count = buffer.CountOf(delimiter);
            this.offsets = new int[count];
        }
    }
}
