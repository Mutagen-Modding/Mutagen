using Loqui;
using Loqui.Internal;
using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class RecordTypeBinaryTranslation : PrimitiveBinaryTranslation<RecordType>
    {
        public readonly static RecordTypeBinaryTranslation Instance = new RecordTypeBinaryTranslation();
        public override int? ExpectedLength => 4;

        public void ParseInto<T>(MutagenFrame frame, int fieldIndex, EDIDSetLink<T> item, ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                if (Parse(frame, ExpectedLength.Value, out RecordType val, errorMask))
                {
                    item.Set(val);
                }
                else
                {
                    item.Unset();
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public void ParseInto<T>(MutagenFrame frame, int fieldIndex, EDIDLink<T> item, ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                if (Parse(frame, ExpectedLength.Value, out RecordType val, errorMask))
                {
                    item.Set(val);
                }
                else
                {
                    item.Unset();
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            finally
            {
                errorMask?.PopIndex();
            }
        }

        protected override RecordType ParseValue(MutagenFrame reader)
        {
            return HeaderTranslation.ReadNextRecordType(reader.Reader);
        }

        protected override void WriteValue(MutagenWriter writer, RecordType item)
        {
            writer.Write(item.Type);
        }

        public void Write<T>(MutagenWriter writer, IEDIDLink<T> item, ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            this.WriteValue(
                writer,
                item.EDID);
        }

        public void Write<T>(
            MutagenWriter writer,
            IEDIDLink<T> item,
            int fieldIndex,
            ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                this.Write(
                    writer,
                    item,
                    errorMask);
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public void Write<T>(
            MutagenWriter writer,
            EDIDSetLink<T> item,
            int fieldIndex,
            ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                if (!item.HasBeenSet) return;
                this.Write(
                    writer,
                    item,
                    errorMask);
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public void Write<M, T>(
            MutagenWriter writer,
            IEDIDLink<T> item,
            RecordType header,
            int fieldIndex,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
            where T : MajorRecord
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                this.Write(
                    writer,
                    item.EDID,
                    header,
                    nullable: nullable,
                    errorMask: errorMask);
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public void Write<T>(
            MutagenWriter writer,
            EDIDSetLink<T> item,
            RecordType header,
            int fieldIndex,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
            where T : MajorRecord
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                if (!item.HasBeenSet) return;
                this.Write(
                    writer,
                    item.EDID,
                    header,
                    nullable: nullable,
                    errorMask: errorMask);
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public void Write<T>(
            MutagenWriter writer,
            IEDIDLink<T> item,
            RecordType header,
            bool nullable,
            ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            this.Write(
                writer,
                item.EDID,
                header,
                nullable,
                errorMask);
        }

        public void Write<T>(
            MutagenWriter writer,
            EDIDSetLink<T> item,
            RecordType header,
            bool nullable,
            ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            if (!item.HasBeenSet)
            {
                return;
            }
            this.Write(
                writer,
                item.EDID,
                header,
                nullable,
                errorMask);
        }
    }
}
