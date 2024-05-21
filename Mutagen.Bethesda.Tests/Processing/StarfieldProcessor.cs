using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Starfield.Internals;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;
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
    public override bool StrictStrings => true;

    public StarfieldProcessor(bool multithread) : base(multithread)
    {
    }

    public override GameRelease GameRelease => GameRelease.Starfield;

    protected override void AddDynamicProcessorInstructions()
    {
        base.AddDynamicProcessorInstructions();
        AddDynamicProcessing(RecordType.Null, ProcessAll);
        AddDynamicProcessing(RecordTypes.GMST, ProcessGameSettings);
        AddDynamicProcessing(RecordTypes.TRNS, ProcessTransforms);
        AddDynamicProcessing(RecordTypes.SCOL, ProcessStaticCollections);
        AddDynamicProcessing(RecordTypes.BNDS, ProcessBendableSplines);
        AddDynamicProcessing(RecordTypes.QUST, ProcessQuests);
        AddDynamicProcessing(RecordTypes.DIAL, ProcessDialog);
        AddDynamicProcessing(RecordTypes.REFR, ProcessPlacedObject);
        AddDynamicProcessing(RecordTypes.ACHR, ProcessPlacedNpc);
        AddDynamicProcessing(RecordTypes.CELL, ProcessCells);
        AddDynamicProcessing(RecordTypes.PGRE, ProcessTraps);
        AddDynamicProcessing(RecordTypes.PHZD, ProcessHazards);
        AddDynamicProcessing(RecordTypes.NPC_, ProcessNpcs);
        AddDynamicProcessing(RecordTypes.LCTN, ProcessLocations);
        AddDynamicProcessing(RecordTypes.WRLD, ProcessWorldspaces);
        AddDynamicProcessing(RecordTypes.PACK, ProcessPackages);
        AddDynamicProcessing(RecordTypes.STMP, ProcessSnapTemplates);
        AddDynamicProcessing(RecordTypes.FURN, ProcessFurniture);
        AddDynamicProcessing(RecordTypes.MATT, ProcessMaterialTypes);
        AddDynamicProcessing(RecordTypes.ZOOM, ProcessZooms);
        AddDynamicProcessing(RecordTypes.LENS, ProcessLenses);
        AddDynamicProcessing(RecordTypes.SFPT, ProcessSurfacePatterns);
        AddDynamicProcessing(RecordTypes.SFTR, ProcessSurfaceTree);
        AddDynamicProcessing(RecordTypes.STAT, ProcessStatics);
        AddDynamicProcessing(RecordTypes.LVLI, ProcessLeveledItems);
        AddDynamicProcessing(RecordTypes.OMOD, ProcessOMOD);
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
        new(RecordTypes.GBFM, FormKey.Factory("2B3DDB:Starfield.esm")),
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
        foreach (var frame in majorFrame.FindEnumerateSubrecords(RecordTypes.OPDS))
        {
            int offset = 0;
            for (int i = 0; i < 20; i++)
            {
                ProcessZeroFloat(frame, fileOffset, ref offset);
            }
        }
    }

    protected override Dictionary<(ModKey ModKey, StringsSource Source), HashSet<uint>>? KnownDeadStringKeys()
    {
        return new Dictionary<(ModKey ModKey, StringsSource Source), HashSet<uint>>
        {
            { (Constants.Starfield, StringsSource.Normal), new() { 0x71B7 } }
        };
    }

    protected override AStringsAlignment[] GetStringsFileAlignments(StringsSource source)
    {
        switch (source)
        {
            case StringsSource.Normal:
                return new AStringsAlignment[]
                {
                    new StringsAlignmentCustom(null, ComponentsStringHandler),
                    new StringsAlignmentCustom("GMST", GameSettingStringHandler),
                    new RecordType[] { "KYWD", "FULL" },
                    new RecordType[] { "DMGT", "FULL" },
                    new RecordType[] { "CLAS", "FULL" },
                    new RecordType[] { "FACT", "FULL" },
                    new RecordType[] { "HDPT", "FULL" },
                    new RecordType[] { "RACE", "FULL", "FMRN", "FDSL", "SNAM" },
                    new RecordType[] { "PNDT", "FULL" },
                    new RecordType[] { "BOOK", "FULL", "CNAM", "ENAM", "FNAM" },
                    new RecordType[] { "LVLI", "ONAM" },
                    new RecordType[] { "LVLN", "ONAM" },
                    new RecordType[] { "ENCH", "FULL" },
                    new RecordType[] { "SPEL", "FULL" },
                    new RecordType[] { "ACTI", "FULL", "ATTX" },
                    new RecordType[] { "FLST", "FULL" },
                    new RecordType[] { "TMLM", "FULL", "BTXT", "INAM", "ITXT", "ISTX", "UNAM" },
                    new RecordType[] { "WEAP", "FULL", "WABB" },
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
                    new RecordType[] { "WRLD", "FULL" },
                    new RecordType[] { "WATR", "FULL" },
                    new RecordType[] { "INNR", "WNAM" },
                    new RecordType[] { "AMMO", "FULL", "ONAM" },
                    new RecordType[] { "FURN", "FULL", "ATTX" },
                    new RecordType[] { "STAT", "FULL" },
                    new RecordType[] { "PKIN", "FULL" },
                    new RecordType[] { "MSTT", "FULL" },
                    new RecordType[] { "FLOR", "FULL", "ATTX" },
                    new RecordType[] { "KEYM", "FULL" },
                    new RecordType[] { "PROJ", "FULL" },
                    new RecordType[] { "HAZD", "FULL" },
                    new RecordType[] { "TERM", "FULL" },
                    new RecordType[] { "IDLE", "FULL" },
                    new RecordType[] { "EXPL", "FULL" },
                    new RecordType[] { "BPTD", "BPTN" },
                    new RecordType[] { "AVIF", "FULL", "ANAM" },
                    new RecordType[] { "MESG", "FULL", "NNAM", "ITXT" },
                    new RecordType[] { "BIOM", "FULL" },
                    new RecordType[] { "RSPJ", "FULL" },
                    new RecordType[] { "PMFT", "FULL" },
                    new RecordType[] { "CHAL", "FULL" },
                    new RecordType[] { "DOOR", "FULL", "ONAM", "CNAM" },
                    new RecordType[] { "FXPD", "FULL" },
                    new RecordType[] { "GBFM", "FULL" },
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
                    new RecordType[] { "AMMO", "DESC" },
                    new RecordType[] { "FURN", "DESC" },
                    new RecordType[] { "MESG", "DESC" },
                    new RecordType[] { "RSPJ", "DESC" },
                    new RecordType[] { "CHAL", "DESC" },
                    new RecordType[] { "RACE", "DESC" },
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

    public void ComponentsStringHandler(
        long loc,
        MajorRecordFrame major,
        List<StringEntry> processedStrings,
        IStringsLookup overlay)
    {
        foreach (var bfcb in major.FindEnumerateSubrecords(RecordTypes.BFCB))
        {
            var componentStr = bfcb.AsString(major.Meta.Encodings.NonTranslated);
            switch (componentStr)
            {
                case "TESFullName_Component":
                {
                    var full = major.TryFindSubrecordAfter(bfcb, RecordTypes.FULL);
                    var bfce2 = major.TryFindSubrecordAfter(bfcb, RecordTypes.BFCE);
                    if (full != null && bfce2 != null && full.Value.Location < bfce2.Value.Location)
                    {
                        AStringsAlignment.ProcessStringLink(loc, processedStrings, overlay, major, full.Value);
                    }

                    break;
                }
                case "BGSSpaceshipHullCode_Component":
                {
                    var hull = major.TryFindSubrecordAfter(bfcb, RecordTypes.HULL);
                    var bfce2 = major.TryFindSubrecordAfter(bfcb, RecordTypes.BFCE);
                    if (hull != null && bfce2 != null && hull.Value.Location < bfce2.Value.Location)
                    {
                        AStringsAlignment.ProcessStringLink(loc, processedStrings, overlay, major, hull.Value);
                    }

                    break;
                }
            }
        }
    }

    private void ProcessAll(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        ProcessComponents(majorFrame, fileOffset);
        ProcessObjectPlacementDefaults(majorFrame, fileOffset);
        ProcessFEIndices(majorFrame, fileOffset);
    }

    private void ProcessFEIndices(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        ProcessMajorRecordFormIDOverflow(majorFrame, fileOffset);

        foreach (var subRec in majorFrame.EnumerateSubrecords())
        {
            var loc = 0;
            if (subRec.ContentLength != 4
                || subRec.Content[2] != 0
                || subRec.Content[3] != 0xFE)
            {
                continue;
            }
            ProcessFormIDOverflow(subRec, fileOffset);
        }
    }

    private void ProcessComponents(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        foreach (var bfcb in majorFrame.FindEnumerateSubrecords(RecordTypes.BFCB))
        {
            switch (bfcb.AsString(majorFrame.Meta.Encodings.NonTranslated))
            {
                case "Blueprint_Component":
                    ProcessBlueprintComponent(majorFrame, bfcb, fileOffset);
                    break;
                case "SurfaceTreePatternSwapInfo_Component":
                    SurfaceTreePatternSwapInfoComponent(majorFrame, bfcb, fileOffset);
                    break;
                case "BGSOrbitedDataComponent_Component":
                    OrbitedDataComponent(majorFrame, bfcb, fileOffset);
                    break;
                case "BGSStarDataComponent_Component":
                    StarDataComponent(majorFrame, bfcb, fileOffset);
                    break;
            }
        }
    }

    private void ProcessBlueprintComponent(
        MajorRecordFrame majorFrame,
        SubrecordPinFrame bfcb,
        long fileOffset)
    {
        var buo4 = majorFrame.TryFindSubrecordAfter(bfcb, RecordTypes.BUO4);
        var bfce = majorFrame.TryFindSubrecordAfter(bfcb, RecordTypes.BFCE);
        if (bfce == null)
        {
            throw new ArgumentException();
        }
        if (buo4 != null)
        {
            int loc = 0;
            while (loc < buo4.Value.ContentLength)
            {
                loc += 8;
                ProcessZeroFloats(buo4.Value, fileOffset, ref loc, 6);
                loc += 4;
            }
        }

        foreach (var bodv in majorFrame.FindEnumerateSubrecords(RecordTypes.BODV))
        {
            if (bodv.Location > bfce.Value.Location)
            {
                break;
            }

            int loc = 0;
            ProcessColorFloats(bodv, fileOffset, ref loc, alpha: false, amount: 3);
        }
    }

    private void SurfaceTreePatternSwapInfoComponent(
        MajorRecordFrame majorFrame,
        SubrecordPinFrame bfcb,
        long fileOffset)
    {
        var data = majorFrame.TryFindSubrecordAfter(bfcb, RecordTypes.DATA);
        if (data != null)
        {
            var count = BinaryPrimitives.ReadInt32LittleEndian(data.Value.Content);
            int refLoc = 4;
            for (int i = 0; i < count; i++)
            {
                ProcessFormIDOverflows(data.Value, fileOffset, ref refLoc, 1);
                refLoc += 4;
            }
        }
    }

    private const float MassDiv = 1.98847E+30f;

    private void OrbitedDataComponent(
        MajorRecordFrame majorFrame,
        SubrecordPinFrame bfcb,
        long fileOffset)
    {
        var data = majorFrame.TryFindSubrecordAfter(bfcb, RecordTypes.DATA);
        if (data != null)
        {
            var slice = data.Value.Content.Slice(8, 4);
            var val = BinaryPrimitives.ReadSingleLittleEndian(slice);
            var d = val / MassDiv;
            var val2 = d * MassDiv;
            var b = new byte[4];
            BinaryPrimitives.WriteSingleLittleEndian(b, val2);
            if (!b.SequenceEqual(slice))
            {
                this.Instructions.SetSubstitution(fileOffset + data.Value.Location + data.Value.HeaderLength + 8, b);
            }
        }
    }
    
    private void StarDataComponent(
        MajorRecordFrame majorFrame,
        SubrecordPinFrame bfcb,
        long fileOffset)
    {
        var data = majorFrame.TryFindSubrecordAfter(bfcb, RecordTypes.DATA);
        if (data != null)
        {
            int index = 0;
            var strLen = BinaryPrimitives.ReadInt32LittleEndian(data.Value.Content.Slice(index));
            index += strLen + 4;
            strLen = BinaryPrimitives.ReadInt32LittleEndian(data.Value.Content.Slice(index));
            index += strLen + 4 + 4;
            var val = BinaryPrimitives.ReadSingleLittleEndian(data.Value.Content.Slice(index));
            var d = val / MassDiv;
            var val2 = d * MassDiv;
            if (val != val2)
            {
                var b = new byte[4];
                BinaryPrimitives.WriteSingleLittleEndian(b, val2);
                this.Instructions.SetSubstitution(fileOffset + data.Value.Location + data.Value.HeaderLength + index, b);
            }
        }
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
        // ProcessXTV2(majorFrame, fileOffset);
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
        ZeroXOWNBool(stream, majorFrame, fileOffset);
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
        if (majorFrame.TryFindSubrecord(RecordTypes.LCEP, out var lcep))
        {
            int loc = 0;
            while (loc < lcep.ContentLength)
            {
                ProcessBool(lcep, offsetLoc: fileOffset, loc + 8, 4, 1);
                loc += 12;
            }
        }
        if (majorFrame.TryFindSubrecord(RecordTypes.ACEP, out var acep))
        {
            int loc = 0;
            while (loc < acep.ContentLength)
            {
                ProcessBool(acep, offsetLoc: fileOffset, loc + 8, 4, 1);
                loc += 12;
            }
        }
    }
    
    private void ProcessWorldspaces(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        foreach (var subRec in majorFrame.FindEnumerateSubrecords(RecordTypes.XCLW))
        {
            ProcessZeroFloats(subRec, fileOffset);
        }
    }

    private void ProcessSnapTemplates(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        foreach (var enam in majorFrame.FindEnumerateSubrecords(RecordTypes.ENAM))
        {
            int loc = 8;
            ProcessZeroFloats(enam, fileOffset, ref loc, 6);
        }
    }

    private void ProcessPackages(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
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
        RecordType citc = new RecordType("CITC");
        RecordType ctda = new RecordType("CTDA");
        RecordType stsc = new RecordType("STSC");
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
                    tpic,
                    citc,
                    ctda,
                    stsc);
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
                
                if (recs[7] != null && finalLoc == recs[7].Value.Location)
                {
                    RecordSpanExtensions.ParseRepeatingSubrecord(
                        majorFrame.Content.Slice(anamPos.Value + anamRecord.TotalLength),
                        majorFrame.Meta,
                        RecordTypes.CTDA,
                        finalLoc + recs[7].Value.TotalLength,
                        out var lenParsed);
                    finalLoc += lenParsed;
                }

                var finalRec =
                    majorFrame.Meta.SubrecordHeader(
                        majorFrame.Content.Slice(anamPos.Value + anamRecord.TotalLength + finalLoc));
                var dataSlice = majorFrame.Content.Slice(anamPos.Value,
                    anamRecord.TotalLength + finalLoc + finalRec.TotalLength);
                if (BinaryStringUtility.ProcessWholeToZString(anamRecord.Content, MutagenEncoding._1252) ==
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
                Instructions.SetSubstitution(
                    fileOffset + majorFrame.HeaderLength + subLoc,
                    item.Data.ToArray());
                subLoc += item.Data.Length;
            }

            foreach (var item in dataValues.OrderBy(i => i.Index))
            {
                byte[] bytes = new byte[] { 0x55, 0x4E, 0x41, 0x4D, 0x01, 0x00, 0x00 };
                bytes[6] = (byte)item.Index;
                Instructions.SetSubstitution(
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
            Instructions.SetSubstitution(
                writeLoc,
                item.Data.ToArray());
            writeLoc += item.Data.Length;
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

    private void ProcessFurniture(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.SNAM, out var frame))
        {
            int offset = 0;
            int i = 0;
            while (i * 0x1C < frame.ContentLength)
            {
                ProcessZeroFloats(frame, fileOffset, ref offset, 3);
                ProcessRotationFloat(frame, fileOffset, ref offset, 57.2958f);
                i++;
                offset = i * 0x1C;
            }
        }
    }
    
    private void ProcessMaterialTypes(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.CNAM, out var cnam))
        {
            ProcessColorFloat(cnam, fileOffset, alpha: false);
        }
    }
    
    private void ProcessZooms(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.ZNAM, out var znam))
        {
            int pos = 4;
            ProcessZeroFloats(znam, fileOffset, ref pos, 4);
            pos += 1;
            ProcessZeroFloat(znam, fileOffset, ref pos);
            pos += 1;
            ProcessZeroFloat(znam, fileOffset, ref pos);
            pos += 1;
            ProcessZeroFloat(znam, fileOffset, ref pos);
        }
    }

    private void ProcessLenses(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        foreach (var subRec in majorFrame.FindEnumerateSubrecords(RecordTypes.LFSD))
        {
            int loc = 0;
            ProcessColorFloat(subRec, fileOffset, ref loc, alpha: false);
            ProcessZeroFloats(subRec, fileOffset, ref loc, 5);
        }
    }

    private void ProcessSurfacePatterns(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        foreach (var bnam in majorFrame.FindEnumerateSubrecords(RecordTypes.BNAM))
        {
            ProcessFormIDOverflows(bnam, fileOffset);
        }
    }

    private void ProcessSurfaceTree(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        foreach (var bnam in majorFrame.FindEnumerateSubrecords(RecordTypes.ENAM))
        {
            ProcessFormIDOverflows(bnam, fileOffset);
        }
    }

    private void ProcessStatics(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        var molms = majorFrame.FindEnumerateSubrecords(RecordTypes.MOLM).ToArray();
        if (molms.Length > 1)
        {
            var range = new RangeInt64(molms[0].Location + fileOffset, molms[^2].Location + fileOffset + molms[^2].TotalLength - 1);
            Instructions.SetRemove(range);

            ProcessLengths(
                majorFrame,
                -range.Width,
                fileOffset);
        }
    }

    private void ProcessLeveledItems(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.LLKC, out var llkc))
        {
            ProcessFormIDOverflow(llkc, fileOffset);
        }
    }

    private readonly string[] OMODRecords = new[]
    {
        "WKEY",
        "AKEY"
    };

    private void ProcessOMOD(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.DATA, out var data))
        {
            var str = BinaryStringUtility.ToZString(data.Content, MutagenEncoding._1252);
            foreach (var target in OMODRecords)
            {
                var strLoc = 0;
                while (true)
                {
                    var index = str.Substring(strLoc).IndexOf(target);
                    if (index == -1) break;
                    strLoc += index + 4;
                    ProcessFormIDOverflow(data, fileOffset, ref strLoc);
                }
            }
        }
    }
}