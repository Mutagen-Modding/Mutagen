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
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Plugins.Aspects;

namespace Mutagen.Bethesda.Fallout4;

/// <summary>
/// Multi-mod overlay that presents multiple Fallout4 mods as a single unified mod.
/// Typically used for reading split mods that were written due to exceeding master limits
/// </summary>
internal class Fallout4MultiModOverlay : IFallout4ModDisposableGetter
{
    private readonly IReadOnlyList<IFallout4ModGetter> _sourceMods;
    private readonly IReadOnlyList<IModDisposeGetter>? _disposeSourceMods;
    private readonly ModKey _modKey;
    private readonly IReadOnlyList<IMasterReferenceGetter> _masters;
    private readonly MergedFallout4ModHeader _modHeader;

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
    /// Creates a new Fallout4MultiModOverlay from multiple source mod files.
    /// </summary>
    public Fallout4MultiModOverlay(
        ModKey modKey,
        IEnumerable<IFallout4ModGetter> sourceMods,
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

        _modHeader = new MergedFallout4ModHeader(sourceList.Select(s => s.ModHeader).ToList(), mergedMasters);
    }

    public ModKey ModKey => _modKey;
    public IFallout4ModHeaderGetter ModHeader => _modHeader;
    public IReadOnlyList<IMasterReferenceGetter> MasterReferences => _masters;
    public Fallout4Release Fallout4Release => _sourceMods[0].Fallout4Release;
    GameRelease IModGetter.GameRelease => Fallout4Release.ToGameRelease();

    public object CommonInstance() => Fallout4ModCommon.Instance;
    public object? CommonSetterInstance() => Fallout4ModSetterCommon.Instance;
    public object CommonSetterTranslationInstance() => Fallout4ModSetterTranslationCommon.Instance;


