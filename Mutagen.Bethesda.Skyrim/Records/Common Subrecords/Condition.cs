using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Skyrim.Internals;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;
using System.Buffers.Binary;
using System.Diagnostics;
using static Mutagen.Bethesda.Skyrim.Condition;

namespace Mutagen.Bethesda.Skyrim;

public partial class Condition
{
    public ConditionData Data { get; set; } = null!;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IConditionDataGetter IConditionGetter.Data => this.Data;
    
    internal const int ParametersUseAliases = 0x02;
    internal const int UseGlobal = 0x04;
    internal const int ParametersUsePackData = 0x08;

    /// <summary>
    /// ParametersUseAliases and ParametersUsePackData exist on ConditionData object instead </ br>
    /// UseGlobal is implicit depending on the class type used for the Condition
    /// </summary>
    [Flags]
    public enum Flag
    {
        OR = 0x01,
        // ParametersUseAliases = 0x02,
        // UseGlobal = 0x04,
        // UsePackData = 0x08,
        SwapSubjectAndTarget = 0x10
    }

    public enum RunOnType
    {
        Subject = 0,
        Target = 1,
        Reference = 2,
        CombatTarget = 3,
        LinkedReference = 4,
        QuestAlias = 5,
        PackageData = 6,
        EventData = 7,
    }
    
    public enum ParameterType
    {
        None,
        Actor,
        ActorBase,
        ActorValue,
        AdvanceAction,
        Alias,
        Alignment,
        AssociationType,
        Axis,
        CastingSource,
        Cell,
        Class,
        CrimeType,
        CriticalStage,
        EncounterZone,
        EquipType,
        Event,
        EventData,
        Faction,
        Float,
        FormList,
        FormType,
        Furniture,
        FurnitureAnim,
        FurnitureEntry,
        Global,
        IdleForm,
        Integer,
        InventoryObject,
        Keyword,
        Knowable,
        Location,
        MagicEffect,
        MagicItem,
        MiscStat,
        ObjectReference,
        Owner,
        Package,
        Packdata,
        Perk,
        Quest,
        QuestStage,
        Race,
        ReferencableObject,
        RefType,
        Region,
        Scene,
        Sex,
        Shout,
        VariableName,
        VATSValueFunction,
        VATSValueParam,
        VoiceType,
        WardState,
        Weather,
        Worldspace,
    }
    
    public enum Function
    {
        GetWantBlocking = 0,
        GetDistance = 1,
        GetLocked = 5,
        GetPos = 6,
        GetAngle = 8,
        GetStartingPos = 10,
        GetStartingAngle = 11,
        GetSecondsPassed = 12,
        GetActorValue = 14,
        GetCurrentTime = 18,
        GetScale = 24,
        IsMoving = 25,
        IsTurning = 26,
        GetLineOfSight = 27,
        GetInSameCell = 32,
        GetDisabled = 35,
        MenuMode = 36,
        GetDisease = 39,
        GetClothingValue = 41,
        SameFaction = 42,
        SameRace = 43,
        SameSex = 44,
        GetDetected = 45,
        GetDead = 46,
        GetItemCount = 47,
        GetGold = 48,
        GetSleeping = 49,
        GetTalkedToPC = 50,
        GetScriptVariable = 53,
        GetQuestRunning = 56,
        GetStage = 58,
        GetStageDone = 59,
        GetFactionRankDifference = 60,
        GetAlarmed = 61,
        IsRaining = 62,
        GetAttacked = 63,
        GetIsCreature = 64,
        GetLockLevel = 65,
        GetShouldAttack = 66,
        GetInCell = 67,
        GetIsClass = 68,
        GetIsRace = 69,
        GetIsSex = 70,
        GetInFaction = 71,
        GetIsID = 72,
        GetFactionRank = 73,
        GetGlobalValue = 74,
        IsSnowing = 75,
        GetRandomPercent = 77,
        GetQuestVariable = 79,
        GetLevel = 80,
        IsRotating = 81,
        GetDeadCount = 84,
        GetIsAlerted = 91,
        GetPlayerControlsDisabled = 98,
        GetHeadingAngle = 99,
        IsWeaponMagicOut = 101,
        IsTorchOut = 102,
        IsShieldOut = 103,
        IsFacingUp = 106,
        GetKnockedState = 107,
        GetWeaponAnimType = 108,
        IsWeaponSkillType = 109,
        GetCurrentAIPackage = 110,
        IsWaiting = 111,
        IsIdlePlaying = 112,
        IsIntimidatedbyPlayer = 116,
        IsPlayerInRegion = 117,
        GetActorAggroRadiusViolated = 118,
        GetCrime = 122,
        IsGreetingPlayer = 123,
        IsGuard = 125,
        HasBeenEaten = 127,
        GetStaminaPercentage = 128,
        GetPCIsClass = 129,
        GetPCIsRace = 130,
        GetPCIsSex = 131,
        GetPCInFaction = 132,
        SameFactionAsPC = 133,
        SameRaceAsPC = 134,
        SameSexAsPC = 135,
        GetIsReference = 136,
        IsTalking = 141,
        GetWalkSpeed = 142,
        GetCurrentAIProcedure = 143,
        GetTrespassWarningLevel = 144,
        IsTrespassing = 145,
        IsInMyOwnedCell = 146,
        GetWindSpeed = 147,
        GetCurrentWeatherPercent = 148,
        GetIsCurrentWeather = 149,
        IsContinuingPackagePCNear = 150,
        GetIsCrimeFaction = 152,
        CanHaveFlames = 153,
        HasFlames = 154,
        GetOpenState = 157,
        GetSitting = 159,
        GetIsCurrentPackage = 161,
        IsCurrentFurnitureRef = 162,
        IsCurrentFurnitureObj = 163,
        GetDayOfWeek = 170,
        GetTalkedToPCParam = 172,
        IsPCSleeping = 175,
        IsPCAMurderer = 176,
        HasSameEditorLocAsRef = 180,
        HasSameEditorLocAsRefAlias = 181,
        GetEquipped = 182,
        IsSwimming = 185,
        GetAmountSoldStolen = 190,
        GetIgnoreCrime = 192,
        GetPCExpelled = 193,
        GetPCFactionMurder = 195,
        GetPCEnemyofFaction = 197,
        GetPCFactionAttack = 199,
        GetDestroyed = 203,
        HasMagicEffect = 214,
        GetDefaultOpen = 215,
        GetAnimAction = 219,
        IsSpellTarget = 223,
        GetVATSMode = 224,
        GetPersuasionNumber = 225,
        GetVampireFeed = 226,
        GetCannibal = 227,
        GetIsClassDefault = 228,
        GetClassDefaultMatch = 229,
        GetInCellParam = 230,
        GetVatsTargetHeight = 235,
        GetIsGhost = 237,
        GetUnconscious = 242,
        GetRestrained = 244,
        GetIsUsedItem = 246,
        GetIsUsedItemType = 247,
        IsScenePlaying = 248,
        IsInDialogueWithPlayer = 249,
        GetLocationCleared = 250,
        GetIsPlayableRace = 254,
        GetOffersServicesNow = 255,
        HasAssociationType = 258,
        HasFamilyRelationship = 259,
        HasParentRelationship = 261,
        IsWarningAbout = 262,
        IsWeaponOut = 263,
        HasSpell = 264,
        IsTimePassing = 265,
        IsPleasant = 266,
        IsCloudy = 267,
        IsSmallBump = 274,
        GetBaseActorValue = 277,
        IsOwner = 278,
        IsCellOwner = 280,
        IsHorseStolen = 282,
        IsLeftUp = 285,
        IsSneaking = 286,
        IsRunning = 287,
        GetFriendHit = 288,
        IsInCombat = 289,
        IsInInterior = 300,
        IsWaterObject = 304,
        GetPlayerAction = 305,
        IsActorUsingATorch = 306,
        IsXBox = 309,
        GetInWorldspace = 310,
        GetPCMiscStat = 312,
        GetPairedAnimation = 313,
        IsActorAVictim = 314,
        GetTotalPersuasionNumber = 315,
        GetIdleDoneOnce = 318,
        GetNoRumors = 320,
        GetCombatState = 323,
        GetWithinPackageLocation = 325,
        IsRidingMount = 327,
        IsFleeing = 329,
        IsInDangerousWater = 332,
        GetIgnoreFriendlyHits = 338,
        IsPlayersLastRiddenMount = 339,
        IsActor = 353,
        IsEssential = 354,
        IsPlayerMovingIntoNewSpace = 358,
        GetInCurrentLoc = 359,
        GetInCurrentLocAlias = 360,
        GetTimeDead = 361,
        HasLinkedRef = 362,
        IsChild = 365,
        GetStolenItemValueNoCrime = 366,
        GetLastPlayerAction = 367,
        IsPlayerActionActive = 368,
        IsTalkingActivatorActor = 370,
        IsInList = 372,
        GetStolenItemValue = 373,
        GetCrimeGoldViolent = 375,
        GetCrimeGoldNonviolent = 376,
        HasShout = 378,
        GetHasNote = 381,
        GetHitLocation = 390,
        IsPC1stPerson = 391,
        GetCauseofDeath = 396,
        IsLimbGone = 397,
        IsWeaponInList = 398,
        IsBribedbyPlayer = 402,
        GetRelationshipRank = 403,
        GetVATSValue = 407,
        IsKiller = 408,
        IsKillerObject = 409,
        GetFactionCombatReaction = 410,
        Exists = 414,
        GetGroupMemberCount = 415,
        GetGroupTargetCount = 416,
        GetIsVoiceType = 426,
        GetPlantedExplosive = 427,
        IsScenePackageRunning = 429,
        GetHealthPercentage = 430,
        GetIsObjectType = 432,
        GetDialogueEmotion = 434,
        GetDialogueEmotionValue = 435,
        GetIsCreatureType = 437,
        GetInCurrentLocFormList = 444,
        GetInZone = 445,
        GetVelocity = 446,
        GetGraphVariableFloat = 447,
        HasPerk = 448,
        GetFactionRelation = 449,
        IsLastIdlePlayed = 450,
        GetPlayerTeammate = 453,
        GetPlayerTeammateCount = 454,
        GetActorCrimePlayerEnemy = 458,
        GetCrimeGold = 459,
        IsPlayerGrabbedRef = 463,
        GetKeywordItemCount = 465,
        GetDestructionStage = 470,
        GetIsAlignment = 473,
        IsProtected = 476,
        GetThreatRatio = 477,
        GetIsUsedItemEquipType = 479,
        IsCarryable = 487,
        GetConcussed = 488,
        GetMapMarkerVisible = 491,
        PlayerKnows = 493,
        GetPermanentActorValue = 494,
        GetKillingBlowLimb = 495,
        CanPayCrimeGold = 497,
        GetDaysInJail = 499,
        EPAlchemyGetMakingPoison = 500,
        EPAlchemyEffectHasKeyword = 501,
        GetAllowWorldInteractions = 503,
        GetLastHitCritical = 508,
        IsCombatTarget = 513,
        GetVATSRightAreaFree = 515,
        GetVATSLeftAreaFree = 516,
        GetVATSBackAreaFree = 517,
        GetVATSFrontAreaFree = 518,
        GetLockIsBroken = 519,
        IsPS3 = 520,
        IsWin32 = 521,
        GetVATSRightTargetVisible = 522,
        GetVATSLeftTargetVisible = 523,
        GetVATSBackTargetVisible = 524,
        GetVATSFrontTargetVisible = 525,
        IsInCriticalStage = 528,
        GetXPForNextLevel = 530,
        GetInfamy = 533,
        GetInfamyViolent = 534,
        GetInfamyNonViolent = 535,
        GetQuestCompleted = 543,
        IsGoreDisabled = 547,
        IsSceneActionComplete = 550,
        GetSpellUsageNum = 552,
        GetActorsInHigh = 554,
        HasLoaded3D = 555,
        HasKeyword = 560,
        HasRefType = 561,
        LocationHasKeyword = 562,
        LocationHasRefType = 563,
        GetIsEditorLocation = 565,
        GetIsAliasRef = 566,
        GetIsEditorLocAlias = 567,
        IsSprinting = 568,
        IsBlocking = 569,
        HasEquippedSpell = 570,
        GetCurrentCastingType = 571,
        GetCurrentDeliveryType = 572,
        GetAttackState = 574,
        GetEventData = 576,
        IsCloserToAThanB = 577,
        GetEquippedShout = 579,
        IsBleedingOut = 580,
        GetRelativeAngle = 584,
        GetMovementDirection = 589,
        IsInScene = 590,
        GetRefTypeDeadCount = 591,
        GetRefTypeAliveCount = 592,
        GetIsFlying = 594,
        IsCurrentSpell = 595,
        SpellHasKeyword = 596,
        GetEquippedItemType = 597,
        GetLocationAliasCleared = 598,
        GetLocAliasRefTypeDeadCount = 600,
        GetLocAliasRefTypeAliveCount = 601,
        IsWardState = 602,
        IsInSameCurrentLocAsRef = 603,
        IsInSameCurrentLocAsRefAlias = 604,
        LocAliasIsLocation = 605,
        GetKeywordDataForLocation = 606,
        GetKeywordDataForAlias = 608,
        LocAliasHasKeyword = 610,
        IsNullPackageData = 611,
        GetNumericPackageData = 612,
        IsFurnitureAnimType = 613,
        IsFurnitureEntryType = 614,
        GetHighestRelationshipRank = 615,
        GetLowestRelationshipRank = 616,
        HasAssociationTypeAny = 617,
        HasFamilyRelationshipAny = 618,
        GetPathingTargetOffset = 619,
        GetPathingTargetAngleOffset = 620,
        GetPathingTargetSpeed = 621,
        GetPathingTargetSpeedAngle = 622,
        GetMovementSpeed = 623,
        GetInContainer = 624,
        IsLocationLoaded = 625,
        IsLocAliasLoaded = 626,
        IsDualCasting = 627,
        GetVMQuestVariable = 629,
        GetVMScriptVariable = 630,
        IsEnteringInteractionQuick = 631,
        IsCasting = 632,
        GetFlyingState = 633,
        IsInFavorState = 635,
        HasTwoHandedWeaponEquipped = 636,
        IsExitingInstant = 637,
        IsInFriendStateWithPlayer = 638,
        GetWithinDistance = 639,
        GetActorValuePercent = 640,
        IsUnique = 641,
        GetLastBumpDirection = 642,
        IsInFurnitureState = 644,
        GetIsInjured = 645,
        GetIsCrashLandRequest = 646,
        GetIsHastyLandRequest = 647,
        IsLinkedTo = 650,
        GetKeywordDataForCurrentLocation = 651,
        GetInSharedCrimeFaction = 652,
        GetBribeSuccess = 654,
        GetIntimidateSuccess = 655,
        GetArrestedState = 656,
        GetArrestingActor = 657,
        EPTemperingItemIsEnchanted = 659,
        EPTemperingItemHasKeyword = 660,
        GetReplacedItemType = 664,
        IsAttacking = 672,
        IsPowerAttacking = 673,
        IsLastHostileActor = 674,
        GetGraphVariableInt = 675,
        GetCurrentShoutVariation = 676,
        ShouldAttackKill = 678,
        GetActivatorHeight = 680,
        EPMagic_IsAdvanceSkill = 681,
        WornHasKeyword = 682,
        GetPathingCurrentSpeed = 683,
        GetPathingCurrentSpeedAngle = 684,
        EPModSkillUsage_AdvanceObjectHasKeyword = 691,
        EPModSkillUsage_IsAdvanceAction = 692,
        EPMagic_SpellHasKeyword = 693,
        GetNoBleedoutRecovery = 694,
        EPMagic_SpellHasSkill = 696,
        IsAttackType = 697,
        IsAllowedToFly = 698,
        HasMagicEffectKeyword = 699,
        IsCommandedActor = 700,
        IsStaggered = 701,
        IsRecoiling = 702,
        IsExitingInteractionQuick = 703,
        IsPathing = 704,
        GetShouldHelp = 705,
        HasBoundWeaponEquipped = 706,
        GetCombatTargetHasKeyword = 707,
        GetCombatGroupMemberCount = 709,
        IsIgnoringCombat = 710,
        GetLightLevel = 711,
        SpellHasCastingPerk = 713,
        IsBeingRidden = 714,
        IsUndead = 715,
        GetRealHoursPassed = 716,
        IsUnlockedDoor = 718,
        IsHostileToActor = 719,
        GetTargetHeight = 720,
        IsPoison = 721,
        WornApparelHasKeywordCount = 722,
        GetItemHealthPercent = 723,
        EffectWasDualCast = 724,
        GetKnockedStateEnum = 725,
        DoesNotExist = 726,
        IsOnFlyingMount = 730,
        CanFlyHere = 731,
        IsFlyingMountPatrolQueud = 732,
        IsFlyingMountFastTravelling = 733,
        IsOverEncumbered = 734,
        GetActorWarmth = 735,
        GetSKSEVersion = 1024,
        GetSKSEVersionMinor = 1025,
        GetSKSEVersionBeta = 1026,
        GetSKSERelease = 1027,
        ClearInvalidRegistrations = 1028,
    }

