using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.Starfield
{
    public static class TypeOptionSolidifierMixIns
    {
        #region Normal
        /// <summary>
        /// Scope a load order query to AcousticSpace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AcousticSpace</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAcousticSpace, IAcousticSpaceGetter> AcousticSpace(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAcousticSpace, IAcousticSpaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAcousticSpace, IAcousticSpaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AcousticSpace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AcousticSpace</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAcousticSpace, IAcousticSpaceGetter> AcousticSpace(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAcousticSpace, IAcousticSpaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAcousticSpace, IAcousticSpaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActionRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ActionRecord</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActionRecord, IActionRecordGetter> ActionRecord(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActionRecord, IActionRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IActionRecord, IActionRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActionRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ActionRecord</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActionRecord, IActionRecordGetter> ActionRecord(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActionRecord, IActionRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActionRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IActionRecord, IActionRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Activator
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Activator</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActivator, IActivatorGetter> Activator(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActivator, IActivatorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IActivator, IActivatorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Activator
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Activator</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActivator, IActivatorGetter> Activator(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActivator, IActivatorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IActivator, IActivatorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActorValueInformation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ActorValueInformation</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActorValueInformation, IActorValueInformationGetter> ActorValueInformation(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActorValueInformation, IActorValueInformationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IActorValueInformation, IActorValueInformationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActorValueInformation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ActorValueInformation</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActorValueInformation, IActorValueInformationGetter> ActorValueInformation(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActorValueInformation, IActorValueInformationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IActorValueInformation, IActorValueInformationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActorValueModulation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ActorValueModulation</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActorValueModulation, IActorValueModulationGetter> ActorValueModulation(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActorValueModulation, IActorValueModulationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActorValueModulationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IActorValueModulation, IActorValueModulationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActorValueModulation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ActorValueModulation</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActorValueModulation, IActorValueModulationGetter> ActorValueModulation(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IActorValueModulation, IActorValueModulationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActorValueModulationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IActorValueModulation, IActorValueModulationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AddonNode
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AddonNode</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAddonNode, IAddonNodeGetter> AddonNode(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAddonNode, IAddonNodeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAddonNode, IAddonNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AddonNode
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AddonNode</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAddonNode, IAddonNodeGetter> AddonNode(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAddonNode, IAddonNodeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAddonNode, IAddonNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AffinityEvent
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AffinityEvent</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAffinityEvent, IAffinityEventGetter> AffinityEvent(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAffinityEvent, IAffinityEventGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAffinityEventGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAffinityEvent, IAffinityEventGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AffinityEvent
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AffinityEvent</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAffinityEvent, IAffinityEventGetter> AffinityEvent(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAffinityEvent, IAffinityEventGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAffinityEventGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAffinityEvent, IAffinityEventGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AimAssistModel
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AimAssistModel</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAimAssistModel, IAimAssistModelGetter> AimAssistModel(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAimAssistModel, IAimAssistModelGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAimAssistModelGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAimAssistModel, IAimAssistModelGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AimAssistModel
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AimAssistModel</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAimAssistModel, IAimAssistModelGetter> AimAssistModel(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAimAssistModel, IAimAssistModelGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAimAssistModelGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAimAssistModel, IAimAssistModelGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AimAssistPose
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AimAssistPose</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAimAssistPose, IAimAssistPoseGetter> AimAssistPose(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAimAssistPose, IAimAssistPoseGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAimAssistPoseGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAimAssistPose, IAimAssistPoseGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AimAssistPose
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AimAssistPose</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAimAssistPose, IAimAssistPoseGetter> AimAssistPose(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAimAssistPose, IAimAssistPoseGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAimAssistPoseGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAimAssistPose, IAimAssistPoseGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AimModel
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AimModel</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAimModel, IAimModelGetter> AimModel(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAimModel, IAimModelGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAimModelGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAimModel, IAimModelGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AimModel
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AimModel</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAimModel, IAimModelGetter> AimModel(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAimModel, IAimModelGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAimModelGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAimModel, IAimModelGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AimOpticalSightMarker
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AimOpticalSightMarker</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAimOpticalSightMarker, IAimOpticalSightMarkerGetter> AimOpticalSightMarker(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAimOpticalSightMarker, IAimOpticalSightMarkerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAimOpticalSightMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAimOpticalSightMarker, IAimOpticalSightMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AimOpticalSightMarker
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AimOpticalSightMarker</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAimOpticalSightMarker, IAimOpticalSightMarkerGetter> AimOpticalSightMarker(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAimOpticalSightMarker, IAimOpticalSightMarkerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAimOpticalSightMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAimOpticalSightMarker, IAimOpticalSightMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AmbienceSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AmbienceSet</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAmbienceSet, IAmbienceSetGetter> AmbienceSet(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAmbienceSet, IAmbienceSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAmbienceSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAmbienceSet, IAmbienceSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AmbienceSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AmbienceSet</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAmbienceSet, IAmbienceSetGetter> AmbienceSet(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAmbienceSet, IAmbienceSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAmbienceSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAmbienceSet, IAmbienceSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ammunition
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ammunition</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAmmunition, IAmmunitionGetter> Ammunition(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAmmunition, IAmmunitionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAmmunition, IAmmunitionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ammunition
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ammunition</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAmmunition, IAmmunitionGetter> Ammunition(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAmmunition, IAmmunitionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAmmunition, IAmmunitionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AnimatedObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AnimatedObject</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAnimatedObject, IAnimatedObjectGetter> AnimatedObject(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAnimatedObject, IAnimatedObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAnimatedObject, IAnimatedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AnimatedObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AnimatedObject</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAnimatedObject, IAnimatedObjectGetter> AnimatedObject(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAnimatedObject, IAnimatedObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAnimatedObject, IAnimatedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AnimationSoundTagSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AnimationSoundTagSet</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAnimationSoundTagSet, IAnimationSoundTagSetGetter> AnimationSoundTagSet(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAnimationSoundTagSet, IAnimationSoundTagSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAnimationSoundTagSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAnimationSoundTagSet, IAnimationSoundTagSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AnimationSoundTagSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AnimationSoundTagSet</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAnimationSoundTagSet, IAnimationSoundTagSetGetter> AnimationSoundTagSet(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAnimationSoundTagSet, IAnimationSoundTagSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAnimationSoundTagSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAnimationSoundTagSet, IAnimationSoundTagSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AObjectModification
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AObjectModification</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAObjectModification, IAObjectModificationGetter> AObjectModification(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAObjectModification, IAObjectModificationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAObjectModificationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAObjectModification, IAObjectModificationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AObjectModification
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AObjectModification</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAObjectModification, IAObjectModificationGetter> AObjectModification(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAObjectModification, IAObjectModificationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAObjectModificationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAObjectModification, IAObjectModificationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to APlacedTrap
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on APlacedTrap</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAPlacedTrap, IAPlacedTrapGetter> APlacedTrap(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAPlacedTrap, IAPlacedTrapGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAPlacedTrap, IAPlacedTrapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to APlacedTrap
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on APlacedTrap</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAPlacedTrap, IAPlacedTrapGetter> APlacedTrap(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAPlacedTrap, IAPlacedTrapGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAPlacedTrapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAPlacedTrap, IAPlacedTrapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Armor
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Armor</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IArmor, IArmorGetter> Armor(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IArmor, IArmorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IArmor, IArmorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Armor
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Armor</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IArmor, IArmorGetter> Armor(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IArmor, IArmorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IArmor, IArmorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ArmorAddon
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ArmorAddon</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IArmorAddon, IArmorAddonGetter> ArmorAddon(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IArmorAddon, IArmorAddonGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IArmorAddon, IArmorAddonGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ArmorAddon
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ArmorAddon</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IArmorAddon, IArmorAddonGetter> ArmorAddon(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IArmorAddon, IArmorAddonGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IArmorAddon, IArmorAddonGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ArtObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ArtObject</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IArtObject, IArtObjectGetter> ArtObject(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IArtObject, IArtObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IArtObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IArtObject, IArtObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ArtObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ArtObject</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IArtObject, IArtObjectGetter> ArtObject(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IArtObject, IArtObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IArtObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IArtObject, IArtObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AStoryManagerNode
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AStoryManagerNode</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAStoryManagerNode, IAStoryManagerNodeGetter> AStoryManagerNode(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAStoryManagerNode, IAStoryManagerNodeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAStoryManagerNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAStoryManagerNode, IAStoryManagerNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AStoryManagerNode
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AStoryManagerNode</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAStoryManagerNode, IAStoryManagerNodeGetter> AStoryManagerNode(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAStoryManagerNode, IAStoryManagerNodeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAStoryManagerNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAStoryManagerNode, IAStoryManagerNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Atmosphere
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Atmosphere</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAtmosphere, IAtmosphereGetter> Atmosphere(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAtmosphere, IAtmosphereGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAtmosphereGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAtmosphere, IAtmosphereGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Atmosphere
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Atmosphere</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAtmosphere, IAtmosphereGetter> Atmosphere(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAtmosphere, IAtmosphereGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAtmosphereGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAtmosphere, IAtmosphereGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AttractionRule
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AttractionRule</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAttractionRule, IAttractionRuleGetter> AttractionRule(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAttractionRule, IAttractionRuleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAttractionRuleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAttractionRule, IAttractionRuleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AttractionRule
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AttractionRule</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAttractionRule, IAttractionRuleGetter> AttractionRule(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAttractionRule, IAttractionRuleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAttractionRuleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAttractionRule, IAttractionRuleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AudioOcclusionPrimitive
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AudioOcclusionPrimitive</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAudioOcclusionPrimitive, IAudioOcclusionPrimitiveGetter> AudioOcclusionPrimitive(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAudioOcclusionPrimitive, IAudioOcclusionPrimitiveGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAudioOcclusionPrimitiveGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAudioOcclusionPrimitive, IAudioOcclusionPrimitiveGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AudioOcclusionPrimitive
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AudioOcclusionPrimitive</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAudioOcclusionPrimitive, IAudioOcclusionPrimitiveGetter> AudioOcclusionPrimitive(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAudioOcclusionPrimitive, IAudioOcclusionPrimitiveGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAudioOcclusionPrimitiveGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAudioOcclusionPrimitive, IAudioOcclusionPrimitiveGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BendableSpline
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on BendableSpline</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBendableSpline, IBendableSplineGetter> BendableSpline(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBendableSpline, IBendableSplineGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBendableSplineGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IBendableSpline, IBendableSplineGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BendableSpline
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on BendableSpline</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBendableSpline, IBendableSplineGetter> BendableSpline(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBendableSpline, IBendableSplineGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBendableSplineGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IBendableSpline, IBendableSplineGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Biome
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Biome</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBiome, IBiomeGetter> Biome(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBiome, IBiomeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBiomeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IBiome, IBiomeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Biome
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Biome</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBiome, IBiomeGetter> Biome(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBiome, IBiomeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBiomeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IBiome, IBiomeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BiomeMarker
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on BiomeMarker</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBiomeMarker, IBiomeMarkerGetter> BiomeMarker(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBiomeMarker, IBiomeMarkerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBiomeMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IBiomeMarker, IBiomeMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BiomeMarker
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on BiomeMarker</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBiomeMarker, IBiomeMarkerGetter> BiomeMarker(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBiomeMarker, IBiomeMarkerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBiomeMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IBiomeMarker, IBiomeMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BiomeSwap
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on BiomeSwap</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBiomeSwap, IBiomeSwapGetter> BiomeSwap(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBiomeSwap, IBiomeSwapGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBiomeSwapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IBiomeSwap, IBiomeSwapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BiomeSwap
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on BiomeSwap</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBiomeSwap, IBiomeSwapGetter> BiomeSwap(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBiomeSwap, IBiomeSwapGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBiomeSwapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IBiomeSwap, IBiomeSwapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BodyPartData
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on BodyPartData</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBodyPartData, IBodyPartDataGetter> BodyPartData(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBodyPartData, IBodyPartDataGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IBodyPartData, IBodyPartDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BodyPartData
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on BodyPartData</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBodyPartData, IBodyPartDataGetter> BodyPartData(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBodyPartData, IBodyPartDataGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IBodyPartData, IBodyPartDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BoneModifier
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on BoneModifier</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBoneModifier, IBoneModifierGetter> BoneModifier(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBoneModifier, IBoneModifierGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBoneModifierGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IBoneModifier, IBoneModifierGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BoneModifier
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on BoneModifier</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBoneModifier, IBoneModifierGetter> BoneModifier(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBoneModifier, IBoneModifierGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBoneModifierGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IBoneModifier, IBoneModifierGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Book
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Book</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBook, IBookGetter> Book(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBook, IBookGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IBook, IBookGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Book
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Book</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBook, IBookGetter> Book(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBook, IBookGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IBook, IBookGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraPath
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CameraPath</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICameraPath, ICameraPathGetter> CameraPath(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICameraPath, ICameraPathGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ICameraPath, ICameraPathGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraPath
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CameraPath</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICameraPath, ICameraPathGetter> CameraPath(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICameraPath, ICameraPathGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ICameraPath, ICameraPathGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraShot
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CameraShot</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICameraShot, ICameraShotGetter> CameraShot(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICameraShot, ICameraShotGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ICameraShot, ICameraShotGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraShot
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CameraShot</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICameraShot, ICameraShotGetter> CameraShot(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICameraShot, ICameraShotGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ICameraShot, ICameraShotGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Cell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Cell</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICell, ICellGetter> Cell(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICell, ICellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ICell, ICellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Cell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Cell</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICell, ICellGetter> Cell(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICell, ICellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ICell, ICellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Challenge
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Challenge</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IChallenge, IChallengeGetter> Challenge(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IChallenge, IChallengeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IChallengeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IChallenge, IChallengeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Challenge
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Challenge</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IChallenge, IChallengeGetter> Challenge(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IChallenge, IChallengeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IChallengeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IChallenge, IChallengeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Class
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Class</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IClass, IClassGetter> Class(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IClass, IClassGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IClass, IClassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Class
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Class</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IClass, IClassGetter> Class(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IClass, IClassGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IClass, IClassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Climate
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Climate</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IClimate, IClimateGetter> Climate(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IClimate, IClimateGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IClimate, IClimateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Climate
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Climate</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IClimate, IClimateGetter> Climate(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IClimate, IClimateGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IClimate, IClimateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Clouds
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Clouds</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IClouds, ICloudsGetter> Clouds(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IClouds, ICloudsGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICloudsGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IClouds, ICloudsGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Clouds
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Clouds</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IClouds, ICloudsGetter> Clouds(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IClouds, ICloudsGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICloudsGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IClouds, ICloudsGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CollisionLayer
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CollisionLayer</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICollisionLayer, ICollisionLayerGetter> CollisionLayer(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICollisionLayer, ICollisionLayerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICollisionLayerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ICollisionLayer, ICollisionLayerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CollisionLayer
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CollisionLayer</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICollisionLayer, ICollisionLayerGetter> CollisionLayer(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICollisionLayer, ICollisionLayerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICollisionLayerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ICollisionLayer, ICollisionLayerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ColorRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ColorRecord</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IColorRecord, IColorRecordGetter> ColorRecord(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IColorRecord, IColorRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IColorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IColorRecord, IColorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ColorRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ColorRecord</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IColorRecord, IColorRecordGetter> ColorRecord(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IColorRecord, IColorRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IColorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IColorRecord, IColorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CombatStyle
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CombatStyle</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICombatStyle, ICombatStyleGetter> CombatStyle(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICombatStyle, ICombatStyleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ICombatStyle, ICombatStyleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CombatStyle
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CombatStyle</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICombatStyle, ICombatStyleGetter> CombatStyle(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICombatStyle, ICombatStyleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ICombatStyle, ICombatStyleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ConditionRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ConditionRecord</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IConditionRecord, IConditionRecordGetter> ConditionRecord(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IConditionRecord, IConditionRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IConditionRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IConditionRecord, IConditionRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ConditionRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ConditionRecord</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IConditionRecord, IConditionRecordGetter> ConditionRecord(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IConditionRecord, IConditionRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IConditionRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IConditionRecord, IConditionRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ConstructibleObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ConstructibleObject</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IConstructibleObject, IConstructibleObjectGetter> ConstructibleObject(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IConstructibleObject, IConstructibleObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IConstructibleObject, IConstructibleObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ConstructibleObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ConstructibleObject</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IConstructibleObject, IConstructibleObjectGetter> ConstructibleObject(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IConstructibleObject, IConstructibleObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IConstructibleObject, IConstructibleObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Container
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Container</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IContainer, IContainerGetter> Container(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IContainer, IContainerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IContainer, IContainerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Container
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Container</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IContainer, IContainerGetter> Container(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IContainer, IContainerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IContainer, IContainerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Curve3D
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Curve3D</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICurve3D, ICurve3DGetter> Curve3D(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICurve3D, ICurve3DGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICurve3DGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ICurve3D, ICurve3DGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Curve3D
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Curve3D</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICurve3D, ICurve3DGetter> Curve3D(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICurve3D, ICurve3DGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICurve3DGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ICurve3D, ICurve3DGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CurveTable
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CurveTable</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICurveTable, ICurveTableGetter> CurveTable(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICurveTable, ICurveTableGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICurveTableGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ICurveTable, ICurveTableGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CurveTable
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CurveTable</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICurveTable, ICurveTableGetter> CurveTable(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICurveTable, ICurveTableGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICurveTableGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ICurveTable, ICurveTableGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DamageType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DamageType</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDamageType, IDamageTypeGetter> DamageType(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDamageType, IDamageTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDamageTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IDamageType, IDamageTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DamageType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DamageType</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDamageType, IDamageTypeGetter> DamageType(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDamageType, IDamageTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDamageTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IDamageType, IDamageTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Debris
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Debris</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDebris, IDebrisGetter> Debris(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDebris, IDebrisGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDebrisGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IDebris, IDebrisGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Debris
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Debris</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDebris, IDebrisGetter> Debris(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDebris, IDebrisGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDebrisGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IDebris, IDebrisGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DefaultObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DefaultObject</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDefaultObject, IDefaultObjectGetter> DefaultObject(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDefaultObject, IDefaultObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDefaultObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IDefaultObject, IDefaultObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DefaultObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DefaultObject</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDefaultObject, IDefaultObjectGetter> DefaultObject(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDefaultObject, IDefaultObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDefaultObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IDefaultObject, IDefaultObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DefaultObjectManager
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DefaultObjectManager</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter> DefaultObjectManager(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DefaultObjectManager
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DefaultObjectManager</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter> DefaultObjectManager(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogBranch
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogBranch</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDialogBranch, IDialogBranchGetter> DialogBranch(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDialogBranch, IDialogBranchGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogBranchGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IDialogBranch, IDialogBranchGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogBranch
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogBranch</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDialogBranch, IDialogBranchGetter> DialogBranch(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDialogBranch, IDialogBranchGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogBranchGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IDialogBranch, IDialogBranchGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogResponses
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogResponses</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDialogResponses, IDialogResponsesGetter> DialogResponses(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDialogResponses, IDialogResponsesGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IDialogResponses, IDialogResponsesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogResponses
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogResponses</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDialogResponses, IDialogResponsesGetter> DialogResponses(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDialogResponses, IDialogResponsesGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IDialogResponses, IDialogResponsesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogTopic
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogTopic</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDialogTopic, IDialogTopicGetter> DialogTopic(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDialogTopic, IDialogTopicGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IDialogTopic, IDialogTopicGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogTopic
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogTopic</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDialogTopic, IDialogTopicGetter> DialogTopic(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDialogTopic, IDialogTopicGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IDialogTopic, IDialogTopicGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Door
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Door</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDoor, IDoorGetter> Door(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDoor, IDoorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IDoor, IDoorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Door
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Door</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDoor, IDoorGetter> Door(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IDoor, IDoorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IDoor, IDoorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EffectSequence
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EffectSequence</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IEffectSequence, IEffectSequenceGetter> EffectSequence(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IEffectSequence, IEffectSequenceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEffectSequenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IEffectSequence, IEffectSequenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EffectSequence
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EffectSequence</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IEffectSequence, IEffectSequenceGetter> EffectSequence(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IEffectSequence, IEffectSequenceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEffectSequenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IEffectSequence, IEffectSequenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EffectShader
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EffectShader</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IEffectShader, IEffectShaderGetter> EffectShader(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IEffectShader, IEffectShaderGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IEffectShader, IEffectShaderGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EffectShader
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EffectShader</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IEffectShader, IEffectShaderGetter> EffectShader(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IEffectShader, IEffectShaderGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IEffectShader, IEffectShaderGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EquipType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EquipType</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IEquipType, IEquipTypeGetter> EquipType(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IEquipType, IEquipTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEquipTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IEquipType, IEquipTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EquipType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EquipType</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IEquipType, IEquipTypeGetter> EquipType(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IEquipType, IEquipTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEquipTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IEquipType, IEquipTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Explosion
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Explosion</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IExplosion, IExplosionGetter> Explosion(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IExplosion, IExplosionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IExplosionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IExplosion, IExplosionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Explosion
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Explosion</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IExplosion, IExplosionGetter> Explosion(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IExplosion, IExplosionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IExplosionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IExplosion, IExplosionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Faction
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Faction</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFaction, IFactionGetter> Faction(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFaction, IFactionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IFaction, IFactionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Faction
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Faction</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFaction, IFactionGetter> Faction(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFaction, IFactionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IFaction, IFactionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FFKWRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on FFKWRecord</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFFKWRecord, IFFKWRecordGetter> FFKWRecord(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFFKWRecord, IFFKWRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFFKWRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IFFKWRecord, IFFKWRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FFKWRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on FFKWRecord</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFFKWRecord, IFFKWRecordGetter> FFKWRecord(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFFKWRecord, IFFKWRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFFKWRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IFFKWRecord, IFFKWRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Flora
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Flora</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFlora, IFloraGetter> Flora(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFlora, IFloraGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IFlora, IFloraGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Flora
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Flora</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFlora, IFloraGetter> Flora(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFlora, IFloraGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IFlora, IFloraGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FogVolume
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on FogVolume</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFogVolume, IFogVolumeGetter> FogVolume(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFogVolume, IFogVolumeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFogVolumeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IFogVolume, IFogVolumeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FogVolume
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on FogVolume</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFogVolume, IFogVolumeGetter> FogVolume(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFogVolume, IFogVolumeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFogVolumeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IFogVolume, IFogVolumeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Footstep
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Footstep</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFootstep, IFootstepGetter> Footstep(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFootstep, IFootstepGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFootstepGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IFootstep, IFootstepGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Footstep
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Footstep</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFootstep, IFootstepGetter> Footstep(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFootstep, IFootstepGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFootstepGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IFootstep, IFootstepGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FootstepSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on FootstepSet</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFootstepSet, IFootstepSetGetter> FootstepSet(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFootstepSet, IFootstepSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFootstepSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IFootstepSet, IFootstepSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FootstepSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on FootstepSet</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFootstepSet, IFootstepSetGetter> FootstepSet(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFootstepSet, IFootstepSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFootstepSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IFootstepSet, IFootstepSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ForceData
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ForceData</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IForceData, IForceDataGetter> ForceData(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IForceData, IForceDataGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IForceDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IForceData, IForceDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ForceData
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ForceData</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IForceData, IForceDataGetter> ForceData(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IForceData, IForceDataGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IForceDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IForceData, IForceDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FormFolderKeywordList
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on FormFolderKeywordList</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFormFolderKeywordList, IFormFolderKeywordListGetter> FormFolderKeywordList(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFormFolderKeywordList, IFormFolderKeywordListGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFormFolderKeywordListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IFormFolderKeywordList, IFormFolderKeywordListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FormFolderKeywordList
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on FormFolderKeywordList</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFormFolderKeywordList, IFormFolderKeywordListGetter> FormFolderKeywordList(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFormFolderKeywordList, IFormFolderKeywordListGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFormFolderKeywordListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IFormFolderKeywordList, IFormFolderKeywordListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FormList
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on FormList</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFormList, IFormListGetter> FormList(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFormList, IFormListGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFormListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IFormList, IFormListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FormList
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on FormList</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFormList, IFormListGetter> FormList(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFormList, IFormListGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFormListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IFormList, IFormListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Furniture
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Furniture</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFurniture, IFurnitureGetter> Furniture(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFurniture, IFurnitureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IFurniture, IFurnitureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Furniture
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Furniture</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFurniture, IFurnitureGetter> Furniture(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IFurniture, IFurnitureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IFurniture, IFurnitureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GalaxyStarData
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GalaxyStarData</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGalaxyStarData, IGalaxyStarDataGetter> GalaxyStarData(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGalaxyStarData, IGalaxyStarDataGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGalaxyStarDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IGalaxyStarData, IGalaxyStarDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GalaxyStarData
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GalaxyStarData</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGalaxyStarData, IGalaxyStarDataGetter> GalaxyStarData(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGalaxyStarData, IGalaxyStarDataGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGalaxyStarDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IGalaxyStarData, IGalaxyStarDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GalaxySunPreset
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GalaxySunPreset</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGalaxySunPreset, IGalaxySunPresetGetter> GalaxySunPreset(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGalaxySunPreset, IGalaxySunPresetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGalaxySunPresetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IGalaxySunPreset, IGalaxySunPresetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GalaxySunPreset
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GalaxySunPreset</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGalaxySunPreset, IGalaxySunPresetGetter> GalaxySunPreset(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGalaxySunPreset, IGalaxySunPresetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGalaxySunPresetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IGalaxySunPreset, IGalaxySunPresetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GenericBaseForm
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GenericBaseForm</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGenericBaseForm, IGenericBaseFormGetter> GenericBaseForm(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGenericBaseForm, IGenericBaseFormGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGenericBaseFormGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IGenericBaseForm, IGenericBaseFormGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GenericBaseForm
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GenericBaseForm</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGenericBaseForm, IGenericBaseFormGetter> GenericBaseForm(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGenericBaseForm, IGenericBaseFormGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGenericBaseFormGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IGenericBaseForm, IGenericBaseFormGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GenericBaseFormTemplate
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GenericBaseFormTemplate</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGenericBaseFormTemplate, IGenericBaseFormTemplateGetter> GenericBaseFormTemplate(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGenericBaseFormTemplate, IGenericBaseFormTemplateGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGenericBaseFormTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IGenericBaseFormTemplate, IGenericBaseFormTemplateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GenericBaseFormTemplate
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GenericBaseFormTemplate</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGenericBaseFormTemplate, IGenericBaseFormTemplateGetter> GenericBaseFormTemplate(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGenericBaseFormTemplate, IGenericBaseFormTemplateGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGenericBaseFormTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IGenericBaseFormTemplate, IGenericBaseFormTemplateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Global
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Global</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGlobal, IGlobalGetter> Global(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGlobal, IGlobalGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IGlobal, IGlobalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Global
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Global</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGlobal, IGlobalGetter> Global(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGlobal, IGlobalGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IGlobal, IGlobalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Grass
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Grass</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGrass, IGrassGetter> Grass(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGrass, IGrassGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IGrass, IGrassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Grass
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Grass</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGrass, IGrassGetter> Grass(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGrass, IGrassGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IGrass, IGrassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GroundCover
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GroundCover</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGroundCover, IGroundCoverGetter> GroundCover(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGroundCover, IGroundCoverGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGroundCoverGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IGroundCover, IGroundCoverGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GroundCover
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GroundCover</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGroundCover, IGroundCoverGetter> GroundCover(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IGroundCover, IGroundCoverGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGroundCoverGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IGroundCover, IGroundCoverGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hazard
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Hazard</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IHazard, IHazardGetter> Hazard(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IHazard, IHazardGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHazardGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IHazard, IHazardGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hazard
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Hazard</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IHazard, IHazardGetter> Hazard(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IHazard, IHazardGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHazardGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IHazard, IHazardGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to HeadPart
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on HeadPart</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IHeadPart, IHeadPartGetter> HeadPart(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IHeadPart, IHeadPartGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IHeadPart, IHeadPartGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to HeadPart
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on HeadPart</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IHeadPart, IHeadPartGetter> HeadPart(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IHeadPart, IHeadPartGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IHeadPart, IHeadPartGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleAnimation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IdleAnimation</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IIdleAnimation, IIdleAnimationGetter> IdleAnimation(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IIdleAnimation, IIdleAnimationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IIdleAnimation, IIdleAnimationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleAnimation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IdleAnimation</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IIdleAnimation, IIdleAnimationGetter> IdleAnimation(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IIdleAnimation, IIdleAnimationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IIdleAnimation, IIdleAnimationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleMarker
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IdleMarker</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IIdleMarker, IIdleMarkerGetter> IdleMarker(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IIdleMarker, IIdleMarkerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IIdleMarker, IIdleMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleMarker
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IdleMarker</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IIdleMarker, IIdleMarkerGetter> IdleMarker(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IIdleMarker, IIdleMarkerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IIdleMarker, IIdleMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ImageSpace</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IImageSpace, IImageSpaceGetter> ImageSpace(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IImageSpace, IImageSpaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IImageSpace, IImageSpaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ImageSpace</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IImageSpace, IImageSpaceGetter> ImageSpace(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IImageSpace, IImageSpaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IImageSpace, IImageSpaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpaceAdapter
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ImageSpaceAdapter</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter> ImageSpaceAdapter(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpaceAdapter
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ImageSpaceAdapter</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter> ImageSpaceAdapter(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Impact
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Impact</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IImpact, IImpactGetter> Impact(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IImpact, IImpactGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImpactGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IImpact, IImpactGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Impact
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Impact</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IImpact, IImpactGetter> Impact(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IImpact, IImpactGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImpactGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IImpact, IImpactGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImpactDataSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ImpactDataSet</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IImpactDataSet, IImpactDataSetGetter> ImpactDataSet(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IImpactDataSet, IImpactDataSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IImpactDataSet, IImpactDataSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImpactDataSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ImpactDataSet</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IImpactDataSet, IImpactDataSetGetter> ImpactDataSet(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IImpactDataSet, IImpactDataSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IImpactDataSet, IImpactDataSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingestible
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ingestible</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IIngestible, IIngestibleGetter> Ingestible(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IIngestible, IIngestibleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IIngestible, IIngestibleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingestible
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ingestible</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IIngestible, IIngestibleGetter> Ingestible(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IIngestible, IIngestibleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IIngestible, IIngestibleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to InstanceNamingRules
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on InstanceNamingRules</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IInstanceNamingRules, IInstanceNamingRulesGetter> InstanceNamingRules(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IInstanceNamingRules, IInstanceNamingRulesGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IInstanceNamingRulesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IInstanceNamingRules, IInstanceNamingRulesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to InstanceNamingRules
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on InstanceNamingRules</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IInstanceNamingRules, IInstanceNamingRulesGetter> InstanceNamingRules(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IInstanceNamingRules, IInstanceNamingRulesGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IInstanceNamingRulesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IInstanceNamingRules, IInstanceNamingRulesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Key
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Key</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IKey, IKeyGetter> Key(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IKey, IKeyGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IKey, IKeyGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Key
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Key</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IKey, IKeyGetter> Key(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IKey, IKeyGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IKey, IKeyGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Keyword
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Keyword</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IKeyword, IKeywordGetter> Keyword(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IKeyword, IKeywordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeywordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IKeyword, IKeywordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Keyword
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Keyword</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IKeyword, IKeywordGetter> Keyword(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IKeyword, IKeywordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeywordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IKeyword, IKeywordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LandscapeTexture
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LandscapeTexture</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILandscapeTexture, ILandscapeTextureGetter> LandscapeTexture(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILandscapeTexture, ILandscapeTextureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILandscapeTexture, ILandscapeTextureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LandscapeTexture
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LandscapeTexture</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILandscapeTexture, ILandscapeTextureGetter> LandscapeTexture(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILandscapeTexture, ILandscapeTextureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILandscapeTexture, ILandscapeTextureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Layer
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Layer</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILayer, ILayerGetter> Layer(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILayer, ILayerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILayerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILayer, ILayerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Layer
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Layer</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILayer, ILayerGetter> Layer(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILayer, ILayerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILayerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILayer, ILayerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LayeredMaterialSwap
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LayeredMaterialSwap</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILayeredMaterialSwap, ILayeredMaterialSwapGetter> LayeredMaterialSwap(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILayeredMaterialSwap, ILayeredMaterialSwapGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILayeredMaterialSwapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILayeredMaterialSwap, ILayeredMaterialSwapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LayeredMaterialSwap
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LayeredMaterialSwap</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILayeredMaterialSwap, ILayeredMaterialSwapGetter> LayeredMaterialSwap(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILayeredMaterialSwap, ILayeredMaterialSwapGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILayeredMaterialSwapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILayeredMaterialSwap, ILayeredMaterialSwapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LegendaryItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LegendaryItem</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILegendaryItem, ILegendaryItemGetter> LegendaryItem(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILegendaryItem, ILegendaryItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILegendaryItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILegendaryItem, ILegendaryItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LegendaryItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LegendaryItem</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILegendaryItem, ILegendaryItemGetter> LegendaryItem(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILegendaryItem, ILegendaryItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILegendaryItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILegendaryItem, ILegendaryItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LensFlare
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LensFlare</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILensFlare, ILensFlareGetter> LensFlare(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILensFlare, ILensFlareGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILensFlareGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILensFlare, ILensFlareGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LensFlare
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LensFlare</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILensFlare, ILensFlareGetter> LensFlare(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILensFlare, ILensFlareGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILensFlareGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILensFlare, ILensFlareGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledBaseForm
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledBaseForm</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledBaseForm, ILeveledBaseFormGetter> LeveledBaseForm(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledBaseForm, ILeveledBaseFormGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledBaseFormGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILeveledBaseForm, ILeveledBaseFormGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledBaseForm
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledBaseForm</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledBaseForm, ILeveledBaseFormGetter> LeveledBaseForm(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledBaseForm, ILeveledBaseFormGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledBaseFormGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILeveledBaseForm, ILeveledBaseFormGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledItem</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledItem, ILeveledItemGetter> LeveledItem(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledItem, ILeveledItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILeveledItem, ILeveledItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledItem</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledItem, ILeveledItemGetter> LeveledItem(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledItem, ILeveledItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILeveledItem, ILeveledItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledNpc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledNpc</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledNpc, ILeveledNpcGetter> LeveledNpc(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledNpc, ILeveledNpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILeveledNpc, ILeveledNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledNpc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledNpc</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledNpc, ILeveledNpcGetter> LeveledNpc(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledNpc, ILeveledNpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILeveledNpc, ILeveledNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledPackIn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledPackIn</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledPackIn, ILeveledPackInGetter> LeveledPackIn(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledPackIn, ILeveledPackInGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledPackInGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILeveledPackIn, ILeveledPackInGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledPackIn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledPackIn</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledPackIn, ILeveledPackInGetter> LeveledPackIn(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledPackIn, ILeveledPackInGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledPackInGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILeveledPackIn, ILeveledPackInGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledSpaceCell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledSpaceCell</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledSpaceCell, ILeveledSpaceCellGetter> LeveledSpaceCell(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledSpaceCell, ILeveledSpaceCellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledSpaceCellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILeveledSpaceCell, ILeveledSpaceCellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledSpaceCell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledSpaceCell</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledSpaceCell, ILeveledSpaceCellGetter> LeveledSpaceCell(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledSpaceCell, ILeveledSpaceCellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledSpaceCellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILeveledSpaceCell, ILeveledSpaceCellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Light
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Light</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILight, ILightGetter> Light(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILight, ILightGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILight, ILightGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Light
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Light</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILight, ILightGetter> Light(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILight, ILightGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILight, ILightGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LightingTemplate
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LightingTemplate</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILightingTemplate, ILightingTemplateGetter> LightingTemplate(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILightingTemplate, ILightingTemplateGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILightingTemplate, ILightingTemplateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LightingTemplate
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LightingTemplate</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILightingTemplate, ILightingTemplateGetter> LightingTemplate(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILightingTemplate, ILightingTemplateGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILightingTemplate, ILightingTemplateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LoadScreen
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LoadScreen</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILoadScreen, ILoadScreenGetter> LoadScreen(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILoadScreen, ILoadScreenGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILoadScreen, ILoadScreenGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LoadScreen
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LoadScreen</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILoadScreen, ILoadScreenGetter> LoadScreen(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILoadScreen, ILoadScreenGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILoadScreen, ILoadScreenGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Location
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Location</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILocation, ILocationGetter> Location(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILocation, ILocationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILocationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILocation, ILocationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Location
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Location</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILocation, ILocationGetter> Location(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILocation, ILocationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILocationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILocation, ILocationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LocationReferenceType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LocationReferenceType</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILocationReferenceType, ILocationReferenceTypeGetter> LocationReferenceType(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LocationReferenceType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LocationReferenceType</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILocationReferenceType, ILocationReferenceTypeGetter> LocationReferenceType(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILocationReferenceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MagicEffect
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MagicEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMagicEffect, IMagicEffectGetter> MagicEffect(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMagicEffect, IMagicEffectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMagicEffect, IMagicEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MagicEffect
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MagicEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMagicEffect, IMagicEffectGetter> MagicEffect(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMagicEffect, IMagicEffectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMagicEffect, IMagicEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MaterialPath
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MaterialPath</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMaterialPath, IMaterialPathGetter> MaterialPath(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMaterialPath, IMaterialPathGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMaterialPathGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMaterialPath, IMaterialPathGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MaterialPath
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MaterialPath</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMaterialPath, IMaterialPathGetter> MaterialPath(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMaterialPath, IMaterialPathGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMaterialPathGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMaterialPath, IMaterialPathGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MaterialType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MaterialType</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMaterialType, IMaterialTypeGetter> MaterialType(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMaterialType, IMaterialTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMaterialTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMaterialType, IMaterialTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MaterialType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MaterialType</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMaterialType, IMaterialTypeGetter> MaterialType(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMaterialType, IMaterialTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMaterialTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMaterialType, IMaterialTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MeleeAimAssistModel
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MeleeAimAssistModel</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMeleeAimAssistModel, IMeleeAimAssistModelGetter> MeleeAimAssistModel(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMeleeAimAssistModel, IMeleeAimAssistModelGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMeleeAimAssistModelGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMeleeAimAssistModel, IMeleeAimAssistModelGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MeleeAimAssistModel
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MeleeAimAssistModel</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMeleeAimAssistModel, IMeleeAimAssistModelGetter> MeleeAimAssistModel(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMeleeAimAssistModel, IMeleeAimAssistModelGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMeleeAimAssistModelGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMeleeAimAssistModel, IMeleeAimAssistModelGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Message
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Message</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMessage, IMessageGetter> Message(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMessage, IMessageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMessageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMessage, IMessageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Message
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Message</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMessage, IMessageGetter> Message(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMessage, IMessageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMessageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMessage, IMessageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MiscItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MiscItem</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMiscItem, IMiscItemGetter> MiscItem(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMiscItem, IMiscItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMiscItem, IMiscItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MiscItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MiscItem</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMiscItem, IMiscItemGetter> MiscItem(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMiscItem, IMiscItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMiscItem, IMiscItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MorphableObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MorphableObject</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMorphableObject, IMorphableObjectGetter> MorphableObject(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMorphableObject, IMorphableObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMorphableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMorphableObject, IMorphableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MorphableObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MorphableObject</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMorphableObject, IMorphableObjectGetter> MorphableObject(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMorphableObject, IMorphableObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMorphableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMorphableObject, IMorphableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MoveableStatic
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MoveableStatic</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMoveableStatic, IMoveableStaticGetter> MoveableStatic(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMoveableStatic, IMoveableStaticGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMoveableStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMoveableStatic, IMoveableStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MoveableStatic
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MoveableStatic</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMoveableStatic, IMoveableStaticGetter> MoveableStatic(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMoveableStatic, IMoveableStaticGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMoveableStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMoveableStatic, IMoveableStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MovementType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MovementType</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMovementType, IMovementTypeGetter> MovementType(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMovementType, IMovementTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMovementTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMovementType, IMovementTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MovementType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MovementType</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMovementType, IMovementTypeGetter> MovementType(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMovementType, IMovementTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMovementTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMovementType, IMovementTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MusicTrack
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MusicTrack</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMusicTrack, IMusicTrackGetter> MusicTrack(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMusicTrack, IMusicTrackGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMusicTrackGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMusicTrack, IMusicTrackGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MusicTrack
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MusicTrack</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMusicTrack, IMusicTrackGetter> MusicTrack(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMusicTrack, IMusicTrackGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMusicTrackGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMusicTrack, IMusicTrackGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MusicType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MusicType</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMusicType, IMusicTypeGetter> MusicType(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMusicType, IMusicTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMusicType, IMusicTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MusicType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MusicType</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMusicType, IMusicTypeGetter> MusicType(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IMusicType, IMusicTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IMusicType, IMusicTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMesh
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on NavigationMesh</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INavigationMesh, INavigationMeshGetter> NavigationMesh(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INavigationMesh, INavigationMeshGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, INavigationMesh, INavigationMeshGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMesh
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on NavigationMesh</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INavigationMesh, INavigationMeshGetter> NavigationMesh(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INavigationMesh, INavigationMeshGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, INavigationMesh, INavigationMeshGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMeshInfoMap
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on NavigationMeshInfoMap</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter> NavigationMeshInfoMap(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMeshInfoMap
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on NavigationMeshInfoMap</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter> NavigationMeshInfoMap(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMeshObstacleCoverManager
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on NavigationMeshObstacleCoverManager</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INavigationMeshObstacleCoverManager, INavigationMeshObstacleCoverManagerGetter> NavigationMeshObstacleCoverManager(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INavigationMeshObstacleCoverManager, INavigationMeshObstacleCoverManagerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INavigationMeshObstacleCoverManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, INavigationMeshObstacleCoverManager, INavigationMeshObstacleCoverManagerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMeshObstacleCoverManager
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on NavigationMeshObstacleCoverManager</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INavigationMeshObstacleCoverManager, INavigationMeshObstacleCoverManagerGetter> NavigationMeshObstacleCoverManager(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INavigationMeshObstacleCoverManager, INavigationMeshObstacleCoverManagerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INavigationMeshObstacleCoverManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, INavigationMeshObstacleCoverManager, INavigationMeshObstacleCoverManagerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INpc, INpcGetter> Npc(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INpc, INpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, INpc, INpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INpc, INpcGetter> Npc(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INpc, INpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, INpc, INpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ObjectEffect
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ObjectEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IObjectEffect, IObjectEffectGetter> ObjectEffect(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IObjectEffect, IObjectEffectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IObjectEffect, IObjectEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ObjectEffect
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ObjectEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IObjectEffect, IObjectEffectGetter> ObjectEffect(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IObjectEffect, IObjectEffectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IObjectEffect, IObjectEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ObjectVisibilityManager
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ObjectVisibilityManager</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IObjectVisibilityManager, IObjectVisibilityManagerGetter> ObjectVisibilityManager(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IObjectVisibilityManager, IObjectVisibilityManagerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IObjectVisibilityManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IObjectVisibilityManager, IObjectVisibilityManagerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ObjectVisibilityManager
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ObjectVisibilityManager</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IObjectVisibilityManager, IObjectVisibilityManagerGetter> ObjectVisibilityManager(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IObjectVisibilityManager, IObjectVisibilityManagerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IObjectVisibilityManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IObjectVisibilityManager, IObjectVisibilityManagerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Outfit
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Outfit</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IOutfit, IOutfitGetter> Outfit(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IOutfit, IOutfitGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IOutfitGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IOutfit, IOutfitGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Outfit
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Outfit</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IOutfit, IOutfitGetter> Outfit(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IOutfit, IOutfitGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IOutfitGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IOutfit, IOutfitGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Package
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Package</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPackage, IPackageGetter> Package(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPackage, IPackageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPackageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPackage, IPackageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Package
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Package</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPackage, IPackageGetter> Package(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPackage, IPackageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPackageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPackage, IPackageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PackIn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PackIn</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPackIn, IPackInGetter> PackIn(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPackIn, IPackInGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPackInGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPackIn, IPackInGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PackIn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PackIn</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPackIn, IPackInGetter> PackIn(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPackIn, IPackInGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPackInGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPackIn, IPackInGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ParticleSystemDefineCollision
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ParticleSystemDefineCollision</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IParticleSystemDefineCollision, IParticleSystemDefineCollisionGetter> ParticleSystemDefineCollision(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IParticleSystemDefineCollision, IParticleSystemDefineCollisionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IParticleSystemDefineCollisionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IParticleSystemDefineCollision, IParticleSystemDefineCollisionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ParticleSystemDefineCollision
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ParticleSystemDefineCollision</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IParticleSystemDefineCollision, IParticleSystemDefineCollisionGetter> ParticleSystemDefineCollision(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IParticleSystemDefineCollision, IParticleSystemDefineCollisionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IParticleSystemDefineCollisionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IParticleSystemDefineCollision, IParticleSystemDefineCollisionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Perk
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Perk</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPerk, IPerkGetter> Perk(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPerk, IPerkGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPerkGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPerk, IPerkGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Perk
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Perk</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPerk, IPerkGetter> Perk(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPerk, IPerkGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPerkGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPerk, IPerkGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PhotoModeFeature
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PhotoModeFeature</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPhotoModeFeature, IPhotoModeFeatureGetter> PhotoModeFeature(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPhotoModeFeature, IPhotoModeFeatureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPhotoModeFeatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPhotoModeFeature, IPhotoModeFeatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PhotoModeFeature
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PhotoModeFeature</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPhotoModeFeature, IPhotoModeFeatureGetter> PhotoModeFeature(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPhotoModeFeature, IPhotoModeFeatureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPhotoModeFeatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPhotoModeFeature, IPhotoModeFeatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedNpc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedNpc</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlacedNpc, IPlacedNpcGetter> PlacedNpc(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlacedNpc, IPlacedNpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlacedNpc, IPlacedNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedNpc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedNpc</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlacedNpc, IPlacedNpcGetter> PlacedNpc(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlacedNpc, IPlacedNpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlacedNpc, IPlacedNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedObject</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlacedObject, IPlacedObjectGetter> PlacedObject(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlacedObject, IPlacedObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlacedObject, IPlacedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedObject</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlacedObject, IPlacedObjectGetter> PlacedObject(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlacedObject, IPlacedObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlacedObject, IPlacedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Planet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Planet</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlanet, IPlanetGetter> Planet(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlanet, IPlanetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlanetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlanet, IPlanetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Planet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Planet</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlanet, IPlanetGetter> Planet(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlanet, IPlanetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlanetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlanet, IPlanetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlanetContentManagerBranchNode
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlanetContentManagerBranchNode</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlanetContentManagerBranchNode, IPlanetContentManagerBranchNodeGetter> PlanetContentManagerBranchNode(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlanetContentManagerBranchNode, IPlanetContentManagerBranchNodeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlanetContentManagerBranchNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlanetContentManagerBranchNode, IPlanetContentManagerBranchNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlanetContentManagerBranchNode
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlanetContentManagerBranchNode</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlanetContentManagerBranchNode, IPlanetContentManagerBranchNodeGetter> PlanetContentManagerBranchNode(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlanetContentManagerBranchNode, IPlanetContentManagerBranchNodeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlanetContentManagerBranchNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlanetContentManagerBranchNode, IPlanetContentManagerBranchNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlanetContentManagerContentNode
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlanetContentManagerContentNode</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlanetContentManagerContentNode, IPlanetContentManagerContentNodeGetter> PlanetContentManagerContentNode(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlanetContentManagerContentNode, IPlanetContentManagerContentNodeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlanetContentManagerContentNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlanetContentManagerContentNode, IPlanetContentManagerContentNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlanetContentManagerContentNode
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlanetContentManagerContentNode</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlanetContentManagerContentNode, IPlanetContentManagerContentNodeGetter> PlanetContentManagerContentNode(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlanetContentManagerContentNode, IPlanetContentManagerContentNodeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlanetContentManagerContentNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlanetContentManagerContentNode, IPlanetContentManagerContentNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlanetContentManagerTree
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlanetContentManagerTree</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlanetContentManagerTree, IPlanetContentManagerTreeGetter> PlanetContentManagerTree(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlanetContentManagerTree, IPlanetContentManagerTreeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlanetContentManagerTreeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlanetContentManagerTree, IPlanetContentManagerTreeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlanetContentManagerTree
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlanetContentManagerTree</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlanetContentManagerTree, IPlanetContentManagerTreeGetter> PlanetContentManagerTree(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlanetContentManagerTree, IPlanetContentManagerTreeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlanetContentManagerTreeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlanetContentManagerTree, IPlanetContentManagerTreeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ProjectedDecal
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ProjectedDecal</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IProjectedDecal, IProjectedDecalGetter> ProjectedDecal(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IProjectedDecal, IProjectedDecalGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IProjectedDecalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IProjectedDecal, IProjectedDecalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ProjectedDecal
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ProjectedDecal</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IProjectedDecal, IProjectedDecalGetter> ProjectedDecal(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IProjectedDecal, IProjectedDecalGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IProjectedDecalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IProjectedDecal, IProjectedDecalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Projectile
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Projectile</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IProjectile, IProjectileGetter> Projectile(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IProjectile, IProjectileGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IProjectileGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IProjectile, IProjectileGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Projectile
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Projectile</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IProjectile, IProjectileGetter> Projectile(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IProjectile, IProjectileGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IProjectileGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IProjectile, IProjectileGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Quest
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Quest</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IQuest, IQuestGetter> Quest(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IQuest, IQuestGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IQuest, IQuestGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Quest
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Quest</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IQuest, IQuestGetter> Quest(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IQuest, IQuestGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IQuest, IQuestGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IRace, IRaceGetter> Race(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IRace, IRaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IRace, IRaceGetter> Race(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IRace, IRaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ReferenceGroup
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ReferenceGroup</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IReferenceGroup, IReferenceGroupGetter> ReferenceGroup(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IReferenceGroup, IReferenceGroupGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IReferenceGroupGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IReferenceGroup, IReferenceGroupGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ReferenceGroup
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ReferenceGroup</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IReferenceGroup, IReferenceGroupGetter> ReferenceGroup(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IReferenceGroup, IReferenceGroupGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IReferenceGroupGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IReferenceGroup, IReferenceGroupGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Region
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Region</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IRegion, IRegionGetter> Region(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IRegion, IRegionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IRegion, IRegionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Region
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Region</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IRegion, IRegionGetter> Region(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IRegion, IRegionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IRegion, IRegionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ResearchProject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ResearchProject</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IResearchProject, IResearchProjectGetter> ResearchProject(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IResearchProject, IResearchProjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IResearchProjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IResearchProject, IResearchProjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ResearchProject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ResearchProject</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IResearchProject, IResearchProjectGetter> ResearchProject(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IResearchProject, IResearchProjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IResearchProjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IResearchProject, IResearchProjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Resource
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Resource</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IResource, IResourceGetter> Resource(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IResource, IResourceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IResourceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IResource, IResourceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Resource
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Resource</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IResource, IResourceGetter> Resource(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IResource, IResourceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IResourceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IResource, IResourceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ResourceGenerationData
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ResourceGenerationData</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IResourceGenerationData, IResourceGenerationDataGetter> ResourceGenerationData(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IResourceGenerationData, IResourceGenerationDataGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IResourceGenerationDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IResourceGenerationData, IResourceGenerationDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ResourceGenerationData
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ResourceGenerationData</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IResourceGenerationData, IResourceGenerationDataGetter> ResourceGenerationData(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IResourceGenerationData, IResourceGenerationDataGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IResourceGenerationDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IResourceGenerationData, IResourceGenerationDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ReverbParameters
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ReverbParameters</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IReverbParameters, IReverbParametersGetter> ReverbParameters(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IReverbParameters, IReverbParametersGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IReverbParametersGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IReverbParameters, IReverbParametersGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ReverbParameters
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ReverbParameters</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IReverbParameters, IReverbParametersGetter> ReverbParameters(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IReverbParameters, IReverbParametersGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IReverbParametersGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IReverbParameters, IReverbParametersGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Scene
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Scene</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IScene, ISceneGetter> Scene(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IScene, ISceneGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISceneGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IScene, ISceneGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Scene
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Scene</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IScene, ISceneGetter> Scene(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IScene, ISceneGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISceneGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IScene, ISceneGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SecondaryDamageList
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SecondaryDamageList</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISecondaryDamageList, ISecondaryDamageListGetter> SecondaryDamageList(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISecondaryDamageList, ISecondaryDamageListGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISecondaryDamageListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISecondaryDamageList, ISecondaryDamageListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SecondaryDamageList
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SecondaryDamageList</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISecondaryDamageList, ISecondaryDamageListGetter> SecondaryDamageList(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISecondaryDamageList, ISecondaryDamageListGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISecondaryDamageListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISecondaryDamageList, ISecondaryDamageListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ShaderParticleGeometry
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ShaderParticleGeometry</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter> ShaderParticleGeometry(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IShaderParticleGeometryGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ShaderParticleGeometry
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ShaderParticleGeometry</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter> ShaderParticleGeometry(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IShaderParticleGeometryGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IShaderParticleGeometry, IShaderParticleGeometryGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SnapTemplate
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SnapTemplate</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISnapTemplate, ISnapTemplateGetter> SnapTemplate(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISnapTemplate, ISnapTemplateGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISnapTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISnapTemplate, ISnapTemplateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SnapTemplate
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SnapTemplate</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISnapTemplate, ISnapTemplateGetter> SnapTemplate(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISnapTemplate, ISnapTemplateGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISnapTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISnapTemplate, ISnapTemplateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SnapTemplateBehavior
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SnapTemplateBehavior</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISnapTemplateBehavior, ISnapTemplateBehaviorGetter> SnapTemplateBehavior(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISnapTemplateBehavior, ISnapTemplateBehaviorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISnapTemplateBehaviorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISnapTemplateBehavior, ISnapTemplateBehaviorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SnapTemplateBehavior
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SnapTemplateBehavior</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISnapTemplateBehavior, ISnapTemplateBehaviorGetter> SnapTemplateBehavior(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISnapTemplateBehavior, ISnapTemplateBehaviorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISnapTemplateBehaviorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISnapTemplateBehavior, ISnapTemplateBehaviorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SnapTemplateNode
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SnapTemplateNode</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISnapTemplateNode, ISnapTemplateNodeGetter> SnapTemplateNode(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISnapTemplateNode, ISnapTemplateNodeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISnapTemplateNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISnapTemplateNode, ISnapTemplateNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SnapTemplateNode
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SnapTemplateNode</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISnapTemplateNode, ISnapTemplateNodeGetter> SnapTemplateNode(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISnapTemplateNode, ISnapTemplateNodeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISnapTemplateNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISnapTemplateNode, ISnapTemplateNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundEchoMarker
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoundEchoMarker</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISoundEchoMarker, ISoundEchoMarkerGetter> SoundEchoMarker(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISoundEchoMarker, ISoundEchoMarkerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundEchoMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISoundEchoMarker, ISoundEchoMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundEchoMarker
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoundEchoMarker</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISoundEchoMarker, ISoundEchoMarkerGetter> SoundEchoMarker(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISoundEchoMarker, ISoundEchoMarkerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundEchoMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISoundEchoMarker, ISoundEchoMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundKeywordMapping
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoundKeywordMapping</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISoundKeywordMapping, ISoundKeywordMappingGetter> SoundKeywordMapping(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISoundKeywordMapping, ISoundKeywordMappingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundKeywordMappingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISoundKeywordMapping, ISoundKeywordMappingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundKeywordMapping
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoundKeywordMapping</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISoundKeywordMapping, ISoundKeywordMappingGetter> SoundKeywordMapping(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISoundKeywordMapping, ISoundKeywordMappingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundKeywordMappingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISoundKeywordMapping, ISoundKeywordMappingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundMarker
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SoundMarker</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISoundMarker, ISoundMarkerGetter> SoundMarker(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISoundMarker, ISoundMarkerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISoundMarker, ISoundMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SoundMarker
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SoundMarker</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISoundMarker, ISoundMarkerGetter> SoundMarker(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISoundMarker, ISoundMarkerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISoundMarker, ISoundMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SpeechChallenge
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SpeechChallenge</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISpeechChallenge, ISpeechChallengeGetter> SpeechChallenge(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISpeechChallenge, ISpeechChallengeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISpeechChallengeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISpeechChallenge, ISpeechChallengeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SpeechChallenge
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SpeechChallenge</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISpeechChallenge, ISpeechChallengeGetter> SpeechChallenge(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISpeechChallenge, ISpeechChallengeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISpeechChallengeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISpeechChallenge, ISpeechChallengeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Spell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Spell</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISpell, ISpellGetter> Spell(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISpell, ISpellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISpell, ISpellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Spell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Spell</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISpell, ISpellGetter> Spell(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISpell, ISpellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISpell, ISpellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StarfieldMajorRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on StarfieldMajorRecord</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IStarfieldMajorRecord, IStarfieldMajorRecordGetter> StarfieldMajorRecord(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IStarfieldMajorRecord, IStarfieldMajorRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStarfieldMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IStarfieldMajorRecord, IStarfieldMajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StarfieldMajorRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on StarfieldMajorRecord</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IStarfieldMajorRecord, IStarfieldMajorRecordGetter> StarfieldMajorRecord(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IStarfieldMajorRecord, IStarfieldMajorRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStarfieldMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IStarfieldMajorRecord, IStarfieldMajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Static
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Static</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IStatic, IStaticGetter> Static(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IStatic, IStaticGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IStatic, IStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Static
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Static</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IStatic, IStaticGetter> Static(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IStatic, IStaticGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IStatic, IStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StaticCollection
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on StaticCollection</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IStaticCollection, IStaticCollectionGetter> StaticCollection(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IStaticCollection, IStaticCollectionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStaticCollectionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IStaticCollection, IStaticCollectionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StaticCollection
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on StaticCollection</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IStaticCollection, IStaticCollectionGetter> StaticCollection(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IStaticCollection, IStaticCollectionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStaticCollectionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IStaticCollection, IStaticCollectionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SurfaceBlock
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SurfaceBlock</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISurfaceBlock, ISurfaceBlockGetter> SurfaceBlock(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISurfaceBlock, ISurfaceBlockGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISurfaceBlockGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISurfaceBlock, ISurfaceBlockGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SurfaceBlock
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SurfaceBlock</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISurfaceBlock, ISurfaceBlockGetter> SurfaceBlock(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISurfaceBlock, ISurfaceBlockGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISurfaceBlockGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISurfaceBlock, ISurfaceBlockGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SurfacePattern
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SurfacePattern</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISurfacePattern, ISurfacePatternGetter> SurfacePattern(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISurfacePattern, ISurfacePatternGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISurfacePatternGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISurfacePattern, ISurfacePatternGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SurfacePattern
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SurfacePattern</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISurfacePattern, ISurfacePatternGetter> SurfacePattern(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISurfacePattern, ISurfacePatternGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISurfacePatternGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISurfacePattern, ISurfacePatternGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SurfacePatternConfig
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SurfacePatternConfig</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISurfacePatternConfig, ISurfacePatternConfigGetter> SurfacePatternConfig(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISurfacePatternConfig, ISurfacePatternConfigGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISurfacePatternConfigGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISurfacePatternConfig, ISurfacePatternConfigGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SurfacePatternConfig
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SurfacePatternConfig</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISurfacePatternConfig, ISurfacePatternConfigGetter> SurfacePatternConfig(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISurfacePatternConfig, ISurfacePatternConfigGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISurfacePatternConfigGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISurfacePatternConfig, ISurfacePatternConfigGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SurfacePatternStyle
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SurfacePatternStyle</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISurfacePatternStyle, ISurfacePatternStyleGetter> SurfacePatternStyle(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISurfacePatternStyle, ISurfacePatternStyleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISurfacePatternStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISurfacePatternStyle, ISurfacePatternStyleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SurfacePatternStyle
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SurfacePatternStyle</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISurfacePatternStyle, ISurfacePatternStyleGetter> SurfacePatternStyle(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISurfacePatternStyle, ISurfacePatternStyleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISurfacePatternStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISurfacePatternStyle, ISurfacePatternStyleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SurfaceTree
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SurfaceTree</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISurfaceTree, ISurfaceTreeGetter> SurfaceTree(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISurfaceTree, ISurfaceTreeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISurfaceTreeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISurfaceTree, ISurfaceTreeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SurfaceTree
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SurfaceTree</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISurfaceTree, ISurfaceTreeGetter> SurfaceTree(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISurfaceTree, ISurfaceTreeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISurfaceTreeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISurfaceTree, ISurfaceTreeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Terminal
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Terminal</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITerminal, ITerminalGetter> Terminal(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITerminal, ITerminalGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITerminalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ITerminal, ITerminalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Terminal
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Terminal</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITerminal, ITerminalGetter> Terminal(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITerminal, ITerminalGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITerminalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ITerminal, ITerminalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TerminalMenu
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on TerminalMenu</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITerminalMenu, ITerminalMenuGetter> TerminalMenu(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITerminalMenu, ITerminalMenuGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITerminalMenuGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ITerminalMenu, ITerminalMenuGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TerminalMenu
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on TerminalMenu</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITerminalMenu, ITerminalMenuGetter> TerminalMenu(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITerminalMenu, ITerminalMenuGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITerminalMenuGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ITerminalMenu, ITerminalMenuGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TextureSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on TextureSet</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITextureSet, ITextureSetGetter> TextureSet(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITextureSet, ITextureSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ITextureSet, ITextureSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TextureSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on TextureSet</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITextureSet, ITextureSetGetter> TextureSet(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITextureSet, ITextureSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ITextureSet, ITextureSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TimeOfDayData
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on TimeOfDayData</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITimeOfDayData, ITimeOfDayDataGetter> TimeOfDayData(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITimeOfDayData, ITimeOfDayDataGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITimeOfDayDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ITimeOfDayData, ITimeOfDayDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TimeOfDayData
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on TimeOfDayData</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITimeOfDayData, ITimeOfDayDataGetter> TimeOfDayData(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITimeOfDayData, ITimeOfDayDataGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITimeOfDayDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ITimeOfDayData, ITimeOfDayDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Transform
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Transform</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITransform, ITransformGetter> Transform(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITransform, ITransformGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITransformGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ITransform, ITransformGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Transform
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Transform</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITransform, ITransformGetter> Transform(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITransform, ITransformGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITransformGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ITransform, ITransformGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Traversal
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Traversal</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITraversal, ITraversalGetter> Traversal(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITraversal, ITraversalGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITraversalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ITraversal, ITraversalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Traversal
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Traversal</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITraversal, ITraversalGetter> Traversal(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITraversal, ITraversalGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITraversalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ITraversal, ITraversalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VoiceType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on VoiceType</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IVoiceType, IVoiceTypeGetter> VoiceType(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IVoiceType, IVoiceTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IVoiceType, IVoiceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VoiceType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on VoiceType</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IVoiceType, IVoiceTypeGetter> VoiceType(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IVoiceType, IVoiceTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IVoiceType, IVoiceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VolumetricLighting
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on VolumetricLighting</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IVolumetricLighting, IVolumetricLightingGetter> VolumetricLighting(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IVolumetricLighting, IVolumetricLightingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IVolumetricLightingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IVolumetricLighting, IVolumetricLightingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VolumetricLighting
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on VolumetricLighting</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IVolumetricLighting, IVolumetricLightingGetter> VolumetricLighting(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IVolumetricLighting, IVolumetricLightingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IVolumetricLightingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IVolumetricLighting, IVolumetricLightingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Water
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Water</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWater, IWaterGetter> Water(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWater, IWaterGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IWater, IWaterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Water
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Water</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWater, IWaterGetter> Water(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWater, IWaterGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IWater, IWaterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weapon
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Weapon</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWeapon, IWeaponGetter> Weapon(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWeapon, IWeaponGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IWeapon, IWeaponGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weapon
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Weapon</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWeapon, IWeaponGetter> Weapon(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWeapon, IWeaponGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IWeapon, IWeaponGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to WeaponBarrelModel
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on WeaponBarrelModel</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWeaponBarrelModel, IWeaponBarrelModelGetter> WeaponBarrelModel(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWeaponBarrelModel, IWeaponBarrelModelGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWeaponBarrelModelGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IWeaponBarrelModel, IWeaponBarrelModelGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to WeaponBarrelModel
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on WeaponBarrelModel</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWeaponBarrelModel, IWeaponBarrelModelGetter> WeaponBarrelModel(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWeaponBarrelModel, IWeaponBarrelModelGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWeaponBarrelModelGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IWeaponBarrelModel, IWeaponBarrelModelGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weather
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Weather</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWeather, IWeatherGetter> Weather(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWeather, IWeatherGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IWeather, IWeatherGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weather
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Weather</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWeather, IWeatherGetter> Weather(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWeather, IWeatherGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IWeather, IWeatherGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to WeatherSetting
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on WeatherSetting</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWeatherSetting, IWeatherSettingGetter> WeatherSetting(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWeatherSetting, IWeatherSettingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWeatherSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IWeatherSetting, IWeatherSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to WeatherSetting
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on WeatherSetting</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWeatherSetting, IWeatherSettingGetter> WeatherSetting(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWeatherSetting, IWeatherSettingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWeatherSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IWeatherSetting, IWeatherSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Worldspace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Worldspace</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWorldspace, IWorldspaceGetter> Worldspace(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWorldspace, IWorldspaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IWorldspace, IWorldspaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Worldspace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Worldspace</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWorldspace, IWorldspaceGetter> Worldspace(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWorldspace, IWorldspaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IWorldspace, IWorldspaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to WWiseEventData
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on WWiseEventData</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWWiseEventData, IWWiseEventDataGetter> WWiseEventData(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWWiseEventData, IWWiseEventDataGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWWiseEventDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IWWiseEventData, IWWiseEventDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to WWiseEventData
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on WWiseEventData</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWWiseEventData, IWWiseEventDataGetter> WWiseEventData(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWWiseEventData, IWWiseEventDataGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWWiseEventDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IWWiseEventData, IWWiseEventDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to WWiseKeywordMapping
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on WWiseKeywordMapping</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWWiseKeywordMapping, IWWiseKeywordMappingGetter> WWiseKeywordMapping(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWWiseKeywordMapping, IWWiseKeywordMappingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWWiseKeywordMappingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IWWiseKeywordMapping, IWWiseKeywordMappingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to WWiseKeywordMapping
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on WWiseKeywordMapping</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWWiseKeywordMapping, IWWiseKeywordMappingGetter> WWiseKeywordMapping(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IWWiseKeywordMapping, IWWiseKeywordMappingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWWiseKeywordMappingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IWWiseKeywordMapping, IWWiseKeywordMappingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Zoom
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Zoom</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IZoom, IZoomGetter> Zoom(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IZoom, IZoomGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IZoomGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IZoom, IZoomGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Zoom
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Zoom</returns>
        public static TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IZoom, IZoomGetter> Zoom(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IZoom, IZoomGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IZoomGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IZoom, IZoomGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

        #region Link Interfaces
        /// <summary>
        /// Scope a load order query to IIdleRelation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IIdleRelation</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IIdleRelation, IIdleRelationGetter> IIdleRelation(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IIdleRelation, IIdleRelationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IIdleRelation, IIdleRelationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IIdleRelation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IIdleRelation</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IIdleRelation, IIdleRelationGetter> IIdleRelation(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IIdleRelation, IIdleRelationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IIdleRelation, IIdleRelationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IConstructible
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IConstructible</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IConstructible, IConstructibleGetter> IConstructible(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IConstructible, IConstructibleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IConstructibleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IConstructible, IConstructibleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IConstructible
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IConstructible</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IConstructible, IConstructibleGetter> IConstructible(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IConstructible, IConstructibleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IConstructibleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IConstructible, IConstructibleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IStaticTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IStaticTarget</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IStaticTarget, IStaticTargetGetter> IStaticTarget(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IStaticTarget, IStaticTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStaticTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IStaticTarget, IStaticTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IStaticTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IStaticTarget</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IStaticTarget, IStaticTargetGetter> IStaticTarget(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IStaticTarget, IStaticTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStaticTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IStaticTarget, IStaticTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IItem</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IItem, IItemGetter> IItem(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IItem, IItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IItem, IItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IItem</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IItem, IItemGetter> IItem(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IItem, IItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IItem, IItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IOutfitTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IOutfitTarget</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IOutfitTarget, IOutfitTargetGetter> IOutfitTarget(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IOutfitTarget, IOutfitTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IOutfitTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IOutfitTarget, IOutfitTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IOutfitTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IOutfitTarget</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IOutfitTarget, IOutfitTargetGetter> IOutfitTarget(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IOutfitTarget, IOutfitTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IOutfitTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IOutfitTarget, IOutfitTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IBindableEquipment
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IBindableEquipment</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBindableEquipment, IBindableEquipmentGetter> IBindableEquipment(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBindableEquipment, IBindableEquipmentGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBindableEquipmentGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IBindableEquipment, IBindableEquipmentGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IBindableEquipment
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IBindableEquipment</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBindableEquipment, IBindableEquipmentGetter> IBindableEquipment(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IBindableEquipment, IBindableEquipmentGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBindableEquipmentGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IBindableEquipment, IBindableEquipmentGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to INpcTemplateTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on INpcTemplateTarget</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INpcTemplateTarget, INpcTemplateTargetGetter> INpcTemplateTarget(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INpcTemplateTarget, INpcTemplateTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcTemplateTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, INpcTemplateTarget, INpcTemplateTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to INpcTemplateTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on INpcTemplateTarget</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INpcTemplateTarget, INpcTemplateTargetGetter> INpcTemplateTarget(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INpcTemplateTarget, INpcTemplateTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INpcTemplateTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, INpcTemplateTarget, INpcTemplateTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IComplexLocation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IComplexLocation</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IComplexLocation, IComplexLocationGetter> IComplexLocation(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IComplexLocation, IComplexLocationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IComplexLocationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IComplexLocation, IComplexLocationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IComplexLocation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IComplexLocation</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IComplexLocation, IComplexLocationGetter> IComplexLocation(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IComplexLocation, IComplexLocationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IComplexLocationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IComplexLocation, IComplexLocationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ICellOrObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ICellOrObject</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICellOrObject, ICellOrObjectGetter> ICellOrObject(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICellOrObject, ICellOrObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICellOrObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ICellOrObject, ICellOrObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ICellOrObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ICellOrObject</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICellOrObject, ICellOrObjectGetter> ICellOrObject(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ICellOrObject, ICellOrObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICellOrObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ICellOrObject, ICellOrObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ISpaceCellSpawn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ISpaceCellSpawn</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISpaceCellSpawn, ISpaceCellSpawnGetter> ISpaceCellSpawn(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISpaceCellSpawn, ISpaceCellSpawnGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISpaceCellSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISpaceCellSpawn, ISpaceCellSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ISpaceCellSpawn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ISpaceCellSpawn</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISpaceCellSpawn, ISpaceCellSpawnGetter> ISpaceCellSpawn(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISpaceCellSpawn, ISpaceCellSpawnGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISpaceCellSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISpaceCellSpawn, ISpaceCellSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IResourceTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IResourceTarget</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IResourceTarget, IResourceTargetGetter> IResourceTarget(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IResourceTarget, IResourceTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IResourceTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IResourceTarget, IResourceTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IResourceTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IResourceTarget</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IResourceTarget, IResourceTargetGetter> IResourceTarget(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IResourceTarget, IResourceTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IResourceTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IResourceTarget, IResourceTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IOwner
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IOwner</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IOwner, IOwnerGetter> IOwner(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IOwner, IOwnerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IOwner, IOwnerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IOwner
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IOwner</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IOwner, IOwnerGetter> IOwner(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IOwner, IOwnerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IOwner, IOwnerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IObjectId
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IObjectId</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IObjectId, IObjectIdGetter> IObjectId(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IObjectId, IObjectIdGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IObjectIdGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IObjectId, IObjectIdGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IObjectId
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IObjectId</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IObjectId, IObjectIdGetter> IObjectId(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IObjectId, IObjectIdGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IObjectIdGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IObjectId, IObjectIdGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRelatable
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IRelatable</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IRelatable, IRelatableGetter> IRelatable(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IRelatable, IRelatableGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRelatableGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IRelatable, IRelatableGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRelatable
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IRelatable</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IRelatable, IRelatableGetter> IRelatable(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IRelatable, IRelatableGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRelatableGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IRelatable, IRelatableGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IAliasVoiceType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IAliasVoiceType</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAliasVoiceType, IAliasVoiceTypeGetter> IAliasVoiceType(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAliasVoiceType, IAliasVoiceTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAliasVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAliasVoiceType, IAliasVoiceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IAliasVoiceType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IAliasVoiceType</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAliasVoiceType, IAliasVoiceTypeGetter> IAliasVoiceType(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IAliasVoiceType, IAliasVoiceTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAliasVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IAliasVoiceType, IAliasVoiceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IVoiceTypeOrList
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IVoiceTypeOrList</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IVoiceTypeOrList, IVoiceTypeOrListGetter> IVoiceTypeOrList(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IVoiceTypeOrList, IVoiceTypeOrListGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IVoiceTypeOrListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IVoiceTypeOrList, IVoiceTypeOrListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IVoiceTypeOrList
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IVoiceTypeOrList</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IVoiceTypeOrList, IVoiceTypeOrListGetter> IVoiceTypeOrList(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IVoiceTypeOrList, IVoiceTypeOrListGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IVoiceTypeOrListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IVoiceTypeOrList, IVoiceTypeOrListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IExternalBaseTemplate
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IExternalBaseTemplate</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IExternalBaseTemplate, IExternalBaseTemplateGetter> IExternalBaseTemplate(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IExternalBaseTemplate, IExternalBaseTemplateGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IExternalBaseTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IExternalBaseTemplate, IExternalBaseTemplateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IExternalBaseTemplate
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IExternalBaseTemplate</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IExternalBaseTemplate, IExternalBaseTemplateGetter> IExternalBaseTemplate(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IExternalBaseTemplate, IExternalBaseTemplateGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IExternalBaseTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IExternalBaseTemplate, IExternalBaseTemplateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILeveledBaseFormTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ILeveledBaseFormTarget</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledBaseFormTarget, ILeveledBaseFormTargetGetter> ILeveledBaseFormTarget(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledBaseFormTarget, ILeveledBaseFormTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledBaseFormTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILeveledBaseFormTarget, ILeveledBaseFormTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILeveledBaseFormTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ILeveledBaseFormTarget</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledBaseFormTarget, ILeveledBaseFormTargetGetter> ILeveledBaseFormTarget(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledBaseFormTarget, ILeveledBaseFormTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledBaseFormTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILeveledBaseFormTarget, ILeveledBaseFormTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IKeywordLinkedReference
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IKeywordLinkedReference</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter> IKeywordLinkedReference(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IKeywordLinkedReference
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IKeywordLinkedReference</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter> IKeywordLinkedReference(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeywordLinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IKeywordLinkedReference, IKeywordLinkedReferenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IHarvestTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IHarvestTarget</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IHarvestTarget, IHarvestTargetGetter> IHarvestTarget(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IHarvestTarget, IHarvestTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHarvestTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IHarvestTarget, IHarvestTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IHarvestTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IHarvestTarget</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IHarvestTarget, IHarvestTargetGetter> IHarvestTarget(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IHarvestTarget, IHarvestTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHarvestTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IHarvestTarget, IHarvestTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to INpcSpawn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on INpcSpawn</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INpcSpawn, INpcSpawnGetter> INpcSpawn(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INpcSpawn, INpcSpawnGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, INpcSpawn, INpcSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to INpcSpawn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on INpcSpawn</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INpcSpawn, INpcSpawnGetter> INpcSpawn(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, INpcSpawn, INpcSpawnGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, INpcSpawn, INpcSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILeveledPackInTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ILeveledPackInTarget</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledPackInTarget, ILeveledPackInTargetGetter> ILeveledPackInTarget(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledPackInTarget, ILeveledPackInTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledPackInTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILeveledPackInTarget, ILeveledPackInTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILeveledPackInTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ILeveledPackInTarget</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledPackInTarget, ILeveledPackInTargetGetter> ILeveledPackInTarget(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILeveledPackInTarget, ILeveledPackInTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledPackInTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILeveledPackInTarget, ILeveledPackInTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IEmittance
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IEmittance</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IEmittance, IEmittanceGetter> IEmittance(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IEmittance, IEmittanceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEmittanceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IEmittance, IEmittanceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IEmittance
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IEmittance</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IEmittance, IEmittanceGetter> IEmittance(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IEmittance, IEmittanceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEmittanceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IEmittance, IEmittanceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILocationRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ILocationRecord</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILocationRecord, ILocationRecordGetter> ILocationRecord(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILocationRecord, ILocationRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILocationRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILocationRecord, ILocationRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILocationRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ILocationRecord</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILocationRecord, ILocationRecordGetter> ILocationRecord(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILocationRecord, ILocationRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILocationRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILocationRecord, ILocationRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IConstructibleObjectTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IConstructibleObjectTarget</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IConstructibleObjectTarget, IConstructibleObjectTargetGetter> IConstructibleObjectTarget(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IConstructibleObjectTarget, IConstructibleObjectTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IConstructibleObjectTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IConstructibleObjectTarget, IConstructibleObjectTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IConstructibleObjectTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IConstructibleObjectTarget</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IConstructibleObjectTarget, IConstructibleObjectTargetGetter> IConstructibleObjectTarget(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IConstructibleObjectTarget, IConstructibleObjectTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IConstructibleObjectTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IConstructibleObjectTarget, IConstructibleObjectTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaced
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlaced</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlaced, IPlacedGetter> IPlaced(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlaced, IPlacedGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlaced, IPlacedGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaced
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlaced</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlaced, IPlacedGetter> IPlaced(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlaced, IPlacedGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlaced, IPlacedGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlacedSimple
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlacedSimple</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlacedSimple, IPlacedSimpleGetter> IPlacedSimple(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlacedSimple, IPlacedSimpleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedSimpleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlacedSimple, IPlacedSimpleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlacedSimple
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlacedSimple</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlacedSimple, IPlacedSimpleGetter> IPlacedSimple(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlacedSimple, IPlacedSimpleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedSimpleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlacedSimple, IPlacedSimpleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILinkedReference
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ILinkedReference</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILinkedReference, ILinkedReferenceGetter> ILinkedReference(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILinkedReference, ILinkedReferenceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILinkedReference, ILinkedReferenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ILinkedReference
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ILinkedReference</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILinkedReference, ILinkedReferenceGetter> ILinkedReference(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ILinkedReference, ILinkedReferenceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILinkedReferenceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ILinkedReference, ILinkedReferenceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlacedThing
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlacedThing</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlacedThing, IPlacedThingGetter> IPlacedThing(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlacedThing, IPlacedThingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedThingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlacedThing, IPlacedThingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlacedThing
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlacedThing</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlacedThing, IPlacedThingGetter> IPlacedThing(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlacedThing, IPlacedThingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedThingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlacedThing, IPlacedThingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ITraversalTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ITraversalTarget</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITraversalTarget, ITraversalTargetGetter> ITraversalTarget(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITraversalTarget, ITraversalTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITraversalTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ITraversalTarget, ITraversalTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ITraversalTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ITraversalTarget</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITraversalTarget, ITraversalTargetGetter> ITraversalTarget(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ITraversalTarget, ITraversalTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITraversalTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ITraversalTarget, ITraversalTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ISpellRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ISpellRecord</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISpellRecord, ISpellRecordGetter> ISpellRecord(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISpellRecord, ISpellRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISpellRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISpellRecord, ISpellRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ISpellRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ISpellRecord</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISpellRecord, ISpellRecordGetter> ISpellRecord(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, ISpellRecord, ISpellRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISpellRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, ISpellRecord, ISpellRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IEffectRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IEffectRecord</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IEffectRecord, IEffectRecordGetter> IEffectRecord(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IEffectRecord, IEffectRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEffectRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IEffectRecord, IEffectRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IEffectRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IEffectRecord</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IEffectRecord, IEffectRecordGetter> IEffectRecord(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IEffectRecord, IEffectRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEffectRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IEffectRecord, IEffectRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaceableObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlaceableObject</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlaceableObject, IPlaceableObjectGetter> IPlaceableObject(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlaceableObject, IPlaceableObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlaceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlaceableObject, IPlaceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaceableObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlaceableObject</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlaceableObject, IPlaceableObjectGetter> IPlaceableObject(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IPlaceableObject, IPlaceableObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlaceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IPlaceableObject, IPlaceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IReferenceableObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IReferenceableObject</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IReferenceableObject, IReferenceableObjectGetter> IReferenceableObject(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IReferenceableObject, IReferenceableObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IReferenceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IReferenceableObject, IReferenceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IReferenceableObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IReferenceableObject</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IReferenceableObject, IReferenceableObjectGetter> IReferenceableObject(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IReferenceableObject, IReferenceableObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IReferenceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IReferenceableObject, IReferenceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IExplodeSpawn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IExplodeSpawn</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IExplodeSpawn, IExplodeSpawnGetter> IExplodeSpawn(this IEnumerable<IModListingGetter<IStarfieldModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IExplodeSpawn, IExplodeSpawnGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IExplodeSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IExplodeSpawn, IExplodeSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IExplodeSpawn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IExplodeSpawn</returns>
        public static TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IExplodeSpawn, IExplodeSpawnGetter> IExplodeSpawn(this IEnumerable<IStarfieldModGetter> mods)
        {
            return new TypedLoadOrderAccess<IStarfieldMod, IStarfieldModGetter, IExplodeSpawn, IExplodeSpawnGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IExplodeSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IStarfieldMod, IStarfieldModGetter, IExplodeSpawn, IExplodeSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

    }
}
