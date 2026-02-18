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

namespace Mutagen.Bethesda.Starfield;

/// <summary>
/// Multi-mod overlay that presents multiple Starfield mods as a single unified mod.
/// Typically used for reading split mods that were written due to exceeding master limits,
/// but can be used with any collection of mods.
/// </summary>
internal class StarfieldMultiModOverlay : IStarfieldModDisposableGetter
{
    private readonly IReadOnlyList<IStarfieldModGetter> _sourceMods;
    private readonly IReadOnlyList<IModDisposeGetter>? _disposeSourceMods;
    private readonly ModKey _modKey;
    private readonly IReadOnlyList<IMasterReferenceGetter> _masters;

    private MergedGroup<IGameSettingGetter>? _gameSettings;
    private MergedGroup<IKeywordGetter>? _keywords;
    private MergedGroup<IFormFolderKeywordListGetter>? _formFolderKeywordLists;
    private MergedGroup<ILocationReferenceTypeGetter>? _locationReferenceTypes;
    private MergedGroup<IActionRecordGetter>? _actions;
    private MergedGroup<ITransformGetter>? _transforms;
    private MergedGroup<ITextureSetGetter>? _textureSets;
    private MergedGroup<IGlobalGetter>? _globals;
    private MergedGroup<IDamageTypeGetter>? _damageTypes;
    private MergedGroup<IClassGetter>? _classes;
    private MergedGroup<IFactionGetter>? _factions;
    private MergedGroup<IAffinityEventGetter>? _affinityEvents;
    private MergedGroup<IHeadPartGetter>? _headParts;
    private MergedGroup<IRaceGetter>? _races;
    private MergedGroup<ISoundMarkerGetter>? _soundMarkers;
    private MergedGroup<ISoundEchoMarkerGetter>? _soundEchoMarkers;
    private MergedGroup<IAcousticSpaceGetter>? _acousticSpaces;
    private MergedGroup<IAudioOcclusionPrimitiveGetter>? _audioOcclusionPrimitives;
    private MergedGroup<IMagicEffectGetter>? _magicEffects;
    private MergedGroup<ILandscapeTextureGetter>? _landscapeTextures;
    private MergedGroup<IProjectedDecalGetter>? _projectedDecals;
    private MergedGroup<IObjectEffectGetter>? _objectEffects;
    private MergedGroup<ISpellGetter>? _spells;
    private MergedGroup<IActivatorGetter>? _activators;
    private MergedGroup<ICurveTableGetter>? _curveTables;
    private MergedGroup<ICurve3DGetter>? _curve3Ds;
    private MergedGroup<IArmorGetter>? _armors;
    private MergedGroup<IBookGetter>? _books;
    private MergedGroup<IContainerGetter>? _containers;
    private MergedGroup<IDoorGetter>? _doors;
    private MergedGroup<ILightGetter>? _lights;
    private MergedGroup<IMiscItemGetter>? _miscItems;
    private MergedGroup<IStaticGetter>? _statics;
    private MergedGroup<IStaticCollectionGetter>? _staticCollections;
    private MergedGroup<IPackInGetter>? _packIns;
    private MergedGroup<IMoveableStaticGetter>? _moveableStatics;
    private MergedGroup<IGrassGetter>? _grasses;
    private MergedGroup<IFloraGetter>? _florae;
    private MergedGroup<IFurnitureGetter>? _furniture;
    private MergedGroup<IWeaponGetter>? _weapons;
    private MergedGroup<IAmmunitionGetter>? _ammunitions;
    private MergedGroup<INpcGetter>? _npcs;
    private MergedGroup<ILeveledNpcGetter>? _leveledNpcs;
    private MergedGroup<ILeveledPackInGetter>? _leveledPackIns;
    private MergedGroup<IKeyGetter>? _keys;
    private MergedGroup<IIngestibleGetter>? _ingestibles;
    private MergedGroup<IIdleMarkerGetter>? _idleMarkers;
    private MergedGroup<IBiomeMarkerGetter>? _biomeMarkers;
    private MergedGroup<INoteGetter>? _notes;
    private MergedGroup<IProjectileGetter>? _projectiles;
    private MergedGroup<IHazardGetter>? _hazards;
    private MergedGroup<IBendableSplineGetter>? _bendableSplines;
    private MergedGroup<ITerminalGetter>? _terminals;
    private MergedGroup<ILeveledItemGetter>? _leveledItems;
    private MergedGroup<IGenericBaseFormTemplateGetter>? _genericBaseFormTemplates;
    private MergedGroup<IGenericBaseFormGetter>? _genericBaseForms;
    private MergedGroup<ILeveledBaseFormGetter>? _leveledBaseForms;
    private MergedGroup<IWeatherGetter>? _weathers;
    private MergedGroup<IWeatherSettingGetter>? _weatherSettings;
    private MergedGroup<IClimateGetter>? _climates;
    private MergedGroup<IShaderParticleGeometryGetter>? _shaderParticleGeometries;
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
    private MergedGroup<IEquipTypeGetter>? _equipTypes;
    private MergedGroup<IOutfitGetter>? _outfits;
    private MergedGroup<IArtObjectGetter>? _artObjects;
    private MergedGroup<IMovementTypeGetter>? _movementTypes;
    private MergedGroup<ICollisionLayerGetter>? _collisionLayers;
    private MergedGroup<IColorRecordGetter>? _colors;
    private MergedGroup<IReverbParametersGetter>? _reverbParameters;
    private MergedGroup<IReferenceGroupGetter>? _referenceGroups;
    private MergedGroup<IAimModelGetter>? _aimModels;
    private MergedGroup<IAimAssistModelGetter>? _aimAssistModels;
    private MergedGroup<IMeleeAimAssistModelGetter>? _meleeAimAssistModels;
    private MergedGroup<ILayerGetter>? _layers;
    private MergedGroup<IConstructibleObjectGetter>? _constructibleObjects;
    private MergedGroup<IAObjectModificationGetter>? _objectModifications;
    private MergedGroup<IZoomGetter>? _zooms;
    private MergedGroup<IInstanceNamingRulesGetter>? _instanceNamingRules;
    private MergedGroup<ISoundKeywordMappingGetter>? _soundKeywordMappings;
    private MergedGroup<IAttractionRuleGetter>? _attractionRules;
    private MergedGroup<ISceneCollectionGetter>? _sceneCollections;
    private MergedGroup<IAnimationSoundTagSetGetter>? _animationSoundTagSets;
    private MergedGroup<IResourceGetter>? _resources;
    private MergedGroup<IBiomeGetter>? _biomes;
    private MergedGroup<INavigationMeshObstacleCoverManagerGetter>? _navigationMeshObstacleCoverManagers;
    private MergedGroup<ILensFlareGetter>? _lensFlares;
    private MergedGroup<IObjectVisibilityManagerGetter>? _objectVisibilityManagers;
    private MergedGroup<ISnapTemplateNodeGetter>? _snapTemplateNodes;
    private MergedGroup<ISnapTemplateGetter>? _snapTemplates;
    private MergedGroup<IGroundCoverGetter>? _groundCovers;
    private MergedGroup<IMorphableObjectGetter>? _morphableObjects;
    private MergedGroup<ITraversalGetter>? _traversals;
    private MergedGroup<IResourceGenerationDataGetter>? _resourceGenerationData;
    private MergedGroup<IObjectSwapGetter>? _objectSwaps;
    private MergedGroup<IAtmosphereGetter>? _atmospheres;
    private MergedGroup<ILeveledSpaceCellGetter>? _leveledSpaceCells;
    private MergedGroup<ISpeechChallengeGetter>? _speechChallenges;
    private MergedGroup<IAimAssistPoseGetter>? _aimAssistPoses;
    private MergedGroup<IVolumetricLightingGetter>? _volumetricLightings;
    private MergedGroup<ISurfaceBlockGetter>? _surfaceBlocks;
    private MergedGroup<ISurfacePatternConfigGetter>? _surfacePatternConfigs;
    private MergedGroup<ISurfacePatternGetter>? _surfacePatterns;
    private MergedGroup<ISurfaceTreeGetter>? _surfaceTrees;
    private MergedGroup<IPlanetContentManagerTreeGetter>? _planetContentManagerTrees;
    private MergedGroup<IBoneModifierGetter>? _boneModifiers;
    private MergedGroup<ISnapTemplateBehaviorGetter>? _snapTemplateBehaviors;
    private MergedGroup<IPlanetGetter>? _planets;
    private MergedGroup<IConditionRecordGetter>? _conditionRecords;
    private MergedGroup<IPlanetContentManagerBranchNodeGetter>? _planetContentManagerBranchNodes;
    private MergedGroup<IPlanetContentManagerContentNodeGetter>? _planetContentManagerContentNodes;
    private MergedGroup<IStarGetter>? _stars;
    private MergedGroup<IWWiseEventDataGetter>? _wWiseEventDatas;
    private MergedGroup<IResearchProjectGetter>? _researchProjects;
    private MergedGroup<IAimOpticalSightMarkerGetter>? _aimOpticalSightMarkers;
    private MergedGroup<IAmbienceSetGetter>? _ambienceSets;
    private MergedGroup<IWeaponBarrelModelGetter>? _weaponBarrelModels;
    private MergedGroup<ISurfacePatternStyleGetter>? _surfacePatternStyles;
    private MergedGroup<ILayeredMaterialSwapGetter>? _layeredMaterialSwaps;
    private MergedGroup<IForceDataGetter>? _forceDatas;
    private MergedGroup<ITerminalMenuGetter>? _terminalMenus;
    private MergedGroup<IEffectSequenceGetter>? _effectSequences;
    private MergedGroup<ISecondaryDamageListGetter>? _secondaryDamageLists;
    private MergedGroup<IMaterialPathGetter>? _materialPaths;
    private MergedGroup<ICloudsGetter>? _clouds;
    private MergedGroup<IFogVolumeGetter>? _fogVolumes;
    private MergedGroup<IWWiseKeywordMappingGetter>? _wWiseKeywordMappings;
    private MergedGroup<ILegendaryItemGetter>? _legendaryItems;
    private MergedGroup<IParticleSystemDefineCollisionGetter>? _particleSystemDefineCollisions;
    private MergedGroup<ISunPresetGetter>? _sunPresets;
    private MergedGroup<IPhotoModeFeatureGetter>? _photoModeFeatures;
    private MergedGroup<IGameplayOptionGetter>? _gameplayOptions;
    private MergedGroup<IGameplayOptionsGroupGetter>? _gameplayOptionsGroups;
    private MergedGroup<ITimeOfDayRecordGetter>? _timeOfDays;
    private MergedGroup<IActorValueModulationGetter>? _actorValueModulations;
    private MergedGroup<IChallengeGetter>? _challenges;
    private MergedGroup<IFacialExpressionGetter>? _facialExpressions;
    private MergedGroup<IPERSGetter>? _pERS;

