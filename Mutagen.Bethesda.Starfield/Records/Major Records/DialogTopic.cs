using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Starfield.Internals;
using Noggog;

namespace Mutagen.Bethesda.Starfield;

public partial class DialogTopic
{
    [Flags]
    public enum MajorFlag
    {
        PartialForm = 0x04000
    }
    
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
        Custom = 0,
        ForceGreet = 1,
        Rumors = 2,
        CustomCommand = 3,
        Call = 4,
        Follow = 5,
        Move = 6,
        AttackCommand = 7,
        Inspect = 8,
        Retrieve = 9,
        Stay = 10,
        Release = 11,
        ShowRelationships = 12,
        Reject = 13,
        Heal = 14,
        Assign = 15,
        Enter = 16,
        CustomScene = 17,
        Show = 18,
        Agree = 19,
        Refuse = 20,
        ExitFavorState = 21,
        MoralRefusal = 22,
        Trade = 23,
        PathingRefusal = 24,
        MeleeAttack = 25,
        PowerAttack = 26,
        Bash = 27,
        Hit = 28,
        Flee = 29,
        BleedOut = 30,
        ElectromagneticShocked = 31,
        AvoidThreat = 32,
        Death = 33,
        Block = 34,
        Taunt = 35,
        ThrowGrenade = 36,
        UseMagic = 37,
        AllyKilled = 38,
        EnemyKilled = 39,
        OrderFallback = 40,
        OrderMoveUp = 41,
        OrderFlank = 42,
        OrderTakeCover = 43,
        Fallback = 44,
        CoverMe = 45,
        SuppressiveFire = 46,
        CrippledLimb = 47,
        PairedAttack = 48,
        TakeSuppressiveFire = 49,
        RangedAttack = 50,
        Reload = 51,
        ShipEnginesDown = 52,
        ShipThrustersDown = 53,
        ShipShieldsDown = 54,
        ShipGravDriveDown = 55,
        ShipWeaponsDown = 56,
        ShipEnginesRepaired = 57,
        ShipGravDriveRepaired = 58,
        ShipThrustersRepaired = 59,
        ShipShieldsRepaired = 60,
        ShipWeaponsRepaired = 61,
        ShipEnemyEngineDown = 62,
        ShipEnemyGravJumpDown = 63,
        ShipEnemyShieldsDown = 64,
        ShipEnemyThrustersDown = 65,
        ShipEnemyWeaponsDown = 66,
        ShipEnemyEngineRepaired = 67,
        ShipEnemyGravJumpRepaired = 68,
        ShipEnemyShieldsRepaired = 69,
        ShipEnemyThrustersRepaired = 70,
        ShipEnemyWeaponsRepaired = 71,
        ShipShieldsZeroHealth = 72,
        ShipEnterCombat = 73,
        ShipLeaveCombat = 74,
        Steal = 75,
        Yield = 76,
        AcceptYield = 77,
        PickpocketCombat = 78,
        Assault = 79,
        Murder = 80,
        AssaultNC = 81,
        MurderNC = 82,
        PickpocketNC = 83,
        StealFromNC = 84,
        TrespassAgainstNC = 85,
        Trespass = 86,
        TresspassNoVisual = 87,
        Curious = 88,
        ThreatBackdown = 89,
        StealTakeback = 90,
        StealTakebackSuccess = 91,
        Piracy = 92,
        SmugglingNC = 93,
        Smuggling = 94,
        ReactToWarn = 95,
        VoicePowerStartShort = 96,
        VoicePowerStartLong = 97,
        VoicePowerEndShort = 98,
        VoicePowerEndLong = 99,
        AlertIdle = 100,
        LostIdle = 101,
        NormalToAlert = 102,
        NormalToCombat = 103,
        NormalToLost = 104,
        AlertToNormal = 105,
        AlertToCombat = 106,
        CombatToNormal = 107,
        CombatToLost = 108,
        LostToNormal = 109,
        LostToCombat = 110,
        DetectFriendDie = 111,
        ServiceRefusal = 112,
        Repair = 113,
        Travel = 114,
        Training = 115,
        BarterExit = 116,
        RepairExit = 117,
        Recharge = 118,
        RechargeExit = 119,
        TrainingExit = 120,
        ObserveCombat = 121,
        NoticeCorpse = 122,
        TimeToGo = 123,
        Goodbye = 124,
        Hello = 125,
        SwingMeleeWeapon = 126,
        ShootBow = 127,
        ZKeyObject = 128,
        Jump = 129,
        KnockOverObject = 130,
        DestroyObject = 131,
        StandonFurniture = 132,
        LockedObject = 133,
        PickpocketTopic = 134,
        ShipCrewGravJumpCalculationComplete = 135,
        ShipCrewLandingStart = 136,
        ShipCrewReturnToOrbit = 137,
        ShipCrewDockingComplete = 138,
        ShipUndockingComplete = 139,
        ShipCrewEnemyDockingInitiated = 140,
        ShipCrewGravJumpCompleted = 141,
        ShipCrewTakeOffInitiated = 142,
        PursueIdleTopic = 143,
        SharedInfo = 144,
        SceneChoice = 145,
        PlayerCastProjectileSpell = 146,
        PlayerCastSelfSpell = 147,
        PlayerShout = 148,
        Idle = 149,
        EnterSprintBreath = 150,
        EnterBowZoomBreath = 151,
        ExitBowZoomBreath = 152,
        ActorCollidewithActor = 153,
        PlayerinIronSights = 154,
        OutofBreath = 155,
        CombatGrunt = 156,
        LeaveWaterBreath = 157,
        ImpatientPositive = 158,
        ImpatientNegative = 159,
        ImpatientNeutral = 160,
        ImpatientQuestion = 161,
        WaitingForPlayerInput = 162,
        Greeting = 163,
        PlayerActivateDoor = 164,
        PlayerActivateTerminals = 165,
        PlayerActivateFurniture = 166,
        PlayerActivateActivators = 167,
        PlayerActivateContainer = 168,
        PlayerAquireFeaturedItem = 169,
        MaxCarbonDioxide = 170,
        BlockingHellos = 171,
        VehiclePassengerEnterVehicle = 172,
        VehiclePassengerEngageBoost = 173,
        VehiclePassengerHorizontalCollision = 174,
        VehiclePassengerVerticalCollision = 175,
        VehiclePassengerEnterWater = 176,
        VehiclePassengerNonMovingIdle = 177,
        VehiclePassengerTriggerMine = 178,
        VehiclePassengerAimMountedWeapon = 179,
        VehiclePassengerKillWithCollision = 180,
        VehiclePassengerKillWithWeapon = 181,
        VehiclePassengerVerticalCollisionLowVelocity = 182,
    }
    
    public enum SubtypeNameEnum
    {
        ActorCollidewithActor = RecordTypeInts.ACAC,
        AcceptYield = RecordTypeInts.ACYI,
        Agree = RecordTypeInts.AGRE,
        AlertIdle = RecordTypeInts.ALIL,
        AllyKilled = RecordTypeInts.ALKL,
        AlertToCombat = RecordTypeInts.ALTC,
        AlertToNormal = RecordTypeInts.ALTN,
        Retrieve = RecordTypeInts.ASKF,
        AttackCommand = RecordTypeInts.ASKG,
        AssaultNC = RecordTypeInts.ASNC,
        Assault = RecordTypeInts.ASSA,
        Assign = RecordTypeInts.ASSI,
        MeleeAttack = RecordTypeInts.ATCK,
        AvoidThreat = RecordTypeInts.AVTH,
        BarterExit = RecordTypeInts.BAEX,
        Bash = RecordTypeInts.BASH,
        SuppressiveFire = RecordTypeInts.BGST,
        BleedOut = RecordTypeInts.BLED,
        BlockingHellos = RecordTypeInts.BLHE,
        Block = RecordTypeInts.BLOC,
        EnterSprintBreath = RecordTypeInts.BREA,
        Move = RecordTypeInts.BRIB,
        CombatToLost = RecordTypeInts.COLO,
        CombatToNormal = RecordTypeInts.COTN,
        CrippledLimb = RecordTypeInts.CRIL,
        Custom = RecordTypeInts.CUST,
        DestroyObject = RecordTypeInts.DEOB,
        Death = RecordTypeInts.DETH,
        DetectFriendDie = RecordTypeInts.DFDA,
        EnterBowZoomBreath = RecordTypeInts.ENBZ,
        EnemyKilled = RecordTypeInts.ENKL,
        Enter = RecordTypeInts.ENTE,
        ExitBowZoomBreath = RecordTypeInts.EXBZ,
        ExitFavorState = RecordTypeInts.FEXT,
        ShootNear = RecordTypeInts.FIWE,
        Follow = RecordTypeInts.FLAT,
        Fallback = RecordTypeInts.FLBK,
        Flee = RecordTypeInts.FLEE,
        Reject = RecordTypeInts.FRJT,
        CustomFVDL = RecordTypeInts.FVDL,
        Goodbye = RecordTypeInts.GBYE,
        Inspect = RecordTypeInts.GIFF,
        Greeting = RecordTypeInts.GREE,
        CombatGrunt = RecordTypeInts.GRNT,
        Heal = RecordTypeInts.HEAL,
        Hello = RecordTypeInts.HELO,
        Hit = RecordTypeInts.HIT_,
        SharedInfo = RecordTypeInts.IDAT,
        Idle = RecordTypeInts.IDLE,
        ImpatientNegative = RecordTypeInts.IMNG,
        ImpatientNeutral = RecordTypeInts.IMNU,
        ImpatientPositive = RecordTypeInts.IMPT,
        ImpatientQuestion = RecordTypeInts.IMQU,
        Call = RecordTypeInts.INTI,
        Jump = RecordTypeInts.JUMP,
        KnockOverObject = RecordTypeInts.KNOO,
        LostIdle = RecordTypeInts.LOIL,
        LockedObject = RecordTypeInts.LOOB,
        LostToCombat = RecordTypeInts.LOTC,
        LostToNormal = RecordTypeInts.LOTN,
        LeaveWaterBreath = RecordTypeInts.LWBS,
        MaxCarbonDioxide = RecordTypeInts.MCO2,
        MoralRefusal = RecordTypeInts.MREF,
        MurderNC = RecordTypeInts.MUNC,
        Murder = RecordTypeInts.MURD,
        NormalToAlert = RecordTypeInts.NOTA,
        NormalToCombat = RecordTypeInts.NOTC,
        NoticeCorpse = RecordTypeInts.NOTI,
        NormalToLost = RecordTypeInts.NOTL,
        ObserveCombat = RecordTypeInts.OBCO,
        OrderMoveUp = RecordTypeInts.ORAV,
        OrderFallback = RecordTypeInts.ORFB,
        OrderFlank = RecordTypeInts.ORFL,
        OrderTakeCover = RecordTypeInts.ORTC,
        OutofBreath = RecordTypeInts.OUTB,
        PlayerActivateActivators = RecordTypeInts.PAAC,
        PlayerActivateContainer = RecordTypeInts.PACO,
        PlayerActivateDoor = RecordTypeInts.PADR,
        PlayerAquireFeaturedItem = RecordTypeInts.PAFI,
        PlayerActivateFurniture = RecordTypeInts.PAFU,
        PlayerActivateTerminals = RecordTypeInts.PATR,
        PairedAttack = RecordTypeInts.PATT,
        PlayerCastProjectileSpell = RecordTypeInts.PCPS,
        PlayerShout = RecordTypeInts.PCSH,
        PlayerCastSelfSpell = RecordTypeInts.PCSS,
        ForceGreet = RecordTypeInts.PFGT,
        PickpocketCombat = RecordTypeInts.PICC,
        PickpocketNC = RecordTypeInts.PICN,
        PickpocketTopic = RecordTypeInts.PICT,
        Piracy = RecordTypeInts.PIRA,
        PlayerinIronSights = RecordTypeInts.PIRN,
        PowerAttack = RecordTypeInts.POAT,
        PathingRefusal = RecordTypeInts.PRJT,
        PursueIdleTopic = RecordTypeInts.PURS,
        RangedAttack = RecordTypeInts.RANG,
        RechargeExit = RecordTypeInts.RCEX,
        Recharge = RecordTypeInts.RECH,
        RepairExit = RecordTypeInts.REEX,
        Refuse = RecordTypeInts.REFU,
        Reload = RecordTypeInts.RELA,
        Release = RecordTypeInts.RELE,
        Repair = RecordTypeInts.REPA,
        CoverMe = RecordTypeInts.RQST,
        Rumors = RecordTypeInts.RUMO,
        SceneChoice = RecordTypeInts.SCCH,
        ShipCrewDockingComplete = RecordTypeInts.SCDC,
        ShipCrewEnemyDockingInitiated = RecordTypeInts.SCDI,
        CustomScene = RecordTypeInts.SCEN,
        ShipCrewGravJumpCompleted = RecordTypeInts.SCGC,
        ShipCrewGravJumpCalculationComplete = RecordTypeInts.SCJC,
        ShipCrewLandingStart = RecordTypeInts.SCLS,
        ShipCrewReturnToOrbit = RecordTypeInts.SCRO,
        ShipCrewTakeOffInitiated = RecordTypeInts.SCTI,
        ShipUndockingComplete = RecordTypeInts.SCUC,
        ShipEnemyEngineDown = RecordTypeInts.SEED,
        ShipEnemyEngineRepaired = RecordTypeInts.SEER,
        ShipEnemyGravJumpDown = RecordTypeInts.SEGD,
        ShipEnemyGravJumpRepaired = RecordTypeInts.SEGR,
        ServiceRefusal = RecordTypeInts.SERU,
        ShipEnemyShieldsDown = RecordTypeInts.SESD,
        ShipEnemyShieldsRepaired = RecordTypeInts.SESR,
        ShipEnemyThrustersDown = RecordTypeInts.SETD,
        ShipEnemyThrustersRepaired = RecordTypeInts.SETR,
        ShipEnemyWeaponsDown = RecordTypeInts.SEWD,
        ShipEnemyWeaponsRepaired = RecordTypeInts.SEWR,
        SmugglingNC = RecordTypeInts.SGNC,
        Show = RecordTypeInts.SHOW,
        ShowRelationships = RecordTypeInts.SHRE,
        ElectromagneticShocked = RecordTypeInts.SHOK,
        ShipShieldsRepaired = RecordTypeInts.SHRP,
        Smuggling = RecordTypeInts.SMUG,
        ReactToWarn = RecordTypeInts.SRTW,
        ShipEnterCombat = RecordTypeInts.SSEC,
        ShipEnginesDown = RecordTypeInts.SSED,
        ShipEnginesRepaired = RecordTypeInts.SSER,
        ShipGravDriveDown = RecordTypeInts.SSGD,
        ShipGravDriveRepaired = RecordTypeInts.SSGR,
        ShipLeaveCombat = RecordTypeInts.SSLC,
        ShipShieldsDown = RecordTypeInts.SSSD,
        ShipShieldsZeroHealth = RecordTypeInts.SSSZ,
        ShipThrustersDown = RecordTypeInts.SSTD,
        ShipThrustersRepaired = RecordTypeInts.SSTR,
        ShipWeaponsDown = RecordTypeInts.SSWD,
        ShipWeaponsRepaired = RecordTypeInts.SSWR,
        Stay = RecordTypeInts.STAY,
        Steal = RecordTypeInts.STEA,
        StealTakeback = RecordTypeInts.STEB,
        StealFromNC = RecordTypeInts.STFN,
        StandonFurniture = RecordTypeInts.STOF,
        StealTakebackSuccess = RecordTypeInts.STTB,
        SwingMeleeWeapon = RecordTypeInts.SWMW,
        TakeSuppressiveFire = RecordTypeInts.TASF,
        Taunt = RecordTypeInts.TAUT,
        ThrowGrenade = RecordTypeInts.THGR,
        ThreatBackdown = RecordTypeInts.THRB,
        TimeToGo = RecordTypeInts.TITG,
        Trade = RecordTypeInts.TRAD,
        Training = RecordTypeInts.TRAI,
        TrespassAgainstNC = RecordTypeInts.TRAN,
        Travel = RecordTypeInts.TRAV,
        Trespass = RecordTypeInts.TRES,
        TrainingExit = RecordTypeInts.TREX,
        TresspassNoVisual = RecordTypeInts.TRNV,
        UseMagic = RecordTypeInts.USMG,
        VehiclePassengerAimMountedWeapon = RecordTypeInts.VPAW,
        VehiclePassengerEngageBoost = RecordTypeInts.VPEB,
        VoicePowerEndLong = RecordTypeInts.VPEL,
        VoicePowerEndShort = RecordTypeInts.VPES,
        VehiclePassengerEnterVehicle = RecordTypeInts.VPEV,
        VehiclePassengerEnterWater = RecordTypeInts.VPEW,
        VehiclePassengerHorizontalCollision = RecordTypeInts.VPHC,
        VehiclePassengerVerticalCollisionLowVelocity = RecordTypeInts.VPLV,
        VehiclePassengerKillWithCollision = RecordTypeInts.VPKC,
        VehiclePassengerKillWithWeapon = RecordTypeInts.VPKW,
        VehiclePassengerNonMovingIdle = RecordTypeInts.VPNM,
        VoicePowerStartLong = RecordTypeInts.VPSL,
        VoicePowerStartShort = RecordTypeInts.VPSS,
        VehiclePassengerTriggerMine = RecordTypeInts.VPTM,
        VehiclePassengerVerticalCollision = RecordTypeInts.VPVC,
        WaitingForPlayerInput = RecordTypeInts.WFPI,
        Curious = RecordTypeInts.WTCR,
        Yield = RecordTypeInts.YIEL,
        ZKeyObject = RecordTypeInts.ZKEY,
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
                    RecordException.EnrichAndThrow(
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

    public static partial ParseResult FillBinaryInfoListCountCustom(MutagenFrame frame, IDialogTopicInternal item, PreviousParse lastParsed)
    {
        frame.ReadSubrecord(RecordTypes.TIFC);
        return null;
    }
}

partial class DialogTopicBinaryWriteTranslation
{
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

    public static partial void WriteBinaryInfoListCountCustom(MutagenWriter writer, IDialogTopicGetter item)
    {
        var list = item.TopicInfoList;
        if (list == null) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.TIFC))
        {
            writer.Write(list.Count);
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
                RecordException.EnrichAndThrow(
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

    public partial ParseResult InfoListCountCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        stream.ReadSubrecord(RecordTypes.TIFC);
        return null;
    }
}