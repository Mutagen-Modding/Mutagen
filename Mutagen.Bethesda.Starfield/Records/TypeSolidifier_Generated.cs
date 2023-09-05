using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.Starfield
{
    public static class TypeOptionSolidifierMixIns
    {
        #region Normal
        /// <summary>
        /// Scope a load order query to ActionRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ActionRecord</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActionRecord, IActionRecordGetter> ActionRecord(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActionRecord, IActionRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IActionRecord, IActionRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActionRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ActionRecord</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActionRecord, IActionRecordGetter> ActionRecord(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActionRecord, IActionRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IActionRecord, IActionRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActorValueInformation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ActorValueInformation</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActorValueInformation, IActorValueInformationGetter> ActorValueInformation(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActorValueInformation, IActorValueInformationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IActorValueInformation, IActorValueInformationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActorValueInformation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ActorValueInformation</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActorValueInformation, IActorValueInformationGetter> ActorValueInformation(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActorValueInformation, IActorValueInformationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IActorValueInformation, IActorValueInformationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AttractionRule
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AttractionRule</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAttractionRule, IAttractionRuleGetter> AttractionRule(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAttractionRule, IAttractionRuleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAttractionRuleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAttractionRule, IAttractionRuleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AttractionRule
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AttractionRule</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAttractionRule, IAttractionRuleGetter> AttractionRule(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAttractionRule, IAttractionRuleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAttractionRuleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAttractionRule, IAttractionRuleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Class
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Class</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IClass, IClassGetter> Class(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IClass, IClassGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IClass, IClassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Class
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Class</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IClass, IClassGetter> Class(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IClass, IClassGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IClass, IClassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CurveTable
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CurveTable</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICurveTable, ICurveTableGetter> CurveTable(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICurveTable, ICurveTableGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICurveTableGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ICurveTable, ICurveTableGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CurveTable
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CurveTable</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICurveTable, ICurveTableGetter> CurveTable(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICurveTable, ICurveTableGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICurveTableGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ICurveTable, ICurveTableGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DamageType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DamageType</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDamageType, IDamageTypeGetter> DamageType(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDamageType, IDamageTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDamageTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IDamageType, IDamageTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DamageType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DamageType</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDamageType, IDamageTypeGetter> DamageType(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDamageType, IDamageTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDamageTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IDamageType, IDamageTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FFKW
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on FFKW</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFFKW, IFFKWGetter> FFKW(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFFKW, IFFKWGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFFKWGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IFFKW, IFFKWGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FFKW
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on FFKW</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFFKW, IFFKWGetter> FFKW(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFFKW, IFFKWGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFFKWGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IFFKW, IFFKWGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Global
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Global</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGlobal, IGlobalGetter> Global(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGlobal, IGlobalGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IGlobal, IGlobalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Global
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Global</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGlobal, IGlobalGetter> Global(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGlobal, IGlobalGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IGlobal, IGlobalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Keyword
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Keyword</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IKeyword, IKeywordGetter> Keyword(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IKeyword, IKeywordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeywordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IKeyword, IKeywordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Keyword
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Keyword</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IKeyword, IKeywordGetter> Keyword(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IKeyword, IKeywordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeywordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IKeyword, IKeywordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LocationReferenceType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LocationReferenceType</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILocationReferenceType, ILocationReferenceTypeGetter> LocationReferenceType(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LocationReferenceType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LocationReferenceType</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILocationReferenceType, ILocationReferenceTypeGetter> LocationReferenceType(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INpc, INpcGetter> Npc(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INpc, INpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, INpc, INpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INpc, INpcGetter> Npc(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INpc, INpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, INpc, INpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IRace, IRaceGetter> Race(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IRace, IRaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IRace, IRaceGetter> Race(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IRace, IRaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Spell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Spell</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISpell, ISpellGetter> Spell(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISpell, ISpellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISpell, ISpellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Spell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Spell</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISpell, ISpellGetter> Spell(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISpell, ISpellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISpell, ISpellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StarfieldMajorRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on StarfieldMajorRecord</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IStarfieldMajorRecord, IStarfieldMajorRecordGetter> StarfieldMajorRecord(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IStarfieldMajorRecord, IStarfieldMajorRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStarfieldMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IStarfieldMajorRecord, IStarfieldMajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StarfieldMajorRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on StarfieldMajorRecord</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IStarfieldMajorRecord, IStarfieldMajorRecordGetter> StarfieldMajorRecord(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IStarfieldMajorRecord, IStarfieldMajorRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStarfieldMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IStarfieldMajorRecord, IStarfieldMajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TextureSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on TextureSet</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITextureSet, ITextureSetGetter> TextureSet(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITextureSet, ITextureSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ITextureSet, ITextureSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TextureSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on TextureSet</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITextureSet, ITextureSetGetter> TextureSet(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITextureSet, ITextureSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ITextureSet, ITextureSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Transform
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Transform</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITransform, ITransformGetter> Transform(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITransform, ITransformGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITransformGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ITransform, ITransformGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Transform
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Transform</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITransform, ITransformGetter> Transform(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITransform, ITransformGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITransformGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ITransform, ITransformGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weapon
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Weapon</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWeapon, IWeaponGetter> Weapon(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWeapon, IWeaponGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IWeapon, IWeaponGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weapon
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Weapon</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWeapon, IWeaponGetter> Weapon(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWeapon, IWeaponGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IWeapon, IWeaponGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

        #region Link Interfaces
        /// <summary>
        /// Scope a load order query to IIdleRelation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IIdleRelation</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IIdleRelation, IIdleRelationGetter> IIdleRelation(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IIdleRelation, IIdleRelationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IIdleRelation, IIdleRelationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IIdleRelation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IIdleRelation</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IIdleRelation, IIdleRelationGetter> IIdleRelation(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IIdleRelation, IIdleRelationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IIdleRelation, IIdleRelationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IKeywordLinkedReference
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IKeywordLinkedReference</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter> IKeywordLinkedReference(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IKeywordLinkedReference
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IKeywordLinkedReference</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter> IKeywordLinkedReference(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILocationRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ILocationRecord</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILocationRecord, ILocationRecordGetter> ILocationRecord(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILocationRecord, ILocationRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILocationRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILocationRecord, ILocationRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILocationRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ILocationRecord</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILocationRecord, ILocationRecordGetter> ILocationRecord(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILocationRecord, ILocationRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILocationRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILocationRecord, ILocationRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IObjectId
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IObjectId</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IObjectId, IObjectIdGetter> IObjectId(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IObjectId, IObjectIdGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IObjectIdGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IObjectId, IObjectIdGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IObjectId
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IObjectId</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IObjectId, IObjectIdGetter> IObjectId(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IObjectId, IObjectIdGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IObjectIdGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IObjectId, IObjectIdGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaceableObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlaceableObject</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlaceableObject, IPlaceableObjectGetter> IPlaceableObject(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlaceableObject, IPlaceableObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlaceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlaceableObject, IPlaceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaceableObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlaceableObject</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlaceableObject, IPlaceableObjectGetter> IPlaceableObject(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlaceableObject, IPlaceableObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlaceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlaceableObject, IPlaceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IReferenceableObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IReferenceableObject</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IReferenceableObject, IReferenceableObjectGetter> IReferenceableObject(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IReferenceableObject, IReferenceableObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IReferenceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IReferenceableObject, IReferenceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IReferenceableObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IReferenceableObject</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IReferenceableObject, IReferenceableObjectGetter> IReferenceableObject(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IReferenceableObject, IReferenceableObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IReferenceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IReferenceableObject, IReferenceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IExplodeSpawn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IExplodeSpawn</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IExplodeSpawn, IExplodeSpawnGetter> IExplodeSpawn(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IExplodeSpawn, IExplodeSpawnGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IExplodeSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IExplodeSpawn, IExplodeSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IExplodeSpawn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IExplodeSpawn</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IExplodeSpawn, IExplodeSpawnGetter> IExplodeSpawn(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IExplodeSpawn, IExplodeSpawnGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IExplodeSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IExplodeSpawn, IExplodeSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

    }
}
