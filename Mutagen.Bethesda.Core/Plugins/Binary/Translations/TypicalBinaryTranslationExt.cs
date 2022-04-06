using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Translations.Binary;
using System;
using Mutagen.Bethesda.Plugins.Meta;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public static class TypicalBinaryTranslationExt
{
    public static void Write<T>(
        this TypicalBinaryTranslation<T, MutagenFrame, MutagenWriter> transl,
        MutagenWriter writer,
        T item,
        RecordType header,
        bool nullable)
    {
        if (item == null)
        {
            if (nullable) return;
            throw new ArgumentException("Non optional string was null.");
        }
        try
        {
            using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
            {
                transl.Write(writer, item);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Factory(ex, header);
        }
    }
}