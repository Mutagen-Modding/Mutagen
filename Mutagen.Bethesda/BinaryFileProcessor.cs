using Mutagen.Bethesda.Internals;
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
        #region Config
        public class Config
        {
            public SortedList<FileLocation, byte> _substitutions;
            private RangeCollection _moveRanges;
            public Dictionary<RangeInt64, FileLocation> _moves;

            public void SetSubstitution(FileLocation loc, byte sub)
            {
                if (_substitutions == null)
                {
                    _substitutions = new SortedList<FileLocation, byte>();
                }
                _substitutions[loc] = sub;
            }

            public void SetSubstitution(FileLocation loc, byte[] sub)
            {
                if (_substitutions == null)
                {
                    _substitutions = new SortedList<FileLocation, byte>();
                }
                for (long i = 0; i < sub.Length; i++)
                {
                    _substitutions[loc + i] = sub[i];
                }
            }

            public void SetMove(RangeInt64 move, FileLocation loc)
            {
                if (_moves == null)
                {
                    _moveRanges = new RangeCollection();
                    _moves = new Dictionary<RangeInt64, FileLocation>();
                }
                if (_moveRanges.Collides(move))
                {
                    throw new ArgumentException("Can not have colliding moves.");
                }
                _moveRanges.Add(move);
                _moves[move] = loc;
                if (_moveRanges.IsEncapsulated(loc))
                {
                    throw new ArgumentException($"Cannot move to a section that is marked to be moved.");
                }
            }
        }
        #endregion

        private Config config;
        private readonly Stream source;
        private readonly byte[] _buffer;
        private readonly List<byte> _expandableBuffer = new List<byte>();
        private int bufferPos;
        private int bufferEnd;
        private FileLocation _position;
        private bool done;
        private SortedList<long, byte[]> _activeMoves;
        private SortedList<long, RangeInt64> _sortedMoves;
        private bool ExpandableBufferActive => _expandableBuffer.Count > 0;

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => source.Length;

        public override long Position
        {
            get => this._position + bufferPos;
            set => throw new NotImplementedException();
        }

        public BinaryFileProcessor(Stream source, Config config, int bufferLen = 4096)
        {
            this.source = source;
            this.config = config;
            if (this.config._moves != null)
            {
                this._activeMoves = new SortedList<long, byte[]>();
                this._sortedMoves = new SortedList<long, RangeInt64>();
                this._sortedMoves.Add(
                    this.config._moves.Select((m) => new KeyValuePair<long, RangeInt64>(m.Key.Min, m.Key)));
            }
            this._buffer = new byte[bufferLen];
        }

        protected override void Dispose(bool disposing)
        {
            this.source.Dispose();
            base.Dispose(disposing);
        }

        private void GetNextBuffer()
        {
            if (bufferPos != bufferEnd)
            {
                throw new ArgumentException("Get next buffer called when current buffer was not fully used.");
            }
            if (bufferEnd != 0)
            {
                if (ExpandableBufferActive)
                {
                    this._position += this._expandableBuffer.Count;
                }
                else
                {
                    this._position += this._buffer.Length;
                }
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
            if (config._substitutions == null) return;
            if (!config._substitutions.TryGetEncapsulatedIndices<FileLocation, byte>(
                lowerKey: new FileLocation(_position + bufferPos),
                higherKey: new FileLocation(_position + bufferEnd),
                result: out var substitutionRange)) return;
            for (int i = substitutionRange.Min; i <= substitutionRange.Max; i++)
            {
                var targetPos = config._substitutions.Keys[i];
                var bufferPos = targetPos - _position;
                var sub = config._substitutions.Values[i];
                if (ExpandableBufferActive)
                {
                    _expandableBuffer[bufferPos] = sub;
                }
                else
                {
                    _buffer[bufferPos] = sub;
                }
            }
        }

        private void DoMoves()
        {
            if (config._moves == null) return;

            int moveDeletions = 0;

            if (_sortedMoves.TryGetEncapsulatedIndices(
                lowerKey: _position + bufferPos,
                higherKey: _position + bufferEnd,
                result: out var moveIndices))
            {
                // Delete out moves
                for (int i = moveIndices.Min; i <= moveIndices.Max; i++)
                {
                    var moveRange = _sortedMoves.Values[i];
                    var moveLocBufStart = new FileLocation(moveRange.Min - this._position - moveDeletions);
                    ContentLength len = new ContentLength((int)Math.Min(bufferEnd - moveLocBufStart, moveRange.Width));

                    // Copy to move cache
                    byte[] moveContents = new byte[moveRange.Width];
                    Array.Copy(this._buffer, moveLocBufStart, moveContents, 0, len);
                    if (len < moveRange.Width)
                    {
                        this.source.Read(moveContents, len, (int)(len - moveRange.Width));
                    }

                    var moveLoc = config._moves[moveRange];
                    this._activeMoves[moveLoc] = moveContents;

                    // Delete moved snippet
                    if (len == moveRange.Width)
                    {
                        moveDeletions += len;
                        var sourceIndex = new FileLocation(moveRange.Max + 1);
                        var moveAmount = new FileLocation(this._buffer.Length - sourceIndex);
                        Array.Copy(this._buffer, sourceIndex, this._buffer, moveRange.Min, moveAmount);
                    }
                    bufferEnd -= len;
                }
            }

            if (_activeMoves.TryGetEncapsulatedIndices(
                lowerKey: _position + bufferPos,
                higherKey: _position + bufferEnd + moveDeletions,
                result: out var activeMoves))
            {
                if (_expandableBuffer.Count == 0)
                {
                    _expandableBuffer.AddRange(_buffer);
                }

                // Paste in moves
                for (int i = activeMoves.Min; i <= activeMoves.Max; i++)
                {
                    var moveToPaste = _activeMoves.Values[i];
                    var moveLoc = _activeMoves.Keys[i];
                    ContentLength adjustedMoveLoc = new ContentLength(moveLoc - moveDeletions - _position);
                    _expandableBuffer.InsertRange(adjustedMoveLoc, moveToPaste);
                }
            }
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
                    if (_expandableBuffer.Count == 0)
                    {
                        Array.Copy(
                            sourceArray: this._buffer,
                            sourceIndex: bufferPos,
                            destinationArray: buffer,
                            destinationIndex: offset,
                            length: toRead);
                    }
                    else
                    {
                        for (int i = 0; i < toRead; i++)
                        {
                            buffer[i + offset] = this._expandableBuffer[bufferPos + i];
                        }
                    }
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
