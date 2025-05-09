using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda.Fallout4;

partial class DialogTopic
{
    [Flags]
    public enum TopicFlag
    {
        DoAllBeforeRepeating = 0x1
    }

    public enum CategoryEnum
    {
        Player = 0,
        Command = 1,
        Scene = 2,
        Combat = 3,
        Favor = 4,
        Detection = 5,
        Service = 6,
        Misc = 7,
    }

    public enum SubtypeEnum
    {
        Custom0 = 0,
        ForceGreet = 1,
        Rumors = 2,
        Custom3 = 3,
        Call = 4,
        Follow = 5,
        Move = 6,
        Attack = 7,
        Inspect = 8,
        Retrieve = 9,
        Stay = 10,
        Release = 11,
        ShowRelationships = 12,
        Reject = 13,
        Heal = 14,
        Assign = 15,
        Enter = 16,
        Custom17 = 17,
        Show = 18,
        Agree = 19,
        Refuse = 20,
        ExitFavorState = 21,
        MoralRefusal = 22,
        Trade = 23,
        PathingRefusal = 24,
        Attack25 = 25,
        PowerAttack = 26,
        Bash = 27,
        Hit = 28,
        Flee = 29,
        BleedOut = 30,
        AvoidThreat = 31,
        Death = 32,
        Block = 33,
        Taunt = 34,
        ThrowGrenade = 35,
        AllyKilled = 36,
        OrderFallback = 37,
        OrderMoveUp = 38,
        OrderFlank = 39,
        OrderTakeCover = 40,
        Retreat = 41,
        CoverMe = 42,
        SuppressiveFire = 43,
        CrippledLimb = 44,
        PairedAttack = 45,
        Steal = 46,
        Yield = 47,
        AcceptYield = 48,
        PickpocketCombat = 49,
        Assault = 50,
        Murder = 51,
        AssaultNC = 52,
        MurderNC = 53,
        PickpocketNC = 54,
        StealFromNC = 55,
        TrespassAgainstNC = 56,
        Trespass = 57,
        UNUSED01 = 58,
        VoicePowerStartShort = 59,
        VoicePowerStartLong = 60,
        VoicePowerEndShort = 61,
        VoicePowerEndLong = 62,
        AlertIdle = 63,
        LostIdle = 64,
        NormalToAlert = 65,
        NormalToCombat = 66,
        NormalToLost = 67,
        AlertToNormal = 68,
        AlertToCombat = 69,
        CombatToNormal = 70,
        CombatToLost = 71,
        LostToNormal = 72,
        LostToCombat = 73,
        DetectFriendDie = 74,
        ServiceRefusal = 75,
        Repair = 76,
        Travel = 77,
        Training = 78,
        BarterExit = 79,
        RepairExit = 80,
        Recharge = 81,
        RechargeExit = 82,
        TrainingExit = 83,
        ObserveCombat = 84,
        NoticeCorpse = 85,
        TimeToGo = 86,
        Goodbye = 87,
        Hello = 88,
        SwingMeleeWeapon = 89,
        ShootBow = 90,
        ZKeyObject = 91,
        Jump = 92,
        KnockOverObject = 93,
        DestroyObject = 94,
        StandonFurniture = 95,
        LockedObject = 96,
        PickpocketTopic = 97,
        PursueIdleTopic = 98,
        SharedInfo = 99,
        SceneChoice = 100,
        PlayerCastProjectileSpell = 101,
        PlayerCastSelfSpell = 102,
        PlayerShout = 103,
        Idle = 104,
        EnterSprintBreath = 105,
        EnterBowZoomBreath = 106,
        ExitBowZoomBreath = 107,
        ActorCollidewithActor = 108,
        PlayerinIronSights = 109,
        OutofBreath = 110,
        CombatGrunt = 111,
        LeaveWaterBreath = 112,
        ImpatientPositive = 113,
        ImpatientNegative = 114,
        ImpatientNeutral = 115,
        ImpatientQuestion = 116,
        WaitingForPlayerInput = 117,
        Greeting = 118,
        PlayerActivateDoor = 119,
        PlayerActivateTerminals = 120,
        PlayerActivateFurniture = 121,
        PlayerActivateActivators = 122,
        PlayerActivateContainer = 123,
        PlayerAquireFeaturedItem = 124,
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
            RecordException.EnrichAndThrow(ex, obj);
            throw;
        }
    }

    public static partial ParseResult FillBinaryInfoCountCustom(MutagenFrame frame, IDialogTopicInternal item, PreviousParse lastParsed)
    {
        frame.ReadSubrecord(RecordTypes.TIFC);
        return (int)DialogTopic_FieldIndex.SubtypeName;
    }
}

partial class DialogTopicBinaryWriteTranslation
{
    public static partial void WriteBinaryInfoCountCustom(MutagenWriter writer, IDialogTopicGetter item)
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
            RecordException.EnrichAndThrow(ex, obj);
            throw;
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
            RecordException.EnrichAndThrow(ex, this);
            throw;
        }
    }

    public partial ParseResult InfoCountCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        stream.ReadSubrecord(RecordTypes.TIFC);
        return (int)DialogTopic_FieldIndex.SubtypeName;
    }
}