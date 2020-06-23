using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        public override GameMode GameMode => GameMode.Skyrim;

        protected override void AddDynamicProcessorInstructions(IMutagenReadStream stream, FormID formID, RecordType recType)
        {
            base.AddDynamicProcessorInstructions(stream, formID, recType);
            var loc = this._AlignedFileLocs[formID];
            ProcessGameSettings(stream, formID, recType, loc);
            ProcessRaces(stream, formID, recType, loc);
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
            stream.Position = loc.Min + majorFrame.Header.HeaderLength + dataIndex.Value;
            ProcessZeroFloat(stream);
        }

        private void ProcessRaces(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Race_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordFrame();
            ProcessPhonemes(stream, formID, majorFrame, loc);
        }

        private void ProcessPhonemes(
            IMutagenReadStream stream,
            FormID formID,
            MajorRecordFrame majorFrame,
            RangeInt64 loc)
        {
            var phonemeListLoc = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, this.Meta, new RecordType("PHTN"));
            if (phonemeListLoc == null) return;
            stream.Position = loc.Min + phonemeListLoc.Value + majorFrame.Header.HeaderLength;

            var phonemeSpan = majorFrame.Content.Slice(phonemeListLoc.Value);
            var finds = UtilityTranslation.FindRepeatingSubrecord(
                phonemeSpan,
                this.Meta,
                new RecordType("PHTN"),
                out var lenParsed);
            if (finds?.Length != 16) return;
            HashSet<FaceFxPhonemes.Target> targets = new HashSet<FaceFxPhonemes.Target>();
            targets.Add(EnumExt.GetValues<FaceFxPhonemes.Target>());
            foreach (var find in finds)
            {
                var subRecord = this.Meta.SubrecordFrame(phonemeSpan.Slice(find));
                var str = BinaryStringUtility.ProcessWholeToZString(subRecord.Content);
                var target = FaceFxPhonemesMixIn.GetTarget(str, out var lipMode);
                if (lipMode) return;
                targets.Remove(target);
            }
            if (targets.Count > 0) return;

            // Remove fully populated phonemes list
            this._Instructions.SetRemove(RangeInt64.FactoryFromLength(stream.Position, lenParsed));
            ModifyLengths(stream, -lenParsed, formID, loc.Min, null);
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
            this._Instructions.SetSubstitution(loc.Min + majorFrame.Header.HeaderLength + initialPos.Value, reordered);
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
                this._Instructions.SetSubstitution(loc.Min + qnam.Value + majorFrame.Header.HeaderLength, bytes);
            }
            var nam9 = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.NAM9);
            if (nam9 != null)
            {
                // Standardize floats
                var subRecord = stream.MetaData.Constants.Subrecord(majorFrame.Content.Slice(nam9.Value), RecordTypes.NAM9);
                stream.Position = loc.Min + nam9.Value + majorFrame.Header.HeaderLength + subRecord.HeaderLength;
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
                    offset: rdat.Value + rdatHeader.Header.TotalLength);
                rdats[index] =
                    new RangeInt64(
                        loc.Min + majorFrame.Header.HeaderLength + rdat.Value,
                        nextRdat == null ? loc.Max : nextRdat.Value - 1 + loc.Min + majorFrame.Header.HeaderLength);
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
                        loc.Min + majorFrame.Header.HeaderLength + pos.Value + 4,
                        2);
                    _Instructions.SetAddition(
                        loc.Min + majorFrame.Header.HeaderLength + pos.Value + stream.MetaData.Constants.SubConstants.HeaderLength + 1,
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
                        loc.Min + majorFrame.Header.HeaderLength + pos.Value + anamFrame.Header.HeaderLength,
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
                    new MutagenMemoryReadStream(vmadFrame.Content, new ParsingBundle(GameMode)),
                    new ParsingBundle(GameMode))
                {
                    Position = processedLen - vmadFrame.Header.HeaderLength
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
                new MutagenMemoryReadStream(frame.Content.Slice(vmadPos.Value), new ParsingBundle(GameMode)),
                new ParsingBundle(GameMode));
            stream.Position += Meta.SubConstants.HeaderLength;
            // Skip version
            stream.Position += 2;
            objectFormat = stream.ReadUInt16();
            var scriptCt = stream.ReadUInt16();
            for (int i = 0; i < scriptCt; i++)
            {
                FixVMADScriptIDs(stream, loc, objectFormat);
            }
            processed = (int)stream.Position;
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
                    //ScriptProperty.Type.ArrayOfString => new ScriptStringListProperty(),
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
                        throw new NotImplementedException();
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
                stream.Position = loc.Min + majorFrame.Header.HeaderLength + pos.Value + stream.MetaData.Constants.SubConstants.HeaderLength;
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
                stream.Position = loc.Min + majorFrame.Header.HeaderLength + pos.Value + stream.MetaData.Constants.SubConstants.HeaderLength;
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
                stream.Position = loc.Min + majorFrame.Header.HeaderLength + pos.Value + stream.MetaData.Constants.SubConstants.HeaderLength;
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessColorFloat(stream);
                ProcessZeroFloat(stream);
            }

            pos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData.Constants, RecordTypes.XRMR);
            if (pos != null)
            {
                stream.Position = loc.Min + majorFrame.Header.HeaderLength + pos.Value + stream.MetaData.Constants.SubConstants.HeaderLength;
                var val = stream.ReadInt32();
                if (val == 0)
                {
                    _Instructions.SetRemove(
                        RangeInt64.FactoryFromLength(
                            loc.Min + majorFrame.Header.HeaderLength + pos.Value,
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
                stream.Position = loc.Min + majorFrame.Header.HeaderLength + pos.Value + stream.MetaData.Constants.SubConstants.HeaderLength;
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
                            int boolIndex = anamRecord.TotalLength + recs[1].Value + cnam.Header.HeaderLength;
                            bytes[boolIndex] = (byte)(bytes[boolIndex] > 0 ? 1 : 0);
                            dataSlice = bytes;
                        }
                    }
                    dataValues.Add((-1, dataSlice));

                    curLoc = anamPos.Value + anamRecord.TotalLength + finalLoc + finalRec.TotalLength;
                    anamPos = anamPos.Value + anamRecord.TotalLength + recs[0];
                }

                var unamLocs = UtilityTranslation.FindRepeatingSubrecord(
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
                        loc.Min + majorFrame.Header.HeaderLength + subLoc,
                        item.Data.ToArray());
                    subLoc += item.Data.Length;
                }
                foreach (var item in dataValues.OrderBy(i => i.Index))
                {
                    byte[] bytes = new byte[] { 0x55, 0x4E, 0x41, 0x4D, 0x01, 0x00, 0x00 };
                    bytes[6] = (byte)item.Index;
                    _Instructions.SetSubstitution(
                        loc.Min + majorFrame.Header.HeaderLength + subLoc,
                        bytes.ToArray());
                    subLoc += bytes.Length;
                }
            }

            // Reorder inputs
            var unamPos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content.Slice(xnamPos.Value), stream.MetaData.Constants, unam);
            if (!unamPos.HasValue) return;
            unamPos += xnamPos.Value;
            var writeLoc = loc.Min + majorFrame.Header.HeaderLength + unamPos.Value;
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
                stream.Position = loc.Min + majorFrame.Header.HeaderLength + pos.Value + stream.MetaData.Constants.SubConstants.HeaderLength;
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
                stream.Position = loc.Min + majorFrame.Header.HeaderLength + pos.Value + stream.MetaData.Constants.SubConstants.HeaderLength;
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
                stream.Position = loc.Min + majorFrame.Header.HeaderLength + pos.Value + stream.MetaData.Constants.SubConstants.HeaderLength;
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

        protected override void PreProcessorJobs(IMutagenReadStream stream)
        {
            base.PreProcessorJobs(stream);
            ProcessStringsFilesIndices(stream, Language.English);
        }

        public void PerkStringHandler(
            IMutagenReadStream stream,
            MajorRecordHeader major, 
            BinaryFileProcessor.Config instr, 
            List<KeyValuePair<uint, uint>> processedStrings,
            StringsLookupOverlay overlay, 
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

        private void ProcessStringsFilesIndices(IMutagenReadStream stream, Language language)
        {
            var stringsFolder = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(this.SourcePath), "Strings"));
            ProcessStringsFiles(
                stringsFolder,
                language,
                StringsSource.Normal,
                RenumberStringsFileEntries(
                    stream,
                    language,
                    StringsSource.Normal,
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
                    new RecordType[] { "MESG", "FULL", "ITXT" }
                ));
            ProcessStringsFiles(
                stringsFolder,
                language,
                StringsSource.DL,
                RenumberStringsFileEntries(
                    stream,
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
                    new RecordType[] { "MESG", "DESC" }
                ));
            ProcessStringsFiles(
                stringsFolder,
                language,
                StringsSource.IL,
                RenumberStringsFileEntries(
                    stream,
                    language,
                    StringsSource.IL,
                    new RecordType[] { "DIAL" },
                    new RecordType[] { "INFO", "NAM1" }
                ));
        }
    }
}
