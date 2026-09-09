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

    private static readonly (SubtypeEnum Subtype, string Marker, CategoryEnum Category)[] SubtypeMarkers =
    [
        (SubtypeEnum.Custom, "CUST", CategoryEnum.Topic),
        (SubtypeEnum.ForceGreet, "PFGT", CategoryEnum.Topic),
        (SubtypeEnum.Rumors, "RUMO", CategoryEnum.Topic),
        (SubtypeEnum.Intimidate, "INTI", CategoryEnum.Favor),
        (SubtypeEnum.Flatter, "FLAT", CategoryEnum.Favor),
        (SubtypeEnum.Bribe, "BRIB", CategoryEnum.Favor),
        (SubtypeEnum.AskGift, "ASKG", CategoryEnum.Favor),
        (SubtypeEnum.Gift, "GIFF", CategoryEnum.Favor),
        (SubtypeEnum.AskFavor, "ASKF", CategoryEnum.Favor),
        (SubtypeEnum.Favor, "FAVO", CategoryEnum.Favor),
        (SubtypeEnum.ShowRelationships, "SHRE", CategoryEnum.Favor),
        (SubtypeEnum.Follow, "FOLL", CategoryEnum.Favor),
        (SubtypeEnum.Reject, "FRJT", CategoryEnum.Favor),
        (SubtypeEnum.Scene, "SCEN", CategoryEnum.Scene),
        (SubtypeEnum.Show, "SHOW", CategoryEnum.Favors),
        (SubtypeEnum.Agree, "AGRE", CategoryEnum.Favors),
        (SubtypeEnum.Refuse, "REFU", CategoryEnum.Favors),
        (SubtypeEnum.ExitFavorState, "FEXT", CategoryEnum.Favors),
        (SubtypeEnum.MoralRefusal, "MREF", CategoryEnum.Favors),
        (SubtypeEnum.FlyingMountLand, "FMLX", CategoryEnum.Favors),
        (SubtypeEnum.FlyingMountCancelLand, "FMXL", CategoryEnum.Favors),
        (SubtypeEnum.FlyingMountAcceptTarget, "FMAT", CategoryEnum.Favors),
        (SubtypeEnum.FlyingMountRejectTarget, "FMRT", CategoryEnum.Favors),
        (SubtypeEnum.FlyingMountNoTarget, "FMNT", CategoryEnum.Favors),
        (SubtypeEnum.FlyingMountDestinationReached, "FMDR", CategoryEnum.Favors),
        (SubtypeEnum.Attack, "ATCK", CategoryEnum.Combat),
        (SubtypeEnum.PowerAttack, "POAT", CategoryEnum.Combat),
        (SubtypeEnum.Bash, "BASH", CategoryEnum.Combat),
        (SubtypeEnum.Hit, "HIT_", CategoryEnum.Combat),
        (SubtypeEnum.Flee, "FLEE", CategoryEnum.Combat),
        (SubtypeEnum.Bleedout, "BLED", CategoryEnum.Combat),
        (SubtypeEnum.AvoidThreat, "AVTH", CategoryEnum.Combat),
        (SubtypeEnum.Death, "DETH", CategoryEnum.Combat),
        (SubtypeEnum.GroupStrategy, "GRST", CategoryEnum.Combat),
        (SubtypeEnum.Block, "BLOC", CategoryEnum.Combat),
        (SubtypeEnum.Taunt, "TAUT", CategoryEnum.Combat),
        (SubtypeEnum.AllyKilled, "ALKL", CategoryEnum.Combat),
        (SubtypeEnum.Steal, "STEA", CategoryEnum.Combat),
        (SubtypeEnum.Yield, "YIEL", CategoryEnum.Combat),
        (SubtypeEnum.AcceptYield, "ACYI", CategoryEnum.Combat),
        (SubtypeEnum.PickpocketCombat, "PICC", CategoryEnum.Combat),
        (SubtypeEnum.Assault, "ASSA", CategoryEnum.Combat),
        (SubtypeEnum.Murder, "MURD", CategoryEnum.Combat),
        (SubtypeEnum.AssaultNC, "ASNC", CategoryEnum.Combat),
        (SubtypeEnum.MurderNC, "MUNC", CategoryEnum.Combat),
        (SubtypeEnum.PickpocketNC, "PICN", CategoryEnum.Combat),
        (SubtypeEnum.StealFromNC, "STFN", CategoryEnum.Combat),
        (SubtypeEnum.TrespassAgainstNC, "TRAN", CategoryEnum.Combat),
        (SubtypeEnum.Trespass, "TRES", CategoryEnum.Combat),
        (SubtypeEnum.WerewolfTransformCrime, "WTCR", CategoryEnum.Combat),
        (SubtypeEnum.VoicePowerStartShort, "VPSS", CategoryEnum.Combat),
        (SubtypeEnum.VoicePowerStartLong, "VPSL", CategoryEnum.Combat),
        (SubtypeEnum.VoicePowerEndShort, "VPES", CategoryEnum.Combat),
        (SubtypeEnum.VoicePowerEndLong, "VPEL", CategoryEnum.Combat),
        (SubtypeEnum.AlertIdle, "ALIL", CategoryEnum.Detection),
        (SubtypeEnum.LostIdle, "LOIL", CategoryEnum.Detection),
        (SubtypeEnum.NormalToAlert, "NOTA", CategoryEnum.Detection),
        (SubtypeEnum.AlertToCombat, "ALTC", CategoryEnum.Detection),
        (SubtypeEnum.NormalToCombat, "NOTC", CategoryEnum.Detection),
        (SubtypeEnum.AlertToNormal, "ALTN", CategoryEnum.Detection),
        (SubtypeEnum.CombatToNormal, "COTN", CategoryEnum.Detection),
        (SubtypeEnum.CombatToLost, "COLO", CategoryEnum.Detection),
        (SubtypeEnum.LostToNormal, "LOTN", CategoryEnum.Detection),
        (SubtypeEnum.LostToCombat, "LOTC", CategoryEnum.Detection),
        (SubtypeEnum.DetectFriendDie, "DFDA", CategoryEnum.Detection),
        (SubtypeEnum.ServiceRefusal, "SERU", CategoryEnum.Service),
        (SubtypeEnum.Repair, "REPA", CategoryEnum.Service),
        (SubtypeEnum.Travel, "TRAV", CategoryEnum.Service),
        (SubtypeEnum.Training, "TRAI", CategoryEnum.Service),
        (SubtypeEnum.BarterExit, "BAEX", CategoryEnum.Service),
        (SubtypeEnum.RepairExit, "REEX", CategoryEnum.Service),
        (SubtypeEnum.Recharge, "RECH", CategoryEnum.Service),
        (SubtypeEnum.RechargeExit, "RCEX", CategoryEnum.Service),
        (SubtypeEnum.TrainingExit, "TREX", CategoryEnum.Service),
        (SubtypeEnum.ObserveCombat, "OBCO", CategoryEnum.Misc),
        (SubtypeEnum.NoticeCorpse, "NOTI", CategoryEnum.Misc),
        (SubtypeEnum.TimeToGo, "TITG", CategoryEnum.Misc),
        (SubtypeEnum.Goodbye, "GBYE", CategoryEnum.Misc),
        (SubtypeEnum.Hello, "HELO", CategoryEnum.Misc),
        (SubtypeEnum.SwingMeleeWeapon, "SWMW", CategoryEnum.Misc),
        (SubtypeEnum.ShootBow, "FIWE", CategoryEnum.Misc),
        (SubtypeEnum.ZKeyObject, "ZKEY", CategoryEnum.Misc),
        (SubtypeEnum.Jump, "JUMP", CategoryEnum.Misc),
        (SubtypeEnum.KnockOverObject, "KNOO", CategoryEnum.Misc),
        (SubtypeEnum.DestroyObject, "DEOB", CategoryEnum.Misc),
        (SubtypeEnum.StandOnFurniture, "STOF", CategoryEnum.Misc),
        (SubtypeEnum.LockedObject, "LOOB", CategoryEnum.Misc),
        (SubtypeEnum.PickpocketTopic, "PICT", CategoryEnum.Misc),
        (SubtypeEnum.PursueIdleTopic, "PURS", CategoryEnum.Misc),
        (SubtypeEnum.SharedInfo, "IDAT", CategoryEnum.Misc),
        (SubtypeEnum.PlayerCastProjectileSpell, "PCPS", CategoryEnum.Misc),
        (SubtypeEnum.PlayerCastSelfSpell, "PCSS", CategoryEnum.Misc),
        (SubtypeEnum.PlayerShout, "PCSH", CategoryEnum.Misc),
        (SubtypeEnum.Idle, "IDLE", CategoryEnum.Misc),
        (SubtypeEnum.EnterSprintBreath, "BREA", CategoryEnum.Misc),
        (SubtypeEnum.EnterBowZoomBreath, "ENBZ", CategoryEnum.Misc),
        (SubtypeEnum.ExitBowZoomBreath, "EXBZ", CategoryEnum.Misc),
        (SubtypeEnum.ActorCollideWithActor, "ACAC", CategoryEnum.Misc),
        (SubtypeEnum.PlayerInIronSights, "PIRN", CategoryEnum.Misc),
        (SubtypeEnum.OutOfBreath, "OUTB", CategoryEnum.Misc),
        (SubtypeEnum.CombatGrunt, "GRNT", CategoryEnum.Misc),
        (SubtypeEnum.LeaveWaterBreath, "LWBS", CategoryEnum.Misc),
    ];

    private static readonly IReadOnlyDictionary<RecordType, SubtypeEnum> _markerToSubtype =
        SubtypeMarkers.ToDictionary(x => new RecordType(x.Marker), x => x.Subtype);

    private static readonly IReadOnlyDictionary<SubtypeEnum, (RecordType Marker, CategoryEnum Category)> _subtypeLookup =
        SubtypeMarkers.ToDictionary(x => x.Subtype, x => (new RecordType(x.Marker), x.Category));

    /// <summary>The subtype a SNAM marker names, or null if it names none.</summary>
    public static SubtypeEnum? SubtypeFromMarker(RecordType marker) =>
        _markerToSubtype.TryGetValue(marker, out var subtype) ? subtype : null;

    /// <summary>The SNAM marker for a subtype, or null if it has none.</summary>
    public static RecordType? MarkerFromSubtype(SubtypeEnum subtype) =>
        _subtypeLookup.TryGetValue(subtype, out var found) ? found.Marker : (RecordType?)null;

    /// <summary>The DATA category for a subtype, or null if it has none.</summary>
    public static CategoryEnum? CategoryFromSubtype(SubtypeEnum subtype) =>
        _subtypeLookup.TryGetValue(subtype, out var found) ? found.Category : null;
}

