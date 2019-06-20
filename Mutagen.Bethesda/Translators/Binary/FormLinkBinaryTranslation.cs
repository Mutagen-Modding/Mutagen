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
            MasterReferences masterReferences)
            where T : class, IMajorRecordInternalGetter
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id, masterReferences))
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
            MasterReferences masterReferences)
            where T : class, IMajorRecordInternalGetter
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id, masterReferences))
            {
                item = new FormIDSetLink<T>(id);
                return true;
            }
            item = new FormIDSetLink<T>();
            return false;
        }


        public void ParseInto<T>(
            MutagenFrame frame,
            MasterReferences masterReferences,
            FormIDSetLink<T> item)
            where T : class, IMajorRecordInternalGetter
        {
            if (FormKeyBinaryTranslation.Instance.Parse(
                frame: frame,
                masterReferences: masterReferences,
                item: out FormKey val))
            {
                item.Set(val);
            }
            else
            {
                item.Unset();
            }
        }

        public void ParseInto<T>(
            MutagenFrame frame,
            MasterReferences masterReferences,
            FormIDLink<T> item)
            where T : class, IMajorRecordInternalGetter
        {
            if (FormKeyBinaryTranslation.Instance.Parse(
                frame: frame,
                masterReferences: masterReferences,
                item: out FormKey val))
            {
                item.Set(val);
            }
            else
            {
                item.Unset();
            }
        }

        public void Write<T>(
            MutagenWriter writer,
            FormIDLink<T> item,
            MasterReferences masterReferences,
            bool nullable = false)
            where T : class, IMajorRecordInternalGetter
        {
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKey,
                masterReferences);
        }

        public void Write<T>(
            MutagenWriter writer,
            FormIDSetLink<T> item,
            MasterReferences masterReferences,
            bool nullable = false)
            where T : class, IMajorRecordInternalGetter
        {
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKey,
                masterReferences);
        }

        public void Write<T>(
            MutagenWriter writer,
            EDIDLink<T> item,
            MasterReferences masterReferences,
            bool nullable = false)
            where T : class, IMajorRecordInternalGetter
        {
            this.Write(
                writer,
                item,
                masterReferences);
        }

        public void Write<T>(
            MutagenWriter writer,
            FormIDLink<T> item,
            MasterReferences masterReferences,
            RecordType header,
            bool nullable = false)
            where T : class, IMajorRecordInternalGetter
        {
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKey,
                masterReferences,
                header);
        }

        public void Write<T>(
            MutagenWriter writer,
            FormIDSetLink<T> item,
            MasterReferences masterReferences,
            RecordType header,
            bool nullable = false)
            where T : class, IMajorRecordInternalGetter
        {
            if (!item.HasBeenSet) return;
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKey,
                masterReferences,
                header);
        }
    }
}
