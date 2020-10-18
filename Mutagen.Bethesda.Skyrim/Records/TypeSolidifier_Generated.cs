using System.Collections.Generic;
using Mutagen.Bethesda.Internals;

namespace Mutagen.Bethesda.Skyrim
{
    public static class TypeOptionSolidifierMixIns
    {
        #region Normal
        public static TypedLoadOrderAccess<ISkyrimMod, IAcousticSpace, IAcousticSpaceGetter> AcousticSpace(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAcousticSpace, IAcousticSpaceGetter>(
                () => listings.WinningOverrides<IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAcousticSpace, IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IAcousticSpace, IAcousticSpaceGetter> AcousticSpace(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAcousticSpace, IAcousticSpaceGetter>(
                () => mods.WinningOverrides<IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAcousticSpace, IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IActionRecord, IActionRecordGetter> ActionRecord(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IActionRecord, IActionRecordGetter>(
                () => listings.WinningOverrides<IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IActionRecord, IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IActionRecord, IActionRecordGetter> ActionRecord(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IActionRecord, IActionRecordGetter>(
                () => mods.WinningOverrides<IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IActionRecord, IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IActivator, IActivatorGetter> Activator(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IActivator, IActivatorGetter>(
                () => listings.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IActivator, IActivatorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IActivator, IActivatorGetter> Activator(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IActivator, IActivatorGetter>(
                () => mods.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IActivator, IActivatorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IActorValueInformation, IActorValueInformationGetter> ActorValueInformation(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IActorValueInformation, IActorValueInformationGetter>(
                () => listings.WinningOverrides<IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IActorValueInformation, IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IActorValueInformation, IActorValueInformationGetter> ActorValueInformation(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IActorValueInformation, IActorValueInformationGetter>(
                () => mods.WinningOverrides<IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IActorValueInformation, IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IAddonNode, IAddonNodeGetter> AddonNode(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAddonNode, IAddonNodeGetter>(
                () => listings.WinningOverrides<IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAddonNode, IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IAddonNode, IAddonNodeGetter> AddonNode(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAddonNode, IAddonNodeGetter>(
                () => mods.WinningOverrides<IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAddonNode, IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IAlchemicalApparatus, IAlchemicalApparatusGetter> AlchemicalApparatus(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAlchemicalApparatus, IAlchemicalApparatusGetter>(
                () => listings.WinningOverrides<IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IAlchemicalApparatus, IAlchemicalApparatusGetter> AlchemicalApparatus(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAlchemicalApparatus, IAlchemicalApparatusGetter>(
                () => mods.WinningOverrides<IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IAmmunition, IAmmunitionGetter> Ammunition(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAmmunition, IAmmunitionGetter>(
                () => listings.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAmmunition, IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IAmmunition, IAmmunitionGetter> Ammunition(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAmmunition, IAmmunitionGetter>(
                () => mods.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAmmunition, IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IANavigationMesh, IANavigationMeshGetter> ANavigationMesh(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IANavigationMesh, IANavigationMeshGetter>(
                () => listings.WinningOverrides<IANavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IANavigationMesh, IANavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IANavigationMesh, IANavigationMeshGetter> ANavigationMesh(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IANavigationMesh, IANavigationMeshGetter>(
                () => mods.WinningOverrides<IANavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IANavigationMesh, IANavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IAnimatedObject, IAnimatedObjectGetter> AnimatedObject(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAnimatedObject, IAnimatedObjectGetter>(
                () => listings.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAnimatedObject, IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IAnimatedObject, IAnimatedObjectGetter> AnimatedObject(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAnimatedObject, IAnimatedObjectGetter>(
                () => mods.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAnimatedObject, IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IAPlacedTrap, IAPlacedTrapGetter> APlacedTrap(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAPlacedTrap, IAPlacedTrapGetter>(
                () => listings.WinningOverrides<IAPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAPlacedTrap, IAPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IAPlacedTrap, IAPlacedTrapGetter> APlacedTrap(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAPlacedTrap, IAPlacedTrapGetter>(
                () => mods.WinningOverrides<IAPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAPlacedTrap, IAPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IArmor, IArmorGetter> Armor(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IArmor, IArmorGetter>(
                () => listings.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IArmor, IArmorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IArmor, IArmorGetter> Armor(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IArmor, IArmorGetter>(
                () => mods.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IArmor, IArmorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IArmorAddon, IArmorAddonGetter> ArmorAddon(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IArmorAddon, IArmorAddonGetter>(
                () => listings.WinningOverrides<IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IArmorAddon, IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IArmorAddon, IArmorAddonGetter> ArmorAddon(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IArmorAddon, IArmorAddonGetter>(
                () => mods.WinningOverrides<IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IArmorAddon, IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IArtObject, IArtObjectGetter> ArtObject(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IArtObject, IArtObjectGetter>(
                () => listings.WinningOverrides<IArtObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IArtObject, IArtObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IArtObject, IArtObjectGetter> ArtObject(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IArtObject, IArtObjectGetter>(
                () => mods.WinningOverrides<IArtObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IArtObject, IArtObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IASpell, IASpellGetter> ASpell(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IASpell, IASpellGetter>(
                () => listings.WinningOverrides<IASpellGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IASpell, IASpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IASpell, IASpellGetter> ASpell(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IASpell, IASpellGetter>(
                () => mods.WinningOverrides<IASpellGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IASpell, IASpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IAssociationType, IAssociationTypeGetter> AssociationType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAssociationType, IAssociationTypeGetter>(
                () => listings.WinningOverrides<IAssociationTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAssociationType, IAssociationTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IAssociationType, IAssociationTypeGetter> AssociationType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAssociationType, IAssociationTypeGetter>(
                () => mods.WinningOverrides<IAssociationTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAssociationType, IAssociationTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IAStoryManagerNode, IAStoryManagerNodeGetter> AStoryManagerNode(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAStoryManagerNode, IAStoryManagerNodeGetter>(
                () => listings.WinningOverrides<IAStoryManagerNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAStoryManagerNode, IAStoryManagerNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IAStoryManagerNode, IAStoryManagerNodeGetter> AStoryManagerNode(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAStoryManagerNode, IAStoryManagerNodeGetter>(
                () => mods.WinningOverrides<IAStoryManagerNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAStoryManagerNode, IAStoryManagerNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IBodyPartData, IBodyPartDataGetter> BodyPartData(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IBodyPartData, IBodyPartDataGetter>(
                () => listings.WinningOverrides<IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IBodyPartData, IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IBodyPartData, IBodyPartDataGetter> BodyPartData(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IBodyPartData, IBodyPartDataGetter>(
                () => mods.WinningOverrides<IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IBodyPartData, IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IBook, IBookGetter> Book(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IBook, IBookGetter>(
                () => listings.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IBook, IBookGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IBook, IBookGetter> Book(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IBook, IBookGetter>(
                () => mods.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IBook, IBookGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ICameraPath, ICameraPathGetter> CameraPath(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICameraPath, ICameraPathGetter>(
                () => listings.WinningOverrides<ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICameraPath, ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ICameraPath, ICameraPathGetter> CameraPath(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICameraPath, ICameraPathGetter>(
                () => mods.WinningOverrides<ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICameraPath, ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ICameraShot, ICameraShotGetter> CameraShot(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICameraShot, ICameraShotGetter>(
                () => listings.WinningOverrides<ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICameraShot, ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ICameraShot, ICameraShotGetter> CameraShot(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICameraShot, ICameraShotGetter>(
                () => mods.WinningOverrides<ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICameraShot, ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ICell, ICellGetter> Cell(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICell, ICellGetter>(
                () => listings.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ICell, ICellGetter> Cell(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICell, ICellGetter>(
                () => mods.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ICellNavigationMesh, ICellNavigationMeshGetter> CellNavigationMesh(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICellNavigationMesh, ICellNavigationMeshGetter>(
                () => listings.WinningOverrides<ICellNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICellNavigationMesh, ICellNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ICellNavigationMesh, ICellNavigationMeshGetter> CellNavigationMesh(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICellNavigationMesh, ICellNavigationMeshGetter>(
                () => mods.WinningOverrides<ICellNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICellNavigationMesh, ICellNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IClass, IClassGetter> Class(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IClass, IClassGetter>(
                () => listings.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IClass, IClassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IClass, IClassGetter> Class(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IClass, IClassGetter>(
                () => mods.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IClass, IClassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IClimate, IClimateGetter> Climate(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IClimate, IClimateGetter>(
                () => listings.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IClimate, IClimateGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IClimate, IClimateGetter> Climate(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IClimate, IClimateGetter>(
                () => mods.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IClimate, IClimateGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ICollisionLayer, ICollisionLayerGetter> CollisionLayer(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICollisionLayer, ICollisionLayerGetter>(
                () => listings.WinningOverrides<ICollisionLayerGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICollisionLayer, ICollisionLayerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ICollisionLayer, ICollisionLayerGetter> CollisionLayer(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICollisionLayer, ICollisionLayerGetter>(
                () => mods.WinningOverrides<ICollisionLayerGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICollisionLayer, ICollisionLayerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IColorRecord, IColorRecordGetter> ColorRecord(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IColorRecord, IColorRecordGetter>(
                () => listings.WinningOverrides<IColorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IColorRecord, IColorRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IColorRecord, IColorRecordGetter> ColorRecord(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IColorRecord, IColorRecordGetter>(
                () => mods.WinningOverrides<IColorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IColorRecord, IColorRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ICombatStyle, ICombatStyleGetter> CombatStyle(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICombatStyle, ICombatStyleGetter>(
                () => listings.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICombatStyle, ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ICombatStyle, ICombatStyleGetter> CombatStyle(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ICombatStyle, ICombatStyleGetter>(
                () => mods.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ICombatStyle, ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IConstructibleObject, IConstructibleObjectGetter> ConstructibleObject(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IConstructibleObject, IConstructibleObjectGetter>(
                () => listings.WinningOverrides<IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IConstructibleObject, IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IConstructibleObject, IConstructibleObjectGetter> ConstructibleObject(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IConstructibleObject, IConstructibleObjectGetter>(
                () => mods.WinningOverrides<IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IConstructibleObject, IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IContainer, IContainerGetter> Container(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IContainer, IContainerGetter>(
                () => listings.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IContainer, IContainerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IContainer, IContainerGetter> Container(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IContainer, IContainerGetter>(
                () => mods.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IContainer, IContainerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IDebris, IDebrisGetter> Debris(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDebris, IDebrisGetter>(
                () => listings.WinningOverrides<IDebrisGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDebris, IDebrisGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IDebris, IDebrisGetter> Debris(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDebris, IDebrisGetter>(
                () => mods.WinningOverrides<IDebrisGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDebris, IDebrisGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IDefaultObjectManager, IDefaultObjectManagerGetter> DefaultObjectManager(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDefaultObjectManager, IDefaultObjectManagerGetter>(
                () => listings.WinningOverrides<IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IDefaultObjectManager, IDefaultObjectManagerGetter> DefaultObjectManager(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDefaultObjectManager, IDefaultObjectManagerGetter>(
                () => mods.WinningOverrides<IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IDialogBranch, IDialogBranchGetter> DialogBranch(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDialogBranch, IDialogBranchGetter>(
                () => listings.WinningOverrides<IDialogBranchGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogBranch, IDialogBranchGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IDialogBranch, IDialogBranchGetter> DialogBranch(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDialogBranch, IDialogBranchGetter>(
                () => mods.WinningOverrides<IDialogBranchGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogBranch, IDialogBranchGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IDialogResponses, IDialogResponsesGetter> DialogResponses(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDialogResponses, IDialogResponsesGetter>(
                () => listings.WinningOverrides<IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogResponses, IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IDialogResponses, IDialogResponsesGetter> DialogResponses(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDialogResponses, IDialogResponsesGetter>(
                () => mods.WinningOverrides<IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogResponses, IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IDialogTopic, IDialogTopicGetter> DialogTopic(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDialogTopic, IDialogTopicGetter>(
                () => listings.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogTopic, IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IDialogTopic, IDialogTopicGetter> DialogTopic(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDialogTopic, IDialogTopicGetter>(
                () => mods.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogTopic, IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IDialogView, IDialogViewGetter> DialogView(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDialogView, IDialogViewGetter>(
                () => listings.WinningOverrides<IDialogViewGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogView, IDialogViewGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IDialogView, IDialogViewGetter> DialogView(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDialogView, IDialogViewGetter>(
                () => mods.WinningOverrides<IDialogViewGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialogView, IDialogViewGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IDoor, IDoorGetter> Door(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDoor, IDoorGetter>(
                () => listings.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDoor, IDoorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IDoor, IDoorGetter> Door(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDoor, IDoorGetter>(
                () => mods.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDoor, IDoorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IDualCastData, IDualCastDataGetter> DualCastData(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDualCastData, IDualCastDataGetter>(
                () => listings.WinningOverrides<IDualCastDataGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDualCastData, IDualCastDataGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IDualCastData, IDualCastDataGetter> DualCastData(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDualCastData, IDualCastDataGetter>(
                () => mods.WinningOverrides<IDualCastDataGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDualCastData, IDualCastDataGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IEffectShader, IEffectShaderGetter> EffectShader(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEffectShader, IEffectShaderGetter>(
                () => listings.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEffectShader, IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IEffectShader, IEffectShaderGetter> EffectShader(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEffectShader, IEffectShaderGetter>(
                () => mods.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEffectShader, IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IEncounterZone, IEncounterZoneGetter> EncounterZone(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEncounterZone, IEncounterZoneGetter>(
                () => listings.WinningOverrides<IEncounterZoneGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEncounterZone, IEncounterZoneGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IEncounterZone, IEncounterZoneGetter> EncounterZone(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEncounterZone, IEncounterZoneGetter>(
                () => mods.WinningOverrides<IEncounterZoneGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEncounterZone, IEncounterZoneGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IEquipType, IEquipTypeGetter> EquipType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEquipType, IEquipTypeGetter>(
                () => listings.WinningOverrides<IEquipTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEquipType, IEquipTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IEquipType, IEquipTypeGetter> EquipType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEquipType, IEquipTypeGetter>(
                () => mods.WinningOverrides<IEquipTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEquipType, IEquipTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IExplosion, IExplosionGetter> Explosion(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IExplosion, IExplosionGetter>(
                () => listings.WinningOverrides<IExplosionGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IExplosion, IExplosionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IExplosion, IExplosionGetter> Explosion(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IExplosion, IExplosionGetter>(
                () => mods.WinningOverrides<IExplosionGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IExplosion, IExplosionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IEyes, IEyesGetter> Eyes(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEyes, IEyesGetter>(
                () => listings.WinningOverrides<IEyesGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEyes, IEyesGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IEyes, IEyesGetter> Eyes(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEyes, IEyesGetter>(
                () => mods.WinningOverrides<IEyesGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEyes, IEyesGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IFaction, IFactionGetter> Faction(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFaction, IFactionGetter>(
                () => listings.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFaction, IFactionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IFaction, IFactionGetter> Faction(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFaction, IFactionGetter>(
                () => mods.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFaction, IFactionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IFlora, IFloraGetter> Flora(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFlora, IFloraGetter>(
                () => listings.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFlora, IFloraGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IFlora, IFloraGetter> Flora(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFlora, IFloraGetter>(
                () => mods.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFlora, IFloraGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IFootstep, IFootstepGetter> Footstep(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFootstep, IFootstepGetter>(
                () => listings.WinningOverrides<IFootstepGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFootstep, IFootstepGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IFootstep, IFootstepGetter> Footstep(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFootstep, IFootstepGetter>(
                () => mods.WinningOverrides<IFootstepGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFootstep, IFootstepGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IFootstepSet, IFootstepSetGetter> FootstepSet(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFootstepSet, IFootstepSetGetter>(
                () => listings.WinningOverrides<IFootstepSetGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFootstepSet, IFootstepSetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IFootstepSet, IFootstepSetGetter> FootstepSet(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFootstepSet, IFootstepSetGetter>(
                () => mods.WinningOverrides<IFootstepSetGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFootstepSet, IFootstepSetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IFormList, IFormListGetter> FormList(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFormList, IFormListGetter>(
                () => listings.WinningOverrides<IFormListGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFormList, IFormListGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IFormList, IFormListGetter> FormList(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFormList, IFormListGetter>(
                () => mods.WinningOverrides<IFormListGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFormList, IFormListGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IFurniture, IFurnitureGetter> Furniture(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFurniture, IFurnitureGetter>(
                () => listings.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFurniture, IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IFurniture, IFurnitureGetter> Furniture(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IFurniture, IFurnitureGetter>(
                () => mods.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IFurniture, IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IGameSetting, IGameSettingGetter> GameSetting(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGameSetting, IGameSettingGetter>(
                () => listings.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSetting, IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IGameSetting, IGameSettingGetter> GameSetting(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGameSetting, IGameSettingGetter>(
                () => mods.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSetting, IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IGameSettingBool, IGameSettingBoolGetter> GameSettingBool(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGameSettingBool, IGameSettingBoolGetter>(
                () => listings.WinningOverrides<IGameSettingBoolGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSettingBool, IGameSettingBoolGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IGameSettingBool, IGameSettingBoolGetter> GameSettingBool(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGameSettingBool, IGameSettingBoolGetter>(
                () => mods.WinningOverrides<IGameSettingBoolGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSettingBool, IGameSettingBoolGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IGameSettingFloat, IGameSettingFloatGetter> GameSettingFloat(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGameSettingFloat, IGameSettingFloatGetter>(
                () => listings.WinningOverrides<IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSettingFloat, IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IGameSettingFloat, IGameSettingFloatGetter> GameSettingFloat(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGameSettingFloat, IGameSettingFloatGetter>(
                () => mods.WinningOverrides<IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSettingFloat, IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IGameSettingInt, IGameSettingIntGetter> GameSettingInt(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGameSettingInt, IGameSettingIntGetter>(
                () => listings.WinningOverrides<IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSettingInt, IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IGameSettingInt, IGameSettingIntGetter> GameSettingInt(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGameSettingInt, IGameSettingIntGetter>(
                () => mods.WinningOverrides<IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSettingInt, IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IGameSettingString, IGameSettingStringGetter> GameSettingString(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGameSettingString, IGameSettingStringGetter>(
                () => listings.WinningOverrides<IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSettingString, IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IGameSettingString, IGameSettingStringGetter> GameSettingString(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGameSettingString, IGameSettingStringGetter>(
                () => mods.WinningOverrides<IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGameSettingString, IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IGlobal, IGlobalGetter> Global(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGlobal, IGlobalGetter>(
                () => listings.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGlobal, IGlobalGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IGlobal, IGlobalGetter> Global(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGlobal, IGlobalGetter>(
                () => mods.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGlobal, IGlobalGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IGlobalFloat, IGlobalFloatGetter> GlobalFloat(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGlobalFloat, IGlobalFloatGetter>(
                () => listings.WinningOverrides<IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGlobalFloat, IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IGlobalFloat, IGlobalFloatGetter> GlobalFloat(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGlobalFloat, IGlobalFloatGetter>(
                () => mods.WinningOverrides<IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGlobalFloat, IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IGlobalInt, IGlobalIntGetter> GlobalInt(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGlobalInt, IGlobalIntGetter>(
                () => listings.WinningOverrides<IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGlobalInt, IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IGlobalInt, IGlobalIntGetter> GlobalInt(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGlobalInt, IGlobalIntGetter>(
                () => mods.WinningOverrides<IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGlobalInt, IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IGlobalShort, IGlobalShortGetter> GlobalShort(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGlobalShort, IGlobalShortGetter>(
                () => listings.WinningOverrides<IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGlobalShort, IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IGlobalShort, IGlobalShortGetter> GlobalShort(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGlobalShort, IGlobalShortGetter>(
                () => mods.WinningOverrides<IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGlobalShort, IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IGrass, IGrassGetter> Grass(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGrass, IGrassGetter>(
                () => listings.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGrass, IGrassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IGrass, IGrassGetter> Grass(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IGrass, IGrassGetter>(
                () => mods.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IGrass, IGrassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IHair, IHairGetter> Hair(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IHair, IHairGetter>(
                () => listings.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHair, IHairGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IHair, IHairGetter> Hair(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IHair, IHairGetter>(
                () => mods.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHair, IHairGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IHazard, IHazardGetter> Hazard(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IHazard, IHazardGetter>(
                () => listings.WinningOverrides<IHazardGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHazard, IHazardGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IHazard, IHazardGetter> Hazard(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IHazard, IHazardGetter>(
                () => mods.WinningOverrides<IHazardGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHazard, IHazardGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IHeadPart, IHeadPartGetter> HeadPart(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IHeadPart, IHeadPartGetter>(
                () => listings.WinningOverrides<IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHeadPart, IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IHeadPart, IHeadPartGetter> HeadPart(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IHeadPart, IHeadPartGetter>(
                () => mods.WinningOverrides<IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHeadPart, IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IIdleAnimation, IIdleAnimationGetter> IdleAnimation(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IIdleAnimation, IIdleAnimationGetter>(
                () => listings.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIdleAnimation, IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IIdleAnimation, IIdleAnimationGetter> IdleAnimation(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IIdleAnimation, IIdleAnimationGetter>(
                () => mods.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIdleAnimation, IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IIdleMarker, IIdleMarkerGetter> IdleMarker(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IIdleMarker, IIdleMarkerGetter>(
                () => listings.WinningOverrides<IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIdleMarker, IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IIdleMarker, IIdleMarkerGetter> IdleMarker(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IIdleMarker, IIdleMarkerGetter>(
                () => mods.WinningOverrides<IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIdleMarker, IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IImageSpace, IImageSpaceGetter> ImageSpace(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IImageSpace, IImageSpaceGetter>(
                () => listings.WinningOverrides<IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImageSpace, IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IImageSpace, IImageSpaceGetter> ImageSpace(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IImageSpace, IImageSpaceGetter>(
                () => mods.WinningOverrides<IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImageSpace, IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IImageSpaceAdapter, IImageSpaceAdapterGetter> ImageSpaceAdapter(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IImageSpaceAdapter, IImageSpaceAdapterGetter>(
                () => listings.WinningOverrides<IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IImageSpaceAdapter, IImageSpaceAdapterGetter> ImageSpaceAdapter(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IImageSpaceAdapter, IImageSpaceAdapterGetter>(
                () => mods.WinningOverrides<IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IImpact, IImpactGetter> Impact(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IImpact, IImpactGetter>(
                () => listings.WinningOverrides<IImpactGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImpact, IImpactGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IImpact, IImpactGetter> Impact(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IImpact, IImpactGetter>(
                () => mods.WinningOverrides<IImpactGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImpact, IImpactGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IImpactDataSet, IImpactDataSetGetter> ImpactDataSet(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IImpactDataSet, IImpactDataSetGetter>(
                () => listings.WinningOverrides<IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImpactDataSet, IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IImpactDataSet, IImpactDataSetGetter> ImpactDataSet(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IImpactDataSet, IImpactDataSetGetter>(
                () => mods.WinningOverrides<IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IImpactDataSet, IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IIngestible, IIngestibleGetter> Ingestible(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IIngestible, IIngestibleGetter>(
                () => listings.WinningOverrides<IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIngestible, IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IIngestible, IIngestibleGetter> Ingestible(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IIngestible, IIngestibleGetter>(
                () => mods.WinningOverrides<IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIngestible, IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IIngredient, IIngredientGetter> Ingredient(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IIngredient, IIngredientGetter>(
                () => listings.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIngredient, IIngredientGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IIngredient, IIngredientGetter> Ingredient(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IIngredient, IIngredientGetter>(
                () => mods.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIngredient, IIngredientGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IKey, IKeyGetter> Key(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IKey, IKeyGetter>(
                () => listings.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IKey, IKeyGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IKey, IKeyGetter> Key(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IKey, IKeyGetter>(
                () => mods.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IKey, IKeyGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IKeyword, IKeywordGetter> Keyword(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IKeyword, IKeywordGetter>(
                () => listings.WinningOverrides<IKeywordGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IKeyword, IKeywordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IKeyword, IKeywordGetter> Keyword(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IKeyword, IKeywordGetter>(
                () => mods.WinningOverrides<IKeywordGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IKeyword, IKeywordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILandscape, ILandscapeGetter> Landscape(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILandscape, ILandscapeGetter>(
                () => listings.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILandscape, ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILandscape, ILandscapeGetter> Landscape(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILandscape, ILandscapeGetter>(
                () => mods.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILandscape, ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILandscapeTexture, ILandscapeTextureGetter> LandscapeTexture(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILandscapeTexture, ILandscapeTextureGetter>(
                () => listings.WinningOverrides<ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILandscapeTexture, ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILandscapeTexture, ILandscapeTextureGetter> LandscapeTexture(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILandscapeTexture, ILandscapeTextureGetter>(
                () => mods.WinningOverrides<ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILandscapeTexture, ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILensFlare, ILensFlareGetter> LensFlare(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILensFlare, ILensFlareGetter>(
                () => listings.WinningOverrides<ILensFlareGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILensFlare, ILensFlareGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILensFlare, ILensFlareGetter> LensFlare(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILensFlare, ILensFlareGetter>(
                () => mods.WinningOverrides<ILensFlareGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILensFlare, ILensFlareGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILeveledItem, ILeveledItemGetter> LeveledItem(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILeveledItem, ILeveledItemGetter>(
                () => listings.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILeveledItem, ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILeveledItem, ILeveledItemGetter> LeveledItem(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILeveledItem, ILeveledItemGetter>(
                () => mods.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILeveledItem, ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILeveledNpc, ILeveledNpcGetter> LeveledNpc(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILeveledNpc, ILeveledNpcGetter>(
                () => listings.WinningOverrides<ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILeveledNpc, ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILeveledNpc, ILeveledNpcGetter> LeveledNpc(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILeveledNpc, ILeveledNpcGetter>(
                () => mods.WinningOverrides<ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILeveledNpc, ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILeveledSpell, ILeveledSpellGetter> LeveledSpell(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILeveledSpell, ILeveledSpellGetter>(
                () => listings.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILeveledSpell, ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILeveledSpell, ILeveledSpellGetter> LeveledSpell(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILeveledSpell, ILeveledSpellGetter>(
                () => mods.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILeveledSpell, ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILight, ILightGetter> Light(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILight, ILightGetter>(
                () => listings.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILight, ILightGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILight, ILightGetter> Light(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILight, ILightGetter>(
                () => mods.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILight, ILightGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILightingTemplate, ILightingTemplateGetter> LightingTemplate(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILightingTemplate, ILightingTemplateGetter>(
                () => listings.WinningOverrides<ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILightingTemplate, ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILightingTemplate, ILightingTemplateGetter> LightingTemplate(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILightingTemplate, ILightingTemplateGetter>(
                () => mods.WinningOverrides<ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILightingTemplate, ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILoadScreen, ILoadScreenGetter> LoadScreen(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILoadScreen, ILoadScreenGetter>(
                () => listings.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILoadScreen, ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILoadScreen, ILoadScreenGetter> LoadScreen(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILoadScreen, ILoadScreenGetter>(
                () => mods.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILoadScreen, ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILocation, ILocationGetter> Location(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILocation, ILocationGetter>(
                () => listings.WinningOverrides<ILocationGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocation, ILocationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILocation, ILocationGetter> Location(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILocation, ILocationGetter>(
                () => mods.WinningOverrides<ILocationGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocation, ILocationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILocationReferenceType, ILocationReferenceTypeGetter> LocationReferenceType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILocationReferenceType, ILocationReferenceTypeGetter>(
                () => listings.WinningOverrides<ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILocationReferenceType, ILocationReferenceTypeGetter> LocationReferenceType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILocationReferenceType, ILocationReferenceTypeGetter>(
                () => mods.WinningOverrides<ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IMagicEffect, IMagicEffectGetter> MagicEffect(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMagicEffect, IMagicEffectGetter>(
                () => listings.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMagicEffect, IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IMagicEffect, IMagicEffectGetter> MagicEffect(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMagicEffect, IMagicEffectGetter>(
                () => mods.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMagicEffect, IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IMaterialObject, IMaterialObjectGetter> MaterialObject(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMaterialObject, IMaterialObjectGetter>(
                () => listings.WinningOverrides<IMaterialObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMaterialObject, IMaterialObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IMaterialObject, IMaterialObjectGetter> MaterialObject(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMaterialObject, IMaterialObjectGetter>(
                () => mods.WinningOverrides<IMaterialObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMaterialObject, IMaterialObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IMaterialType, IMaterialTypeGetter> MaterialType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMaterialType, IMaterialTypeGetter>(
                () => listings.WinningOverrides<IMaterialTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMaterialType, IMaterialTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IMaterialType, IMaterialTypeGetter> MaterialType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMaterialType, IMaterialTypeGetter>(
                () => mods.WinningOverrides<IMaterialTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMaterialType, IMaterialTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IMessage, IMessageGetter> Message(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMessage, IMessageGetter>(
                () => listings.WinningOverrides<IMessageGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMessage, IMessageGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IMessage, IMessageGetter> Message(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMessage, IMessageGetter>(
                () => mods.WinningOverrides<IMessageGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMessage, IMessageGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IMiscItem, IMiscItemGetter> MiscItem(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMiscItem, IMiscItemGetter>(
                () => listings.WinningOverrides<IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMiscItem, IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IMiscItem, IMiscItemGetter> MiscItem(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMiscItem, IMiscItemGetter>(
                () => mods.WinningOverrides<IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMiscItem, IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IMoveableStatic, IMoveableStaticGetter> MoveableStatic(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMoveableStatic, IMoveableStaticGetter>(
                () => listings.WinningOverrides<IMoveableStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMoveableStatic, IMoveableStaticGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IMoveableStatic, IMoveableStaticGetter> MoveableStatic(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMoveableStatic, IMoveableStaticGetter>(
                () => mods.WinningOverrides<IMoveableStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMoveableStatic, IMoveableStaticGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IMovementType, IMovementTypeGetter> MovementType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMovementType, IMovementTypeGetter>(
                () => listings.WinningOverrides<IMovementTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMovementType, IMovementTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IMovementType, IMovementTypeGetter> MovementType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMovementType, IMovementTypeGetter>(
                () => mods.WinningOverrides<IMovementTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMovementType, IMovementTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IMusicTrack, IMusicTrackGetter> MusicTrack(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMusicTrack, IMusicTrackGetter>(
                () => listings.WinningOverrides<IMusicTrackGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMusicTrack, IMusicTrackGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IMusicTrack, IMusicTrackGetter> MusicTrack(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMusicTrack, IMusicTrackGetter>(
                () => mods.WinningOverrides<IMusicTrackGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMusicTrack, IMusicTrackGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IMusicType, IMusicTypeGetter> MusicType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMusicType, IMusicTypeGetter>(
                () => listings.WinningOverrides<IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMusicType, IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IMusicType, IMusicTypeGetter> MusicType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IMusicType, IMusicTypeGetter>(
                () => mods.WinningOverrides<IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IMusicType, IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, INavigationMeshInfoMap, INavigationMeshInfoMapGetter> NavigationMeshInfoMap(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(
                () => listings.WinningOverrides<INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, INavigationMeshInfoMap, INavigationMeshInfoMapGetter> NavigationMeshInfoMap(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(
                () => mods.WinningOverrides<INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, INpc, INpcGetter> Npc(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, INpc, INpcGetter>(
                () => listings.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, INpc, INpcGetter> Npc(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, INpc, INpcGetter>(
                () => mods.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IObjectEffect, IObjectEffectGetter> ObjectEffect(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IObjectEffect, IObjectEffectGetter>(
                () => listings.WinningOverrides<IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IObjectEffect, IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IObjectEffect, IObjectEffectGetter> ObjectEffect(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IObjectEffect, IObjectEffectGetter>(
                () => mods.WinningOverrides<IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IObjectEffect, IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IOutfit, IOutfitGetter> Outfit(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IOutfit, IOutfitGetter>(
                () => listings.WinningOverrides<IOutfitGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IOutfit, IOutfitGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IOutfit, IOutfitGetter> Outfit(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IOutfit, IOutfitGetter>(
                () => mods.WinningOverrides<IOutfitGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IOutfit, IOutfitGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPackage, IPackageGetter> Package(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPackage, IPackageGetter>(
                () => listings.WinningOverrides<IPackageGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPackage, IPackageGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPackage, IPackageGetter> Package(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPackage, IPackageGetter>(
                () => mods.WinningOverrides<IPackageGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPackage, IPackageGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPerk, IPerkGetter> Perk(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPerk, IPerkGetter>(
                () => listings.WinningOverrides<IPerkGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPerk, IPerkGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPerk, IPerkGetter> Perk(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPerk, IPerkGetter>(
                () => mods.WinningOverrides<IPerkGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPerk, IPerkGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedArrow, IPlacedArrowGetter> PlacedArrow(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedArrow, IPlacedArrowGetter>(
                () => listings.WinningOverrides<IPlacedArrowGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedArrow, IPlacedArrowGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedArrow, IPlacedArrowGetter> PlacedArrow(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedArrow, IPlacedArrowGetter>(
                () => mods.WinningOverrides<IPlacedArrowGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedArrow, IPlacedArrowGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedBarrier, IPlacedBarrierGetter> PlacedBarrier(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedBarrier, IPlacedBarrierGetter>(
                () => listings.WinningOverrides<IPlacedBarrierGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedBarrier, IPlacedBarrierGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedBarrier, IPlacedBarrierGetter> PlacedBarrier(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedBarrier, IPlacedBarrierGetter>(
                () => mods.WinningOverrides<IPlacedBarrierGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedBarrier, IPlacedBarrierGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedBeam, IPlacedBeamGetter> PlacedBeam(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedBeam, IPlacedBeamGetter>(
                () => listings.WinningOverrides<IPlacedBeamGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedBeam, IPlacedBeamGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedBeam, IPlacedBeamGetter> PlacedBeam(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedBeam, IPlacedBeamGetter>(
                () => mods.WinningOverrides<IPlacedBeamGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedBeam, IPlacedBeamGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedCone, IPlacedConeGetter> PlacedCone(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedCone, IPlacedConeGetter>(
                () => listings.WinningOverrides<IPlacedConeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedCone, IPlacedConeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedCone, IPlacedConeGetter> PlacedCone(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedCone, IPlacedConeGetter>(
                () => mods.WinningOverrides<IPlacedConeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedCone, IPlacedConeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedFlame, IPlacedFlameGetter> PlacedFlame(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedFlame, IPlacedFlameGetter>(
                () => listings.WinningOverrides<IPlacedFlameGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedFlame, IPlacedFlameGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedFlame, IPlacedFlameGetter> PlacedFlame(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedFlame, IPlacedFlameGetter>(
                () => mods.WinningOverrides<IPlacedFlameGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedFlame, IPlacedFlameGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedHazard, IPlacedHazardGetter> PlacedHazard(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedHazard, IPlacedHazardGetter>(
                () => listings.WinningOverrides<IPlacedHazardGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedHazard, IPlacedHazardGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedHazard, IPlacedHazardGetter> PlacedHazard(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedHazard, IPlacedHazardGetter>(
                () => mods.WinningOverrides<IPlacedHazardGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedHazard, IPlacedHazardGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedMissile, IPlacedMissileGetter> PlacedMissile(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedMissile, IPlacedMissileGetter>(
                () => listings.WinningOverrides<IPlacedMissileGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedMissile, IPlacedMissileGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedMissile, IPlacedMissileGetter> PlacedMissile(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedMissile, IPlacedMissileGetter>(
                () => mods.WinningOverrides<IPlacedMissileGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedMissile, IPlacedMissileGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedNpc, IPlacedNpcGetter> PlacedNpc(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedNpc, IPlacedNpcGetter>(
                () => listings.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedNpc, IPlacedNpcGetter> PlacedNpc(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedNpc, IPlacedNpcGetter>(
                () => mods.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedObject, IPlacedObjectGetter> PlacedObject(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedObject, IPlacedObjectGetter>(
                () => listings.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedObject, IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedObject, IPlacedObjectGetter> PlacedObject(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedObject, IPlacedObjectGetter>(
                () => mods.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedObject, IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedTrap, IPlacedTrapGetter> PlacedTrap(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedTrap, IPlacedTrapGetter>(
                () => listings.WinningOverrides<IPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedTrap, IPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedTrap, IPlacedTrapGetter> PlacedTrap(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedTrap, IPlacedTrapGetter>(
                () => mods.WinningOverrides<IPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedTrap, IPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IProjectile, IProjectileGetter> Projectile(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IProjectile, IProjectileGetter>(
                () => listings.WinningOverrides<IProjectileGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IProjectile, IProjectileGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IProjectile, IProjectileGetter> Projectile(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IProjectile, IProjectileGetter>(
                () => mods.WinningOverrides<IProjectileGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IProjectile, IProjectileGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IQuest, IQuestGetter> Quest(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IQuest, IQuestGetter>(
                () => listings.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IQuest, IQuestGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IQuest, IQuestGetter> Quest(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IQuest, IQuestGetter>(
                () => mods.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IQuest, IQuestGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IRace, IRaceGetter> Race(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IRace, IRaceGetter>(
                () => listings.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRace, IRaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IRace, IRaceGetter> Race(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IRace, IRaceGetter>(
                () => mods.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRace, IRaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IRegion, IRegionGetter> Region(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IRegion, IRegionGetter>(
                () => listings.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRegion, IRegionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IRegion, IRegionGetter> Region(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IRegion, IRegionGetter>(
                () => mods.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRegion, IRegionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IRelationship, IRelationshipGetter> Relationship(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IRelationship, IRelationshipGetter>(
                () => listings.WinningOverrides<IRelationshipGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRelationship, IRelationshipGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IRelationship, IRelationshipGetter> Relationship(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IRelationship, IRelationshipGetter>(
                () => mods.WinningOverrides<IRelationshipGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRelationship, IRelationshipGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IReverbParameters, IReverbParametersGetter> ReverbParameters(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IReverbParameters, IReverbParametersGetter>(
                () => listings.WinningOverrides<IReverbParametersGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IReverbParameters, IReverbParametersGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IReverbParameters, IReverbParametersGetter> ReverbParameters(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IReverbParameters, IReverbParametersGetter>(
                () => mods.WinningOverrides<IReverbParametersGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IReverbParameters, IReverbParametersGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IScene, ISceneGetter> Scene(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IScene, ISceneGetter>(
                () => listings.WinningOverrides<ISceneGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IScene, ISceneGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IScene, ISceneGetter> Scene(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IScene, ISceneGetter>(
                () => mods.WinningOverrides<ISceneGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IScene, ISceneGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IScroll, IScrollGetter> Scroll(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IScroll, IScrollGetter>(
                () => listings.WinningOverrides<IScrollGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IScroll, IScrollGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IScroll, IScrollGetter> Scroll(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IScroll, IScrollGetter>(
                () => mods.WinningOverrides<IScrollGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IScroll, IScrollGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IShaderParticleGeometry, IShaderParticleGeometryGetter> ShaderParticleGeometry(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IShaderParticleGeometry, IShaderParticleGeometryGetter>(
                () => listings.WinningOverrides<IShaderParticleGeometryGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IShaderParticleGeometry, IShaderParticleGeometryGetter> ShaderParticleGeometry(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IShaderParticleGeometry, IShaderParticleGeometryGetter>(
                () => mods.WinningOverrides<IShaderParticleGeometryGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IShout, IShoutGetter> Shout(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IShout, IShoutGetter>(
                () => listings.WinningOverrides<IShoutGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IShout, IShoutGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IShout, IShoutGetter> Shout(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IShout, IShoutGetter>(
                () => mods.WinningOverrides<IShoutGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IShout, IShoutGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> SkyrimMajorRecord(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(
                () => listings.WinningOverrides<ISkyrimMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> SkyrimMajorRecord(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(
                () => mods.WinningOverrides<ISkyrimMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ISoulGem, ISoulGemGetter> SoulGem(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISoulGem, ISoulGemGetter>(
                () => listings.WinningOverrides<ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoulGem, ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ISoulGem, ISoulGemGetter> SoulGem(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISoulGem, ISoulGemGetter>(
                () => mods.WinningOverrides<ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoulGem, ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ISoundCategory, ISoundCategoryGetter> SoundCategory(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISoundCategory, ISoundCategoryGetter>(
                () => listings.WinningOverrides<ISoundCategoryGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundCategory, ISoundCategoryGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ISoundCategory, ISoundCategoryGetter> SoundCategory(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISoundCategory, ISoundCategoryGetter>(
                () => mods.WinningOverrides<ISoundCategoryGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundCategory, ISoundCategoryGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ISoundDescriptor, ISoundDescriptorGetter> SoundDescriptor(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISoundDescriptor, ISoundDescriptorGetter>(
                () => listings.WinningOverrides<ISoundDescriptorGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundDescriptor, ISoundDescriptorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ISoundDescriptor, ISoundDescriptorGetter> SoundDescriptor(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISoundDescriptor, ISoundDescriptorGetter>(
                () => mods.WinningOverrides<ISoundDescriptorGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundDescriptor, ISoundDescriptorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ISoundMarker, ISoundMarkerGetter> SoundMarker(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISoundMarker, ISoundMarkerGetter>(
                () => listings.WinningOverrides<ISoundMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundMarker, ISoundMarkerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ISoundMarker, ISoundMarkerGetter> SoundMarker(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISoundMarker, ISoundMarkerGetter>(
                () => mods.WinningOverrides<ISoundMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundMarker, ISoundMarkerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ISoundOutputModel, ISoundOutputModelGetter> SoundOutputModel(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISoundOutputModel, ISoundOutputModelGetter>(
                () => listings.WinningOverrides<ISoundOutputModelGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundOutputModel, ISoundOutputModelGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ISoundOutputModel, ISoundOutputModelGetter> SoundOutputModel(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISoundOutputModel, ISoundOutputModelGetter>(
                () => mods.WinningOverrides<ISoundOutputModelGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISoundOutputModel, ISoundOutputModelGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ISpell, ISpellGetter> Spell(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISpell, ISpellGetter>(
                () => listings.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISpell, ISpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ISpell, ISpellGetter> Spell(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISpell, ISpellGetter>(
                () => mods.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISpell, ISpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IStatic, IStaticGetter> Static(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IStatic, IStaticGetter>(
                () => listings.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IStatic, IStaticGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IStatic, IStaticGetter> Static(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IStatic, IStaticGetter>(
                () => mods.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IStatic, IStaticGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IStoryManagerBranchNode, IStoryManagerBranchNodeGetter> StoryManagerBranchNode(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IStoryManagerBranchNode, IStoryManagerBranchNodeGetter>(
                () => listings.WinningOverrides<IStoryManagerBranchNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IStoryManagerBranchNode, IStoryManagerBranchNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IStoryManagerBranchNode, IStoryManagerBranchNodeGetter> StoryManagerBranchNode(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IStoryManagerBranchNode, IStoryManagerBranchNodeGetter>(
                () => mods.WinningOverrides<IStoryManagerBranchNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IStoryManagerBranchNode, IStoryManagerBranchNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IStoryManagerEventNode, IStoryManagerEventNodeGetter> StoryManagerEventNode(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IStoryManagerEventNode, IStoryManagerEventNodeGetter>(
                () => listings.WinningOverrides<IStoryManagerEventNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IStoryManagerEventNode, IStoryManagerEventNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IStoryManagerEventNode, IStoryManagerEventNodeGetter> StoryManagerEventNode(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IStoryManagerEventNode, IStoryManagerEventNodeGetter>(
                () => mods.WinningOverrides<IStoryManagerEventNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IStoryManagerEventNode, IStoryManagerEventNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter> StoryManagerQuestNode(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>(
                () => listings.WinningOverrides<IStoryManagerQuestNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter> StoryManagerQuestNode(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>(
                () => mods.WinningOverrides<IStoryManagerQuestNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ITalkingActivator, ITalkingActivatorGetter> TalkingActivator(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ITalkingActivator, ITalkingActivatorGetter>(
                () => listings.WinningOverrides<ITalkingActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ITalkingActivator, ITalkingActivatorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ITalkingActivator, ITalkingActivatorGetter> TalkingActivator(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ITalkingActivator, ITalkingActivatorGetter>(
                () => mods.WinningOverrides<ITalkingActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ITalkingActivator, ITalkingActivatorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ITextureSet, ITextureSetGetter> TextureSet(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ITextureSet, ITextureSetGetter>(
                () => listings.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ITextureSet, ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ITextureSet, ITextureSetGetter> TextureSet(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ITextureSet, ITextureSetGetter>(
                () => mods.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ITextureSet, ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ITree, ITreeGetter> Tree(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ITree, ITreeGetter>(
                () => listings.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ITree, ITreeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ITree, ITreeGetter> Tree(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ITree, ITreeGetter>(
                () => mods.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ITree, ITreeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IVisualEffect, IVisualEffectGetter> VisualEffect(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IVisualEffect, IVisualEffectGetter>(
                () => listings.WinningOverrides<IVisualEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IVisualEffect, IVisualEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IVisualEffect, IVisualEffectGetter> VisualEffect(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IVisualEffect, IVisualEffectGetter>(
                () => mods.WinningOverrides<IVisualEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IVisualEffect, IVisualEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IVoiceType, IVoiceTypeGetter> VoiceType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IVoiceType, IVoiceTypeGetter>(
                () => listings.WinningOverrides<IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IVoiceType, IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IVoiceType, IVoiceTypeGetter> VoiceType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IVoiceType, IVoiceTypeGetter>(
                () => mods.WinningOverrides<IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IVoiceType, IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IVolumetricLighting, IVolumetricLightingGetter> VolumetricLighting(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IVolumetricLighting, IVolumetricLightingGetter>(
                () => listings.WinningOverrides<IVolumetricLightingGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IVolumetricLighting, IVolumetricLightingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IVolumetricLighting, IVolumetricLightingGetter> VolumetricLighting(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IVolumetricLighting, IVolumetricLightingGetter>(
                () => mods.WinningOverrides<IVolumetricLightingGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IVolumetricLighting, IVolumetricLightingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IWater, IWaterGetter> Water(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWater, IWaterGetter>(
                () => listings.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWater, IWaterGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IWater, IWaterGetter> Water(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWater, IWaterGetter>(
                () => mods.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWater, IWaterGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IWeapon, IWeaponGetter> Weapon(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWeapon, IWeaponGetter>(
                () => listings.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWeapon, IWeaponGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IWeapon, IWeaponGetter> Weapon(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWeapon, IWeaponGetter>(
                () => mods.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWeapon, IWeaponGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IWeather, IWeatherGetter> Weather(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWeather, IWeatherGetter>(
                () => listings.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWeather, IWeatherGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IWeather, IWeatherGetter> Weather(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWeather, IWeatherGetter>(
                () => mods.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWeather, IWeatherGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IWordOfPower, IWordOfPowerGetter> WordOfPower(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWordOfPower, IWordOfPowerGetter>(
                () => listings.WinningOverrides<IWordOfPowerGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWordOfPower, IWordOfPowerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IWordOfPower, IWordOfPowerGetter> WordOfPower(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWordOfPower, IWordOfPowerGetter>(
                () => mods.WinningOverrides<IWordOfPowerGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWordOfPower, IWordOfPowerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IWorldspace, IWorldspaceGetter> Worldspace(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWorldspace, IWorldspaceGetter>(
                () => listings.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IWorldspace, IWorldspaceGetter> Worldspace(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWorldspace, IWorldspaceGetter>(
                () => mods.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IWorldspaceNavigationMesh, IWorldspaceNavigationMeshGetter> WorldspaceNavigationMesh(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWorldspaceNavigationMesh, IWorldspaceNavigationMeshGetter>(
                () => listings.WinningOverrides<IWorldspaceNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWorldspaceNavigationMesh, IWorldspaceNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IWorldspaceNavigationMesh, IWorldspaceNavigationMeshGetter> WorldspaceNavigationMesh(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IWorldspaceNavigationMesh, IWorldspaceNavigationMeshGetter>(
                () => mods.WinningOverrides<IWorldspaceNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IWorldspaceNavigationMesh, IWorldspaceNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

        #region Link Interfaces
        public static TypedLoadOrderAccess<ISkyrimMod, IIdleRelation, IIdleRelationGetter> IIdleRelation(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IIdleRelation, IIdleRelationGetter>(
                () => listings.WinningOverrides<IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIdleRelation, IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IIdleRelation, IIdleRelationGetter> IIdleRelation(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IIdleRelation, IIdleRelationGetter>(
                () => mods.WinningOverrides<IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IIdleRelation, IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IObjectId, IObjectIdGetter> IObjectId(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IObjectId, IObjectIdGetter>(
                () => listings.WinningOverrides<IObjectIdGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IObjectId, IObjectIdGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IObjectId, IObjectIdGetter> IObjectId(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IObjectId, IObjectIdGetter>(
                () => mods.WinningOverrides<IObjectIdGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IObjectId, IObjectIdGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IItem, IItemGetter> IItem(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IItem, IItemGetter>(
                () => listings.WinningOverrides<IItemGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IItem, IItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IItem, IItemGetter> IItem(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IItem, IItemGetter>(
                () => mods.WinningOverrides<IItemGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IItem, IItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IOutfitTarget, IOutfitTargetGetter> IOutfitTarget(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IOutfitTarget, IOutfitTargetGetter>(
                () => listings.WinningOverrides<IOutfitTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IOutfitTarget, IOutfitTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IOutfitTarget, IOutfitTargetGetter> IOutfitTarget(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IOutfitTarget, IOutfitTargetGetter>(
                () => mods.WinningOverrides<IOutfitTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IOutfitTarget, IOutfitTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IComplexLocation, IComplexLocationGetter> IComplexLocation(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IComplexLocation, IComplexLocationGetter>(
                () => listings.WinningOverrides<IComplexLocationGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IComplexLocation, IComplexLocationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IComplexLocation, IComplexLocationGetter> IComplexLocation(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IComplexLocation, IComplexLocationGetter>(
                () => mods.WinningOverrides<IComplexLocationGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IComplexLocation, IComplexLocationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IDialog, IDialogGetter> IDialog(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDialog, IDialogGetter>(
                () => listings.WinningOverrides<IDialogGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialog, IDialogGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IDialog, IDialogGetter> IDialog(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IDialog, IDialogGetter>(
                () => mods.WinningOverrides<IDialogGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IDialog, IDialogGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILocationTargetable, ILocationTargetableGetter> ILocationTargetable(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILocationTargetable, ILocationTargetableGetter>(
                () => listings.WinningOverrides<ILocationTargetableGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocationTargetable, ILocationTargetableGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILocationTargetable, ILocationTargetableGetter> ILocationTargetable(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILocationTargetable, ILocationTargetableGetter>(
                () => mods.WinningOverrides<ILocationTargetableGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocationTargetable, ILocationTargetableGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IOwner, IOwnerGetter> IOwner(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IOwner, IOwnerGetter>(
                () => listings.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IOwner, IOwnerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IOwner, IOwnerGetter> IOwner(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IOwner, IOwnerGetter>(
                () => mods.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IOwner, IOwnerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IRelatable, IRelatableGetter> IRelatable(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IRelatable, IRelatableGetter>(
                () => listings.WinningOverrides<IRelatableGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRelatable, IRelatableGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IRelatable, IRelatableGetter> IRelatable(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IRelatable, IRelatableGetter>(
                () => mods.WinningOverrides<IRelatableGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRelatable, IRelatableGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IRegionTarget, IRegionTargetGetter> IRegionTarget(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IRegionTarget, IRegionTargetGetter>(
                () => listings.WinningOverrides<IRegionTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRegionTarget, IRegionTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IRegionTarget, IRegionTargetGetter> IRegionTarget(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IRegionTarget, IRegionTargetGetter>(
                () => mods.WinningOverrides<IRegionTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IRegionTarget, IRegionTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IAliasVoiceType, IAliasVoiceTypeGetter> IAliasVoiceType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAliasVoiceType, IAliasVoiceTypeGetter>(
                () => listings.WinningOverrides<IAliasVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAliasVoiceType, IAliasVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IAliasVoiceType, IAliasVoiceTypeGetter> IAliasVoiceType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IAliasVoiceType, IAliasVoiceTypeGetter>(
                () => mods.WinningOverrides<IAliasVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IAliasVoiceType, IAliasVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILockList, ILockListGetter> ILockList(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILockList, ILockListGetter>(
                () => listings.WinningOverrides<ILockListGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILockList, ILockListGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILockList, ILockListGetter> ILockList(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILockList, ILockListGetter>(
                () => mods.WinningOverrides<ILockListGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILockList, ILockListGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedTrapTarget, IPlacedTrapTargetGetter> IPlacedTrapTarget(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedTrapTarget, IPlacedTrapTargetGetter>(
                () => listings.WinningOverrides<IPlacedTrapTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedTrapTarget, IPlacedTrapTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedTrapTarget, IPlacedTrapTargetGetter> IPlacedTrapTarget(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedTrapTarget, IPlacedTrapTargetGetter>(
                () => mods.WinningOverrides<IPlacedTrapTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedTrapTarget, IPlacedTrapTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IHarvestTarget, IHarvestTargetGetter> IHarvestTarget(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IHarvestTarget, IHarvestTargetGetter>(
                () => listings.WinningOverrides<IHarvestTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHarvestTarget, IHarvestTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IHarvestTarget, IHarvestTargetGetter> IHarvestTarget(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IHarvestTarget, IHarvestTargetGetter>(
                () => mods.WinningOverrides<IHarvestTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IHarvestTarget, IHarvestTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IKeywordLinkedReference, IKeywordLinkedReferenceGetter> IKeywordLinkedReference(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(
                () => listings.WinningOverrides<IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IKeywordLinkedReference, IKeywordLinkedReferenceGetter> IKeywordLinkedReference(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(
                () => mods.WinningOverrides<IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, INpcSpawn, INpcSpawnGetter> INpcSpawn(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, INpcSpawn, INpcSpawnGetter>(
                () => listings.WinningOverrides<INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INpcSpawn, INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, INpcSpawn, INpcSpawnGetter> INpcSpawn(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, INpcSpawn, INpcSpawnGetter>(
                () => mods.WinningOverrides<INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, INpcSpawn, INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ISpellSpawn, ISpellSpawnGetter> ISpellSpawn(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISpellSpawn, ISpellSpawnGetter>(
                () => listings.WinningOverrides<ISpellSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISpellSpawn, ISpellSpawnGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ISpellSpawn, ISpellSpawnGetter> ISpellSpawn(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISpellSpawn, ISpellSpawnGetter>(
                () => mods.WinningOverrides<ISpellSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISpellSpawn, ISpellSpawnGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IEmittance, IEmittanceGetter> IEmittance(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEmittance, IEmittanceGetter>(
                () => listings.WinningOverrides<IEmittanceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEmittance, IEmittanceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IEmittance, IEmittanceGetter> IEmittance(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEmittance, IEmittanceGetter>(
                () => mods.WinningOverrides<IEmittanceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEmittance, IEmittanceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILocationRecord, ILocationRecordGetter> ILocationRecord(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILocationRecord, ILocationRecordGetter>(
                () => listings.WinningOverrides<ILocationRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocationRecord, ILocationRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILocationRecord, ILocationRecordGetter> ILocationRecord(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILocationRecord, ILocationRecordGetter>(
                () => mods.WinningOverrides<ILocationRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILocationRecord, ILocationRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IEffectRecord, IEffectRecordGetter> IEffectRecord(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEffectRecord, IEffectRecordGetter>(
                () => listings.WinningOverrides<IEffectRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEffectRecord, IEffectRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IEffectRecord, IEffectRecordGetter> IEffectRecord(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IEffectRecord, IEffectRecordGetter>(
                () => mods.WinningOverrides<IEffectRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IEffectRecord, IEffectRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILinkedReference, ILinkedReferenceGetter> ILinkedReference(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILinkedReference, ILinkedReferenceGetter>(
                () => listings.WinningOverrides<ILinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILinkedReference, ILinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ILinkedReference, ILinkedReferenceGetter> ILinkedReference(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ILinkedReference, ILinkedReferenceGetter>(
                () => mods.WinningOverrides<ILinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ILinkedReference, ILinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlaced, IPlacedGetter> IPlaced(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlaced, IPlacedGetter>(
                () => listings.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlaced, IPlacedGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlaced, IPlacedGetter> IPlaced(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlaced, IPlacedGetter>(
                () => mods.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlaced, IPlacedGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedSimple, IPlacedSimpleGetter> IPlacedSimple(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedSimple, IPlacedSimpleGetter>(
                () => listings.WinningOverrides<IPlacedSimpleGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedSimple, IPlacedSimpleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedSimple, IPlacedSimpleGetter> IPlacedSimple(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedSimple, IPlacedSimpleGetter>(
                () => mods.WinningOverrides<IPlacedSimpleGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedSimple, IPlacedSimpleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedThing, IPlacedThingGetter> IPlacedThing(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedThing, IPlacedThingGetter>(
                () => listings.WinningOverrides<IPlacedThingGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedThing, IPlacedThingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, IPlacedThing, IPlacedThingGetter> IPlacedThing(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, IPlacedThing, IPlacedThingGetter>(
                () => mods.WinningOverrides<IPlacedThingGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedThing, IPlacedThingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ISound, ISoundGetter> ISound(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISound, ISoundGetter>(
                () => listings.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISound, ISoundGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMod, ISound, ISoundGetter> ISound(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMod, ISound, ISoundGetter>(
                () => mods.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, ISound, ISoundGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

    }
}
