// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

namespace TinMonkey.HL7
{
    using System;
    using System.Buffers;
    using System.IO;
    using System.IO.Pipelines;
    using System.Threading.Tasks;

    /// <summary>HL7 stream parser.</summary>
    public class HL7StreamParser
    {
        /// <summary>The maximum stack limit.</summary>
        private const int MaxStackLimit = 1024;

        /// <summary>The parser options.</summary>
        private readonly HL7ParserOptions options;

        /// <summary>The processor.</summary>
        private readonly IHL7StreamProcessor processor;

        /// <summary>The line number.</summary>
        private int lineNumber;

        /// <summary>Initializes a new instance of the <see cref="HL7StreamParser" /> class.</summary>
        /// <param name="processor">The processor.</param>
        public HL7StreamParser(IHL7StreamProcessor processor)
            : this(processor, HL7ParserOptions.Default)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HL7StreamParser" /> class.</summary>
        /// <param name="processor">The processor.</param>
        /// <param name="options">The parser options.</param>
        /// <exception cref="ArgumentNullException">
        /// Any argument is null.
        /// </exception>
        public HL7StreamParser(IHL7StreamProcessor processor, HL7ParserOptions options)
        {
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>Parses the file asynchronously.</summary>
        /// <param name="path">The file path.</param>
        /// <returns>A task that completes when the file has been processed.</returns>
        public async Task ParseFileAsync(string path)
        {
            using var stream = new FileStream(
                path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize: 1, useAsync: true);

            await this.ParseStreamAsync(stream)
                .ConfigureAwait(false);
        }

        /// <summary>Parses the stream asynchronously.</summary>
        /// <param name="stream">The stream.</param>
        /// <returns>A task that completes when the stream has been processed.</returns>
        public async Task ParseStreamAsync(Stream stream)
        {
            this.lineNumber = 0;
            var reader = PipeReader.Create(stream);

            while (true)
            {
                var read = await reader.ReadAsync()
                    .ConfigureAwait(false);

                var buffer = read.Buffer;

                // Read complete segments from the current buffer.
                var position = this.ReadLines(buffer, read.IsCompleted);

                if (read.IsCompleted)
                {
                    break;
                }

                reader.AdvanceTo(position, buffer.End);
            }

            this.processor.OnComplete();
        }

        /// <summary>Reads the lines.</summary>
        /// <param name="sequence">The sequence.</param>
        /// <param name="isCompleted">if set to <c>true</c> [is completed].</param>
        /// <returns>The next position.</returns>
        private SequencePosition ReadLines(in ReadOnlySequence<byte> sequence, bool isCompleted)
        {
            var reader = new SequenceReader<byte>(sequence);

            while (!reader.End)
            {
                // The official segment terminator in HL7 is a carriage return.
                if (reader.TryReadTo(out ReadOnlySpan<byte> itemBytes, (byte)'\r', advancePastDelimiter: true))
                {
                    // However, due to reasons the file often has a standard windows CRLF terminator so
                    // exclude these if they exist at the start of the line (i.e. it was right after the
                    // segment terminator).
                    _ = reader.IsNext((byte)'\n', advancePast: true);

                    this.ProcessLine(itemBytes);
                }
                else if (isCompleted)
                {
                    // The last item has no final delimiter.
                    this.ReadLastLine(sequence.Slice(reader.Position));
                    reader.Advance(sequence.Length);
                }
                else
                {
                    // no more items in this sequence
                    break;
                }
            }

            return reader.Position;
        }

        /// <summary>Reads the last segment.</summary>
        /// <param name="sequence">The sequence.</param>
        private void ReadLastLine(in ReadOnlySequence<byte> sequence)
        {
            var length = (int)sequence.Length;

            if (length < MaxStackLimit)
            {
                // If the item is small enough we'll stack allocate the buffer
                Span<byte> byteBuffer = stackalloc byte[length];
                sequence.CopyTo(byteBuffer);
                this.ProcessLine(byteBuffer);
            }
            else
            {
                // Otherwise we'll rent an array to use as the buffer
                var byteBuffer = ArrayPool<byte>.Shared.Rent(length);

                try
                {
                    sequence.CopyTo(byteBuffer);
                    this.ProcessLine(byteBuffer);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(byteBuffer);
                }
            }
        }

        /// <summary>Processes the line.</summary>
        /// <param name="buffer">The buffer.</param>
        private void ProcessLine(ReadOnlySpan<byte> buffer)
        {
            if (buffer.IsEmpty && this.options.IgnoreBlankLines)
            {
                return;
            }

            if (buffer[0] == (byte)'#')
            {
                if (this.options.AllowComments)
                {
                    this.processor.OnComment(buffer, ++this.lineNumber);
                }

                return;
            }

            this.processor.OnNext(buffer, ++this.lineNumber);
        }
    }
}
