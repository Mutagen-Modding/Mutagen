using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.Starfield
{
    public static class TypeOptionSolidifierMixIns
    {
        #region Normal
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

    }
}
