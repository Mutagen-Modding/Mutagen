using System.Collections.Generic;
using Mutagen.Bethesda.Internals;

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
        public static TypedLoadOrderAccess<ISkyrimMod, IAcousticSpace, IAcousticSpaceGetter> AcousticSpace(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAcousticSpace, IAcousticSpaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAcousticSpace, IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AcousticSpace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AcousticSpace</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IAcousticSpace, IAcousticSpaceGetter> AcousticSpace(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAcousticSpace, IAcousticSpaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAcousticSpace, IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActionRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ActionRecord</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IActionRecord, IActionRecordGetter> ActionRecord(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IActionRecord, IActionRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IActionRecord, IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActionRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ActionRecord</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IActionRecord, IActionRecordGetter> ActionRecord(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IActionRecord, IActionRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IActionRecord, IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Activator
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Activator</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IActivator, IActivatorGetter> Activator(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IActivator, IActivatorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IActivator, IActivatorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Activator
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Activator</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IActivator, IActivatorGetter> Activator(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IActivator, IActivatorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IActivator, IActivatorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActorValueInformation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ActorValueInformation</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IActorValueInformation, IActorValueInformationGetter> ActorValueInformation(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IActorValueInformation, IActorValueInformationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IActorValueInformation, IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActorValueInformation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ActorValueInformation</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IActorValueInformation, IActorValueInformationGetter> ActorValueInformation(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IActorValueInformation, IActorValueInformationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IActorValueInformation, IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AddonNode
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AddonNode</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IAddonNode, IAddonNodeGetter> AddonNode(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAddonNode, IAddonNodeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAddonNode, IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AddonNode
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AddonNode</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IAddonNode, IAddonNodeGetter> AddonNode(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAddonNode, IAddonNodeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAddonNode, IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AlchemicalApparatus
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AlchemicalApparatus</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IAlchemicalApparatus, IAlchemicalApparatusGetter> AlchemicalApparatus(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAlchemicalApparatus, IAlchemicalApparatusGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AlchemicalApparatus
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AlchemicalApparatus</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IAlchemicalApparatus, IAlchemicalApparatusGetter> AlchemicalApparatus(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAlchemicalApparatus, IAlchemicalApparatusGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ammunition
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ammunition</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IAmmunition, IAmmunitionGetter> Ammunition(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAmmunition, IAmmunitionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAmmunition, IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ammunition
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ammunition</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IAmmunition, IAmmunitionGetter> Ammunition(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAmmunition, IAmmunitionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAmmunition, IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ANavigationMesh
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ANavigationMesh</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IANavigationMesh, IANavigationMeshGetter> ANavigationMesh(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IANavigationMesh, IANavigationMeshGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IANavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IANavigationMesh, IANavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ANavigationMesh
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ANavigationMesh</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IANavigationMesh, IANavigationMeshGetter> ANavigationMesh(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IANavigationMesh, IANavigationMeshGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IANavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IANavigationMesh, IANavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AnimatedObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AnimatedObject</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IAnimatedObject, IAnimatedObjectGetter> AnimatedObject(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAnimatedObject, IAnimatedObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAnimatedObject, IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AnimatedObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AnimatedObject</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IAnimatedObject, IAnimatedObjectGetter> AnimatedObject(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAnimatedObject, IAnimatedObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAnimatedObject, IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to APlacedTrap
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on APlacedTrap</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IAPlacedTrap, IAPlacedTrapGetter> APlacedTrap(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAPlacedTrap, IAPlacedTrapGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAPlacedTrap, IAPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to APlacedTrap
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on APlacedTrap</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IAPlacedTrap, IAPlacedTrapGetter> APlacedTrap(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAPlacedTrap, IAPlacedTrapGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAPlacedTrap, IAPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Armor
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Armor</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IArmor, IArmorGetter> Armor(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IArmor, IArmorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IArmor, IArmorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Armor
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Armor</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IArmor, IArmorGetter> Armor(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IArmor, IArmorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IArmor, IArmorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ArmorAddon
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ArmorAddon</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IArmorAddon, IArmorAddonGetter> ArmorAddon(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IArmorAddon, IArmorAddonGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IArmorAddon, IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ArmorAddon
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ArmorAddon</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IArmorAddon, IArmorAddonGetter> ArmorAddon(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IArmorAddon, IArmorAddonGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IArmorAddon, IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ArtObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ArtObject</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IArtObject, IArtObjectGetter> ArtObject(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IArtObject, IArtObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IArtObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IArtObject, IArtObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ArtObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ArtObject</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IArtObject, IArtObjectGetter> ArtObject(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IArtObject, IArtObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IArtObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IArtObject, IArtObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ASpell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ASpell</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IASpell, IASpellGetter> ASpell(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IASpell, IASpellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IASpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IASpell, IASpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ASpell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ASpell</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IASpell, IASpellGetter> ASpell(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IASpell, IASpellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IASpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IASpell, IASpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AssociationType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AssociationType</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IAssociationType, IAssociationTypeGetter> AssociationType(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAssociationType, IAssociationTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAssociationTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAssociationType, IAssociationTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AssociationType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AssociationType</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IAssociationType, IAssociationTypeGetter> AssociationType(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAssociationType, IAssociationTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAssociationTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAssociationType, IAssociationTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AStoryManagerNode
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AStoryManagerNode</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IAStoryManagerNode, IAStoryManagerNodeGetter> AStoryManagerNode(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAStoryManagerNode, IAStoryManagerNodeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAStoryManagerNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAStoryManagerNode, IAStoryManagerNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AStoryManagerNode
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AStoryManagerNode</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IAStoryManagerNode, IAStoryManagerNodeGetter> AStoryManagerNode(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAStoryManagerNode, IAStoryManagerNodeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAStoryManagerNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAStoryManagerNode, IAStoryManagerNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BodyPartData
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on BodyPartData</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IBodyPartData, IBodyPartDataGetter> BodyPartData(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IBodyPartData, IBodyPartDataGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IBodyPartData, IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BodyPartData
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on BodyPartData</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IBodyPartData, IBodyPartDataGetter> BodyPartData(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IBodyPartData, IBodyPartDataGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IBodyPartData, IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Book
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Book</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IBook, IBookGetter> Book(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IBook, IBookGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IBook, IBookGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Book
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Book</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IBook, IBookGetter> Book(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IBook, IBookGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IBook, IBookGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraPath
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CameraPath</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ICameraPath, ICameraPathGetter> CameraPath(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICameraPath, ICameraPathGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICameraPath, ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraPath
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CameraPath</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ICameraPath, ICameraPathGetter> CameraPath(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICameraPath, ICameraPathGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICameraPath, ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraShot
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CameraShot</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ICameraShot, ICameraShotGetter> CameraShot(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICameraShot, ICameraShotGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICameraShot, ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraShot
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CameraShot</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ICameraShot, ICameraShotGetter> CameraShot(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICameraShot, ICameraShotGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICameraShot, ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Cell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Cell</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ICell, ICellGetter> Cell(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICell, ICellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Cell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Cell</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ICell, ICellGetter> Cell(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICell, ICellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CellNavigationMesh
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CellNavigationMesh</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ICellNavigationMesh, ICellNavigationMeshGetter> CellNavigationMesh(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICellNavigationMesh, ICellNavigationMeshGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICellNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICellNavigationMesh, ICellNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CellNavigationMesh
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CellNavigationMesh</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ICellNavigationMesh, ICellNavigationMeshGetter> CellNavigationMesh(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICellNavigationMesh, ICellNavigationMeshGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICellNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICellNavigationMesh, ICellNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Class
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Class</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IClass, IClassGetter> Class(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IClass, IClassGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IClass, IClassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Class
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Class</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IClass, IClassGetter> Class(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IClass, IClassGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IClass, IClassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Climate
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Climate</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IClimate, IClimateGetter> Climate(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IClimate, IClimateGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IClimate, IClimateGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Climate
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Climate</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IClimate, IClimateGetter> Climate(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IClimate, IClimateGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IClimate, IClimateGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CollisionLayer
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CollisionLayer</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ICollisionLayer, ICollisionLayerGetter> CollisionLayer(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICollisionLayer, ICollisionLayerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICollisionLayerGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICollisionLayer, ICollisionLayerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CollisionLayer
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CollisionLayer</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ICollisionLayer, ICollisionLayerGetter> CollisionLayer(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICollisionLayer, ICollisionLayerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICollisionLayerGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICollisionLayer, ICollisionLayerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ColorRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ColorRecord</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IColorRecord, IColorRecordGetter> ColorRecord(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IColorRecord, IColorRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IColorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IColorRecord, IColorRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ColorRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ColorRecord</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IColorRecord, IColorRecordGetter> ColorRecord(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IColorRecord, IColorRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IColorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IColorRecord, IColorRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CombatStyle
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CombatStyle</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ICombatStyle, ICombatStyleGetter> CombatStyle(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICombatStyle, ICombatStyleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICombatStyle, ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CombatStyle
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CombatStyle</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ICombatStyle, ICombatStyleGetter> CombatStyle(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICombatStyle, ICombatStyleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICombatStyle, ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ConstructibleObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ConstructibleObject</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IConstructibleObject, IConstructibleObjectGetter> ConstructibleObject(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IConstructibleObject, IConstructibleObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IConstructibleObject, IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ConstructibleObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ConstructibleObject</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IConstructibleObject, IConstructibleObjectGetter> ConstructibleObject(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IConstructibleObject, IConstructibleObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IConstructibleObject, IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Container
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Container</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IContainer, IContainerGetter> Container(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IContainer, IContainerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IContainer, IContainerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Container
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Container</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IContainer, IContainerGetter> Container(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IContainer, IContainerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IContainer, IContainerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Debris
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Debris</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IDebris, IDebrisGetter> Debris(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDebris, IDebrisGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDebrisGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDebris, IDebrisGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Debris
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Debris</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IDebris, IDebrisGetter> Debris(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDebris, IDebrisGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDebrisGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDebris, IDebrisGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DefaultObjectManager
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DefaultObjectManager</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IDefaultObjectManager, IDefaultObjectManagerGetter> DefaultObjectManager(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDefaultObjectManager, IDefaultObjectManagerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DefaultObjectManager
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DefaultObjectManager</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IDefaultObjectManager, IDefaultObjectManagerGetter> DefaultObjectManager(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDefaultObjectManager, IDefaultObjectManagerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogBranch
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogBranch</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IDialogBranch, IDialogBranchGetter> DialogBranch(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDialogBranch, IDialogBranchGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogBranchGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogBranch, IDialogBranchGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogBranch
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogBranch</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IDialogBranch, IDialogBranchGetter> DialogBranch(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDialogBranch, IDialogBranchGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogBranchGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogBranch, IDialogBranchGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogResponses
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogResponses</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IDialogResponses, IDialogResponsesGetter> DialogResponses(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDialogResponses, IDialogResponsesGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogResponses, IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogResponses
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogResponses</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IDialogResponses, IDialogResponsesGetter> DialogResponses(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDialogResponses, IDialogResponsesGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogResponses, IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogTopic
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogTopic</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IDialogTopic, IDialogTopicGetter> DialogTopic(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDialogTopic, IDialogTopicGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogTopic, IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogTopic
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogTopic</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IDialogTopic, IDialogTopicGetter> DialogTopic(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDialogTopic, IDialogTopicGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogTopic, IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogView
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogView</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IDialogView, IDialogViewGetter> DialogView(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDialogView, IDialogViewGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogViewGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogView, IDialogViewGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogView
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogView</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IDialogView, IDialogViewGetter> DialogView(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDialogView, IDialogViewGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogViewGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogView, IDialogViewGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Door
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Door</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IDoor, IDoorGetter> Door(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDoor, IDoorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDoor, IDoorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Door
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Door</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IDoor, IDoorGetter> Door(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDoor, IDoorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDoor, IDoorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DualCastData
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DualCastData</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IDualCastData, IDualCastDataGetter> DualCastData(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDualCastData, IDualCastDataGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDualCastDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDualCastData, IDualCastDataGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DualCastData
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DualCastData</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IDualCastData, IDualCastDataGetter> DualCastData(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDualCastData, IDualCastDataGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDualCastDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDualCastData, IDualCastDataGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EffectShader
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EffectShader</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IEffectShader, IEffectShaderGetter> EffectShader(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEffectShader, IEffectShaderGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEffectShader, IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EffectShader
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EffectShader</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IEffectShader, IEffectShaderGetter> EffectShader(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEffectShader, IEffectShaderGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEffectShader, IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EncounterZone
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EncounterZone</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IEncounterZone, IEncounterZoneGetter> EncounterZone(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEncounterZone, IEncounterZoneGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEncounterZoneGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEncounterZone, IEncounterZoneGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EncounterZone
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EncounterZone</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IEncounterZone, IEncounterZoneGetter> EncounterZone(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEncounterZone, IEncounterZoneGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEncounterZoneGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEncounterZone, IEncounterZoneGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EquipType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EquipType</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IEquipType, IEquipTypeGetter> EquipType(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEquipType, IEquipTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEquipTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEquipType, IEquipTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EquipType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EquipType</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IEquipType, IEquipTypeGetter> EquipType(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEquipType, IEquipTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEquipTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEquipType, IEquipTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Explosion
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Explosion</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IExplosion, IExplosionGetter> Explosion(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IExplosion, IExplosionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IExplosionGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IExplosion, IExplosionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Explosion
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Explosion</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IExplosion, IExplosionGetter> Explosion(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IExplosion, IExplosionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IExplosionGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IExplosion, IExplosionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Eyes
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Eyes</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IEyes, IEyesGetter> Eyes(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEyes, IEyesGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEyesGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEyes, IEyesGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Eyes
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Eyes</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IEyes, IEyesGetter> Eyes(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEyes, IEyesGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEyesGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEyes, IEyesGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Faction
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Faction</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IFaction, IFactionGetter> Faction(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFaction, IFactionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFaction, IFactionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Faction
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Faction</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IFaction, IFactionGetter> Faction(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFaction, IFactionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFaction, IFactionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Flora
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Flora</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IFlora, IFloraGetter> Flora(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFlora, IFloraGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFlora, IFloraGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Flora
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Flora</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IFlora, IFloraGetter> Flora(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFlora, IFloraGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFlora, IFloraGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Footstep
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Footstep</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IFootstep, IFootstepGetter> Footstep(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFootstep, IFootstepGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFootstepGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFootstep, IFootstepGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Footstep
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Footstep</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IFootstep, IFootstepGetter> Footstep(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFootstep, IFootstepGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFootstepGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFootstep, IFootstepGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FootstepSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on FootstepSet</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IFootstepSet, IFootstepSetGetter> FootstepSet(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFootstepSet, IFootstepSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFootstepSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFootstepSet, IFootstepSetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FootstepSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on FootstepSet</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IFootstepSet, IFootstepSetGetter> FootstepSet(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFootstepSet, IFootstepSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFootstepSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFootstepSet, IFootstepSetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FormList
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on FormList</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IFormList, IFormListGetter> FormList(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFormList, IFormListGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFormListGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFormList, IFormListGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FormList
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on FormList</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IFormList, IFormListGetter> FormList(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFormList, IFormListGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFormListGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFormList, IFormListGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Furniture
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Furniture</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IFurniture, IFurnitureGetter> Furniture(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFurniture, IFurnitureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFurniture, IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Furniture
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Furniture</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IFurniture, IFurnitureGetter> Furniture(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFurniture, IFurnitureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFurniture, IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSetting, IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSetting, IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingBool
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSettingBool</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IGameSettingBool, IGameSettingBoolGetter> GameSettingBool(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGameSettingBool, IGameSettingBoolGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingBoolGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSettingBool, IGameSettingBoolGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingBool
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSettingBool</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IGameSettingBool, IGameSettingBoolGetter> GameSettingBool(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGameSettingBool, IGameSettingBoolGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingBoolGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSettingBool, IGameSettingBoolGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingFloat
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSettingFloat</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IGameSettingFloat, IGameSettingFloatGetter> GameSettingFloat(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGameSettingFloat, IGameSettingFloatGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSettingFloat, IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingFloat
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSettingFloat</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IGameSettingFloat, IGameSettingFloatGetter> GameSettingFloat(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGameSettingFloat, IGameSettingFloatGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSettingFloat, IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingInt
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSettingInt</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IGameSettingInt, IGameSettingIntGetter> GameSettingInt(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGameSettingInt, IGameSettingIntGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSettingInt, IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingInt
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSettingInt</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IGameSettingInt, IGameSettingIntGetter> GameSettingInt(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGameSettingInt, IGameSettingIntGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSettingInt, IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingString
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSettingString</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IGameSettingString, IGameSettingStringGetter> GameSettingString(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGameSettingString, IGameSettingStringGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSettingString, IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSettingString
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSettingString</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IGameSettingString, IGameSettingStringGetter> GameSettingString(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGameSettingString, IGameSettingStringGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSettingString, IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Global
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Global</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IGlobal, IGlobalGetter> Global(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGlobal, IGlobalGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGlobal, IGlobalGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Global
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Global</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IGlobal, IGlobalGetter> Global(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGlobal, IGlobalGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGlobal, IGlobalGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GlobalFloat
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GlobalFloat</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IGlobalFloat, IGlobalFloatGetter> GlobalFloat(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGlobalFloat, IGlobalFloatGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGlobalFloat, IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GlobalFloat
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GlobalFloat</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IGlobalFloat, IGlobalFloatGetter> GlobalFloat(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGlobalFloat, IGlobalFloatGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGlobalFloat, IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GlobalInt
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GlobalInt</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IGlobalInt, IGlobalIntGetter> GlobalInt(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGlobalInt, IGlobalIntGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGlobalInt, IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GlobalInt
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GlobalInt</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IGlobalInt, IGlobalIntGetter> GlobalInt(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGlobalInt, IGlobalIntGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGlobalInt, IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GlobalShort
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GlobalShort</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IGlobalShort, IGlobalShortGetter> GlobalShort(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGlobalShort, IGlobalShortGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGlobalShort, IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GlobalShort
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GlobalShort</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IGlobalShort, IGlobalShortGetter> GlobalShort(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGlobalShort, IGlobalShortGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGlobalShort, IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Grass
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Grass</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IGrass, IGrassGetter> Grass(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGrass, IGrassGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGrass, IGrassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Grass
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Grass</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IGrass, IGrassGetter> Grass(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGrass, IGrassGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGrass, IGrassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hair
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Hair</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IHair, IHairGetter> Hair(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IHair, IHairGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHair, IHairGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hair
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Hair</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IHair, IHairGetter> Hair(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IHair, IHairGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHair, IHairGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hazard
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Hazard</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IHazard, IHazardGetter> Hazard(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IHazard, IHazardGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHazardGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHazard, IHazardGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hazard
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Hazard</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IHazard, IHazardGetter> Hazard(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IHazard, IHazardGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHazardGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHazard, IHazardGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to HeadPart
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on HeadPart</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IHeadPart, IHeadPartGetter> HeadPart(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IHeadPart, IHeadPartGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHeadPart, IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to HeadPart
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on HeadPart</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IHeadPart, IHeadPartGetter> HeadPart(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IHeadPart, IHeadPartGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHeadPart, IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleAnimation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IdleAnimation</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IIdleAnimation, IIdleAnimationGetter> IdleAnimation(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IIdleAnimation, IIdleAnimationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIdleAnimation, IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleAnimation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IdleAnimation</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IIdleAnimation, IIdleAnimationGetter> IdleAnimation(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IIdleAnimation, IIdleAnimationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIdleAnimation, IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleMarker
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IdleMarker</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IIdleMarker, IIdleMarkerGetter> IdleMarker(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IIdleMarker, IIdleMarkerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIdleMarker, IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleMarker
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IdleMarker</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IIdleMarker, IIdleMarkerGetter> IdleMarker(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IIdleMarker, IIdleMarkerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIdleMarker, IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ImageSpace</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IImageSpace, IImageSpaceGetter> ImageSpace(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IImageSpace, IImageSpaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImageSpace, IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ImageSpace</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IImageSpace, IImageSpaceGetter> ImageSpace(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IImageSpace, IImageSpaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImageSpace, IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpaceAdapter
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ImageSpaceAdapter</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IImageSpaceAdapter, IImageSpaceAdapterGetter> ImageSpaceAdapter(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IImageSpaceAdapter, IImageSpaceAdapterGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpaceAdapter
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ImageSpaceAdapter</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IImageSpaceAdapter, IImageSpaceAdapterGetter> ImageSpaceAdapter(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IImageSpaceAdapter, IImageSpaceAdapterGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Impact
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Impact</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IImpact, IImpactGetter> Impact(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IImpact, IImpactGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImpactGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImpact, IImpactGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Impact
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Impact</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IImpact, IImpactGetter> Impact(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IImpact, IImpactGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImpactGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImpact, IImpactGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImpactDataSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ImpactDataSet</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IImpactDataSet, IImpactDataSetGetter> ImpactDataSet(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IImpactDataSet, IImpactDataSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImpactDataSet, IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImpactDataSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ImpactDataSet</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IImpactDataSet, IImpactDataSetGetter> ImpactDataSet(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IImpactDataSet, IImpactDataSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImpactDataSet, IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingestible
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ingestible</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IIngestible, IIngestibleGetter> Ingestible(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IIngestible, IIngestibleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIngestible, IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingestible
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ingestible</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IIngestible, IIngestibleGetter> Ingestible(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IIngestible, IIngestibleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIngestible, IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingredient
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ingredient</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IIngredient, IIngredientGetter> Ingredient(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IIngredient, IIngredientGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIngredient, IIngredientGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingredient
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ingredient</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IIngredient, IIngredientGetter> Ingredient(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IIngredient, IIngredientGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIngredient, IIngredientGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Key
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Key</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IKey, IKeyGetter> Key(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IKey, IKeyGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IKey, IKeyGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Key
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Key</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IKey, IKeyGetter> Key(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IKey, IKeyGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IKey, IKeyGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Keyword
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Keyword</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IKeyword, IKeywordGetter> Keyword(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IKeyword, IKeywordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeywordGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IKeyword, IKeywordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Keyword
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Keyword</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IKeyword, IKeywordGetter> Keyword(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IKeyword, IKeywordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeywordGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IKeyword, IKeywordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Landscape
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Landscape</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILandscape, ILandscapeGetter> Landscape(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILandscape, ILandscapeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILandscape, ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Landscape
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Landscape</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILandscape, ILandscapeGetter> Landscape(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILandscape, ILandscapeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILandscape, ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LandscapeTexture
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LandscapeTexture</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILandscapeTexture, ILandscapeTextureGetter> LandscapeTexture(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILandscapeTexture, ILandscapeTextureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILandscapeTexture, ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LandscapeTexture
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LandscapeTexture</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILandscapeTexture, ILandscapeTextureGetter> LandscapeTexture(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILandscapeTexture, ILandscapeTextureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILandscapeTexture, ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LensFlare
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LensFlare</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILensFlare, ILensFlareGetter> LensFlare(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILensFlare, ILensFlareGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILensFlareGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILensFlare, ILensFlareGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LensFlare
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LensFlare</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILensFlare, ILensFlareGetter> LensFlare(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILensFlare, ILensFlareGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILensFlareGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILensFlare, ILensFlareGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledItem</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILeveledItem, ILeveledItemGetter> LeveledItem(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILeveledItem, ILeveledItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILeveledItem, ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledItem</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILeveledItem, ILeveledItemGetter> LeveledItem(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILeveledItem, ILeveledItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILeveledItem, ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledNpc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledNpc</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILeveledNpc, ILeveledNpcGetter> LeveledNpc(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILeveledNpc, ILeveledNpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILeveledNpc, ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledNpc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledNpc</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILeveledNpc, ILeveledNpcGetter> LeveledNpc(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILeveledNpc, ILeveledNpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILeveledNpc, ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledSpell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledSpell</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILeveledSpell, ILeveledSpellGetter> LeveledSpell(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILeveledSpell, ILeveledSpellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILeveledSpell, ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledSpell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledSpell</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILeveledSpell, ILeveledSpellGetter> LeveledSpell(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILeveledSpell, ILeveledSpellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILeveledSpell, ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Light
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Light</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILight, ILightGetter> Light(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILight, ILightGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILight, ILightGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Light
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Light</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILight, ILightGetter> Light(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILight, ILightGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILight, ILightGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LightingTemplate
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LightingTemplate</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILightingTemplate, ILightingTemplateGetter> LightingTemplate(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILightingTemplate, ILightingTemplateGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILightingTemplate, ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LightingTemplate
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LightingTemplate</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILightingTemplate, ILightingTemplateGetter> LightingTemplate(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILightingTemplate, ILightingTemplateGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILightingTemplate, ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LoadScreen
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LoadScreen</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILoadScreen, ILoadScreenGetter> LoadScreen(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILoadScreen, ILoadScreenGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILoadScreen, ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LoadScreen
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LoadScreen</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILoadScreen, ILoadScreenGetter> LoadScreen(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILoadScreen, ILoadScreenGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILoadScreen, ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Location
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Location</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILocation, ILocationGetter> Location(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILocation, ILocationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILocationGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocation, ILocationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Location
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Location</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILocation, ILocationGetter> Location(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILocation, ILocationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILocationGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocation, ILocationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LocationReferenceType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LocationReferenceType</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILocationReferenceType, ILocationReferenceTypeGetter> LocationReferenceType(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILocationReferenceType, ILocationReferenceTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LocationReferenceType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LocationReferenceType</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILocationReferenceType, ILocationReferenceTypeGetter> LocationReferenceType(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILocationReferenceType, ILocationReferenceTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MagicEffect
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MagicEffect</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IMagicEffect, IMagicEffectGetter> MagicEffect(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMagicEffect, IMagicEffectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMagicEffect, IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MagicEffect
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MagicEffect</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IMagicEffect, IMagicEffectGetter> MagicEffect(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMagicEffect, IMagicEffectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMagicEffect, IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MaterialObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MaterialObject</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IMaterialObject, IMaterialObjectGetter> MaterialObject(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMaterialObject, IMaterialObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMaterialObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMaterialObject, IMaterialObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MaterialObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MaterialObject</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IMaterialObject, IMaterialObjectGetter> MaterialObject(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMaterialObject, IMaterialObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMaterialObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMaterialObject, IMaterialObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MaterialType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MaterialType</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IMaterialType, IMaterialTypeGetter> MaterialType(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMaterialType, IMaterialTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMaterialTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMaterialType, IMaterialTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MaterialType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MaterialType</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IMaterialType, IMaterialTypeGetter> MaterialType(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMaterialType, IMaterialTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMaterialTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMaterialType, IMaterialTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Message
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Message</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IMessage, IMessageGetter> Message(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMessage, IMessageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMessageGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMessage, IMessageGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Message
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Message</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IMessage, IMessageGetter> Message(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMessage, IMessageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMessageGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMessage, IMessageGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MiscItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MiscItem</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IMiscItem, IMiscItemGetter> MiscItem(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMiscItem, IMiscItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMiscItem, IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MiscItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MiscItem</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IMiscItem, IMiscItemGetter> MiscItem(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMiscItem, IMiscItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMiscItem, IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MoveableStatic
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MoveableStatic</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IMoveableStatic, IMoveableStaticGetter> MoveableStatic(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMoveableStatic, IMoveableStaticGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMoveableStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMoveableStatic, IMoveableStaticGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MoveableStatic
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MoveableStatic</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IMoveableStatic, IMoveableStaticGetter> MoveableStatic(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMoveableStatic, IMoveableStaticGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMoveableStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMoveableStatic, IMoveableStaticGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MovementType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MovementType</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IMovementType, IMovementTypeGetter> MovementType(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMovementType, IMovementTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMovementTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMovementType, IMovementTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MovementType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MovementType</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IMovementType, IMovementTypeGetter> MovementType(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMovementType, IMovementTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMovementTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMovementType, IMovementTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MusicTrack
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MusicTrack</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IMusicTrack, IMusicTrackGetter> MusicTrack(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMusicTrack, IMusicTrackGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMusicTrackGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMusicTrack, IMusicTrackGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MusicTrack
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MusicTrack</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IMusicTrack, IMusicTrackGetter> MusicTrack(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMusicTrack, IMusicTrackGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMusicTrackGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMusicTrack, IMusicTrackGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MusicType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MusicType</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IMusicType, IMusicTypeGetter> MusicType(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMusicType, IMusicTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMusicType, IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MusicType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MusicType</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IMusicType, IMusicTypeGetter> MusicType(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMusicType, IMusicTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMusicType, IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMeshInfoMap
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on NavigationMeshInfoMap</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, INavigationMeshInfoMap, INavigationMeshInfoMapGetter> NavigationMeshInfoMap(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMeshInfoMap
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on NavigationMeshInfoMap</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, INavigationMeshInfoMap, INavigationMeshInfoMapGetter> NavigationMeshInfoMap(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, INpc, INpcGetter> Npc(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, INpc, INpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, INpc, INpcGetter> Npc(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, INpc, INpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ObjectEffect
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ObjectEffect</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IObjectEffect, IObjectEffectGetter> ObjectEffect(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IObjectEffect, IObjectEffectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IObjectEffect, IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ObjectEffect
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ObjectEffect</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IObjectEffect, IObjectEffectGetter> ObjectEffect(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IObjectEffect, IObjectEffectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IObjectEffect, IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Outfit
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Outfit</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IOutfit, IOutfitGetter> Outfit(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IOutfit, IOutfitGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IOutfitGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IOutfit, IOutfitGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Outfit
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Outfit</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IOutfit, IOutfitGetter> Outfit(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IOutfit, IOutfitGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IOutfitGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IOutfit, IOutfitGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Package
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Package</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPackage, IPackageGetter> Package(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPackage, IPackageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPackageGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPackage, IPackageGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Package
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Package</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPackage, IPackageGetter> Package(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPackage, IPackageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPackageGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPackage, IPackageGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Perk
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Perk</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPerk, IPerkGetter> Perk(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPerk, IPerkGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPerkGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPerk, IPerkGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Perk
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Perk</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPerk, IPerkGetter> Perk(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPerk, IPerkGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPerkGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPerk, IPerkGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedArrow
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedArrow</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedArrow, IPlacedArrowGetter> PlacedArrow(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedArrow, IPlacedArrowGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedArrowGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedArrow, IPlacedArrowGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedArrow
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedArrow</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedArrow, IPlacedArrowGetter> PlacedArrow(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedArrow, IPlacedArrowGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedArrowGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedArrow, IPlacedArrowGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedBarrier
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedBarrier</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedBarrier, IPlacedBarrierGetter> PlacedBarrier(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedBarrier, IPlacedBarrierGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedBarrierGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedBarrier, IPlacedBarrierGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedBarrier
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedBarrier</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedBarrier, IPlacedBarrierGetter> PlacedBarrier(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedBarrier, IPlacedBarrierGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedBarrierGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedBarrier, IPlacedBarrierGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedBeam
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedBeam</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedBeam, IPlacedBeamGetter> PlacedBeam(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedBeam, IPlacedBeamGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedBeamGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedBeam, IPlacedBeamGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedBeam
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedBeam</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedBeam, IPlacedBeamGetter> PlacedBeam(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedBeam, IPlacedBeamGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedBeamGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedBeam, IPlacedBeamGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedCone
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedCone</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedCone, IPlacedConeGetter> PlacedCone(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedCone, IPlacedConeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedConeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedCone, IPlacedConeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedCone
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedCone</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedCone, IPlacedConeGetter> PlacedCone(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedCone, IPlacedConeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedConeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedCone, IPlacedConeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedFlame
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedFlame</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedFlame, IPlacedFlameGetter> PlacedFlame(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedFlame, IPlacedFlameGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedFlameGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedFlame, IPlacedFlameGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedFlame
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedFlame</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedFlame, IPlacedFlameGetter> PlacedFlame(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedFlame, IPlacedFlameGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedFlameGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedFlame, IPlacedFlameGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedHazard
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedHazard</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedHazard, IPlacedHazardGetter> PlacedHazard(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedHazard, IPlacedHazardGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedHazardGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedHazard, IPlacedHazardGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedHazard
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedHazard</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedHazard, IPlacedHazardGetter> PlacedHazard(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedHazard, IPlacedHazardGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedHazardGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedHazard, IPlacedHazardGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedMissile
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedMissile</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedMissile, IPlacedMissileGetter> PlacedMissile(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedMissile, IPlacedMissileGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedMissileGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedMissile, IPlacedMissileGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedMissile
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedMissile</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedMissile, IPlacedMissileGetter> PlacedMissile(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedMissile, IPlacedMissileGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedMissileGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedMissile, IPlacedMissileGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedNpc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedNpc</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedNpc, IPlacedNpcGetter> PlacedNpc(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedNpc, IPlacedNpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedNpc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedNpc</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedNpc, IPlacedNpcGetter> PlacedNpc(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedNpc, IPlacedNpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedObject</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedObject, IPlacedObjectGetter> PlacedObject(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedObject, IPlacedObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedObject, IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedObject</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedObject, IPlacedObjectGetter> PlacedObject(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedObject, IPlacedObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedObject, IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedTrap
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedTrap</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedTrap, IPlacedTrapGetter> PlacedTrap(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedTrap, IPlacedTrapGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedTrap, IPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedTrap
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedTrap</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedTrap, IPlacedTrapGetter> PlacedTrap(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedTrap, IPlacedTrapGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedTrap, IPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Projectile
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Projectile</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IProjectile, IProjectileGetter> Projectile(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IProjectile, IProjectileGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IProjectileGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IProjectile, IProjectileGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Projectile
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Projectile</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IProjectile, IProjectileGetter> Projectile(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IProjectile, IProjectileGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IProjectileGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IProjectile, IProjectileGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Quest
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Quest</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IQuest, IQuestGetter> Quest(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IQuest, IQuestGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IQuest, IQuestGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Quest
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Quest</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IQuest, IQuestGetter> Quest(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IQuest, IQuestGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IQuest, IQuestGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IRace, IRaceGetter> Race(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRace, IRaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IRace, IRaceGetter> Race(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRace, IRaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Region
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Region</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IRegion, IRegionGetter> Region(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IRegion, IRegionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRegion, IRegionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Region
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Region</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IRegion, IRegionGetter> Region(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IRegion, IRegionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRegion, IRegionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Relationship
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Relationship</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IRelationship, IRelationshipGetter> Relationship(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IRelationship, IRelationshipGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRelationshipGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRelationship, IRelationshipGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Relationship
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Relationship</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IRelationship, IRelationshipGetter> Relationship(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IRelationship, IRelationshipGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRelationshipGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRelationship, IRelationshipGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ReverbParameters
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ReverbParameters</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IReverbParameters, IReverbParametersGetter> ReverbParameters(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IReverbParameters, IReverbParametersGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IReverbParametersGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IReverbParameters, IReverbParametersGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ReverbParameters
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ReverbParameters</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IReverbParameters, IReverbParametersGetter> ReverbParameters(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IReverbParameters, IReverbParametersGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IReverbParametersGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IReverbParameters, IReverbParametersGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Scene
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Scene</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IScene, ISceneGetter> Scene(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IScene, ISceneGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISceneGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IScene, ISceneGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Scene
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Scene</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IScene, ISceneGetter> Scene(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IScene, ISceneGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISceneGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IScene, ISceneGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Scroll
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Scroll</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IScroll, IScrollGetter> Scroll(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IScroll, IScrollGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IScrollGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IScroll, IScrollGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Scroll
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Scroll</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IScroll, IScrollGetter> Scroll(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IScroll, IScrollGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IScrollGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IScroll, IScrollGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ShaderParticleGeometry
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ShaderParticleGeometry</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IShaderParticleGeometry, IShaderParticleGeometryGetter> ShaderParticleGeometry(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IShaderParticleGeometry, IShaderParticleGeometryGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IShaderParticleGeometryGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ShaderParticleGeometry
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ShaderParticleGeometry</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IShaderParticleGeometry, IShaderParticleGeometryGetter> ShaderParticleGeometry(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IShaderParticleGeometry, IShaderParticleGeometryGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IShaderParticleGeometryGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Shout
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Shout</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IShout, IShoutGetter> Shout(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IShout, IShoutGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IShoutGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IShout, IShoutGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Shout
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Shout</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IShout, IShoutGetter> Shout(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IShout, IShoutGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IShoutGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IShout, IShoutGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SkyrimMajorRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SkyrimMajorRecord</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> SkyrimMajorRecord(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISkyrimMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SkyrimMajorRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SkyrimMajorRecord</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> SkyrimMajorRecord(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISkyrimMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoulGem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoulGem</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISoulGem, ISoulGemGetter> SoulGem(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISoulGem, ISoulGemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoulGem, ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoulGem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoulGem</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISoulGem, ISoulGemGetter> SoulGem(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISoulGem, ISoulGemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoulGem, ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundCategory
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoundCategory</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISoundCategory, ISoundCategoryGetter> SoundCategory(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISoundCategory, ISoundCategoryGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundCategoryGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundCategory, ISoundCategoryGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundCategory
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoundCategory</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISoundCategory, ISoundCategoryGetter> SoundCategory(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISoundCategory, ISoundCategoryGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundCategoryGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundCategory, ISoundCategoryGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundDescriptor
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoundDescriptor</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISoundDescriptor, ISoundDescriptorGetter> SoundDescriptor(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISoundDescriptor, ISoundDescriptorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundDescriptorGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundDescriptor, ISoundDescriptorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundDescriptor
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoundDescriptor</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISoundDescriptor, ISoundDescriptorGetter> SoundDescriptor(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISoundDescriptor, ISoundDescriptorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundDescriptorGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundDescriptor, ISoundDescriptorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundMarker
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoundMarker</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISoundMarker, ISoundMarkerGetter> SoundMarker(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISoundMarker, ISoundMarkerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundMarker, ISoundMarkerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundMarker
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoundMarker</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISoundMarker, ISoundMarkerGetter> SoundMarker(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISoundMarker, ISoundMarkerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundMarker, ISoundMarkerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundOutputModel
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoundOutputModel</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISoundOutputModel, ISoundOutputModelGetter> SoundOutputModel(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISoundOutputModel, ISoundOutputModelGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundOutputModelGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundOutputModel, ISoundOutputModelGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundOutputModel
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoundOutputModel</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISoundOutputModel, ISoundOutputModelGetter> SoundOutputModel(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISoundOutputModel, ISoundOutputModelGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundOutputModelGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundOutputModel, ISoundOutputModelGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Spell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Spell</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISpell, ISpellGetter> Spell(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISpell, ISpellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISpell, ISpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Spell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Spell</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISpell, ISpellGetter> Spell(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISpell, ISpellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISpell, ISpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Static
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Static</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IStatic, IStaticGetter> Static(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IStatic, IStaticGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IStatic, IStaticGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Static
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Static</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IStatic, IStaticGetter> Static(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IStatic, IStaticGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IStatic, IStaticGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StoryManagerBranchNode
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on StoryManagerBranchNode</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IStoryManagerBranchNode, IStoryManagerBranchNodeGetter> StoryManagerBranchNode(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IStoryManagerBranchNode, IStoryManagerBranchNodeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStoryManagerBranchNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IStoryManagerBranchNode, IStoryManagerBranchNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StoryManagerBranchNode
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on StoryManagerBranchNode</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IStoryManagerBranchNode, IStoryManagerBranchNodeGetter> StoryManagerBranchNode(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IStoryManagerBranchNode, IStoryManagerBranchNodeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStoryManagerBranchNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IStoryManagerBranchNode, IStoryManagerBranchNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StoryManagerEventNode
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on StoryManagerEventNode</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IStoryManagerEventNode, IStoryManagerEventNodeGetter> StoryManagerEventNode(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IStoryManagerEventNode, IStoryManagerEventNodeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStoryManagerEventNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IStoryManagerEventNode, IStoryManagerEventNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StoryManagerEventNode
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on StoryManagerEventNode</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IStoryManagerEventNode, IStoryManagerEventNodeGetter> StoryManagerEventNode(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IStoryManagerEventNode, IStoryManagerEventNodeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStoryManagerEventNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IStoryManagerEventNode, IStoryManagerEventNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StoryManagerQuestNode
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on StoryManagerQuestNode</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter> StoryManagerQuestNode(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStoryManagerQuestNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StoryManagerQuestNode
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on StoryManagerQuestNode</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter> StoryManagerQuestNode(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStoryManagerQuestNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TalkingActivator
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on TalkingActivator</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ITalkingActivator, ITalkingActivatorGetter> TalkingActivator(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ITalkingActivator, ITalkingActivatorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITalkingActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ITalkingActivator, ITalkingActivatorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TalkingActivator
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on TalkingActivator</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ITalkingActivator, ITalkingActivatorGetter> TalkingActivator(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ITalkingActivator, ITalkingActivatorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITalkingActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ITalkingActivator, ITalkingActivatorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TextureSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on TextureSet</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ITextureSet, ITextureSetGetter> TextureSet(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ITextureSet, ITextureSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ITextureSet, ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TextureSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on TextureSet</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ITextureSet, ITextureSetGetter> TextureSet(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ITextureSet, ITextureSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ITextureSet, ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Tree
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Tree</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ITree, ITreeGetter> Tree(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ITree, ITreeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ITree, ITreeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Tree
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Tree</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ITree, ITreeGetter> Tree(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ITree, ITreeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ITree, ITreeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VisualEffect
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on VisualEffect</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IVisualEffect, IVisualEffectGetter> VisualEffect(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IVisualEffect, IVisualEffectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IVisualEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IVisualEffect, IVisualEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VisualEffect
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on VisualEffect</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IVisualEffect, IVisualEffectGetter> VisualEffect(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IVisualEffect, IVisualEffectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IVisualEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IVisualEffect, IVisualEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VoiceType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on VoiceType</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IVoiceType, IVoiceTypeGetter> VoiceType(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IVoiceType, IVoiceTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IVoiceType, IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VoiceType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on VoiceType</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IVoiceType, IVoiceTypeGetter> VoiceType(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IVoiceType, IVoiceTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IVoiceType, IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VolumetricLighting
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on VolumetricLighting</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IVolumetricLighting, IVolumetricLightingGetter> VolumetricLighting(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IVolumetricLighting, IVolumetricLightingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IVolumetricLightingGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IVolumetricLighting, IVolumetricLightingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VolumetricLighting
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on VolumetricLighting</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IVolumetricLighting, IVolumetricLightingGetter> VolumetricLighting(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IVolumetricLighting, IVolumetricLightingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IVolumetricLightingGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IVolumetricLighting, IVolumetricLightingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Water
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Water</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IWater, IWaterGetter> Water(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWater, IWaterGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWater, IWaterGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Water
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Water</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IWater, IWaterGetter> Water(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWater, IWaterGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWater, IWaterGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weapon
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Weapon</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IWeapon, IWeaponGetter> Weapon(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWeapon, IWeaponGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWeapon, IWeaponGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weapon
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Weapon</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IWeapon, IWeaponGetter> Weapon(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWeapon, IWeaponGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWeapon, IWeaponGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weather
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Weather</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IWeather, IWeatherGetter> Weather(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWeather, IWeatherGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWeather, IWeatherGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weather
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Weather</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IWeather, IWeatherGetter> Weather(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWeather, IWeatherGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWeather, IWeatherGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to WordOfPower
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on WordOfPower</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IWordOfPower, IWordOfPowerGetter> WordOfPower(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWordOfPower, IWordOfPowerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWordOfPowerGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWordOfPower, IWordOfPowerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to WordOfPower
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on WordOfPower</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IWordOfPower, IWordOfPowerGetter> WordOfPower(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWordOfPower, IWordOfPowerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWordOfPowerGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWordOfPower, IWordOfPowerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Worldspace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Worldspace</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IWorldspace, IWorldspaceGetter> Worldspace(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWorldspace, IWorldspaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Worldspace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Worldspace</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IWorldspace, IWorldspaceGetter> Worldspace(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWorldspace, IWorldspaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to WorldspaceNavigationMesh
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on WorldspaceNavigationMesh</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IWorldspaceNavigationMesh, IWorldspaceNavigationMeshGetter> WorldspaceNavigationMesh(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWorldspaceNavigationMesh, IWorldspaceNavigationMeshGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWorldspaceNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWorldspaceNavigationMesh, IWorldspaceNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to WorldspaceNavigationMesh
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on WorldspaceNavigationMesh</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IWorldspaceNavigationMesh, IWorldspaceNavigationMeshGetter> WorldspaceNavigationMesh(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWorldspaceNavigationMesh, IWorldspaceNavigationMeshGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWorldspaceNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWorldspaceNavigationMesh, IWorldspaceNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

        #region Link Interfaces
        /// <summary>
        /// Scope a load order query to IIdleRelation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IIdleRelation</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IIdleRelation, IIdleRelationGetter> IIdleRelation(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IIdleRelation, IIdleRelationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIdleRelation, IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IIdleRelation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IIdleRelation</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IIdleRelation, IIdleRelationGetter> IIdleRelation(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IIdleRelation, IIdleRelationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIdleRelation, IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IObjectId
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IObjectId</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IObjectId, IObjectIdGetter> IObjectId(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IObjectId, IObjectIdGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IObjectIdGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IObjectId, IObjectIdGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IObjectId
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IObjectId</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IObjectId, IObjectIdGetter> IObjectId(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IObjectId, IObjectIdGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IObjectIdGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IObjectId, IObjectIdGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IItem</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IItem, IItemGetter> IItem(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IItem, IItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IItem, IItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IItem</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IItem, IItemGetter> IItem(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IItem, IItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IItem, IItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IOutfitTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IOutfitTarget</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IOutfitTarget, IOutfitTargetGetter> IOutfitTarget(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IOutfitTarget, IOutfitTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IOutfitTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IOutfitTarget, IOutfitTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IOutfitTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IOutfitTarget</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IOutfitTarget, IOutfitTargetGetter> IOutfitTarget(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IOutfitTarget, IOutfitTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IOutfitTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IOutfitTarget, IOutfitTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IComplexLocation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IComplexLocation</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IComplexLocation, IComplexLocationGetter> IComplexLocation(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IComplexLocation, IComplexLocationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IComplexLocationGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IComplexLocation, IComplexLocationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IComplexLocation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IComplexLocation</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IComplexLocation, IComplexLocationGetter> IComplexLocation(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IComplexLocation, IComplexLocationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IComplexLocationGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IComplexLocation, IComplexLocationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IDialog
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IDialog</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IDialog, IDialogGetter> IDialog(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDialog, IDialogGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialog, IDialogGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IDialog
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IDialog</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IDialog, IDialogGetter> IDialog(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDialog, IDialogGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialog, IDialogGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILocationTargetable
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ILocationTargetable</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILocationTargetable, ILocationTargetableGetter> ILocationTargetable(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILocationTargetable, ILocationTargetableGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILocationTargetableGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocationTargetable, ILocationTargetableGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILocationTargetable
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ILocationTargetable</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILocationTargetable, ILocationTargetableGetter> ILocationTargetable(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILocationTargetable, ILocationTargetableGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILocationTargetableGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocationTargetable, ILocationTargetableGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IOwner
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IOwner</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IOwner, IOwnerGetter> IOwner(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IOwner, IOwnerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IOwner, IOwnerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IOwner
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IOwner</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IOwner, IOwnerGetter> IOwner(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IOwner, IOwnerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IOwner, IOwnerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRelatable
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IRelatable</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IRelatable, IRelatableGetter> IRelatable(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IRelatable, IRelatableGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRelatableGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRelatable, IRelatableGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRelatable
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IRelatable</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IRelatable, IRelatableGetter> IRelatable(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IRelatable, IRelatableGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRelatableGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRelatable, IRelatableGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRegionTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IRegionTarget</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IRegionTarget, IRegionTargetGetter> IRegionTarget(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IRegionTarget, IRegionTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRegionTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRegionTarget, IRegionTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRegionTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IRegionTarget</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IRegionTarget, IRegionTargetGetter> IRegionTarget(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IRegionTarget, IRegionTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRegionTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRegionTarget, IRegionTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IAliasVoiceType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IAliasVoiceType</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IAliasVoiceType, IAliasVoiceTypeGetter> IAliasVoiceType(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAliasVoiceType, IAliasVoiceTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAliasVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAliasVoiceType, IAliasVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IAliasVoiceType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IAliasVoiceType</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IAliasVoiceType, IAliasVoiceTypeGetter> IAliasVoiceType(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAliasVoiceType, IAliasVoiceTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAliasVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAliasVoiceType, IAliasVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILockList
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ILockList</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILockList, ILockListGetter> ILockList(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILockList, ILockListGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILockListGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILockList, ILockListGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILockList
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ILockList</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILockList, ILockListGetter> ILockList(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILockList, ILockListGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILockListGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILockList, ILockListGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlacedTrapTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlacedTrapTarget</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedTrapTarget, IPlacedTrapTargetGetter> IPlacedTrapTarget(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedTrapTarget, IPlacedTrapTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedTrapTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedTrapTarget, IPlacedTrapTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlacedTrapTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlacedTrapTarget</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedTrapTarget, IPlacedTrapTargetGetter> IPlacedTrapTarget(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedTrapTarget, IPlacedTrapTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedTrapTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedTrapTarget, IPlacedTrapTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IHarvestTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IHarvestTarget</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IHarvestTarget, IHarvestTargetGetter> IHarvestTarget(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IHarvestTarget, IHarvestTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHarvestTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHarvestTarget, IHarvestTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IHarvestTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IHarvestTarget</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IHarvestTarget, IHarvestTargetGetter> IHarvestTarget(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IHarvestTarget, IHarvestTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHarvestTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHarvestTarget, IHarvestTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IKeywordLinkedReference
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IKeywordLinkedReference</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IKeywordLinkedReference, IKeywordLinkedReferenceGetter> IKeywordLinkedReference(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IKeywordLinkedReference
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IKeywordLinkedReference</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IKeywordLinkedReference, IKeywordLinkedReferenceGetter> IKeywordLinkedReference(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to INpcSpawn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on INpcSpawn</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, INpcSpawn, INpcSpawnGetter> INpcSpawn(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, INpcSpawn, INpcSpawnGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INpcSpawn, INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to INpcSpawn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on INpcSpawn</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, INpcSpawn, INpcSpawnGetter> INpcSpawn(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, INpcSpawn, INpcSpawnGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INpcSpawn, INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ISpellSpawn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ISpellSpawn</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISpellSpawn, ISpellSpawnGetter> ISpellSpawn(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISpellSpawn, ISpellSpawnGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISpellSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISpellSpawn, ISpellSpawnGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ISpellSpawn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ISpellSpawn</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISpellSpawn, ISpellSpawnGetter> ISpellSpawn(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISpellSpawn, ISpellSpawnGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISpellSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISpellSpawn, ISpellSpawnGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IEmittance
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IEmittance</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IEmittance, IEmittanceGetter> IEmittance(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEmittance, IEmittanceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEmittanceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEmittance, IEmittanceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IEmittance
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IEmittance</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IEmittance, IEmittanceGetter> IEmittance(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEmittance, IEmittanceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEmittanceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEmittance, IEmittanceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILocationRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ILocationRecord</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILocationRecord, ILocationRecordGetter> ILocationRecord(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILocationRecord, ILocationRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILocationRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocationRecord, ILocationRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILocationRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ILocationRecord</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILocationRecord, ILocationRecordGetter> ILocationRecord(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILocationRecord, ILocationRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILocationRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocationRecord, ILocationRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IEffectRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IEffectRecord</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IEffectRecord, IEffectRecordGetter> IEffectRecord(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEffectRecord, IEffectRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEffectRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEffectRecord, IEffectRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IEffectRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IEffectRecord</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IEffectRecord, IEffectRecordGetter> IEffectRecord(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEffectRecord, IEffectRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEffectRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEffectRecord, IEffectRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILinkedReference
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ILinkedReference</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILinkedReference, ILinkedReferenceGetter> ILinkedReference(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILinkedReference, ILinkedReferenceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILinkedReference, ILinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILinkedReference
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ILinkedReference</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ILinkedReference, ILinkedReferenceGetter> ILinkedReference(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILinkedReference, ILinkedReferenceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILinkedReference, ILinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaced
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlaced</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlaced, IPlacedGetter> IPlaced(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlaced, IPlacedGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlaced, IPlacedGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaced
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlaced</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlaced, IPlacedGetter> IPlaced(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlaced, IPlacedGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlaced, IPlacedGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlacedSimple
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlacedSimple</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedSimple, IPlacedSimpleGetter> IPlacedSimple(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedSimple, IPlacedSimpleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedSimpleGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedSimple, IPlacedSimpleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlacedSimple
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlacedSimple</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedSimple, IPlacedSimpleGetter> IPlacedSimple(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedSimple, IPlacedSimpleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedSimpleGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedSimple, IPlacedSimpleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlacedThing
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlacedThing</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedThing, IPlacedThingGetter> IPlacedThing(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedThing, IPlacedThingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedThingGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedThing, IPlacedThingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlacedThing
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlacedThing</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedThing, IPlacedThingGetter> IPlacedThing(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedThing, IPlacedThingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedThingGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedThing, IPlacedThingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ISound
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ISound</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISound, ISoundGetter> ISound(this IEnumerable<IModListing<ISkyrimModGetter>> listings)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISound, ISoundGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISound, ISoundGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ISound
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ISound</returns>
        public static TypedLoadOrderAccess<ISkyrimMod, ISound, ISoundGetter> ISound(this IEnumerable<ISkyrimModGetter> mods)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISound, ISoundGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords),
                (bool includeDeletedRecords) => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISound, ISoundGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

    }
}
