using Mutagen.Bethesda.Fallout76.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda.Fallout76;

partial class Quest
{
    [Flags]
    public enum MajorFlag
    {
        PartialForm = 0x4000
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

    public enum TypeEnum
    {
        None = 0,
        MainQuest = 1,
        Misc = 6,
        SideQuests = 7,
    }

    public enum EventType
    {
        CoOp = 0,
        Versus = 1,
        FreeForAll = 2,
        Defense = 3,
        Waves = 4,
        Boss = 5,
        Special = 6,
        Collection = 7,
        Escort = 8,
        Multi = 9,
    }

    public enum QuestLevel
    {
        Easy = 0,
        Medium = 1,
        Hard = 2,
        VeryHard = 3,
        None = 4,
    }

    public enum ActorReserveType
    {
        None = 0,
        VaultRaid = 1,
        Small = 2,
        Medium = 3,
        Large = 4,
    }

    public enum PublicEventDifficulty
    {
        VeryEasy = 0,
        Easy = 1,
        Medium = 2,
        Hard = 3,
        VeryHard = 4,
        Nuclear = 5,
    }

    public enum QuestType
    {
        None = 0,
        Primary = 1,
        Secondary = 2,
        SideQuest = 3,
        Server = 4,
        Daily = 5,
        PublicEvent = 6,
        Miscellaneous = 7,
        Event = 8,
        DailyOps = 9,
        Expedition = 10,
        Module = 11,
        Caravan = 12,
        Raid = 13,
    }

    [Flags]
    public enum TargetFlag
    {
        CompassMarkerIgnoresLocks = 0x1,
        Hostile = 0x2,
        UseStraightLinePathing = 0x4
    }
}

partial class QuestBinaryWriteTranslation
{
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
                    obj);
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
            RecordException.EnrichAndThrow(ex, obj);
            throw;
        }
    }
}

partial class QuestBinaryCreateTranslation
{
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
                if (FormKey.Factory(
                        frame.MetaData.MasterReferences,
                        new FormID(BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData)),
                        reference: true) != obj.FormKey)
                {
                    throw RecordException.Enrich(
                        new ArgumentException("Quest children group did not match the FormID of the parent."),
                        obj);
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
            RecordException.EnrichAndThrow(ex, obj);
            throw;
        }
    }
}

partial class QuestBinaryOverlay
{
    public ReadOnlyMemorySlice<Byte> General => Array.Empty<byte>();
    public Boolean NEXT2 => false;
    public Boolean NEXT3 => false;
    private ReadOnlyMemorySlice<byte>? _grupData;

    public int Timestamp => _grupData != null ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.GroupHeader(_grupData.Value).LastModifiedData) : 0;

    public int Unknown => _grupData.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_grupData.Value.Slice(20)) : default;

    public IReadOnlyList<IConditionGetter> DialogConditions => Array.Empty<IConditionGetter>();

    public IReadOnlyList<ISceneGetter> Scenes { get; private set; } = [];

    public IReadOnlyList<IDialogTopicGetter> DialogTopics { get; private set; } = [];

    public IReadOnlyList<IDialogBranchGetter> DialogBranches { get; private set; } = [];

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
            var formKey = FormKey.Factory(
                _package.MetaData.MasterReferences,
                new FormID(BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData)),
                reference: true);
            if (formKey != this.FormKey)
            {
                throw RecordException.Enrich(
                    new ArgumentException("Quest children group did not match the FormID of the parent."),
                    this);
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
            RecordException.EnrichAndThrow(ex, this);
            throw;
        }
    }
}
