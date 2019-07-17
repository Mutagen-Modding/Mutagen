using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class BinaryWrapperNumberedList
    {
        public static IReadOnlyList<T> FactoryForEnum<T>(
            ReadOnlyMemorySlice<byte> mem,
            int amount, 
            byte enumLength)
            where T : struct, Enum
        {
            return new NumberedEnumList<T>(mem, amount, enumLength);
        }

        public static IReadOnlyList<T> FactoryForLoqui<T>(
            ReadOnlyMemorySlice<byte> mem,
            int amount, 
            int length,
            BinaryWrapperFactoryPackage package,
            RecordTypeConverter recordTypeConverter,
            UtilityTranslation.BinaryWrapperStreamFactory<T> getter)
        {
            return new NumberedLoquiList<T>(mem, amount, length, package, recordTypeConverter, getter);
        }
    }

    public class NumberedLoquiList<T> : IReadOnlyList<T>
    {
        public int Amount { get; }
        public ReadOnlyMemorySlice<byte> Memory { get; }
        public int Length { get; }
        public BinaryWrapperFactoryPackage Package { get; }
        public UtilityTranslation.BinaryWrapperStreamFactory<T> Getter { get; }
        public RecordTypeConverter RecordTypeConverter { get; }

        public NumberedLoquiList(
            ReadOnlyMemorySlice<byte> mem,
            int amount,
            int length,
            BinaryWrapperFactoryPackage package,
            RecordTypeConverter recordTypeConverter,
            UtilityTranslation.BinaryWrapperStreamFactory<T> getter)
        {
            this.Amount = amount;
            this.Memory = mem;
            this.Length = length;
            this.Package = package;
            this.Getter = getter;
            this.RecordTypeConverter = recordTypeConverter;
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
                    return this.Getter(new BinaryMemoryReadStream(this.Memory.Slice(index * Length)), this.Package, this.RecordTypeConverter);
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

    public class NumberedEnumList<T> : IReadOnlyList<T>
        where T : struct, Enum
    {
        public int Amount { get; }
        public ReadOnlyMemorySlice<byte> Memory { get; }
        public byte EnumLength { get; }

        public NumberedEnumList(ReadOnlyMemorySlice<byte> mem, int amount, byte enumLength)
        {
            this.Amount = amount;
            this.Memory = mem;
            this.EnumLength = enumLength;
        }

        public T this[int index]
        {
            get
            {
                if (index >= this.Amount || index < 0)
                {
                    throw new IndexOutOfRangeException();
                }
                if (((index + 1) * this.EnumLength) < this.Memory.Length)
                {
                    return EnumExt.Parse<T>(BinaryPrimitives.ReadInt32LittleEndian(this.Memory.Span.Slice(index * this.EnumLength)), default(T));
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
