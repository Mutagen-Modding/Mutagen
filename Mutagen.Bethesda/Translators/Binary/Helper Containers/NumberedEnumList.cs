using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class NumberedEnumList<T> : IReadOnlyList<T>
        where T : struct, Enum
    {
        public int Amount { get; }
        public ReadOnlyMemorySlice<byte> Memory { get; }
        private const int Length = 4;

        public NumberedEnumList(ReadOnlyMemorySlice<byte> mem, int amount)
        {
            this.Amount = amount;
            this.Memory = mem;
        }

        public T this[int index]
        {
            get
            {
                if (index >= this.Amount || index < 0)
                {
                    throw new IndexOutOfRangeException();
                }
                if (((index + 1) * Length) < this.Memory.Length)
                {
                    return EnumExt.Parse<T>(BinaryPrimitives.ReadInt32LittleEndian(this.Memory.Span.Slice(index * Length)), default(T));
                }
                return default;
            }
        }

        public int Count => this.Amount;

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < this.Amount; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
