using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.Fallout76
{
    public static class TypeOptionSolidifierMixIns
    {
        #region Normal
        /// <summary>
        /// Scope a load order query to AcousticSpace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AcousticSpace</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAcousticSpace, IAcousticSpaceGetter> AcousticSpace(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAcousticSpace, IAcousticSpaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAcousticSpace, IAcousticSpaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AcousticSpace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AcousticSpace</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAcousticSpace, IAcousticSpaceGetter> AcousticSpace(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAcousticSpace, IAcousticSpaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAcousticSpace, IAcousticSpaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActionRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ActionRecord</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IActionRecord, IActionRecordGetter> ActionRecord(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IActionRecord, IActionRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IActionRecord, IActionRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActionRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ActionRecord</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IActionRecord, IActionRecordGetter> ActionRecord(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IActionRecord, IActionRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IActionRecord, IActionRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Activator
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Activator</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IActivator, IActivatorGetter> Activator(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IActivator, IActivatorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IActivator, IActivatorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Activator
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Activator</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IActivator, IActivatorGetter> Activator(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IActivator, IActivatorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IActivator, IActivatorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActorValueInformation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ActorValueInformation</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IActorValueInformation, IActorValueInformationGetter> ActorValueInformation(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IActorValueInformation, IActorValueInformationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IActorValueInformation, IActorValueInformationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActorValueInformation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ActorValueInformation</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IActorValueInformation, IActorValueInformationGetter> ActorValueInformation(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IActorValueInformation, IActorValueInformationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IActorValueInformation, IActorValueInformationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ADamageType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ADamageType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IADamageType, IADamageTypeGetter> ADamageType(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IADamageType, IADamageTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IADamageTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IADamageType, IADamageTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ADamageType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ADamageType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IADamageType, IADamageTypeGetter> ADamageType(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IADamageType, IADamageTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IADamageTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IADamageType, IADamageTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AddonNode
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AddonNode</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAddonNode, IAddonNodeGetter> AddonNode(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAddonNode, IAddonNodeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAddonNode, IAddonNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AddonNode
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AddonNode</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAddonNode, IAddonNodeGetter> AddonNode(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAddonNode, IAddonNodeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAddonNode, IAddonNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AimAssistModelData
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AimAssistModelData</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAimAssistModelData, IAimAssistModelDataGetter> AimAssistModelData(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAimAssistModelData, IAimAssistModelDataGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAimAssistModelDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAimAssistModelData, IAimAssistModelDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AimAssistModelData
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AimAssistModelData</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAimAssistModelData, IAimAssistModelDataGetter> AimAssistModelData(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAimAssistModelData, IAimAssistModelDataGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAimAssistModelDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAimAssistModelData, IAimAssistModelDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AimAssistPoseData
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AimAssistPoseData</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAimAssistPoseData, IAimAssistPoseDataGetter> AimAssistPoseData(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAimAssistPoseData, IAimAssistPoseDataGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAimAssistPoseDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAimAssistPoseData, IAimAssistPoseDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AimAssistPoseData
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AimAssistPoseData</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAimAssistPoseData, IAimAssistPoseDataGetter> AimAssistPoseData(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAimAssistPoseData, IAimAssistPoseDataGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAimAssistPoseDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAimAssistPoseData, IAimAssistPoseDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AimModel
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AimModel</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAimModel, IAimModelGetter> AimModel(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAimModel, IAimModelGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAimModelGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAimModel, IAimModelGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AimModel
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AimModel</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAimModel, IAimModelGetter> AimModel(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAimModel, IAimModelGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAimModelGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAimModel, IAimModelGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ammunition
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ammunition</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAmmunition, IAmmunitionGetter> Ammunition(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAmmunition, IAmmunitionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAmmunition, IAmmunitionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ammunition
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ammunition</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAmmunition, IAmmunitionGetter> Ammunition(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAmmunition, IAmmunitionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAmmunition, IAmmunitionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AnimatedObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AnimatedObject</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAnimatedObject, IAnimatedObjectGetter> AnimatedObject(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAnimatedObject, IAnimatedObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAnimatedObject, IAnimatedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AnimatedObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AnimatedObject</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAnimatedObject, IAnimatedObjectGetter> AnimatedObject(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAnimatedObject, IAnimatedObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAnimatedObject, IAnimatedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AnimationSoundTagSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AnimationSoundTagSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAnimationSoundTagSet, IAnimationSoundTagSetGetter> AnimationSoundTagSet(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAnimationSoundTagSet, IAnimationSoundTagSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAnimationSoundTagSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAnimationSoundTagSet, IAnimationSoundTagSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AnimationSoundTagSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AnimationSoundTagSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAnimationSoundTagSet, IAnimationSoundTagSetGetter> AnimationSoundTagSet(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAnimationSoundTagSet, IAnimationSoundTagSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAnimationSoundTagSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAnimationSoundTagSet, IAnimationSoundTagSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AObjectModification
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AObjectModification</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAObjectModification, IAObjectModificationGetter> AObjectModification(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAObjectModification, IAObjectModificationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAObjectModificationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAObjectModification, IAObjectModificationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AObjectModification
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AObjectModification</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAObjectModification, IAObjectModificationGetter> AObjectModification(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAObjectModification, IAObjectModificationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAObjectModificationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAObjectModification, IAObjectModificationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Armor
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Armor</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IArmor, IArmorGetter> Armor(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IArmor, IArmorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IArmor, IArmorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Armor
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Armor</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IArmor, IArmorGetter> Armor(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IArmor, IArmorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IArmor, IArmorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ArmorAddon
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ArmorAddon</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IArmorAddon, IArmorAddonGetter> ArmorAddon(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IArmorAddon, IArmorAddonGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IArmorAddon, IArmorAddonGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ArmorAddon
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ArmorAddon</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IArmorAddon, IArmorAddonGetter> ArmorAddon(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IArmorAddon, IArmorAddonGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IArmorAddon, IArmorAddonGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ArtObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ArtObject</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IArtObject, IArtObjectGetter> ArtObject(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IArtObject, IArtObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IArtObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IArtObject, IArtObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ArtObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ArtObject</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IArtObject, IArtObjectGetter> ArtObject(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IArtObject, IArtObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IArtObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IArtObject, IArtObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AssociationType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AssociationType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAssociationType, IAssociationTypeGetter> AssociationType(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAssociationType, IAssociationTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAssociationTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAssociationType, IAssociationTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AssociationType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AssociationType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAssociationType, IAssociationTypeGetter> AssociationType(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAssociationType, IAssociationTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAssociationTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAssociationType, IAssociationTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AttractionRule
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AttractionRule</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAttractionRule, IAttractionRuleGetter> AttractionRule(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAttractionRule, IAttractionRuleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAttractionRuleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAttractionRule, IAttractionRuleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AttractionRule
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AttractionRule</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAttractionRule, IAttractionRuleGetter> AttractionRule(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAttractionRule, IAttractionRuleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAttractionRuleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAttractionRule, IAttractionRuleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ATXDefaultObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ATXDefaultObject</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IATXDefaultObject, IATXDefaultObjectGetter> ATXDefaultObject(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IATXDefaultObject, IATXDefaultObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IATXDefaultObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IATXDefaultObject, IATXDefaultObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ATXDefaultObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ATXDefaultObject</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IATXDefaultObject, IATXDefaultObjectGetter> ATXDefaultObject(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IATXDefaultObject, IATXDefaultObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IATXDefaultObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IATXDefaultObject, IATXDefaultObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AudioCategorySnapshot
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AudioCategorySnapshot</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAudioCategorySnapshot, IAudioCategorySnapshotGetter> AudioCategorySnapshot(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAudioCategorySnapshot, IAudioCategorySnapshotGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAudioCategorySnapshotGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAudioCategorySnapshot, IAudioCategorySnapshotGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AudioCategorySnapshot
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AudioCategorySnapshot</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAudioCategorySnapshot, IAudioCategorySnapshotGetter> AudioCategorySnapshot(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAudioCategorySnapshot, IAudioCategorySnapshotGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAudioCategorySnapshotGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAudioCategorySnapshot, IAudioCategorySnapshotGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AudioEffectChain
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AudioEffectChain</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAudioEffectChain, IAudioEffectChainGetter> AudioEffectChain(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAudioEffectChain, IAudioEffectChainGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAudioEffectChainGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAudioEffectChain, IAudioEffectChainGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AudioEffectChain
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AudioEffectChain</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAudioEffectChain, IAudioEffectChainGetter> AudioEffectChain(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAudioEffectChain, IAudioEffectChainGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAudioEffectChainGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAudioEffectChain, IAudioEffectChainGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AUVFUnknown
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AUVFUnknown</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAUVFUnknown, IAUVFUnknownGetter> AUVFUnknown(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAUVFUnknown, IAUVFUnknownGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAUVFUnknownGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAUVFUnknown, IAUVFUnknownGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AUVFUnknown
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AUVFUnknown</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAUVFUnknown, IAUVFUnknownGetter> AUVFUnknown(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAUVFUnknown, IAUVFUnknownGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAUVFUnknownGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAUVFUnknown, IAUVFUnknownGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Avatar
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Avatar</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAvatar, IAvatarGetter> Avatar(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAvatar, IAvatarGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAvatarGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAvatar, IAvatarGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Avatar
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Avatar</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAvatar, IAvatarGetter> Avatar(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IAvatar, IAvatarGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAvatarGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IAvatar, IAvatarGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BendableSpline
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on BendableSpline</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IBendableSpline, IBendableSplineGetter> BendableSpline(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IBendableSpline, IBendableSplineGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBendableSplineGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IBendableSpline, IBendableSplineGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BendableSpline
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on BendableSpline</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IBendableSpline, IBendableSplineGetter> BendableSpline(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IBendableSpline, IBendableSplineGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBendableSplineGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IBendableSpline, IBendableSplineGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BodyPartData
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on BodyPartData</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IBodyPartData, IBodyPartDataGetter> BodyPartData(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IBodyPartData, IBodyPartDataGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IBodyPartData, IBodyPartDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BodyPartData
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on BodyPartData</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IBodyPartData, IBodyPartDataGetter> BodyPartData(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IBodyPartData, IBodyPartDataGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IBodyPartData, IBodyPartDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Book
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Book</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IBook, IBookGetter> Book(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IBook, IBookGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IBook, IBookGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Book
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Book</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IBook, IBookGetter> Book(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IBook, IBookGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IBook, IBookGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraPath
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CameraPath</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICameraPath, ICameraPathGetter> CameraPath(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICameraPath, ICameraPathGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ICameraPath, ICameraPathGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraPath
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CameraPath</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICameraPath, ICameraPathGetter> CameraPath(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICameraPath, ICameraPathGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ICameraPath, ICameraPathGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraShot
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CameraShot</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICameraShot, ICameraShotGetter> CameraShot(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICameraShot, ICameraShotGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ICameraShot, ICameraShotGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraShot
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CameraShot</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICameraShot, ICameraShotGetter> CameraShot(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICameraShot, ICameraShotGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ICameraShot, ICameraShotGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CampTitle
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CampTitle</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICampTitle, ICampTitleGetter> CampTitle(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICampTitle, ICampTitleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICampTitleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ICampTitle, ICampTitleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CampTitle
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CampTitle</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICampTitle, ICampTitleGetter> CampTitle(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICampTitle, ICampTitleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICampTitleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ICampTitle, ICampTitleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Cell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Cell</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICell, ICellGetter> Cell(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICell, ICellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ICell, ICellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Cell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Cell</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICell, ICellGetter> Cell(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICell, ICellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ICell, ICellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Challenge
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Challenge</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IChallenge, IChallengeGetter> Challenge(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IChallenge, IChallengeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IChallengeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IChallenge, IChallengeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Challenge
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Challenge</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IChallenge, IChallengeGetter> Challenge(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IChallenge, IChallengeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IChallengeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IChallenge, IChallengeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ChallengePassRewardData
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ChallengePassRewardData</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IChallengePassRewardData, IChallengePassRewardDataGetter> ChallengePassRewardData(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IChallengePassRewardData, IChallengePassRewardDataGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IChallengePassRewardDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IChallengePassRewardData, IChallengePassRewardDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ChallengePassRewardData
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ChallengePassRewardData</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IChallengePassRewardData, IChallengePassRewardDataGetter> ChallengePassRewardData(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IChallengePassRewardData, IChallengePassRewardDataGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IChallengePassRewardDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IChallengePassRewardData, IChallengePassRewardDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Class
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Class</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IClass, IClassGetter> Class(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IClass, IClassGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IClass, IClassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Class
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Class</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IClass, IClassGetter> Class(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IClass, IClassGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IClass, IClassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Climate
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Climate</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IClimate, IClimateGetter> Climate(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IClimate, IClimateGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IClimate, IClimateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Climate
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Climate</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IClimate, IClimateGetter> Climate(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IClimate, IClimateGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IClimate, IClimateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CollisionLayer
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CollisionLayer</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICollisionLayer, ICollisionLayerGetter> CollisionLayer(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICollisionLayer, ICollisionLayerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICollisionLayerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ICollisionLayer, ICollisionLayerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CollisionLayer
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CollisionLayer</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICollisionLayer, ICollisionLayerGetter> CollisionLayer(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICollisionLayer, ICollisionLayerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICollisionLayerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ICollisionLayer, ICollisionLayerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ColorRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ColorRecord</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IColorRecord, IColorRecordGetter> ColorRecord(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IColorRecord, IColorRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IColorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IColorRecord, IColorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ColorRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ColorRecord</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IColorRecord, IColorRecordGetter> ColorRecord(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IColorRecord, IColorRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IColorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IColorRecord, IColorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CombatStyle
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CombatStyle</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICombatStyle, ICombatStyleGetter> CombatStyle(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICombatStyle, ICombatStyleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ICombatStyle, ICombatStyleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CombatStyle
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CombatStyle</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICombatStyle, ICombatStyleGetter> CombatStyle(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICombatStyle, ICombatStyleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ICombatStyle, ICombatStyleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Component
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Component</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IComponent, IComponentGetter> Component(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IComponent, IComponentGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IComponentGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IComponent, IComponentGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Component
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Component</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IComponent, IComponentGetter> Component(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IComponent, IComponentGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IComponentGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IComponent, IComponentGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ConditionForm
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ConditionForm</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IConditionForm, IConditionFormGetter> ConditionForm(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IConditionForm, IConditionFormGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IConditionFormGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IConditionForm, IConditionFormGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ConditionForm
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ConditionForm</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IConditionForm, IConditionFormGetter> ConditionForm(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IConditionForm, IConditionFormGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IConditionFormGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IConditionForm, IConditionFormGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ConstructibleObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ConstructibleObject</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IConstructibleObject, IConstructibleObjectGetter> ConstructibleObject(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IConstructibleObject, IConstructibleObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IConstructibleObject, IConstructibleObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ConstructibleObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ConstructibleObject</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IConstructibleObject, IConstructibleObjectGetter> ConstructibleObject(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IConstructibleObject, IConstructibleObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IConstructibleObject, IConstructibleObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ConsumableEntitlement
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ConsumableEntitlement</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IConsumableEntitlement, IConsumableEntitlementGetter> ConsumableEntitlement(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IConsumableEntitlement, IConsumableEntitlementGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IConsumableEntitlementGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IConsumableEntitlement, IConsumableEntitlementGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ConsumableEntitlement
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ConsumableEntitlement</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IConsumableEntitlement, IConsumableEntitlementGetter> ConsumableEntitlement(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IConsumableEntitlement, IConsumableEntitlementGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IConsumableEntitlementGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IConsumableEntitlement, IConsumableEntitlementGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Container
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Container</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IContainer, IContainerGetter> Container(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IContainer, IContainerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IContainer, IContainerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Container
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Container</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IContainer, IContainerGetter> Container(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IContainer, IContainerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IContainer, IContainerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CrateServiceEntitlement
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CrateServiceEntitlement</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICrateServiceEntitlement, ICrateServiceEntitlementGetter> CrateServiceEntitlement(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICrateServiceEntitlement, ICrateServiceEntitlementGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICrateServiceEntitlementGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ICrateServiceEntitlement, ICrateServiceEntitlementGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CrateServiceEntitlement
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CrateServiceEntitlement</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICrateServiceEntitlement, ICrateServiceEntitlementGetter> CrateServiceEntitlement(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICrateServiceEntitlement, ICrateServiceEntitlementGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICrateServiceEntitlementGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ICrateServiceEntitlement, ICrateServiceEntitlementGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Currency
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Currency</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICurrency, ICurrencyGetter> Currency(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICurrency, ICurrencyGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICurrencyGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ICurrency, ICurrencyGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Currency
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Currency</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICurrency, ICurrencyGetter> Currency(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICurrency, ICurrencyGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICurrencyGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ICurrency, ICurrencyGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CurveTable
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CurveTable</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICurveTable, ICurveTableGetter> CurveTable(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICurveTable, ICurveTableGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICurveTableGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ICurveTable, ICurveTableGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CurveTable
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CurveTable</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICurveTable, ICurveTableGetter> CurveTable(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ICurveTable, ICurveTableGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICurveTableGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ICurveTable, ICurveTableGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DailyContentGroup
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DailyContentGroup</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDailyContentGroup, IDailyContentGroupGetter> DailyContentGroup(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDailyContentGroup, IDailyContentGroupGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDailyContentGroupGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDailyContentGroup, IDailyContentGroupGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DailyContentGroup
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DailyContentGroup</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDailyContentGroup, IDailyContentGroupGetter> DailyContentGroup(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDailyContentGroup, IDailyContentGroupGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDailyContentGroupGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDailyContentGroup, IDailyContentGroupGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Debris
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Debris</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDebris, IDebrisGetter> Debris(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDebris, IDebrisGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDebrisGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDebris, IDebrisGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Debris
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Debris</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDebris, IDebrisGetter> Debris(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDebris, IDebrisGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDebrisGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDebris, IDebrisGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DefaultObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DefaultObject</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDefaultObject, IDefaultObjectGetter> DefaultObject(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDefaultObject, IDefaultObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDefaultObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDefaultObject, IDefaultObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DefaultObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DefaultObject</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDefaultObject, IDefaultObjectGetter> DefaultObject(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDefaultObject, IDefaultObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDefaultObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDefaultObject, IDefaultObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DefaultObjectManager
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DefaultObjectManager</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter> DefaultObjectManager(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DefaultObjectManager
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DefaultObjectManager</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter> DefaultObjectManager(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogBranch
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogBranch</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDialogBranch, IDialogBranchGetter> DialogBranch(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDialogBranch, IDialogBranchGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogBranchGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDialogBranch, IDialogBranchGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogBranch
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogBranch</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDialogBranch, IDialogBranchGetter> DialogBranch(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDialogBranch, IDialogBranchGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogBranchGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDialogBranch, IDialogBranchGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogResponses
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogResponses</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDialogResponses, IDialogResponsesGetter> DialogResponses(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDialogResponses, IDialogResponsesGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDialogResponses, IDialogResponsesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogResponses
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogResponses</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDialogResponses, IDialogResponsesGetter> DialogResponses(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDialogResponses, IDialogResponsesGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDialogResponses, IDialogResponsesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogTopic
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogTopic</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDialogTopic, IDialogTopicGetter> DialogTopic(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDialogTopic, IDialogTopicGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDialogTopic, IDialogTopicGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogTopic
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogTopic</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDialogTopic, IDialogTopicGetter> DialogTopic(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDialogTopic, IDialogTopicGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDialogTopic, IDialogTopicGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogView
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogView</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDialogView, IDialogViewGetter> DialogView(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDialogView, IDialogViewGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogViewGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDialogView, IDialogViewGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogView
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogView</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDialogView, IDialogViewGetter> DialogView(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDialogView, IDialogViewGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogViewGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDialogView, IDialogViewGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to District
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on District</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDistrict, IDistrictGetter> District(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDistrict, IDistrictGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDistrictGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDistrict, IDistrictGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to District
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on District</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDistrict, IDistrictGetter> District(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDistrict, IDistrictGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDistrictGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDistrict, IDistrictGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Door
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Door</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDoor, IDoorGetter> Door(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDoor, IDoorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDoor, IDoorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Door
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Door</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDoor, IDoorGetter> Door(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDoor, IDoorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDoor, IDoorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DualCastData
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DualCastData</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDualCastData, IDualCastDataGetter> DualCastData(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDualCastData, IDualCastDataGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDualCastDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDualCastData, IDualCastDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DualCastData
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DualCastData</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDualCastData, IDualCastDataGetter> DualCastData(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IDualCastData, IDualCastDataGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDualCastDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IDualCastData, IDualCastDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EffectShader
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EffectShader</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEffectShader, IEffectShaderGetter> EffectShader(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEffectShader, IEffectShaderGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IEffectShader, IEffectShaderGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EffectShader
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EffectShader</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEffectShader, IEffectShaderGetter> EffectShader(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEffectShader, IEffectShaderGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IEffectShader, IEffectShaderGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Emote
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Emote</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEmote, IEmoteGetter> Emote(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEmote, IEmoteGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEmoteGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IEmote, IEmoteGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Emote
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Emote</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEmote, IEmoteGetter> Emote(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEmote, IEmoteGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEmoteGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IEmote, IEmoteGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EmoteCategory
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EmoteCategory</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEmoteCategory, IEmoteCategoryGetter> EmoteCategory(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEmoteCategory, IEmoteCategoryGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEmoteCategoryGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IEmoteCategory, IEmoteCategoryGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EmoteCategory
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EmoteCategory</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEmoteCategory, IEmoteCategoryGetter> EmoteCategory(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEmoteCategory, IEmoteCategoryGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEmoteCategoryGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IEmoteCategory, IEmoteCategoryGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EncounterZone
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EncounterZone</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEncounterZone, IEncounterZoneGetter> EncounterZone(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEncounterZone, IEncounterZoneGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEncounterZoneGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IEncounterZone, IEncounterZoneGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EncounterZone
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EncounterZone</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEncounterZone, IEncounterZoneGetter> EncounterZone(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEncounterZone, IEncounterZoneGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEncounterZoneGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IEncounterZone, IEncounterZoneGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Entitlement
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Entitlement</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEntitlement, IEntitlementGetter> Entitlement(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEntitlement, IEntitlementGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEntitlementGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IEntitlement, IEntitlementGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Entitlement
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Entitlement</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEntitlement, IEntitlementGetter> Entitlement(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEntitlement, IEntitlementGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEntitlementGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IEntitlement, IEntitlementGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EquipType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EquipType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEquipType, IEquipTypeGetter> EquipType(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEquipType, IEquipTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEquipTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IEquipType, IEquipTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EquipType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EquipType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEquipType, IEquipTypeGetter> EquipType(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEquipType, IEquipTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEquipTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IEquipType, IEquipTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EventPlaylist
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EventPlaylist</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEventPlaylist, IEventPlaylistGetter> EventPlaylist(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEventPlaylist, IEventPlaylistGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEventPlaylistGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IEventPlaylist, IEventPlaylistGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EventPlaylist
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EventPlaylist</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEventPlaylist, IEventPlaylistGetter> EventPlaylist(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEventPlaylist, IEventPlaylistGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEventPlaylistGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IEventPlaylist, IEventPlaylistGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EventQuestWidget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EventQuestWidget</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEventQuestWidget, IEventQuestWidgetGetter> EventQuestWidget(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEventQuestWidget, IEventQuestWidgetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEventQuestWidgetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IEventQuestWidget, IEventQuestWidgetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EventQuestWidget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EventQuestWidget</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEventQuestWidget, IEventQuestWidgetGetter> EventQuestWidget(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEventQuestWidget, IEventQuestWidgetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEventQuestWidgetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IEventQuestWidget, IEventQuestWidgetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Explosion
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Explosion</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IExplosion, IExplosionGetter> Explosion(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IExplosion, IExplosionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IExplosionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IExplosion, IExplosionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Explosion
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Explosion</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IExplosion, IExplosionGetter> Explosion(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IExplosion, IExplosionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IExplosionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IExplosion, IExplosionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Eyes
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Eyes</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEyes, IEyesGetter> Eyes(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEyes, IEyesGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEyesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IEyes, IEyesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Eyes
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Eyes</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEyes, IEyesGetter> Eyes(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IEyes, IEyesGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEyesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IEyes, IEyesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Faction
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Faction</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFaction, IFactionGetter> Faction(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFaction, IFactionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IFaction, IFactionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Faction
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Faction</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFaction, IFactionGetter> Faction(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFaction, IFactionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IFaction, IFactionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Fallout76MajorRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Fallout76MajorRecord</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFallout76MajorRecord, IFallout76MajorRecordGetter> Fallout76MajorRecord(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFallout76MajorRecord, IFallout76MajorRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFallout76MajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IFallout76MajorRecord, IFallout76MajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Fallout76MajorRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Fallout76MajorRecord</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFallout76MajorRecord, IFallout76MajorRecordGetter> Fallout76MajorRecord(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFallout76MajorRecord, IFallout76MajorRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFallout76MajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IFallout76MajorRecord, IFallout76MajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Fish
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Fish</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFish, IFishGetter> Fish(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFish, IFishGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFishGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IFish, IFishGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Fish
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Fish</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFish, IFishGetter> Fish(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFish, IFishGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFishGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IFish, IFishGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Flora
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Flora</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFlora, IFloraGetter> Flora(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFlora, IFloraGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IFlora, IFloraGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Flora
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Flora</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFlora, IFloraGetter> Flora(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFlora, IFloraGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IFlora, IFloraGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Footstep
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Footstep</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFootstep, IFootstepGetter> Footstep(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFootstep, IFootstepGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFootstepGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IFootstep, IFootstepGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Footstep
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Footstep</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFootstep, IFootstepGetter> Footstep(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFootstep, IFootstepGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFootstepGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IFootstep, IFootstepGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FootstepSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on FootstepSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFootstepSet, IFootstepSetGetter> FootstepSet(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFootstepSet, IFootstepSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFootstepSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IFootstepSet, IFootstepSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FootstepSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on FootstepSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFootstepSet, IFootstepSetGetter> FootstepSet(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFootstepSet, IFootstepSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFootstepSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IFootstepSet, IFootstepSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FormList
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on FormList</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFormList, IFormListGetter> FormList(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFormList, IFormListGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFormListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IFormList, IFormListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FormList
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on FormList</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFormList, IFormListGetter> FormList(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFormList, IFormListGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFormListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IFormList, IFormListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Furniture
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Furniture</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFurniture, IFurnitureGetter> Furniture(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFurniture, IFurnitureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IFurniture, IFurnitureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Furniture
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Furniture</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFurniture, IFurnitureGetter> Furniture(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFurniture, IFurnitureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IFurniture, IFurnitureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameplayReward
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameplayReward</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGameplayReward, IGameplayRewardGetter> GameplayReward(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGameplayReward, IGameplayRewardGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameplayRewardGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IGameplayReward, IGameplayRewardGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameplayReward
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameplayReward</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGameplayReward, IGameplayRewardGetter> GameplayReward(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGameplayReward, IGameplayRewardGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameplayRewardGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IGameplayReward, IGameplayRewardGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Global
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Global</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGlobal, IGlobalGetter> Global(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGlobal, IGlobalGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IGlobal, IGlobalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Global
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Global</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGlobal, IGlobalGetter> Global(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGlobal, IGlobalGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IGlobal, IGlobalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GodRays
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GodRays</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGodRays, IGodRaysGetter> GodRays(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGodRays, IGodRaysGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGodRaysGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IGodRays, IGodRaysGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GodRays
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GodRays</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGodRays, IGodRaysGetter> GodRays(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGodRays, IGodRaysGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGodRaysGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IGodRays, IGodRaysGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Grass
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Grass</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGrass, IGrassGetter> Grass(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGrass, IGrassGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IGrass, IGrassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Grass
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Grass</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGrass, IGrassGetter> Grass(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGrass, IGrassGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IGrass, IGrassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GroundCover
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GroundCover</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGroundCover, IGroundCoverGetter> GroundCover(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGroundCover, IGroundCoverGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGroundCoverGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IGroundCover, IGroundCoverGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GroundCover
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GroundCover</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGroundCover, IGroundCoverGetter> GroundCover(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IGroundCover, IGroundCoverGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGroundCoverGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IGroundCover, IGroundCoverGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hazard
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Hazard</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IHazard, IHazardGetter> Hazard(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IHazard, IHazardGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHazardGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IHazard, IHazardGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hazard
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Hazard</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IHazard, IHazardGetter> Hazard(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IHazard, IHazardGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHazardGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IHazard, IHazardGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to HeadPart
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on HeadPart</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IHeadPart, IHeadPartGetter> HeadPart(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IHeadPart, IHeadPartGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IHeadPart, IHeadPartGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to HeadPart
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on HeadPart</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IHeadPart, IHeadPartGetter> HeadPart(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IHeadPart, IHeadPartGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IHeadPart, IHeadPartGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Holotape
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Holotape</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IHolotape, IHolotapeGetter> Holotape(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IHolotape, IHolotapeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHolotapeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IHolotape, IHolotapeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Holotape
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Holotape</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IHolotape, IHolotapeGetter> Holotape(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IHolotape, IHolotapeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHolotapeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IHolotape, IHolotapeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleAnimation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IdleAnimation</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IIdleAnimation, IIdleAnimationGetter> IdleAnimation(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IIdleAnimation, IIdleAnimationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IIdleAnimation, IIdleAnimationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleAnimation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IdleAnimation</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IIdleAnimation, IIdleAnimationGetter> IdleAnimation(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IIdleAnimation, IIdleAnimationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IIdleAnimation, IIdleAnimationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleMarker
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IdleMarker</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IIdleMarker, IIdleMarkerGetter> IdleMarker(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IIdleMarker, IIdleMarkerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IIdleMarker, IIdleMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleMarker
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IdleMarker</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IIdleMarker, IIdleMarkerGetter> IdleMarker(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IIdleMarker, IIdleMarkerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IIdleMarker, IIdleMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ImageSpace</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IImageSpace, IImageSpaceGetter> ImageSpace(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IImageSpace, IImageSpaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IImageSpace, IImageSpaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ImageSpace</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IImageSpace, IImageSpaceGetter> ImageSpace(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IImageSpace, IImageSpaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IImageSpace, IImageSpaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpaceAdapter
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ImageSpaceAdapter</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter> ImageSpaceAdapter(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpaceAdapter
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ImageSpaceAdapter</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter> ImageSpaceAdapter(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Impact
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Impact</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IImpact, IImpactGetter> Impact(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IImpact, IImpactGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImpactGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IImpact, IImpactGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Impact
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Impact</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IImpact, IImpactGetter> Impact(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IImpact, IImpactGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImpactGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IImpact, IImpactGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImpactDataSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ImpactDataSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IImpactDataSet, IImpactDataSetGetter> ImpactDataSet(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IImpactDataSet, IImpactDataSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IImpactDataSet, IImpactDataSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImpactDataSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ImpactDataSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IImpactDataSet, IImpactDataSetGetter> ImpactDataSet(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IImpactDataSet, IImpactDataSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IImpactDataSet, IImpactDataSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingestible
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ingestible</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IIngestible, IIngestibleGetter> Ingestible(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IIngestible, IIngestibleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IIngestible, IIngestibleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingestible
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ingestible</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IIngestible, IIngestibleGetter> Ingestible(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IIngestible, IIngestibleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IIngestible, IIngestibleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingredient
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ingredient</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IIngredient, IIngredientGetter> Ingredient(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IIngredient, IIngredientGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IIngredient, IIngredientGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingredient
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ingredient</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IIngredient, IIngredientGetter> Ingredient(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IIngredient, IIngredientGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IIngredient, IIngredientGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to InstanceNamingRules
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on InstanceNamingRules</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IInstanceNamingRules, IInstanceNamingRulesGetter> InstanceNamingRules(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IInstanceNamingRules, IInstanceNamingRulesGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IInstanceNamingRulesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IInstanceNamingRules, IInstanceNamingRulesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to InstanceNamingRules
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on InstanceNamingRules</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IInstanceNamingRules, IInstanceNamingRulesGetter> InstanceNamingRules(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IInstanceNamingRules, IInstanceNamingRulesGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IInstanceNamingRulesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IInstanceNamingRules, IInstanceNamingRulesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Key
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Key</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IKey, IKeyGetter> Key(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IKey, IKeyGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IKey, IKeyGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Key
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Key</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IKey, IKeyGetter> Key(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IKey, IKeyGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IKey, IKeyGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Keyword
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Keyword</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IKeyword, IKeywordGetter> Keyword(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IKeyword, IKeywordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeywordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IKeyword, IKeywordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Keyword
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Keyword</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IKeyword, IKeywordGetter> Keyword(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IKeyword, IKeywordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeywordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IKeyword, IKeywordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Landscape
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Landscape</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILandscape, ILandscapeGetter> Landscape(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILandscape, ILandscapeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILandscape, ILandscapeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Landscape
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Landscape</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILandscape, ILandscapeGetter> Landscape(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILandscape, ILandscapeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILandscape, ILandscapeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LandscapeTexture
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LandscapeTexture</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILandscapeTexture, ILandscapeTextureGetter> LandscapeTexture(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILandscapeTexture, ILandscapeTextureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILandscapeTexture, ILandscapeTextureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LandscapeTexture
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LandscapeTexture</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILandscapeTexture, ILandscapeTextureGetter> LandscapeTexture(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILandscapeTexture, ILandscapeTextureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILandscapeTexture, ILandscapeTextureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Layer
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Layer</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILayer, ILayerGetter> Layer(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILayer, ILayerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILayerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILayer, ILayerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Layer
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Layer</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILayer, ILayerGetter> Layer(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILayer, ILayerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILayerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILayer, ILayerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LegendaryItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LegendaryItem</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILegendaryItem, ILegendaryItemGetter> LegendaryItem(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILegendaryItem, ILegendaryItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILegendaryItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILegendaryItem, ILegendaryItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LegendaryItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LegendaryItem</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILegendaryItem, ILegendaryItemGetter> LegendaryItem(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILegendaryItem, ILegendaryItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILegendaryItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILegendaryItem, ILegendaryItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LensFlare
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LensFlare</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILensFlare, ILensFlareGetter> LensFlare(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILensFlare, ILensFlareGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILensFlareGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILensFlare, ILensFlareGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LensFlare
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LensFlare</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILensFlare, ILensFlareGetter> LensFlare(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILensFlare, ILensFlareGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILensFlareGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILensFlare, ILensFlareGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledItem</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILeveledItem, ILeveledItemGetter> LeveledItem(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILeveledItem, ILeveledItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILeveledItem, ILeveledItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledItem</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILeveledItem, ILeveledItemGetter> LeveledItem(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILeveledItem, ILeveledItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILeveledItem, ILeveledItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledNpc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledNpc</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILeveledNpc, ILeveledNpcGetter> LeveledNpc(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILeveledNpc, ILeveledNpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILeveledNpc, ILeveledNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledNpc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledNpc</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILeveledNpc, ILeveledNpcGetter> LeveledNpc(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILeveledNpc, ILeveledNpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILeveledNpc, ILeveledNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledPackIn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledPackIn</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILeveledPackIn, ILeveledPackInGetter> LeveledPackIn(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILeveledPackIn, ILeveledPackInGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledPackInGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILeveledPackIn, ILeveledPackInGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledPackIn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledPackIn</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILeveledPackIn, ILeveledPackInGetter> LeveledPackIn(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILeveledPackIn, ILeveledPackInGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledPackInGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILeveledPackIn, ILeveledPackInGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledPerkCard
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledPerkCard</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILeveledPerkCard, ILeveledPerkCardGetter> LeveledPerkCard(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILeveledPerkCard, ILeveledPerkCardGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledPerkCardGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILeveledPerkCard, ILeveledPerkCardGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledPerkCard
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledPerkCard</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILeveledPerkCard, ILeveledPerkCardGetter> LeveledPerkCard(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILeveledPerkCard, ILeveledPerkCardGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledPerkCardGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILeveledPerkCard, ILeveledPerkCardGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledSpell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledSpell</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILeveledSpell, ILeveledSpellGetter> LeveledSpell(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILeveledSpell, ILeveledSpellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILeveledSpell, ILeveledSpellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledSpell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledSpell</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILeveledSpell, ILeveledSpellGetter> LeveledSpell(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILeveledSpell, ILeveledSpellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILeveledSpell, ILeveledSpellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Light
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Light</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILight, ILightGetter> Light(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILight, ILightGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILight, ILightGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Light
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Light</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILight, ILightGetter> Light(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILight, ILightGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILight, ILightGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LightingTemplate
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LightingTemplate</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILightingTemplate, ILightingTemplateGetter> LightingTemplate(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILightingTemplate, ILightingTemplateGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILightingTemplate, ILightingTemplateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LightingTemplate
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LightingTemplate</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILightingTemplate, ILightingTemplateGetter> LightingTemplate(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILightingTemplate, ILightingTemplateGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILightingTemplate, ILightingTemplateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Loadout
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Loadout</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILoadout, ILoadoutGetter> Loadout(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILoadout, ILoadoutGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILoadoutGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILoadout, ILoadoutGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Loadout
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Loadout</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILoadout, ILoadoutGetter> Loadout(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILoadout, ILoadoutGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILoadoutGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILoadout, ILoadoutGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LoadScreen
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LoadScreen</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILoadScreen, ILoadScreenGetter> LoadScreen(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILoadScreen, ILoadScreenGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILoadScreen, ILoadScreenGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LoadScreen
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LoadScreen</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILoadScreen, ILoadScreenGetter> LoadScreen(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILoadScreen, ILoadScreenGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILoadScreen, ILoadScreenGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Location
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Location</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILocation, ILocationGetter> Location(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILocation, ILocationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILocationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILocation, ILocationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Location
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Location</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILocation, ILocationGetter> Location(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILocation, ILocationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILocationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILocation, ILocationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LocationReferenceType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LocationReferenceType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILocationReferenceType, ILocationReferenceTypeGetter> LocationReferenceType(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LocationReferenceType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LocationReferenceType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILocationReferenceType, ILocationReferenceTypeGetter> LocationReferenceType(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MagicEffect
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MagicEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMagicEffect, IMagicEffectGetter> MagicEffect(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMagicEffect, IMagicEffectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMagicEffect, IMagicEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MagicEffect
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MagicEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMagicEffect, IMagicEffectGetter> MagicEffect(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMagicEffect, IMagicEffectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMagicEffect, IMagicEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MainFileHeader
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MainFileHeader</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMainFileHeader, IMainFileHeaderGetter> MainFileHeader(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMainFileHeader, IMainFileHeaderGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMainFileHeaderGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMainFileHeader, IMainFileHeaderGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MainFileHeader
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MainFileHeader</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMainFileHeader, IMainFileHeaderGetter> MainFileHeader(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMainFileHeader, IMainFileHeaderGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMainFileHeaderGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMainFileHeader, IMainFileHeaderGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MaterialObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MaterialObject</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMaterialObject, IMaterialObjectGetter> MaterialObject(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMaterialObject, IMaterialObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMaterialObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMaterialObject, IMaterialObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MaterialObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MaterialObject</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMaterialObject, IMaterialObjectGetter> MaterialObject(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMaterialObject, IMaterialObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMaterialObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMaterialObject, IMaterialObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MaterialSwap
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MaterialSwap</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMaterialSwap, IMaterialSwapGetter> MaterialSwap(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMaterialSwap, IMaterialSwapGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMaterialSwapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMaterialSwap, IMaterialSwapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MaterialSwap
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MaterialSwap</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMaterialSwap, IMaterialSwapGetter> MaterialSwap(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMaterialSwap, IMaterialSwapGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMaterialSwapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMaterialSwap, IMaterialSwapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MaterialType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MaterialType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMaterialType, IMaterialTypeGetter> MaterialType(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMaterialType, IMaterialTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMaterialTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMaterialType, IMaterialTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MaterialType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MaterialType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMaterialType, IMaterialTypeGetter> MaterialType(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMaterialType, IMaterialTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMaterialTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMaterialType, IMaterialTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MenuIcon
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MenuIcon</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMenuIcon, IMenuIconGetter> MenuIcon(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMenuIcon, IMenuIconGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMenuIconGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMenuIcon, IMenuIconGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MenuIcon
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MenuIcon</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMenuIcon, IMenuIconGetter> MenuIcon(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMenuIcon, IMenuIconGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMenuIconGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMenuIcon, IMenuIconGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Message
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Message</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMessage, IMessageGetter> Message(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMessage, IMessageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMessageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMessage, IMessageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Message
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Message</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMessage, IMessageGetter> Message(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMessage, IMessageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMessageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMessage, IMessageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MiscItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MiscItem</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMiscItem, IMiscItemGetter> MiscItem(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMiscItem, IMiscItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMiscItem, IMiscItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MiscItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MiscItem</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMiscItem, IMiscItemGetter> MiscItem(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMiscItem, IMiscItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMiscItem, IMiscItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MiscItemSpawner
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MiscItemSpawner</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMiscItemSpawner, IMiscItemSpawnerGetter> MiscItemSpawner(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMiscItemSpawner, IMiscItemSpawnerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMiscItemSpawnerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMiscItemSpawner, IMiscItemSpawnerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MiscItemSpawner
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MiscItemSpawner</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMiscItemSpawner, IMiscItemSpawnerGetter> MiscItemSpawner(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMiscItemSpawner, IMiscItemSpawnerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMiscItemSpawnerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMiscItemSpawner, IMiscItemSpawnerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ModelSwap
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ModelSwap</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IModelSwap, IModelSwapGetter> ModelSwap(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IModelSwap, IModelSwapGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IModelSwapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IModelSwap, IModelSwapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ModelSwap
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ModelSwap</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IModelSwap, IModelSwapGetter> ModelSwap(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IModelSwap, IModelSwapGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IModelSwapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IModelSwap, IModelSwapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MovableStatic
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MovableStatic</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMovableStatic, IMovableStaticGetter> MovableStatic(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMovableStatic, IMovableStaticGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMovableStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMovableStatic, IMovableStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MovableStatic
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MovableStatic</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMovableStatic, IMovableStaticGetter> MovableStatic(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMovableStatic, IMovableStaticGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMovableStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMovableStatic, IMovableStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MovementType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MovementType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMovementType, IMovementTypeGetter> MovementType(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMovementType, IMovementTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMovementTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMovementType, IMovementTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MovementType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MovementType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMovementType, IMovementTypeGetter> MovementType(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMovementType, IMovementTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMovementTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMovementType, IMovementTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MusicTrack
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MusicTrack</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMusicTrack, IMusicTrackGetter> MusicTrack(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMusicTrack, IMusicTrackGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMusicTrackGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMusicTrack, IMusicTrackGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MusicTrack
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MusicTrack</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMusicTrack, IMusicTrackGetter> MusicTrack(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMusicTrack, IMusicTrackGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMusicTrackGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMusicTrack, IMusicTrackGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MusicType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MusicType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMusicType, IMusicTypeGetter> MusicType(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMusicType, IMusicTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMusicType, IMusicTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MusicType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MusicType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMusicType, IMusicTypeGetter> MusicType(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IMusicType, IMusicTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IMusicType, IMusicTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMesh
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on NavigationMesh</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, INavigationMesh, INavigationMeshGetter> NavigationMesh(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, INavigationMesh, INavigationMeshGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, INavigationMesh, INavigationMeshGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMesh
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on NavigationMesh</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, INavigationMesh, INavigationMeshGetter> NavigationMesh(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, INavigationMesh, INavigationMeshGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, INavigationMesh, INavigationMeshGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMeshInfoMap
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on NavigationMeshInfoMap</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter> NavigationMeshInfoMap(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMeshInfoMap
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on NavigationMeshInfoMap</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter> NavigationMeshInfoMap(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMeshObstacleManager
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on NavigationMeshObstacleManager</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, INavigationMeshObstacleManager, INavigationMeshObstacleManagerGetter> NavigationMeshObstacleManager(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, INavigationMeshObstacleManager, INavigationMeshObstacleManagerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INavigationMeshObstacleManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, INavigationMeshObstacleManager, INavigationMeshObstacleManagerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMeshObstacleManager
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on NavigationMeshObstacleManager</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, INavigationMeshObstacleManager, INavigationMeshObstacleManagerGetter> NavigationMeshObstacleManager(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, INavigationMeshObstacleManager, INavigationMeshObstacleManagerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INavigationMeshObstacleManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, INavigationMeshObstacleManager, INavigationMeshObstacleManagerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Navmesh
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Navmesh</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, INavmesh, INavmeshGetter> Navmesh(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, INavmesh, INavmeshGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INavmeshGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, INavmesh, INavmeshGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Navmesh
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Navmesh</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, INavmesh, INavmeshGetter> Navmesh(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, INavmesh, INavmeshGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INavmeshGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, INavmesh, INavmeshGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, INpc, INpcGetter> Npc(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, INpc, INpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, INpc, INpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, INpc, INpcGetter> Npc(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, INpc, INpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, INpc, INpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ObjectEffect
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ObjectEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IObjectEffect, IObjectEffectGetter> ObjectEffect(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IObjectEffect, IObjectEffectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IObjectEffect, IObjectEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ObjectEffect
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ObjectEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IObjectEffect, IObjectEffectGetter> ObjectEffect(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IObjectEffect, IObjectEffectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IObjectEffect, IObjectEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ObjectVisibilityManager
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ObjectVisibilityManager</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IObjectVisibilityManager, IObjectVisibilityManagerGetter> ObjectVisibilityManager(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IObjectVisibilityManager, IObjectVisibilityManagerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IObjectVisibilityManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IObjectVisibilityManager, IObjectVisibilityManagerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ObjectVisibilityManager
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ObjectVisibilityManager</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IObjectVisibilityManager, IObjectVisibilityManagerGetter> ObjectVisibilityManager(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IObjectVisibilityManager, IObjectVisibilityManagerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IObjectVisibilityManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IObjectVisibilityManager, IObjectVisibilityManagerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Outfit
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Outfit</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IOutfit, IOutfitGetter> Outfit(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IOutfit, IOutfitGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IOutfitGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IOutfit, IOutfitGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Outfit
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Outfit</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IOutfit, IOutfitGetter> Outfit(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IOutfit, IOutfitGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IOutfitGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IOutfit, IOutfitGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Package
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Package</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPackage, IPackageGetter> Package(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPackage, IPackageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPackageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPackage, IPackageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Package
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Package</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPackage, IPackageGetter> Package(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPackage, IPackageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPackageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPackage, IPackageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PackIn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PackIn</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPackIn, IPackInGetter> PackIn(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPackIn, IPackInGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPackInGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPackIn, IPackInGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PackIn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PackIn</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPackIn, IPackInGetter> PackIn(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPackIn, IPackInGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPackInGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPackIn, IPackInGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Perk
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Perk</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPerk, IPerkGetter> Perk(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPerk, IPerkGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPerkGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPerk, IPerkGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Perk
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Perk</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPerk, IPerkGetter> Perk(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPerk, IPerkGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPerkGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPerk, IPerkGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PerkCard
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PerkCard</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPerkCard, IPerkCardGetter> PerkCard(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPerkCard, IPerkCardGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPerkCardGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPerkCard, IPerkCardGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PerkCard
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PerkCard</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPerkCard, IPerkCardGetter> PerkCard(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPerkCard, IPerkCardGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPerkCardGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPerkCard, IPerkCardGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PerkCardPack
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PerkCardPack</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPerkCardPack, IPerkCardPackGetter> PerkCardPack(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPerkCardPack, IPerkCardPackGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPerkCardPackGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPerkCardPack, IPerkCardPackGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PerkCardPack
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PerkCardPack</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPerkCardPack, IPerkCardPackGetter> PerkCardPack(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPerkCardPack, IPerkCardPackGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPerkCardPackGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPerkCardPack, IPerkCardPackGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PhotoModeFeature
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PhotoModeFeature</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPhotoModeFeature, IPhotoModeFeatureGetter> PhotoModeFeature(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPhotoModeFeature, IPhotoModeFeatureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPhotoModeFeatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPhotoModeFeature, IPhotoModeFeatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PhotoModeFeature
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PhotoModeFeature</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPhotoModeFeature, IPhotoModeFeatureGetter> PhotoModeFeature(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPhotoModeFeature, IPhotoModeFeatureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPhotoModeFeatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPhotoModeFeature, IPhotoModeFeatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedNpc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedNpc</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlacedNpc, IPlacedNpcGetter> PlacedNpc(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlacedNpc, IPlacedNpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPlacedNpc, IPlacedNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedNpc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedNpc</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlacedNpc, IPlacedNpcGetter> PlacedNpc(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlacedNpc, IPlacedNpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPlacedNpc, IPlacedNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedObject</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlacedObject, IPlacedObjectGetter> PlacedObject(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlacedObject, IPlacedObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPlacedObject, IPlacedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedObject</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlacedObject, IPlacedObjectGetter> PlacedObject(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlacedObject, IPlacedObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPlacedObject, IPlacedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlayerReference
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlayerReference</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlayerReference, IPlayerReferenceGetter> PlayerReference(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlayerReference, IPlayerReferenceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlayerReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPlayerReference, IPlayerReferenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlayerReference
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlayerReference</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlayerReference, IPlayerReferenceGetter> PlayerReference(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlayerReference, IPlayerReferenceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlayerReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPlayerReference, IPlayerReferenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlayerTitle
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlayerTitle</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlayerTitle, IPlayerTitleGetter> PlayerTitle(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlayerTitle, IPlayerTitleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlayerTitleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPlayerTitle, IPlayerTitleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlayerTitle
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlayerTitle</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlayerTitle, IPlayerTitleGetter> PlayerTitle(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlayerTitle, IPlayerTitleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlayerTitleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPlayerTitle, IPlayerTitleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PowerArmorChassis
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PowerArmorChassis</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPowerArmorChassis, IPowerArmorChassisGetter> PowerArmorChassis(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPowerArmorChassis, IPowerArmorChassisGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPowerArmorChassisGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPowerArmorChassis, IPowerArmorChassisGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PowerArmorChassis
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PowerArmorChassis</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPowerArmorChassis, IPowerArmorChassisGetter> PowerArmorChassis(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPowerArmorChassis, IPowerArmorChassisGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPowerArmorChassisGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPowerArmorChassis, IPowerArmorChassisGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Projectile
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Projectile</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IProjectile, IProjectileGetter> Projectile(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IProjectile, IProjectileGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IProjectileGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IProjectile, IProjectileGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Projectile
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Projectile</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IProjectile, IProjectileGetter> Projectile(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IProjectile, IProjectileGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IProjectileGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IProjectile, IProjectileGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Quest
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Quest</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IQuest, IQuestGetter> Quest(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IQuest, IQuestGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IQuest, IQuestGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Quest
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Quest</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IQuest, IQuestGetter> Quest(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IQuest, IQuestGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IQuest, IQuestGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to QuestModule
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on QuestModule</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IQuestModule, IQuestModuleGetter> QuestModule(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IQuestModule, IQuestModuleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IQuestModuleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IQuestModule, IQuestModuleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to QuestModule
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on QuestModule</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IQuestModule, IQuestModuleGetter> QuestModule(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IQuestModule, IQuestModuleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IQuestModuleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IQuestModule, IQuestModuleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IRace, IRaceGetter> Race(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IRace, IRaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IRace, IRaceGetter> Race(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IRace, IRaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ReferenceGroup
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ReferenceGroup</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IReferenceGroup, IReferenceGroupGetter> ReferenceGroup(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IReferenceGroup, IReferenceGroupGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IReferenceGroupGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IReferenceGroup, IReferenceGroupGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ReferenceGroup
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ReferenceGroup</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IReferenceGroup, IReferenceGroupGetter> ReferenceGroup(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IReferenceGroup, IReferenceGroupGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IReferenceGroupGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IReferenceGroup, IReferenceGroupGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Region
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Region</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IRegion, IRegionGetter> Region(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IRegion, IRegionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IRegion, IRegionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Region
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Region</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IRegion, IRegionGetter> Region(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IRegion, IRegionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IRegion, IRegionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Relationship
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Relationship</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IRelationship, IRelationshipGetter> Relationship(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IRelationship, IRelationshipGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRelationshipGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IRelationship, IRelationshipGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Relationship
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Relationship</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IRelationship, IRelationshipGetter> Relationship(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IRelationship, IRelationshipGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRelationshipGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IRelationship, IRelationshipGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Resource
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Resource</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IResource, IResourceGetter> Resource(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IResource, IResourceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IResourceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IResource, IResourceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Resource
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Resource</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IResource, IResourceGetter> Resource(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IResource, IResourceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IResourceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IResource, IResourceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ReverbParameters
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ReverbParameters</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IReverbParameters, IReverbParametersGetter> ReverbParameters(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IReverbParameters, IReverbParametersGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IReverbParametersGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IReverbParameters, IReverbParametersGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ReverbParameters
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ReverbParameters</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IReverbParameters, IReverbParametersGetter> ReverbParameters(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IReverbParameters, IReverbParametersGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IReverbParametersGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IReverbParameters, IReverbParametersGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Scene
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Scene</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IScene, ISceneGetter> Scene(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IScene, ISceneGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISceneGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IScene, ISceneGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Scene
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Scene</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IScene, ISceneGetter> Scene(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IScene, ISceneGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISceneGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IScene, ISceneGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SceneCollection
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SceneCollection</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISceneCollection, ISceneCollectionGetter> SceneCollection(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISceneCollection, ISceneCollectionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISceneCollectionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISceneCollection, ISceneCollectionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SceneCollection
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SceneCollection</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISceneCollection, ISceneCollectionGetter> SceneCollection(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISceneCollection, ISceneCollectionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISceneCollectionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISceneCollection, ISceneCollectionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ShaderParticleGeometry
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ShaderParticleGeometry</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter> ShaderParticleGeometry(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IShaderParticleGeometryGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ShaderParticleGeometry
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ShaderParticleGeometry</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter> ShaderParticleGeometry(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IShaderParticleGeometryGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SnapTemplate
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SnapTemplate</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISnapTemplate, ISnapTemplateGetter> SnapTemplate(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISnapTemplate, ISnapTemplateGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISnapTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISnapTemplate, ISnapTemplateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SnapTemplate
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SnapTemplate</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISnapTemplate, ISnapTemplateGetter> SnapTemplate(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISnapTemplate, ISnapTemplateGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISnapTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISnapTemplate, ISnapTemplateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SnapTemplateNode
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SnapTemplateNode</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISnapTemplateNode, ISnapTemplateNodeGetter> SnapTemplateNode(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISnapTemplateNode, ISnapTemplateNodeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISnapTemplateNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISnapTemplateNode, ISnapTemplateNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SnapTemplateNode
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SnapTemplateNode</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISnapTemplateNode, ISnapTemplateNodeGetter> SnapTemplateNode(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISnapTemplateNode, ISnapTemplateNodeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISnapTemplateNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISnapTemplateNode, ISnapTemplateNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundCategory
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoundCategory</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundCategory, ISoundCategoryGetter> SoundCategory(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundCategory, ISoundCategoryGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundCategoryGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISoundCategory, ISoundCategoryGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundCategory
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoundCategory</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundCategory, ISoundCategoryGetter> SoundCategory(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundCategory, ISoundCategoryGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundCategoryGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISoundCategory, ISoundCategoryGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundDescriptor
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoundDescriptor</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundDescriptor, ISoundDescriptorGetter> SoundDescriptor(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundDescriptor, ISoundDescriptorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundDescriptorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISoundDescriptor, ISoundDescriptorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundDescriptor
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoundDescriptor</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundDescriptor, ISoundDescriptorGetter> SoundDescriptor(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundDescriptor, ISoundDescriptorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundDescriptorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISoundDescriptor, ISoundDescriptorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundEchoMarker
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoundEchoMarker</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundEchoMarker, ISoundEchoMarkerGetter> SoundEchoMarker(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundEchoMarker, ISoundEchoMarkerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundEchoMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISoundEchoMarker, ISoundEchoMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundEchoMarker
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoundEchoMarker</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundEchoMarker, ISoundEchoMarkerGetter> SoundEchoMarker(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundEchoMarker, ISoundEchoMarkerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundEchoMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISoundEchoMarker, ISoundEchoMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundKeywordMapping
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoundKeywordMapping</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundKeywordMapping, ISoundKeywordMappingGetter> SoundKeywordMapping(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundKeywordMapping, ISoundKeywordMappingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundKeywordMappingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISoundKeywordMapping, ISoundKeywordMappingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundKeywordMapping
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoundKeywordMapping</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundKeywordMapping, ISoundKeywordMappingGetter> SoundKeywordMapping(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundKeywordMapping, ISoundKeywordMappingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundKeywordMappingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISoundKeywordMapping, ISoundKeywordMappingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundMarker
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoundMarker</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundMarker, ISoundMarkerGetter> SoundMarker(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundMarker, ISoundMarkerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISoundMarker, ISoundMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundMarker
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoundMarker</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundMarker, ISoundMarkerGetter> SoundMarker(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundMarker, ISoundMarkerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISoundMarker, ISoundMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundOutputModel
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoundOutputModel</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundOutputModel, ISoundOutputModelGetter> SoundOutputModel(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundOutputModel, ISoundOutputModelGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundOutputModelGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISoundOutputModel, ISoundOutputModelGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundOutputModel
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoundOutputModel</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundOutputModel, ISoundOutputModelGetter> SoundOutputModel(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISoundOutputModel, ISoundOutputModelGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundOutputModelGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISoundOutputModel, ISoundOutputModelGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Spell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Spell</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISpell, ISpellGetter> Spell(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISpell, ISpellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISpell, ISpellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Spell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Spell</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISpell, ISpellGetter> Spell(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISpell, ISpellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISpell, ISpellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SpellThresholdData
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SpellThresholdData</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISpellThresholdData, ISpellThresholdDataGetter> SpellThresholdData(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISpellThresholdData, ISpellThresholdDataGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISpellThresholdDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISpellThresholdData, ISpellThresholdDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SpellThresholdData
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SpellThresholdData</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISpellThresholdData, ISpellThresholdDataGetter> SpellThresholdData(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ISpellThresholdData, ISpellThresholdDataGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISpellThresholdDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ISpellThresholdData, ISpellThresholdDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Static
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Static</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStatic, IStaticGetter> Static(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStatic, IStaticGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IStatic, IStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Static
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Static</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStatic, IStaticGetter> Static(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStatic, IStaticGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IStatic, IStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StaticCollection
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on StaticCollection</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStaticCollection, IStaticCollectionGetter> StaticCollection(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStaticCollection, IStaticCollectionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStaticCollectionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IStaticCollection, IStaticCollectionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StaticCollection
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on StaticCollection</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStaticCollection, IStaticCollectionGetter> StaticCollection(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStaticCollection, IStaticCollectionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStaticCollectionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IStaticCollection, IStaticCollectionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StoryManagerBranchNode
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on StoryManagerBranchNode</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStoryManagerBranchNode, IStoryManagerBranchNodeGetter> StoryManagerBranchNode(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStoryManagerBranchNode, IStoryManagerBranchNodeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStoryManagerBranchNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IStoryManagerBranchNode, IStoryManagerBranchNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StoryManagerBranchNode
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on StoryManagerBranchNode</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStoryManagerBranchNode, IStoryManagerBranchNodeGetter> StoryManagerBranchNode(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStoryManagerBranchNode, IStoryManagerBranchNodeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStoryManagerBranchNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IStoryManagerBranchNode, IStoryManagerBranchNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StoryManagerEventNode
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on StoryManagerEventNode</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStoryManagerEventNode, IStoryManagerEventNodeGetter> StoryManagerEventNode(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStoryManagerEventNode, IStoryManagerEventNodeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStoryManagerEventNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IStoryManagerEventNode, IStoryManagerEventNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StoryManagerEventNode
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on StoryManagerEventNode</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStoryManagerEventNode, IStoryManagerEventNodeGetter> StoryManagerEventNode(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStoryManagerEventNode, IStoryManagerEventNodeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStoryManagerEventNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IStoryManagerEventNode, IStoryManagerEventNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StoryManagerQuestNode
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on StoryManagerQuestNode</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter> StoryManagerQuestNode(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStoryManagerQuestNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StoryManagerQuestNode
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on StoryManagerQuestNode</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter> StoryManagerQuestNode(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStoryManagerQuestNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TalkingActivator
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on TalkingActivator</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITalkingActivator, ITalkingActivatorGetter> TalkingActivator(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITalkingActivator, ITalkingActivatorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITalkingActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ITalkingActivator, ITalkingActivatorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TalkingActivator
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on TalkingActivator</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITalkingActivator, ITalkingActivatorGetter> TalkingActivator(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITalkingActivator, ITalkingActivatorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITalkingActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ITalkingActivator, ITalkingActivatorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Terminal
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Terminal</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITerminal, ITerminalGetter> Terminal(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITerminal, ITerminalGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITerminalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ITerminal, ITerminalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Terminal
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Terminal</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITerminal, ITerminalGetter> Terminal(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITerminal, ITerminalGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITerminalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ITerminal, ITerminalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TextureSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on TextureSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITextureSet, ITextureSetGetter> TextureSet(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITextureSet, ITextureSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ITextureSet, ITextureSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TextureSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on TextureSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITextureSet, ITextureSetGetter> TextureSet(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITextureSet, ITextureSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ITextureSet, ITextureSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Toft
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Toft</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IToft, IToftGetter> Toft(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IToft, IToftGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IToftGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IToft, IToftGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Toft
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Toft</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IToft, IToftGetter> Toft(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IToft, IToftGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IToftGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IToft, IToftGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Transform
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Transform</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITransform, ITransformGetter> Transform(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITransform, ITransformGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITransformGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ITransform, ITransformGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Transform
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Transform</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITransform, ITransformGetter> Transform(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITransform, ITransformGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITransformGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ITransform, ITransformGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Trap
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Trap</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITrap, ITrapGetter> Trap(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITrap, ITrapGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITrapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ITrap, ITrapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Trap
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Trap</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITrap, ITrapGetter> Trap(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITrap, ITrapGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITrapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ITrap, ITrapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Tree
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Tree</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITree, ITreeGetter> Tree(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITree, ITreeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ITree, ITreeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Tree
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Tree</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITree, ITreeGetter> Tree(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, ITree, ITreeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, ITree, ITreeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to UnknownASTM
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on UnknownASTM</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IUnknownASTM, IUnknownASTMGetter> UnknownASTM(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IUnknownASTM, IUnknownASTMGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IUnknownASTMGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IUnknownASTM, IUnknownASTMGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to UnknownASTM
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on UnknownASTM</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IUnknownASTM, IUnknownASTMGetter> UnknownASTM(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IUnknownASTM, IUnknownASTMGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IUnknownASTMGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IUnknownASTM, IUnknownASTMGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Utility
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Utility</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IUtility, IUtilityGetter> Utility(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IUtility, IUtilityGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IUtilityGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IUtility, IUtilityGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Utility
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Utility</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IUtility, IUtilityGetter> Utility(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IUtility, IUtilityGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IUtilityGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IUtility, IUtilityGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VisualEffect
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on VisualEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IVisualEffect, IVisualEffectGetter> VisualEffect(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IVisualEffect, IVisualEffectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IVisualEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IVisualEffect, IVisualEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VisualEffect
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on VisualEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IVisualEffect, IVisualEffectGetter> VisualEffect(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IVisualEffect, IVisualEffectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IVisualEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IVisualEffect, IVisualEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VoiceType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on VoiceType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IVoiceType, IVoiceTypeGetter> VoiceType(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IVoiceType, IVoiceTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IVoiceType, IVoiceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VoiceType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on VoiceType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IVoiceType, IVoiceTypeGetter> VoiceType(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IVoiceType, IVoiceTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IVoiceType, IVoiceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VolumetricLighting
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on VolumetricLighting</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IVolumetricLighting, IVolumetricLightingGetter> VolumetricLighting(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IVolumetricLighting, IVolumetricLightingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IVolumetricLightingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IVolumetricLighting, IVolumetricLightingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VolumetricLighting
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on VolumetricLighting</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IVolumetricLighting, IVolumetricLightingGetter> VolumetricLighting(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IVolumetricLighting, IVolumetricLightingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IVolumetricLightingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IVolumetricLighting, IVolumetricLightingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Water
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Water</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWater, IWaterGetter> Water(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWater, IWaterGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IWater, IWaterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Water
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Water</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWater, IWaterGetter> Water(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWater, IWaterGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IWater, IWaterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to WaveEncounter
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on WaveEncounter</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWaveEncounter, IWaveEncounterGetter> WaveEncounter(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWaveEncounter, IWaveEncounterGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWaveEncounterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IWaveEncounter, IWaveEncounterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to WaveEncounter
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on WaveEncounter</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWaveEncounter, IWaveEncounterGetter> WaveEncounter(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWaveEncounter, IWaveEncounterGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWaveEncounterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IWaveEncounter, IWaveEncounterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weapon
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Weapon</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWeapon, IWeaponGetter> Weapon(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWeapon, IWeaponGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IWeapon, IWeaponGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weapon
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Weapon</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWeapon, IWeaponGetter> Weapon(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWeapon, IWeaponGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IWeapon, IWeaponGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weather
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Weather</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWeather, IWeatherGetter> Weather(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWeather, IWeatherGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IWeather, IWeatherGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weather
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Weather</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWeather, IWeatherGetter> Weather(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWeather, IWeatherGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IWeather, IWeatherGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to WorkshopPermissions
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on WorkshopPermissions</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWorkshopPermissions, IWorkshopPermissionsGetter> WorkshopPermissions(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWorkshopPermissions, IWorkshopPermissionsGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWorkshopPermissionsGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IWorkshopPermissions, IWorkshopPermissionsGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to WorkshopPermissions
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on WorkshopPermissions</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWorkshopPermissions, IWorkshopPermissionsGetter> WorkshopPermissions(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWorkshopPermissions, IWorkshopPermissionsGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWorkshopPermissionsGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IWorkshopPermissions, IWorkshopPermissionsGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Worldspace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Worldspace</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWorldspace, IWorldspaceGetter> Worldspace(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWorldspace, IWorldspaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IWorldspace, IWorldspaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Worldspace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Worldspace</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWorldspace, IWorldspaceGetter> Worldspace(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IWorldspace, IWorldspaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IWorldspace, IWorldspaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Zoom
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Zoom</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IZoom, IZoomGetter> Zoom(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IZoom, IZoomGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IZoomGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IZoom, IZoomGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Zoom
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Zoom</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IZoom, IZoomGetter> Zoom(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IZoom, IZoomGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IZoomGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IZoom, IZoomGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

        #region Link Interfaces
        /// <summary>
        /// Scope a load order query to IComplexLocation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IComplexLocation</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IComplexLocation, IComplexLocationGetter> IComplexLocation(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IComplexLocation, IComplexLocationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IComplexLocationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IComplexLocation, IComplexLocationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IComplexLocation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IComplexLocation</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IComplexLocation, IComplexLocationGetter> IComplexLocation(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IComplexLocation, IComplexLocationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IComplexLocationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IComplexLocation, IComplexLocationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaceableObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlaceableObject</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlaceableObject, IPlaceableObjectGetter> IPlaceableObject(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlaceableObject, IPlaceableObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlaceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPlaceableObject, IPlaceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaceableObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlaceableObject</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlaceableObject, IPlaceableObjectGetter> IPlaceableObject(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlaceableObject, IPlaceableObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlaceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPlaceableObject, IPlaceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IReferenceableObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IReferenceableObject</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IReferenceableObject, IReferenceableObjectGetter> IReferenceableObject(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IReferenceableObject, IReferenceableObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IReferenceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IReferenceableObject, IReferenceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IReferenceableObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IReferenceableObject</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IReferenceableObject, IReferenceableObjectGetter> IReferenceableObject(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IReferenceableObject, IReferenceableObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IReferenceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IReferenceableObject, IReferenceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPreCutMapEntryReference
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPreCutMapEntryReference</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPreCutMapEntryReference, IPreCutMapEntryReferenceGetter> IPreCutMapEntryReference(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPreCutMapEntryReference, IPreCutMapEntryReferenceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPreCutMapEntryReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPreCutMapEntryReference, IPreCutMapEntryReferenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPreCutMapEntryReference
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPreCutMapEntryReference</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPreCutMapEntryReference, IPreCutMapEntryReferenceGetter> IPreCutMapEntryReference(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPreCutMapEntryReference, IPreCutMapEntryReferenceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPreCutMapEntryReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPreCutMapEntryReference, IPreCutMapEntryReferenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IObjectId
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IObjectId</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IObjectId, IObjectIdGetter> IObjectId(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IObjectId, IObjectIdGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IObjectIdGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IObjectId, IObjectIdGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IObjectId
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IObjectId</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IObjectId, IObjectIdGetter> IObjectId(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IObjectId, IObjectIdGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IObjectIdGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IObjectId, IObjectIdGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IConstructibleObjectTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IConstructibleObjectTarget</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IConstructibleObjectTarget, IConstructibleObjectTargetGetter> IConstructibleObjectTarget(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IConstructibleObjectTarget, IConstructibleObjectTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IConstructibleObjectTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IConstructibleObjectTarget, IConstructibleObjectTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IConstructibleObjectTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IConstructibleObjectTarget</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IConstructibleObjectTarget, IConstructibleObjectTargetGetter> IConstructibleObjectTarget(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IConstructibleObjectTarget, IConstructibleObjectTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IConstructibleObjectTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IConstructibleObjectTarget, IConstructibleObjectTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaced
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlaced</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlaced, IPlacedGetter> IPlaced(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlaced, IPlacedGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPlaced, IPlacedGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaced
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlaced</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlaced, IPlacedGetter> IPlaced(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlaced, IPlacedGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPlaced, IPlacedGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlacedSimple
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlacedSimple</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlacedSimple, IPlacedSimpleGetter> IPlacedSimple(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlacedSimple, IPlacedSimpleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedSimpleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPlacedSimple, IPlacedSimpleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlacedSimple
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlacedSimple</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlacedSimple, IPlacedSimpleGetter> IPlacedSimple(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlacedSimple, IPlacedSimpleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedSimpleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPlacedSimple, IPlacedSimpleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IKeywordLinkedReference
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IKeywordLinkedReference</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter> IKeywordLinkedReference(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IKeywordLinkedReference
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IKeywordLinkedReference</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter> IKeywordLinkedReference(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlacedThing
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlacedThing</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlacedThing, IPlacedThingGetter> IPlacedThing(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlacedThing, IPlacedThingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedThingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPlacedThing, IPlacedThingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlacedThing
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlacedThing</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlacedThing, IPlacedThingGetter> IPlacedThing(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IPlacedThing, IPlacedThingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedThingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IPlacedThing, IPlacedThingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IBindableEquipment
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IBindableEquipment</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IBindableEquipment, IBindableEquipmentGetter> IBindableEquipment(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IBindableEquipment, IBindableEquipmentGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBindableEquipmentGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IBindableEquipment, IBindableEquipmentGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IBindableEquipment
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IBindableEquipment</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IBindableEquipment, IBindableEquipmentGetter> IBindableEquipment(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IBindableEquipment, IBindableEquipmentGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBindableEquipmentGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IBindableEquipment, IBindableEquipmentGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IStaticTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IStaticTarget</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStaticTarget, IStaticTargetGetter> IStaticTarget(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStaticTarget, IStaticTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStaticTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IStaticTarget, IStaticTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IStaticTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IStaticTarget</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStaticTarget, IStaticTargetGetter> IStaticTarget(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IStaticTarget, IStaticTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStaticTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IStaticTarget, IStaticTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IHarvestTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IHarvestTarget</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IHarvestTarget, IHarvestTargetGetter> IHarvestTarget(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IHarvestTarget, IHarvestTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHarvestTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IHarvestTarget, IHarvestTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IHarvestTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IHarvestTarget</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IHarvestTarget, IHarvestTargetGetter> IHarvestTarget(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IHarvestTarget, IHarvestTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHarvestTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IHarvestTarget, IHarvestTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IFurnitureAssociation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IFurnitureAssociation</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFurnitureAssociation, IFurnitureAssociationGetter> IFurnitureAssociation(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFurnitureAssociation, IFurnitureAssociationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFurnitureAssociationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IFurnitureAssociation, IFurnitureAssociationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IFurnitureAssociation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IFurnitureAssociation</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFurnitureAssociation, IFurnitureAssociationGetter> IFurnitureAssociation(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IFurnitureAssociation, IFurnitureAssociationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFurnitureAssociationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IFurnitureAssociation, IFurnitureAssociationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IExplodeSpawn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IExplodeSpawn</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IExplodeSpawn, IExplodeSpawnGetter> IExplodeSpawn(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IExplodeSpawn, IExplodeSpawnGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IExplodeSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IExplodeSpawn, IExplodeSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IExplodeSpawn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IExplodeSpawn</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IExplodeSpawn, IExplodeSpawnGetter> IExplodeSpawn(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IExplodeSpawn, IExplodeSpawnGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IExplodeSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IExplodeSpawn, IExplodeSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IItem</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IItem, IItemGetter> IItem(this IEnumerable<IModListingGetter<IFallout76ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IItem, IItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IItem, IItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IItem</returns>
        public static TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IItem, IItemGetter> IItem(this IEnumerable<IFallout76ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout76Mod, IFallout76ModGetter, IItem, IItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout76Mod, IFallout76ModGetter, IItem, IItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

    }
}
