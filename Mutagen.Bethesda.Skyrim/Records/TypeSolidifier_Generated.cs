using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.Skyrim
{
    public static class TypeOptionSolidifierMixIns
    {
        #region Normal
        /// <summary>
        /// Scope a load order query to AcousticSpace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AcousticSpace</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAcousticSpace, IAcousticSpaceGetter> AcousticSpace(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAcousticSpace, IAcousticSpaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAcousticSpace, IAcousticSpaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AcousticSpace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AcousticSpace</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAcousticSpace, IAcousticSpaceGetter> AcousticSpace(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAcousticSpace, IAcousticSpaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAcousticSpace, IAcousticSpaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActionRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ActionRecord</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IActionRecord, IActionRecordGetter> ActionRecord(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IActionRecord, IActionRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IActionRecord, IActionRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActionRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ActionRecord</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IActionRecord, IActionRecordGetter> ActionRecord(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IActionRecord, IActionRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IActionRecord, IActionRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Activator
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Activator</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IActivator, IActivatorGetter> Activator(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IActivator, IActivatorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IActivator, IActivatorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Activator
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Activator</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IActivator, IActivatorGetter> Activator(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IActivator, IActivatorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IActivator, IActivatorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActorValueInformation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ActorValueInformation</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IActorValueInformation, IActorValueInformationGetter> ActorValueInformation(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IActorValueInformation, IActorValueInformationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IActorValueInformation, IActorValueInformationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActorValueInformation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ActorValueInformation</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IActorValueInformation, IActorValueInformationGetter> ActorValueInformation(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IActorValueInformation, IActorValueInformationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IActorValueInformation, IActorValueInformationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AddonNode
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AddonNode</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAddonNode, IAddonNodeGetter> AddonNode(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAddonNode, IAddonNodeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAddonNode, IAddonNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AddonNode
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AddonNode</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAddonNode, IAddonNodeGetter> AddonNode(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAddonNode, IAddonNodeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAddonNode, IAddonNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AlchemicalApparatus
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AlchemicalApparatus</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter> AlchemicalApparatus(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AlchemicalApparatus
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AlchemicalApparatus</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter> AlchemicalApparatus(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ammunition
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ammunition</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAmmunition, IAmmunitionGetter> Ammunition(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAmmunition, IAmmunitionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAmmunition, IAmmunitionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ammunition
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ammunition</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAmmunition, IAmmunitionGetter> Ammunition(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAmmunition, IAmmunitionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAmmunition, IAmmunitionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AnimatedObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AnimatedObject</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAnimatedObject, IAnimatedObjectGetter> AnimatedObject(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAnimatedObject, IAnimatedObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAnimatedObject, IAnimatedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AnimatedObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AnimatedObject</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAnimatedObject, IAnimatedObjectGetter> AnimatedObject(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAnimatedObject, IAnimatedObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAnimatedObject, IAnimatedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to APlacedTrap
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on APlacedTrap</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAPlacedTrap, IAPlacedTrapGetter> APlacedTrap(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAPlacedTrap, IAPlacedTrapGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAPlacedTrap, IAPlacedTrapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to APlacedTrap
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on APlacedTrap</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAPlacedTrap, IAPlacedTrapGetter> APlacedTrap(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAPlacedTrap, IAPlacedTrapGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAPlacedTrap, IAPlacedTrapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Armor
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Armor</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IArmor, IArmorGetter> Armor(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IArmor, IArmorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IArmor, IArmorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Armor
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Armor</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IArmor, IArmorGetter> Armor(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IArmor, IArmorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IArmor, IArmorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ArmorAddon
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ArmorAddon</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IArmorAddon, IArmorAddonGetter> ArmorAddon(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IArmorAddon, IArmorAddonGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IArmorAddon, IArmorAddonGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ArmorAddon
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ArmorAddon</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IArmorAddon, IArmorAddonGetter> ArmorAddon(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IArmorAddon, IArmorAddonGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IArmorAddon, IArmorAddonGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ArtObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ArtObject</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IArtObject, IArtObjectGetter> ArtObject(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IArtObject, IArtObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IArtObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IArtObject, IArtObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ArtObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ArtObject</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IArtObject, IArtObjectGetter> ArtObject(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IArtObject, IArtObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IArtObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IArtObject, IArtObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AssociationType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AssociationType</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAssociationType, IAssociationTypeGetter> AssociationType(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAssociationType, IAssociationTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAssociationTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAssociationType, IAssociationTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AssociationType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AssociationType</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAssociationType, IAssociationTypeGetter> AssociationType(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAssociationType, IAssociationTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAssociationTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAssociationType, IAssociationTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AStoryManagerNode
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AStoryManagerNode</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAStoryManagerNode, IAStoryManagerNodeGetter> AStoryManagerNode(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAStoryManagerNode, IAStoryManagerNodeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAStoryManagerNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAStoryManagerNode, IAStoryManagerNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AStoryManagerNode
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AStoryManagerNode</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAStoryManagerNode, IAStoryManagerNodeGetter> AStoryManagerNode(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAStoryManagerNode, IAStoryManagerNodeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAStoryManagerNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAStoryManagerNode, IAStoryManagerNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BodyPartData
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on BodyPartData</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IBodyPartData, IBodyPartDataGetter> BodyPartData(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IBodyPartData, IBodyPartDataGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IBodyPartData, IBodyPartDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BodyPartData
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on BodyPartData</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IBodyPartData, IBodyPartDataGetter> BodyPartData(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IBodyPartData, IBodyPartDataGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IBodyPartData, IBodyPartDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Book
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Book</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IBook, IBookGetter> Book(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IBook, IBookGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IBook, IBookGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Book
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Book</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IBook, IBookGetter> Book(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IBook, IBookGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IBook, IBookGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraPath
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CameraPath</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ICameraPath, ICameraPathGetter> CameraPath(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ICameraPath, ICameraPathGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICameraPath, ICameraPathGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraPath
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CameraPath</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ICameraPath, ICameraPathGetter> CameraPath(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ICameraPath, ICameraPathGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICameraPath, ICameraPathGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraShot
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CameraShot</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ICameraShot, ICameraShotGetter> CameraShot(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ICameraShot, ICameraShotGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICameraShot, ICameraShotGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraShot
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CameraShot</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ICameraShot, ICameraShotGetter> CameraShot(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ICameraShot, ICameraShotGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICameraShot, ICameraShotGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Cell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Cell</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter> Cell(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Cell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Cell</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter> Cell(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Class
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Class</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IClass, IClassGetter> Class(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IClass, IClassGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IClass, IClassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Class
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Class</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IClass, IClassGetter> Class(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IClass, IClassGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IClass, IClassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Climate
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Climate</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IClimate, IClimateGetter> Climate(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IClimate, IClimateGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IClimate, IClimateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Climate
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Climate</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IClimate, IClimateGetter> Climate(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IClimate, IClimateGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IClimate, IClimateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CollisionLayer
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CollisionLayer</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ICollisionLayer, ICollisionLayerGetter> CollisionLayer(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ICollisionLayer, ICollisionLayerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICollisionLayerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICollisionLayer, ICollisionLayerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CollisionLayer
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CollisionLayer</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ICollisionLayer, ICollisionLayerGetter> CollisionLayer(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ICollisionLayer, ICollisionLayerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICollisionLayerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICollisionLayer, ICollisionLayerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ColorRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ColorRecord</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IColorRecord, IColorRecordGetter> ColorRecord(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IColorRecord, IColorRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IColorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IColorRecord, IColorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ColorRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ColorRecord</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IColorRecord, IColorRecordGetter> ColorRecord(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IColorRecord, IColorRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IColorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IColorRecord, IColorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CombatStyle
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CombatStyle</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ICombatStyle, ICombatStyleGetter> CombatStyle(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ICombatStyle, ICombatStyleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICombatStyle, ICombatStyleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CombatStyle
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CombatStyle</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ICombatStyle, ICombatStyleGetter> CombatStyle(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ICombatStyle, ICombatStyleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICombatStyle, ICombatStyleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ConstructibleObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ConstructibleObject</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IConstructibleObject, IConstructibleObjectGetter> ConstructibleObject(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IConstructibleObject, IConstructibleObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IConstructibleObject, IConstructibleObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ConstructibleObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ConstructibleObject</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IConstructibleObject, IConstructibleObjectGetter> ConstructibleObject(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IConstructibleObject, IConstructibleObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IConstructibleObject, IConstructibleObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Container
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Container</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IContainer, IContainerGetter> Container(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IContainer, IContainerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IContainer, IContainerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Container
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Container</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IContainer, IContainerGetter> Container(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IContainer, IContainerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IContainer, IContainerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Debris
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Debris</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDebris, IDebrisGetter> Debris(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDebris, IDebrisGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDebrisGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDebris, IDebrisGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Debris
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Debris</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDebris, IDebrisGetter> Debris(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDebris, IDebrisGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDebrisGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDebris, IDebrisGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DefaultObjectManager
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DefaultObjectManager</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter> DefaultObjectManager(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DefaultObjectManager
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DefaultObjectManager</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter> DefaultObjectManager(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogBranch
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogBranch</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDialogBranch, IDialogBranchGetter> DialogBranch(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDialogBranch, IDialogBranchGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogBranchGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogBranch, IDialogBranchGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogBranch
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogBranch</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDialogBranch, IDialogBranchGetter> DialogBranch(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDialogBranch, IDialogBranchGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogBranchGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogBranch, IDialogBranchGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogResponses
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogResponses</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDialogResponses, IDialogResponsesGetter> DialogResponses(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDialogResponses, IDialogResponsesGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogResponses, IDialogResponsesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogResponses
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogResponses</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDialogResponses, IDialogResponsesGetter> DialogResponses(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDialogResponses, IDialogResponsesGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogResponses, IDialogResponsesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogTopic
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogTopic</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDialogTopic, IDialogTopicGetter> DialogTopic(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDialogTopic, IDialogTopicGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogTopic, IDialogTopicGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogTopic
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogTopic</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDialogTopic, IDialogTopicGetter> DialogTopic(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDialogTopic, IDialogTopicGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogTopic, IDialogTopicGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogView
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogView</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDialogView, IDialogViewGetter> DialogView(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDialogView, IDialogViewGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogViewGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogView, IDialogViewGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogView
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogView</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDialogView, IDialogViewGetter> DialogView(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDialogView, IDialogViewGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogViewGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogView, IDialogViewGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Door
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Door</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDoor, IDoorGetter> Door(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDoor, IDoorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDoor, IDoorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Door
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Door</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDoor, IDoorGetter> Door(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDoor, IDoorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDoor, IDoorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DualCastData
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DualCastData</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDualCastData, IDualCastDataGetter> DualCastData(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDualCastData, IDualCastDataGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDualCastDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDualCastData, IDualCastDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DualCastData
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DualCastData</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDualCastData, IDualCastDataGetter> DualCastData(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDualCastData, IDualCastDataGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDualCastDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDualCastData, IDualCastDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EffectShader
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EffectShader</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEffectShader, IEffectShaderGetter> EffectShader(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEffectShader, IEffectShaderGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEffectShader, IEffectShaderGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EffectShader
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EffectShader</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEffectShader, IEffectShaderGetter> EffectShader(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEffectShader, IEffectShaderGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEffectShader, IEffectShaderGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EncounterZone
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EncounterZone</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEncounterZone, IEncounterZoneGetter> EncounterZone(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEncounterZone, IEncounterZoneGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEncounterZoneGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEncounterZone, IEncounterZoneGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EncounterZone
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EncounterZone</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEncounterZone, IEncounterZoneGetter> EncounterZone(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEncounterZone, IEncounterZoneGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEncounterZoneGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEncounterZone, IEncounterZoneGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EquipType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EquipType</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEquipType, IEquipTypeGetter> EquipType(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEquipType, IEquipTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEquipTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEquipType, IEquipTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EquipType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EquipType</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEquipType, IEquipTypeGetter> EquipType(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEquipType, IEquipTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEquipTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEquipType, IEquipTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Explosion
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Explosion</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IExplosion, IExplosionGetter> Explosion(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IExplosion, IExplosionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IExplosionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IExplosion, IExplosionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Explosion
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Explosion</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IExplosion, IExplosionGetter> Explosion(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IExplosion, IExplosionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IExplosionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IExplosion, IExplosionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Eyes
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Eyes</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEyes, IEyesGetter> Eyes(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEyes, IEyesGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEyesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEyes, IEyesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Eyes
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Eyes</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEyes, IEyesGetter> Eyes(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEyes, IEyesGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEyesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEyes, IEyesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Faction
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Faction</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFaction, IFactionGetter> Faction(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFaction, IFactionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFaction, IFactionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Faction
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Faction</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFaction, IFactionGetter> Faction(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFaction, IFactionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFaction, IFactionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Flora
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Flora</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFlora, IFloraGetter> Flora(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFlora, IFloraGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFlora, IFloraGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Flora
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Flora</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFlora, IFloraGetter> Flora(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFlora, IFloraGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFlora, IFloraGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Footstep
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Footstep</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFootstep, IFootstepGetter> Footstep(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFootstep, IFootstepGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFootstepGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFootstep, IFootstepGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Footstep
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Footstep</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFootstep, IFootstepGetter> Footstep(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFootstep, IFootstepGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFootstepGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFootstep, IFootstepGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FootstepSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on FootstepSet</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFootstepSet, IFootstepSetGetter> FootstepSet(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFootstepSet, IFootstepSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFootstepSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFootstepSet, IFootstepSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FootstepSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on FootstepSet</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFootstepSet, IFootstepSetGetter> FootstepSet(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFootstepSet, IFootstepSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFootstepSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFootstepSet, IFootstepSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FormList
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on FormList</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFormList, IFormListGetter> FormList(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFormList, IFormListGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFormListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFormList, IFormListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FormList
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on FormList</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFormList, IFormListGetter> FormList(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFormList, IFormListGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFormListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFormList, IFormListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Furniture
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Furniture</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFurniture, IFurnitureGetter> Furniture(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFurniture, IFurnitureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFurniture, IFurnitureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Furniture
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Furniture</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFurniture, IFurnitureGetter> Furniture(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IFurniture, IFurnitureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFurniture, IFurnitureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Global
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Global</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IGlobal, IGlobalGetter> Global(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IGlobal, IGlobalGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGlobal, IGlobalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Global
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Global</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IGlobal, IGlobalGetter> Global(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IGlobal, IGlobalGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGlobal, IGlobalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Grass
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Grass</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IGrass, IGrassGetter> Grass(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IGrass, IGrassGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGrass, IGrassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Grass
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Grass</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IGrass, IGrassGetter> Grass(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IGrass, IGrassGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGrass, IGrassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hair
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Hair</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IHair, IHairGetter> Hair(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IHair, IHairGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHair, IHairGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hair
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Hair</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IHair, IHairGetter> Hair(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IHair, IHairGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHair, IHairGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hazard
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Hazard</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IHazard, IHazardGetter> Hazard(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IHazard, IHazardGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHazardGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHazard, IHazardGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hazard
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Hazard</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IHazard, IHazardGetter> Hazard(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IHazard, IHazardGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHazardGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHazard, IHazardGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to HeadPart
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on HeadPart</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IHeadPart, IHeadPartGetter> HeadPart(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IHeadPart, IHeadPartGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHeadPart, IHeadPartGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to HeadPart
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on HeadPart</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IHeadPart, IHeadPartGetter> HeadPart(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IHeadPart, IHeadPartGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHeadPart, IHeadPartGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleAnimation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IdleAnimation</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IIdleAnimation, IIdleAnimationGetter> IdleAnimation(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IIdleAnimation, IIdleAnimationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIdleAnimation, IIdleAnimationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleAnimation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IdleAnimation</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IIdleAnimation, IIdleAnimationGetter> IdleAnimation(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IIdleAnimation, IIdleAnimationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIdleAnimation, IIdleAnimationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleMarker
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IdleMarker</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IIdleMarker, IIdleMarkerGetter> IdleMarker(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IIdleMarker, IIdleMarkerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIdleMarker, IIdleMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleMarker
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IdleMarker</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IIdleMarker, IIdleMarkerGetter> IdleMarker(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IIdleMarker, IIdleMarkerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIdleMarker, IIdleMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ImageSpace</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IImageSpace, IImageSpaceGetter> ImageSpace(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IImageSpace, IImageSpaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImageSpace, IImageSpaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ImageSpace</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IImageSpace, IImageSpaceGetter> ImageSpace(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IImageSpace, IImageSpaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImageSpace, IImageSpaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpaceAdapter
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ImageSpaceAdapter</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter> ImageSpaceAdapter(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpaceAdapter
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ImageSpaceAdapter</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter> ImageSpaceAdapter(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Impact
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Impact</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IImpact, IImpactGetter> Impact(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IImpact, IImpactGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImpactGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImpact, IImpactGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Impact
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Impact</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IImpact, IImpactGetter> Impact(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IImpact, IImpactGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImpactGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImpact, IImpactGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImpactDataSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ImpactDataSet</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IImpactDataSet, IImpactDataSetGetter> ImpactDataSet(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IImpactDataSet, IImpactDataSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImpactDataSet, IImpactDataSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImpactDataSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ImpactDataSet</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IImpactDataSet, IImpactDataSetGetter> ImpactDataSet(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IImpactDataSet, IImpactDataSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImpactDataSet, IImpactDataSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingestible
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ingestible</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IIngestible, IIngestibleGetter> Ingestible(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IIngestible, IIngestibleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIngestible, IIngestibleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingestible
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ingestible</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IIngestible, IIngestibleGetter> Ingestible(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IIngestible, IIngestibleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIngestible, IIngestibleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingredient
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ingredient</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IIngredient, IIngredientGetter> Ingredient(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IIngredient, IIngredientGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIngredient, IIngredientGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingredient
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ingredient</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IIngredient, IIngredientGetter> Ingredient(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IIngredient, IIngredientGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIngredient, IIngredientGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Key
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Key</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IKey, IKeyGetter> Key(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IKey, IKeyGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IKey, IKeyGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Key
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Key</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IKey, IKeyGetter> Key(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IKey, IKeyGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IKey, IKeyGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Keyword
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Keyword</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IKeyword, IKeywordGetter> Keyword(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IKeyword, IKeywordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeywordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IKeyword, IKeywordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Keyword
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Keyword</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IKeyword, IKeywordGetter> Keyword(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IKeyword, IKeywordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeywordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IKeyword, IKeywordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Landscape
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Landscape</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILandscape, ILandscapeGetter> Landscape(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILandscape, ILandscapeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILandscape, ILandscapeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Landscape
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Landscape</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILandscape, ILandscapeGetter> Landscape(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILandscape, ILandscapeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILandscape, ILandscapeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LandscapeTexture
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LandscapeTexture</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILandscapeTexture, ILandscapeTextureGetter> LandscapeTexture(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILandscapeTexture, ILandscapeTextureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILandscapeTexture, ILandscapeTextureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LandscapeTexture
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LandscapeTexture</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILandscapeTexture, ILandscapeTextureGetter> LandscapeTexture(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILandscapeTexture, ILandscapeTextureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILandscapeTexture, ILandscapeTextureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LensFlare
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LensFlare</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILensFlare, ILensFlareGetter> LensFlare(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILensFlare, ILensFlareGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILensFlareGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILensFlare, ILensFlareGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LensFlare
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LensFlare</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILensFlare, ILensFlareGetter> LensFlare(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILensFlare, ILensFlareGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILensFlareGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILensFlare, ILensFlareGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledItem</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILeveledItem, ILeveledItemGetter> LeveledItem(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILeveledItem, ILeveledItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILeveledItem, ILeveledItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledItem</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILeveledItem, ILeveledItemGetter> LeveledItem(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILeveledItem, ILeveledItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILeveledItem, ILeveledItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledNpc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledNpc</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILeveledNpc, ILeveledNpcGetter> LeveledNpc(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILeveledNpc, ILeveledNpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILeveledNpc, ILeveledNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledNpc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledNpc</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILeveledNpc, ILeveledNpcGetter> LeveledNpc(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILeveledNpc, ILeveledNpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILeveledNpc, ILeveledNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledSpell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledSpell</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILeveledSpell, ILeveledSpellGetter> LeveledSpell(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILeveledSpell, ILeveledSpellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILeveledSpell, ILeveledSpellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledSpell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledSpell</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILeveledSpell, ILeveledSpellGetter> LeveledSpell(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILeveledSpell, ILeveledSpellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILeveledSpell, ILeveledSpellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Light
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Light</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILight, ILightGetter> Light(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILight, ILightGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILight, ILightGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Light
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Light</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILight, ILightGetter> Light(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILight, ILightGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILight, ILightGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LightingTemplate
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LightingTemplate</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILightingTemplate, ILightingTemplateGetter> LightingTemplate(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILightingTemplate, ILightingTemplateGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILightingTemplate, ILightingTemplateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LightingTemplate
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LightingTemplate</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILightingTemplate, ILightingTemplateGetter> LightingTemplate(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILightingTemplate, ILightingTemplateGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILightingTemplate, ILightingTemplateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LoadScreen
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LoadScreen</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILoadScreen, ILoadScreenGetter> LoadScreen(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILoadScreen, ILoadScreenGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILoadScreen, ILoadScreenGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LoadScreen
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LoadScreen</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILoadScreen, ILoadScreenGetter> LoadScreen(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILoadScreen, ILoadScreenGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILoadScreen, ILoadScreenGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Location
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Location</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILocation, ILocationGetter> Location(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILocation, ILocationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILocationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocation, ILocationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Location
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Location</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILocation, ILocationGetter> Location(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILocation, ILocationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILocationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocation, ILocationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LocationReferenceType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LocationReferenceType</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILocationReferenceType, ILocationReferenceTypeGetter> LocationReferenceType(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LocationReferenceType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LocationReferenceType</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILocationReferenceType, ILocationReferenceTypeGetter> LocationReferenceType(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MagicEffect
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MagicEffect</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMagicEffect, IMagicEffectGetter> MagicEffect(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMagicEffect, IMagicEffectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMagicEffect, IMagicEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MagicEffect
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MagicEffect</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMagicEffect, IMagicEffectGetter> MagicEffect(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMagicEffect, IMagicEffectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMagicEffect, IMagicEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MaterialObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MaterialObject</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMaterialObject, IMaterialObjectGetter> MaterialObject(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMaterialObject, IMaterialObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMaterialObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMaterialObject, IMaterialObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MaterialObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MaterialObject</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMaterialObject, IMaterialObjectGetter> MaterialObject(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMaterialObject, IMaterialObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMaterialObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMaterialObject, IMaterialObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MaterialType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MaterialType</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMaterialType, IMaterialTypeGetter> MaterialType(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMaterialType, IMaterialTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMaterialTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMaterialType, IMaterialTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MaterialType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MaterialType</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMaterialType, IMaterialTypeGetter> MaterialType(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMaterialType, IMaterialTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMaterialTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMaterialType, IMaterialTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Message
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Message</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMessage, IMessageGetter> Message(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMessage, IMessageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMessageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMessage, IMessageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Message
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Message</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMessage, IMessageGetter> Message(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMessage, IMessageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMessageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMessage, IMessageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MiscItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MiscItem</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMiscItem, IMiscItemGetter> MiscItem(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMiscItem, IMiscItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMiscItem, IMiscItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MiscItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MiscItem</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMiscItem, IMiscItemGetter> MiscItem(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMiscItem, IMiscItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMiscItem, IMiscItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MoveableStatic
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MoveableStatic</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMoveableStatic, IMoveableStaticGetter> MoveableStatic(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMoveableStatic, IMoveableStaticGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMoveableStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMoveableStatic, IMoveableStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MoveableStatic
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MoveableStatic</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMoveableStatic, IMoveableStaticGetter> MoveableStatic(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMoveableStatic, IMoveableStaticGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMoveableStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMoveableStatic, IMoveableStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MovementType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MovementType</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMovementType, IMovementTypeGetter> MovementType(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMovementType, IMovementTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMovementTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMovementType, IMovementTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MovementType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MovementType</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMovementType, IMovementTypeGetter> MovementType(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMovementType, IMovementTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMovementTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMovementType, IMovementTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MusicTrack
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MusicTrack</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMusicTrack, IMusicTrackGetter> MusicTrack(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMusicTrack, IMusicTrackGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMusicTrackGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMusicTrack, IMusicTrackGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MusicTrack
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MusicTrack</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMusicTrack, IMusicTrackGetter> MusicTrack(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMusicTrack, IMusicTrackGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMusicTrackGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMusicTrack, IMusicTrackGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MusicType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MusicType</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMusicType, IMusicTypeGetter> MusicType(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMusicType, IMusicTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMusicType, IMusicTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MusicType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MusicType</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMusicType, IMusicTypeGetter> MusicType(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMusicType, IMusicTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMusicType, IMusicTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMesh
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on NavigationMesh</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, INavigationMesh, INavigationMeshGetter> NavigationMesh(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, INavigationMesh, INavigationMeshGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INavigationMesh, INavigationMeshGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMesh
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on NavigationMesh</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, INavigationMesh, INavigationMeshGetter> NavigationMesh(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, INavigationMesh, INavigationMeshGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INavigationMesh, INavigationMeshGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMeshInfoMap
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on NavigationMeshInfoMap</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter> NavigationMeshInfoMap(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMeshInfoMap
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on NavigationMeshInfoMap</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter> NavigationMeshInfoMap(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter> Npc(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter> Npc(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ObjectEffect
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ObjectEffect</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IObjectEffect, IObjectEffectGetter> ObjectEffect(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IObjectEffect, IObjectEffectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IObjectEffect, IObjectEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ObjectEffect
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ObjectEffect</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IObjectEffect, IObjectEffectGetter> ObjectEffect(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IObjectEffect, IObjectEffectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IObjectEffect, IObjectEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Outfit
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Outfit</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IOutfit, IOutfitGetter> Outfit(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IOutfit, IOutfitGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IOutfitGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IOutfit, IOutfitGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Outfit
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Outfit</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IOutfit, IOutfitGetter> Outfit(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IOutfit, IOutfitGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IOutfitGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IOutfit, IOutfitGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Package
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Package</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPackage, IPackageGetter> Package(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPackage, IPackageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPackageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPackage, IPackageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Package
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Package</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPackage, IPackageGetter> Package(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPackage, IPackageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPackageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPackage, IPackageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Perk
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Perk</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPerk, IPerkGetter> Perk(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPerk, IPerkGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPerkGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPerk, IPerkGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Perk
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Perk</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPerk, IPerkGetter> Perk(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPerk, IPerkGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPerkGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPerk, IPerkGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedNpc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedNpc</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter> PlacedNpc(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedNpc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedNpc</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter> PlacedNpc(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedObject</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlacedObject, IPlacedObjectGetter> PlacedObject(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlacedObject, IPlacedObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedObject, IPlacedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedObject</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlacedObject, IPlacedObjectGetter> PlacedObject(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlacedObject, IPlacedObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedObject, IPlacedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Projectile
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Projectile</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IProjectile, IProjectileGetter> Projectile(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IProjectile, IProjectileGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IProjectileGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IProjectile, IProjectileGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Projectile
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Projectile</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IProjectile, IProjectileGetter> Projectile(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IProjectile, IProjectileGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IProjectileGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IProjectile, IProjectileGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Quest
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Quest</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IQuest, IQuestGetter> Quest(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IQuest, IQuestGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IQuest, IQuestGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Quest
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Quest</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IQuest, IQuestGetter> Quest(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IQuest, IQuestGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IQuest, IQuestGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IRace, IRaceGetter> Race(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRace, IRaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IRace, IRaceGetter> Race(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRace, IRaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Region
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Region</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IRegion, IRegionGetter> Region(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IRegion, IRegionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRegion, IRegionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Region
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Region</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IRegion, IRegionGetter> Region(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IRegion, IRegionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRegion, IRegionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Relationship
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Relationship</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IRelationship, IRelationshipGetter> Relationship(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IRelationship, IRelationshipGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRelationshipGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRelationship, IRelationshipGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Relationship
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Relationship</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IRelationship, IRelationshipGetter> Relationship(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IRelationship, IRelationshipGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRelationshipGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRelationship, IRelationshipGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ReverbParameters
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ReverbParameters</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IReverbParameters, IReverbParametersGetter> ReverbParameters(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IReverbParameters, IReverbParametersGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IReverbParametersGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IReverbParameters, IReverbParametersGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ReverbParameters
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ReverbParameters</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IReverbParameters, IReverbParametersGetter> ReverbParameters(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IReverbParameters, IReverbParametersGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IReverbParametersGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IReverbParameters, IReverbParametersGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Scene
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Scene</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IScene, ISceneGetter> Scene(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IScene, ISceneGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISceneGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IScene, ISceneGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Scene
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Scene</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IScene, ISceneGetter> Scene(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IScene, ISceneGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISceneGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IScene, ISceneGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Scroll
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Scroll</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IScroll, IScrollGetter> Scroll(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IScroll, IScrollGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IScrollGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IScroll, IScrollGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Scroll
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Scroll</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IScroll, IScrollGetter> Scroll(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IScroll, IScrollGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IScrollGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IScroll, IScrollGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ShaderParticleGeometry
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ShaderParticleGeometry</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter> ShaderParticleGeometry(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IShaderParticleGeometryGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ShaderParticleGeometry
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ShaderParticleGeometry</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter> ShaderParticleGeometry(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IShaderParticleGeometryGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Shout
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Shout</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IShout, IShoutGetter> Shout(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IShout, IShoutGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IShoutGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IShout, IShoutGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Shout
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Shout</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IShout, IShoutGetter> Shout(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IShout, IShoutGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IShoutGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IShout, IShoutGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SkyrimMajorRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SkyrimMajorRecord</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> SkyrimMajorRecord(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISkyrimMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SkyrimMajorRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SkyrimMajorRecord</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> SkyrimMajorRecord(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISkyrimMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoulGem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoulGem</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISoulGem, ISoulGemGetter> SoulGem(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISoulGem, ISoulGemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoulGem, ISoulGemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoulGem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoulGem</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISoulGem, ISoulGemGetter> SoulGem(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISoulGem, ISoulGemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoulGem, ISoulGemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundCategory
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoundCategory</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISoundCategory, ISoundCategoryGetter> SoundCategory(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISoundCategory, ISoundCategoryGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundCategoryGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundCategory, ISoundCategoryGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundCategory
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoundCategory</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISoundCategory, ISoundCategoryGetter> SoundCategory(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISoundCategory, ISoundCategoryGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundCategoryGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundCategory, ISoundCategoryGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundDescriptor
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoundDescriptor</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISoundDescriptor, ISoundDescriptorGetter> SoundDescriptor(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISoundDescriptor, ISoundDescriptorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundDescriptorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundDescriptor, ISoundDescriptorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundDescriptor
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoundDescriptor</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISoundDescriptor, ISoundDescriptorGetter> SoundDescriptor(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISoundDescriptor, ISoundDescriptorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundDescriptorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundDescriptor, ISoundDescriptorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundMarker
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoundMarker</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISoundMarker, ISoundMarkerGetter> SoundMarker(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISoundMarker, ISoundMarkerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundMarker, ISoundMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundMarker
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoundMarker</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISoundMarker, ISoundMarkerGetter> SoundMarker(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISoundMarker, ISoundMarkerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundMarker, ISoundMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundOutputModel
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoundOutputModel</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISoundOutputModel, ISoundOutputModelGetter> SoundOutputModel(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISoundOutputModel, ISoundOutputModelGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundOutputModelGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundOutputModel, ISoundOutputModelGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundOutputModel
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoundOutputModel</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISoundOutputModel, ISoundOutputModelGetter> SoundOutputModel(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISoundOutputModel, ISoundOutputModelGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundOutputModelGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundOutputModel, ISoundOutputModelGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Spell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Spell</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISpell, ISpellGetter> Spell(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISpell, ISpellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISpell, ISpellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Spell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Spell</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISpell, ISpellGetter> Spell(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISpell, ISpellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISpell, ISpellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Static
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Static</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IStatic, IStaticGetter> Static(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IStatic, IStaticGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IStatic, IStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Static
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Static</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IStatic, IStaticGetter> Static(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IStatic, IStaticGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IStatic, IStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TalkingActivator
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on TalkingActivator</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ITalkingActivator, ITalkingActivatorGetter> TalkingActivator(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ITalkingActivator, ITalkingActivatorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITalkingActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ITalkingActivator, ITalkingActivatorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TalkingActivator
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on TalkingActivator</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ITalkingActivator, ITalkingActivatorGetter> TalkingActivator(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ITalkingActivator, ITalkingActivatorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITalkingActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ITalkingActivator, ITalkingActivatorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TextureSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on TextureSet</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ITextureSet, ITextureSetGetter> TextureSet(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ITextureSet, ITextureSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ITextureSet, ITextureSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TextureSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on TextureSet</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ITextureSet, ITextureSetGetter> TextureSet(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ITextureSet, ITextureSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ITextureSet, ITextureSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Tree
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Tree</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ITree, ITreeGetter> Tree(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ITree, ITreeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ITree, ITreeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Tree
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Tree</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ITree, ITreeGetter> Tree(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ITree, ITreeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ITree, ITreeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VisualEffect
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on VisualEffect</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IVisualEffect, IVisualEffectGetter> VisualEffect(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IVisualEffect, IVisualEffectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IVisualEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IVisualEffect, IVisualEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VisualEffect
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on VisualEffect</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IVisualEffect, IVisualEffectGetter> VisualEffect(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IVisualEffect, IVisualEffectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IVisualEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IVisualEffect, IVisualEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VoiceType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on VoiceType</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IVoiceType, IVoiceTypeGetter> VoiceType(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IVoiceType, IVoiceTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IVoiceType, IVoiceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VoiceType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on VoiceType</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IVoiceType, IVoiceTypeGetter> VoiceType(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IVoiceType, IVoiceTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IVoiceType, IVoiceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VolumetricLighting
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on VolumetricLighting</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IVolumetricLighting, IVolumetricLightingGetter> VolumetricLighting(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IVolumetricLighting, IVolumetricLightingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IVolumetricLightingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IVolumetricLighting, IVolumetricLightingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VolumetricLighting
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on VolumetricLighting</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IVolumetricLighting, IVolumetricLightingGetter> VolumetricLighting(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IVolumetricLighting, IVolumetricLightingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IVolumetricLightingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IVolumetricLighting, IVolumetricLightingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Water
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Water</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWater, IWaterGetter> Water(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWater, IWaterGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWater, IWaterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Water
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Water</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWater, IWaterGetter> Water(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWater, IWaterGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWater, IWaterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weapon
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Weapon</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWeapon, IWeaponGetter> Weapon(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWeapon, IWeaponGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWeapon, IWeaponGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weapon
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Weapon</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWeapon, IWeaponGetter> Weapon(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWeapon, IWeaponGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWeapon, IWeaponGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weather
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Weather</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWeather, IWeatherGetter> Weather(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWeather, IWeatherGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWeather, IWeatherGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weather
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Weather</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWeather, IWeatherGetter> Weather(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWeather, IWeatherGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWeather, IWeatherGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to WordOfPower
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on WordOfPower</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWordOfPower, IWordOfPowerGetter> WordOfPower(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWordOfPower, IWordOfPowerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWordOfPowerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWordOfPower, IWordOfPowerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to WordOfPower
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on WordOfPower</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWordOfPower, IWordOfPowerGetter> WordOfPower(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWordOfPower, IWordOfPowerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWordOfPowerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWordOfPower, IWordOfPowerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Worldspace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Worldspace</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter> Worldspace(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Worldspace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Worldspace</returns>
        public static TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter> Worldspace(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

        #region Link Interfaces
        /// <summary>
        /// Scope a load order query to IPlaceableObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlaceableObject</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlaceableObject, IPlaceableObjectGetter> IPlaceableObject(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlaceableObject, IPlaceableObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlaceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlaceableObject, IPlaceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaceableObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlaceableObject</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlaceableObject, IPlaceableObjectGetter> IPlaceableObject(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlaceableObject, IPlaceableObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlaceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlaceableObject, IPlaceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IReferenceableObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IReferenceableObject</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IReferenceableObject, IReferenceableObjectGetter> IReferenceableObject(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IReferenceableObject, IReferenceableObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IReferenceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IReferenceableObject, IReferenceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IReferenceableObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IReferenceableObject</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IReferenceableObject, IReferenceableObjectGetter> IReferenceableObject(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IReferenceableObject, IReferenceableObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IReferenceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IReferenceableObject, IReferenceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IExplodeSpawn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IExplodeSpawn</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IExplodeSpawn, IExplodeSpawnGetter> IExplodeSpawn(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IExplodeSpawn, IExplodeSpawnGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IExplodeSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IExplodeSpawn, IExplodeSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IExplodeSpawn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IExplodeSpawn</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IExplodeSpawn, IExplodeSpawnGetter> IExplodeSpawn(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IExplodeSpawn, IExplodeSpawnGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IExplodeSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IExplodeSpawn, IExplodeSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IIdleRelation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IIdleRelation</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IIdleRelation, IIdleRelationGetter> IIdleRelation(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IIdleRelation, IIdleRelationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIdleRelation, IIdleRelationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IIdleRelation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IIdleRelation</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IIdleRelation, IIdleRelationGetter> IIdleRelation(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IIdleRelation, IIdleRelationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIdleRelation, IIdleRelationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IObjectId
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IObjectId</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IObjectId, IObjectIdGetter> IObjectId(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IObjectId, IObjectIdGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IObjectIdGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IObjectId, IObjectIdGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IObjectId
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IObjectId</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IObjectId, IObjectIdGetter> IObjectId(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IObjectId, IObjectIdGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IObjectIdGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IObjectId, IObjectIdGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IItem</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IItem, IItemGetter> IItem(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IItem, IItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IItem, IItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IItem</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IItem, IItemGetter> IItem(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IItem, IItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IItem, IItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IItemOrList
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IItemOrList</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IItemOrList, IItemOrListGetter> IItemOrList(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IItemOrList, IItemOrListGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IItemOrListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IItemOrList, IItemOrListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IItemOrList
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IItemOrList</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IItemOrList, IItemOrListGetter> IItemOrList(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IItemOrList, IItemOrListGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IItemOrListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IItemOrList, IItemOrListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IConstructible
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IConstructible</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IConstructible, IConstructibleGetter> IConstructible(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IConstructible, IConstructibleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IConstructibleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IConstructible, IConstructibleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IConstructible
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IConstructible</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IConstructible, IConstructibleGetter> IConstructible(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IConstructible, IConstructibleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IConstructibleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IConstructible, IConstructibleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IOutfitTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IOutfitTarget</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IOutfitTarget, IOutfitTargetGetter> IOutfitTarget(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IOutfitTarget, IOutfitTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IOutfitTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IOutfitTarget, IOutfitTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IOutfitTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IOutfitTarget</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IOutfitTarget, IOutfitTargetGetter> IOutfitTarget(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IOutfitTarget, IOutfitTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IOutfitTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IOutfitTarget, IOutfitTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IBindableEquipment
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IBindableEquipment</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IBindableEquipment, IBindableEquipmentGetter> IBindableEquipment(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IBindableEquipment, IBindableEquipmentGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBindableEquipmentGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IBindableEquipment, IBindableEquipmentGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IBindableEquipment
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IBindableEquipment</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IBindableEquipment, IBindableEquipmentGetter> IBindableEquipment(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IBindableEquipment, IBindableEquipmentGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBindableEquipmentGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IBindableEquipment, IBindableEquipmentGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IComplexLocation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IComplexLocation</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IComplexLocation, IComplexLocationGetter> IComplexLocation(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IComplexLocation, IComplexLocationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IComplexLocationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IComplexLocation, IComplexLocationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IComplexLocation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IComplexLocation</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IComplexLocation, IComplexLocationGetter> IComplexLocation(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IComplexLocation, IComplexLocationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IComplexLocationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IComplexLocation, IComplexLocationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IDialog
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IDialog</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDialog, IDialogGetter> IDialog(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDialog, IDialogGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialog, IDialogGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IDialog
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IDialog</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDialog, IDialogGetter> IDialog(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IDialog, IDialogGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialog, IDialogGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IOwner
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IOwner</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IOwner, IOwnerGetter> IOwner(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IOwner, IOwnerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IOwner, IOwnerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IOwner
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IOwner</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IOwner, IOwnerGetter> IOwner(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IOwner, IOwnerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IOwner, IOwnerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRelatable
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IRelatable</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IRelatable, IRelatableGetter> IRelatable(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IRelatable, IRelatableGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRelatableGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRelatable, IRelatableGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRelatable
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IRelatable</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IRelatable, IRelatableGetter> IRelatable(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IRelatable, IRelatableGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRelatableGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRelatable, IRelatableGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRegionTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IRegionTarget</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IRegionTarget, IRegionTargetGetter> IRegionTarget(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IRegionTarget, IRegionTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRegionTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRegionTarget, IRegionTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRegionTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IRegionTarget</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IRegionTarget, IRegionTargetGetter> IRegionTarget(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IRegionTarget, IRegionTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRegionTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRegionTarget, IRegionTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IAliasVoiceType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IAliasVoiceType</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAliasVoiceType, IAliasVoiceTypeGetter> IAliasVoiceType(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAliasVoiceType, IAliasVoiceTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAliasVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAliasVoiceType, IAliasVoiceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IAliasVoiceType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IAliasVoiceType</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAliasVoiceType, IAliasVoiceTypeGetter> IAliasVoiceType(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IAliasVoiceType, IAliasVoiceTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAliasVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAliasVoiceType, IAliasVoiceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILockList
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ILockList</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILockList, ILockListGetter> ILockList(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILockList, ILockListGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILockListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILockList, ILockListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILockList
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ILockList</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILockList, ILockListGetter> ILockList(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILockList, ILockListGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILockListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILockList, ILockListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IWorldspaceOrList
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IWorldspaceOrList</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWorldspaceOrList, IWorldspaceOrListGetter> IWorldspaceOrList(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWorldspaceOrList, IWorldspaceOrListGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWorldspaceOrListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWorldspaceOrList, IWorldspaceOrListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IWorldspaceOrList
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IWorldspaceOrList</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWorldspaceOrList, IWorldspaceOrListGetter> IWorldspaceOrList(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWorldspaceOrList, IWorldspaceOrListGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWorldspaceOrListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWorldspaceOrList, IWorldspaceOrListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IVoiceTypeOrList
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IVoiceTypeOrList</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IVoiceTypeOrList, IVoiceTypeOrListGetter> IVoiceTypeOrList(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IVoiceTypeOrList, IVoiceTypeOrListGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IVoiceTypeOrListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IVoiceTypeOrList, IVoiceTypeOrListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IVoiceTypeOrList
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IVoiceTypeOrList</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IVoiceTypeOrList, IVoiceTypeOrListGetter> IVoiceTypeOrList(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IVoiceTypeOrList, IVoiceTypeOrListGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IVoiceTypeOrListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IVoiceTypeOrList, IVoiceTypeOrListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to INpcOrList
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on INpcOrList</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, INpcOrList, INpcOrListGetter> INpcOrList(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, INpcOrList, INpcOrListGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcOrListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INpcOrList, INpcOrListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to INpcOrList
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on INpcOrList</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, INpcOrList, INpcOrListGetter> INpcOrList(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, INpcOrList, INpcOrListGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INpcOrListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INpcOrList, INpcOrListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IWeaponOrList
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IWeaponOrList</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWeaponOrList, IWeaponOrListGetter> IWeaponOrList(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWeaponOrList, IWeaponOrListGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWeaponOrListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWeaponOrList, IWeaponOrListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IWeaponOrList
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IWeaponOrList</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWeaponOrList, IWeaponOrListGetter> IWeaponOrList(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IWeaponOrList, IWeaponOrListGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWeaponOrListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWeaponOrList, IWeaponOrListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ISpellOrList
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ISpellOrList</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISpellOrList, ISpellOrListGetter> ISpellOrList(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISpellOrList, ISpellOrListGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISpellOrListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISpellOrList, ISpellOrListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ISpellOrList
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ISpellOrList</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISpellOrList, ISpellOrListGetter> ISpellOrList(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISpellOrList, ISpellOrListGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISpellOrListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISpellOrList, ISpellOrListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlacedTrapTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlacedTrapTarget</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlacedTrapTarget, IPlacedTrapTargetGetter> IPlacedTrapTarget(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlacedTrapTarget, IPlacedTrapTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedTrapTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedTrapTarget, IPlacedTrapTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlacedTrapTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlacedTrapTarget</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlacedTrapTarget, IPlacedTrapTargetGetter> IPlacedTrapTarget(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlacedTrapTarget, IPlacedTrapTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedTrapTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedTrapTarget, IPlacedTrapTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IHarvestTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IHarvestTarget</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IHarvestTarget, IHarvestTargetGetter> IHarvestTarget(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IHarvestTarget, IHarvestTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHarvestTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHarvestTarget, IHarvestTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IHarvestTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IHarvestTarget</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IHarvestTarget, IHarvestTargetGetter> IHarvestTarget(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IHarvestTarget, IHarvestTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHarvestTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHarvestTarget, IHarvestTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IMagicItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IMagicItem</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMagicItem, IMagicItemGetter> IMagicItem(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMagicItem, IMagicItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMagicItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMagicItem, IMagicItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IMagicItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IMagicItem</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMagicItem, IMagicItemGetter> IMagicItem(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IMagicItem, IMagicItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMagicItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMagicItem, IMagicItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IKeywordLinkedReference
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IKeywordLinkedReference</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter> IKeywordLinkedReference(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IKeywordLinkedReference
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IKeywordLinkedReference</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter> IKeywordLinkedReference(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to INpcSpawn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on INpcSpawn</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, INpcSpawn, INpcSpawnGetter> INpcSpawn(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, INpcSpawn, INpcSpawnGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INpcSpawn, INpcSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to INpcSpawn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on INpcSpawn</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, INpcSpawn, INpcSpawnGetter> INpcSpawn(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, INpcSpawn, INpcSpawnGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INpcSpawn, INpcSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ISpellRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ISpellRecord</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISpellRecord, ISpellRecordGetter> ISpellRecord(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISpellRecord, ISpellRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISpellRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISpellRecord, ISpellRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ISpellRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ISpellRecord</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISpellRecord, ISpellRecordGetter> ISpellRecord(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISpellRecord, ISpellRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISpellRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISpellRecord, ISpellRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IEmittance
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IEmittance</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEmittance, IEmittanceGetter> IEmittance(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEmittance, IEmittanceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEmittanceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEmittance, IEmittanceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IEmittance
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IEmittance</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEmittance, IEmittanceGetter> IEmittance(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEmittance, IEmittanceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEmittanceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEmittance, IEmittanceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILocationRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ILocationRecord</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILocationRecord, ILocationRecordGetter> ILocationRecord(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILocationRecord, ILocationRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILocationRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocationRecord, ILocationRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILocationRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ILocationRecord</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILocationRecord, ILocationRecordGetter> ILocationRecord(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILocationRecord, ILocationRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILocationRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocationRecord, ILocationRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IKnowable
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IKnowable</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IKnowable, IKnowableGetter> IKnowable(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IKnowable, IKnowableGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKnowableGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IKnowable, IKnowableGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IKnowable
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IKnowable</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IKnowable, IKnowableGetter> IKnowable(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IKnowable, IKnowableGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKnowableGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IKnowable, IKnowableGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IEffectRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IEffectRecord</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEffectRecord, IEffectRecordGetter> IEffectRecord(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEffectRecord, IEffectRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEffectRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEffectRecord, IEffectRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IEffectRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IEffectRecord</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEffectRecord, IEffectRecordGetter> IEffectRecord(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IEffectRecord, IEffectRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEffectRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEffectRecord, IEffectRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILinkedReference
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ILinkedReference</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILinkedReference, ILinkedReferenceGetter> ILinkedReference(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILinkedReference, ILinkedReferenceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILinkedReference, ILinkedReferenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILinkedReference
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ILinkedReference</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILinkedReference, ILinkedReferenceGetter> ILinkedReference(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ILinkedReference, ILinkedReferenceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILinkedReference, ILinkedReferenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaced
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlaced</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlaced, IPlacedGetter> IPlaced(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlaced, IPlacedGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlaced, IPlacedGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaced
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlaced</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlaced, IPlacedGetter> IPlaced(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlaced, IPlacedGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlaced, IPlacedGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlacedSimple
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlacedSimple</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlacedSimple, IPlacedSimpleGetter> IPlacedSimple(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlacedSimple, IPlacedSimpleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedSimpleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedSimple, IPlacedSimpleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlacedSimple
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlacedSimple</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlacedSimple, IPlacedSimpleGetter> IPlacedSimple(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlacedSimple, IPlacedSimpleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedSimpleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedSimple, IPlacedSimpleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlacedThing
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlacedThing</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlacedThing, IPlacedThingGetter> IPlacedThing(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlacedThing, IPlacedThingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedThingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedThing, IPlacedThingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlacedThing
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlacedThing</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlacedThing, IPlacedThingGetter> IPlacedThing(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, IPlacedThing, IPlacedThingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedThingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedThing, IPlacedThingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ISound
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ISound</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISound, ISoundGetter> ISound(this IEnumerable<IModListingGetter<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISound, ISoundGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISound, ISoundGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ISound
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ISound</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISound, ISoundGetter> ISound(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimModGetter, ISound, ISoundGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISound, ISoundGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

    }
}
