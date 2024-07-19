using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Starfield.Internals;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;
using System.Buffers.Binary;
using System.Diagnostics;
using static Mutagen.Bethesda.Starfield.Condition;

namespace Mutagen.Bethesda.Starfield;

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
        CommandTarget = 9,
        EventCameraRef = 10,
        MyKiller = 11,
        PlayerShip = 14
    }

    public enum ParameterType
    {
        None,
        AcousticSpace,
        Actor,
        ActorBase,
        ActorValue,
        Alias,
        Alignment,
        AssociationType,
        Axis,
        BiomeMask,
        CastingSource,
        Cell,
        Class,
        ConditionForm,
        CrimeType,
        CriticalStage,
        DamageCauseType,
        DamageType,
        EquipType,
        Event,
        EventData,
        Faction,
        Float,
        Form,
        FormList,
        FormType,
        Furniture,
        FurnitureEntry,
        Global,
        IdleForm,
        Integer,
        Keyword,
        LimbCategory,
        Location,
        MagicEffect,
        MagicItem,
        MiscStat,
        ObjectReference,
        Owner,
        Package,
        Packdata,
        Perk,
        PerkCategory,
        PerkSkillGroup,
        PerkSkillGroupComparison,
        Planet,
        Pronoun,
        Quest,
        QuestStage,
        Race,
        ReactionType,
        ReferencableObject,
        RefType,
        Region,
        ResearchProject,
        Resource,
        Scene,
        Sex,
        SnapTemplateNode,
        SpeechChallenge,
        String,
        VoiceType,
        WardState,
        Weather,
        WorldSpace,
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
        GetValue = 14,
        GetCurrentTime = 18,
        GetScale = 24,
        IsMoving = 25,
        IsTurning = 26,
        GetLineOfSight = 27,
        GetInSameCell = 32,
        GetDisabled = 35,
        MenuPaused = 36,
        GetCameraShipSize = 38,
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
        WouldBeStealing = 79,
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
        GetDistanceFromCelestialBody = 110,
        IsWaiting = 111,
        IsIdlePlaying = 112,
        IsIntimidatedbyPlayer = 116,
        IsPlayerInRegion = 117,
        GetActorAggroRadiusViolated = 118,
        GetCrime = 122,
        IsGreetingPlayer = 123,
        IsGuard = 125,
        GetStaminaPercentage = 128,
        HasBeenRead = 129,
        GetDying = 130,
        GetSceneActionPercent = 131,
        WouldRefuseCommand = 132,
        SameFactionAsPC = 133,
        SameRaceAsPC = 134,
        SameSexAsPC = 135,
        GetIsReference = 136,
        IsTalking = 141,
        GetComponentCount = 142,
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
        HasSameEditorLocationAsRef = 180,
        HasSameEditorLocationAsRefAlias = 181,
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
        IsSpellTarget = 223,
        GetVATSMode = 224,
        GetPersuasionNumber = 225,
        GetVampireFeed = 226,
        GetCannibal = 227,
        GetIsClassDefault = 228,
        GetClassDefaultMatch = 229,
        GetInCellParam = 230,
        GetPlayerDialogueInput = 231,
        GetVatsTargetHeight = 235,
        GetIsGhost = 237,
        GetUnconscious = 242,
        GetRestrained = 244,
        GetIsUsedItem = 246,
        GetIsUsedItemType = 247,
        IsScenePlaying = 248,
        IsInDialogueWithPlayer = 249,
        GetLocationExplored = 250,
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
        AnimAction_EffectHasKeyword = 276,
        GetBaseValue = 277,
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
        GetIsCurrentLocation = 359,
        GetIsCurrentLocationAlias = 360,
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
        IsOwnedBy = 378,
        GetCommandDistance = 380,
        GetCommandLocationDistance = 381,
        GetHitLocation = 390,
        IsPC1stPerson = 391,
        GetCauseofDeath = 396,
        IsWeaponInList = 398,
        IsBribedbyPlayer = 402,
        GetRelationshipRank = 403,
        GetStrongestEnemyHasKeyword = 406,
        GetVATSValue = 407,
        IsKiller = 408,
        IsPlayerInShipTargetingMode = 409,
        GetFactionCombatReaction = 410,
        IsShipTargetInShipTargetingMode = 411,
        Exists = 414,
        GetGroupMemberCount = 415,
        GetGroupTargetCount = 416,
        GetIsVoiceType = 426,
        GetPlantedExplosive = 427,
        IsScenePackageRunning = 429,
        GetHealthPercentage = 430,
        GetActiveBoostDuration = 431,
        GetIsObjectType = 432,
        PlayerVisualDetection = 434,
        PlayerAudioDetection = 435,
        GetIsCreatureType = 437,
        HasKey = 438,
        IsFurnitureEntryType = 439,
        GetInCurrentLocationFormList = 444,
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
        GetPlayerActivated = 483,
        GetFullyEnabledActorsInHigh = 485,
        IsCarryable = 487,
        GetConcussed = 488,
        GetBiomeScanPercent = 490,
        GetMapMarkerVisible = 491,
        PlayerKnows = 493,
        GetPermanentValue = 494,
        EPMagic_EffectIsDetrimental = 495,
        CanPayCrimeGold = 497,
        GetDaysInJail = 499,
        EPAlchemyGetMakingPoison = 500,
        EPAlchemyEffectHasKeyword = 501,
        GetAllowWorldInteractions = 503,
        GetPerkRank = 507,
        GetLastHitCritical = 508,
        LastCrippledCondition = 511,
        HasSharedPowerGrid = 512,
        IsCombatTarget = 513,
        GetVATSRightAreaFree = 515,
        GetVATSLeftAreaFree = 516,
        GetVATSBackAreaFree = 517,
        GetVATSFrontAreaFree = 518,
        GetIsLockBroken = 519,
        IsWindowsPC = 521,
        GetVATSRightTargetVisible = 522,
        GetVATSLeftTargetVisible = 523,
        GetVATSBackTargetVisible = 524,
        GetVATSFrontTargetVisible = 525,
        IsInCriticalStage = 528,
        GetXPForNextLevel = 530,
        GetInfamy = 533,
        GetInfamyViolent = 534,
        GetInfamyNonViolent = 535,
        GetTypeCommandPerforming = 536,
        GetQuestCompleted = 543,
        GetSpeechChallengeSuccessScene = 544,
        IsGoreDisabled = 547,
        IsSceneActionComplete = 550,
        GetActorsInHigh = 554,
        HasLoaded3D = 555,
        HasKeyword = 560,
        HasRefType = 561,
        LocationHasKeyword = 562,
        LocationHasRefType = 563,
        GetIsEditorLocation = 565,
        GetIsAliasRef = 566,
        GetIsEditorLocationAlias = 567,
        IsSprinting = 568,
        IsBlocking = 569,
        HasEquippedSpell = 570,
        GetCurrentCastingType = 571,
        GetCurrentDeliveryType = 572,
        GetAttackState = 574,
        GetEventData = 576,
        IsCloserToAThanB = 577,
        LevelMinusPCLevel = 578,
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
        GetLocationAliasExplored = 598,
        GetLocationAliasRefTypeDeadCount = 600,
        GetLocationAliasRefTypeAliveCount = 601,
        IsWardState = 602,
        IsInSameCurrentLocationAsRef = 603,
        IsInSameCurrentLocationAsAlias = 604,
        LocationAliasIsLocation = 605,
        GetKeywordDataForLocation = 606,
        GetKeywordDataForAlias = 608,
        LocationAliasHasKeyword = 610,
        IsNullPackageData = 611,
        GetNumericPackageData = 612,
        IsPlayerRadioOn = 613,
        GetPlayerRadioFrequency = 614,
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
        IsLocationAliasLoaded = 626,
        IsDualCasting = 627,
        GetVMQuestVariable = 629,
        GetCombatAudioDetection = 630,
        GetCombatVisualDetection = 631,
        IsCasting = 632,
        GetFlyingState = 633,
        IsInFavorState = 635,
        HasTwoHandedWeaponEquipped = 636,
        IsFurnitureExitType = 637,
        IsInFriendStatewithPlayer = 638,
        GetWithinDistance = 639,
        GetValuePercent = 640,
        IsUnique = 641,
        GetLastBumpDirection = 642,
        GetInfoChallangeSuccess = 644,
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
        HasVMScript = 659,
        GetVMScriptVariable = 660,
        GetWorkshopResourceDamage = 661,
        HasValidRumorTopic = 664,
        IsAttacking = 672,
        IsPowerAttacking = 673,
        IsLastHostileActor = 674,
        GetGraphVariableInt = 675,
        GetDockerOrientation = 676,
        ShouldAttackKill = 678,
        GetActivationHeight = 680,
        SSLPI_ReplacePayloadHasKeyword = 681,
        WornHasKeyword = 682,
        GetPathingCurrentSpeed = 683,
        GetPathingCurrentSpeedAngle = 684,
        GetWorkshopObjectCount = 691,
        EPMagic_SpellHasKeyword = 693,
        GetNoBleedoutRecovery = 694,
        EPMagic_SpellHasSkill = 696,
        IsAttackType = 697,
        IsAllowedToFly = 698,
        HasMagicEffectOrSpellKeyword = 699,
        IsCommandedActor = 700,
        IsStaggered = 701,
        IsRecoiling = 702,
        HasScopeWeaponEquipped = 703,
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
        GetKnockStateEnum = 725,
        DoesNotExist = 726,
        GetSpeechChallengeSuccessGame = 728,
        GetActorStance = 729,
        SpeechScenarioHasKeyword = 730,
        CanProduceForWorkshop = 734,
        CanFlyHere = 735,
        EPIsDamageType = 736,
        IsCurrentSpeechChallengeObject = 737,
        GetActorGunState = 738,
        GetVoiceLineLength = 739,
        ObjectTemplateItem_HasKeyword = 741,
        ObjectTemplateItem_HasUniqueKeyword = 742,
        ObjectTemplateItem_GetLevel = 743,
        MovementIdleMatches = 744,
        GetActionData = 745,
        GetActionDataShort = 746,
        GetActionDataByte = 747,
        GetActionDataFlag = 748,
        ModdedItemHasKeyword = 749,
        GetAngryWithPlayer = 750,
        IsCameraUnderWater = 751,
        IsActorRefOwner = 753,
        HasActorRefOwner = 754,
        GetLoadedAmmoCount = 756,
        IsTimeSpanSunrise = 757,
        IsTimeSpanMorning = 758,
        IsTimeSpanAfternoon = 759,
        IsTimeSpanEvening = 760,
        IsTimeSpanSunset = 761,
        IsTimeSpanNight = 762,
        IsTimeSpanMidnight = 763,
        IsTimeSpanAnyDay = 764,
        IsTimeSpanAnyNight = 765,
        CurrentFurnitureHasKeyword = 766,
        GetWeaponEquipIndex = 767,
        IsOverEncumbered = 769,
        IsPackageRequestingBlockedIdles = 770,
        GetActionDataInt = 771,
        GetVATSRightMinusLeftAreaFree = 772,
        GetInIronSights = 773,
        GetActorStaggerDirection = 774,
        GetActorStaggerMagnitude = 775,
        WornCoversBipedSlot = 776,
        GetInventoryValue = 777,
        IsPlayerInConversation = 778,
        IsInDialogueCamera = 779,
        IsMyDialogueTargetPlayer = 780,
        IsMyDialogueTargetActor = 781,
        GetMyDialogueTargetDistance = 782,
        IsSeatOccupied = 783,
        IsPlayerRiding = 784,
        IsTryingEventCamera = 785,
        UseLeftSideCamera = 786,
        GetNoteType = 787,
        LocationHasPlayerOwnedWorkshop = 788,
        IsStartingAction = 789,
        IsMidAction = 790,
        IsWeaponChargeAttack = 791,
        IsInWorkshopMode = 792,
        IsWeaponChargingHoldAttack = 793,
        IsEncounterAbovePlayerLevel = 794,
        IsMeleeAttacking = 795,
        GetVATSQueuedTargetsUnique = 796,
        GetCurrentLocationExplored = 797,
        IsPowered = 798,
        GetTransmitterDistance = 799,
        GetCameraPlaybackTime = 800,
        IsInWater = 801,
        GetWithinActivateDistance = 802,
        IsUnderWater = 803,
        IsInSameSpace = 804,
        LocationAllowsReset = 805,
        GetVATSBackRightAreaFree = 806,
        GetVATSBackLeftAreaFree = 807,
        GetVATSBackRightTargetVisible = 808,
        GetVATSBackLeftTargetVisible = 809,
        GetVATSTargetLimbVisible = 810,
        IsPlayerListening = 811,
        GetPathingRequestedQuickTurn = 812,
        EPIsCalculatingBaseDamage = 813,
        GetReanimating = 814,
        GetCombatDialogueDataInt = 815,
        IsDocked = 816,
        IsDockedWith = 817,
        GetLastDialogueCameraHasKeyword = 818,
        GetActionDataForm = 819,
        IsInSpace = 820,
        GetSpaceship = 822,
        ShipContainsRef = 823,
        IsInSpaceship = 824,
        ShipHasActorInPilotSeat = 825,
        ActorPackageHasRandomConversationsFlagOn = 826,
        GetActorInShipPilotSeat = 827,
        IsSpaceship = 828,
        GetInAcousticSpace = 829,
        CurrentShipLanded = 830,
        IsDockedAsChild = 831,
        PlayerHailResponse = 832,
        IsHerdLeader = 833,
        HasHerdLeader = 834,
        GetPlayerHomeSpaceShip = 836,
        IsTrueForConditionForm = 837,
        GetNumElementsInRefCollection = 838,
        GetCurrentWeatherHasKeyword = 839,
        IsSnappedTo = 840,
        HasKeywordOnNode = 841,
        HasKeywordOnStacked = 842,
        HasVisualDetection = 843,
        HasSoundDetection = 844,
        IsSuppressed = 845,
        IsSpaceshipEngineDestroyed = 846,
        IsLanded = 847,
        IsSpaceshipShieldsDestroyed = 848,
        IsSpaceshipGravDriveDestroyed = 849,
        GetNumberOfSpaceshipWeaponsDestroyed = 850,
        GetIsCurrentLocationExact = 851,
        GetIsEditorLocationExact = 852,
        IsInThreatBackdown = 853,
        IsInsidePrimitive = 854,
        GetCameraActorCount = 855,
        GetIsCurrentLocationAliasExact = 856,
        IsJailInSystem = 857,
        BodyHasKeyword = 858,
        BiomeHasKeyword = 859,
        SystemHasKeyword = 860,
        GetDistanceFromLocationWithKeyword = 861,
        GetPlanetVisited = 862,
        IsLocalDay = 863,
        SpeechChallengePreviousSceneHasKeyword = 864,
        GetBiomeMaskValue = 865,
        BodyIsType = 866,
        BodyIsAtmosphereType = 867,
        BodyIsTemperatureType = 868,
        GetBodyTemperature = 869,
        GetBodyPressure = 870,
        GetBodyGravity = 871,
        GetBodySurveyPercent = 872,
        IsPlayerLoitering = 873,
        IsResearchComplete = 874,
        BodyIsPlanetTraitKnown = 875,
        HasPerkCategory = 876,
        HasPerkSkillGroup = 877,
        CountAquiredPerkRanksByType = 878,
        IsScanned = 879,
        IsScannableKeywordRevealed = 880,
        IsMyVictim = 881,
        GetResourceScarcity = 882,
        CheckContrabandStatus = 883,
        IsPlayerSpaceFarTravelling = 886,
        IsPlayerSpaceFarTravelDeparture = 887,
        IsPlayerSpaceFarTravelArrival = 888,
        BiomeHasWeather = 889,
        GetSystemSurveyPercent = 891,
        SystemBodyHasKeyword = 892,
        GetShipGroupThreatRatio = 894,
        IsOnGrazingTerrain = 895,
        GetDistanceGalacticParsec = 896,
        GetDistanceGalacticMegaMeter = 897,
        GetShipToShipGroupThreatRatio = 898,
        GetGroupMembersInRadiusCount = 899,
        GetShipPiracyValue = 900,
        GetDistanceFromCelestialBodyAliasParsecs = 901,
        GetDistanceFromCelestialBodyAliasMegaMeters = 902,
        IsInsidePrimitiveTopAndBottom = 904,
        GetPlayerBountyCrimeFaction = 905,
        GetIsFloating = 906,
        LocationOrParentHasKeyword = 907,
        IsCelestialBodyScanned = 908,
        IsActorReactionInCooldown = 912,
        BiomeSupportsCreature = 916,
        EPMagic_SpellHasMagicEffect = 919,
        IsFacingActor = 921,
        IsSameVoiceType = 922,
        GetValueCurrentLocation = 923,
        IsBoostPackActive = 924,
        GetTimeSinceLastBoostPackEnded = 925,
        EPGetLastCombatHitCritical = 926,
        EPGetLastCombatHitKill = 927,
        EPGetLastCombatHitGunBash = 928,
        EPIsLastCombatHitLimbInCategory = 929,
        IsEditorLocationInsidePrimitive = 930,
        GetIsPronoun = 931,
        GetDistanceGalacticLightYears = 932,
        GetDistanceFromCelestialBodyAliasLightyears = 933,
        IsOnPlayerHomeSpaceShip = 934,
        EPMagic_EffectHasKeyword = 935,
        EPMagic_SpellIs = 936,
        IsPlayerSteadyingWeapon = 937,
        ResourceVeinHasKeyword = 938,
        GetLastCombatHitActorConsecutiveHits = 939,
        GetCurrentAndLastWeatherHaveKeyword = 940,
        IsPCEquippedWeaponNthAttack = 941,
        GetWaterDepth = 942,
        AreHostileActorsNear = 943,
        GetPlayerGravityScale = 944,
        IsInSameGroup = 945,
        IsBoostPackHovering = 946,
        GetUsedWeightCapacityConditionFunction = 947,
        BodyHasResource = 949,
        GetPCIsReloading = 950,
        ActorExposedToSky = 951,
        GetQuestStarting = 954,
        BodyHasResourceWithKeyword = 955,
        GetShipReactorClass = 957,
        ShipReactorHasClassKeyword = 958,
        EPIsResistanceActorValue = 960,
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
            42 => (ParameterType.Actor, ParameterType.None),
            43 => (ParameterType.Actor, ParameterType.None),
            44 => (ParameterType.Actor, ParameterType.None),
            45 => (ParameterType.Actor, ParameterType.None),
            47 => (ParameterType.ReferencableObject, ParameterType.None),
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
            79 => (ParameterType.ObjectReference, ParameterType.None),
            84 => (ParameterType.ActorBase, ParameterType.None),
            98 => (ParameterType.Integer, ParameterType.Integer),
            99 => (ParameterType.ObjectReference, ParameterType.None),
            109 => (ParameterType.ActorValue, ParameterType.None),
            110 => (ParameterType.Planet, ParameterType.Integer),
            117 => (ParameterType.Region, ParameterType.None),
            122 => (ParameterType.Actor, ParameterType.CrimeType),
            131 => (ParameterType.Scene, ParameterType.Integer),
            132 => (ParameterType.ObjectReference, ParameterType.None),
            136 => (ParameterType.ObjectReference, ParameterType.None),
            142 => (ParameterType.ReferencableObject, ParameterType.None),
            149 => (ParameterType.Weather, ParameterType.None),
            152 => (ParameterType.Faction, ParameterType.None),
            161 => (ParameterType.Package, ParameterType.None),
            162 => (ParameterType.ObjectReference, ParameterType.None),
            163 => (ParameterType.Furniture, ParameterType.None),
            172 => (ParameterType.Actor, ParameterType.None),
            180 => (ParameterType.ObjectReference, ParameterType.Keyword),
            181 => (ParameterType.Alias, ParameterType.Keyword),
            182 => (ParameterType.ReferencableObject, ParameterType.None),
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
            276 => (ParameterType.Keyword, ParameterType.None),
            277 => (ParameterType.ActorValue, ParameterType.None),
            278 => (ParameterType.Owner, ParameterType.None),
            280 => (ParameterType.Cell, ParameterType.Owner),
            289 => (ParameterType.Integer, ParameterType.None),
            310 => (ParameterType.WorldSpace, ParameterType.None),
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
            378 => (ParameterType.Actor, ParameterType.None),
            396 => (ParameterType.DamageCauseType, ParameterType.None),
            398 => (ParameterType.FormList, ParameterType.None),
            403 => (ParameterType.Actor, ParameterType.None),
            406 => (ParameterType.Keyword, ParameterType.None),
            407 => (ParameterType.Integer, ParameterType.Integer),
            408 => (ParameterType.Actor, ParameterType.None),
            410 => (ParameterType.Faction, ParameterType.Faction),
            414 => (ParameterType.ObjectReference, ParameterType.None),
            426 => (ParameterType.VoiceType, ParameterType.None),
            432 => (ParameterType.FormType, ParameterType.None),
            437 => (ParameterType.Integer, ParameterType.None),
            438 => (ParameterType.ObjectReference, ParameterType.None),
            439 => (ParameterType.FurnitureEntry, ParameterType.None),
            444 => (ParameterType.FormList, ParameterType.None),
            445 => (ParameterType.Location, ParameterType.None),
            446 => (ParameterType.Axis, ParameterType.None),
            447 => (ParameterType.String, ParameterType.None),
            448 => (ParameterType.Perk, ParameterType.None),
            449 => (ParameterType.Actor, ParameterType.None),
            450 => (ParameterType.IdleForm, ParameterType.None),
            459 => (ParameterType.Faction, ParameterType.None),
            463 => (ParameterType.ObjectReference, ParameterType.None),
            465 => (ParameterType.Keyword, ParameterType.None),
            473 => (ParameterType.Alignment, ParameterType.None),
            477 => (ParameterType.Actor, ParameterType.None),
            479 => (ParameterType.EquipType, ParameterType.None),
            493 => (ParameterType.ReferencableObject, ParameterType.None),
            494 => (ParameterType.ActorValue, ParameterType.None),
            497 => (ParameterType.Faction, ParameterType.None),
            501 => (ParameterType.Keyword, ParameterType.None),
            507 => (ParameterType.Perk, ParameterType.None),
            511 => (ParameterType.ActorValue, ParameterType.None),
            512 => (ParameterType.ObjectReference, ParameterType.None),
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
            612 => (ParameterType.Packdata, ParameterType.None),
            617 => (ParameterType.AssociationType, ParameterType.None),
            619 => (ParameterType.Axis, ParameterType.None),
            620 => (ParameterType.Axis, ParameterType.None),
            622 => (ParameterType.Axis, ParameterType.None),
            624 => (ParameterType.ObjectReference, ParameterType.None),
            625 => (ParameterType.Location, ParameterType.None),
            626 => (ParameterType.Alias, ParameterType.None),
            629 => (ParameterType.Quest, ParameterType.String),
            637 => (ParameterType.FurnitureEntry, ParameterType.None),
            639 => (ParameterType.ObjectReference, ParameterType.Float),
            640 => (ParameterType.ActorValue, ParameterType.None),
            650 => (ParameterType.ObjectReference, ParameterType.Keyword),
            651 => (ParameterType.Keyword, ParameterType.None),
            652 => (ParameterType.ObjectReference, ParameterType.None),
            659 => (ParameterType.String, ParameterType.None),
            660 => (ParameterType.String, ParameterType.String),
            661 => (ParameterType.ActorValue, ParameterType.None),
            664 => (ParameterType.Quest, ParameterType.None),
            675 => (ParameterType.String, ParameterType.None),
            678 => (ParameterType.Actor, ParameterType.None),
            681 => (ParameterType.Keyword, ParameterType.None),
            682 => (ParameterType.Keyword, ParameterType.None),
            684 => (ParameterType.Axis, ParameterType.None),
            691 => (ParameterType.ReferencableObject, ParameterType.None),
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
            730 => (ParameterType.Keyword, ParameterType.None),
            736 => (ParameterType.DamageType, ParameterType.None),
            737 => (ParameterType.SpeechChallenge, ParameterType.None),
            741 => (ParameterType.Keyword, ParameterType.None),
            742 => (ParameterType.Keyword, ParameterType.None),
            744 => (ParameterType.Integer, ParameterType.Integer),
            746 => (ParameterType.Integer, ParameterType.None),
            747 => (ParameterType.Integer, ParameterType.None),
            748 => (ParameterType.Integer, ParameterType.None),
            749 => (ParameterType.Keyword, ParameterType.None),
            753 => (ParameterType.Actor, ParameterType.None),
            754 => (ParameterType.Actor, ParameterType.None),
            766 => (ParameterType.Keyword, ParameterType.None),
            772 => (ParameterType.ObjectReference, ParameterType.None),
            773 => (ParameterType.ObjectReference, ParameterType.None),
            776 => (ParameterType.Integer, ParameterType.None),
            783 => (ParameterType.Keyword, ParameterType.None),
            802 => (ParameterType.ObjectReference, ParameterType.None),
            804 => (ParameterType.ObjectReference, ParameterType.None),
            806 => (ParameterType.ObjectReference, ParameterType.None),
            807 => (ParameterType.ObjectReference, ParameterType.None),
            808 => (ParameterType.ObjectReference, ParameterType.None),
            809 => (ParameterType.ObjectReference, ParameterType.None),
            810 => (ParameterType.ObjectReference, ParameterType.None),
            811 => (ParameterType.Float, ParameterType.None),
            817 => (ParameterType.ObjectReference, ParameterType.None),
            818 => (ParameterType.Keyword, ParameterType.None),
            819 => (ParameterType.Form, ParameterType.None),
            822 => (ParameterType.ObjectReference, ParameterType.None),
            823 => (ParameterType.ObjectReference, ParameterType.None),
            827 => (ParameterType.ObjectReference, ParameterType.None),
            829 => (ParameterType.AcousticSpace, ParameterType.None),
            833 => (ParameterType.Actor, ParameterType.None),
            834 => (ParameterType.Actor, ParameterType.None),
            837 => (ParameterType.ConditionForm, ParameterType.None),
            838 => (ParameterType.Alias, ParameterType.None),
            839 => (ParameterType.Keyword, ParameterType.None),
            840 => (ParameterType.ObjectReference, ParameterType.SnapTemplateNode),
            841 => (ParameterType.Keyword, ParameterType.None),
            842 => (ParameterType.Keyword, ParameterType.None),
            843 => (ParameterType.Actor, ParameterType.None),
            844 => (ParameterType.Actor, ParameterType.None),
            851 => (ParameterType.Location, ParameterType.None),
            852 => (ParameterType.Location, ParameterType.None),
            854 => (ParameterType.Keyword, ParameterType.None),
            856 => (ParameterType.Alias, ParameterType.None),
            858 => (ParameterType.Keyword, ParameterType.None),
            859 => (ParameterType.Keyword, ParameterType.None),
            860 => (ParameterType.Keyword, ParameterType.None),
            861 => (ParameterType.Keyword, ParameterType.None),
            864 => (ParameterType.Keyword, ParameterType.None),
            865 => (ParameterType.BiomeMask, ParameterType.BiomeMask),
            866 => (ParameterType.Keyword, ParameterType.None),
            867 => (ParameterType.Keyword, ParameterType.None),
            868 => (ParameterType.Keyword, ParameterType.None),
            874 => (ParameterType.ResearchProject, ParameterType.None),
            875 => (ParameterType.Keyword, ParameterType.None),
            876 => (ParameterType.PerkCategory, ParameterType.None),
            877 => (ParameterType.PerkSkillGroupComparison, ParameterType.PerkSkillGroup),
            878 => (ParameterType.PerkCategory, ParameterType.PerkSkillGroupComparison),
            880 => (ParameterType.Keyword, ParameterType.None),
            881 => (ParameterType.Actor, ParameterType.Integer),
            883 => (ParameterType.Integer, ParameterType.None),
            889 => (ParameterType.Weather, ParameterType.None),
            892 => (ParameterType.Keyword, ParameterType.None),
            894 => (ParameterType.ObjectReference, ParameterType.None),
            896 => (ParameterType.ObjectReference, ParameterType.None),
            897 => (ParameterType.ObjectReference, ParameterType.None),
            898 => (ParameterType.ObjectReference, ParameterType.None),
            899 => (ParameterType.Global, ParameterType.None),
            900 => (ParameterType.ObjectReference, ParameterType.None),
            901 => (ParameterType.Alias, ParameterType.None),
            902 => (ParameterType.Alias, ParameterType.None),
            904 => (ParameterType.Keyword, ParameterType.None),
            905 => (ParameterType.Faction, ParameterType.None),
            907 => (ParameterType.Keyword, ParameterType.None),
            912 => (ParameterType.ReactionType, ParameterType.None),
            916 => (ParameterType.ActorBase, ParameterType.None),
            919 => (ParameterType.MagicEffect, ParameterType.None),
            921 => (ParameterType.ObjectReference, ParameterType.None),
            922 => (ParameterType.ObjectReference, ParameterType.None),
            923 => (ParameterType.ActorValue, ParameterType.None),
            929 => (ParameterType.LimbCategory, ParameterType.None),
            930 => (ParameterType.Keyword, ParameterType.None),
            931 => (ParameterType.Pronoun, ParameterType.None),
            932 => (ParameterType.ObjectReference, ParameterType.None),
            933 => (ParameterType.Alias, ParameterType.None),
            935 => (ParameterType.Keyword, ParameterType.None),
            936 => (ParameterType.MagicItem, ParameterType.None),
            938 => (ParameterType.Keyword, ParameterType.None),
            940 => (ParameterType.Keyword, ParameterType.None),
            941 => (ParameterType.Integer, ParameterType.None),
            945 => (ParameterType.Actor, ParameterType.None),
            949 => (ParameterType.Resource, ParameterType.Integer),
            954 => (ParameterType.Quest, ParameterType.None),
            955 => (ParameterType.Keyword, ParameterType.Integer),
            958 => (ParameterType.Keyword, ParameterType.None),
            960 => (ParameterType.ActorValue, ParameterType.None),
            _ => (ParameterType.None, ParameterType.None),
        };
    }

    public static Condition CreateFromBinary(
        MutagenFrame frame,
        TypedParseParams translationParams)
    {
        if (!frame.Reader.TryGetSubrecordHeader(Mutagen.Bethesda.Starfield.Internals.RecordTypes.CTDA, out var subRecMeta))
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
        if (funcData is not IConditionStringParameter stringParameter) return;
        if (!frame.TryGetSubrecord(out var subMeta)) return;
        switch (subMeta.RecordType.TypeInt)
        {
            case RecordTypeInts.CIS1:
                stringParameter.FirstStringParameter =
                    BinaryStringUtility.ProcessWholeToZString(subMeta.Content, frame.MetaData.Encodings.NonTranslated);
                break;
            case RecordTypeInts.CIS2:
                stringParameter.SecondStringParameter =
                    BinaryStringUtility.ProcessWholeToZString(subMeta.Content, frame.MetaData.Encodings.NonTranslated);
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
        item.RunOnType =
            EnumBinaryTranslation<Condition.RunOnType, MutagenFrame, MutagenWriter>.Instance.Parse(
                reader: frame.SpawnWithLength(4));
        item.Reference.SetTo(
            FormLinkBinaryTranslation.Instance.Parse(
                reader: frame,
                defaultVal: FormKey.Null));
        item.Unknown3 = frame.ReadInt32();
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
                return GetValueConditionData.CreateFromBinary(frame);
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
                return MenuPausedConditionData.CreateFromBinary(frame);
            case 38:
                return GetCameraShipSizeConditionData.CreateFromBinary(frame);
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
                return WouldBeStealingConditionData.CreateFromBinary(frame);
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
                return GetDistanceFromCelestialBodyConditionData.CreateFromBinary(frame);
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
            case 128:
                return GetStaminaPercentageConditionData.CreateFromBinary(frame);
            case 129:
                return HasBeenReadConditionData.CreateFromBinary(frame);
            case 130:
                return GetDyingConditionData.CreateFromBinary(frame);
            case 131:
                return GetSceneActionPercentConditionData.CreateFromBinary(frame);
            case 132:
                return WouldRefuseCommandConditionData.CreateFromBinary(frame);
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
                return GetComponentCountConditionData.CreateFromBinary(frame);
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
                return HasSameEditorLocationAsRefConditionData.CreateFromBinary(frame);
            case 181:
                return HasSameEditorLocationAsRefAliasConditionData.CreateFromBinary(frame);
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
            case 231:
                return GetPlayerDialogueInputConditionData.CreateFromBinary(frame);
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
                return GetLocationExploredConditionData.CreateFromBinary(frame);
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
            case 276:
                return AnimAction_EffectHasKeywordConditionData.CreateFromBinary(frame);
            case 277:
                return GetBaseValueConditionData.CreateFromBinary(frame);
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
                return GetIsCurrentLocationConditionData.CreateFromBinary(frame);
            case 360:
                return GetIsCurrentLocationAliasConditionData.CreateFromBinary(frame);
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
                return IsOwnedByConditionData.CreateFromBinary(frame);
            case 380:
                return GetCommandDistanceConditionData.CreateFromBinary(frame);
            case 381:
                return GetCommandLocationDistanceConditionData.CreateFromBinary(frame);
            case 390:
                return GetHitLocationConditionData.CreateFromBinary(frame);
            case 391:
                return IsPC1stPersonConditionData.CreateFromBinary(frame);
            case 396:
                return GetCauseofDeathConditionData.CreateFromBinary(frame);
            case 398:
                return IsWeaponInListConditionData.CreateFromBinary(frame);
            case 402:
                return IsBribedbyPlayerConditionData.CreateFromBinary(frame);
            case 403:
                return GetRelationshipRankConditionData.CreateFromBinary(frame);
            case 406:
                return GetStrongestEnemyHasKeywordConditionData.CreateFromBinary(frame);
            case 407:
                return GetVATSValueConditionData.CreateFromBinary(frame);
            case 408:
                return IsKillerConditionData.CreateFromBinary(frame);
            case 409:
                return IsPlayerInShipTargetingModeConditionData.CreateFromBinary(frame);
            case 410:
                return GetFactionCombatReactionConditionData.CreateFromBinary(frame);
            case 411:
                return IsShipTargetInShipTargetingModeConditionData.CreateFromBinary(frame);
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
            case 431:
                return GetActiveBoostDurationConditionData.CreateFromBinary(frame);
            case 432:
                return GetIsObjectTypeConditionData.CreateFromBinary(frame);
            case 434:
                return PlayerVisualDetectionConditionData.CreateFromBinary(frame);
            case 435:
                return PlayerAudioDetectionConditionData.CreateFromBinary(frame);
            case 437:
                return GetIsCreatureTypeConditionData.CreateFromBinary(frame);
            case 438:
                return HasKeyConditionData.CreateFromBinary(frame);
            case 439:
                return IsFurnitureEntryTypeConditionData.CreateFromBinary(frame);
            case 444:
                return GetInCurrentLocationFormListConditionData.CreateFromBinary(frame);
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
            case 483:
                return GetPlayerActivatedConditionData.CreateFromBinary(frame);
            case 485:
                return GetFullyEnabledActorsInHighConditionData.CreateFromBinary(frame);
            case 487:
                return IsCarryableConditionData.CreateFromBinary(frame);
            case 488:
                return GetConcussedConditionData.CreateFromBinary(frame);
            case 490:
                return GetBiomeScanPercentConditionData.CreateFromBinary(frame);
            case 491:
                return GetMapMarkerVisibleConditionData.CreateFromBinary(frame);
            case 493:
                return PlayerKnowsConditionData.CreateFromBinary(frame);
            case 494:
                return GetPermanentValueConditionData.CreateFromBinary(frame);
            case 495:
                return EPMagic_EffectIsDetrimentalConditionData.CreateFromBinary(frame);
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
            case 507:
                return GetPerkRankConditionData.CreateFromBinary(frame);
            case 508:
                return GetLastHitCriticalConditionData.CreateFromBinary(frame);
            case 511:
                return LastCrippledConditionConditionData.CreateFromBinary(frame);
            case 512:
                return HasSharedPowerGridConditionData.CreateFromBinary(frame);
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
                return GetIsLockBrokenConditionData.CreateFromBinary(frame);
            case 521:
                return IsWindowsPCConditionData.CreateFromBinary(frame);
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
            case 536:
                return GetTypeCommandPerformingConditionData.CreateFromBinary(frame);
            case 543:
                return GetQuestCompletedConditionData.CreateFromBinary(frame);
            case 544:
                return GetSpeechChallengeSuccessSceneConditionData.CreateFromBinary(frame);
            case 547:
                return IsGoreDisabledConditionData.CreateFromBinary(frame);
            case 550:
                return IsSceneActionCompleteConditionData.CreateFromBinary(frame);
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
                return GetIsEditorLocationAliasConditionData.CreateFromBinary(frame);
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
            case 578:
                return LevelMinusPCLevelConditionData.CreateFromBinary(frame);
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
                return GetLocationAliasExploredConditionData.CreateFromBinary(frame);
            case 600:
                return GetLocationAliasRefTypeDeadCountConditionData.CreateFromBinary(frame);
            case 601:
                return GetLocationAliasRefTypeAliveCountConditionData.CreateFromBinary(frame);
            case 602:
                return IsWardStateConditionData.CreateFromBinary(frame);
            case 603:
                return IsInSameCurrentLocationAsRefConditionData.CreateFromBinary(frame);
            case 604:
                return IsInSameCurrentLocationAsAliasConditionData.CreateFromBinary(frame);
            case 605:
                return LocationAliasIsLocationConditionData.CreateFromBinary(frame);
            case 606:
                return GetKeywordDataForLocationConditionData.CreateFromBinary(frame);
            case 608:
                return GetKeywordDataForAliasConditionData.CreateFromBinary(frame);
            case 610:
                return LocationAliasHasKeywordConditionData.CreateFromBinary(frame);
            case 611:
                return IsNullPackageDataConditionData.CreateFromBinary(frame);
            case 612:
                return GetNumericPackageDataConditionData.CreateFromBinary(frame);
            case 613:
                return IsPlayerRadioOnConditionData.CreateFromBinary(frame);
            case 614:
                return GetPlayerRadioFrequencyConditionData.CreateFromBinary(frame);
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
                return IsLocationAliasLoadedConditionData.CreateFromBinary(frame);
            case 627:
                return IsDualCastingConditionData.CreateFromBinary(frame);
            case 629:
                return GetVMQuestVariableConditionData.CreateFromBinary(frame);
            case 630:
                return GetCombatAudioDetectionConditionData.CreateFromBinary(frame);
            case 631:
                return GetCombatVisualDetectionConditionData.CreateFromBinary(frame);
            case 632:
                return IsCastingConditionData.CreateFromBinary(frame);
            case 633:
                return GetFlyingStateConditionData.CreateFromBinary(frame);
            case 635:
                return IsInFavorStateConditionData.CreateFromBinary(frame);
            case 636:
                return HasTwoHandedWeaponEquippedConditionData.CreateFromBinary(frame);
            case 637:
                return IsFurnitureExitTypeConditionData.CreateFromBinary(frame);
            case 638:
                return IsInFriendStatewithPlayerConditionData.CreateFromBinary(frame);
            case 639:
                return GetWithinDistanceConditionData.CreateFromBinary(frame);
            case 640:
                return GetValuePercentConditionData.CreateFromBinary(frame);
            case 641:
                return IsUniqueConditionData.CreateFromBinary(frame);
            case 642:
                return GetLastBumpDirectionConditionData.CreateFromBinary(frame);
            case 644:
                return GetInfoChallangeSuccessConditionData.CreateFromBinary(frame);
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
                return HasVMScriptConditionData.CreateFromBinary(frame);
            case 660:
                return GetVMScriptVariableConditionData.CreateFromBinary(frame);
            case 661:
                return GetWorkshopResourceDamageConditionData.CreateFromBinary(frame);
            case 664:
                return HasValidRumorTopicConditionData.CreateFromBinary(frame);
            case 672:
                return IsAttackingConditionData.CreateFromBinary(frame);
            case 673:
                return IsPowerAttackingConditionData.CreateFromBinary(frame);
            case 674:
                return IsLastHostileActorConditionData.CreateFromBinary(frame);
            case 675:
                return GetGraphVariableIntConditionData.CreateFromBinary(frame);
            case 676:
                return GetDockerOrientationConditionData.CreateFromBinary(frame);
            case 678:
                return ShouldAttackKillConditionData.CreateFromBinary(frame);
            case 680:
                return GetActivationHeightConditionData.CreateFromBinary(frame);
            case 681:
                return SSLPI_ReplacePayloadHasKeywordConditionData.CreateFromBinary(frame);
            case 682:
                return WornHasKeywordConditionData.CreateFromBinary(frame);
            case 683:
                return GetPathingCurrentSpeedConditionData.CreateFromBinary(frame);
            case 684:
                return GetPathingCurrentSpeedAngleConditionData.CreateFromBinary(frame);
            case 691:
                return GetWorkshopObjectCountConditionData.CreateFromBinary(frame);
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
                return HasMagicEffectOrSpellKeywordConditionData.CreateFromBinary(frame);
            case 700:
                return IsCommandedActorConditionData.CreateFromBinary(frame);
            case 701:
                return IsStaggeredConditionData.CreateFromBinary(frame);
            case 702:
                return IsRecoilingConditionData.CreateFromBinary(frame);
            case 703:
                return HasScopeWeaponEquippedConditionData.CreateFromBinary(frame);
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
                return GetKnockStateEnumConditionData.CreateFromBinary(frame);
            case 726:
                return DoesNotExistConditionData.CreateFromBinary(frame);
            case 728:
                return GetSpeechChallengeSuccessGameConditionData.CreateFromBinary(frame);
            case 729:
                return GetActorStanceConditionData.CreateFromBinary(frame);
            case 730:
                return SpeechScenarioHasKeywordConditionData.CreateFromBinary(frame);
            case 734:
                return CanProduceForWorkshopConditionData.CreateFromBinary(frame);
            case 735:
                return CanFlyHereConditionData.CreateFromBinary(frame);
            case 736:
                return EPIsDamageTypeConditionData.CreateFromBinary(frame);
            case 737:
                return IsCurrentSpeechChallengeObjectConditionData.CreateFromBinary(frame);
            case 738:
                return GetActorGunStateConditionData.CreateFromBinary(frame);
            case 739:
                return GetVoiceLineLengthConditionData.CreateFromBinary(frame);
            case 741:
                return ObjectTemplateItem_HasKeywordConditionData.CreateFromBinary(frame);
            case 742:
                return ObjectTemplateItem_HasUniqueKeywordConditionData.CreateFromBinary(frame);
            case 743:
                return ObjectTemplateItem_GetLevelConditionData.CreateFromBinary(frame);
            case 744:
                return MovementIdleMatchesConditionData.CreateFromBinary(frame);
            case 745:
                return GetActionDataConditionData.CreateFromBinary(frame);
            case 746:
                return GetActionDataShortConditionData.CreateFromBinary(frame);
            case 747:
                return GetActionDataByteConditionData.CreateFromBinary(frame);
            case 748:
                return GetActionDataFlagConditionData.CreateFromBinary(frame);
            case 749:
                return ModdedItemHasKeywordConditionData.CreateFromBinary(frame);
            case 750:
                return GetAngryWithPlayerConditionData.CreateFromBinary(frame);
            case 751:
                return IsCameraUnderWaterConditionData.CreateFromBinary(frame);
            case 753:
                return IsActorRefOwnerConditionData.CreateFromBinary(frame);
            case 754:
                return HasActorRefOwnerConditionData.CreateFromBinary(frame);
            case 756:
                return GetLoadedAmmoCountConditionData.CreateFromBinary(frame);
            case 757:
                return IsTimeSpanSunriseConditionData.CreateFromBinary(frame);
            case 758:
                return IsTimeSpanMorningConditionData.CreateFromBinary(frame);
            case 759:
                return IsTimeSpanAfternoonConditionData.CreateFromBinary(frame);
            case 760:
                return IsTimeSpanEveningConditionData.CreateFromBinary(frame);
            case 761:
                return IsTimeSpanSunsetConditionData.CreateFromBinary(frame);
            case 762:
                return IsTimeSpanNightConditionData.CreateFromBinary(frame);
            case 763:
                return IsTimeSpanMidnightConditionData.CreateFromBinary(frame);
            case 764:
                return IsTimeSpanAnyDayConditionData.CreateFromBinary(frame);
            case 765:
                return IsTimeSpanAnyNightConditionData.CreateFromBinary(frame);
            case 766:
                return CurrentFurnitureHasKeywordConditionData.CreateFromBinary(frame);
            case 767:
                return GetWeaponEquipIndexConditionData.CreateFromBinary(frame);
            case 769:
                return IsOverEncumberedConditionData.CreateFromBinary(frame);
            case 770:
                return IsPackageRequestingBlockedIdlesConditionData.CreateFromBinary(frame);
            case 771:
                return GetActionDataIntConditionData.CreateFromBinary(frame);
            case 772:
                return GetVATSRightMinusLeftAreaFreeConditionData.CreateFromBinary(frame);
            case 773:
                return GetInIronSightsConditionData.CreateFromBinary(frame);
            case 774:
                return GetActorStaggerDirectionConditionData.CreateFromBinary(frame);
            case 775:
                return GetActorStaggerMagnitudeConditionData.CreateFromBinary(frame);
            case 776:
                return WornCoversBipedSlotConditionData.CreateFromBinary(frame);
            case 777:
                return GetInventoryValueConditionData.CreateFromBinary(frame);
            case 778:
                return IsPlayerInConversationConditionData.CreateFromBinary(frame);
            case 779:
                return IsInDialogueCameraConditionData.CreateFromBinary(frame);
            case 780:
                return IsMyDialogueTargetPlayerConditionData.CreateFromBinary(frame);
            case 781:
                return IsMyDialogueTargetActorConditionData.CreateFromBinary(frame);
            case 782:
                return GetMyDialogueTargetDistanceConditionData.CreateFromBinary(frame);
            case 783:
                return IsSeatOccupiedConditionData.CreateFromBinary(frame);
            case 784:
                return IsPlayerRidingConditionData.CreateFromBinary(frame);
            case 785:
                return IsTryingEventCameraConditionData.CreateFromBinary(frame);
            case 786:
                return UseLeftSideCameraConditionData.CreateFromBinary(frame);
            case 787:
                return GetNoteTypeConditionData.CreateFromBinary(frame);
            case 788:
                return LocationHasPlayerOwnedWorkshopConditionData.CreateFromBinary(frame);
            case 789:
                return IsStartingActionConditionData.CreateFromBinary(frame);
            case 790:
                return IsMidActionConditionData.CreateFromBinary(frame);
            case 791:
                return IsWeaponChargeAttackConditionData.CreateFromBinary(frame);
            case 792:
                return IsInWorkshopModeConditionData.CreateFromBinary(frame);
            case 793:
                return IsWeaponChargingHoldAttackConditionData.CreateFromBinary(frame);
            case 794:
                return IsEncounterAbovePlayerLevelConditionData.CreateFromBinary(frame);
            case 795:
                return IsMeleeAttackingConditionData.CreateFromBinary(frame);
            case 796:
                return GetVATSQueuedTargetsUniqueConditionData.CreateFromBinary(frame);
            case 797:
                return GetCurrentLocationExploredConditionData.CreateFromBinary(frame);
            case 798:
                return IsPoweredConditionData.CreateFromBinary(frame);
            case 799:
                return GetTransmitterDistanceConditionData.CreateFromBinary(frame);
            case 800:
                return GetCameraPlaybackTimeConditionData.CreateFromBinary(frame);
            case 801:
                return IsInWaterConditionData.CreateFromBinary(frame);
            case 802:
                return GetWithinActivateDistanceConditionData.CreateFromBinary(frame);
            case 803:
                return IsUnderWaterConditionData.CreateFromBinary(frame);
            case 804:
                return IsInSameSpaceConditionData.CreateFromBinary(frame);
            case 805:
                return LocationAllowsResetConditionData.CreateFromBinary(frame);
            case 806:
                return GetVATSBackRightAreaFreeConditionData.CreateFromBinary(frame);
            case 807:
                return GetVATSBackLeftAreaFreeConditionData.CreateFromBinary(frame);
            case 808:
                return GetVATSBackRightTargetVisibleConditionData.CreateFromBinary(frame);
            case 809:
                return GetVATSBackLeftTargetVisibleConditionData.CreateFromBinary(frame);
            case 810:
                return GetVATSTargetLimbVisibleConditionData.CreateFromBinary(frame);
            case 811:
                return IsPlayerListeningConditionData.CreateFromBinary(frame);
            case 812:
                return GetPathingRequestedQuickTurnConditionData.CreateFromBinary(frame);
            case 813:
                return EPIsCalculatingBaseDamageConditionData.CreateFromBinary(frame);
            case 814:
                return GetReanimatingConditionData.CreateFromBinary(frame);
            case 815:
                return GetCombatDialogueDataIntConditionData.CreateFromBinary(frame);
            case 816:
                return IsDockedConditionData.CreateFromBinary(frame);
            case 817:
                return IsDockedWithConditionData.CreateFromBinary(frame);
            case 818:
                return GetLastDialogueCameraHasKeywordConditionData.CreateFromBinary(frame);
            case 819:
                return GetActionDataFormConditionData.CreateFromBinary(frame);
            case 820:
                return IsInSpaceConditionData.CreateFromBinary(frame);
            case 822:
                return GetSpaceshipConditionData.CreateFromBinary(frame);
            case 823:
                return ShipContainsRefConditionData.CreateFromBinary(frame);
            case 824:
                return IsInSpaceshipConditionData.CreateFromBinary(frame);
            case 825:
                return ShipHasActorInPilotSeatConditionData.CreateFromBinary(frame);
            case 826:
                return ActorPackageHasRandomConversationsFlagOnConditionData.CreateFromBinary(frame);
            case 827:
                return GetActorInShipPilotSeatConditionData.CreateFromBinary(frame);
            case 828:
                return IsSpaceshipConditionData.CreateFromBinary(frame);
            case 829:
                return GetInAcousticSpaceConditionData.CreateFromBinary(frame);
            case 830:
                return CurrentShipLandedConditionData.CreateFromBinary(frame);
            case 831:
                return IsDockedAsChildConditionData.CreateFromBinary(frame);
            case 832:
                return PlayerHailResponseConditionData.CreateFromBinary(frame);
            case 833:
                return IsHerdLeaderConditionData.CreateFromBinary(frame);
            case 834:
                return HasHerdLeaderConditionData.CreateFromBinary(frame);
            case 836:
                return GetPlayerHomeSpaceShipConditionData.CreateFromBinary(frame);
            case 837:
                return IsTrueForConditionFormConditionData.CreateFromBinary(frame);
            case 838:
                return GetNumElementsInRefCollectionConditionData.CreateFromBinary(frame);
            case 839:
                return GetCurrentWeatherHasKeywordConditionData.CreateFromBinary(frame);
            case 840:
                return IsSnappedToConditionData.CreateFromBinary(frame);
            case 841:
                return HasKeywordOnNodeConditionData.CreateFromBinary(frame);
            case 842:
                return HasKeywordOnStackedConditionData.CreateFromBinary(frame);
            case 843:
                return HasVisualDetectionConditionData.CreateFromBinary(frame);
            case 844:
                return HasSoundDetectionConditionData.CreateFromBinary(frame);
            case 845:
                return IsSuppressedConditionData.CreateFromBinary(frame);
            case 846:
                return IsSpaceshipEngineDestroyedConditionData.CreateFromBinary(frame);
            case 847:
                return IsLandedConditionData.CreateFromBinary(frame);
            case 848:
                return IsSpaceshipShieldsDestroyedConditionData.CreateFromBinary(frame);
            case 849:
                return IsSpaceshipGravDriveDestroyedConditionData.CreateFromBinary(frame);
            case 850:
                return GetNumberOfSpaceshipWeaponsDestroyedConditionData.CreateFromBinary(frame);
            case 851:
                return GetIsCurrentLocationExactConditionData.CreateFromBinary(frame);
            case 852:
                return GetIsEditorLocationExactConditionData.CreateFromBinary(frame);
            case 853:
                return IsInThreatBackdownConditionData.CreateFromBinary(frame);
            case 854:
                return IsInsidePrimitiveConditionData.CreateFromBinary(frame);
            case 855:
                return GetCameraActorCountConditionData.CreateFromBinary(frame);
            case 856:
                return GetIsCurrentLocationAliasExactConditionData.CreateFromBinary(frame);
            case 857:
                return IsJailInSystemConditionData.CreateFromBinary(frame);
            case 858:
                return BodyHasKeywordConditionData.CreateFromBinary(frame);
            case 859:
                return BiomeHasKeywordConditionData.CreateFromBinary(frame);
            case 860:
                return SystemHasKeywordConditionData.CreateFromBinary(frame);
            case 861:
                return GetDistanceFromLocationWithKeywordConditionData.CreateFromBinary(frame);
            case 862:
                return GetPlanetVisitedConditionData.CreateFromBinary(frame);
            case 863:
                return IsLocalDayConditionData.CreateFromBinary(frame);
            case 864:
                return SpeechChallengePreviousSceneHasKeywordConditionData.CreateFromBinary(frame);
            case 865:
                return GetBiomeMaskValueConditionData.CreateFromBinary(frame);
            case 866:
                return BodyIsTypeConditionData.CreateFromBinary(frame);
            case 867:
                return BodyIsAtmosphereTypeConditionData.CreateFromBinary(frame);
            case 868:
                return BodyIsTemperatureTypeConditionData.CreateFromBinary(frame);
            case 869:
                return GetBodyTemperatureConditionData.CreateFromBinary(frame);
            case 870:
                return GetBodyPressureConditionData.CreateFromBinary(frame);
            case 871:
                return GetBodyGravityConditionData.CreateFromBinary(frame);
            case 872:
                return GetBodySurveyPercentConditionData.CreateFromBinary(frame);
            case 873:
                return IsPlayerLoiteringConditionData.CreateFromBinary(frame);
            case 874:
                return IsResearchCompleteConditionData.CreateFromBinary(frame);
            case 875:
                return BodyIsPlanetTraitKnownConditionData.CreateFromBinary(frame);
            case 876:
                return HasPerkCategoryConditionData.CreateFromBinary(frame);
            case 877:
                return HasPerkSkillGroupConditionData.CreateFromBinary(frame);
            case 878:
                return CountAquiredPerkRanksByTypeConditionData.CreateFromBinary(frame);
            case 879:
                return IsScannedConditionData.CreateFromBinary(frame);
            case 880:
                return IsScannableKeywordRevealedConditionData.CreateFromBinary(frame);
            case 881:
                return IsMyVictimConditionData.CreateFromBinary(frame);
            case 882:
                return GetResourceScarcityConditionData.CreateFromBinary(frame);
            case 883:
                return CheckContrabandStatusConditionData.CreateFromBinary(frame);
            case 886:
                return IsPlayerSpaceFarTravellingConditionData.CreateFromBinary(frame);
            case 887:
                return IsPlayerSpaceFarTravelDepartureConditionData.CreateFromBinary(frame);
            case 888:
                return IsPlayerSpaceFarTravelArrivalConditionData.CreateFromBinary(frame);
            case 889:
                return BiomeHasWeatherConditionData.CreateFromBinary(frame);
            case 891:
                return GetSystemSurveyPercentConditionData.CreateFromBinary(frame);
            case 892:
                return SystemBodyHasKeywordConditionData.CreateFromBinary(frame);
            case 894:
                return GetShipGroupThreatRatioConditionData.CreateFromBinary(frame);
            case 895:
                return IsOnGrazingTerrainConditionData.CreateFromBinary(frame);
            case 896:
                return GetDistanceGalacticParsecConditionData.CreateFromBinary(frame);
            case 897:
                return GetDistanceGalacticMegaMeterConditionData.CreateFromBinary(frame);
            case 898:
                return GetShipToShipGroupThreatRatioConditionData.CreateFromBinary(frame);
            case 899:
                return GetGroupMembersInRadiusCountConditionData.CreateFromBinary(frame);
            case 900:
                return GetShipPiracyValueConditionData.CreateFromBinary(frame);
            case 901:
                return GetDistanceFromCelestialBodyAliasParsecsConditionData.CreateFromBinary(frame);
            case 902:
                return GetDistanceFromCelestialBodyAliasMegaMetersConditionData.CreateFromBinary(frame);
            case 904:
                return IsInsidePrimitiveTopAndBottomConditionData.CreateFromBinary(frame);
            case 905:
                return GetPlayerBountyCrimeFactionConditionData.CreateFromBinary(frame);
            case 906:
                return GetIsFloatingConditionData.CreateFromBinary(frame);
            case 907:
                return LocationOrParentHasKeywordConditionData.CreateFromBinary(frame);
            case 908:
                return IsCelestialBodyScannedConditionData.CreateFromBinary(frame);
            case 912:
                return IsActorReactionInCooldownConditionData.CreateFromBinary(frame);
            case 916:
                return BiomeSupportsCreatureConditionData.CreateFromBinary(frame);
            case 919:
                return EPMagic_SpellHasMagicEffectConditionData.CreateFromBinary(frame);
            case 921:
                return IsFacingActorConditionData.CreateFromBinary(frame);
            case 922:
                return IsSameVoiceTypeConditionData.CreateFromBinary(frame);
            case 923:
                return GetValueCurrentLocationConditionData.CreateFromBinary(frame);
            case 924:
                return IsBoostPackActiveConditionData.CreateFromBinary(frame);
            case 925:
                return GetTimeSinceLastBoostPackEndedConditionData.CreateFromBinary(frame);
            case 926:
                return EPGetLastCombatHitCriticalConditionData.CreateFromBinary(frame);
            case 927:
                return EPGetLastCombatHitKillConditionData.CreateFromBinary(frame);
            case 928:
                return EPGetLastCombatHitGunBashConditionData.CreateFromBinary(frame);
            case 929:
                return EPIsLastCombatHitLimbInCategoryConditionData.CreateFromBinary(frame);
            case 930:
                return IsEditorLocationInsidePrimitiveConditionData.CreateFromBinary(frame);
            case 931:
                return GetIsPronounConditionData.CreateFromBinary(frame);
            case 932:
                return GetDistanceGalacticLightYearsConditionData.CreateFromBinary(frame);
            case 933:
                return GetDistanceFromCelestialBodyAliasLightyearsConditionData.CreateFromBinary(frame);
            case 934:
                return IsOnPlayerHomeSpaceShipConditionData.CreateFromBinary(frame);
            case 935:
                return EPMagic_EffectHasKeywordConditionData.CreateFromBinary(frame);
            case 936:
                return EPMagic_SpellIsConditionData.CreateFromBinary(frame);
            case 937:
                return IsPlayerSteadyingWeaponConditionData.CreateFromBinary(frame);
            case 938:
                return ResourceVeinHasKeywordConditionData.CreateFromBinary(frame);
            case 939:
                return GetLastCombatHitActorConsecutiveHitsConditionData.CreateFromBinary(frame);
            case 940:
                return GetCurrentAndLastWeatherHaveKeywordConditionData.CreateFromBinary(frame);
            case 941:
                return IsPCEquippedWeaponNthAttackConditionData.CreateFromBinary(frame);
            case 942:
                return GetWaterDepthConditionData.CreateFromBinary(frame);
            case 943:
                return AreHostileActorsNearConditionData.CreateFromBinary(frame);
            case 944:
                return GetPlayerGravityScaleConditionData.CreateFromBinary(frame);
            case 945:
                return IsInSameGroupConditionData.CreateFromBinary(frame);
            case 946:
                return IsBoostPackHoveringConditionData.CreateFromBinary(frame);
            case 947:
                return GetUsedWeightCapacityConditionFunctionConditionData.CreateFromBinary(frame);
            case 949:
                return BodyHasResourceConditionData.CreateFromBinary(frame);
            case 950:
                return GetPCIsReloadingConditionData.CreateFromBinary(frame);
            case 951:
                return ActorExposedToSkyConditionData.CreateFromBinary(frame);
            case 954:
                return GetQuestStartingConditionData.CreateFromBinary(frame);
            case 955:
                return BodyHasResourceWithKeywordConditionData.CreateFromBinary(frame);
            case 957:
                return GetShipReactorClassConditionData.CreateFromBinary(frame);
            case 958:
                return ShipReactorHasClassKeywordConditionData.CreateFromBinary(frame);
            case 960:
                return EPIsResistanceActorValueConditionData.CreateFromBinary(frame);
            default:
                return null;
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
        if (obj is not IConditionStringParameter stringParameter) return;
        if (stringParameter.FirstStringParameter is { } param1)
        {
            using (HeaderExport.Subrecord(writer, RecordTypes.CIS1))
            {
                StringBinaryTranslation.Instance.Write(writer, param1, StringBinaryType.NullTerminate);
            }
        }

        if (stringParameter.SecondStringParameter is { } param2)
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
        writer.Write(item.Data.Unknown3);
    }
}

abstract partial class ConditionBinaryOverlay
{
    public IConditionDataGetter Data { get; private set; } = null!;

    private static RecordTriggerSpecs IncludeTriggers = new(
        RecordCollection.Factory(
            RecordTypes.CIS1,
            RecordTypes.CIS2));

    public partial Condition.Flag GetFlagsCustom(int location) => ConditionBinaryCreateTranslation
        .GetFlag(_structData.Span[location])
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

    public static IConditionGetter ConditionFactory(OverlayStream stream, BinaryOverlayFactoryPackage package,
        TypedParseParams _)
    {
        return ConditionFactory(stream, package);
    }

    public static IReadOnlyList<IConditionGetter> ConstructBinaryOverlayCountedList(OverlayStream stream,
        BinaryOverlayFactoryPackage package)
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

    public static IReadOnlyList<IConditionGetter> ConstructBinaryOverlayList(OverlayStream stream,
        BinaryOverlayFactoryPackage package)
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

    public static IReadOnlyList<IConditionGetter> ConstructBinayOverlayList(OverlayStream stream, BinaryOverlayFactoryPackage package)
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