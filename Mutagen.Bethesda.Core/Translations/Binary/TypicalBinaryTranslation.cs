using Noggog;

namespace Mutagen.Bethesda.Translations.Binary;

public abstract class TypicalBinaryTranslation<TItem, TReader, TWriter>
    where TReader : IBinaryReadStream
    where TWriter : IBinaryWriteStream
{
    protected abstract TItem ParseBytes(MemorySlice<byte> bytes);

    public abstract TItem Parse(TReader reader);

    public bool Parse(TReader reader, out TItem item)
    {
        item = Parse(reader);
        return true;
    }

    public abstract void Write(TWriter writer, TItem item);
}