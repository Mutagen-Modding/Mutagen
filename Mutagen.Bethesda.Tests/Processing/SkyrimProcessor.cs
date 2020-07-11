using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Internals;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.Tests
{
    public class SkyrimProcessor : Processor
    {
        public override GameRelease GameRelease => GameRelease.SkyrimLE;

        protected override void AddDynamicProcessorInstructions(IMutagenReadStream stream, FormID formID, RecordType recType)
        {
            base.AddDynamicProcessorInstructions(stream, formID, recType);
            var loc = this._AlignedFileLocs[formID];
            ProcessGameSettings(stream, formID, recType, loc);
            ProcessFurniture(stream, formID, recType, loc);
            ProcessNpcs(stream, formID, recType, loc);
            ProcessRegions(stream, formID, recType, loc);
            ProcessCells(stream, formID, recType, loc);
            ProcessPlaced(stream, formID, recType, loc);
            ProcessNavmeshes(stream, formID, recType, loc);
            ProcessDialogs(stream, formID, recType, loc);
            ProcessQuests(stream, formID, recType, loc);
            ProcessPackages(stream, formID, recType, loc);
            ProcessShaders(stream, formID, recType, loc);
            ProcessExplosions(stream, formID, recType, loc);
            ProcessImageSpaceAdapters(stream, formID, recType, loc);
            ProcessLoadScreens(stream, formID, recType, loc);
            ProcessActivators(stream, formID, recType, loc);
            ProcessWeathers(stream, formID, recType, loc);
        }

        private void ProcessGameSettings(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!GameSetting_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordFrame();
            var edidLoc = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, this.Meta, new RecordType("EDID"), navigateToContent: true);
            if (edidLoc == null) return;
            if ((char)majorFrame.Content[edidLoc.Value] != 'f') return;

            var dataIndex = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, this.Meta, new RecordType("DATA"), navigateToContent: true);
            if (dataIndex == null) return;
            stream.Position = loc.Min + majorFrame.HeaderLength + dataIndex.Value;
            ProcessZeroFloat(stream);
        }

        private void ProcessFurniture(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Furniture_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordMemoryFrame(readSafe: true);

            // Find and store marker data
            var data = new Dictionary<int, ReadOnlyMemorySlice<byte>>();
            var indices = new List<int>();
            var initialPos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.ENAM);
            if (initialPos == null) return;
            var pos = initialPos.Value;
            while (pos < majorFrame.Content.Length)
            {
                var positions = UtilityTranslation.FindNextSubrecords(
                    majorFrame.Content.Slice(pos),
                    stream.MetaData.Constants,
                    out var lenParsed,
                    stopOnAlreadyEncounteredRecord: true,
                    new RecordType[]
                    {
                        RecordTypes.ENAM,
                        new RecordType("NAM0"),
                        new RecordType("FNMK"),
                    });
                var enamPos = positions[0];
                if (enamPos == null) break;
                var enamFrame = stream.MetaData.Constants.SubrecordFrame(majorFrame.Content.Slice(pos + enamPos.Value));
                var index = BinaryPrimitives.ReadInt32LittleEndian(enamFrame.Content);
                data.Add(index, majorFrame.Content.Slice(pos + enamPos.Value, lenParsed));
                indices.Add(index);
                pos += lenParsed;
            }

            if (indices.SequenceEqual(indices.OrderBy(i => i))) return;
            byte[] reordered = new byte[data.Values.Select(l => l.Length).Sum()];
            int transferPos = 0;
            foreach (var index in indices.OrderBy(i => i))
            {
                var bytes = data[index];
                bytes.Span.CopyTo(reordered.AsSpan().Slice(transferPos));
                transferPos += bytes.Length;
            }
            this._Instructions.SetSubstitution(loc.Min + majorFrame.HeaderLength + initialPos.Value, reordered);
        }

        private void ProcessNpcs(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Npc_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordMemoryFrame(readSafe: true);

            var qnam = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.QNAM, navigateToContent: true);
            if (qnam != null)
            {
                // Standardize float rounding errors
                var r = IBinaryStreamExt.GetColorByte(SpanExt.GetFloat(majorFrame.Content.Slice(qnam.Value, 4)));
                var g = IBinaryStreamExt.GetColorByte(SpanExt.GetFloat(majorFrame.Content.Slice(qnam.Value + 4, 4)));
                var b = IBinaryStreamExt.GetColorByte(SpanExt.GetFloat(majorFrame.Content.Slice(qnam.Value + 8, 4)));
                byte[] bytes = new byte[12];
                using var writer = new MutagenWriter(new MemoryStream(bytes), stream.MetaData.Constants);
                writer.Write(r / 255f);
                writer.Write(g / 255f);
                writer.Write(b / 255f);
                this._Instructions.SetSubstitution(loc.Min + qnam.Value + majorFrame.HeaderLength, bytes);
            }
            var nam9 = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.NAM9);
            if (nam9 != null)
            {
                // Standardize floats
                var subRecord = stream.MetaData.Constants.Subrecord(majorFrame.Content.Slice(nam9.Value), RecordTypes.NAM9);
                stream.Position = loc.Min + nam9.Value + majorFrame.HeaderLength + subRecord.HeaderLength;
                var final = stream.Position + subRecord.ContentLength;
                while (stream.Position < final)
                {
                    ProcessZeroFloat(stream);
                }
            }
        }

        private void ProcessRegions(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Region_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordMemoryFrame(readSafe: true);

            var rdat = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.RDAT, navigateToContent: false);
            if (rdat == null) return;

            // Order RDATs by index
            SortedList<uint, RangeInt64> rdats = new SortedList<uint, RangeInt64>();
            List<uint> raw = new List<uint>();
            while (rdat != null)
            {
                var rdatHeader = stream.MetaData.Constants.SubrecordFrame(majorFrame.Content.Slice(rdat.Value));
                var index = BinaryPrimitives.ReadUInt32LittleEndian(rdatHeader.Content);
                var nextRdat = UtilityTranslation.FindFirstSubrecord(
                    majorFrame.Content,
                    stream.MetaData.Constants,
                    RecordTypes.RDAT,
                    navigateToContent: false,
                    offset: rdat.Value + rdatHeader.TotalLength);
                rdats[index] =
                    new RangeInt64(
                        loc.Min + majorFrame.HeaderLength + rdat.Value,
                        nextRdat == null ? loc.Max : nextRdat.Value - 1 + loc.Min + majorFrame.HeaderLength);
                raw.Add(index);
                rdat = nextRdat;
            }
            if (raw.SequenceEqual(rdats.Keys)) return;
            foreach (var item in rdats.Reverse())
            {
                this._Instructions.SetMove(
                    loc: loc.Max + 1,
                    section: item.Value);
            }
        }

        private void ProcessCells(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!RecordTypes.CELL.Equals(recType)) return;
            CleanEmptyCellGroups(
                stream,
                formID,
                loc,
                numSubGroups: 2);

            // Process odd length changing flags
            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordMemoryFrame(readSafe: true);
            var sizeChange = 0;

            var pos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.DATA);
            if (pos != null)
            {
                var subHeader = stream.MetaData.Constants.Subrecord(majorFrame.Content.Slice(pos.Value));
                if (subHeader.ContentLength == 1)
                {
                    _Instructions.SetSubstitution(
                        loc.Min + majorFrame.HeaderLength + pos.Value + 4,
                        2);
                    _Instructions.SetAddition(
                        loc.Min + majorFrame.HeaderLength + pos.Value + stream.MetaData.Constants.SubConstants.HeaderLength + 1,
                        new byte[] { 0 });
                    sizeChange++;
                }
            }

            ProcessSubrecordLengths(
                stream,
                sizeChange,
                loc.Min,
                formID);
        }

        private void ProcessDialogs(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!DialogTopic_Registration.TriggeringRecordType.Equals(recType)) return;
            CleanEmptyDialogGroups(
                stream,
                formID,
                loc);

            // Reset misnumbered counter
            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordMemoryFrame(readSafe: true);

            var pos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.TIFC);
            if (pos != null)
            {
                var subHeader = stream.MetaData.Constants.SubrecordFrame(majorFrame.Content.Slice(pos.Value));
                var count = BinaryPrimitives.ReadUInt32LittleEndian(subHeader.Content);

                uint actualCount = 0;
                var groupFrame = stream.ReadGroupFrame();
                if (groupFrame.IsGroup)
                {
                    int groupPos = 0;
                    while (groupPos < groupFrame.Content.Length)
                    {
                        var majorMeta = stream.MetaData.Constants.MajorRecord(groupFrame.Content.Slice(groupPos));
                        actualCount++;
                        groupPos += checked((int)majorMeta.TotalLength);
                    }
                }

                if (actualCount != count)
                {
                    byte[] b = new byte[4];
                    BinaryPrimitives.WriteUInt32LittleEndian(b, actualCount);
                    _Instructions.SetSubstitution(
                        loc.Min + majorFrame.HeaderLength + pos.Value + stream.MetaData.Constants.SubConstants.HeaderLength,
                        b);
                }
            }
        }

        private void ProcessQuests(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Quest_Registration.TriggeringRecordType.Equals(recType)) return;

            // Process next alias ID subrecords to align to their current contents
            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordMemoryFrame(readSafe: true);
            var content = majorFrame.Content;

            var pos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.ANAM);
            if (pos != null)
            {
                var anamFrame = stream.MetaData.Constants.SubrecordFrame(majorFrame.Content.Slice(pos.Value));
                var next = BinaryPrimitives.ReadUInt32LittleEndian(anamFrame.Content);
                var targets = new RecordType[]
                {
                    RecordTypes.ALST,
                    RecordTypes.ALLS
                };
                var locs = UtilityTranslation.FindAllOfSubrecords(
                    majorFrame.Content,
                    stream.MetaData.Constants,
                    targets.ToGetter(),
                    navigateToContent: true);
                uint actualNext = 0;
                if (locs.Length > 0)
                {
                    actualNext = locs
                        .Select(l =>
                        {
                            return BinaryPrimitives.ReadUInt32LittleEndian(content.Slice(l));
                        })
                        .Max();
                    actualNext++;
                }
                if (actualNext != next)
                {
                    byte[] sub = new byte[4];
                    BinaryPrimitives.WriteUInt32LittleEndian(sub, actualNext);
                    _Instructions.SetSubstitution(
                        loc.Min + majorFrame.HeaderLength + pos.Value + anamFrame.HeaderLength,
                        sub);
                }
            }

            var sizeChange = FixMissingCounters(
                majorFrame,
                loc,
                new RecordType("COCT"),
                new RecordType("CNTO"));

            ProcessSubrecordLengths(
                stream,
                sizeChange,
                loc.Min,
                formID);

            FixVMADFormIDs(
                majorFrame,
                loc,
                out var vmadPos,
                out var objectFormat,
                out var processedLen);
            if (vmadPos != null)
            {
                var vmadFrame = Meta.SubrecordMemoryFrame(majorFrame.Content.Slice(vmadPos.Value));
                stream = new MutagenInterfaceReadStream(
                    new MutagenMemoryReadStream(vmadFrame.Content, new ParsingBundle(GameRelease)),
                    new ParsingBundle(GameRelease))
                {
                    Position = processedLen - vmadFrame.HeaderLength
                };
                if (stream.Complete) return;
                // skip unknown
                stream.Position += 1;
                var fragCount = stream.ReadUInt16();
                // skip name
                var len = stream.ReadUInt16();
                stream.Position += len;
                for (int i = 0; i < fragCount; i++)
                {
                    stream.Position += 9;
                    // skip name
                    len = stream.ReadUInt16();
                    stream.Position += len;
                    // skip name
                    len = stream.ReadUInt16();
                    stream.Position += len;
                }
                var aliasCount = stream.ReadUInt16();
                for (int i = 0; i < aliasCount; i++)
                {
                    FixObjectPropertyIDs(stream, loc, objectFormat);
                    // skip version
                    stream.Position += 2;
                    objectFormat = stream.ReadUInt16();
                    var numScripts = stream.ReadUInt16();
                    for (int j = 0; j < numScripts; j++)
                    {
                        FixVMADScriptIDs(stream, loc, objectFormat);
                    }
                }
            }
        }

        public void FixVMADFormIDs(
            MajorRecordMemoryFrame frame,
            RangeInt64 loc,
            out int? vmadPos,
            out ushort objectFormat,
            out int processed)
        {
            vmadPos = UtilityTranslation.FindFirstSubrecord(frame.Content, Meta, RecordTypes.VMAD);
            if (vmadPos == null)
            {
                processed = 0;
                objectFormat = 0;
                return;
            }
            var stream = new MutagenInterfaceReadStream(
                new MutagenMemoryReadStream(frame.HeaderAndContentData, new ParsingBundle(GameRelease)),
                new ParsingBundle(GameRelease))
            {
                Position = vmadPos.Value + frame.HeaderLength
            };
            stream.Position += Meta.SubConstants.HeaderLength;
            // Skip version
            stream.Position += 2;
            objectFormat = stream.ReadUInt16();
            var scriptCt = stream.ReadUInt16();
            for (int i = 0; i < scriptCt; i++)
            {
                FixVMADScriptIDs(stream, loc, objectFormat);
            }
            processed = (int)(stream.Position - vmadPos.Value - frame.HeaderLength);
        }

        private void FixVMADScriptIDs(IMutagenReadStream stream, RangeInt64 loc, ushort objectFormat)
        {
            // skip name
            var len = stream.ReadUInt16();
            stream.Position += len;
            // Skip flags
            stream.Position += 1;
            var propCount = stream.ReadUInt16();
            for (int j = 0; j < propCount; j++)
            {
                // skip name
                len = stream.ReadUInt16();
                stream.Position += len;
                var type = (ScriptProperty.Type)stream.ReadUInt8();
                // skip flags
                stream.Position += 1;
                // Going to cheat here, and use the autogenerated records
                ScriptProperty prop = type switch
                {
                    ScriptProperty.Type.None => new ScriptProperty(),
                    ScriptProperty.Type.Object => new ScriptObjectProperty(),
                    ScriptProperty.Type.String => new ScriptStringProperty(),
                    ScriptProperty.Type.Int => new ScriptIntProperty(),
                    ScriptProperty.Type.Float => new ScriptFloatProperty(),
                    ScriptProperty.Type.Bool => new ScriptBoolProperty(),
                    ScriptProperty.Type.ArrayOfObject => new ScriptObjectListProperty(),
                    ScriptProperty.Type.ArrayOfString => new ScriptStringListProperty(),
                    ScriptProperty.Type.ArrayOfInt => new ScriptIntListProperty(),
                    ScriptProperty.Type.ArrayOfFloat => new ScriptFloatListProperty(),
                    ScriptProperty.Type.ArrayOfBool => new ScriptBoolListProperty(),
                    _ => throw new NotImplementedException(),
                };
                switch (prop)
                {
                    case ScriptObjectProperty obj:
                        FixObjectPropertyIDs(stream, loc, objectFormat);
                        break;
                    case ScriptObjectListProperty objList:
                        var count = stream.ReadUInt32();
                        for (int i = 0; i < count; i++)
                        {
                            FixObjectPropertyIDs(stream, loc, objectFormat);
                        }
                        break;
                    default:
                        prop.CopyInFromBinary(new MutagenFrame(stream));
                        break;
                }
            }
        }

        private void FixObjectPropertyIDs(IMutagenReadStream stream, RangeInt64 loc, ushort objectFormat)
        {
            switch (objectFormat)
            {
                case 2:
                    stream.Position += 4;
                    ProcessFormIDOverflow(stream, loc);
                    break;
                case 1:
                    ProcessFormIDOverflow(stream, loc);
                    stream.Position += 4;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ProcessPlaced(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!PlacedObject_Registration.TriggeringRecordType.Equals(recType)
                && !PlacedNpc_Registration.TriggeringRecordType.Equals(recType)
                && !PlacedArrow_Registration.TriggeringRecordType.Equals(recType)
                && !PlacedBarrier_Registration.TriggeringRecordType.Equals(recType)
                && !PlacedBeam_Registration.TriggeringRecordType.Equals(recType)
                && !PlacedCone_Registration.TriggeringRecordType.Equals(recType)
                && !PlacedFlame_Registration.TriggeringRecordType.Equals(recType)
                && !PlacedHazard_Registration.TriggeringRecordType.Equals(recType)
                && !PlacedMissile_Registration.TriggeringRecordType.Equals(recType)
                && !PlacedTrap_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordMemoryFrame(readSafe: true);
            var sizeChange = 0;

            var pos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.DATA);
            if (pos != null)
            {
                stream.Position = loc.Min + majorFrame.HeaderLength + pos.Value + stream.MetaData.Constants.SubConstants.HeaderLength;
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
            }

            pos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.XTEL);
            if (pos != null)
            {
                stream.Position = loc.Min + majorFrame.HeaderLength + pos.Value + stream.MetaData.Constants.SubConstants.HeaderLength;
                stream.Position += 4;
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
            }

            pos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.XPRM);
            if (pos != null)
            {
                stream.Position = loc.Min + majorFrame.HeaderLength + pos.Value + stream.MetaData.Constants.SubConstants.HeaderLength;
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessColorFloat(stream);
                ProcessZeroFloat(stream);
            }

            pos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.XRMR);
            if (pos != null)
            {
                stream.Position = loc.Min + majorFrame.HeaderLength + pos.Value + stream.MetaData.Constants.SubConstants.HeaderLength;
                var val = stream.ReadInt32();
                if (val == 0)
                {
                    _Instructions.SetRemove(
                        RangeInt64.FactoryFromLength(
                            loc.Min + majorFrame.HeaderLength + pos.Value,
                            10));
                    sizeChange -= 10;
                }
            }

            ProcessSubrecordLengths(
                stream,
                sizeChange,
                loc.Min,
                formID);
        }

        private void ProcessNavmeshes(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!RecordTypes.NAVM.Equals(recType)) return;

            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordMemoryFrame(readSafe: true);

            var pos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.NVNM);
            if (pos != null)
            {
                stream.Position = loc.Min + majorFrame.HeaderLength + pos.Value + stream.MetaData.Constants.SubConstants.HeaderLength;
                stream.Position += 16;
                var count = stream.ReadInt32() * 3;
                for (int i = 0; i < count; i++)
                {
                    ProcessZeroFloat(stream);
                }
            }
        }

        private void ProcessPackages(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Package_Registration.TriggeringRecordType.Equals(recType)) return;

            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordMemoryFrame(readSafe: true);

            // Reorder Idle subrecords

            // Reorder data values
            var xnamPos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.XNAM);
            if (xnamPos == null)
            {
                throw new ArgumentException();
            }

            var pkcuPos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.PKCU, navigateToContent: true);
            if (!pkcuPos.HasValue)
            {
                throw new ArgumentException();
            }
            var count = BinaryPrimitives.ReadInt32LittleEndian(majorFrame.Content.Slice(pkcuPos.Value));

            if (count == 0) return;

            var anamPos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.ANAM);
            RecordType pldt = new RecordType("PLDT");
            RecordType ptda = new RecordType("PTDA");
            RecordType pdto = new RecordType("PDTO");
            RecordType tpic = new RecordType("TPIC");
            RecordType unam = new RecordType("UNAM");
            RecordType bnam = new RecordType("BNAM");
            RecordType pnam = new RecordType("PNAM");
            // Reorder data values to be in index ordering
            if (anamPos.HasValue && anamPos.Value < xnamPos.Value)
            {
                var startLoc = anamPos.Value;
                var dataValues = new List<(sbyte Index, ReadOnlyMemorySlice<byte> Data)>();
                var curLoc = startLoc;
                while (anamPos.HasValue && anamPos.Value < xnamPos.Value)
                {
                    var anamRecord = stream.MetaData.Constants.SubrecordFrame(majorFrame.Content.Slice(anamPos.Value));
                    var recs = UtilityTranslation.FindNextSubrecords(
                        majorFrame.Content.Slice(anamPos.Value + anamRecord.TotalLength),
                        stream.MetaData.Constants,
                        out var _,
                        RecordTypes.ANAM,
                        RecordTypes.CNAM,
                        pldt,
                        ptda,
                        pdto,
                        tpic);
                    int finalLoc;
                    if (recs[0] == null)
                    {
                        finalLoc = recs.NotNull().Max();
                    }
                    else if (recs[0] == 0)
                    {
                        dataValues.Add(
                            (-1, majorFrame.Content.Slice(anamPos.Value, anamRecord.TotalLength)));
                        curLoc = anamPos.Value + anamRecord.TotalLength;
                        anamPos = anamPos.Value + anamRecord.TotalLength;
                        continue;
                    }
                    else
                    {
                        finalLoc = recs
                            .NotNull()
                            .Where(i => i < recs[0]!.Value)
                            .Max();
                    }
                    var finalRec = stream.MetaData.Constants.Subrecord(majorFrame.Content.Slice(anamPos.Value + anamRecord.TotalLength + finalLoc));
                    var dataSlice = majorFrame.Content.Slice(anamPos.Value, anamRecord.TotalLength + finalLoc + finalRec.TotalLength);
                    if (BinaryStringUtility.ProcessWholeToZString(anamRecord.Content) == "Bool"
                        && recs[1] != null)
                    {
                        // Ensure bool value is 1 or 0
                        var cnam = stream.MetaData.Constants.SubrecordFrame(majorFrame.Content.Slice(anamPos.Value + anamRecord.TotalLength + recs[1].Value));
                        if (cnam.Content.Length != 1)
                        {
                            throw new ArgumentException();
                        }
                        if (cnam.Content[0] > 1)
                        {
                            var bytes = dataSlice.ToArray();
                            int boolIndex = anamRecord.TotalLength + recs[1].Value + cnam.HeaderLength;
                            bytes[boolIndex] = (byte)(bytes[boolIndex] > 0 ? 1 : 0);
                            dataSlice = bytes;
                        }
                    }
                    dataValues.Add((-1, dataSlice));

                    curLoc = anamPos.Value + anamRecord.TotalLength + finalLoc + finalRec.TotalLength;
                    anamPos = anamPos.Value + anamRecord.TotalLength + recs[0];
                }

                var unamLocs = UtilityTranslation.ParseRepeatingSubrecord(
                    majorFrame.Content.Slice(curLoc),
                    stream.MetaData.Constants,
                    unam,
                    out var _);
                if (unamLocs == null
                    || unamLocs.Length != dataValues.Count
                    || unamLocs.Length != count)
                {
                    throw new ArgumentException();
                }

                for (sbyte i = 0; i < unamLocs.Length; i++)
                {
                    var unamRec = stream.MetaData.Constants.SubrecordFrame(majorFrame.Content.Slice(curLoc + unamLocs[i]));
                    dataValues[i] = (
                        (sbyte)unamRec.Content[0],
                        dataValues[i].Data);
                }

                var subLoc = startLoc;
                foreach (var item in dataValues.OrderBy(i => i.Index))
                {
                    _Instructions.SetSubstitution(
                        loc.Min + majorFrame.HeaderLength + subLoc,
                        item.Data.ToArray());
                    subLoc += item.Data.Length;
                }
                foreach (var item in dataValues.OrderBy(i => i.Index))
                {
                    byte[] bytes = new byte[] { 0x55, 0x4E, 0x41, 0x4D, 0x01, 0x00, 0x00 };
                    bytes[6] = (byte)item.Index;
                    _Instructions.SetSubstitution(
                        loc.Min + majorFrame.HeaderLength + subLoc,
                        bytes.ToArray());
                    subLoc += bytes.Length;
                }
            }

            // Reorder inputs
            var unamPos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content.Slice(xnamPos.Value), stream.MetaData.Constants, unam);
            if (!unamPos.HasValue) return;
            unamPos += xnamPos.Value;
            var writeLoc = loc.Min + majorFrame.HeaderLength + unamPos.Value;
            var inputValues = new List<(sbyte Index, ReadOnlyMemorySlice<byte> Data)>();
            while (unamPos.HasValue)
            {
                var unamRecord = stream.MetaData.Constants.SubrecordFrame(majorFrame.Content.Slice(unamPos.Value));
                var recs = UtilityTranslation.FindNextSubrecords(
                    majorFrame.Content.Slice(unamPos.Value + unamRecord.TotalLength),
                    stream.MetaData.Constants,
                    out var _,
                    unam,
                    bnam,
                    pnam);
                int finalLoc;
                if (recs[0] == null)
                {
                    finalLoc = recs.NotNull().Max();
                }
                else if (recs[0] == 0)
                {
                    inputValues.Add(
                        ((sbyte)unamRecord.Content[0], majorFrame.Content.Slice(unamPos.Value, unamRecord.TotalLength)));
                    unamPos = unamPos.Value + unamRecord.TotalLength;
                    continue;
                }
                else
                {
                    finalLoc = recs
                        .NotNull()
                        .Where(i => i < recs[0]!.Value)
                        .Max();
                }
                var finalRec = stream.MetaData.Constants.Subrecord(majorFrame.Content.Slice(unamPos.Value + unamRecord.TotalLength + finalLoc));
                inputValues.Add(
                    ((sbyte)unamRecord.Content[0], majorFrame.Content.Slice(unamPos.Value, unamRecord.TotalLength + finalLoc + finalRec.TotalLength)));

                unamPos = unamPos.Value + unamRecord.TotalLength + recs[0];
            }
            foreach (var item in inputValues.OrderBy(i => i.Index))
            {
                _Instructions.SetSubstitution(
                    writeLoc,
                    item.Data.ToArray());
                writeLoc += item.Data.Length;
            }
        }

        private void ProcessShaders(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!EffectShader_Registration.TriggeringRecordType.Equals(recType)) return;

            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordMemoryFrame(readSafe: true);

            var pos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.DATA);
            if (pos != null)
            {
                stream.Position = loc.Min + majorFrame.HeaderLength + pos.Value + stream.MetaData.Constants.SubConstants.HeaderLength;
                stream.Position += 20;
                for (int i = 0; i < 9; i++)
                {
                    ProcessZeroFloat(stream);
                }
                stream.Position += 4;
                for (int i = 0; i < 8; i++)
                {
                    ProcessZeroFloat(stream);
                }
                stream.Position += 20;
                for (int i = 0; i < 19; i++)
                {
                    ProcessZeroFloat(stream);
                }
                stream.Position += 12;
                for (int i = 0; i < 11; i++)
                {
                    ProcessZeroFloat(stream);
                }
                stream.Position += 4;
                for (int i = 0; i < 5; i++)
                {
                    ProcessZeroFloat(stream);
                }
                stream.Position += 4;
                ProcessZeroFloat(stream);
                stream.Position += 8;
                for (int i = 0; i < 6; i++)
                {
                    ProcessZeroFloat(stream);
                }
                stream.Position += 12;
                for (int i = 0; i < 9; i++)
                {
                    ProcessZeroFloat(stream);
                }
                stream.Position += 32;
                for (int i = 0; i < 2; i++)
                {
                    ProcessZeroFloat(stream);
                }
            }
        }

        private void ProcessExplosions(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Explosion_Registration.TriggeringRecordType.Equals(recType)) return;

            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordMemoryFrame(readSafe: true);

            var pos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.DATA);
            if (pos != null)
            {
                stream.Position = loc.Min + majorFrame.HeaderLength + pos.Value + stream.MetaData.Constants.SubConstants.HeaderLength;
                for (int i = 0; i < 6; i++)
                {
                    ProcessFormIDOverflow(stream, loc: null);
                }
                for (int i = 0; i < 5; i++)
                {
                    ProcessZeroFloat(stream);
                }
            }
        }

        private void ProcessImageSpaceAdapters(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!ImageSpaceAdapter_Registration.TriggeringRecordType.Equals(recType)) return;

            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordMemoryFrame(readSafe: true);

            int subLoc = 0;
            void ProcessKeyframe(int contentLen)
            {
                stream.Position = loc.Min + stream.MetaData.Constants.MajorConstants.HeaderLength + subLoc;
                stream.Position += stream.MetaData.Constants.SubConstants.HeaderLength;
                var endPos = stream.Position + contentLen;
                while (stream.Position < endPos)
                {
                    ProcessZeroFloat(stream);
                }
            }

            while (subLoc < majorFrame.Content.Length)
            {
                var subRecord = stream.MetaData.Constants.Subrecord(majorFrame.Content.Slice(subLoc));
                switch (subRecord.RecordTypeInt)
                {
                    case RecordTypeInts.QIAD:
                    case RecordTypeInts.RIAD:
                        ProcessKeyframe(subRecord.ContentLength);
                        break;
                    default:
                        break;
                }
                subLoc += subRecord.TotalLength;
            }

            var pos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.DATA);
            if (pos != null)
            {
                stream.Position = loc.Min + majorFrame.HeaderLength + pos.Value + stream.MetaData.Constants.SubConstants.HeaderLength;
                for (int i = 0; i < 6; i++)
                {
                    ProcessFormIDOverflow(stream, loc: null);
                }
                for (int i = 0; i < 5; i++)
                {
                    ProcessZeroFloat(stream);
                }
            }
        }

        private void ProcessLoadScreens(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!LoadScreen_Registration.TriggeringRecordType.Equals(recType)) return;

            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordMemoryFrame(readSafe: true);

            var pos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.XNAM);
            if (pos != null)
            {
                stream.Position = loc.Min + majorFrame.HeaderLength + pos.Value + stream.MetaData.Constants.SubConstants.HeaderLength;
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
            }
        }

        private void ProcessActivators(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Activator_Registration.TriggeringRecordType.Equals(recType)) return;

            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordMemoryFrame(readSafe: true);

            FixVMADFormIDs(
                majorFrame,
                loc,
                out var vmadPos,
                out var objectFormat,
                out var processedLen);
        }

        private void ProcessWeathers(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Weather_Registration.TriggeringRecordType.Equals(recType)) return;

            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordMemoryFrame(readSafe: true);

            var pos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.SNAM);
            if (pos != null)
            {
                var locs = UtilityTranslation.ParseRepeatingSubrecord(majorFrame.Content.Slice(pos.Value), stream.MetaData.Constants, RecordTypes.SNAM, out var _);
                foreach (var snam in locs)
                {
                    stream.Position = loc.Min + majorFrame.HeaderLength + stream.MetaData.Constants.SubConstants.HeaderLength + snam + pos.Value;
                    ProcessFormIDOverflow(stream, loc: null);
                }
            }
        }

        protected override IEnumerable<Task> ExtraJobs(Func<IMutagenReadStream> streamGetter)
        {
            foreach (var t in base.ExtraJobs(streamGetter))
            {
                yield return t;
            }
            foreach (var source in EnumExt.GetValues<StringsSource>())
            {
                yield return TaskExt.Run(DoMultithreading, () =>
                {
                    return ProcessStringsFilesIndices(streamGetter, new DirectoryInfo(Path.GetDirectoryName(this.SourcePath)), Language.English, source, ModKey.Factory(Path.GetFileName(this.SourcePath)));
                });
            }
        }

        public void PerkStringHandler(
            IMutagenReadStream stream,
            MajorRecordHeader major,
            BinaryFileProcessor.Config instr,
            List<KeyValuePair<uint, uint>> processedStrings,
            IStringsLookup overlay,
            ref uint newIndex)
        {
            var majorCompletePos = stream.Position + major.ContentLength;
            long? lastepft = null;
            while (stream.Position < majorCompletePos)
            {
                var sub = stream.GetSubrecord();
                switch (sub.RecordTypeInt)
                {
                    case RecordTypeInts.FULL:
                    case RecordTypeInts.EPF2:
                        AStringsAlignment.ProcessStringLink(stream, instr, processedStrings, overlay, ref newIndex);
                        break;
                    case RecordTypeInts.EPFT:
                        lastepft = stream.Position;
                        break;
                    case RecordTypeInts.EPFD:
                        var pos = stream.Position;
                        stream.Position = lastepft.Value;
                        var epftFrame = stream.ReadSubrecordFrame();
                        if (epftFrame.Content[0] == (byte)APerkEntryPointEffect.ParameterType.LString)
                        {
                            stream.Position = pos;
                            AStringsAlignment.ProcessStringLink(stream, instr, processedStrings, overlay, ref newIndex);
                        }
                        stream.Position = pos;
                        break;
                    default:
                        break;
                }
                stream.Position += sub.TotalLength;
            }
        }

        private async Task ProcessStringsFilesIndices(Func<IMutagenReadStream> streamGetter, DirectoryInfo dataFolder, Language language, StringsSource source, ModKey modKey)
        {
            using var stream = streamGetter();
            switch (source)
            {
                case StringsSource.Normal:
                    ProcessStringsFiles(
                        modKey,
                        dataFolder,
                        language,
                        source,
                        RenumberStringsFileEntries(
                            modKey,
                            stream,
                            dataFolder,
                            language,
                            source,
                            new RecordType[] { "ACTI", "FULL" },
                            new RecordType[] { "APPA", "FULL" },
                            new RecordType[] { "AMMO", "FULL" },
                            new RecordType[] { "ARMO", "FULL" },
                            new RecordType[] { "BOOK", "FULL" },
                            new RecordType[] { "CLAS", "FULL" },
                            new RecordType[] { "EYES", "FULL" },
                            new RecordType[] { "CONT", "FULL" },
                            new RecordType[] { "DOOR", "FULL" },
                            new RecordType[] { "FACT", "FULL" },
                            new RecordType[] { "FURN", "FULL" },
                            new RecordType[] { "HAZD", "FULL" },
                            new RecordType[] { "HDPT", "FULL" },
                            new RecordType[] { "ALCH", "FULL" },
                            new RecordType[] { "INGR", "FULL" },
                            new RecordType[] { "LIGH", "FULL" },
                            new RecordType[] { "MGEF", "FULL", "DNAM" },
                            new RecordType[] { "MISC", "FULL" },
                            new RecordType[] { "MSTT", "FULL" },
                            new RecordType[] { "NPC_", "FULL" },
                            new RecordType[] { "ENCH", "FULL" },
                            new RecordType[] { "PROJ", "FULL" },
                            new RecordType[] { "RACE", "FULL" },
                            new RecordType[] { "SCRL", "FULL" },
                            new RecordType[] { "SLGM", "FULL" },
                            new RecordType[] { "SPEL", "FULL" },
                            new RecordType[] { "TACT", "FULL" },
                            new RecordType[] { "TREE", "FULL" },
                            new RecordType[] { "WEAP", "FULL" },
                            new RecordType[] { "FLOR", "FULL" },
                            new RecordType[] { "KEYM", "FULL" },
                            new RecordType[] { "CELL", "FULL" },
                            new RecordType[] { "REFR", "FULL" },
                            new RecordType[] { "WRLD", "FULL" },
                            new RecordType[] { "DIAL", "FULL" },
                            new RecordType[] { "INFO", "RNAM" },
                            new RecordType[] { "QUST", "FULL", "NNAM" },
                            new RecordType[] { "WATR", "FULL" },
                            new RecordType[] { "EXPL", "FULL" },
                            new StringsAlignmentCustom("PERK", PerkStringHandler),
                            new RecordType[] { "BPTD", "BPTN" },
                            new RecordType[] { "AVIF", "FULL" },
                            new RecordType[] { "LCTN", "FULL" },
                            new RecordType[] { "MESG", "FULL", "ITXT" },
                            new RecordType[] { "WOOP", "FULL", "TNAM" },
                            new RecordType[] { "SHOU", "FULL" },
                            new RecordType[] { "SNCT", "FULL" },
                            new RecordType[] { "CLFM", "FULL" }
                        ));
                    break;
                case StringsSource.DL:
                    ProcessStringsFiles(
                        modKey,
                        dataFolder,
                        language,
                        StringsSource.DL,
                        RenumberStringsFileEntries(
                            modKey,
                            stream,
                            dataFolder,
                            language,
                            StringsSource.DL,
                            new RecordType[] { "SCRL", "DESC" },
                            new RecordType[] { "APPA", "DESC" },
                            new RecordType[] { "AMMO", "DESC" },
                            new RecordType[] { "ARMO", "DESC" },
                            new RecordType[] { "ALCH", "DESC" },
                            new RecordType[] { "WEAP", "DESC" },
                            new RecordType[] { "BOOK", "DESC" },
                            new RecordType[] { "QUST", "CNAM" },
                            new RecordType[] { "LSCR", "DESC" },
                            new RecordType[] { "PERK", "DESC" },
                            new RecordType[] { "AVIF", "DESC" },
                            new RecordType[] { "MESG", "DESC" },
                            new RecordType[] { "SHOU", "DESC" },
                            new RecordType[] { "COLL", "DESC" }
                        ));
                    break;
                case StringsSource.IL:
                    ProcessStringsFiles(
                        modKey,
                        dataFolder,
                        language,
                        StringsSource.IL,
                        RenumberStringsFileEntries(
                            modKey,
                            stream,
                            dataFolder,
                            language,
                            StringsSource.IL,
                            new RecordType[] { "DIAL" },
                            new RecordType[] { "INFO", "NAM1" }
                        ));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
