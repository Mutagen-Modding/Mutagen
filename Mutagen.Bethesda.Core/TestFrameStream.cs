using System;
using System.IO;

namespace Noggog.Streams
{
    public class TestFrameStream : Stream
    {
        private readonly Stream _wrap;
        private readonly bool _dispose;
        private readonly long _limit;
        private readonly long _offset;

        public override bool CanRead => _wrap.CanRead;

        public override bool CanSeek => _wrap.CanSeek;

        public override bool CanWrite => _wrap.CanWrite;

        public override long Length { get; }

        public override long Position 
        {
            get => _wrap.Position + _offset;
            set => _wrap.Position = value - _offset;
        }

        public TestFrameStream(Stream wrap, long limit, bool doDispose = true)
        {
            _offset = -wrap.Position;
            _wrap = wrap;
            _dispose = doDispose;
            _limit = limit;
            if (_wrap.Length >= 0)
            {
                Length = Math.Min(_wrap.Remaining(), _limit);
            }
            else
            {
                Length = _limit;
            }
        }

        public override void Flush()
        {
            _wrap.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            // if (Position + count > Length)
            // {
            //     throw new ArgumentOutOfRangeException($"Tried to write more data than was available: {Position + count} > {Length}");
            // }
            var diff = Length - (Position + count);
            if (diff < 0)
            {
                count = (int)(count + diff);
            }
            if (count == 0) return 0;
            return _wrap.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length + offset;
                    break;
                default:
                    break;
            }
            return Position;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (Position + count > Length)
            {
                throw new ArgumentOutOfRangeException($"Tried to write more data than was available: {Position + count} > {Length}");
            }
            _wrap.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _dispose)
            {
                _wrap.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
