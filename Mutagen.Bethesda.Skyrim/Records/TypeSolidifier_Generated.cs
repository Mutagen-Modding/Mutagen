using System.Collections.Generic;
namespace Mutagen.Bethesda.Skyrim
{
    public static class TypeOptionSolidifierMixIns
    {
        #region Normal
        public static TypedLoadOrderAccess<IAcousticSpaceGetter> AcousticSpace(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAcousticSpaceGetter>(() => listings.WinningOverrides<IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAcousticSpaceGetter> AcousticSpace(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAcousticSpaceGetter>(() => mods.WinningOverrides<IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IActionRecordGetter> ActionRecord(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IActionRecordGetter>(() => listings.WinningOverrides<IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IActionRecordGetter> ActionRecord(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IActionRecordGetter>(() => mods.WinningOverrides<IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IActivatorGetter> Activator(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IActivatorGetter>(() => listings.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IActivatorGetter> Activator(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IActivatorGetter>(() => mods.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IActorValueInformationGetter> ActorValueInformation(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IActorValueInformationGetter>(() => listings.WinningOverrides<IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IActorValueInformationGetter> ActorValueInformation(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IActorValueInformationGetter>(() => mods.WinningOverrides<IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAddonNodeGetter> AddonNode(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAddonNodeGetter>(() => listings.WinningOverrides<IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAddonNodeGetter> AddonNode(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAddonNodeGetter>(() => mods.WinningOverrides<IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAlchemicalApparatusGetter> AlchemicalApparatus(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAlchemicalApparatusGetter>(() => listings.WinningOverrides<IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAlchemicalApparatusGetter> AlchemicalApparatus(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAlchemicalApparatusGetter>(() => mods.WinningOverrides<IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAmmunitionGetter> Ammunition(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAmmunitionGetter>(() => listings.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAmmunitionGetter> Ammunition(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAmmunitionGetter>(() => mods.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IANavigationMeshGetter> ANavigationMesh(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IANavigationMeshGetter>(() => listings.WinningOverrides<IANavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IANavigationMeshGetter> ANavigationMesh(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IANavigationMeshGetter>(() => mods.WinningOverrides<IANavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAnimatedObjectGetter> AnimatedObject(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAnimatedObjectGetter>(() => listings.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAnimatedObjectGetter> AnimatedObject(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAnimatedObjectGetter>(() => mods.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAPlacedTrapGetter> APlacedTrap(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAPlacedTrapGetter>(() => listings.WinningOverrides<IAPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAPlacedTrapGetter> APlacedTrap(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAPlacedTrapGetter>(() => mods.WinningOverrides<IAPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IArmorGetter> Armor(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IArmorGetter>(() => listings.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IArmorGetter> Armor(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IArmorGetter>(() => mods.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IArmorAddonGetter> ArmorAddon(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IArmorAddonGetter>(() => listings.WinningOverrides<IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IArmorAddonGetter> ArmorAddon(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IArmorAddonGetter>(() => mods.WinningOverrides<IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IArtObjectGetter> ArtObject(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IArtObjectGetter>(() => listings.WinningOverrides<IArtObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IArtObjectGetter> ArtObject(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IArtObjectGetter>(() => mods.WinningOverrides<IArtObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IASpellGetter> ASpell(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IASpellGetter>(() => listings.WinningOverrides<IASpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IASpellGetter> ASpell(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IASpellGetter>(() => mods.WinningOverrides<IASpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAssociationTypeGetter> AssociationType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAssociationTypeGetter>(() => listings.WinningOverrides<IAssociationTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAssociationTypeGetter> AssociationType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAssociationTypeGetter>(() => mods.WinningOverrides<IAssociationTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAStoryManagerNodeGetter> AStoryManagerNode(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAStoryManagerNodeGetter>(() => listings.WinningOverrides<IAStoryManagerNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAStoryManagerNodeGetter> AStoryManagerNode(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAStoryManagerNodeGetter>(() => mods.WinningOverrides<IAStoryManagerNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IBodyPartDataGetter> BodyPartData(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IBodyPartDataGetter>(() => listings.WinningOverrides<IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IBodyPartDataGetter> BodyPartData(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IBodyPartDataGetter>(() => mods.WinningOverrides<IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IBookGetter> Book(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IBookGetter>(() => listings.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IBookGetter> Book(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IBookGetter>(() => mods.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ICameraPathGetter> CameraPath(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ICameraPathGetter>(() => listings.WinningOverrides<ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ICameraPathGetter> CameraPath(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ICameraPathGetter>(() => mods.WinningOverrides<ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ICameraShotGetter> CameraShot(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ICameraShotGetter>(() => listings.WinningOverrides<ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ICameraShotGetter> CameraShot(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ICameraShotGetter>(() => mods.WinningOverrides<ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ICellGetter> Cell(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ICellGetter>(() => listings.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ICellGetter> Cell(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ICellGetter>(() => mods.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ICellNavigationMeshGetter> CellNavigationMesh(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ICellNavigationMeshGetter>(() => listings.WinningOverrides<ICellNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ICellNavigationMeshGetter> CellNavigationMesh(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ICellNavigationMeshGetter>(() => mods.WinningOverrides<ICellNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IClassGetter> Class(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IClassGetter>(() => listings.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IClassGetter> Class(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IClassGetter>(() => mods.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IClimateGetter> Climate(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IClimateGetter>(() => listings.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IClimateGetter> Climate(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IClimateGetter>(() => mods.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ICollisionLayerGetter> CollisionLayer(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ICollisionLayerGetter>(() => listings.WinningOverrides<ICollisionLayerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ICollisionLayerGetter> CollisionLayer(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ICollisionLayerGetter>(() => mods.WinningOverrides<ICollisionLayerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IColorRecordGetter> ColorRecord(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IColorRecordGetter>(() => listings.WinningOverrides<IColorRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IColorRecordGetter> ColorRecord(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IColorRecordGetter>(() => mods.WinningOverrides<IColorRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ICombatStyleGetter> CombatStyle(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ICombatStyleGetter>(() => listings.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ICombatStyleGetter> CombatStyle(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ICombatStyleGetter>(() => mods.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IConstructibleObjectGetter> ConstructibleObject(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IConstructibleObjectGetter>(() => listings.WinningOverrides<IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IConstructibleObjectGetter> ConstructibleObject(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IConstructibleObjectGetter>(() => mods.WinningOverrides<IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IContainerGetter> Container(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IContainerGetter>(() => listings.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IContainerGetter> Container(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IContainerGetter>(() => mods.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDebrisGetter> Debris(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDebrisGetter>(() => listings.WinningOverrides<IDebrisGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDebrisGetter> Debris(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDebrisGetter>(() => mods.WinningOverrides<IDebrisGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDefaultObjectManagerGetter> DefaultObjectManager(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDefaultObjectManagerGetter>(() => listings.WinningOverrides<IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDefaultObjectManagerGetter> DefaultObjectManager(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDefaultObjectManagerGetter>(() => mods.WinningOverrides<IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDialogBranchGetter> DialogBranch(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDialogBranchGetter>(() => listings.WinningOverrides<IDialogBranchGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDialogBranchGetter> DialogBranch(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDialogBranchGetter>(() => mods.WinningOverrides<IDialogBranchGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDialogResponsesGetter> DialogResponses(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDialogResponsesGetter>(() => listings.WinningOverrides<IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDialogResponsesGetter> DialogResponses(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDialogResponsesGetter>(() => mods.WinningOverrides<IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDialogTopicGetter> DialogTopic(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDialogTopicGetter>(() => listings.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDialogTopicGetter> DialogTopic(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDialogTopicGetter>(() => mods.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDialogViewGetter> DialogView(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDialogViewGetter>(() => listings.WinningOverrides<IDialogViewGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDialogViewGetter> DialogView(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDialogViewGetter>(() => mods.WinningOverrides<IDialogViewGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDoorGetter> Door(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDoorGetter>(() => listings.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDoorGetter> Door(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDoorGetter>(() => mods.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDualCastDataGetter> DualCastData(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDualCastDataGetter>(() => listings.WinningOverrides<IDualCastDataGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDualCastDataGetter> DualCastData(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDualCastDataGetter>(() => mods.WinningOverrides<IDualCastDataGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IEffectShaderGetter> EffectShader(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IEffectShaderGetter>(() => listings.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IEffectShaderGetter> EffectShader(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IEffectShaderGetter>(() => mods.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IEncounterZoneGetter> EncounterZone(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IEncounterZoneGetter>(() => listings.WinningOverrides<IEncounterZoneGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IEncounterZoneGetter> EncounterZone(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IEncounterZoneGetter>(() => mods.WinningOverrides<IEncounterZoneGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IEquipTypeGetter> EquipType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IEquipTypeGetter>(() => listings.WinningOverrides<IEquipTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IEquipTypeGetter> EquipType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IEquipTypeGetter>(() => mods.WinningOverrides<IEquipTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IExplosionGetter> Explosion(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IExplosionGetter>(() => listings.WinningOverrides<IExplosionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IExplosionGetter> Explosion(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IExplosionGetter>(() => mods.WinningOverrides<IExplosionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IEyesGetter> Eyes(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IEyesGetter>(() => listings.WinningOverrides<IEyesGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IEyesGetter> Eyes(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IEyesGetter>(() => mods.WinningOverrides<IEyesGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IFactionGetter> Faction(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IFactionGetter>(() => listings.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IFactionGetter> Faction(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IFactionGetter>(() => mods.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IFloraGetter> Flora(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IFloraGetter>(() => listings.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IFloraGetter> Flora(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IFloraGetter>(() => mods.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IFootstepGetter> Footstep(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IFootstepGetter>(() => listings.WinningOverrides<IFootstepGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IFootstepGetter> Footstep(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IFootstepGetter>(() => mods.WinningOverrides<IFootstepGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IFootstepSetGetter> FootstepSet(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IFootstepSetGetter>(() => listings.WinningOverrides<IFootstepSetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IFootstepSetGetter> FootstepSet(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IFootstepSetGetter>(() => mods.WinningOverrides<IFootstepSetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IFormListGetter> FormList(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IFormListGetter>(() => listings.WinningOverrides<IFormListGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IFormListGetter> FormList(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IFormListGetter>(() => mods.WinningOverrides<IFormListGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IFurnitureGetter> Furniture(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IFurnitureGetter>(() => listings.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IFurnitureGetter> Furniture(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IFurnitureGetter>(() => mods.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGameSettingGetter> GameSetting(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGameSettingGetter>(() => listings.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGameSettingGetter> GameSetting(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGameSettingGetter>(() => mods.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGameSettingBoolGetter> GameSettingBool(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGameSettingBoolGetter>(() => listings.WinningOverrides<IGameSettingBoolGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGameSettingBoolGetter> GameSettingBool(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGameSettingBoolGetter>(() => mods.WinningOverrides<IGameSettingBoolGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGameSettingFloatGetter> GameSettingFloat(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGameSettingFloatGetter>(() => listings.WinningOverrides<IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGameSettingFloatGetter> GameSettingFloat(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGameSettingFloatGetter>(() => mods.WinningOverrides<IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGameSettingIntGetter> GameSettingInt(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGameSettingIntGetter>(() => listings.WinningOverrides<IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGameSettingIntGetter> GameSettingInt(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGameSettingIntGetter>(() => mods.WinningOverrides<IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGameSettingStringGetter> GameSettingString(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGameSettingStringGetter>(() => listings.WinningOverrides<IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGameSettingStringGetter> GameSettingString(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGameSettingStringGetter>(() => mods.WinningOverrides<IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGlobalGetter> Global(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGlobalGetter>(() => listings.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGlobalGetter> Global(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGlobalGetter>(() => mods.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGlobalFloatGetter> GlobalFloat(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGlobalFloatGetter>(() => listings.WinningOverrides<IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGlobalFloatGetter> GlobalFloat(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGlobalFloatGetter>(() => mods.WinningOverrides<IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGlobalIntGetter> GlobalInt(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGlobalIntGetter>(() => listings.WinningOverrides<IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGlobalIntGetter> GlobalInt(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGlobalIntGetter>(() => mods.WinningOverrides<IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGlobalShortGetter> GlobalShort(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGlobalShortGetter>(() => listings.WinningOverrides<IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGlobalShortGetter> GlobalShort(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGlobalShortGetter>(() => mods.WinningOverrides<IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGrassGetter> Grass(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGrassGetter>(() => listings.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGrassGetter> Grass(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGrassGetter>(() => mods.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IHairGetter> Hair(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IHairGetter>(() => listings.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IHairGetter> Hair(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IHairGetter>(() => mods.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IHazardGetter> Hazard(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IHazardGetter>(() => listings.WinningOverrides<IHazardGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IHazardGetter> Hazard(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IHazardGetter>(() => mods.WinningOverrides<IHazardGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IHeadPartGetter> HeadPart(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IHeadPartGetter>(() => listings.WinningOverrides<IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IHeadPartGetter> HeadPart(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IHeadPartGetter>(() => mods.WinningOverrides<IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IIdleAnimationGetter> IdleAnimation(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IIdleAnimationGetter>(() => listings.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IIdleAnimationGetter> IdleAnimation(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IIdleAnimationGetter>(() => mods.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IIdleMarkerGetter> IdleMarker(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IIdleMarkerGetter>(() => listings.WinningOverrides<IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IIdleMarkerGetter> IdleMarker(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IIdleMarkerGetter>(() => mods.WinningOverrides<IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IImageSpaceGetter> ImageSpace(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IImageSpaceGetter>(() => listings.WinningOverrides<IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IImageSpaceGetter> ImageSpace(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IImageSpaceGetter>(() => mods.WinningOverrides<IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IImageSpaceAdapterGetter> ImageSpaceAdapter(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IImageSpaceAdapterGetter>(() => listings.WinningOverrides<IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IImageSpaceAdapterGetter> ImageSpaceAdapter(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IImageSpaceAdapterGetter>(() => mods.WinningOverrides<IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IImpactGetter> Impact(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IImpactGetter>(() => listings.WinningOverrides<IImpactGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IImpactGetter> Impact(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IImpactGetter>(() => mods.WinningOverrides<IImpactGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IImpactDataSetGetter> ImpactDataSet(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IImpactDataSetGetter>(() => listings.WinningOverrides<IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IImpactDataSetGetter> ImpactDataSet(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IImpactDataSetGetter>(() => mods.WinningOverrides<IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IIngestibleGetter> Ingestible(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IIngestibleGetter>(() => listings.WinningOverrides<IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IIngestibleGetter> Ingestible(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IIngestibleGetter>(() => mods.WinningOverrides<IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IIngredientGetter> Ingredient(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IIngredientGetter>(() => listings.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IIngredientGetter> Ingredient(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IIngredientGetter>(() => mods.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IKeyGetter> Key(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IKeyGetter>(() => listings.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IKeyGetter> Key(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IKeyGetter>(() => mods.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IKeywordGetter> Keyword(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IKeywordGetter>(() => listings.WinningOverrides<IKeywordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IKeywordGetter> Keyword(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IKeywordGetter>(() => mods.WinningOverrides<IKeywordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILandscapeGetter> Landscape(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILandscapeGetter>(() => listings.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILandscapeGetter> Landscape(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILandscapeGetter>(() => mods.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILandscapeTextureGetter> LandscapeTexture(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILandscapeTextureGetter>(() => listings.WinningOverrides<ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILandscapeTextureGetter> LandscapeTexture(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILandscapeTextureGetter>(() => mods.WinningOverrides<ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILensFlareGetter> LensFlare(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILensFlareGetter>(() => listings.WinningOverrides<ILensFlareGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILensFlareGetter> LensFlare(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILensFlareGetter>(() => mods.WinningOverrides<ILensFlareGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILeveledItemGetter> LeveledItem(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILeveledItemGetter>(() => listings.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILeveledItemGetter> LeveledItem(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILeveledItemGetter>(() => mods.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILeveledNpcGetter> LeveledNpc(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILeveledNpcGetter>(() => listings.WinningOverrides<ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILeveledNpcGetter> LeveledNpc(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILeveledNpcGetter>(() => mods.WinningOverrides<ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILeveledSpellGetter> LeveledSpell(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILeveledSpellGetter>(() => listings.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILeveledSpellGetter> LeveledSpell(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILeveledSpellGetter>(() => mods.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILightGetter> Light(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILightGetter>(() => listings.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILightGetter> Light(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILightGetter>(() => mods.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILightingTemplateGetter> LightingTemplate(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILightingTemplateGetter>(() => listings.WinningOverrides<ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILightingTemplateGetter> LightingTemplate(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILightingTemplateGetter>(() => mods.WinningOverrides<ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILoadScreenGetter> LoadScreen(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILoadScreenGetter>(() => listings.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILoadScreenGetter> LoadScreen(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILoadScreenGetter>(() => mods.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILocationGetter> Location(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILocationGetter>(() => listings.WinningOverrides<ILocationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILocationGetter> Location(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILocationGetter>(() => mods.WinningOverrides<ILocationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILocationReferenceTypeGetter> LocationReferenceType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILocationReferenceTypeGetter>(() => listings.WinningOverrides<ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILocationReferenceTypeGetter> LocationReferenceType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILocationReferenceTypeGetter>(() => mods.WinningOverrides<ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMagicEffectGetter> MagicEffect(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMagicEffectGetter>(() => listings.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMagicEffectGetter> MagicEffect(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMagicEffectGetter>(() => mods.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMaterialObjectGetter> MaterialObject(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMaterialObjectGetter>(() => listings.WinningOverrides<IMaterialObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMaterialObjectGetter> MaterialObject(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMaterialObjectGetter>(() => mods.WinningOverrides<IMaterialObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMaterialTypeGetter> MaterialType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMaterialTypeGetter>(() => listings.WinningOverrides<IMaterialTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMaterialTypeGetter> MaterialType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMaterialTypeGetter>(() => mods.WinningOverrides<IMaterialTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMessageGetter> Message(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMessageGetter>(() => listings.WinningOverrides<IMessageGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMessageGetter> Message(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMessageGetter>(() => mods.WinningOverrides<IMessageGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMiscItemGetter> MiscItem(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMiscItemGetter>(() => listings.WinningOverrides<IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMiscItemGetter> MiscItem(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMiscItemGetter>(() => mods.WinningOverrides<IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMoveableStaticGetter> MoveableStatic(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMoveableStaticGetter>(() => listings.WinningOverrides<IMoveableStaticGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMoveableStaticGetter> MoveableStatic(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMoveableStaticGetter>(() => mods.WinningOverrides<IMoveableStaticGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMovementTypeGetter> MovementType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMovementTypeGetter>(() => listings.WinningOverrides<IMovementTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMovementTypeGetter> MovementType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMovementTypeGetter>(() => mods.WinningOverrides<IMovementTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMusicTrackGetter> MusicTrack(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMusicTrackGetter>(() => listings.WinningOverrides<IMusicTrackGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMusicTrackGetter> MusicTrack(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMusicTrackGetter>(() => mods.WinningOverrides<IMusicTrackGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMusicTypeGetter> MusicType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMusicTypeGetter>(() => listings.WinningOverrides<IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMusicTypeGetter> MusicType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMusicTypeGetter>(() => mods.WinningOverrides<IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<INavigationMeshInfoMapGetter> NavigationMeshInfoMap(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<INavigationMeshInfoMapGetter>(() => listings.WinningOverrides<INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<INavigationMeshInfoMapGetter> NavigationMeshInfoMap(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<INavigationMeshInfoMapGetter>(() => mods.WinningOverrides<INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<INpcGetter> Npc(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<INpcGetter>(() => listings.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<INpcGetter> Npc(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<INpcGetter>(() => mods.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IObjectEffectGetter> ObjectEffect(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IObjectEffectGetter>(() => listings.WinningOverrides<IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IObjectEffectGetter> ObjectEffect(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IObjectEffectGetter>(() => mods.WinningOverrides<IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOutfitGetter> Outfit(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOutfitGetter>(() => listings.WinningOverrides<IOutfitGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOutfitGetter> Outfit(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOutfitGetter>(() => mods.WinningOverrides<IOutfitGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPackageGetter> Package(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPackageGetter>(() => listings.WinningOverrides<IPackageGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPackageGetter> Package(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPackageGetter>(() => mods.WinningOverrides<IPackageGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPerkGetter> Perk(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPerkGetter>(() => listings.WinningOverrides<IPerkGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPerkGetter> Perk(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPerkGetter>(() => mods.WinningOverrides<IPerkGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedArrowGetter> PlacedArrow(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedArrowGetter>(() => listings.WinningOverrides<IPlacedArrowGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedArrowGetter> PlacedArrow(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedArrowGetter>(() => mods.WinningOverrides<IPlacedArrowGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedBarrierGetter> PlacedBarrier(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedBarrierGetter>(() => listings.WinningOverrides<IPlacedBarrierGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedBarrierGetter> PlacedBarrier(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedBarrierGetter>(() => mods.WinningOverrides<IPlacedBarrierGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedBeamGetter> PlacedBeam(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedBeamGetter>(() => listings.WinningOverrides<IPlacedBeamGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedBeamGetter> PlacedBeam(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedBeamGetter>(() => mods.WinningOverrides<IPlacedBeamGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedConeGetter> PlacedCone(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedConeGetter>(() => listings.WinningOverrides<IPlacedConeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedConeGetter> PlacedCone(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedConeGetter>(() => mods.WinningOverrides<IPlacedConeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedFlameGetter> PlacedFlame(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedFlameGetter>(() => listings.WinningOverrides<IPlacedFlameGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedFlameGetter> PlacedFlame(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedFlameGetter>(() => mods.WinningOverrides<IPlacedFlameGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedHazardGetter> PlacedHazard(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedHazardGetter>(() => listings.WinningOverrides<IPlacedHazardGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedHazardGetter> PlacedHazard(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedHazardGetter>(() => mods.WinningOverrides<IPlacedHazardGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedMissileGetter> PlacedMissile(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedMissileGetter>(() => listings.WinningOverrides<IPlacedMissileGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedMissileGetter> PlacedMissile(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedMissileGetter>(() => mods.WinningOverrides<IPlacedMissileGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedNpcGetter> PlacedNpc(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedNpcGetter>(() => listings.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedNpcGetter> PlacedNpc(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedNpcGetter>(() => mods.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedObjectGetter> PlacedObject(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedObjectGetter>(() => listings.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedObjectGetter> PlacedObject(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedObjectGetter>(() => mods.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedTrapGetter> PlacedTrap(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedTrapGetter>(() => listings.WinningOverrides<IPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedTrapGetter> PlacedTrap(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedTrapGetter>(() => mods.WinningOverrides<IPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IProjectileGetter> Projectile(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IProjectileGetter>(() => listings.WinningOverrides<IProjectileGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IProjectileGetter> Projectile(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IProjectileGetter>(() => mods.WinningOverrides<IProjectileGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IQuestGetter> Quest(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IQuestGetter>(() => listings.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IQuestGetter> Quest(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IQuestGetter>(() => mods.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IRaceGetter> Race(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IRaceGetter>(() => listings.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IRaceGetter> Race(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IRaceGetter>(() => mods.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IRegionGetter> Region(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IRegionGetter>(() => listings.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IRegionGetter> Region(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IRegionGetter>(() => mods.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IRelationshipGetter> Relationship(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IRelationshipGetter>(() => listings.WinningOverrides<IRelationshipGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IRelationshipGetter> Relationship(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IRelationshipGetter>(() => mods.WinningOverrides<IRelationshipGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IReverbParametersGetter> ReverbParameters(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IReverbParametersGetter>(() => listings.WinningOverrides<IReverbParametersGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IReverbParametersGetter> ReverbParameters(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IReverbParametersGetter>(() => mods.WinningOverrides<IReverbParametersGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISceneGetter> Scene(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISceneGetter>(() => listings.WinningOverrides<ISceneGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISceneGetter> Scene(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISceneGetter>(() => mods.WinningOverrides<ISceneGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IScrollGetter> Scroll(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IScrollGetter>(() => listings.WinningOverrides<IScrollGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IScrollGetter> Scroll(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IScrollGetter>(() => mods.WinningOverrides<IScrollGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IShaderParticleGeometryGetter> ShaderParticleGeometry(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IShaderParticleGeometryGetter>(() => listings.WinningOverrides<IShaderParticleGeometryGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IShaderParticleGeometryGetter> ShaderParticleGeometry(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IShaderParticleGeometryGetter>(() => mods.WinningOverrides<IShaderParticleGeometryGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IShoutGetter> Shout(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IShoutGetter>(() => listings.WinningOverrides<IShoutGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IShoutGetter> Shout(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IShoutGetter>(() => mods.WinningOverrides<IShoutGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMajorRecordGetter> SkyrimMajorRecord(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMajorRecordGetter>(() => listings.WinningOverrides<ISkyrimMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkyrimMajorRecordGetter> SkyrimMajorRecord(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkyrimMajorRecordGetter>(() => mods.WinningOverrides<ISkyrimMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISoulGemGetter> SoulGem(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISoulGemGetter>(() => listings.WinningOverrides<ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISoulGemGetter> SoulGem(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISoulGemGetter>(() => mods.WinningOverrides<ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISoundCategoryGetter> SoundCategory(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISoundCategoryGetter>(() => listings.WinningOverrides<ISoundCategoryGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISoundCategoryGetter> SoundCategory(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISoundCategoryGetter>(() => mods.WinningOverrides<ISoundCategoryGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISoundDescriptorGetter> SoundDescriptor(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISoundDescriptorGetter>(() => listings.WinningOverrides<ISoundDescriptorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISoundDescriptorGetter> SoundDescriptor(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISoundDescriptorGetter>(() => mods.WinningOverrides<ISoundDescriptorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISoundMarkerGetter> SoundMarker(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISoundMarkerGetter>(() => listings.WinningOverrides<ISoundMarkerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISoundMarkerGetter> SoundMarker(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISoundMarkerGetter>(() => mods.WinningOverrides<ISoundMarkerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISoundOutputModelGetter> SoundOutputModel(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISoundOutputModelGetter>(() => listings.WinningOverrides<ISoundOutputModelGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISoundOutputModelGetter> SoundOutputModel(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISoundOutputModelGetter>(() => mods.WinningOverrides<ISoundOutputModelGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISpellGetter> Spell(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISpellGetter>(() => listings.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISpellGetter> Spell(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISpellGetter>(() => mods.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IStaticGetter> Static(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IStaticGetter>(() => listings.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IStaticGetter> Static(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IStaticGetter>(() => mods.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IStoryManagerBranchNodeGetter> StoryManagerBranchNode(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IStoryManagerBranchNodeGetter>(() => listings.WinningOverrides<IStoryManagerBranchNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IStoryManagerBranchNodeGetter> StoryManagerBranchNode(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IStoryManagerBranchNodeGetter>(() => mods.WinningOverrides<IStoryManagerBranchNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IStoryManagerEventNodeGetter> StoryManagerEventNode(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IStoryManagerEventNodeGetter>(() => listings.WinningOverrides<IStoryManagerEventNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IStoryManagerEventNodeGetter> StoryManagerEventNode(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IStoryManagerEventNodeGetter>(() => mods.WinningOverrides<IStoryManagerEventNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IStoryManagerQuestNodeGetter> StoryManagerQuestNode(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IStoryManagerQuestNodeGetter>(() => listings.WinningOverrides<IStoryManagerQuestNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IStoryManagerQuestNodeGetter> StoryManagerQuestNode(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IStoryManagerQuestNodeGetter>(() => mods.WinningOverrides<IStoryManagerQuestNodeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ITalkingActivatorGetter> TalkingActivator(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ITalkingActivatorGetter>(() => listings.WinningOverrides<ITalkingActivatorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ITalkingActivatorGetter> TalkingActivator(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ITalkingActivatorGetter>(() => mods.WinningOverrides<ITalkingActivatorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ITextureSetGetter> TextureSet(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ITextureSetGetter>(() => listings.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ITextureSetGetter> TextureSet(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ITextureSetGetter>(() => mods.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ITreeGetter> Tree(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ITreeGetter>(() => listings.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ITreeGetter> Tree(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ITreeGetter>(() => mods.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IVisualEffectGetter> VisualEffect(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IVisualEffectGetter>(() => listings.WinningOverrides<IVisualEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IVisualEffectGetter> VisualEffect(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IVisualEffectGetter>(() => mods.WinningOverrides<IVisualEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IVoiceTypeGetter> VoiceType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IVoiceTypeGetter>(() => listings.WinningOverrides<IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IVoiceTypeGetter> VoiceType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IVoiceTypeGetter>(() => mods.WinningOverrides<IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IVolumetricLightingGetter> VolumetricLighting(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IVolumetricLightingGetter>(() => listings.WinningOverrides<IVolumetricLightingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IVolumetricLightingGetter> VolumetricLighting(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IVolumetricLightingGetter>(() => mods.WinningOverrides<IVolumetricLightingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IWaterGetter> Water(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IWaterGetter>(() => listings.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IWaterGetter> Water(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IWaterGetter>(() => mods.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IWeaponGetter> Weapon(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IWeaponGetter>(() => listings.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IWeaponGetter> Weapon(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IWeaponGetter>(() => mods.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IWeatherGetter> Weather(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IWeatherGetter>(() => listings.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IWeatherGetter> Weather(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IWeatherGetter>(() => mods.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IWordOfPowerGetter> WordOfPower(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IWordOfPowerGetter>(() => listings.WinningOverrides<IWordOfPowerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IWordOfPowerGetter> WordOfPower(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IWordOfPowerGetter>(() => mods.WinningOverrides<IWordOfPowerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IWorldspaceGetter> Worldspace(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IWorldspaceGetter>(() => listings.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IWorldspaceGetter> Worldspace(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IWorldspaceGetter>(() => mods.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IWorldspaceNavigationMeshGetter> WorldspaceNavigationMesh(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IWorldspaceNavigationMeshGetter>(() => listings.WinningOverrides<IWorldspaceNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IWorldspaceNavigationMeshGetter> WorldspaceNavigationMesh(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IWorldspaceNavigationMeshGetter>(() => mods.WinningOverrides<IWorldspaceNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

        #region Link Interfaces
        public static TypedLoadOrderAccess<IIdleRelationGetter> IIdleRelation(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IIdleRelationGetter>(() => listings.WinningOverrides<IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IIdleRelationGetter> IIdleRelation(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IIdleRelationGetter>(() => mods.WinningOverrides<IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IObjectIdGetter> IObjectId(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IObjectIdGetter>(() => listings.WinningOverrides<IObjectIdGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IObjectIdGetter> IObjectId(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IObjectIdGetter>(() => mods.WinningOverrides<IObjectIdGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IItemGetter> IItem(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IItemGetter>(() => listings.WinningOverrides<IItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IItemGetter> IItem(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IItemGetter>(() => mods.WinningOverrides<IItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOutfitTargetGetter> IOutfitTarget(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOutfitTargetGetter>(() => listings.WinningOverrides<IOutfitTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOutfitTargetGetter> IOutfitTarget(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOutfitTargetGetter>(() => mods.WinningOverrides<IOutfitTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IComplexLocationGetter> IComplexLocation(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IComplexLocationGetter>(() => listings.WinningOverrides<IComplexLocationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IComplexLocationGetter> IComplexLocation(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IComplexLocationGetter>(() => mods.WinningOverrides<IComplexLocationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDialogGetter> IDialog(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDialogGetter>(() => listings.WinningOverrides<IDialogGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDialogGetter> IDialog(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDialogGetter>(() => mods.WinningOverrides<IDialogGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILocationTargetableGetter> ILocationTargetable(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILocationTargetableGetter>(() => listings.WinningOverrides<ILocationTargetableGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILocationTargetableGetter> ILocationTargetable(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILocationTargetableGetter>(() => mods.WinningOverrides<ILocationTargetableGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOwnerGetter> IOwner(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOwnerGetter>(() => listings.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOwnerGetter> IOwner(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOwnerGetter>(() => mods.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IRelatableGetter> IRelatable(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IRelatableGetter>(() => listings.WinningOverrides<IRelatableGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IRelatableGetter> IRelatable(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IRelatableGetter>(() => mods.WinningOverrides<IRelatableGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IRegionTargetGetter> IRegionTarget(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IRegionTargetGetter>(() => listings.WinningOverrides<IRegionTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IRegionTargetGetter> IRegionTarget(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IRegionTargetGetter>(() => mods.WinningOverrides<IRegionTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAliasVoiceTypeGetter> IAliasVoiceType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAliasVoiceTypeGetter>(() => listings.WinningOverrides<IAliasVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAliasVoiceTypeGetter> IAliasVoiceType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAliasVoiceTypeGetter>(() => mods.WinningOverrides<IAliasVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILockListGetter> ILockList(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILockListGetter>(() => listings.WinningOverrides<ILockListGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILockListGetter> ILockList(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILockListGetter>(() => mods.WinningOverrides<ILockListGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedTrapTargetGetter> IPlacedTrapTarget(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedTrapTargetGetter>(() => listings.WinningOverrides<IPlacedTrapTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedTrapTargetGetter> IPlacedTrapTarget(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedTrapTargetGetter>(() => mods.WinningOverrides<IPlacedTrapTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IHarvestTargetGetter> IHarvestTarget(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IHarvestTargetGetter>(() => listings.WinningOverrides<IHarvestTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IHarvestTargetGetter> IHarvestTarget(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IHarvestTargetGetter>(() => mods.WinningOverrides<IHarvestTargetGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IKeywordLinkedReferenceGetter> IKeywordLinkedReference(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IKeywordLinkedReferenceGetter>(() => listings.WinningOverrides<IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IKeywordLinkedReferenceGetter> IKeywordLinkedReference(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IKeywordLinkedReferenceGetter>(() => mods.WinningOverrides<IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<INpcSpawnGetter> INpcSpawn(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<INpcSpawnGetter>(() => listings.WinningOverrides<INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<INpcSpawnGetter> INpcSpawn(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<INpcSpawnGetter>(() => mods.WinningOverrides<INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISpellSpawnGetter> ISpellSpawn(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISpellSpawnGetter>(() => listings.WinningOverrides<ISpellSpawnGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISpellSpawnGetter> ISpellSpawn(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISpellSpawnGetter>(() => mods.WinningOverrides<ISpellSpawnGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IEmittanceGetter> IEmittance(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IEmittanceGetter>(() => listings.WinningOverrides<IEmittanceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IEmittanceGetter> IEmittance(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IEmittanceGetter>(() => mods.WinningOverrides<IEmittanceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILocationRecordGetter> ILocationRecord(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILocationRecordGetter>(() => listings.WinningOverrides<ILocationRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILocationRecordGetter> ILocationRecord(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILocationRecordGetter>(() => mods.WinningOverrides<ILocationRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IEffectRecordGetter> IEffectRecord(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IEffectRecordGetter>(() => listings.WinningOverrides<IEffectRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IEffectRecordGetter> IEffectRecord(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IEffectRecordGetter>(() => mods.WinningOverrides<IEffectRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILinkedReferenceGetter> ILinkedReference(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILinkedReferenceGetter>(() => listings.WinningOverrides<ILinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILinkedReferenceGetter> ILinkedReference(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILinkedReferenceGetter>(() => mods.WinningOverrides<ILinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedGetter> IPlaced(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedGetter>(() => listings.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedGetter> IPlaced(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedGetter>(() => mods.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedSimpleGetter> IPlacedSimple(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedSimpleGetter>(() => listings.WinningOverrides<IPlacedSimpleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedSimpleGetter> IPlacedSimple(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedSimpleGetter>(() => mods.WinningOverrides<IPlacedSimpleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedThingGetter> IPlacedThing(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedThingGetter>(() => listings.WinningOverrides<IPlacedThingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedThingGetter> IPlacedThing(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedThingGetter>(() => mods.WinningOverrides<IPlacedThingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISoundGetter> ISound(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISoundGetter>(() => listings.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISoundGetter> ISound(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISoundGetter>(() => mods.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

    }
}
