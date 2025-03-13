using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Noggog;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Skyrim.Assets;
using Mutagen.Bethesda.Skyrim.Records.Assets.VoiceType;
using RecordTypes = Mutagen.Bethesda.Skyrim.Internals.RecordTypes;

namespace Mutagen.Bethesda.Skyrim;

public partial class DialogTopic
{
    [Flags]
    public enum TopicFlag
    {
        DoAllBeforeRepeating = 0x1
    }

    public enum CategoryEnum
    {
        Topic = 0,
        Favor = 1,
        Scene = 2,
        Combat = 3,
        Favors = 4,
        Detection = 5,
        Service = 6,
        Misc = 7,
    }

    public enum SubtypeEnum
    {
        Custom = 0,
        ForceGreet = 1,
        Rumors = 2,
        Intimidate = 4,
        Flatter = 5,
        Bribe = 6,
        AskGift = 7,
        Gift = 8,
        AskFavor = 9,
        Favor = 10,
        ShowRelationships = 11,
        Follow = 12,
        Reject = 13,
        Scene = 14,
        Show = 15,
        Agree = 16,
        Refuse = 17,
        ExitFavorState = 18,
        MoralRefusal = 19,
        FlyingMountLand = 20,
        FlyingMountCancelLand = 21,
        FlyingMountAcceptTarget = 22,
        FlyingMountRejectTarget = 23,
        FlyingMountNoTarget = 24,
        FlyingMountDestinationReached = 25,
        Attack = 26,
        PowerAttack = 27,
        Bash = 28,
        Hit = 29,
        Flee = 30,
        Bleedout = 31,
        AvoidThreat = 32,
        Death = 33,
        GroupStrategy = 34,
        Block = 35,
        Taunt = 36,
        AllyKilled = 37,
        Steal = 38,
        Yield = 39,
        AcceptYield = 40,
        PickpocketCombat = 41,
        Assault = 42,
        Murder = 43,
        AssaultNC = 44,
        MurderNC = 45,
        PickpocketNC = 46,
        StealFromNC = 47,
        TrespassAgainstNC = 48,
        Trespass = 49,
        WerewolfTransformCrime = 50,
        VoicePowerStartShort = 51,
        VoicePowerStartLong = 52,
        VoicePowerEndShort = 53,
        VoicePowerEndLong = 54,
        AlertIdle = 55,
        LostIdle = 56,
        NormalToAlert = 57,
        AlertToCombat = 58,
        NormalToCombat = 59,
        AlertToNormal = 60,
        CombatToNormal = 61,
        CombatToLost = 62,
        LostToNormal = 63,
        LostToCombat = 64,
        DetectFriendDie = 65,
        ServiceRefusal = 66,
        Repair = 67,
        Travel = 68,
        Training = 69,
        BarterExit = 70,
        RepairExit = 71,
        Recharge = 72,
        RechargeExit = 73,
        TrainingExit = 74,
        ObserveCombat = 75,
        NoticeCorpse = 76,
        TimeToGo = 77,
        Goodbye = 78,
        Hello = 79,
        SwingMeleeWeapon = 80,
        ShootBow = 81,
        ZKeyObject = 82,
        Jump = 83,
        KnockOverObject = 84,
        DestroyObject = 85,
        StandOnFurniture = 86,
        LockedObject = 87,
        PickpocketTopic = 88,
        PursueIdleTopic = 89,
        SharedInfo = 90,
        PlayerCastProjectileSpell = 91,
        PlayerCastSelfSpell = 92,
        PlayerShout = 93,
        Idle = 94,
        EnterSprintBreath = 95,
        EnterBowZoomBreath = 96,
        ExitBowZoomBreath = 97,
        ActorCollideWithActor = 98,
        PlayerInIronSights = 99,
        OutOfBreath = 100,
        CombatGrunt = 101,
        LeaveWaterBreath = 102,
    }
}

partial class DialogTopicSetterCommon
{
    private static partial void RemapResolvedAssetLinks(
        IDialogTopic obj,
        IReadOnlyDictionary<IAssetLinkGetter, string> mapping,
        IAssetLinkCache? linkCache,
        AssetLinkQuery queryCategories)
    {
        // Nothing to do here, we can't change the form key of the dialogue or any other parameters, like quest editor id
    }
}

partial class DialogTopicCommon
{
    public static partial IEnumerable<IAssetLinkGetter> GetResolvedAssetLinks(IDialogTopicGetter obj, IAssetLinkCache linkCache, Type? assetType)
    {
        if (assetType != null && assetType != typeof(SkyrimSoundAssetType)) yield break;
        
        var voiceTypeLookup = linkCache.GetComponent<VoiceTypeAssetLookup>();
        foreach (var voiceTypePath in voiceTypeLookup.GetVoiceTypePaths(obj))
        {
            yield return new AssetLink<SkyrimSoundAssetType>(voiceTypePath);
        }
    }
}

