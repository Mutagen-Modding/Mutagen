using Loqui;
using Loqui.Interfaces;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Analysis.DI;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Noggog.StructuredStrings;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda.Skyrim;

/// <summary>
/// Multi-mod overlay that presents multiple Skyrim mods as a single unified mod.
/// Typically used for reading split mods that were written due to exceeding master limits,
/// but can be used with any collection of mods.
/// </summary>
internal class SkyrimMultiModOverlay : ISkyrimModDisposableGetter
{
    private readonly IReadOnlyList<ISkyrimModGetter> _sourceMods;
    private readonly ModKey _modKey;
    private readonly GameRelease _gameRelease;
    private readonly IReadOnlyList<IModMasterStyledGetter> _masters;

    // Cached merged groups - lazy loaded on first access
    private MergedGroup<IGameSetting, IGameSettingGetter>? _gameSettings;
    private MergedGroup<IKeyword, IKeywordGetter>? _keywords;
    private MergedGroup<ILocationReferenceType, ILocationReferenceTypeGetter>? _locationReferenceTypes;
    private MergedGroup<IActionRecord, IActionRecordGetter>? _actions;
    private MergedGroup<ITextureSet, ITextureSetGetter>? _textureSets;
    private MergedGroup<IGlobal, IGlobalGetter>? _globals;
    private MergedGroup<IClass, IClassGetter>? _classes;
    private MergedGroup<IFaction, IFactionGetter>? _factions;
    private MergedGroup<IHeadPart, IHeadPartGetter>? _headParts;
    private MergedGroup<IHair, IHairGetter>? _hairs;
    private MergedGroup<IEyes, IEyesGetter>? _eyes;
    private MergedGroup<IRace, IRaceGetter>? _races;
    private MergedGroup<ISoundMarker, ISoundMarkerGetter>? _soundMarkers;
    private MergedGroup<IAcousticSpace, IAcousticSpaceGetter>? _acousticSpaces;
    private MergedGroup<IMagicEffect, IMagicEffectGetter>? _magicEffects;
    private MergedGroup<ILandscapeTexture, ILandscapeTextureGetter>? _landscapeTextures;
    private MergedGroup<IObjectEffect, IObjectEffectGetter>? _objectEffects;
    private MergedGroup<ISpell, ISpellGetter>? _spells;
    private MergedGroup<IScroll, IScrollGetter>? _scrolls;
    private MergedGroup<IActivator, IActivatorGetter>? _activators;
    private MergedGroup<ITalkingActivator, ITalkingActivatorGetter>? _talkingActivators;
    private MergedGroup<IArmor, IArmorGetter>? _armors;
    private MergedGroup<IBook, IBookGetter>? _books;
    private MergedGroup<IContainer, IContainerGetter>? _containers;
    private MergedGroup<IDoor, IDoorGetter>? _doors;
    private MergedGroup<IIngredient, IIngredientGetter>? _ingredients;
    private MergedGroup<ILight, ILightGetter>? _lights;
    private MergedGroup<IMiscItem, IMiscItemGetter>? _miscItems;
    private MergedGroup<IAlchemicalApparatus, IAlchemicalApparatusGetter>? _alchemicalApparatuses;
    private MergedGroup<IStatic, IStaticGetter>? _statics;
    private MergedGroup<IMoveableStatic, IMoveableStaticGetter>? _moveableStatics;
    private MergedGroup<IGrass, IGrassGetter>? _grasses;
    private MergedGroup<ITree, ITreeGetter>? _trees;
    private MergedGroup<IFlora, IFloraGetter>? _florae;
    private MergedGroup<IFurniture, IFurnitureGetter>? _furniture;
    private MergedGroup<IWeapon, IWeaponGetter>? _weapons;
    private MergedGroup<IAmmunition, IAmmunitionGetter>? _ammunitions;
    private MergedGroup<INpc, INpcGetter>? _npcs;
    private MergedGroup<ILeveledNpc, ILeveledNpcGetter>? _leveledNpcs;
    private MergedGroup<IKey, IKeyGetter>? _keys;
    private MergedGroup<IIngestible, IIngestibleGetter>? _ingestibles;
    private MergedGroup<IIdleMarker, IIdleMarkerGetter>? _idleMarkers;
    private MergedGroup<IConstructibleObject, IConstructibleObjectGetter>? _constructibleObjects;
    private MergedGroup<IProjectile, IProjectileGetter>? _projectiles;
    private MergedGroup<IHazard, IHazardGetter>? _hazards;
    private MergedGroup<ISoulGem, ISoulGemGetter>? _soulGems;
    private MergedGroup<ILeveledItem, ILeveledItemGetter>? _leveledItems;
    private MergedGroup<IWeather, IWeatherGetter>? _weathers;
    private MergedGroup<IClimate, IClimateGetter>? _climates;
    private MergedGroup<IShaderParticleGeometry, IShaderParticleGeometryGetter>? _shaderParticleGeometries;
    private MergedGroup<IVisualEffect, IVisualEffectGetter>? _visualEffects;
    private MergedGroup<IRegion, IRegionGetter>? _regions;
    private MergedGroup<INavigationMeshInfoMap, INavigationMeshInfoMapGetter>? _navigationMeshInfoMaps;
    private MergedGroup<IWorldspace, IWorldspaceGetter>? _worldspaces;
    private MergedGroup<IDialogTopic, IDialogTopicGetter>? _dialogTopics;
    private MergedGroup<IQuest, IQuestGetter>? _quests;
    private MergedGroup<IIdleAnimation, IIdleAnimationGetter>? _idleAnimations;
    private MergedGroup<IPackage, IPackageGetter>? _packages;
    private MergedGroup<ICombatStyle, ICombatStyleGetter>? _combatStyles;
    private MergedGroup<ILoadScreen, ILoadScreenGetter>? _loadScreens;
    private MergedGroup<ILeveledSpell, ILeveledSpellGetter>? _leveledSpells;
    private MergedGroup<IAnimatedObject, IAnimatedObjectGetter>? _animatedObjects;
    private MergedGroup<IWater, IWaterGetter>? _waters;
    private MergedGroup<IEffectShader, IEffectShaderGetter>? _effectShaders;
    private MergedGroup<IExplosion, IExplosionGetter>? _explosions;
    private MergedGroup<IDebris, IDebrisGetter>? _debris;
    private MergedGroup<IImageSpace, IImageSpaceGetter>? _imageSpaces;
    private MergedGroup<IImageSpaceAdapter, IImageSpaceAdapterGetter>? _imageSpaceAdapters;
    private MergedGroup<IFormList, IFormListGetter>? _formLists;
    private MergedGroup<IPerk, IPerkGetter>? _perks;
    private MergedGroup<IBodyPartData, IBodyPartDataGetter>? _bodyParts;
    private MergedGroup<IAddonNode, IAddonNodeGetter>? _addonNodes;
    private MergedGroup<IActorValueInformation, IActorValueInformationGetter>? _actorValueInformation;
    private MergedGroup<ICameraShot, ICameraShotGetter>? _cameraShots;
    private MergedGroup<ICameraPath, ICameraPathGetter>? _cameraPaths;
    private MergedGroup<IVoiceType, IVoiceTypeGetter>? _voiceTypes;
    private MergedGroup<IMaterialType, IMaterialTypeGetter>? _materialTypes;
    private MergedGroup<IImpact, IImpactGetter>? _impacts;
    private MergedGroup<IImpactDataSet, IImpactDataSetGetter>? _impactDataSets;
    private MergedGroup<IArmorAddon, IArmorAddonGetter>? _armorAddons;
    private MergedGroup<IEncounterZone, IEncounterZoneGetter>? _encounterZones;
    private MergedGroup<ILocation, ILocationGetter>? _locations;
    private MergedGroup<IMessage, IMessageGetter>? _messages;
    private MergedGroup<IDefaultObjectManager, IDefaultObjectManagerGetter>? _defaultObjectManagers;
    private MergedGroup<ILightingTemplate, ILightingTemplateGetter>? _lightingTemplates;
    private MergedGroup<IMusicType, IMusicTypeGetter>? _musicTypes;
    private MergedGroup<IFootstep, IFootstepGetter>? _footsteps;
    private MergedGroup<IFootstepSet, IFootstepSetGetter>? _footstepSets;
    private MergedGroup<IStoryManagerBranchNode, IStoryManagerBranchNodeGetter>? _storyManagerBranchNodes;
    private MergedGroup<IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>? _storyManagerQuestNodes;
    private MergedGroup<IStoryManagerEventNode, IStoryManagerEventNodeGetter>? _storyManagerEventNodes;
    private MergedGroup<IDialogBranch, IDialogBranchGetter>? _dialogBranches;
    private MergedGroup<IMusicTrack, IMusicTrackGetter>? _musicTracks;
    private MergedGroup<IDialogView, IDialogViewGetter>? _dialogViews;
    private MergedGroup<IWordOfPower, IWordOfPowerGetter>? _wordsOfPower;
    private MergedGroup<IShout, IShoutGetter>? _shouts;
    private MergedGroup<IEquipType, IEquipTypeGetter>? _equipTypes;
    private MergedGroup<IRelationship, IRelationshipGetter>? _relationships;
    private MergedGroup<IScene, ISceneGetter>? _scenes;
    private MergedGroup<IAssociationType, IAssociationTypeGetter>? _associationTypes;
    private MergedGroup<IOutfit, IOutfitGetter>? _outfits;
    private MergedGroup<IArtObject, IArtObjectGetter>? _artObjects;
    private MergedGroup<IMaterialObject, IMaterialObjectGetter>? _materialObjects;
    private MergedGroup<IMovementType, IMovementTypeGetter>? _movementTypes;
    private MergedGroup<ISoundDescriptor, ISoundDescriptorGetter>? _soundDescriptors;
    private MergedGroup<IDualCastData, IDualCastDataGetter>? _dualCastData;
    private MergedGroup<ISoundCategory, ISoundCategoryGetter>? _soundCategories;
    private MergedGroup<ISoundOutputModel, ISoundOutputModelGetter>? _soundOutputModels;
    private MergedGroup<ICollisionLayer, ICollisionLayerGetter>? _collisionLayers;
    private MergedGroup<IColorRecord, IColorRecordGetter>? _colors;
    private MergedGroup<IReverbParameters, IReverbParametersGetter>? _reverbParameters;
    private MergedGroup<IVolumetricLighting, IVolumetricLightingGetter>? _volumetricLightings;
    private MergedGroup<ILensFlare, ILensFlareGetter>? _lensFlares;

