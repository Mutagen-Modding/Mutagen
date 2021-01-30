using System.Collections.Generic;
using Mutagen.Bethesda.Internals;

namespace Mutagen.Bethesda.Fallout4
{
    public static class TypeOptionSolidifierMixIns
    {
        #region Normal
        /// <summary>
        /// Scope a load order query to ActionRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ActionRecord</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IActionRecord, IActionRecordGetter> ActionRecord(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IActionRecord, IActionRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IActionRecord, IActionRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActionRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ActionRecord</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IActionRecord, IActionRecordGetter> ActionRecord(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IActionRecord, IActionRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IActionRecord, IActionRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AttractionRule
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AttractionRule</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IAttractionRule, IAttractionRuleGetter> AttractionRule(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IAttractionRule, IAttractionRuleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAttractionRuleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IAttractionRule, IAttractionRuleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AttractionRule
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AttractionRule</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IAttractionRule, IAttractionRuleGetter> AttractionRule(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IAttractionRule, IAttractionRuleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAttractionRuleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IAttractionRule, IAttractionRuleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Fallout4MajorRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Fallout4MajorRecord</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IFallout4MajorRecord, IFallout4MajorRecordGetter> Fallout4MajorRecord(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IFallout4MajorRecord, IFallout4MajorRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFallout4MajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IFallout4MajorRecord, IFallout4MajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Fallout4MajorRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Fallout4MajorRecord</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IFallout4MajorRecord, IFallout4MajorRecordGetter> Fallout4MajorRecord(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IFallout4MajorRecord, IFallout4MajorRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFallout4MajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IFallout4MajorRecord, IFallout4MajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingBool
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSettingBool</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IGameSettingBool, IGameSettingBoolGetter> GameSettingBool(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IGameSettingBool, IGameSettingBoolGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingBoolGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSettingBool, IGameSettingBoolGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingBool
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSettingBool</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IGameSettingBool, IGameSettingBoolGetter> GameSettingBool(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IGameSettingBool, IGameSettingBoolGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingBoolGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSettingBool, IGameSettingBoolGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingFloat
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSettingFloat</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IGameSettingFloat, IGameSettingFloatGetter> GameSettingFloat(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IGameSettingFloat, IGameSettingFloatGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSettingFloat, IGameSettingFloatGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingFloat
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSettingFloat</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IGameSettingFloat, IGameSettingFloatGetter> GameSettingFloat(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IGameSettingFloat, IGameSettingFloatGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSettingFloat, IGameSettingFloatGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingInt
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSettingInt</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IGameSettingInt, IGameSettingIntGetter> GameSettingInt(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IGameSettingInt, IGameSettingIntGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSettingInt, IGameSettingIntGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingInt
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSettingInt</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IGameSettingInt, IGameSettingIntGetter> GameSettingInt(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IGameSettingInt, IGameSettingIntGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSettingInt, IGameSettingIntGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingString
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSettingString</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IGameSettingString, IGameSettingStringGetter> GameSettingString(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IGameSettingString, IGameSettingStringGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSettingString, IGameSettingStringGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingString
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSettingString</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IGameSettingString, IGameSettingStringGetter> GameSettingString(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IGameSettingString, IGameSettingStringGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSettingString, IGameSettingStringGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingUInt
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSettingUInt</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IGameSettingUInt, IGameSettingUIntGetter> GameSettingUInt(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IGameSettingUInt, IGameSettingUIntGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingUIntGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSettingUInt, IGameSettingUIntGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingUInt
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSettingUInt</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IGameSettingUInt, IGameSettingUIntGetter> GameSettingUInt(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IGameSettingUInt, IGameSettingUIntGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingUIntGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IGameSettingUInt, IGameSettingUIntGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Keyword
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Keyword</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IKeyword, IKeywordGetter> Keyword(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IKeyword, IKeywordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeywordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IKeyword, IKeywordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Keyword
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Keyword</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IKeyword, IKeywordGetter> Keyword(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IKeyword, IKeywordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeywordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IKeyword, IKeywordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LocationReferenceType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LocationReferenceType</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, ILocationReferenceType, ILocationReferenceTypeGetter> LocationReferenceType(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, ILocationReferenceType, ILocationReferenceTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LocationReferenceType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LocationReferenceType</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, ILocationReferenceType, ILocationReferenceTypeGetter> LocationReferenceType(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, ILocationReferenceType, ILocationReferenceTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Transform
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Transform</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, ITransform, ITransformGetter> Transform(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, ITransform, ITransformGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITransformGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, ITransform, ITransformGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Transform
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Transform</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, ITransform, ITransformGetter> Transform(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, ITransform, ITransformGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITransformGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, ITransform, ITransformGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

        #region Link Interfaces
        /// <summary>
        /// Scope a load order query to IIdleRelation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IIdleRelation</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IIdleRelation, IIdleRelationGetter> IIdleRelation(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IIdleRelation, IIdleRelationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IIdleRelation, IIdleRelationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IIdleRelation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IIdleRelation</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IIdleRelation, IIdleRelationGetter> IIdleRelation(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IIdleRelation, IIdleRelationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IIdleRelation, IIdleRelationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IKeywordLinkedReference
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IKeywordLinkedReference</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IKeywordLinkedReference, IKeywordLinkedReferenceGetter> IKeywordLinkedReference(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IKeywordLinkedReference
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IKeywordLinkedReference</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, IKeywordLinkedReference, IKeywordLinkedReferenceGetter> IKeywordLinkedReference(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILocationRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ILocationRecord</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, ILocationRecord, ILocationRecordGetter> ILocationRecord(this IEnumerable<IModListing<IFallout4ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, ILocationRecord, ILocationRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILocationRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, ILocationRecord, ILocationRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILocationRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ILocationRecord</returns>
        public static TypedLoadOrderAccess<IFallout4Mod, ILocationRecord, ILocationRecordGetter> ILocationRecord(this IEnumerable<IFallout4ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout4Mod, ILocationRecord, ILocationRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILocationRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<IFallout4Mod, IFallout4ModGetter, ILocationRecord, ILocationRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

    }
}
