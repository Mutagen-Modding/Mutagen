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
            out FormKey item)
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id))
            {
                item = id;
                return true;
            }
            item = FormKey.Null;
            return false;
        }

        public FormKey Parse(MutagenFrame frame)
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id))
            {
                return id;
            }
            return FormKey.Null;
        }

        public FormKey Parse(
            MutagenFrame frame,
            FormKey defaultVal)
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id))
            {
                return id;
            }
            return defaultVal;
        }

        public bool Parse<T>(
            MutagenFrame frame,
            [MaybeNullWhen(false)] out FormLink<T> item)
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
            [MaybeNullWhen(false)] out IFormLinkGetter<T> item)
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
            [MaybeNullWhen(false)] out IFormLink<T> item)
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
            [MaybeNullWhen(false)] out IFormLinkNullableGetter<T> item)
            where T : class, IMajorRecordCommonGetter
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id))
            {
                item = new FormLinkNullable<T>(id);
                return true;
            }
            item = new FormLinkNullable<T>();
            return false;
        }

        public bool Parse<T>(
            MutagenFrame frame,
            [MaybeNullWhen(false)] out IFormLinkNullable<T> item)
            where T : class, IMajorRecordCommonGetter
        {
            if (FormKeyBinaryTranslation.Instance.Parse(frame, out FormKey id))
            {
                item = new FormLinkNullable<T>(id);
                return true;
            }
            item = new FormLinkNullable<T>();
            return false;
        }

        public void Write<T>(
            MutagenWriter writer,
            IFormLinkGetter<T> item)
            where T : class, IMajorRecordCommonGetter
        {
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKey);
        }

        public void Write<T>(
            MutagenWriter writer,
            IEDIDLink<T> item,
            MasterReferenceReader masterReferences)
            where T : class, IMajorRecordCommonGetter
        {
            this.Write(
                writer,
                item,
                masterReferences);
        }

        public void Write<T>(
            MutagenWriter writer,
            FormLink<T> item,
            RecordType header)
            where T : class, IMajorRecordCommonGetter
        {
            try
            {
                FormKeyBinaryTranslation.Instance.Write(
                    writer,
                    item.FormKey,
                    header);
            }
            catch (Exception ex)
            {
                throw SubrecordException.Factory(ex, header);
            }
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
            FormLinkNullable<T> item,
            RecordType header)
            where T : class, IMajorRecordCommonGetter
        {
            if (item.FormKeyNullable == null) return;
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKeyNullable.Value,
                header);
        }

        public void WriteNullable<T>(
            MutagenWriter writer,
            IFormLinkNullableGetter<T> item,
            RecordType header)
            where T : class, IMajorRecordCommonGetter
        {
            if (item.FormKeyNullable == null) return;
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKeyNullable.Value,
                header);
        }

        public void WriteNullable<T>(
            MutagenWriter writer,
            FormLinkNullable<T> item)
            where T : class, IMajorRecordCommonGetter
        {
            if (item.FormKeyNullable == null) return;
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKeyNullable.Value);
        }

        public void WriteNullable<T>(
            MutagenWriter writer,
            IFormLinkNullableGetter<T> item)
            where T : class, IMajorRecordCommonGetter
        {
            if (item.FormKeyNullable == null) return;
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKeyNullable.Value);
        }
    }
}
