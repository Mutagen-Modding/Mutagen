using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using static Mutagen.Bethesda.Fallout4.Condition;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class Condition
    {
        // ToDo: (copied from Skyrim) Confirm correctness and completeness
        [Flags]
        public enum Flag
        {
            OR = 0x01,
            ParametersUseAliases = 0x02,
            UseGlobal = 0x04,
            UsePackData = 0x08,
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
            MyKiller = 11
        }

        public enum ParameterType
        {
            None = 0,
            String = 1, //new
            Integer = 2,
            Float = 3,
            Actor = 4,
            ActorBase = 5,
            ActorValue = 6,
            AdvanceAction = 7,
            Alias = 8,
            Alignment = 9,
            AssociationType = 10,
            Axis = 11,
            CastingSource = 12,
            Cell = 13,
            Class = 14,
            CrimeType = 15,
            CriticalStage = 16,
            EncounterZone = 17,
            EquipType = 18,
            Event = 19,
            EventData = 20,
            Faction = 21,
            FormList = 22,
            FormType = 23,
            Furniture = 24,
            FurnitureAnim = 25,
            FurnitureEntry = 26,
            Global = 27,
            IdleForm = 28,
            InventoryObject = 29,
            Keyword = 30,
            Location = 31,
            MagicEffect = 32,
            MagicItem = 33,
            MiscStat = 34,
            ObjectReference = 35,
            Owner = 36,
            Package = 37,
            Packdata = 38,
            Perk = 39,
            Quest = 40,
            QuestStage = 41,
            Race = 42,
            ReferencableObject = 43,
            RefType = 44,
            Region = 45,
            Scene = 46,
            Sex = 47,
            Shout = 48,
            VariableName = 49,
            VATSValueFunction = 50,
            VATSValueParam = 51,
            VoiceType = 52,
            WardState = 53,
            Weather = 54,
            Worldspace = 55,
            DamageType = 56 //new
        }

        public enum FunctionType
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
            GetCurrentAIPackage = 110,
            IsWaiting = 111,
            IsIdlePlaying = 112,
            IsIntimidatebyPlayer = 116,
            IsPlayerInRegion = 117,
            GetActorAggroRadiusViolated = 118,
            GetCrime = 122,
            IsGreetingPlayer = 123,
            IsGuard = 125,
            HasBeenEaten = 127,
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
            GetInCurrentLocation = 359,
            GetInCurrentLocationAlias = 360,
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
            GetMapMarkerVisible = 491,
            PlayerKnows = 493,
            GetPermanentValue = 494,
            GetKillingBlowLimb = 495,
            CanPayCrimeGold = 497,
            GetDaysInJail = 499,
            EPAlchemyGetMakingPoison = 500,
            EPAlchemyEffectHasKeyword = 501,
            GetAllowWorldInteractions = 503,
            DialogueGetAv = 506,
            DialogueHasPerk = 507,
            GetLastHitCritical = 508,
            DialogueGetItemCount = 510,
            LastCrippledCondition = 511,
            HasSharedPowerGrid = 512,
            IsCombatTarget = 513,
            GetVATSRightAreaFree = 515,
            GetVATSLeftAreaFree = 516,
            GetVATSBackAreaFree = 517,
            GetVATSFrontAreaFree = 518,
            GetLockIsBroken = 519,
            IsPS3 = 520,
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
            GetSpeechChallengeSuccessLevel = 544,
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
            GetLocationAliasCleared = 598,
            GetLocationAliasRefTypeDeadCount = 600,
            GetLocationAliasRefTypeAliveCount = 601,
            IsWardState = 602,
            IsInSameCurrentLocationAsRef = 603,
            IsInSameCurrentLocationAsRefAlias = 604,
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
            IsInFriendStateWithPlayer = 638,
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
            ShouldAttackKill = 678,
            GetActivationHeight = 680,
            WornHasKeyword = 682,
            GetPathingCurrentSpeed = 683,
            GetPathingCurrentSpeedAngle = 684,
            GetWorkshopObjectCount = 691,
            EPMagic_SpellHasKeyword = 693,
            GetNoBleedoutRecovery = 694,
            EPMagic_SpellHasSkill = 696,
            IsAttackType = 697,
            IsAllowedToFly = 698,
            HasMagicEffectKeyword = 699,
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
            GetPlayerWalkAwayFromDialogueScene = 728,
            GetActorStance = 729,
            CanProduceForWorkshop = 734,
            CanFlyHere = 735,
            EPIsDamageType = 736,
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
            GetCurrentLocationCleared = 797,
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
            IsInRobotWorkbench = 817
        }

        public enum ParameterCategory
        {
            None,
            Number,
            Form,
            String
        }
        public static (ParameterType First, ParameterType Second, ParameterType Third) GetParameterTypes(ushort function)
        {
            return function switch
            {
                1 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                6 => (ParameterType.Axis, ParameterType.None, ParameterType.None),
                8 => (ParameterType.Axis, ParameterType.None, ParameterType.None),
                10 => (ParameterType.Axis, ParameterType.None, ParameterType.None),
                11 => (ParameterType.Axis, ParameterType.None, ParameterType.None),
                14 => (ParameterType.ActorValue, ParameterType.None, ParameterType.None),
                27 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                32 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                36 => (ParameterType.Integer, ParameterType.None, ParameterType.None),
                42 => (ParameterType.Actor, ParameterType.None, ParameterType.None),
                43 => (ParameterType.Actor, ParameterType.None, ParameterType.None),
                44 => (ParameterType.Actor, ParameterType.None, ParameterType.None),
                45 => (ParameterType.Actor, ParameterType.None, ParameterType.None),
                47 => (ParameterType.ReferencableObject, ParameterType.None, ParameterType.None),
                56 => (ParameterType.Quest, ParameterType.None, ParameterType.None),
                58 => (ParameterType.Quest, ParameterType.None, ParameterType.None),
                59 => (ParameterType.Quest, ParameterType.QuestStage, ParameterType.None),
                60 => (ParameterType.Faction, ParameterType.Actor, ParameterType.None),
                66 => (ParameterType.Actor, ParameterType.None, ParameterType.None),
                67 => (ParameterType.Cell, ParameterType.None, ParameterType.None),
                68 => (ParameterType.Class, ParameterType.None, ParameterType.None),
                69 => (ParameterType.Race, ParameterType.None, ParameterType.None),
                70 => (ParameterType.Sex, ParameterType.None, ParameterType.None),
                71 => (ParameterType.Faction, ParameterType.None, ParameterType.None),
                72 => (ParameterType.ReferencableObject, ParameterType.None, ParameterType.None),
                73 => (ParameterType.Faction, ParameterType.None, ParameterType.None),
                74 => (ParameterType.Global, ParameterType.None, ParameterType.None),
                79 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                84 => (ParameterType.ActorBase, ParameterType.None, ParameterType.None),
                98 => (ParameterType.Integer, ParameterType.Integer, ParameterType.Integer),
                99 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                109 => (ParameterType.ActorValue, ParameterType.None, ParameterType.None),
                117 => (ParameterType.Region, ParameterType.None, ParameterType.None),
                122 => (ParameterType.Actor, ParameterType.CrimeType, ParameterType.None),
                131 => (ParameterType.Scene, ParameterType.Integer, ParameterType.None),
                132 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                136 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                142 => (ParameterType.ReferencableObject, ParameterType.None, ParameterType.None),
                149 => (ParameterType.Weather, ParameterType.None, ParameterType.None),
                152 => (ParameterType.Faction, ParameterType.None, ParameterType.None),
                161 => (ParameterType.Package, ParameterType.None, ParameterType.None),
                162 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                163 => (ParameterType.Furniture, ParameterType.None, ParameterType.None),
                172 => (ParameterType.Actor, ParameterType.None, ParameterType.None),
                180 => (ParameterType.ObjectReference, ParameterType.Keyword, ParameterType.None),
                181 => (ParameterType.Alias, ParameterType.Keyword, ParameterType.None),
                182 => (ParameterType.ReferencableObject, ParameterType.None, ParameterType.None),
                193 => (ParameterType.Faction, ParameterType.None, ParameterType.None),
                195 => (ParameterType.Faction, ParameterType.None, ParameterType.None),
                197 => (ParameterType.Faction, ParameterType.None, ParameterType.None),
                199 => (ParameterType.Faction, ParameterType.None, ParameterType.None),
                214 => (ParameterType.MagicEffect, ParameterType.None, ParameterType.None),
                223 => (ParameterType.MagicItem, ParameterType.None, ParameterType.None),
                228 => (ParameterType.Class, ParameterType.None, ParameterType.None),
                230 => (ParameterType.Cell, ParameterType.ObjectReference, ParameterType.None),
                246 => (ParameterType.ReferencableObject, ParameterType.None, ParameterType.None),
                247 => (ParameterType.FormType, ParameterType.None, ParameterType.None),
                248 => (ParameterType.Scene, ParameterType.None, ParameterType.None),
                250 => (ParameterType.Location, ParameterType.None, ParameterType.None),
                258 => (ParameterType.Actor, ParameterType.AssociationType, ParameterType.None),
                259 => (ParameterType.Actor, ParameterType.None, ParameterType.None),
                261 => (ParameterType.Actor, ParameterType.None, ParameterType.None),
                262 => (ParameterType.FormList, ParameterType.None, ParameterType.None),
                264 => (ParameterType.MagicItem, ParameterType.None, ParameterType.None),
                277 => (ParameterType.ActorValue, ParameterType.None, ParameterType.None),
                278 => (ParameterType.Owner, ParameterType.None, ParameterType.None),
                280 => (ParameterType.Cell, ParameterType.Owner, ParameterType.None),
                289 => (ParameterType.Integer, ParameterType.None, ParameterType.None),
                310 => (ParameterType.Worldspace, ParameterType.None, ParameterType.None),
                312 => (ParameterType.MiscStat, ParameterType.None, ParameterType.None),
                325 => (ParameterType.Packdata, ParameterType.None, ParameterType.None),
                359 => (ParameterType.Location, ParameterType.None, ParameterType.None),
                360 => (ParameterType.Alias, ParameterType.None, ParameterType.None),
                362 => (ParameterType.Keyword, ParameterType.None, ParameterType.None),
                366 => (ParameterType.Faction, ParameterType.None, ParameterType.None),
                368 => (ParameterType.Integer, ParameterType.None, ParameterType.None),
                370 => (ParameterType.Actor, ParameterType.None, ParameterType.None),
                372 => (ParameterType.FormList, ParameterType.None, ParameterType.None),
                373 => (ParameterType.Faction, ParameterType.None, ParameterType.None),
                375 => (ParameterType.Faction, ParameterType.None, ParameterType.None),
                376 => (ParameterType.Faction, ParameterType.None, ParameterType.None),
                378 => (ParameterType.Actor, ParameterType.None, ParameterType.None),
                397 => (ParameterType.Integer, ParameterType.None, ParameterType.None),
                398 => (ParameterType.FormList, ParameterType.None, ParameterType.None),
                403 => (ParameterType.Actor, ParameterType.None, ParameterType.None),
                407 => (ParameterType.Integer, ParameterType.Integer, ParameterType.None),
                408 => (ParameterType.Actor, ParameterType.None, ParameterType.None),
                409 => (ParameterType.FormList, ParameterType.None, ParameterType.None),
                410 => (ParameterType.Faction, ParameterType.Faction, ParameterType.None),
                414 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                426 => (ParameterType.VoiceType, ParameterType.None, ParameterType.None),
                432 => (ParameterType.FormType, ParameterType.None, ParameterType.None),
                437 => (ParameterType.Integer, ParameterType.None, ParameterType.None),
                438 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                439 => (ParameterType.ReferencableObject, ParameterType.None, ParameterType.None),
                444 => (ParameterType.FormList, ParameterType.None, ParameterType.None),
                445 => (ParameterType.EncounterZone, ParameterType.None, ParameterType.None),
                446 => (ParameterType.Axis, ParameterType.None, ParameterType.None),
                447 => (ParameterType.String, ParameterType.None, ParameterType.None),
                448 => (ParameterType.Perk, ParameterType.None, ParameterType.None),
                449 => (ParameterType.Actor, ParameterType.None, ParameterType.None),
                450 => (ParameterType.IdleForm, ParameterType.None, ParameterType.None),
                459 => (ParameterType.Faction, ParameterType.None, ParameterType.None),
                463 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                465 => (ParameterType.Keyword, ParameterType.None, ParameterType.None),
                473 => (ParameterType.Alignment, ParameterType.None, ParameterType.None),
                477 => (ParameterType.Actor, ParameterType.None, ParameterType.None),
                479 => (ParameterType.EquipType, ParameterType.None, ParameterType.None),
                493 => (ParameterType.ReferencableObject, ParameterType.None, ParameterType.None),
                494 => (ParameterType.ActorValue, ParameterType.None, ParameterType.None),
                497 => (ParameterType.Faction, ParameterType.None, ParameterType.None),
                501 => (ParameterType.Keyword, ParameterType.None, ParameterType.None),
                506 => (ParameterType.ActorValue, ParameterType.None, ParameterType.None),
                507 => (ParameterType.Perk, ParameterType.None, ParameterType.None),
                510 => (ParameterType.ReferencableObject, ParameterType.None, ParameterType.None),
                511 => (ParameterType.ActorValue, ParameterType.None, ParameterType.None),
                512 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                513 => (ParameterType.Actor, ParameterType.None, ParameterType.None),
                515 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                516 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                517 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                518 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                522 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                523 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                524 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                525 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                528 => (ParameterType.CriticalStage, ParameterType.None, ParameterType.None),
                533 => (ParameterType.Faction, ParameterType.None, ParameterType.None),
                534 => (ParameterType.Faction, ParameterType.None, ParameterType.None),
                535 => (ParameterType.Faction, ParameterType.None, ParameterType.None),
                543 => (ParameterType.Quest, ParameterType.None, ParameterType.None),
                550 => (ParameterType.Scene, ParameterType.Integer, ParameterType.None),
                552 => (ParameterType.MagicItem, ParameterType.None, ParameterType.None),
                560 => (ParameterType.Keyword, ParameterType.None, ParameterType.None),
                561 => (ParameterType.RefType, ParameterType.None, ParameterType.None),
                562 => (ParameterType.Keyword, ParameterType.None, ParameterType.None),
                563 => (ParameterType.RefType, ParameterType.None, ParameterType.None),
                565 => (ParameterType.Location, ParameterType.None, ParameterType.None),
                566 => (ParameterType.Alias, ParameterType.None, ParameterType.None),
                567 => (ParameterType.Alias, ParameterType.None, ParameterType.None),
                570 => (ParameterType.CastingSource, ParameterType.None, ParameterType.None),
                571 => (ParameterType.CastingSource, ParameterType.None, ParameterType.None),
                572 => (ParameterType.CastingSource, ParameterType.None, ParameterType.None),
                576 => (ParameterType.Event, ParameterType.EventData, ParameterType.String),
                577 => (ParameterType.ObjectReference, ParameterType.ObjectReference, ParameterType.None),
                584 => (ParameterType.ObjectReference, ParameterType.Axis, ParameterType.None),
                591 => (ParameterType.Location, ParameterType.RefType, ParameterType.None),
                592 => (ParameterType.Location, ParameterType.RefType, ParameterType.None),
                595 => (ParameterType.MagicItem, ParameterType.CastingSource, ParameterType.None),
                596 => (ParameterType.CastingSource, ParameterType.Keyword, ParameterType.None),
                597 => (ParameterType.CastingSource, ParameterType.None, ParameterType.None),
                598 => (ParameterType.Alias, ParameterType.None, ParameterType.None),
                600 => (ParameterType.Alias, ParameterType.RefType, ParameterType.None),
                601 => (ParameterType.Alias, ParameterType.RefType, ParameterType.None),
                602 => (ParameterType.WardState, ParameterType.None, ParameterType.None),
                603 => (ParameterType.ObjectReference, ParameterType.Keyword, ParameterType.None),
                604 => (ParameterType.Alias, ParameterType.Keyword, ParameterType.None),
                605 => (ParameterType.Alias, ParameterType.Location, ParameterType.None),
                606 => (ParameterType.Location, ParameterType.Keyword, ParameterType.None),
                608 => (ParameterType.Alias, ParameterType.Keyword, ParameterType.None),
                610 => (ParameterType.Alias, ParameterType.Keyword, ParameterType.None),
                611 => (ParameterType.Packdata, ParameterType.None, ParameterType.None),
                612 => (ParameterType.Packdata, ParameterType.None, ParameterType.None),
                617 => (ParameterType.AssociationType, ParameterType.None, ParameterType.None),
                619 => (ParameterType.Axis, ParameterType.None, ParameterType.None),
                620 => (ParameterType.Axis, ParameterType.None, ParameterType.None),
                622 => (ParameterType.Axis, ParameterType.None, ParameterType.None),
                624 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                625 => (ParameterType.Location, ParameterType.None, ParameterType.None),
                626 => (ParameterType.Alias, ParameterType.None, ParameterType.None),
                629 => (ParameterType.Quest, ParameterType.String, ParameterType.None),
                637 => (ParameterType.ReferencableObject, ParameterType.None, ParameterType.None),
                639 => (ParameterType.ObjectReference, ParameterType.Float, ParameterType.None),
                640 => (ParameterType.ActorValue, ParameterType.None, ParameterType.None),
                650 => (ParameterType.ObjectReference, ParameterType.Keyword, ParameterType.None),
                651 => (ParameterType.Keyword, ParameterType.None, ParameterType.None),
                652 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                659 => (ParameterType.String, ParameterType.None, ParameterType.None),
                660 => (ParameterType.String, ParameterType.String, ParameterType.None),
                661 => (ParameterType.ActorValue, ParameterType.None, ParameterType.None),
                664 => (ParameterType.Quest, ParameterType.None, ParameterType.None),
                675 => (ParameterType.String, ParameterType.None, ParameterType.None),
                678 => (ParameterType.Actor, ParameterType.None, ParameterType.None),
                682 => (ParameterType.Keyword, ParameterType.None, ParameterType.None),
                684 => (ParameterType.Axis, ParameterType.None, ParameterType.None),
                691 => (ParameterType.ReferencableObject, ParameterType.None, ParameterType.None),
                693 => (ParameterType.Keyword, ParameterType.None, ParameterType.None),
                696 => (ParameterType.ActorValue, ParameterType.None, ParameterType.None),
                697 => (ParameterType.Keyword, ParameterType.None, ParameterType.None),
                699 => (ParameterType.Keyword, ParameterType.None, ParameterType.None),
                705 => (ParameterType.Actor, ParameterType.None, ParameterType.None),
                706 => (ParameterType.CastingSource, ParameterType.None, ParameterType.None),
                707 => (ParameterType.Keyword, ParameterType.None, ParameterType.None),
                713 => (ParameterType.Perk, ParameterType.None, ParameterType.None),
                719 => (ParameterType.Actor, ParameterType.None, ParameterType.None),
                720 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                722 => (ParameterType.Keyword, ParameterType.None, ParameterType.None),
                736 => (ParameterType.DamageType, ParameterType.None, ParameterType.None),
                741 => (ParameterType.Keyword, ParameterType.None, ParameterType.None),
                742 => (ParameterType.Keyword, ParameterType.None, ParameterType.None),
                744 => (ParameterType.Integer, ParameterType.Integer, ParameterType.None),
                746 => (ParameterType.Integer, ParameterType.None, ParameterType.None),
                747 => (ParameterType.Integer, ParameterType.None, ParameterType.None),
                748 => (ParameterType.Integer, ParameterType.None, ParameterType.None),
                749 => (ParameterType.Keyword, ParameterType.None, ParameterType.None),
                753 => (ParameterType.Actor, ParameterType.None, ParameterType.None),
                754 => (ParameterType.Actor, ParameterType.None, ParameterType.None),
                766 => (ParameterType.Keyword, ParameterType.None, ParameterType.None),
                772 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                773 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                776 => (ParameterType.Integer, ParameterType.None, ParameterType.None),
                783 => (ParameterType.Keyword, ParameterType.None, ParameterType.None),
                802 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                804 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                806 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                807 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                808 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                809 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                810 => (ParameterType.ObjectReference, ParameterType.None, ParameterType.None),
                811 => (ParameterType.Float, ParameterType.None, ParameterType.None),
                _ => (ParameterType.None, ParameterType.None, ParameterType.None),
            };
        }

        public static Condition CreateFromBinary(
            MutagenFrame frame,
            RecordTypeConverter? recordTypeConverter)
        {
            if (!frame.Reader.TryGetSubrecord(Internals.RecordTypes.CTDA, out var subRecMeta))
            {
                throw new ArgumentException();
            }
            var flagByte = frame.GetUInt8(subRecMeta.HeaderLength);
            Condition.Flag flag = ConditionBinaryCreateTranslation.GetFlag(flagByte);
            if (flag.HasFlag(Condition.Flag.UseGlobal))
            {
                return ConditionGlobal.CreateFromBinary(frame.SpawnWithLength(subRecMeta.ContentLength, checkFraming: false));
            }
            else
            {
                return ConditionFloat.CreateFromBinary(frame.SpawnWithLength(subRecMeta.ContentLength, checkFraming: false));
            }
        }

        public static bool TryCreateFromBinary(
            MutagenFrame frame,
            out Condition condition,
            RecordTypeConverter? recordTypeConverter)
        {
            condition = CreateFromBinary(frame, recordTypeConverter);
            return true;
        }
    }

    public partial class ConditionMixIn
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
                case ParameterType.DamageType:
                    return ParameterCategory.Form;
                case ParameterType.String:
                    return ParameterCategory.String;
                default:
                    throw new NotImplementedException();
            }
        }

    }

    namespace Internals
    {
        public partial class Condition_Registration
        {
            public static readonly RecordType CIS1 = new RecordType("CIS1");
            public static readonly RecordType CIS2 = new RecordType("CIS2");

            public static ICollectionGetter<RecordType> TriggeringRecordTypes => _TriggeringRecordTypes.Value;
            private static readonly Lazy<ICollectionGetter<RecordType>> _TriggeringRecordTypes = new Lazy<ICollectionGetter<RecordType>>(() =>
            {
                return new CollectionGetterWrapper<RecordType>(
                    new RecordType[]
                    {
                        RecordTypes.CTDA,
                    }
                );
            });
        }

        public partial class ConditionBinaryCreateTranslation
        {
            public const byte CompareMask = 0xE0;
            public const byte FlagMask = 0x1F;

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
                    conditions.Add(Condition.CreateFromBinary(frame, default(RecordTypeConverter)));
                }
            }

            public static void FillConditionsList(IList<Condition> conditions, MutagenFrame frame)
            {
                conditions.Clear();
                while (frame.Reader.TryGetSubrecord(RecordTypes.CTDA, out var subMeta))
                {
                    conditions.Add(Condition.CreateFromBinary(frame, default(RecordTypeConverter)));
                }
            }

            public static partial void FillBinaryFlagsCustom(MutagenFrame frame, ICondition item)
            {
                byte b = frame.ReadUInt8();
                item.Flags = GetFlag(b);
                item.CompareOperator = GetCompareOperator(b);
            }

            public static void CustomStringImports(MutagenFrame frame, IConditionData item)
            {
                if (!frame.Reader.TryGetSubrecordFrame(out var subMeta)) return;
                switch (subMeta.RecordType.TypeInt)
                {
                    case 0x31534943: // CIS1
                        item.ParameterOneString = BinaryStringUtility.ProcessWholeToZString(subMeta.Content);
                        break;
                    case 0x32534943: // CIS2
                        item.ParameterTwoString = BinaryStringUtility.ProcessWholeToZString(subMeta.Content);
                        break;
                    default:
                        return;
                }
                frame.Position += subMeta.TotalLength;
            }
        }

        public partial class ConditionBinaryWriteTranslation
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
                writer.Write(GetFlagWriteByte(item.Flags, item.CompareOperator));
            }

            public static void CustomStringExports(MutagenWriter writer, IConditionDataGetter obj)
            {
                if (obj.ParameterOneString is { } param1)
                {
                    using (HeaderExport.Subrecord(writer, Condition_Registration.CIS1))
                    {
                        StringBinaryTranslation.Instance.Write(writer, param1, StringBinaryType.NullTerminate);
                    }
                }
                if (obj.ParameterTwoString is { } param2)
                {
                    using (HeaderExport.Subrecord(writer, Condition_Registration.CIS2))
                    {
                        StringBinaryTranslation.Instance.Write(writer, param2, StringBinaryType.NullTerminate);
                    }
                }
            }
        }

        public partial class ConditionBinaryOverlay
        {
            private static ICollectionGetter<RecordType> IncludeTriggers = new CollectionGetterWrapper<RecordType>(
                new RecordType[]
                {
                    new RecordType("CIS1"),
                    new RecordType("CIS2"),
                });

            private Condition.Flag GetFlagsCustom(int location) => ConditionBinaryCreateTranslation.GetFlag(_data.Span[location]);
            public CompareOperator CompareOperator => ConditionBinaryCreateTranslation.GetCompareOperator(_data.Span[0]);

            public static ConditionBinaryOverlay ConditionFactory(OverlayStream stream, BinaryOverlayFactoryPackage package)
            {
                var subRecMeta = stream.GetSubrecordFrame();
                if (subRecMeta.RecordType != RecordTypes.CTDA)
                {
                    throw new ArgumentException();
                }
                Condition.Flag flag = ConditionBinaryCreateTranslation.GetFlag(subRecMeta.Content[0]);
                if (flag.HasFlag(Condition.Flag.UseGlobal))
                {
                    return ConditionGlobalBinaryOverlay.ConditionGlobalFactory(stream, package);
                }
                else
                {
                    return ConditionFloatBinaryOverlay.ConditionFloatFactory(stream, package);
                }
            }

            public static IReadOnlyList<ConditionBinaryOverlay> ConstructBinayOverlayCountedList(OverlayStream stream, BinaryOverlayFactoryPackage package)
            {
                var counterMeta = stream.ReadSubrecordFrame();
                if (counterMeta.RecordType != RecordTypes.CITC
                    || counterMeta.Content.Length != 4)
                {
                    throw new ArgumentException();
                }
                var count = BinaryPrimitives.ReadUInt32LittleEndian(counterMeta.Content);
                var ret = ConstructBinayOverlayList(stream, package);
                if (count != ret.Count)
                {
                    throw new ArgumentException("Number of parsed conditions did not matched labeled count.");
                }
                return ret;
            }

            public static IReadOnlyList<ConditionBinaryOverlay> ConstructBinayOverlayList(OverlayStream stream, BinaryOverlayFactoryPackage package)
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
                return BinaryOverlayList.FactoryByArray<ConditionBinaryOverlay>(
                    mem: span,
                    package: package,
                    getter: (s, p) => ConditionBinaryOverlay.ConditionFactory(new OverlayStream(s, p), p),
                    locs: recordLocs);
            }
        }

        public partial class ConditionGlobalBinaryCreateTranslation
        {
            public static partial void CustomBinaryEndImport(MutagenFrame frame, IConditionGlobal obj)
            {
                ConditionBinaryCreateTranslation.CustomStringImports(frame, obj.Data);
            }
        }

        public partial class ConditionGlobalBinaryWriteTranslation
        {
            public static partial void CustomBinaryEndExport(MutagenWriter writer, IConditionGlobalGetter obj)
            {
                ConditionBinaryWriteTranslation.CustomStringExports(writer, obj.Data);
            }
        }

        public partial class ConditionFloatBinaryCreateTranslation
        {
            public static partial void CustomBinaryEndImport(MutagenFrame frame, IConditionFloat obj)
            {
                ConditionBinaryCreateTranslation.CustomStringImports(frame, obj.Data);
            }
        }

        public partial class ConditionFloatBinaryWriteTranslation
        {
            public static partial void CustomBinaryEndExport(MutagenWriter writer, IConditionFloatGetter obj)
            {
                ConditionBinaryWriteTranslation.CustomStringExports(writer, obj.Data);
            }
        }

        public partial class ConditionDataBinaryCreateTranslation
        {
            public static partial void FillBinaryParameterParsingCustom(MutagenFrame frame, IConditionData item)
            {
                item.ParameterOneNumber = frame.ReadInt32();
                item.ParameterTwoNumber = frame.ReadInt32();
                item.ParameterOneRecord.SetTo(FormKey.Factory(frame.MetaData.MasterReferences!, (uint)item.ParameterOneNumber));
                item.ParameterTwoRecord.SetTo(FormKey.Factory(frame.MetaData.MasterReferences!, (uint)item.ParameterTwoNumber));
                item.RunOnType = (Condition.RunOnType)frame.ReadInt32();
                item.Unknown4 = frame.ReadInt32();
                item.ParameterThreeNumber = frame.ReadInt32();
            }
        }

        public partial class ConditionDataBinaryWriteTranslation
        {
            public static partial void WriteBinaryParameterParsingCustom(MutagenWriter writer, IConditionDataGetter item)
            {
                var paramTypes = Condition.GetParameterTypes((ushort)item.Function);
                switch (paramTypes.First.GetCategory())
                {
                    case Condition.ParameterCategory.None:
                    case Condition.ParameterCategory.Number:
                        writer.Write(item.ParameterOneNumber);
                        break;
                    case Condition.ParameterCategory.Form:
                        FormKeyBinaryTranslation.Instance.Write(writer, item.ParameterOneRecord.FormKey);
                        break;
                    case Condition.ParameterCategory.String:
                        writer.Write(item.ParameterOneString);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                switch (paramTypes.Second.GetCategory())
                {
                    case Condition.ParameterCategory.None:
                    case Condition.ParameterCategory.Number:
                        writer.Write(item.ParameterTwoNumber);
                        break;
                    case Condition.ParameterCategory.Form:
                        FormKeyBinaryTranslation.Instance.Write(writer, item.ParameterTwoRecord.FormKey);
                        break;
                    case Condition.ParameterCategory.String:
                        writer.Write(item.ParameterTwoString);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                writer.Write((int)item.RunOnType);
                writer.Write(item.Unknown4);
                switch (paramTypes.Third.GetCategory())
                {
                    case Condition.ParameterCategory.None:
                    case Condition.ParameterCategory.Number:
                        writer.Write(item.ParameterThreeNumber);
                        break;
                    case Condition.ParameterCategory.Form:
                    case Condition.ParameterCategory.String:
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public partial class ConditionDataBinaryOverlay
        {
            private ReadOnlyMemorySlice<byte> _data2;

            public IFormLinkGetter<IFallout4MajorRecordGetter> ParameterOneRecord => new FormLink<IFallout4MajorRecordGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(_data2)));

            public int ParameterOneNumber => BinaryPrimitives.ReadInt32LittleEndian(_data2);

            public IFormLinkGetter<IFallout4MajorRecordGetter> ParameterTwoRecord => new FormLink<IFallout4MajorRecordGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(_data2.Slice(4))));

            public int ParameterTwoNumber => BinaryPrimitives.ReadInt32LittleEndian(_data2.Slice(4));

            public RunOnType RunOnType => (RunOnType)BinaryPrimitives.ReadInt32LittleEndian(_data2.Slice(8));

            public int Unknown4 => BinaryPrimitives.ReadInt32LittleEndian(_data2.Slice(12));

            public int ParameterThreeNumber => BinaryPrimitives.ReadInt32LittleEndian(_data2.Slice(16));

            private ReadOnlyMemorySlice<byte> _stringParamData1;
            public bool ParameterOneString_IsSet { get; private set; }
            public string? ParameterOneString => ParameterOneString_IsSet ? BinaryStringUtility.ProcessWholeToZString(_stringParamData1) : null;

            private ReadOnlyMemorySlice<byte> _stringParamData2;
            public bool ParameterTwoString_IsSet { get; private set; }
            public string? ParameterTwoString => ParameterTwoString_IsSet ? BinaryStringUtility.ProcessWholeToZString(_stringParamData2) : null;

            partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
            {
                stream.Position -= 0x4;
                _data2 = stream.RemainingMemory.Slice(4, 0x14);
                stream.Position += 0x18;
                if (stream.Complete || !stream.TryGetSubrecord(out var subFrame)) return;
                switch (subFrame.RecordTypeInt)
                {
                    case 0x31534943: // CIS1
                        _stringParamData1 = stream.RemainingMemory.Slice(subFrame.HeaderLength, subFrame.ContentLength);
                        ParameterOneString_IsSet = true;
                        break;
                    case 0x32534943: // CIS2
                        _stringParamData2 = stream.RemainingMemory.Slice(subFrame.HeaderLength, subFrame.ContentLength);
                        ParameterTwoString_IsSet = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
