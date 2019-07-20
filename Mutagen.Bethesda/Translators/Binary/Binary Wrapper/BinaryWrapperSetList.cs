using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public abstract class BinaryWrapperSetList<T>
    {
        public static IReadOnlySetList<T> FactoryByArray(
            ReadOnlyMemorySlice<byte> mem,
            BinaryWrapperFactoryPackage package,
            UtilityTranslation.BinaryWrapperSpanFactory<T> getter,
            int[] locs)
        {
            return new BinaryWrapperListByLocationArray(
                mem,
                package,
                getter,
                locs);
        }

        public static IReadOnlySetList<T> FactoryByArray(
            ReadOnlyMemorySlice<byte> mem,
            BinaryWrapperFactoryPackage package,
            RecordTypeConverter recordTypeConverter,
            UtilityTranslation.BinaryWrapperSpanRecordFactory<T> getter,
            int[] locs)
        {
            return new BinaryWrapperRecordListByLocationArray(
                mem,
                package,
                recordTypeConverter,
                getter,
                locs);
        }

        public static IReadOnlySetList<T> FactoryByStartIndex(
            ReadOnlyMemorySlice<byte> mem,
            BinaryWrapperFactoryPackage package,
            int itemLength,
            UtilityTranslation.BinaryWrapperSpanFactory<T> getter)
        {
            return new BinaryWrapperListByStartIndex(
                mem,
                package,
                getter,
                itemLength);
        }

        public static IReadOnlySetList<T> FactoryByLazyParse(
            ReadOnlyMemorySlice<byte> mem,
            BinaryWrapperFactoryPackage package,
            UtilityTranslation.BinaryWrapperFactory<T> getter)
        {
            return new BinaryWrapperLazyList(
                mem,
                package,
                getter);
        }

        private class BinaryWrapperListByLocationArray : IReadOnlySetList<T>
        {
            private int[] _locations;
            BinaryWrapperFactoryPackage _package;
            private ReadOnlyMemorySlice<byte> _mem;
            private UtilityTranslation.BinaryWrapperSpanFactory<T> _getter;

            public BinaryWrapperListByLocationArray(
                ReadOnlyMemorySlice<byte> mem,
                BinaryWrapperFactoryPackage package,
                UtilityTranslation.BinaryWrapperSpanFactory<T> getter,
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

        private class BinaryWrapperRecordListByLocationArray : IReadOnlySetList<T>
        {
            private int[] _locations;
            private BinaryWrapperFactoryPackage _package;
            private ReadOnlyMemorySlice<byte> _mem;
            private UtilityTranslation.BinaryWrapperSpanRecordFactory<T> _getter;
            private RecordTypeConverter _recordTypeConverter;

            public BinaryWrapperRecordListByLocationArray(
                ReadOnlyMemorySlice<byte> mem,
                BinaryWrapperFactoryPackage package,
                RecordTypeConverter recordTypeConverter,
                UtilityTranslation.BinaryWrapperSpanRecordFactory<T> getter,
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

        public class BinaryWrapperListByStartIndex : IReadOnlySetList<T>
        {
            private int _itemLength;
            BinaryWrapperFactoryPackage _package;
            private ReadOnlyMemorySlice<byte> _mem;
            private UtilityTranslation.BinaryWrapperSpanFactory<T> _getter;

            public BinaryWrapperListByStartIndex(
                ReadOnlyMemorySlice<byte> mem,
                BinaryWrapperFactoryPackage package,
                UtilityTranslation.BinaryWrapperSpanFactory<T> getter,
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
                    return _getter(_mem.Slice(startIndex, startIndex + _itemLength), _package);
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

        public class BinaryWrapperLazyList : IReadOnlySetList<T>
        {
            private readonly Lazy<IReadOnlyList<T>> _list;
            private ReadOnlyMemorySlice<byte> _mem;
            private BinaryWrapperFactoryPackage _package;
            private UtilityTranslation.BinaryWrapperFactory<T> _getter;

            public BinaryWrapperLazyList(
                ReadOnlyMemorySlice<byte> mem,
                BinaryWrapperFactoryPackage package,
                UtilityTranslation.BinaryWrapperFactory<T> getter)
            {
                this._mem = mem;
                this._getter = getter;
                this._package = package;
                this._list = new Lazy<IReadOnlyList<T>>(ConstructList, isThreadSafe: true);
            }

            private IReadOnlyList<T> ConstructList()
            {
                var ret = new List<T>();
                using (var stream = new BinaryMemoryReadStream(_mem))
                {
                    while (!stream.Complete)
                    {
                        ret.Add(_getter(stream, _package));
                    }
                }
                return ret;
            }

            public T this[int index] => this._list.Value[index];

            public int Count => this._list.Value.Count;

            public bool HasBeenSet => true;

            public IEnumerator<T> GetEnumerator() => this._list.Value.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }
    }
}
