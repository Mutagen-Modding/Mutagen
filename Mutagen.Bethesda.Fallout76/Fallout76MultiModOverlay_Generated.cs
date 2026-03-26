using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Exceptions;
using Noggog;
using Noggog.StructuredStrings;
using Loqui.Internal;

namespace Mutagen.Bethesda.Fallout76;

/// <summary>
/// Multi-mod overlay that presents multiple Fallout76 mods as a single unified mod.
/// Typically used for reading split mods that were written due to exceeding master limits,
/// but can be used with any collection of mods.
/// </summary>
internal class Fallout76MultiModOverlay : IFallout76ModDisposableGetter
{
    private readonly IReadOnlyList<IFallout76ModGetter> _sourceMods;
    private readonly IReadOnlyList<IModDisposeGetter>? _disposeSourceMods;
    private readonly ModKey _modKey;
    private readonly IReadOnlyList<IMasterReferenceGetter> _masters;

    private MergedGroup<IGameSettingGetter>? _gameSettings;
    private MergedGroup<IKeywordGetter>? _keywords;
    private MergedGroup<ILocationReferenceTypeGetter>? _locationReferenceTypes;
    private MergedGroup<IActionRecordGetter>? _actions;
    private MergedGroup<ITransformGetter>? _transforms;
    private MergedGroup<IComponentGetter>? _components;
    private MergedGroup<ITextureSetGetter>? _textureSets;
    private MergedGroup<IGlobalGetter>? _globals;
    private MergedGroup<IADamageTypeGetter>? _damageTypes;
    private MergedGroup<IClassGetter>? _classes;
    private MergedGroup<IFactionGetter>? _factions;
    private MergedGroup<IHeadPartGetter>? _headParts;
    private MergedGroup<IRaceGetter>? _races;
    private MergedGroup<ISoundMarkerGetter>? _soundMarkers;
    private MergedGroup<IAcousticSpaceGetter>? _acousticSpaces;
    private MergedGroup<IMagicEffectGetter>? _magicEffects;
    private MergedGroup<ILandscapeTextureGetter>? _landscapeTextures;
    private MergedGroup<IObjectEffectGetter>? _objectEffects;
    private MergedGroup<ISpellGetter>? _spells;
    private MergedGroup<IActivatorGetter>? _activators;
    private MergedGroup<ITalkingActivatorGetter>? _talkingActivators;
    private MergedGroup<IArmorGetter>? _armors;
    private MergedGroup<IBookGetter>? _books;
    private MergedGroup<IContainerGetter>? _containers;
    private MergedGroup<IDoorGetter>? _doors;
    private MergedGroup<IIngredientGetter>? _ingredients;
    private MergedGroup<ILightGetter>? _lights;
    private MergedGroup<IMiscItemGetter>? _miscItems;
    private MergedGroup<IStaticGetter>? _statics;
    private MergedGroup<IStaticCollectionGetter>? _staticCollections;
    private MergedGroup<IMovableStaticGetter>? _movableStatics;
    private MergedGroup<IGrassGetter>? _grasses;
    private MergedGroup<ITreeGetter>? _trees;
    private MergedGroup<IFloraGetter>? _florae;
    private MergedGroup<IFurnitureGetter>? _furniture;
    private MergedGroup<IWeaponGetter>? _weapons;
    private MergedGroup<IAmmunitionGetter>? _ammunitions;
    private MergedGroup<INpcGetter>? _npcs;
    private MergedGroup<ILeveledNpcGetter>? _leveledNpcs;
    private MergedGroup<IKeyGetter>? _keys;
    private MergedGroup<IIngestibleGetter>? _ingestibles;
    private MergedGroup<IIdleMarkerGetter>? _idleMarkers;
    private MergedGroup<IHolotapeGetter>? _holotapes;
    private MergedGroup<IProjectileGetter>? _projectiles;
    private MergedGroup<IHazardGetter>? _hazards;
    private MergedGroup<IBendableSplineGetter>? _bendableSplines;
    private MergedGroup<ITerminalGetter>? _terminals;
    private MergedGroup<ILeveledItemGetter>? _leveledItems;
    private MergedGroup<IWeatherGetter>? _weather;
    private MergedGroup<IClimateGetter>? _climates;
    private MergedGroup<IShaderParticleGeometryGetter>? _shaderParticleGeometries;
    private MergedGroup<IVisualEffectGetter>? _visualEffects;
    private MergedGroup<IRegionGetter>? _regions;
    private MergedGroup<INavigationMeshInfoMapGetter>? _navigationMeshInfoMaps;
    private MergedListGroup? _cells;
    private MergedGroup<IWorldspaceGetter>? _worldspaces;
    private MergedGroup<IQuestGetter>? _quests;
    private MergedGroup<IIdleAnimationGetter>? _idleAnimations;
    private MergedGroup<IPackageGetter>? _packages;
    private MergedGroup<ICombatStyleGetter>? _combatStyles;
    private MergedGroup<ILoadScreenGetter>? _loadScreens;
    private MergedGroup<IAnimatedObjectGetter>? _animatedObjects;
    private MergedGroup<IWaterGetter>? _waters;
    private MergedGroup<IEffectShaderGetter>? _effectShaders;
    private MergedGroup<IExplosionGetter>? _explosions;
    private MergedGroup<IDebrisGetter>? _debris;
    private MergedGroup<IImageSpaceGetter>? _imageSpaces;
    private MergedGroup<IImageSpaceAdapterGetter>? _imageSpaceAdapters;
    private MergedGroup<IFormListGetter>? _formLists;
    private MergedGroup<IPerkGetter>? _perks;
    private MergedGroup<IBodyPartDataGetter>? _bodyParts;
    private MergedGroup<IAddonNodeGetter>? _addonNodes;
    private MergedGroup<IActorValueInformationGetter>? _actorValueInformation;
    private MergedGroup<ICameraShotGetter>? _cameraShots;
    private MergedGroup<ICameraPathGetter>? _cameraPaths;
    private MergedGroup<IVoiceTypeGetter>? _voiceTypes;
    private MergedGroup<IMaterialTypeGetter>? _materialTypes;
    private MergedGroup<IImpactGetter>? _impacts;
    private MergedGroup<IImpactDataSetGetter>? _impactDataSets;
    private MergedGroup<IArmorAddonGetter>? _armorAddons;
    private MergedGroup<IEncounterZoneGetter>? _encounterZones;
    private MergedGroup<ILocationGetter>? _locations;
    private MergedGroup<IMessageGetter>? _messages;
    private MergedGroup<IDefaultObjectManagerGetter>? _defaultObjectManagers;
    private MergedGroup<IDefaultObjectGetter>? _defaultObjects;
    private MergedGroup<ILightingTemplateGetter>? _lightingTemplates;
    private MergedGroup<IMusicTypeGetter>? _musicTypes;
    private MergedGroup<IFootstepGetter>? _footsteps;
    private MergedGroup<IFootstepSetGetter>? _footstepSets;
    private MergedGroup<IStoryManagerBranchNodeGetter>? _storyManagerBranchNodes;
    private MergedGroup<IStoryManagerQuestNodeGetter>? _storyManagerQuestNodes;
    private MergedGroup<IStoryManagerEventNodeGetter>? _storyManagerEventNodes;
    private MergedGroup<IMusicTrackGetter>? _musicTracks;
    private MergedGroup<IDialogViewGetter>? _dialogViews;
    private MergedGroup<IEquipTypeGetter>? _equipTypes;
    private MergedGroup<IRelationshipGetter>? _relationships;
    private MergedGroup<IAssociationTypeGetter>? _associationTypes;
    private MergedGroup<IOutfitGetter>? _outfits;
    private MergedGroup<IArtObjectGetter>? _artObjects;
    private MergedGroup<IMaterialObjectGetter>? _materialObjects;
    private MergedGroup<IMovementTypeGetter>? _movementTypes;
    private MergedGroup<ISoundDescriptorGetter>? _soundDescriptors;
    private MergedGroup<ISoundCategoryGetter>? _soundCategories;
    private MergedGroup<ISoundOutputModelGetter>? _soundOutputModels;
    private MergedGroup<ICollisionLayerGetter>? _collisionLayers;
    private MergedGroup<IColorRecordGetter>? _colors;
    private MergedGroup<IReverbParametersGetter>? _reverbParameters;
    private MergedGroup<IPackInGetter>? _packIns;
    private MergedGroup<IReferenceGroupGetter>? _referenceGroups;
    private MergedGroup<IAimModelGetter>? _aimModels;
    private MergedGroup<ILayerGetter>? _layers;
    private MergedGroup<IConstructibleObjectGetter>? _constructibleObjects;
    private MergedGroup<IAObjectModificationGetter>? _objectModifications;
    private MergedGroup<IMaterialSwapGetter>? _materialSwaps;
    private MergedGroup<IZoomGetter>? _zooms;
    private MergedGroup<IInstanceNamingRulesGetter>? _instanceNamingRules;
    private MergedGroup<ISoundKeywordMappingGetter>? _soundKeywordMappings;
    private MergedGroup<IAudioEffectChainGetter>? _audioEffectChains;
    private MergedGroup<ISceneCollectionGetter>? _sceneCollections;
    private MergedGroup<IAttractionRuleGetter>? _attractionRules;
    private MergedGroup<IAudioCategorySnapshotGetter>? _audioCategorySnapshots;
    private MergedGroup<IAnimationSoundTagSetGetter>? _animationSoundTagSets;
    private MergedGroup<INavigationMeshObstacleManagerGetter>? _navigationMeshObstacleManagers;
    private MergedGroup<ILensFlareGetter>? _lensFlares;
    private MergedGroup<IGodRaysGetter>? _godRays;
    private MergedGroup<IObjectVisibilityManagerGetter>? _objectVisibilityManagers;

