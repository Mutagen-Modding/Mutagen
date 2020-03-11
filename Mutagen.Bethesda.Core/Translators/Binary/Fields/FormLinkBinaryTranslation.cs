using Loqui.Internal;
using Mutagen.Bethesda.Internals;
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
            MasterReferenceReader masterReferences,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id, masterReferences))
            {
                item = id;
                return true;
            }
            item = FormKey.Null;
            return false;
        }

        public FormKey Parse(
            MutagenFrame frame,
            MasterReferenceReader masterReferences,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id, masterReferences))
            {
                return id;
            }
            return FormKey.Null;
        }

        public FormKey Parse(
            MutagenFrame frame,
            MasterReferenceReader masterReferences,
            FormKey defaultVal,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id, masterReferences))
            {
                return id;
            }
            return defaultVal;
        }

        public bool Parse<T>(
            MutagenFrame frame,
            out IFormLink<T> item,
            MasterReferenceReader masterReferences,
            RecordTypeConverter? recordTypeConverter = null)
            where T : class, IMajorRecordCommonGetter
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id, masterReferences))
            {
                item = new FormLink<T>(id);
                return true;
            }
            item = new FormLink<T>();
            return false;
        }

        public void Write<T>(
            MutagenWriter writer,
            IFormLinkGetter<T> item,
            MasterReferenceReader masterReferences,
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
            MasterReferenceReader masterReferences,
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
            IFormLinkGetter<T> item,
            MasterReferenceReader masterReferences,
            RecordType header)
            where T : class, IMajorRecordCommonGetter
        {
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKey,
                masterReferences,
                header);
        }

        public void WriteNullable<T>(
            MutagenWriter writer,
            IFormLinkNullableGetter<T> item,
            MasterReferenceReader masterReferences,
            RecordType header)
            where T : class, IMajorRecordCommonGetter
        {
            if (item.FormKey == null) return;
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKey,
                masterReferences,
                header);
        }
    }
}
