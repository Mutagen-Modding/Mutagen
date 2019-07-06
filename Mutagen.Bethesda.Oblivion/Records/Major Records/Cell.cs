using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CSharpExt.Rx;
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

        partial void PostDuplicate(Cell obj, Cell rhs, Func<FormKey> getNextFormKey, IList<(IMajorRecordCommon Record, FormKey OriginalFormKey)> duplicatedRecords)
        {
            if (rhs.PathGrid_IsSet
                && rhs.PathGrid != null)
            {
                obj.PathGrid = (PathGrid)rhs.PathGrid.Duplicate(getNextFormKey, duplicatedRecords);
            }
            if (rhs.Landscape_IsSet
                && rhs.Landscape != null)
            {
                obj.Landscape = (Landscape)rhs.Landscape.Duplicate(getNextFormKey, duplicatedRecords);
            }
            obj.Persistent.SetTo(rhs.Persistent.Select((i) => (IPlaced)i.Duplicate(getNextFormKey, duplicatedRecords)));
            obj.Temporary.SetTo(rhs.Temporary.Select((i) => (IPlaced)i.Duplicate(getNextFormKey, duplicatedRecords)));
            obj.VisibleWhenDistant.SetTo(rhs.VisibleWhenDistant.Select((i) => (IPlaced)i.Duplicate(getNextFormKey)));
        }
    }

    namespace Internals
    {
        public partial class CellBinaryCreateTranslation
        {
            public static async Task CustomBinaryEndImport(MutagenFrame frame, Cell obj, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (frame.Reader.Complete) return;
                var next = HeaderTranslation.GetNextType(
                    reader: frame.Reader,
                    contentLength: out var len,
                    finalPos: out var finalPos,
                    hopGroup: false);
                if (!next.Equals(Group_Registration.GRUP_HEADER)) return;
                var formKey = FormKey.Factory(masterReferences, frame.Reader.GetUInt32(offset: 8));
                var grupType = (GroupTypeEnum)frame.Reader.GetInt32(offset: 12);
                if (grupType == GroupTypeEnum.CellChildren)
                {
                    frame.Reader.Position += 16;
                    obj.Timestamp = frame.Reader.ReadBytes(4);
                    if (formKey != obj.FormKey)
                    {
                        throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
                    }
                }
                else
                {
                    return;
                }
                var subFrame = frame.SpawnWithLength(len - Mutagen.Bethesda.Constants.RECORD_HEADER_LENGTH);
                while (!subFrame.Complete)
                {
                    var persistGroup = HeaderTranslation.GetNextType(
                        reader: frame.Reader,
                        contentLength: out var persistLen,
                        finalPos: out var _,
                        hopGroup: false);
                    if (!persistGroup.Equals(Group_Registration.GRUP_HEADER))
                    {
                        throw new ArgumentException();
                    }
                    GroupTypeEnum type = (GroupTypeEnum)subFrame.Reader.GetUInt32(offset: 12);
                    var itemFrame = frame.SpawnWithLength(persistLen);
                    switch (type)
                    {
                        case GroupTypeEnum.CellPersistentChildren:
                            ParseTypical(
                                frame: itemFrame,
                                obj: obj,
                                fieldIndex: (int)Cell_FieldIndex.Persistent,
                                masterReferences: masterReferences,
                                coll: obj.Persistent,
                                errorMask: errorMask,
                                persistentParse: true);
                            break;
                        case GroupTypeEnum.CellTemporaryChildren:
                            ParseTemporary(
                                itemFrame,
                                obj,
                                masterReferences,
                                errorMask);
                            break;
                        case GroupTypeEnum.CellVisibleDistantChildren:
                            ParseTypical(
                                frame: itemFrame,
                                obj: obj,
                                fieldIndex: (int)Cell_FieldIndex.VisibleWhenDistant,
                                masterReferences: masterReferences,
                                coll: obj.VisibleWhenDistant,
                                errorMask: errorMask,
                                persistentParse: false);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            static void ParseTypical(
                MutagenFrame frame,
                Cell obj,
                int fieldIndex,
                MasterReferences masterReferences,
                ISourceSetList<IPlaced> coll,
                ErrorMaskBuilder errorMask,
                bool persistentParse)
            {
                frame.Reader.Position += 8;
                var formKey = FormKey.Factory(masterReferences, frame.Reader.ReadUInt32());
                if (formKey != obj.FormKey)
                {
                    throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
                }
                frame.Reader.Position += 4;
                if (persistentParse)
                {
                    obj.PersistentTimestamp = frame.Reader.ReadBytes(4);
                }
                else
                {
                    obj.VisibleWhenDistantTimestamp = frame.Reader.ReadBytes(4);
                }
                Mutagen.Bethesda.Binary.ListBinaryTranslation<IPlaced>.Instance.ParseRepeatedItem(
                    frame: frame,
                    fieldIndex: fieldIndex,
                    item: coll,
                    lengthLength: 4,
                    errorMask: errorMask,
                    transl: (MutagenFrame r, RecordType header, out IPlaced placed, ErrorMaskBuilder errMaskInternal) =>
                    {
                        switch (header.TypeInt)
                        {
                            case 0x45524341: // "ACRE":
                            if (LoquiBinaryTranslation<PlacedCreature>.Instance.Parse(
                                    frame: r,
                                    item: out var placedCrea,
                                    masterReferences: masterReferences,
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
                                    masterReferences: masterReferences,
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
                                    masterReferences: masterReferences,
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

            static bool ParseTemporaryOutliers(MutagenFrame frame, Cell obj, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                var majorMeta = frame.MetaData.GetMajorRecord(frame);
                var nextHeader = majorMeta.RecordType;
                if (nextHeader.Equals(PathGrid_Registration.PGRD_HEADER))
                {
                    using (errorMask.PushIndex((int)Cell_FieldIndex.PathGrid))
                    {
                        obj.PathGrid = PathGrid.CreateFromBinary(
                            frame.SpawnWithLength(majorMeta.TotalLength),
                            errorMask: errorMask,
                            masterReferences: masterReferences,
                            recordTypeConverter: null);
                    }
                    return true;
                }
                else if (nextHeader.Equals(Landscape_Registration.LAND_HEADER))
                {
                    using (errorMask.PushIndex((int)Cell_FieldIndex.Landscape))
                    {
                        obj.Landscape = Landscape.CreateFromBinary(
                            frame.SpawnWithLength(majorMeta.TotalLength),
                            errorMask: errorMask,
                            masterReferences: masterReferences,
                            recordTypeConverter: null);
                    }
                    return true;
                }
                return false;
            }

            static void ParseTemporary(MutagenFrame frame, Cell obj, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                frame.Reader.Position += 8;
                var formKey = FormKey.Factory(masterReferences, frame.Reader.ReadUInt32());
                if (formKey != obj.FormKey)
                {
                    throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
                }
                frame.Reader.Position += 4;
                obj.TemporaryTimestamp = frame.Reader.ReadBytes(4);
                Mutagen.Bethesda.Binary.ListBinaryTranslation<IPlaced>.Instance.ParseRepeatedItem(
                    frame: frame,
                    item: obj.Temporary,
                    fieldIndex: (int)Cell_FieldIndex.Persistent,
                    lengthLength: 4,
                    errorMask: errorMask,
                    transl: (MutagenFrame r, RecordType header, out IPlaced placed, ErrorMaskBuilder listSubMask) =>
                    {
                        switch (header.TypeInt)
                        {
                            case 0x45524341: // "ACRE":
                            if (LoquiBinaryTranslation<PlacedCreature>.Instance.Parse(
                                    frame: r,
                                    item: out var placedCrea,
                                    masterReferences: masterReferences,
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
                                    masterReferences: masterReferences,
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
                                    masterReferences: masterReferences,
                                    errorMask: errorMask))
                                {
                                    placed = placedObj;
                                    return true;
                                }
                                break;
                            default:
                                if (ParseTemporaryOutliers(frame, obj, masterReferences, errorMask))
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
        }

        public partial class CellBinaryWriteTranslation
        {
            static partial void CustomBinaryEndExport(MutagenWriter writer, ICellInternalGetter obj, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (obj.Persistent.Count == 0
                    && obj.Temporary.Count == 0
                    && obj.VisibleWhenDistant.Count == 0
                    && !obj.PathGrid_IsSet
                    && !obj.Landscape_IsSet) return;
                using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, ObjectType.Group))
                {
                    FormKeyBinaryTranslation.Instance.Write(
                        writer,
                        obj.FormKey,
                        masterReferences);
                    writer.Write((int)GroupTypeEnum.CellChildren);
                    writer.Write(obj.Timestamp);
                    if (obj.Persistent.Count > 0)
                    {
                        using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, ObjectType.Group))
                        {
                            FormKeyBinaryTranslation.Instance.Write(
                                writer,
                                obj.FormKey,
                                masterReferences);
                            writer.Write((int)GroupTypeEnum.CellPersistentChildren);
                            writer.Write(obj.PersistentTimestamp);
                            Mutagen.Bethesda.Binary.ListBinaryTranslation<IPlaced>.Instance.Write(
                                writer: writer,
                                items: obj.Persistent,
                                fieldIndex: (int)Cell_FieldIndex.Persistent,
                                errorMask: errorMask,
                                transl: (r, item, subErr) =>
                                {
                                    item.WriteToBinary(
                                        r,
                                        masterReferences,
                                        errorMask);
                                });
                        }
                    }
                    if (obj.Temporary.Count > 0
                        || obj.PathGrid_IsSet
                        || obj.Landscape_IsSet)
                    {
                        using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, ObjectType.Group))
                        {
                            FormKeyBinaryTranslation.Instance.Write(
                                writer,
                                obj.FormKey,
                                masterReferences);
                            writer.Write((int)GroupTypeEnum.CellTemporaryChildren);
                            writer.Write(obj.TemporaryTimestamp);
                            if (obj.Landscape_IsSet)
                            {
                                obj.Landscape.WriteToBinary(
                                    writer,
                                    masterReferences: masterReferences,
                                    errorMask: errorMask);
                            }
                            if (obj.PathGrid_IsSet)
                            {
                                obj.PathGrid.WriteToBinary(
                                    writer,
                                    masterReferences: masterReferences,
                                    errorMask: errorMask);
                            }
                            Mutagen.Bethesda.Binary.ListBinaryTranslation<IPlaced>.Instance.Write(
                                writer: writer,
                                items: obj.Temporary,
                                fieldIndex: (int)Cell_FieldIndex.Temporary,
                                errorMask: errorMask,
                                transl: (r, item, subErr) =>
                                {
                                    item.WriteToBinary(
                                        r,
                                        masterReferences,
                                        errorMask);
                                });
                        }
                    }
                    if (obj.VisibleWhenDistant.Count > 0)
                    {
                        using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, ObjectType.Group))
                        {
                            FormKeyBinaryTranslation.Instance.Write(
                                writer,
                                obj.FormKey,
                                masterReferences);
                            writer.Write((int)GroupTypeEnum.CellVisibleDistantChildren);
                            writer.Write(obj.VisibleWhenDistantTimestamp);
                            Mutagen.Bethesda.Binary.ListBinaryTranslation<IPlaced>.Instance.Write(
                                writer: writer,
                                items: obj.VisibleWhenDistant,
                                fieldIndex: (int)Cell_FieldIndex.VisibleWhenDistant,
                                errorMask: errorMask,
                                transl: (r, item, subErr) =>
                                {
                                    item.WriteToBinary(
                                        r,
                                        masterReferences,
                                        errorMask);
                                });
                        }
                    }
                }
            }
        }
    }
}
