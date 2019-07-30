using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion.Internals;

namespace Mutagen.Bethesda.Oblivion
{
    namespace Internals
    {
        public partial class DialogTopicCommon
        {
            partial void PostDuplicate(DialogTopic obj, DialogTopic rhs, Func<FormKey> getNextFormKey, IList<(IMajorRecordCommon Record, FormKey OriginalFormKey)> duplicatedRecords)
            {
                obj.Items.SetTo(rhs.Items.Select((dia) => (DialogItem)dia.Duplicate(getNextFormKey, duplicatedRecords)));
            }
        }

        public partial class DialogTopicBinaryCreateTranslation
        { 
            static partial void CustomBinaryEndImport(MutagenFrame frame, DialogTopic obj, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (frame.Reader.Complete) return;
                GroupRecordMeta groupMeta = frame.MetaData.GetGroup(frame);
                if (!groupMeta.IsGroup) return;
                if (groupMeta.GroupType == (int)GroupTypeEnum.TopicChildren)
                {
                    obj.Timestamp = groupMeta.LastModifiedSpan.ToArray();
                    if (FormKey.Factory(masterReferences, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeSpan)) != obj.FormKey)
                    {
                        throw new ArgumentException("Dialog children group did not match the FormID of the parent.");
                    }
                }
                else
                {
                    return;
                }
                frame.Reader.Position += groupMeta.HeaderLength;
                Mutagen.Bethesda.Binary.ListBinaryTranslation<DialogItem>.Instance.ParseRepeatedItem(
                    frame: frame.SpawnWithLength(groupMeta.ContentLength),
                    fieldIndex: (int)DialogTopic_FieldIndex.Items,
                    lengthLength: 4,
                    item: obj.Items,
                    errorMask: errorMask,
                    transl: (MutagenFrame r, RecordType header, out DialogItem listItem, ErrorMaskBuilder listErrorMask) =>
                    {
                        return LoquiBinaryTranslation<DialogItem>.Instance.Parse(
                            frame: r,
                            item: out listItem,
                            masterReferences: masterReferences,
                            errorMask: listErrorMask);
                    }
                    );
            }
        }

        public partial class DialogTopicBinaryWriteTranslation
        {
            static partial void CustomBinaryEndExport(MutagenWriter writer, IDialogTopicInternalGetter obj, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (obj.Items.Count == 0) return;
                using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, ObjectType.Group))
                {
                    FormKeyBinaryTranslation.Instance.Write(
                        writer,
                        obj.FormKey,
                        masterReferences);
                    writer.Write((int)GroupTypeEnum.TopicChildren);
                    writer.Write(obj.Timestamp);
                    Mutagen.Bethesda.Binary.ListBinaryTranslation<IDialogItemInternalGetter>.Instance.Write(
                        writer: writer,
                        items: obj.Items,
                        fieldIndex: (int)DialogTopic_FieldIndex.Items,
                        errorMask: errorMask,
                        transl: (MutagenWriter subWriter, IDialogItemInternalGetter subItem, ErrorMaskBuilder listErrMask) =>
                        {
                            subItem.WriteToBinary(
                                 writer: subWriter,
                                 masterReferences: masterReferences,
                                 errorMask: listErrMask);
                        });
                }
            }

        }
    }
}
