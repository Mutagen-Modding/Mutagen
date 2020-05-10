using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Binary
{
    public abstract class TypicalBinaryTranslation<T>
        where T : class
    {
        protected abstract T ParseBytes(byte[] bytes);

        protected abstract T ParseValue(MutagenFrame reader);

        public bool Parse(MutagenFrame frame, out T item)
        {
            item = ParseValue(frame);
            return true;
        }

        public T Parse(MutagenFrame frame)
        {
            return ParseValue(frame);
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
            using (HeaderExport.ExportHeader(writer, header, ObjectType.Subrecord))
            {
                Write(writer, item);
            }
        }
    }
}
