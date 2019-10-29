using System;
using System.IO;

namespace Vysn.Voice
{
    internal sealed class AudioStream : Stream
    {
        /// <inheritdoc />
        public override bool CanRead
            => false;

        /// <inheritdoc />
        public override bool CanSeek
            => false;

        /// <inheritdoc />
        public override bool CanWrite
            => true;

        /// <inheritdoc />
        public override long Length
            => throw new NotSupportedException("This property isn't supported.");

        /// <inheritdoc />
        public override long Position
        {
            get => throw new NotSupportedException("Cannot get position of this stream.");
            set => throw new NotSupportedException("Cannot set position of this stream.");
        }

        private readonly AudioEncoder _encoder;

        public AudioStream(AudioEncoder encoder)
        {
            _encoder = encoder;
        }

        /// <inheritdoc />
        public override void Flush()
        {
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
            => 0;

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
            => 0;

        /// <inheritdoc />
        public override void SetLength(long value)
            => throw new NotSupportedException("Cannot set length of this stream.");

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            var data = buffer.AsSpan(offset, count);
            _encoder.Encode(data);
        }
    }
}