using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Records.Binary.Streams;
using Noggog;
using System;

namespace Mutagen.Bethesda.Records.Binary.Translations
{
    public abstract class TypicalBinaryTranslation<T>
    {
        protected abstract T ParseBytes(MemorySlice<byte> bytes);

        protected abstract T ParseValue(MutagenFrame reader);

        public bool Parse(MutagenFrame reader, out T item)
        {
            item = ParseValue(reader);
            return true;
        }

        public T Parse(MutagenFrame reader)
        {
            return ParseValue(reader);
        }

        public abstract void Write(MutagenWriter writer, T item);

        public void Write(
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
                    Write(writer, item);
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Enrich(ex, header);
            }
        }
    }
}