    public enum ParameterCategory
    {
        None,
        Number,
        Form,
        String
    }

    public static (ParameterType First, ParameterType Second) GetParameterTypes(Function function)
    {
        return ((ushort)function) switch
        {
            1 => (ParameterType.ObjectReference, ParameterType.None),
            6 => (ParameterType.Axis, ParameterType.None),
            8 => (ParameterType.Axis, ParameterType.None),
            10 => (ParameterType.Axis, ParameterType.None),
            11 => (ParameterType.Axis, ParameterType.None),
            14 => (ParameterType.ActorValue, ParameterType.None),
            27 => (ParameterType.ObjectReference, ParameterType.None),
            32 => (ParameterType.ObjectReference, ParameterType.None),
            36 => (ParameterType.Integer, ParameterType.None),
            42 => (ParameterType.Actor, ParameterType.None),
            43 => (ParameterType.Actor, ParameterType.None),
            44 => (ParameterType.Actor, ParameterType.None),
            45 => (ParameterType.Actor, ParameterType.None),
            47 => (ParameterType.InventoryObject, ParameterType.None),
            53 => (ParameterType.ObjectReference, ParameterType.VariableName),
            56 => (ParameterType.Quest, ParameterType.None),
            58 => (ParameterType.Quest, ParameterType.None),
            59 => (ParameterType.Quest, ParameterType.QuestStage),
            60 => (ParameterType.Faction, ParameterType.Actor),
            66 => (ParameterType.Actor, ParameterType.None),
            67 => (ParameterType.Cell, ParameterType.None),
            68 => (ParameterType.Class, ParameterType.None),
            69 => (ParameterType.Race, ParameterType.None),
            70 => (ParameterType.Sex, ParameterType.None),
            71 => (ParameterType.Faction, ParameterType.None),
            72 => (ParameterType.ReferencableObject, ParameterType.None),
            73 => (ParameterType.Faction, ParameterType.None),
            74 => (ParameterType.Global, ParameterType.None),
            79 => (ParameterType.Quest, ParameterType.VariableName),
            84 => (ParameterType.ActorBase, ParameterType.None),
            98 => (ParameterType.Integer, ParameterType.Integer),
            99 => (ParameterType.ObjectReference, ParameterType.None),
            109 => (ParameterType.ActorValue, ParameterType.None),
            117 => (ParameterType.Region, ParameterType.None),
            122 => (ParameterType.Actor, ParameterType.CrimeType),
            129 => (ParameterType.Class, ParameterType.None),
            130 => (ParameterType.Race, ParameterType.None),
            131 => (ParameterType.Sex, ParameterType.None),
            132 => (ParameterType.Faction, ParameterType.None),
            136 => (ParameterType.ObjectReference, ParameterType.None),
            149 => (ParameterType.Weather, ParameterType.None),
            152 => (ParameterType.Faction, ParameterType.None),
            161 => (ParameterType.Package, ParameterType.None),
            162 => (ParameterType.ObjectReference, ParameterType.None),
            163 => (ParameterType.Furniture, ParameterType.None),
            172 => (ParameterType.Actor, ParameterType.None),
            180 => (ParameterType.ObjectReference, ParameterType.Keyword),
            181 => (ParameterType.Alias, ParameterType.Keyword),
            182 => (ParameterType.InventoryObject, ParameterType.None),
            193 => (ParameterType.Faction, ParameterType.None),
            195 => (ParameterType.Faction, ParameterType.None),
            197 => (ParameterType.Faction, ParameterType.None),
            199 => (ParameterType.Faction, ParameterType.None),
            214 => (ParameterType.MagicEffect, ParameterType.None),
            223 => (ParameterType.MagicItem, ParameterType.None),
            228 => (ParameterType.Class, ParameterType.None),
            230 => (ParameterType.Cell, ParameterType.ObjectReference),
            246 => (ParameterType.ReferencableObject, ParameterType.None),
            247 => (ParameterType.FormType, ParameterType.None),
            248 => (ParameterType.Scene, ParameterType.None),
            250 => (ParameterType.Location, ParameterType.None),
            258 => (ParameterType.Actor, ParameterType.AssociationType),
            259 => (ParameterType.Actor, ParameterType.None),
            261 => (ParameterType.Actor, ParameterType.None),
            262 => (ParameterType.FormList, ParameterType.None),
            264 => (ParameterType.MagicItem, ParameterType.None),
            277 => (ParameterType.ActorValue, ParameterType.None),
            278 => (ParameterType.Owner, ParameterType.None),
            280 => (ParameterType.Cell, ParameterType.Owner),
            289 => (ParameterType.Integer, ParameterType.None),
            310 => (WorldSpace: ParameterType.Worldspace, ParameterType.None),
            312 => (ParameterType.MiscStat, ParameterType.None),
            325 => (ParameterType.Packdata, ParameterType.None),
            359 => (ParameterType.Location, ParameterType.None),
            360 => (ParameterType.Alias, ParameterType.None),
            362 => (ParameterType.Keyword, ParameterType.None),
            366 => (ParameterType.Faction, ParameterType.None),
            368 => (ParameterType.Integer, ParameterType.None),
            370 => (ParameterType.Actor, ParameterType.None),
            372 => (ParameterType.FormList, ParameterType.None),
            373 => (ParameterType.Faction, ParameterType.None),
            375 => (ParameterType.Faction, ParameterType.None),
            376 => (ParameterType.Faction, ParameterType.None),
            378 => (ParameterType.Shout, ParameterType.None),
            381 => (ParameterType.Integer, ParameterType.None),
            397 => (ParameterType.Integer, ParameterType.None),
            398 => (ParameterType.FormList, ParameterType.None),
            403 => (ParameterType.ObjectReference, ParameterType.None),
            407 => (ParameterType.VATSValueFunction, ParameterType.VATSValueParam),
            408 => (ParameterType.Actor, ParameterType.None),
            409 => (ParameterType.FormList, ParameterType.None),
            410 => (ParameterType.Faction, ParameterType.Faction),
            414 => (ParameterType.ObjectReference, ParameterType.None),
            426 => (ParameterType.VoiceType, ParameterType.None),
            432 => (ParameterType.FormType, ParameterType.None),
            437 => (ParameterType.Integer, ParameterType.None),
            444 => (ParameterType.FormList, ParameterType.None),
            445 => (ParameterType.EncounterZone, ParameterType.None),
            446 => (ParameterType.Axis, ParameterType.None),
            447 => (ParameterType.VariableName, ParameterType.None),
            448 => (ParameterType.Perk, ParameterType.Integer),
            449 => (ParameterType.Actor, ParameterType.None),
            450 => (ParameterType.IdleForm, ParameterType.None),
            459 => (ParameterType.Faction, ParameterType.None),
            463 => (ParameterType.ObjectReference, ParameterType.None),
            465 => (ParameterType.Keyword, ParameterType.None),
            473 => (ParameterType.Alignment, ParameterType.None),
            477 => (ParameterType.Actor, ParameterType.None),
            479 => (ParameterType.EquipType, ParameterType.None),
            493 => (ParameterType.Knowable, ParameterType.None),
            494 => (ParameterType.ActorValue, ParameterType.None),
            501 => (ParameterType.Keyword, ParameterType.None),
            513 => (ParameterType.Actor, ParameterType.None),
            515 => (ParameterType.ObjectReference, ParameterType.None),
            516 => (ParameterType.ObjectReference, ParameterType.None),
            517 => (ParameterType.ObjectReference, ParameterType.None),
            518 => (ParameterType.ObjectReference, ParameterType.None),
            522 => (ParameterType.ObjectReference, ParameterType.None),
            523 => (ParameterType.ObjectReference, ParameterType.None),
            524 => (ParameterType.ObjectReference, ParameterType.None),
            525 => (ParameterType.ObjectReference, ParameterType.None),
            528 => (ParameterType.CriticalStage, ParameterType.None),
            533 => (ParameterType.Faction, ParameterType.None),
            534 => (ParameterType.Faction, ParameterType.None),
            535 => (ParameterType.Faction, ParameterType.None),
            543 => (ParameterType.Quest, ParameterType.None),
            550 => (ParameterType.Scene, ParameterType.Integer),
            552 => (ParameterType.MagicItem, ParameterType.None),
            560 => (ParameterType.Keyword, ParameterType.None),
            561 => (ParameterType.RefType, ParameterType.None),
            562 => (ParameterType.Keyword, ParameterType.None),
            563 => (ParameterType.RefType, ParameterType.None),
            565 => (ParameterType.Location, ParameterType.None),
            566 => (ParameterType.Alias, ParameterType.None),
            567 => (ParameterType.Alias, ParameterType.None),
            570 => (ParameterType.CastingSource, ParameterType.None),
            571 => (ParameterType.CastingSource, ParameterType.None),
            572 => (ParameterType.CastingSource, ParameterType.None),
            576 => (ParameterType.Event, ParameterType.EventData),
            577 => (ParameterType.ObjectReference, ParameterType.ObjectReference),
            579 => (ParameterType.Shout, ParameterType.None),
            584 => (ParameterType.ObjectReference, ParameterType.Axis),
            591 => (ParameterType.Location, ParameterType.RefType),
            592 => (ParameterType.Location, ParameterType.RefType),
            595 => (ParameterType.MagicItem, ParameterType.CastingSource),
            596 => (ParameterType.CastingSource, ParameterType.Keyword),
            597 => (ParameterType.CastingSource, ParameterType.None),
            598 => (ParameterType.Alias, ParameterType.None),
            600 => (ParameterType.Alias, ParameterType.RefType),
            601 => (ParameterType.Alias, ParameterType.RefType),
            602 => (ParameterType.WardState, ParameterType.None),
            603 => (ParameterType.ObjectReference, ParameterType.Keyword),
            604 => (ParameterType.Alias, ParameterType.Keyword),
            605 => (ParameterType.Alias, ParameterType.Location),
            606 => (ParameterType.Location, ParameterType.Keyword),
            608 => (ParameterType.Alias, ParameterType.Keyword),
            610 => (ParameterType.Alias, ParameterType.Keyword),
            611 => (ParameterType.Packdata, ParameterType.None),
            612 => (ParameterType.Integer, ParameterType.None),
            613 => (ParameterType.FurnitureAnim, ParameterType.None),
            614 => (ParameterType.FurnitureEntry, ParameterType.None),
            617 => (ParameterType.AssociationType, ParameterType.None),
            619 => (ParameterType.Axis, ParameterType.None),
            620 => (ParameterType.Axis, ParameterType.None),
            622 => (ParameterType.Axis, ParameterType.None),
            624 => (ParameterType.ObjectReference, ParameterType.None),
            625 => (ParameterType.Location, ParameterType.None),
            626 => (ParameterType.Alias, ParameterType.None),
            629 => (ParameterType.Quest, ParameterType.VariableName),
            630 => (ParameterType.ObjectReference, ParameterType.VariableName),
            639 => (ParameterType.ObjectReference, ParameterType.Float),
            640 => (ParameterType.ActorValue, ParameterType.None),
            644 => (ParameterType.FurnitureAnim, ParameterType.None),
            650 => (ParameterType.ObjectReference, ParameterType.Keyword),
            651 => (ParameterType.Keyword, ParameterType.None),
            652 => (ParameterType.ObjectReference, ParameterType.None),
            660 => (ParameterType.Keyword, ParameterType.None),
            664 => (ParameterType.CastingSource, ParameterType.None),
            675 => (ParameterType.VariableName, ParameterType.None),
            678 => (ParameterType.Actor, ParameterType.None),
            681 => (ParameterType.ActorValue, ParameterType.None),
            682 => (ParameterType.Keyword, ParameterType.None),
            684 => (ParameterType.Axis, ParameterType.None),
            691 => (ParameterType.Keyword, ParameterType.None),
            692 => (ParameterType.AdvanceAction, ParameterType.None),
            693 => (ParameterType.Keyword, ParameterType.None),
            696 => (ParameterType.ActorValue, ParameterType.None),
            697 => (ParameterType.Keyword, ParameterType.None),
            699 => (ParameterType.Keyword, ParameterType.None),
            705 => (ParameterType.Actor, ParameterType.None),
            706 => (ParameterType.CastingSource, ParameterType.None),
            707 => (ParameterType.Keyword, ParameterType.None),
            713 => (ParameterType.Perk, ParameterType.None),
            719 => (ParameterType.Actor, ParameterType.None),
            720 => (ParameterType.ObjectReference, ParameterType.None),
            722 => (ParameterType.Keyword, ParameterType.None),
            _ => (ParameterType.None, ParameterType.None),
        };
    }

