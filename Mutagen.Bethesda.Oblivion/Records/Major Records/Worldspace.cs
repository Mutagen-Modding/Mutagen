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
    public partial class Worldspace
    {
        private byte[] _timeStamp;

        [Flags]
        public enum Flag
        {
            SmallWorld = 0x01,
            CantFastTravel = 0x02,
            OblivionWorldspace = 0x04,
            NoLODWater = 0x10,
        }

        static partial void FillBinary_OffsetLength_Custom(MutagenFrame frame, Worldspace item, Func<Worldspace_ErrorMask> errorMask)
        {
            if (!HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var xLen).Type.Equals("XXXX")
                || xLen != 4)
            {
                throw new ArgumentException();
            }
            var contentLen = frame.Reader.ReadInt32();
            if (!HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var oLen).Type.Equals("OFST")
                || oLen != 0)
            {
                throw new ArgumentException();
            }
            item.OffsetData = frame.Reader.ReadBytes(contentLen);
        }

        static partial void WriteBinary_OffsetLength_Custom(MutagenWriter writer, Worldspace item, Func<Worldspace_ErrorMask> errorMask)
        {
            using (HeaderExport.ExportSubRecordHeader(writer, Worldspace_Registration.XXXX_HEADER))
            {
                writer.Write(item.OffsetData.Length);
            }
            writer.Write(Worldspace_Registration.OFST_HEADER.Type);
            writer.WriteZeros(2);
            writer.Write(item.OffsetData);
        }

        static partial void CustomBinaryEnd_Import(MutagenFrame frame, Worldspace obj, Func<Worldspace_ErrorMask> errorMask)
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
                    throw new ArgumentException("Cell children group did not match the FormID of the parent worldspace.");
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
                            obj.Road_Property.SetIfSucceededOrDefault(LoquiBinaryTranslation<Road, Road_ErrorMask>.Instance.Parse(
                                frame: subFrame,
                                fieldIndex: (int)Worldspace_FieldIndex.Road,
                                errorMask: errorMask));
                            break;
                        case "CELL":
                            obj.TopCell_Property.SetIfSucceededOrDefault(LoquiBinaryTranslation<Cell, Cell_ErrorMask>.Instance.Parse(
                                frame: subFrame,
                                fieldIndex: (int)Worldspace_FieldIndex.TopCell,
                                errorMask: errorMask));
                            break;
                        case "GRUP":
                            obj._SubCells.SetIfSucceededOrDefault(Mutagen.Bethesda.Binary.ListBinaryTranslation<WorldspaceBlock, MaskItem<Exception, WorldspaceBlock_ErrorMask>>.Instance.ParseRepeatedItem(
                                frame: frame,
                                triggeringRecord: Worldspace_Registration.GRUP_HEADER,
                                fieldIndex: (int)Worldspace_FieldIndex.SubCells,
                                lengthLength: Mutagen.Bethesda.Constants.RECORD_LENGTHLENGTH,
                                errorMask: errorMask,
                                transl: (MutagenFrame r, bool listDoMasks, out MaskItem<Exception, WorldspaceBlock_ErrorMask> listSubMask) =>
                                {
                                    return LoquiBinaryTranslation<WorldspaceBlock, WorldspaceBlock_ErrorMask>.Instance.Parse(
                                        frame: r.Spawn(snapToFinalPosition: false),
                                        doMasks: listDoMasks,
                                        errorMask: out listSubMask);
                                }));
                            break;
                        default:
                            return;
                    }
                }
            }
        }

        static partial void CustomBinaryEnd_Export(MutagenWriter writer, Worldspace obj, Func<Worldspace_ErrorMask> errorMask)
        {
            throw new NotImplementedException();
        }
    }
}
