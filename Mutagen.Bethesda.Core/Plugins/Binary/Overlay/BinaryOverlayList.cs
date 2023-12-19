using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;
using System.Buffers.Binary;
using System.Collections;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Internals;

namespace Mutagen.Bethesda.Plugins.Binary.Overlay;

internal abstract class BinaryOverlayList
{
    public static IReadOnlyList<T> FactoryByArray<T>(
        ReadOnlyMemorySlice<byte> mem,
        BinaryOverlayFactoryPackage package,
        PluginBinaryOverlay.SpanFactory<T> getter,
        IReadOnlyList<int> locs)
    {
        return new BinaryOverlayListByLocationArray<T>(
            mem,
            package,
            getter,
            locs);
    }

    public static IReadOnlyList<T> FactoryByArray<T>(
        ReadOnlyMemorySlice<byte> mem,
        BinaryOverlayFactoryPackage package,
        TypedParseParams translationParams,
        PluginBinaryOverlay.SpanRecordFactory<T> getter,
        IReadOnlyList<int> locs)
    {
        return new BinaryOverlayRecordListByLocationArray<T>(
            mem,
            package,
            translationParams.RecordTypeConverter,
            getter,
            locs);
    }

    public static IReadOnlyList<T> FactoryByStartIndex<T>(
        ReadOnlyMemorySlice<byte> mem,
        BinaryOverlayFactoryPackage package,
        int itemLength,
        PluginBinaryOverlay.SpanFactory<T> getter)
    {
        return new BinaryOverlayListByStartIndex<T>(
            mem,
            package,
            getter,
            itemLength);
    }

    public static IReadOnlyList<T> FactoryByCount<T>(
        ReadOnlyMemorySlice<byte> mem,
        BinaryOverlayFactoryPackage package,
        RecordType trigger,
        int itemLength,
        uint count,
        PluginBinaryOverlay.SpanFactory<T> getter,
        bool skipHeader = true)
    {
        if ((mem.Length / (itemLength + package.MetaData.Constants.SubConstants.HeaderLength)) != count)
        {
            throw new ArgumentException("Item count and expected size did not match.");
        }
        return new BinaryOverlayListByStartIndexWithRecord<T>(
            mem,
            package,
            getter,
            itemLength,
            trigger,
            skipHeader: skipHeader);
    }

    public static IReadOnlyList<T> FactoryByCount<T>(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        int itemLength,
        int countLength,
        RecordType trigger,
        RecordType countType,
        PluginBinaryOverlay.SpanFactory<T> getter)
    {
        var mem = stream.RemainingMemory;
        var initialHeader = package.MetaData.Constants.Subrecord(mem);
        var recType = initialHeader.RecordType;
        if (recType == countType)
        {
            var count = countLength switch
            {
                1 => initialHeader.Content[0],
                2 => (int)BinaryPrimitives.ReadUInt16LittleEndian(initialHeader.Content),
                4 => checked((int)BinaryPrimitives.ReadUInt32LittleEndian(initialHeader.Content)),
                _ => throw new NotImplementedException(),
            };
            stream.Position += initialHeader.TotalLength;
            if (!stream.TryReadSubrecord(trigger, out var contentFrame))
            {
                if (count == 0) return Array.Empty<T>();
                throw new ArgumentException($"List with a non zero {initialHeader.RecordType} counter did not follow up with expected type: {trigger}");
            }
            return new BinaryOverlayListByStartIndex<T>(
                contentFrame.Content,
                package,
                getter,
                itemLength);
        }
        else
        {
            return FactoryByStartIndex(
                mem: stream.RemainingMemory,
                package: package,
                getter: getter,
                itemLength: itemLength);
        }
    }

