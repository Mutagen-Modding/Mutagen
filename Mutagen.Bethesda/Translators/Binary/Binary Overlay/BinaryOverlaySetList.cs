using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public abstract class BinaryOverlaySetList<T>
    {
        public static IReadOnlySetList<T> FactoryByArray(
            ReadOnlyMemorySlice<byte> mem,
            BinaryOverlayFactoryPackage package,
            BinaryOverlay.SpanFactory<T> getter,
            int[] locs)
        {
            return new BinaryOverlayListByLocationArray(
                mem,
                package,
                getter,
                locs);
        }

        public static IReadOnlySetList<T> FactoryByArray(
            ReadOnlyMemorySlice<byte> mem,
            BinaryOverlayFactoryPackage package,
            RecordTypeConverter? recordTypeConverter,
            BinaryOverlay.SpanRecordFactory<T> getter,
            int[] locs)
        {
            return new BinaryOverlayRecordListByLocationArray(
                mem,
                package,
                recordTypeConverter,
                getter,
                locs);
        }

        public static IReadOnlySetList<T> FactoryByStartIndex(
            ReadOnlyMemorySlice<byte> mem,
            BinaryOverlayFactoryPackage package,
            int itemLength,
            BinaryOverlay.SpanFactory<T> getter)
        {
            return new BinaryOverlayListByStartIndex(
                mem,
                package,
                getter,
                itemLength);
        }

        public static IReadOnlySetList<T> FactoryByLazyParse(
            ReadOnlyMemorySlice<byte> mem,
            BinaryOverlayFactoryPackage package,
            BinaryOverlay.Factory<T> getter)
        {
            return new BinaryOverlayLazyList(
                mem,
                package,
                (m, p) =>
                {
                    var ret = new List<T>();
                    using (var stream = new BinaryMemoryReadStream(m))
                    {
                        while (!stream.Complete)
                        {
                            ret.Add(getter(stream, p));
                        }
                    }
                    return ret;
                });
        }

        public static IReadOnlySetList<T> FactoryByLazyParse(
            ReadOnlyMemorySlice<byte> mem,
            BinaryOverlayFactoryPackage package,
            Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, IReadOnlyList<T>> getter)
        {
            return new BinaryOverlayLazyList(
                mem,
                package,
                getter);
        }

        private class BinaryOverlayListByLocationArray : IReadOnlySetList<T>
        {
            private int[] _locations;
            BinaryOverlayFactoryPackage _package;
            private ReadOnlyMemorySlice<byte> _mem;
            private BinaryOverlay.SpanFactory<T> _getter;

            public BinaryOverlayListByLocationArray(
                ReadOnlyMemorySlice<byte> mem,
                BinaryOverlayFactoryPackage package,
                BinaryOverlay.SpanFactory<T> getter,
                int[] locs)
            {
                this._mem = mem;
                this._getter = getter;
                this._package = package;
                this._locations = locs;
            }

            public T this[int index] => _getter(_mem.Slice(_locations[index]), _package);

            public int Count => _locations.Length;

            public bool HasBeenSet => true;

            public IEnumerator<T> GetEnumerator()
            {
                for (int i = 0; i < _locations.Length; i++)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }

        private class BinaryOverlayRecordListByLocationArray : IReadOnlySetList<T>
        {
            private int[] _locations;
            private BinaryOverlayFactoryPackage _package;
            private ReadOnlyMemorySlice<byte> _mem;
            private BinaryOverlay.SpanRecordFactory<T> _getter;
            private RecordTypeConverter? _recordTypeConverter;

            public BinaryOverlayRecordListByLocationArray(
                ReadOnlyMemorySlice<byte> mem,
                BinaryOverlayFactoryPackage package,
                RecordTypeConverter? recordTypeConverter,
                BinaryOverlay.SpanRecordFactory<T> getter,
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

            public bool HasBeenSet => true;

            public IEnumerator<T> GetEnumerator()
            {
                for (int i = 0; i < _locations.Length; i++)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }

        public class BinaryOverlayListByStartIndex : IReadOnlySetList<T>
        {
            private int _itemLength;
            BinaryOverlayFactoryPackage _package;
            private ReadOnlyMemorySlice<byte> _mem;
            private BinaryOverlay.SpanFactory<T> _getter;

            public BinaryOverlayListByStartIndex(
                ReadOnlyMemorySlice<byte> mem,
                BinaryOverlayFactoryPackage package,
                BinaryOverlay.SpanFactory<T> getter,
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

            public bool HasBeenSet => true;

            public IEnumerator<T> GetEnumerator()
            {
                for (int i = 0; i < this.Count; i++)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }

        public class BinaryOverlayLazyList : IReadOnlySetList<T>
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

            public bool HasBeenSet => true;

            public IEnumerator<T> GetEnumerator() => this._list.Value.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }
    }
}
