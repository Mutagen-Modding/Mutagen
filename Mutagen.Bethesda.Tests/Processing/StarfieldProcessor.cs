using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Starfield.Internals;
using Mutagen.Bethesda.Strings;
using Noggog;
using APerkEntryPointEffect = Mutagen.Bethesda.Starfield.APerkEntryPointEffect;
using ScriptBoolListProperty = Mutagen.Bethesda.Starfield.ScriptBoolListProperty;
using ScriptBoolProperty = Mutagen.Bethesda.Starfield.ScriptBoolProperty;
using ScriptFloatListProperty = Mutagen.Bethesda.Starfield.ScriptFloatListProperty;
using ScriptFloatProperty = Mutagen.Bethesda.Starfield.ScriptFloatProperty;
using ScriptIntListProperty = Mutagen.Bethesda.Starfield.ScriptIntListProperty;
using ScriptIntProperty = Mutagen.Bethesda.Starfield.ScriptIntProperty;
using ScriptObjectListProperty = Mutagen.Bethesda.Starfield.ScriptObjectListProperty;
using ScriptObjectProperty = Mutagen.Bethesda.Starfield.ScriptObjectProperty;
using ScriptProperty = Mutagen.Bethesda.Starfield.ScriptProperty;
using ScriptStringListProperty = Mutagen.Bethesda.Starfield.ScriptStringListProperty;
using ScriptStringProperty = Mutagen.Bethesda.Starfield.ScriptStringProperty;

namespace Mutagen.Bethesda.Tests;

public class StarfieldProcessor : Processor
{
    public override bool StrictStrings => false;

    public StarfieldProcessor(bool multithread) : base(multithread)
    {
    }

    public override GameRelease GameRelease => GameRelease.Starfield;

    protected override void AddDynamicProcessorInstructions()
    {
        base.AddDynamicProcessorInstructions();
        AddDynamicProcessing(RecordTypes.GMST, ProcessGameSettings);
        AddDynamicProcessing(RecordTypes.TRNS, ProcessTransforms);
        AddDynamicProcessing(RecordTypes.SCOL, ProcessStaticCollections);
        AddDynamicProcessing(RecordTypes.BNDS, ProcessBendableSplines);
        AddDynamicProcessing(RecordTypes.PDCL, ProcessProjectedDecals);
        AddDynamicProcessing(RecordTypes.MISC, ProcessMisc);
        AddDynamicProcessing(RecordTypes.QUST, ProcessQuests);
        AddDynamicProcessing(RecordTypes.DIAL, ProcessDialog);
        AddDynamicProcessing(RecordTypes.INFO, ProcessDialogResponses);
        AddDynamicProcessing(RecordTypes.REFR, ProcessPlacedObject);
        AddDynamicProcessing(RecordTypes.ACHR, ProcessPlacedNpc);
        AddDynamicProcessing(RecordTypes.CELL, ProcessCells);
        AddDynamicProcessing(RecordTypes.PGRE, ProcessTraps);
        AddDynamicProcessing(RecordTypes.PHZD, ProcessHazards);
        AddDynamicProcessing(RecordTypes.NPC_, ProcessNpcs);
        AddDynamicProcessing(RecordTypes.LCTN, ProcessLocations);
    }

    protected override IEnumerable<Task> ExtraJobs(Func<IMutagenReadStream> streamGetter)
    {
        foreach (var job in base.ExtraJobs(streamGetter))
        {
            yield return job;
        }
    }

    public override KeyValuePair<RecordType, FormKey>[] TrimmedRecords => new KeyValuePair<RecordType, FormKey>[]
    {
        new(RecordTypes.NAVM, FormKey.Factory("110AD3:Starfield.esm")),
        new(RecordTypes.NAVM, FormKey.Factory("14FC69:Starfield.esm")),
        new(RecordTypes.NAVM, FormKey.Factory("17FEC6:Starfield.esm")),
        new(RecordTypes.NAVM, FormKey.Factory("1BA29E:Starfield.esm")),
        new(RecordTypes.NAVM, FormKey.Factory("1BA29F:Starfield.esm")),
        new(RecordTypes.NAVM, FormKey.Factory("2F01CA:Starfield.esm")),
    };