    public static Condition CreateFromBinary(
        MutagenFrame frame,
        TypedParseParams translationParams)
    {
        if (!frame.Reader.TryGetSubrecordHeader(Mutagen.Bethesda.Skyrim.Internals.RecordTypes.CTDA, out var subRecMeta))
        {
            throw new ArgumentException();
        }
        var flagByte = frame.GetUInt8(subRecMeta.HeaderLength);
        Condition.Flag flag = ConditionBinaryCreateTranslation.GetFlag(flagByte);
        Condition ret;
        if (flag.HasFlag((Condition.Flag)Condition.UseGlobal))
        {
            ret = ConditionGlobal.CreateFromBinary(frame.SpawnWithLength(subRecMeta.ContentLength, checkFraming: false));
            ret.Flags = ret.Flags.SetFlag((Condition.Flag)UseGlobal, false);
        }
        else
        {
            ret = ConditionFloat.CreateFromBinary(frame.SpawnWithLength(subRecMeta.ContentLength, checkFraming: false));
        }

        ret.Data.UseAliases = flag.HasFlag((Condition.Flag)ParametersUseAliases);
        ret.Data.UsePackageData = flag.HasFlag((Condition.Flag)ParametersUsePackData);
        ret.Flags = ret.Flags.SetFlag((Condition.Flag)ParametersUseAliases, false);
        ret.Flags = ret.Flags.SetFlag((Condition.Flag)ParametersUsePackData, false);
        return ret;
    }

    public static bool TryCreateFromBinary(
        MutagenFrame frame,
        out Condition condition,
        TypedParseParams translationParams)
    {
        condition = CreateFromBinary(frame, translationParams);
        return true;
    }
}

public static class ParameterTypeMixIn
{
    public static Condition.ParameterCategory GetCategory(this Condition.ParameterType type)
    {
        switch (type)
        {
            case ParameterType.None:
                return ParameterCategory.None;
            case ParameterType.Integer:
            case ParameterType.Float:
            case ParameterType.VariableName:
            case ParameterType.Sex:
            case ParameterType.ActorValue:
            case ParameterType.CrimeType:
            case ParameterType.Axis:
            case ParameterType.QuestStage:
            case ParameterType.MiscStat:
            case ParameterType.Alignment:
            case ParameterType.EquipType:
            case ParameterType.FormType:
            case ParameterType.CriticalStage:
            case ParameterType.VATSValueFunction:
            case ParameterType.VATSValueParam:
            case ParameterType.AdvanceAction:
            case ParameterType.CastingSource:
            case ParameterType.Alias:
            case ParameterType.Packdata:
            case ParameterType.FurnitureAnim:
            case ParameterType.FurnitureEntry:
            case ParameterType.WardState:
            case ParameterType.Event:
                return ParameterCategory.Number;
            case ParameterType.ObjectReference:
            case ParameterType.InventoryObject:
            case ParameterType.Actor:
            case ParameterType.VoiceType:
            case ParameterType.IdleForm:
            case ParameterType.FormList:
            case ParameterType.Quest:
            case ParameterType.Faction:
            case ParameterType.Cell:
            case ParameterType.Class:
            case ParameterType.Race:
            case ParameterType.ActorBase:
            case ParameterType.Global:
            case ParameterType.Weather:
            case ParameterType.Package:
            case ParameterType.EncounterZone:
            case ParameterType.Perk:
            case ParameterType.Owner:
            case ParameterType.Furniture:
            case ParameterType.MagicItem:
            case ParameterType.MagicEffect:
            case ParameterType.Worldspace:
            case ParameterType.ReferencableObject:
            case ParameterType.Region:
            case ParameterType.Keyword:
            case ParameterType.Shout:
            case ParameterType.Location:
            case ParameterType.RefType:
            case ParameterType.AssociationType:
            case ParameterType.Scene:
            case ParameterType.EventData:
            case ParameterType.Knowable:
                return ParameterCategory.Form;
            default:
                throw new NotImplementedException();
        }
    }

}

partial class ConditionBinaryCreateTranslation
{
    public const byte CompareMask = 0xE0;
    public const byte FlagMask = 0x1F;
    public const int EventFunctionIndex = 4672;

    public static Condition.Flag GetFlag(byte b)
    {
        return (Condition.Flag)(FlagMask & b);
    }

    public static CompareOperator GetCompareOperator(byte b)
    {
        return (CompareOperator)((CompareMask & b) >> 5);
    }

    public static void FillConditionsList(IList<Condition> conditions, MutagenFrame frame, int count)
    {
        conditions.Clear();
        for (int i = 0; i < count; i++)
        {
            conditions.Add(Condition.CreateFromBinary(frame, default(TypedParseParams)));
        }
    }

    public static void FillConditionsList(IList<Condition> conditions, MutagenFrame frame)
    {
        conditions.Clear();
        while (frame.Reader.TryGetSubrecordHeader(RecordTypes.CTDA, out var subMeta))
        {
            conditions.Add(Condition.CreateFromBinary(frame, default(TypedParseParams)));
        }
    }

    public static partial void FillBinaryFlagsCustom(MutagenFrame frame, ICondition item)
    {
        byte b = frame.ReadUInt8();
        item.Flags = GetFlag(b);
        item.CompareOperator = GetCompareOperator(b);
    }

    private static void ParseString<TStream>(TStream frame, IConditionData funcData)
        where TStream : IMutagenReadStream
    {
        if (funcData is not IConditionParameters parameters) return;
        if (!frame.TryGetSubrecord(out var subMeta)) return;
        switch (subMeta.RecordType.TypeInt)
        {
            case RecordTypeInts.CIS1:
                parameters.StringParameter1 = BinaryStringUtility.ProcessWholeToZString(subMeta.Content, frame.MetaData.Encodings.NonTranslated);
                break;
            case RecordTypeInts.CIS2:
                parameters.StringParameter2 = BinaryStringUtility.ProcessWholeToZString(subMeta.Content, frame.MetaData.Encodings.NonTranslated);
                break;
            default:
                return;
        }
        frame.Position += subMeta.TotalLength;
    }

    public static void CustomStringImports<TStream>(TStream frame, IConditionData item)
        where TStream : IMutagenReadStream
    {
        if (item is not IConditionData funcData) return;
        ParseString(frame, funcData);
        ParseString(frame, funcData);
    }

    public static ConditionData CreateDataFromBinary(MutagenFrame frame, ushort functionIndex)
    {
        var ret = CreateDataFromBinaryInternal(frame, functionIndex);
        if (ret == null)
        {
            var unknown = UnknownConditionData.CreateFromBinary(frame);
            unknown.Function = (Function)functionIndex;
            ret = unknown;
        }
        FillEndingParams(frame, ret);
        return ret;
    }
    
    public static void FillEndingParams(MutagenFrame frame, IConditionData item)
    {
        item.RunOnType = EnumBinaryTranslation<Condition.RunOnType, MutagenFrame, MutagenWriter>.Instance.Parse(reader: frame.SpawnWithLength(4));
        item.Reference.SetTo(
            FormLinkBinaryTranslation.Instance.Parse(
                reader: frame,
                defaultVal: FormKey.Null));
        item.RunOnTypeIndex = frame.ReadInt32();
    }
    
