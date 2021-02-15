using System.Collections.Generic;
using Mutagen.Bethesda.Internals;

namespace Mutagen.Bethesda.Fallout4
{
    public static class TypeOptionSolidifierMixIns
    {
        #region Normal
        /// <summary>
        /// Scope a load order query to Fallout4MajorRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Fallout4MajorRecord</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IFallout4MajorRecord, IFallout4MajorRecordGetter> Fallout4MajorRecord(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IFallout4MajorRecord, IFallout4MajorRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFallout4MajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IFallout4MajorRecord, IFallout4MajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Fallout4MajorRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Fallout4MajorRecord</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IFallout4MajorRecord, IFallout4MajorRecordGetter> Fallout4MajorRecord(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IFallout4MajorRecord, IFallout4MajorRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFallout4MajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IFallout4MajorRecord, IFallout4MajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingBool
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSettingBool</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSettingBool, IGameSettingBoolGetter> GameSettingBool(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSettingBool, IGameSettingBoolGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingBoolGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSettingBool, IGameSettingBoolGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingBool
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSettingBool</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSettingBool, IGameSettingBoolGetter> GameSettingBool(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSettingBool, IGameSettingBoolGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingBoolGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSettingBool, IGameSettingBoolGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingFloat
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSettingFloat</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSettingFloat, IGameSettingFloatGetter> GameSettingFloat(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSettingFloat, IGameSettingFloatGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSettingFloat, IGameSettingFloatGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingFloat
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSettingFloat</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSettingFloat, IGameSettingFloatGetter> GameSettingFloat(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSettingFloat, IGameSettingFloatGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSettingFloat, IGameSettingFloatGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingInt
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSettingInt</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSettingInt, IGameSettingIntGetter> GameSettingInt(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSettingInt, IGameSettingIntGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSettingInt, IGameSettingIntGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingInt
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSettingInt</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSettingInt, IGameSettingIntGetter> GameSettingInt(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSettingInt, IGameSettingIntGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSettingInt, IGameSettingIntGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingString
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSettingString</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSettingString, IGameSettingStringGetter> GameSettingString(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSettingString, IGameSettingStringGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSettingString, IGameSettingStringGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingString
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSettingString</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSettingString, IGameSettingStringGetter> GameSettingString(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSettingString, IGameSettingStringGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSettingString, IGameSettingStringGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingUInt
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSettingUInt</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSettingUInt, IGameSettingUIntGetter> GameSettingUInt(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSettingUInt, IGameSettingUIntGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingUIntGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSettingUInt, IGameSettingUIntGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingUInt
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSettingUInt</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSettingUInt, IGameSettingUIntGetter> GameSettingUInt(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IGameSettingUInt, IGameSettingUIntGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingUIntGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSettingUInt, IGameSettingUIntGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Keyword
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Keyword</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IKeyword, IKeywordGetter> Keyword(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IKeyword, IKeywordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeywordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IKeyword, IKeywordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Keyword
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Keyword</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IKeyword, IKeywordGetter> Keyword(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IKeyword, IKeywordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeywordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IKeyword, IKeywordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

        #region Link Interfaces
        /// <summary>
        /// Scope a load order query to IKeywordLinkedReference
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IKeywordLinkedReference</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter> IKeywordLinkedReference(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IKeywordLinkedReference
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IKeywordLinkedReference</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter> IKeywordLinkedReference(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IFallout4ModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

    }
}