    public static IReadOnlyList<T>? FactoryByCountNullIfZero<T>(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        int itemLength,
        int countLength,
        RecordType trigger,
        RecordType countType,
        PluginBinaryOverlay.SpanFactory<T> getter)
    {
        var mem = stream.RemainingMemory;
        var initialHeader = package.MetaData.Constants.Subrecord(mem);
        var recType = initialHeader.RecordType;
        if (recType == countType)
        {
            var count = countLength switch
            {
                1 => initialHeader.Content[0],
                2 => (int)BinaryPrimitives.ReadUInt16LittleEndian(initialHeader.Content),
                4 => checked((int)BinaryPrimitives.ReadUInt32LittleEndian(initialHeader.Content)),
                _ => throw new NotImplementedException(),
            };
            stream.Position += initialHeader.TotalLength;
            if (count == 0) return null;
            var contentFrame = stream.ReadSubrecord(trigger);
            return new BinaryOverlayListByStartIndex<T>(
                contentFrame.Content,
                package,
                getter,
                itemLength);
        }
        else
        {
            return FactoryByStartIndex(
                mem: stream.RemainingMemory,
                package: package,
                getter: getter,
                itemLength: itemLength);
        }
    }

    public static IReadOnlyList<T> FactoryByCountPerItem<T>(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        int countLength,
        RecordTriggerSpecs trigger,
        RecordType countType,
        TypedParseParams translationParams,
        PluginBinaryOverlay.SpanRecordFactory<T> getter,
        bool skipHeader = true)
    {
        var mem = stream.RemainingMemory;
        var initialHeader = package.MetaData.Constants.Subrecord(mem);
        var recType = initialHeader.RecordType;
        if (recType == countType)
        {
            var count = countLength switch
            {
                1 => initialHeader.Content[0],
                2 => BinaryPrimitives.ReadUInt16LittleEndian(initialHeader.Content),
                4 => BinaryPrimitives.ReadUInt32LittleEndian(initialHeader.Content),
                _ => throw new NotImplementedException(),
            };
            stream.Position += initialHeader.TotalLength;
            return FactoryByArray(
                mem: stream.RemainingMemory,
                package: package,
                translationParams: translationParams,
                getter: getter,
                locs: PluginBinaryOverlay.ParseRecordLocationsByCount(
                    stream: stream,
                    count: count,
                    trigger: trigger,
                    constants: package.MetaData.Constants.SubConstants,
                    skipHeader: false));
        }
        else
        {
            return FactoryByArray(
                mem: stream.RemainingMemory,
                package: package,
                translationParams: translationParams,
                getter: getter,
                locs: PluginBinaryOverlay.ParseRecordLocations(
                    stream: stream,
                    constants: package.MetaData.Constants.SubConstants,
                    trigger: trigger,
                    skipHeader: skipHeader,
                    translationParams: translationParams.RecordTypeConverter));
        }
    }

    public static IReadOnlyList<T> FactoryByCountPerItem<T>(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        int countLength,
        RecordType trigger,
        RecordType countType,
        TypedParseParams translationParams,
        PluginBinaryOverlay.SpanRecordFactory<T> getter,
        bool skipHeader = true)
    {
        var mem = stream.RemainingMemory;
        var initialHeader = package.MetaData.Constants.Subrecord(mem);
        var recType = initialHeader.RecordType;
        if (recType == countType)
        {
            var count = countLength switch
            {
                1 => initialHeader.Content[0],
                2 => BinaryPrimitives.ReadUInt16LittleEndian(initialHeader.Content),
                4 => BinaryPrimitives.ReadUInt32LittleEndian(initialHeader.Content),
                _ => throw new NotImplementedException(),
            };
            stream.Position += initialHeader.TotalLength;
            return FactoryByArray(
                mem: stream.RemainingMemory,
                package: package,
                translationParams: translationParams,
                getter: getter,
                locs: PluginBinaryOverlay.ParseRecordLocationsByCount(
                    stream: stream,
                    count: count,
                    trigger: trigger,
                    constants: package.MetaData.Constants.SubConstants,
                    skipHeader: false));
        }
        else
        {
            return FactoryByArray(
                mem: stream.RemainingMemory,
                package: package,
                translationParams: translationParams,
                getter: getter,
                locs: PluginBinaryOverlay.ParseRecordLocations(
                    stream: stream,
                    constants: package.MetaData.Constants.SubConstants,
                    trigger: trigger,
                    skipHeader: skipHeader,
                    translationParams: translationParams.RecordTypeConverter));
        }
    }