    public static ConditionData? CreateDataFromBinaryInternal(MutagenFrame frame, ushort functionIndex)
    {
        switch (functionIndex)
        {
            case 0:
                return GetWantBlockingConditionData.CreateFromBinary(frame);
            case 1:
                return GetDistanceConditionData.CreateFromBinary(frame);
            case 5:
                return GetLockedConditionData.CreateFromBinary(frame);
            case 6:
                return GetPosConditionData.CreateFromBinary(frame);
            case 8:
                return GetAngleConditionData.CreateFromBinary(frame);
            case 10:
                return GetStartingPosConditionData.CreateFromBinary(frame);
            case 11:
                return GetStartingAngleConditionData.CreateFromBinary(frame);
            case 12:
                return GetSecondsPassedConditionData.CreateFromBinary(frame);
            case 14:
                return GetActorValueConditionData.CreateFromBinary(frame);
            case 18:
                return GetCurrentTimeConditionData.CreateFromBinary(frame);
            case 24:
                return GetScaleConditionData.CreateFromBinary(frame);
            case 25:
                return IsMovingConditionData.CreateFromBinary(frame);
            case 26:
                return IsTurningConditionData.CreateFromBinary(frame);
            case 27:
                return GetLineOfSightConditionData.CreateFromBinary(frame);
            case 32:
                return GetInSameCellConditionData.CreateFromBinary(frame);
            case 35:
                return GetDisabledConditionData.CreateFromBinary(frame);
            case 36:
                return MenuModeConditionData.CreateFromBinary(frame);
            case 39:
                return GetDiseaseConditionData.CreateFromBinary(frame);
            case 41:
                return GetClothingValueConditionData.CreateFromBinary(frame);
            case 42:
                return SameFactionConditionData.CreateFromBinary(frame);
            case 43:
                return SameRaceConditionData.CreateFromBinary(frame);
            case 44:
                return SameSexConditionData.CreateFromBinary(frame);
            case 45:
                return GetDetectedConditionData.CreateFromBinary(frame);
            case 46:
                return GetDeadConditionData.CreateFromBinary(frame);
            case 47:
                return GetItemCountConditionData.CreateFromBinary(frame);
            case 48:
                return GetGoldConditionData.CreateFromBinary(frame);
            case 49:
                return GetSleepingConditionData.CreateFromBinary(frame);
            case 50:
                return GetTalkedToPCConditionData.CreateFromBinary(frame);
            case 53:
                return GetScriptVariableConditionData.CreateFromBinary(frame);
            case 56:
                return GetQuestRunningConditionData.CreateFromBinary(frame);
            case 58:
                return GetStageConditionData.CreateFromBinary(frame);
            case 59:
                return GetStageDoneConditionData.CreateFromBinary(frame);
            case 60:
                return GetFactionRankDifferenceConditionData.CreateFromBinary(frame);
            case 61:
                return GetAlarmedConditionData.CreateFromBinary(frame);
            case 62:
                return IsRainingConditionData.CreateFromBinary(frame);
            case 63:
                return GetAttackedConditionData.CreateFromBinary(frame);
            case 64:
                return GetIsCreatureConditionData.CreateFromBinary(frame);
            case 65:
                return GetLockLevelConditionData.CreateFromBinary(frame);
            case 66:
                return GetShouldAttackConditionData.CreateFromBinary(frame);
            case 67:
                return GetInCellConditionData.CreateFromBinary(frame);
            case 68:
                return GetIsClassConditionData.CreateFromBinary(frame);
            case 69:
                return GetIsRaceConditionData.CreateFromBinary(frame);
            case 70:
                return GetIsSexConditionData.CreateFromBinary(frame);
            case 71:
                return GetInFactionConditionData.CreateFromBinary(frame);
            case 72:
                return GetIsIDConditionData.CreateFromBinary(frame);
            case 73:
                return GetFactionRankConditionData.CreateFromBinary(frame);
            case 74:
                return GetGlobalValueConditionData.CreateFromBinary(frame);
            case 75:
                return IsSnowingConditionData.CreateFromBinary(frame);
            case 77:
                return GetRandomPercentConditionData.CreateFromBinary(frame);
            case 79:
                return GetQuestVariableConditionData.CreateFromBinary(frame);
            case 80:
                return GetLevelConditionData.CreateFromBinary(frame);
            case 81:
                return IsRotatingConditionData.CreateFromBinary(frame);
            case 84:
                return GetDeadCountConditionData.CreateFromBinary(frame);
            case 91:
                return GetIsAlertedConditionData.CreateFromBinary(frame);
            case 98:
                return GetPlayerControlsDisabledConditionData.CreateFromBinary(frame);
            case 99:
                return GetHeadingAngleConditionData.CreateFromBinary(frame);
            case 101:
                return IsWeaponMagicOutConditionData.CreateFromBinary(frame);
            case 102:
                return IsTorchOutConditionData.CreateFromBinary(frame);
            case 103:
                return IsShieldOutConditionData.CreateFromBinary(frame);
            case 106:
                return IsFacingUpConditionData.CreateFromBinary(frame);
            case 107:
                return GetKnockedStateConditionData.CreateFromBinary(frame);
            case 108:
                return GetWeaponAnimTypeConditionData.CreateFromBinary(frame);
            case 109:
                return IsWeaponSkillTypeConditionData.CreateFromBinary(frame);
            case 110:
                return GetCurrentAIPackageConditionData.CreateFromBinary(frame);
            case 111:
                return IsWaitingConditionData.CreateFromBinary(frame);
            case 112:
                return IsIdlePlayingConditionData.CreateFromBinary(frame);
            case 116:
                return IsIntimidatedbyPlayerConditionData.CreateFromBinary(frame);
            case 117:
                return IsPlayerInRegionConditionData.CreateFromBinary(frame);
            case 118:
                return GetActorAggroRadiusViolatedConditionData.CreateFromBinary(frame);
            case 122:
                return GetCrimeConditionData.CreateFromBinary(frame);
            case 123:
                return IsGreetingPlayerConditionData.CreateFromBinary(frame);
            case 125:
                return IsGuardConditionData.CreateFromBinary(frame);
            case 127:
                return HasBeenEatenConditionData.CreateFromBinary(frame);
            case 128:
                return GetStaminaPercentageConditionData.CreateFromBinary(frame);
            case 129:
                return GetPCIsClassConditionData.CreateFromBinary(frame);
            case 130:
                return GetPCIsRaceConditionData.CreateFromBinary(frame);
            case 131:
                return GetPCIsSexConditionData.CreateFromBinary(frame);
            case 132:
                return GetPCInFactionConditionData.CreateFromBinary(frame);
            case 133:
                return SameFactionAsPCConditionData.CreateFromBinary(frame);
            case 134:
                return SameRaceAsPCConditionData.CreateFromBinary(frame);
            case 135:
                return SameSexAsPCConditionData.CreateFromBinary(frame);
            case 136:
                return GetIsReferenceConditionData.CreateFromBinary(frame);
            case 141:
                return IsTalkingConditionData.CreateFromBinary(frame);
            case 142:
                return GetWalkSpeedConditionData.CreateFromBinary(frame);
            case 143:
                return GetCurrentAIProcedureConditionData.CreateFromBinary(frame);
            case 144:
                return GetTrespassWarningLevelConditionData.CreateFromBinary(frame);
            case 145:
                return IsTrespassingConditionData.CreateFromBinary(frame);
            case 146:
                return IsInMyOwnedCellConditionData.CreateFromBinary(frame);
            case 147:
                return GetWindSpeedConditionData.CreateFromBinary(frame);
            case 148:
                return GetCurrentWeatherPercentConditionData.CreateFromBinary(frame);
            case 149:
                return GetIsCurrentWeatherConditionData.CreateFromBinary(frame);
            case 150:
                return IsContinuingPackagePCNearConditionData.CreateFromBinary(frame);
            case 152:
                return GetIsCrimeFactionConditionData.CreateFromBinary(frame);
            case 153:
                return CanHaveFlamesConditionData.CreateFromBinary(frame);
            case 154:
                return HasFlamesConditionData.CreateFromBinary(frame);
            case 157:
                return GetOpenStateConditionData.CreateFromBinary(frame);
            case 159:
                return GetSittingConditionData.CreateFromBinary(frame);
            case 161:
                return GetIsCurrentPackageConditionData.CreateFromBinary(frame);
            case 162:
                return IsCurrentFurnitureRefConditionData.CreateFromBinary(frame);
            case 163:
                return IsCurrentFurnitureObjConditionData.CreateFromBinary(frame);
            case 170:
                return GetDayOfWeekConditionData.CreateFromBinary(frame);
            case 172:
                return GetTalkedToPCParamConditionData.CreateFromBinary(frame);
            case 175:
                return IsPCSleepingConditionData.CreateFromBinary(frame);
            case 176:
                return IsPCAMurdererConditionData.CreateFromBinary(frame);
            case 180:
                return HasSameEditorLocAsRefConditionData.CreateFromBinary(frame);
            case 181:
                return HasSameEditorLocAsRefAliasConditionData.CreateFromBinary(frame);
            case 182:
                return GetEquippedConditionData.CreateFromBinary(frame);
            case 185:
                return IsSwimmingConditionData.CreateFromBinary(frame);
            case 190:
                return GetAmountSoldStolenConditionData.CreateFromBinary(frame);
            case 192:
                return GetIgnoreCrimeConditionData.CreateFromBinary(frame);
            case 193:
                return GetPCExpelledConditionData.CreateFromBinary(frame);
            case 195:
                return GetPCFactionMurderConditionData.CreateFromBinary(frame);
            case 197:
                return GetPCEnemyofFactionConditionData.CreateFromBinary(frame);
            case 199:
                return GetPCFactionAttackConditionData.CreateFromBinary(frame);
            case 203:
                return GetDestroyedConditionData.CreateFromBinary(frame);
            case 214:
                return HasMagicEffectConditionData.CreateFromBinary(frame);
            case 215:
                return GetDefaultOpenConditionData.CreateFromBinary(frame);
            case 219:
                return GetAnimActionConditionData.CreateFromBinary(frame);
            case 223:
                return IsSpellTargetConditionData.CreateFromBinary(frame);
            case 224:
                return GetVATSModeConditionData.CreateFromBinary(frame);
            case 225:
                return GetPersuasionNumberConditionData.CreateFromBinary(frame);
            case 226:
                return GetVampireFeedConditionData.CreateFromBinary(frame);
            case 227:
                return GetCannibalConditionData.CreateFromBinary(frame);
            case 228:
                return GetIsClassDefaultConditionData.CreateFromBinary(frame);
            case 229:
                return GetClassDefaultMatchConditionData.CreateFromBinary(frame);
            case 230:
                return GetInCellParamConditionData.CreateFromBinary(frame);
            case 235:
                return GetVatsTargetHeightConditionData.CreateFromBinary(frame);
            case 237:
                return GetIsGhostConditionData.CreateFromBinary(frame);
            case 242:
                return GetUnconsciousConditionData.CreateFromBinary(frame);
            case 244:
                return GetRestrainedConditionData.CreateFromBinary(frame);
            case 246:
                return GetIsUsedItemConditionData.CreateFromBinary(frame);
            case 247:
                return GetIsUsedItemTypeConditionData.CreateFromBinary(frame);
            case 248:
                return IsScenePlayingConditionData.CreateFromBinary(frame);
            case 249:
                return IsInDialogueWithPlayerConditionData.CreateFromBinary(frame);
            case 250:
                return GetLocationClearedConditionData.CreateFromBinary(frame);
            case 254:
                return GetIsPlayableRaceConditionData.CreateFromBinary(frame);
            case 255:
                return GetOffersServicesNowConditionData.CreateFromBinary(frame);
            case 258:
                return HasAssociationTypeConditionData.CreateFromBinary(frame);
            case 259:
                return HasFamilyRelationshipConditionData.CreateFromBinary(frame);
            case 261:
                return HasParentRelationshipConditionData.CreateFromBinary(frame);
            case 262:
                return IsWarningAboutConditionData.CreateFromBinary(frame);
            case 263:
                return IsWeaponOutConditionData.CreateFromBinary(frame);
            case 264:
                return HasSpellConditionData.CreateFromBinary(frame);
            case 265:
                return IsTimePassingConditionData.CreateFromBinary(frame);
            case 266:
                return IsPleasantConditionData.CreateFromBinary(frame);
            case 267:
                return IsCloudyConditionData.CreateFromBinary(frame);
            case 274:
                return IsSmallBumpConditionData.CreateFromBinary(frame);
            case 277:
                return GetBaseActorValueConditionData.CreateFromBinary(frame);
            case 278:
                return IsOwnerConditionData.CreateFromBinary(frame);
            case 280:
                return IsCellOwnerConditionData.CreateFromBinary(frame);
            case 282:
                return IsHorseStolenConditionData.CreateFromBinary(frame);
            case 285:
                return IsLeftUpConditionData.CreateFromBinary(frame);
            case 286:
                return IsSneakingConditionData.CreateFromBinary(frame);
            case 287:
                return IsRunningConditionData.CreateFromBinary(frame);
            case 288:
                return GetFriendHitConditionData.CreateFromBinary(frame);
            case 289:
                return IsInCombatConditionData.CreateFromBinary(frame);
            case 300:
                return IsInInteriorConditionData.CreateFromBinary(frame);
            case 304:
                return IsWaterObjectConditionData.CreateFromBinary(frame);
            case 305:
                return GetPlayerActionConditionData.CreateFromBinary(frame);
            case 306:
                return IsActorUsingATorchConditionData.CreateFromBinary(frame);
            case 309:
                return IsXBoxConditionData.CreateFromBinary(frame);
            case 310:
                return GetInWorldspaceConditionData.CreateFromBinary(frame);
            case 312:
                return GetPCMiscStatConditionData.CreateFromBinary(frame);
            case 313:
                return GetPairedAnimationConditionData.CreateFromBinary(frame);
            case 314:
                return IsActorAVictimConditionData.CreateFromBinary(frame);
            case 315:
                return GetTotalPersuasionNumberConditionData.CreateFromBinary(frame);
            case 318:
                return GetIdleDoneOnceConditionData.CreateFromBinary(frame);
            case 320:
                return GetNoRumorsConditionData.CreateFromBinary(frame);
            case 323:
                return GetCombatStateConditionData.CreateFromBinary(frame);
            case 325:
                return GetWithinPackageLocationConditionData.CreateFromBinary(frame);
            case 327:
                return IsRidingMountConditionData.CreateFromBinary(frame);
            case 329:
                return IsFleeingConditionData.CreateFromBinary(frame);
            case 332:
                return IsInDangerousWaterConditionData.CreateFromBinary(frame);
            case 338:
                return GetIgnoreFriendlyHitsConditionData.CreateFromBinary(frame);
            case 339:
                return IsPlayersLastRiddenMountConditionData.CreateFromBinary(frame);
            case 353:
                return IsActorConditionData.CreateFromBinary(frame);
            case 354:
                return IsEssentialConditionData.CreateFromBinary(frame);
            case 358:
                return IsPlayerMovingIntoNewSpaceConditionData.CreateFromBinary(frame);
            case 359:
                return GetInCurrentLocConditionData.CreateFromBinary(frame);
            case 360:
                return GetInCurrentLocAliasConditionData.CreateFromBinary(frame);
            case 361:
                return GetTimeDeadConditionData.CreateFromBinary(frame);
            case 362:
                return HasLinkedRefConditionData.CreateFromBinary(frame);
            case 365:
                return IsChildConditionData.CreateFromBinary(frame);
            case 366:
                return GetStolenItemValueNoCrimeConditionData.CreateFromBinary(frame);
            case 367:
                return GetLastPlayerActionConditionData.CreateFromBinary(frame);
            case 368:
                return IsPlayerActionActiveConditionData.CreateFromBinary(frame);
            case 370:
                return IsTalkingActivatorActorConditionData.CreateFromBinary(frame);
            case 372:
                return IsInListConditionData.CreateFromBinary(frame);
            case 373:
                return GetStolenItemValueConditionData.CreateFromBinary(frame);
            case 375:
                return GetCrimeGoldViolentConditionData.CreateFromBinary(frame);
            case 376:
                return GetCrimeGoldNonviolentConditionData.CreateFromBinary(frame);
            case 378:
                return HasShoutConditionData.CreateFromBinary(frame);
            case 381:
                return GetHasNoteConditionData.CreateFromBinary(frame);
            case 390:
                return GetHitLocationConditionData.CreateFromBinary(frame);
            case 391:
                return IsPC1stPersonConditionData.CreateFromBinary(frame);
            case 396:
                return GetCauseofDeathConditionData.CreateFromBinary(frame);
            case 397:
                return IsLimbGoneConditionData.CreateFromBinary(frame);
            case 398:
                return IsWeaponInListConditionData.CreateFromBinary(frame);
            case 402:
                return IsBribedbyPlayerConditionData.CreateFromBinary(frame);
            case 403:
                return GetRelationshipRankConditionData.CreateFromBinary(frame);
            case 407:
                return AGetVATSValueConditionData.CreateFromBinary(frame);
            case 408:
                return IsKillerConditionData.CreateFromBinary(frame);
            case 409:
                return IsKillerObjectConditionData.CreateFromBinary(frame);
            case 410:
                return GetFactionCombatReactionConditionData.CreateFromBinary(frame);
            case 414:
                return ExistsConditionData.CreateFromBinary(frame);
            case 415:
                return GetGroupMemberCountConditionData.CreateFromBinary(frame);
            case 416:
                return GetGroupTargetCountConditionData.CreateFromBinary(frame);
            case 426:
                return GetIsVoiceTypeConditionData.CreateFromBinary(frame);
            case 427:
                return GetPlantedExplosiveConditionData.CreateFromBinary(frame);
            case 429:
                return IsScenePackageRunningConditionData.CreateFromBinary(frame);
            case 430:
                return GetHealthPercentageConditionData.CreateFromBinary(frame);
            case 432:
                return GetIsObjectTypeConditionData.CreateFromBinary(frame);
            case 434:
                return GetDialogueEmotionConditionData.CreateFromBinary(frame);
            case 435:
                return GetDialogueEmotionValueConditionData.CreateFromBinary(frame);
            case 437:
                return GetIsCreatureTypeConditionData.CreateFromBinary(frame);
            case 444:
                return GetInCurrentLocFormListConditionData.CreateFromBinary(frame);
            case 445:
                return GetInZoneConditionData.CreateFromBinary(frame);
            case 446:
                return GetVelocityConditionData.CreateFromBinary(frame);
            case 447:
                return GetGraphVariableFloatConditionData.CreateFromBinary(frame);
            case 448:
                return HasPerkConditionData.CreateFromBinary(frame);
            case 449:
                return GetFactionRelationConditionData.CreateFromBinary(frame);
            case 450:
                return IsLastIdlePlayedConditionData.CreateFromBinary(frame);
            case 453:
                return GetPlayerTeammateConditionData.CreateFromBinary(frame);
            case 454:
                return GetPlayerTeammateCountConditionData.CreateFromBinary(frame);
            case 458:
                return GetActorCrimePlayerEnemyConditionData.CreateFromBinary(frame);
            case 459:
                return GetCrimeGoldConditionData.CreateFromBinary(frame);
            case 463:
                return IsPlayerGrabbedRefConditionData.CreateFromBinary(frame);
            case 465:
                return GetKeywordItemCountConditionData.CreateFromBinary(frame);
            case 470:
                return GetDestructionStageConditionData.CreateFromBinary(frame);
            case 473:
                return GetIsAlignmentConditionData.CreateFromBinary(frame);
            case 476:
                return IsProtectedConditionData.CreateFromBinary(frame);
            case 477:
                return GetThreatRatioConditionData.CreateFromBinary(frame);
            case 479:
                return GetIsUsedItemEquipTypeConditionData.CreateFromBinary(frame);
            case 487:
                return IsCarryableConditionData.CreateFromBinary(frame);
            case 488:
                return GetConcussedConditionData.CreateFromBinary(frame);
            case 491:
                return GetMapMarkerVisibleConditionData.CreateFromBinary(frame);
            case 493:
                return PlayerKnowsConditionData.CreateFromBinary(frame);
            case 494:
                return GetPermanentActorValueConditionData.CreateFromBinary(frame);
            case 495:
                return GetKillingBlowLimbConditionData.CreateFromBinary(frame);
            case 497:
                return CanPayCrimeGoldConditionData.CreateFromBinary(frame);
            case 499:
                return GetDaysInJailConditionData.CreateFromBinary(frame);
            case 500:
                return EPAlchemyGetMakingPoisonConditionData.CreateFromBinary(frame);
            case 501:
                return EPAlchemyEffectHasKeywordConditionData.CreateFromBinary(frame);
            case 503:
                return GetAllowWorldInteractionsConditionData.CreateFromBinary(frame);
            case 508:
                return GetLastHitCriticalConditionData.CreateFromBinary(frame);
            case 513:
                return IsCombatTargetConditionData.CreateFromBinary(frame);
            case 515:
                return GetVATSRightAreaFreeConditionData.CreateFromBinary(frame);
            case 516:
                return GetVATSLeftAreaFreeConditionData.CreateFromBinary(frame);
            case 517:
                return GetVATSBackAreaFreeConditionData.CreateFromBinary(frame);
            case 518:
                return GetVATSFrontAreaFreeConditionData.CreateFromBinary(frame);
            case 519:
                return GetLockIsBrokenConditionData.CreateFromBinary(frame);
            case 520:
                return IsPS3ConditionData.CreateFromBinary(frame);
            case 521:
                return IsWin32ConditionData.CreateFromBinary(frame);
            case 522:
                return GetVATSRightTargetVisibleConditionData.CreateFromBinary(frame);
            case 523:
                return GetVATSLeftTargetVisibleConditionData.CreateFromBinary(frame);
            case 524:
                return GetVATSBackTargetVisibleConditionData.CreateFromBinary(frame);
            case 525:
                return GetVATSFrontTargetVisibleConditionData.CreateFromBinary(frame);
            case 528:
                return IsInCriticalStageConditionData.CreateFromBinary(frame);
            case 530:
                return GetXPForNextLevelConditionData.CreateFromBinary(frame);
            case 533:
                return GetInfamyConditionData.CreateFromBinary(frame);
            case 534:
                return GetInfamyViolentConditionData.CreateFromBinary(frame);
            case 535:
                return GetInfamyNonViolentConditionData.CreateFromBinary(frame);
            case 543:
                return GetQuestCompletedConditionData.CreateFromBinary(frame);
            case 547:
                return IsGoreDisabledConditionData.CreateFromBinary(frame);
            case 550:
                return IsSceneActionCompleteConditionData.CreateFromBinary(frame);
            case 552:
                return GetSpellUsageNumConditionData.CreateFromBinary(frame);
            case 554:
                return GetActorsInHighConditionData.CreateFromBinary(frame);
            case 555:
                return HasLoaded3DConditionData.CreateFromBinary(frame);
            case 560:
                return HasKeywordConditionData.CreateFromBinary(frame);
            case 561:
                return HasRefTypeConditionData.CreateFromBinary(frame);
            case 562:
                return LocationHasKeywordConditionData.CreateFromBinary(frame);
            case 563:
                return LocationHasRefTypeConditionData.CreateFromBinary(frame);
            case 565:
                return GetIsEditorLocationConditionData.CreateFromBinary(frame);
            case 566:
                return GetIsAliasRefConditionData.CreateFromBinary(frame);
            case 567:
                return GetIsEditorLocAliasConditionData.CreateFromBinary(frame);
            case 568:
                return IsSprintingConditionData.CreateFromBinary(frame);
            case 569:
                return IsBlockingConditionData.CreateFromBinary(frame);
            case 570:
                return HasEquippedSpellConditionData.CreateFromBinary(frame);
            case 571:
                return GetCurrentCastingTypeConditionData.CreateFromBinary(frame);
            case 572:
                return GetCurrentDeliveryTypeConditionData.CreateFromBinary(frame);
            case 574:
                return GetAttackStateConditionData.CreateFromBinary(frame);
            case 576:
                return GetEventDataConditionData.CreateFromBinary(frame);
            case 577:
                return IsCloserToAThanBConditionData.CreateFromBinary(frame);
            case 579:
                return GetEquippedShoutConditionData.CreateFromBinary(frame);
            case 580:
                return IsBleedingOutConditionData.CreateFromBinary(frame);
            case 584:
                return GetRelativeAngleConditionData.CreateFromBinary(frame);
            case 589:
                return GetMovementDirectionConditionData.CreateFromBinary(frame);
            case 590:
                return IsInSceneConditionData.CreateFromBinary(frame);
            case 591:
                return GetRefTypeDeadCountConditionData.CreateFromBinary(frame);
            case 592:
                return GetRefTypeAliveCountConditionData.CreateFromBinary(frame);
            case 594:
                return GetIsFlyingConditionData.CreateFromBinary(frame);
            case 595:
                return IsCurrentSpellConditionData.CreateFromBinary(frame);
            case 596:
                return SpellHasKeywordConditionData.CreateFromBinary(frame);
            case 597:
                return GetEquippedItemTypeConditionData.CreateFromBinary(frame);
            case 598:
                return GetLocationAliasClearedConditionData.CreateFromBinary(frame);
            case 600:
                return GetLocAliasRefTypeDeadCountConditionData.CreateFromBinary(frame);
            case 601:
                return GetLocAliasRefTypeAliveCountConditionData.CreateFromBinary(frame);
            case 602:
                return IsWardStateConditionData.CreateFromBinary(frame);
            case 603:
                return IsInSameCurrentLocAsRefConditionData.CreateFromBinary(frame);
            case 604:
                return IsInSameCurrentLocAsRefAliasConditionData.CreateFromBinary(frame);
            case 605:
                return LocAliasIsLocationConditionData.CreateFromBinary(frame);
            case 606:
                return GetKeywordDataForLocationConditionData.CreateFromBinary(frame);
            case 608:
                return GetKeywordDataForAliasConditionData.CreateFromBinary(frame);
            case 610:
                return LocAliasHasKeywordConditionData.CreateFromBinary(frame);
            case 611:
                return IsNullPackageDataConditionData.CreateFromBinary(frame);
            case 612:
                return GetNumericPackageDataConditionData.CreateFromBinary(frame);
            case 613:
                return IsFurnitureAnimTypeConditionData.CreateFromBinary(frame);
            case 614:
                return IsFurnitureEntryTypeConditionData.CreateFromBinary(frame);
            case 615:
                return GetHighestRelationshipRankConditionData.CreateFromBinary(frame);
            case 616:
                return GetLowestRelationshipRankConditionData.CreateFromBinary(frame);
            case 617:
                return HasAssociationTypeAnyConditionData.CreateFromBinary(frame);
            case 618:
                return HasFamilyRelationshipAnyConditionData.CreateFromBinary(frame);
            case 619:
                return GetPathingTargetOffsetConditionData.CreateFromBinary(frame);
            case 620:
                return GetPathingTargetAngleOffsetConditionData.CreateFromBinary(frame);
            case 621:
                return GetPathingTargetSpeedConditionData.CreateFromBinary(frame);
            case 622:
                return GetPathingTargetSpeedAngleConditionData.CreateFromBinary(frame);
            case 623:
                return GetMovementSpeedConditionData.CreateFromBinary(frame);
            case 624:
                return GetInContainerConditionData.CreateFromBinary(frame);
            case 625:
                return IsLocationLoadedConditionData.CreateFromBinary(frame);
            case 626:
                return IsLocAliasLoadedConditionData.CreateFromBinary(frame);
            case 627:
                return IsDualCastingConditionData.CreateFromBinary(frame);
            case 629:
                return GetVMQuestVariableConditionData.CreateFromBinary(frame);
            case 630:
                return GetVMScriptVariableConditionData.CreateFromBinary(frame);
            case 631:
                return IsEnteringInteractionQuickConditionData.CreateFromBinary(frame);
            case 632:
                return IsCastingConditionData.CreateFromBinary(frame);
            case 633:
                return GetFlyingStateConditionData.CreateFromBinary(frame);
            case 635:
                return IsInFavorStateConditionData.CreateFromBinary(frame);
            case 636:
                return HasTwoHandedWeaponEquippedConditionData.CreateFromBinary(frame);
            case 637:
                return IsExitingInstantConditionData.CreateFromBinary(frame);
            case 638:
                return IsInFriendStateWithPlayerConditionData.CreateFromBinary(frame);
            case 639:
                return GetWithinDistanceConditionData.CreateFromBinary(frame);
            case 640:
                return GetActorValuePercentConditionData.CreateFromBinary(frame);
            case 641:
                return IsUniqueConditionData.CreateFromBinary(frame);
            case 642:
                return GetLastBumpDirectionConditionData.CreateFromBinary(frame);
            case 644:
                return IsInFurnitureStateConditionData.CreateFromBinary(frame);
            case 645:
                return GetIsInjuredConditionData.CreateFromBinary(frame);
            case 646:
                return GetIsCrashLandRequestConditionData.CreateFromBinary(frame);
            case 647:
                return GetIsHastyLandRequestConditionData.CreateFromBinary(frame);
            case 650:
                return IsLinkedToConditionData.CreateFromBinary(frame);
            case 651:
                return GetKeywordDataForCurrentLocationConditionData.CreateFromBinary(frame);
            case 652:
                return GetInSharedCrimeFactionConditionData.CreateFromBinary(frame);
            case 654:
                return GetBribeSuccessConditionData.CreateFromBinary(frame);
            case 655:
                return GetIntimidateSuccessConditionData.CreateFromBinary(frame);
            case 656:
                return GetArrestedStateConditionData.CreateFromBinary(frame);
            case 657:
                return GetArrestingActorConditionData.CreateFromBinary(frame);
            case 659:
                return EPTemperingItemIsEnchantedConditionData.CreateFromBinary(frame);
            case 660:
                return EPTemperingItemHasKeywordConditionData.CreateFromBinary(frame);
            case 664:
                return GetReplacedItemTypeConditionData.CreateFromBinary(frame);
            case 672:
                return IsAttackingConditionData.CreateFromBinary(frame);
            case 673:
                return IsPowerAttackingConditionData.CreateFromBinary(frame);
            case 674:
                return IsLastHostileActorConditionData.CreateFromBinary(frame);
            case 675:
                return GetGraphVariableIntConditionData.CreateFromBinary(frame);
            case 676:
                return GetCurrentShoutVariationConditionData.CreateFromBinary(frame);
            case 678:
                return ShouldAttackKillConditionData.CreateFromBinary(frame);
            case 680:
                return GetActivatorHeightConditionData.CreateFromBinary(frame);
            case 681:
                return EPMagic_IsAdvanceSkillConditionData.CreateFromBinary(frame);
            case 682:
                return WornHasKeywordConditionData.CreateFromBinary(frame);
            case 683:
                return GetPathingCurrentSpeedConditionData.CreateFromBinary(frame);
            case 684:
                return GetPathingCurrentSpeedAngleConditionData.CreateFromBinary(frame);
            case 691:
                return EPModSkillUsage_AdvanceObjectHasKeywordConditionData.CreateFromBinary(frame);
            case 692:
                return EPModSkillUsage_IsAdvanceActionConditionData.CreateFromBinary(frame);
            case 693:
                return EPMagic_SpellHasKeywordConditionData.CreateFromBinary(frame);
            case 694:
                return GetNoBleedoutRecoveryConditionData.CreateFromBinary(frame);
            case 696:
                return EPMagic_SpellHasSkillConditionData.CreateFromBinary(frame);
            case 697:
                return IsAttackTypeConditionData.CreateFromBinary(frame);
            case 698:
                return IsAllowedToFlyConditionData.CreateFromBinary(frame);
            case 699:
                return HasMagicEffectKeywordConditionData.CreateFromBinary(frame);
            case 700:
                return IsCommandedActorConditionData.CreateFromBinary(frame);
            case 701:
                return IsStaggeredConditionData.CreateFromBinary(frame);
            case 702:
                return IsRecoilingConditionData.CreateFromBinary(frame);
            case 703:
                return IsExitingInteractionQuickConditionData.CreateFromBinary(frame);
            case 704:
                return IsPathingConditionData.CreateFromBinary(frame);
            case 705:
                return GetShouldHelpConditionData.CreateFromBinary(frame);
            case 706:
                return HasBoundWeaponEquippedConditionData.CreateFromBinary(frame);
            case 707:
                return GetCombatTargetHasKeywordConditionData.CreateFromBinary(frame);
            case 709:
                return GetCombatGroupMemberCountConditionData.CreateFromBinary(frame);
            case 710:
                return IsIgnoringCombatConditionData.CreateFromBinary(frame);
            case 711:
                return GetLightLevelConditionData.CreateFromBinary(frame);
            case 713:
                return SpellHasCastingPerkConditionData.CreateFromBinary(frame);
            case 714:
                return IsBeingRiddenConditionData.CreateFromBinary(frame);
            case 715:
                return IsUndeadConditionData.CreateFromBinary(frame);
            case 716:
                return GetRealHoursPassedConditionData.CreateFromBinary(frame);
            case 718:
                return IsUnlockedDoorConditionData.CreateFromBinary(frame);
            case 719:
                return IsHostileToActorConditionData.CreateFromBinary(frame);
            case 720:
                return GetTargetHeightConditionData.CreateFromBinary(frame);
            case 721:
                return IsPoisonConditionData.CreateFromBinary(frame);
            case 722:
                return WornApparelHasKeywordCountConditionData.CreateFromBinary(frame);
            case 723:
                return GetItemHealthPercentConditionData.CreateFromBinary(frame);
            case 724:
                return EffectWasDualCastConditionData.CreateFromBinary(frame);
            case 725:
                return GetKnockedStateEnumConditionData.CreateFromBinary(frame);
            case 726:
                return DoesNotExistConditionData.CreateFromBinary(frame);
            case 730:
                return IsOnFlyingMountConditionData.CreateFromBinary(frame);
            case 731:
                return CanFlyHereConditionData.CreateFromBinary(frame);
            case 732:
                return IsFlyingMountPatrolQueudConditionData.CreateFromBinary(frame);
            case 733:
                return IsFlyingMountFastTravellingConditionData.CreateFromBinary(frame);
            case 734:
                return IsOverEncumberedConditionData.CreateFromBinary(frame);
            case 735:
                return GetActorWarmthConditionData.CreateFromBinary(frame);
            case 1024:
                return GetSKSEVersionConditionData.CreateFromBinary(frame);
            case 1025:
                return GetSKSEVersionMinorConditionData.CreateFromBinary(frame);
            case 1026:
                return GetSKSEVersionBetaConditionData.CreateFromBinary(frame);
            case 1027:
                return GetSKSEReleaseConditionData.CreateFromBinary(frame);
            case 1028:
                return ClearInvalidRegistrationsConditionData.CreateFromBinary(frame);
            default:
                return null;
        }
    }

