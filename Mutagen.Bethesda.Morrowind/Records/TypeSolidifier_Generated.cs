using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.Morrowind
{
    public static class TypeOptionSolidifierMixIns
    {
        #region Normal
        /// <summary>
        /// Scope a load order query to Cell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Cell</returns>
        public static TypedLoadOrderAccess<IMorrowindMod, IMorrowindModGetter, ICell, ICellGetter> Cell(this IEnumerable<IModListingGetter<IMorrowindModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IMorrowindMod, IMorrowindModGetter, ICell, ICellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IMorrowindMod, IMorrowindModGetter, ICell, ICellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Cell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Cell</returns>
        public static TypedLoadOrderAccess<IMorrowindMod, IMorrowindModGetter, ICell, ICellGetter> Cell(this IEnumerable<IMorrowindModGetter> mods)
        {
            return new TypedLoadOrderAccess<IMorrowindMod, IMorrowindModGetter, ICell, ICellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IMorrowindMod, IMorrowindModGetter, ICell, ICellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TopLevelTypedLoadOrderAccess<IMorrowindMod, IMorrowindModGetter, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IModListingGetter<IMorrowindModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IMorrowindMod, IMorrowindModGetter, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IMorrowindMod, IMorrowindModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TopLevelTypedLoadOrderAccess<IMorrowindMod, IMorrowindModGetter, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IMorrowindModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IMorrowindMod, IMorrowindModGetter, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IMorrowindMod, IMorrowindModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MorrowindMajorRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MorrowindMajorRecord</returns>
        public static TypedLoadOrderAccess<IMorrowindMod, IMorrowindModGetter, IMorrowindMajorRecord, IMorrowindMajorRecordGetter> MorrowindMajorRecord(this IEnumerable<IModListingGetter<IMorrowindModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IMorrowindMod, IMorrowindModGetter, IMorrowindMajorRecord, IMorrowindMajorRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMorrowindMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IMorrowindMod, IMorrowindModGetter, IMorrowindMajorRecord, IMorrowindMajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MorrowindMajorRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MorrowindMajorRecord</returns>
        public static TypedLoadOrderAccess<IMorrowindMod, IMorrowindModGetter, IMorrowindMajorRecord, IMorrowindMajorRecordGetter> MorrowindMajorRecord(this IEnumerable<IMorrowindModGetter> mods)
        {
            return new TypedLoadOrderAccess<IMorrowindMod, IMorrowindModGetter, IMorrowindMajorRecord, IMorrowindMajorRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMorrowindMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IMorrowindMod, IMorrowindModGetter, IMorrowindMajorRecord, IMorrowindMajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

    }
}
