using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Starfield;

/// <summary>
/// An abstract class representing a Perk entry point effect.
/// Implemented by: [PerkEntryPointModifyValue, PerkEntryPointAddRangeToValue, PerkEntryPointModifyActorValue, PerkEntryPointAbsoluteValue
/// PerkEntryPointAddLeveledItem, PerkEntryPointAddActivateChoice, PerkEntryPointSelectSpell, PerkEntryPointSelectText, PerkEntryPointSetText]
/// </summary>
partial class APerkEntryPointEffect
{
    public enum EntryType
    {
        ModBreathTimer = 0,
        ModMyCriticalHitChance = 1,
        ModMyCriticalHitDamageMult = 2,
        ModMineExplodeChance = 3,
        ModIncomingLimbDamage = 4,
        ModBookActorValueBonus = 5,
        ModRecoveredHealth = 6,
        SetShouldAttack = 7,
        ModBuyPrices = 8,
        AddLeveledListOnDeath = 9,
        SetMaxCarryWeight = 10,
        ModAddictionChance = 11,
        ModAddictionDuration = 12,
        ModPositiveChemDuration = 13,
        Activate = 14,
        IgnoreRunningDuringDetection = 15,
        IgnoreBrokenLock = 16,
        ModEnemyCriticalHitChance = 17,
        ModSneakAttackMult = 18,
        ModMaxPlaceableMines = 19,
        ModBowZoom = 20,
        ModRecoverArrowChance = 21,
        ModExp = 22,
        ModTelekinesisDistance = 23,
        ModTelekinesisDamageMult = 24,
        ModTelekinesisDamage = 25,
        ModBashingDamage = 26,
        ModPowerAttackOxygen = 27,
        ModPowerAttackDamage = 28,
        ModSpellMagnitude = 29,
        ModSpellDuration = 30,
        ModSecondaryValueWeight = 31,
        ModArmorWeight = 32,
        ModIncomingStagger = 33,
        ModTargetStagger = 34,
        ModWeaponAttackDamage = 35,
        ModIncomingWeaponDamage = 36,
        ModTargetDamageResistance = 37,
        ModSpellCost = 38,
        ModPercentBlocked = 39,
        ModShieldDeflectArrowChance = 40,
        ModIncomingSpellMagnitude = 41,
        ModIncomingSpellDuration = 42,
        ModPlayerIntimidation = 43,
        ModRicochetChance = 44,
        ModRicochetDamage = 45,
        ModBribeAmount = 46,
        ModDetectionLight = 47,
        ModDetectionMovement = 48,
        ModScrapRewardMult = 49,
        SetSweepAttack = 50,
        ApplyCombatHitSpell = 51,
        ApplyBashingSpell = 52,
        ApplyReanimateSpell = 53,
        SetBooleanGraphVariable = 54,
        ModSpellCastingSoundEvent = 55,
        ModPickpocketChance = 56,
        ModDetectionSneakSkill = 57,
        ModFallingDamage = 58,
        ModLockpickSweetSpot = 59,
        ModSellPrices = 60,
        CanPickpocketEquippedItem = 61,
        SetPlayerGateLockpick = 62,
        SetLockpickStartingArc = 63,
        SetProgressionPicking = 64,
        SetLockpicksUnbreakable = 65,
        ModAlchemyEffectiveness = 66,
        ApplyWeaponSwingSpell = 67,
        ModCommandedActorLimit = 68,
        ApplySneakingSpell = 69,
        ModPlayerMagicSlowdown = 70,
        ModWardMagickaAbsorptionPct = 71,
        ModInitialIngredientEffectsLearned = 72,
        PurifyAlchemyIngredients = 73,
        SetFilterActivation = 74,
        SetDualCast = 75,
        ModOutgoingExplosionLimbDamage = 76,
        ModEnchantmentPower = 77,
        ModSoulPctCapturedtoWeapon = 78,
        ModVATSAttackOxygen = 79,
        ModReflectDamageChance = 80,
        SetActivateLabel = 81,
        ModKillExperience = 82,
        ModPoisonDoseCount = 83,
        SetApplyPlacedItem = 84,
        ModArmorRating = 85,
        Modlockpickingcrimechance = 86,
        Modingredientsharvested = 87,
        ModSpellRangeTargetLocation = 88,
        ModCriticalChargeMultonRicochet = 89,
        Modlockpickingkeyrewardchance = 90,
        ModAutoLockpickingChance = 91,
        ModAutoHackingChance = 92,
        ModTypedWeaponAttackDamage = 93,
        ModTypedIncomingWeaponDamage = 94,
        ModCharismaChallengeChance = 95,
        ModSprintOxygenDrainRate = 96,
        ModDrawnWeaponWeightSpeedEffect = 97,
        SetPlayerGateHacking = 98,
        ModPlayerExplosionDamage = 99,
        ModPlayerExplosionScale = 100,
        SetRadsToHealthMult = 101,
        ModActorScopeStability = 102,
        ModActorGrenadeSpeedMult = 103,
        ModExplosionForce = 104,
        ModVATSPenetrationMinVisibility = 105,
        ModRadsforRadHealthMax = 106,
        ModVATSPlayerO2OnKillChance = 107,
        SetVATSFillCriticalBarOnHit = 108,
        ModVATSConcentratedFireChanceBonus = 109,
        ModVATSCriticalCount = 110,
        ModVATSHoldEmSteadyBonus = 111,
        ModTypedSpellMagnitude = 112,
        ModTypedIncomingSpellMagnitude = 113,
        SetVATSGunFuNumTargetsForCrits = 114,
        ModOutgoingLimbDamage = 115,
        ModRestoreOxygenCostValue = 116,
        ModVATSReloadOxygen = 117,
        ModIncomingBatteryDamage = 118,
        ModVATSCriticalCharge = 119,
        ModExpLocation = 120,
        ModExpSpeech = 121,
        ModVATSHeadShotChance = 122,
        ModVATSHitChance = 123,
        ModIncomingExplosionDamage = 124,
        ModAmmoHealthMult = 125,
        ModHackingGuesses = 126,
        ModTerminalLockoutTime = 127,
        SetUndetectable = 128,
        InvestInVendor = 129,
        ModOutgoingLimbBashDamage = 130,
        SetRunWhileOverEncumbered = 131,
        GetComponentRadarDistance = 132,
        ShowGrenadeTrajectory = 133,
        ModConeoffireMult = 134,
        ModVATSConcentratedFireDamageMult = 135,
        ModOxygenUse = 136,
        ModVATSCriticalFillChanceOnBank = 137,
        ModVATSCriticalFillChanceOnUse = 138,
        SetVATSCriticalFillOnOxygenReward = 139,
        SetVATSCriticalFillOnStranger = 140,
        ModGunRangeMult = 141,
        ModScopeHoldBreathOxygenDrainMult = 142,
        SetForceDecapitate = 143,
        ModVATSShootExplosiveDamageMult = 144,
        ModScroungerFillAmmoChance = 145,
        SetCanExplodePants = 146,
        SetVATSPenetrationFullDamage = 147,
        ModVATSGunFu2ndTargetDmgMult = 148,
        ModVATSGunFu3rdTargetDmgMult = 149,
        ModVATSGunFu4thTargetDmgMult = 150,
        ModVATSBlitzMaxDistance = 151,
        SetVATSBlitzMaxDmgMult = 152,
        ModVATSBlitzDmgBonusDist = 153,
        ModBashCriticalChance = 154,
        ModJumpingOxygen = 155,
        ModCarbonDioxideforCarbonDioxideOxygenMax = 156,
        SetCarbonDioxidetoOxygenMult = 157,
        ModOxygenDrainRateforRunningwhileEncumbered = 158,
        IronSightsActivate = 159,
        ModRecoveredStarPower = 160,
        ModWorkshopProductionTimer = 161,
        ModActivityMaxRepetitions = 162,
        ModShipSystemRepairRate = 163,
        ModMagazineSize = 164,
        ModTypedIncomingWeaponDamageSpaceship = 165,
        ModWeaponReloadSpeed = 166,
        ModMagazineDrawSpeed = 167,
        ModResearchCritChance = 168,
        ModResearchCost = 169,
        ModShipCargoMax = 170,
        ModShipGravJumpFuelCost = 171,
        ModShipGravJumpCalculationTime = 172,
        ModPlayerGravity = 173,
        ModWorkshopBuildCost = 174,
        ModWorkshopRepairCost = 175,
        ModPersuasion_Skill_Player_Success_Chance = 176,
        ModPlayer_Bounty = 177,
        ModCraftingDupeChance = 178,
        ModCraftingReturnQuantity = 179,
        ModSpellArea = 180,
        ModCraftingCreationRecipeLevel = 181,
        ApplyCombatHitSpellSelf = 182,
        ModItemWeight = 183,
        ModShipTargetingFireAPCost = 184,
        ModSpellEffectMagnitude = 185,
        ModSpellEffectDuration = 186,
        ModSpellEffectArea = 187,
        ModShipGravJumpDistance = 188,
        ModSocialSkillChanceBase = 189,
        ModSocialSkillChanceBonus = 190,
        ModSocialSkillChanceMaxLevelDelta = 191,
        SetZeroGSkillRank = 192,
        ModShipWeaponRechargeRate = 193,
        ModShipWeaponExplosionScale = 194,
        ModShipWeaponAccuracy = 195,
        ModShipTurnSpeedRegularMode = 196,
        ModShipTurnSpeedBoostMode = 197,
        ModShipTurnSpeedThrusterMode = 198,
        ModShipTurnSweetSpot = 199,
        ModPlanetScanMaxDistance = 200,
        ApplyJetpackSpell = 201,
        ModShipSystemFullRepairChance = 202,
        ModPlayerZeroGWeaponRecoilScale = 203,
        ModCraftingFreebieChance = 204,
        ModWeaponShotCount = 205,
        OverrideWeaponProjectile = 206,
        AllowPickpocket = 207,
        ModBleedoutChance = 208,
        ModBleedoutRecoverChance = 209,
        ModBleedoutHealthPercent = 210,
        ApplyStartMantleSpell = 211,
        ApplyStartCombatSlideSpell = 212,
        ApplyCombatHitSpellAttacker = 213,
        ShipBoostDefeatsMissiles = 214,
        ShipBoostDisengagesEnemies = 215,
        AddProducedItems = 216,
        ApplySpellonSpaceshipTargetLockAcquired	 = 217,
        AddHarvestedItems = 218,
        ModPowerGenerated = 219,
        EnableStealthMeter = 220,
        ApplyAimDownSightSpell = 221,
        ModAffinityGain = 222,
        AllowJetpack = 223,
        ModSneakDoorSoundLevel = 224,
        ModCraftingCost = 225,
        EnablePlayerShipThrusterMode = 226,
        ModWeaponWeight = 227,
        ModDamageResistance = 228,
        SetHighestPilotableClassRating = 229,
        ModCriticalChanceonTargetLockedSpaceship = 230,
        ModJetpackFuelDrainMult = 231,
        EnableJetpackHover = 232,
        EnablePlayerShipTargetingMode = 233,
        EnableScanningTargetedShipsInventory = 234,
        ModifyPilotedSpaceship = 235,
        ModifyWeaponFireRate = 236,
        ModifyShipBoostFuel = 237,
        ModifyShipBoostRechargeRate = 238,
        ModShipSystemDamageMitigation = 239,
        ApplySpellonShipBoost = 240,
        AllowScanTrackedItems = 241,
        EnableScannerZoom = 242,
        ModCraftingModFreebieChance = 243,
        EnableScanningTargetedShipsPowerAllocation = 244,
        EnableScanningTargetedShipsCargoValue = 245,
        EnableScanningTargetedShipsCrewCount = 246,
        EnableScanningTargetedShipsHealthValues = 247,
        EnableScanningTargetedShipsDamageRating = 248,
        EnableScanningTargetedShipsWeaponStats = 249,
        ModArmorResistance = 250,
    }

    public enum FunctionType
    {
        SetValue = 1,
        AddValue = 2,
        MultiplyValue = 3,
        AddRangeToValue = 4,
        AddActorValueMult = 5,
        AbsoluteValue = 6,
        NegativeAbsoluteValue = 7,
        AddLeveledList = 8,
        AddActivateChoice = 9,
        SelectSpell = 10,
        SelectText = 11,
        SetToActorValueMult = 12,
        MultiplyActorValueMult = 13,
        MultiplyOnePlusActorValueMult = 14,
        SetText = 15,
        LegendaryMagicEffectEvent = 16,
    }

    public enum ParameterType
    {
        None = 0,
        Float = 1,
        FloatFloat = 2,
        LeveledItem = 3,
        SpellWithStrings = 4,
        Spell = 5,
        String = 6,
        LString = 7,
        ActorValue = 8,
        ReplacementProjectile = 9,
    }
}