    public static IReadOnlyList<T> FactoryByCountPerItem<T>(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        int itemLength,
        int countLength,
        RecordType trigger,
        RecordType countType,
        PluginBinaryOverlay.SpanFactory<T> getter,
        bool skipHeader = true)
    {
        var mem = stream.RemainingMemory;
        var initialHeader = package.MetaData.Constants.Subrecord(mem);
        var recType = initialHeader.RecordType;
        if (recType == countType)
        {
            var count = countLength switch
            {
                1 => initialHeader.Content[0],
                2 => BinaryPrimitives.ReadUInt16LittleEndian(initialHeader.Content),
                4 => BinaryPrimitives.ReadUInt32LittleEndian(initialHeader.Content),
                _ => throw new NotImplementedException(),
            };
            var countLen = initialHeader.TotalLength;
            var contentLen = checked((int)((itemLength + package.MetaData.Constants.SubConstants.HeaderLength) * count));
            stream.Position += countLen + contentLen;
            return FactoryByCount(
                mem.Slice(countLen, contentLen),
                package: package,
                itemLength: itemLength,
                trigger: trigger,
                count: count,
                getter: getter,
                skipHeader: skipHeader);
        }
        else
        {
            return FactoryByArray(
                mem: stream.RemainingMemory,
                package: package,
                getter: getter,
                locs: PluginBinaryOverlay.ParseRecordLocations(
                    stream: stream,
                    constants: package.MetaData.Constants.SubConstants,
                    trigger: trigger,
                    skipHeader: skipHeader));
        }
    }

    public static IReadOnlyList<T> FactoryByCount<T>(
        ReadOnlyMemorySlice<byte> mem,
        BinaryOverlayFactoryPackage package,
        RecordTriggerSpecs trigger,
        int itemLength,
        uint count,
        PluginBinaryOverlay.SpanFactory<T> getter)
    {
        if ((mem.Length / (itemLength + package.MetaData.Constants.SubConstants.HeaderLength)) != count)
        {
            throw new ArgumentException("Item count and expected size did not match.");
        }
        return new BinaryOverlayListByStartIndexWithRecordSet<T>(
            mem,
            package,
            getter,
            itemLength,
            trigger.TriggeringRecordTypes);
    }

    public static IReadOnlyList<T> FactoryByCount<T>(
        ReadOnlyMemorySlice<byte> mem,
        BinaryOverlayFactoryPackage package,
        int itemLength,
        uint count,
        PluginBinaryOverlay.SpanFactory<T> getter)
    {
        if ((mem.Length / itemLength) != count)
        {
            throw new ArgumentException("Item count and expected size did not match.");
        }
        return new BinaryOverlayListByStartIndex<T>(
            mem,
            package,
            getter,
            itemLength);
    }

    public static IReadOnlyList<T> FactoryByCountLength<T>(
        ReadOnlyMemorySlice<byte> mem,
        BinaryOverlayFactoryPackage package,
        int itemLength,
        byte countLength,
        PluginBinaryOverlay.SpanFactory<T> getter)
    {
        return FactoryByCountLength<T>(
            mem,
            package,
            itemLength: itemLength,
            countLength: countLength,
            expectedLengthLength: 0,
            getter: getter);
    }

