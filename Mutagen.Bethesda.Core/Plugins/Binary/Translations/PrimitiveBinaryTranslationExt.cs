using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records.Internals;
using System;

namespace Mutagen.Bethesda.Translations.Binary
{
    public static class PrimitiveBinaryTranslationExt
    {
        public static void Write<T>(
            this PrimitiveBinaryTranslation<T, MutagenFrame, MutagenWriter> transl,
            MutagenWriter writer,
            T item,
            RecordType header,
            Action<MutagenWriter, T>? write = null)
            where T : struct
        {
            write ??= transl.Write;
            try
            {
                using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
                {
                    write(writer, item);
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Factory(ex, header);
            }
        }

        public static void WriteNullable<T>(
            this PrimitiveBinaryTranslation<T, MutagenFrame, MutagenWriter> transl,
            MutagenWriter writer,
            T? item,
            RecordType header,
            Action<MutagenWriter, T>? write = null)
            where T : struct
        {
            if (!item.HasValue) return;
            write ??= transl.Write;
            try
            {
                using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
                {
                    write(writer, item.Value);
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Factory(ex, header);
            }
        }

        public static void WriteNullable<T>(
            this PrimitiveBinaryTranslation<T, MutagenFrame, MutagenWriter> transl,
            MutagenWriter writer,
            T? item,
            Action<MutagenWriter, T>? write = null)
            where T : struct
        {
            if (!item.HasValue) return;
            write ??= transl.Write;
            write(writer, item.Value);
        }
    }
}