    public ushort GetFunctionIndex(IConditionDataGetter data)
    {
        switch (data)
        {
            case IGetWantBlockingConditionDataGetter:
                return 0;
            case IGetDistanceConditionDataGetter:
                return 1;
            case IGetLockedConditionDataGetter:
                return 5;
            case IGetPosConditionDataGetter:
                return 6;
            case IGetAngleConditionDataGetter:
                return 8;
            case IGetStartingPosConditionDataGetter:
                return 10;
            case IGetStartingAngleConditionDataGetter:
                return 11;
            case IGetSecondsPassedConditionDataGetter:
                return 12;
            case IGetActorValueConditionDataGetter:
                return 14;
            case IGetCurrentTimeConditionDataGetter:
                return 18;
            case IGetScaleConditionDataGetter:
                return 24;
            case IIsMovingConditionDataGetter:
                return 25;
            case IIsTurningConditionDataGetter:
                return 26;
            case IGetLineOfSightConditionDataGetter:
                return 27;
            case IGetInSameCellConditionDataGetter:
                return 32;
            case IGetDisabledConditionDataGetter:
                return 35;
            case IMenuModeConditionDataGetter:
                return 36;
            case IGetDiseaseConditionDataGetter:
                return 39;
            case IGetClothingValueConditionDataGetter:
                return 41;
            case ISameFactionConditionDataGetter:
                return 42;
            case ISameRaceConditionDataGetter:
                return 43;
            case ISameSexConditionDataGetter:
                return 44;
            case IGetDetectedConditionDataGetter:
                return 45;
            case IGetDeadConditionDataGetter:
                return 46;
            case IGetItemCountConditionDataGetter:
                return 47;
            case IGetGoldConditionDataGetter:
                return 48;
            case IGetSleepingConditionDataGetter:
                return 49;
            case IGetTalkedToPCConditionDataGetter:
                return 50;
            case IGetScriptVariableConditionDataGetter:
                return 53;
            case IGetQuestRunningConditionDataGetter:
                return 56;
            case IGetStageConditionDataGetter:
                return 58;
            case IGetStageDoneConditionDataGetter:
                return 59;
            case IGetFactionRankDifferenceConditionDataGetter:
                return 60;
            case IGetAlarmedConditionDataGetter:
                return 61;
            case IIsRainingConditionDataGetter:
                return 62;
            case IGetAttackedConditionDataGetter:
                return 63;
            case IGetIsCreatureConditionDataGetter:
                return 64;
            case IGetLockLevelConditionDataGetter:
                return 65;
            case IGetShouldAttackConditionDataGetter:
                return 66;
            case IGetInCellConditionDataGetter:
                return 67;
            case IGetIsClassConditionDataGetter:
                return 68;
            case IGetIsRaceConditionDataGetter:
                return 69;
            case IGetIsSexConditionDataGetter:
                return 70;
            case IGetInFactionConditionDataGetter:
                return 71;
            case IGetIsIDConditionDataGetter:
                return 72;
            case IGetFactionRankConditionDataGetter:
                return 73;
            case IGetGlobalValueConditionDataGetter:
                return 74;
            case IIsSnowingConditionDataGetter:
                return 75;
            case IGetRandomPercentConditionDataGetter:
                return 77;
            case IGetQuestVariableConditionDataGetter:
                return 79;
            case IGetLevelConditionDataGetter:
                return 80;
            case IIsRotatingConditionDataGetter:
                return 81;
            case IGetDeadCountConditionDataGetter:
                return 84;
            case IGetIsAlertedConditionDataGetter:
                return 91;
            case IGetPlayerControlsDisabledConditionDataGetter:
                return 98;
            case IGetHeadingAngleConditionDataGetter:
                return 99;
            case IIsWeaponMagicOutConditionDataGetter:
                return 101;
            case IIsTorchOutConditionDataGetter:
                return 102;
            case IIsShieldOutConditionDataGetter:
                return 103;
            case IIsFacingUpConditionDataGetter:
                return 106;
            case IGetKnockedStateConditionDataGetter:
                return 107;
            case IGetWeaponAnimTypeConditionDataGetter:
                return 108;
            case IIsWeaponSkillTypeConditionDataGetter:
                return 109;
            case IGetCurrentAIPackageConditionDataGetter:
                return 110;
            case IIsWaitingConditionDataGetter:
                return 111;
            case IIsIdlePlayingConditionDataGetter:
                return 112;
            case IIsIntimidatedbyPlayerConditionDataGetter:
                return 116;
            case IIsPlayerInRegionConditionDataGetter:
                return 117;
            case IGetActorAggroRadiusViolatedConditionDataGetter:
                return 118;
            case IGetCrimeConditionDataGetter:
                return 122;
            case IIsGreetingPlayerConditionDataGetter:
                return 123;
            case IIsGuardConditionDataGetter:
                return 125;
            case IHasBeenEatenConditionDataGetter:
                return 127;
            case IGetStaminaPercentageConditionDataGetter:
                return 128;
            case IGetPCIsClassConditionDataGetter:
                return 129;
            case IGetPCIsRaceConditionDataGetter:
                return 130;
            case IGetPCIsSexConditionDataGetter:
                return 131;
            case IGetPCInFactionConditionDataGetter:
                return 132;
            case ISameFactionAsPCConditionDataGetter:
                return 133;
            case ISameRaceAsPCConditionDataGetter:
                return 134;
            case ISameSexAsPCConditionDataGetter:
                return 135;
            case IGetIsReferenceConditionDataGetter:
                return 136;
            case IIsTalkingConditionDataGetter:
                return 141;
            case IGetWalkSpeedConditionDataGetter:
                return 142;
            case IGetCurrentAIProcedureConditionDataGetter:
                return 143;
            case IGetTrespassWarningLevelConditionDataGetter:
                return 144;
            case IIsTrespassingConditionDataGetter:
                return 145;
            case IIsInMyOwnedCellConditionDataGetter:
                return 146;
            case IGetWindSpeedConditionDataGetter:
                return 147;
            case IGetCurrentWeatherPercentConditionDataGetter:
                return 148;
            case IGetIsCurrentWeatherConditionDataGetter:
                return 149;
            case IIsContinuingPackagePCNearConditionDataGetter:
                return 150;
            case IGetIsCrimeFactionConditionDataGetter:
                return 152;
            case ICanHaveFlamesConditionDataGetter:
                return 153;
            case IHasFlamesConditionDataGetter:
                return 154;
            case IGetOpenStateConditionDataGetter:
                return 157;
            case IGetSittingConditionDataGetter:
                return 159;
            case IGetIsCurrentPackageConditionDataGetter:
                return 161;
            case IIsCurrentFurnitureRefConditionDataGetter:
                return 162;
            case IIsCurrentFurnitureObjConditionDataGetter:
                return 163;
            case IGetDayOfWeekConditionDataGetter:
                return 170;
            case IGetTalkedToPCParamConditionDataGetter:
                return 172;
            case IIsPCSleepingConditionDataGetter:
                return 175;
            case IIsPCAMurdererConditionDataGetter:
                return 176;
            case IHasSameEditorLocAsRefConditionDataGetter:
                return 180;
            case IHasSameEditorLocAsRefAliasConditionDataGetter:
                return 181;
            case IGetEquippedConditionDataGetter:
                return 182;
            case IIsSwimmingConditionDataGetter:
                return 185;
            case IGetAmountSoldStolenConditionDataGetter:
                return 190;
            case IGetIgnoreCrimeConditionDataGetter:
                return 192;
            case IGetPCExpelledConditionDataGetter:
                return 193;
            case IGetPCFactionMurderConditionDataGetter:
                return 195;
            case IGetPCEnemyofFactionConditionDataGetter:
                return 197;
            case IGetPCFactionAttackConditionDataGetter:
                return 199;
            case IGetDestroyedConditionDataGetter:
                return 203;
            case IHasMagicEffectConditionDataGetter:
                return 214;
            case IGetDefaultOpenConditionDataGetter:
                return 215;
            case IGetAnimActionConditionDataGetter:
                return 219;
            case IIsSpellTargetConditionDataGetter:
                return 223;
            case IGetVATSModeConditionDataGetter:
                return 224;
            case IGetPersuasionNumberConditionDataGetter:
                return 225;
            case IGetVampireFeedConditionDataGetter:
                return 226;
            case IGetCannibalConditionDataGetter:
                return 227;
            case IGetIsClassDefaultConditionDataGetter:
                return 228;
            case IGetClassDefaultMatchConditionDataGetter:
                return 229;
            case IGetInCellParamConditionDataGetter:
                return 230;
            case IGetVatsTargetHeightConditionDataGetter:
                return 235;
            case IGetIsGhostConditionDataGetter:
                return 237;
            case IGetUnconsciousConditionDataGetter:
                return 242;
            case IGetRestrainedConditionDataGetter:
                return 244;
            case IGetIsUsedItemConditionDataGetter:
                return 246;
            case IGetIsUsedItemTypeConditionDataGetter:
                return 247;
            case IIsScenePlayingConditionDataGetter:
                return 248;
            case IIsInDialogueWithPlayerConditionDataGetter:
                return 249;
            case IGetLocationClearedConditionDataGetter:
                return 250;
            case IGetIsPlayableRaceConditionDataGetter:
                return 254;
            case IGetOffersServicesNowConditionDataGetter:
                return 255;
            case IHasAssociationTypeConditionDataGetter:
                return 258;
            case IHasFamilyRelationshipConditionDataGetter:
                return 259;
            case IHasParentRelationshipConditionDataGetter:
                return 261;
            case IIsWarningAboutConditionDataGetter:
                return 262;
            case IIsWeaponOutConditionDataGetter:
                return 263;
            case IHasSpellConditionDataGetter:
                return 264;
            case IIsTimePassingConditionDataGetter:
                return 265;
            case IIsPleasantConditionDataGetter:
                return 266;
            case IIsCloudyConditionDataGetter:
                return 267;
            case IIsSmallBumpConditionDataGetter:
                return 274;
            case IGetBaseActorValueConditionDataGetter:
                return 277;
            case IIsOwnerConditionDataGetter:
                return 278;
            case IIsCellOwnerConditionDataGetter:
                return 280;
            case IIsHorseStolenConditionDataGetter:
                return 282;
            case IIsLeftUpConditionDataGetter:
                return 285;
            case IIsSneakingConditionDataGetter:
                return 286;
            case IIsRunningConditionDataGetter:
                return 287;
            case IGetFriendHitConditionDataGetter:
                return 288;
            case IIsInCombatConditionDataGetter:
                return 289;
            case IIsInInteriorConditionDataGetter:
                return 300;
            case IIsWaterObjectConditionDataGetter:
                return 304;
            case IGetPlayerActionConditionDataGetter:
                return 305;
            case IIsActorUsingATorchConditionDataGetter:
                return 306;
            case IIsXBoxConditionDataGetter:
                return 309;
            case IGetInWorldspaceConditionDataGetter:
                return 310;
            case IGetPCMiscStatConditionDataGetter:
                return 312;
            case IGetPairedAnimationConditionDataGetter:
                return 313;
            case IIsActorAVictimConditionDataGetter:
                return 314;
            case IGetTotalPersuasionNumberConditionDataGetter:
                return 315;
            case IGetIdleDoneOnceConditionDataGetter:
                return 318;
            case IGetNoRumorsConditionDataGetter:
                return 320;
            case IGetCombatStateConditionDataGetter:
                return 323;
            case IGetWithinPackageLocationConditionDataGetter:
                return 325;
            case IIsRidingMountConditionDataGetter:
                return 327;
            case IIsFleeingConditionDataGetter:
                return 329;
            case IIsInDangerousWaterConditionDataGetter:
                return 332;
            case IGetIgnoreFriendlyHitsConditionDataGetter:
                return 338;
            case IIsPlayersLastRiddenMountConditionDataGetter:
                return 339;
            case IIsActorConditionDataGetter:
                return 353;
            case IIsEssentialConditionDataGetter:
                return 354;
            case IIsPlayerMovingIntoNewSpaceConditionDataGetter:
                return 358;
            case IGetInCurrentLocConditionDataGetter:
                return 359;
            case IGetInCurrentLocAliasConditionDataGetter:
                return 360;
            case IGetTimeDeadConditionDataGetter:
                return 361;
            case IHasLinkedRefConditionDataGetter:
                return 362;
            case IIsChildConditionDataGetter:
                return 365;
            case IGetStolenItemValueNoCrimeConditionDataGetter:
                return 366;
            case IGetLastPlayerActionConditionDataGetter:
                return 367;
            case IIsPlayerActionActiveConditionDataGetter:
                return 368;
            case IIsTalkingActivatorActorConditionDataGetter:
                return 370;
            case IIsInListConditionDataGetter:
                return 372;
            case IGetStolenItemValueConditionDataGetter:
                return 373;
            case IGetCrimeGoldViolentConditionDataGetter:
                return 375;
            case IGetCrimeGoldNonviolentConditionDataGetter:
                return 376;
            case IHasShoutConditionDataGetter:
                return 378;
            case IGetHasNoteConditionDataGetter:
                return 381;
            case IGetHitLocationConditionDataGetter:
                return 390;
            case IIsPC1stPersonConditionDataGetter:
                return 391;
            case IGetCauseofDeathConditionDataGetter:
                return 396;
            case IIsLimbGoneConditionDataGetter:
                return 397;
            case IIsWeaponInListConditionDataGetter:
                return 398;
            case IIsBribedbyPlayerConditionDataGetter:
                return 402;
            case IGetRelationshipRankConditionDataGetter:
                return 403;
            case IAGetVATSValueConditionDataGetter:
                return 407;
            case IIsKillerConditionDataGetter:
                return 408;
            case IIsKillerObjectConditionDataGetter:
                return 409;
            case IGetFactionCombatReactionConditionDataGetter:
                return 410;
            case IExistsConditionDataGetter:
                return 414;
            case IGetGroupMemberCountConditionDataGetter:
                return 415;
            case IGetGroupTargetCountConditionDataGetter:
                return 416;
            case IGetIsVoiceTypeConditionDataGetter:
                return 426;
            case IGetPlantedExplosiveConditionDataGetter:
                return 427;
            case IIsScenePackageRunningConditionDataGetter:
                return 429;
            case IGetHealthPercentageConditionDataGetter:
                return 430;
            case IGetIsObjectTypeConditionDataGetter:
                return 432;
            case IGetDialogueEmotionConditionDataGetter:
                return 434;
            case IGetDialogueEmotionValueConditionDataGetter:
                return 435;
            case IGetIsCreatureTypeConditionDataGetter:
                return 437;
            case IGetInCurrentLocFormListConditionDataGetter:
                return 444;
            case IGetInZoneConditionDataGetter:
                return 445;
            case IGetVelocityConditionDataGetter:
                return 446;
            case IGetGraphVariableFloatConditionDataGetter:
                return 447;
            case IHasPerkConditionDataGetter:
                return 448;
            case IGetFactionRelationConditionDataGetter:
                return 449;
            case IIsLastIdlePlayedConditionDataGetter:
                return 450;
            case IGetPlayerTeammateConditionDataGetter:
                return 453;
            case IGetPlayerTeammateCountConditionDataGetter:
                return 454;
            case IGetActorCrimePlayerEnemyConditionDataGetter:
                return 458;
            case IGetCrimeGoldConditionDataGetter:
                return 459;
            case IIsPlayerGrabbedRefConditionDataGetter:
                return 463;
            case IGetKeywordItemCountConditionDataGetter:
                return 465;
            case IGetDestructionStageConditionDataGetter:
                return 470;
            case IGetIsAlignmentConditionDataGetter:
                return 473;
            case IIsProtectedConditionDataGetter:
                return 476;
            case IGetThreatRatioConditionDataGetter:
                return 477;
            case IGetIsUsedItemEquipTypeConditionDataGetter:
                return 479;
            case IIsCarryableConditionDataGetter:
                return 487;
            case IGetConcussedConditionDataGetter:
                return 488;
            case IGetMapMarkerVisibleConditionDataGetter:
                return 491;
            case IPlayerKnowsConditionDataGetter:
                return 493;
            case IGetPermanentActorValueConditionDataGetter:
                return 494;
            case IGetKillingBlowLimbConditionDataGetter:
                return 495;
            case ICanPayCrimeGoldConditionDataGetter:
                return 497;
            case IGetDaysInJailConditionDataGetter:
                return 499;
            case IEPAlchemyGetMakingPoisonConditionDataGetter:
                return 500;
            case IEPAlchemyEffectHasKeywordConditionDataGetter:
                return 501;
            case IGetAllowWorldInteractionsConditionDataGetter:
                return 503;
            case IGetLastHitCriticalConditionDataGetter:
                return 508;
            case IIsCombatTargetConditionDataGetter:
                return 513;
            case IGetVATSRightAreaFreeConditionDataGetter:
                return 515;
            case IGetVATSLeftAreaFreeConditionDataGetter:
                return 516;
            case IGetVATSBackAreaFreeConditionDataGetter:
                return 517;
            case IGetVATSFrontAreaFreeConditionDataGetter:
                return 518;
            case IGetLockIsBrokenConditionDataGetter:
                return 519;
            case IIsPS3ConditionDataGetter:
                return 520;
            case IIsWin32ConditionDataGetter:
                return 521;
            case IGetVATSRightTargetVisibleConditionDataGetter:
                return 522;
            case IGetVATSLeftTargetVisibleConditionDataGetter:
                return 523;
            case IGetVATSBackTargetVisibleConditionDataGetter:
                return 524;
            case IGetVATSFrontTargetVisibleConditionDataGetter:
                return 525;
            case IIsInCriticalStageConditionDataGetter:
                return 528;
            case IGetXPForNextLevelConditionDataGetter:
                return 530;
            case IGetInfamyConditionDataGetter:
                return 533;
            case IGetInfamyViolentConditionDataGetter:
                return 534;
            case IGetInfamyNonViolentConditionDataGetter:
                return 535;
            case IGetQuestCompletedConditionDataGetter:
                return 543;
            case IIsGoreDisabledConditionDataGetter:
                return 547;
            case IIsSceneActionCompleteConditionDataGetter:
                return 550;
            case IGetSpellUsageNumConditionDataGetter:
                return 552;
            case IGetActorsInHighConditionDataGetter:
                return 554;
            case IHasLoaded3DConditionDataGetter:
                return 555;
            case IHasKeywordConditionDataGetter:
                return 560;
            case IHasRefTypeConditionDataGetter:
                return 561;
            case ILocationHasKeywordConditionDataGetter:
                return 562;
            case ILocationHasRefTypeConditionDataGetter:
                return 563;
            case IGetIsEditorLocationConditionDataGetter:
                return 565;
            case IGetIsAliasRefConditionDataGetter:
                return 566;
            case IGetIsEditorLocAliasConditionDataGetter:
                return 567;
            case IIsSprintingConditionDataGetter:
                return 568;
            case IIsBlockingConditionDataGetter:
                return 569;
            case IHasEquippedSpellConditionDataGetter:
                return 570;
            case IGetCurrentCastingTypeConditionDataGetter:
                return 571;
            case IGetCurrentDeliveryTypeConditionDataGetter:
                return 572;
            case IGetAttackStateConditionDataGetter:
                return 574;
            case IGetEventDataConditionDataGetter:
                return 576;
            case IIsCloserToAThanBConditionDataGetter:
                return 577;
            case IGetEquippedShoutConditionDataGetter:
                return 579;
            case IIsBleedingOutConditionDataGetter:
                return 580;
            case IGetRelativeAngleConditionDataGetter:
                return 584;
            case IGetMovementDirectionConditionDataGetter:
                return 589;
            case IIsInSceneConditionDataGetter:
                return 590;
            case IGetRefTypeDeadCountConditionDataGetter:
                return 591;
            case IGetRefTypeAliveCountConditionDataGetter:
                return 592;
            case IGetIsFlyingConditionDataGetter:
                return 594;
            case IIsCurrentSpellConditionDataGetter:
                return 595;
            case ISpellHasKeywordConditionDataGetter:
                return 596;
            case IGetEquippedItemTypeConditionDataGetter:
                return 597;
            case IGetLocationAliasClearedConditionDataGetter:
                return 598;
            case IGetLocAliasRefTypeDeadCountConditionDataGetter:
                return 600;
            case IGetLocAliasRefTypeAliveCountConditionDataGetter:
                return 601;
            case IIsWardStateConditionDataGetter:
                return 602;
            case IIsInSameCurrentLocAsRefConditionDataGetter:
                return 603;
            case IIsInSameCurrentLocAsRefAliasConditionDataGetter:
                return 604;
            case ILocAliasIsLocationConditionDataGetter:
                return 605;
            case IGetKeywordDataForLocationConditionDataGetter:
                return 606;
            case IGetKeywordDataForAliasConditionDataGetter:
                return 608;
            case ILocAliasHasKeywordConditionDataGetter:
                return 610;
            case IIsNullPackageDataConditionDataGetter:
                return 611;
            case IGetNumericPackageDataConditionDataGetter:
                return 612;
            case IIsFurnitureAnimTypeConditionDataGetter:
                return 613;
            case IIsFurnitureEntryTypeConditionDataGetter:
                return 614;
            case IGetHighestRelationshipRankConditionDataGetter:
                return 615;
            case IGetLowestRelationshipRankConditionDataGetter:
                return 616;
            case IHasAssociationTypeAnyConditionDataGetter:
                return 617;
            case IHasFamilyRelationshipAnyConditionDataGetter:
                return 618;
            case IGetPathingTargetOffsetConditionDataGetter:
                return 619;
            case IGetPathingTargetAngleOffsetConditionDataGetter:
                return 620;
            case IGetPathingTargetSpeedConditionDataGetter:
                return 621;
            case IGetPathingTargetSpeedAngleConditionDataGetter:
                return 622;
            case IGetMovementSpeedConditionDataGetter:
                return 623;
            case IGetInContainerConditionDataGetter:
                return 624;
            case IIsLocationLoadedConditionDataGetter:
                return 625;
            case IIsLocAliasLoadedConditionDataGetter:
                return 626;
            case IIsDualCastingConditionDataGetter:
                return 627;
            case IGetVMQuestVariableConditionDataGetter:
                return 629;
            case IGetVMScriptVariableConditionDataGetter:
                return 630;
            case IIsEnteringInteractionQuickConditionDataGetter:
                return 631;
            case IIsCastingConditionDataGetter:
                return 632;
            case IGetFlyingStateConditionDataGetter:
                return 633;
            case IIsInFavorStateConditionDataGetter:
                return 635;
            case IHasTwoHandedWeaponEquippedConditionDataGetter:
                return 636;
            case IIsExitingInstantConditionDataGetter:
                return 637;
            case IIsInFriendStateWithPlayerConditionDataGetter:
                return 638;
            case IGetWithinDistanceConditionDataGetter:
                return 639;
            case IGetActorValuePercentConditionDataGetter:
                return 640;
            case IIsUniqueConditionDataGetter:
                return 641;
            case IGetLastBumpDirectionConditionDataGetter:
                return 642;
            case IIsInFurnitureStateConditionDataGetter:
                return 644;
            case IGetIsInjuredConditionDataGetter:
                return 645;
            case IGetIsCrashLandRequestConditionDataGetter:
                return 646;
            case IGetIsHastyLandRequestConditionDataGetter:
                return 647;
            case IIsLinkedToConditionDataGetter:
                return 650;
            case IGetKeywordDataForCurrentLocationConditionDataGetter:
                return 651;
            case IGetInSharedCrimeFactionConditionDataGetter:
                return 652;
            case IGetBribeSuccessConditionDataGetter:
                return 654;
            case IGetIntimidateSuccessConditionDataGetter:
                return 655;
            case IGetArrestedStateConditionDataGetter:
                return 656;
            case IGetArrestingActorConditionDataGetter:
                return 657;
            case IEPTemperingItemIsEnchantedConditionDataGetter:
                return 659;
            case IEPTemperingItemHasKeywordConditionDataGetter:
                return 660;
            case IGetReplacedItemTypeConditionDataGetter:
                return 664;
            case IIsAttackingConditionDataGetter:
                return 672;
            case IIsPowerAttackingConditionDataGetter:
                return 673;
            case IIsLastHostileActorConditionDataGetter:
                return 674;
            case IGetGraphVariableIntConditionDataGetter:
                return 675;
            case IGetCurrentShoutVariationConditionDataGetter:
                return 676;
            case IShouldAttackKillConditionDataGetter:
                return 678;
            case IGetActivatorHeightConditionDataGetter:
                return 680;
            case IEPMagic_IsAdvanceSkillConditionDataGetter:
                return 681;
            case IWornHasKeywordConditionDataGetter:
                return 682;
            case IGetPathingCurrentSpeedConditionDataGetter:
                return 683;
            case IGetPathingCurrentSpeedAngleConditionDataGetter:
                return 684;
            case IEPModSkillUsage_AdvanceObjectHasKeywordConditionDataGetter:
                return 691;
            case IEPModSkillUsage_IsAdvanceActionConditionDataGetter:
                return 692;
            case IEPMagic_SpellHasKeywordConditionDataGetter:
                return 693;
            case IGetNoBleedoutRecoveryConditionDataGetter:
                return 694;
            case IEPMagic_SpellHasSkillConditionDataGetter:
                return 696;
            case IIsAttackTypeConditionDataGetter:
                return 697;
            case IIsAllowedToFlyConditionDataGetter:
                return 698;
            case IHasMagicEffectKeywordConditionDataGetter:
                return 699;
            case IIsCommandedActorConditionDataGetter:
                return 700;
            case IIsStaggeredConditionDataGetter:
                return 701;
            case IIsRecoilingConditionDataGetter:
                return 702;
            case IIsExitingInteractionQuickConditionDataGetter:
                return 703;
            case IIsPathingConditionDataGetter:
                return 704;
            case IGetShouldHelpConditionDataGetter:
                return 705;
            case IHasBoundWeaponEquippedConditionDataGetter:
                return 706;
            case IGetCombatTargetHasKeywordConditionDataGetter:
                return 707;
            case IGetCombatGroupMemberCountConditionDataGetter:
                return 709;
            case IIsIgnoringCombatConditionDataGetter:
                return 710;
            case IGetLightLevelConditionDataGetter:
                return 711;
            case ISpellHasCastingPerkConditionDataGetter:
                return 713;
            case IIsBeingRiddenConditionDataGetter:
                return 714;
            case IIsUndeadConditionDataGetter:
                return 715;
            case IGetRealHoursPassedConditionDataGetter:
                return 716;
            case IIsUnlockedDoorConditionDataGetter:
                return 718;
            case IIsHostileToActorConditionDataGetter:
                return 719;
            case IGetTargetHeightConditionDataGetter:
                return 720;
            case IIsPoisonConditionDataGetter:
                return 721;
            case IWornApparelHasKeywordCountConditionDataGetter:
                return 722;
            case IGetItemHealthPercentConditionDataGetter:
                return 723;
            case IEffectWasDualCastConditionDataGetter:
                return 724;
            case IGetKnockedStateEnumConditionDataGetter:
                return 725;
            case IDoesNotExistConditionDataGetter:
                return 726;
            case IIsOnFlyingMountConditionDataGetter:
                return 730;
            case ICanFlyHereConditionDataGetter:
                return 731;
            case IIsFlyingMountPatrolQueudConditionDataGetter:
                return 732;
            case IIsFlyingMountFastTravellingConditionDataGetter:
                return 733;
            case IIsOverEncumberedConditionDataGetter:
                return 734;
            case IGetActorWarmthConditionDataGetter:
                return 735;
            case IGetSKSEVersionConditionDataGetter:
                return 1024;
            case IGetSKSEVersionMinorConditionDataGetter:
                return 1025;
            case IGetSKSEVersionBetaConditionDataGetter:
                return 1026;
            case IGetSKSEReleaseConditionDataGetter:
                return 1027;
            case IClearInvalidRegistrationsConditionDataGetter:
                return 1028;
            default:
                throw new NotImplementedException();
        }
    }
    
