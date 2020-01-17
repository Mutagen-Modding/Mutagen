using Loqui.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class FormLinkBinaryTranslation
    {
        public readonly static FormLinkBinaryTranslation Instance = new FormLinkBinaryTranslation();

        public bool Parse(
            MutagenFrame frame,
            out FormKey item,
            MasterReferences masterReferences)
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id, masterReferences))
            {
                item = id;
                return true;
            }
            item = FormKey.NULL;
            return false;
        }

        public FormKey Parse(
            MutagenFrame frame,
            MasterReferences masterReferences)
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id, masterReferences))
            {
                return id;
            }
            return FormKey.NULL;
        }

        public FormKey Parse(
            MutagenFrame frame,
            MasterReferences masterReferences,
            FormKey defaultVal)
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id, masterReferences))
            {
                return id;
            }
            return defaultVal;
        }

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
