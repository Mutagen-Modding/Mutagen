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

        protected override void AddDynamicProcessorInstructions()
        {
            base.AddDynamicProcessorInstructions();
            AddDynamicProcessing(RecordTypes.GMST, ProcessGameSettings);
            AddDynamicProcessing(RecordTypes.FURN, ProcessFurniture);
            AddDynamicProcessing(RecordTypes.NPC_, ProcessNpcs);
            AddDynamicProcessing(RecordTypes.REGN, ProcessRegions);
            AddDynamicProcessing(RecordTypes.CELL, ProcessCells);
            AddDynamicProcessing(
                ProcessPlaced,
                PlacedObject_Registration.TriggeringRecordType,
                PlacedNpc_Registration.TriggeringRecordType,
                PlacedArrow_Registration.TriggeringRecordType,
                PlacedBarrier_Registration.TriggeringRecordType,
                PlacedBeam_Registration.TriggeringRecordType,
                PlacedCone_Registration.TriggeringRecordType,
                PlacedFlame_Registration.TriggeringRecordType,
                PlacedHazard_Registration.TriggeringRecordType,
                PlacedMissile_Registration.TriggeringRecordType,
                PlacedTrap_Registration.TriggeringRecordType);
            AddDynamicProcessing(RecordTypes.NAVM, ProcessNavmeshes);
            AddDynamicProcessing(RecordTypes.DIAL, ProcessDialogs);
            AddDynamicProcessing(RecordTypes.QUST, ProcessQuests);
            AddDynamicProcessing(RecordTypes.PACK, ProcessPackages);
            AddDynamicProcessing(RecordTypes.EFSH, ProcessShaders);
            AddDynamicProcessing(RecordTypes.EXPL, ProcessExplosions);
            AddDynamicProcessing(RecordTypes.IMAD, ProcessImageSpaceAdapters);
            AddDynamicProcessing(RecordTypes.LSCR, ProcessLoadScreens);
            AddDynamicProcessing(RecordTypes.ACTI, ProcessActivators);
            AddDynamicProcessing(RecordTypes.WTHR, ProcessWeathers);
        }

        private void ProcessGameSettings(
            MajorRecordFrame majorFrame,
            long fileOffset)
        {
            if (!majorFrame.TryLocateSubrecordFrame("EDID", out var edidFrame)) return;
            if ((char)edidFrame.Content[0] != 'f') return;

            if (!majorFrame.TryLocateSubrecordPinFrame(RecordTypes.DATA, out var dataRec)) return;
            ProcessZeroFloat(dataRec, fileOffset);
        }

        private void ProcessFurniture(
            MajorRecordFrame majorFrame,
            long fileOffset)
        {
            // Find and store marker data
            var data = new Dictionary<int, ReadOnlyMemorySlice<byte>>();
            var indices = new List<int>();
            if (!majorFrame.TryLocateSubrecordFrame(RecordTypes.ENAM, out var enamFrame, out var initialPos)) return;
            var pos = initialPos - majorFrame.HeaderLength;
            while (pos < majorFrame.Content.Length)
            {
                var positions = UtilityTranslation.FindNextSubrecords(
                    majorFrame.Content.Slice(pos),
                    majorFrame.Meta,
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
                enamFrame = majorFrame.Meta.SubrecordFrame(majorFrame.Content.Slice(pos + enamPos.Value));
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
            this._Instructions.SetSubstitution(fileOffset + initialPos, reordered);
        }

        private void ProcessNpcs(
            MajorRecordFrame majorFrame,
            long fileOffset)
        {
            if (majorFrame.TryLocateSubrecordFrame(RecordTypes.QNAM, out var qnamFrame, out var qnamLoc))
            {
                // Standardize float rounding errors
                var r = IBinaryStreamExt.GetColorByte(qnamFrame.Content.Slice(0, 4).Float());
                var g = IBinaryStreamExt.GetColorByte(qnamFrame.Content.Slice(4, 4).Float());
                var b = IBinaryStreamExt.GetColorByte(qnamFrame.Content.Slice(8, 4).Float());
                byte[] bytes = new byte[12];
                using var writer = new MutagenWriter(new MemoryStream(bytes), majorFrame.Meta);
                writer.Write(r / 255f);
                writer.Write(g / 255f);
                writer.Write(b / 255f);
                this._Instructions.SetSubstitution(fileOffset + qnamLoc + qnamFrame.HeaderLength, bytes);
            }
            if (majorFrame.TryLocateSubrecordFrame(RecordTypes.NAM9, out var nam9Frame, out var nam9Loc))
            {
                nam9Loc += nam9Frame.HeaderLength;
                var endPos = nam9Loc + nam9Frame.ContentLength;
                while (nam9Loc < endPos)
                {
                    ProcessZeroFloat(majorFrame, fileOffset, ref nam9Loc);
                }
            }
        }

        private void ProcessRegions(
            MajorRecordFrame majorFrame,
            long fileOffset)
        {
            var rdat = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, majorFrame.Meta, RecordTypes.RDAT, navigateToContent: false);
            if (rdat == null) return;

            // Order RDATs by index
            SortedList<uint, RangeInt64> rdats = new SortedList<uint, RangeInt64>();
            List<uint> raw = new List<uint>();
            while (rdat != null)
            {
                var rdatHeader = majorFrame.Meta.SubrecordFrame(majorFrame.Content.Slice(rdat.Value));
                var index = BinaryPrimitives.ReadUInt32LittleEndian(rdatHeader.Content);
                var nextRdat = UtilityTranslation.FindFirstSubrecord(
                    majorFrame.Content,
                    majorFrame.Meta,
                    RecordTypes.RDAT,
                    navigateToContent: false,
                    offset: rdat.Value + rdatHeader.TotalLength);
                rdats[index] =
                    new RangeInt64(
                        fileOffset + majorFrame.HeaderLength + rdat.Value,
                        nextRdat == null ? fileOffset + majorFrame.TotalLength - 1 : nextRdat.Value - 1 + fileOffset + majorFrame.HeaderLength);
                raw.Add(index);
                rdat = nextRdat;
            }
            if (raw.SequenceEqual(rdats.Keys)) return;
            foreach (var item in rdats.Reverse())
            {
                this._Instructions.SetMove(
                    loc: fileOffset + majorFrame.TotalLength,
                    section: item.Value);
            }
        }

        private void ProcessCells(
            IMutagenReadStream stream,
            MajorRecordFrame majorFrame,
            long fileOffset)
        {
            CleanEmptyCellGroups(
                stream,
                majorFrame.FormID,
                fileOffset,
                numSubGroups: 2);

            // Process odd length changing flags
            var sizeChange = 0;
            if (majorFrame.TryLocateSubrecordFrame(RecordTypes.DATA, out var dataRec, out var dataIndex))
            {
                if (dataRec.ContentLength == 1)
                {
                    _Instructions.SetSubstitution(
                        fileOffset + dataIndex + 4,
                        2);
                    _Instructions.SetAddition(
                        fileOffset + dataIndex + stream.MetaData.Constants.SubConstants.HeaderLength + 1,
                        new byte[] { 0 });
                    sizeChange++;
                }
            }

            ProcessLengths(
                majorFrame,
                sizeChange,
                fileOffset);
        }

        private void ProcessDialogs(
            IMutagenReadStream stream,
            MajorRecordFrame majorFrame,
            long fileOffset)
        {
            CleanEmptyDialogGroups(
                stream,
                majorFrame.FormID,
                fileOffset);

            // Reset misnumbered counter
            if (majorFrame.TryLocateSubrecordFrame(RecordTypes.TIFC, out var tifcRec, out var tifcIndex))
            {
                var count = tifcRec.AsUInt32();

                uint actualCount = 0;
                if (stream.TryReadGroupFrame(out var groupFrame))
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
                        fileOffset + tifcIndex + stream.MetaData.Constants.SubConstants.HeaderLength,
                        b);
                }
            }
        }

        private void ProcessQuests(
            MajorRecordFrame majorFrame,
            long fileOffset)
        {
            if (majorFrame.TryLocateSubrecordFrame(RecordTypes.ANAM, out var anamRec, out var anamIndex))
            {
                var next = anamRec.AsUInt32();
                var targets = new RecordType[]
                {
                    RecordTypes.ALST,
                    RecordTypes.ALLS
                };
                var locs = UtilityTranslation.FindAllOfSubrecords(
                    majorFrame.Content,
                    majorFrame.Meta,
                    targets.ToGetter(),
                    navigateToContent: true);
                uint actualNext = 0;
                if (locs.Length > 0)
                {
                    actualNext = locs
                        .Select(l =>
                        {
                            return BinaryPrimitives.ReadUInt32LittleEndian(majorFrame.Content.Slice(l));
                        })
                        .Max();
                    actualNext++;
                }
                if (actualNext != next)
                {
                    byte[] sub = new byte[4];
                    BinaryPrimitives.WriteUInt32LittleEndian(sub, actualNext);
                    _Instructions.SetSubstitution(
                        fileOffset + anamIndex + anamRec.HeaderLength,
                        sub);
                }
            }

            var sizeChange = FixMissingCounters(
                majorFrame,
                fileOffset,
                new RecordType("COCT"),
                new RecordType("CNTO"));

            ProcessLengths(
                majorFrame,
                sizeChange,
                fileOffset);

            FixVMADFormIDs(
                majorFrame,
                fileOffset,
                out var vmadPos,
                out var objectFormat,
                out var processedLen);
            if (vmadPos != null)
            {
                var vmadFrame = Meta.SubrecordFrame(majorFrame.Content.Slice(vmadPos.Value));
                var stream = new MutagenInterfaceReadStream(
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
                    FixObjectPropertyIDs(stream, fileOffset, objectFormat);
                    // skip version
                    stream.Position += 2;
                    objectFormat = stream.ReadUInt16();
                    var numScripts = stream.ReadUInt16();
                    for (int j = 0; j < numScripts; j++)
                    {
                        FixVMADScriptIDs(stream, fileOffset, objectFormat);
                    }
                }
            }
        }

        public void FixVMADFormIDs(
            MajorRecordFrame frame,
            long fileOffset,
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
                FixVMADScriptIDs(stream, fileOffset, objectFormat);
            }
            processed = (int)(stream.Position - vmadPos.Value - frame.HeaderLength);
        }

        private void FixVMADScriptIDs(IMutagenReadStream stream, long fileOffset, ushort objectFormat)
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
                        FixObjectPropertyIDs(stream, fileOffset, objectFormat);
                        break;
                    case ScriptObjectListProperty objList:
                        var count = stream.ReadUInt32();
                        for (int i = 0; i < count; i++)
                        {
                            FixObjectPropertyIDs(stream, fileOffset, objectFormat);
                        }
                        break;
                    default:
                        prop.CopyInFromBinary(new MutagenFrame(stream));
                        break;
                }
            }
        }

        private void FixObjectPropertyIDs(IMutagenReadStream stream, long fileOffset, ushort objectFormat)
        {
            switch (objectFormat)
            {
                case 2:
                    {
                        stream.Position += 4;
                        long offset = fileOffset + stream.Position - 4;
                        ProcessFormIDOverflow(stream.ReadSpan(4), ref offset);
                    }
                    break;
                case 1:
                    {
                        long offset = fileOffset + stream.Position - 4;
                        ProcessFormIDOverflow(stream.ReadSpan(4), ref offset);
                        stream.Position += 4;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ProcessPlaced(
            MajorRecordFrame majorFrame,
            long fileOffset)
        {
            var sizeChange = 0;

            if (majorFrame.TryLocateSubrecordPinFrame(RecordTypes.DATA, out var dataRec))
            {
                ProcessZeroFloats(dataRec, fileOffset, 6);
            }

            if (majorFrame.TryLocateSubrecordPinFrame(RecordTypes.XTEL, out var xtelRec))
            {
                var offset = 4;
                ProcessZeroFloats(xtelRec, fileOffset, ref offset, 6);
            }

            if (majorFrame.TryLocateSubrecordPinFrame(RecordTypes.XPRM, out var xprmRec))
            {
                int offset = 0;
                ProcessZeroFloats(xprmRec, fileOffset, ref offset, 3);
                ProcessColorFloat(xprmRec, fileOffset, ref offset);
                ProcessZeroFloat(xprmRec, fileOffset, ref offset);
            }

            if (majorFrame.TryLocateSubrecordFrame(RecordTypes.XRMR, out var xrmrRec, out var xrmrIndex))
            {
                if (xrmrRec.AsInt32() == 0)
                {
                    _Instructions.SetRemove(
                        RangeInt64.FactoryFromLength(
                            fileOffset + xrmrIndex,
                            10));
                    sizeChange -= 10;
                }
            }

            ProcessLengths(
                majorFrame,
                sizeChange,
                fileOffset);
        }

        private void ProcessNavmeshes(
            MajorRecordFrame majorFrame,
            long fileOffset)
        {
            if (majorFrame.TryLocateSubrecordFrame(RecordTypes.NVNM, out var nvnmRec, out var nvnmIndex))
            {
                nvnmIndex += nvnmRec.HeaderLength + 16;
                var count = majorFrame.HeaderAndContentData.Slice(nvnmIndex).Int32() * 3;
                nvnmIndex += 4;
                for (int i = 0; i < count; i++)
                {
                    ProcessZeroFloat(majorFrame, fileOffset, ref nvnmIndex);
                }
            }
        }

        private void ProcessPackages(
            MajorRecordFrame majorFrame,
            long fileOffset)
        {
            // Reorder Idle subrecords

            // Reorder data values
            var xnamPos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, majorFrame.Meta, RecordTypes.XNAM);
            if (xnamPos == null)
            {
                throw new ArgumentException();
            }

            if (!majorFrame.TryLocateSubrecordFrame(RecordTypes.PKCU, out var pkcuRec))
            {
                throw new ArgumentException();
            }
            var count = pkcuRec.Content.Int32();

            if (count == 0) return;

            var anamPos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, majorFrame.Meta, RecordTypes.ANAM);
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
                    var anamRecord = majorFrame.Meta.SubrecordFrame(majorFrame.Content.Slice(anamPos.Value));
                    var recs = UtilityTranslation.FindNextSubrecords(
                        majorFrame.Content.Slice(anamPos.Value + anamRecord.TotalLength),
                        majorFrame.Meta,
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
                    var finalRec = majorFrame.Meta.Subrecord(majorFrame.Content.Slice(anamPos.Value + anamRecord.TotalLength + finalLoc));
                    var dataSlice = majorFrame.Content.Slice(anamPos.Value, anamRecord.TotalLength + finalLoc + finalRec.TotalLength);
                    if (BinaryStringUtility.ProcessWholeToZString(anamRecord.Content) == "Bool"
                        && recs[1] != null)
                    {
                        // Ensure bool value is 1 or 0
                        var cnam = majorFrame.Meta.SubrecordFrame(majorFrame.Content.Slice(anamPos.Value + anamRecord.TotalLength + recs[1].Value));
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
                    majorFrame.Meta,
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
                    var unamRec = majorFrame.Meta.SubrecordFrame(majorFrame.Content.Slice(curLoc + unamLocs[i]));
                    dataValues[i] = (
                        (sbyte)unamRec.Content[0],
                        dataValues[i].Data);
                }

                var subLoc = startLoc;
                foreach (var item in dataValues.OrderBy(i => i.Index))
                {
                    _Instructions.SetSubstitution(
                        fileOffset + majorFrame.HeaderLength + subLoc,
                        item.Data.ToArray());
                    subLoc += item.Data.Length;
                }
                foreach (var item in dataValues.OrderBy(i => i.Index))
                {
                    byte[] bytes = new byte[] { 0x55, 0x4E, 0x41, 0x4D, 0x01, 0x00, 0x00 };
                    bytes[6] = (byte)item.Index;
                    _Instructions.SetSubstitution(
                        fileOffset + majorFrame.HeaderLength + subLoc,
                        bytes.ToArray());
                    subLoc += bytes.Length;
                }
            }

            // Reorder inputs
            var unamPos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content.Slice(xnamPos.Value), majorFrame.Meta, unam);
            if (!unamPos.HasValue) return;
            unamPos += xnamPos.Value;
            var writeLoc = fileOffset + majorFrame.HeaderLength + unamPos.Value;
            var inputValues = new List<(sbyte Index, ReadOnlyMemorySlice<byte> Data)>();
            while (unamPos.HasValue)
            {
                var unamRecord = majorFrame.Meta.SubrecordFrame(majorFrame.Content.Slice(unamPos.Value));
                var recs = UtilityTranslation.FindNextSubrecords(
                    majorFrame.Content.Slice(unamPos.Value + unamRecord.TotalLength),
                    majorFrame.Meta,
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
                var finalRec = majorFrame.Meta.Subrecord(majorFrame.Content.Slice(unamPos.Value + unamRecord.TotalLength + finalLoc));
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
            MajorRecordFrame majorFrame,
            long fileOffset)
        {
            if (majorFrame.TryLocateSubrecordPinFrame(RecordTypes.DATA, out var dataRec))
            {
                var index = 20;
                ProcessZeroFloats(dataRec, fileOffset, ref index, 9);
                index += 4;
                ProcessZeroFloats(dataRec, fileOffset, ref index, 8);
                index += 20;
                ProcessZeroFloats(dataRec, fileOffset, ref index, 19);
                index += 12;
                ProcessZeroFloats(dataRec, fileOffset, ref index, 11);
                index += 4;
                ProcessZeroFloats(dataRec, fileOffset, ref index, 5);
                index += 4;
                ProcessZeroFloat(dataRec, fileOffset, ref index);
                index += 8;
                ProcessZeroFloats(dataRec, fileOffset, ref index, 6);
                index += 12;
                ProcessZeroFloats(dataRec, fileOffset, ref index, 9);
                index += 32;
                ProcessZeroFloats(dataRec, fileOffset, ref index, 2);
            }
        }

        private void ProcessExplosions(
            MajorRecordFrame majorFrame,
            long fileOffset)
        {
            if (majorFrame.TryLocateSubrecordPinFrame(RecordTypes.DATA, out var dataRec))
            {
                int offset = 0;
                ProcessFormIDOverflows(dataRec, fileOffset, ref offset, 6);
                ProcessZeroFloats(dataRec, fileOffset, ref offset, 5);
            }
        }

        private void ProcessImageSpaceAdapters(
            MajorRecordFrame majorFrame,
            long fileOffset)
        {
            void ProcessKeyframe(SubrecordPinFrame subrecord)
            {
                ProcessZeroFloats(subrecord, fileOffset, subrecord.ContentLength / 4);
            }

            foreach (var subrecord in majorFrame)
            {
                switch (subrecord.RecordTypeInt)
                {
                    case RecordTypeInts.QIAD:
                    case RecordTypeInts.RIAD:
                        ProcessKeyframe(subrecord);
                        break;
                    default:
                        break;
                }
            }

            if (majorFrame.TryLocateSubrecordPinFrame(RecordTypes.DATA, out var dataRec))
            {
                int offset = 0;
                ProcessFormIDOverflows(dataRec, fileOffset, ref offset, 6);
                ProcessZeroFloats(dataRec, fileOffset, ref offset, 5);
            }
        }

        private void ProcessLoadScreens(
            MajorRecordFrame majorFrame,
            long fileOffset)
        {
            if (majorFrame.TryLocateSubrecordPinFrame(RecordTypes.XNAM, out var xnamRec))
            {
                ProcessZeroFloats(xnamRec, fileOffset, 3);
            }
        }

        private void ProcessActivators(
            MajorRecordFrame majorFrame,
            long fileOffset)
        {
            FixVMADFormIDs(
                majorFrame,
                fileOffset,
                out var vmadPos,
                out var objectFormat,
                out var processedLen);
        }

        private void ProcessWeathers(
            MajorRecordFrame majorFrame,
            long fileOffset)
        {
            if (majorFrame.TryLocateSubrecordFrame(RecordTypes.SNAM, out var _, out var initialIndex))
            {
                foreach (var snam in majorFrame.FindEnumerateSubrecords(RecordTypes.SNAM))
                {
                    ProcessFormIDOverflow(snam, fileOffset);
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
            BinaryFileProcessor.ConfigConstructor instr,
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
