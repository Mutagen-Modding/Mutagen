using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class EmptySetList<T> : IReadOnlySetList<T>
    {
        public static readonly EmptySetList<T> Instance = new EmptySetList<T>();

        private EmptySetList()
        {
        }

        public T this[int index] => throw new IndexOutOfRangeException();

        public int Count => 0;

        public bool HasBeenSet => false;

        public IEnumerator<T> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