    public static partial void FillBinaryFunctionParseCustom(MutagenFrame frame, ICondition obj)
    {
        switch (obj)
        {
            case IConditionGlobal global:
                global.ComparisonValue.SetTo(FormLinkBinaryTranslation.Instance.Parse(frame));
                break;
            case IConditionFloat f:
                f.ComparisonValue = FloatBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Parse(frame);
                break;
            default:
                throw new NotImplementedException();
        }
        
        var functionIndex = frame.ReadUInt16();
        obj.Unknown2 = frame.ReadUInt16();
        obj.Data = CreateDataFromBinary(frame, functionIndex);
    }
}

partial class ConditionBinaryWriteTranslation
{
    public static byte GetFlagWriteByte(Condition.Flag flag, CompareOperator compare)
    {
        int b = ((int)flag) & 0x1F;
        int b2 = ((int)compare) << 5;
        return (byte)(b | b2);
    }

    public static void WriteConditionsList(IReadOnlyList<IConditionGetter> condList, MutagenWriter writer)
    {
        foreach (var cond in condList)
        {
            cond.WriteToBinary(writer);
        }
    }

    public static partial void WriteBinaryFlagsCustom(MutagenWriter writer, IConditionGetter item)
    {
        var flags = item.Flags;
        if (item.Data.UseAliases)
        {
            flags = flags.SetFlag((Condition.Flag)ParametersUseAliases, true);
        }
        if (item.Data.UsePackageData)
        {
            flags = flags.SetFlag((Condition.Flag)ParametersUsePackData, true);
        }

        if (item is IConditionGlobalGetter)
        {
            flags = flags.SetFlag((Condition.Flag)UseGlobal, true);
        }
        writer.Write(GetFlagWriteByte(flags, item.CompareOperator));
    }