partial class DialogTopicBinaryCreateTranslation
{
    public static partial void CustomBinaryEndImport(MutagenFrame frame, IDialogTopicInternal obj)
    {
        if (DialogTopic.SubtypeFromMarker(obj.SubtypeName) is { } snamSubtype)
        {
            obj.Subtype = snamSubtype;
            if (DialogTopic.CategoryFromSubtype(snamSubtype) is { } snamCategory)
            {
                obj.Category = snamCategory;
            }
        }
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

    public static partial ParseResult FillBinaryDataCustom(MutagenFrame frame, IDialogTopicInternal item, PreviousParse lastParsed)
    {
        var content = frame.ReadSubrecord().Content;
        if (content.Length >= 1)
        {
            item.TopicFlags = (DialogTopic.TopicFlag)content[0];
        }
        if (content.Length >= 2)
        {
            item.Category = (DialogTopic.CategoryEnum)content[1];
        }
        if (content.Length >= 4)
        {
            item.Subtype = (DialogTopic.SubtypeEnum)BinaryPrimitives.ReadUInt16LittleEndian(content.Slice(2));
        }
        return (int)DialogTopic_FieldIndex.Subtype;
    }

    public static partial void FillBinarySubtypeNameCustom(MutagenFrame frame, IDialogTopicInternal item, PreviousParse lastParsed)
    {
        var content = frame.ReadSubrecord().Content;
        if (content.Length < 4) return;
        item.SubtypeName = new RecordType(BinaryPrimitives.ReadInt32LittleEndian(content));
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
    public static partial void WriteBinaryDataCustom(MutagenWriter writer, IDialogTopicGetter item)
    {
        using (HeaderExport.Subrecord(writer, RecordTypes.DATA))
        {
            writer.Write((byte)item.TopicFlags);
            writer.Write((byte)(DialogTopic.CategoryFromSubtype(item.Subtype) ?? item.Category));
            writer.Write(checked((ushort)item.Subtype));
        }
    }

    public static partial void WriteBinarySubtypeNameCustom(MutagenWriter writer, IDialogTopicGetter item)
    {
        var marker = DialogTopic.MarkerFromSubtype(item.Subtype) ?? item.SubtypeName;
        using (HeaderExport.Subrecord(writer, RecordTypes.SNAM))
        {
            writer.Write(marker.TypeInt);
        }
    }

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
            RecordException.EnrichAndThrow(ex, obj);
            throw;
        }
    }
}

