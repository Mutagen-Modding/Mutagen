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
using Noggog;
using Noggog.Notifying;

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

        static partial void CustomBinaryEnd_Import(MutagenFrame frame, Cell obj, ErrorMaskBuilder errorMask)
        {
            if (frame.Reader.Complete) return;
            var next = HeaderTranslation.GetNextType(frame.Reader, out var len, hopGroup: false);
            if (!next.Equals(Group_Registration.GRUP_HEADER)) return;
            frame.Reader.Position += 8;
            var id = FormID.Factory(frame.Reader.ReadUInt32());
            var grupType = (GroupTypeEnum)frame.Reader.ReadInt32();
            if (grupType == GroupTypeEnum.CellChildren)
            {
                obj._overallTimeStamp = frame.Reader.ReadBytes(4);
                if (id != obj.FormID)
                {
                    throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
                }
            }
            else
            {
                frame.Reader.Position -= 16;
                return;
            }
            using (var subFrame = frame.SpawnWithLength(len - Mutagen.Bethesda.Constants.RECORD_HEADER_LENGTH))
            {
                while (!subFrame.Complete)
                {
                    var persistGroup = HeaderTranslation.GetNextType(frame.Reader, out var persistLen, hopGroup: false);
                    if (!persistGroup.Equals(Group_Registration.GRUP_HEADER))
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
                                ParseTypical(
                                    frame: itemFrame, 
                                    obj: obj,
                                    fieldIndex: (int)Cell_FieldIndex.Persistent,
                                    coll: obj.Persistent,
                                    errorMask: errorMask, 
                                    persistentParse: true);
                                break;
                            case GroupTypeEnum.CellTemporaryChildren:
                                ParseTemporary(
                                    itemFrame, 
                                    obj,
                                    errorMask);
                                break;
                            case GroupTypeEnum.CellVisibleDistantChildren:
                                ParseTypical(
                                    frame: itemFrame, 
                                    obj: obj, 
                                    fieldIndex: (int)Cell_FieldIndex.VisibleWhenDistant,
                                    coll: obj.VisibleWhenDistant,
                                    errorMask: errorMask, 
                                    persistentParse: false);
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                }
            }
        }

        static void ParseTypical(
            MutagenFrame frame,
            Cell obj,
            int fieldIndex,
            INotifyingCollection<Placed> coll,
            ErrorMaskBuilder errorMask,
            bool persistentParse)
        {
            frame.Reader.Position += 8;
            var id = FormID.Factory(frame.Reader.ReadUInt32());
            if (id != obj.FormID)
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
            Mutagen.Bethesda.Binary.ListBinaryTranslation<Placed>.Instance.ParseRepeatedItem(
                frame: frame,
                fieldIndex: fieldIndex,
                item: coll,
                lengthLength: 4,
                errorMask: errorMask,
                transl: (MutagenFrame r, RecordType header, out Placed placed, ErrorMaskBuilder errMaskInternal) =>
                {
                    switch (header.TypeInt)
                    {
                        case 0x45524341: // "ACRE":
                            if (LoquiBinaryTranslation<PlacedCreature>.Instance.Parse(
                                frame: r,
                                item: out var placedCrea,
                                errorMask: errMaskInternal))
                            {
                                placed = placedCrea;
                                return true;
                            }
                            break;
                        case 0x52484341: //"ACHR":
                            if (LoquiBinaryTranslation<PlacedNPC>.Instance.Parse(
                                frame: r,
                                item: out var placedNPC,
                                errorMask: errMaskInternal))
                            {
                                placed = placedNPC;
                                return true;
                            }
                            break;
                        case 0x52464552: // "REFR":
                            if (LoquiBinaryTranslation<PlacedObject>.Instance.Parse(
                                frame: r,
                                item: out var placedObj,
                                errorMask: errMaskInternal))
                            {
                                placed = placedObj;
                                return true;
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    placed = null;
                    return false;
                }
                );
        }

        static bool ParseTemporaryOutliers(MutagenFrame frame, Cell obj, ErrorMaskBuilder errorMask)
        {
            var nextHeader = HeaderTranslation.GetNextRecordType(frame.Reader, out var pathLen);
            if (nextHeader.Equals(PathGrid_Registration.PGRD_HEADER))
            {
                using (var subFrame = frame.SpawnWithLength(pathLen + Mutagen.Bethesda.Constants.RECORD_HEADER_LENGTH))
                {
                    using (errorMask.PushIndex((int)Cell_FieldIndex.PathGrid))
                    {
                        obj.PathGrid = PathGrid.Create_Binary(
                            subFrame,
                            errorMask: errorMask,
                            recordTypeConverter: null);
                    }
                }
                return true;
            }
            else if (nextHeader.Equals(Landscape_Registration.LAND_HEADER))
            {
                using (var subFrame = frame.SpawnWithLength(pathLen + Mutagen.Bethesda.Constants.RECORD_HEADER_LENGTH))
                {
                    using (errorMask.PushIndex((int)Cell_FieldIndex.Landscape))
                    {
                        obj.Landscape = Landscape.Create_Binary(
                            subFrame,
                            errorMask: errorMask,
                            recordTypeConverter: null);
                    }
                }
                return true;
            }
            return false;
        }

        static void ParseTemporary(MutagenFrame frame, Cell obj, ErrorMaskBuilder errorMask)
        {
            frame.Reader.Position += 8;
            var id = FormID.Factory(frame.Reader.ReadUInt32());
            if (id != obj.FormID)
            {
                throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
            }
            frame.Reader.Position += 4;
            obj._temporaryTimeStamp = frame.Reader.ReadBytes(4);
            Mutagen.Bethesda.Binary.ListBinaryTranslation<Placed>.Instance.ParseRepeatedItem(
                frame: frame,
                item: obj.Temporary,
                fieldIndex: (int)Cell_FieldIndex.Persistent,
                lengthLength: 4,
                errorMask: errorMask,
                transl: (MutagenFrame r, RecordType header, out Placed placed, ErrorMaskBuilder listSubMask) =>
                {
                    switch (header.TypeInt)
                    {
                        case 0x45524341: // "ACRE":
                            if (LoquiBinaryTranslation<PlacedCreature>.Instance.Parse(
                                frame: r,
                                item: out var placedCrea,
                                errorMask: errorMask))
                            {
                                placed = placedCrea;
                                return true;
                            }
                            break;
                        case 0x52484341: //"ACHR":
                            if (LoquiBinaryTranslation<PlacedNPC>.Instance.Parse(
                                frame: r,
                                item: out var placedNPC,
                                errorMask: errorMask))
                            {
                                placed = placedNPC;
                                return true;
                            }
                            break;
                        case 0x52464552: // "REFR":
                            if (LoquiBinaryTranslation<PlacedObject>.Instance.Parse(
                                frame: r,
                                item: out var placedObj,
                                errorMask: errorMask))
                            {
                                placed = placedObj;
                                return true;
                            }
                            break;
                        default:
                            if (ParseTemporaryOutliers(frame, obj, errorMask))
                            {
                                listSubMask = null;
                                placed = null;
                                return false;
                            }
                            throw new NotImplementedException();
                    }
                    placed = null;
                    return false;
                });
        }

        static partial void CustomBinaryEnd_Export(MutagenWriter writer, Cell obj, ErrorMaskBuilder errorMask)
        {
            if (obj.Persistent.Count == 0
                && obj.Temporary.Count == 0
                && obj.VisibleWhenDistant.Count == 0
                && !obj.PathGrid_Property.HasBeenSet
                && !obj.Landscape_Property.HasBeenSet) return;
            using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, ObjectType.Group))
            {
                FormIDBinaryTranslation.Instance.Write(
                    writer, 
                    obj.FormID, 
                    errorMask);
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
                        FormIDBinaryTranslation.Instance.Write(
                            writer,
                            obj.FormID,
                            errorMask);
                        writer.Write((int)GroupTypeEnum.CellPersistentChildren);
                        if (obj._persistentTimeStamp != null)
                        {
                            writer.Write(obj._persistentTimeStamp);
                        }
                        else
                        {
                            writer.WriteZeros(4);
                        }
                        Mutagen.Bethesda.Binary.ListBinaryTranslation<Placed>.Instance.Write(
                            writer: writer,
                            items: obj.Persistent,
                            fieldIndex: (int)Cell_FieldIndex.Persistent,
                            errorMask: errorMask,
                            transl: LoquiBinaryTranslation<Placed>.Instance.Write);
                    }
                }
                if (obj.Temporary.Count > 0
                    || obj.PathGrid_Property.HasBeenSet
                    || obj.Landscape_Property.HasBeenSet)
                {
                    using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, ObjectType.Group))
                    {
                        FormIDBinaryTranslation.Instance.Write(
                            writer,
                            obj.FormID,
                            errorMask);
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
                            LoquiBinaryTranslation<Landscape>.Instance.Write(
                                writer,
                                obj.Landscape,
                                (int)Cell_FieldIndex.Landscape,
                                errorMask);
                        }
                        if (obj.PathGrid_Property.HasBeenSet)
                        {
                            LoquiBinaryTranslation<PathGrid>.Instance.Write(
                                writer,
                                obj.PathGrid,
                                (int)Cell_FieldIndex.PathGrid,
                                errorMask);
                        }
                        Mutagen.Bethesda.Binary.ListBinaryTranslation<Placed>.Instance.Write(
                            writer: writer,
                            items: obj.Temporary,
                            fieldIndex: (int)Cell_FieldIndex.Temporary,
                            errorMask: errorMask,
                            transl: LoquiBinaryTranslation<Placed>.Instance.Write);
                    }
                }
                if (obj.VisibleWhenDistant.Count > 0)
                {
                    using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, ObjectType.Group))
                    {
                        FormIDBinaryTranslation.Instance.Write(
                            writer,
                            obj.FormID,
                            errorMask);
                        writer.Write((int)GroupTypeEnum.CellVisibleDistantChildren);
                        if (obj._visibleWhenDistantTimeStamp != null)
                        {
                            writer.Write(obj._visibleWhenDistantTimeStamp);
                        }
                        else
                        {
                            writer.WriteZeros(4);
                        }
                        Mutagen.Bethesda.Binary.ListBinaryTranslation<Placed>.Instance.Write(
                            writer: writer,
                            items: obj.VisibleWhenDistant,
                            fieldIndex: (int)Cell_FieldIndex.VisibleWhenDistant,
                            errorMask: errorMask,
                            transl: LoquiBinaryTranslation<Placed>.Instance.Write);
                    }
                }
            }
        }
    }
}