    private void ProcessStaticCollections(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        foreach (var frame in majorFrame.FindEnumerateSubrecords(RecordTypes.DATA))
        {
            int offset = 0;
            ProcessZeroFloats(frame, fileOffset, ref offset);
        }
    }

    private void ProcessBendableSplines(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.DNAM, out var frame))
        {
            int offset = 8;
            ProcessColorFloat(frame, fileOffset, ref offset, alpha: false);
        }
    }

    private void ProcessObjectPlacementDefaults(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.OPDS, out var frame))
        {
            for (int i = 0; i < 20; i++)
            {
                int offset = 0;
                ProcessZeroFloat(frame, fileOffset, ref offset);
            }
        }
    }

    private void ProcessProjectedDecals(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        ProcessObjectPlacementDefaults(majorFrame, fileOffset);
    }

    private void ProcessMisc(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        ProcessObjectPlacementDefaults(majorFrame, fileOffset);
    }

    protected override Dictionary<(ModKey ModKey, StringsSource Source), HashSet<uint>>? KnownDeadStringKeys()
    {
        return new Dictionary<(ModKey ModKey, StringsSource Source), HashSet<uint>>
        {
        };
    }

    protected override AStringsAlignment[] GetStringsFileAlignments(StringsSource source)
    {
        switch (source)
        {
            case StringsSource.Normal:
                return new AStringsAlignment[]
                {
                    new StringsAlignmentCustom("GMST", GameSettingStringHandler),
                    new RecordType[] { "KYWD", "FULL" },
                    new RecordType[] { "DMGT", "FULL" },
                    new RecordType[] { "CLAS", "FULL" },
                    new RecordType[] { "FACT", "FULL" },
                    new RecordType[] { "HDPT", "FULL" },
                    new RecordType[] { "RACE", "FULL" },
                    new RecordType[] { "PNDT", "FULL" },
                    new RecordType[] { "BOOK", "FULL", "CNAM", "ENAM", "FNAM" },
                    new RecordType[] { "LVLI", "ONAM" },
                    new RecordType[] { "ENCH", "FULL" },
                    new RecordType[] { "SPEL", "FULL" },
                    new RecordType[] { "ACTI", "FULL", "ATTX" },
                    new RecordType[] { "FLST", "FULL" },
                    new RecordType[] { "TMLM", "FULL", "BTXT", "INAM", "ITXT", "ISTX", "UNAM" },
                    new RecordType[] { "WEAP", "FULL" },
                    new RecordType[] { "PERK", "FULL" },
                    new RecordType[] { "ARMO", "FULL" },
                    new RecordType[] { "CONT", "FULL" },
                    new RecordType[] { "OMOD", "FULL" },
                    new RecordType[] { "DIAL", "FULL" },
                    new RecordType[] { "INFO", "RNAM" },
                    new RecordType[] { "CELL", "FULL" },
                    new RecordType[] { "LCTN", "FULL" },
                    new RecordType[] { "NPC_", "FULL", "SHRT", "LNAM", "ATTX" },
                    new RecordType[] { "REFR", "FULL", "UNAM" },
                    new RecordType[] { "QUST", "FULL", "NNAM", "QMDP", "QMSU", "QMDT", "QMDS" },
                    new RecordType[] { "MGEF", "FULL", "DNAM" },
                    new RecordType[] { "ALCH", "FULL", "DNAM" },
                    new StringsAlignmentCustom("PERK", PerkStringHandler),
                    new RecordType[] { "MISC", "FULL", "NNAM" },
                    new RecordType[] { "IRES", "FULL", "NNAM" },
                    new RecordType[] { "LSCR", "DESC" },
                };
            case StringsSource.DL:
                return new AStringsAlignment[]
                {
                    new RecordType[] { "SPEL", "DESC" },
                    new RecordType[] { "COBJ", "DESC" },
                    new RecordType[] { "PERK", "DESC" },
                    new RecordType[] { "BOOK", "DESC" },
                    new RecordType[] { "ALCH", "DESC" },
                    new RecordType[] { "OMOD", "DESC" },
                    new RecordType[] { "QUST", "CNAM" },
                };
            case StringsSource.IL:
                return new AStringsAlignment[]
                {
                    new RecordType[] { "INFO", "NAM1" },
                };
            default:
                throw new NotImplementedException();
        }
    }

    public void GameSettingStringHandler(
        long loc,
        MajorRecordFrame major,
        List<StringEntry> processedStrings,
        IStringsLookup overlay)
    {
        if (!major.TryFindSubrecord("EDID", out var edidRec)) throw new ArgumentException();
        if (edidRec.Content[0] != (byte)'s') return;
        if (!major.TryFindSubrecord("DATA", out var dataRec)) throw new ArgumentException();
        AStringsAlignment.ProcessStringLink(loc, processedStrings, overlay, major, dataRec);
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

    private void ProcessTransforms(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (!majorFrame.TryFindSubrecord(RecordTypes.DATA, out var dataRec)) return;
        int offset = 0;
        ProcessZeroFloats(dataRec, fileOffset, ref offset, 9);
    }

    public void PerkStringHandler(
        long loc,
        MajorRecordFrame major,
        List<StringEntry> processedStrings,
        IStringsLookup overlay)
    {
        SubrecordPinFrame? lastepft = null;
        foreach (var sub in major.EnumerateSubrecords())
        {
            switch (sub.RecordTypeInt)
            {
                case RecordTypeInts.FULL:
                case RecordTypeInts.EPF2:
                    AStringsAlignment.ProcessStringLink(loc, processedStrings, overlay, major, sub);
                    break;
                case RecordTypeInts.EPFT:
                    lastepft = sub;
                    break;
                case RecordTypeInts.EPFD:
                    if (lastepft!.Value.Content[0] == (byte)APerkEntryPointEffect.ParameterType.LString)
                    {
                        AStringsAlignment.ProcessStringLink(loc, processedStrings, overlay, major, sub);
                    }

                    break;
                default:
                    break;
            }
        }
    }

    private void ProcessDialogResponses(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        foreach (var subRec in majorFrame.FindEnumerateSubrecords(RecordTypes.TRDA))
        {
            int loc = 0;
            ProcessMaxIsNegativeFormID(subRec, fileOffset, ref loc);
        }
    }

    private void ProcessPlacedObject(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.XTEL, out var xtel))
        {
            int loc = 4;
            ProcessZeroFloats(xtel, fileOffset, ref loc, 6);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XRGB, out var rgb))
        {
            int loc = 0;
            ProcessZeroFloats(rgb, fileOffset, ref loc, 3);
        }

        ProcessRagdollData(majorFrame, fileOffset);

        foreach (var subRec in majorFrame.FindEnumerateSubrecords(RecordTypes.OPDS))
        {
            int loc = 4;
            ProcessZeroFloats(subRec, fileOffset, ref loc);
        }

        foreach (var subRec in majorFrame.FindEnumerateSubrecords(RecordTypes.DATA))
        {
            int loc = 0;
            ProcessZeroFloats(subRec, fileOffset, ref loc);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XPRM, out var xprm))
        {
            int loc = 0;
            ProcessZeroFloats(xprm, fileOffset, ref loc, 3);
            ProcessColorFloat(xprm, fileOffset, ref loc, alpha: false);
            ProcessZeroFloats(xprm, fileOffset, ref loc, 1);
        }

        ProcessVolumesComponents(majorFrame, fileOffset);

        if (majorFrame.TryFindSubrecord(RecordTypes.XBSD, out var XBSD))
        {
            Instructions.SetSubstitution(fileOffset + XBSD.ContentLocation + 0x15, new byte[3]);
        }

        ZeroXOWNBool(stream, majorFrame, fileOffset);
        ProcessObjectPlacementDefaults(majorFrame, fileOffset);
        ProcessXTV2(majorFrame, fileOffset);
    }

    private void ProcessRagdollData(MajorRecordFrame majorFrame, long fileOffset)
    {
        foreach (var subRec in majorFrame.FindEnumerateSubrecords(RecordTypes.XRGD))
        {
            int loc = 0;
            while (loc < subRec.ContentLength)
            {
                loc += 4;
                ProcessZeroFloats(subRec, fileOffset, ref loc, 6);
            }
        }
    }

    private void ProcessVolumesComponents(MajorRecordFrame majorFrame, long fileOffset)
    {
        if (!majorFrame.TryFindSubrecord(RecordTypes.VLMS, out var VLMS)) return;

        int amount = BinaryPrimitives.ReadInt32LittleEndian(VLMS.Content);
        int loc = 4;

        for (int i = 0; i < amount; i++)
        {
            var type = BinaryPrimitives.ReadInt32LittleEndian(VLMS.Content.Slice(loc));
            loc += 4;
            ProcessZeroFloats(VLMS, fileOffset, ref loc, 19);
            switch (type)
            {
                case 1:
                    ProcessZeroFloats(VLMS, fileOffset, ref loc, 1);
                    break;
                case 3:
                    ProcessZeroFloats(VLMS, fileOffset, ref loc, 2);
                    break;
                case 5:
                    ProcessZeroFloats(VLMS, fileOffset, ref loc, 3);
                    break;
            }
        }
    }

    private void ProcessPlacedNpc(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        foreach (var subRec in majorFrame.FindEnumerateSubrecords(RecordTypes.DATA))
        {
            int loc = 0;
            ProcessZeroFloats(subRec, fileOffset, ref loc);
        }
        
        if (majorFrame.TryFindSubrecord(RecordTypes.XRGB, out var rgb))
        {
            int loc = 0;
            ProcessZeroFloats(rgb, fileOffset, ref loc, 3);
        }

        ProcessRagdollData(majorFrame, fileOffset);
        ZeroXOWNBool(stream, majorFrame, fileOffset);
    }

    private void ProcessCells(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        ZeroXOWNBool(stream, majorFrame, fileOffset);
        ProcessXTV2(majorFrame, fileOffset);
    }

    private void ZeroXOWNBool(IMutagenReadStream stream, MajorRecordFrame majorFrame, long fileOffset)
    {
        foreach (var subRec in majorFrame.FindEnumerateSubrecords(RecordTypes.XOWN))
        {
            Instructions.SetSubstitution(
                fileOffset + subRec.Location + stream.MetaData.Constants.SubConstants.HeaderLength + 9, new byte[3]);
        }
    }

    private void ProcessTraps(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        ProcessRagdollData(majorFrame, fileOffset);
        ProcessPositionRotationData(majorFrame, fileOffset);
    }

    private void ProcessHazards(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        ProcessPositionRotationData(majorFrame, fileOffset);
    }

    private HashSet<RecordType> _stopRecs = new HashSet<RecordType>()
    {
        RecordTypes.DATA,
        RecordTypes.CNAM,
    };
    
    private void ProcessNpcs(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (!majorFrame.TryFindSubrecord(RecordTypes.STOP, out var rec))
        {
            var subRec = majorFrame.FindEnumerateSubrecords(_stopRecs).First();
            var b = new byte[6];
            BinaryPrimitives.WriteInt32LittleEndian(b, RecordTypes.STOP.TypeInt);
            Instructions.SetAddition(fileOffset + subRec.Location, b);
            ProcessLengths(majorFrame, 6, fileOffset);
        }
    }
    
    private void ProcessLocations(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.LCEP, out var rec))
        {
            int loc = 0;
            while (loc < rec.ContentLength)
            {
                ProcessBool(rec, offsetLoc: fileOffset, loc + 8, 4, 1);
                loc += 12;
            }
        }
    }
    
    private void ProcessPositionRotationData(MajorRecordFrame majorFrame, long fileOffset)
    {
        foreach (var subRec in majorFrame.FindEnumerateSubrecords(RecordTypes.DATA))
        {
            int loc = 0;
            ProcessZeroFloats(subRec, fileOffset, ref loc, 6);
        }
    }

    private void ProcessXTV2(MajorRecordFrame majorFrame, long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.XTV2, out var xtv2))
        {
            if (TryGetXtv2TrimLocation(xtv2, out _, out var cutLen))
            {
                RemoveEndingBytes(xtv2, fileOffset, cutLen);
                var remainingLen = xtv2.ContentLength - cutLen;
                if (remainingLen < ushort.MaxValue && xtv2.LengthOverrideRecordLocation.HasValue)
                {
                    RemoveOverflowRecord(majorFrame, xtv2, fileOffset, checked((uint)remainingLen));
                    cutLen += 10;
                    ProcessLengths(majorFrame, -cutLen, fileOffset);
                }
                else
                {
                    ProcessLengths(majorFrame, xtv2, -cutLen, fileOffset);
                }
                return;
            }
            
            RemoveUnnecessaryOverflowRecord(majorFrame, xtv2, fileOffset);

            try
            {
                var span = xtv2.Content;
                int pos = 0;
                while (span.Length > pos)
                {
                    pos += 0x28;
                    var flag = BinaryPrimitives.ReadInt32LittleEndian(span.Slice(pos));
                    if (flag == 0)
                    {
                        pos += 0x10;
                    }
                    else if (flag == 4)
                    {
                        pos += 0xC;
                    }
                    else
                    {
                        if (EnumExt.HasFlag(flag, 4))
                        {
                            Instructions.SetSubstitution(fileOffset + xtv2.ContentLocation + pos, new byte[]
                            {
                                4, 0, 0, 0
                            });
                            pos += 0xC;
                        }
                        else
                        {
                            Instructions.SetSubstitution(fileOffset + xtv2.ContentLocation + pos, new byte[]
                            {
                                0, 0, 0, 0
                            });
                            pos += 0x10;
                        }
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }
    }

    private bool TryGetXtv2TrimLocation(SubrecordPinFrame xtv2, out int loc, out int cut)
    {
        loc = 0;
        while (xtv2.ContentLength > loc)
        {
            if (CheckIfXtv2IsFluff(xtv2.Content.Slice(loc), out var hasFormLink))
            {
                cut = xtv2.ContentLength - loc;
                return true;
            }

            loc += hasFormLink ? 0x38 : 0x34;
        }

        cut = 0;
        return false;
    }
    
    private bool CheckIfXtv2IsFluff(ReadOnlyMemorySlice<byte> mem, out bool hasFormLink)
    {
        try
        {
            var starter = mem.Slice(0, 0x28);
            var flags = BinaryPrimitives.ReadInt32LittleEndian(mem.Slice(0x28, 4));
            hasFormLink = !Enums.HasFlag(flags, 4);
            if (!hasFormLink)
            {
                return false;
            }

            var ender = mem.Slice(0x2C, 12);
            if (starter.Any(b => b != 0)) return false;
            if (ender.Any(b => b != 0)) return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            hasFormLink = false;
            return true;
        }
        
        return true;
    }

    private void ProcessDialog(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        var formKey = FormKey.Factory(stream.MetaData.MasterReferences!, majorFrame.FormID.Raw);
        CleanEmptyDialogGroups(
            stream,
            formKey,
            fileOffset);

        uint actualCount = 0;
        List<FormID> infos = new();
        stream.Position = fileOffset + majorFrame.TotalLength;
        if (stream.TryReadGroup(out var groupFrame)
            && groupFrame.GroupType == 7)
        {
            int groupPos = 0;
            while (groupPos < groupFrame.Content.Length)
            {
                var majorMeta = stream.MetaData.Constants.MajorRecordHeader(groupFrame.Content.Slice(groupPos));
                actualCount++;
                groupPos += checked((int)majorMeta.TotalLength);
                if (majorMeta.RecordType == RecordTypes.INFO)
                {
                    infos.Add(majorMeta.FormID);
                }
            }
        }

        // Reset misnumbered counter
        if (majorFrame.TryFindSubrecord(RecordTypes.TIFC, out var tifcRec))
        {
            var count = tifcRec.AsUInt32();

            if (actualCount != count)
            {
                byte[] b = new byte[4];
                BinaryPrimitives.WriteUInt32LittleEndian(b, actualCount);
                Instructions.SetSubstitution(
                    fileOffset + tifcRec.Location + stream.MetaData.Constants.SubConstants.HeaderLength,
                    b);
            }
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.TIFL, out var rec))
        {
            byte[] b = new byte[infos.Count * 4];
            var slice = b.AsSpan();
            foreach (var tifl in infos)
            {
                BinaryPrimitives.WriteUInt32LittleEndian(slice, tifl.Raw);
                slice = slice.Slice(4);
            }

            SwapSubrecordContent(fileOffset, majorFrame, rec, b);
        }
    }

    private void ProcessQuests(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        var formKey = FormKey.Factory(stream.MetaData.MasterReferences!, majorFrame.FormID.Raw);

        if (majorFrame.TryFindSubrecord(RecordTypes.ANAM, out var anamRec))
        {
            var next = anamRec.AsUInt32();
            var targets = new RecordType[]
            {
                RecordTypes.ALST,
                RecordTypes.ALLS,
                RecordTypes.ALCS,
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
                Instructions.SetSubstitution(
                    fileOffset + anamRec.Location + anamRec.HeaderLength,
                    sub);
            }
        }

        CleanEmptyQuestGroups(
            stream,
            formKey,
            fileOffset);

        FixVMADs(majorFrame, fileOffset);
    }

    public void FixVMADs(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        FixVMADFormIDs(
            majorFrame,
            fileOffset,
            out var vmad,
            out var objectFormat,
            out var processedLen);
        if (vmad != null)
        {
            fileOffset += majorFrame.Header.HeaderLength + vmad.Value.Location + vmad.Value.Header.HeaderLength;
            var stream2 = new MutagenMemoryReadStream(vmad.Value.Content, Bundle)
            {
                Position = processedLen - vmad.Value.HeaderLength
            };
            if (stream2.Complete) return;
            // skip unknown
            stream2.Position += 1;
            var fragCount = stream2.ReadUInt16();
            FixVMADScriptIDs(stream2, fileOffset, objectFormat);
            for (int i = 0; i < fragCount; i++)
            {
                stream2.Position += 9;
                // skip name
                var len = stream2.ReadUInt16();
                stream2.Position += len;
                // skip name
                len = stream2.ReadUInt16();
                stream2.Position += len;
            }

            var aliasCount = stream2.ReadUInt16();
            for (int i = 0; i < aliasCount; i++)
            {
                FixObjectPropertyIDs(stream2, fileOffset, objectFormat);
                // skip version
                stream2.Position += 2;
                objectFormat = stream2.ReadUInt16();
                var numScripts = stream2.ReadUInt16();
                for (int j = 0; j < numScripts; j++)
                {
                    FixVMADScriptIDs(stream2, fileOffset, objectFormat);
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
        if (len == 0) return;
        // Skip flags
        stream.Position += 1;
        FixScriptEntry(stream, fileOffset, objectFormat, isStruct: false);
    }

    private void FixScriptEntry(IMutagenReadStream stream, long fileOffset, ushort objectFormat, bool isStruct)
    {
        var propCount = isStruct ? stream.ReadUInt32() : stream.ReadUInt16();
        for (int propIndex = 0; propIndex < propCount; propIndex++)
        {
            // skip name
            var len = stream.ReadUInt16();
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
                ScriptProperty.Type.ArrayOfStruct => new ScriptStructListProperty(),
                ScriptProperty.Type.Struct => new ScriptStructProperty(),
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
                case ScriptStructProperty structProp:
                    FixScriptEntry(stream, fileOffset, objectFormat, isStruct: true);
                    break;
                case ScriptStructListProperty structList:
                    var structListCount = stream.ReadUInt32();
                    for (int j = 0; j < structListCount; j++)
                    {
                        FixScriptEntry(stream, fileOffset, objectFormat, isStruct: true);
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
}