    public SkyrimMultiModOverlay(
        ModKey modKey,
        GameRelease gameRelease,
        IReadOnlyList<ISkyrimModGetter> sourceMods,
        IReadOnlyList<IModMasterStyledGetter> masters)
    {
        _modKey = modKey;
        _gameRelease = gameRelease;
        _sourceMods = sourceMods;
        _masters = masters;
    }

    public ModKey ModKey => _modKey;
    public GameRelease GameRelease => _gameRelease;
    public SkyrimRelease SkyrimRelease => (SkyrimRelease)_gameRelease;

    // Synthesized properties
    public ISkyrimModHeaderGetter ModHeader => _sourceMods[0].ModHeader;
    public uint NextFormID => _sourceMods.Max(m => m.ModHeader.Stats.NextFormID);
    public uint NextFormIDRaw => NextFormID;

    // Master references - merged from all source mods according to provided load order
    public IReadOnlyList<IModMasterStyledGetter> Masters => _masters;
    public IReadOnlyList<IMasterReferenceGetter> MasterReferences =>
        _masters.Select(m => new MasterReference { Master = m.ModKey } as IMasterReferenceGetter).ToList();

    // Groups - lazy loaded and cached
    public ISkyrimGroupGetter<IGameSettingGetter> GameSettings =>
        _gameSettings ??= new MergedGroup<IGameSetting, IGameSettingGetter>(_sourceMods.Select(m => m.GameSettings));

    public ISkyrimGroupGetter<IKeywordGetter> Keywords =>
        _keywords ??= new MergedGroup<IKeyword, IKeywordGetter>(_sourceMods.Select(m => m.Keywords));

    public ISkyrimGroupGetter<ILocationReferenceTypeGetter> LocationReferenceTypes =>
        _locationReferenceTypes ??= new MergedGroup<ILocationReferenceType, ILocationReferenceTypeGetter>(_sourceMods.Select(m => m.LocationReferenceTypes));

    public ISkyrimGroupGetter<IActionRecordGetter> Actions =>
        _actions ??= new MergedGroup<IActionRecord, IActionRecordGetter>(_sourceMods.Select(m => m.Actions));

    public ISkyrimGroupGetter<ITextureSetGetter> TextureSets =>
        _textureSets ??= new MergedGroup<ITextureSet, ITextureSetGetter>(_sourceMods.Select(m => m.TextureSets));

    public ISkyrimGroupGetter<IGlobalGetter> Globals =>
        _globals ??= new MergedGroup<IGlobal, IGlobalGetter>(_sourceMods.Select(m => m.Globals));

    public ISkyrimGroupGetter<IClassGetter> Classes =>
        _classes ??= new MergedGroup<IClass, IClassGetter>(_sourceMods.Select(m => m.Classes));

    public ISkyrimGroupGetter<IFactionGetter> Factions =>
        _factions ??= new MergedGroup<IFaction, IFactionGetter>(_sourceMods.Select(m => m.Factions));

    public ISkyrimGroupGetter<IHeadPartGetter> HeadParts =>
        _headParts ??= new MergedGroup<IHeadPart, IHeadPartGetter>(_sourceMods.Select(m => m.HeadParts));

    public ISkyrimGroupGetter<IHairGetter> Hairs =>
        _hairs ??= new MergedGroup<IHair, IHairGetter>(_sourceMods.Select(m => m.Hairs));

    public ISkyrimGroupGetter<IEyesGetter> Eyes =>
        _eyes ??= new MergedGroup<IEyes, IEyesGetter>(_sourceMods.Select(m => m.Eyes));

    public ISkyrimGroupGetter<IRaceGetter> Races =>
        _races ??= new MergedGroup<IRace, IRaceGetter>(_sourceMods.Select(m => m.Races));

    public ISkyrimGroupGetter<ISoundMarkerGetter> SoundMarkers =>
        _soundMarkers ??= new MergedGroup<ISoundMarker, ISoundMarkerGetter>(_sourceMods.Select(m => m.SoundMarkers));

    public ISkyrimGroupGetter<IAcousticSpaceGetter> AcousticSpaces =>
        _acousticSpaces ??= new MergedGroup<IAcousticSpace, IAcousticSpaceGetter>(_sourceMods.Select(m => m.AcousticSpaces));

    public ISkyrimGroupGetter<IMagicEffectGetter> MagicEffects =>
        _magicEffects ??= new MergedGroup<IMagicEffect, IMagicEffectGetter>(_sourceMods.Select(m => m.MagicEffects));

