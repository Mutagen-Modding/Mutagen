using Noggog;

namespace Mutagen.Bethesda.Tests;

public class BinaryFileProcessor : Stream
{
    #region Config
    public class Config
    {
        internal SortedList<long, byte[]> _additions = new();
        internal SortedList<long, byte> _substitutions = new();
        internal Dictionary<RangeInt64, long> _moves = new();
        internal Dictionary<long, List<RangeInt64>> _sameLocMoves = new();
        public bool HasProcessing => _moves?.Count > 0
                                     || _substitutions?.Count > 0;
    }

    public class ConfigConstructor
    {
        internal Dictionary<long, byte[]> _additions = new();
        internal Dictionary<long, byte> _substitutions = new();
        internal RangeCollection _moveRanges = new();
        internal Dictionary<RangeInt64, long> _moves = new();
        internal Dictionary<long, List<RangeInt64>> _sameLocMoves = new();
        public bool HasProcessing => _moves?.Count > 0
                                     || _substitutions?.Count > 0;

        public Config GetConfig()
        {
            return new Config()
            {
                _substitutions = new SortedList<long, byte>(_substitutions),
                _additions = new SortedList<long, byte[]>(_additions),
                _moves = _moves,
                _sameLocMoves = _sameLocMoves,
            };
        }

        public void SetSubstitution(long loc, byte sub)
        {
            lock (_substitutions)
            {
                _substitutions[loc] = sub;
            }
        }

        public void SetSubstitution(long loc, byte[] sub)
        {
            lock (_substitutions)
            {
                for (long i = 0; i < sub.Length; i++)
                {
                    _substitutions[loc + i] = sub[i];
                }
            }
        }

        public void SetAddition(long loc, byte[] addition)
        {
            lock (_additions)
            {
                _additions[loc] = addition;
            }
        }

        public void SetRemove(RangeInt64 section)
        {
            SetMove(
                section: section,
                loc: long.MaxValue);
        }

        public void SetMove(RangeInt64 section, long loc)
        {
            if (section.Width == 0) return;
            if (loc == section.Min) return;
            if (loc <= section.Max)
            {
                throw new NotImplementedException("Cannot move a section earlier in the stream, within the move itself, or into the same spot");
            }
            lock (_moves)
            {
                if (_moveRanges.Collides(section))
                {
                    throw new ArgumentException("Can not have colliding moves.");
                }
                _moveRanges.Add(section);
                _moves[section] = loc;
                _sameLocMoves.GetOrAdd(loc).Add(section);
                if (_moveRanges.IsEncapsulated(loc))
                {
                    throw new ArgumentException($"Cannot move to a section that is marked to be moved: {loc}");
                }
            }
        }
    }
    #endregion

    private Config config;
    private readonly Stream source;
    private readonly byte[] _buffer;
    private readonly List<byte> _expandableBuffer = new();
    private int bufferPos;
    private int bufferEnd;
    private int extraRead;
    private long _position;
    private bool done;
    private SortedList<long, List<(RangeInt64 Section, byte[] Data)>> _activeMoves;
    private SortedList<long, RangeInt64> _sortedMoves;
    private bool ExpandableBufferActive => _expandableBuffer.Count > 0;
    public bool HasProcessing { get; private set; }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => source.Length;

    public override long Position
    {
        get => _position + bufferPos;
        set => SetPosition(value);
    }

    public BinaryFileProcessor(Stream source, Config config, int bufferLen = 4096)
    {
        this.source = source;
        this.config = config;
        if (this.config._moves != null)
        {
            _activeMoves = new SortedList<long, List<(RangeInt64 Section, byte[] Data)>>();
            _sortedMoves = new SortedList<long, RangeInt64>();
            _sortedMoves.Add(
                this.config._moves.Select((m) => new KeyValuePair<long, RangeInt64>(m.Key.Min, m.Key)));
        }
        _buffer = new byte[bufferLen];
        HasProcessing = this.config.HasProcessing;
    }

    protected override void Dispose(bool disposing)
    {
        source.Dispose();
        base.Dispose(disposing);
    }

    private void GetNextBuffer()
    {
        if (bufferPos != bufferEnd)
        {
            throw new ArgumentException("Get next buffer called when current buffer was not fully used.");
        }
        if (bufferEnd != 0
            || _position != 0)
        {
            _position += _buffer.Length + extraRead;
        }
        bufferPos = 0;
        var prevExtraRead = extraRead;
        extraRead = 0;
        bufferEnd = source.Read(_buffer, 0, _buffer.Length);
        _expandableBuffer.Clear();
        if (bufferEnd == 0)
        {
            done = true;
            return;
        }

        RangeInt64 targetSection = new RangeInt64(_position + bufferPos, _position + bufferEnd - 1);
            
        DoSubstitutions(targetSection);
        DoMoves(prevExtraRead, targetSection);
    }

