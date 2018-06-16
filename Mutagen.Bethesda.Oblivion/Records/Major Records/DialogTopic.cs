using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion.Internals;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class DialogTopic
    {
        private byte[] _overallTimeStamp;

        static partial void CustomBinaryEnd_Import(MutagenFrame frame, DialogTopic obj, Func<DialogTopic_ErrorMask> errorMask)
        {
            if (frame.Reader.Complete) return;
            var next = HeaderTranslation.GetNextType(frame.Reader, out var len, hopGroup: false);
            if (!next.Equals("GRUP")) return;
            frame.Reader.Position += 8;
            var id = frame.Reader.ReadUInt32();
            var grupType = (GroupTypeEnum)frame.Reader.ReadInt32();
            if (grupType == GroupTypeEnum.TopicChildren)
            {
                obj._overallTimeStamp = frame.Reader.ReadBytes(4);
                if (id != obj.FormID.ID)
                {
                    throw new ArgumentException("Dialog children group did not match the FormID of the parent.");
                }
            }
            else
            {
                frame.Reader.Position -= 16;
                return;
            }
            using (var subFrame = frame.SpawnWithLength(len - Constants.RECORD_HEADER_LENGTH))
            {
                obj.Items.SetIfSucceeded(Mutagen.Bethesda.Binary.ListBinaryTranslation<DialogItem, MaskItem<Exception, DialogItem_ErrorMask>>.Instance.ParseRepeatedItem(
                    frame: subFrame,
                    fieldIndex: (int)DialogTopic_FieldIndex.Items,
                    lengthLength: 4,
                    errorMask: errorMask,
                    transl: (MutagenFrame r, RecordType header, bool listDoMasks, out MaskItem<Exception, DialogItem_ErrorMask> listSubMask) =>
                    {
                        return LoquiBinaryTranslation<DialogItem, DialogItem_ErrorMask>.Instance.Parse(
                            frame: r,
                            doMasks: listDoMasks,
                            errorMask: out listSubMask);
                    }
                    ));
            }
        }

        static partial void CustomBinaryEnd_Export(MutagenWriter writer, DialogTopic obj, Func<DialogTopic_ErrorMask> errorMask)
        {
            if (obj.Items.Count == 0) return;
            using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, ObjectType.Group))
            {
                writer.Write(obj.FormID.ID);
                writer.Write((int)GroupTypeEnum.TopicChildren);
                if (obj._overallTimeStamp != null)
                {
                    writer.Write(obj._overallTimeStamp);
                }
                else
                {
                    writer.WriteZeros(4);
                }
                Mutagen.Bethesda.Binary.ListBinaryTranslation<DialogItem, MaskItem<Exception, DialogItem_ErrorMask>>.Instance.Write(
                    writer: writer,
                    item: obj.Items,
                    fieldIndex: (int)DialogTopic_FieldIndex.Items,
                    errorMask: errorMask,
                    transl: (MutagenWriter subWriter, DialogItem subItem, bool listDoMasks, out MaskItem<Exception, DialogItem_ErrorMask> listSubMask) =>
                    {
                        LoquiBinaryTranslation<DialogItem, DialogItem_ErrorMask>.Instance.Write(
                            writer: subWriter,
                            item: subItem,
                            doMasks: listDoMasks,
                            errorMask: out listSubMask);
                    });
            }
        }
    }
}
