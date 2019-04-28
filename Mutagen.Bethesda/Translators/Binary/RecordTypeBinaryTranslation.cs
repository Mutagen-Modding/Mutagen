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
        public override int ExpectedLength => 4;

        public void ParseInto<T>(MutagenFrame frame, int fieldIndex, EDIDSetLink<T> item, ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            using (errorMask?.PushIndex(fieldIndex))
            {
                try
                {
                    if (Parse(frame, ExpectedLength, out RecordType val, errorMask))
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
            }
        }

        public void ParseInto<T>(MutagenFrame frame, int fieldIndex, EDIDLink<T> item, ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            using (errorMask?.PushIndex(fieldIndex))
            {
                try
                {
                    if (Parse(frame, ExpectedLength, out RecordType val, errorMask))
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
            }
        }

        public override RecordType ParseValue(MutagenFrame reader)
        {
            return HeaderTranslation.ReadNextRecordType(reader.Reader);
        }

        public bool Parse<T>(
            MutagenFrame frame,
            out EDIDLink<T> item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
            where T : class, IMajorRecord
        {
            if (!frame.TryCheckUpcomingRead(4, out var ex))
            {
                frame.Position = frame.FinalLocation;
                errorMask.ReportExceptionOrThrow(ex);
                item = new EDIDLink<T>();
                return false;
            }

            item = new EDIDLink<T>(HeaderTranslation.ReadNextRecordType(frame));
            return true;
        }

        public bool Parse<T>(
            MutagenFrame frame,
            out EDIDSetLink<T> item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
            where T : class, IMajorRecord
        {
            if (!frame.TryCheckUpcomingRead(4, out var ex))
            {
                frame.Position = frame.FinalLocation;
                errorMask.ReportExceptionOrThrow(ex);
                item = new EDIDSetLink<T>();
                return false;
            }

            item = new EDIDSetLink<T>(HeaderTranslation.ReadNextRecordType(frame));
            return true;
        }

        public override void WriteValue(MutagenWriter writer, RecordType item)
        {
            writer.Write(item.TypeInt);
        }

        public void Write<T>(MutagenWriter writer, IEDIDLink<T> item, ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            this.WriteValue(
                writer,
                item.Linked ? item.EDID : EDIDLink<T>.UNLINKED);
        }

        public void Write<T>(
            MutagenWriter writer,
            IEDIDLink<T> item,
            int fieldIndex,
            ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            using (errorMask?.PushIndex(fieldIndex))
            {
                try
                {
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
            }
        }

        public void Write<T>(
            MutagenWriter writer,
            EDIDSetLink<T> item,
            int fieldIndex,
            ErrorMaskBuilder errorMask)
            where T : MajorRecord
        {
            using (errorMask?.PushIndex(fieldIndex))
            {
                try
                {
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
            using (errorMask?.PushIndex(fieldIndex))
            {
                try
                {
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
            using (errorMask?.PushIndex(fieldIndex))
            {
                try
                {
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

        public void Write<T>(
            MutagenWriter writer,
            EDIDLink<T> item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
            where T : class, IMajorRecord
        {
            Int32BinaryTranslation.Instance.Write(
                writer,
                item.EDID.TypeInt,
                errorMask: errorMask);
        }

        public void Write<T>(
            MutagenWriter writer,
            EDIDSetLink<T> item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
            where T : class, IMajorRecord
        {
            Int32BinaryTranslation.Instance.Write(
                writer,
                item.EDID.TypeInt,
                errorMask: errorMask);
        }

        public void Write<T>(
            MutagenWriter writer,
            EDIDSetLink<T> item,
            MasterReferences masterReferences,
            int fieldIndex,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
            where T : class, IMajorRecord
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    this.Write(
                        writer,
                        item,
                        masterReferences,
                        errorMask);
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
            }
        }
    }
}
