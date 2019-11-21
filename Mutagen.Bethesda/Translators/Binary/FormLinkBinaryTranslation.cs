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
            out IFormIDLink<T> item,
            MasterReferences masterReferences)
            where T : class, IMajorRecordCommonGetter
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
            out IFormIDSetLink<T> item,
            MasterReferences masterReferences)
            where T : class, IMajorRecordCommonGetter
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
            IFormIDSetLink<T> item)
            where T : class, IMajorRecordCommonGetter
        {
            if (FormKeyBinaryTranslation.Instance.Parse(
                frame: frame,
                masterReferences: masterReferences,
                item: out FormKey val))
            {
                item.FormKey = val;
            }
            else
            {
                item.FormKey = FormKey.NULL;
            }
        }

        public void ParseInto<T>(
            MutagenFrame frame,
            MasterReferences masterReferences,
            IFormIDLink<T> item)
            where T : class, IMajorRecordCommonGetter
        {
            if (FormKeyBinaryTranslation.Instance.Parse(
                frame: frame,
                masterReferences: masterReferences,
                item: out FormKey val))
            {
                item.FormKey = val;
            }
            else
            {
                item.FormKey = FormKey.NULL;
            }
        }

        public void Write<T>(
            MutagenWriter writer,
            IFormIDLinkGetter<T> item,
            MasterReferences masterReferences,
            bool nullable = false)
            where T : class, IMajorRecordCommonGetter
        {
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKey,
                masterReferences);
        }

        public void Write<T>(
            MutagenWriter writer,
            IFormIDSetLinkGetter<T> item,
            MasterReferences masterReferences,
            bool nullable = false)
            where T : class, IMajorRecordCommonGetter
        {
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKey,
                masterReferences);
        }

        public void Write<T>(
            MutagenWriter writer,
            IEDIDLinkGetter<T> item,
            MasterReferences masterReferences,
            bool nullable = false)
            where T : class, IMajorRecordCommonGetter
        {
            this.Write(
                writer,
                item,
                masterReferences);
        }

        public void Write<T>(
            MutagenWriter writer,
            IFormIDLinkGetter<T> item,
            MasterReferences masterReferences,
            RecordType header,
            bool nullable = false)
            where T : class, IMajorRecordCommonGetter
        {
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKey,
                masterReferences,
                header);
        }

        public void Write<T>(
            MutagenWriter writer,
            IFormIDSetLinkGetter<T> item,
            MasterReferences masterReferences,
            RecordType header,
            bool nullable = false)
            where T : class, IMajorRecordCommonGetter
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
