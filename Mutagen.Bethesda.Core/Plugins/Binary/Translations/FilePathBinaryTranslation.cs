using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public sealed class FilePathBinaryTranslation
{
    public static readonly FilePathBinaryTranslation Instance = new();

    public bool Parse<TReader>(TReader reader, out FilePath item)
        where TReader : IMutagenReadStream
    {
        if (!StringBinaryTranslation.Instance.Parse(reader, out var str))
        {
            item = default(FilePath);
            return false;
        }
        item = new FilePath(str);
        return true;
    }

    public void Write(
        MutagenWriter writer,
        FilePath item,
        RecordType header)
    {
        StringBinaryTranslation.Instance.Write(
            writer,
            item.RelativePath,
            header);
    }
}