using System.Collections.Generic;
using Mutagen.Bethesda.Internals;

namespace Mutagen.Bethesda.Oblivion
{
    public static class TypeOptionSolidifierMixIns
    {
        #region Normal
        public static TypedLoadOrderAccess<IOblivionMod, IAClothing, IAClothingGetter> AClothing(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAClothing, IAClothingGetter>(
                () => listings.WinningOverrides<IAClothingGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAClothing, IAClothingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IAClothing, IAClothingGetter> AClothing(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAClothing, IAClothingGetter>(
                () => mods.WinningOverrides<IAClothingGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAClothing, IAClothingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IActivator, IActivatorGetter> Activator(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IActivator, IActivatorGetter>(
                () => listings.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IActivator, IActivatorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IActivator, IActivatorGetter> Activator(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IActivator, IActivatorGetter>(
                () => mods.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IActivator, IActivatorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IAIPackage, IAIPackageGetter> AIPackage(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAIPackage, IAIPackageGetter>(
                () => listings.WinningOverrides<IAIPackageGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAIPackage, IAIPackageGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IAIPackage, IAIPackageGetter> AIPackage(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAIPackage, IAIPackageGetter>(
                () => mods.WinningOverrides<IAIPackageGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAIPackage, IAIPackageGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IAItem, IAItemGetter> AItem(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAItem, IAItemGetter>(
                () => listings.WinningOverrides<IAItemGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAItem, IAItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IAItem, IAItemGetter> AItem(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAItem, IAItemGetter>(
                () => mods.WinningOverrides<IAItemGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAItem, IAItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IAlchemicalApparatus, IAlchemicalApparatusGetter> AlchemicalApparatus(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAlchemicalApparatus, IAlchemicalApparatusGetter>(
                () => listings.WinningOverrides<IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IAlchemicalApparatus, IAlchemicalApparatusGetter> AlchemicalApparatus(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAlchemicalApparatus, IAlchemicalApparatusGetter>(
                () => mods.WinningOverrides<IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IAmmunition, IAmmunitionGetter> Ammunition(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAmmunition, IAmmunitionGetter>(
                () => listings.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAmmunition, IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IAmmunition, IAmmunitionGetter> Ammunition(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAmmunition, IAmmunitionGetter>(
                () => mods.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAmmunition, IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IAnimatedObject, IAnimatedObjectGetter> AnimatedObject(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAnimatedObject, IAnimatedObjectGetter>(
                () => listings.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAnimatedObject, IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IAnimatedObject, IAnimatedObjectGetter> AnimatedObject(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IAnimatedObject, IAnimatedObjectGetter>(
                () => mods.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IAnimatedObject, IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IANpc, IANpcGetter> ANpc(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IANpc, IANpcGetter>(
                () => listings.WinningOverrides<IANpcGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IANpc, IANpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IANpc, IANpcGetter> ANpc(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IANpc, IANpcGetter>(
                () => mods.WinningOverrides<IANpcGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IANpc, IANpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IANpcSpawn, IANpcSpawnGetter> ANpcSpawn(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IANpcSpawn, IANpcSpawnGetter>(
                () => listings.WinningOverrides<IANpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IANpcSpawn, IANpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IANpcSpawn, IANpcSpawnGetter> ANpcSpawn(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IANpcSpawn, IANpcSpawnGetter>(
                () => mods.WinningOverrides<IANpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IANpcSpawn, IANpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IArmor, IArmorGetter> Armor(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IArmor, IArmorGetter>(
                () => listings.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IArmor, IArmorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IArmor, IArmorGetter> Armor(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IArmor, IArmorGetter>(
                () => mods.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IArmor, IArmorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IASpell, IASpellGetter> ASpell(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IASpell, IASpellGetter>(
                () => listings.WinningOverrides<IASpellGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IASpell, IASpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IASpell, IASpellGetter> ASpell(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IASpell, IASpellGetter>(
                () => mods.WinningOverrides<IASpellGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IASpell, IASpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IBirthsign, IBirthsignGetter> Birthsign(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IBirthsign, IBirthsignGetter>(
                () => listings.WinningOverrides<IBirthsignGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IBirthsign, IBirthsignGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IBirthsign, IBirthsignGetter> Birthsign(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IBirthsign, IBirthsignGetter>(
                () => mods.WinningOverrides<IBirthsignGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IBirthsign, IBirthsignGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IBook, IBookGetter> Book(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IBook, IBookGetter>(
                () => listings.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IBook, IBookGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IBook, IBookGetter> Book(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IBook, IBookGetter>(
                () => mods.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IBook, IBookGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ICell, ICellGetter> Cell(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ICell, ICellGetter>(
                () => listings.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ICell, ICellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ICell, ICellGetter> Cell(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ICell, ICellGetter>(
                () => mods.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ICell, ICellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IClass, IClassGetter> Class(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IClass, IClassGetter>(
                () => listings.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IClass, IClassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IClass, IClassGetter> Class(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IClass, IClassGetter>(
                () => mods.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IClass, IClassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IClimate, IClimateGetter> Climate(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IClimate, IClimateGetter>(
                () => listings.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IClimate, IClimateGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IClimate, IClimateGetter> Climate(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IClimate, IClimateGetter>(
                () => mods.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IClimate, IClimateGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IClothing, IClothingGetter> Clothing(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IClothing, IClothingGetter>(
                () => listings.WinningOverrides<IClothingGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IClothing, IClothingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IClothing, IClothingGetter> Clothing(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IClothing, IClothingGetter>(
                () => mods.WinningOverrides<IClothingGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IClothing, IClothingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ICombatStyle, ICombatStyleGetter> CombatStyle(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ICombatStyle, ICombatStyleGetter>(
                () => listings.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ICombatStyle, ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ICombatStyle, ICombatStyleGetter> CombatStyle(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ICombatStyle, ICombatStyleGetter>(
                () => mods.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ICombatStyle, ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IContainer, IContainerGetter> Container(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IContainer, IContainerGetter>(
                () => listings.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IContainer, IContainerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IContainer, IContainerGetter> Container(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IContainer, IContainerGetter>(
                () => mods.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IContainer, IContainerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ICreature, ICreatureGetter> Creature(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ICreature, ICreatureGetter>(
                () => listings.WinningOverrides<ICreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ICreature, ICreatureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ICreature, ICreatureGetter> Creature(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ICreature, ICreatureGetter>(
                () => mods.WinningOverrides<ICreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ICreature, ICreatureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IDialogItem, IDialogItemGetter> DialogItem(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IDialogItem, IDialogItemGetter>(
                () => listings.WinningOverrides<IDialogItemGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IDialogItem, IDialogItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IDialogItem, IDialogItemGetter> DialogItem(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IDialogItem, IDialogItemGetter>(
                () => mods.WinningOverrides<IDialogItemGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IDialogItem, IDialogItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IDialogTopic, IDialogTopicGetter> DialogTopic(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IDialogTopic, IDialogTopicGetter>(
                () => listings.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IDialogTopic, IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IDialogTopic, IDialogTopicGetter> DialogTopic(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IDialogTopic, IDialogTopicGetter>(
                () => mods.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IDialogTopic, IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IDoor, IDoorGetter> Door(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IDoor, IDoorGetter>(
                () => listings.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IDoor, IDoorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IDoor, IDoorGetter> Door(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IDoor, IDoorGetter>(
                () => mods.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IDoor, IDoorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IEffectShader, IEffectShaderGetter> EffectShader(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IEffectShader, IEffectShaderGetter>(
                () => listings.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IEffectShader, IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IEffectShader, IEffectShaderGetter> EffectShader(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IEffectShader, IEffectShaderGetter>(
                () => mods.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IEffectShader, IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IEnchantment, IEnchantmentGetter> Enchantment(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IEnchantment, IEnchantmentGetter>(
                () => listings.WinningOverrides<IEnchantmentGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IEnchantment, IEnchantmentGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IEnchantment, IEnchantmentGetter> Enchantment(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IEnchantment, IEnchantmentGetter>(
                () => mods.WinningOverrides<IEnchantmentGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IEnchantment, IEnchantmentGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IEye, IEyeGetter> Eye(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IEye, IEyeGetter>(
                () => listings.WinningOverrides<IEyeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IEye, IEyeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IEye, IEyeGetter> Eye(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IEye, IEyeGetter>(
                () => mods.WinningOverrides<IEyeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IEye, IEyeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IFaction, IFactionGetter> Faction(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IFaction, IFactionGetter>(
                () => listings.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IFaction, IFactionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IFaction, IFactionGetter> Faction(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IFaction, IFactionGetter>(
                () => mods.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IFaction, IFactionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IFlora, IFloraGetter> Flora(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IFlora, IFloraGetter>(
                () => listings.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IFlora, IFloraGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IFlora, IFloraGetter> Flora(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IFlora, IFloraGetter>(
                () => mods.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IFlora, IFloraGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IFurniture, IFurnitureGetter> Furniture(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IFurniture, IFurnitureGetter>(
                () => listings.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IFurniture, IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IFurniture, IFurnitureGetter> Furniture(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IFurniture, IFurnitureGetter>(
                () => mods.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IFurniture, IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IGameSetting, IGameSettingGetter> GameSetting(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGameSetting, IGameSettingGetter>(
                () => listings.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGameSetting, IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IGameSetting, IGameSettingGetter> GameSetting(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGameSetting, IGameSettingGetter>(
                () => mods.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGameSetting, IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IGameSettingFloat, IGameSettingFloatGetter> GameSettingFloat(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGameSettingFloat, IGameSettingFloatGetter>(
                () => listings.WinningOverrides<IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGameSettingFloat, IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IGameSettingFloat, IGameSettingFloatGetter> GameSettingFloat(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGameSettingFloat, IGameSettingFloatGetter>(
                () => mods.WinningOverrides<IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGameSettingFloat, IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IGameSettingInt, IGameSettingIntGetter> GameSettingInt(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGameSettingInt, IGameSettingIntGetter>(
                () => listings.WinningOverrides<IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGameSettingInt, IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IGameSettingInt, IGameSettingIntGetter> GameSettingInt(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGameSettingInt, IGameSettingIntGetter>(
                () => mods.WinningOverrides<IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGameSettingInt, IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IGameSettingString, IGameSettingStringGetter> GameSettingString(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGameSettingString, IGameSettingStringGetter>(
                () => listings.WinningOverrides<IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGameSettingString, IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IGameSettingString, IGameSettingStringGetter> GameSettingString(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGameSettingString, IGameSettingStringGetter>(
                () => mods.WinningOverrides<IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGameSettingString, IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IGlobal, IGlobalGetter> Global(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGlobal, IGlobalGetter>(
                () => listings.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGlobal, IGlobalGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IGlobal, IGlobalGetter> Global(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGlobal, IGlobalGetter>(
                () => mods.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGlobal, IGlobalGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IGlobalFloat, IGlobalFloatGetter> GlobalFloat(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGlobalFloat, IGlobalFloatGetter>(
                () => listings.WinningOverrides<IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGlobalFloat, IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IGlobalFloat, IGlobalFloatGetter> GlobalFloat(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGlobalFloat, IGlobalFloatGetter>(
                () => mods.WinningOverrides<IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGlobalFloat, IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IGlobalInt, IGlobalIntGetter> GlobalInt(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGlobalInt, IGlobalIntGetter>(
                () => listings.WinningOverrides<IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGlobalInt, IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IGlobalInt, IGlobalIntGetter> GlobalInt(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGlobalInt, IGlobalIntGetter>(
                () => mods.WinningOverrides<IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGlobalInt, IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IGlobalShort, IGlobalShortGetter> GlobalShort(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGlobalShort, IGlobalShortGetter>(
                () => listings.WinningOverrides<IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGlobalShort, IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IGlobalShort, IGlobalShortGetter> GlobalShort(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGlobalShort, IGlobalShortGetter>(
                () => mods.WinningOverrides<IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGlobalShort, IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IGrass, IGrassGetter> Grass(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGrass, IGrassGetter>(
                () => listings.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGrass, IGrassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IGrass, IGrassGetter> Grass(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IGrass, IGrassGetter>(
                () => mods.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IGrass, IGrassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IHair, IHairGetter> Hair(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IHair, IHairGetter>(
                () => listings.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IHair, IHairGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IHair, IHairGetter> Hair(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IHair, IHairGetter>(
                () => mods.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IHair, IHairGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IIdleAnimation, IIdleAnimationGetter> IdleAnimation(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IIdleAnimation, IIdleAnimationGetter>(
                () => listings.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IIdleAnimation, IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IIdleAnimation, IIdleAnimationGetter> IdleAnimation(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IIdleAnimation, IIdleAnimationGetter>(
                () => mods.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IIdleAnimation, IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IIngredient, IIngredientGetter> Ingredient(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IIngredient, IIngredientGetter>(
                () => listings.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IIngredient, IIngredientGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IIngredient, IIngredientGetter> Ingredient(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IIngredient, IIngredientGetter>(
                () => mods.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IIngredient, IIngredientGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IKey, IKeyGetter> Key(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IKey, IKeyGetter>(
                () => listings.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IKey, IKeyGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IKey, IKeyGetter> Key(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IKey, IKeyGetter>(
                () => mods.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IKey, IKeyGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ILandscape, ILandscapeGetter> Landscape(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILandscape, ILandscapeGetter>(
                () => listings.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILandscape, ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ILandscape, ILandscapeGetter> Landscape(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILandscape, ILandscapeGetter>(
                () => mods.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILandscape, ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ILandTexture, ILandTextureGetter> LandTexture(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILandTexture, ILandTextureGetter>(
                () => listings.WinningOverrides<ILandTextureGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILandTexture, ILandTextureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ILandTexture, ILandTextureGetter> LandTexture(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILandTexture, ILandTextureGetter>(
                () => mods.WinningOverrides<ILandTextureGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILandTexture, ILandTextureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ILeveledCreature, ILeveledCreatureGetter> LeveledCreature(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILeveledCreature, ILeveledCreatureGetter>(
                () => listings.WinningOverrides<ILeveledCreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILeveledCreature, ILeveledCreatureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ILeveledCreature, ILeveledCreatureGetter> LeveledCreature(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILeveledCreature, ILeveledCreatureGetter>(
                () => mods.WinningOverrides<ILeveledCreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILeveledCreature, ILeveledCreatureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ILeveledItem, ILeveledItemGetter> LeveledItem(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILeveledItem, ILeveledItemGetter>(
                () => listings.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILeveledItem, ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ILeveledItem, ILeveledItemGetter> LeveledItem(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILeveledItem, ILeveledItemGetter>(
                () => mods.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILeveledItem, ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ILeveledSpell, ILeveledSpellGetter> LeveledSpell(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILeveledSpell, ILeveledSpellGetter>(
                () => listings.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILeveledSpell, ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ILeveledSpell, ILeveledSpellGetter> LeveledSpell(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILeveledSpell, ILeveledSpellGetter>(
                () => mods.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILeveledSpell, ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ILight, ILightGetter> Light(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILight, ILightGetter>(
                () => listings.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILight, ILightGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ILight, ILightGetter> Light(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILight, ILightGetter>(
                () => mods.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILight, ILightGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ILoadScreen, ILoadScreenGetter> LoadScreen(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILoadScreen, ILoadScreenGetter>(
                () => listings.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILoadScreen, ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ILoadScreen, ILoadScreenGetter> LoadScreen(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ILoadScreen, ILoadScreenGetter>(
                () => mods.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ILoadScreen, ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IMagicEffect, IMagicEffectGetter> MagicEffect(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IMagicEffect, IMagicEffectGetter>(
                () => listings.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IMagicEffect, IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IMagicEffect, IMagicEffectGetter> MagicEffect(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IMagicEffect, IMagicEffectGetter>(
                () => mods.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IMagicEffect, IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IMiscellaneous, IMiscellaneousGetter> Miscellaneous(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IMiscellaneous, IMiscellaneousGetter>(
                () => listings.WinningOverrides<IMiscellaneousGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IMiscellaneous, IMiscellaneousGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IMiscellaneous, IMiscellaneousGetter> Miscellaneous(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IMiscellaneous, IMiscellaneousGetter>(
                () => mods.WinningOverrides<IMiscellaneousGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IMiscellaneous, IMiscellaneousGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, INpc, INpcGetter> Npc(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, INpc, INpcGetter>(
                () => listings.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, INpc, INpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, INpc, INpcGetter> Npc(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, INpc, INpcGetter>(
                () => mods.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, INpc, INpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IOblivionMajorRecord, IOblivionMajorRecordGetter> OblivionMajorRecord(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionMajorRecord, IOblivionMajorRecordGetter>(
                () => listings.WinningOverrides<IOblivionMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IOblivionMajorRecord, IOblivionMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IOblivionMajorRecord, IOblivionMajorRecordGetter> OblivionMajorRecord(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOblivionMajorRecord, IOblivionMajorRecordGetter>(
                () => mods.WinningOverrides<IOblivionMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IOblivionMajorRecord, IOblivionMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IPathGrid, IPathGridGetter> PathGrid(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPathGrid, IPathGridGetter>(
                () => listings.WinningOverrides<IPathGridGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPathGrid, IPathGridGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IPathGrid, IPathGridGetter> PathGrid(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPathGrid, IPathGridGetter>(
                () => mods.WinningOverrides<IPathGridGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPathGrid, IPathGridGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IPlace, IPlaceGetter> Place(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPlace, IPlaceGetter>(
                () => listings.WinningOverrides<IPlaceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPlace, IPlaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IPlace, IPlaceGetter> Place(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPlace, IPlaceGetter>(
                () => mods.WinningOverrides<IPlaceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPlace, IPlaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IPlacedCreature, IPlacedCreatureGetter> PlacedCreature(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPlacedCreature, IPlacedCreatureGetter>(
                () => listings.WinningOverrides<IPlacedCreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPlacedCreature, IPlacedCreatureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IPlacedCreature, IPlacedCreatureGetter> PlacedCreature(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPlacedCreature, IPlacedCreatureGetter>(
                () => mods.WinningOverrides<IPlacedCreatureGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPlacedCreature, IPlacedCreatureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IPlacedNpc, IPlacedNpcGetter> PlacedNpc(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPlacedNpc, IPlacedNpcGetter>(
                () => listings.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPlacedNpc, IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IPlacedNpc, IPlacedNpcGetter> PlacedNpc(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPlacedNpc, IPlacedNpcGetter>(
                () => mods.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPlacedNpc, IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IPlacedObject, IPlacedObjectGetter> PlacedObject(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPlacedObject, IPlacedObjectGetter>(
                () => listings.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPlacedObject, IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IPlacedObject, IPlacedObjectGetter> PlacedObject(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPlacedObject, IPlacedObjectGetter>(
                () => mods.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPlacedObject, IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IPotion, IPotionGetter> Potion(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPotion, IPotionGetter>(
                () => listings.WinningOverrides<IPotionGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPotion, IPotionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IPotion, IPotionGetter> Potion(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPotion, IPotionGetter>(
                () => mods.WinningOverrides<IPotionGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPotion, IPotionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IQuest, IQuestGetter> Quest(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IQuest, IQuestGetter>(
                () => listings.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IQuest, IQuestGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IQuest, IQuestGetter> Quest(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IQuest, IQuestGetter>(
                () => mods.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IQuest, IQuestGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IRace, IRaceGetter> Race(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IRace, IRaceGetter>(
                () => listings.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IRace, IRaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IRace, IRaceGetter> Race(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IRace, IRaceGetter>(
                () => mods.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IRace, IRaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IRegion, IRegionGetter> Region(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IRegion, IRegionGetter>(
                () => listings.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IRegion, IRegionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IRegion, IRegionGetter> Region(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IRegion, IRegionGetter>(
                () => mods.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IRegion, IRegionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IRoad, IRoadGetter> Road(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IRoad, IRoadGetter>(
                () => listings.WinningOverrides<IRoadGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IRoad, IRoadGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IRoad, IRoadGetter> Road(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IRoad, IRoadGetter>(
                () => mods.WinningOverrides<IRoadGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IRoad, IRoadGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IScript, IScriptGetter> Script(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IScript, IScriptGetter>(
                () => listings.WinningOverrides<IScriptGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IScript, IScriptGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IScript, IScriptGetter> Script(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IScript, IScriptGetter>(
                () => mods.WinningOverrides<IScriptGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IScript, IScriptGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ISigilStone, ISigilStoneGetter> SigilStone(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISigilStone, ISigilStoneGetter>(
                () => listings.WinningOverrides<ISigilStoneGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISigilStone, ISigilStoneGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ISigilStone, ISigilStoneGetter> SigilStone(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISigilStone, ISigilStoneGetter>(
                () => mods.WinningOverrides<ISigilStoneGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISigilStone, ISigilStoneGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ISkillRecord, ISkillRecordGetter> SkillRecord(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISkillRecord, ISkillRecordGetter>(
                () => listings.WinningOverrides<ISkillRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISkillRecord, ISkillRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ISkillRecord, ISkillRecordGetter> SkillRecord(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISkillRecord, ISkillRecordGetter>(
                () => mods.WinningOverrides<ISkillRecordGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISkillRecord, ISkillRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ISoulGem, ISoulGemGetter> SoulGem(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISoulGem, ISoulGemGetter>(
                () => listings.WinningOverrides<ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISoulGem, ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ISoulGem, ISoulGemGetter> SoulGem(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISoulGem, ISoulGemGetter>(
                () => mods.WinningOverrides<ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISoulGem, ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ISound, ISoundGetter> Sound(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISound, ISoundGetter>(
                () => listings.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISound, ISoundGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ISound, ISoundGetter> Sound(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISound, ISoundGetter>(
                () => mods.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISound, ISoundGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ISpell, ISpellGetter> Spell(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISpell, ISpellGetter>(
                () => listings.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISpell, ISpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ISpell, ISpellGetter> Spell(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISpell, ISpellGetter>(
                () => mods.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISpell, ISpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ISpellLeveled, ISpellLeveledGetter> SpellLeveled(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISpellLeveled, ISpellLeveledGetter>(
                () => listings.WinningOverrides<ISpellLeveledGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISpellLeveled, ISpellLeveledGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ISpellLeveled, ISpellLeveledGetter> SpellLeveled(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISpellLeveled, ISpellLeveledGetter>(
                () => mods.WinningOverrides<ISpellLeveledGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISpellLeveled, ISpellLeveledGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ISpellUnleveled, ISpellUnleveledGetter> SpellUnleveled(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISpellUnleveled, ISpellUnleveledGetter>(
                () => listings.WinningOverrides<ISpellUnleveledGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISpellUnleveled, ISpellUnleveledGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ISpellUnleveled, ISpellUnleveledGetter> SpellUnleveled(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISpellUnleveled, ISpellUnleveledGetter>(
                () => mods.WinningOverrides<ISpellUnleveledGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISpellUnleveled, ISpellUnleveledGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IStatic, IStaticGetter> Static(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IStatic, IStaticGetter>(
                () => listings.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IStatic, IStaticGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IStatic, IStaticGetter> Static(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IStatic, IStaticGetter>(
                () => mods.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IStatic, IStaticGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ISubspace, ISubspaceGetter> Subspace(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISubspace, ISubspaceGetter>(
                () => listings.WinningOverrides<ISubspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISubspace, ISubspaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ISubspace, ISubspaceGetter> Subspace(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ISubspace, ISubspaceGetter>(
                () => mods.WinningOverrides<ISubspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ISubspace, ISubspaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ITree, ITreeGetter> Tree(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ITree, ITreeGetter>(
                () => listings.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ITree, ITreeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, ITree, ITreeGetter> Tree(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, ITree, ITreeGetter>(
                () => mods.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, ITree, ITreeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IWater, IWaterGetter> Water(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IWater, IWaterGetter>(
                () => listings.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IWater, IWaterGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IWater, IWaterGetter> Water(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IWater, IWaterGetter>(
                () => mods.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IWater, IWaterGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IWeapon, IWeaponGetter> Weapon(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IWeapon, IWeaponGetter>(
                () => listings.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IWeapon, IWeaponGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IWeapon, IWeaponGetter> Weapon(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IWeapon, IWeaponGetter>(
                () => mods.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IWeapon, IWeaponGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IWeather, IWeatherGetter> Weather(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IWeather, IWeatherGetter>(
                () => listings.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IWeather, IWeatherGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IWeather, IWeatherGetter> Weather(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IWeather, IWeatherGetter>(
                () => mods.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IWeather, IWeatherGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IWorldspace, IWorldspaceGetter> Worldspace(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IWorldspace, IWorldspaceGetter>(
                () => listings.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IWorldspace, IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IWorldspace, IWorldspaceGetter> Worldspace(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IWorldspace, IWorldspaceGetter>(
                () => mods.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IWorldspace, IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

        #region Link Interfaces
        public static TypedLoadOrderAccess<IOblivionMod, IOwner, IOwnerGetter> IOwner(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOwner, IOwnerGetter>(
                () => listings.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IOwner, IOwnerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IOwner, IOwnerGetter> IOwner(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IOwner, IOwnerGetter>(
                () => mods.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IOwner, IOwnerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IPlaced, IPlacedGetter> IPlaced(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPlaced, IPlacedGetter>(
                () => listings.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                () => listings.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPlaced, IPlacedGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMod, IPlaced, IPlacedGetter> IPlaced(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMod, IPlaced, IPlacedGetter>(
                () => mods.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords),
                () => mods.WinningOverrideContexts<IOblivionMod, IOblivionModGetter, IPlaced, IPlacedGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

    }
}
