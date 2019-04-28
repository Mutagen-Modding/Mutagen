using Loqui.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class FormLinkBinaryTranslation
    {
        public readonly static FormLinkBinaryTranslation Instance = new FormLinkBinaryTranslation();

        public bool Parse<T>(
            MutagenFrame frame,
            out FormIDLink<T> item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
            where T : class, IMajorRecord
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id, masterReferences, errorMask))
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
            where T : class, IMajorRecord
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id, masterReferences, errorMask))
            {
                item = new FormIDSetLink<T>(id);
                return true;
            }
            item = new FormIDSetLink<T>();
            return false;
        }


        public void ParseInto<T>(
            MutagenFrame frame,
            int fieldIndex,
            MasterReferences masterReferences,
            FormIDSetLink<T> item,
            ErrorMaskBuilder errorMask)
            where T : class, IMajorRecord
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    if (FormKeyBinaryTranslation.Instance.Parse(
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
            }
        }

        public void ParseInto<T>(
            MutagenFrame frame,
            int fieldIndex,
            MasterReferences masterReferences,
            FormIDLink<T> item,
            ErrorMaskBuilder errorMask)
            where T : class, IMajorRecord
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    if (FormKeyBinaryTranslation.Instance.Parse(
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
            }
        }

        public void Write<T>(
            MutagenWriter writer,
            FormIDLink<T> item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
            where T : class, IMajorRecord
        {
            FormKeyBinaryTranslation.Instance.Write(
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
            where T : class, IMajorRecord
        {
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKey,
                masterReferences,
                errorMask: errorMask);
        }

        public void Write<T>(
            MutagenWriter writer,
            FormIDLink<T> item,
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

        public void Write<T>(
            MutagenWriter writer,
            FormIDSetLink<T> item,
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

        public void Write<T>(
            MutagenWriter writer,
            EDIDLink<T> item,
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

        public void Write<T>(
            MutagenWriter writer,
            FormIDLink<T> item,
            MasterReferences masterReferences,
            RecordType header,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
            where T : class, IMajorRecord
        {
            FormKeyBinaryTranslation.Instance.Write(
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
            where T : class, IMajorRecord
        {
            if (!item.HasBeenSet) return;
            FormKeyBinaryTranslation.Instance.Write(
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
            where T : class, IMajorRecord
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    FormKeyBinaryTranslation.Instance.Write(
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
            where T : class, IMajorRecord
        {
            if (!item.HasBeenSet) return;
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKey,
                masterReferences,
                header,
                errorMask: errorMask);
        }
    }
}