    public static void CustomStringExports(MutagenWriter writer, IConditionDataGetter obj)
    {
        if (obj is not IConditionParametersGetter parameters) return;
        if (parameters.StringParameter1 is { } param1)
        {
            using (HeaderExport.Subrecord(writer, RecordTypes.CIS1))
            {
                StringBinaryTranslation.Instance.Write(writer, param1, StringBinaryType.NullTerminate);
            }
        }
        if (parameters.StringParameter2 is { } param2)
        {
            using (HeaderExport.Subrecord(writer, RecordTypes.CIS2))
            {
                StringBinaryTranslation.Instance.Write(writer, param2, StringBinaryType.NullTerminate);
            }
        }
    }

    public static partial void WriteBinaryFunctionParseCustom(MutagenWriter writer, IConditionGetter item)
    {
        switch (item)
        {
            case IConditionGlobalGetter global:
                FormLinkBinaryTranslation.Instance.Write(writer, global.ComparisonValue);
                break;
            case IConditionFloatGetter f:
                FloatBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(writer, f.ComparisonValue);
                break;
            default:
                throw new NotImplementedException();
        }
        
        var data = item.Data;
        writer.Write((ushort)data.Function);
        writer.Write(item.Unknown2);
        data.WriteToBinary(writer);
        
        EnumBinaryTranslation<RunOnType, MutagenFrame, MutagenWriter>.Instance.Write(writer, data.RunOnType, 4);
        FormLinkBinaryTranslation.Instance.Write(writer, data.Reference);
        writer.Write(item.Data.RunOnTypeIndex);
    }
}

