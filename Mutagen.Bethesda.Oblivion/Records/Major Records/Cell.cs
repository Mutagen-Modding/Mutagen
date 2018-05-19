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
        private byte[] _overallTimeStamp;
        private byte[] _persistentTimeStamp;
        private byte[] _temporaryTimeStamp;
        private byte[] _visibleWhenDistantTimeStamp;

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
            frame.Reader.Position += 8;
            var id = frame.Reader.ReadUInt32();
            var grupType = (GroupTypeEnum)frame.Reader.ReadInt32();
            if (grupType == GroupTypeEnum.CellChildren)
            {
                obj._overallTimeStamp = frame.Reader.ReadBytes(4);
                if (id != obj.FormID.ID)
                {
                    throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
                }
            }
            else
            {
                frame.Reader.Position -= 16;
                return;
            }
            using (var subFrame = frame.SpawnWithLength(len - Constants.RECORD_HEADER_LENGTH))
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
                    using (var itemFrame = frame.SpawnWithLength(persistLen))
                    {
                        switch (type)
                        {
                            case GroupTypeEnum.CellPersistentChildren:
                                obj.Persistent.SetIfSucceededOrDefault(ParseTypical(itemFrame, obj, errorMask, persistentParse: true));
                                break;
                            case GroupTypeEnum.CellTemporaryChildren:
                                ParseTemporary(itemFrame, obj, errorMask);
                                break;
                            case GroupTypeEnum.CellVisibleDistantChildren:
                                obj.VisibleWhenDistant.SetIfSucceededOrDefault(ParseTypical(itemFrame, obj, errorMask, persistentParse: false));
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                }
            }
        }

        static TryGet<IEnumerable<Placed>> ParseTypical(
            MutagenFrame frame,
            Cell obj, 
            Func<Cell_ErrorMask> errorMask,
            bool persistentParse)
        {
            frame.Reader.Position += 8;
            var id = frame.Reader.ReadUInt32();
            if (id != obj.FormID.ID)
            {
                throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
            }
            frame.Reader.Position += 4;
            if (persistentParse)
            {
                obj._persistentTimeStamp = frame.Reader.ReadBytes(4);
            }
            else
            {
                obj._visibleWhenDistantTimeStamp = frame.Reader.ReadBytes(4);
            }
            return Mutagen.Bethesda.Binary.ListBinaryTranslation<Placed, MaskItem<Exception, Placed_ErrorMask>>.Instance.ParseRepeatedItem(
                frame: frame,
                fieldIndex: (int)Cell_FieldIndex.Persistent,
                lengthLength: 4,
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

        static void ParseTemporaryOutliers(MutagenFrame frame, Cell obj, Func<Cell_ErrorMask> errorMask)
        {
            for (int i = 0; i < 2; i++)
            {
                var nextHeader = HeaderTranslation.GetNextRecordType(frame.Reader, out var pathLen);
                if (nextHeader.Equals(PathGrid_Registration.PGRD_HEADER))
                {
                    using (var subFrame = frame.SpawnWithLength(pathLen + Constants.RECORD_HEADER_LENGTH))
                    {
                        obj.PathGrid = PathGrid.Create_Binary(
                            subFrame,
                            out var subMask);
                        if (subMask != null)
                        {
                            errorMask().PathGrid = new MaskItem<Exception, PathGrid_ErrorMask>(null, subMask);
                        }
                    }
                }
                else if (nextHeader.Equals(Landscape_Registration.LAND_HEADER))
                {
                    using (var subFrame = frame.SpawnWithLength(pathLen + Constants.RECORD_HEADER_LENGTH))
                    {
                        obj.Landscape = Landscape.Create_Binary(
                            subFrame,
                            out var subMask);
                        if (subMask != null)
                        {
                            errorMask().Landscape = new MaskItem<Exception, Landscape_ErrorMask>(null, subMask);
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }

        static void ParseTemporary(MutagenFrame frame, Cell obj, Func<Cell_ErrorMask> errorMask)
        {
            frame.Reader.Position += 8;
            var id = frame.Reader.ReadUInt32();
            if (id != obj.FormID.ID)
            {
                throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
            }
            frame.Reader.Position += 4;
            obj._temporaryTimeStamp = frame.Reader.ReadBytes(4);
            ParseTemporaryOutliers(frame, obj, errorMask);
            var persistentTryGet = Mutagen.Bethesda.Binary.ListBinaryTranslation<Placed, MaskItem<Exception, Placed_ErrorMask>>.Instance.ParseRepeatedItem(
                frame: frame,
                fieldIndex: (int)Cell_FieldIndex.Persistent,
                lengthLength: 4,
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
            obj.Temporary.SetIfSucceededOrDefault(persistentTryGet);
        }

        static partial void CustomBinaryEnd_Export(MutagenWriter writer, Cell obj, Func<Cell_ErrorMask> errorMask)
        {
            if (obj.Persistent.Count == 0
                && obj.Temporary.Count == 0
                && obj.VisibleWhenDistant.Count == 0
                && !obj.PathGrid_Property.HasBeenSet
                && !obj.Landscape_Property.HasBeenSet) return;
            using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, ObjectType.Group))
            {
                writer.Write(obj.FormID.ID);
                writer.Write((int)GroupTypeEnum.CellChildren);
                if (obj._overallTimeStamp != null)
                {
                    writer.Write(obj._overallTimeStamp);
                }
                else
                {
                    writer.WriteZeros(4);
                }
                if (obj.Persistent.Count > 0)
                {
                    using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, ObjectType.Group))
                    {
                        writer.Write(obj.FormID.ID);
                        writer.Write((int)GroupTypeEnum.CellPersistentChildren);
                        if (obj._persistentTimeStamp != null)
                        {
                            writer.Write(obj._persistentTimeStamp);
                        }
                        else
                        {
                            writer.WriteZeros(4);
                        }
                        Mutagen.Bethesda.Binary.ListBinaryTranslation<Placed, MaskItem<Exception, Placed_ErrorMask>>.Instance.Write(
                            writer: writer,
                            item: obj.Persistent,
                            fieldIndex: (int)Cell_FieldIndex.Persistent,
                            errorMask: errorMask,
                            transl: (MutagenWriter subWriter, Placed subItem, bool listDoMasks, out MaskItem<Exception, Placed_ErrorMask> listSubMask) =>
                            {
                                LoquiBinaryTranslation<Placed, Placed_ErrorMask>.Instance.Write(
                                    writer: subWriter,
                                    item: subItem,
                                    doMasks: listDoMasks,
                                    errorMask: out listSubMask);
                            });
                    }
                }
                if (obj.Temporary.Count > 0
                    || obj.PathGrid_Property.HasBeenSet
                    || obj.Landscape_Property.HasBeenSet)
                {
                    using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, ObjectType.Group))
                    {
                        writer.Write(obj.FormID.ID);
                        writer.Write((int)GroupTypeEnum.CellTemporaryChildren);
                        if (obj._temporaryTimeStamp != null)
                        {
                            writer.Write(obj._temporaryTimeStamp);
                        }
                        else
                        {
                            writer.WriteZeros(4);
                        }
                        if (obj.Landscape_Property.HasBeenSet)
                        {
                            LoquiBinaryTranslation<Landscape, Landscape_ErrorMask>.Instance.Write(
                                writer,
                                obj.Landscape,
                                (int)Cell_FieldIndex.Landscape,
                                errorMask);
                        }
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
                            transl: (MutagenWriter subWriter, Placed subItem, bool listDoMasks, out MaskItem<Exception, Placed_ErrorMask> listSubMask) =>
                            {
                                LoquiBinaryTranslation<Placed, Placed_ErrorMask>.Instance.Write(
                                    writer: subWriter,
                                    item: subItem,
                                    doMasks: listDoMasks,
                                    errorMask: out listSubMask);
                            });
                    }
                }
                if (obj.VisibleWhenDistant.Count > 0)
                {
                    using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, ObjectType.Group))
                    {
                        writer.Write(obj.FormID.ID);
                        writer.Write((int)GroupTypeEnum.CellVisibleDistantChildren);
                        if (obj._visibleWhenDistantTimeStamp != null)
                        {
                            writer.Write(obj._visibleWhenDistantTimeStamp);
                        }
                        else
                        {
                            writer.WriteZeros(4);
                        }
                        Mutagen.Bethesda.Binary.ListBinaryTranslation<Placed, MaskItem<Exception, Placed_ErrorMask>>.Instance.Write(
                            writer: writer,
                            item: obj.VisibleWhenDistant,
                            fieldIndex: (int)Cell_FieldIndex.VisibleWhenDistant,
                            errorMask: errorMask,
                            transl: (MutagenWriter subWriter, Placed subItem, bool listDoMasks, out MaskItem<Exception, Placed_ErrorMask> listSubMask) =>
                            {
                                LoquiBinaryTranslation<Placed, Placed_ErrorMask>.Instance.Write(
                                    writer: subWriter,
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