    public ISkyrimGroupGetter<ILandscapeTextureGetter> LandscapeTextures =>
        _landscapeTextures ??= new MergedGroup<ILandscapeTexture, ILandscapeTextureGetter>(_sourceMods.Select(m => m.LandscapeTextures));

    public ISkyrimGroupGetter<IObjectEffectGetter> ObjectEffects =>
        _objectEffects ??= new MergedGroup<IObjectEffect, IObjectEffectGetter>(_sourceMods.Select(m => m.ObjectEffects));

    public ISkyrimGroupGetter<ISpellGetter> Spells =>
        _spells ??= new MergedGroup<ISpell, ISpellGetter>(_sourceMods.Select(m => m.Spells));

    public ISkyrimGroupGetter<IScrollGetter> Scrolls =>
        _scrolls ??= new MergedGroup<IScroll, IScrollGetter>(_sourceMods.Select(m => m.Scrolls));

    public ISkyrimGroupGetter<IActivatorGetter> Activators =>
        _activators ??= new MergedGroup<IActivator, IActivatorGetter>(_sourceMods.Select(m => m.Activators));

    public ISkyrimGroupGetter<ITalkingActivatorGetter> TalkingActivators =>
        _talkingActivators ??= new MergedGroup<ITalkingActivator, ITalkingActivatorGetter>(_sourceMods.Select(m => m.TalkingActivators));

    public ISkyrimGroupGetter<IArmorGetter> Armors =>
        _armors ??= new MergedGroup<IArmor, IArmorGetter>(_sourceMods.Select(m => m.Armors));

    public ISkyrimGroupGetter<IBookGetter> Books =>
        _books ??= new MergedGroup<IBook, IBookGetter>(_sourceMods.Select(m => m.Books));

    public ISkyrimGroupGetter<IContainerGetter> Containers =>
        _containers ??= new MergedGroup<IContainer, IContainerGetter>(_sourceMods.Select(m => m.Containers));

    public ISkyrimGroupGetter<IDoorGetter> Doors =>
        _doors ??= new MergedGroup<IDoor, IDoorGetter>(_sourceMods.Select(m => m.Doors));

    public ISkyrimGroupGetter<IIngredientGetter> Ingredients =>
        _ingredients ??= new MergedGroup<IIngredient, IIngredientGetter>(_sourceMods.Select(m => m.Ingredients));

    public ISkyrimGroupGetter<ILightGetter> Lights =>
        _lights ??= new MergedGroup<ILight, ILightGetter>(_sourceMods.Select(m => m.Lights));

    public ISkyrimGroupGetter<IMiscItemGetter> MiscItems =>
        _miscItems ??= new MergedGroup<IMiscItem, IMiscItemGetter>(_sourceMods.Select(m => m.MiscItems));

    public ISkyrimGroupGetter<IAlchemicalApparatusGetter> AlchemicalApparatuses =>
        _alchemicalApparatuses ??= new MergedGroup<IAlchemicalApparatus, IAlchemicalApparatusGetter>(_sourceMods.Select(m => m.AlchemicalApparatuses));

    public ISkyrimGroupGetter<IStaticGetter> Statics =>
        _statics ??= new MergedGroup<IStatic, IStaticGetter>(_sourceMods.Select(m => m.Statics));

    public ISkyrimGroupGetter<IMoveableStaticGetter> MoveableStatics =>
        _moveableStatics ??= new MergedGroup<IMoveableStatic, IMoveableStaticGetter>(_sourceMods.Select(m => m.MoveableStatics));

    public ISkyrimGroupGetter<IGrassGetter> Grasses =>
        _grasses ??= new MergedGroup<IGrass, IGrassGetter>(_sourceMods.Select(m => m.Grasses));

    public ISkyrimGroupGetter<ITreeGetter> Trees =>
        _trees ??= new MergedGroup<ITree, ITreeGetter>(_sourceMods.Select(m => m.Trees));

    public ISkyrimGroupGetter<IFloraGetter> Florae =>
        _florae ??= new MergedGroup<IFlora, IFloraGetter>(_sourceMods.Select(m => m.Florae));

    public ISkyrimGroupGetter<IFurnitureGetter> Furniture =>
        _furniture ??= new MergedGroup<IFurniture, IFurnitureGetter>(_sourceMods.Select(m => m.Furniture));

    public ISkyrimGroupGetter<IWeaponGetter> Weapons =>
        _weapons ??= new MergedGroup<IWeapon, IWeaponGetter>(_sourceMods.Select(m => m.Weapons));

    public ISkyrimGroupGetter<IAmmunitionGetter> Ammunitions =>
        _ammunitions ??= new MergedGroup<IAmmunition, IAmmunitionGetter>(_sourceMods.Select(m => m.Ammunitions));

    public ISkyrimGroupGetter<INpcGetter> Npcs =>
        _npcs ??= new MergedGroup<INpc, INpcGetter>(_sourceMods.Select(m => m.Npcs));

    public ISkyrimGroupGetter<ILeveledNpcGetter> LeveledNpcs =>
        _leveledNpcs ??= new MergedGroup<ILeveledNpc, ILeveledNpcGetter>(_sourceMods.Select(m => m.LeveledNpcs));

    public ISkyrimGroupGetter<IKeyGetter> Keys =>
        _keys ??= new MergedGroup<IKey, IKeyGetter>(_sourceMods.Select(m => m.Keys));

    public ISkyrimGroupGetter<IIngestibleGetter> Ingestibles =>
        _ingestibles ??= new MergedGroup<IIngestible, IIngestibleGetter>(_sourceMods.Select(m => m.Ingestibles));

    public ISkyrimGroupGetter<IIdleMarkerGetter> IdleMarkers =>
        _idleMarkers ??= new MergedGroup<IIdleMarker, IIdleMarkerGetter>(_sourceMods.Select(m => m.IdleMarkers));

    public ISkyrimGroupGetter<IConstructibleObjectGetter> ConstructibleObjects =>
        _constructibleObjects ??= new MergedGroup<IConstructibleObject, IConstructibleObjectGetter>(_sourceMods.Select(m => m.ConstructibleObjects));

    public ISkyrimGroupGetter<IProjectileGetter> Projectiles =>
        _projectiles ??= new MergedGroup<IProjectile, IProjectileGetter>(_sourceMods.Select(m => m.Projectiles));

    public ISkyrimGroupGetter<IHazardGetter> Hazards =>
        _hazards ??= new MergedGroup<IHazard, IHazardGetter>(_sourceMods.Select(m => m.Hazards));

    public ISkyrimGroupGetter<ISoulGemGetter> SoulGems =>
        _soulGems ??= new MergedGroup<ISoulGem, ISoulGemGetter>(_sourceMods.Select(m => m.SoulGems));

    public ISkyrimGroupGetter<ILeveledItemGetter> LeveledItems =>
        _leveledItems ??= new MergedGroup<ILeveledItem, ILeveledItemGetter>(_sourceMods.Select(m => m.LeveledItems));

    public ISkyrimGroupGetter<IWeatherGetter> Weathers =>
        _weathers ??= new MergedGroup<IWeather, IWeatherGetter>(_sourceMods.Select(m => m.Weathers));

    public ISkyrimGroupGetter<IClimateGetter> Climates =>
        _climates ??= new MergedGroup<IClimate, IClimateGetter>(_sourceMods.Select(m => m.Climates));

    public ISkyrimGroupGetter<IShaderParticleGeometryGetter> ShaderParticleGeometries =>
        _shaderParticleGeometries ??= new MergedGroup<IShaderParticleGeometry, IShaderParticleGeometryGetter>(_sourceMods.Select(m => m.ShaderParticleGeometries));