abstract partial class ConditionBinaryOverlay
{
    public IConditionDataGetter Data { get; private set; } = null!;

    private static RecordTriggerSpecs IncludeTriggers = new(
        RecordCollection.Factory(
            RecordTypes.CIS1,
            RecordTypes.CIS2));

    public partial Condition.Flag GetFlagsCustom(int location) => ConditionBinaryCreateTranslation.GetFlag(_structData.Span[location])
        .SetFlag((Condition.Flag)Condition.ParametersUseAliases, false)
        .SetFlag((Condition.Flag)Condition.UseGlobal, false);
    public CompareOperator CompareOperator => ConditionBinaryCreateTranslation.GetCompareOperator(_structData.Span[0]);
    public ushort Unknown2 => BinaryPrimitives.ReadUInt16LittleEndian(_structData.Span.Slice(10));

    public static IConditionGetter ConditionFactory(OverlayStream stream, BinaryOverlayFactoryPackage package)
    {
        var subRecMeta = stream.GetSubrecord();
        if (subRecMeta.RecordType != RecordTypes.CTDA)
        {
            throw new ArgumentException();
        }

        var finalPos = stream.Position + subRecMeta.TotalLength;
        Condition.Flag flag = ConditionBinaryCreateTranslation.GetFlag(subRecMeta.Content[0]);
        ConditionBinaryOverlay ret;
        if (flag.HasFlag((Condition.Flag)Condition.UseGlobal))
        {
            ret = (ConditionBinaryOverlay)ConditionGlobalBinaryOverlay.ConditionGlobalFactory(stream, package);
        }
        else
        {
            ret = (ConditionBinaryOverlay)ConditionFloatBinaryOverlay.ConditionFloatFactory(stream, package);
        }

        var functionIndex = BinaryPrimitives.ReadUInt16LittleEndian(ret._structData.Slice(8));
        IConditionData data;
        stream.Position += 12;
        var mutagenFrame = new MutagenFrame(stream);
        if (functionIndex == ConditionBinaryCreateTranslation.EventFunctionIndex)
        {
            data = GetEventDataConditionData.CreateFromBinary(mutagenFrame);
            ConditionBinaryCreateTranslation.FillEndingParams(mutagenFrame, data);
        }
        else
        {
            data = ConditionBinaryCreateTranslation.CreateDataFromBinary(mutagenFrame, functionIndex);
        }
        
        ret.Data = data;
        
        stream.Position = finalPos;
        ConditionBinaryCreateTranslation.CustomStringImports(stream, data);
        data.UseAliases = flag.HasFlag((Condition.Flag)ParametersUseAliases);
        data.UsePackageData = flag.HasFlag((Condition.Flag)ParametersUsePackData);

        return ret;
    }

    public static IConditionGetter ConditionFactory(OverlayStream stream, BinaryOverlayFactoryPackage package, TypedParseParams _)
    {
        return ConditionFactory(stream, package);
    }

    public static IReadOnlyList<IConditionGetter> ConstructBinaryOverlayCountedList(OverlayStream stream, BinaryOverlayFactoryPackage package)
    {
        var counterMeta = stream.ReadSubrecord();
        if (counterMeta.RecordType != RecordTypes.CITC
            || counterMeta.Content.Length != 4)
        {
            throw new ArgumentException();
        }
        var count = BinaryPrimitives.ReadUInt32LittleEndian(counterMeta.Content);
        var ret = ConstructBinaryOverlayList(stream, package);
        if (count != ret.Count)
        {
            throw new ArgumentException("Number of parsed conditions did not matched labeled count.");
        }
        return ret;
    }

    public static IReadOnlyList<IConditionGetter> ConstructBinaryOverlayList(OverlayStream stream, BinaryOverlayFactoryPackage package)
    {
        var span = stream.RemainingMemory;
        var pos = stream.Position;
        var recordLocs = ParseRecordLocations(
            stream: stream,
            finalPos: long.MaxValue,
            constants: package.MetaData.Constants.SubConstants,
            trigger: RecordTypes.CTDA,
            includeTriggers: IncludeTriggers,
            skipHeader: false);
        span = span.Slice(0, stream.Position - pos);
        return BinaryOverlayList.FactoryByArray<IConditionGetter>(
            mem: span,
            package: package,
            getter: (s, p) => ConditionBinaryOverlay.ConditionFactory(new OverlayStream(s, p), p),
            locs: recordLocs);
    }
}
