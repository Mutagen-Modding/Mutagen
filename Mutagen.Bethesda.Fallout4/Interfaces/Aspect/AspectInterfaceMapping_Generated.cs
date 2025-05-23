/*
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * Autogenerated by Loqui.  Do not manually change.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
*/
using System;
using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Mutagen.Bethesda.Plugins.Aspects;
using Loqui;

namespace Mutagen.Bethesda.Fallout4
{
    internal class Fallout4AspectInterfaceMapping : IInterfaceMapping
    {
        public IReadOnlyDictionary<Type, InterfaceMappingResult> InterfaceToObjectTypes { get; }

        public GameCategory GameCategory => GameCategory.Fallout4;

        public Fallout4AspectInterfaceMapping()
        {
            var dict = new Dictionary<Type, InterfaceMappingResult>();
            dict[typeof(IEnchantable)] = new InterfaceMappingResult(
                true,
                new ILoquiRegistration[]
                {
                    Weapon_Registration.Instance,
                },
                new InterfaceMappingTypes(
                    Setter: typeof(IEnchantable),
                    Getter: typeof(IEnchantableGetter)));
            dict[typeof(IEnchantableGetter)] = dict[typeof(IEnchantable)] with { Setter = false };
            dict[typeof(IHarvestable)] = new InterfaceMappingResult(
                true,
                new ILoquiRegistration[]
                {
                    Flora_Registration.Instance,
                    Tree_Registration.Instance,
                },
                new InterfaceMappingTypes(
                    Setter: typeof(IHarvestable),
                    Getter: typeof(IHarvestableGetter)));
            dict[typeof(IHarvestableGetter)] = dict[typeof(IHarvestable)] with { Setter = false };
            dict[typeof(IHasDestructible)] = new InterfaceMappingResult(
                true,
                new ILoquiRegistration[]
                {
                    Activator_Registration.Instance,
                    Ammunition_Registration.Instance,
                    Armor_Registration.Instance,
                    Book_Registration.Instance,
                    Container_Registration.Instance,
                    Door_Registration.Instance,
                    Flora_Registration.Instance,
                    Furniture_Registration.Instance,
                    Ingestible_Registration.Instance,
                    Ingredient_Registration.Instance,
                    Key_Registration.Instance,
                    Light_Registration.Instance,
                    MiscItem_Registration.Instance,
                    MovableStatic_Registration.Instance,
                    Npc_Registration.Instance,
                    Projectile_Registration.Instance,
                    TalkingActivator_Registration.Instance,
                    Weapon_Registration.Instance,
                },
                new InterfaceMappingTypes(
                    Setter: typeof(IHasDestructible),
                    Getter: typeof(IHasDestructibleGetter)));
            dict[typeof(IHasDestructibleGetter)] = dict[typeof(IHasDestructible)] with { Setter = false };
            dict[typeof(IHasIcons)] = new InterfaceMappingResult(
                true,
                new ILoquiRegistration[]
                {
                    ArmorModel_Registration.Instance,
                    Book_Registration.Instance,
                    Holotape_Registration.Instance,
                    Ingestible_Registration.Instance,
                    Ingredient_Registration.Instance,
                    Key_Registration.Instance,
                    Light_Registration.Instance,
                    MiscItem_Registration.Instance,
                    RegionData_Registration.Instance,
                    RegionGrasses_Registration.Instance,
                    RegionLand_Registration.Instance,
                    RegionMap_Registration.Instance,
                    RegionObjects_Registration.Instance,
                    RegionSounds_Registration.Instance,
                    RegionWeather_Registration.Instance,
                    Weapon_Registration.Instance,
                },
                new InterfaceMappingTypes(
                    Setter: typeof(IHasIcons),
                    Getter: typeof(IHasIconsGetter)));
            dict[typeof(IHasIconsGetter)] = dict[typeof(IHasIcons)] with { Setter = false };
            dict[typeof(IHaveVirtualMachineAdapter)] = new InterfaceMappingResult(
                true,
                new ILoquiRegistration[]
                {
                    Activator_Registration.Instance,
                    APlacedTrap_Registration.Instance,
                    Armor_Registration.Instance,
                    Book_Registration.Instance,
                    Container_Registration.Instance,
                    DialogResponses_Registration.Instance,
                    Door_Registration.Instance,
                    Flora_Registration.Instance,
                    Furniture_Registration.Instance,
                    Holotape_Registration.Instance,
                    Ingredient_Registration.Instance,
                    Key_Registration.Instance,
                    Light_Registration.Instance,
                    MagicEffect_Registration.Instance,
                    MiscItem_Registration.Instance,
                    MovableStatic_Registration.Instance,
                    Npc_Registration.Instance,
                    Package_Registration.Instance,
                    Perk_Registration.Instance,
                    PlacedArrow_Registration.Instance,
                    PlacedBarrier_Registration.Instance,
                    PlacedBeam_Registration.Instance,
                    PlacedCone_Registration.Instance,
                    PlacedFlame_Registration.Instance,
                    PlacedHazard_Registration.Instance,
                    PlacedMissile_Registration.Instance,
                    PlacedNpc_Registration.Instance,
                    PlacedObject_Registration.Instance,
                    PlacedTrap_Registration.Instance,
                    Quest_Registration.Instance,
                    Scene_Registration.Instance,
                    Static_Registration.Instance,
                    StaticCollection_Registration.Instance,
                    TalkingActivator_Registration.Instance,
                    Terminal_Registration.Instance,
                    Tree_Registration.Instance,
                    Weapon_Registration.Instance,
                },
                new InterfaceMappingTypes(
                    Setter: typeof(IHaveVirtualMachineAdapter),
                    Getter: typeof(IHaveVirtualMachineAdapterGetter)));
            dict[typeof(IHaveVirtualMachineAdapterGetter)] = dict[typeof(IHaveVirtualMachineAdapter)] with { Setter = false };
            dict[typeof(IKeywordCommon)] = new InterfaceMappingResult(
                true,
                new ILoquiRegistration[]
                {
                    Keyword_Registration.Instance,
                },
                new InterfaceMappingTypes(
                    Setter: typeof(IKeywordCommon),
                    Getter: typeof(IKeywordCommonGetter)));
            dict[typeof(IKeywordCommonGetter)] = dict[typeof(IKeywordCommon)] with { Setter = false };
            dict[typeof(IKeyworded<IKeywordGetter>)] = new InterfaceMappingResult(
                true,
                new ILoquiRegistration[]
                {
                    Activator_Registration.Instance,
                    Ammunition_Registration.Instance,
                    Armor_Registration.Instance,
                    ArtObject_Registration.Instance,
                    Book_Registration.Instance,
                    Container_Registration.Instance,
                    Door_Registration.Instance,
                    Flora_Registration.Instance,
                    Furniture_Registration.Instance,
                    IdleMarker_Registration.Instance,
                    Ingestible_Registration.Instance,
                    Ingredient_Registration.Instance,
                    InstanceNamingRule_Registration.Instance,
                    Key_Registration.Instance,
                    Light_Registration.Instance,
                    Location_Registration.Instance,
                    MagicEffect_Registration.Instance,
                    MiscItem_Registration.Instance,
                    MovableStatic_Registration.Instance,
                    Npc_Registration.Instance,
                    ObjectTemplate_Registration.Instance,
                    QuestReferenceAlias_Registration.Instance,
                    Race_Registration.Instance,
                    Scene_Registration.Instance,
                    SoundKeywordMapping_Registration.Instance,
                    Spell_Registration.Instance,
                    TalkingActivator_Registration.Instance,
                    Terminal_Registration.Instance,
                    Weapon_Registration.Instance,
                },
                new InterfaceMappingTypes(
                    Setter: null,
                    Getter: typeof(IKeywordedGetter<IKeywordGetter>)));
            dict[typeof(IKeywordedGetter<IKeywordGetter>)] = dict[typeof(IKeyworded<IKeywordGetter>)] with { Setter = false };
            dict[typeof(IModeled)] = new InterfaceMappingResult(
                true,
                new ILoquiRegistration[]
                {
                    Activator_Registration.Instance,
                    AddonNode_Registration.Instance,
                    Ammunition_Registration.Instance,
                    AnimatedObject_Registration.Instance,
                    AObjectModification_Registration.Instance,
                    ArmorModel_Registration.Instance,
                    ArmorModification_Registration.Instance,
                    ArtObject_Registration.Instance,
                    BodyData_Registration.Instance,
                    BodyPartData_Registration.Instance,
                    Book_Registration.Instance,
                    CameraShot_Registration.Instance,
                    Climate_Registration.Instance,
                    Container_Registration.Instance,
                    DestructionStage_Registration.Instance,
                    Door_Registration.Instance,
                    EffectShader_Registration.Instance,
                    Explosion_Registration.Instance,
                    Flora_Registration.Instance,
                    Furniture_Registration.Instance,
                    Grass_Registration.Instance,
                    Hazard_Registration.Instance,
                    HeadPart_Registration.Instance,
                    Holotape_Registration.Instance,
                    IdleMarker_Registration.Instance,
                    Impact_Registration.Instance,
                    Ingestible_Registration.Instance,
                    Ingredient_Registration.Instance,
                    Key_Registration.Instance,
                    LeveledNpc_Registration.Instance,
                    Light_Registration.Instance,
                    MaterialObject_Registration.Instance,
                    MiscItem_Registration.Instance,
                    MovableStatic_Registration.Instance,
                    NpcModification_Registration.Instance,
                    ObjectModification_Registration.Instance,
                    Projectile_Registration.Instance,
                    Static_Registration.Instance,
                    StaticCollection_Registration.Instance,
                    TalkingActivator_Registration.Instance,
                    Terminal_Registration.Instance,
                    Tree_Registration.Instance,
                    UnknownObjectModification_Registration.Instance,
                    Weapon_Registration.Instance,
                    WeaponModification_Registration.Instance,
                },
                new InterfaceMappingTypes(
                    Setter: typeof(IModeled),
                    Getter: typeof(IModeledGetter)));
            dict[typeof(IModeledGetter)] = dict[typeof(IModeled)] with { Setter = false };
            dict[typeof(INamed)] = new InterfaceMappingResult(
                true,
                new ILoquiRegistration[]
                {
                    ActionRecord_Registration.Instance,
                    Activator_Registration.Instance,
                    ActorValueInformation_Registration.Instance,
                    Ammunition_Registration.Instance,
                    AObjectModification_Registration.Instance,
                    APackageData_Registration.Instance,
                    Armor_Registration.Instance,
                    ArmorModification_Registration.Instance,
                    BodyPart_Registration.Instance,
                    Bone_Registration.Instance,
                    Book_Registration.Instance,
                    Cell_Registration.Instance,
                    Class_Registration.Instance,
                    ColorRecord_Registration.Instance,
                    Component_Registration.Instance,
                    Container_Registration.Instance,
                    DialogTopic_Registration.Instance,
                    Door_Registration.Instance,
                    Explosion_Registration.Instance,
                    FaceMorph_Registration.Instance,
                    Faction_Registration.Instance,
                    FormList_Registration.Instance,
                    Furniture_Registration.Instance,
                    Hazard_Registration.Instance,
                    HeadPart_Registration.Instance,
                    Holotape_Registration.Instance,
                    Ingestible_Registration.Instance,
                    Ingredient_Registration.Instance,
                    InstanceNamingRule_Registration.Instance,
                    Keyword_Registration.Instance,
                    Light_Registration.Instance,
                    Location_Registration.Instance,
                    MagicEffect_Registration.Instance,
                    MaterialType_Registration.Instance,
                    Message_Registration.Instance,
                    MiscItem_Registration.Instance,
                    MorphGroup_Registration.Instance,
                    MorphPreset_Registration.Instance,
                    MovableStatic_Registration.Instance,
                    MovementType_Registration.Instance,
                    Npc_Registration.Instance,
                    NpcModification_Registration.Instance,
                    ObjectEffect_Registration.Instance,
                    ObjectModification_Registration.Instance,
                    ObjectTemplate_Registration.Instance,
                    PackageDataBool_Registration.Instance,
                    PackageDataFloat_Registration.Instance,
                    PackageDataInt_Registration.Instance,
                    PackageDataLocation_Registration.Instance,
                    PackageDataObjectList_Registration.Instance,
                    PackageDataTarget_Registration.Instance,
                    PackageDataTopic_Registration.Instance,
                    Perk_Registration.Instance,
                    Projectile_Registration.Instance,
                    Quest_Registration.Instance,
                    QuestLocationAlias_Registration.Instance,
                    QuestReferenceAlias_Registration.Instance,
                    Race_Registration.Instance,
                    ReferenceGroup_Registration.Instance,
                    RegionMap_Registration.Instance,
                    SceneAction_Registration.Instance,
                    ScenePhase_Registration.Instance,
                    SoundCategory_Registration.Instance,
                    Spell_Registration.Instance,
                    Static_Registration.Instance,
                    StaticCollection_Registration.Instance,
                    TalkingActivator_Registration.Instance,
                    Terminal_Registration.Instance,
                    TintGroup_Registration.Instance,
                    TintTemplateOption_Registration.Instance,
                    Tree_Registration.Instance,
                    UnknownObjectModification_Registration.Instance,
                    Water_Registration.Instance,
                    Weapon_Registration.Instance,
                    WeaponModification_Registration.Instance,
                    Worldspace_Registration.Instance,
                },
                new InterfaceMappingTypes(
                    Setter: typeof(INamed),
                    Getter: typeof(INamedGetter)));
            dict[typeof(INamedGetter)] = dict[typeof(INamed)] with { Setter = false };
            dict[typeof(INamedRequired)] = new InterfaceMappingResult(
                true,
                new ILoquiRegistration[]
                {
                    ActionRecord_Registration.Instance,
                    Activator_Registration.Instance,
                    ActorValueInformation_Registration.Instance,
                    Ammunition_Registration.Instance,
                    AObjectModification_Registration.Instance,
                    APackageData_Registration.Instance,
                    Armor_Registration.Instance,
                    ArmorModification_Registration.Instance,
                    BipedObjectData_Registration.Instance,
                    BodyPart_Registration.Instance,
                    Bone_Registration.Instance,
                    Book_Registration.Instance,
                    Cell_Registration.Instance,
                    Class_Registration.Instance,
                    CollisionLayer_Registration.Instance,
                    ColorRecord_Registration.Instance,
                    Component_Registration.Instance,
                    Container_Registration.Instance,
                    DialogTopic_Registration.Instance,
                    Door_Registration.Instance,
                    Explosion_Registration.Instance,
                    FaceMorph_Registration.Instance,
                    Faction_Registration.Instance,
                    Flora_Registration.Instance,
                    FormList_Registration.Instance,
                    Furniture_Registration.Instance,
                    Hazard_Registration.Instance,
                    HeadPart_Registration.Instance,
                    Holotape_Registration.Instance,
                    Ingestible_Registration.Instance,
                    Ingredient_Registration.Instance,
                    InstanceNamingRule_Registration.Instance,
                    Key_Registration.Instance,
                    Keyword_Registration.Instance,
                    Light_Registration.Instance,
                    Location_Registration.Instance,
                    MagicEffect_Registration.Instance,
                    MaterialType_Registration.Instance,
                    Message_Registration.Instance,
                    MiscItem_Registration.Instance,
                    MorphGroup_Registration.Instance,
                    MorphPreset_Registration.Instance,
                    MovableStatic_Registration.Instance,
                    MovementType_Registration.Instance,
                    Npc_Registration.Instance,
                    NpcModification_Registration.Instance,
                    ObjectEffect_Registration.Instance,
                    ObjectModification_Registration.Instance,
                    ObjectTemplate_Registration.Instance,
                    PackageDataBool_Registration.Instance,
                    PackageDataFloat_Registration.Instance,
                    PackageDataInt_Registration.Instance,
                    PackageDataLocation_Registration.Instance,
                    PackageDataObjectList_Registration.Instance,
                    PackageDataTarget_Registration.Instance,
                    PackageDataTopic_Registration.Instance,
                    Perk_Registration.Instance,
                    Phoneme_Registration.Instance,
                    PlacedObjectMapMarker_Registration.Instance,
                    Projectile_Registration.Instance,
                    Quest_Registration.Instance,
                    QuestLocationAlias_Registration.Instance,
                    QuestReferenceAlias_Registration.Instance,
                    Race_Registration.Instance,
                    ReferenceGroup_Registration.Instance,
                    RegionMap_Registration.Instance,
                    SceneAction_Registration.Instance,
                    ScenePhase_Registration.Instance,
                    ScriptBoolListProperty_Registration.Instance,
                    ScriptBoolProperty_Registration.Instance,
                    ScriptEntry_Registration.Instance,
                    ScriptFloatListProperty_Registration.Instance,
                    ScriptFloatProperty_Registration.Instance,
                    ScriptIntListProperty_Registration.Instance,
                    ScriptIntProperty_Registration.Instance,
                    ScriptObjectListProperty_Registration.Instance,
                    ScriptObjectProperty_Registration.Instance,
                    ScriptProperty_Registration.Instance,
                    ScriptStringListProperty_Registration.Instance,
                    ScriptStringProperty_Registration.Instance,
                    ScriptStructListProperty_Registration.Instance,
                    ScriptStructProperty_Registration.Instance,
                    ScriptVariableListProperty_Registration.Instance,
                    ScriptVariableProperty_Registration.Instance,
                    SoundCategory_Registration.Instance,
                    Spell_Registration.Instance,
                    Static_Registration.Instance,
                    StaticCollection_Registration.Instance,
                    TalkingActivator_Registration.Instance,
                    Terminal_Registration.Instance,
                    TintGroup_Registration.Instance,
                    TintTemplateOption_Registration.Instance,
                    Tree_Registration.Instance,
                    UnknownObjectModification_Registration.Instance,
                    Water_Registration.Instance,
                    Weapon_Registration.Instance,
                    WeaponModification_Registration.Instance,
                    Worldspace_Registration.Instance,
                },
                new InterfaceMappingTypes(
                    Setter: typeof(INamedRequired),
                    Getter: typeof(INamedRequiredGetter)));
            dict[typeof(INamedRequiredGetter)] = dict[typeof(INamedRequired)] with { Setter = false };
            dict[typeof(ITranslatedNamed)] = new InterfaceMappingResult(
                true,
                new ILoquiRegistration[]
                {
                    Activator_Registration.Instance,
                    ActorValueInformation_Registration.Instance,
                    Ammunition_Registration.Instance,
                    AObjectModification_Registration.Instance,
                    Armor_Registration.Instance,
                    ArmorModification_Registration.Instance,
                    BodyPart_Registration.Instance,
                    Book_Registration.Instance,
                    Cell_Registration.Instance,
                    Class_Registration.Instance,
                    ColorRecord_Registration.Instance,
                    Component_Registration.Instance,
                    Container_Registration.Instance,
                    DialogTopic_Registration.Instance,
                    Door_Registration.Instance,
                    Explosion_Registration.Instance,
                    FaceMorph_Registration.Instance,
                    Faction_Registration.Instance,
                    FormList_Registration.Instance,
                    Furniture_Registration.Instance,
                    Hazard_Registration.Instance,
                    HeadPart_Registration.Instance,
                    Holotape_Registration.Instance,
                    Ingestible_Registration.Instance,
                    Ingredient_Registration.Instance,
                    InstanceNamingRule_Registration.Instance,
                    Keyword_Registration.Instance,
                    Light_Registration.Instance,
                    Location_Registration.Instance,
                    MagicEffect_Registration.Instance,
                    Message_Registration.Instance,
                    MiscItem_Registration.Instance,
                    MorphPreset_Registration.Instance,
                    MovableStatic_Registration.Instance,
                    Npc_Registration.Instance,
                    NpcModification_Registration.Instance,
                    ObjectEffect_Registration.Instance,
                    ObjectModification_Registration.Instance,
                    ObjectTemplate_Registration.Instance,
                    Perk_Registration.Instance,
                    Projectile_Registration.Instance,
                    Quest_Registration.Instance,
                    Race_Registration.Instance,
                    RegionMap_Registration.Instance,
                    SoundCategory_Registration.Instance,
                    Spell_Registration.Instance,
                    Static_Registration.Instance,
                    StaticCollection_Registration.Instance,
                    TalkingActivator_Registration.Instance,
                    Terminal_Registration.Instance,
                    TintGroup_Registration.Instance,
                    TintTemplateOption_Registration.Instance,
                    Tree_Registration.Instance,
                    UnknownObjectModification_Registration.Instance,
                    Water_Registration.Instance,
                    Weapon_Registration.Instance,
                    WeaponModification_Registration.Instance,
                    Worldspace_Registration.Instance,
                },
                new InterfaceMappingTypes(
                    Setter: typeof(ITranslatedNamed),
                    Getter: typeof(ITranslatedNamedGetter)));
            dict[typeof(ITranslatedNamedGetter)] = dict[typeof(ITranslatedNamed)] with { Setter = false };
            dict[typeof(ITranslatedNamedRequired)] = new InterfaceMappingResult(
                true,
                new ILoquiRegistration[]
                {
                    Activator_Registration.Instance,
                    ActorValueInformation_Registration.Instance,
                    Ammunition_Registration.Instance,
                    AObjectModification_Registration.Instance,
                    Armor_Registration.Instance,
                    ArmorModification_Registration.Instance,
                    BodyPart_Registration.Instance,
                    Book_Registration.Instance,
                    Cell_Registration.Instance,
                    Class_Registration.Instance,
                    ColorRecord_Registration.Instance,
                    Component_Registration.Instance,
                    Container_Registration.Instance,
                    DialogTopic_Registration.Instance,
                    Door_Registration.Instance,
                    Explosion_Registration.Instance,
                    FaceMorph_Registration.Instance,
                    Faction_Registration.Instance,
                    Flora_Registration.Instance,
                    FormList_Registration.Instance,
                    Furniture_Registration.Instance,
                    Hazard_Registration.Instance,
                    HeadPart_Registration.Instance,
                    Holotape_Registration.Instance,
                    Ingestible_Registration.Instance,
                    Ingredient_Registration.Instance,
                    InstanceNamingRule_Registration.Instance,
                    Key_Registration.Instance,
                    Keyword_Registration.Instance,
                    Light_Registration.Instance,
                    Location_Registration.Instance,
                    MagicEffect_Registration.Instance,
                    Message_Registration.Instance,
                    MiscItem_Registration.Instance,
                    MorphPreset_Registration.Instance,
                    MovableStatic_Registration.Instance,
                    Npc_Registration.Instance,
                    NpcModification_Registration.Instance,
                    ObjectEffect_Registration.Instance,
                    ObjectModification_Registration.Instance,
                    ObjectTemplate_Registration.Instance,
                    Perk_Registration.Instance,
                    PlacedObjectMapMarker_Registration.Instance,
                    Projectile_Registration.Instance,
                    Quest_Registration.Instance,
                    Race_Registration.Instance,
                    RegionMap_Registration.Instance,
                    SoundCategory_Registration.Instance,
                    Spell_Registration.Instance,
                    Static_Registration.Instance,
                    StaticCollection_Registration.Instance,
                    TalkingActivator_Registration.Instance,
                    Terminal_Registration.Instance,
                    TintGroup_Registration.Instance,
                    TintTemplateOption_Registration.Instance,
                    Tree_Registration.Instance,
                    UnknownObjectModification_Registration.Instance,
                    Water_Registration.Instance,
                    Weapon_Registration.Instance,
                    WeaponModification_Registration.Instance,
                    Worldspace_Registration.Instance,
                },
                new InterfaceMappingTypes(
                    Setter: typeof(ITranslatedNamedRequired),
                    Getter: typeof(ITranslatedNamedRequiredGetter)));
            dict[typeof(ITranslatedNamedRequiredGetter)] = dict[typeof(ITranslatedNamedRequired)] with { Setter = false };
            dict[typeof(IObjectBounded)] = new InterfaceMappingResult(
                true,
                new ILoquiRegistration[]
                {
                    AcousticSpace_Registration.Instance,
                    Activator_Registration.Instance,
                    AddonNode_Registration.Instance,
                    Ammunition_Registration.Instance,
                    Armor_Registration.Instance,
                    ArtObject_Registration.Instance,
                    Book_Registration.Instance,
                    Component_Registration.Instance,
                    Container_Registration.Instance,
                    Door_Registration.Instance,
                    Explosion_Registration.Instance,
                    Flora_Registration.Instance,
                    Furniture_Registration.Instance,
                    Grass_Registration.Instance,
                    Hazard_Registration.Instance,
                    IdleMarker_Registration.Instance,
                    Ingestible_Registration.Instance,
                    Ingredient_Registration.Instance,
                    Key_Registration.Instance,
                    LeveledItem_Registration.Instance,
                    LeveledNpc_Registration.Instance,
                    Light_Registration.Instance,
                    MiscItem_Registration.Instance,
                    MovableStatic_Registration.Instance,
                    Npc_Registration.Instance,
                    ObjectEffect_Registration.Instance,
                    Projectile_Registration.Instance,
                    SoundMarker_Registration.Instance,
                    Spell_Registration.Instance,
                    Static_Registration.Instance,
                    StaticCollection_Registration.Instance,
                    TalkingActivator_Registration.Instance,
                    Terminal_Registration.Instance,
                    TextureSet_Registration.Instance,
                    Tree_Registration.Instance,
                    Weapon_Registration.Instance,
                },
                new InterfaceMappingTypes(
                    Setter: typeof(IObjectBounded),
                    Getter: typeof(IObjectBoundedGetter)));
            dict[typeof(IObjectBoundedGetter)] = dict[typeof(IObjectBounded)] with { Setter = false };
            dict[typeof(IObjectBoundedOptional)] = new InterfaceMappingResult(
                true,
                new ILoquiRegistration[]
                {
                    BendableSpline_Registration.Instance,
                    Holotape_Registration.Instance,
                    PackIn_Registration.Instance,
                },
                new InterfaceMappingTypes(
                    Setter: typeof(IObjectBoundedOptional),
                    Getter: typeof(IObjectBoundedOptionalGetter)));
            dict[typeof(IObjectBoundedOptionalGetter)] = dict[typeof(IObjectBoundedOptional)] with { Setter = false };
            dict[typeof(IPositionRotation)] = new InterfaceMappingResult(
                true,
                new ILoquiRegistration[]
                {
                    APlacedTrap_Registration.Instance,
                    NpcFaceMorph_Registration.Instance,
                    PlacedArrow_Registration.Instance,
                    PlacedBarrier_Registration.Instance,
                    PlacedBeam_Registration.Instance,
                    PlacedCone_Registration.Instance,
                    PlacedFlame_Registration.Instance,
                    PlacedHazard_Registration.Instance,
                    PlacedMissile_Registration.Instance,
                    PlacedNpc_Registration.Instance,
                    PlacedObject_Registration.Instance,
                    PlacedTrap_Registration.Instance,
                    RagdollData_Registration.Instance,
                    StaticPlacement_Registration.Instance,
                    TeleportDestination_Registration.Instance,
                    Transform_Registration.Instance,
                },
                new InterfaceMappingTypes(
                    Setter: typeof(IPositionRotation),
                    Getter: typeof(IPositionRotationGetter)));
            dict[typeof(IPositionRotationGetter)] = dict[typeof(IPositionRotation)] with { Setter = false };
            dict[typeof(IScripted)] = new InterfaceMappingResult(
                true,
                new ILoquiRegistration[]
                {
                    Activator_Registration.Instance,
                    APlacedTrap_Registration.Instance,
                    Armor_Registration.Instance,
                    Book_Registration.Instance,
                    Container_Registration.Instance,
                    Door_Registration.Instance,
                    Flora_Registration.Instance,
                    Furniture_Registration.Instance,
                    Holotape_Registration.Instance,
                    Ingredient_Registration.Instance,
                    Key_Registration.Instance,
                    Light_Registration.Instance,
                    MagicEffect_Registration.Instance,
                    MiscItem_Registration.Instance,
                    MovableStatic_Registration.Instance,
                    Npc_Registration.Instance,
                    PlacedArrow_Registration.Instance,
                    PlacedBarrier_Registration.Instance,
                    PlacedBeam_Registration.Instance,
                    PlacedCone_Registration.Instance,
                    PlacedFlame_Registration.Instance,
                    PlacedHazard_Registration.Instance,
                    PlacedMissile_Registration.Instance,
                    PlacedNpc_Registration.Instance,
                    PlacedObject_Registration.Instance,
                    PlacedTrap_Registration.Instance,
                    Static_Registration.Instance,
                    StaticCollection_Registration.Instance,
                    TalkingActivator_Registration.Instance,
                    Tree_Registration.Instance,
                    Weapon_Registration.Instance,
                },
                new InterfaceMappingTypes(
                    Setter: typeof(IScripted),
                    Getter: typeof(IScriptedGetter)));
            dict[typeof(IScriptedGetter)] = dict[typeof(IScripted)] with { Setter = false };
            dict[typeof(IWeightValue)] = new InterfaceMappingResult(
                true,
                new ILoquiRegistration[]
                {
                    Ammunition_Registration.Instance,
                    Book_Registration.Instance,
                    Holotape_Registration.Instance,
                    Ingestible_Registration.Instance,
                    Key_Registration.Instance,
                    Light_Registration.Instance,
                    Weapon_Registration.Instance,
                },
                new InterfaceMappingTypes(
                    Setter: typeof(IWeightValue),
                    Getter: typeof(IWeightValueGetter)));
            dict[typeof(IWeightValueGetter)] = dict[typeof(IWeightValue)] with { Setter = false };
            InterfaceToObjectTypes = dict;
        }
    }
}

