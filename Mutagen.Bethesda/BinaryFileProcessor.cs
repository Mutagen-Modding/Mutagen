using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using Noggog.Utility;
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
            internal SortedList<FileLocation, byte[]> _additions;
            internal SortedList<FileLocation, byte> _substitutions;
            internal RangeCollection _moveRanges;
            internal Dictionary<FileSection, FileLocation> _moves;
            internal Dictionary<FileLocation, List<FileSection>> _sameLocMoves;
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

            public void SetAddition(FileLocation loc, byte[] addition)
            {
                if (_additions == null)
                {
                    _additions = new SortedList<FileLocation, byte[]>();
                }
                _additions[loc] = addition;
            }

            public void SetMove(FileSection move, FileLocation loc)
            {
                if (move.Width == 0) return;
                if (loc == move.Min) return;
                if (loc < move.Min)
                {
                    throw new NotImplementedException("Cannot move a section earlier in the stream");
                }
                if (_moves == null)
                {
                    _moveRanges = new RangeCollection();
                    _moves = new Dictionary<FileSection, FileLocation>();
                    _sameLocMoves = new Dictionary<FileLocation, List<FileSection>>();
                }
                if (_moveRanges.Collides(move.Range))
                {
                    throw new ArgumentException("Can not have colliding moves.");
                }
                _moveRanges.Add(move.Range);
                _moves[move] = loc;
                _sameLocMoves.TryCreateValue(loc).Add(move);
                if (_moveRanges.IsEncapsulated(loc))
                {
                    throw new ArgumentException($"Cannot move to a section that is marked to be moved: {loc}");
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
        private int extraRead;
        private FileLocation _position;
        private bool done;
        private SortedList<FileLocation, List<(FileSection Section, byte[] Data)>> _activeMoves;
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
                this._activeMoves = new SortedList<FileLocation, List<(FileSection Section, byte[] Data)>>();
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
                this._position += this._buffer.Length + extraRead;
            }
            bufferPos = 0;
            var prevExtraRead = extraRead;
            extraRead = 0;
            bufferEnd = this.source.Read(this._buffer, 0, this._buffer.Length);
            this._expandableBuffer.Clear();
            if (bufferEnd == 0)
            {
                done = true;
                return;
            }

            FileSection targetSection = new FileSection(_position + bufferPos, _position + bufferEnd - 1);
            
            DoSubstitutions(targetSection);
            DoMoves(prevExtraRead, targetSection);
        }

        private void DoSubstitutions(FileSection targetSection)
        {
            if (config._substitutions == null) return;
            if (!config._substitutions.TryGetEncapsulatedIndices<FileLocation, byte>(
                lowerKey: new FileLocation(_position + bufferPos),
                higherKey: new FileLocation(_position + bufferEnd - 1),
                result: out var substitutionRange)) return;
            for (int i = substitutionRange.Min; i <= substitutionRange.Max; i++)
            {
                var targetPos = config._substitutions.Keys[i];
                var bufferPos = targetPos - _position;
                var sub = config._substitutions.Values[i];
                _buffer[bufferPos] = sub;
            }
        }

        private void DoMoves(int prevExtraRead, FileSection targetSection)
        {
            if (config._moves == null) return;

            if (targetSection.Range.IsInRange(0xC5CF50))
            {
                int wer = 23;
                wer++;
            }

            int moveDeletions = 0;

            FileLocation[] moveFromKeys = null;
            List<FileLocation> moveToKeys = new List<FileLocation>();
            List<FileLocation> additionKeys = null;

            if (_sortedMoves.TryGetEncapsulatedIndices(
                lowerKey: targetSection.Min,
                higherKey: targetSection.Max,
                result: out var moveIndices))
            {
                moveFromKeys = moveIndices.Select((i) => _sortedMoves.Keys[i]).ToArray();
            }

            if (_activeMoves.TryGetEncapsulatedIndices(
                lowerKey: targetSection.Min,
                higherKey: targetSection.Max,
                result: out var activeMoves))
            {
                moveToKeys = activeMoves.Select((i) => _activeMoves.Keys[i]).ToList();
                moveToKeys.Sort();
            }

            if (config._additions != null
                && config._additions.TryGetEncapsulatedIndices(
                    lowerKey: targetSection.Min,
                    higherKey: targetSection.Max,
                    result: out var activeAdditions))
            {
                additionKeys = activeAdditions.Select((i) => config._additions.Keys[i]).ToList();
                additionKeys.Sort();
            }

            int moveFromIndex = 0;
            int moveToIndex = 0;
            int additionIndex = 0;

            while ((moveFromIndex < moveFromKeys?.Length)
                || moveToIndex < moveToKeys?.Count
                || additionIndex < additionKeys?.Count)
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
                FileLocation? additionKey = null;
                if (additionIndex < additionKeys?.Count)
                {
                    additionKey = additionKeys[additionIndex];
                }

                if (moveFromKey != null
                    && (moveToKey == null || moveFromKey < moveToKey)
                    && (additionKey == null || moveFromKey < additionKey))
                {
                    // Delete out move
                    var moveRange = _sortedMoves[moveFromKey.Value];
                    var moveLocBufStart = new FileLocation(moveRange.Min - this._position - moveDeletions);
                    ContentLength len = new ContentLength((int)Math.Min(bufferEnd - moveLocBufStart, moveRange.Width));

                    // Copy to move cache
                    byte[] moveContents = new byte[moveRange.Width];
                    CopyOver(
                        sourceIndex: (int)moveLocBufStart,
                        destinationArray: moveContents,
                        destinationIndex: 0,
                        length: len);
                    if (len < moveRange.Width)
                    {
                        extraRead += this.source.Read(moveContents, len, (int)(moveRange.Width - len));
                    }

                    var moveLoc = config._moves[moveRange];
                    this._activeMoves.TryCreate(moveLoc).Add((moveRange, moveContents));
                    if (targetSection.Range.IsInRange(moveLoc))
                    {
                        moveToKeys.InsertSorted(moveLoc, replaceDuplicate: true);
                    }

                    // Delete moved snippet
                    if (len == moveRange.Width)
                    {
                        var sourceIndex = moveRange.Max + 1 - this._position - moveDeletions;
                        var moveAmount = (this.ExpandableBufferActive ? this._expandableBuffer.Count : this._buffer.Length) - sourceIndex;
                        var destination = moveRange.Min - this._position - moveDeletions;
                        if (ExpandableBufferActive)
                        {
                            for (int i = 0; i < moveAmount; i++)
                            {
                                this._expandableBuffer[i + destination] = this._expandableBuffer[i + sourceIndex];
                            }
                        }
                        else
                        {
                            Array.Copy(
                                sourceArray: this._buffer,
                                sourceIndex: sourceIndex,
                                destinationArray: this._buffer,
                                destinationIndex: destination,
                                length: moveAmount);
                        }
                        moveDeletions += len;
                    }
                    bufferEnd -= len;
                    moveFromIndex++;
                }
                else if (moveToKey != null
                    && (additionKey == null || moveToKey < additionKey))
                {
                    InitializeExpandableBuffer();
                    var movesToPaste = _activeMoves[moveToKey.Value];
                    var adjustedMoveLoc = new ContentLength(moveToKey.Value - _position);
                    var adjustedMoveLoc2 = new ContentLength(adjustedMoveLoc - moveDeletions);
                    foreach (var moveToPaste in movesToPaste.OrderBy((i) => config._sameLocMoves[moveToKey.Value].IndexOf(i.Section)))
                    {
                        _expandableBuffer.InsertRange(adjustedMoveLoc2, moveToPaste.Data);
                        moveDeletions -= moveToPaste.Data.Length;
                        bufferEnd += moveToPaste.Data.Length;
                    }

                    this._activeMoves.Remove(moveToKey.Value);
                    moveToIndex++;
                }
                else
                {
                    InitializeExpandableBuffer();
                    var data = config._additions[additionKey.Value];
                    var indexToAdd = (int)(additionKey.Value - _position - moveDeletions);
                    _expandableBuffer.InsertRange(indexToAdd, data);
                    moveDeletions -= data.Length;
                    bufferEnd += data.Length;
                    additionIndex++;
                }
            }
        }

        private void InitializeExpandableBuffer()
        {
            if (_expandableBuffer.Count == 0)
            {
                _expandableBuffer.AddRange(_buffer);
            }
        }

        private void CopyOver(int sourceIndex, byte[] destinationArray, int destinationIndex, int length)
        {
            if (ExpandableBufferActive)
            {
                _expandableBuffer.CopyTo(
                    index: sourceIndex,
                    array: destinationArray,
                    arrayIndex: 0,
                    count: length);
            }
            else
            {
                Array.Copy(
                    sourceArray: this._buffer,
                    sourceIndex: sourceIndex,
                    destinationArray: destinationArray,
                    destinationIndex: 0,
                    length: length);
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