    public IFallout4GroupGetter<IGameSettingGetter> GameSettings =>
        _gameSettings ??= new MergedGroup<IGameSettingGetter>(
            _sourceMods.Select(m => m.GameSettings));
    public IFallout4GroupGetter<IKeywordGetter> Keywords =>
        _keywords ??= new MergedGroup<IKeywordGetter>(
            _sourceMods.Select(m => m.Keywords));
    public IFallout4GroupGetter<ILocationReferenceTypeGetter> LocationReferenceTypes =>
        _locationReferenceTypes ??= new MergedGroup<ILocationReferenceTypeGetter>(
            _sourceMods.Select(m => m.LocationReferenceTypes));
    public IFallout4GroupGetter<IActionRecordGetter> Actions =>
        _actions ??= new MergedGroup<IActionRecordGetter>(
            _sourceMods.Select(m => m.Actions));
    public IFallout4GroupGetter<ITransformGetter> Transforms =>
        _transforms ??= new MergedGroup<ITransformGetter>(
            _sourceMods.Select(m => m.Transforms));
    public IFallout4GroupGetter<IComponentGetter> Components =>
        _components ??= new MergedGroup<IComponentGetter>(
            _sourceMods.Select(m => m.Components));
    public IFallout4GroupGetter<ITextureSetGetter> TextureSets =>
        _textureSets ??= new MergedGroup<ITextureSetGetter>(
            _sourceMods.Select(m => m.TextureSets));
    public IFallout4GroupGetter<IGlobalGetter> Globals =>
        _globals ??= new MergedGroup<IGlobalGetter>(
            _sourceMods.Select(m => m.Globals));
    public IFallout4GroupGetter<IADamageTypeGetter> DamageTypes =>
        _damageTypes ??= new MergedGroup<IADamageTypeGetter>(
            _sourceMods.Select(m => m.DamageTypes));
    public IFallout4GroupGetter<IClassGetter> Classes =>
        _classes ??= new MergedGroup<IClassGetter>(
            _sourceMods.Select(m => m.Classes));
    public IFallout4GroupGetter<IFactionGetter> Factions =>
        _factions ??= new MergedGroup<IFactionGetter>(
            _sourceMods.Select(m => m.Factions));
    public IFallout4GroupGetter<IHeadPartGetter> HeadParts =>
        _headParts ??= new MergedGroup<IHeadPartGetter>(
            _sourceMods.Select(m => m.HeadParts));
    public IFallout4GroupGetter<IRaceGetter> Races =>
        _races ??= new MergedGroup<IRaceGetter>(
            _sourceMods.Select(m => m.Races));
    public IFallout4GroupGetter<ISoundMarkerGetter> SoundMarkers =>
        _soundMarkers ??= new MergedGroup<ISoundMarkerGetter>(
            _sourceMods.Select(m => m.SoundMarkers));
    public IFallout4GroupGetter<IAcousticSpaceGetter> AcousticSpaces =>
        _acousticSpaces ??= new MergedGroup<IAcousticSpaceGetter>(
            _sourceMods.Select(m => m.AcousticSpaces));
    public IFallout4GroupGetter<IMagicEffectGetter> MagicEffects =>
        _magicEffects ??= new MergedGroup<IMagicEffectGetter>(
            _sourceMods.Select(m => m.MagicEffects));
    public IFallout4GroupGetter<ILandscapeTextureGetter> LandscapeTextures =>
        _landscapeTextures ??= new MergedGroup<ILandscapeTextureGetter>(
            _sourceMods.Select(m => m.LandscapeTextures));
    public IFallout4GroupGetter<IObjectEffectGetter> ObjectEffects =>
        _objectEffects ??= new MergedGroup<IObjectEffectGetter>(
            _sourceMods.Select(m => m.ObjectEffects));
    public IFallout4GroupGetter<ISpellGetter> Spells =>
        _spells ??= new MergedGroup<ISpellGetter>(
            _sourceMods.Select(m => m.Spells));
    public IFallout4GroupGetter<IActivatorGetter> Activators =>
        _activators ??= new MergedGroup<IActivatorGetter>(
            _sourceMods.Select(m => m.Activators));
    public IFallout4GroupGetter<ITalkingActivatorGetter> TalkingActivators =>
        _talkingActivators ??= new MergedGroup<ITalkingActivatorGetter>(
            _sourceMods.Select(m => m.TalkingActivators));
    public IFallout4GroupGetter<IArmorGetter> Armors =>
        _armors ??= new MergedGroup<IArmorGetter>(
            _sourceMods.Select(m => m.Armors));
    public IFallout4GroupGetter<IBookGetter> Books =>
        _books ??= new MergedGroup<IBookGetter>(
            _sourceMods.Select(m => m.Books));
    public IFallout4GroupGetter<IContainerGetter> Containers =>
        _containers ??= new MergedGroup<IContainerGetter>(
            _sourceMods.Select(m => m.Containers));
    public IFallout4GroupGetter<IDoorGetter> Doors =>
        _doors ??= new MergedGroup<IDoorGetter>(
            _sourceMods.Select(m => m.Doors));
    public IFallout4GroupGetter<IIngredientGetter> Ingredients =>
        _ingredients ??= new MergedGroup<IIngredientGetter>(
            _sourceMods.Select(m => m.Ingredients));
    public IFallout4GroupGetter<ILightGetter> Lights =>
        _lights ??= new MergedGroup<ILightGetter>(
            _sourceMods.Select(m => m.Lights));
    public IFallout4GroupGetter<IMiscItemGetter> MiscItems =>
        _miscItems ??= new MergedGroup<IMiscItemGetter>(
            _sourceMods.Select(m => m.MiscItems));
    public IFallout4GroupGetter<IStaticGetter> Statics =>
        _statics ??= new MergedGroup<IStaticGetter>(
            _sourceMods.Select(m => m.Statics));
    public IFallout4GroupGetter<IStaticCollectionGetter> StaticCollections =>
        _staticCollections ??= new MergedGroup<IStaticCollectionGetter>(
            _sourceMods.Select(m => m.StaticCollections));
    public IFallout4GroupGetter<IMovableStaticGetter> MovableStatics =>
        _movableStatics ??= new MergedGroup<IMovableStaticGetter>(
            _sourceMods.Select(m => m.MovableStatics));
    public IFallout4GroupGetter<IGrassGetter> Grasses =>
        _grasses ??= new MergedGroup<IGrassGetter>(
            _sourceMods.Select(m => m.Grasses));
    public IFallout4GroupGetter<ITreeGetter> Trees =>
        _trees ??= new MergedGroup<ITreeGetter>(
            _sourceMods.Select(m => m.Trees));
    public IFallout4GroupGetter<IFloraGetter> Florae =>
        _florae ??= new MergedGroup<IFloraGetter>(
            _sourceMods.Select(m => m.Florae));
    public IFallout4GroupGetter<IFurnitureGetter> Furniture =>
        _furniture ??= new MergedGroup<IFurnitureGetter>(
            _sourceMods.Select(m => m.Furniture));
    public IFallout4GroupGetter<IWeaponGetter> Weapons =>
        _weapons ??= new MergedGroup<IWeaponGetter>(
            _sourceMods.Select(m => m.Weapons));
    public IFallout4GroupGetter<IAmmunitionGetter> Ammunitions =>
        _ammunitions ??= new MergedGroup<IAmmunitionGetter>(
            _sourceMods.Select(m => m.Ammunitions));
    public IFallout4GroupGetter<INpcGetter> Npcs =>
        _npcs ??= new MergedGroup<INpcGetter>(
            _sourceMods.Select(m => m.Npcs));
    public IFallout4GroupGetter<ILeveledNpcGetter> LeveledNpcs =>
        _leveledNpcs ??= new MergedGroup<ILeveledNpcGetter>(
            _sourceMods.Select(m => m.LeveledNpcs));
    public IFallout4GroupGetter<IKeyGetter> Keys =>
        _keys ??= new MergedGroup<IKeyGetter>(
            _sourceMods.Select(m => m.Keys));
    public IFallout4GroupGetter<IIngestibleGetter> Ingestibles =>
        _ingestibles ??= new MergedGroup<IIngestibleGetter>(
            _sourceMods.Select(m => m.Ingestibles));
    public IFallout4GroupGetter<IIdleMarkerGetter> IdleMarkers =>
        _idleMarkers ??= new MergedGroup<IIdleMarkerGetter>(
            _sourceMods.Select(m => m.IdleMarkers));
    public IFallout4GroupGetter<IHolotapeGetter> Holotapes =>
        _holotapes ??= new MergedGroup<IHolotapeGetter>(
            _sourceMods.Select(m => m.Holotapes));
    public IFallout4GroupGetter<IProjectileGetter> Projectiles =>
        _projectiles ??= new MergedGroup<IProjectileGetter>(
            _sourceMods.Select(m => m.Projectiles));
    public IFallout4GroupGetter<IHazardGetter> Hazards =>
        _hazards ??= new MergedGroup<IHazardGetter>(
            _sourceMods.Select(m => m.Hazards));
    public IFallout4GroupGetter<IBendableSplineGetter> BendableSplines =>
        _bendableSplines ??= new MergedGroup<IBendableSplineGetter>(
            _sourceMods.Select(m => m.BendableSplines));
    public IFallout4GroupGetter<ITerminalGetter> Terminals =>
        _terminals ??= new MergedGroup<ITerminalGetter>(
            _sourceMods.Select(m => m.Terminals));
    public IFallout4GroupGetter<ILeveledItemGetter> LeveledItems =>
        _leveledItems ??= new MergedGroup<ILeveledItemGetter>(
            _sourceMods.Select(m => m.LeveledItems));
    public IFallout4GroupGetter<IWeatherGetter> Weather =>
        _weather ??= new MergedGroup<IWeatherGetter>(
            _sourceMods.Select(m => m.Weather));
    public IFallout4GroupGetter<IClimateGetter> Climates =>
        _climates ??= new MergedGroup<IClimateGetter>(
            _sourceMods.Select(m => m.Climates));
    public IFallout4GroupGetter<IShaderParticleGeometryGetter> ShaderParticleGeometries =>
        _shaderParticleGeometries ??= new MergedGroup<IShaderParticleGeometryGetter>(
            _sourceMods.Select(m => m.ShaderParticleGeometries));
    public IFallout4GroupGetter<IVisualEffectGetter> VisualEffects =>
        _visualEffects ??= new MergedGroup<IVisualEffectGetter>(
            _sourceMods.Select(m => m.VisualEffects));
    public IFallout4GroupGetter<IRegionGetter> Regions =>
        _regions ??= new MergedGroup<IRegionGetter>(
            _sourceMods.Select(m => m.Regions));
    public IFallout4GroupGetter<INavigationMeshInfoMapGetter> NavigationMeshInfoMaps =>
        _navigationMeshInfoMaps ??= new MergedGroup<INavigationMeshInfoMapGetter>(
            _sourceMods.Select(m => m.NavigationMeshInfoMaps));
    public IFallout4ListGroupGetter<ICellBlockGetter> Cells =>
        _cells ??= new MergedListGroup(_sourceMods.Select(m => m.Cells));
    public IFallout4GroupGetter<IWorldspaceGetter> Worldspaces =>
        _worldspaces ??= new MergedGroup<IWorldspaceGetter>(
            _sourceMods.Select(m => m.Worldspaces), allowDuplicateOverrides: true,
            duplicateMerger: MergedWorldspace.Merge);
    public IFallout4GroupGetter<IQuestGetter> Quests =>
        _quests ??= new MergedGroup<IQuestGetter>(
            _sourceMods.Select(m => m.Quests), allowDuplicateOverrides: true);
    public IFallout4GroupGetter<IIdleAnimationGetter> IdleAnimations =>
        _idleAnimations ??= new MergedGroup<IIdleAnimationGetter>(
            _sourceMods.Select(m => m.IdleAnimations));
    public IFallout4GroupGetter<IPackageGetter> Packages =>
        _packages ??= new MergedGroup<IPackageGetter>(
            _sourceMods.Select(m => m.Packages));
    public IFallout4GroupGetter<ICombatStyleGetter> CombatStyles =>
        _combatStyles ??= new MergedGroup<ICombatStyleGetter>(
            _sourceMods.Select(m => m.CombatStyles));
    public IFallout4GroupGetter<ILoadScreenGetter> LoadScreens =>
        _loadScreens ??= new MergedGroup<ILoadScreenGetter>(
            _sourceMods.Select(m => m.LoadScreens));
    public IFallout4GroupGetter<IAnimatedObjectGetter> AnimatedObjects =>
        _animatedObjects ??= new MergedGroup<IAnimatedObjectGetter>(
            _sourceMods.Select(m => m.AnimatedObjects));
    public IFallout4GroupGetter<IWaterGetter> Waters =>
        _waters ??= new MergedGroup<IWaterGetter>(
            _sourceMods.Select(m => m.Waters));
    public IFallout4GroupGetter<IEffectShaderGetter> EffectShaders =>
        _effectShaders ??= new MergedGroup<IEffectShaderGetter>(
            _sourceMods.Select(m => m.EffectShaders));
    public IFallout4GroupGetter<IExplosionGetter> Explosions =>
        _explosions ??= new MergedGroup<IExplosionGetter>(
            _sourceMods.Select(m => m.Explosions));
    public IFallout4GroupGetter<IDebrisGetter> Debris =>
        _debris ??= new MergedGroup<IDebrisGetter>(
            _sourceMods.Select(m => m.Debris));
    public IFallout4GroupGetter<IImageSpaceGetter> ImageSpaces =>
        _imageSpaces ??= new MergedGroup<IImageSpaceGetter>(
            _sourceMods.Select(m => m.ImageSpaces));
    public IFallout4GroupGetter<IImageSpaceAdapterGetter> ImageSpaceAdapters =>
        _imageSpaceAdapters ??= new MergedGroup<IImageSpaceAdapterGetter>(
            _sourceMods.Select(m => m.ImageSpaceAdapters));
    public IFallout4GroupGetter<IFormListGetter> FormLists =>
        _formLists ??= new MergedGroup<IFormListGetter>(
            _sourceMods.Select(m => m.FormLists));
    public IFallout4GroupGetter<IPerkGetter> Perks =>
        _perks ??= new MergedGroup<IPerkGetter>(
            _sourceMods.Select(m => m.Perks));
    public IFallout4GroupGetter<IBodyPartDataGetter> BodyParts =>
        _bodyParts ??= new MergedGroup<IBodyPartDataGetter>(
            _sourceMods.Select(m => m.BodyParts));
    public IFallout4GroupGetter<IAddonNodeGetter> AddonNodes =>
        _addonNodes ??= new MergedGroup<IAddonNodeGetter>(
            _sourceMods.Select(m => m.AddonNodes));
    public IFallout4GroupGetter<IActorValueInformationGetter> ActorValueInformation =>
        _actorValueInformation ??= new MergedGroup<IActorValueInformationGetter>(
            _sourceMods.Select(m => m.ActorValueInformation));
    public IFallout4GroupGetter<ICameraShotGetter> CameraShots =>
        _cameraShots ??= new MergedGroup<ICameraShotGetter>(
            _sourceMods.Select(m => m.CameraShots));
    public IFallout4GroupGetter<ICameraPathGetter> CameraPaths =>
        _cameraPaths ??= new MergedGroup<ICameraPathGetter>(
            _sourceMods.Select(m => m.CameraPaths));
    public IFallout4GroupGetter<IVoiceTypeGetter> VoiceTypes =>
        _voiceTypes ??= new MergedGroup<IVoiceTypeGetter>(
            _sourceMods.Select(m => m.VoiceTypes));
    public IFallout4GroupGetter<IMaterialTypeGetter> MaterialTypes =>
        _materialTypes ??= new MergedGroup<IMaterialTypeGetter>(
            _sourceMods.Select(m => m.MaterialTypes));
    public IFallout4GroupGetter<IImpactGetter> Impacts =>
        _impacts ??= new MergedGroup<IImpactGetter>(
            _sourceMods.Select(m => m.Impacts));
    public IFallout4GroupGetter<IImpactDataSetGetter> ImpactDataSets =>
        _impactDataSets ??= new MergedGroup<IImpactDataSetGetter>(
            _sourceMods.Select(m => m.ImpactDataSets));
    public IFallout4GroupGetter<IArmorAddonGetter> ArmorAddons =>
        _armorAddons ??= new MergedGroup<IArmorAddonGetter>(
            _sourceMods.Select(m => m.ArmorAddons));
    public IFallout4GroupGetter<IEncounterZoneGetter> EncounterZones =>
        _encounterZones ??= new MergedGroup<IEncounterZoneGetter>(
            _sourceMods.Select(m => m.EncounterZones));
    public IFallout4GroupGetter<ILocationGetter> Locations =>
        _locations ??= new MergedGroup<ILocationGetter>(
            _sourceMods.Select(m => m.Locations));
    public IFallout4GroupGetter<IMessageGetter> Messages =>
        _messages ??= new MergedGroup<IMessageGetter>(
            _sourceMods.Select(m => m.Messages));
    public IFallout4GroupGetter<IDefaultObjectManagerGetter> DefaultObjectManagers =>
        _defaultObjectManagers ??= new MergedGroup<IDefaultObjectManagerGetter>(
            _sourceMods.Select(m => m.DefaultObjectManagers));
    public IFallout4GroupGetter<IDefaultObjectGetter> DefaultObjects =>
        _defaultObjects ??= new MergedGroup<IDefaultObjectGetter>(
            _sourceMods.Select(m => m.DefaultObjects));
    public IFallout4GroupGetter<ILightingTemplateGetter> LightingTemplates =>
        _lightingTemplates ??= new MergedGroup<ILightingTemplateGetter>(
            _sourceMods.Select(m => m.LightingTemplates));
    public IFallout4GroupGetter<IMusicTypeGetter> MusicTypes =>
        _musicTypes ??= new MergedGroup<IMusicTypeGetter>(
            _sourceMods.Select(m => m.MusicTypes));
    public IFallout4GroupGetter<IFootstepGetter> Footsteps =>
        _footsteps ??= new MergedGroup<IFootstepGetter>(
            _sourceMods.Select(m => m.Footsteps));
    public IFallout4GroupGetter<IFootstepSetGetter> FootstepSets =>
        _footstepSets ??= new MergedGroup<IFootstepSetGetter>(
            _sourceMods.Select(m => m.FootstepSets));
    public IFallout4GroupGetter<IStoryManagerBranchNodeGetter> StoryManagerBranchNodes =>
        _storyManagerBranchNodes ??= new MergedGroup<IStoryManagerBranchNodeGetter>(
            _sourceMods.Select(m => m.StoryManagerBranchNodes));
    public IFallout4GroupGetter<IStoryManagerQuestNodeGetter> StoryManagerQuestNodes =>
        _storyManagerQuestNodes ??= new MergedGroup<IStoryManagerQuestNodeGetter>(
            _sourceMods.Select(m => m.StoryManagerQuestNodes));
    public IFallout4GroupGetter<IStoryManagerEventNodeGetter> StoryManagerEventNodes =>
        _storyManagerEventNodes ??= new MergedGroup<IStoryManagerEventNodeGetter>(
            _sourceMods.Select(m => m.StoryManagerEventNodes));
    public IFallout4GroupGetter<IMusicTrackGetter> MusicTracks =>
        _musicTracks ??= new MergedGroup<IMusicTrackGetter>(
            _sourceMods.Select(m => m.MusicTracks));
    public IFallout4GroupGetter<IDialogViewGetter> DialogViews =>
        _dialogViews ??= new MergedGroup<IDialogViewGetter>(
            _sourceMods.Select(m => m.DialogViews));
    public IFallout4GroupGetter<IEquipTypeGetter> EquipTypes =>
        _equipTypes ??= new MergedGroup<IEquipTypeGetter>(
            _sourceMods.Select(m => m.EquipTypes));
    public IFallout4GroupGetter<IRelationshipGetter> Relationships =>
        _relationships ??= new MergedGroup<IRelationshipGetter>(
            _sourceMods.Select(m => m.Relationships));
    public IFallout4GroupGetter<IAssociationTypeGetter> AssociationTypes =>
        _associationTypes ??= new MergedGroup<IAssociationTypeGetter>(
            _sourceMods.Select(m => m.AssociationTypes));
    public IFallout4GroupGetter<IOutfitGetter> Outfits =>
        _outfits ??= new MergedGroup<IOutfitGetter>(
            _sourceMods.Select(m => m.Outfits));
    public IFallout4GroupGetter<IArtObjectGetter> ArtObjects =>
        _artObjects ??= new MergedGroup<IArtObjectGetter>(
            _sourceMods.Select(m => m.ArtObjects));
    public IFallout4GroupGetter<IMaterialObjectGetter> MaterialObjects =>
        _materialObjects ??= new MergedGroup<IMaterialObjectGetter>(
            _sourceMods.Select(m => m.MaterialObjects));
    public IFallout4GroupGetter<IMovementTypeGetter> MovementTypes =>
        _movementTypes ??= new MergedGroup<IMovementTypeGetter>(
            _sourceMods.Select(m => m.MovementTypes));
    public IFallout4GroupGetter<ISoundDescriptorGetter> SoundDescriptors =>
        _soundDescriptors ??= new MergedGroup<ISoundDescriptorGetter>(
            _sourceMods.Select(m => m.SoundDescriptors));
    public IFallout4GroupGetter<ISoundCategoryGetter> SoundCategories =>
        _soundCategories ??= new MergedGroup<ISoundCategoryGetter>(
            _sourceMods.Select(m => m.SoundCategories));
    public IFallout4GroupGetter<ISoundOutputModelGetter> SoundOutputModels =>
        _soundOutputModels ??= new MergedGroup<ISoundOutputModelGetter>(
            _sourceMods.Select(m => m.SoundOutputModels));
    public IFallout4GroupGetter<ICollisionLayerGetter> CollisionLayers =>
        _collisionLayers ??= new MergedGroup<ICollisionLayerGetter>(
            _sourceMods.Select(m => m.CollisionLayers));
    public IFallout4GroupGetter<IColorRecordGetter> Colors =>
        _colors ??= new MergedGroup<IColorRecordGetter>(
            _sourceMods.Select(m => m.Colors));
    public IFallout4GroupGetter<IReverbParametersGetter> ReverbParameters =>
        _reverbParameters ??= new MergedGroup<IReverbParametersGetter>(
            _sourceMods.Select(m => m.ReverbParameters));
    public IFallout4GroupGetter<IPackInGetter> PackIns =>
        _packIns ??= new MergedGroup<IPackInGetter>(
            _sourceMods.Select(m => m.PackIns));
    public IFallout4GroupGetter<IReferenceGroupGetter> ReferenceGroups =>
        _referenceGroups ??= new MergedGroup<IReferenceGroupGetter>(
            _sourceMods.Select(m => m.ReferenceGroups));
    public IFallout4GroupGetter<IAimModelGetter> AimModels =>
        _aimModels ??= new MergedGroup<IAimModelGetter>(
            _sourceMods.Select(m => m.AimModels));
    public IFallout4GroupGetter<ILayerGetter> Layers =>
        _layers ??= new MergedGroup<ILayerGetter>(
            _sourceMods.Select(m => m.Layers));
    public IFallout4GroupGetter<IConstructibleObjectGetter> ConstructibleObjects =>
        _constructibleObjects ??= new MergedGroup<IConstructibleObjectGetter>(
            _sourceMods.Select(m => m.ConstructibleObjects));
    public IFallout4GroupGetter<IAObjectModificationGetter> ObjectModifications =>
        _objectModifications ??= new MergedGroup<IAObjectModificationGetter>(
            _sourceMods.Select(m => m.ObjectModifications));
    public IFallout4GroupGetter<IMaterialSwapGetter> MaterialSwaps =>
        _materialSwaps ??= new MergedGroup<IMaterialSwapGetter>(
            _sourceMods.Select(m => m.MaterialSwaps));
    public IFallout4GroupGetter<IZoomGetter> Zooms =>
        _zooms ??= new MergedGroup<IZoomGetter>(
            _sourceMods.Select(m => m.Zooms));
    public IFallout4GroupGetter<IInstanceNamingRulesGetter> InstanceNamingRules =>
        _instanceNamingRules ??= new MergedGroup<IInstanceNamingRulesGetter>(
            _sourceMods.Select(m => m.InstanceNamingRules));
    public IFallout4GroupGetter<ISoundKeywordMappingGetter> SoundKeywordMappings =>
        _soundKeywordMappings ??= new MergedGroup<ISoundKeywordMappingGetter>(
            _sourceMods.Select(m => m.SoundKeywordMappings));
    public IFallout4GroupGetter<IAudioEffectChainGetter> AudioEffectChains =>
        _audioEffectChains ??= new MergedGroup<IAudioEffectChainGetter>(
            _sourceMods.Select(m => m.AudioEffectChains));
    public IFallout4GroupGetter<ISceneCollectionGetter> SceneCollections =>
        _sceneCollections ??= new MergedGroup<ISceneCollectionGetter>(
            _sourceMods.Select(m => m.SceneCollections));
    public IFallout4GroupGetter<IAttractionRuleGetter> AttractionRules =>
        _attractionRules ??= new MergedGroup<IAttractionRuleGetter>(
            _sourceMods.Select(m => m.AttractionRules));
    public IFallout4GroupGetter<IAudioCategorySnapshotGetter> AudioCategorySnapshots =>
        _audioCategorySnapshots ??= new MergedGroup<IAudioCategorySnapshotGetter>(
            _sourceMods.Select(m => m.AudioCategorySnapshots));
    public IFallout4GroupGetter<IAnimationSoundTagSetGetter> AnimationSoundTagSets =>
        _animationSoundTagSets ??= new MergedGroup<IAnimationSoundTagSetGetter>(
            _sourceMods.Select(m => m.AnimationSoundTagSets));
    public IFallout4GroupGetter<INavigationMeshObstacleManagerGetter> NavigationMeshObstacleManagers =>
        _navigationMeshObstacleManagers ??= new MergedGroup<INavigationMeshObstacleManagerGetter>(
            _sourceMods.Select(m => m.NavigationMeshObstacleManagers));
    public IFallout4GroupGetter<ILensFlareGetter> LensFlares =>
        _lensFlares ??= new MergedGroup<ILensFlareGetter>(
            _sourceMods.Select(m => m.LensFlares));
    public IFallout4GroupGetter<IGodRaysGetter> GodRays =>
        _godRays ??= new MergedGroup<IGodRaysGetter>(
            _sourceMods.Select(m => m.GodRays));
    public IFallout4GroupGetter<IObjectVisibilityManagerGetter> ObjectVisibilityManagers =>
        _objectVisibilityManagers ??= new MergedGroup<IObjectVisibilityManagerGetter>(
            _sourceMods.Select(m => m.ObjectVisibilityManagers));

