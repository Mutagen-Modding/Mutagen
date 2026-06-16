using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.Fallout3
{
    public static class TypeOptionSolidifierMixIns
    {
        #region Normal
        /// <summary>
        /// Scope a load order query to AcousticSpace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AcousticSpace</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAcousticSpace, IAcousticSpaceGetter> AcousticSpace(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAcousticSpace, IAcousticSpaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IAcousticSpace, IAcousticSpaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AcousticSpace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AcousticSpace</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAcousticSpace, IAcousticSpaceGetter> AcousticSpace(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAcousticSpace, IAcousticSpaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAcousticSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IAcousticSpace, IAcousticSpaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Activator
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Activator</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IActivator, IActivatorGetter> Activator(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IActivator, IActivatorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IActivator, IActivatorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Activator
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Activator</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IActivator, IActivatorGetter> Activator(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IActivator, IActivatorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IActivator, IActivatorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActorValueInformation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ActorValueInformation</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IActorValueInformation, IActorValueInformationGetter> ActorValueInformation(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IActorValueInformation, IActorValueInformationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IActorValueInformation, IActorValueInformationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ActorValueInformation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ActorValueInformation</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IActorValueInformation, IActorValueInformationGetter> ActorValueInformation(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IActorValueInformation, IActorValueInformationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActorValueInformationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IActorValueInformation, IActorValueInformationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AddonNode
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AddonNode</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAddonNode, IAddonNodeGetter> AddonNode(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAddonNode, IAddonNodeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IAddonNode, IAddonNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AddonNode
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AddonNode</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAddonNode, IAddonNodeGetter> AddonNode(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAddonNode, IAddonNodeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAddonNodeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IAddonNode, IAddonNodeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AmmoEffect
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AmmoEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAmmoEffect, IAmmoEffectGetter> AmmoEffect(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAmmoEffect, IAmmoEffectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAmmoEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IAmmoEffect, IAmmoEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AmmoEffect
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AmmoEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAmmoEffect, IAmmoEffectGetter> AmmoEffect(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAmmoEffect, IAmmoEffectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAmmoEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IAmmoEffect, IAmmoEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ammunition
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ammunition</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAmmunition, IAmmunitionGetter> Ammunition(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAmmunition, IAmmunitionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IAmmunition, IAmmunitionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ammunition
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ammunition</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAmmunition, IAmmunitionGetter> Ammunition(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAmmunition, IAmmunitionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IAmmunition, IAmmunitionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AnimatedObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on AnimatedObject</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAnimatedObject, IAnimatedObjectGetter> AnimatedObject(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAnimatedObject, IAnimatedObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IAnimatedObject, IAnimatedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to AnimatedObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on AnimatedObject</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAnimatedObject, IAnimatedObjectGetter> AnimatedObject(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAnimatedObject, IAnimatedObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IAnimatedObject, IAnimatedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Armor
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Armor</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IArmor, IArmorGetter> Armor(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IArmor, IArmorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IArmor, IArmorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Armor
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Armor</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IArmor, IArmorGetter> Armor(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IArmor, IArmorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IArmor, IArmorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ArmorAddon
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ArmorAddon</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IArmorAddon, IArmorAddonGetter> ArmorAddon(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IArmorAddon, IArmorAddonGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IArmorAddon, IArmorAddonGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ArmorAddon
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ArmorAddon</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IArmorAddon, IArmorAddonGetter> ArmorAddon(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IArmorAddon, IArmorAddonGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IArmorAddonGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IArmorAddon, IArmorAddonGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BodyPartData
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on BodyPartData</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IBodyPartData, IBodyPartDataGetter> BodyPartData(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IBodyPartData, IBodyPartDataGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IBodyPartData, IBodyPartDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to BodyPartData
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on BodyPartData</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IBodyPartData, IBodyPartDataGetter> BodyPartData(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IBodyPartData, IBodyPartDataGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBodyPartDataGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IBodyPartData, IBodyPartDataGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Book
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Book</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IBook, IBookGetter> Book(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IBook, IBookGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IBook, IBookGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Book
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Book</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IBook, IBookGetter> Book(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IBook, IBookGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IBook, IBookGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraPath
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CameraPath</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICameraPath, ICameraPathGetter> CameraPath(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICameraPath, ICameraPathGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICameraPath, ICameraPathGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraPath
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CameraPath</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICameraPath, ICameraPathGetter> CameraPath(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICameraPath, ICameraPathGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICameraPathGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICameraPath, ICameraPathGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraShot
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CameraShot</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICameraShot, ICameraShotGetter> CameraShot(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICameraShot, ICameraShotGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICameraShot, ICameraShotGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CameraShot
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CameraShot</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICameraShot, ICameraShotGetter> CameraShot(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICameraShot, ICameraShotGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICameraShotGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICameraShot, ICameraShotGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CaravanCard
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CaravanCard</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICaravanCard, ICaravanCardGetter> CaravanCard(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICaravanCard, ICaravanCardGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICaravanCardGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICaravanCard, ICaravanCardGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CaravanCard
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CaravanCard</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICaravanCard, ICaravanCardGetter> CaravanCard(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICaravanCard, ICaravanCardGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICaravanCardGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICaravanCard, ICaravanCardGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CaravanDeck
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CaravanDeck</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICaravanDeck, ICaravanDeckGetter> CaravanDeck(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICaravanDeck, ICaravanDeckGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICaravanDeckGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICaravanDeck, ICaravanDeckGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CaravanDeck
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CaravanDeck</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICaravanDeck, ICaravanDeckGetter> CaravanDeck(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICaravanDeck, ICaravanDeckGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICaravanDeckGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICaravanDeck, ICaravanDeckGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CaravanMoney
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CaravanMoney</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICaravanMoney, ICaravanMoneyGetter> CaravanMoney(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICaravanMoney, ICaravanMoneyGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICaravanMoneyGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICaravanMoney, ICaravanMoneyGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CaravanMoney
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CaravanMoney</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICaravanMoney, ICaravanMoneyGetter> CaravanMoney(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICaravanMoney, ICaravanMoneyGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICaravanMoneyGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICaravanMoney, ICaravanMoneyGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Casino
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Casino</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICasino, ICasinoGetter> Casino(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICasino, ICasinoGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICasinoGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICasino, ICasinoGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Casino
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Casino</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICasino, ICasinoGetter> Casino(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICasino, ICasinoGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICasinoGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICasino, ICasinoGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CasinoChip
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CasinoChip</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICasinoChip, ICasinoChipGetter> CasinoChip(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICasinoChip, ICasinoChipGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICasinoChipGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICasinoChip, ICasinoChipGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CasinoChip
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CasinoChip</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICasinoChip, ICasinoChipGetter> CasinoChip(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICasinoChip, ICasinoChipGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICasinoChipGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICasinoChip, ICasinoChipGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Cell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Cell</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICell, ICellGetter> Cell(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICell, ICellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICell, ICellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Cell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Cell</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICell, ICellGetter> Cell(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICell, ICellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICell, ICellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Challenge
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Challenge</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IChallenge, IChallengeGetter> Challenge(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IChallenge, IChallengeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IChallengeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IChallenge, IChallengeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Challenge
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Challenge</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IChallenge, IChallengeGetter> Challenge(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IChallenge, IChallengeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IChallengeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IChallenge, IChallengeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Class
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Class</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IClass, IClassGetter> Class(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IClass, IClassGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IClass, IClassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Class
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Class</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IClass, IClassGetter> Class(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IClass, IClassGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IClass, IClassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Climate
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Climate</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IClimate, IClimateGetter> Climate(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IClimate, IClimateGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IClimate, IClimateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Climate
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Climate</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IClimate, IClimateGetter> Climate(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IClimate, IClimateGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IClimate, IClimateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CombatStyle
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on CombatStyle</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICombatStyle, ICombatStyleGetter> CombatStyle(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICombatStyle, ICombatStyleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICombatStyle, ICombatStyleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to CombatStyle
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on CombatStyle</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICombatStyle, ICombatStyleGetter> CombatStyle(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICombatStyle, ICombatStyleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICombatStyle, ICombatStyleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ConstructibleObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ConstructibleObject</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IConstructibleObject, IConstructibleObjectGetter> ConstructibleObject(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IConstructibleObject, IConstructibleObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IConstructibleObject, IConstructibleObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ConstructibleObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ConstructibleObject</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IConstructibleObject, IConstructibleObjectGetter> ConstructibleObject(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IConstructibleObject, IConstructibleObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IConstructibleObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IConstructibleObject, IConstructibleObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Container
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Container</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IContainer, IContainerGetter> Container(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IContainer, IContainerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IContainer, IContainerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Container
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Container</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IContainer, IContainerGetter> Container(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IContainer, IContainerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IContainer, IContainerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Creature
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Creature</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICreature, ICreatureGetter> Creature(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICreature, ICreatureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICreature, ICreatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Creature
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Creature</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICreature, ICreatureGetter> Creature(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICreature, ICreatureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICreature, ICreatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Debris
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Debris</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDebris, IDebrisGetter> Debris(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDebris, IDebrisGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDebrisGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IDebris, IDebrisGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Debris
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Debris</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDebris, IDebrisGetter> Debris(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDebris, IDebrisGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDebrisGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IDebris, IDebrisGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DefaultObjectManager
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DefaultObjectManager</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter> DefaultObjectManager(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DefaultObjectManager
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DefaultObjectManager</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter> DefaultObjectManager(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDefaultObjectManagerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IDefaultObjectManager, IDefaultObjectManagerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DehydrationStage
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DehydrationStage</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDehydrationStage, IDehydrationStageGetter> DehydrationStage(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDehydrationStage, IDehydrationStageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDehydrationStageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IDehydrationStage, IDehydrationStageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DehydrationStage
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DehydrationStage</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDehydrationStage, IDehydrationStageGetter> DehydrationStage(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDehydrationStage, IDehydrationStageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDehydrationStageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IDehydrationStage, IDehydrationStageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogResponses
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogResponses</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDialogResponses, IDialogResponsesGetter> DialogResponses(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDialogResponses, IDialogResponsesGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IDialogResponses, IDialogResponsesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogResponses
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogResponses</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDialogResponses, IDialogResponsesGetter> DialogResponses(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDialogResponses, IDialogResponsesGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogResponsesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IDialogResponses, IDialogResponsesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogTopic
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on DialogTopic</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDialogTopic, IDialogTopicGetter> DialogTopic(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDialogTopic, IDialogTopicGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IDialogTopic, IDialogTopicGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to DialogTopic
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on DialogTopic</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDialogTopic, IDialogTopicGetter> DialogTopic(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDialogTopic, IDialogTopicGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IDialogTopic, IDialogTopicGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Door
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Door</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDoor, IDoorGetter> Door(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDoor, IDoorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IDoor, IDoorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Door
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Door</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDoor, IDoorGetter> Door(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IDoor, IDoorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IDoor, IDoorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EffectShader
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EffectShader</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEffectShader, IEffectShaderGetter> EffectShader(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEffectShader, IEffectShaderGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IEffectShader, IEffectShaderGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EffectShader
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EffectShader</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEffectShader, IEffectShaderGetter> EffectShader(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEffectShader, IEffectShaderGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IEffectShader, IEffectShaderGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EncounterZone
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on EncounterZone</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEncounterZone, IEncounterZoneGetter> EncounterZone(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEncounterZone, IEncounterZoneGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEncounterZoneGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IEncounterZone, IEncounterZoneGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to EncounterZone
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on EncounterZone</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEncounterZone, IEncounterZoneGetter> EncounterZone(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEncounterZone, IEncounterZoneGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEncounterZoneGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IEncounterZone, IEncounterZoneGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Explosion
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Explosion</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IExplosion, IExplosionGetter> Explosion(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IExplosion, IExplosionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IExplosionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IExplosion, IExplosionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Explosion
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Explosion</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IExplosion, IExplosionGetter> Explosion(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IExplosion, IExplosionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IExplosionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IExplosion, IExplosionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Eye
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Eye</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEye, IEyeGetter> Eye(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEye, IEyeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEyeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IEye, IEyeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Eye
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Eye</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEye, IEyeGetter> Eye(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEye, IEyeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEyeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IEye, IEyeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Eyes
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Eyes</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEyes, IEyesGetter> Eyes(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEyes, IEyesGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEyesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IEyes, IEyesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Eyes
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Eyes</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEyes, IEyesGetter> Eyes(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEyes, IEyesGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEyesGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IEyes, IEyesGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Faction
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Faction</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFaction, IFactionGetter> Faction(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFaction, IFactionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IFaction, IFactionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Faction
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Faction</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFaction, IFactionGetter> Faction(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFaction, IFactionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IFaction, IFactionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Fallout3MajorRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Fallout3MajorRecord</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFallout3MajorRecord, IFallout3MajorRecordGetter> Fallout3MajorRecord(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFallout3MajorRecord, IFallout3MajorRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFallout3MajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IFallout3MajorRecord, IFallout3MajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Fallout3MajorRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Fallout3MajorRecord</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFallout3MajorRecord, IFallout3MajorRecordGetter> Fallout3MajorRecord(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFallout3MajorRecord, IFallout3MajorRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFallout3MajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IFallout3MajorRecord, IFallout3MajorRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FormList
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on FormList</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFormList, IFormListGetter> FormList(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFormList, IFormListGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFormListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IFormList, IFormListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to FormList
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on FormList</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFormList, IFormListGetter> FormList(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFormList, IFormListGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFormListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IFormList, IFormListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Furniture
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Furniture</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFurniture, IFurnitureGetter> Furniture(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFurniture, IFurnitureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IFurniture, IFurnitureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Furniture
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Furniture</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFurniture, IFurnitureGetter> Furniture(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IFurniture, IFurnitureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IFurniture, IFurnitureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to GameSetting
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on GameSetting</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IGameSetting, IGameSettingGetter> GameSetting(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IGameSetting, IGameSettingGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IGameSetting, IGameSettingGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Global
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Global</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IGlobal, IGlobalGetter> Global(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IGlobal, IGlobalGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IGlobal, IGlobalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Global
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Global</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IGlobal, IGlobalGetter> Global(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IGlobal, IGlobalGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IGlobal, IGlobalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Grass
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Grass</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IGrass, IGrassGetter> Grass(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IGrass, IGrassGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IGrass, IGrassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Grass
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Grass</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IGrass, IGrassGetter> Grass(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IGrass, IGrassGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IGrass, IGrassGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hair
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Hair</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IHair, IHairGetter> Hair(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IHair, IHairGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IHair, IHairGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Hair
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Hair</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IHair, IHairGetter> Hair(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IHair, IHairGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IHair, IHairGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to HeadPart
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on HeadPart</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IHeadPart, IHeadPartGetter> HeadPart(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IHeadPart, IHeadPartGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IHeadPart, IHeadPartGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to HeadPart
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on HeadPart</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IHeadPart, IHeadPartGetter> HeadPart(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IHeadPart, IHeadPartGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHeadPartGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IHeadPart, IHeadPartGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to HungerStage
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on HungerStage</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IHungerStage, IHungerStageGetter> HungerStage(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IHungerStage, IHungerStageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IHungerStageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IHungerStage, IHungerStageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to HungerStage
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on HungerStage</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IHungerStage, IHungerStageGetter> HungerStage(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IHungerStage, IHungerStageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IHungerStageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IHungerStage, IHungerStageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleAnimation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IdleAnimation</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IIdleAnimation, IIdleAnimationGetter> IdleAnimation(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IIdleAnimation, IIdleAnimationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IIdleAnimation, IIdleAnimationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleAnimation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IdleAnimation</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IIdleAnimation, IIdleAnimationGetter> IdleAnimation(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IIdleAnimation, IIdleAnimationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IIdleAnimation, IIdleAnimationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleMarker
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IdleMarker</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IIdleMarker, IIdleMarkerGetter> IdleMarker(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IIdleMarker, IIdleMarkerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IIdleMarker, IIdleMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IdleMarker
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IdleMarker</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IIdleMarker, IIdleMarkerGetter> IdleMarker(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IIdleMarker, IIdleMarkerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIdleMarkerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IIdleMarker, IIdleMarkerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ImageSpace</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IImageSpace, IImageSpaceGetter> ImageSpace(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IImageSpace, IImageSpaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IImageSpace, IImageSpaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ImageSpace</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IImageSpace, IImageSpaceGetter> ImageSpace(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IImageSpace, IImageSpaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImageSpaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IImageSpace, IImageSpaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpaceAdapter
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ImageSpaceAdapter</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter> ImageSpaceAdapter(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImageSpaceAdapter
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ImageSpaceAdapter</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter> ImageSpaceAdapter(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImageSpaceAdapterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IImageSpaceAdapter, IImageSpaceAdapterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Impact
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Impact</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IImpact, IImpactGetter> Impact(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IImpact, IImpactGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImpactGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IImpact, IImpactGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Impact
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Impact</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IImpact, IImpactGetter> Impact(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IImpact, IImpactGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImpactGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IImpact, IImpactGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImpactDataSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ImpactDataSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IImpactDataSet, IImpactDataSetGetter> ImpactDataSet(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IImpactDataSet, IImpactDataSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IImpactDataSet, IImpactDataSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ImpactDataSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ImpactDataSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IImpactDataSet, IImpactDataSetGetter> ImpactDataSet(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IImpactDataSet, IImpactDataSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IImpactDataSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IImpactDataSet, IImpactDataSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingestible
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ingestible</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IIngestible, IIngestibleGetter> Ingestible(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IIngestible, IIngestibleGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IIngestible, IIngestibleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingestible
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ingestible</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IIngestible, IIngestibleGetter> Ingestible(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IIngestible, IIngestibleGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIngestibleGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IIngestible, IIngestibleGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingredient
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ingredient</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IIngredient, IIngredientGetter> Ingredient(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IIngredient, IIngredientGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IIngredient, IIngredientGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ingredient
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ingredient</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IIngredient, IIngredientGetter> Ingredient(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IIngredient, IIngredientGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IIngredient, IIngredientGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ItemMod
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ItemMod</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IItemMod, IItemModGetter> ItemMod(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IItemMod, IItemModGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IItemModGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IItemMod, IItemModGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ItemMod
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ItemMod</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IItemMod, IItemModGetter> ItemMod(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IItemMod, IItemModGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IItemModGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IItemMod, IItemModGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Key
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Key</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IKey, IKeyGetter> Key(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IKey, IKeyGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IKey, IKeyGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Key
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Key</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IKey, IKeyGetter> Key(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IKey, IKeyGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IKey, IKeyGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Landscape
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Landscape</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILandscape, ILandscapeGetter> Landscape(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILandscape, ILandscapeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ILandscape, ILandscapeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Landscape
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Landscape</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILandscape, ILandscapeGetter> Landscape(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILandscape, ILandscapeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ILandscape, ILandscapeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LandscapeTexture
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LandscapeTexture</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILandscapeTexture, ILandscapeTextureGetter> LandscapeTexture(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILandscapeTexture, ILandscapeTextureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ILandscapeTexture, ILandscapeTextureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LandscapeTexture
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LandscapeTexture</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILandscapeTexture, ILandscapeTextureGetter> LandscapeTexture(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILandscapeTexture, ILandscapeTextureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILandscapeTextureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ILandscapeTexture, ILandscapeTextureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledCreature
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledCreature</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILeveledCreature, ILeveledCreatureGetter> LeveledCreature(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILeveledCreature, ILeveledCreatureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledCreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ILeveledCreature, ILeveledCreatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledCreature
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledCreature</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILeveledCreature, ILeveledCreatureGetter> LeveledCreature(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILeveledCreature, ILeveledCreatureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledCreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ILeveledCreature, ILeveledCreatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledItem</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILeveledItem, ILeveledItemGetter> LeveledItem(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILeveledItem, ILeveledItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ILeveledItem, ILeveledItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledItem</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILeveledItem, ILeveledItemGetter> LeveledItem(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILeveledItem, ILeveledItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ILeveledItem, ILeveledItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledNpc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LeveledNpc</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILeveledNpc, ILeveledNpcGetter> LeveledNpc(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILeveledNpc, ILeveledNpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ILeveledNpc, ILeveledNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LeveledNpc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LeveledNpc</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILeveledNpc, ILeveledNpcGetter> LeveledNpc(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILeveledNpc, ILeveledNpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILeveledNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ILeveledNpc, ILeveledNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Light
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Light</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILight, ILightGetter> Light(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILight, ILightGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ILight, ILightGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Light
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Light</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILight, ILightGetter> Light(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILight, ILightGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ILight, ILightGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LightingTemplate
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LightingTemplate</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILightingTemplate, ILightingTemplateGetter> LightingTemplate(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILightingTemplate, ILightingTemplateGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ILightingTemplate, ILightingTemplateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LightingTemplate
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LightingTemplate</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILightingTemplate, ILightingTemplateGetter> LightingTemplate(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILightingTemplate, ILightingTemplateGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILightingTemplateGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ILightingTemplate, ILightingTemplateGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LoadScreen
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LoadScreen</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILoadScreen, ILoadScreenGetter> LoadScreen(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILoadScreen, ILoadScreenGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ILoadScreen, ILoadScreenGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LoadScreen
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LoadScreen</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILoadScreen, ILoadScreenGetter> LoadScreen(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILoadScreen, ILoadScreenGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ILoadScreen, ILoadScreenGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LoadScreenType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on LoadScreenType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILoadScreenType, ILoadScreenTypeGetter> LoadScreenType(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILoadScreenType, ILoadScreenTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ILoadScreenTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ILoadScreenType, ILoadScreenTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to LoadScreenType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on LoadScreenType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILoadScreenType, ILoadScreenTypeGetter> LoadScreenType(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ILoadScreenType, ILoadScreenTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ILoadScreenTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ILoadScreenType, ILoadScreenTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MagicEffect
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MagicEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMagicEffect, IMagicEffectGetter> MagicEffect(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMagicEffect, IMagicEffectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IMagicEffect, IMagicEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MagicEffect
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MagicEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMagicEffect, IMagicEffectGetter> MagicEffect(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMagicEffect, IMagicEffectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IMagicEffect, IMagicEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MediaLocationController
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MediaLocationController</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMediaLocationController, IMediaLocationControllerGetter> MediaLocationController(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMediaLocationController, IMediaLocationControllerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMediaLocationControllerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IMediaLocationController, IMediaLocationControllerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MediaLocationController
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MediaLocationController</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMediaLocationController, IMediaLocationControllerGetter> MediaLocationController(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMediaLocationController, IMediaLocationControllerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMediaLocationControllerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IMediaLocationController, IMediaLocationControllerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MediaSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MediaSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMediaSet, IMediaSetGetter> MediaSet(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMediaSet, IMediaSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMediaSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IMediaSet, IMediaSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MediaSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MediaSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMediaSet, IMediaSetGetter> MediaSet(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMediaSet, IMediaSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMediaSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IMediaSet, IMediaSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MenuIcon
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MenuIcon</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMenuIcon, IMenuIconGetter> MenuIcon(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMenuIcon, IMenuIconGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMenuIconGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IMenuIcon, IMenuIconGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MenuIcon
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MenuIcon</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMenuIcon, IMenuIconGetter> MenuIcon(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMenuIcon, IMenuIconGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMenuIconGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IMenuIcon, IMenuIconGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Message
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Message</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMessage, IMessageGetter> Message(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMessage, IMessageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMessageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IMessage, IMessageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Message
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Message</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMessage, IMessageGetter> Message(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMessage, IMessageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMessageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IMessage, IMessageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MiscItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MiscItem</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMiscItem, IMiscItemGetter> MiscItem(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMiscItem, IMiscItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IMiscItem, IMiscItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MiscItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MiscItem</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMiscItem, IMiscItemGetter> MiscItem(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMiscItem, IMiscItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMiscItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IMiscItem, IMiscItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MoveableStatic
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MoveableStatic</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMoveableStatic, IMoveableStaticGetter> MoveableStatic(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMoveableStatic, IMoveableStaticGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMoveableStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IMoveableStatic, IMoveableStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MoveableStatic
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MoveableStatic</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMoveableStatic, IMoveableStaticGetter> MoveableStatic(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMoveableStatic, IMoveableStaticGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMoveableStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IMoveableStatic, IMoveableStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MusicType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on MusicType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMusicType, IMusicTypeGetter> MusicType(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMusicType, IMusicTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IMusicType, IMusicTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to MusicType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on MusicType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMusicType, IMusicTypeGetter> MusicType(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IMusicType, IMusicTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IMusicTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IMusicType, IMusicTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMesh
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on NavigationMesh</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INavigationMesh, INavigationMeshGetter> NavigationMesh(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INavigationMesh, INavigationMeshGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, INavigationMesh, INavigationMeshGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMesh
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on NavigationMesh</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INavigationMesh, INavigationMeshGetter> NavigationMesh(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INavigationMesh, INavigationMeshGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INavigationMeshGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, INavigationMesh, INavigationMeshGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMeshInfoMap
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on NavigationMeshInfoMap</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter> NavigationMeshInfoMap(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to NavigationMeshInfoMap
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on NavigationMeshInfoMap</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter> NavigationMeshInfoMap(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INavigationMeshInfoMapGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Note
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Note</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INote, INoteGetter> Note(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INote, INoteGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INoteGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, INote, INoteGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Note
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Note</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INote, INoteGetter> Note(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INote, INoteGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INoteGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, INote, INoteGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INpc, INpcGetter> Npc(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INpc, INpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, INpc, INpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Npc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Npc</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INpc, INpcGetter> Npc(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INpc, INpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, INpc, INpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ObjectEffect
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ObjectEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IObjectEffect, IObjectEffectGetter> ObjectEffect(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IObjectEffect, IObjectEffectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IObjectEffect, IObjectEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ObjectEffect
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ObjectEffect</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IObjectEffect, IObjectEffectGetter> ObjectEffect(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IObjectEffect, IObjectEffectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IObjectEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IObjectEffect, IObjectEffectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Package
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Package</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPackage, IPackageGetter> Package(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPackage, IPackageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPackageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPackage, IPackageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Package
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Package</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPackage, IPackageGetter> Package(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPackage, IPackageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPackageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPackage, IPackageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Perk
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Perk</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPerk, IPerkGetter> Perk(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPerk, IPerkGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPerkGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPerk, IPerkGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Perk
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Perk</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPerk, IPerkGetter> Perk(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPerk, IPerkGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPerkGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPerk, IPerkGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlaceableWater
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlaceableWater</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlaceableWater, IPlaceableWaterGetter> PlaceableWater(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlaceableWater, IPlaceableWaterGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlaceableWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlaceableWater, IPlaceableWaterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlaceableWater
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlaceableWater</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlaceableWater, IPlaceableWaterGetter> PlaceableWater(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlaceableWater, IPlaceableWaterGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlaceableWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlaceableWater, IPlaceableWaterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedBeam
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedBeam</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedBeam, IPlacedBeamGetter> PlacedBeam(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedBeam, IPlacedBeamGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedBeamGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlacedBeam, IPlacedBeamGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedBeam
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedBeam</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedBeam, IPlacedBeamGetter> PlacedBeam(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedBeam, IPlacedBeamGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedBeamGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlacedBeam, IPlacedBeamGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedCreature
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedCreature</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedCreature, IPlacedCreatureGetter> PlacedCreature(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedCreature, IPlacedCreatureGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedCreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlacedCreature, IPlacedCreatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedCreature
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedCreature</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedCreature, IPlacedCreatureGetter> PlacedCreature(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedCreature, IPlacedCreatureGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedCreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlacedCreature, IPlacedCreatureGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedGrenade
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedGrenade</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedGrenade, IPlacedGrenadeGetter> PlacedGrenade(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedGrenade, IPlacedGrenadeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedGrenadeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlacedGrenade, IPlacedGrenadeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedGrenade
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedGrenade</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedGrenade, IPlacedGrenadeGetter> PlacedGrenade(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedGrenade, IPlacedGrenadeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedGrenadeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlacedGrenade, IPlacedGrenadeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedMissile
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedMissile</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedMissile, IPlacedMissileGetter> PlacedMissile(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedMissile, IPlacedMissileGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedMissileGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlacedMissile, IPlacedMissileGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedMissile
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedMissile</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedMissile, IPlacedMissileGetter> PlacedMissile(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedMissile, IPlacedMissileGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedMissileGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlacedMissile, IPlacedMissileGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedNpc
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedNpc</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedNpc, IPlacedNpcGetter> PlacedNpc(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedNpc, IPlacedNpcGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlacedNpc, IPlacedNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedNpc
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedNpc</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedNpc, IPlacedNpcGetter> PlacedNpc(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedNpc, IPlacedNpcGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlacedNpc, IPlacedNpcGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on PlacedObject</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedObject, IPlacedObjectGetter> PlacedObject(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedObject, IPlacedObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlacedObject, IPlacedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to PlacedObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on PlacedObject</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedObject, IPlacedObjectGetter> PlacedObject(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlacedObject, IPlacedObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlacedObject, IPlacedObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Projectile
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Projectile</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IProjectile, IProjectileGetter> Projectile(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IProjectile, IProjectileGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IProjectileGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IProjectile, IProjectileGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Projectile
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Projectile</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IProjectile, IProjectileGetter> Projectile(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IProjectile, IProjectileGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IProjectileGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IProjectile, IProjectileGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Quest
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Quest</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IQuest, IQuestGetter> Quest(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IQuest, IQuestGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IQuest, IQuestGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Quest
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Quest</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IQuest, IQuestGetter> Quest(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IQuest, IQuestGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IQuest, IQuestGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRace, IRaceGetter> Race(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRace, IRaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Race
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Race</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRace, IRaceGetter> Race(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRace, IRaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRace, IRaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to RadiationStage
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on RadiationStage</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRadiationStage, IRadiationStageGetter> RadiationStage(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRadiationStage, IRadiationStageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRadiationStageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRadiationStage, IRadiationStageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to RadiationStage
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on RadiationStage</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRadiationStage, IRadiationStageGetter> RadiationStage(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRadiationStage, IRadiationStageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRadiationStageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRadiationStage, IRadiationStageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ragdoll
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Ragdoll</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRagdoll, IRagdollGetter> Ragdoll(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRagdoll, IRagdollGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRagdollGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRagdoll, IRagdollGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Ragdoll
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Ragdoll</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRagdoll, IRagdollGetter> Ragdoll(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRagdoll, IRagdollGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRagdollGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRagdoll, IRagdollGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Recipe
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Recipe</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRecipe, IRecipeGetter> Recipe(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRecipe, IRecipeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRecipeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRecipe, IRecipeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Recipe
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Recipe</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRecipe, IRecipeGetter> Recipe(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRecipe, IRecipeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRecipeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRecipe, IRecipeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to RecipeCategory
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on RecipeCategory</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRecipeCategory, IRecipeCategoryGetter> RecipeCategory(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRecipeCategory, IRecipeCategoryGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRecipeCategoryGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRecipeCategory, IRecipeCategoryGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to RecipeCategory
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on RecipeCategory</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRecipeCategory, IRecipeCategoryGetter> RecipeCategory(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRecipeCategory, IRecipeCategoryGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRecipeCategoryGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRecipeCategory, IRecipeCategoryGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Region
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Region</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRegion, IRegionGetter> Region(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRegion, IRegionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRegion, IRegionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Region
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Region</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRegion, IRegionGetter> Region(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRegion, IRegionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRegion, IRegionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Reputation
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Reputation</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IReputation, IReputationGetter> Reputation(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IReputation, IReputationGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IReputationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IReputation, IReputationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Reputation
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Reputation</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IReputation, IReputationGetter> Reputation(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IReputation, IReputationGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IReputationGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IReputation, IReputationGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Script
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Script</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IScript, IScriptGetter> Script(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IScript, IScriptGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IScriptGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IScript, IScriptGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Script
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Script</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IScript, IScriptGetter> Script(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IScript, IScriptGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IScriptGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IScript, IScriptGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SleepDeprivationStage
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on SleepDeprivationStage</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ISleepDeprivationStage, ISleepDeprivationStageGetter> SleepDeprivationStage(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ISleepDeprivationStage, ISleepDeprivationStageGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISleepDeprivationStageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ISleepDeprivationStage, ISleepDeprivationStageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to SleepDeprivationStage
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on SleepDeprivationStage</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ISleepDeprivationStage, ISleepDeprivationStageGetter> SleepDeprivationStage(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ISleepDeprivationStage, ISleepDeprivationStageGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISleepDeprivationStageGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ISleepDeprivationStage, ISleepDeprivationStageGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Sound
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Sound</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ISound, ISoundGetter> Sound(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ISound, ISoundGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ISound, ISoundGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Sound
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Sound</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ISound, ISoundGetter> Sound(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ISound, ISoundGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ISound, ISoundGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Spell
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Spell</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ISpell, ISpellGetter> Spell(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ISpell, ISpellGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ISpell, ISpellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Spell
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Spell</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ISpell, ISpellGetter> Spell(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ISpell, ISpellGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ISpell, ISpellGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Static
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Static</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IStatic, IStaticGetter> Static(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IStatic, IStaticGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IStatic, IStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Static
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Static</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IStatic, IStaticGetter> Static(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IStatic, IStaticGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IStatic, IStaticGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StaticCollection
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on StaticCollection</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IStaticCollection, IStaticCollectionGetter> StaticCollection(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IStaticCollection, IStaticCollectionGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IStaticCollectionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IStaticCollection, IStaticCollectionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to StaticCollection
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on StaticCollection</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IStaticCollection, IStaticCollectionGetter> StaticCollection(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IStaticCollection, IStaticCollectionGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IStaticCollectionGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IStaticCollection, IStaticCollectionGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TalkingActivator
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on TalkingActivator</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ITalkingActivator, ITalkingActivatorGetter> TalkingActivator(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ITalkingActivator, ITalkingActivatorGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITalkingActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ITalkingActivator, ITalkingActivatorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TalkingActivator
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on TalkingActivator</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ITalkingActivator, ITalkingActivatorGetter> TalkingActivator(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ITalkingActivator, ITalkingActivatorGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITalkingActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ITalkingActivator, ITalkingActivatorGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Terminal
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Terminal</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ITerminal, ITerminalGetter> Terminal(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ITerminal, ITerminalGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITerminalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ITerminal, ITerminalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Terminal
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Terminal</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ITerminal, ITerminalGetter> Terminal(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ITerminal, ITerminalGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITerminalGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ITerminal, ITerminalGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TextureSet
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on TextureSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ITextureSet, ITextureSetGetter> TextureSet(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ITextureSet, ITextureSetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ITextureSet, ITextureSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to TextureSet
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on TextureSet</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ITextureSet, ITextureSetGetter> TextureSet(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ITextureSet, ITextureSetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITextureSetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ITextureSet, ITextureSetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Tree
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Tree</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ITree, ITreeGetter> Tree(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ITree, ITreeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ITree, ITreeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Tree
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Tree</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ITree, ITreeGetter> Tree(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ITree, ITreeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ITree, ITreeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VoiceType
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on VoiceType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IVoiceType, IVoiceTypeGetter> VoiceType(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IVoiceType, IVoiceTypeGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IVoiceType, IVoiceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to VoiceType
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on VoiceType</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IVoiceType, IVoiceTypeGetter> VoiceType(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IVoiceType, IVoiceTypeGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IVoiceTypeGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IVoiceType, IVoiceTypeGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Water
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Water</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IWater, IWaterGetter> Water(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IWater, IWaterGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IWater, IWaterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Water
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Water</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IWater, IWaterGetter> Water(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IWater, IWaterGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IWater, IWaterGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weapon
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Weapon</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IWeapon, IWeaponGetter> Weapon(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IWeapon, IWeaponGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IWeapon, IWeaponGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weapon
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Weapon</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IWeapon, IWeaponGetter> Weapon(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IWeapon, IWeaponGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IWeapon, IWeaponGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weather
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Weather</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IWeather, IWeatherGetter> Weather(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IWeather, IWeatherGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IWeather, IWeatherGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Weather
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Weather</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IWeather, IWeatherGetter> Weather(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IWeather, IWeatherGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IWeather, IWeatherGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Worldspace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on Worldspace</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IWorldspace, IWorldspaceGetter> Worldspace(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IWorldspace, IWorldspaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IWorldspace, IWorldspaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to Worldspace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on Worldspace</returns>
        public static TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IWorldspace, IWorldspaceGetter> Worldspace(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TopLevelTypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IWorldspace, IWorldspaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IWorldspace, IWorldspaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

        #region Link Interfaces
        /// <summary>
        /// Scope a load order query to IActorBase
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IActorBase</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IActorBase, IActorBaseGetter> IActorBase(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IActorBase, IActorBaseGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActorBaseGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IActorBase, IActorBaseGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IActorBase
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IActorBase</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IActorBase, IActorBaseGetter> IActorBase(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IActorBase, IActorBaseGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActorBaseGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IActorBase, IActorBaseGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IActorValueOrPerk
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IActorValueOrPerk</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IActorValueOrPerk, IActorValueOrPerkGetter> IActorValueOrPerk(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IActorValueOrPerk, IActorValueOrPerkGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IActorValueOrPerkGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IActorValueOrPerk, IActorValueOrPerkGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IActorValueOrPerk
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IActorValueOrPerk</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IActorValueOrPerk, IActorValueOrPerkGetter> IActorValueOrPerk(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IActorValueOrPerk, IActorValueOrPerkGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IActorValueOrPerkGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IActorValueOrPerk, IActorValueOrPerkGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IAmmoOrList
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IAmmoOrList</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAmmoOrList, IAmmoOrListGetter> IAmmoOrList(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAmmoOrList, IAmmoOrListGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IAmmoOrListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IAmmoOrList, IAmmoOrListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IAmmoOrList
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IAmmoOrList</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAmmoOrList, IAmmoOrListGetter> IAmmoOrList(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IAmmoOrList, IAmmoOrListGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IAmmoOrListGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IAmmoOrList, IAmmoOrListGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IBoundItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IBoundItem</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IBoundItem, IBoundItemGetter> IBoundItem(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IBoundItem, IBoundItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IBoundItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IBoundItem, IBoundItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IBoundItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IBoundItem</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IBoundItem, IBoundItemGetter> IBoundItem(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IBoundItem, IBoundItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IBoundItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IBoundItem, IBoundItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ICellOrWorldspace
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on ICellOrWorldspace</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICellOrWorldspace, ICellOrWorldspaceGetter> ICellOrWorldspace(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICellOrWorldspace, ICellOrWorldspaceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<ICellOrWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICellOrWorldspace, ICellOrWorldspaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to ICellOrWorldspace
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on ICellOrWorldspace</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICellOrWorldspace, ICellOrWorldspaceGetter> ICellOrWorldspace(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, ICellOrWorldspace, ICellOrWorldspaceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<ICellOrWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, ICellOrWorldspace, ICellOrWorldspaceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IEffectRecord
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IEffectRecord</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEffectRecord, IEffectRecordGetter> IEffectRecord(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEffectRecord, IEffectRecordGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEffectRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IEffectRecord, IEffectRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IEffectRecord
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IEffectRecord</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEffectRecord, IEffectRecordGetter> IEffectRecord(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEffectRecord, IEffectRecordGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEffectRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IEffectRecord, IEffectRecordGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IEmittance
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IEmittance</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEmittance, IEmittanceGetter> IEmittance(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEmittance, IEmittanceGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IEmittanceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IEmittance, IEmittanceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IEmittance
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IEmittance</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEmittance, IEmittanceGetter> IEmittance(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IEmittance, IEmittanceGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IEmittanceGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IEmittance, IEmittanceGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IExplodeSpawn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IExplodeSpawn</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IExplodeSpawn, IExplodeSpawnGetter> IExplodeSpawn(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IExplodeSpawn, IExplodeSpawnGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IExplodeSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IExplodeSpawn, IExplodeSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IExplodeSpawn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IExplodeSpawn</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IExplodeSpawn, IExplodeSpawnGetter> IExplodeSpawn(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IExplodeSpawn, IExplodeSpawnGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IExplodeSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IExplodeSpawn, IExplodeSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IItem</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IItem, IItemGetter> IItem(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IItem, IItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IItem, IItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IItem</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IItem, IItemGetter> IItem(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IItem, IItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IItem, IItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to INpcSpawn
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on INpcSpawn</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INpcSpawn, INpcSpawnGetter> INpcSpawn(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INpcSpawn, INpcSpawnGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, INpcSpawn, INpcSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to INpcSpawn
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on INpcSpawn</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INpcSpawn, INpcSpawnGetter> INpcSpawn(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, INpcSpawn, INpcSpawnGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<INpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, INpcSpawn, INpcSpawnGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IOwner
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IOwner</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IOwner, IOwnerGetter> IOwner(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IOwner, IOwnerGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IOwner, IOwnerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IOwner
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IOwner</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IOwner, IOwnerGetter> IOwner(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IOwner, IOwnerGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IOwner, IOwnerGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPackageLocationObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPackageLocationObject</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPackageLocationObject, IPackageLocationObjectGetter> IPackageLocationObject(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPackageLocationObject, IPackageLocationObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPackageLocationObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPackageLocationObject, IPackageLocationObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPackageLocationObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPackageLocationObject</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPackageLocationObject, IPackageLocationObjectGetter> IPackageLocationObject(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPackageLocationObject, IPackageLocationObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPackageLocationObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPackageLocationObject, IPackageLocationObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPackageTargetObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPackageTargetObject</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPackageTargetObject, IPackageTargetObjectGetter> IPackageTargetObject(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPackageTargetObject, IPackageTargetObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPackageTargetObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPackageTargetObject, IPackageTargetObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPackageTargetObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPackageTargetObject</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPackageTargetObject, IPackageTargetObjectGetter> IPackageTargetObject(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPackageTargetObject, IPackageTargetObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPackageTargetObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPackageTargetObject, IPackageTargetObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaceableObject
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlaceableObject</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlaceableObject, IPlaceableObjectGetter> IPlaceableObject(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlaceableObject, IPlaceableObjectGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlaceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlaceableObject, IPlaceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaceableObject
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlaceableObject</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlaceableObject, IPlaceableObjectGetter> IPlaceableObject(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlaceableObject, IPlaceableObjectGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlaceableObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlaceableObject, IPlaceableObjectGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaced
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IPlaced</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlaced, IPlacedGetter> IPlaced(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlaced, IPlacedGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlaced, IPlacedGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IPlaced
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IPlaced</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlaced, IPlacedGetter> IPlaced(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IPlaced, IPlacedGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IPlaced, IPlacedGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRecipeItem
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IRecipeItem</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRecipeItem, IRecipeItemGetter> IRecipeItem(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRecipeItem, IRecipeItemGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRecipeItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRecipeItem, IRecipeItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRecipeItem
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IRecipeItem</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRecipeItem, IRecipeItemGetter> IRecipeItem(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRecipeItem, IRecipeItemGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRecipeItemGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRecipeItem, IRecipeItemGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRegionTarget
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IRegionTarget</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRegionTarget, IRegionTargetGetter> IRegionTarget(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRegionTarget, IRegionTargetGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRegionTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRegionTarget, IRegionTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRegionTarget
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IRegionTarget</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRegionTarget, IRegionTargetGetter> IRegionTarget(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRegionTarget, IRegionTargetGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRegionTargetGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRegionTarget, IRegionTargetGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRelatable
        /// </summary>
        /// <param name="listings">ModListings to query</param>
        /// <returns>A typed object to do further queries on IRelatable</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRelatable, IRelatableGetter> IRelatable(this IEnumerable<IModListingGetter<IFallout3ModGetter>> listings)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRelatable, IRelatableGetter>(
                (bool includeDeletedRecords) => listings.WinningOverrides<IRelatableGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => listings.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRelatable, IRelatableGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        /// <summary>
        /// Scope a load order query to IRelatable
        /// </summary>
        /// <param name="mods">Mods to query</param>
        /// <returns>A typed object to do further queries on IRelatable</returns>
        public static TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRelatable, IRelatableGetter> IRelatable(this IEnumerable<IFallout3ModGetter> mods)
        {
            return new TypedLoadOrderAccess<IFallout3Mod, IFallout3ModGetter, IRelatable, IRelatableGetter>(
                (bool includeDeletedRecords) => mods.WinningOverrides<IRelatableGetter>(includeDeletedRecords: includeDeletedRecords),
                (ILinkCache linkCache, bool includeDeletedRecords) => mods.WinningContextOverrides<IFallout3Mod, IFallout3ModGetter, IRelatable, IRelatableGetter>(linkCache, includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

    }
}
