using Loqui;
using Loqui.Internal;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class FormKeyBinaryTranslation
    {
        public readonly static FormKeyBinaryTranslation Instance = new FormKeyBinaryTranslation();

        public bool Parse<T>(
            MutagenFrame frame,
            out FormIDLink<T> item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
            where T : IMajorRecord
        {
            if (Parse(frame, out FormKey id, masterReferences, errorMask))
            {
                item = new FormIDLink<T>(id);
                return true;
            }
            item = new FormIDLink<T>();
            return false;
        }

        public bool Parse<T>(
            MutagenFrame frame,
            out FormIDSetLink<T> item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
            where T : IMajorRecord
        {
            if (Parse(frame, out FormKey id, masterReferences, errorMask))
            {
                item = new FormIDSetLink<T>(id);
                return true;
            }
            item = new FormIDSetLink<T>();
            return false;
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

        public bool Parse(
            MutagenFrame frame,
            out FormKey item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
        {
            var id = frame.ReadUInt32();
            var formID = new FormID(id);
            if (masterReferences.Masters.TryGet(formID.ModID.ID, out var master))
            {
                item = new FormKey(
                    master.Master,
                    id);
                return true;
            }
            item = new FormKey(
                masterReferences.CurrentMod,
                id);
            return true;
        }

        public void ParseInto<T>(
            MutagenFrame frame,
            int fieldIndex,
            MasterReferences masterReferences,
            FormIDSetLink<T> item,
            ErrorMaskBuilder errorMask)
            where T : IMajorRecord
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                if (Parse(
                    frame: frame,
                    masterReferences: masterReferences,
                    item: out FormKey val,
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

        public void ParseInto<T>(
            MutagenFrame frame,
            int fieldIndex,
            MasterReferences masterReferences,
            FormIDLink<T> item, 
            ErrorMaskBuilder errorMask)
            where T : IMajorRecord
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                if (Parse(
                    frame: frame,
                    masterReferences: masterReferences,
                    item: out FormKey val,
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

        public void Write(
            MutagenWriter writer,
            FormKey item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
        {
            UInt32BinaryTranslation.Instance.Write(
                writer: writer,
                item: item.GetFormID(masterReferences).Raw,
                errorMask: errorMask);
        }

        public void Write(
            MutagenWriter writer,
            FormKey item,
            int fieldIndex,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
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
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public void Write<T>(
            MutagenWriter writer,
            FormIDLink<T> item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
            where T : IMajorRecord
        {
            this.Write(
                writer,
                item.FormKey,
                masterReferences,
                errorMask: errorMask);
        }

        public void Write<T>(
            MutagenWriter writer,
            FormIDSetLink<T> item,
            MasterReferences masterReferences, 
            ErrorMaskBuilder errorMask,
            bool nullable = false)
            where T : IMajorRecord
        {
            this.Write(
                writer,
                item.FormKey,
                masterReferences,
                errorMask: errorMask);
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
            FormIDLink<T> item,
            MasterReferences masterReferences,
            int fieldIndex,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
            where T : IMajorRecord
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
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
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public void Write<T>(
            MutagenWriter writer,
            FormIDSetLink<T> item,
            MasterReferences masterReferences,
            int fieldIndex,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
            where T : IMajorRecord
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
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
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public void Write<T>(
            MutagenWriter writer,
            EDIDLink<T> item,
            MasterReferences masterReferences,
            int fieldIndex,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
            where T : class, IMajorRecord
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
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
            finally
            {
                errorMask?.PopIndex();
            }
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
            try
            {
                errorMask?.PushIndex(fieldIndex);
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
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public void Write(
            MutagenWriter writer,
            FormKey item,
            MasterReferences masterReferences,
            RecordType header,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
        {
            using (HeaderExport.ExportHeader(writer, header, ObjectType.Subrecord))
            {
                this.Write(
                    writer,
                    item,
                    masterReferences,
                    errorMask);
            }
        }

        public void Write<T>(
            MutagenWriter writer,
            FormIDLink<T> item,
            MasterReferences masterReferences,
            RecordType header,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
            where T : IMajorRecord
        {
            this.Write(
                writer,
                item.FormKey,
                masterReferences,
                header,
                errorMask: errorMask);
        }

        public void Write<T>(
            MutagenWriter writer,
            FormIDSetLink<T> item,
            MasterReferences masterReferences,
            RecordType header,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
            where T : IMajorRecord
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.FormKey,
                masterReferences,
                header,
                errorMask: errorMask);
        }

        public void Write<T>(
            MutagenWriter writer,
            FormIDLink<T> item,
            MasterReferences masterReferences,
            RecordType header,
            int fieldIndex,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
            where T : IMajorRecord
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                this.Write(
                    writer,
                    item.FormKey,
                    masterReferences,
                    header,
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
            MasterReferences masterReferences,
            RecordType header,
            int fieldIndex,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
            where T : IMajorRecord
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.FormKey,
                masterReferences,
                header,
                errorMask: errorMask);
        }
    }
}
