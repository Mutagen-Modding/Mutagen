using Loqui.Internal;
using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class FormLinkBinaryTranslation
    {
        public readonly static FormLinkBinaryTranslation Instance = new FormLinkBinaryTranslation();

        public bool Parse(
            MutagenFrame frame,
            out FormKey item,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id))
            {
                item = id;
                return true;
            }
            item = FormKey.Null;
            return false;
        }

        public FormKey Parse(
            MutagenFrame frame,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id))
            {
                return id;
            }
            return FormKey.Null;
        }

        public FormKey Parse(
            MutagenFrame frame,
            FormKey defaultVal,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id))
            {
                return id;
            }
            return defaultVal;
        }

        public bool Parse<T>(
            MutagenFrame frame,
            [MaybeNullWhen(false)] out FormLink<T> item,
            RecordTypeConverter? recordTypeConverter = null)
            where T : class, IMajorRecordCommonGetter
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id))
            {
                item = new FormLink<T>(id);
                return true;
            }
            item = new FormLink<T>();
            return false;
        }

        public bool Parse<T>(
            MutagenFrame frame,
            [MaybeNullWhen(false)] out IFormLink<T> item,
            RecordTypeConverter? recordTypeConverter = null)
            where T : class, IMajorRecordCommonGetter
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id))
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
            RecordTypeConverter? recordTypeConverter = null)
            where T : class, IMajorRecordCommonGetter
        {
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKey);
        }

        public void Write<T>(
            MutagenWriter writer,
            IEDIDLinkGetter<T> item,
            MasterReferenceReader masterReferences,
            RecordTypeConverter? recordTypeConverter = null)
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
            RecordType header)
            where T : class, IMajorRecordCommonGetter
        {
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKey,
                header);
        }

        public void WriteNullable<T>(
            MutagenWriter writer,
            IFormLinkNullableGetter<T> item,
            RecordType header)
            where T : class, IMajorRecordCommonGetter
        {
            if (item.FormKey == null) return;
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKey.Value,
                header);
        }

        public void WriteNullable<T>(
            MutagenWriter writer,
            IFormLinkNullableGetter<T> item)
            where T : class, IMajorRecordCommonGetter
        {
            if (item.FormKey == null) return;
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKey.Value);
        }
    }
}