partial class DialogTopicBinaryOverlay
{
    public IReadOnlyList<IDialogResponsesGetter> Responses { get; private set; } = [];

    private ReadOnlyMemorySlice<byte>? _grupData;

    public int Timestamp => _grupData != null ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.GroupHeader(_grupData.Value).LastModifiedData) : 0;

    public int Unknown => _grupData.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_grupData.Value.Slice(20)) : default;

    partial void CustomEnd(OverlayStream stream, int finalPos, int offset)
    {
        try
        {
            if (stream.Complete) return;
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

    public partial ParseResult ResponseCountCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        return null;
    }

    private RangeInt32? _DATALocation;

    private int? _SubtypeNameLocation;

    public partial ParseResult DataCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        var header = stream.GetSubrecordHeader();
        var start = (stream.Position - offset) + header.HeaderLength;
        _DATALocation = new RangeInt32(start, start + header.ContentLength - 1);
        return (int)DialogTopic_FieldIndex.Subtype;
    }

    partial void SubtypeNameCustomParse(OverlayStream stream, int finalPos, int offset)
    {
        _SubtypeNameLocation = stream.Position - offset;
    }

    private ReadOnlySpan<byte> DataContent =>
        _DATALocation is { } loc ? _recordData.Span.Slice(loc.Min, loc.Max - loc.Min + 1) : default;

    public partial RecordType GetSubtypeNameCustom() => _SubtypeNameLocation.HasValue
        ? new RecordType(BinaryPrimitives.ReadInt32LittleEndian(HeaderTranslation.ExtractSubrecordMemory(_recordData, _SubtypeNameLocation.Value, _package.MetaData.Constants)))
        : RecordType.Null;

    public DialogTopic.TopicFlag TopicFlags
    {
        get
        {
            var data = DataContent;
            return data.Length >= 1 ? (DialogTopic.TopicFlag)data[0] : default;
        }
    }

    public DialogTopic.CategoryEnum Category
    {
        get
        {
            if (DialogTopic.SubtypeFromMarker(SubtypeName) is { } subtype
                && DialogTopic.CategoryFromSubtype(subtype) is { } category)
            {
                return category;
            }
            var data = DataContent;
            return data.Length >= 2 ? (DialogTopic.CategoryEnum)data[1] : default;
        }
    }

    public DialogTopic.SubtypeEnum Subtype
    {
        get
        {
            if (DialogTopic.SubtypeFromMarker(SubtypeName) is { } subtype) return subtype;
            var data = DataContent;
            return data.Length >= 4 ? (DialogTopic.SubtypeEnum)BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(2)) : default;
        }
    }
}