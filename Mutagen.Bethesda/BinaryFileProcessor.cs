using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class BinaryFileProcessor : Stream
    {
        private SortedList<long, byte> _substitutions;
        private Dictionary<RangeInt64, long> _moves;
        private readonly Stream source;
        private readonly byte[] _buffer;
        private int bufferPos;
        private int bufferEnd;
        private long _position;
        private bool done;

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => source.Length;

        public override long Position
        {
            get => this._position + bufferPos;
            set => throw new NotImplementedException();
        }

        public BinaryFileProcessor(Stream source, int bufferLen = 4096)
        {
            this.source = source;
            this._buffer = new byte[bufferLen];
        }

        protected override void Dispose(bool disposing)
        {
            this.source.Dispose();
            base.Dispose(disposing);
        }

        #region Set Rules API
        public void SetSubstitution(long loc, byte sub)
        {
            if (_substitutions == null)
            {
                _substitutions = new SortedList<long, byte>();
            }
            _substitutions[loc] = sub;
        }

        public void SetMove(RangeInt64 move, long loc)
        {
            if (_moves == null)
            {
                _moves = new Dictionary<RangeInt64, long>();
            }
            _moves[move] = loc;
        }
        #endregion

        private void GetNextBuffer()
        {
            if (bufferPos != bufferEnd)
            {
                throw new ArgumentException("Get next buffer called when current buffer was not fully used.");
            }
            if (bufferEnd != 0)
            {
                this._position += this._buffer.Length;
            }
            bufferPos = 0;
            bufferEnd = this.source.Read(this._buffer, 0, this._buffer.Length);
            if (bufferEnd == 0)
            {
                done = true;
                return;
            }

            DoSubstitutions();
            DoMoves();
        }

        private void DoSubstitutions()
        {
            if (_substitutions == null) return;
            if (!_substitutions.TryGetEncapsulatedIndices(
                lowerKey: _position + bufferPos,
                higherKey: _position + bufferEnd,
                result: out var substitutionRange)) return;
            for (int i = substitutionRange.Min; i <= substitutionRange.Max; i++)
            {
                _buffer[_substitutions.Keys[i] - _position] = _substitutions.Values[i];
            }
        }

        private void DoMoves()
        {
            if (_moves == null) return;

        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count == 0) return 0;
            int initialCount = count;
            while (count > 0 && !done)
            {
                var remaining = bufferEnd - bufferPos;
                if (remaining > 0)
                {
                    var toRead = Math.Min(remaining, count);
                    Array.Copy(
                        sourceArray: this._buffer,
                        sourceIndex: bufferPos,
                        destinationArray: buffer,
                        destinationIndex: offset,
                        length: toRead);
                    bufferPos += toRead;
                    if (toRead == count) return initialCount;
                    offset += toRead;
                    count -= toRead;
                }
                GetNextBuffer();
            }
            return initialCount - count;
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        #region N/A
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
        #endregion
    }
}
