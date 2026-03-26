using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.FalloutNV
{
    public static class TypeOptionSolidifierMixIns
    {
        #region Normal
        /// <summary>
        /// Scope a load order query to AcousticSpace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AcousticSpace</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAcousticSpace, IAcousticSpaceGetter> AcousticSpace(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAcousticSpace, IAcousticSpaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IAcousticSpace, IAcousticSpaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AcousticSpace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AcousticSpace</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAcousticSpace, IAcousticSpaceGetter> AcousticSpace(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAcousticSpace, IAcousticSpaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IAcousticSpace, IAcousticSpaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Activator
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Activator</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IActivator, IActivatorGetter> Activator(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IActivator, IActivatorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IActivator, IActivatorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Activator
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Activator</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IActivator, IActivatorGetter> Activator(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IActivator, IActivatorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IActivator, IActivatorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActorValueInformation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ActorValueInformation</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IActorValueInformation, IActorValueInformationGetter> ActorValueInformation(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IActorValueInformation, IActorValueInformationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IActorValueInformation, IActorValueInformationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActorValueInformation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ActorValueInformation</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IActorValueInformation, IActorValueInformationGetter> ActorValueInformation(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IActorValueInformation, IActorValueInformationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IActorValueInformation, IActorValueInformationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AddonNode
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AddonNode</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAddonNode, IAddonNodeGetter> AddonNode(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAddonNode, IAddonNodeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IAddonNode, IAddonNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AddonNode
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AddonNode</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAddonNode, IAddonNodeGetter> AddonNode(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAddonNode, IAddonNodeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IAddonNode, IAddonNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AmmoEffect
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AmmoEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAmmoEffect, IAmmoEffectGetter> AmmoEffect(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAmmoEffect, IAmmoEffectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAmmoEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IAmmoEffect, IAmmoEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AmmoEffect
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AmmoEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAmmoEffect, IAmmoEffectGetter> AmmoEffect(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAmmoEffect, IAmmoEffectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAmmoEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IAmmoEffect, IAmmoEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ammunition
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ammunition</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAmmunition, IAmmunitionGetter> Ammunition(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAmmunition, IAmmunitionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IAmmunition, IAmmunitionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ammunition
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ammunition</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAmmunition, IAmmunitionGetter> Ammunition(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAmmunition, IAmmunitionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IAmmunition, IAmmunitionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AnimatedObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AnimatedObject</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAnimatedObject, IAnimatedObjectGetter> AnimatedObject(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAnimatedObject, IAnimatedObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IAnimatedObject, IAnimatedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AnimatedObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AnimatedObject</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAnimatedObject, IAnimatedObjectGetter> AnimatedObject(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAnimatedObject, IAnimatedObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IAnimatedObject, IAnimatedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Armor
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Armor</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IArmor, IArmorGetter> Armor(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IArmor, IArmorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IArmor, IArmorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Armor
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Armor</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IArmor, IArmorGetter> Armor(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IArmor, IArmorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IArmor, IArmorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ArmorAddon
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ArmorAddon</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IArmorAddon, IArmorAddonGetter> ArmorAddon(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IArmorAddon, IArmorAddonGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IArmorAddon, IArmorAddonGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ArmorAddon
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ArmorAddon</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IArmorAddon, IArmorAddonGetter> ArmorAddon(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IArmorAddon, IArmorAddonGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IArmorAddon, IArmorAddonGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BodyPartData
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on BodyPartData</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IBodyPartData, IBodyPartDataGetter> BodyPartData(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IBodyPartData, IBodyPartDataGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IBodyPartData, IBodyPartDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BodyPartData
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on BodyPartData</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IBodyPartData, IBodyPartDataGetter> BodyPartData(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IBodyPartData, IBodyPartDataGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IBodyPartData, IBodyPartDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Book
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Book</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IBook, IBookGetter> Book(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IBook, IBookGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IBook, IBookGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Book
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Book</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IBook, IBookGetter> Book(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IBook, IBookGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IBook, IBookGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraPath
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CameraPath</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICameraPath, ICameraPathGetter> CameraPath(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICameraPath, ICameraPathGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICameraPath, ICameraPathGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraPath
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CameraPath</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICameraPath, ICameraPathGetter> CameraPath(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICameraPath, ICameraPathGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICameraPath, ICameraPathGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraShot
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CameraShot</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICameraShot, ICameraShotGetter> CameraShot(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICameraShot, ICameraShotGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICameraShot, ICameraShotGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraShot
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CameraShot</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICameraShot, ICameraShotGetter> CameraShot(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICameraShot, ICameraShotGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICameraShot, ICameraShotGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CaravanCard
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CaravanCard</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICaravanCard, ICaravanCardGetter> CaravanCard(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICaravanCard, ICaravanCardGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICaravanCardGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICaravanCard, ICaravanCardGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CaravanCard
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CaravanCard</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICaravanCard, ICaravanCardGetter> CaravanCard(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICaravanCard, ICaravanCardGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICaravanCardGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICaravanCard, ICaravanCardGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CaravanDeck
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CaravanDeck</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICaravanDeck, ICaravanDeckGetter> CaravanDeck(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICaravanDeck, ICaravanDeckGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICaravanDeckGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICaravanDeck, ICaravanDeckGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CaravanDeck
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CaravanDeck</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICaravanDeck, ICaravanDeckGetter> CaravanDeck(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICaravanDeck, ICaravanDeckGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICaravanDeckGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICaravanDeck, ICaravanDeckGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CaravanMoney
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CaravanMoney</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICaravanMoney, ICaravanMoneyGetter> CaravanMoney(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICaravanMoney, ICaravanMoneyGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICaravanMoneyGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICaravanMoney, ICaravanMoneyGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CaravanMoney
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CaravanMoney</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICaravanMoney, ICaravanMoneyGetter> CaravanMoney(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICaravanMoney, ICaravanMoneyGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICaravanMoneyGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICaravanMoney, ICaravanMoneyGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Casino
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Casino</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICasino, ICasinoGetter> Casino(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICasino, ICasinoGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICasinoGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICasino, ICasinoGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Casino
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Casino</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICasino, ICasinoGetter> Casino(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICasino, ICasinoGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICasinoGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICasino, ICasinoGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CasinoChip
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CasinoChip</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICasinoChip, ICasinoChipGetter> CasinoChip(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICasinoChip, ICasinoChipGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICasinoChipGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICasinoChip, ICasinoChipGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CasinoChip
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CasinoChip</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICasinoChip, ICasinoChipGetter> CasinoChip(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICasinoChip, ICasinoChipGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICasinoChipGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICasinoChip, ICasinoChipGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Cell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Cell</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICell, ICellGetter> Cell(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICell, ICellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICell, ICellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Cell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Cell</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICell, ICellGetter> Cell(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICell, ICellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICell, ICellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Challenge
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Challenge</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IChallenge, IChallengeGetter> Challenge(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IChallenge, IChallengeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IChallengeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IChallenge, IChallengeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Challenge
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Challenge</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IChallenge, IChallengeGetter> Challenge(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IChallenge, IChallengeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IChallengeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IChallenge, IChallengeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Class
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Class</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IClass, IClassGetter> Class(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IClass, IClassGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IClass, IClassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Class
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Class</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IClass, IClassGetter> Class(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IClass, IClassGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IClass, IClassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Climate
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Climate</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IClimate, IClimateGetter> Climate(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IClimate, IClimateGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IClimate, IClimateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Climate
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Climate</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IClimate, IClimateGetter> Climate(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IClimate, IClimateGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IClimate, IClimateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CombatStyle
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CombatStyle</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICombatStyle, ICombatStyleGetter> CombatStyle(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICombatStyle, ICombatStyleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICombatStyle, ICombatStyleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CombatStyle
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CombatStyle</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICombatStyle, ICombatStyleGetter> CombatStyle(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICombatStyle, ICombatStyleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICombatStyle, ICombatStyleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ConstructibleObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ConstructibleObject</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IConstructibleObject, IConstructibleObjectGetter> ConstructibleObject(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IConstructibleObject, IConstructibleObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IConstructibleObject, IConstructibleObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ConstructibleObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ConstructibleObject</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IConstructibleObject, IConstructibleObjectGetter> ConstructibleObject(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IConstructibleObject, IConstructibleObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IConstructibleObject, IConstructibleObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Container
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Container</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IContainer, IContainerGetter> Container(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IContainer, IContainerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IContainer, IContainerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Container
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Container</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IContainer, IContainerGetter> Container(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IContainer, IContainerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IContainer, IContainerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Creature
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Creature</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICreature, ICreatureGetter> Creature(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICreature, ICreatureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICreature, ICreatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Creature
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Creature</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICreature, ICreatureGetter> Creature(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICreature, ICreatureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICreature, ICreatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Debris
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Debris</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDebris, IDebrisGetter> Debris(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDebris, IDebrisGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDebrisGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IDebris, IDebrisGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Debris
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Debris</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDebris, IDebrisGetter> Debris(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDebris, IDebrisGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDebrisGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IDebris, IDebrisGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DefaultObjectManager
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DefaultObjectManager</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter> DefaultObjectManager(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DefaultObjectManager
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DefaultObjectManager</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter> DefaultObjectManager(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DehydrationStage
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DehydrationStage</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDehydrationStage, IDehydrationStageGetter> DehydrationStage(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDehydrationStage, IDehydrationStageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDehydrationStageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IDehydrationStage, IDehydrationStageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DehydrationStage
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DehydrationStage</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDehydrationStage, IDehydrationStageGetter> DehydrationStage(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDehydrationStage, IDehydrationStageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDehydrationStageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IDehydrationStage, IDehydrationStageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogResponses
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogResponses</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDialogResponses, IDialogResponsesGetter> DialogResponses(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDialogResponses, IDialogResponsesGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IDialogResponses, IDialogResponsesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogResponses
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogResponses</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDialogResponses, IDialogResponsesGetter> DialogResponses(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDialogResponses, IDialogResponsesGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IDialogResponses, IDialogResponsesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogTopic
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogTopic</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDialogTopic, IDialogTopicGetter> DialogTopic(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDialogTopic, IDialogTopicGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IDialogTopic, IDialogTopicGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogTopic
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogTopic</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDialogTopic, IDialogTopicGetter> DialogTopic(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDialogTopic, IDialogTopicGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IDialogTopic, IDialogTopicGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Door
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Door</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDoor, IDoorGetter> Door(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDoor, IDoorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IDoor, IDoorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Door
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Door</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDoor, IDoorGetter> Door(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IDoor, IDoorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IDoor, IDoorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EffectShader
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EffectShader</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IEffectShader, IEffectShaderGetter> EffectShader(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IEffectShader, IEffectShaderGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IEffectShader, IEffectShaderGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EffectShader
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EffectShader</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IEffectShader, IEffectShaderGetter> EffectShader(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IEffectShader, IEffectShaderGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IEffectShader, IEffectShaderGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EncounterZone
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EncounterZone</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IEncounterZone, IEncounterZoneGetter> EncounterZone(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IEncounterZone, IEncounterZoneGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEncounterZoneGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IEncounterZone, IEncounterZoneGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EncounterZone
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EncounterZone</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IEncounterZone, IEncounterZoneGetter> EncounterZone(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IEncounterZone, IEncounterZoneGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEncounterZoneGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IEncounterZone, IEncounterZoneGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Explosion
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Explosion</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IExplosion, IExplosionGetter> Explosion(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IExplosion, IExplosionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IExplosionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IExplosion, IExplosionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Explosion
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Explosion</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IExplosion, IExplosionGetter> Explosion(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IExplosion, IExplosionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IExplosionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IExplosion, IExplosionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Eyes
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Eyes</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IEyes, IEyesGetter> Eyes(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IEyes, IEyesGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEyesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IEyes, IEyesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Eyes
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Eyes</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IEyes, IEyesGetter> Eyes(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IEyes, IEyesGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEyesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IEyes, IEyesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Faction
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Faction</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IFaction, IFactionGetter> Faction(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IFaction, IFactionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IFaction, IFactionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Faction
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Faction</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IFaction, IFactionGetter> Faction(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IFaction, IFactionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IFaction, IFactionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FalloutNVMajorRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on FalloutNVMajorRecord</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IFalloutNVMajorRecord, IFalloutNVMajorRecordGetter> FalloutNVMajorRecord(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IFalloutNVMajorRecord, IFalloutNVMajorRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFalloutNVMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IFalloutNVMajorRecord, IFalloutNVMajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FalloutNVMajorRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on FalloutNVMajorRecord</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IFalloutNVMajorRecord, IFalloutNVMajorRecordGetter> FalloutNVMajorRecord(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IFalloutNVMajorRecord, IFalloutNVMajorRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFalloutNVMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IFalloutNVMajorRecord, IFalloutNVMajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FormList
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on FormList</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IFormList, IFormListGetter> FormList(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IFormList, IFormListGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFormListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IFormList, IFormListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FormList
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on FormList</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IFormList, IFormListGetter> FormList(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IFormList, IFormListGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFormListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IFormList, IFormListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Furniture
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Furniture</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IFurniture, IFurnitureGetter> Furniture(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IFurniture, IFurnitureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IFurniture, IFurnitureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Furniture
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Furniture</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IFurniture, IFurnitureGetter> Furniture(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IFurniture, IFurnitureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IFurniture, IFurnitureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Global
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Global</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IGlobal, IGlobalGetter> Global(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IGlobal, IGlobalGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IGlobal, IGlobalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Global
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Global</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IGlobal, IGlobalGetter> Global(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IGlobal, IGlobalGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IGlobal, IGlobalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Grass
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Grass</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IGrass, IGrassGetter> Grass(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IGrass, IGrassGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IGrass, IGrassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Grass
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Grass</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IGrass, IGrassGetter> Grass(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IGrass, IGrassGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IGrass, IGrassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hair
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Hair</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IHair, IHairGetter> Hair(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IHair, IHairGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IHair, IHairGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hair
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Hair</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IHair, IHairGetter> Hair(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IHair, IHairGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IHair, IHairGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to HeadPart
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on HeadPart</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IHeadPart, IHeadPartGetter> HeadPart(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IHeadPart, IHeadPartGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IHeadPart, IHeadPartGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to HeadPart
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on HeadPart</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IHeadPart, IHeadPartGetter> HeadPart(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IHeadPart, IHeadPartGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IHeadPart, IHeadPartGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to HungerStage
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on HungerStage</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IHungerStage, IHungerStageGetter> HungerStage(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IHungerStage, IHungerStageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHungerStageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IHungerStage, IHungerStageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to HungerStage
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on HungerStage</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IHungerStage, IHungerStageGetter> HungerStage(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IHungerStage, IHungerStageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHungerStageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IHungerStage, IHungerStageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleAnimation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IdleAnimation</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IIdleAnimation, IIdleAnimationGetter> IdleAnimation(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IIdleAnimation, IIdleAnimationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IIdleAnimation, IIdleAnimationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleAnimation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IdleAnimation</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IIdleAnimation, IIdleAnimationGetter> IdleAnimation(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IIdleAnimation, IIdleAnimationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IIdleAnimation, IIdleAnimationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleMarker
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IdleMarker</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IIdleMarker, IIdleMarkerGetter> IdleMarker(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IIdleMarker, IIdleMarkerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IIdleMarker, IIdleMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleMarker
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IdleMarker</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IIdleMarker, IIdleMarkerGetter> IdleMarker(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IIdleMarker, IIdleMarkerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IIdleMarker, IIdleMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ImageSpace</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IImageSpace, IImageSpaceGetter> ImageSpace(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IImageSpace, IImageSpaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IImageSpace, IImageSpaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ImageSpace</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IImageSpace, IImageSpaceGetter> ImageSpace(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IImageSpace, IImageSpaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IImageSpace, IImageSpaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpaceAdapter
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ImageSpaceAdapter</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter> ImageSpaceAdapter(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpaceAdapter
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ImageSpaceAdapter</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter> ImageSpaceAdapter(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Impact
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Impact</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IImpact, IImpactGetter> Impact(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IImpact, IImpactGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImpactGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IImpact, IImpactGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Impact
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Impact</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IImpact, IImpactGetter> Impact(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IImpact, IImpactGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImpactGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IImpact, IImpactGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImpactDataSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ImpactDataSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IImpactDataSet, IImpactDataSetGetter> ImpactDataSet(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IImpactDataSet, IImpactDataSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IImpactDataSet, IImpactDataSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImpactDataSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ImpactDataSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IImpactDataSet, IImpactDataSetGetter> ImpactDataSet(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IImpactDataSet, IImpactDataSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IImpactDataSet, IImpactDataSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingestible
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ingestible</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IIngestible, IIngestibleGetter> Ingestible(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IIngestible, IIngestibleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IIngestible, IIngestibleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingestible
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ingestible</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IIngestible, IIngestibleGetter> Ingestible(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IIngestible, IIngestibleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IIngestible, IIngestibleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingredient
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ingredient</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IIngredient, IIngredientGetter> Ingredient(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IIngredient, IIngredientGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IIngredient, IIngredientGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingredient
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ingredient</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IIngredient, IIngredientGetter> Ingredient(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IIngredient, IIngredientGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IIngredient, IIngredientGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ItemMod
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ItemMod</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IItemMod, IItemModGetter> ItemMod(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IItemMod, IItemModGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IItemModGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IItemMod, IItemModGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ItemMod
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ItemMod</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IItemMod, IItemModGetter> ItemMod(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IItemMod, IItemModGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IItemModGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IItemMod, IItemModGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Key
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Key</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IKey, IKeyGetter> Key(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IKey, IKeyGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IKey, IKeyGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Key
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Key</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IKey, IKeyGetter> Key(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IKey, IKeyGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IKey, IKeyGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Landscape
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Landscape</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILandscape, ILandscapeGetter> Landscape(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILandscape, ILandscapeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ILandscape, ILandscapeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Landscape
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Landscape</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILandscape, ILandscapeGetter> Landscape(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILandscape, ILandscapeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ILandscape, ILandscapeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LandscapeTexture
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LandscapeTexture</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILandscapeTexture, ILandscapeTextureGetter> LandscapeTexture(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILandscapeTexture, ILandscapeTextureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ILandscapeTexture, ILandscapeTextureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LandscapeTexture
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LandscapeTexture</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILandscapeTexture, ILandscapeTextureGetter> LandscapeTexture(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILandscapeTexture, ILandscapeTextureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ILandscapeTexture, ILandscapeTextureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledCreature
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledCreature</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILeveledCreature, ILeveledCreatureGetter> LeveledCreature(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILeveledCreature, ILeveledCreatureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledCreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ILeveledCreature, ILeveledCreatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledCreature
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledCreature</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILeveledCreature, ILeveledCreatureGetter> LeveledCreature(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILeveledCreature, ILeveledCreatureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledCreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ILeveledCreature, ILeveledCreatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledItem</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILeveledItem, ILeveledItemGetter> LeveledItem(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILeveledItem, ILeveledItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ILeveledItem, ILeveledItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledItem</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILeveledItem, ILeveledItemGetter> LeveledItem(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILeveledItem, ILeveledItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ILeveledItem, ILeveledItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledNpc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledNpc</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILeveledNpc, ILeveledNpcGetter> LeveledNpc(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILeveledNpc, ILeveledNpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ILeveledNpc, ILeveledNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledNpc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledNpc</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILeveledNpc, ILeveledNpcGetter> LeveledNpc(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILeveledNpc, ILeveledNpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ILeveledNpc, ILeveledNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Light
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Light</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILight, ILightGetter> Light(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILight, ILightGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ILight, ILightGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Light
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Light</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILight, ILightGetter> Light(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILight, ILightGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ILight, ILightGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LightingTemplate
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LightingTemplate</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILightingTemplate, ILightingTemplateGetter> LightingTemplate(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILightingTemplate, ILightingTemplateGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ILightingTemplate, ILightingTemplateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LightingTemplate
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LightingTemplate</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILightingTemplate, ILightingTemplateGetter> LightingTemplate(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILightingTemplate, ILightingTemplateGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ILightingTemplate, ILightingTemplateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LoadScreen
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LoadScreen</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILoadScreen, ILoadScreenGetter> LoadScreen(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILoadScreen, ILoadScreenGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ILoadScreen, ILoadScreenGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LoadScreen
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LoadScreen</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILoadScreen, ILoadScreenGetter> LoadScreen(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILoadScreen, ILoadScreenGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ILoadScreen, ILoadScreenGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LoadScreenType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LoadScreenType</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILoadScreenType, ILoadScreenTypeGetter> LoadScreenType(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILoadScreenType, ILoadScreenTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILoadScreenTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ILoadScreenType, ILoadScreenTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LoadScreenType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LoadScreenType</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILoadScreenType, ILoadScreenTypeGetter> LoadScreenType(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ILoadScreenType, ILoadScreenTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILoadScreenTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ILoadScreenType, ILoadScreenTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MagicEffect
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MagicEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMagicEffect, IMagicEffectGetter> MagicEffect(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMagicEffect, IMagicEffectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IMagicEffect, IMagicEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MagicEffect
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MagicEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMagicEffect, IMagicEffectGetter> MagicEffect(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMagicEffect, IMagicEffectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IMagicEffect, IMagicEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MediaLocationController
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MediaLocationController</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMediaLocationController, IMediaLocationControllerGetter> MediaLocationController(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMediaLocationController, IMediaLocationControllerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMediaLocationControllerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IMediaLocationController, IMediaLocationControllerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MediaLocationController
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MediaLocationController</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMediaLocationController, IMediaLocationControllerGetter> MediaLocationController(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMediaLocationController, IMediaLocationControllerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMediaLocationControllerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IMediaLocationController, IMediaLocationControllerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MediaSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MediaSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMediaSet, IMediaSetGetter> MediaSet(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMediaSet, IMediaSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMediaSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IMediaSet, IMediaSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MediaSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MediaSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMediaSet, IMediaSetGetter> MediaSet(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMediaSet, IMediaSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMediaSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IMediaSet, IMediaSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MenuIcon
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MenuIcon</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMenuIcon, IMenuIconGetter> MenuIcon(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMenuIcon, IMenuIconGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMenuIconGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IMenuIcon, IMenuIconGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MenuIcon
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MenuIcon</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMenuIcon, IMenuIconGetter> MenuIcon(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMenuIcon, IMenuIconGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMenuIconGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IMenuIcon, IMenuIconGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Message
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Message</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMessage, IMessageGetter> Message(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMessage, IMessageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMessageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IMessage, IMessageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Message
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Message</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMessage, IMessageGetter> Message(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMessage, IMessageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMessageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IMessage, IMessageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MiscItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MiscItem</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMiscItem, IMiscItemGetter> MiscItem(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMiscItem, IMiscItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IMiscItem, IMiscItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MiscItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MiscItem</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMiscItem, IMiscItemGetter> MiscItem(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMiscItem, IMiscItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IMiscItem, IMiscItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MoveableStatic
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MoveableStatic</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMoveableStatic, IMoveableStaticGetter> MoveableStatic(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMoveableStatic, IMoveableStaticGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMoveableStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IMoveableStatic, IMoveableStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MoveableStatic
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MoveableStatic</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMoveableStatic, IMoveableStaticGetter> MoveableStatic(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMoveableStatic, IMoveableStaticGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMoveableStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IMoveableStatic, IMoveableStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MusicType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MusicType</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMusicType, IMusicTypeGetter> MusicType(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMusicType, IMusicTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IMusicType, IMusicTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MusicType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MusicType</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMusicType, IMusicTypeGetter> MusicType(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IMusicType, IMusicTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IMusicType, IMusicTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMesh
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on NavigationMesh</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INavigationMesh, INavigationMeshGetter> NavigationMesh(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INavigationMesh, INavigationMeshGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, INavigationMesh, INavigationMeshGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMesh
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on NavigationMesh</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INavigationMesh, INavigationMeshGetter> NavigationMesh(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INavigationMesh, INavigationMeshGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, INavigationMesh, INavigationMeshGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMeshInfoMap
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on NavigationMeshInfoMap</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter> NavigationMeshInfoMap(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMeshInfoMap
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on NavigationMeshInfoMap</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter> NavigationMeshInfoMap(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Note
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Note</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INote, INoteGetter> Note(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INote, INoteGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INoteGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, INote, INoteGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Note
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Note</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INote, INoteGetter> Note(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INote, INoteGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INoteGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, INote, INoteGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INpc, INpcGetter> Npc(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INpc, INpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, INpc, INpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INpc, INpcGetter> Npc(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INpc, INpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, INpc, INpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ObjectEffect
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ObjectEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IObjectEffect, IObjectEffectGetter> ObjectEffect(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IObjectEffect, IObjectEffectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IObjectEffect, IObjectEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ObjectEffect
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ObjectEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IObjectEffect, IObjectEffectGetter> ObjectEffect(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IObjectEffect, IObjectEffectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IObjectEffect, IObjectEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Package
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Package</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPackage, IPackageGetter> Package(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPackage, IPackageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPackageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPackage, IPackageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Package
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Package</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPackage, IPackageGetter> Package(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPackage, IPackageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPackageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPackage, IPackageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Perk
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Perk</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPerk, IPerkGetter> Perk(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPerk, IPerkGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPerkGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPerk, IPerkGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Perk
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Perk</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPerk, IPerkGetter> Perk(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPerk, IPerkGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPerkGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPerk, IPerkGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlaceableWater
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlaceableWater</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlaceableWater, IPlaceableWaterGetter> PlaceableWater(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlaceableWater, IPlaceableWaterGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlaceableWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPlaceableWater, IPlaceableWaterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlaceableWater
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlaceableWater</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlaceableWater, IPlaceableWaterGetter> PlaceableWater(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlaceableWater, IPlaceableWaterGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlaceableWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPlaceableWater, IPlaceableWaterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedBeam
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedBeam</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedBeam, IPlacedBeamGetter> PlacedBeam(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedBeam, IPlacedBeamGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedBeamGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPlacedBeam, IPlacedBeamGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedBeam
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedBeam</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedBeam, IPlacedBeamGetter> PlacedBeam(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedBeam, IPlacedBeamGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedBeamGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPlacedBeam, IPlacedBeamGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedCreature
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedCreature</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedCreature, IPlacedCreatureGetter> PlacedCreature(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedCreature, IPlacedCreatureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedCreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPlacedCreature, IPlacedCreatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedCreature
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedCreature</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedCreature, IPlacedCreatureGetter> PlacedCreature(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedCreature, IPlacedCreatureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedCreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPlacedCreature, IPlacedCreatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedGrenade
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedGrenade</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedGrenade, IPlacedGrenadeGetter> PlacedGrenade(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedGrenade, IPlacedGrenadeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedGrenadeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPlacedGrenade, IPlacedGrenadeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedGrenade
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedGrenade</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedGrenade, IPlacedGrenadeGetter> PlacedGrenade(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedGrenade, IPlacedGrenadeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedGrenadeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPlacedGrenade, IPlacedGrenadeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedMissile
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedMissile</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedMissile, IPlacedMissileGetter> PlacedMissile(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedMissile, IPlacedMissileGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedMissileGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPlacedMissile, IPlacedMissileGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedMissile
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedMissile</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedMissile, IPlacedMissileGetter> PlacedMissile(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedMissile, IPlacedMissileGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedMissileGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPlacedMissile, IPlacedMissileGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedNpc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedNpc</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedNpc, IPlacedNpcGetter> PlacedNpc(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedNpc, IPlacedNpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPlacedNpc, IPlacedNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedNpc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedNpc</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedNpc, IPlacedNpcGetter> PlacedNpc(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedNpc, IPlacedNpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPlacedNpc, IPlacedNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedObject</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedObject, IPlacedObjectGetter> PlacedObject(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedObject, IPlacedObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPlacedObject, IPlacedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedObject</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedObject, IPlacedObjectGetter> PlacedObject(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlacedObject, IPlacedObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPlacedObject, IPlacedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Projectile
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Projectile</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IProjectile, IProjectileGetter> Projectile(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IProjectile, IProjectileGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IProjectileGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IProjectile, IProjectileGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Projectile
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Projectile</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IProjectile, IProjectileGetter> Projectile(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IProjectile, IProjectileGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IProjectileGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IProjectile, IProjectileGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Quest
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Quest</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IQuest, IQuestGetter> Quest(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IQuest, IQuestGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IQuest, IQuestGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Quest
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Quest</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IQuest, IQuestGetter> Quest(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IQuest, IQuestGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IQuest, IQuestGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRace, IRaceGetter> Race(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IRace, IRaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRace, IRaceGetter> Race(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IRace, IRaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to RadiationStage
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on RadiationStage</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRadiationStage, IRadiationStageGetter> RadiationStage(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRadiationStage, IRadiationStageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRadiationStageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IRadiationStage, IRadiationStageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to RadiationStage
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on RadiationStage</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRadiationStage, IRadiationStageGetter> RadiationStage(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRadiationStage, IRadiationStageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRadiationStageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IRadiationStage, IRadiationStageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ragdoll
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ragdoll</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRagdoll, IRagdollGetter> Ragdoll(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRagdoll, IRagdollGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRagdollGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IRagdoll, IRagdollGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ragdoll
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ragdoll</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRagdoll, IRagdollGetter> Ragdoll(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRagdoll, IRagdollGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRagdollGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IRagdoll, IRagdollGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Recipe
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Recipe</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRecipe, IRecipeGetter> Recipe(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRecipe, IRecipeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRecipeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IRecipe, IRecipeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Recipe
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Recipe</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRecipe, IRecipeGetter> Recipe(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRecipe, IRecipeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRecipeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IRecipe, IRecipeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to RecipeCategory
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on RecipeCategory</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRecipeCategory, IRecipeCategoryGetter> RecipeCategory(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRecipeCategory, IRecipeCategoryGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRecipeCategoryGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IRecipeCategory, IRecipeCategoryGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to RecipeCategory
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on RecipeCategory</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRecipeCategory, IRecipeCategoryGetter> RecipeCategory(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRecipeCategory, IRecipeCategoryGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRecipeCategoryGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IRecipeCategory, IRecipeCategoryGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Region
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Region</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRegion, IRegionGetter> Region(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRegion, IRegionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IRegion, IRegionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Region
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Region</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRegion, IRegionGetter> Region(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRegion, IRegionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IRegion, IRegionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Reputation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Reputation</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IReputation, IReputationGetter> Reputation(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IReputation, IReputationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IReputationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IReputation, IReputationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Reputation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Reputation</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IReputation, IReputationGetter> Reputation(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IReputation, IReputationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IReputationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IReputation, IReputationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Script
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Script</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IScript, IScriptGetter> Script(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IScript, IScriptGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IScriptGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IScript, IScriptGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Script
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Script</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IScript, IScriptGetter> Script(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IScript, IScriptGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IScriptGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IScript, IScriptGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SleepDeprivationStage
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SleepDeprivationStage</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ISleepDeprivationStage, ISleepDeprivationStageGetter> SleepDeprivationStage(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ISleepDeprivationStage, ISleepDeprivationStageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISleepDeprivationStageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ISleepDeprivationStage, ISleepDeprivationStageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SleepDeprivationStage
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SleepDeprivationStage</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ISleepDeprivationStage, ISleepDeprivationStageGetter> SleepDeprivationStage(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ISleepDeprivationStage, ISleepDeprivationStageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISleepDeprivationStageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ISleepDeprivationStage, ISleepDeprivationStageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Sound
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Sound</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ISound, ISoundGetter> Sound(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ISound, ISoundGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ISound, ISoundGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Sound
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Sound</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ISound, ISoundGetter> Sound(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ISound, ISoundGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ISound, ISoundGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Spell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Spell</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ISpell, ISpellGetter> Spell(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ISpell, ISpellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ISpell, ISpellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Spell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Spell</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ISpell, ISpellGetter> Spell(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ISpell, ISpellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ISpell, ISpellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Static
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Static</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IStatic, IStaticGetter> Static(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IStatic, IStaticGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IStatic, IStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Static
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Static</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IStatic, IStaticGetter> Static(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IStatic, IStaticGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IStatic, IStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StaticCollection
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on StaticCollection</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IStaticCollection, IStaticCollectionGetter> StaticCollection(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IStaticCollection, IStaticCollectionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStaticCollectionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IStaticCollection, IStaticCollectionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StaticCollection
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on StaticCollection</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IStaticCollection, IStaticCollectionGetter> StaticCollection(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IStaticCollection, IStaticCollectionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStaticCollectionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IStaticCollection, IStaticCollectionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TalkingActivator
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on TalkingActivator</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ITalkingActivator, ITalkingActivatorGetter> TalkingActivator(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ITalkingActivator, ITalkingActivatorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITalkingActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ITalkingActivator, ITalkingActivatorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TalkingActivator
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on TalkingActivator</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ITalkingActivator, ITalkingActivatorGetter> TalkingActivator(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ITalkingActivator, ITalkingActivatorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITalkingActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ITalkingActivator, ITalkingActivatorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Terminal
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Terminal</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ITerminal, ITerminalGetter> Terminal(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ITerminal, ITerminalGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITerminalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ITerminal, ITerminalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Terminal
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Terminal</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ITerminal, ITerminalGetter> Terminal(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ITerminal, ITerminalGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITerminalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ITerminal, ITerminalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TextureSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on TextureSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ITextureSet, ITextureSetGetter> TextureSet(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ITextureSet, ITextureSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ITextureSet, ITextureSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TextureSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on TextureSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ITextureSet, ITextureSetGetter> TextureSet(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ITextureSet, ITextureSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ITextureSet, ITextureSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Tree
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Tree</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ITree, ITreeGetter> Tree(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ITree, ITreeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ITree, ITreeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Tree
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Tree</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ITree, ITreeGetter> Tree(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ITree, ITreeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ITree, ITreeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VoiceType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on VoiceType</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IVoiceType, IVoiceTypeGetter> VoiceType(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IVoiceType, IVoiceTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IVoiceType, IVoiceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VoiceType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on VoiceType</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IVoiceType, IVoiceTypeGetter> VoiceType(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IVoiceType, IVoiceTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IVoiceType, IVoiceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Water
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Water</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IWater, IWaterGetter> Water(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IWater, IWaterGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IWater, IWaterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Water
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Water</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IWater, IWaterGetter> Water(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IWater, IWaterGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IWater, IWaterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weapon
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Weapon</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IWeapon, IWeaponGetter> Weapon(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IWeapon, IWeaponGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IWeapon, IWeaponGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weapon
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Weapon</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IWeapon, IWeaponGetter> Weapon(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IWeapon, IWeaponGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IWeapon, IWeaponGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weather
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Weather</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IWeather, IWeatherGetter> Weather(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IWeather, IWeatherGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IWeather, IWeatherGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weather
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Weather</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IWeather, IWeatherGetter> Weather(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IWeather, IWeatherGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IWeather, IWeatherGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Worldspace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Worldspace</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IWorldspace, IWorldspaceGetter> Worldspace(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IWorldspace, IWorldspaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IWorldspace, IWorldspaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Worldspace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Worldspace</returns>
        public static TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IWorldspace, IWorldspaceGetter> Worldspace(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IWorldspace, IWorldspaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IWorldspace, IWorldspaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

        #region Link Interfaces
        /// <summary>
        /// Scope a load order query to IPlaceableObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlaceableObject</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlaceableObject, IPlaceableObjectGetter> IPlaceableObject(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlaceableObject, IPlaceableObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlaceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPlaceableObject, IPlaceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaceableObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlaceableObject</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlaceableObject, IPlaceableObjectGetter> IPlaceableObject(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlaceableObject, IPlaceableObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlaceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPlaceableObject, IPlaceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IReferenceableObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IReferenceableObject</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IReferenceableObject, IReferenceableObjectGetter> IReferenceableObject(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IReferenceableObject, IReferenceableObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IReferenceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IReferenceableObject, IReferenceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IReferenceableObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IReferenceableObject</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IReferenceableObject, IReferenceableObjectGetter> IReferenceableObject(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IReferenceableObject, IReferenceableObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IReferenceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IReferenceableObject, IReferenceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IExplodeSpawn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IExplodeSpawn</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IExplodeSpawn, IExplodeSpawnGetter> IExplodeSpawn(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IExplodeSpawn, IExplodeSpawnGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IExplodeSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IExplodeSpawn, IExplodeSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IExplodeSpawn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IExplodeSpawn</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IExplodeSpawn, IExplodeSpawnGetter> IExplodeSpawn(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IExplodeSpawn, IExplodeSpawnGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IExplodeSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IExplodeSpawn, IExplodeSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IItem</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IItem, IItemGetter> IItem(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IItem, IItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IItem, IItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IItem</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IItem, IItemGetter> IItem(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IItem, IItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IItem, IItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IAmmoOrList
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IAmmoOrList</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAmmoOrList, IAmmoOrListGetter> IAmmoOrList(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAmmoOrList, IAmmoOrListGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAmmoOrListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IAmmoOrList, IAmmoOrListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IAmmoOrList
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IAmmoOrList</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAmmoOrList, IAmmoOrListGetter> IAmmoOrList(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IAmmoOrList, IAmmoOrListGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAmmoOrListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IAmmoOrList, IAmmoOrListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IBoundItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IBoundItem</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IBoundItem, IBoundItemGetter> IBoundItem(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IBoundItem, IBoundItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBoundItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IBoundItem, IBoundItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IBoundItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IBoundItem</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IBoundItem, IBoundItemGetter> IBoundItem(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IBoundItem, IBoundItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBoundItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IBoundItem, IBoundItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ICellOrWorldspace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ICellOrWorldspace</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICellOrWorldspace, ICellOrWorldspaceGetter> ICellOrWorldspace(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICellOrWorldspace, ICellOrWorldspaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICellOrWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICellOrWorldspace, ICellOrWorldspaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ICellOrWorldspace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ICellOrWorldspace</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICellOrWorldspace, ICellOrWorldspaceGetter> ICellOrWorldspace(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICellOrWorldspace, ICellOrWorldspaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICellOrWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICellOrWorldspace, ICellOrWorldspaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ICreatureSpawn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ICreatureSpawn</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICreatureSpawn, ICreatureSpawnGetter> ICreatureSpawn(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICreatureSpawn, ICreatureSpawnGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICreatureSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICreatureSpawn, ICreatureSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ICreatureSpawn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ICreatureSpawn</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICreatureSpawn, ICreatureSpawnGetter> ICreatureSpawn(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ICreatureSpawn, ICreatureSpawnGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICreatureSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ICreatureSpawn, ICreatureSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to INpcOrCreatureSpawn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on INpcOrCreatureSpawn</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INpcOrCreatureSpawn, INpcOrCreatureSpawnGetter> INpcOrCreatureSpawn(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INpcOrCreatureSpawn, INpcOrCreatureSpawnGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcOrCreatureSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, INpcOrCreatureSpawn, INpcOrCreatureSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to INpcOrCreatureSpawn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on INpcOrCreatureSpawn</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INpcOrCreatureSpawn, INpcOrCreatureSpawnGetter> INpcOrCreatureSpawn(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INpcOrCreatureSpawn, INpcOrCreatureSpawnGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INpcOrCreatureSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, INpcOrCreatureSpawn, INpcOrCreatureSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRelatable
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IRelatable</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRelatable, IRelatableGetter> IRelatable(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRelatable, IRelatableGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRelatableGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IRelatable, IRelatableGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRelatable
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IRelatable</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRelatable, IRelatableGetter> IRelatable(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IRelatable, IRelatableGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRelatableGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IRelatable, IRelatableGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IOwner
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IOwner</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IOwner, IOwnerGetter> IOwner(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IOwner, IOwnerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IOwner, IOwnerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IOwner
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IOwner</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IOwner, IOwnerGetter> IOwner(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IOwner, IOwnerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IOwner, IOwnerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IIdleRelation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IIdleRelation</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IIdleRelation, IIdleRelationGetter> IIdleRelation(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IIdleRelation, IIdleRelationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IIdleRelation, IIdleRelationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IIdleRelation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IIdleRelation</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IIdleRelation, IIdleRelationGetter> IIdleRelation(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IIdleRelation, IIdleRelationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIdleRelationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IIdleRelation, IIdleRelationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to INpcSpawn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on INpcSpawn</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INpcSpawn, INpcSpawnGetter> INpcSpawn(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INpcSpawn, INpcSpawnGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, INpcSpawn, INpcSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to INpcSpawn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on INpcSpawn</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INpcSpawn, INpcSpawnGetter> INpcSpawn(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, INpcSpawn, INpcSpawnGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, INpcSpawn, INpcSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IEmittance
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IEmittance</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IEmittance, IEmittanceGetter> IEmittance(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IEmittance, IEmittanceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEmittanceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IEmittance, IEmittanceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IEmittance
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IEmittance</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IEmittance, IEmittanceGetter> IEmittance(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IEmittance, IEmittanceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEmittanceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IEmittance, IEmittanceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ISoundOrNpcSpawn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ISoundOrNpcSpawn</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ISoundOrNpcSpawn, ISoundOrNpcSpawnGetter> ISoundOrNpcSpawn(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ISoundOrNpcSpawn, ISoundOrNpcSpawnGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundOrNpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ISoundOrNpcSpawn, ISoundOrNpcSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ISoundOrNpcSpawn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ISoundOrNpcSpawn</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ISoundOrNpcSpawn, ISoundOrNpcSpawnGetter> ISoundOrNpcSpawn(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, ISoundOrNpcSpawn, ISoundOrNpcSpawnGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundOrNpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, ISoundOrNpcSpawn, ISoundOrNpcSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IEffectRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IEffectRecord</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IEffectRecord, IEffectRecordGetter> IEffectRecord(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IEffectRecord, IEffectRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEffectRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IEffectRecord, IEffectRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IEffectRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IEffectRecord</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IEffectRecord, IEffectRecordGetter> IEffectRecord(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IEffectRecord, IEffectRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEffectRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IEffectRecord, IEffectRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaced
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlaced</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlaced, IPlacedGetter> IPlaced(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlaced, IPlacedGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPlaced, IPlacedGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaced
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlaced</returns>
        public static TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlaced, IPlacedGetter> IPlaced(this IEnumerable<IFalloutNVModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFalloutNVMod, IFalloutNVModGetter, IPlaced, IPlacedGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFalloutNVMod, IFalloutNVModGetter, IPlaced, IPlacedGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

    }
}
