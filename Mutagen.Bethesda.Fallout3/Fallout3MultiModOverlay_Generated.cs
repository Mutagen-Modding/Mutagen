#nullable enable
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
using Mutagen.Bethesda.Fallout3.Assets;

namespace Mutagen.Bethesda.Fallout3;

/// <summary>
/// Multi-mod overlay that presents multiple Fallout3 mods as a single unified mod.
/// Typically used for reading split mods that were written due to exceeding master limits
/// </summary>
internal class Fallout3MultiModOverlay : IFallout3ModDisposableGetter
{
    private readonly IReadOnlyList<IFallout3ModGetter> _sourceMods;
    private readonly IReadOnlyList<IModDisposeGetter>? _disposeSourceMods;
    private readonly ModKey _modKey;
    private readonly IReadOnlyList<IMasterReferenceGetter> _masters;
    private readonly MergedFallout3ModHeader _modHeader;

    private MergedGroup<IGameSettingGetter>? _gameSettings;
    private MergedGroup<ITextureSetGetter>? _textureSets;
    private MergedGroup<IMenuIconGetter>? _menuIcons;
    private MergedGroup<IGlobalGetter>? _globals;
    private MergedGroup<IClassGetter>? _classes;
    private MergedGroup<IFactionGetter>? _factions;
    private MergedGroup<IHeadPartGetter>? _headParts;
    private MergedGroup<IHairGetter>? _hairs;
    private MergedGroup<IEyesGetter>? _eyes;
    private MergedGroup<IRaceGetter>? _races;
    private MergedGroup<ISoundGetter>? _sounds;
    private MergedGroup<IAcousticSpaceGetter>? _acousticSpaces;
    private MergedGroup<IMagicEffectGetter>? _magicEffects;
    private MergedGroup<IScriptGetter>? _scripts;
    private MergedGroup<ILandscapeTextureGetter>? _landscapeTextures;
    private MergedGroup<IObjectEffectGetter>? _objectEffects;
    private MergedGroup<ISpellGetter>? _spells;
    private MergedGroup<IActivatorGetter>? _activators;
    private MergedGroup<ITalkingActivatorGetter>? _talkingActivators;
    private MergedGroup<ITerminalGetter>? _terminals;
    private MergedGroup<IArmorGetter>? _armors;
    private MergedGroup<IBookGetter>? _books;
    private MergedGroup<IContainerGetter>? _containers;
    private MergedGroup<IDoorGetter>? _doors;
    private MergedGroup<IIngredientGetter>? _ingredients;
    private MergedGroup<ILightGetter>? _lights;
    private MergedGroup<IMiscItemGetter>? _miscItems;
    private MergedGroup<IStaticGetter>? _statics;
    private MergedGroup<IStaticCollectionGetter>? _staticCollections;
    private MergedGroup<IMoveableStaticGetter>? _moveableStatics;
    private MergedGroup<IPlaceableWaterGetter>? _placeableWaters;
    private MergedGroup<IGrassGetter>? _grasses;
    private MergedGroup<ITreeGetter>? _trees;
    private MergedGroup<IFurnitureGetter>? _furniture;
    private MergedGroup<IWeaponGetter>? _weapons;
    private MergedGroup<IAmmunitionGetter>? _ammunitions;
    private MergedGroup<INpcGetter>? _npcs;
    private MergedGroup<ICreatureGetter>? _creatures;
    private MergedGroup<ILeveledCreatureGetter>? _leveledCreatures;
    private MergedGroup<ILeveledNpcGetter>? _leveledNpcs;
    private MergedGroup<IKeyGetter>? _keys;
    private MergedGroup<IIngestibleGetter>? _ingestibles;
    private MergedGroup<IIdleMarkerGetter>? _idleMarkers;
    private MergedGroup<INoteGetter>? _notes;
    private MergedGroup<IConstructibleObjectGetter>? _constructibleObjects;
    private MergedGroup<IProjectileGetter>? _projectiles;
    private MergedGroup<ILeveledItemGetter>? _leveledItems;
    private MergedGroup<IWeatherGetter>? _weather;
    private MergedGroup<IClimateGetter>? _climates;
    private MergedGroup<IRegionGetter>? _regions;
    private MergedGroup<INavigationMeshInfoMapGetter>? _navigationMeshInfoMaps;
    private MergedListGroup? _cells;
    private MergedGroup<IWorldspaceGetter>? _worldspaces;
    private MergedGroup<IDialogTopicGetter>? _dialogTopics;
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
    private MergedGroup<IMessageGetter>? _messages;
    private MergedGroup<IPerkGetter>? _perks;
    private MergedGroup<IBodyPartDataGetter>? _bodyParts;
    private MergedGroup<IAddonNodeGetter>? _addonNodes;
    private MergedGroup<IActorValueInformationGetter>? _actorValueInformation;
    private MergedGroup<IRadiationStageGetter>? _radiationStages;
    private MergedGroup<ICameraShotGetter>? _cameraShots;
    private MergedGroup<ICameraPathGetter>? _cameraPaths;
    private MergedGroup<IVoiceTypeGetter>? _voiceTypes;
    private MergedGroup<IImpactGetter>? _impacts;
    private MergedGroup<IImpactDataSetGetter>? _impactDataSets;
    private MergedGroup<IArmorAddonGetter>? _armorAddons;
    private MergedGroup<IEncounterZoneGetter>? _encounterZones;
    private MergedGroup<IRagdollGetter>? _ragdolls;
    private MergedGroup<IDefaultObjectManagerGetter>? _defaultObjectManagers;
    private MergedGroup<ILightingTemplateGetter>? _lightingTemplates;
    private MergedGroup<IMusicTypeGetter>? _musicTypes;
    private MergedGroup<IFormListGetter>? _formLists;
    private MergedGroup<IItemModGetter>? _itemMods;
    private MergedGroup<IReputationGetter>? _reputations;
    private MergedGroup<IRecipeGetter>? _recipes;
    private MergedGroup<IRecipeCategoryGetter>? _recipeCategories;
    private MergedGroup<ICasinoChipGetter>? _casinoChips;
    private MergedGroup<ICasinoGetter>? _casinos;
    private MergedGroup<ILoadScreenTypeGetter>? _loadScreenTypes;
    private MergedGroup<IMediaSetGetter>? _mediaSets;
    private MergedGroup<IMediaLocationControllerGetter>? _mediaLocationControllers;
    private MergedGroup<IChallengeGetter>? _challenges;
    private MergedGroup<IAmmoEffectGetter>? _ammoEffects;
    private MergedGroup<ICaravanCardGetter>? _caravanCards;
    private MergedGroup<ICaravanMoneyGetter>? _caravanMonies;
    private MergedGroup<ICaravanDeckGetter>? _caravanDecks;
    private MergedGroup<IDehydrationStageGetter>? _dehydrationStages;
    private MergedGroup<IHungerStageGetter>? _hungerStages;
    private MergedGroup<ISleepDeprivationStageGetter>? _sleepDeprivationStages;