    public static IReadOnlyList<T> FactoryByCountLength<T>(
        ReadOnlyMemorySlice<byte> mem,
        BinaryOverlayFactoryPackage package,
        int itemLength,
        byte countLength,
        byte expectedLengthLength,
        PluginBinaryOverlay.SpanFactory<T> getter)
    {
        var count = countLength switch
        {
            1 => mem[0],
            2 => BinaryPrimitives.ReadUInt16LittleEndian(mem),
            4 => BinaryPrimitives.ReadUInt32LittleEndian(mem),
            _ => throw new NotImplementedException(),
        };
        if (((mem.Length - countLength) / itemLength) < count)
        {
            throw new ArgumentException("Item count and expected size did not match.");
        }
        return new BinaryOverlayListByStartIndex<T>(
            mem.Slice(countLength + expectedLengthLength, checked((int)(count * itemLength))),
            package,
            getter,
            itemLength);
    }

    public static IReadOnlyList<string> FactoryByCountLengthWithItemLength<T>(
        ReadOnlyMemorySlice<byte> mem,
        BinaryOverlayFactoryPackage package,
        byte countLength,
        byte itemLengthLength,
        PluginBinaryOverlay.SpanFactory<string> getter)
    {
        var count = countLength switch
        {
            4 => BinaryPrimitives.ReadUInt32LittleEndian(mem),
            2 => BinaryPrimitives.ReadUInt16LittleEndian(mem),
            1 => mem[0],
            _ => throw new NotImplementedException(),
        };
        Func<ReadOnlyMemorySlice<byte>, int> itemLenGetter = itemLengthLength switch
        {
            4 => (mem) => checked((int)BinaryPrimitives.ReadUInt32LittleEndian(mem)),
            2 => (mem) => BinaryPrimitives.ReadUInt16LittleEndian(mem),
            1 => (mem) => mem[0],
            _ => throw new NotImplementedException(),
        };
        int[] locs = new int[count];
        int loc = 0;
        for (int i = 0; i < count - 1; i++)
        {
            locs[i] = loc;
            var len = itemLenGetter(mem.Slice(loc));
            loc += len + 2;
        }
        return FactoryByArray(
            mem.Slice(countLength),
            package,
            getter,
            locs);
    }

    public static IReadOnlyList<T> FactoryByCount<T>(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        uint count,
        PluginBinaryOverlay.StreamFactory<T> getter)
    {
        var ret = new List<T>(checked((int)count));
        for (uint i = 0; i < count; i++)
        {
            ret.Add(getter(stream, package));
        }
        return ret;
    }

    public static IReadOnlyList<T> FactoryByCountLength<T>(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        int itemLength,
        uint count,
        PluginBinaryOverlay.SpanFactory<T> getter)
    {
        var ret = new List<T>(checked((int)count));
        for (uint i = 0; i < count; i++)
        {
            ret.Add(getter(stream.ReadMemory(itemLength), package));
        }
        return ret;
    }

    public static IReadOnlyList<T> FactoryByLazyParse<T>(
        ReadOnlyMemorySlice<byte> mem,
        BinaryOverlayFactoryPackage package,
        PluginBinaryOverlay.Factory<T> getter)
    {
        return new BinaryOverlayLazyList<T>(
            mem,
            package,
            (m, p) =>
            {
                var ret = new List<T>();
                using (var stream = new OverlayStream(m, package))
                {
                    while (!stream.Complete)
                    {
                        ret.Add(getter(stream, p));
                    }
                }
                return ret;
            });
    }

    public static IReadOnlyList<T> FactoryByLazyParse<T>(
        ReadOnlyMemorySlice<byte> mem,
        BinaryOverlayFactoryPackage package,
        Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, IReadOnlyList<T>> getter)
    {
        return new BinaryOverlayLazyList<T>(
            mem,
            package,
            getter);
    }

    public static IReadOnlyList<T> FactoryByLazyParse<T>(
        ReadOnlyMemorySlice<byte> mem,
        BinaryOverlayFactoryPackage package,
        byte countLength,
        PluginBinaryOverlay.Factory<T> getter)
    {
        // Don't care about count, at the moment
        var contentMem = mem.Slice(countLength);
        return FactoryByLazyParse(contentMem, package, getter);
    }