    BinaryModdedWriteBuilderTargetChoice<IFallout4ModGetter> IFallout4ModGetter.BeginWrite =>
        new BinaryModdedWriteBuilderTargetChoice<IFallout4ModGetter>(this, Fallout4Mod.Fallout4WriteBuilderInstantiator.Instance);

    IBinaryModdedWriteBuilderTargetChoice IModGetter.BeginWrite =>
        new BinaryModdedWriteBuilderTargetChoice<IFallout4ModGetter>(this, Fallout4Mod.Fallout4WriteBuilderInstantiator.Instance);

    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
        => Fallout4ModCommon.Instance.EnumerateAssetLinks(this, queryCategories, linkCache, assetType);

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true)
        => Fallout4ModCommon.Instance.EnumerateFormLinks(this, iterateNestedRecords);

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
        => Fallout4ModCommon.Instance.EnumerateMajorRecords(this);

    public IEnumerable<T> EnumerateMajorRecords<T>(bool throwIfUnknown = true) where T : class, IMajorRecordQueryableGetter
        => Fallout4ModCommon.Instance.EnumerateMajorRecords(this, typeof(T), throwIfUnknown).Select(m => (T)m);

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
        => Fallout4ModCommon.Instance.EnumerateMajorRecords(this, type, throwIfUnknown);

    IGroupGetter<TMajor>? IModGetter.TryGetTopLevelGroup<TMajor>()
    {
        return (IGroupGetter<TMajor>?)((Fallout4ModCommon)((IFallout4ModGetter)this).CommonInstance()!).GetGroup(
            obj: this,
            type: typeof(TMajor));
    }

    public IGroupGetter? TryGetTopLevelGroup(Type type)
    {
        return (IGroupGetter?)((Fallout4ModCommon)((IFallout4ModGetter)this).CommonInstance()!).GetGroup(
            obj: this,
            type: type);
    }

    IEnumerable<IModContext<IFallout4Mod, IFallout4ModGetter, TSetter, TGetter>> IMajorRecordContextEnumerable<IFallout4Mod, IFallout4ModGetter>.EnumerateMajorRecordContexts<TSetter, TGetter>(ILinkCache linkCache, bool throwIfUnknown)
    {
        var seen = new HashSet<FormKey>();
        for (int i = _sourceMods.Count - 1; i >= 0; i--)
        {
            foreach (var context in _sourceMods[i].EnumerateMajorRecordContexts<TSetter, TGetter>(linkCache, throwIfUnknown))
            {
                if (context.Record is IMajorRecordGetter majorRecord && seen.Add(majorRecord.FormKey))
                {
                    yield return context;
                }
            }
        }
    }

    IEnumerable<IModContext<IFallout4Mod, IFallout4ModGetter, IMajorRecord, IMajorRecordGetter>> IMajorRecordContextEnumerable<IFallout4Mod, IFallout4ModGetter>.EnumerateMajorRecordContexts(ILinkCache linkCache, Type type, bool throwIfUnknown)
    {
        var seen = new HashSet<FormKey>();
        for (int i = _sourceMods.Count - 1; i >= 0; i--)
        {
            foreach (var context in _sourceMods[i].EnumerateMajorRecordContexts(linkCache, type, throwIfUnknown))
            {
                if (seen.Add(context.Record.FormKey))
                {
                    yield return context;
                }
            }
        }
    }

    public IEnumerable<IModContext<IMajorRecordGetter>> EnumerateMajorRecordSimpleContexts()
    {
        var seen = new HashSet<FormKey>();
        for (int i = _sourceMods.Count - 1; i >= 0; i--)
        {
            foreach (var context in _sourceMods[i].EnumerateMajorRecordSimpleContexts())
            {
                if (seen.Add(context.Record.FormKey))
                {
                    yield return context;
                }
            }
        }
    }

    public IEnumerable<IModContext<TGetter>> EnumerateMajorRecordSimpleContexts<TGetter>(bool throwIfUnknown = true) where TGetter : class, IMajorRecordQueryableGetter
    {
        var seen = new HashSet<FormKey>();
        for (int i = _sourceMods.Count - 1; i >= 0; i--)
        {
            foreach (var context in _sourceMods[i].EnumerateMajorRecordSimpleContexts<TGetter>(throwIfUnknown))
            {
                if (context.Record is IMajorRecordGetter majorRecord && seen.Add(majorRecord.FormKey))
                {
                    yield return context;
                }
            }
        }
    }

    public IEnumerable<IModContext<IMajorRecordGetter>> EnumerateMajorRecordSimpleContexts(Type type, bool throwIfUnknown = true)
    {
        var seen = new HashSet<FormKey>();
        for (int i = _sourceMods.Count - 1; i >= 0; i--)
        {
            foreach (var context in _sourceMods[i].EnumerateMajorRecordSimpleContexts(type, throwIfUnknown))
            {
                if (seen.Add(context.Record.FormKey))
                {
                    yield return context;
                }
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
        => Fallout4ModCommon.Instance.GetRecordCount(this);

    IMod IModGetter.DeepCopy()
    {
        return ((Fallout4ModSetterTranslationCommon)((IFallout4ModGetter)this).CommonSetterTranslationInstance()!).DeepCopy(
            item: this,
            errorMask: null,
            copyMask: null);
    }

    IReadOnlyList<IFormLinkGetter<IMajorRecordGetter>>? IModGetter.OverriddenForms =>
        _sourceMods.Where(m => m.OverriddenForms != null)
            .SelectMany(m => m.OverriddenForms!)
            .Distinct()
            .ToList();

    public uint NextFormID => _sourceMods.Max(m => m.NextFormID);

    public ILoquiRegistration Registration => Fallout4Mod_Registration.Instance;

    public void Print(StructuredStringBuilder sb, string? name = null)
        => Fallout4ModCommon.Instance.Print(this, sb, name);

    public IMask<bool> GetEqualsMask(object rhs, EqualsMaskHelper.Include include)
        => Fallout4ModCommon.Instance.GetEqualsMask(this, (IFallout4ModGetter)rhs, include);

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
/// Merged ModStats. NextFormID is the max across sources; NumRecords is zeroed.
/// Throws InvalidDataException if must-match fields disagree between sources.
/// </summary>
internal class MergedModStats : IModStatsGetter
{
    private readonly IReadOnlyList<IModStatsGetter> _sources;

    private static readonly ModStats.TranslationMask _mustMatchMask = new ModStats.TranslationMask(defaultOn: true)
    {
        NumRecords = false,
        NextFormID = false,
    };

    public MergedModStats(IReadOnlyList<IModStatsGetter> sources)
    {
        _sources = sources;
        ValidateConsistency();
    }

    private void ValidateConsistency()
    {
        if (_sources.Count <= 1) return;
        var first = _sources[0];
        for (int i = 1; i < _sources.Count; i++)
        {
            if (!first.Equals(_sources[i], _mustMatchMask))
            {
                throw new System.IO.InvalidDataException($"MergedModStats: source mod {i} disagrees with source mod 0 on must-match fields.");
            }
        }
    }

    public Single Version => _sources[0].Version;
    public UInt32 NumRecords => 0;
    public UInt32 NextFormID => _sources.Max(s => s.NextFormID);

    public ILoquiRegistration Registration => IModStatsGetter.StaticRegistration;
    public object CommonInstance() => _sources[0].CommonInstance();
    public object? CommonSetterInstance() => null;
    public object CommonSetterTranslationInstance() => _sources[0].CommonSetterTranslationInstance();
    public void Print(StructuredStringBuilder sb, string? name = null) => _sources[0].Print(sb, name);
    object IBinaryItem.BinaryWriteTranslator => ModStatsBinaryWriteTranslation.Instance;
    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
        => ((ModStatsBinaryWriteTranslation)((IBinaryItem)this).BinaryWriteTranslator).Write(item: this, writer: writer, translationParams: translationParams);
}

/// <summary>
/// Merged ModHeader that projects aggregate state (masters, overridden forms) across all source mods.
/// Throws InvalidDataException if must-match fields (Version, Flags, Author, etc.) disagree between sources.
/// </summary>
internal class MergedFallout4ModHeader : IFallout4ModHeaderGetter
{
    private readonly IReadOnlyList<IFallout4ModHeaderGetter> _sources;
    private readonly IReadOnlyList<IMasterReferenceGetter> _masters;
    private readonly MergedModStats _stats;

    private static readonly Fallout4ModHeader.TranslationMask _mustMatchMask = new Fallout4ModHeader.TranslationMask(defaultOn: true)
    {
        MasterReferences = false,
        OverriddenForms = false,
        TransientTypes = false,
        Stats = new ModStats.TranslationMask(defaultOn: true)
        {
            NumRecords = false,
            NextFormID = false,
        },
    };

    public MergedFallout4ModHeader(IReadOnlyList<IFallout4ModHeaderGetter> sources, IReadOnlyList<IMasterReferenceGetter> masters)
    {
        _sources = sources;
        _masters = masters;
        _stats = new MergedModStats(sources.Select(s => s.Stats).ToList());
        ValidateConsistency();
    }

    private void ValidateConsistency()
    {
        if (_sources.Count <= 1) return;
        var first = _sources[0];
        for (int i = 1; i < _sources.Count; i++)
        {
            if (!first.Equals(_sources[i], _mustMatchMask))
            {
                throw new System.IO.InvalidDataException($"MergedFallout4ModHeader: source mod {i} disagrees with source mod 0 on must-match fields.");
            }
        }
    }

    public Fallout4ModHeader.HeaderFlag Flags => _sources[0].Flags;
    public UInt32 FormID => _sources[0].FormID;
    public Int32 Version => _sources[0].Version;
    public UInt16 FormVersion => _sources[0].FormVersion;
    public UInt16 Version2 => _sources[0].Version2;
    public IModStatsGetter Stats => _stats;
    public ReadOnlyMemorySlice<Byte>? TypeOffsets => _sources[0].TypeOffsets;
    public ReadOnlyMemorySlice<Byte>? Deleted => _sources[0].Deleted;
    public String? Author => _sources[0].Author;
    public String? Description => _sources[0].Description;
    public IReadOnlyList<IMasterReferenceGetter> MasterReferences => _masters;
    public IReadOnlyList<IFormLinkGetter<IFallout4MajorRecordGetter>>? OverriddenForms
    {
        get
        {
            var merged = _sources.Where(s => s.OverriddenForms != null)
                .SelectMany(s => s.OverriddenForms!)
                .Distinct()
                .ToList();
            return merged.Count == 0 ? null : merged;
        }
    }
    public ReadOnlyMemorySlice<Byte>? Screenshot => _sources[0].Screenshot;
    public IReadOnlyList<ITransientTypeGetter> TransientTypes => _sources.SelectMany(s => s.TransientTypes).ToList();
    public ReadOnlyMemorySlice<Byte>? INTV => _sources[0].INTV;
    public Int32? INCC => _sources[0].INCC;

    public ILoquiRegistration Registration => IFallout4ModHeaderGetter.StaticRegistration;
    public object CommonInstance() => Fallout4ModHeaderCommon.Instance;
    public object? CommonSetterInstance() => null;
    public object CommonSetterTranslationInstance() => _sources[0].CommonSetterTranslationInstance();
    public void Print(StructuredStringBuilder sb, string? name = null) => _sources[0].Print(sb, name);
    object IBinaryItem.BinaryWriteTranslator => Fallout4ModHeaderBinaryWriteTranslation.Instance;
    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
        => ((Fallout4ModHeaderBinaryWriteTranslation)((IBinaryItem)this).BinaryWriteTranslator).Write(item: this, writer: writer, translationParams: translationParams);
    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true) => Fallout4ModHeaderCommon.Instance.EnumerateFormLinks(this, iterateNestedRecords);
}

/// <summary>
/// Merged group that combines multiple groups into a single unified view.
/// Validates no duplicate FormKeys exist and caches results.
/// When allowDuplicateOverrides is true, duplicate FormKeys are allowed and the later copy wins.
/// </summary>
internal class MergedGroup<TGetter> : IFallout4GroupGetter<TGetter>, IReadOnlyCache<TGetter, FormKey>
    where TGetter : class, IFallout4MajorRecordGetter, IBinaryItem
{
    private readonly IEnumerable<IGroupGetter<TGetter>> _sourceGroups;
    private readonly bool _allowDuplicateOverrides;
    private readonly Func<TGetter, TGetter, TGetter>? _duplicateMerger;
    private readonly Lazy<Dictionary<FormKey, TGetter>> _cache;

    public MergedGroup(IEnumerable<IGroupGetter<TGetter>> sourceGroups, bool allowDuplicateOverrides = false, Func<TGetter, TGetter, TGetter>? duplicateMerger = null)
    {
        _sourceGroups = sourceGroups;
        _allowDuplicateOverrides = allowDuplicateOverrides;
        _duplicateMerger = duplicateMerger;
        _cache = new Lazy<Dictionary<FormKey, TGetter>>(BuildCache);
    }

    private Dictionary<FormKey, TGetter> BuildCache()
    {
        var cache = new Dictionary<FormKey, TGetter>();
        foreach (var group in _sourceGroups)
        {
            foreach (var record in group)
            {
                if (!cache.TryAdd(record.FormKey, record))
                {
                    if (_allowDuplicateOverrides)
                    {
                        if (_duplicateMerger != null)
                        {
                            cache[record.FormKey] = _duplicateMerger(cache[record.FormKey], record);
                        }
                        else
                        {
                            cache[record.FormKey] = record;
                        }
                    }
                    else
                    {
                        throw new SplitModException(
                            $"Duplicate FormKey {record.FormKey} found in split mods. " +
                            "This indicates corruption or an error in the splitting logic.");
                    }
                }
            }
        }
        return cache;
    }

    private Dictionary<FormKey, TGetter> Cache => _cache.Value;

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
        foreach (var record in EnumerateMajorRecords())
        {
            if (record is not IAssetLinkContainerGetter assetContainer) continue;
            foreach (var link in assetContainer.EnumerateAssetLinks(queryCategories, linkCache, assetType))
            {
                yield return link;
            }
        }
    }

    object IBinaryItem.BinaryWriteTranslator => Fallout4GroupBinaryWriteTranslation.Instance;
    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
        => Fallout4GroupBinaryWriteTranslation.Instance.Write(writer: writer, item: this, translationParams: translationParams);

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
        var seen = new HashSet<FormKey>();
        var sources = _sourceGroups.Reverse();
        foreach (var group in sources)
        {
            if (group is not IMajorRecordGetterEnumerable enumerable) continue;
            foreach (var record in enumerable.EnumerateMajorRecords())
            {
                if (seen.Add(record.FormKey)) yield return record;
            }
        }
    }

    IEnumerable<T> IMajorRecordGetterEnumerable.EnumerateMajorRecords<T>(bool throwIfUnknown)
    {
        var seen = new HashSet<FormKey>();
        var sources = _sourceGroups.Reverse();
        foreach (var group in sources)
        {
            if (group is not IMajorRecordGetterEnumerable enumerable) continue;
            foreach (var record in enumerable.EnumerateMajorRecords<T>(throwIfUnknown))
            {
                if (record is IMajorRecordGetter major && seen.Add(major.FormKey)) yield return record;
            }
        }
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
    {
        var seen = new HashSet<FormKey>();
        var sources = _sourceGroups.Reverse();
        foreach (var group in sources)
        {
            if (group is not IMajorRecordGetterEnumerable enumerable) continue;
            foreach (var record in enumerable.EnumerateMajorRecords(type, throwIfUnknown))
            {
                if (seen.Add(record.FormKey)) yield return record;
            }
        }
    }

    // IFallout4GroupGetter members
    public object CommonInstance(Type type) => GenericCommonInstanceGetter.Get(Fallout4GroupCommon<TGetter>.Instance, typeof(ICellBlockGetter), type);
    public object? CommonSetterInstance(Type type) => null;
    public object CommonSetterTranslationInstance() => Fallout4GroupSetterTranslationCommon.Instance;

    GroupTypeEnum IFallout4GroupGetter<TGetter>.Type => GroupTypeEnum.Type;
    int IFallout4GroupGetter<TGetter>.LastModified => 0;
    public int Unknown => 0;

    IReadOnlyCache<TGetter, FormKey> IFallout4GroupGetter<TGetter>.RecordCache => this;
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
        Fallout4GroupMixIn.Print(
            item: this,
            sb: sb,
            name: name);
    }
}