    public ISkyrimGroupGetter<IVisualEffectGetter> VisualEffects =>
        _visualEffects ??= new MergedGroup<IVisualEffect, IVisualEffectGetter>(_sourceMods.Select(m => m.VisualEffects));

    public ISkyrimGroupGetter<IRegionGetter> Regions =>
        _regions ??= new MergedGroup<IRegion, IRegionGetter>(_sourceMods.Select(m => m.Regions));

    public ISkyrimGroupGetter<INavigationMeshInfoMapGetter> NavigationMeshInfoMaps =>
        _navigationMeshInfoMaps ??= new MergedGroup<INavigationMeshInfoMap, INavigationMeshInfoMapGetter>(_sourceMods.Select(m => m.NavigationMeshInfoMaps));

    // Cached merged list group for Cells
    private MergedListGroup? _cells;

    // Special case: Cells uses ISkyrimListGroupGetter instead of ISkyrimGroupGetter
    public ISkyrimListGroupGetter<ICellBlockGetter> Cells =>
        _cells ??= new MergedListGroup(_sourceMods.Select(m => m.Cells));

    public ISkyrimGroupGetter<IWorldspaceGetter> Worldspaces =>
        _worldspaces ??= new MergedGroup<IWorldspace, IWorldspaceGetter>(_sourceMods.Select(m => m.Worldspaces));

    public ISkyrimGroupGetter<IDialogTopicGetter> DialogTopics =>
        _dialogTopics ??= new MergedGroup<IDialogTopic, IDialogTopicGetter>(_sourceMods.Select(m => m.DialogTopics));

    public ISkyrimGroupGetter<IQuestGetter> Quests =>
        _quests ??= new MergedGroup<IQuest, IQuestGetter>(_sourceMods.Select(m => m.Quests));

    public ISkyrimGroupGetter<IIdleAnimationGetter> IdleAnimations =>
        _idleAnimations ??= new MergedGroup<IIdleAnimation, IIdleAnimationGetter>(_sourceMods.Select(m => m.IdleAnimations));

    public ISkyrimGroupGetter<IPackageGetter> Packages =>
        _packages ??= new MergedGroup<IPackage, IPackageGetter>(_sourceMods.Select(m => m.Packages));

    public ISkyrimGroupGetter<ICombatStyleGetter> CombatStyles =>
        _combatStyles ??= new MergedGroup<ICombatStyle, ICombatStyleGetter>(_sourceMods.Select(m => m.CombatStyles));

    public ISkyrimGroupGetter<ILoadScreenGetter> LoadScreens =>
        _loadScreens ??= new MergedGroup<ILoadScreen, ILoadScreenGetter>(_sourceMods.Select(m => m.LoadScreens));

    public ISkyrimGroupGetter<ILeveledSpellGetter> LeveledSpells =>
        _leveledSpells ??= new MergedGroup<ILeveledSpell, ILeveledSpellGetter>(_sourceMods.Select(m => m.LeveledSpells));

    public ISkyrimGroupGetter<IAnimatedObjectGetter> AnimatedObjects =>
        _animatedObjects ??= new MergedGroup<IAnimatedObject, IAnimatedObjectGetter>(_sourceMods.Select(m => m.AnimatedObjects));

    public ISkyrimGroupGetter<IWaterGetter> Waters =>
        _waters ??= new MergedGroup<IWater, IWaterGetter>(_sourceMods.Select(m => m.Waters));

    public ISkyrimGroupGetter<IEffectShaderGetter> EffectShaders =>
        _effectShaders ??= new MergedGroup<IEffectShader, IEffectShaderGetter>(_sourceMods.Select(m => m.EffectShaders));

    public ISkyrimGroupGetter<IExplosionGetter> Explosions =>
        _explosions ??= new MergedGroup<IExplosion, IExplosionGetter>(_sourceMods.Select(m => m.Explosions));

    public ISkyrimGroupGetter<IDebrisGetter> Debris =>
        _debris ??= new MergedGroup<IDebris, IDebrisGetter>(_sourceMods.Select(m => m.Debris));

    public ISkyrimGroupGetter<IImageSpaceGetter> ImageSpaces =>
        _imageSpaces ??= new MergedGroup<IImageSpace, IImageSpaceGetter>(_sourceMods.Select(m => m.ImageSpaces));

    public ISkyrimGroupGetter<IImageSpaceAdapterGetter> ImageSpaceAdapters =>
        _imageSpaceAdapters ??= new MergedGroup<IImageSpaceAdapter, IImageSpaceAdapterGetter>(_sourceMods.Select(m => m.ImageSpaceAdapters));

    public ISkyrimGroupGetter<IFormListGetter> FormLists =>
        _formLists ??= new MergedGroup<IFormList, IFormListGetter>(_sourceMods.Select(m => m.FormLists));

    public ISkyrimGroupGetter<IPerkGetter> Perks =>
        _perks ??= new MergedGroup<IPerk, IPerkGetter>(_sourceMods.Select(m => m.Perks));

    public ISkyrimGroupGetter<IBodyPartDataGetter> BodyParts =>
        _bodyParts ??= new MergedGroup<IBodyPartData, IBodyPartDataGetter>(_sourceMods.Select(m => m.BodyParts));

    public ISkyrimGroupGetter<IAddonNodeGetter> AddonNodes =>
        _addonNodes ??= new MergedGroup<IAddonNode, IAddonNodeGetter>(_sourceMods.Select(m => m.AddonNodes));

    public ISkyrimGroupGetter<IActorValueInformationGetter> ActorValueInformation =>
        _actorValueInformation ??= new MergedGroup<IActorValueInformation, IActorValueInformationGetter>(_sourceMods.Select(m => m.ActorValueInformation));

    public ISkyrimGroupGetter<ICameraShotGetter> CameraShots =>
        _cameraShots ??= new MergedGroup<ICameraShot, ICameraShotGetter>(_sourceMods.Select(m => m.CameraShots));

    public ISkyrimGroupGetter<ICameraPathGetter> CameraPaths =>
        _cameraPaths ??= new MergedGroup<ICameraPath, ICameraPathGetter>(_sourceMods.Select(m => m.CameraPaths));

    public ISkyrimGroupGetter<IVoiceTypeGetter> VoiceTypes =>
        _voiceTypes ??= new MergedGroup<IVoiceType, IVoiceTypeGetter>(_sourceMods.Select(m => m.VoiceTypes));

    public ISkyrimGroupGetter<IMaterialTypeGetter> MaterialTypes =>
        _materialTypes ??= new MergedGroup<IMaterialType, IMaterialTypeGetter>(_sourceMods.Select(m => m.MaterialTypes));

    public ISkyrimGroupGetter<IImpactGetter> Impacts =>
        _impacts ??= new MergedGroup<IImpact, IImpactGetter>(_sourceMods.Select(m => m.Impacts));

    public ISkyrimGroupGetter<IImpactDataSetGetter> ImpactDataSets =>
        _impactDataSets ??= new MergedGroup<IImpactDataSet, IImpactDataSetGetter>(_sourceMods.Select(m => m.ImpactDataSets));

    public ISkyrimGroupGetter<IArmorAddonGetter> ArmorAddons =>
        _armorAddons ??= new MergedGroup<IArmorAddon, IArmorAddonGetter>(_sourceMods.Select(m => m.ArmorAddons));

    public ISkyrimGroupGetter<IEncounterZoneGetter> EncounterZones =>
        _encounterZones ??= new MergedGroup<IEncounterZone, IEncounterZoneGetter>(_sourceMods.Select(m => m.EncounterZones));

