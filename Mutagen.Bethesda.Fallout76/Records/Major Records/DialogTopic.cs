using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using RecordTypes = Mutagen.Bethesda.Fallout76.Internals.RecordTypes;

namespace Mutagen.Bethesda.Fallout76;

partial class DialogTopic
{
    [Flags]
    public enum MajorFlag
    {
    }
}

partial class DialogTopicBinaryCreateTranslation
{
    public static partial void CustomBinaryEndImport(MutagenFrame frame, IDialogTopicInternal obj)
    {
        try
        {
            if (frame.Reader.Complete) return;
            if (!frame.TryGetGroupHeader(out var groupMeta)) return;
            if (groupMeta.GroupType == (int)GroupTypeEnum.TopicChildren)
            {
                obj.Timestamp = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.LastModifiedData);
                obj.Unknown = frame.GetInt32(offset: 20);
                if (FormKey.Factory(
                        frame.MetaData.MasterReferences,
                        new FormID(BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData)),
                        reference: true) != obj.FormKey)
                {
                    throw RecordException.Enrich(
                        new ArgumentException("Dialog children group did not match the FormID of the parent."),
                        obj);
                }
            }
            else
            {
                return;
            }
            frame.Reader.Position += groupMeta.HeaderLength;
            obj.Responses.SetTo(ListBinaryTranslation<DialogResponses>.Instance.Parse(
                reader: frame.SpawnWithLength(groupMeta.ContentLength),
                transl: (MutagenFrame r, RecordType header, [MaybeNullWhen(false)] out DialogResponses listItem) =>
                {
                    return LoquiBinaryTranslation<DialogResponses>.Instance.Parse(
                        frame: r,
                        item: out listItem);
                }));
        }
        catch (Exception ex)
        {
            RecordException.EnrichAndThrow(ex, obj);
            throw;
        }
    }
}

partial class DialogTopicBinaryWriteTranslation
{
    public static partial void CustomBinaryEndExport(MutagenWriter writer, IDialogTopicGetter obj)
    {
        try
        {
            if (obj.Responses is not { } resp
                || resp.Count == 0)
            {
                return;
            }
            using (HeaderExport.Header(writer, RecordTypes.GRUP, ObjectType.Group))
            {
                FormKeyBinaryTranslation.Instance.Write(
                    writer,
                    obj);
                writer.Write((int)GroupTypeEnum.TopicChildren);
                writer.Write(obj.Timestamp);
                writer.Write(obj.Unknown);
                ListBinaryTranslation<IDialogResponsesGetter>.Instance.Write(
                    writer: writer,
                    items: resp,
                    transl: (MutagenWriter subWriter, IDialogResponsesGetter subItem) =>
                    {
                        subItem.WriteToBinary(subWriter);
                    });
            }
        }
        catch (Exception ex)
        {
            RecordException.EnrichAndThrow(ex, obj);
            throw;
        }
    }
}

partial class DialogTopicBinaryOverlay
{
    public IReadOnlyList<IDialogResponsesGetter> Responses { get; private set; } = [];

    private ReadOnlyMemorySlice<byte>? _grupData;

    public int Timestamp => _grupData != null ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.GroupHeader(_grupData.Value).LastModifiedData) : 0;

    public int Unknown => _grupData.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_grupData.Value.Slice(20)) : default;

    public ReadOnlyMemorySlice<Byte> SetAfterLoad => Array.Empty<byte>();

    partial void CustomEnd(OverlayStream stream, int finalPos, int offset)
    {
        try
        {
            if (stream.Complete) return;
            if (!stream.TryGetGroupHeader(out var groupMeta)) return;
            if (groupMeta.GroupType != (int)GroupTypeEnum.TopicChildren) return;
            this._grupData = stream.ReadMemory(checked((int)groupMeta.TotalLength));
            var formKey = FormKey.Factory(
                _package.MetaData.MasterReferences,
                new FormID(BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData)),
                reference: true);
            if (formKey != this.FormKey)
            {
                throw RecordException.Enrich(
                    new ArgumentException("Dialog children group did not match the FormID of the parent."),
                    this);
            }
            var contentSpan = this._grupData.Value.Slice(_package.MetaData.Constants.GroupConstants.HeaderLength);
            this.Responses = BinaryOverlayList.FactoryByArray<IDialogResponsesGetter>(
                contentSpan,
                _package,
                getter: (s, p) => DialogResponsesBinaryOverlay.DialogResponsesFactory(new OverlayStream(s, p), p),
                locs: ParseRecordLocations(
                    stream: new OverlayStream(contentSpan, _package),
                    trigger: DialogResponses_Registration.TriggeringRecordType,
                    constants: stream.MetaData.Constants.MajorConstants,
                    skipHeader: false));
        }
        catch (Exception ex)
        {
            RecordException.EnrichAndThrow(ex, this);
            throw;
        }
    }
}
