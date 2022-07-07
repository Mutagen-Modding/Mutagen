using Noggog;
using System.Buffers.Binary;
using System.Collections;

namespace Mutagen.Bethesda.Plugins.Binary.Overlay;

internal sealed class BinaryOverlayNumberedList
{
    public static IReadOnlyList<T> FactoryForEnum<T>(
        ReadOnlyMemorySlice<byte> mem,
        int amount, 
        byte enumLength)
        where T : struct, Enum
    {
        return new NumberedEnumList<T>(mem, amount, enumLength);
    }
}

internal sealed class NumberedEnumList<T> : IReadOnlyList<T>
    where T : struct, Enum
{
    public int Amount { get; }
    public ReadOnlyMemorySlice<byte> Memory { get; }
    public byte EnumLength { get; }

    public NumberedEnumList(ReadOnlyMemorySlice<byte> mem, int amount, byte enumLength)
    {
        Amount = amount;
        Memory = mem;
        EnumLength = enumLength;
    }

    public T this[int index]
    {
        get
        {
            if (index >= Amount || index < 0)
            {
                throw new IndexOutOfRangeException();
            }
            if (((index + 1) * EnumLength) < Memory.Length)
            {
                return EnumExt.Parse<T>(BinaryPrimitives.ReadInt32LittleEndian(Memory.Span.Slice(index * EnumLength)), default(T));
            }
            return default;
        }
    }

    public int Count => Amount;

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Amount; i++)
        {
            yield return this[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}