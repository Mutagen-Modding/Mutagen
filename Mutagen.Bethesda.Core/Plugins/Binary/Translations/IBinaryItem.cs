using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Plugins.Binary.Translations
{
    public interface IBinaryWriteTranslator
    {
        void Write(
            MutagenWriter writer,
            object item,
            RecordTypeConverter? recordTypeConverter = null);
    }

    public interface IBinaryItem
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object BinaryWriteTranslator { get; }
        void WriteToBinary(
            MutagenWriter writer,
            RecordTypeConverter? recordTypeConverter = null);
    }
}