    /// <summary>
    /// Creates a new Fallout76MultiModOverlay from multiple source mod files.
    /// </summary>
    public Fallout76MultiModOverlay(
        ModKey modKey,
        IEnumerable<IFallout76ModGetter> sourceMods,
        IReadOnlyList<IMasterReferenceGetter> mergedMasters)
    {
        _modKey = modKey;
        var sourceList = sourceMods.ToList();
        _sourceMods = sourceList;
        _masters = mergedMasters;

        // Track disposable mods for cleanup
        var disposables = sourceList.OfType<IModDisposeGetter>().ToList();
        _disposeSourceMods = disposables.Count > 0 ? disposables : null;

        if (_sourceMods.Count == 0)
        {
            throw new ArgumentException("Must provide at least one source mod", nameof(sourceMods));
        }
    }

    public ModKey ModKey => _modKey;
    public IFallout76ModHeaderGetter ModHeader => _sourceMods[0].ModHeader;
    public IReadOnlyList<IMasterReferenceGetter> MasterReferences => _masters;
    public Fallout76Release Fallout76Release => _sourceMods[0].Fallout76Release;
    GameRelease IModGetter.GameRelease => Fallout76Release.ToGameRelease();

    public object CommonInstance() => Fallout76ModCommon.Instance;
    public object? CommonSetterInstance() => Fallout76ModSetterCommon.Instance;
    public object CommonSetterTranslationInstance() => Fallout76ModSetterTranslationCommon.Instance;


