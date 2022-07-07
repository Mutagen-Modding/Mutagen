using System.Buffers.Binary;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Internals;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;
using Noggog;

namespace Mutagen.Bethesda.Tests;

public class SkyrimProcessor : Processor
{
    public override GameRelease GameRelease { get; }
    public override bool StrictStrings => true;
    
    protected override Dictionary<(ModKey ModKey, StringsSource Source), HashSet<uint>>? KnownDeadStringKeys()
    {
        return new Dictionary<(ModKey ModKey, StringsSource Source), HashSet<uint>>
        {
            { (Constants.Update, StringsSource.Normal), new() { 34 } }
        };
    }

    public SkyrimProcessor(GameRelease release, bool multithread)
        : base(multithread)
    {
        GameRelease = release;
    }

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
        if (!majorFrame.TryFindSubrecord("EDID", out var edidFrame)) return;
        if ((char)edidFrame.Content[0] != 'f') return;

        if (!majorFrame.TryFindSubrecord(RecordTypes.DATA, out var dataRec)) return;
        ProcessZeroFloat(dataRec, fileOffset);
    }

    private void ProcessFurniture(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        // Find and store marker data
        var data = new Dictionary<int, ReadOnlyMemorySlice<byte>>();
        var indices = new List<int>();
        if (!majorFrame.TryFindSubrecord(RecordTypes.ENAM, out var rec)) return;
        var pos = rec.Location - majorFrame.HeaderLength;
        while (pos < majorFrame.Content.Length)
        {
            var positions = RecordSpanExtensions.TryFindNextSubrecords(
                majorFrame.Content.Slice(pos),
                majorFrame.Meta,
                out var lenParsed,
                stopOnAlreadyEncounteredRecord: true,
                new RecordType[]
                {
                    RecordTypes.ENAM,
                    new("NAM0"),
                    new("FNMK"),
                });
            var enam = positions[0];
            if (enam == null) break;
            var index = enam.Value.AsInt32();
            data.Add(index, majorFrame.Content.Slice(pos + enam.Value.Location, lenParsed));
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

        _instructions.SetSubstitution(fileOffset + rec.Location, reordered);
    }

    private void ProcessNpcs(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.QNAM, out var qnamFrame))
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
            _instructions.SetSubstitution(fileOffset + qnamFrame.Location + qnamFrame.HeaderLength, bytes);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.NAM9, out var nam9Frame))
        {
            var nam9Loc = nam9Frame.Location;
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
        var rdat = RecordSpanExtensions.TryFindSubrecord(majorFrame.Content, majorFrame.Meta, RecordTypes.RDAT);
        if (rdat == null) return;

        // Order RDATs by index
        SortedList<uint, RangeInt64> rdats = new SortedList<uint, RangeInt64>();
        List<uint> raw = new List<uint>();
        while (rdat != null)
        {
            var index = BinaryPrimitives.ReadUInt32LittleEndian(rdat.Value.Content);
            var nextRdat = RecordSpanExtensions.TryFindSubrecord(
                majorFrame.Content,
                majorFrame.Meta,
                RecordTypes.RDAT,
                offset: rdat.Value.EndLocation);
            rdats[index] =
                new RangeInt64(
                    fileOffset + majorFrame.HeaderLength + rdat.Value.Location,
                    nextRdat == null
                        ? fileOffset + majorFrame.TotalLength - 1
                        : nextRdat.Value.Location - 1 + fileOffset + majorFrame.HeaderLength);
            raw.Add(index);
            rdat = nextRdat;
        }

        if (raw.SequenceEqual(rdats.Keys)) return;
        foreach (var item in rdats.Reverse())
        {
            _instructions.SetMove(
                loc: fileOffset + majorFrame.TotalLength,
                section: item.Value);
        }
    }

    private void ProcessCells(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        var formKey = FormKey.Factory(stream.MetaData.MasterReferences!, majorFrame.FormID.Raw);
        CleanEmptyCellGroups(
            stream,
            formKey,
            fileOffset,
            numSubGroups: 2);
        
        if (majorFrame.TryFindSubrecord(RecordTypes.MHDT, out var pin))
        {
            ProcessZeroFloat(pin, offsetLoc: fileOffset);
        }

        // Process odd length changing flags
        var sizeChange = 0;
        if (majorFrame.TryFindSubrecord(RecordTypes.DATA, out var dataRec))
        {
            if (dataRec.ContentLength == 1)
            {
                _instructions.SetSubstitution(
                    fileOffset + dataRec.Location + 4,
                    2);
                _instructions.SetAddition(
                    fileOffset + dataRec.Location + stream.MetaData.Constants.SubConstants.HeaderLength + 1,
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
        var formKey = FormKey.Factory(stream.MetaData.MasterReferences!, majorFrame.FormID.Raw);
        CleanEmptyDialogGroups(
            stream,
            formKey,
            fileOffset);

        // Reset misnumbered counter
        if (majorFrame.TryFindSubrecord(RecordTypes.TIFC, out var tifcRec))
        {
            var count = tifcRec.AsUInt32();

            uint actualCount = 0;
            stream.Position = fileOffset + majorFrame.TotalLength;
            if (stream.TryReadGroup(out var groupFrame))
            {
                int groupPos = 0;
                while (groupPos < groupFrame.Content.Length)
                {
                    var majorMeta = stream.MetaData.Constants.MajorRecordHeader(groupFrame.Content.Slice(groupPos));
                    actualCount++;
                    groupPos += checked((int)majorMeta.TotalLength);
                }
            }

            if (actualCount != count)
            {
                byte[] b = new byte[4];
                BinaryPrimitives.WriteUInt32LittleEndian(b, actualCount);
                _instructions.SetSubstitution(
                    fileOffset + tifcRec.Location + stream.MetaData.Constants.SubConstants.HeaderLength,
                    b);
            }
        }
    }

    private void ProcessQuests(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.ANAM, out var anamRec))
        {
            var next = anamRec.AsUInt32();
            var targets = new RecordType[]
            {
                RecordTypes.ALST,
                RecordTypes.ALLS
            };
            var locs = RecordSpanExtensions.FindAllOfSubrecords(
                majorFrame.Content,
                majorFrame.Meta,
                targets);
            uint actualNext = 0;
            if (locs.Count > 0)
            {
                actualNext = locs
                    .Select(l => l.AsUInt32())
                    .Max();
                actualNext++;
            }

            if (actualNext != next)
            {
                byte[] sub = new byte[4];
                BinaryPrimitives.WriteUInt32LittleEndian(sub, actualNext);
                _instructions.SetSubstitution(
                    fileOffset + anamRec.Location + anamRec.HeaderLength,
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
            out var vmad,
            out var objectFormat,
            out var processedLen);
        if (vmad != null)
        {
            var stream = new MutagenMemoryReadStream(vmad.Value.Content, Bundle)
            {
                Position = processedLen - vmad.Value.HeaderLength
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
        out SubrecordPinFrame? vmad,
        out ushort objectFormat,
        out int processed)
    {
        vmad = RecordSpanExtensions.TryFindSubrecord(frame.Content, Meta, RecordTypes.VMAD);
        if (vmad == null)
        {
            processed = 0;
            objectFormat = 0;
            return;
        }

        var stream = new MutagenMemoryReadStream(frame.HeaderAndContentData, Bundle)
        {
            Position = vmad.Value.Location + frame.HeaderLength
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

        processed = (int)(stream.Position - vmad.Value.Location - frame.HeaderLength);
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
                long offset = fileOffset + stream.Position;
                ProcessFormIDOverflow(stream.ReadSpan(4), ref offset);
            }
                break;
            case 1:
            {
                long offset = fileOffset + stream.Position;
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

        if (majorFrame.TryFindSubrecord(RecordTypes.DATA, out var dataRec))
        {
            ProcessZeroFloats(dataRec, fileOffset, 6);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XTEL, out var xtelRec))
        {
            var offset = 4;
            ProcessZeroFloats(xtelRec, fileOffset, ref offset, 6);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XPRM, out var xprmRec))
        {
            int offset = 0;
            ProcessZeroFloats(xprmRec, fileOffset, ref offset, 3);
            ProcessColorFloat(xprmRec, fileOffset, ref offset, alpha: false);
            ProcessZeroFloat(xprmRec, fileOffset, ref offset);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XRMR, out var xrmrRec))
        {
            if (xrmrRec.AsInt32() == 0)
            {
                _instructions.SetRemove(
                    RangeInt64.FromLength(
                        fileOffset + xrmrRec.Location,
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
        if (majorFrame.TryFindSubrecord(RecordTypes.NVNM, out var nvnmRec))
        {
            var nvnmIndex = nvnmRec.Location;
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
        var xnamPos =
            RecordSpanExtensions.TryFindSubrecord(majorFrame.Content, majorFrame.Meta, RecordTypes.XNAM)?.Location;
        if (xnamPos == null)
        {
            throw new ArgumentException();
        }

        if (!majorFrame.TryFindSubrecord(RecordTypes.PKCU, out var pkcuRec))
        {
            throw new ArgumentException();
        }

        var count = pkcuRec.Content.Int32();

        if (count == 0) return;

        var anamPos =
            RecordSpanExtensions.TryFindSubrecord(majorFrame.Content, majorFrame.Meta, RecordTypes.ANAM)?.Location;
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
                var anamRecord = majorFrame.Meta.Subrecord(majorFrame.Content.Slice(anamPos.Value));
                var recs = RecordSpanExtensions.TryFindNextSubrecords(
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
                    finalLoc = recs.NotNull().Select(x => x.Location).Max();
                }
                else if (recs[0]!.Value.Location == 0)
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
                        .Select(x => x.Location)
                        .Where(i => i < recs[0]!.Value.Location)
                        .Max();
                }

                var finalRec =
                    majorFrame.Meta.SubrecordHeader(
                        majorFrame.Content.Slice(anamPos.Value + anamRecord.TotalLength + finalLoc));
                var dataSlice = majorFrame.Content.Slice(anamPos.Value,
                    anamRecord.TotalLength + finalLoc + finalRec.TotalLength);
                if (BinaryStringUtility.ProcessWholeToZString(anamRecord.Content, MutagenEncodingProvider._1252) ==
                    "Bool"
                    && recs[1] != null)
                {
                    // Ensure bool value is 1 or 0
                    var cnam = majorFrame.Meta.Subrecord(
                        majorFrame.Content.Slice(anamPos.Value + anamRecord.TotalLength + recs[1].Value.Location));
                    if (cnam.Content.Length != 1)
                    {
                        throw new ArgumentException();
                    }

                    if (cnam.Content[0] > 1)
                    {
                        var bytes = dataSlice.ToArray();
                        int boolIndex = anamRecord.TotalLength + recs[1].Value.Location + cnam.HeaderLength;
                        bytes[boolIndex] = (byte)(bytes[boolIndex] > 0 ? 1 : 0);
                        dataSlice = bytes;
                    }
                }

                dataValues.Add((-1, dataSlice));

                curLoc = anamPos.Value + anamRecord.TotalLength + finalLoc + finalRec.TotalLength;
                anamPos = anamPos.Value + anamRecord.TotalLength + recs[0]?.Location;
            }

            var unamLocs = RecordSpanExtensions.ParseRepeatingSubrecord(
                majorFrame.Content.Slice(curLoc),
                majorFrame.Meta,
                unam,
                out var _);
            if (unamLocs == null
                || unamLocs.Count != dataValues.Count
                || unamLocs.Count != count)
            {
                throw new ArgumentException();
            }

            for (sbyte i = 0; i < unamLocs.Count; i++)
            {
                var unamRec = majorFrame.Meta.Subrecord(majorFrame.Content.Slice(curLoc + unamLocs[i].Location));
                dataValues[i] = (
                    (sbyte)unamRec.Content[0],
                    dataValues[i].Data);
            }

            var subLoc = startLoc;
            foreach (var item in dataValues.OrderBy(i => i.Index))
            {
                _instructions.SetSubstitution(
                    fileOffset + majorFrame.HeaderLength + subLoc,
                    item.Data.ToArray());
                subLoc += item.Data.Length;
            }

            foreach (var item in dataValues.OrderBy(i => i.Index))
            {
                byte[] bytes = new byte[] { 0x55, 0x4E, 0x41, 0x4D, 0x01, 0x00, 0x00 };
                bytes[6] = (byte)item.Index;
                _instructions.SetSubstitution(
                    fileOffset + majorFrame.HeaderLength + subLoc,
                    bytes.ToArray());
                subLoc += bytes.Length;
            }
        }

        // Reorder inputs
        var unamPos =
            RecordSpanExtensions.TryFindSubrecord(majorFrame.Content.Slice(xnamPos.Value), majorFrame.Meta, unam)?.Location;
        if (!unamPos.HasValue) return;
        unamPos += xnamPos.Value;
        var writeLoc = fileOffset + majorFrame.HeaderLength + unamPos.Value;
        var inputValues = new List<(sbyte Index, ReadOnlyMemorySlice<byte> Data)>();
        while (unamPos.HasValue)
        {
            var unamRecord = majorFrame.Meta.Subrecord(majorFrame.Content.Slice(unamPos.Value));
            var recs = RecordSpanExtensions.TryFindNextSubrecords(
                majorFrame.Content.Slice(unamPos.Value + unamRecord.TotalLength),
                majorFrame.Meta,
                out var _,
                unam,
                bnam,
                pnam);
            int finalLoc;
            if (recs[0] == null)
            {
                finalLoc = recs.NotNull().Select(x => x.Location).Max();
            }
            else if (recs[0].Value.Location == 0)
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
                    .Select(x => x.Location)
                    .Where(i => i < recs[0]!.Value.Location)
                    .Max();
            }

            var finalRec =
                majorFrame.Meta.SubrecordHeader(majorFrame.Content.Slice(unamPos.Value + unamRecord.TotalLength + finalLoc));
            inputValues.Add(
                ((sbyte)unamRecord.Content[0],
                    majorFrame.Content.Slice(unamPos.Value, unamRecord.TotalLength + finalLoc + finalRec.TotalLength)));

            unamPos = unamPos.Value + unamRecord.TotalLength + recs[0]?.Location;
        }

        foreach (var item in inputValues.OrderBy(i => i.Index))
        {
            _instructions.SetSubstitution(
                writeLoc,
                item.Data.ToArray());
            writeLoc += item.Data.Length;
        }
    }

    private void ProcessShaders(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.DATA, out var dataRec))
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
        if (majorFrame.TryFindSubrecord(RecordTypes.DATA, out var dataRec))
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

        if (majorFrame.TryFindSubrecord(RecordTypes.DATA, out var dataRec))
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
        if (majorFrame.TryFindSubrecord(RecordTypes.XNAM, out var xnamRec))
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
        foreach (var snam in majorFrame.FindEnumerateSubrecords(RecordTypes.SNAM))
        {
            ProcessFormIDOverflow(snam, fileOffset);
        }
    }

    public void PerkStringHandler(
        IMutagenReadStream stream,
        MajorRecordHeader major,
        List<StringEntry> processedStrings,
        IStringsLookup overlay)
    {
        var majorCompletePos = stream.Position + major.ContentLength;
        long? lastepft = null;
        while (stream.Position < majorCompletePos)
        {
            var sub = stream.GetSubrecordHeader();
            switch (sub.RecordTypeInt)
            {
                case RecordTypeInts.FULL:
                case RecordTypeInts.EPF2:
                    AStringsAlignment.ProcessStringLink(stream, processedStrings, overlay);
                    break;
                case RecordTypeInts.EPFT:
                    lastepft = stream.Position;
                    break;
                case RecordTypeInts.EPFD:
                    var pos = stream.Position;
                    stream.Position = lastepft.Value;
                    var epftFrame = stream.ReadSubrecord();
                    if (epftFrame.Content[0] == (byte)APerkEntryPointEffect.ParameterType.LString)
                    {
                        stream.Position = pos;
                        AStringsAlignment.ProcessStringLink(stream, processedStrings, overlay);
                    }

                    stream.Position = pos;
                    break;
                default:
                    break;
            }

            stream.Position += sub.TotalLength;
        }
    }

    public void GameSettingStringHandler(
        IMutagenReadStream stream,
        MajorRecordHeader major,
        List<StringEntry> processedStrings,
        IStringsLookup overlay)
    {
        stream.Position -= major.HeaderLength;
        var majorRec = stream.GetMajorRecord();
        if (!majorRec.TryFindSubrecord("EDID", out var edidRec)) throw new ArgumentException();
        if (edidRec.Content[0] != (byte)'s') return;
        if (!majorRec.TryFindSubrecord("DATA", out var dataRec)) throw new ArgumentException();
        stream.Position += dataRec.Location;
        AStringsAlignment.ProcessStringLink(stream, processedStrings, overlay);
    }

    protected override AStringsAlignment[] GetStringsFileAlignments(
        StringsSource source)
    {
        switch (source)
        {
            case StringsSource.Normal:
                return new AStringsAlignment[]
                {
                    new StringsAlignmentCustom("GMST", GameSettingStringHandler),
                    new RecordType[] { "LSCR", "DESC" },
                    new RecordType[] { "ACTI", "FULL", "RNAM" },
                    new RecordType[] { "APPA", "FULL" },
                    new RecordType[] { "AMMO", "FULL" },
                    new RecordType[] { "ARMO", "FULL" },
                    new RecordType[] { "BOOK", "FULL" },
                    new RecordType[] { "CLAS", "FULL" },
                    new RecordType[] { "EYES", "FULL" },
                    new RecordType[] { "CONT", "FULL" },
                    new RecordType[] { "DOOR", "FULL" },
                    new RecordType[] { "FACT", "FULL", "MNAM", "FNAM" },
                    new RecordType[] { "FURN", "FULL" },
                    new RecordType[] { "HAZD", "FULL" },
                    new RecordType[] { "HDPT", "FULL" },
                    new RecordType[] { "ALCH", "FULL" },
                    new RecordType[] { "INGR", "FULL" },
                    new RecordType[] { "LIGH", "FULL" },
                    new RecordType[] { "MGEF", "FULL", "DNAM" },
                    new RecordType[] { "MISC", "FULL" },
                    new RecordType[] { "MSTT", "FULL" },
                    new RecordType[] { "NPC_", "FULL", "SHRT" },
                    new RecordType[] { "ENCH", "FULL" },
                    new RecordType[] { "PROJ", "FULL" },
                    new RecordType[] { "RACE", "FULL" },
                    new RecordType[] { "SCRL", "FULL" },
                    new RecordType[] { "SLGM", "FULL" },
                    new RecordType[] { "SPEL", "FULL" },
                    new RecordType[] { "TACT", "FULL" },
                    new RecordType[] { "TREE", "FULL" },
                    new RecordType[] { "WEAP", "FULL" },
                    new RecordType[] { "FLOR", "FULL", "RNAM" },
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
                    new RecordType[] { "CLFM", "FULL" },
                    new RecordType[] { "REGN", "RDMP" }
                };
            case StringsSource.DL:
                return new AStringsAlignment[]
                {
                    new RecordType[] { "SCRL", "DESC" },
                    new RecordType[] { "APPA", "DESC" },
                    new RecordType[] { "AMMO", "DESC" },
                    new RecordType[] { "ARMO", "DESC" },
                    new RecordType[] { "ALCH", "DESC" },
                    new RecordType[] { "WEAP", "DESC" },
                    new RecordType[] { "BOOK", "DESC", "CNAM" },
                    new RecordType[] { "QUST", "CNAM" },
                    new RecordType[] { "PERK", "DESC" },
                    new RecordType[] { "AVIF", "DESC" },
                    new RecordType[] { "MESG", "DESC" },
                    new RecordType[] { "SHOU", "DESC" },
                    new RecordType[] { "COLL", "DESC" },
                    new RecordType[] { "RACE", "DESC" },
                    new RecordType[] { "SPEL", "DESC" }
                };
            case StringsSource.IL:
                return new AStringsAlignment[]
                {
                    new RecordType[] { "DIAL" },
                    new RecordType[] { "INFO", "NAM1" }
                };
            default:
                throw new NotImplementedException();
        }
    }
}