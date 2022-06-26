using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public class ModKeyBinaryTranslation
{
    public static readonly ModKeyBinaryTranslation Instance = new();

    public bool Parse<TReader>(TReader reader, [MaybeNullWhen(false)]out ModKey item)
        where TReader : IMutagenReadStream
    {
        if (!StringBinaryTranslation.Instance.Parse(reader, out var str))
        {
            item = default!;
            return false;
        }

        return ModKey.TryFromNameAndExtension(str, out item!);
    }

    public ModKey Parse<TReader>(TReader reader)
        where TReader : IMutagenReadStream
    {
        if (!StringBinaryTranslation.Instance.Parse(reader, out var str))
        {
            return ModKey.Null;
        }

        if (!ModKey.TryFromNameAndExtension(str, out var item))
        {
            return ModKey.Null;
        }

        return item;
    }

    public void Write(MutagenWriter writer, ModKey item, long length)
    {
        StringBinaryTranslation.Instance.Write(writer, item.ToString(), length);
    }

    public void Write(
        MutagenWriter writer,
        ModKey item,
        RecordType header,
        StringBinaryType binaryType = StringBinaryType.NullTerminate)
    {
        StringBinaryTranslation.Instance.Write(
            writer,
            item.ToString(),
            header,
            binaryType);
    }
}