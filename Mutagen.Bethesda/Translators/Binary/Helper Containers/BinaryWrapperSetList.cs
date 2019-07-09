using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class BinaryWrapperSetList<T> : IReadOnlySetList<T>
    {
        private int[] _locations;
        private ReadOnlyMemorySlice<byte> _mem;
        private Func<ReadOnlyMemorySlice<byte>, T> _getter;

        public BinaryWrapperSetList(
            ReadOnlyMemorySlice<byte> mem,
            Func<ReadOnlyMemorySlice<byte>, T> getter,
            int[] locs)
        {
            this._mem = mem;
            this._getter = getter;
            this._locations = locs;
        }

        public T this[int index] => _getter(_mem.Slice(_locations[index]));

        public int Count => _locations.Length;

        public bool HasBeenSet => _locations != null;

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _locations.Length; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