    private class BinaryOverlayListByLocationArray<T> : IReadOnlyList<T>
    {
        private IReadOnlyList<int> _locations;
        BinaryOverlayFactoryPackage _package;
        private ReadOnlyMemorySlice<byte> _mem;
        private PluginBinaryOverlay.SpanFactory<T> _getter;

        public BinaryOverlayListByLocationArray(
            ReadOnlyMemorySlice<byte> mem,
            BinaryOverlayFactoryPackage package,
            PluginBinaryOverlay.SpanFactory<T> getter,
            IReadOnlyList<int> locs)
        {
            _mem = mem;
            _getter = getter;
            _package = package;
            _locations = locs;
        }

        public T this[int index] => _getter(_mem.Slice(_locations[index]), _package);

        public int Count => _locations.Count;

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _locations.Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private class BinaryOverlayRecordListByLocationArray<T> : IReadOnlyList<T>
    {
        private IReadOnlyList<int> _locations;
        private BinaryOverlayFactoryPackage _package;
        private ReadOnlyMemorySlice<byte> _mem;
        private PluginBinaryOverlay.SpanRecordFactory<T> _getter;
        private RecordTypeConverter? _recordTypeConverter;

        public BinaryOverlayRecordListByLocationArray(
            ReadOnlyMemorySlice<byte> mem,
            BinaryOverlayFactoryPackage package,
            RecordTypeConverter? recordTypeConverter,
            PluginBinaryOverlay.SpanRecordFactory<T> getter,
            IReadOnlyList<int> locs)
        {
            _mem = mem;
            _getter = getter;
            _package = package;
            _recordTypeConverter = recordTypeConverter;
            _locations = locs;
        }

        public T this[int index] => _getter(_mem.Slice(_locations[index]), _package, _recordTypeConverter);

        public int Count => _locations.Count;

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _locations.Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class BinaryOverlayListByStartIndex<T> : IReadOnlyList<T>
    {
        private readonly int _itemLength;
        private readonly BinaryOverlayFactoryPackage _package;
        private readonly ReadOnlyMemorySlice<byte> _mem;
        private readonly PluginBinaryOverlay.SpanFactory<T> _getter;

        public BinaryOverlayListByStartIndex(
            ReadOnlyMemorySlice<byte> mem,
            BinaryOverlayFactoryPackage package,
            PluginBinaryOverlay.SpanFactory<T> getter,
            int itemLength)
        {
            _mem = mem;
            _package = package;
            _getter = getter;
            _itemLength = itemLength;
            Count = _mem.Length / _itemLength;
            if (_mem.Length % _itemLength != 0)
            {
                Count++;
            }
        }

        public T this[int index]
        {
            get
            {
                var startIndex = index * _itemLength;
                return _getter(_mem.Slice(startIndex), _package);
            }
        }

        public int Count { get; }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class BinaryOverlayListByStartIndexWithRecord<T> : IReadOnlyList<T>
    {
        private readonly int _itemLength;
        private readonly BinaryOverlayFactoryPackage _package;
        private readonly ReadOnlyMemorySlice<byte> _mem;
        private readonly PluginBinaryOverlay.SpanFactory<T> _getter;
        private readonly RecordType _recordType;
        private readonly int _totalItemLength;
        private readonly int _sliceOffset;
        private readonly int _itemOffset;

        public BinaryOverlayListByStartIndexWithRecord(
            ReadOnlyMemorySlice<byte> mem,
            BinaryOverlayFactoryPackage package,
            PluginBinaryOverlay.SpanFactory<T> getter,
            int itemLength,
            RecordType recordType,
            bool skipHeader)
        {
            _mem = mem;
            _package = package;
            _getter = getter;
            _itemLength = itemLength;
            _recordType = recordType;
            _totalItemLength = itemLength + _package.MetaData.Constants.SubConstants.HeaderLength;
            if (skipHeader)
            {
                _sliceOffset = _package.MetaData.Constants.SubConstants.HeaderLength;
                _itemOffset = 0;
            }
            else
            {
                _sliceOffset = 0;
                _itemOffset = _package.MetaData.Constants.SubConstants.HeaderLength;
            }
        }

        public T this[int index]
        {
            get
            {
                var startIndex = index * _totalItemLength;
                var subMeta = _package.MetaData.Constants.SubrecordHeader(_mem.Slice(startIndex));
                if (subMeta.RecordType != _recordType)
                {
                    throw new ArgumentException($"Unexpected record type: {subMeta.RecordType} != {_recordType}");
                }
                if (subMeta.ContentLength != _itemLength)
                {
                    throw new ArgumentException($"Unexpected record length: {subMeta.ContentLength} != {_itemLength}");
                }
                return _getter(_mem.Slice(startIndex + _sliceOffset, _itemLength + _itemOffset), _package);
            }
        }

        public int Count => _mem.Length / _totalItemLength;

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class BinaryOverlayListByStartIndexWithRecordSet<T> : IReadOnlyList<T>
    {
        private readonly int _itemLength;
        private readonly BinaryOverlayFactoryPackage _package;
        private readonly ReadOnlyMemorySlice<byte> _mem;
        private readonly PluginBinaryOverlay.SpanFactory<T> _getter;
        private readonly IReadOnlyRecordCollection _recordTypes;
        private readonly int _totalItemLength;

        public BinaryOverlayListByStartIndexWithRecordSet(
            ReadOnlyMemorySlice<byte> mem,
            BinaryOverlayFactoryPackage package,
            PluginBinaryOverlay.SpanFactory<T> getter,
            int itemLength,
            IReadOnlyRecordCollection recordTypes)
        {
            _mem = mem;
            _package = package;
            _getter = getter;
            _itemLength = itemLength;
            _recordTypes = recordTypes;
            _totalItemLength = itemLength + _package.MetaData.Constants.SubConstants.HeaderLength;
        }

        public T this[int index]
        {
            get
            {
                var startIndex = index * _totalItemLength;
                var subMeta = _package.MetaData.Constants.SubrecordHeader(_mem.Slice(startIndex));
                if (!_recordTypes.Contains(subMeta.RecordType))
                {
                    throw new ArgumentException($"Unexpected record type: {subMeta.RecordType}");
                }
                if (subMeta.ContentLength != _itemLength)
                {
                    throw new ArgumentException($"Unexpected record length: {subMeta.ContentLength} != {_itemLength}");
                }
                return _getter(_mem.Slice(startIndex + _package.MetaData.Constants.SubConstants.HeaderLength, _itemLength), _package);
            }
        }

        public int Count => _mem.Length / _totalItemLength;

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class BinaryOverlayLazyList<T> : IReadOnlyList<T>
    {
        private readonly Lazy<IReadOnlyList<T>> _list;
        private readonly ReadOnlyMemorySlice<byte> _mem;
        private readonly BinaryOverlayFactoryPackage _package;
        private readonly Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, IReadOnlyList<T>> _getter;

        public BinaryOverlayLazyList(
            ReadOnlyMemorySlice<byte> mem,
            BinaryOverlayFactoryPackage package,
            Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, IReadOnlyList<T>> getter)
        {
            _mem = mem;
            _getter = getter;
            _package = package;
            _list = new Lazy<IReadOnlyList<T>>(ConstructList, isThreadSafe: true);
        }

        private IReadOnlyList<T> ConstructList()
        {
            return _getter(_mem, _package);
        }

        public T this[int index] => _list.Value[index];

        public int Count => _list.Value.Count;

        public IEnumerator<T> GetEnumerator() => _list.Value.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}