    /// <summary>
    /// Creates a new Fallout3MultiModOverlay from multiple source mod files.
    /// </summary>
    public Fallout3MultiModOverlay(
        ModKey modKey,
        IEnumerable<IFallout3ModGetter> sourceMods,
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

        _modHeader = new MergedFallout3ModHeader(sourceList.Select(s => s.ModHeader).ToList(), mergedMasters);
    }

    public ModKey ModKey => _modKey;
    public IFallout3ModHeaderGetter ModHeader => _modHeader;
    public IReadOnlyList<IMasterReferenceGetter> MasterReferences => _masters;
    public Fallout3Release Fallout3Release => _sourceMods[0].Fallout3Release;
    GameRelease IModGetter.GameRelease => Fallout3Release.ToGameRelease();

    public object CommonInstance() => Fallout3ModCommon.Instance;
    public object? CommonSetterInstance() => Fallout3ModSetterCommon.Instance;
    public object CommonSetterTranslationInstance() => Fallout3ModSetterTranslationCommon.Instance;


    public IFallout3GroupGetter<IGameSettingGetter> GameSettings =>
        _gameSettings ??= new MergedGroup<IGameSettingGetter>(
            _sourceMods.Select(m => m.GameSettings));
    public IFallout3GroupGetter<ITextureSetGetter> TextureSets =>
        _textureSets ??= new MergedGroup<ITextureSetGetter>(
            _sourceMods.Select(m => m.TextureSets));
    public IFallout3GroupGetter<IMenuIconGetter> MenuIcons =>
        _menuIcons ??= new MergedGroup<IMenuIconGetter>(
            _sourceMods.Select(m => m.MenuIcons));
    public IFallout3GroupGetter<IGlobalGetter> Globals =>
        _globals ??= new MergedGroup<IGlobalGetter>(
            _sourceMods.Select(m => m.Globals));
    public IFallout3GroupGetter<IClassGetter> Classes =>
        _classes ??= new MergedGroup<IClassGetter>(
            _sourceMods.Select(m => m.Classes));
    public IFallout3GroupGetter<IFactionGetter> Factions =>
        _factions ??= new MergedGroup<IFactionGetter>(
            _sourceMods.Select(m => m.Factions));
    public IFallout3GroupGetter<IHeadPartGetter> HeadParts =>
        _headParts ??= new MergedGroup<IHeadPartGetter>(
            _sourceMods.Select(m => m.HeadParts));
    public IFallout3GroupGetter<IHairGetter> Hairs =>
        _hairs ??= new MergedGroup<IHairGetter>(
            _sourceMods.Select(m => m.Hairs));
    public IFallout3GroupGetter<IEyesGetter> Eyes =>
        _eyes ??= new MergedGroup<IEyesGetter>(
            _sourceMods.Select(m => m.Eyes));
    public IFallout3GroupGetter<IRaceGetter> Races =>
        _races ??= new MergedGroup<IRaceGetter>(
            _sourceMods.Select(m => m.Races));
    public IFallout3GroupGetter<ISoundGetter> Sounds =>
        _sounds ??= new MergedGroup<ISoundGetter>(
            _sourceMods.Select(m => m.Sounds));
    public IFallout3GroupGetter<IAcousticSpaceGetter> AcousticSpaces =>
        _acousticSpaces ??= new MergedGroup<IAcousticSpaceGetter>(
            _sourceMods.Select(m => m.AcousticSpaces));
    public IFallout3GroupGetter<IMagicEffectGetter> MagicEffects =>
        _magicEffects ??= new MergedGroup<IMagicEffectGetter>(
            _sourceMods.Select(m => m.MagicEffects));
    public IFallout3GroupGetter<IScriptGetter> Scripts =>
        _scripts ??= new MergedGroup<IScriptGetter>(
            _sourceMods.Select(m => m.Scripts));
    public IFallout3GroupGetter<ILandscapeTextureGetter> LandscapeTextures =>
        _landscapeTextures ??= new MergedGroup<ILandscapeTextureGetter>(
            _sourceMods.Select(m => m.LandscapeTextures));
    public IFallout3GroupGetter<IObjectEffectGetter> ObjectEffects =>
        _objectEffects ??= new MergedGroup<IObjectEffectGetter>(
            _sourceMods.Select(m => m.ObjectEffects));
    public IFallout3GroupGetter<ISpellGetter> Spells =>
        _spells ??= new MergedGroup<ISpellGetter>(
            _sourceMods.Select(m => m.Spells));
    public IFallout3GroupGetter<IActivatorGetter> Activators =>
        _activators ??= new MergedGroup<IActivatorGetter>(
            _sourceMods.Select(m => m.Activators));
    public IFallout3GroupGetter<ITalkingActivatorGetter> TalkingActivators =>
        _talkingActivators ??= new MergedGroup<ITalkingActivatorGetter>(
            _sourceMods.Select(m => m.TalkingActivators));
    public IFallout3GroupGetter<ITerminalGetter> Terminals =>
        _terminals ??= new MergedGroup<ITerminalGetter>(
            _sourceMods.Select(m => m.Terminals));
    public IFallout3GroupGetter<IArmorGetter> Armors =>
        _armors ??= new MergedGroup<IArmorGetter>(
            _sourceMods.Select(m => m.Armors));
    public IFallout3GroupGetter<IBookGetter> Books =>
        _books ??= new MergedGroup<IBookGetter>(
            _sourceMods.Select(m => m.Books));
    public IFallout3GroupGetter<IContainerGetter> Containers =>
        _containers ??= new MergedGroup<IContainerGetter>(
            _sourceMods.Select(m => m.Containers));
    public IFallout3GroupGetter<IDoorGetter> Doors =>
        _doors ??= new MergedGroup<IDoorGetter>(
            _sourceMods.Select(m => m.Doors));
    public IFallout3GroupGetter<IIngredientGetter> Ingredients =>
        _ingredients ??= new MergedGroup<IIngredientGetter>(
            _sourceMods.Select(m => m.Ingredients));
    public IFallout3GroupGetter<ILightGetter> Lights =>
        _lights ??= new MergedGroup<ILightGetter>(
            _sourceMods.Select(m => m.Lights));
    public IFallout3GroupGetter<IMiscItemGetter> MiscItems =>
        _miscItems ??= new MergedGroup<IMiscItemGetter>(
            _sourceMods.Select(m => m.MiscItems));
    public IFallout3GroupGetter<IStaticGetter> Statics =>
        _statics ??= new MergedGroup<IStaticGetter>(
            _sourceMods.Select(m => m.Statics));
    public IFallout3GroupGetter<IStaticCollectionGetter> StaticCollections =>
        _staticCollections ??= new MergedGroup<IStaticCollectionGetter>(
            _sourceMods.Select(m => m.StaticCollections));
    public IFallout3GroupGetter<IMoveableStaticGetter> MoveableStatics =>
        _moveableStatics ??= new MergedGroup<IMoveableStaticGetter>(
            _sourceMods.Select(m => m.MoveableStatics));
    public IFallout3GroupGetter<IPlaceableWaterGetter> PlaceableWaters =>
        _placeableWaters ??= new MergedGroup<IPlaceableWaterGetter>(
            _sourceMods.Select(m => m.PlaceableWaters));
    public IFallout3GroupGetter<IGrassGetter> Grasses =>
        _grasses ??= new MergedGroup<IGrassGetter>(
            _sourceMods.Select(m => m.Grasses));
    public IFallout3GroupGetter<ITreeGetter> Trees =>
        _trees ??= new MergedGroup<ITreeGetter>(
            _sourceMods.Select(m => m.Trees));
    public IFallout3GroupGetter<IFurnitureGetter> Furniture =>
        _furniture ??= new MergedGroup<IFurnitureGetter>(
            _sourceMods.Select(m => m.Furniture));
    public IFallout3GroupGetter<IWeaponGetter> Weapons =>
        _weapons ??= new MergedGroup<IWeaponGetter>(
            _sourceMods.Select(m => m.Weapons));
    public IFallout3GroupGetter<IAmmunitionGetter> Ammunitions =>
        _ammunitions ??= new MergedGroup<IAmmunitionGetter>(
            _sourceMods.Select(m => m.Ammunitions));
    public IFallout3GroupGetter<INpcGetter> Npcs =>
        _npcs ??= new MergedGroup<INpcGetter>(
            _sourceMods.Select(m => m.Npcs));
    public IFallout3GroupGetter<ICreatureGetter> Creatures =>
        _creatures ??= new MergedGroup<ICreatureGetter>(
            _sourceMods.Select(m => m.Creatures));
    public IFallout3GroupGetter<ILeveledCreatureGetter> LeveledCreatures =>
        _leveledCreatures ??= new MergedGroup<ILeveledCreatureGetter>(
            _sourceMods.Select(m => m.LeveledCreatures));
    public IFallout3GroupGetter<ILeveledNpcGetter> LeveledNpcs =>
        _leveledNpcs ??= new MergedGroup<ILeveledNpcGetter>(
            _sourceMods.Select(m => m.LeveledNpcs));
    public IFallout3GroupGetter<IKeyGetter> Keys =>
        _keys ??= new MergedGroup<IKeyGetter>(
            _sourceMods.Select(m => m.Keys));
    public IFallout3GroupGetter<IIngestibleGetter> Ingestibles =>
        _ingestibles ??= new MergedGroup<IIngestibleGetter>(
            _sourceMods.Select(m => m.Ingestibles));
    public IFallout3GroupGetter<IIdleMarkerGetter> IdleMarkers =>
        _idleMarkers ??= new MergedGroup<IIdleMarkerGetter>(
            _sourceMods.Select(m => m.IdleMarkers));
    public IFallout3GroupGetter<INoteGetter> Notes =>
        _notes ??= new MergedGroup<INoteGetter>(
            _sourceMods.Select(m => m.Notes));
    public IFallout3GroupGetter<IConstructibleObjectGetter> ConstructibleObjects =>
        _constructibleObjects ??= new MergedGroup<IConstructibleObjectGetter>(
            _sourceMods.Select(m => m.ConstructibleObjects));
    public IFallout3GroupGetter<IProjectileGetter> Projectiles =>
        _projectiles ??= new MergedGroup<IProjectileGetter>(
            _sourceMods.Select(m => m.Projectiles));
    public IFallout3GroupGetter<ILeveledItemGetter> LeveledItems =>
        _leveledItems ??= new MergedGroup<ILeveledItemGetter>(
            _sourceMods.Select(m => m.LeveledItems));
    public IFallout3GroupGetter<IWeatherGetter> Weather =>
        _weather ??= new MergedGroup<IWeatherGetter>(
            _sourceMods.Select(m => m.Weather));
    public IFallout3GroupGetter<IClimateGetter> Climates =>
        _climates ??= new MergedGroup<IClimateGetter>(
            _sourceMods.Select(m => m.Climates));
    public IFallout3GroupGetter<IRegionGetter> Regions =>
        _regions ??= new MergedGroup<IRegionGetter>(
            _sourceMods.Select(m => m.Regions));
    public IFallout3GroupGetter<INavigationMeshInfoMapGetter> NavigationMeshInfoMaps =>
        _navigationMeshInfoMaps ??= new MergedGroup<INavigationMeshInfoMapGetter>(
            _sourceMods.Select(m => m.NavigationMeshInfoMaps));
    public IFallout3ListGroupGetter<ICellBlockGetter> Cells =>
        _cells ??= new MergedListGroup(_sourceMods.Select(m => m.Cells));
    public IFallout3GroupGetter<IWorldspaceGetter> Worldspaces =>
        _worldspaces ??= new MergedGroup<IWorldspaceGetter>(
            _sourceMods.Select(m => m.Worldspaces), allowDuplicateOverrides: true,
            duplicateMerger: MergedWorldspace.Merge);
    public IFallout3GroupGetter<IDialogTopicGetter> DialogTopics =>
        _dialogTopics ??= new MergedGroup<IDialogTopicGetter>(
            _sourceMods.Select(m => m.DialogTopics), allowDuplicateOverrides: true);
    public IFallout3GroupGetter<IQuestGetter> Quests =>
        _quests ??= new MergedGroup<IQuestGetter>(
            _sourceMods.Select(m => m.Quests));
    public IFallout3GroupGetter<IIdleAnimationGetter> IdleAnimations =>
        _idleAnimations ??= new MergedGroup<IIdleAnimationGetter>(
            _sourceMods.Select(m => m.IdleAnimations));
    public IFallout3GroupGetter<IPackageGetter> Packages =>
        _packages ??= new MergedGroup<IPackageGetter>(
            _sourceMods.Select(m => m.Packages));
    public IFallout3GroupGetter<ICombatStyleGetter> CombatStyles =>
        _combatStyles ??= new MergedGroup<ICombatStyleGetter>(
            _sourceMods.Select(m => m.CombatStyles));
    public IFallout3GroupGetter<ILoadScreenGetter> LoadScreens =>
        _loadScreens ??= new MergedGroup<ILoadScreenGetter>(
            _sourceMods.Select(m => m.LoadScreens));
    public IFallout3GroupGetter<IAnimatedObjectGetter> AnimatedObjects =>
        _animatedObjects ??= new MergedGroup<IAnimatedObjectGetter>(
            _sourceMods.Select(m => m.AnimatedObjects));
    public IFallout3GroupGetter<IWaterGetter> Waters =>
        _waters ??= new MergedGroup<IWaterGetter>(
            _sourceMods.Select(m => m.Waters));
    public IFallout3GroupGetter<IEffectShaderGetter> EffectShaders =>
        _effectShaders ??= new MergedGroup<IEffectShaderGetter>(
            _sourceMods.Select(m => m.EffectShaders));
    public IFallout3GroupGetter<IExplosionGetter> Explosions =>
        _explosions ??= new MergedGroup<IExplosionGetter>(
            _sourceMods.Select(m => m.Explosions));
    public IFallout3GroupGetter<IDebrisGetter> Debris =>
        _debris ??= new MergedGroup<IDebrisGetter>(
            _sourceMods.Select(m => m.Debris));
    public IFallout3GroupGetter<IImageSpaceGetter> ImageSpaces =>
        _imageSpaces ??= new MergedGroup<IImageSpaceGetter>(
            _sourceMods.Select(m => m.ImageSpaces));
    public IFallout3GroupGetter<IImageSpaceAdapterGetter> ImageSpaceAdapters =>
        _imageSpaceAdapters ??= new MergedGroup<IImageSpaceAdapterGetter>(
            _sourceMods.Select(m => m.ImageSpaceAdapters));
    public IFallout3GroupGetter<IMessageGetter> Messages =>
        _messages ??= new MergedGroup<IMessageGetter>(
            _sourceMods.Select(m => m.Messages));
    public IFallout3GroupGetter<IPerkGetter> Perks =>
        _perks ??= new MergedGroup<IPerkGetter>(
            _sourceMods.Select(m => m.Perks));
    public IFallout3GroupGetter<IBodyPartDataGetter> BodyParts =>
        _bodyParts ??= new MergedGroup<IBodyPartDataGetter>(
            _sourceMods.Select(m => m.BodyParts));
    public IFallout3GroupGetter<IAddonNodeGetter> AddonNodes =>
        _addonNodes ??= new MergedGroup<IAddonNodeGetter>(
            _sourceMods.Select(m => m.AddonNodes));
    public IFallout3GroupGetter<IActorValueInformationGetter> ActorValueInformation =>
        _actorValueInformation ??= new MergedGroup<IActorValueInformationGetter>(
            _sourceMods.Select(m => m.ActorValueInformation));
    public IFallout3GroupGetter<IRadiationStageGetter> RadiationStages =>
        _radiationStages ??= new MergedGroup<IRadiationStageGetter>(
            _sourceMods.Select(m => m.RadiationStages));
    public IFallout3GroupGetter<ICameraShotGetter> CameraShots =>
        _cameraShots ??= new MergedGroup<ICameraShotGetter>(
            _sourceMods.Select(m => m.CameraShots));
    public IFallout3GroupGetter<ICameraPathGetter> CameraPaths =>
        _cameraPaths ??= new MergedGroup<ICameraPathGetter>(
            _sourceMods.Select(m => m.CameraPaths));
    public IFallout3GroupGetter<IVoiceTypeGetter> VoiceTypes =>
        _voiceTypes ??= new MergedGroup<IVoiceTypeGetter>(
            _sourceMods.Select(m => m.VoiceTypes));
    public IFallout3GroupGetter<IImpactGetter> Impacts =>
        _impacts ??= new MergedGroup<IImpactGetter>(
            _sourceMods.Select(m => m.Impacts));
    public IFallout3GroupGetter<IImpactDataSetGetter> ImpactDataSets =>
        _impactDataSets ??= new MergedGroup<IImpactDataSetGetter>(
            _sourceMods.Select(m => m.ImpactDataSets));
    public IFallout3GroupGetter<IArmorAddonGetter> ArmorAddons =>
        _armorAddons ??= new MergedGroup<IArmorAddonGetter>(
            _sourceMods.Select(m => m.ArmorAddons));
    public IFallout3GroupGetter<IEncounterZoneGetter> EncounterZones =>
        _encounterZones ??= new MergedGroup<IEncounterZoneGetter>(
            _sourceMods.Select(m => m.EncounterZones));
    public IFallout3GroupGetter<IRagdollGetter> Ragdolls =>
        _ragdolls ??= new MergedGroup<IRagdollGetter>(
            _sourceMods.Select(m => m.Ragdolls));
    public IFallout3GroupGetter<IDefaultObjectManagerGetter> DefaultObjectManagers =>
        _defaultObjectManagers ??= new MergedGroup<IDefaultObjectManagerGetter>(
            _sourceMods.Select(m => m.DefaultObjectManagers));
    public IFallout3GroupGetter<ILightingTemplateGetter> LightingTemplates =>
        _lightingTemplates ??= new MergedGroup<ILightingTemplateGetter>(
            _sourceMods.Select(m => m.LightingTemplates));
    public IFallout3GroupGetter<IMusicTypeGetter> MusicTypes =>
        _musicTypes ??= new MergedGroup<IMusicTypeGetter>(
            _sourceMods.Select(m => m.MusicTypes));
    public IFallout3GroupGetter<IFormListGetter> FormLists =>
        _formLists ??= new MergedGroup<IFormListGetter>(
            _sourceMods.Select(m => m.FormLists));
    public IFallout3GroupGetter<IItemModGetter> ItemMods =>
        _itemMods ??= new MergedGroup<IItemModGetter>(
            _sourceMods.Select(m => m.ItemMods));
    public IFallout3GroupGetter<IReputationGetter> Reputations =>
        _reputations ??= new MergedGroup<IReputationGetter>(
            _sourceMods.Select(m => m.Reputations));
    public IFallout3GroupGetter<IRecipeGetter> Recipes =>
        _recipes ??= new MergedGroup<IRecipeGetter>(
            _sourceMods.Select(m => m.Recipes));
    public IFallout3GroupGetter<IRecipeCategoryGetter> RecipeCategories =>
        _recipeCategories ??= new MergedGroup<IRecipeCategoryGetter>(
            _sourceMods.Select(m => m.RecipeCategories));
    public IFallout3GroupGetter<ICasinoChipGetter> CasinoChips =>
        _casinoChips ??= new MergedGroup<ICasinoChipGetter>(
            _sourceMods.Select(m => m.CasinoChips));
    public IFallout3GroupGetter<ICasinoGetter> Casinos =>
        _casinos ??= new MergedGroup<ICasinoGetter>(
            _sourceMods.Select(m => m.Casinos));
    public IFallout3GroupGetter<ILoadScreenTypeGetter> LoadScreenTypes =>
        _loadScreenTypes ??= new MergedGroup<ILoadScreenTypeGetter>(
            _sourceMods.Select(m => m.LoadScreenTypes));
    public IFallout3GroupGetter<IMediaSetGetter> MediaSets =>
        _mediaSets ??= new MergedGroup<IMediaSetGetter>(
            _sourceMods.Select(m => m.MediaSets));
    public IFallout3GroupGetter<IMediaLocationControllerGetter> MediaLocationControllers =>
        _mediaLocationControllers ??= new MergedGroup<IMediaLocationControllerGetter>(
            _sourceMods.Select(m => m.MediaLocationControllers));
    public IFallout3GroupGetter<IChallengeGetter> Challenges =>
        _challenges ??= new MergedGroup<IChallengeGetter>(
            _sourceMods.Select(m => m.Challenges));
    public IFallout3GroupGetter<IAmmoEffectGetter> AmmoEffects =>
        _ammoEffects ??= new MergedGroup<IAmmoEffectGetter>(
            _sourceMods.Select(m => m.AmmoEffects));
    public IFallout3GroupGetter<ICaravanCardGetter> CaravanCards =>
        _caravanCards ??= new MergedGroup<ICaravanCardGetter>(
            _sourceMods.Select(m => m.CaravanCards));
    public IFallout3GroupGetter<ICaravanMoneyGetter> CaravanMonies =>
        _caravanMonies ??= new MergedGroup<ICaravanMoneyGetter>(
            _sourceMods.Select(m => m.CaravanMonies));
    public IFallout3GroupGetter<ICaravanDeckGetter> CaravanDecks =>
        _caravanDecks ??= new MergedGroup<ICaravanDeckGetter>(
            _sourceMods.Select(m => m.CaravanDecks));
    public IFallout3GroupGetter<IDehydrationStageGetter> DehydrationStages =>
        _dehydrationStages ??= new MergedGroup<IDehydrationStageGetter>(
            _sourceMods.Select(m => m.DehydrationStages));
    public IFallout3GroupGetter<IHungerStageGetter> HungerStages =>
        _hungerStages ??= new MergedGroup<IHungerStageGetter>(
            _sourceMods.Select(m => m.HungerStages));
    public IFallout3GroupGetter<ISleepDeprivationStageGetter> SleepDeprivationStages =>
        _sleepDeprivationStages ??= new MergedGroup<ISleepDeprivationStageGetter>(
            _sourceMods.Select(m => m.SleepDeprivationStages));

