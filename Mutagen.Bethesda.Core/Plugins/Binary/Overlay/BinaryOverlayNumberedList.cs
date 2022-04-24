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
            return this.Getter(new OverlayStream(this.Memory.Slice(index * Length), this.Package), this.Package, this.RecordTypeConverter);
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

internal class NumberedEnumList<T> : IReadOnlyList<T>
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