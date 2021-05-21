using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Translations.Binary;
using System;

namespace Mutagen.Bethesda.Plugins.Binary.Translations
{
    public static class FloatBinaryTranslationExt
    {
        public static void Write(
            this PrimitiveBinaryTranslation<float, MutagenFrame, MutagenWriter> transl,
            MutagenWriter writer,
            float item,
            RecordType header,
            float multiplier)
        {
            transl.Write(writer, item / multiplier, header);
        }

        public static void WriteNullable(
            this PrimitiveBinaryTranslation<float, MutagenFrame, MutagenWriter> transl,
            MutagenWriter writer,
            float? item,
            RecordType header,
            float multiplier)
        {
            if (!item.HasValue) return;
            transl.Write(writer, item.Value / multiplier, header);
        }

        public static void Write(
            this PrimitiveBinaryTranslation<float, MutagenFrame, MutagenWriter> transl,
            MutagenWriter writer,
            float? item,
            RecordType header,
            FloatIntegerType integerType,
            double multiplier)
        {
            try
            {
                if (item == null) return;
                using (HeaderExport.Subrecord(writer, header))
                {
                    FloatBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(writer, item, integerType, multiplier);
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Factory(ex, header);
            }
        }
    }
}