    public IFallout76GroupGetter<IGameSettingGetter> GameSettings =>
        _gameSettings ??= new MergedGroup<IGameSettingGetter>(
            _sourceMods.Select(m => m.GameSettings));
    public IFallout76GroupGetter<IKeywordGetter> Keywords =>
        _keywords ??= new MergedGroup<IKeywordGetter>(
            _sourceMods.Select(m => m.Keywords));
    public IFallout76GroupGetter<ILocationReferenceTypeGetter> LocationReferenceTypes =>
        _locationReferenceTypes ??= new MergedGroup<ILocationReferenceTypeGetter>(
            _sourceMods.Select(m => m.LocationReferenceTypes));
    public IFallout76GroupGetter<IActionRecordGetter> Actions =>
        _actions ??= new MergedGroup<IActionRecordGetter>(
            _sourceMods.Select(m => m.Actions));
    public IFallout76GroupGetter<ITransformGetter> Transforms =>
        _transforms ??= new MergedGroup<ITransformGetter>(
            _sourceMods.Select(m => m.Transforms));
    public IFallout76GroupGetter<IComponentGetter> Components =>
        _components ??= new MergedGroup<IComponentGetter>(
            _sourceMods.Select(m => m.Components));
    public IFallout76GroupGetter<ITextureSetGetter> TextureSets =>
        _textureSets ??= new MergedGroup<ITextureSetGetter>(
            _sourceMods.Select(m => m.TextureSets));
    public IFallout76GroupGetter<IGlobalGetter> Globals =>
        _globals ??= new MergedGroup<IGlobalGetter>(
            _sourceMods.Select(m => m.Globals));
    public IFallout76GroupGetter<IADamageTypeGetter> DamageTypes =>
        _damageTypes ??= new MergedGroup<IADamageTypeGetter>(
            _sourceMods.Select(m => m.DamageTypes));
    public IFallout76GroupGetter<IClassGetter> Classes =>
        _classes ??= new MergedGroup<IClassGetter>(
            _sourceMods.Select(m => m.Classes));
    public IFallout76GroupGetter<IFactionGetter> Factions =>
        _factions ??= new MergedGroup<IFactionGetter>(
            _sourceMods.Select(m => m.Factions));
    public IFallout76GroupGetter<IHeadPartGetter> HeadParts =>
        _headParts ??= new MergedGroup<IHeadPartGetter>(
            _sourceMods.Select(m => m.HeadParts));
    public IFallout76GroupGetter<IRaceGetter> Races =>
        _races ??= new MergedGroup<IRaceGetter>(
            _sourceMods.Select(m => m.Races));
    public IFallout76GroupGetter<ISoundMarkerGetter> SoundMarkers =>
        _soundMarkers ??= new MergedGroup<ISoundMarkerGetter>(
            _sourceMods.Select(m => m.SoundMarkers));
    public IFallout76GroupGetter<IAcousticSpaceGetter> AcousticSpaces =>
        _acousticSpaces ??= new MergedGroup<IAcousticSpaceGetter>(
            _sourceMods.Select(m => m.AcousticSpaces));
    public IFallout76GroupGetter<IMagicEffectGetter> MagicEffects =>
        _magicEffects ??= new MergedGroup<IMagicEffectGetter>(
            _sourceMods.Select(m => m.MagicEffects));
    public IFallout76GroupGetter<ILandscapeTextureGetter> LandscapeTextures =>
        _landscapeTextures ??= new MergedGroup<ILandscapeTextureGetter>(
            _sourceMods.Select(m => m.LandscapeTextures));
    public IFallout76GroupGetter<IObjectEffectGetter> ObjectEffects =>
        _objectEffects ??= new MergedGroup<IObjectEffectGetter>(
            _sourceMods.Select(m => m.ObjectEffects));
    public IFallout76GroupGetter<ISpellGetter> Spells =>
        _spells ??= new MergedGroup<ISpellGetter>(
            _sourceMods.Select(m => m.Spells));
    public IFallout76GroupGetter<IActivatorGetter> Activators =>
        _activators ??= new MergedGroup<IActivatorGetter>(
            _sourceMods.Select(m => m.Activators));
    public IFallout76GroupGetter<ITalkingActivatorGetter> TalkingActivators =>
        _talkingActivators ??= new MergedGroup<ITalkingActivatorGetter>(
            _sourceMods.Select(m => m.TalkingActivators));
    public IFallout76GroupGetter<IArmorGetter> Armors =>
        _armors ??= new MergedGroup<IArmorGetter>(
            _sourceMods.Select(m => m.Armors));
    public IFallout76GroupGetter<IBookGetter> Books =>
        _books ??= new MergedGroup<IBookGetter>(
            _sourceMods.Select(m => m.Books));
    public IFallout76GroupGetter<IContainerGetter> Containers =>
        _containers ??= new MergedGroup<IContainerGetter>(
            _sourceMods.Select(m => m.Containers));
    public IFallout76GroupGetter<IDoorGetter> Doors =>
        _doors ??= new MergedGroup<IDoorGetter>(
            _sourceMods.Select(m => m.Doors));
    public IFallout76GroupGetter<IIngredientGetter> Ingredients =>
        _ingredients ??= new MergedGroup<IIngredientGetter>(
            _sourceMods.Select(m => m.Ingredients));
    public IFallout76GroupGetter<ILightGetter> Lights =>
        _lights ??= new MergedGroup<ILightGetter>(
            _sourceMods.Select(m => m.Lights));
    public IFallout76GroupGetter<IMiscItemGetter> MiscItems =>
        _miscItems ??= new MergedGroup<IMiscItemGetter>(
            _sourceMods.Select(m => m.MiscItems));
    public IFallout76GroupGetter<IStaticGetter> Statics =>
        _statics ??= new MergedGroup<IStaticGetter>(
            _sourceMods.Select(m => m.Statics));
    public IFallout76GroupGetter<IStaticCollectionGetter> StaticCollections =>
        _staticCollections ??= new MergedGroup<IStaticCollectionGetter>(
            _sourceMods.Select(m => m.StaticCollections));
    public IFallout76GroupGetter<IMovableStaticGetter> MovableStatics =>
        _movableStatics ??= new MergedGroup<IMovableStaticGetter>(
            _sourceMods.Select(m => m.MovableStatics));
    public IFallout76GroupGetter<IGrassGetter> Grasses =>
        _grasses ??= new MergedGroup<IGrassGetter>(
            _sourceMods.Select(m => m.Grasses));
    public IFallout76GroupGetter<ITreeGetter> Trees =>
        _trees ??= new MergedGroup<ITreeGetter>(
            _sourceMods.Select(m => m.Trees));
    public IFallout76GroupGetter<IFloraGetter> Florae =>
        _florae ??= new MergedGroup<IFloraGetter>(
            _sourceMods.Select(m => m.Florae));
    public IFallout76GroupGetter<IFurnitureGetter> Furniture =>
        _furniture ??= new MergedGroup<IFurnitureGetter>(
            _sourceMods.Select(m => m.Furniture));
    public IFallout76GroupGetter<IWeaponGetter> Weapons =>
        _weapons ??= new MergedGroup<IWeaponGetter>(
            _sourceMods.Select(m => m.Weapons));
    public IFallout76GroupGetter<IAmmunitionGetter> Ammunitions =>
        _ammunitions ??= new MergedGroup<IAmmunitionGetter>(
            _sourceMods.Select(m => m.Ammunitions));
    public IFallout76GroupGetter<INpcGetter> Npcs =>
        _npcs ??= new MergedGroup<INpcGetter>(
            _sourceMods.Select(m => m.Npcs));
    public IFallout76GroupGetter<ILeveledNpcGetter> LeveledNpcs =>
        _leveledNpcs ??= new MergedGroup<ILeveledNpcGetter>(
            _sourceMods.Select(m => m.LeveledNpcs));
    public IFallout76GroupGetter<IKeyGetter> Keys =>
        _keys ??= new MergedGroup<IKeyGetter>(
            _sourceMods.Select(m => m.Keys));
    public IFallout76GroupGetter<IIngestibleGetter> Ingestibles =>
        _ingestibles ??= new MergedGroup<IIngestibleGetter>(
            _sourceMods.Select(m => m.Ingestibles));
    public IFallout76GroupGetter<IIdleMarkerGetter> IdleMarkers =>
        _idleMarkers ??= new MergedGroup<IIdleMarkerGetter>(
            _sourceMods.Select(m => m.IdleMarkers));
    public IFallout76GroupGetter<IHolotapeGetter> Holotapes =>
        _holotapes ??= new MergedGroup<IHolotapeGetter>(
            _sourceMods.Select(m => m.Holotapes));
    public IFallout76GroupGetter<IProjectileGetter> Projectiles =>
        _projectiles ??= new MergedGroup<IProjectileGetter>(
            _sourceMods.Select(m => m.Projectiles));
    public IFallout76GroupGetter<IHazardGetter> Hazards =>
        _hazards ??= new MergedGroup<IHazardGetter>(
            _sourceMods.Select(m => m.Hazards));
    public IFallout76GroupGetter<IBendableSplineGetter> BendableSplines =>
        _bendableSplines ??= new MergedGroup<IBendableSplineGetter>(
            _sourceMods.Select(m => m.BendableSplines));
    public IFallout76GroupGetter<ITerminalGetter> Terminals =>
        _terminals ??= new MergedGroup<ITerminalGetter>(
            _sourceMods.Select(m => m.Terminals));
    public IFallout76GroupGetter<ILeveledItemGetter> LeveledItems =>
        _leveledItems ??= new MergedGroup<ILeveledItemGetter>(
            _sourceMods.Select(m => m.LeveledItems));
    public IFallout76GroupGetter<IWeatherGetter> Weather =>
        _weather ??= new MergedGroup<IWeatherGetter>(
            _sourceMods.Select(m => m.Weather));
    public IFallout76GroupGetter<IClimateGetter> Climates =>
        _climates ??= new MergedGroup<IClimateGetter>(
            _sourceMods.Select(m => m.Climates));
    public IFallout76GroupGetter<IShaderParticleGeometryGetter> ShaderParticleGeometries =>
        _shaderParticleGeometries ??= new MergedGroup<IShaderParticleGeometryGetter>(
            _sourceMods.Select(m => m.ShaderParticleGeometries));
    public IFallout76GroupGetter<IVisualEffectGetter> VisualEffects =>
        _visualEffects ??= new MergedGroup<IVisualEffectGetter>(
            _sourceMods.Select(m => m.VisualEffects));
    public IFallout76GroupGetter<IRegionGetter> Regions =>
        _regions ??= new MergedGroup<IRegionGetter>(
            _sourceMods.Select(m => m.Regions));
    public IFallout76GroupGetter<INavigationMeshInfoMapGetter> NavigationMeshInfoMaps =>
        _navigationMeshInfoMaps ??= new MergedGroup<INavigationMeshInfoMapGetter>(
            _sourceMods.Select(m => m.NavigationMeshInfoMaps));
    public IFallout76ListGroupGetter<ICellBlockGetter> Cells =>
        _cells ??= new MergedListGroup(_sourceMods.Select(m => m.Cells));
    public IFallout76GroupGetter<IWorldspaceGetter> Worldspaces =>
        _worldspaces ??= new MergedGroup<IWorldspaceGetter>(
            _sourceMods.Select(m => m.Worldspaces));
    public IFallout76GroupGetter<IQuestGetter> Quests =>
        _quests ??= new MergedGroup<IQuestGetter>(
            _sourceMods.Select(m => m.Quests));
    public IFallout76GroupGetter<IIdleAnimationGetter> IdleAnimations =>
        _idleAnimations ??= new MergedGroup<IIdleAnimationGetter>(
            _sourceMods.Select(m => m.IdleAnimations));
    public IFallout76GroupGetter<IPackageGetter> Packages =>
        _packages ??= new MergedGroup<IPackageGetter>(
            _sourceMods.Select(m => m.Packages));
    public IFallout76GroupGetter<ICombatStyleGetter> CombatStyles =>
        _combatStyles ??= new MergedGroup<ICombatStyleGetter>(
            _sourceMods.Select(m => m.CombatStyles));
    public IFallout76GroupGetter<ILoadScreenGetter> LoadScreens =>
        _loadScreens ??= new MergedGroup<ILoadScreenGetter>(
            _sourceMods.Select(m => m.LoadScreens));
    public IFallout76GroupGetter<IAnimatedObjectGetter> AnimatedObjects =>
        _animatedObjects ??= new MergedGroup<IAnimatedObjectGetter>(
            _sourceMods.Select(m => m.AnimatedObjects));
    public IFallout76GroupGetter<IWaterGetter> Waters =>
        _waters ??= new MergedGroup<IWaterGetter>(
            _sourceMods.Select(m => m.Waters));
    public IFallout76GroupGetter<IEffectShaderGetter> EffectShaders =>
        _effectShaders ??= new MergedGroup<IEffectShaderGetter>(
            _sourceMods.Select(m => m.EffectShaders));
    public IFallout76GroupGetter<IExplosionGetter> Explosions =>
        _explosions ??= new MergedGroup<IExplosionGetter>(
            _sourceMods.Select(m => m.Explosions));
    public IFallout76GroupGetter<IDebrisGetter> Debris =>
        _debris ??= new MergedGroup<IDebrisGetter>(
            _sourceMods.Select(m => m.Debris));
    public IFallout76GroupGetter<IImageSpaceGetter> ImageSpaces =>
        _imageSpaces ??= new MergedGroup<IImageSpaceGetter>(
            _sourceMods.Select(m => m.ImageSpaces));
    public IFallout76GroupGetter<IImageSpaceAdapterGetter> ImageSpaceAdapters =>
        _imageSpaceAdapters ??= new MergedGroup<IImageSpaceAdapterGetter>(
            _sourceMods.Select(m => m.ImageSpaceAdapters));
    public IFallout76GroupGetter<IFormListGetter> FormLists =>
        _formLists ??= new MergedGroup<IFormListGetter>(
            _sourceMods.Select(m => m.FormLists));
    public IFallout76GroupGetter<IPerkGetter> Perks =>
        _perks ??= new MergedGroup<IPerkGetter>(
            _sourceMods.Select(m => m.Perks));
    public IFallout76GroupGetter<IBodyPartDataGetter> BodyParts =>
        _bodyParts ??= new MergedGroup<IBodyPartDataGetter>(
            _sourceMods.Select(m => m.BodyParts));
    public IFallout76GroupGetter<IAddonNodeGetter> AddonNodes =>
        _addonNodes ??= new MergedGroup<IAddonNodeGetter>(
            _sourceMods.Select(m => m.AddonNodes));
    public IFallout76GroupGetter<IActorValueInformationGetter> ActorValueInformation =>
        _actorValueInformation ??= new MergedGroup<IActorValueInformationGetter>(
            _sourceMods.Select(m => m.ActorValueInformation));
    public IFallout76GroupGetter<ICameraShotGetter> CameraShots =>
        _cameraShots ??= new MergedGroup<ICameraShotGetter>(
            _sourceMods.Select(m => m.CameraShots));
    public IFallout76GroupGetter<ICameraPathGetter> CameraPaths =>
        _cameraPaths ??= new MergedGroup<ICameraPathGetter>(
            _sourceMods.Select(m => m.CameraPaths));
    public IFallout76GroupGetter<IVoiceTypeGetter> VoiceTypes =>
        _voiceTypes ??= new MergedGroup<IVoiceTypeGetter>(
            _sourceMods.Select(m => m.VoiceTypes));
    public IFallout76GroupGetter<IMaterialTypeGetter> MaterialTypes =>
        _materialTypes ??= new MergedGroup<IMaterialTypeGetter>(
            _sourceMods.Select(m => m.MaterialTypes));
    public IFallout76GroupGetter<IImpactGetter> Impacts =>
        _impacts ??= new MergedGroup<IImpactGetter>(
            _sourceMods.Select(m => m.Impacts));
    public IFallout76GroupGetter<IImpactDataSetGetter> ImpactDataSets =>
        _impactDataSets ??= new MergedGroup<IImpactDataSetGetter>(
            _sourceMods.Select(m => m.ImpactDataSets));
    public IFallout76GroupGetter<IArmorAddonGetter> ArmorAddons =>
        _armorAddons ??= new MergedGroup<IArmorAddonGetter>(
            _sourceMods.Select(m => m.ArmorAddons));
    public IFallout76GroupGetter<IEncounterZoneGetter> EncounterZones =>
        _encounterZones ??= new MergedGroup<IEncounterZoneGetter>(
            _sourceMods.Select(m => m.EncounterZones));
    public IFallout76GroupGetter<ILocationGetter> Locations =>
        _locations ??= new MergedGroup<ILocationGetter>(
            _sourceMods.Select(m => m.Locations));
    public IFallout76GroupGetter<IMessageGetter> Messages =>
        _messages ??= new MergedGroup<IMessageGetter>(
            _sourceMods.Select(m => m.Messages));
    public IFallout76GroupGetter<IDefaultObjectManagerGetter> DefaultObjectManagers =>
        _defaultObjectManagers ??= new MergedGroup<IDefaultObjectManagerGetter>(
            _sourceMods.Select(m => m.DefaultObjectManagers));
    public IFallout76GroupGetter<IDefaultObjectGetter> DefaultObjects =>
        _defaultObjects ??= new MergedGroup<IDefaultObjectGetter>(
            _sourceMods.Select(m => m.DefaultObjects));
    public IFallout76GroupGetter<ILightingTemplateGetter> LightingTemplates =>
        _lightingTemplates ??= new MergedGroup<ILightingTemplateGetter>(
            _sourceMods.Select(m => m.LightingTemplates));
    public IFallout76GroupGetter<IMusicTypeGetter> MusicTypes =>
        _musicTypes ??= new MergedGroup<IMusicTypeGetter>(
            _sourceMods.Select(m => m.MusicTypes));
    public IFallout76GroupGetter<IFootstepGetter> Footsteps =>
        _footsteps ??= new MergedGroup<IFootstepGetter>(
            _sourceMods.Select(m => m.Footsteps));
    public IFallout76GroupGetter<IFootstepSetGetter> FootstepSets =>
        _footstepSets ??= new MergedGroup<IFootstepSetGetter>(
            _sourceMods.Select(m => m.FootstepSets));
    public IFallout76GroupGetter<IStoryManagerBranchNodeGetter> StoryManagerBranchNodes =>
        _storyManagerBranchNodes ??= new MergedGroup<IStoryManagerBranchNodeGetter>(
            _sourceMods.Select(m => m.StoryManagerBranchNodes));
    public IFallout76GroupGetter<IStoryManagerQuestNodeGetter> StoryManagerQuestNodes =>
        _storyManagerQuestNodes ??= new MergedGroup<IStoryManagerQuestNodeGetter>(
            _sourceMods.Select(m => m.StoryManagerQuestNodes));
    public IFallout76GroupGetter<IStoryManagerEventNodeGetter> StoryManagerEventNodes =>
        _storyManagerEventNodes ??= new MergedGroup<IStoryManagerEventNodeGetter>(
            _sourceMods.Select(m => m.StoryManagerEventNodes));
    public IFallout76GroupGetter<IMusicTrackGetter> MusicTracks =>
        _musicTracks ??= new MergedGroup<IMusicTrackGetter>(
            _sourceMods.Select(m => m.MusicTracks));
    public IFallout76GroupGetter<IDialogViewGetter> DialogViews =>
        _dialogViews ??= new MergedGroup<IDialogViewGetter>(
            _sourceMods.Select(m => m.DialogViews));
    public IFallout76GroupGetter<IEquipTypeGetter> EquipTypes =>
        _equipTypes ??= new MergedGroup<IEquipTypeGetter>(
            _sourceMods.Select(m => m.EquipTypes));
    public IFallout76GroupGetter<IRelationshipGetter> Relationships =>
        _relationships ??= new MergedGroup<IRelationshipGetter>(
            _sourceMods.Select(m => m.Relationships));
    public IFallout76GroupGetter<IAssociationTypeGetter> AssociationTypes =>
        _associationTypes ??= new MergedGroup<IAssociationTypeGetter>(
            _sourceMods.Select(m => m.AssociationTypes));
    public IFallout76GroupGetter<IOutfitGetter> Outfits =>
        _outfits ??= new MergedGroup<IOutfitGetter>(
            _sourceMods.Select(m => m.Outfits));
    public IFallout76GroupGetter<IArtObjectGetter> ArtObjects =>
        _artObjects ??= new MergedGroup<IArtObjectGetter>(
            _sourceMods.Select(m => m.ArtObjects));
    public IFallout76GroupGetter<IMaterialObjectGetter> MaterialObjects =>
        _materialObjects ??= new MergedGroup<IMaterialObjectGetter>(
            _sourceMods.Select(m => m.MaterialObjects));
    public IFallout76GroupGetter<IMovementTypeGetter> MovementTypes =>
        _movementTypes ??= new MergedGroup<IMovementTypeGetter>(
            _sourceMods.Select(m => m.MovementTypes));
    public IFallout76GroupGetter<ISoundDescriptorGetter> SoundDescriptors =>
        _soundDescriptors ??= new MergedGroup<ISoundDescriptorGetter>(
            _sourceMods.Select(m => m.SoundDescriptors));
    public IFallout76GroupGetter<ISoundCategoryGetter> SoundCategories =>
        _soundCategories ??= new MergedGroup<ISoundCategoryGetter>(
            _sourceMods.Select(m => m.SoundCategories));
    public IFallout76GroupGetter<ISoundOutputModelGetter> SoundOutputModels =>
        _soundOutputModels ??= new MergedGroup<ISoundOutputModelGetter>(
            _sourceMods.Select(m => m.SoundOutputModels));
    public IFallout76GroupGetter<ICollisionLayerGetter> CollisionLayers =>
        _collisionLayers ??= new MergedGroup<ICollisionLayerGetter>(
            _sourceMods.Select(m => m.CollisionLayers));
    public IFallout76GroupGetter<IColorRecordGetter> Colors =>
        _colors ??= new MergedGroup<IColorRecordGetter>(
            _sourceMods.Select(m => m.Colors));
    public IFallout76GroupGetter<IReverbParametersGetter> ReverbParameters =>
        _reverbParameters ??= new MergedGroup<IReverbParametersGetter>(
            _sourceMods.Select(m => m.ReverbParameters));
    public IFallout76GroupGetter<IPackInGetter> PackIns =>
        _packIns ??= new MergedGroup<IPackInGetter>(
            _sourceMods.Select(m => m.PackIns));
    public IFallout76GroupGetter<IReferenceGroupGetter> ReferenceGroups =>
        _referenceGroups ??= new MergedGroup<IReferenceGroupGetter>(
            _sourceMods.Select(m => m.ReferenceGroups));
    public IFallout76GroupGetter<IAimModelGetter> AimModels =>
        _aimModels ??= new MergedGroup<IAimModelGetter>(
            _sourceMods.Select(m => m.AimModels));
    public IFallout76GroupGetter<ILayerGetter> Layers =>
        _layers ??= new MergedGroup<ILayerGetter>(
            _sourceMods.Select(m => m.Layers));
    public IFallout76GroupGetter<IConstructibleObjectGetter> ConstructibleObjects =>
        _constructibleObjects ??= new MergedGroup<IConstructibleObjectGetter>(
            _sourceMods.Select(m => m.ConstructibleObjects));
    public IFallout76GroupGetter<IAObjectModificationGetter> ObjectModifications =>
        _objectModifications ??= new MergedGroup<IAObjectModificationGetter>(
            _sourceMods.Select(m => m.ObjectModifications));
    public IFallout76GroupGetter<IMaterialSwapGetter> MaterialSwaps =>
        _materialSwaps ??= new MergedGroup<IMaterialSwapGetter>(
            _sourceMods.Select(m => m.MaterialSwaps));
    public IFallout76GroupGetter<IZoomGetter> Zooms =>
        _zooms ??= new MergedGroup<IZoomGetter>(
            _sourceMods.Select(m => m.Zooms));
    public IFallout76GroupGetter<IInstanceNamingRulesGetter> InstanceNamingRules =>
        _instanceNamingRules ??= new MergedGroup<IInstanceNamingRulesGetter>(
            _sourceMods.Select(m => m.InstanceNamingRules));
    public IFallout76GroupGetter<ISoundKeywordMappingGetter> SoundKeywordMappings =>
        _soundKeywordMappings ??= new MergedGroup<ISoundKeywordMappingGetter>(
            _sourceMods.Select(m => m.SoundKeywordMappings));
    public IFallout76GroupGetter<IAudioEffectChainGetter> AudioEffectChains =>
        _audioEffectChains ??= new MergedGroup<IAudioEffectChainGetter>(
            _sourceMods.Select(m => m.AudioEffectChains));
    public IFallout76GroupGetter<ISceneCollectionGetter> SceneCollections =>
        _sceneCollections ??= new MergedGroup<ISceneCollectionGetter>(
            _sourceMods.Select(m => m.SceneCollections));
    public IFallout76GroupGetter<IAttractionRuleGetter> AttractionRules =>
        _attractionRules ??= new MergedGroup<IAttractionRuleGetter>(
            _sourceMods.Select(m => m.AttractionRules));
    public IFallout76GroupGetter<IAudioCategorySnapshotGetter> AudioCategorySnapshots =>
        _audioCategorySnapshots ??= new MergedGroup<IAudioCategorySnapshotGetter>(
            _sourceMods.Select(m => m.AudioCategorySnapshots));
    public IFallout76GroupGetter<IAnimationSoundTagSetGetter> AnimationSoundTagSets =>
        _animationSoundTagSets ??= new MergedGroup<IAnimationSoundTagSetGetter>(
            _sourceMods.Select(m => m.AnimationSoundTagSets));
    public IFallout76GroupGetter<INavigationMeshObstacleManagerGetter> NavigationMeshObstacleManagers =>
        _navigationMeshObstacleManagers ??= new MergedGroup<INavigationMeshObstacleManagerGetter>(
            _sourceMods.Select(m => m.NavigationMeshObstacleManagers));
    public IFallout76GroupGetter<ILensFlareGetter> LensFlares =>
        _lensFlares ??= new MergedGroup<ILensFlareGetter>(
            _sourceMods.Select(m => m.LensFlares));
    public IFallout76GroupGetter<IGodRaysGetter> GodRays =>
        _godRays ??= new MergedGroup<IGodRaysGetter>(
            _sourceMods.Select(m => m.GodRays));
    public IFallout76GroupGetter<IObjectVisibilityManagerGetter> ObjectVisibilityManagers =>
        _objectVisibilityManagers ??= new MergedGroup<IObjectVisibilityManagerGetter>(
            _sourceMods.Select(m => m.ObjectVisibilityManagers));

