using Noggog;

namespace Mutagen.Bethesda.Translations.Binary;

public interface IPrimitiveBinaryTranslation
{
    int ExpectedLength { get; }
}

public abstract class PrimitiveBinaryTranslation<TItem, TReader, TWriter> : IPrimitiveBinaryTranslation
    where TItem : struct
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    public abstract int ExpectedLength { get; }

    public abstract TItem Parse(TReader reader);

    public bool Parse(TReader reader, out TItem item)
    {
        item = Parse(reader);
        return true;
    }

    public abstract void Write(TWriter writer, TItem item);

    public void WriteNullable(TWriter writer, TItem? item)
    {
        if (!item.HasValue) return;
        Write(writer, item.Value);
    }

    public void Write(
        TWriter writer,
        TItem item,
        Action<TWriter, TItem>? write = null)
    {
        write ??= Write;
        write(writer, item);
    }

    public void WriteNullable(
        TWriter writer,
        TItem? item,
        Action<TWriter, TItem>? write = null)
    {
        if (!item.HasValue) return;
        write ??= Write;
        write(writer, item.Value);
    }
}