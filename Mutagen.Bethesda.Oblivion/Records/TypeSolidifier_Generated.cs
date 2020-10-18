using System.Collections.Generic;
namespace Mutagen.Bethesda.Oblivion
{
    public static class TypeOptionSolidifierMixIns
    {
        #region Normal
        public static TypedLoadOrderAccess<IAClothingGetter> AClothing(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAClothingGetter>(() => listings.WinningOverrides<IAClothingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAClothingGetter> AClothing(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAClothingGetter>(() => mods.WinningOverrides<IAClothingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IActivatorGetter> Activator(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IActivatorGetter>(() => listings.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IActivatorGetter> Activator(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IActivatorGetter>(() => mods.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAIPackageGetter> AIPackage(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAIPackageGetter>(() => listings.WinningOverrides<IAIPackageGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAIPackageGetter> AIPackage(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAIPackageGetter>(() => mods.WinningOverrides<IAIPackageGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAItemGetter> AItem(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAItemGetter>(() => listings.WinningOverrides<IAItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAItemGetter> AItem(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAItemGetter>(() => mods.WinningOverrides<IAItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAlchemicalApparatusGetter> AlchemicalApparatus(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAlchemicalApparatusGetter>(() => listings.WinningOverrides<IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAlchemicalApparatusGetter> AlchemicalApparatus(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAlchemicalApparatusGetter>(() => mods.WinningOverrides<IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAmmunitionGetter> Ammunition(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAmmunitionGetter>(() => listings.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAmmunitionGetter> Ammunition(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAmmunitionGetter>(() => mods.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAnimatedObjectGetter> AnimatedObject(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAnimatedObjectGetter>(() => listings.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IAnimatedObjectGetter> AnimatedObject(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IAnimatedObjectGetter>(() => mods.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IANpcGetter> ANpc(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IANpcGetter>(() => listings.WinningOverrides<IANpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IANpcGetter> ANpc(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IANpcGetter>(() => mods.WinningOverrides<IANpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IANpcSpawnGetter> ANpcSpawn(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IANpcSpawnGetter>(() => listings.WinningOverrides<IANpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IANpcSpawnGetter> ANpcSpawn(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IANpcSpawnGetter>(() => mods.WinningOverrides<IANpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IArmorGetter> Armor(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IArmorGetter>(() => listings.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IArmorGetter> Armor(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IArmorGetter>(() => mods.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IASpellGetter> ASpell(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IASpellGetter>(() => listings.WinningOverrides<IASpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IASpellGetter> ASpell(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IASpellGetter>(() => mods.WinningOverrides<IASpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IBirthsignGetter> Birthsign(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IBirthsignGetter>(() => listings.WinningOverrides<IBirthsignGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IBirthsignGetter> Birthsign(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IBirthsignGetter>(() => mods.WinningOverrides<IBirthsignGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IBookGetter> Book(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IBookGetter>(() => listings.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IBookGetter> Book(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IBookGetter>(() => mods.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ICellGetter> Cell(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ICellGetter>(() => listings.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ICellGetter> Cell(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ICellGetter>(() => mods.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IClassGetter> Class(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IClassGetter>(() => listings.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IClassGetter> Class(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IClassGetter>(() => mods.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IClimateGetter> Climate(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IClimateGetter>(() => listings.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IClimateGetter> Climate(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IClimateGetter>(() => mods.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IClothingGetter> Clothing(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IClothingGetter>(() => listings.WinningOverrides<IClothingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IClothingGetter> Clothing(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IClothingGetter>(() => mods.WinningOverrides<IClothingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ICombatStyleGetter> CombatStyle(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ICombatStyleGetter>(() => listings.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ICombatStyleGetter> CombatStyle(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ICombatStyleGetter>(() => mods.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IContainerGetter> Container(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IContainerGetter>(() => listings.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IContainerGetter> Container(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IContainerGetter>(() => mods.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ICreatureGetter> Creature(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ICreatureGetter>(() => listings.WinningOverrides<ICreatureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ICreatureGetter> Creature(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ICreatureGetter>(() => mods.WinningOverrides<ICreatureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDialogItemGetter> DialogItem(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDialogItemGetter>(() => listings.WinningOverrides<IDialogItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDialogItemGetter> DialogItem(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDialogItemGetter>(() => mods.WinningOverrides<IDialogItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDialogTopicGetter> DialogTopic(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDialogTopicGetter>(() => listings.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDialogTopicGetter> DialogTopic(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDialogTopicGetter>(() => mods.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDoorGetter> Door(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDoorGetter>(() => listings.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IDoorGetter> Door(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IDoorGetter>(() => mods.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IEffectShaderGetter> EffectShader(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IEffectShaderGetter>(() => listings.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IEffectShaderGetter> EffectShader(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IEffectShaderGetter>(() => mods.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IEnchantmentGetter> Enchantment(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IEnchantmentGetter>(() => listings.WinningOverrides<IEnchantmentGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IEnchantmentGetter> Enchantment(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IEnchantmentGetter>(() => mods.WinningOverrides<IEnchantmentGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IEyeGetter> Eye(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IEyeGetter>(() => listings.WinningOverrides<IEyeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IEyeGetter> Eye(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IEyeGetter>(() => mods.WinningOverrides<IEyeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IFactionGetter> Faction(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IFactionGetter>(() => listings.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IFactionGetter> Faction(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IFactionGetter>(() => mods.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IFloraGetter> Flora(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IFloraGetter>(() => listings.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IFloraGetter> Flora(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IFloraGetter>(() => mods.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IFurnitureGetter> Furniture(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IFurnitureGetter>(() => listings.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IFurnitureGetter> Furniture(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IFurnitureGetter>(() => mods.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGameSettingGetter> GameSetting(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGameSettingGetter>(() => listings.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGameSettingGetter> GameSetting(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGameSettingGetter>(() => mods.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGameSettingFloatGetter> GameSettingFloat(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGameSettingFloatGetter>(() => listings.WinningOverrides<IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGameSettingFloatGetter> GameSettingFloat(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGameSettingFloatGetter>(() => mods.WinningOverrides<IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGameSettingIntGetter> GameSettingInt(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGameSettingIntGetter>(() => listings.WinningOverrides<IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGameSettingIntGetter> GameSettingInt(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGameSettingIntGetter>(() => mods.WinningOverrides<IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGameSettingStringGetter> GameSettingString(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGameSettingStringGetter>(() => listings.WinningOverrides<IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGameSettingStringGetter> GameSettingString(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGameSettingStringGetter>(() => mods.WinningOverrides<IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGlobalGetter> Global(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGlobalGetter>(() => listings.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGlobalGetter> Global(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGlobalGetter>(() => mods.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGlobalFloatGetter> GlobalFloat(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGlobalFloatGetter>(() => listings.WinningOverrides<IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGlobalFloatGetter> GlobalFloat(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGlobalFloatGetter>(() => mods.WinningOverrides<IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGlobalIntGetter> GlobalInt(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGlobalIntGetter>(() => listings.WinningOverrides<IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGlobalIntGetter> GlobalInt(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGlobalIntGetter>(() => mods.WinningOverrides<IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGlobalShortGetter> GlobalShort(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGlobalShortGetter>(() => listings.WinningOverrides<IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGlobalShortGetter> GlobalShort(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGlobalShortGetter>(() => mods.WinningOverrides<IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGrassGetter> Grass(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGrassGetter>(() => listings.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IGrassGetter> Grass(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IGrassGetter>(() => mods.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IHairGetter> Hair(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IHairGetter>(() => listings.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IHairGetter> Hair(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IHairGetter>(() => mods.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IIdleAnimationGetter> IdleAnimation(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IIdleAnimationGetter>(() => listings.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IIdleAnimationGetter> IdleAnimation(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IIdleAnimationGetter>(() => mods.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IIngredientGetter> Ingredient(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IIngredientGetter>(() => listings.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IIngredientGetter> Ingredient(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IIngredientGetter>(() => mods.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IKeyGetter> Key(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IKeyGetter>(() => listings.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IKeyGetter> Key(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IKeyGetter>(() => mods.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILandscapeGetter> Landscape(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILandscapeGetter>(() => listings.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILandscapeGetter> Landscape(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILandscapeGetter>(() => mods.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILandTextureGetter> LandTexture(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILandTextureGetter>(() => listings.WinningOverrides<ILandTextureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILandTextureGetter> LandTexture(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILandTextureGetter>(() => mods.WinningOverrides<ILandTextureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILeveledCreatureGetter> LeveledCreature(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILeveledCreatureGetter>(() => listings.WinningOverrides<ILeveledCreatureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILeveledCreatureGetter> LeveledCreature(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILeveledCreatureGetter>(() => mods.WinningOverrides<ILeveledCreatureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILeveledItemGetter> LeveledItem(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILeveledItemGetter>(() => listings.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILeveledItemGetter> LeveledItem(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILeveledItemGetter>(() => mods.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILeveledSpellGetter> LeveledSpell(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILeveledSpellGetter>(() => listings.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILeveledSpellGetter> LeveledSpell(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILeveledSpellGetter>(() => mods.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILightGetter> Light(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILightGetter>(() => listings.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILightGetter> Light(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILightGetter>(() => mods.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILoadScreenGetter> LoadScreen(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILoadScreenGetter>(() => listings.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ILoadScreenGetter> LoadScreen(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ILoadScreenGetter>(() => mods.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMagicEffectGetter> MagicEffect(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMagicEffectGetter>(() => listings.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMagicEffectGetter> MagicEffect(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMagicEffectGetter>(() => mods.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMiscellaneousGetter> Miscellaneous(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMiscellaneousGetter>(() => listings.WinningOverrides<IMiscellaneousGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IMiscellaneousGetter> Miscellaneous(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IMiscellaneousGetter>(() => mods.WinningOverrides<IMiscellaneousGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<INpcGetter> Npc(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<INpcGetter>(() => listings.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<INpcGetter> Npc(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<INpcGetter>(() => mods.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMajorRecordGetter> OblivionMajorRecord(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMajorRecordGetter>(() => listings.WinningOverrides<IOblivionMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOblivionMajorRecordGetter> OblivionMajorRecord(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOblivionMajorRecordGetter>(() => mods.WinningOverrides<IOblivionMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPathGridGetter> PathGrid(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPathGridGetter>(() => listings.WinningOverrides<IPathGridGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPathGridGetter> PathGrid(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPathGridGetter>(() => mods.WinningOverrides<IPathGridGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlaceGetter> Place(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlaceGetter>(() => listings.WinningOverrides<IPlaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlaceGetter> Place(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlaceGetter>(() => mods.WinningOverrides<IPlaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedCreatureGetter> PlacedCreature(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedCreatureGetter>(() => listings.WinningOverrides<IPlacedCreatureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedCreatureGetter> PlacedCreature(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedCreatureGetter>(() => mods.WinningOverrides<IPlacedCreatureGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedNpcGetter> PlacedNpc(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedNpcGetter>(() => listings.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedNpcGetter> PlacedNpc(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedNpcGetter>(() => mods.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedObjectGetter> PlacedObject(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedObjectGetter>(() => listings.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedObjectGetter> PlacedObject(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedObjectGetter>(() => mods.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPotionGetter> Potion(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPotionGetter>(() => listings.WinningOverrides<IPotionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPotionGetter> Potion(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPotionGetter>(() => mods.WinningOverrides<IPotionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IQuestGetter> Quest(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IQuestGetter>(() => listings.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IQuestGetter> Quest(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IQuestGetter>(() => mods.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IRaceGetter> Race(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IRaceGetter>(() => listings.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IRaceGetter> Race(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IRaceGetter>(() => mods.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IRegionGetter> Region(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IRegionGetter>(() => listings.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IRegionGetter> Region(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IRegionGetter>(() => mods.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IRoadGetter> Road(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IRoadGetter>(() => listings.WinningOverrides<IRoadGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IRoadGetter> Road(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IRoadGetter>(() => mods.WinningOverrides<IRoadGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IScriptGetter> Script(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IScriptGetter>(() => listings.WinningOverrides<IScriptGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IScriptGetter> Script(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IScriptGetter>(() => mods.WinningOverrides<IScriptGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISigilStoneGetter> SigilStone(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISigilStoneGetter>(() => listings.WinningOverrides<ISigilStoneGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISigilStoneGetter> SigilStone(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISigilStoneGetter>(() => mods.WinningOverrides<ISigilStoneGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkillRecordGetter> SkillRecord(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkillRecordGetter>(() => listings.WinningOverrides<ISkillRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISkillRecordGetter> SkillRecord(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISkillRecordGetter>(() => mods.WinningOverrides<ISkillRecordGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISoulGemGetter> SoulGem(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISoulGemGetter>(() => listings.WinningOverrides<ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISoulGemGetter> SoulGem(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISoulGemGetter>(() => mods.WinningOverrides<ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISoundGetter> Sound(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISoundGetter>(() => listings.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISoundGetter> Sound(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISoundGetter>(() => mods.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISpellGetter> Spell(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISpellGetter>(() => listings.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISpellGetter> Spell(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISpellGetter>(() => mods.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISpellLeveledGetter> SpellLeveled(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISpellLeveledGetter>(() => listings.WinningOverrides<ISpellLeveledGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISpellLeveledGetter> SpellLeveled(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISpellLeveledGetter>(() => mods.WinningOverrides<ISpellLeveledGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISpellUnleveledGetter> SpellUnleveled(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISpellUnleveledGetter>(() => listings.WinningOverrides<ISpellUnleveledGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISpellUnleveledGetter> SpellUnleveled(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISpellUnleveledGetter>(() => mods.WinningOverrides<ISpellUnleveledGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IStaticGetter> Static(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IStaticGetter>(() => listings.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IStaticGetter> Static(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IStaticGetter>(() => mods.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISubspaceGetter> Subspace(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISubspaceGetter>(() => listings.WinningOverrides<ISubspaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ISubspaceGetter> Subspace(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ISubspaceGetter>(() => mods.WinningOverrides<ISubspaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ITreeGetter> Tree(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ITreeGetter>(() => listings.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<ITreeGetter> Tree(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<ITreeGetter>(() => mods.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IWaterGetter> Water(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IWaterGetter>(() => listings.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IWaterGetter> Water(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IWaterGetter>(() => mods.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IWeaponGetter> Weapon(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IWeaponGetter>(() => listings.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IWeaponGetter> Weapon(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IWeaponGetter>(() => mods.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IWeatherGetter> Weather(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IWeatherGetter>(() => listings.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IWeatherGetter> Weather(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IWeatherGetter>(() => mods.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IWorldspaceGetter> Worldspace(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IWorldspaceGetter>(() => listings.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IWorldspaceGetter> Worldspace(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IWorldspaceGetter>(() => mods.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

        #region Link Interfaces
        public static TypedLoadOrderAccess<IOwnerGetter> IOwner(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOwnerGetter>(() => listings.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IOwnerGetter> IOwner(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IOwnerGetter>(() => mods.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedGetter> IPlaced(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedGetter>(() => listings.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        public static TypedLoadOrderAccess<IPlacedGetter> IPlaced(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return new TypedLoadOrderAccess<IPlacedGetter>(() => mods.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords));
        }

        #endregion

    }
}
