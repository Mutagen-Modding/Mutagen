using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Meta;
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
        RecordTypes.ALED,
    };

    public static partial ParseResult FillBinaryAliasParseCustom(MutagenFrame frame, IQuestInternal item,
        PreviousParse lastParsed)
    {
        frame = frame.SpawnAll();
        frame.TryReadSubrecord(RecordTypes.ANAM, out _);
        item.Aliases = new();
        while (frame.TryGetSubrecord(_expectedAliasRecords, out var subRec))
        {
            switch (subRec.RecordTypeInt)
            {
                case RecordTypeInts.ALST:
                {
                    var ret = QuestReferenceAlias.CreateFromBinary(frame);
                    item.Aliases.Add(ret);
                    break;
                }
                case RecordTypeInts.ALLS:
                {
                    var ret = QuestLocationAlias.CreateFromBinary(frame);
                    item.Aliases.Add(ret);
                    break;
                }
                case RecordTypeInts.ALCS:
                case RecordTypeInts.ALMI:
                {
                    var ret = new QuestCollectionAlias();
                    ret.Collection.SetTo(
                        ListBinaryTranslation<CollectionAlias>.Instance.Parse(
                            frame,
                            CollectionAlias.TryCreateFromBinary));
                    item.Aliases.Add(ret);
                    break;
                }
                case RecordTypeInts.ALED:
                    frame.Position += subRec.TotalLength;
                    break;
                default:
                    return (int)Quest_FieldIndex.Aliases;
            }
        }

        return (int)Quest_FieldIndex.Aliases;
    }
    
    public static partial void ParseSubgroupsLogic(MutagenFrame frame, IQuestInternal obj)
    {
        try
        {
            if (frame.Reader.Complete) return;
            if (!frame.TryGetGroupHeader(out var groupMeta)) return;
            if (groupMeta.GroupType == (int)GroupTypeEnum.QuestChildren)
            {
                obj.Timestamp = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.LastModifiedData);
                obj.Unknown = frame.GetInt32(offset: 20);
                if (FormKey.Factory(frame.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData)) != obj.FormKey)
                {
                    throw new ArgumentException("Quest children group did not match the FormID of the parent.");
                }
            }
            else
            {
                return;
            }
            frame.Reader.Position += groupMeta.HeaderLength;
            frame = frame.SpawnWithLength(groupMeta.ContentLength);
            var records = ListBinaryTranslation<IMajorRecord>.Instance.Parse(
                reader: frame,
                transl: (MutagenFrame r, RecordType header, [MaybeNullWhen(false)] out IMajorRecord rec) =>
                {
                    switch (header.TypeInt)
                    {
                        case RecordTypeInts.DIAL:
                            rec = DialogTopic.CreateFromBinary(r);
                            return true;
                        case RecordTypeInts.SCEN:
                            rec = Scene.CreateFromBinary(r);
                            return true;
                        case RecordTypeInts.DLBR:
                            rec = DialogBranch.CreateFromBinary(r);
                            return true;
                        default:
                            throw new NotImplementedException();
                    }
                });
            obj.Scenes.SetTo(records.WhereCastable<IMajorRecord, Scene>());
            obj.DialogTopics.SetTo(records.WhereCastable<IMajorRecord, DialogTopic>());
            obj.DialogBranches.SetTo(records.WhereCastable<IMajorRecord, DialogBranch>());
        }
        catch (Exception ex)
        {
            throw RecordException.Enrich(ex, obj);
        }
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
                        {
                            return collAlias.Collection
                                .Select(x => Math.Max(x.ReferenceAlias.ID, x.ID ?? 0))
                                .StartWith((uint)0)
                                .Max();
                        }
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
                    refAlias.WriteToBinary(writer);
                    break;
                case IQuestLocationAliasGetter locAlias:
                    locAlias.WriteToBinary(writer);
                    break;
                case IQuestCollectionAliasGetter collAlias:
                    collAlias.WriteToBinary(writer);
                    break;
                default:
                    throw new NotImplementedException();
            }
            using (HeaderExport.Subrecord(writer, RecordTypes.ALED)) { }
        }
    }

    public static partial void WriteSubgroupsLogic(MutagenWriter writer, IQuestGetter obj)
    {
        try
        {
            var scenes = obj.Scenes;
            var dialogTopics = obj.DialogTopics;
            var dialogBranches = obj.DialogBranches;
            if (scenes.Count == 0
                && dialogTopics.Count == 0
                && dialogBranches.Count == 0)
            {
                return;
            }
            using (HeaderExport.Header(writer, RecordTypes.GRUP, ObjectType.Group))
            {
                FormKeyBinaryTranslation.Instance.Write(
                    writer,
                    obj.FormKey);
                writer.Write((int)GroupTypeEnum.QuestChildren);
                writer.Write(obj.Timestamp);
                writer.Write(obj.Unknown);
                ListBinaryTranslation<IDialogBranchGetter>.Instance.Write(
                    writer: writer,
                    items: dialogBranches,
                    transl: (MutagenWriter subWriter, IDialogBranchGetter subItem) =>
                    {
                        subItem.WriteToBinary(subWriter);
                    });
                ListBinaryTranslation<IDialogTopicGetter>.Instance.Write(
                    writer: writer,
                    items: dialogTopics,
                    transl: (MutagenWriter subWriter, IDialogTopicGetter subItem) =>
                    {
                        subItem.WriteToBinary(subWriter);
                    });
                ListBinaryTranslation<ISceneGetter>.Instance.Write(
                    writer: writer,
                    items: scenes,
                    transl: (MutagenWriter subWriter, ISceneGetter subItem) =>
                    {
                        subItem.WriteToBinary(subWriter);
                    });
            }
        }
        catch (Exception ex)
        {
            throw RecordException.Enrich(ex, obj);
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

    private static readonly IReadOnlyRecordCollection _aliasEndTriggers = RecordCollection.Factory(RecordTypes.ALED);
    private static readonly IReadOnlyRecordCollection _aliasStartTriggers = RecordCollection.Factory(
        RecordTypes.ALST,
        RecordTypes.ALLS,
        RecordTypes.ALCS);
    
    public partial ParseResult AliasParseCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        stream.TryReadSubrecord(RecordTypes.ANAM, out _);
        var mem = stream.RemainingMemory;
        var locs = ParseRecordLocationsEnder(
            stream,
            startTriggers: _aliasStartTriggers,
            endTriggers: _aliasEndTriggers,
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
                        return QuestReferenceAliasBinaryOverlay
                            .QuestReferenceAliasFactory(s, p);
                    }
                    case RecordTypeInts.ALLS:
                    {
                        return QuestLocationAliasBinaryOverlay
                            .QuestLocationAliasFactory(s, p);
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

    public partial void ParseSubgroupsLogic(OverlayStream stream, int finalPos, int offset)
    {
        try
        {
            if (stream.Complete) return;
            if (!stream.TryGetGroupHeader(out var groupMeta)) return;
            if (groupMeta.GroupType != (int)GroupTypeEnum.QuestChildren) return;
            this._grupData = stream.ReadMemory(checked((int)groupMeta.TotalLength));
            var formKey = FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData));
            if (formKey != this.FormKey)
            {
                throw new ArgumentException("Quest children group did not match the FormID of the parent.");
            }
            var contentSpan = this._grupData.Value.Slice(_package.MetaData.Constants.GroupConstants.HeaderLength);
            var locs = ParseRecordLocations(
                    stream: new OverlayStream(contentSpan, _package),
                    trigger: QuestSubGroupTriggerSpecs,
                    constants: stream.MetaData.Constants.MajorConstants,
                    triggersAlwaysAreNewRecords: true,
                    skipHeader: false).Select(x => _package.MetaData.Constants.MajorRecordHeader(contentSpan.Slice(x)).Pin(x));

            this.DialogBranches = BinaryOverlayList.FactoryByArray<IDialogBranchGetter>(
                contentSpan,
                _package,
                getter: (s, p) => DialogBranchBinaryOverlay.DialogBranchFactory(new OverlayStream(s, p), p),
                locs: locs.Where(s => s.RecordType == RecordTypes.DLBR).Select(x => x.Location).ToArray());

            this.DialogTopics = BinaryOverlayList.FactoryByArray<IDialogTopicGetter>(
                contentSpan,
                _package,
                getter: (s, p) => DialogTopicBinaryOverlay.DialogTopicFactory(new OverlayStream(s, p), p),
                locs: locs.Where(s => s.RecordType == RecordTypes.DIAL).Select(x => x.Location).ToArray());

            this.Scenes = BinaryOverlayList.FactoryByArray<ISceneGetter>(
                contentSpan,
                _package,
                getter: (s, p) => SceneBinaryOverlay.SceneFactory(new OverlayStream(s, p), p),
                locs: locs.Where(s => s.RecordType == RecordTypes.SCEN).Select(x => x.Location).ToArray());
        }
        catch (Exception ex)
        {
            throw RecordException.Enrich(ex, this);
        }
    }
}