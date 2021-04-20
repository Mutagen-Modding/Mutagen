using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Constants;
using Mutagen.Bethesda.Records.Binary.Overlay;
using Mutagen.Bethesda.Records.Binary.Streams;
using Noggog;

namespace Mutagen.Bethesda.Oblivion
{
    namespace Internals
    {
        public partial class DialogTopicBinaryCreateTranslation
        {
            static partial void CustomBinaryEndImport(MutagenFrame frame, IDialogTopicInternal obj)
            {
                try
                {
                    if (frame.Reader.Complete) return;
                    if (!frame.TryGetGroup(out var groupMeta)) return;
                    if (groupMeta.GroupType == (int)GroupTypeEnum.TopicChildren)
                    {
                        obj.Timestamp = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.LastModifiedData);
                        if (FormKey.Factory(frame.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData)) != obj.FormKey)
                        {
                            throw new ArgumentException("Dialog children group did not match the FormID of the parent.");
                        }
                    }
                    else
                    {
                        return;
                    }
                    frame.Reader.Position += groupMeta.HeaderLength;
                    obj.Items.SetTo(Mutagen.Bethesda.Binary.ListBinaryTranslation<DialogItem>.Instance.Parse(
                        frame: frame.SpawnWithLength(groupMeta.ContentLength),
                        transl: (MutagenFrame r, RecordType header, out DialogItem listItem) =>
                        {
                            return LoquiBinaryTranslation<DialogItem>.Instance.Parse(
                                frame: r,
                                item: out listItem);
                        }));
                }
                catch (Exception ex)
                {
                    throw RecordException.Enrich(ex, obj);
                }
            }
        }

        public partial class DialogTopicBinaryWriteTranslation
        {
            static partial void CustomBinaryEndExport(MutagenWriter writer, IDialogTopicGetter obj)
            {
                try
                {
                    if (!obj.Items.TryGet(out var items)
                        || items.Count == 0) return;
                    using (HeaderExport.Header(writer, RecordTypes.GRUP, ObjectType.Group))
                    {
                        FormKeyBinaryTranslation.Instance.Write(
                            writer,
                            obj.FormKey);
                        writer.Write((int)GroupTypeEnum.TopicChildren);
                        writer.Write(obj.Timestamp);
                        Mutagen.Bethesda.Binary.ListBinaryTranslation<IDialogItemGetter>.Instance.Write(
                            writer: writer,
                            items: items,
                            transl: (MutagenWriter subWriter, IDialogItemGetter subItem) =>
                            {
                                subItem.WriteToBinary(subWriter);
                            });
                    }
                }
                catch (Exception ex)
                {
                    throw RecordException.Enrich(ex, obj);
                }
            }
        }

        public partial class DialogTopicBinaryOverlay
        {
            private ReadOnlyMemorySlice<byte>? _grupData;

            public int Timestamp => _grupData != null ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.Group(_grupData.Value).LastModifiedData) : 0;

            public IReadOnlyList<IDialogItemGetter> Items { get; private set; } = ListExt.Empty<IDialogItemGetter>();

            partial void CustomEnd(OverlayStream stream, int finalPos, int offset)
            {
                try
                {
                    if (stream.Complete) return;
                    var startPos = stream.Position;
                    if (!stream.TryGetGroup(out var groupMeta)) return;
                    if (groupMeta.GroupType != (int)GroupTypeEnum.TopicChildren) return;
                    this._grupData = stream.ReadMemory(checked((int)groupMeta.TotalLength));
                    var formKey = FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData));
                    if (formKey != this.FormKey)
                    {
                        throw new ArgumentException("Dialog children group did not match the FormID of the parent.");
                    }
                    var contentSpan = this._grupData.Value.Slice(_package.MetaData.Constants.GroupConstants.HeaderLength);
                    this.Items = BinaryOverlayList.FactoryByArray<IDialogItemGetter>(
                        contentSpan,
                        _package,
                        getter: (s, p) => DialogItemBinaryOverlay.DialogItemFactory(new OverlayStream(s, p), p),
                        locs: ParseRecordLocations(
                            stream: new OverlayStream(contentSpan, _package),
                            trigger: DialogItem_Registration.TriggeringRecordType,
                            constants: GameConstants.Oblivion.MajorConstants,
                            skipHeader: false));
                }
                catch (Exception ex)
                {
                    throw RecordException.Enrich(ex, this);
                }
            }
        }
    }
}
