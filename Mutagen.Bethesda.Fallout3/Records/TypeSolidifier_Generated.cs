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
        /// Scope a load order query to Class
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Class</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IClass, IClassGetter> Class(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IClass, IClassGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IClass, IClassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Class
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Class</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IClass, IClassGetter> Class(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IClass, IClassGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IClass, IClassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Eyes
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Eyes</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEyes, IEyesGetter> Eyes(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEyes, IEyesGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEyesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IEyes, IEyesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Eyes
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Eyes</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEyes, IEyesGetter> Eyes(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEyes, IEyesGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEyesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IEyes, IEyesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Faction
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Faction</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFaction, IFactionGetter> Faction(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFaction, IFactionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IFaction, IFactionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Faction
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Faction</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFaction, IFactionGetter> Faction(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFaction, IFactionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IFaction, IFactionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
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
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
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
        /// Scope a load order query to Hair
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Hair</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IHair, IHairGetter> Hair(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IHair, IHairGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IHair, IHairGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hair
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Hair</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IHair, IHairGetter> Hair(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IHair, IHairGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IHair, IHairGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to HeadPart
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on HeadPart</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IHeadPart, IHeadPartGetter> HeadPart(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IHeadPart, IHeadPartGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IHeadPart, IHeadPartGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to HeadPart
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on HeadPart</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IHeadPart, IHeadPartGetter> HeadPart(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IHeadPart, IHeadPartGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IHeadPart, IHeadPartGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MenuIcon
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MenuIcon</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMenuIcon, IMenuIconGetter> MenuIcon(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMenuIcon, IMenuIconGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMenuIconGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IMenuIcon, IMenuIconGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MenuIcon
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MenuIcon</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMenuIcon, IMenuIconGetter> MenuIcon(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMenuIcon, IMenuIconGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMenuIconGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IMenuIcon, IMenuIconGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INpc, INpcGetter> Npc(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INpc, INpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, INpc, INpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INpc, INpcGetter> Npc(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INpc, INpcGetter>(
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
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRace, IRaceGetter> Race(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRace, IRaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRace, IRaceGetter> Race(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRace, IRaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Sound
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Sound</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ISound, ISoundGetter> Sound(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ISound, ISoundGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ISound, ISoundGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Sound
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Sound</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ISound, ISoundGetter> Sound(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ISound, ISoundGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ISound, ISoundGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TextureSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on TextureSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ITextureSet, ITextureSetGetter> TextureSet(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ITextureSet, ITextureSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ITextureSet, ITextureSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TextureSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on TextureSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ITextureSet, ITextureSetGetter> TextureSet(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ITextureSet, ITextureSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ITextureSet, ITextureSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

        #region Link Interfaces
        /// <summary>
        /// Scope a load order query to IRelatable
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IRelatable</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRelatable, IRelatableGetter> IRelatable(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRelatable, IRelatableGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRelatableGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRelatable, IRelatableGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRelatable
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IRelatable</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRelatable, IRelatableGetter> IRelatable(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRelatable, IRelatableGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRelatableGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRelatable, IRelatableGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

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
