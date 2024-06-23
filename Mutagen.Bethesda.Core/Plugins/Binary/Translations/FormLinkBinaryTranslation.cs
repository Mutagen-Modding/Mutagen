using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public sealed class FormLinkBinaryTranslation
{
    public static readonly FormLinkBinaryTranslation Instance = new();

    public bool Parse<TReader>(
        TReader reader,
        out FormKey item,
        bool maxIsNone = false)
        where TReader : IMutagenReadStream
    {
        if (FormKeyBinaryTranslation.Instance.Parse(reader, out FormKey id, maxIsNone: maxIsNone))
        {
            item = id;
            return true;
        }
        item = FormKey.Null;
        return false;
    }

    public FormKey Parse(MutagenFrame reader, bool maxIsNone = false)
    {
        if (FormKeyBinaryTranslation.Instance.Parse(reader, out FormKey id, maxIsNone: maxIsNone))
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
            item);
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
                item,
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
            item,
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
            item,
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
            item,
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
            item);
    }

    public void WriteNullable<T>(
        MutagenWriter writer,
        IFormLinkNullableGetter<T> item)
        where T : class, IMajorRecordGetter
    {
        if (item.FormKeyNullable == null) return;
        FormKeyBinaryTranslation.Instance.Write(
            writer,
            item);
    }

    internal IFormLink<TMajorGetter> Factory<TMajorGetter>(ParsingMeta meta, SubrecordFrame frame, bool isSet = true, bool maxIsNull = false)
        where TMajorGetter : class, IMajorRecordGetter
    {
        return isSet ? new FormLink<TMajorGetter>(FormKeyBinaryTranslation.Instance.Parse(frame, meta.MasterReferences.Raw)) : new FormLink<TMajorGetter>();
    }

    internal IFormLink<TMajorGetter> Factory<TMajorGetter>(ParsingMeta meta, ReadOnlyMemorySlice<byte>? s, bool maxIsNull = false)
        where TMajorGetter : class, IMajorRecordGetter
    {
        return s.HasValue ? new FormLink<TMajorGetter>(FormKey.Factory(meta.MasterReferences.Raw, BinaryPrimitives.ReadUInt32LittleEndian(s.Value), maxIsNull: maxIsNull)) : new FormLink<TMajorGetter>();
    }

    internal IFormLinkNullable<TMajorGetter> FactoryNullable<TMajorGetter>(ParsingMeta meta, SubrecordFrame frame, bool isSet = true, bool maxIsNull = false)
        where TMajorGetter : class, IMajorRecordGetter
    {
        return isSet ? new FormLinkNullable<TMajorGetter>(FormKeyBinaryTranslation.Instance.Parse(frame, meta.MasterReferences.Raw)) : new FormLinkNullable<TMajorGetter>();
    }

    internal IFormLinkNullable<TMajorGetter> FactoryNullable<TMajorGetter>(ParsingMeta meta, ReadOnlyMemorySlice<byte>? s, bool maxIsNull = false)
        where TMajorGetter : class, IMajorRecordGetter
    {
        return s.HasValue ? new FormLinkNullable<TMajorGetter>(FormKey.Factory(meta.MasterReferences.Raw, BinaryPrimitives.ReadUInt32LittleEndian(s.Value), maxIsNull: maxIsNull)) : new FormLinkNullable<TMajorGetter>();
    }

    internal IFormLinkGetter<TMajorGetter> OverlayFactory<TMajorGetter>(BinaryOverlayFactoryPackage p, ReadOnlySpan<byte> s)
        where TMajorGetter : class, IMajorRecordGetter
    {
        return new FormLink<TMajorGetter>(FormKey.Factory(p.MetaData.MasterReferences.Raw, BinaryPrimitives.ReadUInt32LittleEndian(s), maxIsNull: false));
    }

    internal IFormLinkGetter<TMajorGetter> OverlayFactory<TMajorGetter>(BinaryOverlayFactoryPackage p, ReadOnlySpan<byte> s, bool isSet, bool maxIsNull = false)
        where TMajorGetter : class, IMajorRecordGetter
    {
        return isSet ? new FormLink<TMajorGetter>(FormKey.Factory(p.MetaData.MasterReferences.Raw, BinaryPrimitives.ReadUInt32LittleEndian(s), maxIsNull: maxIsNull)) : FormLinkGetter<TMajorGetter>.Null;
    }

    internal IFormLinkGetter<TMajorGetter> OverlayFactory<TMajorGetter>(BinaryOverlayFactoryPackage p, ReadOnlyMemorySlice<byte>? s, bool maxIsNull = false)
        where TMajorGetter : class, IMajorRecordGetter
    {
        return s.HasValue ? new FormLink<TMajorGetter>(FormKey.Factory(p.MetaData.MasterReferences.Raw, BinaryPrimitives.ReadUInt32LittleEndian(s.Value), maxIsNull: maxIsNull)) : FormLinkGetter<TMajorGetter>.Null;
    }

    internal IFormLinkNullableGetter<TMajorGetter> NullableOverlayFactory<TMajorGetter>(BinaryOverlayFactoryPackage p, ReadOnlySpan<byte> s)
        where TMajorGetter : class, IMajorRecordGetter
    {
        return new FormLinkNullable<TMajorGetter>(FormKey.Factory(p.MetaData.MasterReferences.Raw, BinaryPrimitives.ReadUInt32LittleEndian(s)));
    }

    internal IFormLinkNullableGetter<TMajorGetter> NullableOverlayFactory<TMajorGetter>(BinaryOverlayFactoryPackage p, ReadOnlyMemorySlice<byte> s, int? loc)
        where TMajorGetter : class, IMajorRecordGetter
    {
        return loc.HasValue ? new FormLinkNullable<TMajorGetter>(FormKey.Factory(p.MetaData.MasterReferences.Raw, BinaryPrimitives.ReadUInt32LittleEndian(HeaderTranslation.ExtractSubrecordMemory(s, loc.Value, p.MetaData.Constants)))) : FormLinkNullableGetter<TMajorGetter>.Null;
    }

    internal IFormLinkNullableGetter<TMajor> NullableRecordOverlayFactory<TMajor>(
        BinaryOverlayFactoryPackage package,
        ReadOnlyMemorySlice<byte> recordData,
        int? location,
        bool maxIsNull = false)
        where TMajor : class, IMajorRecordGetter
    {
        return location.HasValue ? 
            new FormLinkNullable<TMajor>(
                FormKey.Factory(
                    package.MetaData.MasterReferences.Raw,
                    BinaryPrimitives.ReadUInt32LittleEndian(HeaderTranslation.ExtractSubrecordMemory(recordData, location.Value, package.MetaData.Constants)),
                    maxIsNull: maxIsNull)) : 
            FormLinkNullable<TMajor>.Null;
    }
}