    BinaryModdedWriteBuilderTargetChoice<IFallout3ModGetter> IFallout3ModGetter.BeginWrite =>
        new BinaryModdedWriteBuilderTargetChoice<IFallout3ModGetter>(this, Fallout3Mod.Fallout3WriteBuilderInstantiator.Instance);

    IBinaryModdedWriteBuilderTargetChoice IModGetter.BeginWrite =>
        new BinaryModdedWriteBuilderTargetChoice<IFallout3ModGetter>(this, Fallout3Mod.Fallout3WriteBuilderInstantiator.Instance);

    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
        => Fallout3ModCommon.Instance.EnumerateAssetLinks(this, queryCategories, linkCache, assetType);

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true)
        => Fallout3ModCommon.Instance.EnumerateFormLinks(this, iterateNestedRecords);

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
        => Fallout3ModCommon.Instance.EnumerateMajorRecords(this);

    public IEnumerable<T> EnumerateMajorRecords<T>(bool throwIfUnknown = true) where T : class, IMajorRecordQueryableGetter
        => Fallout3ModCommon.Instance.EnumerateMajorRecords(this, typeof(T), throwIfUnknown).Select(m => (T)m);

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
        => Fallout3ModCommon.Instance.EnumerateMajorRecords(this, type, throwIfUnknown);

    IGroupGetter<TMajor>? IModGetter.TryGetTopLevelGroup<TMajor>()
    {
        return (IGroupGetter<TMajor>?)((Fallout3ModCommon)((IFallout3ModGetter)this).CommonInstance()!).GetGroup(
            obj: this,
            type: typeof(TMajor));
    }

    public IGroupGetter? TryGetTopLevelGroup(Type type)
    {
        return (IGroupGetter?)((Fallout3ModCommon)((IFallout3ModGetter)this).CommonInstance()!).GetGroup(
            obj: this,
            type: type);
    }

    IEnumerable<IModContext<IFallout3Mod, IFallout3ModGetter, TSetter, TGetter>> IMajorRecordContextEnumerable<IFallout3Mod, IFallout3ModGetter>.EnumerateMajorRecordContexts<TSetter, TGetter>(ILinkCache linkCache, bool throwIfUnknown)
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

    IEnumerable<IModContext<IFallout3Mod, IFallout3ModGetter, IMajorRecord, IMajorRecordGetter>> IMajorRecordContextEnumerable<IFallout3Mod, IFallout3ModGetter>.EnumerateMajorRecordContexts(ILinkCache linkCache, Type type, bool throwIfUnknown)
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
        => Fallout3ModCommon.Instance.GetRecordCount(this);

    IMod IModGetter.DeepCopy()
    {
        return ((Fallout3ModSetterTranslationCommon)((IFallout3ModGetter)this).CommonSetterTranslationInstance()!).DeepCopy(
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

    public ILoquiRegistration Registration => Fallout3Mod_Registration.Instance;

    public void Print(StructuredStringBuilder sb, string? name = null)
        => Fallout3ModCommon.Instance.Print(this, sb, name);

    public IMask<bool> GetEqualsMask(object rhs, EqualsMaskHelper.Include include)
        => Fallout3ModCommon.Instance.GetEqualsMask(this, (IFallout3ModGetter)rhs, include);

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
internal class MergedFallout3ModHeader : IFallout3ModHeaderGetter
{
    private readonly IReadOnlyList<IFallout3ModHeaderGetter> _sources;
    private readonly IReadOnlyList<IMasterReferenceGetter> _masters;
    private readonly MergedModStats _stats;

    private static readonly Fallout3ModHeader.TranslationMask _mustMatchMask = new Fallout3ModHeader.TranslationMask(defaultOn: true)
    {
        MasterReferences = false,
        OverriddenForms = false,
        Stats = new ModStats.TranslationMask(defaultOn: true)
        {
            NumRecords = false,
            NextFormID = false,
        },
    };

    public MergedFallout3ModHeader(IReadOnlyList<IFallout3ModHeaderGetter> sources, IReadOnlyList<IMasterReferenceGetter> masters)
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
                throw new System.IO.InvalidDataException($"MergedFallout3ModHeader: source mod {i} disagrees with source mod 0 on must-match fields.");
            }
        }
    }

    public Fallout3ModHeader.HeaderFlag Flags => _sources[0].Flags;
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
    public ReadOnlyMemorySlice<Byte>? Screenshot => _sources[0].Screenshot;
    public IReadOnlyList<IFormLinkGetter<IFallout3MajorRecordGetter>>? OverriddenForms
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

    public ILoquiRegistration Registration => IFallout3ModHeaderGetter.StaticRegistration;
    public object CommonInstance() => Fallout3ModHeaderCommon.Instance;
    public object? CommonSetterInstance() => null;
    public object CommonSetterTranslationInstance() => _sources[0].CommonSetterTranslationInstance();
    public void Print(StructuredStringBuilder sb, string? name = null) => _sources[0].Print(sb, name);
    object IBinaryItem.BinaryWriteTranslator => Fallout3ModHeaderBinaryWriteTranslation.Instance;
    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
        => ((Fallout3ModHeaderBinaryWriteTranslation)((IBinaryItem)this).BinaryWriteTranslator).Write(item: this, writer: writer, translationParams: translationParams);
    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true) => Fallout3ModHeaderCommon.Instance.EnumerateFormLinks(this, iterateNestedRecords);
}