    public ISkyrimGroupGetter<ILocationGetter> Locations =>
        _locations ??= new MergedGroup<ILocation, ILocationGetter>(_sourceMods.Select(m => m.Locations));

    public ISkyrimGroupGetter<IMessageGetter> Messages =>
        _messages ??= new MergedGroup<IMessage, IMessageGetter>(_sourceMods.Select(m => m.Messages));

    public ISkyrimGroupGetter<IDefaultObjectManagerGetter> DefaultObjectManagers =>
        _defaultObjectManagers ??= new MergedGroup<IDefaultObjectManager, IDefaultObjectManagerGetter>(_sourceMods.Select(m => m.DefaultObjectManagers));

    public ISkyrimGroupGetter<ILightingTemplateGetter> LightingTemplates =>
        _lightingTemplates ??= new MergedGroup<ILightingTemplate, ILightingTemplateGetter>(_sourceMods.Select(m => m.LightingTemplates));

    public ISkyrimGroupGetter<IMusicTypeGetter> MusicTypes =>
        _musicTypes ??= new MergedGroup<IMusicType, IMusicTypeGetter>(_sourceMods.Select(m => m.MusicTypes));

    public ISkyrimGroupGetter<IFootstepGetter> Footsteps =>
        _footsteps ??= new MergedGroup<IFootstep, IFootstepGetter>(_sourceMods.Select(m => m.Footsteps));

    public ISkyrimGroupGetter<IFootstepSetGetter> FootstepSets =>
        _footstepSets ??= new MergedGroup<IFootstepSet, IFootstepSetGetter>(_sourceMods.Select(m => m.FootstepSets));

    public ISkyrimGroupGetter<IStoryManagerBranchNodeGetter> StoryManagerBranchNodes =>
        _storyManagerBranchNodes ??= new MergedGroup<IStoryManagerBranchNode, IStoryManagerBranchNodeGetter>(_sourceMods.Select(m => m.StoryManagerBranchNodes));

    public ISkyrimGroupGetter<IStoryManagerQuestNodeGetter> StoryManagerQuestNodes =>
        _storyManagerQuestNodes ??= new MergedGroup<IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>(_sourceMods.Select(m => m.StoryManagerQuestNodes));

    public ISkyrimGroupGetter<IStoryManagerEventNodeGetter> StoryManagerEventNodes =>
        _storyManagerEventNodes ??= new MergedGroup<IStoryManagerEventNode, IStoryManagerEventNodeGetter>(_sourceMods.Select(m => m.StoryManagerEventNodes));

    public ISkyrimGroupGetter<IDialogBranchGetter> DialogBranches =>
        _dialogBranches ??= new MergedGroup<IDialogBranch, IDialogBranchGetter>(_sourceMods.Select(m => m.DialogBranches));

    public ISkyrimGroupGetter<IMusicTrackGetter> MusicTracks =>
        _musicTracks ??= new MergedGroup<IMusicTrack, IMusicTrackGetter>(_sourceMods.Select(m => m.MusicTracks));

    public ISkyrimGroupGetter<IDialogViewGetter> DialogViews =>
        _dialogViews ??= new MergedGroup<IDialogView, IDialogViewGetter>(_sourceMods.Select(m => m.DialogViews));

    public ISkyrimGroupGetter<IWordOfPowerGetter> WordsOfPower =>
        _wordsOfPower ??= new MergedGroup<IWordOfPower, IWordOfPowerGetter>(_sourceMods.Select(m => m.WordsOfPower));

    public ISkyrimGroupGetter<IShoutGetter> Shouts =>
        _shouts ??= new MergedGroup<IShout, IShoutGetter>(_sourceMods.Select(m => m.Shouts));

    public ISkyrimGroupGetter<IEquipTypeGetter> EquipTypes =>
        _equipTypes ??= new MergedGroup<IEquipType, IEquipTypeGetter>(_sourceMods.Select(m => m.EquipTypes));

    public ISkyrimGroupGetter<IRelationshipGetter> Relationships =>
        _relationships ??= new MergedGroup<IRelationship, IRelationshipGetter>(_sourceMods.Select(m => m.Relationships));

    public ISkyrimGroupGetter<ISceneGetter> Scenes =>
        _scenes ??= new MergedGroup<IScene, ISceneGetter>(_sourceMods.Select(m => m.Scenes));

    public ISkyrimGroupGetter<IAssociationTypeGetter> AssociationTypes =>
        _associationTypes ??= new MergedGroup<IAssociationType, IAssociationTypeGetter>(_sourceMods.Select(m => m.AssociationTypes));

    public ISkyrimGroupGetter<IOutfitGetter> Outfits =>
        _outfits ??= new MergedGroup<IOutfit, IOutfitGetter>(_sourceMods.Select(m => m.Outfits));

    public ISkyrimGroupGetter<IArtObjectGetter> ArtObjects =>
        _artObjects ??= new MergedGroup<IArtObject, IArtObjectGetter>(_sourceMods.Select(m => m.ArtObjects));

    public ISkyrimGroupGetter<IMaterialObjectGetter> MaterialObjects =>
        _materialObjects ??= new MergedGroup<IMaterialObject, IMaterialObjectGetter>(_sourceMods.Select(m => m.MaterialObjects));

    public ISkyrimGroupGetter<IMovementTypeGetter> MovementTypes =>
        _movementTypes ??= new MergedGroup<IMovementType, IMovementTypeGetter>(_sourceMods.Select(m => m.MovementTypes));

    public ISkyrimGroupGetter<ISoundDescriptorGetter> SoundDescriptors =>
        _soundDescriptors ??= new MergedGroup<ISoundDescriptor, ISoundDescriptorGetter>(_sourceMods.Select(m => m.SoundDescriptors));

    public ISkyrimGroupGetter<IDualCastDataGetter> DualCastData =>
        _dualCastData ??= new MergedGroup<IDualCastData, IDualCastDataGetter>(_sourceMods.Select(m => m.DualCastData));

    public ISkyrimGroupGetter<ISoundCategoryGetter> SoundCategories =>
        _soundCategories ??= new MergedGroup<ISoundCategory, ISoundCategoryGetter>(_sourceMods.Select(m => m.SoundCategories));

    public ISkyrimGroupGetter<ISoundOutputModelGetter> SoundOutputModels =>
        _soundOutputModels ??= new MergedGroup<ISoundOutputModel, ISoundOutputModelGetter>(_sourceMods.Select(m => m.SoundOutputModels));

    public ISkyrimGroupGetter<ICollisionLayerGetter> CollisionLayers =>
        _collisionLayers ??= new MergedGroup<ICollisionLayer, ICollisionLayerGetter>(_sourceMods.Select(m => m.CollisionLayers));

    public ISkyrimGroupGetter<IColorRecordGetter> Colors =>
        _colors ??= new MergedGroup<IColorRecord, IColorRecordGetter>(_sourceMods.Select(m => m.Colors));

    public ISkyrimGroupGetter<IReverbParametersGetter> ReverbParameters =>
        _reverbParameters ??= new MergedGroup<IReverbParameters, IReverbParametersGetter>(_sourceMods.Select(m => m.ReverbParameters));

    public ISkyrimGroupGetter<IVolumetricLightingGetter> VolumetricLightings =>
        _volumetricLightings ??= new MergedGroup<IVolumetricLighting, IVolumetricLightingGetter>(_sourceMods.Select(m => m.VolumetricLightings));

    public ISkyrimGroupGetter<ILensFlareGetter> LensFlares =>
        _lensFlares ??= new MergedGroup<ILensFlare, ILensFlareGetter>(_sourceMods.Select(m => m.LensFlares));

