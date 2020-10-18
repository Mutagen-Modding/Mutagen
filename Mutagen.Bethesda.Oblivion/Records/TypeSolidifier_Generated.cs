using System.Collections.Generic;
namespace Mutagen.Bethesda.Oblivion
{
    public static class TypeOptionSolidifierMixIns
    {
        #region Normal
        public static IEnumerable<IAClothingGetter> AClothing(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IAClothingGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAClothingGetter> AClothing(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IAClothingGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IActivatorGetter> Activator(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IActivatorGetter> Activator(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IActivatorGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAIPackageGetter> AIPackage(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IAIPackageGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAIPackageGetter> AIPackage(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IAIPackageGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAItemGetter> AItem(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IAItemGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAItemGetter> AItem(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IAItemGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAlchemicalApparatusGetter> AlchemicalApparatus(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAlchemicalApparatusGetter> AlchemicalApparatus(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IAlchemicalApparatusGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAmmunitionGetter> Ammunition(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAmmunitionGetter> Ammunition(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IAmmunitionGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAnimatedObjectGetter> AnimatedObject(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IAnimatedObjectGetter> AnimatedObject(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IAnimatedObjectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IANpcGetter> ANpc(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IANpcGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IANpcGetter> ANpc(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IANpcGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IANpcSpawnGetter> ANpcSpawn(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IANpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IANpcSpawnGetter> ANpcSpawn(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IANpcSpawnGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IArmorGetter> Armor(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IArmorGetter> Armor(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IArmorGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IASpellGetter> ASpell(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IASpellGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IASpellGetter> ASpell(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IASpellGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IBirthsignGetter> Birthsign(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IBirthsignGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IBirthsignGetter> Birthsign(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IBirthsignGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IBookGetter> Book(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IBookGetter> Book(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IBookGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ICellGetter> Cell(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ICellGetter> Cell(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ICellGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IClassGetter> Class(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IClassGetter> Class(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IClassGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IClimateGetter> Climate(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IClimateGetter> Climate(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IClimateGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IClothingGetter> Clothing(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IClothingGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IClothingGetter> Clothing(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IClothingGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ICombatStyleGetter> CombatStyle(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ICombatStyleGetter> CombatStyle(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ICombatStyleGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IContainerGetter> Container(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IContainerGetter> Container(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IContainerGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ICreatureGetter> Creature(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ICreatureGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ICreatureGetter> Creature(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ICreatureGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDialogItemGetter> DialogItem(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IDialogItemGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDialogItemGetter> DialogItem(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IDialogItemGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDialogTopicGetter> DialogTopic(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDialogTopicGetter> DialogTopic(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IDialogTopicGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDoorGetter> Door(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IDoorGetter> Door(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IDoorGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IEffectShaderGetter> EffectShader(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IEffectShaderGetter> EffectShader(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IEffectShaderGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IEnchantmentGetter> Enchantment(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IEnchantmentGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IEnchantmentGetter> Enchantment(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IEnchantmentGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IEyeGetter> Eye(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IEyeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IEyeGetter> Eye(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IEyeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IFactionGetter> Faction(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IFactionGetter> Faction(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IFactionGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IFloraGetter> Flora(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IFloraGetter> Flora(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IFloraGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IFurnitureGetter> Furniture(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IFurnitureGetter> Furniture(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IFurnitureGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGameSettingGetter> GameSetting(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGameSettingGetter> GameSetting(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IGameSettingGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGameSettingFloatGetter> GameSettingFloat(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGameSettingFloatGetter> GameSettingFloat(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IGameSettingFloatGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGameSettingIntGetter> GameSettingInt(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGameSettingIntGetter> GameSettingInt(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IGameSettingIntGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGameSettingStringGetter> GameSettingString(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGameSettingStringGetter> GameSettingString(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IGameSettingStringGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGlobalGetter> Global(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGlobalGetter> Global(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IGlobalGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGlobalFloatGetter> GlobalFloat(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGlobalFloatGetter> GlobalFloat(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IGlobalFloatGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGlobalIntGetter> GlobalInt(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGlobalIntGetter> GlobalInt(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IGlobalIntGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGlobalShortGetter> GlobalShort(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGlobalShortGetter> GlobalShort(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IGlobalShortGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGrassGetter> Grass(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IGrassGetter> Grass(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IGrassGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IHairGetter> Hair(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IHairGetter> Hair(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IHairGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IIdleAnimationGetter> IdleAnimation(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IIdleAnimationGetter> IdleAnimation(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IIdleAnimationGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IIngredientGetter> Ingredient(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IIngredientGetter> Ingredient(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IIngredientGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IKeyGetter> Key(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IKeyGetter> Key(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IKeyGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILandscapeGetter> Landscape(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILandscapeGetter> Landscape(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILandscapeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILandTextureGetter> LandTexture(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILandTextureGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILandTextureGetter> LandTexture(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILandTextureGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILeveledCreatureGetter> LeveledCreature(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILeveledCreatureGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILeveledCreatureGetter> LeveledCreature(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILeveledCreatureGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILeveledItemGetter> LeveledItem(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILeveledItemGetter> LeveledItem(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILeveledItemGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILeveledSpellGetter> LeveledSpell(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILeveledSpellGetter> LeveledSpell(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILeveledSpellGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILightGetter> Light(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILightGetter> Light(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILightGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILoadScreenGetter> LoadScreen(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ILoadScreenGetter> LoadScreen(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ILoadScreenGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMagicEffectGetter> MagicEffect(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMagicEffectGetter> MagicEffect(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IMagicEffectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMiscellaneousGetter> Miscellaneous(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IMiscellaneousGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IMiscellaneousGetter> Miscellaneous(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IMiscellaneousGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<INpcGetter> Npc(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<INpcGetter> Npc(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<INpcGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IOblivionMajorRecordGetter> OblivionMajorRecord(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IOblivionMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IOblivionMajorRecordGetter> OblivionMajorRecord(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IOblivionMajorRecordGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPathGridGetter> PathGrid(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPathGridGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPathGridGetter> PathGrid(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPathGridGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlaceGetter> Place(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPlaceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlaceGetter> Place(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPlaceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedCreatureGetter> PlacedCreature(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPlacedCreatureGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedCreatureGetter> PlacedCreature(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPlacedCreatureGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedNpcGetter> PlacedNpc(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedNpcGetter> PlacedNpc(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPlacedNpcGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedObjectGetter> PlacedObject(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedObjectGetter> PlacedObject(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPlacedObjectGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPotionGetter> Potion(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPotionGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPotionGetter> Potion(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPotionGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IQuestGetter> Quest(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IQuestGetter> Quest(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IQuestGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IRaceGetter> Race(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IRaceGetter> Race(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IRaceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IRegionGetter> Region(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IRegionGetter> Region(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IRegionGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IRoadGetter> Road(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IRoadGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IRoadGetter> Road(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IRoadGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IScriptGetter> Script(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IScriptGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IScriptGetter> Script(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IScriptGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISigilStoneGetter> SigilStone(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ISigilStoneGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISigilStoneGetter> SigilStone(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ISigilStoneGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISkillRecordGetter> SkillRecord(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ISkillRecordGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISkillRecordGetter> SkillRecord(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ISkillRecordGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISoulGemGetter> SoulGem(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISoulGemGetter> SoulGem(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ISoulGemGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISoundGetter> Sound(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISoundGetter> Sound(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ISoundGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISpellGetter> Spell(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISpellGetter> Spell(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ISpellGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISpellLeveledGetter> SpellLeveled(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ISpellLeveledGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISpellLeveledGetter> SpellLeveled(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ISpellLeveledGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISpellUnleveledGetter> SpellUnleveled(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ISpellUnleveledGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISpellUnleveledGetter> SpellUnleveled(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ISpellUnleveledGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IStaticGetter> Static(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IStaticGetter> Static(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IStaticGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISubspaceGetter> Subspace(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ISubspaceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ISubspaceGetter> Subspace(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ISubspaceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ITreeGetter> Tree(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<ITreeGetter> Tree(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<ITreeGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IWaterGetter> Water(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IWaterGetter> Water(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IWaterGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IWeaponGetter> Weapon(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IWeaponGetter> Weapon(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IWeaponGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IWeatherGetter> Weather(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IWeatherGetter> Weather(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IWeatherGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IWorldspaceGetter> Worldspace(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IWorldspaceGetter> Worldspace(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IWorldspaceGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        #endregion

        #region Link Interfaces
        public static IEnumerable<IOwnerGetter> IOwner(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IOwnerGetter> IOwner(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IOwnerGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedGetter> IPlaced(
            this IEnumerable<IModListing<IOblivionModGetter>> listings,
            bool includeDeletedRecords = false)
        {
            return listings.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        public static IEnumerable<IPlacedGetter> IPlaced(
            this IEnumerable<IOblivionModGetter> mods,
            bool includeDeletedRecords = false)
        {
            return mods.WinningOverrides<IPlacedGetter>(includeDeletedRecords: includeDeletedRecords);
        }

        #endregion

    }
}
