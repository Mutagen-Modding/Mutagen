using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Internals;

namespace Mutagen.Bethesda.Plugins.Binary.Overlay
{
    public abstract class BinaryOverlayList
    {
        public static IReadOnlyList<T> FactoryByArray<T>(
            ReadOnlyMemorySlice<byte> mem,
            BinaryOverlayFactoryPackage package,
            PluginBinaryOverlay.SpanFactory<T> getter,
            int[] locs)
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
            TypedParseParams? parseParams,
            PluginBinaryOverlay.SpanRecordFactory<T> getter,
            int[] locs)
        {
            return new BinaryOverlayRecordListByLocationArray<T>(
                mem,
                package,
                parseParams?.RecordTypeConverter,
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
            RecordType subrecordType,
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
                subrecordType,
                skipHeader: skipHeader);
        }

        public static IReadOnlyList<T> FactoryByCount<T>(
            OverlayStream stream,
            BinaryOverlayFactoryPackage package,
            int itemLength,
            int countLength,
            RecordType subrecordType,
            RecordType countType,
            PluginBinaryOverlay.SpanFactory<T> getter)
        {
            var mem = stream.RemainingMemory;
            var initialHeader = package.MetaData.Constants.SubrecordFrame(mem);
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
                if (!stream.TryReadSubrecordFrame(subrecordType, out var contentFrame))
                {
                    if (count == 0) return Array.Empty<T>();
                    throw new ArgumentException($"List with a non zero {initialHeader.RecordType} counter did not follow up with expected type: {subrecordType}");
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
            RecordType subrecordType,
            RecordType countType,
            PluginBinaryOverlay.SpanFactory<T> getter)
        {
            var mem = stream.RemainingMemory;
            var initialHeader = package.MetaData.Constants.SubrecordFrame(mem);
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
                var contentFrame = stream.ReadSubrecordFrame(subrecordType);
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
            TriggeringRecordCollection subrecordType,
            RecordType countType,
            TypedParseParams? parseParams,
            PluginBinaryOverlay.SpanRecordFactory<T> getter,
            bool skipHeader = true)
        {
            var mem = stream.RemainingMemory;
            var initialHeader = package.MetaData.Constants.SubrecordFrame(mem);
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
                    parseParams: parseParams,
                    getter: getter,
                    locs: PluginBinaryOverlay.ParseRecordLocationsByCount(
                        stream: stream,
                        count: count,
                        trigger: subrecordType,
                        constants: package.MetaData.Constants.SubConstants,
                        skipHeader: false));
            }
            else
            {
                return FactoryByArray(
                    mem: stream.RemainingMemory,
                    package: package,
                    parseParams: parseParams,
                    getter: getter,
                    locs: PluginBinaryOverlay.ParseRecordLocations(
                        stream: stream,
                        constants: package.MetaData.Constants.SubConstants,
                        triggers: subrecordType,
                        skipHeader: skipHeader,
                        parseParams: parseParams?.RecordTypeConverter));
            }
        }

        public static IReadOnlyList<T> FactoryByCountPerItem<T>(
            OverlayStream stream,
            BinaryOverlayFactoryPackage package,
            int countLength,
            RecordType subrecordType,
            RecordType countType,
            TypedParseParams? parseParams,
            PluginBinaryOverlay.SpanRecordFactory<T> getter,
            bool skipHeader = true)
        {
            var mem = stream.RemainingMemory;
            var initialHeader = package.MetaData.Constants.SubrecordFrame(mem);
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
                    parseParams: parseParams,
                    getter: getter,
                    locs: PluginBinaryOverlay.ParseRecordLocationsByCount(
                        stream: stream,
                        count: count,
                        trigger: subrecordType,
                        constants: package.MetaData.Constants.SubConstants,
                        skipHeader: false));
            }
            else
            {
                return FactoryByArray(
                    mem: stream.RemainingMemory,
                    package: package,
                    parseParams: parseParams,
                    getter: getter,
                    locs: PluginBinaryOverlay.ParseRecordLocations(
                        stream: stream,
                        constants: package.MetaData.Constants.SubConstants,
                        trigger: subrecordType,
                        skipHeader: skipHeader,
                        parseParams: parseParams?.RecordTypeConverter));
            }
        }

        public static IReadOnlyList<T> FactoryByCountPerItem<T>(
            OverlayStream stream,
            BinaryOverlayFactoryPackage package,
            int itemLength,
            int countLength,
            RecordType subrecordType,
            RecordType countType,
            PluginBinaryOverlay.SpanFactory<T> getter,
            bool skipHeader = true)
        {
            var mem = stream.RemainingMemory;
            var initialHeader = package.MetaData.Constants.SubrecordFrame(mem);
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
                    subrecordType: subrecordType,
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
                        trigger: subrecordType,
                        skipHeader: skipHeader));
            }
        }

        public static IReadOnlyList<T> FactoryByCount<T>(
            ReadOnlyMemorySlice<byte> mem,
            BinaryOverlayFactoryPackage package,
            TriggeringRecordCollection subrecordType,
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
                subrecordType);
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
            var count = countLength switch
            {
                1 => mem[0],
                4 => BinaryPrimitives.ReadUInt32LittleEndian(mem),
                _ => throw new NotImplementedException(),
            };
            if (((mem.Length - countLength) / itemLength) < count)
            {
                throw new ArgumentException("Item count and expected size did not match.");
            }
            return new BinaryOverlayListByStartIndex<T>(
                mem.Slice(countLength, checked((int)(count * itemLength))),
                package,
                getter,
                itemLength);
        }

        public static IReadOnlyList<string> FactoryByCountLength<T>(
            ReadOnlyMemorySlice<byte> mem,
            BinaryOverlayFactoryPackage package,
            byte countLength,
            PluginBinaryOverlay.SpanFactory<string> getter)
        {
            var count = countLength switch
            {
                4 => BinaryPrimitives.ReadUInt32LittleEndian(mem),
                _ => throw new NotImplementedException(),
            };
            int[] locs = new int[count];
            int loc = 0;
            for (int i = 0; i < count - 1; i++)
            {
                locs[i] = loc;
                var len = BinaryPrimitives.ReadUInt16LittleEndian(mem.Slice(loc));
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
            return FactoryByLazyParse(mem.Slice(countLength), package, getter);
        }

        private class BinaryOverlayListByLocationArray<T> : IReadOnlyList<T>
        {
            private int[] _locations;
            BinaryOverlayFactoryPackage _package;
            private ReadOnlyMemorySlice<byte> _mem;
            private PluginBinaryOverlay.SpanFactory<T> _getter;

            public BinaryOverlayListByLocationArray(
                ReadOnlyMemorySlice<byte> mem,
                BinaryOverlayFactoryPackage package,
                PluginBinaryOverlay.SpanFactory<T> getter,
                int[] locs)
            {
                this._mem = mem;
                this._getter = getter;
                this._package = package;
                this._locations = locs;
            }

            public T this[int index] => _getter(_mem.Slice(_locations[index]), _package);

            public int Count => _locations.Length;

            public IEnumerator<T> GetEnumerator()
            {
                for (int i = 0; i < _locations.Length; i++)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }

        private class BinaryOverlayRecordListByLocationArray<T> : IReadOnlyList<T>
        {
            private int[] _locations;
            private BinaryOverlayFactoryPackage _package;
            private ReadOnlyMemorySlice<byte> _mem;
            private PluginBinaryOverlay.SpanRecordFactory<T> _getter;
            private TypedParseParams? _recordTypeConverter;

            public BinaryOverlayRecordListByLocationArray(
                ReadOnlyMemorySlice<byte> mem,
                BinaryOverlayFactoryPackage package,
                RecordTypeConverter? recordTypeConverter,
                PluginBinaryOverlay.SpanRecordFactory<T> getter,
                int[] locs)
            {
                this._mem = mem;
                this._getter = getter;
                this._package = package;
                this._recordTypeConverter = recordTypeConverter;
                this._locations = locs;
            }

            public T this[int index] => _getter(_mem.Slice(_locations[index]), _package, _recordTypeConverter);

            public int Count => _locations.Length;

            public IEnumerator<T> GetEnumerator()
            {
                for (int i = 0; i < _locations.Length; i++)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
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
                this._mem = mem;
                this._package = package;
                this._getter = getter;
                this._itemLength = itemLength;
            }

            public T this[int index]
            {
                get
                {
                    var startIndex = index * _itemLength;
                    return _getter(_mem.Slice(startIndex, _itemLength), _package);
                }
            }

            public int Count => this._mem.Length / _itemLength;

            public IEnumerator<T> GetEnumerator()
            {
                for (int i = 0; i < this.Count; i++)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
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
                this._mem = mem;
                this._package = package;
                this._getter = getter;
                this._itemLength = itemLength;
                this._recordType = recordType;
                this._totalItemLength = itemLength + this._package.MetaData.Constants.SubConstants.HeaderLength;
                if (skipHeader)
                {
                    _sliceOffset = this._package.MetaData.Constants.SubConstants.HeaderLength;
                    _itemOffset = 0;
                }
                else
                {
                    _sliceOffset = 0;
                    _itemOffset = this._package.MetaData.Constants.SubConstants.HeaderLength;
                }
            }

            public T this[int index]
            {
                get
                {
                    var startIndex = index * this._totalItemLength;
                    var subMeta = _package.MetaData.Constants.Subrecord(_mem.Slice(startIndex));
                    if (subMeta.RecordType != this._recordType)
                    {
                        throw new ArgumentException($"Unexpected record type: {subMeta.RecordType} != {this._recordType}");
                    }
                    if (subMeta.ContentLength != this._itemLength)
                    {
                        throw new ArgumentException($"Unexpected record length: {subMeta.ContentLength} != {this._itemLength}");
                    }
                    return _getter(_mem.Slice(startIndex + _sliceOffset, _itemLength + _itemOffset), _package);
                }
            }

            public int Count => this._mem.Length / this._totalItemLength;

            public IEnumerator<T> GetEnumerator()
            {
                for (int i = 0; i < this.Count; i++)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }

        public class BinaryOverlayListByStartIndexWithRecordSet<T> : IReadOnlyList<T>
        {
            private readonly int _itemLength;
            private readonly BinaryOverlayFactoryPackage _package;
            private readonly ReadOnlyMemorySlice<byte> _mem;
            private readonly PluginBinaryOverlay.SpanFactory<T> _getter;
            private readonly TriggeringRecordCollection _recordTypes;
            private readonly int _totalItemLength;

            public BinaryOverlayListByStartIndexWithRecordSet(
                ReadOnlyMemorySlice<byte> mem,
                BinaryOverlayFactoryPackage package,
                PluginBinaryOverlay.SpanFactory<T> getter,
                int itemLength,
                TriggeringRecordCollection recordTypes)
            {
                this._mem = mem;
                this._package = package;
                this._getter = getter;
                this._itemLength = itemLength;
                this._recordTypes = recordTypes;
                this._totalItemLength = itemLength + this._package.MetaData.Constants.SubConstants.HeaderLength;
            }

            public T this[int index]
            {
                get
                {
                    var startIndex = index * this._totalItemLength;
                    var subMeta = _package.MetaData.Constants.Subrecord(_mem.Slice(startIndex));
                    if (!this._recordTypes.Contains(subMeta.RecordType))
                    {
                        throw new ArgumentException($"Unexpected record type: {subMeta.RecordType}");
                    }
                    if (subMeta.ContentLength != this._itemLength)
                    {
                        throw new ArgumentException($"Unexpected record length: {subMeta.ContentLength} != {this._itemLength}");
                    }
                    return _getter(_mem.Slice(startIndex + _package.MetaData.Constants.SubConstants.HeaderLength, _itemLength), _package);
                }
            }

            public int Count => this._mem.Length / this._totalItemLength;

            public IEnumerator<T> GetEnumerator()
            {
                for (int i = 0; i < this.Count; i++)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
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
                this._mem = mem;
                this._getter = getter;
                this._package = package;
                this._list = new Lazy<IReadOnlyList<T>>(ConstructList, isThreadSafe: true);
            }

            private IReadOnlyList<T> ConstructList()
            {
                return this._getter(_mem, _package);
            }

            public T this[int index] => this._list.Value[index];

            public int Count => this._list.Value.Count;

            public IEnumerator<T> GetEnumerator() => this._list.Value.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }
    }
}
