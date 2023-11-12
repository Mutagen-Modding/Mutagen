using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Starfield.Internals;
using Mutagen.Bethesda.Strings;
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
    }

    protected override IEnumerable<Task> ExtraJobs(Func<IMutagenReadStream> streamGetter)
    {
        foreach (var job in base.ExtraJobs(streamGetter))
        {
            yield return job;
        }
    }

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
                    new RecordType[] { "LVLI", "ONAM" },
                    new RecordType[] { "ENCH", "FULL" },
                    new RecordType[] { "SPEL", "FULL" },
                    new RecordType[] { "ACTI", "FULL", "ATTX" },
                    new RecordType[] { "FLST", "FULL" },
                    new RecordType[] { "TMLM", "FULL", "BTXT", "INAM", "ITXT", "ISTX", "UNAM"  },
                    new RecordType[] { "WEAP", "FULL" },
                    new RecordType[] { "PERK", "FULL" },
                    new RecordType[] { "ARMO", "FULL" },
                    new StringsAlignmentCustom("PERK", PerkStringHandler),
                    new RecordType[] { "MISC", "FULL", "NNAM" },
                    new RecordType[] { "IRES", "FULL", "NNAM" },
                };
            case StringsSource.DL:
                return new AStringsAlignment[]
                {
                    new RecordType[] { "SPEL", "DESC" },
                    new RecordType[] { "COBJ", "DESC" },
                    new RecordType[] { "PERK", "DESC" },
                };
            case StringsSource.IL:
                return new AStringsAlignment[]
                {
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
                _instructions.SetSubstitution(
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