/// <summary>
/// Merged list group that combines multiple list groups (like Cells) into a single unified view.
/// CellBlocks from different mods are merged by BlockNumber.
/// </summary>
internal class MergedListGroup : IFallout4ListGroupGetter<ICellBlockGetter>
{
    private readonly IEnumerable<IFallout4ListGroupGetter<ICellBlockGetter>> _sourceGroups;
    private readonly Lazy<List<ICellBlockGetter>> _cache;

    public MergedListGroup(IEnumerable<IFallout4ListGroupGetter<ICellBlockGetter>> sourceGroups)
    {
        _sourceGroups = sourceGroups;
        _cache = new Lazy<List<ICellBlockGetter>>(MergeBlocks);
    }

    private List<ICellBlockGetter> MergeBlocks()
    {
        var blocksByNumber = new Dictionary<int, List<ICellBlockGetter>>();
        foreach (var group in _sourceGroups)
        {
            foreach (var block in group.Records)
            {
                blocksByNumber.GetOrAdd(block.BlockNumber).Add(block);
            }
        }
        var result = new List<ICellBlockGetter>();
        foreach (var blockNumber in blocksByNumber.Keys.OrderBy(k => k))
        {
            var blocksForNumber = blocksByNumber[blockNumber];
            result.Add(blocksForNumber.Count == 1 ? blocksForNumber[0] : new MergedCellBlock(blockNumber, blocksForNumber));
        }
        return result;
    }