    /// <summary>
    /// Creates a new StarfieldMultiModOverlay from multiple source mod files.
    /// </summary>
    public StarfieldMultiModOverlay(
        ModKey modKey,
        IEnumerable<IStarfieldModGetter> sourceMods,
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
    public IStarfieldModHeaderGetter ModHeader => _sourceMods[0].ModHeader;
    public IReadOnlyList<IMasterReferenceGetter> MasterReferences => _masters;
    public StarfieldRelease StarfieldRelease => _sourceMods[0].StarfieldRelease;
    GameRelease IModGetter.GameRelease => StarfieldRelease.ToGameRelease();

    public object CommonInstance() => StarfieldModCommon.Instance;
    public object? CommonSetterInstance() => StarfieldModSetterCommon.Instance;
    public object CommonSetterTranslationInstance() => StarfieldModSetterTranslationCommon.Instance;


    public IStarfieldGroupGetter<IGameSettingGetter> GameSettings =>
        _gameSettings ??= new MergedGroup<IGameSettingGetter>(
            _sourceMods.Select(m => m.GameSettings));
    public IStarfieldGroupGetter<IKeywordGetter> Keywords =>
        _keywords ??= new MergedGroup<IKeywordGetter>(
            _sourceMods.Select(m => m.Keywords));
    public IStarfieldGroupGetter<IFormFolderKeywordListGetter> FormFolderKeywordLists =>
        _formFolderKeywordLists ??= new MergedGroup<IFormFolderKeywordListGetter>(
            _sourceMods.Select(m => m.FormFolderKeywordLists));
    public IStarfieldGroupGetter<ILocationReferenceTypeGetter> LocationReferenceTypes =>
        _locationReferenceTypes ??= new MergedGroup<ILocationReferenceTypeGetter>(
            _sourceMods.Select(m => m.LocationReferenceTypes));
    public IStarfieldGroupGetter<IActionRecordGetter> Actions =>
        _actions ??= new MergedGroup<IActionRecordGetter>(
            _sourceMods.Select(m => m.Actions));
    public IStarfieldGroupGetter<ITransformGetter> Transforms =>
        _transforms ??= new MergedGroup<ITransformGetter>(
            _sourceMods.Select(m => m.Transforms));
    public IStarfieldGroupGetter<ITextureSetGetter> TextureSets =>
        _textureSets ??= new MergedGroup<ITextureSetGetter>(
            _sourceMods.Select(m => m.TextureSets));
    public IStarfieldGroupGetter<IGlobalGetter> Globals =>
        _globals ??= new MergedGroup<IGlobalGetter>(
            _sourceMods.Select(m => m.Globals));
    public IStarfieldGroupGetter<IDamageTypeGetter> DamageTypes =>
        _damageTypes ??= new MergedGroup<IDamageTypeGetter>(
            _sourceMods.Select(m => m.DamageTypes));
    public IStarfieldGroupGetter<IClassGetter> Classes =>
        _classes ??= new MergedGroup<IClassGetter>(
            _sourceMods.Select(m => m.Classes));
    public IStarfieldGroupGetter<IFactionGetter> Factions =>
        _factions ??= new MergedGroup<IFactionGetter>(
            _sourceMods.Select(m => m.Factions));
    public IStarfieldGroupGetter<IAffinityEventGetter> AffinityEvents =>
        _affinityEvents ??= new MergedGroup<IAffinityEventGetter>(
            _sourceMods.Select(m => m.AffinityEvents));
    public IStarfieldGroupGetter<IHeadPartGetter> HeadParts =>
        _headParts ??= new MergedGroup<IHeadPartGetter>(
            _sourceMods.Select(m => m.HeadParts));
    public IStarfieldGroupGetter<IRaceGetter> Races =>
        _races ??= new MergedGroup<IRaceGetter>(
            _sourceMods.Select(m => m.Races));
    public IStarfieldGroupGetter<ISoundMarkerGetter> SoundMarkers =>
        _soundMarkers ??= new MergedGroup<ISoundMarkerGetter>(
            _sourceMods.Select(m => m.SoundMarkers));
    public IStarfieldGroupGetter<ISoundEchoMarkerGetter> SoundEchoMarkers =>
        _soundEchoMarkers ??= new MergedGroup<ISoundEchoMarkerGetter>(
            _sourceMods.Select(m => m.SoundEchoMarkers));
    public IStarfieldGroupGetter<IAcousticSpaceGetter> AcousticSpaces =>
        _acousticSpaces ??= new MergedGroup<IAcousticSpaceGetter>(
            _sourceMods.Select(m => m.AcousticSpaces));
    public IStarfieldGroupGetter<IAudioOcclusionPrimitiveGetter> AudioOcclusionPrimitives =>
        _audioOcclusionPrimitives ??= new MergedGroup<IAudioOcclusionPrimitiveGetter>(
            _sourceMods.Select(m => m.AudioOcclusionPrimitives));
    public IStarfieldGroupGetter<IMagicEffectGetter> MagicEffects =>
        _magicEffects ??= new MergedGroup<IMagicEffectGetter>(
            _sourceMods.Select(m => m.MagicEffects));
    public IStarfieldGroupGetter<ILandscapeTextureGetter> LandscapeTextures =>
        _landscapeTextures ??= new MergedGroup<ILandscapeTextureGetter>(
            _sourceMods.Select(m => m.LandscapeTextures));
    public IStarfieldGroupGetter<IProjectedDecalGetter> ProjectedDecals =>
        _projectedDecals ??= new MergedGroup<IProjectedDecalGetter>(
            _sourceMods.Select(m => m.ProjectedDecals));
    public IStarfieldGroupGetter<IObjectEffectGetter> ObjectEffects =>
        _objectEffects ??= new MergedGroup<IObjectEffectGetter>(
            _sourceMods.Select(m => m.ObjectEffects));
    public IStarfieldGroupGetter<ISpellGetter> Spells =>
        _spells ??= new MergedGroup<ISpellGetter>(
            _sourceMods.Select(m => m.Spells));
    public IStarfieldGroupGetter<IActivatorGetter> Activators =>
        _activators ??= new MergedGroup<IActivatorGetter>(
            _sourceMods.Select(m => m.Activators));
    public IStarfieldGroupGetter<ICurveTableGetter> CurveTables =>
        _curveTables ??= new MergedGroup<ICurveTableGetter>(
            _sourceMods.Select(m => m.CurveTables));
    public IStarfieldGroupGetter<ICurve3DGetter> Curve3Ds =>
        _curve3Ds ??= new MergedGroup<ICurve3DGetter>(
            _sourceMods.Select(m => m.Curve3Ds));
    public IStarfieldGroupGetter<IArmorGetter> Armors =>
        _armors ??= new MergedGroup<IArmorGetter>(
            _sourceMods.Select(m => m.Armors));
    public IStarfieldGroupGetter<IBookGetter> Books =>
        _books ??= new MergedGroup<IBookGetter>(
            _sourceMods.Select(m => m.Books));
    public IStarfieldGroupGetter<IContainerGetter> Containers =>
        _containers ??= new MergedGroup<IContainerGetter>(
            _sourceMods.Select(m => m.Containers));
    public IStarfieldGroupGetter<IDoorGetter> Doors =>
        _doors ??= new MergedGroup<IDoorGetter>(
            _sourceMods.Select(m => m.Doors));
    public IStarfieldGroupGetter<ILightGetter> Lights =>
        _lights ??= new MergedGroup<ILightGetter>(
            _sourceMods.Select(m => m.Lights));
    public IStarfieldGroupGetter<IMiscItemGetter> MiscItems =>
        _miscItems ??= new MergedGroup<IMiscItemGetter>(
            _sourceMods.Select(m => m.MiscItems));
    public IStarfieldGroupGetter<IStaticGetter> Statics =>
        _statics ??= new MergedGroup<IStaticGetter>(
            _sourceMods.Select(m => m.Statics));
    public IStarfieldGroupGetter<IStaticCollectionGetter> StaticCollections =>
        _staticCollections ??= new MergedGroup<IStaticCollectionGetter>(
            _sourceMods.Select(m => m.StaticCollections));
    public IStarfieldGroupGetter<IPackInGetter> PackIns =>
        _packIns ??= new MergedGroup<IPackInGetter>(
            _sourceMods.Select(m => m.PackIns));
    public IStarfieldGroupGetter<IMoveableStaticGetter> MoveableStatics =>
        _moveableStatics ??= new MergedGroup<IMoveableStaticGetter>(
            _sourceMods.Select(m => m.MoveableStatics));
    public IStarfieldGroupGetter<IGrassGetter> Grasses =>
        _grasses ??= new MergedGroup<IGrassGetter>(
            _sourceMods.Select(m => m.Grasses));
    public IStarfieldGroupGetter<IFloraGetter> Florae =>
        _florae ??= new MergedGroup<IFloraGetter>(
            _sourceMods.Select(m => m.Florae));
    public IStarfieldGroupGetter<IFurnitureGetter> Furniture =>
        _furniture ??= new MergedGroup<IFurnitureGetter>(
            _sourceMods.Select(m => m.Furniture));
    public IStarfieldGroupGetter<IWeaponGetter> Weapons =>
        _weapons ??= new MergedGroup<IWeaponGetter>(
            _sourceMods.Select(m => m.Weapons));
    public IStarfieldGroupGetter<IAmmunitionGetter> Ammunitions =>
        _ammunitions ??= new MergedGroup<IAmmunitionGetter>(
            _sourceMods.Select(m => m.Ammunitions));
    public IStarfieldGroupGetter<INpcGetter> Npcs =>
        _npcs ??= new MergedGroup<INpcGetter>(
            _sourceMods.Select(m => m.Npcs));
    public IStarfieldGroupGetter<ILeveledNpcGetter> LeveledNpcs =>
        _leveledNpcs ??= new MergedGroup<ILeveledNpcGetter>(
            _sourceMods.Select(m => m.LeveledNpcs));
    public IStarfieldGroupGetter<ILeveledPackInGetter> LeveledPackIns =>
        _leveledPackIns ??= new MergedGroup<ILeveledPackInGetter>(
            _sourceMods.Select(m => m.LeveledPackIns));
    public IStarfieldGroupGetter<IKeyGetter> Keys =>
        _keys ??= new MergedGroup<IKeyGetter>(
            _sourceMods.Select(m => m.Keys));
    public IStarfieldGroupGetter<IIngestibleGetter> Ingestibles =>
        _ingestibles ??= new MergedGroup<IIngestibleGetter>(
            _sourceMods.Select(m => m.Ingestibles));
    public IStarfieldGroupGetter<IIdleMarkerGetter> IdleMarkers =>
        _idleMarkers ??= new MergedGroup<IIdleMarkerGetter>(
            _sourceMods.Select(m => m.IdleMarkers));
    public IStarfieldGroupGetter<IBiomeMarkerGetter> BiomeMarkers =>
        _biomeMarkers ??= new MergedGroup<IBiomeMarkerGetter>(
            _sourceMods.Select(m => m.BiomeMarkers));
    public IStarfieldGroupGetter<INoteGetter> Notes =>
        _notes ??= new MergedGroup<INoteGetter>(
            _sourceMods.Select(m => m.Notes));
    public IStarfieldGroupGetter<IProjectileGetter> Projectiles =>
        _projectiles ??= new MergedGroup<IProjectileGetter>(
            _sourceMods.Select(m => m.Projectiles));
    public IStarfieldGroupGetter<IHazardGetter> Hazards =>
        _hazards ??= new MergedGroup<IHazardGetter>(
            _sourceMods.Select(m => m.Hazards));
    public IStarfieldGroupGetter<IBendableSplineGetter> BendableSplines =>
        _bendableSplines ??= new MergedGroup<IBendableSplineGetter>(
            _sourceMods.Select(m => m.BendableSplines));
    public IStarfieldGroupGetter<ITerminalGetter> Terminals =>
        _terminals ??= new MergedGroup<ITerminalGetter>(
            _sourceMods.Select(m => m.Terminals));
    public IStarfieldGroupGetter<ILeveledItemGetter> LeveledItems =>
        _leveledItems ??= new MergedGroup<ILeveledItemGetter>(
            _sourceMods.Select(m => m.LeveledItems));
    public IStarfieldGroupGetter<IGenericBaseFormTemplateGetter> GenericBaseFormTemplates =>
        _genericBaseFormTemplates ??= new MergedGroup<IGenericBaseFormTemplateGetter>(
            _sourceMods.Select(m => m.GenericBaseFormTemplates));
    public IStarfieldGroupGetter<IGenericBaseFormGetter> GenericBaseForms =>
        _genericBaseForms ??= new MergedGroup<IGenericBaseFormGetter>(
            _sourceMods.Select(m => m.GenericBaseForms));
    public IStarfieldGroupGetter<ILeveledBaseFormGetter> LeveledBaseForms =>
        _leveledBaseForms ??= new MergedGroup<ILeveledBaseFormGetter>(
            _sourceMods.Select(m => m.LeveledBaseForms));
    public IStarfieldGroupGetter<IWeatherGetter> Weathers =>
        _weathers ??= new MergedGroup<IWeatherGetter>(
            _sourceMods.Select(m => m.Weathers));
    public IStarfieldGroupGetter<IWeatherSettingGetter> WeatherSettings =>
        _weatherSettings ??= new MergedGroup<IWeatherSettingGetter>(
            _sourceMods.Select(m => m.WeatherSettings));
    public IStarfieldGroupGetter<IClimateGetter> Climates =>
        _climates ??= new MergedGroup<IClimateGetter>(
            _sourceMods.Select(m => m.Climates));
    public IStarfieldGroupGetter<IShaderParticleGeometryGetter> ShaderParticleGeometries =>
        _shaderParticleGeometries ??= new MergedGroup<IShaderParticleGeometryGetter>(
            _sourceMods.Select(m => m.ShaderParticleGeometries));
    public IStarfieldGroupGetter<IRegionGetter> Regions =>
        _regions ??= new MergedGroup<IRegionGetter>(
            _sourceMods.Select(m => m.Regions));
    public IStarfieldGroupGetter<INavigationMeshInfoMapGetter> NavigationMeshInfoMaps =>
        _navigationMeshInfoMaps ??= new MergedGroup<INavigationMeshInfoMapGetter>(
            _sourceMods.Select(m => m.NavigationMeshInfoMaps));
    public IStarfieldListGroupGetter<ICellBlockGetter> Cells =>
        _cells ??= new MergedListGroup(_sourceMods.Select(m => m.Cells));
    public IStarfieldGroupGetter<IWorldspaceGetter> Worldspaces =>
        _worldspaces ??= new MergedGroup<IWorldspaceGetter>(
            _sourceMods.Select(m => m.Worldspaces));
    public IStarfieldGroupGetter<IQuestGetter> Quests =>
        _quests ??= new MergedGroup<IQuestGetter>(
            _sourceMods.Select(m => m.Quests));
    public IStarfieldGroupGetter<IIdleAnimationGetter> IdleAnimations =>
        _idleAnimations ??= new MergedGroup<IIdleAnimationGetter>(
            _sourceMods.Select(m => m.IdleAnimations));
    public IStarfieldGroupGetter<IPackageGetter> Packages =>
        _packages ??= new MergedGroup<IPackageGetter>(
            _sourceMods.Select(m => m.Packages));
    public IStarfieldGroupGetter<ICombatStyleGetter> CombatStyles =>
        _combatStyles ??= new MergedGroup<ICombatStyleGetter>(
            _sourceMods.Select(m => m.CombatStyles));
    public IStarfieldGroupGetter<ILoadScreenGetter> LoadScreens =>
        _loadScreens ??= new MergedGroup<ILoadScreenGetter>(
            _sourceMods.Select(m => m.LoadScreens));
    public IStarfieldGroupGetter<IAnimatedObjectGetter> AnimatedObjects =>
        _animatedObjects ??= new MergedGroup<IAnimatedObjectGetter>(
            _sourceMods.Select(m => m.AnimatedObjects));
    public IStarfieldGroupGetter<IWaterGetter> Waters =>
        _waters ??= new MergedGroup<IWaterGetter>(
            _sourceMods.Select(m => m.Waters));
    public IStarfieldGroupGetter<IEffectShaderGetter> EffectShaders =>
        _effectShaders ??= new MergedGroup<IEffectShaderGetter>(
            _sourceMods.Select(m => m.EffectShaders));
    public IStarfieldGroupGetter<IExplosionGetter> Explosions =>
        _explosions ??= new MergedGroup<IExplosionGetter>(
            _sourceMods.Select(m => m.Explosions));
    public IStarfieldGroupGetter<IDebrisGetter> Debris =>
        _debris ??= new MergedGroup<IDebrisGetter>(
            _sourceMods.Select(m => m.Debris));
    public IStarfieldGroupGetter<IImageSpaceGetter> ImageSpaces =>
        _imageSpaces ??= new MergedGroup<IImageSpaceGetter>(
            _sourceMods.Select(m => m.ImageSpaces));
    public IStarfieldGroupGetter<IImageSpaceAdapterGetter> ImageSpaceAdapters =>
        _imageSpaceAdapters ??= new MergedGroup<IImageSpaceAdapterGetter>(
            _sourceMods.Select(m => m.ImageSpaceAdapters));
    public IStarfieldGroupGetter<IFormListGetter> FormLists =>
        _formLists ??= new MergedGroup<IFormListGetter>(
            _sourceMods.Select(m => m.FormLists));
    public IStarfieldGroupGetter<IPerkGetter> Perks =>
        _perks ??= new MergedGroup<IPerkGetter>(
            _sourceMods.Select(m => m.Perks));
    public IStarfieldGroupGetter<IBodyPartDataGetter> BodyParts =>
        _bodyParts ??= new MergedGroup<IBodyPartDataGetter>(
            _sourceMods.Select(m => m.BodyParts));
    public IStarfieldGroupGetter<IAddonNodeGetter> AddonNodes =>
        _addonNodes ??= new MergedGroup<IAddonNodeGetter>(
            _sourceMods.Select(m => m.AddonNodes));
    public IStarfieldGroupGetter<IActorValueInformationGetter> ActorValueInformation =>
        _actorValueInformation ??= new MergedGroup<IActorValueInformationGetter>(
            _sourceMods.Select(m => m.ActorValueInformation));
    public IStarfieldGroupGetter<ICameraShotGetter> CameraShots =>
        _cameraShots ??= new MergedGroup<ICameraShotGetter>(
            _sourceMods.Select(m => m.CameraShots));
    public IStarfieldGroupGetter<ICameraPathGetter> CameraPaths =>
        _cameraPaths ??= new MergedGroup<ICameraPathGetter>(
            _sourceMods.Select(m => m.CameraPaths));
    public IStarfieldGroupGetter<IVoiceTypeGetter> VoiceTypes =>
        _voiceTypes ??= new MergedGroup<IVoiceTypeGetter>(
            _sourceMods.Select(m => m.VoiceTypes));
    public IStarfieldGroupGetter<IMaterialTypeGetter> MaterialTypes =>
        _materialTypes ??= new MergedGroup<IMaterialTypeGetter>(
            _sourceMods.Select(m => m.MaterialTypes));
    public IStarfieldGroupGetter<IImpactGetter> Impacts =>
        _impacts ??= new MergedGroup<IImpactGetter>(
            _sourceMods.Select(m => m.Impacts));
    public IStarfieldGroupGetter<IImpactDataSetGetter> ImpactDataSets =>
        _impactDataSets ??= new MergedGroup<IImpactDataSetGetter>(
            _sourceMods.Select(m => m.ImpactDataSets));
    public IStarfieldGroupGetter<IArmorAddonGetter> ArmorAddons =>
        _armorAddons ??= new MergedGroup<IArmorAddonGetter>(
            _sourceMods.Select(m => m.ArmorAddons));
    public IStarfieldGroupGetter<ILocationGetter> Locations =>
        _locations ??= new MergedGroup<ILocationGetter>(
            _sourceMods.Select(m => m.Locations));
    public IStarfieldGroupGetter<IMessageGetter> Messages =>
        _messages ??= new MergedGroup<IMessageGetter>(
            _sourceMods.Select(m => m.Messages));
    public IStarfieldGroupGetter<IDefaultObjectManagerGetter> DefaultObjectManagers =>
        _defaultObjectManagers ??= new MergedGroup<IDefaultObjectManagerGetter>(
            _sourceMods.Select(m => m.DefaultObjectManagers));
    public IStarfieldGroupGetter<IDefaultObjectGetter> DefaultObjects =>
        _defaultObjects ??= new MergedGroup<IDefaultObjectGetter>(
            _sourceMods.Select(m => m.DefaultObjects));
    public IStarfieldGroupGetter<ILightingTemplateGetter> LightingTemplates =>
        _lightingTemplates ??= new MergedGroup<ILightingTemplateGetter>(
            _sourceMods.Select(m => m.LightingTemplates));
    public IStarfieldGroupGetter<IMusicTypeGetter> MusicTypes =>
        _musicTypes ??= new MergedGroup<IMusicTypeGetter>(
            _sourceMods.Select(m => m.MusicTypes));
    public IStarfieldGroupGetter<IFootstepGetter> Footsteps =>
        _footsteps ??= new MergedGroup<IFootstepGetter>(
            _sourceMods.Select(m => m.Footsteps));
    public IStarfieldGroupGetter<IFootstepSetGetter> FootstepSets =>
        _footstepSets ??= new MergedGroup<IFootstepSetGetter>(
            _sourceMods.Select(m => m.FootstepSets));
    public IStarfieldGroupGetter<IStoryManagerBranchNodeGetter> StoryManagerBranchNodes =>
        _storyManagerBranchNodes ??= new MergedGroup<IStoryManagerBranchNodeGetter>(
            _sourceMods.Select(m => m.StoryManagerBranchNodes));
    public IStarfieldGroupGetter<IStoryManagerQuestNodeGetter> StoryManagerQuestNodes =>
        _storyManagerQuestNodes ??= new MergedGroup<IStoryManagerQuestNodeGetter>(
            _sourceMods.Select(m => m.StoryManagerQuestNodes));
    public IStarfieldGroupGetter<IStoryManagerEventNodeGetter> StoryManagerEventNodes =>
        _storyManagerEventNodes ??= new MergedGroup<IStoryManagerEventNodeGetter>(
            _sourceMods.Select(m => m.StoryManagerEventNodes));
    public IStarfieldGroupGetter<IMusicTrackGetter> MusicTracks =>
        _musicTracks ??= new MergedGroup<IMusicTrackGetter>(
            _sourceMods.Select(m => m.MusicTracks));
    public IStarfieldGroupGetter<IEquipTypeGetter> EquipTypes =>
        _equipTypes ??= new MergedGroup<IEquipTypeGetter>(
            _sourceMods.Select(m => m.EquipTypes));
    public IStarfieldGroupGetter<IOutfitGetter> Outfits =>
        _outfits ??= new MergedGroup<IOutfitGetter>(
            _sourceMods.Select(m => m.Outfits));
    public IStarfieldGroupGetter<IArtObjectGetter> ArtObjects =>
        _artObjects ??= new MergedGroup<IArtObjectGetter>(
            _sourceMods.Select(m => m.ArtObjects));
    public IStarfieldGroupGetter<IMovementTypeGetter> MovementTypes =>
        _movementTypes ??= new MergedGroup<IMovementTypeGetter>(
            _sourceMods.Select(m => m.MovementTypes));
    public IStarfieldGroupGetter<ICollisionLayerGetter> CollisionLayers =>
        _collisionLayers ??= new MergedGroup<ICollisionLayerGetter>(
            _sourceMods.Select(m => m.CollisionLayers));
    public IStarfieldGroupGetter<IColorRecordGetter> Colors =>
        _colors ??= new MergedGroup<IColorRecordGetter>(
            _sourceMods.Select(m => m.Colors));
    public IStarfieldGroupGetter<IReverbParametersGetter> ReverbParameters =>
        _reverbParameters ??= new MergedGroup<IReverbParametersGetter>(
            _sourceMods.Select(m => m.ReverbParameters));
    public IStarfieldGroupGetter<IReferenceGroupGetter> ReferenceGroups =>
        _referenceGroups ??= new MergedGroup<IReferenceGroupGetter>(
            _sourceMods.Select(m => m.ReferenceGroups));
    public IStarfieldGroupGetter<IAimModelGetter> AimModels =>
        _aimModels ??= new MergedGroup<IAimModelGetter>(
            _sourceMods.Select(m => m.AimModels));
    public IStarfieldGroupGetter<IAimAssistModelGetter> AimAssistModels =>
        _aimAssistModels ??= new MergedGroup<IAimAssistModelGetter>(
            _sourceMods.Select(m => m.AimAssistModels));
    public IStarfieldGroupGetter<IMeleeAimAssistModelGetter> MeleeAimAssistModels =>
        _meleeAimAssistModels ??= new MergedGroup<IMeleeAimAssistModelGetter>(
            _sourceMods.Select(m => m.MeleeAimAssistModels));
    public IStarfieldGroupGetter<ILayerGetter> Layers =>
        _layers ??= new MergedGroup<ILayerGetter>(
            _sourceMods.Select(m => m.Layers));
    public IStarfieldGroupGetter<IConstructibleObjectGetter> ConstructibleObjects =>
        _constructibleObjects ??= new MergedGroup<IConstructibleObjectGetter>(
            _sourceMods.Select(m => m.ConstructibleObjects));
    public IStarfieldGroupGetter<IAObjectModificationGetter> ObjectModifications =>
        _objectModifications ??= new MergedGroup<IAObjectModificationGetter>(
            _sourceMods.Select(m => m.ObjectModifications));
    public IStarfieldGroupGetter<IZoomGetter> Zooms =>
        _zooms ??= new MergedGroup<IZoomGetter>(
            _sourceMods.Select(m => m.Zooms));
    public IStarfieldGroupGetter<IInstanceNamingRulesGetter> InstanceNamingRules =>
        _instanceNamingRules ??= new MergedGroup<IInstanceNamingRulesGetter>(
            _sourceMods.Select(m => m.InstanceNamingRules));
    public IStarfieldGroupGetter<ISoundKeywordMappingGetter> SoundKeywordMappings =>
        _soundKeywordMappings ??= new MergedGroup<ISoundKeywordMappingGetter>(
            _sourceMods.Select(m => m.SoundKeywordMappings));
    public IStarfieldGroupGetter<IAttractionRuleGetter> AttractionRules =>
        _attractionRules ??= new MergedGroup<IAttractionRuleGetter>(
            _sourceMods.Select(m => m.AttractionRules));
    public IStarfieldGroupGetter<ISceneCollectionGetter> SceneCollections =>
        _sceneCollections ??= new MergedGroup<ISceneCollectionGetter>(
            _sourceMods.Select(m => m.SceneCollections));
    public IStarfieldGroupGetter<IAnimationSoundTagSetGetter> AnimationSoundTagSets =>
        _animationSoundTagSets ??= new MergedGroup<IAnimationSoundTagSetGetter>(
            _sourceMods.Select(m => m.AnimationSoundTagSets));
    public IStarfieldGroupGetter<IResourceGetter> Resources =>
        _resources ??= new MergedGroup<IResourceGetter>(
            _sourceMods.Select(m => m.Resources));
    public IStarfieldGroupGetter<IBiomeGetter> Biomes =>
        _biomes ??= new MergedGroup<IBiomeGetter>(
            _sourceMods.Select(m => m.Biomes));
    public IStarfieldGroupGetter<INavigationMeshObstacleCoverManagerGetter> NavigationMeshObstacleCoverManagers =>
        _navigationMeshObstacleCoverManagers ??= new MergedGroup<INavigationMeshObstacleCoverManagerGetter>(
            _sourceMods.Select(m => m.NavigationMeshObstacleCoverManagers));
    public IStarfieldGroupGetter<ILensFlareGetter> LensFlares =>
        _lensFlares ??= new MergedGroup<ILensFlareGetter>(
            _sourceMods.Select(m => m.LensFlares));
    public IStarfieldGroupGetter<IObjectVisibilityManagerGetter> ObjectVisibilityManagers =>
        _objectVisibilityManagers ??= new MergedGroup<IObjectVisibilityManagerGetter>(
            _sourceMods.Select(m => m.ObjectVisibilityManagers));
    public IStarfieldGroupGetter<ISnapTemplateNodeGetter> SnapTemplateNodes =>
        _snapTemplateNodes ??= new MergedGroup<ISnapTemplateNodeGetter>(
            _sourceMods.Select(m => m.SnapTemplateNodes));
    public IStarfieldGroupGetter<ISnapTemplateGetter> SnapTemplates =>
        _snapTemplates ??= new MergedGroup<ISnapTemplateGetter>(
            _sourceMods.Select(m => m.SnapTemplates));
    public IStarfieldGroupGetter<IGroundCoverGetter> GroundCovers =>
        _groundCovers ??= new MergedGroup<IGroundCoverGetter>(
            _sourceMods.Select(m => m.GroundCovers));
    public IStarfieldGroupGetter<IMorphableObjectGetter> MorphableObjects =>
        _morphableObjects ??= new MergedGroup<IMorphableObjectGetter>(
            _sourceMods.Select(m => m.MorphableObjects));
    public IStarfieldGroupGetter<ITraversalGetter> Traversals =>
        _traversals ??= new MergedGroup<ITraversalGetter>(
            _sourceMods.Select(m => m.Traversals));
    public IStarfieldGroupGetter<IResourceGenerationDataGetter> ResourceGenerationData =>
        _resourceGenerationData ??= new MergedGroup<IResourceGenerationDataGetter>(
            _sourceMods.Select(m => m.ResourceGenerationData));
    public IStarfieldGroupGetter<IObjectSwapGetter> ObjectSwaps =>
        _objectSwaps ??= new MergedGroup<IObjectSwapGetter>(
            _sourceMods.Select(m => m.ObjectSwaps));
    public IStarfieldGroupGetter<IAtmosphereGetter> Atmospheres =>
        _atmospheres ??= new MergedGroup<IAtmosphereGetter>(
            _sourceMods.Select(m => m.Atmospheres));
    public IStarfieldGroupGetter<ILeveledSpaceCellGetter> LeveledSpaceCells =>
        _leveledSpaceCells ??= new MergedGroup<ILeveledSpaceCellGetter>(
            _sourceMods.Select(m => m.LeveledSpaceCells));
    public IStarfieldGroupGetter<ISpeechChallengeGetter> SpeechChallenges =>
        _speechChallenges ??= new MergedGroup<ISpeechChallengeGetter>(
            _sourceMods.Select(m => m.SpeechChallenges));
    public IStarfieldGroupGetter<IAimAssistPoseGetter> AimAssistPoses =>
        _aimAssistPoses ??= new MergedGroup<IAimAssistPoseGetter>(
            _sourceMods.Select(m => m.AimAssistPoses));
    public IStarfieldGroupGetter<IVolumetricLightingGetter> VolumetricLightings =>
        _volumetricLightings ??= new MergedGroup<IVolumetricLightingGetter>(
            _sourceMods.Select(m => m.VolumetricLightings));
    public IStarfieldGroupGetter<ISurfaceBlockGetter> SurfaceBlocks =>
        _surfaceBlocks ??= new MergedGroup<ISurfaceBlockGetter>(
            _sourceMods.Select(m => m.SurfaceBlocks));
    public IStarfieldGroupGetter<ISurfacePatternConfigGetter> SurfacePatternConfigs =>
        _surfacePatternConfigs ??= new MergedGroup<ISurfacePatternConfigGetter>(
            _sourceMods.Select(m => m.SurfacePatternConfigs));
    public IStarfieldGroupGetter<ISurfacePatternGetter> SurfacePatterns =>
        _surfacePatterns ??= new MergedGroup<ISurfacePatternGetter>(
            _sourceMods.Select(m => m.SurfacePatterns));
    public IStarfieldGroupGetter<ISurfaceTreeGetter> SurfaceTrees =>
        _surfaceTrees ??= new MergedGroup<ISurfaceTreeGetter>(
            _sourceMods.Select(m => m.SurfaceTrees));
    public IStarfieldGroupGetter<IPlanetContentManagerTreeGetter> PlanetContentManagerTrees =>
        _planetContentManagerTrees ??= new MergedGroup<IPlanetContentManagerTreeGetter>(
            _sourceMods.Select(m => m.PlanetContentManagerTrees));
    public IStarfieldGroupGetter<IBoneModifierGetter> BoneModifiers =>
        _boneModifiers ??= new MergedGroup<IBoneModifierGetter>(
            _sourceMods.Select(m => m.BoneModifiers));
    public IStarfieldGroupGetter<ISnapTemplateBehaviorGetter> SnapTemplateBehaviors =>
        _snapTemplateBehaviors ??= new MergedGroup<ISnapTemplateBehaviorGetter>(
            _sourceMods.Select(m => m.SnapTemplateBehaviors));
    public IStarfieldGroupGetter<IPlanetGetter> Planets =>
        _planets ??= new MergedGroup<IPlanetGetter>(
            _sourceMods.Select(m => m.Planets));
    public IStarfieldGroupGetter<IConditionRecordGetter> ConditionRecords =>
        _conditionRecords ??= new MergedGroup<IConditionRecordGetter>(
            _sourceMods.Select(m => m.ConditionRecords));
    public IStarfieldGroupGetter<IPlanetContentManagerBranchNodeGetter> PlanetContentManagerBranchNodes =>
        _planetContentManagerBranchNodes ??= new MergedGroup<IPlanetContentManagerBranchNodeGetter>(
            _sourceMods.Select(m => m.PlanetContentManagerBranchNodes));
    public IStarfieldGroupGetter<IPlanetContentManagerContentNodeGetter> PlanetContentManagerContentNodes =>
        _planetContentManagerContentNodes ??= new MergedGroup<IPlanetContentManagerContentNodeGetter>(
            _sourceMods.Select(m => m.PlanetContentManagerContentNodes));
    public IStarfieldGroupGetter<IStarGetter> Stars =>
        _stars ??= new MergedGroup<IStarGetter>(
            _sourceMods.Select(m => m.Stars));
    public IStarfieldGroupGetter<IWWiseEventDataGetter> WWiseEventDatas =>
        _wWiseEventDatas ??= new MergedGroup<IWWiseEventDataGetter>(
            _sourceMods.Select(m => m.WWiseEventDatas));
    public IStarfieldGroupGetter<IResearchProjectGetter> ResearchProjects =>
        _researchProjects ??= new MergedGroup<IResearchProjectGetter>(
            _sourceMods.Select(m => m.ResearchProjects));
    public IStarfieldGroupGetter<IAimOpticalSightMarkerGetter> AimOpticalSightMarkers =>
        _aimOpticalSightMarkers ??= new MergedGroup<IAimOpticalSightMarkerGetter>(
            _sourceMods.Select(m => m.AimOpticalSightMarkers));
    public IStarfieldGroupGetter<IAmbienceSetGetter> AmbienceSets =>
        _ambienceSets ??= new MergedGroup<IAmbienceSetGetter>(
            _sourceMods.Select(m => m.AmbienceSets));
    public IStarfieldGroupGetter<IWeaponBarrelModelGetter> WeaponBarrelModels =>
        _weaponBarrelModels ??= new MergedGroup<IWeaponBarrelModelGetter>(
            _sourceMods.Select(m => m.WeaponBarrelModels));
    public IStarfieldGroupGetter<ISurfacePatternStyleGetter> SurfacePatternStyles =>
        _surfacePatternStyles ??= new MergedGroup<ISurfacePatternStyleGetter>(
            _sourceMods.Select(m => m.SurfacePatternStyles));
    public IStarfieldGroupGetter<ILayeredMaterialSwapGetter> LayeredMaterialSwaps =>
        _layeredMaterialSwaps ??= new MergedGroup<ILayeredMaterialSwapGetter>(
            _sourceMods.Select(m => m.LayeredMaterialSwaps));
    public IStarfieldGroupGetter<IForceDataGetter> ForceDatas =>
        _forceDatas ??= new MergedGroup<IForceDataGetter>(
            _sourceMods.Select(m => m.ForceDatas));
    public IStarfieldGroupGetter<ITerminalMenuGetter> TerminalMenus =>
        _terminalMenus ??= new MergedGroup<ITerminalMenuGetter>(
            _sourceMods.Select(m => m.TerminalMenus));
    public IStarfieldGroupGetter<IEffectSequenceGetter> EffectSequences =>
        _effectSequences ??= new MergedGroup<IEffectSequenceGetter>(
            _sourceMods.Select(m => m.EffectSequences));
    public IStarfieldGroupGetter<ISecondaryDamageListGetter> SecondaryDamageLists =>
        _secondaryDamageLists ??= new MergedGroup<ISecondaryDamageListGetter>(
            _sourceMods.Select(m => m.SecondaryDamageLists));
    public IStarfieldGroupGetter<IMaterialPathGetter> MaterialPaths =>
        _materialPaths ??= new MergedGroup<IMaterialPathGetter>(
            _sourceMods.Select(m => m.MaterialPaths));
    public IStarfieldGroupGetter<ICloudsGetter> Clouds =>
        _clouds ??= new MergedGroup<ICloudsGetter>(
            _sourceMods.Select(m => m.Clouds));
    public IStarfieldGroupGetter<IFogVolumeGetter> FogVolumes =>
        _fogVolumes ??= new MergedGroup<IFogVolumeGetter>(
            _sourceMods.Select(m => m.FogVolumes));
    public IStarfieldGroupGetter<IWWiseKeywordMappingGetter> WWiseKeywordMappings =>
        _wWiseKeywordMappings ??= new MergedGroup<IWWiseKeywordMappingGetter>(
            _sourceMods.Select(m => m.WWiseKeywordMappings));
    public IStarfieldGroupGetter<ILegendaryItemGetter> LegendaryItems =>
        _legendaryItems ??= new MergedGroup<ILegendaryItemGetter>(
            _sourceMods.Select(m => m.LegendaryItems));
    public IStarfieldGroupGetter<IParticleSystemDefineCollisionGetter> ParticleSystemDefineCollisions =>
        _particleSystemDefineCollisions ??= new MergedGroup<IParticleSystemDefineCollisionGetter>(
            _sourceMods.Select(m => m.ParticleSystemDefineCollisions));
    public IStarfieldGroupGetter<ISunPresetGetter> SunPresets =>
        _sunPresets ??= new MergedGroup<ISunPresetGetter>(
            _sourceMods.Select(m => m.SunPresets));
    public IStarfieldGroupGetter<IPhotoModeFeatureGetter> PhotoModeFeatures =>
        _photoModeFeatures ??= new MergedGroup<IPhotoModeFeatureGetter>(
            _sourceMods.Select(m => m.PhotoModeFeatures));
    public IStarfieldGroupGetter<IGameplayOptionGetter> GameplayOptions =>
        _gameplayOptions ??= new MergedGroup<IGameplayOptionGetter>(
            _sourceMods.Select(m => m.GameplayOptions));
    public IStarfieldGroupGetter<IGameplayOptionsGroupGetter> GameplayOptionsGroups =>
        _gameplayOptionsGroups ??= new MergedGroup<IGameplayOptionsGroupGetter>(
            _sourceMods.Select(m => m.GameplayOptionsGroups));
    public IStarfieldGroupGetter<ITimeOfDayRecordGetter> TimeOfDays =>
        _timeOfDays ??= new MergedGroup<ITimeOfDayRecordGetter>(
            _sourceMods.Select(m => m.TimeOfDays));
    public IStarfieldGroupGetter<IActorValueModulationGetter> ActorValueModulations =>
        _actorValueModulations ??= new MergedGroup<IActorValueModulationGetter>(
            _sourceMods.Select(m => m.ActorValueModulations));
    public IStarfieldGroupGetter<IChallengeGetter> Challenges =>
        _challenges ??= new MergedGroup<IChallengeGetter>(
            _sourceMods.Select(m => m.Challenges));
    public IStarfieldGroupGetter<IFacialExpressionGetter> FacialExpressions =>
        _facialExpressions ??= new MergedGroup<IFacialExpressionGetter>(
            _sourceMods.Select(m => m.FacialExpressions));
    public IStarfieldGroupGetter<IPERSGetter> PERS =>
        _pERS ??= new MergedGroup<IPERSGetter>(
            _sourceMods.Select(m => m.PERS));

    BinaryModdedWriteBuilderTargetChoice<IStarfieldModGetter> IStarfieldModGetter.BeginWrite =>
        new BinaryModdedWriteBuilderTargetChoice<IStarfieldModGetter>(this, StarfieldMod.StarfieldWriteBuilderInstantiator.Instance);

    IBinaryModdedWriteBuilderTargetChoice IModGetter.BeginWrite =>
        new BinaryModdedWriteBuilderTargetChoice<IStarfieldModGetter>(this, StarfieldMod.StarfieldWriteBuilderInstantiator.Instance);

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
        return (IGroupGetter<TMajor>?)((StarfieldModCommon)((IStarfieldModGetter)this).CommonInstance()!).GetGroup(
            obj: this,
            type: typeof(TMajor));
    }

