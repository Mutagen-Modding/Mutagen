using System.Collections.Generic;
using Mutagen.Bethesda.Internals;

namespace Mutagen.Bethesda.Oblivion
{
    public static class TypeOptionSolidifierMixIns
    {
        #region Normal
        /// <summary>
        /// Scope a load order query to AClothing
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AClothing</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IAClothing, IAClothingGetter> AClothing(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAClothing, IAClothingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAClothingGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAClothing, IAClothingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AClothing
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AClothing</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IAClothing, IAClothingGetter> AClothing(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAClothing, IAClothingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAClothingGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAClothing, IAClothingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Activator
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Activator</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IActivator, IActivatorGetter> Activator(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IActivator, IActivatorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IActivator, IActivatorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Activator
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Activator</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IActivator, IActivatorGetter> Activator(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IActivator, IActivatorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IActivator, IActivatorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AIPackage
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AIPackage</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IAIPackage, IAIPackageGetter> AIPackage(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAIPackage, IAIPackageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAIPackageGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAIPackage, IAIPackageGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AIPackage
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AIPackage</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IAIPackage, IAIPackageGetter> AIPackage(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAIPackage, IAIPackageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAIPackageGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAIPackage, IAIPackageGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AItem</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IAItem, IAItemGetter> AItem(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAItem, IAItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAItem, IAItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AItem</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IAItem, IAItemGetter> AItem(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAItem, IAItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAItem, IAItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AlchemicalApparatus
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AlchemicalApparatus</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IAlchemicalApparatus, IAlchemicalApparatusGetter> AlchemicalApparatus(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAlchemicalApparatus, IAlchemicalApparatusGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AlchemicalApparatus
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AlchemicalApparatus</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IAlchemicalApparatus, IAlchemicalApparatusGetter> AlchemicalApparatus(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAlchemicalApparatus, IAlchemicalApparatusGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ammunition
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ammunition</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IAmmunition, IAmmunitionGetter> Ammunition(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAmmunition, IAmmunitionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAmmunition, IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ammunition
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ammunition</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IAmmunition, IAmmunitionGetter> Ammunition(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAmmunition, IAmmunitionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAmmunition, IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AnimatedObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AnimatedObject</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IAnimatedObject, IAnimatedObjectGetter> AnimatedObject(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAnimatedObject, IAnimatedObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAnimatedObject, IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AnimatedObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AnimatedObject</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IAnimatedObject, IAnimatedObjectGetter> AnimatedObject(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAnimatedObject, IAnimatedObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAnimatedObject, IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ANpc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ANpc</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IANpc, IANpcGetter> ANpc(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IANpc, IANpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IANpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IANpc, IANpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ANpc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ANpc</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IANpc, IANpcGetter> ANpc(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IANpc, IANpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IANpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IANpc, IANpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ANpcSpawn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ANpcSpawn</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IANpcSpawn, IANpcSpawnGetter> ANpcSpawn(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IANpcSpawn, IANpcSpawnGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IANpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IANpcSpawn, IANpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ANpcSpawn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ANpcSpawn</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IANpcSpawn, IANpcSpawnGetter> ANpcSpawn(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IANpcSpawn, IANpcSpawnGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IANpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IANpcSpawn, IANpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Armor
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Armor</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IArmor, IArmorGetter> Armor(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IArmor, IArmorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IArmor, IArmorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Armor
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Armor</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IArmor, IArmorGetter> Armor(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IArmor, IArmorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IArmor, IArmorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ASpell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ASpell</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IASpell, IASpellGetter> ASpell(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IASpell, IASpellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IASpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IASpell, IASpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ASpell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ASpell</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IASpell, IASpellGetter> ASpell(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IASpell, IASpellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IASpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IASpell, IASpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Birthsign
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Birthsign</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IBirthsign, IBirthsignGetter> Birthsign(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IBirthsign, IBirthsignGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBirthsignGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IBirthsign, IBirthsignGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Birthsign
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Birthsign</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IBirthsign, IBirthsignGetter> Birthsign(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IBirthsign, IBirthsignGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBirthsignGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IBirthsign, IBirthsignGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Book
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Book</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IBook, IBookGetter> Book(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IBook, IBookGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IBook, IBookGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Book
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Book</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IBook, IBookGetter> Book(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IBook, IBookGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IBook, IBookGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Cell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Cell</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ICell, ICellGetter> Cell(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ICell, ICellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ICell, ICellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Cell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Cell</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ICell, ICellGetter> Cell(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ICell, ICellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ICell, ICellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Class
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Class</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IClass, IClassGetter> Class(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IClass, IClassGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IClass, IClassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Class
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Class</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IClass, IClassGetter> Class(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IClass, IClassGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IClass, IClassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Climate
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Climate</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IClimate, IClimateGetter> Climate(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IClimate, IClimateGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IClimate, IClimateGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Climate
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Climate</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IClimate, IClimateGetter> Climate(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IClimate, IClimateGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IClimate, IClimateGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Clothing
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Clothing</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IClothing, IClothingGetter> Clothing(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IClothing, IClothingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IClothingGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IClothing, IClothingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Clothing
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Clothing</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IClothing, IClothingGetter> Clothing(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IClothing, IClothingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IClothingGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IClothing, IClothingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CombatStyle
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CombatStyle</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ICombatStyle, ICombatStyleGetter> CombatStyle(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ICombatStyle, ICombatStyleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ICombatStyle, ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CombatStyle
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CombatStyle</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ICombatStyle, ICombatStyleGetter> CombatStyle(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ICombatStyle, ICombatStyleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ICombatStyle, ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Container
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Container</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IContainer, IContainerGetter> Container(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IContainer, IContainerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IContainer, IContainerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Container
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Container</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IContainer, IContainerGetter> Container(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IContainer, IContainerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IContainer, IContainerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Creature
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Creature</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ICreature, ICreatureGetter> Creature(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ICreature, ICreatureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ICreature, ICreatureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Creature
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Creature</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ICreature, ICreatureGetter> Creature(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ICreature, ICreatureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ICreature, ICreatureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogItem</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IDialogItem, IDialogItemGetter> DialogItem(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IDialogItem, IDialogItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IDialogItem, IDialogItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogItem</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IDialogItem, IDialogItemGetter> DialogItem(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IDialogItem, IDialogItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IDialogItem, IDialogItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogTopic
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogTopic</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IDialogTopic, IDialogTopicGetter> DialogTopic(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IDialogTopic, IDialogTopicGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IDialogTopic, IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogTopic
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogTopic</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IDialogTopic, IDialogTopicGetter> DialogTopic(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IDialogTopic, IDialogTopicGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IDialogTopic, IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Door
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Door</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IDoor, IDoorGetter> Door(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IDoor, IDoorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IDoor, IDoorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Door
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Door</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IDoor, IDoorGetter> Door(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IDoor, IDoorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IDoor, IDoorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EffectShader
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EffectShader</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IEffectShader, IEffectShaderGetter> EffectShader(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IEffectShader, IEffectShaderGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IEffectShader, IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EffectShader
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EffectShader</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IEffectShader, IEffectShaderGetter> EffectShader(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IEffectShader, IEffectShaderGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IEffectShader, IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Enchantment
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Enchantment</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IEnchantment, IEnchantmentGetter> Enchantment(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IEnchantment, IEnchantmentGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEnchantmentGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IEnchantment, IEnchantmentGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Enchantment
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Enchantment</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IEnchantment, IEnchantmentGetter> Enchantment(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IEnchantment, IEnchantmentGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEnchantmentGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IEnchantment, IEnchantmentGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Eye
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Eye</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IEye, IEyeGetter> Eye(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IEye, IEyeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEyeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IEye, IEyeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Eye
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Eye</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IEye, IEyeGetter> Eye(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IEye, IEyeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEyeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IEye, IEyeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Faction
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Faction</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IFaction, IFactionGetter> Faction(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IFaction, IFactionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IFaction, IFactionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Faction
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Faction</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IFaction, IFactionGetter> Faction(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IFaction, IFactionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IFaction, IFactionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Flora
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Flora</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IFlora, IFloraGetter> Flora(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IFlora, IFloraGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IFlora, IFloraGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Flora
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Flora</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IFlora, IFloraGetter> Flora(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IFlora, IFloraGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IFlora, IFloraGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Furniture
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Furniture</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IFurniture, IFurnitureGetter> Furniture(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IFurniture, IFurnitureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IFurniture, IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Furniture
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Furniture</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IFurniture, IFurnitureGetter> Furniture(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IFurniture, IFurnitureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IFurniture, IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGameSetting, IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGameSetting, IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingFloat
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSettingFloat</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IGameSettingFloat, IGameSettingFloatGetter> GameSettingFloat(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGameSettingFloat, IGameSettingFloatGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGameSettingFloat, IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingFloat
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSettingFloat</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IGameSettingFloat, IGameSettingFloatGetter> GameSettingFloat(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGameSettingFloat, IGameSettingFloatGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGameSettingFloat, IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingInt
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSettingInt</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IGameSettingInt, IGameSettingIntGetter> GameSettingInt(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGameSettingInt, IGameSettingIntGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGameSettingInt, IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingInt
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSettingInt</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IGameSettingInt, IGameSettingIntGetter> GameSettingInt(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGameSettingInt, IGameSettingIntGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGameSettingInt, IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingString
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSettingString</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IGameSettingString, IGameSettingStringGetter> GameSettingString(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGameSettingString, IGameSettingStringGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGameSettingString, IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingString
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSettingString</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IGameSettingString, IGameSettingStringGetter> GameSettingString(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGameSettingString, IGameSettingStringGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGameSettingString, IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Global
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Global</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IGlobal, IGlobalGetter> Global(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGlobal, IGlobalGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGlobal, IGlobalGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Global
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Global</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IGlobal, IGlobalGetter> Global(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGlobal, IGlobalGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGlobal, IGlobalGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GlobalFloat
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GlobalFloat</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IGlobalFloat, IGlobalFloatGetter> GlobalFloat(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGlobalFloat, IGlobalFloatGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGlobalFloat, IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GlobalFloat
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GlobalFloat</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IGlobalFloat, IGlobalFloatGetter> GlobalFloat(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGlobalFloat, IGlobalFloatGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGlobalFloat, IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GlobalInt
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GlobalInt</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IGlobalInt, IGlobalIntGetter> GlobalInt(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGlobalInt, IGlobalIntGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGlobalInt, IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GlobalInt
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GlobalInt</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IGlobalInt, IGlobalIntGetter> GlobalInt(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGlobalInt, IGlobalIntGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGlobalInt, IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GlobalShort
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GlobalShort</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IGlobalShort, IGlobalShortGetter> GlobalShort(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGlobalShort, IGlobalShortGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGlobalShort, IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GlobalShort
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GlobalShort</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IGlobalShort, IGlobalShortGetter> GlobalShort(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGlobalShort, IGlobalShortGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGlobalShort, IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Grass
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Grass</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IGrass, IGrassGetter> Grass(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGrass, IGrassGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGrass, IGrassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Grass
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Grass</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IGrass, IGrassGetter> Grass(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGrass, IGrassGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGrass, IGrassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hair
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Hair</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IHair, IHairGetter> Hair(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IHair, IHairGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IHair, IHairGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hair
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Hair</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IHair, IHairGetter> Hair(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IHair, IHairGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IHair, IHairGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleAnimation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IdleAnimation</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IIdleAnimation, IIdleAnimationGetter> IdleAnimation(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IIdleAnimation, IIdleAnimationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IIdleAnimation, IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleAnimation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IdleAnimation</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IIdleAnimation, IIdleAnimationGetter> IdleAnimation(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IIdleAnimation, IIdleAnimationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IIdleAnimation, IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingredient
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ingredient</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IIngredient, IIngredientGetter> Ingredient(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IIngredient, IIngredientGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IIngredient, IIngredientGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingredient
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ingredient</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IIngredient, IIngredientGetter> Ingredient(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IIngredient, IIngredientGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IIngredient, IIngredientGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Key
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Key</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IKey, IKeyGetter> Key(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IKey, IKeyGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IKey, IKeyGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Key
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Key</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IKey, IKeyGetter> Key(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IKey, IKeyGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IKey, IKeyGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Landscape
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Landscape</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ILandscape, ILandscapeGetter> Landscape(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILandscape, ILandscapeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILandscape, ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Landscape
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Landscape</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ILandscape, ILandscapeGetter> Landscape(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILandscape, ILandscapeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILandscape, ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LandTexture
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LandTexture</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ILandTexture, ILandTextureGetter> LandTexture(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILandTexture, ILandTextureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILandTextureGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILandTexture, ILandTextureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LandTexture
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LandTexture</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ILandTexture, ILandTextureGetter> LandTexture(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILandTexture, ILandTextureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILandTextureGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILandTexture, ILandTextureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledCreature
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledCreature</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ILeveledCreature, ILeveledCreatureGetter> LeveledCreature(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILeveledCreature, ILeveledCreatureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledCreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILeveledCreature, ILeveledCreatureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledCreature
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledCreature</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ILeveledCreature, ILeveledCreatureGetter> LeveledCreature(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILeveledCreature, ILeveledCreatureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledCreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILeveledCreature, ILeveledCreatureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledItem</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ILeveledItem, ILeveledItemGetter> LeveledItem(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILeveledItem, ILeveledItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILeveledItem, ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledItem</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ILeveledItem, ILeveledItemGetter> LeveledItem(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILeveledItem, ILeveledItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILeveledItem, ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledSpell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledSpell</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ILeveledSpell, ILeveledSpellGetter> LeveledSpell(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILeveledSpell, ILeveledSpellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILeveledSpell, ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledSpell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledSpell</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ILeveledSpell, ILeveledSpellGetter> LeveledSpell(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILeveledSpell, ILeveledSpellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILeveledSpell, ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Light
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Light</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ILight, ILightGetter> Light(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILight, ILightGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILight, ILightGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Light
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Light</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ILight, ILightGetter> Light(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILight, ILightGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILight, ILightGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LoadScreen
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LoadScreen</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ILoadScreen, ILoadScreenGetter> LoadScreen(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILoadScreen, ILoadScreenGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILoadScreen, ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LoadScreen
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LoadScreen</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ILoadScreen, ILoadScreenGetter> LoadScreen(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILoadScreen, ILoadScreenGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILoadScreen, ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MagicEffect
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MagicEffect</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IMagicEffect, IMagicEffectGetter> MagicEffect(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IMagicEffect, IMagicEffectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IMagicEffect, IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MagicEffect
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MagicEffect</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IMagicEffect, IMagicEffectGetter> MagicEffect(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IMagicEffect, IMagicEffectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IMagicEffect, IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Miscellaneous
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Miscellaneous</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IMiscellaneous, IMiscellaneousGetter> Miscellaneous(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IMiscellaneous, IMiscellaneousGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMiscellaneousGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IMiscellaneous, IMiscellaneousGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Miscellaneous
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Miscellaneous</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IMiscellaneous, IMiscellaneousGetter> Miscellaneous(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IMiscellaneous, IMiscellaneousGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMiscellaneousGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IMiscellaneous, IMiscellaneousGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TypedLoadOrderAccess<IOblivionMod, INpc, INpcGetter> Npc(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, INpc, INpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, INpc, INpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TypedLoadOrderAccess<IOblivionMod, INpc, INpcGetter> Npc(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, INpc, INpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, INpc, INpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to OblivionMajorRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on OblivionMajorRecord</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionMajorRecord, IOblivionMajorRecordGetter> OblivionMajorRecord(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionMajorRecord, IOblivionMajorRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IOblivionMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IOblivionMajorRecord, IOblivionMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to OblivionMajorRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on OblivionMajorRecord</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionMajorRecord, IOblivionMajorRecordGetter> OblivionMajorRecord(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionMajorRecord, IOblivionMajorRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IOblivionMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IOblivionMajorRecord, IOblivionMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PathGrid
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PathGrid</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IPathGrid, IPathGridGetter> PathGrid(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPathGrid, IPathGridGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPathGridGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPathGrid, IPathGridGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PathGrid
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PathGrid</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IPathGrid, IPathGridGetter> PathGrid(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPathGrid, IPathGridGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPathGridGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPathGrid, IPathGridGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Place
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Place</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IPlace, IPlaceGetter> Place(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPlace, IPlaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPlace, IPlaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Place
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Place</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IPlace, IPlaceGetter> Place(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPlace, IPlaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPlace, IPlaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedCreature
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedCreature</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IPlacedCreature, IPlacedCreatureGetter> PlacedCreature(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPlacedCreature, IPlacedCreatureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedCreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPlacedCreature, IPlacedCreatureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedCreature
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedCreature</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IPlacedCreature, IPlacedCreatureGetter> PlacedCreature(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPlacedCreature, IPlacedCreatureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedCreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPlacedCreature, IPlacedCreatureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedNpc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedNpc</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IPlacedNpc, IPlacedNpcGetter> PlacedNpc(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPlacedNpc, IPlacedNpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPlacedNpc, IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedNpc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedNpc</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IPlacedNpc, IPlacedNpcGetter> PlacedNpc(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPlacedNpc, IPlacedNpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPlacedNpc, IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedObject</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IPlacedObject, IPlacedObjectGetter> PlacedObject(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPlacedObject, IPlacedObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPlacedObject, IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedObject</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IPlacedObject, IPlacedObjectGetter> PlacedObject(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPlacedObject, IPlacedObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPlacedObject, IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Potion
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Potion</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IPotion, IPotionGetter> Potion(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPotion, IPotionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPotionGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPotion, IPotionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Potion
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Potion</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IPotion, IPotionGetter> Potion(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPotion, IPotionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPotionGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPotion, IPotionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Quest
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Quest</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IQuest, IQuestGetter> Quest(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IQuest, IQuestGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IQuest, IQuestGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Quest
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Quest</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IQuest, IQuestGetter> Quest(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IQuest, IQuestGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IQuest, IQuestGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IRace, IRaceGetter> Race(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IRace, IRaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IRace, IRaceGetter> Race(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IRace, IRaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Region
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Region</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IRegion, IRegionGetter> Region(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IRegion, IRegionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IRegion, IRegionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Region
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Region</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IRegion, IRegionGetter> Region(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IRegion, IRegionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IRegion, IRegionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Road
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Road</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IRoad, IRoadGetter> Road(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IRoad, IRoadGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRoadGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IRoad, IRoadGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Road
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Road</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IRoad, IRoadGetter> Road(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IRoad, IRoadGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRoadGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IRoad, IRoadGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Script
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Script</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IScript, IScriptGetter> Script(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IScript, IScriptGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IScriptGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IScript, IScriptGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Script
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Script</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IScript, IScriptGetter> Script(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IScript, IScriptGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IScriptGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IScript, IScriptGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SigilStone
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SigilStone</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ISigilStone, ISigilStoneGetter> SigilStone(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISigilStone, ISigilStoneGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISigilStoneGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISigilStone, ISigilStoneGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SigilStone
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SigilStone</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ISigilStone, ISigilStoneGetter> SigilStone(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISigilStone, ISigilStoneGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISigilStoneGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISigilStone, ISigilStoneGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SkillRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SkillRecord</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ISkillRecord, ISkillRecordGetter> SkillRecord(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISkillRecord, ISkillRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISkillRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISkillRecord, ISkillRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SkillRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SkillRecord</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ISkillRecord, ISkillRecordGetter> SkillRecord(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISkillRecord, ISkillRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISkillRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISkillRecord, ISkillRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoulGem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoulGem</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ISoulGem, ISoulGemGetter> SoulGem(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISoulGem, ISoulGemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISoulGem, ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoulGem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoulGem</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ISoulGem, ISoulGemGetter> SoulGem(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISoulGem, ISoulGemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISoulGem, ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Sound
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Sound</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ISound, ISoundGetter> Sound(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISound, ISoundGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISound, ISoundGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Sound
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Sound</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ISound, ISoundGetter> Sound(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISound, ISoundGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISound, ISoundGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Spell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Spell</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ISpell, ISpellGetter> Spell(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISpell, ISpellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISpell, ISpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Spell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Spell</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ISpell, ISpellGetter> Spell(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISpell, ISpellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISpell, ISpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SpellLeveled
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SpellLeveled</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ISpellLeveled, ISpellLeveledGetter> SpellLeveled(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISpellLeveled, ISpellLeveledGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISpellLeveledGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISpellLeveled, ISpellLeveledGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SpellLeveled
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SpellLeveled</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ISpellLeveled, ISpellLeveledGetter> SpellLeveled(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISpellLeveled, ISpellLeveledGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISpellLeveledGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISpellLeveled, ISpellLeveledGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SpellUnleveled
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SpellUnleveled</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ISpellUnleveled, ISpellUnleveledGetter> SpellUnleveled(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISpellUnleveled, ISpellUnleveledGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISpellUnleveledGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISpellUnleveled, ISpellUnleveledGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SpellUnleveled
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SpellUnleveled</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ISpellUnleveled, ISpellUnleveledGetter> SpellUnleveled(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISpellUnleveled, ISpellUnleveledGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISpellUnleveledGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISpellUnleveled, ISpellUnleveledGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Static
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Static</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IStatic, IStaticGetter> Static(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IStatic, IStaticGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IStatic, IStaticGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Static
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Static</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IStatic, IStaticGetter> Static(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IStatic, IStaticGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IStatic, IStaticGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Subspace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Subspace</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ISubspace, ISubspaceGetter> Subspace(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISubspace, ISubspaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISubspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISubspace, ISubspaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Subspace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Subspace</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ISubspace, ISubspaceGetter> Subspace(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISubspace, ISubspaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISubspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISubspace, ISubspaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Tree
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Tree</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ITree, ITreeGetter> Tree(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ITree, ITreeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ITree, ITreeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Tree
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Tree</returns>
        public static TypedLoadOrderAccess<IOblivionMod, ITree, ITreeGetter> Tree(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ITree, ITreeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ITree, ITreeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Water
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Water</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IWater, IWaterGetter> Water(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IWater, IWaterGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IWater, IWaterGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Water
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Water</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IWater, IWaterGetter> Water(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IWater, IWaterGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IWater, IWaterGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weapon
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Weapon</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IWeapon, IWeaponGetter> Weapon(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IWeapon, IWeaponGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IWeapon, IWeaponGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weapon
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Weapon</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IWeapon, IWeaponGetter> Weapon(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IWeapon, IWeaponGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IWeapon, IWeaponGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weather
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Weather</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IWeather, IWeatherGetter> Weather(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IWeather, IWeatherGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IWeather, IWeatherGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weather
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Weather</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IWeather, IWeatherGetter> Weather(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IWeather, IWeatherGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IWeather, IWeatherGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Worldspace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Worldspace</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IWorldspace, IWorldspaceGetter> Worldspace(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IWorldspace, IWorldspaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IWorldspace, IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Worldspace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Worldspace</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IWorldspace, IWorldspaceGetter> Worldspace(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IWorldspace, IWorldspaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IWorldspace, IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

        #region Link Interfaces
        /// <summary>
        /// Scope a load order query to IOwner
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IOwner</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOwner, IOwnerGetter> IOwner(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOwner, IOwnerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IOwner, IOwnerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IOwner
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IOwner</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOwner, IOwnerGetter> IOwner(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOwner, IOwnerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IOwner, IOwnerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaced
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlaced</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IPlaced, IPlacedGetter> IPlaced(this IEnumerable<IModListing<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPlaced, IPlacedGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPlaced, IPlacedGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaced
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlaced</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IPlaced, IPlacedGetter> IPlaced(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPlaced, IPlacedGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPlaced, IPlacedGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

    }
}