    // Additional ISkyrimModGetter interface members (stubs for now)
    BinaryModdedWriteBuilderTargetChoice<ISkyrimModGetter> ISkyrimModGetter.BeginWrite => throw new NotSupportedException("Multi-mod overlay does not support write operations.");
    IBinaryModdedWriteBuilderTargetChoice IModGetter.BeginWrite => throw new NotSupportedException("Multi-mod overlay does not support write operations.");

    object ISkyrimModGetter.CommonInstance() => this;
    object ISkyrimModGetter.CommonSetterInstance() => throw new NotSupportedException("Multi-mod overlay is read-only.");
    object ISkyrimModGetter.CommonSetterTranslationInstance() => throw new NotSupportedException("Multi-mod overlay is read-only.");

    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var link in mod.EnumerateAssetLinks(queryCategories, linkCache, assetType))
            {
                yield return link;
            }
        }
    }

    ILoquiRegistration ILoquiObject.Registration => null!;

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        sb.AppendLine($"Multi-Mod Overlay: {ModKey} ({_sourceMods.Count} source mods)");
        foreach (var mod in _sourceMods)
        {
            sb.AppendLine($"  - {mod.ModKey}");
        }
    }

    IEnumerable<IModContext<ISkyrimMod, ISkyrimModGetter, TSetter, TGetter>> IMajorRecordContextEnumerable<ISkyrimMod, ISkyrimModGetter>.EnumerateMajorRecordContexts<TSetter, TGetter>(ILinkCache linkCache, bool throwIfUnknown)
    {
        throw new NotImplementedException("Major record contexts not yet implemented for multi-mod overlay.");
    }

    IEnumerable<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> IMajorRecordContextEnumerable<ISkyrimMod, ISkyrimModGetter>.EnumerateMajorRecordContexts(ILinkCache linkCache, Type type, bool throwIfUnknown)
    {
        throw new NotImplementedException("Major record contexts not yet implemented for multi-mod overlay.");
    }

    IGroupGetter<TMajor>? IModGetter.TryGetTopLevelGroup<TMajor>()
    {
        throw new NotImplementedException("TryGetTopLevelGroup not yet implemented for multi-mod overlay.");
    }

    public IGroupGetter? TryGetTopLevelGroup(Type type)
    {
        throw new NotImplementedException("TryGetTopLevelGroup not yet implemented for multi-mod overlay.");
    }

    public void WriteToBinary(FilePath path, BinaryWriteParameters? param = null)
    {
        throw new NotSupportedException("Multi-mod overlay cannot be written to binary. Write the source mods individually.");
    }

    public void WriteToBinary(Stream stream, BinaryWriteParameters? param = null)
    {
        throw new NotSupportedException("Multi-mod overlay cannot be written to binary. Write the source mods individually.");
    }

    public uint GetDefaultInitialNextFormID(bool? isSmallMasterOverride = null)
    {
        return _sourceMods[0].GetDefaultInitialNextFormID(isSmallMasterOverride);
    }

    public uint GetRecordCount()
    {
        return (uint)_sourceMods.Sum(m => m.GetRecordCount());
    }

    IMod IModGetter.DeepCopy()
    {
        throw new NotSupportedException("Multi-mod overlay does not support DeepCopy.");
    }

    IReadOnlyList<IFormLinkGetter<IMajorRecordGetter>> IModGetter.OverriddenForms =>
        throw new NotImplementedException("OverriddenForms not yet implemented for multi-mod overlay.");

    // IModFlagsGetter members
    public bool CanUseLocalization => _sourceMods[0].CanUseLocalization;
    public bool UsingLocalization => _sourceMods[0].UsingLocalization;
    public bool CanBeSmallMaster => _sourceMods[0].CanBeSmallMaster;
    public bool IsSmallMaster => _sourceMods[0].IsSmallMaster;
    public bool CanBeMediumMaster => _sourceMods[0].CanBeMediumMaster;
    public bool IsMediumMaster => _sourceMods[0].IsMediumMaster;
    public bool IsMaster => _sourceMods[0].IsMaster;
    public bool ListsOverriddenForms => _sourceMods[0].ListsOverriddenForms;

    // IModMasterStyledGetter
    public MasterStyle MasterStyle => _sourceMods[0].MasterStyle;

    // IMajorRecordGetterEnumerable
    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var record in mod.EnumerateMajorRecords())
            {
                yield return record;
            }
        }
    }

    IEnumerable<T> IMajorRecordGetterEnumerable.EnumerateMajorRecords<T>(bool throwIfUnknown)
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var record in mod.EnumerateMajorRecords<T>(throwIfUnknown))
            {
                yield return record;
            }
        }
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var record in mod.EnumerateMajorRecords(type, throwIfUnknown))
            {
                yield return record;
            }
        }
    }

    // IMajorRecordSimpleContextEnumerable
    IEnumerable<IModContext<IMajorRecordGetter>> IMajorRecordSimpleContextEnumerable.EnumerateMajorRecordSimpleContexts()
    {
        throw new NotImplementedException("Major record simple contexts not yet implemented for multi-mod overlay.");
    }

    IEnumerable<IModContext<TMajor>> IMajorRecordSimpleContextEnumerable.EnumerateMajorRecordSimpleContexts<TMajor>(bool throwIfUnknown)
    {
        throw new NotImplementedException("Major record simple contexts not yet implemented for multi-mod overlay.");
    }

    IEnumerable<IModContext<IMajorRecordGetter>> IMajorRecordSimpleContextEnumerable.EnumerateMajorRecordSimpleContexts(Type type, bool throwIfUnknown)
    {
        throw new NotImplementedException("Major record simple contexts not yet implemented for multi-mod overlay.");
    }

    // IFormLinkContainerGetter
    public IEnumerable<IFormLinkGetter> EnumerateFormLinks()
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var link in mod.EnumerateFormLinks())
            {
                yield return link;
            }
        }
    }

    // IEqualsMask
    IMask<bool> IEqualsMask.GetEqualsMask(object rhs, EqualsMaskHelper.Include include)
    {
        throw new NotImplementedException("GetEqualsMask not yet implemented for multi-mod overlay.");
    }

    public void Dispose()
    {
        foreach (var mod in _sourceMods)
        {
            (mod as IDisposable)?.Dispose();
        }
    }
}

