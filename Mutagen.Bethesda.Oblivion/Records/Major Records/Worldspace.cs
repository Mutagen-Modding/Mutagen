using System;
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
    public partial class Worldspace
    {
        private byte[] _timeStamp;
        private bool usingOffsetLength;

        [Flags]
        public enum Flag
        {
            SmallWorld = 0x01,
            CantFastTravel = 0x02,
            OblivionWorldspace = 0x04,
            NoLODWater = 0x10,
        }

        static partial void FillBinary_OffsetLength_Custom(MutagenFrame frame, Worldspace item, ErrorMaskBuilder errorMask)
        {
            item.usingOffsetLength = true;
            if (!HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var xLen).Type.Equals("XXXX")
                || xLen != 4)
            {
                errorMask.ReportExceptionOrThrow(new ArgumentException());
                return;
            }
            var contentLen = frame.Reader.ReadInt32();
            if (!HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var oLen).Type.Equals("OFST")
                || oLen != 0)
            {
                errorMask.ReportExceptionOrThrow(new ArgumentException());
                return;
            }
            item.OffsetData = frame.Reader.ReadBytes(contentLen);
        }

        static partial void WriteBinary_OffsetLength_Custom(MutagenWriter writer, Worldspace item, ErrorMaskBuilder errorMask)
        {
            if (!item.OffsetData_Property.HasBeenSet) return;
            if (!item.usingOffsetLength) return;
            using (HeaderExport.ExportSubRecordHeader(writer, Worldspace_Registration.XXXX_HEADER))
            {
                writer.Write(item.OffsetData.Length);
            }
            writer.Write(Worldspace_Registration.OFST_HEADER.Type);
            writer.WriteZeros(2);
            writer.Write(item.OffsetData);
        }

        static partial void FillBinary_OffsetData_Custom(MutagenFrame frame, Worldspace item, int fieldIndex, ErrorMaskBuilder errorMask)
        {
            if (item.usingOffsetLength) return;
            if (!HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var len).Type.Equals("OFST"))
            {
                throw new ArgumentException();
            }
            item.OffsetData = frame.Reader.ReadBytes(len);
        }

        static partial void WriteBinary_OffsetData_Custom(MutagenWriter writer, Worldspace item, int fieldIndex, ErrorMaskBuilder errorMask)
        {
            if (item.usingOffsetLength) return;
            if (!item.OffsetData_Property.HasBeenSet) return;
            using (HeaderExport.ExportSubRecordHeader(writer, Worldspace_Registration.OFST_HEADER))
            {
                ByteArrayBinaryTranslation.Instance.Write(writer, item.OffsetData);
            }
        }

        static partial void CustomBinaryEnd_Import(MutagenFrame frame, Worldspace obj, ErrorMaskBuilder errorMask)
        {
            if (frame.Reader.Complete) return;
            var next = HeaderTranslation.GetNextType(frame.Reader, out var len, hopGroup: false);
            if (!next.Equals("GRUP")) return;
            frame.Reader.Position += 8;
            var id = frame.Reader.ReadUInt32();
            var grupType = (GroupTypeEnum)frame.Reader.ReadInt32();
            if (grupType == GroupTypeEnum.WorldChildren)
            {
                obj._timeStamp = frame.Reader.ReadBytes(4);
                if (id != obj.FormID.ID)
                {
                    errorMask.ReportExceptionOrThrow(
                        new ArgumentException("Cell children group did not match the FormID of the parent worldspace."));
                    return;
                }
            }
            else
            {
                frame.Reader.Position -= 16;
                return;
            }
            using (var subFrame = MutagenFrame.ByLength(frame.Reader, len - 20))
            {
                for (int i = 0; i < 3; i++)
                {
                    if (subFrame.Complete) return;
                    var subType = HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var subLen);
                    subFrame.Reader.Position -= 6;
                    switch (subType.Type)
                    {
                        case "ROAD":
                            LoquiBinaryTranslation<Road>.Instance.ParseInto(
                                frame: subFrame,
                                item: obj.Road_Property,
                                fieldIndex: (int)Worldspace_FieldIndex.Road,
                                errorMask: errorMask);
                            break;
                        case "CELL":
                            LoquiBinaryTranslation<Cell>.Instance.ParseInto(
                                frame: subFrame,
                                item: obj.TopCell_Property,
                                fieldIndex: (int)Worldspace_FieldIndex.TopCell,
                                errorMask: errorMask);
                            break;
                        case "GRUP":
                            Mutagen.Bethesda.Binary.ListBinaryTranslation<WorldspaceBlock>.Instance.ParseRepeatedItem(
                                frame: frame,
                                item: obj._SubCells,
                                triggeringRecord: Worldspace_Registration.GRUP_HEADER,
                                fieldIndex: (int)Worldspace_FieldIndex.SubCells,
                                lengthLength: Mutagen.Bethesda.Constants.RECORD_LENGTHLENGTH,
                                errorMask: errorMask,
                                transl: (MutagenFrame r, out WorldspaceBlock block, ErrorMaskBuilder subErrorMask) =>
                                {
                                    return LoquiBinaryTranslation<WorldspaceBlock>.Instance.Parse(
                                        frame: r.Spawn(snapToFinalPosition: false),
                                        item: out block,
                                        errorMask: errorMask);
                                });
                            break;
                        default:
                            return;
                    }
                }
            }
        }

        static partial void CustomBinaryEnd_Export(MutagenWriter writer, Worldspace obj, ErrorMaskBuilder errorMask)
        {
            if (obj._SubCells.Count == 0
                && !obj.Road_Property.HasBeenSet
                && !obj.TopCell_Property.HasBeenSet) return;
            using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, ObjectType.Group))
            {
                writer.Write(obj.FormID.ID);
                writer.Write((int)GroupTypeEnum.WorldChildren);
                if (obj._timeStamp != null)
                {
                    writer.Write(obj._timeStamp);
                }
                else
                {
                    writer.WriteZeros(4);
                }
                if (obj.Road_Property.HasBeenSet)
                {
                    LoquiBinaryTranslation<Road, Road_ErrorMask>.Instance.Write(
                        writer,
                        obj.Road,
                        (int)Worldspace_FieldIndex.Road,
                        errorMask);
                }
                if (obj.TopCell_Property.HasBeenSet)
                {
                    LoquiBinaryTranslation<Cell, Cell_ErrorMask>.Instance.Write(
                        writer,
                        obj.TopCell,
                        (int)Worldspace_FieldIndex.TopCell,
                        errorMask);
                }
                Mutagen.Bethesda.Binary.ListBinaryTranslation<WorldspaceBlock, MaskItem<Exception, WorldspaceBlock_ErrorMask>>.Instance.Write(
                    writer: writer,
                    item: obj.SubCells,
                    fieldIndex: (int)Worldspace_FieldIndex.SubCells,
                    errorMask: errorMask,
                    transl: (MutagenWriter subWriter, WorldspaceBlock subItem, bool listDoMasks, out MaskItem<Exception, WorldspaceBlock_ErrorMask> listSubMask) =>
                    {
                        LoquiBinaryTranslation<WorldspaceBlock, WorldspaceBlock_ErrorMask>.Instance.Write(
                            writer: subWriter,
                            item: subItem,
                            doMasks: listDoMasks,
                            errorMask: out listSubMask);
                    });
            }
        }
    }
}
