using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield.Internals;
using Noggog;

namespace Mutagen.Bethesda.Starfield;

public partial class Quest
{
    [Flags]
    public enum MajorFlag
    {
        PartialForm = 0x04000
    }

    [Flags]
    public enum Flag
    {
        StartGameEnabled = 0x0001,
        Completed = 0x0002,
        AddIdleTopicToHello = 0x0004,
        AllowRepeatedStages = 0x0008,
        StartsEnabled = 0x0010,
        DisplaysInHud = 0x0020,
        Failed = 0x0040,
        StageWait = 0x0080,
        RunOnce = 0x0100,
        ExcludeFromDialogExport = 0x0200,
        WarnOnAliasFillFailure = 0x0400,
        Active = 0x0800,
        RepeatsConditions = 0x1000,
        KeepInstance = 0x2000,
        WantDormant = 0x4000,
        HasDialogueData = 0x8000,
    }

    [Flags]
    public enum TargetFlag
    {
        CompassMarkerIgnoresLocks = 0x1,
        Hostile = 0x2,
        UseStraightLinePathing = 0x4
    }


    public enum TypeEnum
    {
        None = 0,
        MainQuest = 1,
        BrotherhoodOfSteel = 2,
        Institute = 3,
        Minutemen = 4,
        Railroad = 5,
        Misc = 6,
        SideQuests = 7,
        DLC01 = 8,
        DLC02 = 9,
        DLC03 = 10,
        DLC04 = 11,
        DLC05 = 12,
        DLC06 = 13,
        DLC07 = 14,
    }
}

partial class QuestBinaryCreateTranslation
{
    public static partial void FillBinaryDialogConditionsCustom(MutagenFrame frame, IQuestInternal item,
        PreviousParse lastParsed)
    {
        ConditionBinaryCreateTranslation.FillConditionsList(item.DialogConditions, frame);
    }

    public static partial ParseResult FillBinaryUnusedConditionsLogicCustom(MutagenFrame frame, IQuestInternal item,
        PreviousParse lastParsed)
    {
        var nextHeader = frame.ReadSubrecord(RecordTypes.NEXT);
        if (nextHeader.Content.Length != 0)
        {
            throw new ArgumentException("Unexpected NEXT header");
        }

        item.UnusedConditions = new();
        ConditionBinaryCreateTranslation.FillConditionsList(item.UnusedConditions, frame);
        return null;
    }

    private static readonly HashSet<RecordType> _expectedAliasRecords = new()
    {
        RecordTypes.ALST,
        RecordTypes.ALLS,
        RecordTypes.ALCS,
        RecordTypes.ALMI,
    };

    public static partial ParseResult FillBinaryAliasParseCustom(MutagenFrame frame, IQuestInternal item,
        PreviousParse lastParsed)
    {
        frame = frame.SpawnAll();
        frame.TryReadSubrecord(RecordTypes.ANAM, out _);
        item.Aliases = new();
        while (frame.TryReadSubrecord(_expectedAliasRecords, out var subRec))
        {
            switch (subRec.RecordTypeInt)
            {
                case RecordTypeInts.ALST:
                {
                    var id = subRec.AsUInt32();
                    var ret = QuestReferenceAlias.CreateFromBinary(frame);
                    ret.ID = id;
                    item.Aliases.Add(ret);
                    break;
                }
                case RecordTypeInts.ALLS:
                {
                    var id = subRec.AsUInt32();
                    var ret = QuestLocationAlias.CreateFromBinary(frame);
                    ret.ID = id;
                    item.Aliases.Add(ret);
                    break;
                }
                case RecordTypeInts.ALCS:
                case RecordTypeInts.ALMI:
                {
                    frame.Position -= subRec.TotalLength;
                    var ret = new QuestCollectionAlias();
                    ret.Collection.SetTo(
                        ListBinaryTranslation<CollectionAlias>.Instance.Parse(
                            frame,
                            CollectionAlias.TryCreateFromBinary));
                    item.Aliases.Add(ret);
                    break;
                }
                default:
                    frame.Position -= subRec.TotalLength;
                    return (int)Quest_FieldIndex.Aliases;
            }
        }

        return (int)Quest_FieldIndex.Aliases;
    }
}

