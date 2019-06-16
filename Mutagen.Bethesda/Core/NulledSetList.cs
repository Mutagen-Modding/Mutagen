using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Internals
{
    public class NulledSetList<T> : IReadOnlySetList<T>
    {
        public IReadOnlyList<T> WrappedList;

        public T this[int index] => WrappedList[index];

        public int Count => WrappedList?.Count ?? 0;

        public bool HasBeenSet => WrappedList != null;

        public IEnumerator<T> GetEnumerator()
        {
            if (this.WrappedList == null) yield break;
            foreach (var item in this.WrappedList)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