    public IGroupGetter? TryGetTopLevelGroup(Type type)
    {
        return (IGroupGetter?)((StarfieldModCommon)((IStarfieldModGetter)this).CommonInstance()!).GetGroup(
            obj: this,
            type: type);
    }

    IEnumerable<IModContext<IStarfieldMod, IStarfieldModGetter, TSetter, TGetter>> IMajorRecordContextEnumerable<IStarfieldMod, IStarfieldModGetter>.EnumerateMajorRecordContexts<TSetter, TGetter>(ILinkCache linkCache, bool throwIfUnknown)
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var context in mod.EnumerateMajorRecordContexts<TSetter, TGetter>(linkCache, throwIfUnknown))
            {
                yield return context;
            }
        }
    }

    IEnumerable<IModContext<IStarfieldMod, IStarfieldModGetter, IMajorRecord, IMajorRecordGetter>> IMajorRecordContextEnumerable<IStarfieldMod, IStarfieldModGetter>.EnumerateMajorRecordContexts(ILinkCache linkCache, Type type, bool throwIfUnknown)
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
        return ((StarfieldModSetterTranslationCommon)((IStarfieldModGetter)this).CommonSetterTranslationInstance()!).DeepCopy(
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

    public ILoquiRegistration Registration => StarfieldMod_Registration.Instance;

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
internal class MergedGroup<TGetter> : IStarfieldGroupGetter<TGetter>, IReadOnlyCache<TGetter, FormKey>
    where TGetter : class, IStarfieldMajorRecordGetter, IBinaryItem
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

    // IStarfieldGroupGetter members
    public object CommonInstance(Type type) => GenericCommonInstanceGetter.Get(StarfieldGroupCommon<TGetter>.Instance, typeof(ICellBlockGetter), type);
    public object? CommonSetterInstance(Type type) => null;
    public object CommonSetterTranslationInstance() => StarfieldGroupSetterTranslationCommon.Instance;

    GroupTypeEnum IStarfieldGroupGetter<TGetter>.Type => GroupTypeEnum.Type;
    int IStarfieldGroupGetter<TGetter>.LastModified => 0;
    public int Unknown => 0;

    IReadOnlyCache<TGetter, FormKey> IStarfieldGroupGetter<TGetter>.RecordCache => this;
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
        StarfieldGroupMixIn.Print(
            item: this,
            sb: sb,
            name: name);
    }
}

