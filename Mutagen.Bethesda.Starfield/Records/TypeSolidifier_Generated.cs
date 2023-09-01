using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.Starfield
{
    public static class TypeOptionSolidifierMixIns
    {
        #region Normal
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

        #endregion

    }
}