/// <summary>
/// Merged group that combines multiple groups into a single unified view.
/// Validates no duplicate FormKeys exist and caches results.
/// </summary>
internal class MergedGroup<TMod, TModGetter> : ISkyrimGroupGetter<TModGetter>, IReadOnlyCache<TModGetter, FormKey>
    where TMod : class, ISkyrimMajorRecord, TModGetter
    where TModGetter : class, ISkyrimMajorRecordGetter, IBinaryItem
{
    private readonly IEnumerable<IGroupGetter<TModGetter>> _sourceGroups;
    private Dictionary<FormKey, TModGetter>? _cache;
    private readonly object _cacheLock = new object();

    public MergedGroup(IEnumerable<IGroupGetter<TModGetter>> sourceGroups)
    {
        _sourceGroups = sourceGroups;
    }

    private Dictionary<FormKey, TModGetter> Cache
    {
        get
        {
            if (_cache != null) return _cache;

            lock (_cacheLock)
            {
                if (_cache != null) return _cache;

                var cache = new Dictionary<FormKey, TModGetter>();
                foreach (var group in _sourceGroups)
                {
                    foreach (var record in group)
                    {
                        if (!cache.TryAdd(record.FormKey, record))
                        {
                            throw new SplitModException(
                                $"Duplicate FormKey {record.FormKey} found in split mods. " +
                                "This indicates corruption or an error in the splitting logic.");
                        }
                    }
                }

                _cache = cache;
                return _cache;
            }
        }
    }

    public IEnumerator<TModGetter> GetEnumerator() => Cache.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => Cache.Count;

    public bool TryGetValue(FormKey key, [MaybeNullWhen(false)] out TModGetter value)
    {
        return Cache.TryGetValue(key, out value);
    }

    public TModGetter this[FormKey key] => Cache[key];
    IMajorRecordGetter IGroupGetter.this[FormKey key] => this[key];

    public bool ContainsKey(FormKey key) => Cache.ContainsKey(key);

    public IEnumerable<FormKey> FormKeys => Cache.Keys;

    public Type Type => typeof(TModGetter);

    public IEnumerable<TModGetter> Records => Cache.Values;

    // IGroupGetter members
    IMod IGroupGetter.SourceMod => throw new NotSupportedException("Merged group has multiple source mods, not a single source.");
    IEnumerable<TModGetter> IGroupCommonGetter<TModGetter>.Records => Cache.Values;
    IEnumerable<ILoquiObject> IGroupCommonGetter.Records => Cache.Values;
    IEnumerable<IMajorRecordGetter> IGroupGetter.Records => Cache.Values.Cast<IMajorRecordGetter>();
    IReadOnlyCache<IMajorRecordGetter, FormKey> IGroupGetter.RecordCache => new MajorRecordCacheWrapper(this);

    // IGroupCommonGetter members
    public ILoquiRegistration ContainedRecordRegistration => throw new NotImplementedException();
    public Type ContainedRecordType => typeof(TModGetter);

    // IAssetLinkContainerGetter
    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
    {
        foreach (var record in Cache.Values)
        {
            if (record is IAssetLinkContainerGetter assetContainer)
            {
                foreach (var link in assetContainer.EnumerateAssetLinks(queryCategories, linkCache, assetType))
                {
                    yield return link;
                }
            }
        }
    }

    // IBinaryItem - merged groups don't support binary writing
    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
    {
        throw new NotSupportedException("Merged groups cannot be written to binary. Write the source mods individually.");
    }

    object IBinaryItem.BinaryWriteTranslator => throw new NotSupportedException("Merged groups do not support binary writing.");

    // IFormLinkContainerGetter
    public IEnumerable<IFormLinkGetter> EnumerateFormLinks()
    {
        foreach (var record in Cache.Values)
        {
            if (record is IFormLinkContainerGetter formLinkContainer)
            {
                foreach (var link in formLinkContainer.EnumerateFormLinks())
                {
                    yield return link;
                }
            }
        }
    }

    // ILoquiObject - use null! since we don't have a real registration for merged groups
    ILoquiRegistration ILoquiObject.Registration => null!;

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        sb.AppendLine($"Merged Group ({Count} records):");
        foreach (var record in Cache.Values.Take(10))
        {
            sb.AppendLine($"  - {record.FormKey}: {record.EditorID}");
        }
        if (Count > 10)
        {
            sb.AppendLine($"  ... and {Count - 10} more");
        }
    }

    // IMajorRecordGetterEnumerable
    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords() => Cache.Values;

    IEnumerable<T> IMajorRecordGetterEnumerable.EnumerateMajorRecords<T>(bool throwIfUnknown)
    {
        return Cache.Values.OfType<T>();
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
    {
        return Cache.Values.Where(r => type.IsAssignableFrom(r.GetType()));
    }

    // ISkyrimGroupGetter members
    public object CommonInstance(Type type) => this;
    public object CommonSetterInstance(Type type) => throw new NotSupportedException("Merged group is read-only.");
    public object CommonSetterTranslationInstance() => throw new NotSupportedException("Merged group is read-only.");

    GroupTypeEnum ISkyrimGroupGetter<TModGetter>.Type => GroupTypeEnum.Type;
    int ISkyrimGroupGetter<TModGetter>.LastModified => 0;
    public int Unknown => 0;

    IReadOnlyCache<TModGetter, FormKey> ISkyrimGroupGetter<TModGetter>.RecordCache => this;

    IReadOnlyCache<TModGetter, FormKey> IGroupGetter<TModGetter>.RecordCache => this;

    // IReadOnlyCache explicit implementations
    IEnumerable<FormKey> IReadOnlyCache<TModGetter, FormKey>.Keys => Cache.Keys;
    IEnumerable<TModGetter> IReadOnlyCache<TModGetter, FormKey>.Items => Cache.Values;

    TModGetter? IReadOnlyCache<TModGetter, FormKey>.TryGetValue(FormKey key)
    {
        return TryGetValue(key, out var value) ? value : null;
    }

    IEnumerator<IKeyValue<FormKey, TModGetter>> IEnumerable<IKeyValue<FormKey, TModGetter>>.GetEnumerator()
    {
        return Cache.Select(kvp => (IKeyValue<FormKey, TModGetter>)new KeyValue<FormKey, TModGetter>(kvp.Key, kvp.Value)).GetEnumerator();
    }

    // Wrapper to cast TModGetter to IMajorRecordGetter for IGroupGetter.RecordCache
    private class MajorRecordCacheWrapper : IReadOnlyCache<IMajorRecordGetter, FormKey>
    {
        private readonly MergedGroup<TMod, TModGetter> _source;

        public MajorRecordCacheWrapper(MergedGroup<TMod, TModGetter> source)
        {
            _source = source;
        }

        public IMajorRecordGetter this[FormKey key] => _source[key];
        public IEnumerable<FormKey> Keys => _source.Cache.Keys;
        public IEnumerable<IMajorRecordGetter> Items => _source.Cache.Values.Cast<IMajorRecordGetter>();
        public int Count => _source.Count;
        public bool ContainsKey(FormKey key) => _source.ContainsKey(key);
        public IMajorRecordGetter? TryGetValue(FormKey key) => _source.TryGetValue(key, out var value) ? value : null;

        public IEnumerator<IKeyValue<FormKey, IMajorRecordGetter>> GetEnumerator()
        {
            return _source.Cache.Select(kvp => (IKeyValue<FormKey, IMajorRecordGetter>)new KeyValue<FormKey, IMajorRecordGetter>(kvp.Key, kvp.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

/// <summary>
/// Merged list group that combines multiple list groups (like Cells) into a single unified view.
/// CellBlocks from different mods are merged by BlockNumber.
/// </summary>
internal class MergedListGroup : ISkyrimListGroupGetter<ICellBlockGetter>
{
    private readonly IEnumerable<ISkyrimListGroupGetter<ICellBlockGetter>> _sourceGroups;
    private List<ICellBlockGetter>? _cache;
    private readonly object _cacheLock = new object();

    public MergedListGroup(IEnumerable<ISkyrimListGroupGetter<ICellBlockGetter>> sourceGroups)
    {
        _sourceGroups = sourceGroups;
    }

    private List<ICellBlockGetter> Cache
    {
        get
        {
            if (_cache != null) return _cache;

            lock (_cacheLock)
            {
                if (_cache != null) return _cache;

                // Merge CellBlocks by BlockNumber
                var blocksByNumber = new Dictionary<int, List<ICellBlockGetter>>();

                foreach (var group in _sourceGroups)
                {
                    foreach (var block in group.Records)
                    {
                        if (!blocksByNumber.ContainsKey(block.BlockNumber))
                        {
                            blocksByNumber[block.BlockNumber] = new List<ICellBlockGetter>();
                        }
                        blocksByNumber[block.BlockNumber].Add(block);
                    }
                }

                // Create merged blocks - for now, just take all blocks from all mods
                // In the future, we might want to merge blocks with the same BlockNumber more intelligently
                var result = new List<ICellBlockGetter>();
                foreach (var blockNumber in blocksByNumber.Keys.OrderBy(k => k))
                {
                    // If multiple mods have blocks with the same number, we need to merge them
                    var blocksForNumber = blocksByNumber[blockNumber];
                    if (blocksForNumber.Count == 1)
                    {
                        result.Add(blocksForNumber[0]);
                    }
                    else
                    {
                        // Multiple blocks with same number - create a merged block
                        result.Add(new MergedCellBlock(blockNumber, blocksForNumber));
                    }
                }

                _cache = result;
                return _cache;
            }
        }
    }

    public IEnumerator<ICellBlockGetter> GetEnumerator() => Cache.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => Cache.Count;

    public IReadOnlyList<ICellBlockGetter> Records => Cache;

    // ISkyrimListGroupGetter properties
    public GroupTypeEnum Type => _sourceGroups.FirstOrDefault()?.Type ?? GroupTypeEnum.InteriorCellBlock;
    public int LastModified => _sourceGroups.Max(g => g.LastModified);
    public int Unknown => _sourceGroups.FirstOrDefault()?.Unknown ?? 0;

    // ILoquiObject
    ILoquiRegistration ILoquiObject.Registration => null!;

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        sb.AppendLine($"Merged List Group ({Count} blocks):");
        foreach (var block in Cache.Take(5))
        {
            sb.AppendLine($"  - Block {block.BlockNumber} ({block.SubBlocks.Count} sub-blocks)");
        }
        if (Count > 5)
        {
            sb.AppendLine($"  ... and {Count - 5} more");
        }
    }

    // IAssetLinkContainerGetter
    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
    {
        foreach (var block in Cache)
        {
            if (block is IAssetLinkContainerGetter assetContainer)
            {
                foreach (var link in assetContainer.EnumerateAssetLinks(queryCategories, linkCache, assetType))
                {
                    yield return link;
                }
            }
        }
    }

    // IBinaryItem
    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
    {
        throw new NotSupportedException("Merged list groups cannot be written to binary. Write the source mods individually.");
    }

    object IBinaryItem.BinaryWriteTranslator => throw new NotSupportedException("Merged list groups do not support binary writing.");

    // IFormLinkContainerGetter
    public IEnumerable<IFormLinkGetter> EnumerateFormLinks()
    {
        foreach (var block in Cache)
        {
            if (block is IFormLinkContainerGetter formLinkContainer)
            {
                foreach (var link in formLinkContainer.EnumerateFormLinks())
                {
                    yield return link;
                }
            }
        }
    }

    // IMajorRecordGetterEnumerable
    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
    {
        foreach (var block in Cache)
        {
            foreach (var record in block.EnumerateMajorRecords())
            {
                yield return record;
            }
        }
    }

    IEnumerable<T> IMajorRecordGetterEnumerable.EnumerateMajorRecords<T>(bool throwIfUnknown)
    {
        foreach (var block in Cache)
        {
            foreach (var record in block.EnumerateMajorRecords<T>(throwIfUnknown))
            {
                yield return record;
            }
        }
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
    {
        foreach (var block in Cache)
        {
            foreach (var record in block.EnumerateMajorRecords(type, throwIfUnknown))
            {
                yield return record;
            }
        }
    }

    // ISkyrimListGroupGetter specific
    public object CommonInstance(Type type) => this;
    public object? CommonSetterInstance(Type type) => throw new NotSupportedException("Merged list group is read-only.");
    public object CommonSetterTranslationInstance() => throw new NotSupportedException("Merged list group is read-only.");
}

/// <summary>
/// Merged CellBlock that combines multiple CellBlocks with the same BlockNumber from different mods.
/// </summary>
internal class MergedCellBlock : ICellBlockGetter
{
    private readonly int _blockNumber;
    private readonly List<ICellBlockGetter> _sourceBlocks;
    private List<ICellSubBlockGetter>? _mergedSubBlocks;
    private readonly object _mergeLock = new object();

    public MergedCellBlock(int blockNumber, List<ICellBlockGetter> sourceBlocks)
    {
        _blockNumber = blockNumber;
        _sourceBlocks = sourceBlocks;
    }

    public int BlockNumber => _blockNumber;
    public GroupTypeEnum GroupType => _sourceBlocks.FirstOrDefault()?.GroupType ?? GroupTypeEnum.InteriorCellBlock;
    public int LastModified => _sourceBlocks.Max(b => b.LastModified);
    public int Unknown => _sourceBlocks.FirstOrDefault()?.Unknown ?? 0;

    public IReadOnlyList<ICellSubBlockGetter> SubBlocks
    {
        get
        {
            if (_mergedSubBlocks != null) return _mergedSubBlocks;

            lock (_mergeLock)
            {
                if (_mergedSubBlocks != null) return _mergedSubBlocks;

                // Merge SubBlocks from all source blocks
                var allSubBlocks = new List<ICellSubBlockGetter>();
                foreach (var block in _sourceBlocks)
                {
                    allSubBlocks.AddRange(block.SubBlocks);
                }

                _mergedSubBlocks = allSubBlocks;
                return _mergedSubBlocks;
            }
        }
    }

    // ILoquiObject
    ILoquiRegistration ILoquiObject.Registration => null!;

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        sb.AppendLine($"Merged Cell Block {BlockNumber} ({SubBlocks.Count} sub-blocks from {_sourceBlocks.Count} source blocks)");
    }

    // IAssetLinkContainerGetter
    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
    {
        foreach (var block in _sourceBlocks)
        {
            if (block is IAssetLinkContainerGetter assetContainer)
            {
                foreach (var link in assetContainer.EnumerateAssetLinks(queryCategories, linkCache, assetType))
                {
                    yield return link;
                }
            }
        }
    }

    // IBinaryItem
    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
    {
        throw new NotSupportedException("Merged cell blocks cannot be written to binary. Write the source mods individually.");
    }

    object IBinaryItem.BinaryWriteTranslator => throw new NotSupportedException("Merged cell blocks do not support binary writing.");

    // IFormLinkContainerGetter
    public IEnumerable<IFormLinkGetter> EnumerateFormLinks()
    {
        foreach (var block in _sourceBlocks)
        {
            if (block is IFormLinkContainerGetter formLinkContainer)
            {
                foreach (var link in formLinkContainer.EnumerateFormLinks())
                {
                    yield return link;
                }
            }
        }
    }

    // IMajorRecordGetterEnumerable
    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
    {
        foreach (var subBlock in SubBlocks)
        {
            foreach (var record in subBlock.EnumerateMajorRecords())
            {
                yield return record;
            }
        }
    }

    IEnumerable<T> IMajorRecordGetterEnumerable.EnumerateMajorRecords<T>(bool throwIfUnknown)
    {
        foreach (var subBlock in SubBlocks)
        {
            foreach (var record in subBlock.EnumerateMajorRecords<T>(throwIfUnknown))
            {
                yield return record;
            }
        }
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
    {
        foreach (var subBlock in SubBlocks)
        {
            foreach (var record in subBlock.EnumerateMajorRecords(type, throwIfUnknown))
            {
                yield return record;
            }
        }
    }

    // ICellBlockGetter specific
    public object CommonInstance() => this;
    public object? CommonSetterInstance() => throw new NotSupportedException("Merged cell block is read-only.");
    public object CommonSetterTranslationInstance() => throw new NotSupportedException("Merged cell block is read-only.");
}
