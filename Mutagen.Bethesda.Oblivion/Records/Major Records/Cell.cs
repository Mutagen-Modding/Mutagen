using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Cell
    {
        [Flags]
        public enum Flag
        {
            IsInteriorCell = 0x0001,
            HasWater = 0x0002,
            InvertFastTravelBehavior = 0x0004,
            ForceHideLand = 0x0008,
            PublicPlace = 0x0020,
            HandChanged = 0x0040,
            BehaveLikeExteriod = 0x0080,
        }

        static partial void CustomBinaryEnd_Import(MutagenFrame frame, Cell obj, Func<Cell_ErrorMask> errorMask)
        {
            if (frame.Reader.Complete) return;
            var next = HeaderTranslation.GetNextType(frame.Reader, out var len, hopGroup: false);
            if (!next.Equals("GRUP")) return;
            frame.Reader.Position += 12;
            var grupType = (GroupTypeEnum)frame.Reader.ReadInt32();
            if (grupType == GroupTypeEnum.CellChildren)
            {
                frame.Reader.Position += 4;
            }
            else
            {
                frame.Reader.Position -= 16;
                return;
            }
            using (var subFrame = frame.Spawn(len - Constants.RECORD_HEADER_LENGTH))
            {
                while (!subFrame.Complete)
                {
                    var persistGroup = HeaderTranslation.GetNextType(frame.Reader, out var persistLen, hopGroup: false);
                    if (!persistGroup.Equals("GRUP"))
                    {
                        throw new ArgumentException();
                    }
                    subFrame.Reader.Position += 12;
                    GroupTypeEnum type = (GroupTypeEnum)subFrame.Reader.ReadUInt32();
                    subFrame.Reader.Position -= 16;
                    using (var itemFrame = frame.Spawn(persistLen))
                    {
                        switch (type)
                        {
                            case GroupTypeEnum.CellPersistentChildren:
                                obj.Persistent.SetIfSucceeded(ParsePersistent(itemFrame, obj, errorMask));
                                break;
                            case GroupTypeEnum.CellTemporaryChildren:
                                ParseTemporary(itemFrame, obj, errorMask);
                                break;
                            case GroupTypeEnum.CellVisibleDistantChildren:
                                obj.VisibleWhenDistant.SetIfSucceeded(ParsePersistent(itemFrame, obj, errorMask));
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                }
            }
        }

        static TryGet<IEnumerable<Placed>> ParsePersistent(MutagenFrame frame, Cell obj, Func<Cell_ErrorMask> errorMask)
        {
            frame.Reader.Position += Constants.GRUP_LENGTH;
            return Mutagen.Bethesda.Binary.ListBinaryTranslation<Placed, MaskItem<Exception, Placed_ErrorMask>>.Instance.ParseRepeatedItem(
                frame: frame,
                fieldIndex: (int)Cell_FieldIndex.Persistent,
                lengthLength: new ContentLength(4),
                errorMask: errorMask,
                transl: (MutagenFrame r, RecordType header, bool listDoMasks, out MaskItem<Exception, Placed_ErrorMask> listSubMask) =>
                {
                    switch (header.Type)
                    {
                        case "ACRE":
                            return LoquiBinaryTranslation<PlacedCreature, PlacedCreature_ErrorMask>.Instance.Parse(
                                frame: r,
                                doMasks: listDoMasks,
                                errorMask: out listSubMask).Bubble<PlacedCreature, Placed>();
                        case "ACHR":
                            return LoquiBinaryTranslation<PlacedNPC, PlacedNPC_ErrorMask>.Instance.Parse(
                                frame: r,
                                doMasks: listDoMasks,
                                errorMask: out listSubMask).Bubble<PlacedNPC, Placed>();
                        case "REFR":
                            return LoquiBinaryTranslation<PlacedObject, PlacedObject_ErrorMask>.Instance.Parse(
                                frame: r,
                                doMasks: listDoMasks,
                                errorMask: out listSubMask).Bubble<PlacedObject, Placed>();
                        default:
                            throw new NotImplementedException();
                    }
                }
                );
        }

        static void ParseTemporary(MutagenFrame frame, Cell obj, Func<Cell_ErrorMask> errorMask)
        {
            frame.Reader.Position += Constants.GRUP_LENGTH;
            var pathGridHeader = HeaderTranslation.GetNextRecordType(frame.Reader, out var pathLen);
            if (pathGridHeader.Equals(PathGrid_Registration.PGRD_HEADER))
            {
                using (var pathGridFrame = frame.Spawn(pathLen + Constants.RECORD_HEADER_LENGTH))
                {
                    obj.PathGrid = PathGrid.Create_Binary(
                        pathGridFrame,
                        out var pathGridMask);
                    if (pathGridMask != null)
                    {
                        errorMask().PathGrid = new MaskItem<Exception, PathGrid_ErrorMask>(null, pathGridMask);
                    }
                }
            }
            var persistentTryGet = Mutagen.Bethesda.Binary.ListBinaryTranslation<Placed, MaskItem<Exception, Placed_ErrorMask>>.Instance.ParseRepeatedItem(
                frame: frame,
                fieldIndex: (int)Cell_FieldIndex.Persistent,
                lengthLength: new ContentLength(4),
                errorMask: errorMask,
                transl: (MutagenFrame r, RecordType header, bool listDoMasks, out MaskItem<Exception, Placed_ErrorMask> listSubMask) =>
                {
                    switch (header.Type)
                    {
                        case "ACRE":
                            return LoquiBinaryTranslation<PlacedCreature, PlacedCreature_ErrorMask>.Instance.Parse(
                                frame: r,
                                doMasks: listDoMasks,
                                errorMask: out listSubMask).Bubble<PlacedCreature, Placed>();
                        case "ACHR":
                            return LoquiBinaryTranslation<PlacedNPC, PlacedNPC_ErrorMask>.Instance.Parse(
                                frame: r,
                                doMasks: listDoMasks,
                                errorMask: out listSubMask).Bubble<PlacedNPC, Placed>();
                        case "REFR":
                            return LoquiBinaryTranslation<PlacedObject, PlacedObject_ErrorMask>.Instance.Parse(
                                frame: r,
                                doMasks: listDoMasks,
                                errorMask: out listSubMask).Bubble<PlacedObject, Placed>();
                        default:
                            throw new NotImplementedException();
                    }
                }
                );
            obj.Temporary.SetIfSucceeded(persistentTryGet);
        }

        static partial void CustomBinaryEnd_Export(MutagenWriter writer, Cell obj, Func<Cell_ErrorMask> errorMask)
        {
            if (obj.Persistent.Count == 0
                && obj.Temporary.Count == 0
                && obj.VisibleWhenDistant.Count == 0
                && !obj.PathGrid_Property.HasBeenSet) return;
            using (HeaderExport.ExportRecordHeader(writer, Group_Registration.GRUP_HEADER))
            {
                writer.WriteZeros(4);
                writer.Write((int)GroupTypeEnum.CellChildren);
                writer.WriteZeros(4);
                if (obj.Persistent.Count > 0)
                {
                    using (HeaderExport.ExportRecordHeader(writer, Group_Registration.GRUP_HEADER))
                    {
                        writer.WriteZeros(4);
                        writer.Write((int)GroupTypeEnum.CellPersistentChildren);
                        writer.WriteZeros(4);
                        Mutagen.Bethesda.Binary.ListBinaryTranslation<Placed, MaskItem<Exception, Placed_ErrorMask>>.Instance.Write(
                            writer: writer,
                            item: obj.Persistent,
                            fieldIndex: (int)Cell_FieldIndex.Persistent,
                            errorMask: errorMask,
                            transl: (Placed subItem, bool listDoMasks, out MaskItem<Exception, Placed_ErrorMask> listSubMask) =>
                            {
                                LoquiBinaryTranslation<Placed, Placed_ErrorMask>.Instance.Write(
                                    writer: writer,
                                    item: subItem,
                                    doMasks: listDoMasks,
                                    errorMask: out listSubMask);
                            });
                    }
                }
                if (obj.Temporary.Count > 0
                    || obj.PathGrid_Property.HasBeenSet)
                {
                    using (HeaderExport.ExportRecordHeader(writer, Group_Registration.GRUP_HEADER))
                    {
                        writer.WriteZeros(4);
                        writer.Write((int)GroupTypeEnum.CellTemporaryChildren);
                        writer.WriteZeros(4);
                        if (obj.PathGrid_Property.HasBeenSet)
                        {
                            LoquiBinaryTranslation<PathGrid, PathGrid_ErrorMask>.Instance.Write(
                                writer,
                                obj.PathGrid,
                                (int)Cell_FieldIndex.PathGrid,
                                errorMask);
                        }
                        Mutagen.Bethesda.Binary.ListBinaryTranslation<Placed, MaskItem<Exception, Placed_ErrorMask>>.Instance.Write(
                            writer: writer,
                            item: obj.Temporary,
                            fieldIndex: (int)Cell_FieldIndex.Temporary,
                            errorMask: errorMask,
                            transl: (Placed subItem, bool listDoMasks, out MaskItem<Exception, Placed_ErrorMask> listSubMask) =>
                            {
                                LoquiBinaryTranslation<Placed, Placed_ErrorMask>.Instance.Write(
                                    writer: writer,
                                    item: subItem,
                                    doMasks: listDoMasks,
                                    errorMask: out listSubMask);
                            });
                    }
                }
                if (obj.Persistent.Count > 0)
                {
                    using (HeaderExport.ExportRecordHeader(writer, Group_Registration.GRUP_HEADER))
                    {
                        writer.WriteZeros(4);
                        writer.Write((int)GroupTypeEnum.CellVisibleDistantChildren);
                        writer.WriteZeros(4);
                        Mutagen.Bethesda.Binary.ListBinaryTranslation<Placed, MaskItem<Exception, Placed_ErrorMask>>.Instance.Write(
                            writer: writer,
                            item: obj.VisibleWhenDistant,
                            fieldIndex: (int)Cell_FieldIndex.VisibleWhenDistant,
                            errorMask: errorMask,
                            transl: (Placed subItem, bool listDoMasks, out MaskItem<Exception, Placed_ErrorMask> listSubMask) =>
                            {
                                LoquiBinaryTranslation<Placed, Placed_ErrorMask>.Instance.Write(
                                    writer: writer,
                                    item: subItem,
                                    doMasks: listDoMasks,
                                    errorMask: out listSubMask);
                            });
                    }
                }
            }
        }
    }
}
