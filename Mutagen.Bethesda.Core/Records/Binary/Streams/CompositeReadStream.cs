using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mutagen.Bethesda.Records.Binary.Streams
{
    /// <summary>
    /// Class for reading through several streams.
    /// Minimal implementation, but can be fleshed out eventually
    /// </summary>
    public class CompositeReadStream : Stream
    {
        private readonly Stream[] _subStreams;
        private int _targetStreamIndex;
        private long _position;
        private readonly long _length;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _length;

        private Stream TargetStream => _subStreams[_targetStreamIndex];
        private bool Done => _targetStreamIndex >= _subStreams.Length;

        public override long Position
        {
            get => _position;
            set
            {
                if (value != _position)
                {
                    throw new NotImplementedException();
                }
            }
        }

        public CompositeReadStream(IEnumerable<Stream> streams, bool resetPositions = true, bool trimNulls = true)
        {
            if (trimNulls)
            {
                this._subStreams = streams.NotNull().ToArray();
            }
            else
            {
                this._subStreams = streams.ToArray();
            }
            foreach (var stream in this._subStreams)
            {
                this._length += stream.Length;
                if (stream.Position != 0)
                {
                    if (resetPositions)
                    {
                        stream.Position = 0;
                    }
                    else
                    {
                        throw new ArgumentException("Input streams must be set to position 0");
                    }
                }
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_targetStreamIndex >= _subStreams.Length) return 0;
            int totalRead = 0;
            while (!Done && count > 0)
            {
                var toRead = (int)Math.Min(count, TargetStream.Length);
                var amountRead = this.TargetStream.Read(buffer, offset, toRead);
                totalRead += amountRead;
                count -= amountRead;
                offset += amountRead;
                this._position += amountRead;
                if (this.TargetStream.Remaining() == 0)
                {
                    this._targetStreamIndex++;
                }
            }
            return totalRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
