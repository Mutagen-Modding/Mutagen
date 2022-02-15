using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Skyrim.Internals;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using static Mutagen.Bethesda.Skyrim.Condition;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Condition
    {
        public abstract ConditionData Data { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IConditionDataGetter IConditionGetter.Data => this.Data;

        // ToDo
        // Confirm correctness and completeness
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
        }

        public enum ParameterType
        {
            None,
            Integer,
            Float,
            VariableName,
            Sex,
            ActorValue,
            CrimeType,
            Axis,
            QuestStage,
            MiscStat,
            Alignment,
            EquipType,
            FormType,
            CriticalStage,
            ObjectReference,
            InventoryObject,
            Actor,
            VoiceType,
            IdleForm,
            FormList,
            Quest,
            Faction,
            Cell,
            Class,
            Race,
            ActorBase,
            Global,
            Weather,
            Package,
            EncounterZone,
            Perk,
            Owner,
            Furniture,
            MagicItem,
            MagicEffect,
            Worldspace,
            VATSValueFunction,
            VATSValueParam,
            ReferencableObject,
            Region,
            Keyword,
            AdvanceAction,
            CastingSource,
            Shout,
            Location,
            RefType,
            Alias,
            Packdata,
            AssociationType,
            FurnitureAnim,
            FurnitureEntry,
            Scene,
            WardState,
            Event,
            EventData,
            Knowable
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
            IsIntimidatebyPlayer = 116,
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
                310 => (ParameterType.Worldspace, ParameterType.None),
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
            TypedParseParams? translationParams)
        {
            if (!frame.Reader.TryGetSubrecord(Mutagen.Bethesda.Skyrim.Internals.RecordTypes.CTDA, out var subRecMeta))
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
            TypedParseParams? translationParams)
        {
            condition = CreateFromBinary(frame, translationParams);
            return true;
        }
    }

    public partial interface ICondition
    {
        new ConditionData Data { get; set; }
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

    public partial interface IConditionGetter
    {
        IConditionDataGetter Data { get; }
    }

    namespace Internals
    {
        public partial class Condition_Registration
        {
            public static readonly RecordType CIS1 = new RecordType("CIS1");
            public static readonly RecordType CIS2 = new RecordType("CIS2");
            public static TriggeringRecordCollection TriggeringRecordTypes => _TriggeringRecordTypes.Value;
            private static readonly Lazy<TriggeringRecordCollection> _TriggeringRecordTypes = new Lazy<TriggeringRecordCollection>(() =>
            {
                return new TriggeringRecordCollection(
                    RecordTypes.CTDA);
            });
        }

        public partial class ConditionBinaryCreateTranslation
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
                while (frame.Reader.TryGetSubrecord(RecordTypes.CTDA, out var subMeta))
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

            public static void CustomStringImports(MutagenFrame frame, IConditionData item)
            {
                if (!frame.Reader.TryGetSubrecordFrame(out var subMeta)) return;
                if (!(item is IFunctionConditionData funcData)) return;
                switch (subMeta.RecordType.TypeInt)
                {
                    case 0x31534943: // CIS1
                        funcData.ParameterOneString = BinaryStringUtility.ProcessWholeToZString(subMeta.Content, frame.MetaData.Encodings.NonTranslated);
                        break;
                    case 0x32534943: // CIS2
                        funcData.ParameterTwoString = BinaryStringUtility.ProcessWholeToZString(subMeta.Content, frame.MetaData.Encodings.NonTranslated);
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
                if (!(obj is IFunctionConditionDataGetter funcData)) return;
                if (funcData.ParameterOneString is { } param1)
                {
                    using (HeaderExport.Subrecord(writer, Condition_Registration.CIS1))
                    {
                        StringBinaryTranslation.Instance.Write(writer, param1, StringBinaryType.NullTerminate);
                    }
                }
                if (funcData.ParameterTwoString is { } param2)
                {
                    using (HeaderExport.Subrecord(writer, Condition_Registration.CIS2))
                    {
                        StringBinaryTranslation.Instance.Write(writer, param2, StringBinaryType.NullTerminate);
                    }
                }
            }
        }

        public abstract partial class ConditionBinaryOverlay
        {
            public abstract IConditionDataGetter Data { get; }
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            IConditionDataGetter IConditionGetter.Data => this.Data;

            private static TriggeringRecordCollection IncludeTriggers = new TriggeringRecordCollection(
                new RecordType("CIS1"),
                new RecordType("CIS2"));

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

            public static ConditionBinaryOverlay ConditionFactory(OverlayStream stream, BinaryOverlayFactoryPackage package, TypedParseParams? _)
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
            public static partial void FillBinaryDataCustom(MutagenFrame frame, IConditionGlobal item)
            {
                var functionIndex = frame.GetUInt16();
                if (functionIndex == ConditionBinaryCreateTranslation.EventFunctionIndex)
                {
                    item.Data = GetEventData.CreateFromBinary(frame);
                }
                else
                {
                    item.Data = FunctionConditionData.CreateFromBinary(frame);
                }
            }

            public static partial void CustomBinaryEndImport(MutagenFrame frame, IConditionGlobal obj)
            {
                ConditionBinaryCreateTranslation.CustomStringImports(frame, obj.Data);
            }
        }

        public partial class ConditionGlobalBinaryWriteTranslation
        {
            public static partial void WriteBinaryDataCustom(MutagenWriter writer, IConditionGlobalGetter item)
            {
                item.Data.WriteToBinary(writer);
            }

            public static partial void CustomBinaryEndExport(MutagenWriter writer, IConditionGlobalGetter obj)
            {
                ConditionBinaryWriteTranslation.CustomStringExports(writer, obj.Data);
            }
        }

        public partial class ConditionFloatBinaryCreateTranslation
        {
            public static partial void FillBinaryDataCustom(MutagenFrame frame, IConditionFloat item)
            {
                var functionIndex = frame.GetUInt16();
                if (functionIndex == ConditionBinaryCreateTranslation.EventFunctionIndex)
                {
                    item.Data = GetEventData.CreateFromBinary(frame);
                }
                else
                {
                    item.Data = FunctionConditionData.CreateFromBinary(frame);
                }
            }

            public static partial void CustomBinaryEndImport(MutagenFrame frame, IConditionFloat obj)
            {
                ConditionBinaryCreateTranslation.CustomStringImports(frame, obj.Data);
            }
        }

        public partial class ConditionFloatBinaryWriteTranslation
        {
            public static partial void WriteBinaryDataCustom(MutagenWriter writer, IConditionFloatGetter item)
            {
                item.Data.WriteToBinary(writer);
            }

            public static partial void CustomBinaryEndExport(MutagenWriter writer, IConditionFloatGetter obj)
            {
                ConditionBinaryWriteTranslation.CustomStringExports(writer, obj.Data);
            }
        }

        public partial class FunctionConditionDataBinaryCreateTranslation
        {
            public static partial void FillBinaryParameterParsingCustom(MutagenFrame frame, IFunctionConditionData item)
            {
                item.ParameterOneNumber = frame.ReadInt32();
                item.ParameterTwoNumber = frame.ReadInt32();
                item.ParameterOneRecord.FormKey = FormKey.Factory(frame.MetaData.MasterReferences!, (uint)item.ParameterOneNumber);
                item.ParameterTwoRecord.FormKey = FormKey.Factory(frame.MetaData.MasterReferences!, (uint)item.ParameterTwoNumber);
                GetEventDataBinaryCreateTranslation.FillEndingParams(frame, item);
            }
        }

        public partial class GetEventDataBinaryCreateTranslation
        {
            public static void FillEndingParams(MutagenFrame frame, IConditionData item)
            {
                item.RunOnType = EnumBinaryTranslation<Condition.RunOnType, MutagenFrame, MutagenWriter>.Instance.Parse(reader: frame.SpawnWithLength(4));
                item.Reference.SetTo(
                    FormLinkBinaryTranslation.Instance.Parse(
                        reader: frame,
                        defaultVal: FormKey.Null));
                item.Unknown3 = frame.ReadInt32();
            }

            public static partial void FillBinaryParameterParsingCustom(MutagenFrame frame, IGetEventData item)
            {
                FillEndingParams(frame, item);
            }
        }

        public partial class FunctionConditionDataBinaryWriteTranslation
        {
            public static partial void WriteBinaryParameterParsingCustom(MutagenWriter writer, IFunctionConditionDataGetter item)
            {
                var paramTypes = Condition.GetParameterTypes(item.Function);
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
                    default:
                        throw new NotImplementedException();
                }
                GetEventDataBinaryWriteTranslation.WriteCommonParams(writer, item);
            }
        }

        public partial class GetEventDataBinaryWriteTranslation
        {
            public static void WriteCommonParams(MutagenWriter writer, IConditionDataGetter item)
            {
                EnumBinaryTranslation<Condition.RunOnType, MutagenFrame, MutagenWriter>.Instance.Write(
                    writer,
                    item.RunOnType,
                    length: 4);
                FormLinkBinaryTranslation.Instance.Write(
                    writer: writer,
                    item: item.Reference);
                writer.Write(item.Unknown3);
            }

            public static partial void WriteBinaryParameterParsingCustom(MutagenWriter writer, IGetEventDataGetter item)
            {
                WriteCommonParams(writer, item);
            }
        }

        public partial class ConditionFloatBinaryOverlay
        {
            private IConditionDataGetter GetDataCustom(int location)
            {
                var functionIndex = BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(location));
                if (functionIndex == ConditionBinaryCreateTranslation.EventFunctionIndex)
                {
                    return GetEventDataBinaryOverlay.GetEventDataFactory(new OverlayStream(_data.Slice(location), _package), _package);
                }
                else
                {
                    return FunctionConditionDataBinaryOverlay.FunctionConditionDataFactory(new OverlayStream(_data.Slice(location), _package), _package);
                }
            }

            partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
            {
                stream.Position = offset;
                _data = stream.RemainingMemory;
            }
        }

        public partial class ConditionGlobalBinaryOverlay
        {
            private IConditionDataGetter GetDataCustom(int location)
            {
                var functionIndex = BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(location));
                if (functionIndex == ConditionBinaryCreateTranslation.EventFunctionIndex)
                {
                    return GetEventDataBinaryOverlay.GetEventDataFactory(new OverlayStream(_data.Slice(location), _package), _package);
                }
                else
                {
                    return FunctionConditionDataBinaryOverlay.FunctionConditionDataFactory(new OverlayStream(_data.Slice(location), _package), _package);
                }
            }

            partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
            {
                stream.Position = offset;
                _data = stream.RemainingMemory;
            }
        }

        public partial class ConditionDataBinaryOverlay
        {
            public Condition.RunOnType RunOnType => (Condition.RunOnType)BinaryPrimitives.ReadInt32LittleEndian(_data.Span.Slice(0xC, 0x4));
            public IFormLinkGetter<ISkyrimMajorRecordGetter> Reference => new FormLink<ISkyrimMajorRecordGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(_data.Span.Slice(0x10, 0x4))));
            public Int32 Unknown3 => BinaryPrimitives.ReadInt32LittleEndian(_data.Slice(0x14, 0x4));
        }

        public partial class FunctionConditionDataBinaryOverlay
        {
            private ReadOnlyMemorySlice<byte> _data2;

            public IFormLinkGetter<ISkyrimMajorRecordGetter> ParameterOneRecord => new FormLink<ISkyrimMajorRecordGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(_data2)));

            public int ParameterOneNumber => BinaryPrimitives.ReadInt32LittleEndian(_data2);

            public IFormLinkGetter<ISkyrimMajorRecordGetter> ParameterTwoRecord => new FormLink<ISkyrimMajorRecordGetter>(FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(_data2.Slice(4))));

            public int ParameterTwoNumber => BinaryPrimitives.ReadInt32LittleEndian(_data2.Slice(4));

            private ReadOnlyMemorySlice<byte> _stringParamData1;
            public bool ParameterOneStringIsSet { get; private set; }
            public string? ParameterOneString => ParameterOneStringIsSet ? BinaryStringUtility.ProcessWholeToZString(_stringParamData1, _package.MetaData.Encodings.NonTranslated) : null;

            private ReadOnlyMemorySlice<byte> _stringParamData2;
            public bool ParameterTwoStringIsSet { get; private set; }
            public string? ParameterTwoString => ParameterTwoStringIsSet ? BinaryStringUtility.ProcessWholeToZString(_stringParamData2, _package.MetaData.Encodings.NonTranslated) : null;

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
                        ParameterOneStringIsSet = true;
                        break;
                    case 0x32534943: // CIS2
                        _stringParamData2 = stream.RemainingMemory.Slice(subFrame.HeaderLength, subFrame.ContentLength);
                        ParameterTwoStringIsSet = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
