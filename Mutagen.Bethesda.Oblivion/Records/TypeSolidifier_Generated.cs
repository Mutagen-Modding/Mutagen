using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.Oblivion
{
    public static class TypeOptionSolidifierMixIns
    {
        #region Normal
        /// <summary>
        /// Scope a load order query to Activator
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Activator</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IActivator, IActivatorGetter> Activator(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IActivator, IActivatorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IActivator, IActivatorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Activator
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Activator</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IActivator, IActivatorGetter> Activator(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IActivator, IActivatorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IActivator, IActivatorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AIPackage
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AIPackage</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IAIPackage, IAIPackageGetter> AIPackage(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IAIPackage, IAIPackageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAIPackageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IAIPackage, IAIPackageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AIPackage
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AIPackage</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IAIPackage, IAIPackageGetter> AIPackage(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IAIPackage, IAIPackageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAIPackageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IAIPackage, IAIPackageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AlchemicalApparatus
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AlchemicalApparatus</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter> AlchemicalApparatus(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AlchemicalApparatus
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AlchemicalApparatus</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter> AlchemicalApparatus(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ammunition
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ammunition</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IAmmunition, IAmmunitionGetter> Ammunition(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IAmmunition, IAmmunitionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IAmmunition, IAmmunitionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ammunition
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ammunition</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IAmmunition, IAmmunitionGetter> Ammunition(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IAmmunition, IAmmunitionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IAmmunition, IAmmunitionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AnimatedObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AnimatedObject</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IAnimatedObject, IAnimatedObjectGetter> AnimatedObject(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IAnimatedObject, IAnimatedObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IAnimatedObject, IAnimatedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AnimatedObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AnimatedObject</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IAnimatedObject, IAnimatedObjectGetter> AnimatedObject(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IAnimatedObject, IAnimatedObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IAnimatedObject, IAnimatedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Armor
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Armor</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IArmor, IArmorGetter> Armor(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IArmor, IArmorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IArmor, IArmorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Armor
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Armor</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IArmor, IArmorGetter> Armor(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IArmor, IArmorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IArmor, IArmorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Birthsign
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Birthsign</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IBirthsign, IBirthsignGetter> Birthsign(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IBirthsign, IBirthsignGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBirthsignGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IBirthsign, IBirthsignGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Birthsign
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Birthsign</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IBirthsign, IBirthsignGetter> Birthsign(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IBirthsign, IBirthsignGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBirthsignGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IBirthsign, IBirthsignGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Book
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Book</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IBook, IBookGetter> Book(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IBook, IBookGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IBook, IBookGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Book
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Book</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IBook, IBookGetter> Book(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IBook, IBookGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IBook, IBookGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Cell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Cell</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ICell, ICellGetter> Cell(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ICell, ICellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ICell, ICellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Cell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Cell</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ICell, ICellGetter> Cell(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ICell, ICellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ICell, ICellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Class
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Class</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IClass, IClassGetter> Class(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IClass, IClassGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IClass, IClassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Class
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Class</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IClass, IClassGetter> Class(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IClass, IClassGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IClass, IClassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Climate
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Climate</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IClimate, IClimateGetter> Climate(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IClimate, IClimateGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IClimate, IClimateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Climate
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Climate</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IClimate, IClimateGetter> Climate(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IClimate, IClimateGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IClimate, IClimateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Clothing
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Clothing</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IClothing, IClothingGetter> Clothing(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IClothing, IClothingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IClothingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IClothing, IClothingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Clothing
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Clothing</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IClothing, IClothingGetter> Clothing(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IClothing, IClothingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IClothingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IClothing, IClothingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CombatStyle
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CombatStyle</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ICombatStyle, ICombatStyleGetter> CombatStyle(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ICombatStyle, ICombatStyleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ICombatStyle, ICombatStyleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CombatStyle
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CombatStyle</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ICombatStyle, ICombatStyleGetter> CombatStyle(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ICombatStyle, ICombatStyleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ICombatStyle, ICombatStyleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Container
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Container</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IContainer, IContainerGetter> Container(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IContainer, IContainerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IContainer, IContainerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Container
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Container</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IContainer, IContainerGetter> Container(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IContainer, IContainerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IContainer, IContainerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Creature
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Creature</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ICreature, ICreatureGetter> Creature(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ICreature, ICreatureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ICreature, ICreatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Creature
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Creature</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ICreature, ICreatureGetter> Creature(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ICreature, ICreatureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ICreature, ICreatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogItem</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IDialogItem, IDialogItemGetter> DialogItem(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IDialogItem, IDialogItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IDialogItem, IDialogItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogItem</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IDialogItem, IDialogItemGetter> DialogItem(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IDialogItem, IDialogItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IDialogItem, IDialogItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogTopic
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogTopic</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IDialogTopic, IDialogTopicGetter> DialogTopic(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IDialogTopic, IDialogTopicGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IDialogTopic, IDialogTopicGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogTopic
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogTopic</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IDialogTopic, IDialogTopicGetter> DialogTopic(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IDialogTopic, IDialogTopicGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IDialogTopic, IDialogTopicGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Door
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Door</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IDoor, IDoorGetter> Door(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IDoor, IDoorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IDoor, IDoorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Door
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Door</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IDoor, IDoorGetter> Door(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IDoor, IDoorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IDoor, IDoorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EffectShader
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EffectShader</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IEffectShader, IEffectShaderGetter> EffectShader(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IEffectShader, IEffectShaderGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IEffectShader, IEffectShaderGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EffectShader
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EffectShader</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IEffectShader, IEffectShaderGetter> EffectShader(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IEffectShader, IEffectShaderGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IEffectShader, IEffectShaderGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Enchantment
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Enchantment</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IEnchantment, IEnchantmentGetter> Enchantment(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IEnchantment, IEnchantmentGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEnchantmentGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IEnchantment, IEnchantmentGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Enchantment
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Enchantment</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IEnchantment, IEnchantmentGetter> Enchantment(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IEnchantment, IEnchantmentGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEnchantmentGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IEnchantment, IEnchantmentGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Eye
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Eye</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IEye, IEyeGetter> Eye(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IEye, IEyeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEyeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IEye, IEyeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Eye
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Eye</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IEye, IEyeGetter> Eye(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IEye, IEyeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEyeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IEye, IEyeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Faction
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Faction</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IFaction, IFactionGetter> Faction(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IFaction, IFactionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IFaction, IFactionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Faction
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Faction</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IFaction, IFactionGetter> Faction(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IFaction, IFactionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IFaction, IFactionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Flora
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Flora</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IFlora, IFloraGetter> Flora(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IFlora, IFloraGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IFlora, IFloraGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Flora
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Flora</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IFlora, IFloraGetter> Flora(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IFlora, IFloraGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IFlora, IFloraGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Furniture
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Furniture</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IFurniture, IFurnitureGetter> Furniture(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IFurniture, IFurnitureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IFurniture, IFurnitureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Furniture
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Furniture</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IFurniture, IFurnitureGetter> Furniture(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IFurniture, IFurnitureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IFurniture, IFurnitureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Global
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Global</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IGlobal, IGlobalGetter> Global(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IGlobal, IGlobalGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IGlobal, IGlobalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Global
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Global</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IGlobal, IGlobalGetter> Global(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IGlobal, IGlobalGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IGlobal, IGlobalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Grass
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Grass</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IGrass, IGrassGetter> Grass(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IGrass, IGrassGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IGrass, IGrassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Grass
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Grass</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IGrass, IGrassGetter> Grass(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IGrass, IGrassGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IGrass, IGrassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hair
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Hair</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IHair, IHairGetter> Hair(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IHair, IHairGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IHair, IHairGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hair
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Hair</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IHair, IHairGetter> Hair(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IHair, IHairGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IHair, IHairGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleAnimation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IdleAnimation</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IIdleAnimation, IIdleAnimationGetter> IdleAnimation(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IIdleAnimation, IIdleAnimationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IIdleAnimation, IIdleAnimationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleAnimation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IdleAnimation</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IIdleAnimation, IIdleAnimationGetter> IdleAnimation(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IIdleAnimation, IIdleAnimationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IIdleAnimation, IIdleAnimationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingredient
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ingredient</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IIngredient, IIngredientGetter> Ingredient(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IIngredient, IIngredientGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IIngredient, IIngredientGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingredient
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ingredient</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IIngredient, IIngredientGetter> Ingredient(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IIngredient, IIngredientGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IIngredient, IIngredientGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Key
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Key</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IKey, IKeyGetter> Key(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IKey, IKeyGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IKey, IKeyGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Key
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Key</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IKey, IKeyGetter> Key(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IKey, IKeyGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IKey, IKeyGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Landscape
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Landscape</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILandscape, ILandscapeGetter> Landscape(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILandscape, ILandscapeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ILandscape, ILandscapeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Landscape
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Landscape</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILandscape, ILandscapeGetter> Landscape(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILandscape, ILandscapeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ILandscape, ILandscapeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LandTexture
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LandTexture</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILandTexture, ILandTextureGetter> LandTexture(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILandTexture, ILandTextureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILandTextureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ILandTexture, ILandTextureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LandTexture
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LandTexture</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILandTexture, ILandTextureGetter> LandTexture(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILandTexture, ILandTextureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILandTextureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ILandTexture, ILandTextureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledCreature
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledCreature</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILeveledCreature, ILeveledCreatureGetter> LeveledCreature(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILeveledCreature, ILeveledCreatureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledCreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ILeveledCreature, ILeveledCreatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledCreature
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledCreature</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILeveledCreature, ILeveledCreatureGetter> LeveledCreature(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILeveledCreature, ILeveledCreatureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledCreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ILeveledCreature, ILeveledCreatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledItem</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILeveledItem, ILeveledItemGetter> LeveledItem(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILeveledItem, ILeveledItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ILeveledItem, ILeveledItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledItem</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILeveledItem, ILeveledItemGetter> LeveledItem(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILeveledItem, ILeveledItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ILeveledItem, ILeveledItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledSpell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledSpell</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILeveledSpell, ILeveledSpellGetter> LeveledSpell(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILeveledSpell, ILeveledSpellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ILeveledSpell, ILeveledSpellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledSpell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledSpell</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILeveledSpell, ILeveledSpellGetter> LeveledSpell(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILeveledSpell, ILeveledSpellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ILeveledSpell, ILeveledSpellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Light
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Light</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILight, ILightGetter> Light(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILight, ILightGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ILight, ILightGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Light
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Light</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILight, ILightGetter> Light(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILight, ILightGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ILight, ILightGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LoadScreen
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LoadScreen</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILoadScreen, ILoadScreenGetter> LoadScreen(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILoadScreen, ILoadScreenGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ILoadScreen, ILoadScreenGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LoadScreen
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LoadScreen</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILoadScreen, ILoadScreenGetter> LoadScreen(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ILoadScreen, ILoadScreenGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ILoadScreen, ILoadScreenGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MagicEffect
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MagicEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IMagicEffect, IMagicEffectGetter> MagicEffect(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IMagicEffect, IMagicEffectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IMagicEffect, IMagicEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MagicEffect
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MagicEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IMagicEffect, IMagicEffectGetter> MagicEffect(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IMagicEffect, IMagicEffectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IMagicEffect, IMagicEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Miscellaneous
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Miscellaneous</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IMiscellaneous, IMiscellaneousGetter> Miscellaneous(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IMiscellaneous, IMiscellaneousGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMiscellaneousGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IMiscellaneous, IMiscellaneousGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Miscellaneous
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Miscellaneous</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IMiscellaneous, IMiscellaneousGetter> Miscellaneous(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IMiscellaneous, IMiscellaneousGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMiscellaneousGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IMiscellaneous, IMiscellaneousGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, INpc, INpcGetter> Npc(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, INpc, INpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, INpc, INpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, INpc, INpcGetter> Npc(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, INpc, INpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, INpc, INpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to OblivionMajorRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on OblivionMajorRecord</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IOblivionMajorRecord, IOblivionMajorRecordGetter> OblivionMajorRecord(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IOblivionMajorRecord, IOblivionMajorRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IOblivionMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IOblivionMajorRecord, IOblivionMajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to OblivionMajorRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on OblivionMajorRecord</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IOblivionMajorRecord, IOblivionMajorRecordGetter> OblivionMajorRecord(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IOblivionMajorRecord, IOblivionMajorRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IOblivionMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IOblivionMajorRecord, IOblivionMajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PathGrid
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PathGrid</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPathGrid, IPathGridGetter> PathGrid(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPathGrid, IPathGridGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPathGridGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IPathGrid, IPathGridGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PathGrid
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PathGrid</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPathGrid, IPathGridGetter> PathGrid(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPathGrid, IPathGridGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPathGridGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IPathGrid, IPathGridGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedCreature
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedCreature</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPlacedCreature, IPlacedCreatureGetter> PlacedCreature(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPlacedCreature, IPlacedCreatureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedCreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IPlacedCreature, IPlacedCreatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedCreature
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedCreature</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPlacedCreature, IPlacedCreatureGetter> PlacedCreature(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPlacedCreature, IPlacedCreatureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedCreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IPlacedCreature, IPlacedCreatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedNpc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedNpc</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPlacedNpc, IPlacedNpcGetter> PlacedNpc(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPlacedNpc, IPlacedNpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IPlacedNpc, IPlacedNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedNpc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedNpc</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPlacedNpc, IPlacedNpcGetter> PlacedNpc(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPlacedNpc, IPlacedNpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IPlacedNpc, IPlacedNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedObject</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPlacedObject, IPlacedObjectGetter> PlacedObject(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPlacedObject, IPlacedObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IPlacedObject, IPlacedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedObject</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPlacedObject, IPlacedObjectGetter> PlacedObject(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPlacedObject, IPlacedObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IPlacedObject, IPlacedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Potion
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Potion</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPotion, IPotionGetter> Potion(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPotion, IPotionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPotionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IPotion, IPotionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Potion
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Potion</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPotion, IPotionGetter> Potion(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPotion, IPotionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPotionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IPotion, IPotionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Quest
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Quest</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IQuest, IQuestGetter> Quest(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IQuest, IQuestGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IQuest, IQuestGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Quest
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Quest</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IQuest, IQuestGetter> Quest(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IQuest, IQuestGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IQuest, IQuestGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IRace, IRaceGetter> Race(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IRace, IRaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IRace, IRaceGetter> Race(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IRace, IRaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Region
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Region</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IRegion, IRegionGetter> Region(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IRegion, IRegionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IRegion, IRegionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Region
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Region</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IRegion, IRegionGetter> Region(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IRegion, IRegionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IRegion, IRegionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Road
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Road</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IRoad, IRoadGetter> Road(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IRoad, IRoadGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRoadGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IRoad, IRoadGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Road
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Road</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IRoad, IRoadGetter> Road(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IRoad, IRoadGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRoadGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IRoad, IRoadGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Script
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Script</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IScript, IScriptGetter> Script(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IScript, IScriptGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IScriptGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IScript, IScriptGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Script
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Script</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IScript, IScriptGetter> Script(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IScript, IScriptGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IScriptGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IScript, IScriptGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SigilStone
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SigilStone</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISigilStone, ISigilStoneGetter> SigilStone(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISigilStone, ISigilStoneGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISigilStoneGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ISigilStone, ISigilStoneGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SigilStone
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SigilStone</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISigilStone, ISigilStoneGetter> SigilStone(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISigilStone, ISigilStoneGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISigilStoneGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ISigilStone, ISigilStoneGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SkillRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SkillRecord</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISkillRecord, ISkillRecordGetter> SkillRecord(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISkillRecord, ISkillRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISkillRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ISkillRecord, ISkillRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SkillRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SkillRecord</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISkillRecord, ISkillRecordGetter> SkillRecord(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISkillRecord, ISkillRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISkillRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ISkillRecord, ISkillRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoulGem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoulGem</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISoulGem, ISoulGemGetter> SoulGem(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISoulGem, ISoulGemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ISoulGem, ISoulGemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoulGem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoulGem</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISoulGem, ISoulGemGetter> SoulGem(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISoulGem, ISoulGemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ISoulGem, ISoulGemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Sound
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Sound</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISound, ISoundGetter> Sound(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISound, ISoundGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ISound, ISoundGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Sound
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Sound</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISound, ISoundGetter> Sound(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISound, ISoundGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ISound, ISoundGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Spell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Spell</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISpell, ISpellGetter> Spell(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISpell, ISpellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ISpell, ISpellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Spell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Spell</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISpell, ISpellGetter> Spell(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISpell, ISpellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ISpell, ISpellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SpellLeveled
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SpellLeveled</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISpellLeveled, ISpellLeveledGetter> SpellLeveled(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISpellLeveled, ISpellLeveledGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISpellLeveledGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ISpellLeveled, ISpellLeveledGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SpellLeveled
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SpellLeveled</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISpellLeveled, ISpellLeveledGetter> SpellLeveled(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISpellLeveled, ISpellLeveledGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISpellLeveledGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ISpellLeveled, ISpellLeveledGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Static
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Static</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IStatic, IStaticGetter> Static(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IStatic, IStaticGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IStatic, IStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Static
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Static</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IStatic, IStaticGetter> Static(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IStatic, IStaticGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IStatic, IStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Subspace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Subspace</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISubspace, ISubspaceGetter> Subspace(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISubspace, ISubspaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISubspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ISubspace, ISubspaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Subspace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Subspace</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISubspace, ISubspaceGetter> Subspace(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISubspace, ISubspaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISubspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ISubspace, ISubspaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Tree
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Tree</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ITree, ITreeGetter> Tree(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ITree, ITreeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ITree, ITreeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Tree
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Tree</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ITree, ITreeGetter> Tree(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ITree, ITreeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ITree, ITreeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Water
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Water</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IWater, IWaterGetter> Water(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IWater, IWaterGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IWater, IWaterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Water
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Water</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IWater, IWaterGetter> Water(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IWater, IWaterGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IWater, IWaterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weapon
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Weapon</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IWeapon, IWeaponGetter> Weapon(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IWeapon, IWeaponGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IWeapon, IWeaponGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weapon
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Weapon</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IWeapon, IWeaponGetter> Weapon(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IWeapon, IWeaponGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IWeapon, IWeaponGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weather
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Weather</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IWeather, IWeatherGetter> Weather(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IWeather, IWeatherGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IWeather, IWeatherGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weather
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Weather</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IWeather, IWeatherGetter> Weather(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IWeather, IWeatherGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IWeather, IWeatherGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Worldspace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Worldspace</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IWorldspace, IWorldspaceGetter> Worldspace(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IWorldspace, IWorldspaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IWorldspace, IWorldspaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Worldspace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Worldspace</returns>
        public static TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IWorldspace, IWorldspaceGetter> Worldspace(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IWorldspace, IWorldspaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IWorldspace, IWorldspaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

        #region Link Interfaces
        /// <summary>
        /// Scope a load order query to IItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IItem</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IItem, IItemGetter> IItem(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IItem, IItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IItem, IItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IItem</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IItem, IItemGetter> IItem(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IItem, IItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IItem, IItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to INpcSpawn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on INpcSpawn</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, INpcSpawn, INpcSpawnGetter> INpcSpawn(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, INpcSpawn, INpcSpawnGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, INpcSpawn, INpcSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to INpcSpawn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on INpcSpawn</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, INpcSpawn, INpcSpawnGetter> INpcSpawn(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, INpcSpawn, INpcSpawnGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, INpcSpawn, INpcSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to INpcRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on INpcRecord</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, INpcRecord, INpcRecordGetter> INpcRecord(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, INpcRecord, INpcRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, INpcRecord, INpcRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to INpcRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on INpcRecord</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, INpcRecord, INpcRecordGetter> INpcRecord(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, INpcRecord, INpcRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INpcRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, INpcRecord, INpcRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IOwner
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IOwner</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IOwner, IOwnerGetter> IOwner(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IOwner, IOwnerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IOwner, IOwnerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IOwner
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IOwner</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IOwner, IOwnerGetter> IOwner(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IOwner, IOwnerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IOwner, IOwnerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRelatable
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IRelatable</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IRelatable, IRelatableGetter> IRelatable(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IRelatable, IRelatableGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRelatableGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IRelatable, IRelatableGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRelatable
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IRelatable</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IRelatable, IRelatableGetter> IRelatable(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IRelatable, IRelatableGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRelatableGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IRelatable, IRelatableGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaced
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlaced</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPlaced, IPlacedGetter> IPlaced(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPlaced, IPlacedGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IPlaced, IPlacedGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaced
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlaced</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPlaced, IPlacedGetter> IPlaced(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPlaced, IPlacedGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IPlaced, IPlacedGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ISpellRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ISpellRecord</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISpellRecord, ISpellRecordGetter> ISpellRecord(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISpellRecord, ISpellRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISpellRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ISpellRecord, ISpellRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ISpellRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ISpellRecord</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISpellRecord, ISpellRecordGetter> ISpellRecord(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, ISpellRecord, ISpellRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISpellRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, ISpellRecord, ISpellRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlace</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPlace, IPlaceGetter> IPlace(this IEnumerable<IModListingGetter<IOblivionModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPlace, IPlaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IPlace, IPlaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlace</returns>
        public static TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPlace, IPlaceGetter> IPlace(this IEnumerable<IOblivionModGetter> mods)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionModGetter, IPlace, IPlaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IOblivionMod, IOblivionModGetter, IPlace, IPlaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

    }
}