    private void DoSubstitutions(RangeInt64 targetSection)
    {
        if (config._substitutions == null) return;
        if (!config._substitutions.TryGetEncapsulatedIndices<long, byte>(
                lowerKey: _position + bufferPos,
                higherKey: _position + bufferEnd - 1,
                result: out var substitutionRange)) return;
        for (int i = substitutionRange.Min; i <= substitutionRange.Max; i++)
        {
            var targetPos = config._substitutions.Keys[i];
            var bufferPos = targetPos - _position;
            var sub = config._substitutions.Values[i];
            _buffer[bufferPos] = sub;
        }
    }

    private void DoMoves(int prevExtraRead, RangeInt64 targetSection)
    {
        if (config._moves == null) return;
        int moveDeletions = 0;

        long[] moveFromKeys = null;
        List<long> moveToKeys = new List<long>();
        List<long> additionKeys = null;

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
            long? moveFromKey = null;
            if (moveFromIndex < moveFromKeys?.Length)
            {
                moveFromKey = moveFromKeys[moveFromIndex];
            }
            long? moveToKey = null;
            if (moveToIndex < moveToKeys.Count)
            {
                moveToKey = moveToKeys[moveToIndex];
            }
            long? additionKey = null;
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
                var moveLocBufStart = moveRange.Min - _position - moveDeletions;
                int len = (int)Math.Min(bufferEnd - moveLocBufStart, moveRange.Width);

                // Copy to move cache
                byte[] moveContents = new byte[moveRange.Width];
                CopyOver(
                    sourceIndex: (int)moveLocBufStart,
                    destinationArray: moveContents,
                    destinationIndex: 0,
                    length: len);
                if (len < moveRange.Width)
                {
                    extraRead += source.Read(moveContents, len, (int)(moveRange.Width - len));
                }

                var moveLoc = config._moves[moveRange];
                _activeMoves.GetOrAdd(moveLoc).Add((moveRange, moveContents));
                if (targetSection.IsInRange(moveLoc))
                {
                    PreSortedListExt.Set<long>(moveToKeys, moveLoc);
                }

                // Delete moved snippet
                if (len == moveRange.Width)
                {
                    var sourceIndex = checked((int)(moveRange.Max + 1 - _position - moveDeletions));
                    var moveAmount = (ExpandableBufferActive ? _expandableBuffer.Count : _buffer.Length) - sourceIndex;
                    int destination = checked((int)(moveRange.Min - _position - moveDeletions));
                    if (ExpandableBufferActive)
                    {
                        for (int i = 0; i < moveAmount; i++)
                        {
                            _expandableBuffer[i + destination] = _expandableBuffer[i + sourceIndex];
                        }
                    }
                    else
                    {
                        Array.Copy(
                            sourceArray: _buffer,
                            sourceIndex: sourceIndex,
                            destinationArray: _buffer,
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
                var adjustedMoveLoc = moveToKey.Value - _position;
                int adjustedMoveLoc2 = checked((int)adjustedMoveLoc - moveDeletions);
                foreach (var moveToPaste in movesToPaste.OrderBy((i) => config._sameLocMoves[moveToKey.Value].IndexOf(i.Section)))
                {
                    _expandableBuffer.InsertRange(adjustedMoveLoc2, moveToPaste.Data);
                    moveDeletions -= moveToPaste.Data.Length;
                    bufferEnd += moveToPaste.Data.Length;
                }

                _activeMoves.Remove(moveToKey.Value);
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
                sourceArray: _buffer,
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
            var ret = source.Read(buffer, offset, count);
            _position += ret;
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
                        sourceArray: _buffer,
                        sourceIndex: bufferPos,
                        destinationArray: buffer,
                        destinationIndex: offset,
                        length: toRead);
                }
                else
                {
                    for (int i = 0; i < toRead; i++)
                    {
                        buffer[i + offset] = _expandableBuffer[bufferPos + i];
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
        if (pos == Position) return;
        if (pos < Position)
        {
            throw new NotImplementedException("Cannot move back in position");
        }
        var diff = pos - Position;
        if (diff > int.MaxValue)
        {
            throw new NotImplementedException("Need to upgrade to move large positions");
        }
        byte[] trash = new byte[diff];
        Read(trash, offset: 0, count: (int)diff);
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