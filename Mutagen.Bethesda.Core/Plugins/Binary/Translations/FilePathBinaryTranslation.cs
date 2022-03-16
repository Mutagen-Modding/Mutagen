using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Strings.DI;

namespace Mutagen.Bethesda.Plugins.Binary.Translations
{
    public class FilePathBinaryTranslation
    {
        public static readonly FilePathBinaryTranslation Instance = new();

        public bool Parse(MutagenFrame reader, out FilePath item)
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
}
