using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Records.Binary.Streams;

namespace Mutagen.Bethesda.Records.Binary.Translations
{
    public class ModKeyBinaryTranslation
    {
        public readonly static ModKeyBinaryTranslation Instance = new ModKeyBinaryTranslation();

        public bool Parse(MutagenFrame frame, [MaybeNullWhen(false)]out ModKey item)
        {
            if (!StringBinaryTranslation.Instance.Parse(frame, out var str))
            {
                item = default!;
                return false;
            }

            return ModKey.TryFromNameAndExtension(str, out item!);
        }

        public ModKey Parse(MutagenFrame frame)
        {
            if (!StringBinaryTranslation.Instance.Parse(frame, out var str))
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
}
