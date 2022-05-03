using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;
using System.Buffers.Binary;
using System.Collections;

namespace Mutagen.Bethesda.Plugins.Binary.Overlay;

internal class BinaryOverlayNumberedList
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
        BinaryOverlayFactoryPackage package,
        RecordTypeConverter recordTypeConverter,
        PluginBinaryOverlay.ConverterFactory<T> getter)
    {
        return new NumberedLoquiList<T>(mem, amount, length, package, recordTypeConverter, getter);
    }
}

internal class NumberedLoquiList<T> : IReadOnlyList<T>
{
    public int Amount { get; }
    public ReadOnlyMemorySlice<byte> Memory { get; }
    public int Length { get; }
    public BinaryOverlayFactoryPackage Package { get; }
    public PluginBinaryOverlay.ConverterFactory<T> Getter { get; }
    public RecordTypeConverter RecordTypeConverter { get; }

    public NumberedLoquiList(
        ReadOnlyMemorySlice<byte> mem,
        int amount,
        int length,
        BinaryOverlayFactoryPackage package,
        RecordTypeConverter recordTypeConverter,
        PluginBinaryOverlay.ConverterFactory<T> getter)
    {
        Amount = amount;
        Memory = mem;
        Length = length;
        Package = package;
        Getter = getter;
        RecordTypeConverter = recordTypeConverter;
    }

    public T this[int index]
    {
        get
        {
            if (index >= Amount || index < 0)
            {
                throw new IndexOutOfRangeException();
            }
            return Getter(new OverlayStream(Memory.Slice(index * Length), Package), Package, RecordTypeConverter);
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

internal class NumberedEnumList<T> : IReadOnlyList<T>
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