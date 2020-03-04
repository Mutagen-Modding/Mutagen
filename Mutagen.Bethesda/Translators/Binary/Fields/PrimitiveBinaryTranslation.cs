using Noggog;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public interface IPrimitiveBinaryTranslation
    {
        int ExpectedLength { get; }
    }

    public abstract class PrimitiveBinaryTranslation<T> : IPrimitiveBinaryTranslation
        where T : struct
    {
        public abstract int ExpectedLength { get; }

        public abstract T ParseValue(MutagenFrame reader);

        public T Parse(MutagenFrame frame, T defaultVal = default)
        {
            if (Parse(frame, out var item))
            {
                return item;
            }
            return defaultVal;
        }

        public bool Parse(MutagenFrame frame, [MaybeNullWhen(false)]out T item)
        {
            item = ParseValue(frame);
            return true;
        }

        public abstract void Write(MutagenWriter writer, T item);

        public void WriteNullable(MutagenWriter writer, T? item)
        {
            if (!item.HasValue) return;
            Write(writer, item.Value);
        }

        public void Write(
            MutagenWriter writer,
            T item,
            RecordType header,
            Action<MutagenWriter, T>? write = null)
        {
            write = write ?? this.Write;
            using (HeaderExport.ExportHeader(writer, header, ObjectType.Subrecord))
            {
                write(writer, item);
            }
        }

        public void Write(
            MutagenWriter writer,
            T item,
            Action<MutagenWriter, T>? write = null)
        {
            write = write ?? Write;
            write(writer, item);
        }

        public void WriteNullable(
            MutagenWriter writer,
            T? item,
            RecordType header,
            Action<MutagenWriter, T>? write = null)
        {
            if (!item.HasValue) return;
            write = write ?? this.Write;
            using (HeaderExport.ExportHeader(writer, header, ObjectType.Subrecord))
            {
                write(writer, item.Value);
            }
        }

        public void WriteNullable(
            MutagenWriter writer,
            T? item,
            Action<MutagenWriter, T>? write = null)
        {
            if (!item.HasValue) return;
            write = write ?? this.Write;
            write(writer, item.Value);
        }
    }
}
