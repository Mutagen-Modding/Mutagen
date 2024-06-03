using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Translations.Binary;
using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;

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
        FormKey item,
        bool nullable = false)
    {
        if (item == FormKey.None)
        {
            UInt32BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
                writer: writer,
                item: uint.MaxValue);
            return;
        }
        if (writer.MetaData.CleanNulls && item.IsNull)
        {
            item = FormKey.Null;
        }
        UInt32BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
            writer: writer,
            item: GetFormID(writer.MetaData.MasterReferences!, item).Raw);
    }

    private FormID GetFormID(IReadOnlyMasterReferenceCollection masterIndices, FormKey key)
    {
        if (masterIndices.TryGetIndex(key.ModKey, out var index))
        {
            return new FormID(
                index,
                key.ID);
        }
        if (key == FormKey.Null)
        {
            return FormID.Null;
        }
        throw new UnmappableFormIDException(
            key, 
            masterIndices.Masters
                .Select(x => x.Master)
                .ToArray());
    }

    public void Write(
        MutagenWriter writer,
        FormKey item,
        RecordType header,
        bool nullable = false)
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
}