    private List<ICellBlockGetter> Cache => _cache.Value;

    public IEnumerator<ICellBlockGetter> GetEnumerator() => Cache.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => Cache.Count;
    public ICellBlockGetter this[int index] => Cache[index];
    public IReadOnlyList<ICellBlockGetter> Records => Cache;

    ILoquiRegistration ILoquiObject.Registration => Fallout4Mod_Registration.Instance;
    public static ILoquiRegistration StaticRegistration => Fallout4Mod_Registration.Instance;

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        Fallout4ListGroupMixIn.Print(
            item: this,
            sb: sb,
            name: name);
    }

    // IFallout4ListGroupGetter properties
    public GroupTypeEnum Type => _sourceGroups.FirstOrDefault()?.Type ?? GroupTypeEnum.InteriorCellBlock;
    public int LastModified => _sourceGroups.Max(g => g.LastModified);
    public int Unknown => 0;

    public object CommonInstance(Type type) => GenericCommonInstanceGetter.Get(Fallout4ListGroupCommon<ICellBlockGetter>.Instance, typeof(ICellBlockGetter), type);
    public object? CommonSetterInstance(Type type) => GenericCommonInstanceGetter.Get(Fallout4ListGroupSetterCommon<ICellBlock>.Instance, typeof(ICellBlockGetter), type);
    public object CommonSetterTranslationInstance() => Fallout4ListGroupSetterTranslationCommon.Instance;

    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
        => Fallout4ListGroupCommon<ICellBlockGetter>.Instance.EnumerateAssetLinks(this, queryCategories, linkCache, assetType);

    object IBinaryItem.BinaryWriteTranslator => Fallout4ListGroupBinaryWriteTranslation.Instance;
    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
        => Fallout4ListGroupBinaryWriteTranslation.Instance.Write(writer: writer, item: this, translationParams: translationParams);

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true)
        => Fallout4ListGroupCommon<ICellBlockGetter>.Instance.EnumerateFormLinks(this, iterateNestedRecords);

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
        => Fallout4ListGroupCommon<ICellBlockGetter>.Instance.EnumerateMajorRecords(this);

    IEnumerable<T> IMajorRecordGetterEnumerable.EnumerateMajorRecords<T>(bool throwIfUnknown)
        => Fallout4ListGroupCommon<ICellBlockGetter>.Instance.EnumerateMajorRecords(this, typeof(T), throwIfUnknown).Select(m => (T)m);

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
        => Fallout4ListGroupCommon<ICellBlockGetter>.Instance.EnumerateMajorRecords(this, type, throwIfUnknown);
}

