using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Translations.Binary;
using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public sealed class FormKeyBinaryTranslation
{
    public static readonly FormKeyBinaryTranslation Instance = new();

    public FormKey Parse(
        ReadOnlySpan<byte> span,
        IReadOnlyMasterReferenceCollection masterReferences,
        bool maxIsNone = false)
    {
        var id = BinaryPrimitives.ReadUInt32LittleEndian(span);
        if (maxIsNone && id == uint.MaxValue)
        {
            return FormKey.None;
        }

        return FormKey.Factory(masterReferences, id);
    }

    public bool Parse<TReader>(
        TReader reader,
        out FormKey item,
        bool maxIsNone = false)
        where TReader : IMutagenReadStream
    {
        item = Parse(
            reader.ReadSpan(4),
            reader.MetaData.MasterReferences!,
            maxIsNone: maxIsNone);
        return true;
    }

    public FormKey Parse<TReader>(TReader reader)
        where TReader : IMutagenReadStream
    {
        return Parse(
            reader.ReadSpan(4),
            reader.MetaData.MasterReferences!);
    }

    public void Write(
        MutagenWriter writer,
        IFormLinkIdentifier item,
        bool nullable = false)
    {
        if (item.FormKey == FormKey.None)
        {
            UInt32BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
                writer: writer,
                item: uint.MaxValue);
            return;
        }

        if (writer.MetaData.CleanNulls && item.FormKey.IsNull)
        {
            item = FormLinkInformation.Null;
        }

        var formID = GetFormID(
            writer.MetaData.Header!,
            writer.MetaData.MasterReferences!,
            item);

        UInt32BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
            writer: writer,
            item: formID.Raw);
    }

    private FormID GetFormID(
        IModFlagsGetter? modFlags,
        IReadOnlyMasterReferenceCollection masterIndices,
        IFormLinkIdentifier key)
    {
        if (modFlags != null && key.FormKey.ModKey == masterIndices.CurrentMod)
        {
            bool isLightMaster = modFlags.CanBeLightMaster
                                 && modFlags.IsLightMaster;
            bool isHalfMaster = modFlags.CanBeHalfMaster
                                && modFlags.IsHalfMaster;
            if (isLightMaster && isHalfMaster)
            {
                throw new NotImplementedException("Mod cannot be both a light and half master");
            }

            if (isLightMaster)
            {
                return new FormID(
                    ModIndex.LightMaster,
                    key.FormKey.ID);
            }

            if (isHalfMaster)
            {
                return new FormID(
                    ModIndex.HalfMaster,
                    key.FormKey.ID);
            }
        }

        if (masterIndices.TryGetIndex(key.FormKey.ModKey, out var index))
        {
            return new FormID(
                index,
                key.FormKey.ID);
        }

        if (key.FormKey == FormKey.Null)
        {
            return FormID.Null;
        }

        throw new UnmappableFormIDException(
            key.FormKey,
            masterIndices.Masters
                .Select(x => x.Master)
                .ToArray());
    }

    public void Write(
        MutagenWriter writer,
        IFormLinkIdentifier item,
        RecordType header)
    {
        try
        {
            using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
            {
                Write(
                    writer,
                    item);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, header);
        }
    }

    public void Write<TMajor>(
        MutagenWriter writer,
        IFormLinkNullableGetter<TMajor> item,
        RecordType header)
        where TMajor : class, IMajorRecordGetter
    {
        if (item.FormKeyNullable == null) return;
        Write(writer, item.ToStandardizedIdentifier(), header);
    }
}