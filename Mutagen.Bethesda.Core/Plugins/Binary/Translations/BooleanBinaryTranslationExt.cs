using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Translations.Binary;
using System;

namespace Mutagen.Bethesda.Plugins.Binary.Translations
{
    public static class BooleanBinaryTranslationExt
    {
        public static void WriteAsMarker(
            this PrimitiveBinaryTranslation<bool, MutagenFrame, MutagenWriter> transl,
            MutagenWriter writer,
            bool item,
            RecordType header)
        {
            try
            {
                if (!item) return;
                using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
                {
                    // Presence of marker signifies true
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Factory(ex, header);
            }
        }

        public static void Write(
            this PrimitiveBinaryTranslation<bool, MutagenFrame, MutagenWriter> transl,
            MutagenWriter writer,
            bool item,
            RecordType header,
            byte byteLength)
        {
            try
            {
                using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
                {
                    writer.Write(item ? 1 : 0, byteLength);
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Factory(ex, header);
            }
        }

        public static void WriteNullable(
            this PrimitiveBinaryTranslation<bool, MutagenFrame, MutagenWriter> transl,
            MutagenWriter writer,
            bool? item,
            RecordType header,
            byte byteLength)
        {
            try
            {
                if (!item.HasValue) return;
                using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
                {
                    writer.Write(item.Value ? 1 : 0, byteLength);
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Factory(ex, header);
            }
        }
    }
}