/// <summary>
/// Merged cell block that combines multiple cell blocks with the same BlockNumber.
/// </summary>
internal class MergedCellBlock : ICellBlockGetter
{
    private readonly int _blockNumber;
    private readonly List<ICellBlockGetter> _sourceBlocks;
    private readonly Lazy<List<ICellSubBlockGetter>> _mergedSubBlocks;

    public MergedCellBlock(int blockNumber, List<ICellBlockGetter> sourceBlocks)
    {
        _blockNumber = blockNumber;
        _sourceBlocks = sourceBlocks;
        _mergedSubBlocks = new Lazy<List<ICellSubBlockGetter>>(() => _sourceBlocks.SelectMany(b => b.SubBlocks).ToList());
    }

    public int BlockNumber => _blockNumber;
    public GroupTypeEnum GroupType => _sourceBlocks.FirstOrDefault()?.GroupType ?? GroupTypeEnum.InteriorCellBlock;
    public int LastModified => _sourceBlocks.Max(b => b.LastModified);
    public int Unknown => 0;
    public IReadOnlyList<ICellSubBlockGetter> SubBlocks => _mergedSubBlocks.Value;

    ILoquiRegistration ILoquiObject.Registration => null!;

    public void Print(StructuredStringBuilder sb, string? name = null) => CellBlockCommon.Instance.Print(this, sb, name);

    public object CommonInstance() => CellBlockCommon.Instance;
    public object? CommonSetterInstance() => null;
    public object CommonSetterTranslationInstance() => CellBlockSetterTranslationCommon.Instance;

    object IBinaryItem.BinaryWriteTranslator => CellBlockBinaryWriteTranslation.Instance;
    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
        => CellBlockBinaryWriteTranslation.Instance.Write(writer: writer, item: this, translationParams: translationParams);

    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
        => CellBlockCommon.Instance.EnumerateAssetLinks(this, queryCategories, linkCache, assetType);

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true)
        => CellBlockCommon.Instance.EnumerateFormLinks(this, iterateNestedRecords);

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
        => CellBlockCommon.Instance.EnumerateMajorRecords(this);

    public IEnumerable<T> EnumerateMajorRecords<T>(bool throwIfUnknown = true) where T : class, IMajorRecordQueryableGetter
        => CellBlockCommon.Instance.EnumerateMajorRecords(this, typeof(T), throwIfUnknown).Select(m => (T)m);

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
        => CellBlockCommon.Instance.EnumerateMajorRecords(this, type, throwIfUnknown);
}

/// <summary>
/// Merged worldspace that combines SubCells from multiple worldspace copies with the same FormKey.
/// </summary>
internal class MergedWorldspace : IWorldspaceGetter
{
    private readonly IWorldspaceGetter _primary;
    private readonly List<IWorldspaceGetter> _allCopies;
    private readonly Lazy<IReadOnlyList<IWorldspaceBlockGetter>> _mergedSubCells;

    public MergedWorldspace(IWorldspaceGetter primary, List<IWorldspaceGetter> allCopies)
    {
        _primary = primary;
        _allCopies = allCopies;
        _mergedSubCells = new Lazy<IReadOnlyList<IWorldspaceBlockGetter>>(MergeSubCells);
    }

    public static IWorldspaceGetter Merge(IWorldspaceGetter existing, IWorldspaceGetter newer)
    {
        List<IWorldspaceGetter> allCopies;
        if (existing is MergedWorldspace merged)
        {
            allCopies = merged._allCopies;
            allCopies.Add(newer);
        }
        else
        {
            allCopies = new List<IWorldspaceGetter> { existing, newer };
        }
        return new MergedWorldspace(newer, allCopies);
    }

    private IReadOnlyList<IWorldspaceBlockGetter> MergeSubCells()
    {
        var blocksByKey = new Dictionary<(short X, short Y), List<IWorldspaceBlockGetter>>();
        foreach (var ws in _allCopies)
        {
            foreach (var block in ws.SubCells)
            {
                var key = (block.BlockNumberX, block.BlockNumberY);
                blocksByKey.GetOrAdd(key).Add(block);
            }
        }
        var result = new List<IWorldspaceBlockGetter>();
        foreach (var (_, blocks) in blocksByKey)
        {
            result.Add(blocks.Count == 1 ? blocks[0] : new MergedWorldspaceBlock(blocks));
        }
        return result;
    }

    public IReadOnlyList<IWorldspaceBlockGetter> SubCells => _mergedSubCells.Value;

    public FormKey FormKey => _primary.FormKey;
    public string? EditorID => _primary.EditorID;
    public int MajorRecordFlagsRaw => _primary.MajorRecordFlagsRaw;
    public uint VersionControl => _primary.VersionControl;
    ushort IFallout4MajorRecordGetter.FormVersion => _primary.FormVersion;
    ushort? IMajorRecordGetter.FormVersion => _primary.FormVersion;
    ushort? IFormVersionGetter.FormVersion => _primary.FormVersion;
    public ushort Version2 => _primary.Version2;
    public Fallout4MajorRecord.Fallout4MajorRecordFlag Fallout4MajorRecordFlags => _primary.Fallout4MajorRecordFlags;
    public Worldspace.MajorFlag MajorFlags => _primary.MajorFlags;
    public bool IsCompressed => (MajorRecordFlagsRaw & Mutagen.Bethesda.Plugins.Internals.Constants.CompressedFlag) != 0;
    public bool IsDeleted => (MajorRecordFlagsRaw & Mutagen.Bethesda.Plugins.Internals.Constants.DeletedFlag) != 0;
    Type ILinkIdentifier.Type => typeof(IWorldspaceGetter);
    public bool Equals(IFormLinkGetter? other) => other != null && other.FormKey == FormKey && typeof(IWorldspaceGetter).IsAssignableFrom(other.Type);

