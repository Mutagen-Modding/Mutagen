using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public interface IGenderedItemGetter<out T> : IEnumerable<T>
    {
        T Male { get; }
        T Female { get; }
    }

    public class GenderedItem<T> : IGenderedItemGetter<T>
    {
        public T Male { get; set; }
        public T Female { get; set; }

        public GenderedItem(T male, T female)
        {
            this.Male = male;
            this.Female = female;
        }

        public IEnumerator<T> GetEnumerator()
        {
            yield return Male;
            yield return Female;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
