using Loqui;
using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class RecordTypeBinaryTranslation : PrimitiveBinaryTranslation<RecordType>
    {
        public readonly static RecordTypeBinaryTranslation Instance = new RecordTypeBinaryTranslation();
        public override int? ExpectedLength => 4;

        protected override RecordType ParseValue(MutagenFrame reader)
        {
            return HeaderTranslation.ReadNextRecordType(reader.Reader);
        }

        protected override void WriteValue(MutagenWriter writer, RecordType item)
        {
            writer.Write(item.Type);
        }

        public void Write<T>(MutagenWriter writer, IEDIDLink<T> item, bool doMasks, out Exception errorMask)
            where T : MajorRecord
        {
            this.Write(
                writer,
                item.EDID,
                doMasks: doMasks,
                errorMask: out errorMask);
        }

        public void Write<M, T>(
            MutagenWriter writer,
            IEDIDLink<T> item,
            int fieldIndex,
            Func<M> errorMask)
            where M : IErrorMask
            where T : MajorRecord
        {
            this.Write(
                writer,
                item,
                errorMask != null,
                out var subMask);
            ErrorMask.HandleException(
                errorMask,
                fieldIndex,
                subMask);
        }

        public void Write<M, T>(
            MutagenWriter writer,
            EDIDSetLink<T> item,
            int fieldIndex,
            Func<M> errorMask)
            where M : IErrorMask
            where T : MajorRecord
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item,
                errorMask != null,
                out var subMask);
            ErrorMask.HandleException(
                errorMask,
                fieldIndex,
                subMask);
        }

        public void Write<M, T>(
            MutagenWriter writer,
            IEDIDLink<T> item,
            RecordType header,
            int fieldIndex,
            Func<M> errorMask,
            bool nullable = false)
            where M : IErrorMask
            where T : MajorRecord
        {
            this.Write(
                writer,
                item.EDID,
                header,
                nullable: nullable,
                doMasks: errorMask != null,
                errorMask: out var subMask);
            ErrorMask.HandleException(
                errorMask,
                fieldIndex,
                subMask);
        }

        public void Write<M, T>(
            MutagenWriter writer,
            EDIDSetLink<T> item,
            RecordType header,
            int fieldIndex,
            Func<M> errorMask,
            bool nullable = false)
            where M : IErrorMask
            where T : MajorRecord
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.EDID,
                header,
                nullable: nullable,
                doMasks: errorMask != null,
                errorMask: out var subMask);
            ErrorMask.HandleException(
                errorMask,
                fieldIndex,
                subMask);
        }

        public void Write<T>(
            MutagenWriter writer,
            IEDIDLink<T> item,
            RecordType header,
            bool nullable,
            bool doMasks,
            out Exception errorMask)
            where T : MajorRecord
        {
            this.Write(
                writer,
                item.EDID,
                header,
                nullable,
                doMasks,
                out errorMask);
        }

        public void Write<T>(
            MutagenWriter writer,
            EDIDSetLink<T> item,
            RecordType header,
            bool nullable,
            bool doMasks,
            out Exception errorMask)
            where T : MajorRecord
        {
            if (!item.HasBeenSet)
            {
                errorMask = null;
                return;
            }
            this.Write(
                writer,
                item.EDID,
                header,
                nullable,
                doMasks,
                out errorMask);
        }
    }
}
