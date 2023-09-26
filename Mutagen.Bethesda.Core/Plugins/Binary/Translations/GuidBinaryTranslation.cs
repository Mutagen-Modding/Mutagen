using System.Data;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public sealed class GuidBinaryTranslation : PrimitiveBinaryTranslation<Guid, MutagenFrame, MutagenWriter>
{
    public static readonly GuidBinaryTranslation Instance = new();
    public override int ExpectedLength => 3;

    public override Guid Parse(MutagenFrame reader)
    {
        return new Guid(reader.ReadSpan(16));
    }

    public override void Write(MutagenWriter writer, Guid item)
    {
        Span<byte> bytes = stackalloc byte[16];
        if (!item.TryWriteBytes(bytes))
        {
            throw new DataException();
        }
        writer.Write(bytes);
    }
}