partial class QuestBinaryWriteTranslation
{
    public static partial void WriteBinaryDialogConditionsCustom(MutagenWriter writer, IQuestGetter item)
    {
        ConditionBinaryWriteTranslation.WriteConditionsList(item.DialogConditions, writer);
    }

    public static partial void WriteBinaryUnusedConditionsLogicCustom(MutagenWriter writer, IQuestGetter item)
    {
        var unusedConditions = item.UnusedConditions;
        if (unusedConditions == null) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.NEXT))
        {
        }

        ConditionBinaryWriteTranslation.WriteConditionsList(unusedConditions, writer);
    }

    public static partial void WriteBinaryAliasParseCustom(MutagenWriter writer, IQuestGetter item)
    {
        var aliases = item.Aliases;
        if (aliases == null) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.ANAM))
        {
            if (aliases.Count == 0)
            {
                writer.Write(0);
            }
            else
            {
                var max = aliases.Select(x =>
                {
                    switch (x)
                    {
                        case IQuestReferenceAliasGetter refAlias:
                            return refAlias.ID;
                        case IQuestLocationAliasGetter locAlias:
                            return locAlias.ID;
                        case IQuestCollectionAliasGetter collAlias:
                            return default(UInt32);
                        default:
                            throw new NotImplementedException();
                    }
                }).Max();
                writer.Write(max + 1);
            }
        }

        foreach (var alias in aliases)
        {
            switch (alias)
            {
                case IQuestReferenceAliasGetter refAlias:
                    using (HeaderExport.Subrecord(writer, RecordTypes.ALST))
                    {
                        writer.Write(refAlias.ID);
                    }

                    refAlias.WriteToBinary(writer);
                    break;
                case IQuestLocationAliasGetter locAlias:
                    using (HeaderExport.Subrecord(writer, RecordTypes.ALLS))
                    {
                        writer.Write(locAlias.ID);
                    }

                    locAlias.WriteToBinary(writer);
                    break;
                case IQuestCollectionAliasGetter collAlias:
                    collAlias.WriteToBinary(writer);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}

partial class QuestBinaryOverlay
{
    public IReadOnlyList<IConditionGetter> DialogConditions { get; private set; } = Array.Empty<IConditionGetter>();
    public IReadOnlyList<IConditionGetter>? UnusedConditions { get; private set; }
    public IReadOnlyList<IAQuestAliasGetter>? Aliases { get; private set; }

    private ReadOnlyMemorySlice<byte>? _grupData;

    public int Timestamp => _grupData != null
        ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.GroupHeader(_grupData.Value)
            .LastModifiedData)
        : 0;

    public int Unknown =>
        _grupData.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_grupData.Value.Slice(20)) : default;

    public IReadOnlyList<ISceneGetter> Scenes { get; private set; } = Array.Empty<ISceneGetter>();

    public IReadOnlyList<IDialogTopicGetter> DialogTopics { get; private set; } = Array.Empty<IDialogTopicGetter>();

    public IReadOnlyList<IDialogBranchGetter> DialogBranches { get; private set; } =
        Array.Empty<IDialogBranchGetter>();

    partial void DialogConditionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type,
        PreviousParse lastParsed)
    {
        DialogConditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
    }

    public partial ParseResult UnusedConditionsLogicCustomParse(OverlayStream stream, int offset,
        PreviousParse lastParsed)
    {
        var nextHeader = stream.ReadSubrecord(RecordTypes.NEXT);
        if (nextHeader.Content.Length != 0)
        {
            throw new ArgumentException("Unexpected NEXT header");
        }

        UnusedConditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);

        return null;
    }

    public static RecordTriggerSpecs QuestAliasTriggerSpecs => _aliasTriggerSpecs.Value;

    private static readonly Lazy<RecordTriggerSpecs> _aliasTriggerSpecs = new Lazy<RecordTriggerSpecs>(() =>
    {
        return new RecordTriggerSpecs(
            RecordCollection.Factory(
                RecordTypes.ALST,
                RecordTypes.ALLS,
                RecordTypes.ALCS,
                RecordTypes.ALED,
                RecordTypes.ALID,
                RecordTypes.FNAM,
                RecordTypes.ALFI,
                RecordTypes.ALFL,
                RecordTypes.ALFR,
                RecordTypes.ALUA,
                RecordTypes.ALFA,
                RecordTypes.KNAM,
                RecordTypes.ALRT,
                RecordTypes.ALEQ,
                RecordTypes.ALEA,
                RecordTypes.ALCO,
                RecordTypes.ALCA,
                RecordTypes.ALCL,
                RecordTypes.ALNA,
                RecordTypes.ALNT,
                RecordTypes.ALFE,
                RecordTypes.ALFD,
                RecordTypes.ALCC,
                RecordTypes.CTDA,
                RecordTypes.CIS1,
                RecordTypes.CIS2,
                RecordTypes.KWDA,
                RecordTypes.KSIZ,
                RecordTypes.CNTO,
                RecordTypes.COCT,
                RecordTypes.COED,
                RecordTypes.SPOR,
                RecordTypes.OCOR,
                RecordTypes.GWOR,
                RecordTypes.ECOR,
                RecordTypes.ALLA,
                RecordTypes.ALDN,
                RecordTypes.ALFV,
                RecordTypes.ALDI,
                RecordTypes.ALSP,
                RecordTypes.ALFC,
                RecordTypes.ALPC,
                RecordTypes.VTCK,
                RecordTypes.ALMI),
            RecordCollection.Factory(
                RecordTypes.ALST,
                RecordTypes.ALLS,
                RecordTypes.ALCS));
    });

    public partial ParseResult AliasParseCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        stream.TryReadSubrecord(RecordTypes.ANAM, out _);
        var mem = stream.RemainingMemory;
        var locs = ParseRecordLocations(
            stream,
            QuestAliasTriggerSpecs,
            _package.MetaData.Constants.SubConstants,
            skipHeader: false);
        Aliases = BinaryOverlayList.FactoryByArray<IAQuestAliasGetter>(
            mem,
            _package,
            locs: locs,
            getter: (s, p) =>
            {
                var subRec = p.MetaData.Constants.Subrecord(s);
                switch (subRec.RecordTypeInt)
                {
                    case RecordTypeInts.ALST:
                    {
                        var id = subRec.AsUInt32();
                        var ret = (QuestReferenceAliasBinaryOverlay)QuestReferenceAliasBinaryOverlay
                            .QuestReferenceAliasFactory(s.Slice(subRec.TotalLength), p);
                        ret.ID = id;
                        return ret;
                    }
                    case RecordTypeInts.ALLS:
                    {
                        var id = subRec.AsUInt32();
                        var ret = (QuestLocationAliasBinaryOverlay)QuestLocationAliasBinaryOverlay
                            .QuestLocationAliasFactory(s.Slice(subRec.TotalLength), p);
                        ret.ID = id;
                        return ret;
                    }
                    case RecordTypeInts.ALCS:
                    {
                        return QuestCollectionAliasBinaryOverlay.QuestCollectionAliasFactory(s, p);
                    }
                    default:
                        throw new NotImplementedException();
                }
            });
        return (int)Quest_FieldIndex.Aliases;
    }

    public static RecordTriggerSpecs QuestSubGroupTriggerSpecs => _subGroupTriggerSpecs.Value;

    private static readonly Lazy<RecordTriggerSpecs> _subGroupTriggerSpecs = new Lazy<RecordTriggerSpecs>(() =>
    {
        var triggers =
            RecordCollection.Factory(
                RecordTypes.DIAL,
                RecordTypes.DLBR,
                RecordTypes.SCEN);
        var all =
            RecordCollection.Factory(
                RecordTypes.DIAL,
                RecordTypes.DLBR,
                RecordTypes.SCEN,
                RecordTypes.GRUP);
        return new RecordTriggerSpecs(allRecordTypes: all, triggeringRecordTypes: triggers);
    });
}