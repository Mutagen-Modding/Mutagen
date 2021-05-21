using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Translations.Binary;
using System;

namespace Mutagen.Bethesda.Plugins.Binary.Translations
{
    public static class EnumBinaryTranslationExt
    {
        public static bool Parse<TEnum>(
            this EnumBinaryTranslation<TEnum, MutagenFrame, MutagenWriter> transl,
            MutagenFrame reader,
            out TEnum item)
            where TEnum : struct, Enum, IConvertible
        {
            return transl.Parse(reader, checked((int)reader.Remaining), out item);
        }

        public static TEnum Parse<TEnum>(
            this EnumBinaryTranslation<TEnum, MutagenFrame, MutagenWriter> transl,
            MutagenFrame reader)
            where TEnum : struct, Enum, IConvertible
        {
            return transl.Parse(reader, checked((int)reader.Remaining));
        }

        public static void Write<TEnum>(
            this EnumBinaryTranslation<TEnum, MutagenFrame, MutagenWriter> transl,
            MutagenWriter writer,
            TEnum item,
            RecordType header,
            long length)
            where TEnum : struct, Enum, IConvertible
        {
            try
            {
                using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
                {
                    transl.WriteValue(writer, item, length);
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Factory(ex, header);
            }
        }

        public static void WriteNullable<TEnum>(
            this EnumBinaryTranslation<TEnum, MutagenFrame, MutagenWriter> transl,
            MutagenWriter writer,
            TEnum? item,
            RecordType header,
            long length)
            where TEnum : struct, Enum, IConvertible
        {
            try
            {
                if (!item.HasValue) return;
                using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
                {
                    transl.WriteValue(writer, item.Value, length);
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Factory(ex, header);
            }
        }
    }
}
