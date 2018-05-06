using Loqui;
using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class FormIDBinaryTranslation : PrimitiveBinaryTranslation<FormID>
    {
        public readonly static FormIDBinaryTranslation Instance = new FormIDBinaryTranslation();
        public override int? ExpectedLength => 4;

        protected override FormID ParseValue(MutagenFrame reader)
        {
            return FormID.Factory(reader.Reader.ReadBytes(ExpectedLength.Value));
        }

        protected override void WriteValue(MutagenWriter writer, FormID item)
        {
            writer.Write(item.ToBytes());
        }

        public void Write<T>(MutagenWriter writer, ILink<T> item, bool doMasks, out Exception errorMask)
            where T : MajorRecord
        {
            this.Write(
                writer,
                item.FormID,
                doMasks: doMasks,
                errorMask: out errorMask);
        }

        public void Write<M, T>(
            MutagenWriter writer,
            ILink<T> item,
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
            ILink<T> item,
            RecordType header,
            int fieldIndex,
            Func<M> errorMask,
            bool nullable = false)
            where M : IErrorMask
            where T : MajorRecord
        {
            this.Write(
                writer,
                item.FormID,
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
            FormIDSetLink<T> item,
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
                item.FormID,
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
            ILink<T> item,
            RecordType header,
            bool nullable,
            bool doMasks,
            out Exception errorMask)
            where T : MajorRecord
        {
            this.Write(
                writer,
                item.FormID,
                header,
                nullable,
                doMasks,
                out errorMask);
        }

        public void Write<T>(
            MutagenWriter writer,
            FormIDSetLink<T> item,
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
                item.FormID,
                header,
                nullable,
                doMasks,
                out errorMask);
        }
    }
}