    public IReadOnlyList<IWorldspaceGridReferenceGetter> LargeReferences => _primary.LargeReferences;
    public IWorldspaceMaxHeightGetter? MaxHeight => _primary.MaxHeight;
    public ITranslatedStringGetter? Name => _primary.Name;
    public P2Int16? FixedDimensionsCenterCell => _primary.FixedDimensionsCenterCell;
    public IFormLinkNullableGetter<ILightingTemplateGetter> InteriorLighting => _primary.InteriorLighting;
    public IFormLinkNullableGetter<IEncounterZoneGetter> EncounterZone => _primary.EncounterZone;
    public IFormLinkNullableGetter<ILocationGetter> Location => _primary.Location;
    public IWorldspaceParentGetter? Parent => _primary.Parent;
    public IFormLinkNullableGetter<IClimateGetter> Climate => _primary.Climate;
    public IFormLinkNullableGetter<IWaterGetter> Water => _primary.Water;
    public IFormLinkNullableGetter<IWaterGetter> LodWater => _primary.LodWater;
    public Single? LodWaterHeight => _primary.LodWaterHeight;
    public IWorldspaceLandDefaultsGetter? LandDefaults => _primary.LandDefaults;
    public String? MapImage => _primary.MapImage;
    public IModelGetter? CloudModel => _primary.CloudModel;
    public IWorldspaceMapGetter? MapData => _primary.MapData;
    public Single WorldMapOffsetScale => _primary.WorldMapOffsetScale;
    public P3Float WorldMapCellOffset => _primary.WorldMapCellOffset;
    public Single? DistantLodMultiplier => _primary.DistantLodMultiplier;
    public Worldspace.Flag Flags => _primary.Flags;
    public P2Float ObjectBoundsMin => _primary.ObjectBoundsMin;
    public P2Float ObjectBoundsMax => _primary.ObjectBoundsMax;
    public IFormLinkNullableGetter<IMusicTypeGetter> Music => _primary.Music;
    public String? CanopyShadow => _primary.CanopyShadow;
    public String? WaterEnvironmentMap => _primary.WaterEnvironmentMap;
    public String? HdLodDiffuseTexture => _primary.HdLodDiffuseTexture;
    public String? HdLodNormalTexture => _primary.HdLodNormalTexture;
    public IWorldDefaultLevelDataGetter? DefaultLevelData => _primary.DefaultLevelData;
    public ReadOnlyMemorySlice<Byte>? OffsetData => _primary.OffsetData;
    public ReadOnlyMemorySlice<Byte>? CellSizeData => _primary.CellSizeData;
    public ICellGetter? TopCell => _primary.TopCell;
    public Int32 SubCellsTimestamp => _primary.SubCellsTimestamp;
    public Int32 SubCellsUnknown => _primary.SubCellsUnknown;

    ITranslatedStringGetter? ITranslatedNamedGetter.Name => _primary.Name;
    ITranslatedStringGetter ITranslatedNamedRequiredGetter.Name => _primary.Name ?? TranslatedString.Empty;
    string? INamedGetter.Name => _primary.Name?.String;
    string INamedRequiredGetter.Name => _primary.Name?.String ?? string.Empty;

    ILoquiRegistration ILoquiObject.Registration => Worldspace_Registration.Instance;
    public object CommonInstance() => WorldspaceCommon.Instance;
    public object? CommonSetterInstance() => null;
    public object CommonSetterTranslationInstance() => WorldspaceSetterTranslationCommon.Instance;

    public void Print(StructuredStringBuilder sb, string? name) => WorldspaceCommon.Instance.Print(this, sb, name);

    object IBinaryItem.BinaryWriteTranslator => WorldspaceBinaryWriteTranslation.Instance;
    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
        => WorldspaceBinaryWriteTranslation.Instance.Write(writer: writer, item: this, translationParams: translationParams);

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true)
        => WorldspaceCommon.Instance.EnumerateFormLinks(this, iterateNestedRecords);
    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
        => WorldspaceCommon.Instance.EnumerateAssetLinks(this, queryCategories, linkCache, assetType);

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
        => WorldspaceCommon.Instance.EnumerateMajorRecords(this);

    public IEnumerable<T> EnumerateMajorRecords<T>(bool throwIfUnknown = true) where T : class, IMajorRecordQueryableGetter
        => WorldspaceCommon.Instance.EnumerateMajorRecords(this, typeof(T), throwIfUnknown).Select(m => (T)m);

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
        => WorldspaceCommon.Instance.EnumerateMajorRecords(this, type, throwIfUnknown);
}

/// <summary>
/// Merged worldspace block that combines multiple blocks with the same (BlockNumberX, BlockNumberY).
/// </summary>
internal class MergedWorldspaceBlock : IWorldspaceBlockGetter
{
    private readonly List<IWorldspaceBlockGetter> _sourceBlocks;
    private readonly Lazy<IReadOnlyList<IWorldspaceSubBlockGetter>> _mergedItems;

    public MergedWorldspaceBlock(List<IWorldspaceBlockGetter> sourceBlocks)
    {
        _sourceBlocks = sourceBlocks;
        _mergedItems = new Lazy<IReadOnlyList<IWorldspaceSubBlockGetter>>(MergeItems);
    }

    public short BlockNumberY => _sourceBlocks[0].BlockNumberY;
    public short BlockNumberX => _sourceBlocks[0].BlockNumberX;
    public GroupTypeEnum GroupType => _sourceBlocks[0].GroupType;
    public int LastModified => _sourceBlocks.Max(b => b.LastModified);
    public int Unknown => 0;

    private IReadOnlyList<IWorldspaceSubBlockGetter> MergeItems()
    {
        var subBlocksByKey = new Dictionary<(short X, short Y), List<IWorldspaceSubBlockGetter>>();
        foreach (var block in _sourceBlocks)
        {
            foreach (var subBlock in block.Items)
            {
                var key = (subBlock.BlockNumberX, subBlock.BlockNumberY);
                subBlocksByKey.GetOrAdd(key).Add(subBlock);
            }
        }
        var result = new List<IWorldspaceSubBlockGetter>();
        foreach (var (_, subBlocks) in subBlocksByKey)
        {
            result.Add(subBlocks.Count == 1 ? subBlocks[0] : new MergedWorldspaceSubBlock(subBlocks));
        }
        return result;
    }

    public IReadOnlyList<IWorldspaceSubBlockGetter> Items => _mergedItems.Value;

    ILoquiRegistration ILoquiObject.Registration => null!;
    public void Print(StructuredStringBuilder sb, string? name) => WorldspaceBlockCommon.Instance.Print(this, sb, name);
    public object CommonInstance() => WorldspaceBlockCommon.Instance;
    public object? CommonSetterInstance() => null;
    public object CommonSetterTranslationInstance() => WorldspaceBlockSetterTranslationCommon.Instance;

    object IBinaryItem.BinaryWriteTranslator => WorldspaceBlockBinaryWriteTranslation.Instance;
    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
        => WorldspaceBlockBinaryWriteTranslation.Instance.Write(writer: writer, item: this, translationParams: translationParams);

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true)
        => WorldspaceBlockCommon.Instance.EnumerateFormLinks(this, iterateNestedRecords);

    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
        => WorldspaceBlockCommon.Instance.EnumerateAssetLinks(this, queryCategories, linkCache, assetType);

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
        => WorldspaceBlockCommon.Instance.EnumerateMajorRecords(this);

    public IEnumerable<T> EnumerateMajorRecords<T>(bool throwIfUnknown = true) where T : class, IMajorRecordQueryableGetter
        => WorldspaceBlockCommon.Instance.EnumerateMajorRecords(this, typeof(T), throwIfUnknown).Select(m => (T)m);

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
        => WorldspaceBlockCommon.Instance.EnumerateMajorRecords(this, type, throwIfUnknown);
}

/// <summary>
/// Merged worldspace sub-block that deduplicates cells by FormKey and merges their placed objects.
/// </summary>
internal class MergedWorldspaceSubBlock : IWorldspaceSubBlockGetter
{
    private readonly List<IWorldspaceSubBlockGetter> _sourceSubBlocks;
    private readonly Lazy<IReadOnlyList<ICellGetter>> _mergedItems;

    public MergedWorldspaceSubBlock(List<IWorldspaceSubBlockGetter> sourceSubBlocks)
    {
        _sourceSubBlocks = sourceSubBlocks;
        _mergedItems = new Lazy<IReadOnlyList<ICellGetter>>(MergeItems);
    }

    public short BlockNumberY => _sourceSubBlocks[0].BlockNumberY;
    public short BlockNumberX => _sourceSubBlocks[0].BlockNumberX;
    public GroupTypeEnum GroupType => _sourceSubBlocks[0].GroupType;
    public int LastModified => _sourceSubBlocks.Max(sb => sb.LastModified);
    public int Unknown => 0;

