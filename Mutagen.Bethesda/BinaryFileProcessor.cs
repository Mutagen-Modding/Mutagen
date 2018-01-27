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
            public Dictionary<FileSection, FileLocation> _moves;
            public bool HasProcessing => this._moves?.Count > 0
                || this._substitutions?.Count > 0;

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

            public void SetMove(FileSection move, FileLocation loc)
            {
                if (loc == move.Min) return;
                if (loc < move.Min)
                {
                    throw new NotImplementedException("Cannot move a section earlier in the stream");
                }
                if (_moves == null)
                {
                    _moveRanges = new RangeCollection();
                    _moves = new Dictionary<FileSection, FileLocation>();
                }
                if (_moveRanges.Collides(move.Range))
                {
                    throw new ArgumentException("Can not have colliding moves.");
                }
                _moveRanges.Add(move.Range);
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
        private SortedList<FileLocation, byte[]> _activeMoves;
        private SortedList<FileLocation, FileSection> _sortedMoves;
        private bool ExpandableBufferActive => _expandableBuffer.Count > 0;
        public bool HasProcessing { get; private set; }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => source.Length;

        public override long Position
        {
            get => this._position + bufferPos;
            set => this.SetPosition(value);
        }

        public BinaryFileProcessor(Stream source, Config config, int bufferLen = 4096)
        {
            this.source = source;
            this.config = config;
            if (this.config._moves != null)
            {
                this._activeMoves = new SortedList<FileLocation, byte[]>();
                this._sortedMoves = new SortedList<FileLocation, FileSection>();
                this._sortedMoves.Add(
                    this.config._moves.Select((m) => new KeyValuePair<FileLocation, FileSection>(m.Key.Min, m.Key)));
            }
            this._buffer = new byte[bufferLen];
            this.HasProcessing = this.config.HasProcessing;
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

            FileLocation[] moveFromKeys = null;
            List<FileLocation> moveToKeys = new List<FileLocation>();
            FileSection targetSection = new FileSection(_position + bufferPos, _position + bufferEnd);

            if (_sortedMoves.TryGetEncapsulatedIndices(
                lowerKey: targetSection.Min,
                higherKey: targetSection.Max,
                result: out var moveIndices))
            {
                moveFromKeys = moveIndices.Select((i) => _sortedMoves.Keys[i]).ToArray();
            }
            else
            {
                moveFromKeys = null;
            }

            if (_activeMoves.TryGetEncapsulatedIndices(
                lowerKey: targetSection.Min,
                higherKey: targetSection.Max,
                result: out var activeMoves))
            {
                moveToKeys.AddRange(activeMoves.Select((i) => _activeMoves.Keys[i]));
            }

            int moveFromIndex = 0;
            int moveToIndex = 0;

            while ((moveFromIndex < moveFromKeys?.Length)
                || moveToIndex < moveToKeys.Count)
            {
                FileLocation? moveFromKey = null;
                if (moveFromIndex < moveFromKeys?.Length)
                {
                    moveFromKey = moveFromKeys[moveFromIndex];
                }
                FileLocation? moveToKey = null;
                if (moveToIndex < moveToKeys.Count)
                {
                    moveToKey = moveToKeys[moveToIndex];
                }

                if (moveToKey == null || moveFromKey < moveToKey)
                {
                    // Delete out move
                    var moveRange = _sortedMoves[moveFromKey.Value];
                    var moveLocBufStart = new FileLocation(moveRange.Min - this._position - moveDeletions);
                    ContentLength len = new ContentLength((int)Math.Min(bufferEnd - moveLocBufStart, moveRange.Width));

                    // Copy to move cache
                    byte[] moveContents = new byte[moveRange.Width];
                    Array.Copy(this._buffer, moveLocBufStart, moveContents, 0, len);
                    if (len < moveRange.Width)
                    {
                        this.source.Read(moveContents, len, (int)(moveRange.Width - len));
                    }

                    var moveLoc = config._moves[moveRange];
                    this._activeMoves[moveLoc] = moveContents;
                    if (targetSection.Range.IsInRange(moveLoc))
                    {
                        moveToKeys.Add(moveLoc);
                    }

                    // Delete moved snippet
                    if (len == moveRange.Width)
                    {
                        moveDeletions += len;
                        var sourceIndex = moveRange.Max + 1 - this._position;
                        var moveAmount = this._buffer.Length - sourceIndex;
                        var destination = moveRange.Min - this._position;
                        Array.Copy(
                            sourceArray: this._buffer,
                            sourceIndex: sourceIndex,
                            destinationArray: this._buffer,
                            destinationIndex: destination,
                            length: moveAmount);
                    }
                    bufferEnd -= len;
                    moveFromIndex++;
                }
                else
                {
                    if (_expandableBuffer.Count == 0)
                    {
                        _expandableBuffer.AddRange(_buffer);
                    }

                    var moveToPaste = _activeMoves[moveToKey.Value];
                    var adjustedMoveLoc = new ContentLength(moveToKey.Value - _position);
                    var adjustedMoveLoc2 = new ContentLength(adjustedMoveLoc - moveDeletions);
                    _expandableBuffer.InsertRange(adjustedMoveLoc2, moveToPaste);

                    this._activeMoves.Remove(moveToKey.Value);
                    moveToIndex++;
                }
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!HasProcessing)
            {
                var ret = this.source.Read(buffer, offset, count);
                this._position += ret;
                return ret;
            }
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

        private void SetPosition(long pos)
        {
            if (pos == this.Position) return;
            if (pos < this.Position)
            {
                throw new NotImplementedException("Cannot move back in position");
            }
            var diff = pos - this.Position;
            if (diff > int.MaxValue)
            {
                throw new NotImplementedException("Need to upgrade to move large positions");
            }
            byte[] trash = new byte[diff];
            this.Read(trash, offset: 0, count: (int)diff);
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
