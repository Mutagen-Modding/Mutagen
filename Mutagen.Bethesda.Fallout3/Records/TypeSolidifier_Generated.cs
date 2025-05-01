using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.Fallout3
{
    public static class TypeOptionSolidifierMixIns
    {
        #region Normal
        /// <summary>
        /// Scope a load order query to Cell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Cell</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICell, ICellGetter> Cell(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICell, ICellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICell, ICellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Cell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Cell</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICell, ICellGetter> Cell(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICell, ICellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICell, ICellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Fallout3MajorRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Fallout3MajorRecord</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFallout3MajorRecord, IFallout3MajorRecordGetter> Fallout3MajorRecord(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFallout3MajorRecord, IFallout3MajorRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFallout3MajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IFallout3MajorRecord, IFallout3MajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Fallout3MajorRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Fallout3MajorRecord</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFallout3MajorRecord, IFallout3MajorRecordGetter> Fallout3MajorRecord(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFallout3MajorRecord, IFallout3MajorRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFallout3MajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IFallout3MajorRecord, IFallout3MajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Global
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Global</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IGlobal, IGlobalGetter> Global(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IGlobal, IGlobalGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IGlobal, IGlobalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Global
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Global</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IGlobal, IGlobalGetter> Global(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IGlobal, IGlobalGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IGlobal, IGlobalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INpc, INpcGetter> Npc(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INpc, INpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, INpc, INpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INpc, INpcGetter> Npc(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INpc, INpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, INpc, INpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedObject</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedObject, IPlacedObjectGetter> PlacedObject(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedObject, IPlacedObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlacedObject, IPlacedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedObject</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedObject, IPlacedObjectGetter> PlacedObject(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedObject, IPlacedObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlacedObject, IPlacedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRace, IRaceGetter> Race(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRace, IRaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRace, IRaceGetter> Race(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRace, IRaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

        #region Link Interfaces
        /// <summary>
        /// Scope a load order query to IPlaced
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlaced</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlaced, IPlacedGetter> IPlaced(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlaced, IPlacedGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlaced, IPlacedGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaced
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlaced</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlaced, IPlacedGetter> IPlaced(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlaced, IPlacedGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlaced, IPlacedGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

    }
}
