using System.Collections.Generic;
namespace Mutagen.Bethesda.Skyrim
{
    public static class TypeOptionSolidifierMixIns
    {
        #region Normal
        public static IEnumerable<IAcousticSpaceGetter> AcousticSpace(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAcousticSpaceGetter> AcousticSpace(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IActionRecordGetter> ActionRecord(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IActionRecordGetter> ActionRecord(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IActivatorGetter> Activator(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IActivatorGetter> Activator(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IActorValueInformationGetter> ActorValueInformation(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IActorValueInformationGetter> ActorValueInformation(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAddonNodeGetter> AddonNode(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAddonNodeGetter> AddonNode(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAlchemicalApparatusGetter> AlchemicalApparatus(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAlchemicalApparatusGetter> AlchemicalApparatus(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAmmunitionGetter> Ammunition(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAmmunitionGetter> Ammunition(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IANavigationMeshGetter> ANavigationMesh(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IANavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IANavigationMeshGetter> ANavigationMesh(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IANavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAnimatedObjectGetter> AnimatedObject(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAnimatedObjectGetter> AnimatedObject(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAPlacedTrapGetter> APlacedTrap(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IAPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAPlacedTrapGetter> APlacedTrap(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IAPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IArmorGetter> Armor(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IArmorGetter> Armor(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IArmorAddonGetter> ArmorAddon(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IArmorAddonGetter> ArmorAddon(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IArtObjectGetter> ArtObject(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IArtObjectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IArtObjectGetter> ArtObject(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IArtObjectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IASpellGetter> ASpell(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IASpellGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IASpellGetter> ASpell(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IASpellGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAssociationTypeGetter> AssociationType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IAssociationTypeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAssociationTypeGetter> AssociationType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IAssociationTypeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAStoryManagerNodeGetter> AStoryManagerNode(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IAStoryManagerNodeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAStoryManagerNodeGetter> AStoryManagerNode(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IAStoryManagerNodeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IBodyPartDataGetter> BodyPartData(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IBodyPartDataGetter> BodyPartData(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IBookGetter> Book(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IBookGetter> Book(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ICameraPathGetter> CameraPath(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ICameraPathGetter> CameraPath(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ICameraShotGetter> CameraShot(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ICameraShotGetter> CameraShot(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ICellGetter> Cell(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ICellGetter> Cell(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ICellNavigationMeshGetter> CellNavigationMesh(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ICellNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ICellNavigationMeshGetter> CellNavigationMesh(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ICellNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IClassGetter> Class(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IClassGetter> Class(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IClimateGetter> Climate(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IClimateGetter> Climate(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ICollisionLayerGetter> CollisionLayer(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ICollisionLayerGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ICollisionLayerGetter> CollisionLayer(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ICollisionLayerGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IColorRecordGetter> ColorRecord(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IColorRecordGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IColorRecordGetter> ColorRecord(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IColorRecordGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ICombatStyleGetter> CombatStyle(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ICombatStyleGetter> CombatStyle(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IConstructibleObjectGetter> ConstructibleObject(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IConstructibleObjectGetter> ConstructibleObject(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IContainerGetter> Container(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IContainerGetter> Container(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDebrisGetter> Debris(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IDebrisGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDebrisGetter> Debris(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IDebrisGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDefaultObjectManagerGetter> DefaultObjectManager(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDefaultObjectManagerGetter> DefaultObjectManager(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDialogBranchGetter> DialogBranch(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IDialogBranchGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDialogBranchGetter> DialogBranch(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IDialogBranchGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDialogResponsesGetter> DialogResponses(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDialogResponsesGetter> DialogResponses(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDialogTopicGetter> DialogTopic(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDialogTopicGetter> DialogTopic(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDialogViewGetter> DialogView(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IDialogViewGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDialogViewGetter> DialogView(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IDialogViewGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDoorGetter> Door(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDoorGetter> Door(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDualCastDataGetter> DualCastData(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IDualCastDataGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDualCastDataGetter> DualCastData(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IDualCastDataGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IEffectShaderGetter> EffectShader(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IEffectShaderGetter> EffectShader(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IEncounterZoneGetter> EncounterZone(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IEncounterZoneGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IEncounterZoneGetter> EncounterZone(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IEncounterZoneGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IEquipTypeGetter> EquipType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IEquipTypeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IEquipTypeGetter> EquipType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IEquipTypeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IExplosionGetter> Explosion(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IExplosionGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IExplosionGetter> Explosion(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IExplosionGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IEyesGetter> Eyes(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IEyesGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IEyesGetter> Eyes(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IEyesGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IFactionGetter> Faction(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IFactionGetter> Faction(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IFloraGetter> Flora(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IFloraGetter> Flora(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IFootstepGetter> Footstep(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IFootstepGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IFootstepGetter> Footstep(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IFootstepGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IFootstepSetGetter> FootstepSet(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IFootstepSetGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IFootstepSetGetter> FootstepSet(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IFootstepSetGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IFormListGetter> FormList(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IFormListGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IFormListGetter> FormList(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IFormListGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IFurnitureGetter> Furniture(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IFurnitureGetter> Furniture(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGameSettingGetter> GameSetting(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGameSettingGetter> GameSetting(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGameSettingBoolGetter> GameSettingBool(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IGameSettingBoolGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGameSettingBoolGetter> GameSettingBool(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IGameSettingBoolGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGameSettingFloatGetter> GameSettingFloat(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGameSettingFloatGetter> GameSettingFloat(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGameSettingIntGetter> GameSettingInt(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGameSettingIntGetter> GameSettingInt(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGameSettingStringGetter> GameSettingString(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGameSettingStringGetter> GameSettingString(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGlobalGetter> Global(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGlobalGetter> Global(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGlobalFloatGetter> GlobalFloat(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGlobalFloatGetter> GlobalFloat(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGlobalIntGetter> GlobalInt(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGlobalIntGetter> GlobalInt(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGlobalShortGetter> GlobalShort(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGlobalShortGetter> GlobalShort(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGrassGetter> Grass(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGrassGetter> Grass(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IHairGetter> Hair(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IHairGetter> Hair(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IHazardGetter> Hazard(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IHazardGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IHazardGetter> Hazard(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IHazardGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IHeadPartGetter> HeadPart(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IHeadPartGetter> HeadPart(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IIdleAnimationGetter> IdleAnimation(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IIdleAnimationGetter> IdleAnimation(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IIdleMarkerGetter> IdleMarker(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IIdleMarkerGetter> IdleMarker(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IImageSpaceGetter> ImageSpace(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IImageSpaceGetter> ImageSpace(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IImageSpaceAdapterGetter> ImageSpaceAdapter(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IImageSpaceAdapterGetter> ImageSpaceAdapter(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IImpactGetter> Impact(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IImpactGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IImpactGetter> Impact(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IImpactGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IImpactDataSetGetter> ImpactDataSet(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IImpactDataSetGetter> ImpactDataSet(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IIngestibleGetter> Ingestible(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IIngestibleGetter> Ingestible(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IIngredientGetter> Ingredient(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IIngredientGetter> Ingredient(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IKeyGetter> Key(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IKeyGetter> Key(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IKeywordGetter> Keyword(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IKeywordGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IKeywordGetter> Keyword(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IKeywordGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILandscapeGetter> Landscape(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILandscapeGetter> Landscape(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILandscapeTextureGetter> LandscapeTexture(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILandscapeTextureGetter> LandscapeTexture(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILensFlareGetter> LensFlare(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILensFlareGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILensFlareGetter> LensFlare(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILensFlareGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILeveledItemGetter> LeveledItem(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILeveledItemGetter> LeveledItem(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILeveledNpcGetter> LeveledNpc(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILeveledNpcGetter> LeveledNpc(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILeveledSpellGetter> LeveledSpell(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILeveledSpellGetter> LeveledSpell(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILightGetter> Light(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILightGetter> Light(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILightingTemplateGetter> LightingTemplate(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILightingTemplateGetter> LightingTemplate(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILoadScreenGetter> LoadScreen(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILoadScreenGetter> LoadScreen(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILocationGetter> Location(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILocationGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILocationGetter> Location(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILocationGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILocationReferenceTypeGetter> LocationReferenceType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILocationReferenceTypeGetter> LocationReferenceType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMagicEffectGetter> MagicEffect(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMagicEffectGetter> MagicEffect(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMaterialObjectGetter> MaterialObject(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IMaterialObjectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMaterialObjectGetter> MaterialObject(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IMaterialObjectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMaterialTypeGetter> MaterialType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IMaterialTypeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMaterialTypeGetter> MaterialType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IMaterialTypeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMessageGetter> Message(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IMessageGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMessageGetter> Message(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IMessageGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMiscItemGetter> MiscItem(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMiscItemGetter> MiscItem(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMoveableStaticGetter> MoveableStatic(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IMoveableStaticGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMoveableStaticGetter> MoveableStatic(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IMoveableStaticGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMovementTypeGetter> MovementType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IMovementTypeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMovementTypeGetter> MovementType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IMovementTypeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMusicTrackGetter> MusicTrack(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IMusicTrackGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMusicTrackGetter> MusicTrack(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IMusicTrackGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMusicTypeGetter> MusicType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMusicTypeGetter> MusicType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<INavigationMeshInfoMapGetter> NavigationMeshInfoMap(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<INavigationMeshInfoMapGetter> NavigationMeshInfoMap(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<INpcGetter> Npc(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<INpcGetter> Npc(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IObjectEffectGetter> ObjectEffect(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IObjectEffectGetter> ObjectEffect(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IOutfitGetter> Outfit(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IOutfitGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IOutfitGetter> Outfit(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IOutfitGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPackageGetter> Package(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPackageGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPackageGetter> Package(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPackageGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPerkGetter> Perk(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPerkGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPerkGetter> Perk(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPerkGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedArrowGetter> PlacedArrow(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPlacedArrowGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedArrowGetter> PlacedArrow(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPlacedArrowGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedBarrierGetter> PlacedBarrier(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPlacedBarrierGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedBarrierGetter> PlacedBarrier(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPlacedBarrierGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedBeamGetter> PlacedBeam(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPlacedBeamGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedBeamGetter> PlacedBeam(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPlacedBeamGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedConeGetter> PlacedCone(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPlacedConeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedConeGetter> PlacedCone(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPlacedConeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedFlameGetter> PlacedFlame(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPlacedFlameGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedFlameGetter> PlacedFlame(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPlacedFlameGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedHazardGetter> PlacedHazard(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPlacedHazardGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedHazardGetter> PlacedHazard(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPlacedHazardGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedMissileGetter> PlacedMissile(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPlacedMissileGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedMissileGetter> PlacedMissile(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPlacedMissileGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedNpcGetter> PlacedNpc(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedNpcGetter> PlacedNpc(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedObjectGetter> PlacedObject(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedObjectGetter> PlacedObject(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedTrapGetter> PlacedTrap(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedTrapGetter> PlacedTrap(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IProjectileGetter> Projectile(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IProjectileGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IProjectileGetter> Projectile(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IProjectileGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IQuestGetter> Quest(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IQuestGetter> Quest(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IRaceGetter> Race(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IRaceGetter> Race(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IRegionGetter> Region(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IRegionGetter> Region(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IRelationshipGetter> Relationship(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IRelationshipGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IRelationshipGetter> Relationship(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IRelationshipGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IReverbParametersGetter> ReverbParameters(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IReverbParametersGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IReverbParametersGetter> ReverbParameters(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IReverbParametersGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISceneGetter> Scene(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ISceneGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISceneGetter> Scene(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ISceneGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IScrollGetter> Scroll(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IScrollGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IScrollGetter> Scroll(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IScrollGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IShaderParticleGeometryGetter> ShaderParticleGeometry(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IShaderParticleGeometryGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IShaderParticleGeometryGetter> ShaderParticleGeometry(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IShaderParticleGeometryGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IShoutGetter> Shout(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IShoutGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IShoutGetter> Shout(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IShoutGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISkyrimMajorRecordGetter> SkyrimMajorRecord(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ISkyrimMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISkyrimMajorRecordGetter> SkyrimMajorRecord(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ISkyrimMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISoulGemGetter> SoulGem(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISoulGemGetter> SoulGem(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISoundCategoryGetter> SoundCategory(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ISoundCategoryGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISoundCategoryGetter> SoundCategory(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ISoundCategoryGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISoundDescriptorGetter> SoundDescriptor(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ISoundDescriptorGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISoundDescriptorGetter> SoundDescriptor(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ISoundDescriptorGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISoundMarkerGetter> SoundMarker(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ISoundMarkerGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISoundMarkerGetter> SoundMarker(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ISoundMarkerGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISoundOutputModelGetter> SoundOutputModel(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ISoundOutputModelGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISoundOutputModelGetter> SoundOutputModel(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ISoundOutputModelGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISpellGetter> Spell(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISpellGetter> Spell(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IStaticGetter> Static(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IStaticGetter> Static(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IStoryManagerBranchNodeGetter> StoryManagerBranchNode(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IStoryManagerBranchNodeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IStoryManagerBranchNodeGetter> StoryManagerBranchNode(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IStoryManagerBranchNodeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IStoryManagerEventNodeGetter> StoryManagerEventNode(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IStoryManagerEventNodeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IStoryManagerEventNodeGetter> StoryManagerEventNode(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IStoryManagerEventNodeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IStoryManagerQuestNodeGetter> StoryManagerQuestNode(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IStoryManagerQuestNodeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IStoryManagerQuestNodeGetter> StoryManagerQuestNode(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IStoryManagerQuestNodeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ITalkingActivatorGetter> TalkingActivator(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ITalkingActivatorGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ITalkingActivatorGetter> TalkingActivator(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ITalkingActivatorGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ITextureSetGetter> TextureSet(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ITextureSetGetter> TextureSet(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ITreeGetter> Tree(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ITreeGetter> Tree(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IVisualEffectGetter> VisualEffect(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IVisualEffectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IVisualEffectGetter> VisualEffect(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IVisualEffectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IVoiceTypeGetter> VoiceType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IVoiceTypeGetter> VoiceType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IVolumetricLightingGetter> VolumetricLighting(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IVolumetricLightingGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IVolumetricLightingGetter> VolumetricLighting(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IVolumetricLightingGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IWaterGetter> Water(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IWaterGetter> Water(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IWeaponGetter> Weapon(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IWeaponGetter> Weapon(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IWeatherGetter> Weather(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IWeatherGetter> Weather(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IWordOfPowerGetter> WordOfPower(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IWordOfPowerGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IWordOfPowerGetter> WordOfPower(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IWordOfPowerGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IWorldspaceGetter> Worldspace(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IWorldspaceGetter> Worldspace(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IWorldspaceNavigationMeshGetter> WorldspaceNavigationMesh(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IWorldspaceNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IWorldspaceNavigationMeshGetter> WorldspaceNavigationMesh(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IWorldspaceNavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        #endregion

        #region Link Interfaces
        public static IEnumerable<IIdleRelationGetter> IIdleRelation(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IIdleRelationGetter> IIdleRelation(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IObjectIdGetter> IObjectId(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IObjectIdGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IObjectIdGetter> IObjectId(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IObjectIdGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IItemGetter> IItem(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IItemGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IItemGetter> IItem(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IItemGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IOutfitTargetGetter> IOutfitTarget(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IOutfitTargetGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IOutfitTargetGetter> IOutfitTarget(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IOutfitTargetGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IComplexLocationGetter> IComplexLocation(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IComplexLocationGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IComplexLocationGetter> IComplexLocation(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IComplexLocationGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDialogGetter> IDialog(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IDialogGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDialogGetter> IDialog(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IDialogGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILocationTargetableGetter> ILocationTargetable(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILocationTargetableGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILocationTargetableGetter> ILocationTargetable(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILocationTargetableGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IOwnerGetter> IOwner(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IOwnerGetter> IOwner(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IRelatableGetter> IRelatable(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IRelatableGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IRelatableGetter> IRelatable(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IRelatableGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IRegionTargetGetter> IRegionTarget(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IRegionTargetGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IRegionTargetGetter> IRegionTarget(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IRegionTargetGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAliasVoiceTypeGetter> IAliasVoiceType(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IAliasVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAliasVoiceTypeGetter> IAliasVoiceType(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IAliasVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILockListGetter> ILockList(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILockListGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILockListGetter> ILockList(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILockListGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedTrapTargetGetter> IPlacedTrapTarget(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPlacedTrapTargetGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedTrapTargetGetter> IPlacedTrapTarget(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPlacedTrapTargetGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IHarvestTargetGetter> IHarvestTarget(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IHarvestTargetGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IHarvestTargetGetter> IHarvestTarget(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IHarvestTargetGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IKeywordLinkedReferenceGetter> IKeywordLinkedReference(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IKeywordLinkedReferenceGetter> IKeywordLinkedReference(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<INpcSpawnGetter> INpcSpawn(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<INpcSpawnGetter> INpcSpawn(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISpellSpawnGetter> ISpellSpawn(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ISpellSpawnGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISpellSpawnGetter> ISpellSpawn(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ISpellSpawnGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IEmittanceGetter> IEmittance(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IEmittanceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IEmittanceGetter> IEmittance(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IEmittanceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILocationRecordGetter> ILocationRecord(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILocationRecordGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILocationRecordGetter> ILocationRecord(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILocationRecordGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IEffectRecordGetter> IEffectRecord(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IEffectRecordGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IEffectRecordGetter> IEffectRecord(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IEffectRecordGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILinkedReferenceGetter> ILinkedReference(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILinkedReferenceGetter> ILinkedReference(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedGetter> IPlaced(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedGetter> IPlaced(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedSimpleGetter> IPlacedSimple(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPlacedSimpleGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedSimpleGetter> IPlacedSimple(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPlacedSimpleGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedThingGetter> IPlacedThing(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPlacedThingGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedThingGetter> IPlacedThing(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPlacedThingGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISoundGetter> ISound(
            this IEnumerable<IModListing<ISkyrimModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISoundGetter> ISound(
            this IEnumerable<ISkyrimModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        #endregion

    }
}