    private IReadOnlyList<ICellGetter> MergeItems()
    {
        var cellsByFormKey = new Dictionary<FormKey, List<ICellGetter>>();
        foreach (var subBlock in _sourceSubBlocks)
        {
            foreach (var cell in subBlock.Items)
            {
                cellsByFormKey.GetOrAdd(cell.FormKey).Add(cell);
            }
        }
        var result = new List<ICellGetter>();
        foreach (var (_, cells) in cellsByFormKey)
        {
            result.Add(cells.Count == 1 ? cells[0] : new MergedWorldspaceCell(cells));
        }
        return result;
    }

    public IReadOnlyList<ICellGetter> Items => _mergedItems.Value;

    ILoquiRegistration ILoquiObject.Registration => null!;
    public void Print(StructuredStringBuilder sb, string? name) => WorldspaceSubBlockCommon.Instance.Print(this, sb, name);
    public object CommonInstance() => WorldspaceSubBlockCommon.Instance;
    public object? CommonSetterInstance() => null;
    public object CommonSetterTranslationInstance() => WorldspaceSubBlockSetterTranslationCommon.Instance;

    object IBinaryItem.BinaryWriteTranslator => WorldspaceSubBlockBinaryWriteTranslation.Instance;
    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
        => WorldspaceSubBlockBinaryWriteTranslation.Instance.Write(writer: writer, item: this, translationParams: translationParams);

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true)
        => WorldspaceSubBlockCommon.Instance.EnumerateFormLinks(this, iterateNestedRecords);

    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
        => WorldspaceSubBlockCommon.Instance.EnumerateAssetLinks(this, queryCategories, linkCache, assetType);

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
        => WorldspaceSubBlockCommon.Instance.EnumerateMajorRecords(this);

    public IEnumerable<T> EnumerateMajorRecords<T>(bool throwIfUnknown = true) where T : class, IMajorRecordQueryableGetter
        => WorldspaceSubBlockCommon.Instance.EnumerateMajorRecords(this, typeof(T), throwIfUnknown).Select(m => (T)m);

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
        => WorldspaceSubBlockCommon.Instance.EnumerateMajorRecords(this, type, throwIfUnknown);
}

/// <summary>
/// Merged cell that combines Persistent and Temporary placed objects from multiple cells with the same FormKey.
/// </summary>
internal class MergedWorldspaceCell : ICellGetter
{
    private readonly ICellGetter _primary;
    private readonly List<ICellGetter> _allCopies;

    public MergedWorldspaceCell(List<ICellGetter> allCopies)
    {
        _primary = allCopies[^1];
        _allCopies = allCopies;
    }

    public IReadOnlyList<IPlacedGetter> Persistent => _allCopies.SelectMany(c => c.Persistent).ToList();
    public IReadOnlyList<IPlacedGetter> Temporary => _allCopies.SelectMany(c => c.Temporary).ToList();

    public FormKey FormKey => _primary.FormKey;
    public string? EditorID => _primary.EditorID;
    public int MajorRecordFlagsRaw => _primary.MajorRecordFlagsRaw;
    public uint VersionControl => _primary.VersionControl;
    ushort IFallout4MajorRecordGetter.FormVersion => _primary.FormVersion;
    ushort? IMajorRecordGetter.FormVersion => _primary.FormVersion;
    ushort? IFormVersionGetter.FormVersion => _primary.FormVersion;
    public ushort Version2 => _primary.Version2;
    public Fallout4MajorRecord.Fallout4MajorRecordFlag Fallout4MajorRecordFlags => _primary.Fallout4MajorRecordFlags;
    public Cell.MajorFlag MajorFlags => _primary.MajorFlags;
    public bool IsCompressed => (MajorRecordFlagsRaw & Mutagen.Bethesda.Plugins.Internals.Constants.CompressedFlag) != 0;
    public bool IsDeleted => (MajorRecordFlagsRaw & Mutagen.Bethesda.Plugins.Internals.Constants.DeletedFlag) != 0;
    Type ILinkIdentifier.Type => typeof(ICellGetter);
    public bool Equals(IFormLinkGetter? other) => other != null && other.FormKey == FormKey && typeof(ICellGetter).IsAssignableFrom(other.Type);

    public ITranslatedStringGetter? Name => _primary.Name;
    public Cell.Flag Flags => _primary.Flags;
    public UInt16? PreVisFilesTimestamp => _primary.PreVisFilesTimestamp;
    public IFormLinkNullableGetter<ICellGetter> InPreVisFileOf => _primary.InPreVisFileOf;
    public UInt16? PreCombinedFilesTimestamp => _primary.PreCombinedFilesTimestamp;
    public ICellGridGetter? Grid => _primary.Grid;
    public ICellLightingGetter? Lighting => _primary.Lighting;
    public Byte? PrecombinedObjectLevelXY => _primary.PrecombinedObjectLevelXY;
    public Byte? PrecombinedObjectLevelZ => _primary.PrecombinedObjectLevelZ;
    public ReadOnlyMemorySlice<Byte>? TVDT => _primary.TVDT;
    public ICellMaxHeightDataGetter? MaxHeightData => _primary.MaxHeightData;
    public IFormLinkGetter<ILightingTemplateGetter> LightingTemplate => _primary.LightingTemplate;
    public Single? WaterHeight => _primary.WaterHeight;
    public IReadOnlyList<IFormLinkGetter<IRegionGetter>>? Regions => _primary.Regions;
    public IFormLinkNullableGetter<ILocationGetter> Location => _primary.Location;
    public ReadOnlyMemorySlice<Byte>? XWCN => _primary.XWCN;
    public ICellWaterVelocityGetter? WaterVelocity => _primary.WaterVelocity;
    public IFormLinkNullableGetter<IWaterGetter> Water => _primary.Water;
    public IOwnershipGetter? Ownership => _primary.Ownership;
    public Int32? FactionRank => _primary.FactionRank;
    public IFormLinkNullableGetter<ILockListGetter> LockList => _primary.LockList;
    public ICellExteriorLodGetter? ExteriorLod => _primary.ExteriorLod;
    public String? WaterEnvironmentMap => _primary.WaterEnvironmentMap;
    public IFormLinkNullableGetter<IRegionGetter> SkyAndWeatherFromRegion => _primary.SkyAndWeatherFromRegion;
    public IFormLinkNullableGetter<IAcousticSpaceGetter> AcousticSpace => _primary.AcousticSpace;
    public IFormLinkNullableGetter<IEncounterZoneGetter> EncounterZone => _primary.EncounterZone;
    public IFormLinkNullableGetter<IMusicTypeGetter> Music => _primary.Music;
    public IFormLinkNullableGetter<IImageSpaceGetter> ImageSpace => _primary.ImageSpace;
    public IFormLinkNullableGetter<IGodRaysGetter> GodRays => _primary.GodRays;
    public IReadOnlyList<IFormLinkGetter<IPlacedThingGetter>>? PhysicsReferences => _primary.PhysicsReferences;
    public IReadOnlyList<UInt32> CombinedMeshes => _primary.CombinedMeshes;
    public IReadOnlyList<ICellCombinedMeshReferenceGetter> CombinedMeshReferences => _primary.CombinedMeshReferences;
    public ILandscapeGetter? Landscape => _primary.Landscape;
    public IReadOnlyList<INavigationMeshGetter> NavigationMeshes => _primary.NavigationMeshes;
    public Int32 Timestamp => _primary.Timestamp;
    public Int32 UnknownGroupData => _primary.UnknownGroupData;
    public Int32 PersistentTimestamp => _primary.PersistentTimestamp;
    public Int32 PersistentUnknownGroupData => _primary.PersistentUnknownGroupData;
    public Int32 TemporaryTimestamp => _primary.TemporaryTimestamp;
    public Int32 TemporaryUnknownGroupData => _primary.TemporaryUnknownGroupData;

    ITranslatedStringGetter? ITranslatedNamedGetter.Name => _primary.Name;
    ITranslatedStringGetter ITranslatedNamedRequiredGetter.Name => _primary.Name ?? TranslatedString.Empty;
    string? INamedGetter.Name => _primary.Name?.String;
    string INamedRequiredGetter.Name => _primary.Name?.String ?? string.Empty;

    ILoquiRegistration ILoquiObject.Registration => Cell_Registration.Instance;
    public object CommonInstance() => CellCommon.Instance;
    public object? CommonSetterInstance() => null;
    public object CommonSetterTranslationInstance() => CellSetterTranslationCommon.Instance;

    public void Print(StructuredStringBuilder sb, string? name) => CellCommon.Instance.Print(this, sb, name);

    object IBinaryItem.BinaryWriteTranslator => CellBinaryWriteTranslation.Instance;
    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
        => CellBinaryWriteTranslation.Instance.Write(writer: writer, item: this, translationParams: translationParams);

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true)
        => CellCommon.Instance.EnumerateFormLinks(this, iterateNestedRecords);
    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
        => CellCommon.Instance.EnumerateAssetLinks(this, queryCategories, linkCache, assetType);

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
        => CellCommon.Instance.EnumerateMajorRecords(this);

    public IEnumerable<T> EnumerateMajorRecords<T>(bool throwIfUnknown = true) where T : class, IMajorRecordQueryableGetter
        => CellCommon.Instance.EnumerateMajorRecords(this, typeof(T), throwIfUnknown).Select(m => (T)m);

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
        => CellCommon.Instance.EnumerateMajorRecords(this, type, throwIfUnknown);
}