    BinaryModdedWriteBuilderTargetChoice<IFallout76ModGetter> IFallout76ModGetter.BeginWrite =>
        new BinaryModdedWriteBuilderTargetChoice<IFallout76ModGetter>(this, Fallout76Mod.Fallout76WriteBuilderInstantiator.Instance);

    IBinaryModdedWriteBuilderTargetChoice IModGetter.BeginWrite =>
        new BinaryModdedWriteBuilderTargetChoice<IFallout76ModGetter>(this, Fallout76Mod.Fallout76WriteBuilderInstantiator.Instance);

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

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true)
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var link in mod.EnumerateFormLinks(iterateNestedRecords))
            {
                yield return link;
            }
        }
    }

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

    public IEnumerable<T> EnumerateMajorRecords<T>(bool throwIfUnknown = true) where T : class, IMajorRecordQueryableGetter
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

    IGroupGetter<TMajor>? IModGetter.TryGetTopLevelGroup<TMajor>()
    {
        return (IGroupGetter<TMajor>?)((Fallout76ModCommon)((IFallout76ModGetter)this).CommonInstance()!).GetGroup(
            obj: this,
            type: typeof(TMajor));
    }

    public IGroupGetter? TryGetTopLevelGroup(Type type)
    {
        return (IGroupGetter?)((Fallout76ModCommon)((IFallout76ModGetter)this).CommonInstance()!).GetGroup(
            obj: this,
            type: type);
    }

    IEnumerable<IModContext<IFallout76Mod, IFallout76ModGetter, TSetter, TGetter>> IMajorRecordContextEnumerable<IFallout76Mod, IFallout76ModGetter>.EnumerateMajorRecordContexts<TSetter, TGetter>(ILinkCache linkCache, bool throwIfUnknown)
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var context in mod.EnumerateMajorRecordContexts<TSetter, TGetter>(linkCache, throwIfUnknown))
            {
                yield return context;
            }
        }
    }

    IEnumerable<IModContext<IFallout76Mod, IFallout76ModGetter, IMajorRecord, IMajorRecordGetter>> IMajorRecordContextEnumerable<IFallout76Mod, IFallout76ModGetter>.EnumerateMajorRecordContexts(ILinkCache linkCache, Type type, bool throwIfUnknown)
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var context in mod.EnumerateMajorRecordContexts(linkCache, type, throwIfUnknown))
            {
                yield return context;
            }
        }
    }

    public IEnumerable<IModContext<IMajorRecordGetter>> EnumerateMajorRecordSimpleContexts()
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var context in mod.EnumerateMajorRecordSimpleContexts())
            {
                yield return context;
            }
        }
    }

    public IEnumerable<IModContext<TGetter>> EnumerateMajorRecordSimpleContexts<TGetter>(bool throwIfUnknown = true) where TGetter : class, IMajorRecordQueryableGetter
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var context in mod.EnumerateMajorRecordSimpleContexts<TGetter>(throwIfUnknown))
            {
                yield return context;
            }
        }
    }

    public IEnumerable<IModContext<IMajorRecordGetter>> EnumerateMajorRecordSimpleContexts(Type type, bool throwIfUnknown = true)
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var context in mod.EnumerateMajorRecordSimpleContexts(type, throwIfUnknown))
            {
                yield return context;
            }
        }
    }

    public void WriteToBinary(FilePath path, BinaryWriteParameters? param = null) => this.WriteToBinary(path, importMask: null, param: param);

    public void WriteToBinary(Stream stream, BinaryWriteParameters? param = null) => this.WriteToBinary(stream, importMask: null, param: param);

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
        return ((Fallout76ModSetterTranslationCommon)((IFallout76ModGetter)this).CommonSetterTranslationInstance()!).DeepCopy(
            item: this,
            errorMask: null,
            copyMask: null);
    }

    #pragma warning disable CS8603 // Possible null reference return
    IReadOnlyList<IFormLinkGetter<IMajorRecordGetter>> IModGetter.OverriddenForms
    {
        get => _sourceMods.SelectMany(m => m.OverriddenForms).ToList();
    }
    #pragma warning restore CS8603

    public uint NextFormID => _sourceMods.Max(m => m.NextFormID);

    public ILoquiRegistration Registration => Fallout76Mod_Registration.Instance;

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        throw new NotSupportedException("Multi-mod overlay does not support printing.");
    }

    public IMask<bool> GetEqualsMask(object rhs, EqualsMaskHelper.Include include)
    {
        throw new NotSupportedException("Multi-mod overlay does not support equality masking.");
    }

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

    // IDisposable
    public void Dispose()
    {
        if (_disposeSourceMods == null) return;

        foreach (var mod in _disposeSourceMods)
        {
            mod.Dispose();
        }
    }
}
/// <summary>
/// Merged group that combines multiple groups into a single unified view.
/// Validates no duplicate FormKeys exist and caches results.
/// </summary>
internal class MergedGroup<TGetter> : IFallout76GroupGetter<TGetter>, IReadOnlyCache<TGetter, FormKey>
    where TGetter : class, IFallout76MajorRecordGetter, IBinaryItem
{
    private readonly IEnumerable<IGroupGetter<TGetter>> _sourceGroups;
    private Dictionary<FormKey, TGetter>? _cache;
    private readonly object _cacheLock = new object();

    public MergedGroup(IEnumerable<IGroupGetter<TGetter>> sourceGroups)
    {
        _sourceGroups = sourceGroups;
    }

    private Dictionary<FormKey, TGetter> Cache
    {
        get
        {
            if (_cache != null) return _cache;

            lock (_cacheLock)
            {
                if (_cache != null) return _cache;

                var cache = new Dictionary<FormKey, TGetter>();
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

    public IEnumerator<TGetter> GetEnumerator() => Cache.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => Cache.Count;

    public bool TryGetValue(FormKey key, [MaybeNullWhen(false)] out TGetter value)
    {
        return Cache.TryGetValue(key, out value);
    }

    public TGetter this[FormKey key] => Cache[key];
    IMajorRecordGetter IGroupGetter.this[FormKey key] => this[key];

    public bool ContainsKey(FormKey key) => Cache.ContainsKey(key);
    public IEnumerable<FormKey> FormKeys => Cache.Keys;
    public Type Type => typeof(TGetter);
    public IEnumerable<TGetter> Records => Cache.Values;

    IMod IGroupGetter.SourceMod => throw new NotSupportedException("Merged group has multiple source mods, not a single source.");
    IEnumerable<TGetter> IGroupCommonGetter<TGetter>.Records => Cache.Values;
    IEnumerable<ILoquiObject> IGroupCommonGetter.Records => Cache.Values;
    IEnumerable<IMajorRecordGetter> IGroupGetter.Records => Cache.Values.Cast<IMajorRecordGetter>();
    IReadOnlyCache<IMajorRecordGetter, FormKey> IGroupGetter.RecordCache => new MajorRecordCacheWrapper(this);

    public ILoquiRegistration ContainedRecordRegistration => _sourceGroups.First().ContainedRecordRegistration;
    public Type ContainedRecordType => typeof(TGetter);

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

    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
    {
        throw new NotSupportedException("Merged groups cannot be written to binary. Write the source mods individually.");
    }

    object IBinaryItem.BinaryWriteTranslator => throw new NotSupportedException("Merged groups do not support binary writing.");

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true)
    {
        foreach (var record in Cache.Values)
        {
            if (record is IFormLinkContainerGetter formLinkContainer)
            {
                foreach (var link in formLinkContainer.EnumerateFormLinks(iterateNestedRecords))
                {
                    yield return link;
                }
            }
        }
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
    {
        return Cache.Values;
    }

    IEnumerable<T> IMajorRecordGetterEnumerable.EnumerateMajorRecords<T>(bool throwIfUnknown)
    {
        return Cache.Values.WhereCastable<TGetter, T>();
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
    {
        return Cache.Values.Where(r => type.IsAssignableFrom(r.GetType()));
    }

    // IFallout76GroupGetter members
    public object CommonInstance(Type type) => GenericCommonInstanceGetter.Get(Fallout76GroupCommon<TGetter>.Instance, typeof(ICellBlockGetter), type);
    public object? CommonSetterInstance(Type type) => null;
    public object CommonSetterTranslationInstance() => Fallout76GroupSetterTranslationCommon.Instance;

    GroupTypeEnum IFallout76GroupGetter<TGetter>.Type => GroupTypeEnum.Type;
    int IFallout76GroupGetter<TGetter>.LastModified => 0;
    public int Unknown => 0;

    IReadOnlyCache<TGetter, FormKey> IFallout76GroupGetter<TGetter>.RecordCache => this;
    IReadOnlyCache<TGetter, FormKey> IGroupGetter<TGetter>.RecordCache => this;

    // IReadOnlyCache explicit implementations
    IEnumerable<FormKey> IReadOnlyCache<TGetter, FormKey>.Keys => Cache.Keys;
    IEnumerable<TGetter> IReadOnlyCache<TGetter, FormKey>.Items => Cache.Values;

    TGetter? IReadOnlyCache<TGetter, FormKey>.TryGetValue(FormKey key)
    {
        return TryGetValue(key, out var value) ? value : null;
    }

    IEnumerator<IKeyValue<FormKey, TGetter>> IEnumerable<IKeyValue<FormKey, TGetter>>.GetEnumerator()
    {
        return Cache.Select(kvp => (IKeyValue<FormKey, TGetter>)new KeyValue<FormKey, TGetter>(kvp.Key, kvp.Value)).GetEnumerator();
    }

    // Wrapper to cast TGetter to IMajorRecordGetter for IGroupGetter.RecordCache
    private class MajorRecordCacheWrapper : IReadOnlyCache<IMajorRecordGetter, FormKey>
    {
        private readonly MergedGroup<TGetter> _source;

        public MajorRecordCacheWrapper(MergedGroup<TGetter> source)
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

    // ILoquiObject
    ILoquiRegistration ILoquiObject.Registration => null!;

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        Fallout76GroupMixIn.Print(
            item: this,
            sb: sb,
            name: name);
    }
}

/// <summary>
/// Merged list group that combines multiple list groups (like Cells) into a single unified view.
/// CellBlocks from different mods are merged by BlockNumber.
/// </summary>
internal class MergedListGroup : IFallout76ListGroupGetter<ICellBlockGetter>
{
    private readonly IEnumerable<IFallout76ListGroupGetter<ICellBlockGetter>> _sourceGroups;
    private List<ICellBlockGetter>? _cache;
    private readonly object _cacheLock = new object();

    public MergedListGroup(IEnumerable<IFallout76ListGroupGetter<ICellBlockGetter>> sourceGroups)
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

                // Create merged blocks
                var result = new List<ICellBlockGetter>();
                foreach (var blockNumber in blocksByNumber.Keys.OrderBy(k => k))
                {
                    var blocksForNumber = blocksByNumber[blockNumber];
                    if (blocksForNumber.Count == 1)
                    {
                        result.Add(blocksForNumber[0]);
                    }
                    else
                    {
                        // Multiple blocks with same number - merge them
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
    public ICellBlockGetter this[int index] => Cache[index];
    public IReadOnlyList<ICellBlockGetter> Records => Cache;

    ILoquiRegistration ILoquiObject.Registration => Fallout76Mod_Registration.Instance;
    public static ILoquiRegistration StaticRegistration => Fallout76Mod_Registration.Instance;

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        Fallout76ListGroupMixIn.Print(
            item: this,
            sb: sb,
            name: name);
    }

    // IFallout76ListGroupGetter properties
    public GroupTypeEnum Type => _sourceGroups.FirstOrDefault()?.Type ?? GroupTypeEnum.InteriorCellBlock;
    public int LastModified => _sourceGroups.Max(g => g.LastModified);
    public int Unknown => 0;

    public object CommonInstance(Type type) => GenericCommonInstanceGetter.Get(Fallout76ListGroupCommon<ICellBlockGetter>.Instance, typeof(ICellBlockGetter), type);
    public object? CommonSetterInstance(Type type) => GenericCommonInstanceGetter.Get(Fallout76ListGroupSetterCommon<ICellBlock>.Instance, typeof(ICellBlockGetter), type);
    public object CommonSetterTranslationInstance() => Fallout76ListGroupSetterTranslationCommon.Instance;

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

    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
    {
        throw new NotSupportedException("Merged groups cannot be written to binary. Write the source mods individually.");
    }

    object IBinaryItem.BinaryWriteTranslator => throw new NotSupportedException("Merged groups do not support binary writing.");

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true)
    {
        foreach (var block in Cache)
        {
            if (block is IFormLinkContainerGetter formLinkContainer)
            {
                foreach (var link in formLinkContainer.EnumerateFormLinks(iterateNestedRecords))
                {
                    yield return link;
                }
            }
        }
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
    {
        foreach (var block in Cache)
        {
            if (block is IMajorRecordGetterEnumerable enumerable)
            {
                foreach (var record in enumerable.EnumerateMajorRecords())
                {
                    yield return record;
                }
            }
        }
    }

    IEnumerable<T> IMajorRecordGetterEnumerable.EnumerateMajorRecords<T>(bool throwIfUnknown)
    {
        foreach (var block in Cache)
        {
            if (block is IMajorRecordGetterEnumerable enumerable)
            {
                foreach (var record in enumerable.EnumerateMajorRecords<T>(throwIfUnknown))
                {
                    yield return record;
                }
            }
        }
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
    {
        foreach (var block in Cache)
        {
            if (block is IMajorRecordGetterEnumerable enumerable)
            {
                foreach (var record in enumerable.EnumerateMajorRecords(type, throwIfUnknown))
                {
                    yield return record;
                }
            }
        }
    }
}

/// <summary>
/// Merged cell block that combines multiple cell blocks with the same BlockNumber.
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
    public int Unknown => 0;

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

    ILoquiRegistration ILoquiObject.Registration => null!;

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        sb.AppendLine($"Merged Cell Block {BlockNumber} ({SubBlocks.Count} sub-blocks from {_sourceBlocks.Count} source blocks)");
    }

    public object CommonInstance() => CellBlockCommon.Instance;
    public object? CommonSetterInstance() => null;
    public object CommonSetterTranslationInstance() => CellBlockSetterTranslationCommon.Instance;

    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
    {
        throw new NotSupportedException("Merged cell blocks cannot be written to binary.");
    }

    object IBinaryItem.BinaryWriteTranslator => throw new NotSupportedException("Merged cell blocks do not support binary writing.");

    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
    {
        foreach (var subBlock in SubBlocks)
        {
            if (subBlock is IAssetLinkContainerGetter assetContainer)
            {
                foreach (var link in assetContainer.EnumerateAssetLinks(queryCategories, linkCache, assetType))
                {
                    yield return link;
                }
            }
        }
    }

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true)
    {
        foreach (var subBlock in SubBlocks)
        {
            if (subBlock is IFormLinkContainerGetter formLinkContainer)
            {
                foreach (var link in formLinkContainer.EnumerateFormLinks(iterateNestedRecords))
                {
                    yield return link;
                }
            }
        }
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
    {
        foreach (var subBlock in SubBlocks)
        {
            if (subBlock is IMajorRecordGetterEnumerable enumerable)
            {
                foreach (var record in enumerable.EnumerateMajorRecords())
                {
                    yield return record;
                }
            }
        }
    }

    public IEnumerable<T> EnumerateMajorRecords<T>(bool throwIfUnknown = true) where T : class, IMajorRecordQueryableGetter
    {
        foreach (var subBlock in SubBlocks)
        {
            if (subBlock is IMajorRecordGetterEnumerable enumerable)
            {
                foreach (var record in enumerable.EnumerateMajorRecords<T>(throwIfUnknown))
                {
                    yield return record;
                }
            }
        }
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
    {
        foreach (var subBlock in SubBlocks)
        {
            if (subBlock is IMajorRecordGetterEnumerable enumerable)
            {
                foreach (var record in enumerable.EnumerateMajorRecords(type, throwIfUnknown))
                {
                    yield return record;
                }
            }
        }
    }
}