/// <summary>
/// Merged group that combines multiple groups into a single unified view.
/// Validates no duplicate FormKeys exist and caches results.
/// When allowDuplicateOverrides is true, duplicate FormKeys are allowed and the later copy wins.
/// </summary>
internal class MergedGroup<TGetter> : IFallout3GroupGetter<TGetter>, IReadOnlyCache<TGetter, FormKey>
    where TGetter : class, IFallout3MajorRecordGetter, IBinaryItem
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

    object IBinaryItem.BinaryWriteTranslator => Fallout3GroupBinaryWriteTranslation.Instance;
    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
        => Fallout3GroupBinaryWriteTranslation.Instance.Write(writer: writer, item: this, translationParams: translationParams);

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

    // IFallout3GroupGetter members
    public object CommonInstance(Type type) => GenericCommonInstanceGetter.Get(Fallout3GroupCommon<TGetter>.Instance, typeof(ICellBlockGetter), type);
    public object? CommonSetterInstance(Type type) => null;
    public object CommonSetterTranslationInstance() => Fallout3GroupSetterTranslationCommon.Instance;

    GroupTypeEnum IFallout3GroupGetter<TGetter>.Type => GroupTypeEnum.Type;
    int IFallout3GroupGetter<TGetter>.LastModified => 0;
    public int Unknown => 0;

    IReadOnlyCache<TGetter, FormKey> IFallout3GroupGetter<TGetter>.RecordCache => this;
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
        Fallout3GroupMixIn.Print(
            item: this,
            sb: sb,
            name: name);
    }
}

