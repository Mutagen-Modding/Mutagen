using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda.Plugins.Binary.Translations
{
    public class FormLinkBinaryTranslation
    {
        public readonly static FormLinkBinaryTranslation Instance = new();

        public bool Parse<TReader>(
            TReader reader,
            out FormKey item)
            where TReader : IMutagenReadStream
        {
            if (FormKeyBinaryTranslation.Instance.Parse(reader, out FormKey id))
            {
                item = id;
                return true;
            }
            item = FormKey.Null;
            return false;
        }

        public FormKey Parse(MutagenFrame reader)
        {
            if (FormKeyBinaryTranslation.Instance.Parse(reader, out FormKey id))
            {
                return id;
            }
            return FormKey.Null;
        }

        public FormKey Parse(
            MutagenFrame reader,
            FormKey defaultVal)
        {
            if (FormKeyBinaryTranslation.Instance.Parse(reader, out FormKey id))
            {
                return id;
            }
            return defaultVal;
        }

        public bool Parse<T>(
            MutagenFrame reader,
            [MaybeNullWhen(false)] out FormLink<T> item)
            where T : class, IMajorRecordGetter
        {
            if (FormKeyBinaryTranslation.Instance.Parse(reader, out FormKey id))
            {
                item = new FormLink<T>(id);
                return true;
            }
            item = new FormLink<T>();
            return false;
        }

        public bool Parse<T>(
            MutagenFrame reader,
            [MaybeNullWhen(false)] out IFormLinkGetter<T> item)
            where T : class, IMajorRecordGetter
        {
            if (FormKeyBinaryTranslation.Instance.Parse(reader, out FormKey id))
            {
                item = new FormLink<T>(id);
                return true;
            }
            item = new FormLink<T>();
            return false;
        }

        public bool Parse<T>(
            MutagenFrame reader,
            [MaybeNullWhen(false)] out IFormLink<T> item)
            where T : class, IMajorRecordGetter
        {
            if (FormKeyBinaryTranslation.Instance.Parse(reader, out FormKey id))
            {
                item = new FormLink<T>(id);
                return true;
            }
            item = new FormLink<T>();
            return false;
        }

        public bool Parse<T>(
            MutagenFrame reader,
            [MaybeNullWhen(false)] out IFormLinkNullableGetter<T> item)
            where T : class, IMajorRecordGetter
        {
            if (FormKeyBinaryTranslation.Instance.Parse(reader, out FormKey id))
            {
                item = new FormLinkNullable<T>(id);
                return true;
            }
            item = new FormLinkNullable<T>();
            return false;
        }

        public bool Parse<T>(
            MutagenFrame reader,
            [MaybeNullWhen(false)] out IFormLinkNullable<T> item)
            where T : class, IMajorRecordGetter
        {
            if (FormKeyBinaryTranslation.Instance.Parse(reader, out FormKey id))
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
            where T : class, IMajorRecordGetter
        {
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKey);
        }

        public void Write<T>(
            MutagenWriter writer,
            FormLink<T> item,
            RecordType header)
            where T : class, IMajorRecordGetter
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
                throw SubrecordException.Enrich(ex, header);
            }
        }

        public void Write<T>(
            MutagenWriter writer,
            IFormLinkGetter<T> item,
            RecordType header)
            where T : class, IMajorRecordGetter
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
            where T : class, IMajorRecordGetter
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
            where T : class, IMajorRecordGetter
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
            where T : class, IMajorRecordGetter
        {
            if (item.FormKeyNullable == null) return;
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKeyNullable.Value);
        }

        public void WriteNullable<T>(
            MutagenWriter writer,
            IFormLinkNullableGetter<T> item)
            where T : class, IMajorRecordGetter
        {
            if (item.FormKeyNullable == null) return;
            FormKeyBinaryTranslation.Instance.Write(
                writer,
                item.FormKeyNullable.Value);
        }
    }
}