/// <summary>
/// Merged list group that combines multiple list groups (like Cells) into a single unified view.
/// CellBlocks from different mods are merged by BlockNumber.
/// </summary>
internal class MergedListGroup : IStarfieldListGroupGetter<ICellBlockGetter>
{
    private readonly IEnumerable<IStarfieldListGroupGetter<ICellBlockGetter>> _sourceGroups;
    private List<ICellBlockGetter>? _cache;
    private readonly object _cacheLock = new object();

    public MergedListGroup(IEnumerable<IStarfieldListGroupGetter<ICellBlockGetter>> sourceGroups)
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

    ILoquiRegistration ILoquiObject.Registration => StarfieldMod_Registration.Instance;
    public static ILoquiRegistration StaticRegistration => StarfieldMod_Registration.Instance;

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        StarfieldListGroupMixIn.Print(
            item: this,
            sb: sb,
            name: name);
    }

    // IStarfieldListGroupGetter properties
    public GroupTypeEnum Type => _sourceGroups.FirstOrDefault()?.Type ?? GroupTypeEnum.InteriorCellBlock;
    public int LastModified => _sourceGroups.Max(g => g.LastModified);
    public int Unknown => 0;

    public object CommonInstance(Type type) => GenericCommonInstanceGetter.Get(StarfieldListGroupCommon<ICellBlockGetter>.Instance, typeof(ICellBlockGetter), type);
    public object? CommonSetterInstance(Type type) => GenericCommonInstanceGetter.Get(StarfieldListGroupSetterCommon<ICellBlock>.Instance, typeof(ICellBlockGetter), type);
    public object CommonSetterTranslationInstance() => StarfieldListGroupSetterTranslationCommon.Instance;

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