partial class DialogTopicBinaryCreateTranslation
{
    public static partial void CustomBinaryEndImport(MutagenFrame frame, IDialogTopicInternal obj)
    {
        try
        {
            if (frame.Reader.Complete) return;
            if (!frame.TryGetGroupHeader(out var groupMeta)) return;
            if (groupMeta.GroupType == (int)GroupTypeEnum.TopicChildren)
            {
                obj.Timestamp = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.LastModifiedData);
                obj.Unknown = frame.GetInt32(offset: 20);
                if (FormKey.Factory(
                        frame.MetaData.MasterReferences,
                        new FormID(BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData)),
                        reference: true) != obj.FormKey)
                {
                    throw RecordException.Enrich(
                        new ArgumentException("Dialog children group did not match the FormID of the parent."),
                        obj);
                }
            }
            else
            {
                return;
            }
            frame.Reader.Position += groupMeta.HeaderLength;
            obj.Responses.SetTo(ListBinaryTranslation<DialogResponses>.Instance.Parse(
                reader: frame.SpawnWithLength(groupMeta.ContentLength),
                transl: (MutagenFrame r, RecordType header, [MaybeNullWhen(false)] out DialogResponses listItem) =>
                {
                    return LoquiBinaryTranslation<DialogResponses>.Instance.Parse(
                        frame: r,
                        item: out listItem);
                }));
        }
        catch (Exception ex)
        {
            throw RecordException.Enrich(ex, obj);
        }
    }

    public static partial ParseResult FillBinaryResponseCountCustom(MutagenFrame frame, IDialogTopicInternal item, PreviousParse lastParsed)
    {
        // Skip counter
        frame.ReadSubrecord();
        return null;
    }
}

partial class DialogTopicBinaryWriteTranslation
{
    public static partial void WriteBinaryResponseCountCustom(MutagenWriter writer, IDialogTopicGetter item)
    {
        if (item.Responses is not { } resp
            || resp.Count == 0)
        {
            using (HeaderExport.Subrecord(writer, RecordTypes.TIFC))
            {
                writer.WriteZeros(4);
            }
        }
        else
        {
            using (HeaderExport.Subrecord(writer, RecordTypes.TIFC))
            {
                writer.Write(resp.Count);
            }
        }
    }

    public static partial void CustomBinaryEndExport(MutagenWriter writer, IDialogTopicGetter obj)
    {
        try
        {
            if (obj.Responses is not { } resp
                || resp.Count == 0)
            {
                return;
            }
            using (HeaderExport.Header(writer, RecordTypes.GRUP, ObjectType.Group))
            {
                FormKeyBinaryTranslation.Instance.Write(
                    writer,
                    obj);
                writer.Write((int)GroupTypeEnum.TopicChildren);
                writer.Write(obj.Timestamp);
                writer.Write(obj.Unknown);
                ListBinaryTranslation<IDialogResponsesGetter>.Instance.Write(
                    writer: writer,
                    items: resp,
                    transl: (MutagenWriter subWriter, IDialogResponsesGetter subItem) =>
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

partial class DialogTopicBinaryOverlay
{
    public IReadOnlyList<IDialogResponsesGetter> Responses { get; private set; } = Array.Empty<IDialogResponsesGetter>();

    private ReadOnlyMemorySlice<byte>? _grupData;

    public int Timestamp => _grupData != null ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.GroupHeader(_grupData.Value).LastModifiedData) : 0;

    public int Unknown => _grupData.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_grupData.Value.Slice(20)) : default;

    partial void CustomEnd(OverlayStream stream, int finalPos, int offset)
    {
        try
        {
            if (stream.Complete) return;
            var startPos = stream.Position;
            if (!stream.TryGetGroupHeader(out var groupMeta)) return;
            if (groupMeta.GroupType != (int)GroupTypeEnum.TopicChildren) return;
            this._grupData = stream.ReadMemory(checked((int)groupMeta.TotalLength));
            var formKey = FormKey.Factory(
                _package.MetaData.MasterReferences, 
                new FormID(BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData)),
                reference: true);
            if (formKey != this.FormKey)
            {
                throw RecordException.Enrich(
                    new ArgumentException("Dialog children group did not match the FormID of the parent."),
                    this);
            }
            var contentSpan = this._grupData.Value.Slice(_package.MetaData.Constants.GroupConstants.HeaderLength);
            this.Responses = BinaryOverlayList.FactoryByArray<IDialogResponsesGetter>(
                contentSpan,
                _package,
                getter: (s, p) => DialogResponsesBinaryOverlay.DialogResponsesFactory(new OverlayStream(s, p), p),
                locs: ParseRecordLocations(
                    stream: new OverlayStream(contentSpan, _package),
                    trigger: DialogResponses_Registration.TriggeringRecordType,
                    constants: stream.MetaData.Constants.MajorConstants,
                    skipHeader: false));
        }
        catch (Exception ex)
        {
            throw RecordException.Enrich(ex, this);
        }
    }

    public partial ParseResult ResponseCountCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        return null;
    }
}