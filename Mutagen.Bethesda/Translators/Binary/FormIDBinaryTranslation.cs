using Loqui;
using Loqui.Internal;
using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class FormIDBinaryTranslation : PrimitiveBinaryTranslation<FormID>
    {
        public readonly static FormIDBinaryTranslation Instance = new FormIDBinaryTranslation();
        public override int? ExpectedLength => 4;

        public void ParseInto<T>(MutagenFrame frame, int fieldIndex, FormIDSetLink<T> item, ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                if (Parse(
                    frame: frame,
                    length: ExpectedLength.Value,
                    item: out FormID val,
                    errorMask: errorMask))
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

        public void ParseInto<T>(MutagenFrame frame, int fieldIndex, FormIDLink<T> item, ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                if (Parse(
                    frame: frame,
                    item: out FormID val,
                    errorMask: errorMask))
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

        public bool Parse<T>(MutagenFrame frame, out FormIDLink<T> item, ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            if (Parse(frame, out FormID id, errorMask))
            {
                item = new FormIDLink<T>(id);
                return true;
            }
            item = new FormIDLink<T>();
            return false;
        }

        public bool Parse<T>(MutagenFrame frame, out FormIDSetLink<T> item, ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            if (Parse(frame, out FormID id, errorMask))
            {
                item = new FormIDSetLink<T>(id);
                return true;
            }
            item = new FormIDSetLink<T>();
            return false;
        }

        public bool Parse<T>(MutagenFrame frame, out EDIDLink<T> item, ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            if (ExpectedLength.HasValue)
            {
                if (!frame.TryCheckUpcomingRead(this.ExpectedLength.Value, out var ex))
                {
                    frame.Position = frame.FinalLocation;
                    errorMask.ReportExceptionOrThrow(ex);
                    item = new EDIDLink<T>();
                    return false;
                }
            }
            
            item = new EDIDLink<T>(HeaderTranslation.ReadNextRecordType(frame));
            return true;
        }

        public bool Parse<T>(MutagenFrame frame, out EDIDSetLink<T> item, ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            if (ExpectedLength.HasValue)
            {
                if (!frame.TryCheckUpcomingRead(this.ExpectedLength.Value, out var ex))
                {
                    frame.Position = frame.FinalLocation;
                    errorMask.ReportExceptionOrThrow(ex);
                    item = new EDIDSetLink<T>();
                    return false;
                }
            }

            item = new EDIDSetLink<T>(HeaderTranslation.ReadNextRecordType(frame));
            return true;
        }

        protected override FormID ParseValue(MutagenFrame reader)
        {
            return FormID.Factory(reader.Reader.ReadUInt32());
        }

        protected override void WriteValue(MutagenWriter writer, FormID item)
        {
            writer.Write(item.ToBytes());
        }

        public void Write<T>(MutagenWriter writer, FormIDLink<T> item, ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            this.Write(
                writer,
                item.FormID,
                errorMask: errorMask);
        }

        public void Write<T>(MutagenWriter writer, FormIDSetLink<T> item, ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            this.Write(
                writer,
                item.FormID,
                errorMask: errorMask);
        }

        public void Write<T>(MutagenWriter writer, EDIDLink<T> item, ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            Int32BinaryTranslation.Instance.Write(
                writer,
                item.EDID.TypeInt,
                errorMask: errorMask);
        }

        public void Write<T>(MutagenWriter writer, EDIDSetLink<T> item, ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            Int32BinaryTranslation.Instance.Write(
                writer,
                item.EDID.TypeInt,
                errorMask: errorMask);
        }

        public void Write<T>(
            MutagenWriter writer,
            FormIDLink<T> item,
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
            FormIDSetLink<T> item,
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
            EDIDLink<T> item,
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
            FormIDLink<T> item,
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
                    item.FormID,
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
            FormIDSetLink<T> item,
            RecordType header,
            int fieldIndex,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
            where T : MajorRecord
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.FormID,
                header,
                nullable: nullable,
                errorMask: errorMask);
        }
    }
}
