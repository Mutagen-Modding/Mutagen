using Mutagen.Bethesda.Records.Binary.Streams;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Records.Binary.Translations
{
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
}