/// <summary>
/// Merged list group that combines multiple list groups (like Cells) into a single unified view.
/// CellBlocks from different mods are merged by BlockNumber.
/// </summary>
internal class MergedListGroup : IFallout3ListGroupGetter<ICellBlockGetter>
{
    private readonly IEnumerable<IFallout3ListGroupGetter<ICellBlockGetter>> _sourceGroups;
    private readonly Lazy<List<ICellBlockGetter>> _cache;

    public MergedListGroup(IEnumerable<IFallout3ListGroupGetter<ICellBlockGetter>> sourceGroups)
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

    ILoquiRegistration ILoquiObject.Registration => Fallout3Mod_Registration.Instance;
    public static ILoquiRegistration StaticRegistration => Fallout3Mod_Registration.Instance;

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        Fallout3ListGroupMixIn.Print(
            item: this,
            sb: sb,
            name: name);
    }

    // IFallout3ListGroupGetter properties
    public GroupTypeEnum Type => _sourceGroups.FirstOrDefault()?.Type ?? GroupTypeEnum.InteriorCellBlock;
    public int LastModified => _sourceGroups.Max(g => g.LastModified);
    public int Unknown => 0;

    public object CommonInstance(Type type) => GenericCommonInstanceGetter.Get(Fallout3ListGroupCommon<ICellBlockGetter>.Instance, typeof(ICellBlockGetter), type);
    public object? CommonSetterInstance(Type type) => GenericCommonInstanceGetter.Get(Fallout3ListGroupSetterCommon<ICellBlock>.Instance, typeof(ICellBlockGetter), type);
    public object CommonSetterTranslationInstance() => Fallout3ListGroupSetterTranslationCommon.Instance;

    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
        => Fallout3ListGroupCommon<ICellBlockGetter>.Instance.EnumerateAssetLinks(this, queryCategories, linkCache, assetType);

    object IBinaryItem.BinaryWriteTranslator => Fallout3ListGroupBinaryWriteTranslation.Instance;
    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
        => Fallout3ListGroupBinaryWriteTranslation.Instance.Write(writer: writer, item: this, translationParams: translationParams);

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true)
        => Fallout3ListGroupCommon<ICellBlockGetter>.Instance.EnumerateFormLinks(this, iterateNestedRecords);

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
        => Fallout3ListGroupCommon<ICellBlockGetter>.Instance.EnumerateMajorRecords(this);

    IEnumerable<T> IMajorRecordGetterEnumerable.EnumerateMajorRecords<T>(bool throwIfUnknown)
        => Fallout3ListGroupCommon<ICellBlockGetter>.Instance.EnumerateMajorRecords(this, typeof(T), throwIfUnknown).Select(m => (T)m);

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
        => Fallout3ListGroupCommon<ICellBlockGetter>.Instance.EnumerateMajorRecords(this, type, throwIfUnknown);
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
        _mergedSubBlocks = new Lazy<List<ICellSubBlockGetter>>(MergeSubBlocks);
    }

    private List<ICellSubBlockGetter> MergeSubBlocks()
    {
        var subBlocksByNumber = new Dictionary<int, List<ICellSubBlockGetter>>();
        var order = new List<int>();
        foreach (var block in _sourceBlocks)
        {
            foreach (var subBlock in block.SubBlocks)
            {
                if (!subBlocksByNumber.TryGetValue(subBlock.BlockNumber, out var list))
                {
                    list = new List<ICellSubBlockGetter>();
                    subBlocksByNumber[subBlock.BlockNumber] = list;
                    order.Add(subBlock.BlockNumber);
                }
                list.Add(subBlock);
            }
        }
        var result = new List<ICellSubBlockGetter>(order.Count);
        foreach (var number in order)
        {
            var subBlocks = subBlocksByNumber[number];
            result.Add(subBlocks.Count == 1 ? subBlocks[0] : new MergedCellSubBlock(number, subBlocks));
        }
        return result;
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
/// Merged cell sub-block that combines multiple sub-blocks with the same BlockNumber, deduplicating cells by FormKey.
/// </summary>
internal class MergedCellSubBlock : ICellSubBlockGetter
{
    private readonly int _blockNumber;
    private readonly List<ICellSubBlockGetter> _sourceSubBlocks;
    private readonly Lazy<IReadOnlyList<ICellGetter>> _mergedCells;

    public MergedCellSubBlock(int blockNumber, List<ICellSubBlockGetter> sourceSubBlocks)
    {
        _blockNumber = blockNumber;
        _sourceSubBlocks = sourceSubBlocks;
        _mergedCells = new Lazy<IReadOnlyList<ICellGetter>>(MergeCells);
    }

    public int BlockNumber => _blockNumber;
    public GroupTypeEnum GroupType => _sourceSubBlocks[^1].GroupType;
    public int LastModified => _sourceSubBlocks.Max(sb => sb.LastModified);
    public int Unknown => 0;

    private IReadOnlyList<ICellGetter> MergeCells()
    {
        var cellsByFormKey = new Dictionary<FormKey, List<ICellGetter>>();
        var order = new List<FormKey>();
        foreach (var subBlock in _sourceSubBlocks)
        {
            foreach (var cell in subBlock.Cells)
            {
                if (!cellsByFormKey.TryGetValue(cell.FormKey, out var list))
                {
                    list = new List<ICellGetter>();
                    cellsByFormKey[cell.FormKey] = list;
                    order.Add(cell.FormKey);
                }
                list.Add(cell);
            }
        }
        var result = new List<ICellGetter>(order.Count);
        foreach (var formKey in order)
        {
            var cells = cellsByFormKey[formKey];
            result.Add(cells.Count == 1 ? cells[0] : new MergedWorldspaceCell(cells));
        }
        return result;
    }

    public IReadOnlyList<ICellGetter> Cells => _mergedCells.Value;

    ILoquiRegistration ILoquiObject.Registration => null!;
    public void Print(StructuredStringBuilder sb, string? name) => CellSubBlockCommon.Instance.Print(this, sb, name);
    public object CommonInstance() => CellSubBlockCommon.Instance;
    public object? CommonSetterInstance() => null;
    public object CommonSetterTranslationInstance() => CellSubBlockSetterTranslationCommon.Instance;

    object IBinaryItem.BinaryWriteTranslator => CellSubBlockBinaryWriteTranslation.Instance;
    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
        => CellSubBlockBinaryWriteTranslation.Instance.Write(writer: writer, item: this, translationParams: translationParams);

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true)
        => CellSubBlockCommon.Instance.EnumerateFormLinks(this, iterateNestedRecords);

    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
        => CellSubBlockCommon.Instance.EnumerateAssetLinks(this, queryCategories, linkCache, assetType);

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
        => CellSubBlockCommon.Instance.EnumerateMajorRecords(this);

    public IEnumerable<T> EnumerateMajorRecords<T>(bool throwIfUnknown = true) where T : class, IMajorRecordQueryableGetter
        => CellSubBlockCommon.Instance.EnumerateMajorRecords(this, typeof(T), throwIfUnknown).Select(m => (T)m);

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
        => CellSubBlockCommon.Instance.EnumerateMajorRecords(this, type, throwIfUnknown);
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
    ushort IFallout3MajorRecordGetter.FormVersion => _primary.FormVersion;
    ushort? IMajorRecordGetter.FormVersion => _primary.FormVersion;
    ushort? IFormVersionGetter.FormVersion => _primary.FormVersion;
    public ushort Version2 => _primary.Version2;
    public Fallout3MajorRecord.Fallout3MajorRecordFlag Fallout3MajorRecordFlags => _primary.Fallout3MajorRecordFlags;
    public Worldspace.MajorFlag MajorFlags => _primary.MajorFlags;
    public bool IsCompressed => (MajorRecordFlagsRaw & Mutagen.Bethesda.Plugins.Internals.Constants.CompressedFlag) != 0;
    public bool IsDeleted => (MajorRecordFlagsRaw & Mutagen.Bethesda.Plugins.Internals.Constants.DeletedFlag) != 0;
    Type ILinkIdentifier.Type => typeof(IWorldspaceGetter);
    public bool Equals(IFormLinkGetter? other) => other != null && other.FormKey == FormKey && typeof(IWorldspaceGetter).IsAssignableFrom(other.Type);

    public String? Name => _primary.Name;
    public IFormLinkNullableGetter<IEncounterZoneGetter> EncounterZone => _primary.EncounterZone;
    public IWorldspaceParentGetter? Parent => _primary.Parent;
    public IFormLinkNullableGetter<IClimateGetter> Climate => _primary.Climate;
    public IFormLinkNullableGetter<IWaterGetter> Water => _primary.Water;
    public IFormLinkNullableGetter<IWaterGetter> LodWater => _primary.LodWater;
    public Single? LodWaterHeight => _primary.LodWaterHeight;
    public IWorldspaceLandDataGetter? LandData => _primary.LandData;
    public String? MapImage => _primary.MapImage;
    public String? SmallMapImage => _primary.SmallMapImage;
    public IWorldspaceMapDataGetter? MapData => _primary.MapData;
    public Single WorldMapScale => _primary.WorldMapScale;
    public Single CellXOffset => _primary.CellXOffset;
    public Single CellYOffset => _primary.CellYOffset;
    public IFormLinkNullableGetter<IImageSpaceGetter> ImageSpace => _primary.ImageSpace;
    public Worldspace.WorldspaceFlag Flags => _primary.Flags;
    public P2Float? ObjectBoundsMin => _primary.ObjectBoundsMin;
    public P2Float? ObjectBoundsMax => _primary.ObjectBoundsMax;
    public IFormLinkNullableGetter<IMusicTypeGetter> Music => _primary.Music;
    public String? CanopyShadow => _primary.CanopyShadow;
    public String? WaterNoiseTexture => _primary.WaterNoiseTexture;
    public IReadOnlyList<IWorldspaceImpactSwapGetter> ImpactSwapData => _primary.ImpactSwapData;
    public IFootstepMaterialsGetter? FootstepMaterials => _primary.FootstepMaterials;
    public IReadOnlyList<UInt32>? OffsetData => _primary.OffsetData;
    public ICellGetter? TopCell => _primary.TopCell;
    public Int32 SubCellsTimestamp => _primary.SubCellsTimestamp;
    public Int32 SubCellsUnknownGroupData => _primary.SubCellsUnknownGroupData;

    string? INamedGetter.Name => _primary.Name;
    string INamedRequiredGetter.Name => _primary.Name ?? string.Empty;

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
    ushort IFallout3MajorRecordGetter.FormVersion => _primary.FormVersion;
    ushort? IMajorRecordGetter.FormVersion => _primary.FormVersion;
    ushort? IFormVersionGetter.FormVersion => _primary.FormVersion;
    public ushort Version2 => _primary.Version2;
    public Fallout3MajorRecord.Fallout3MajorRecordFlag Fallout3MajorRecordFlags => _primary.Fallout3MajorRecordFlags;
    public Cell.MajorFlag MajorFlags => _primary.MajorFlags;
    public bool IsCompressed => (MajorRecordFlagsRaw & Mutagen.Bethesda.Plugins.Internals.Constants.CompressedFlag) != 0;
    public bool IsDeleted => (MajorRecordFlagsRaw & Mutagen.Bethesda.Plugins.Internals.Constants.DeletedFlag) != 0;
    Type ILinkIdentifier.Type => typeof(ICellGetter);
    public bool Equals(IFormLinkGetter? other) => other != null && other.FormKey == FormKey && typeof(ICellGetter).IsAssignableFrom(other.Type);

    public String? Name => _primary.Name;
    public Cell.Flag Flags => _primary.Flags;
    public ICellGridGetter? Grid => _primary.Grid;
    public ICellLightingGetter? Lighting => _primary.Lighting;
    public IFootstepMaterialsGetter? FootstepMaterials => _primary.FootstepMaterials;
    public IFormLinkNullableGetter<ILightingTemplateGetter> LightTemplate => _primary.LightTemplate;
    public Cell.LightingInheritFlag? LightInheritFlags => _primary.LightInheritFlags;
    public Single? WaterHeight => _primary.WaterHeight;
    public String? WaterNoiseTexture => _primary.WaterNoiseTexture;
    public IReadOnlyList<IFormLinkGetter<IRegionGetter>>? Regions => _primary.Regions;
    public IFormLinkNullableGetter<IImageSpaceGetter> ImageSpace => _primary.ImageSpace;
    public ReadOnlyMemorySlice<Byte>? XCET => _primary.XCET;
    public IFormLinkNullableGetter<IEncounterZoneGetter> EncounterZone => _primary.EncounterZone;
    public IFormLinkNullableGetter<IClimateGetter> Climate => _primary.Climate;
    public IFormLinkNullableGetter<IWaterGetter> Water => _primary.Water;
    public IFormLinkNullableGetter<IOwnerGetter> Owner => _primary.Owner;
    public Int32? FactionRank => _primary.FactionRank;
    public IFormLinkNullableGetter<IAcousticSpaceGetter> AcousticSpace => _primary.AcousticSpace;
    public ReadOnlyMemorySlice<Byte>? XCMT => _primary.XCMT;
    public IFormLinkNullableGetter<IMusicTypeGetter> MusicType => _primary.MusicType;
    public ILandscapeGetter? Landscape => _primary.Landscape;
    public IReadOnlyList<INavigationMeshGetter> NavigationMeshes => _primary.NavigationMeshes;
    public Int32 Timestamp => _primary.Timestamp;
    public Int32 UnknownGroupData => _primary.UnknownGroupData;
    public Int32 PersistentTimestamp => _primary.PersistentTimestamp;
    public Int32 PersistentUnknownGroupData => _primary.PersistentUnknownGroupData;
    public Int32 TemporaryTimestamp => _primary.TemporaryTimestamp;
    public Int32 TemporaryUnknownGroupData => _primary.TemporaryUnknownGroupData;
    public Int32 VisibleWhenDistantTimestamp => _primary.VisibleWhenDistantTimestamp;
    public Int32 VisibleWhenDistantUnknownGroupData => _primary.VisibleWhenDistantUnknownGroupData;
    public IReadOnlyList<IPlacedGetter> VisibleWhenDistant => _primary.VisibleWhenDistant;

    string? INamedGetter.Name => _primary.Name;
    string INamedRequiredGetter.Name => _primary.Name ?? string.Empty;

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

