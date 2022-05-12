/*
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * Autogenerated by Loqui.  Do not manually change.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
*/
using System;
using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Loqui;

namespace Mutagen.Bethesda.Fallout4;

internal class Fallout4LinkInterfaceMapping : IInterfaceMapping
{
    public IReadOnlyDictionary<Type, InterfaceMappingResult> InterfaceToObjectTypes { get; }

    public GameCategory GameCategory => GameCategory.Fallout4;

    public Fallout4LinkInterfaceMapping()
    {
        var dict = new Dictionary<Type, InterfaceMappingResult>();
        dict[typeof(IPlaceableObject)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            AcousticSpace_Registration.Instance,
            Activator_Registration.Instance,
            Ammunition_Registration.Instance,
            Armor_Registration.Instance,
            BendableSpline_Registration.Instance,
            Book_Registration.Instance,
            Container_Registration.Instance,
            Door_Registration.Instance,
            Flora_Registration.Instance,
            Furniture_Registration.Instance,
            Holotape_Registration.Instance,
            IdleMarker_Registration.Instance,
            Ingestible_Registration.Instance,
            Key_Registration.Instance,
            Light_Registration.Instance,
            MiscItem_Registration.Instance,
            MovableStatic_Registration.Instance,
            SoundMarker_Registration.Instance,
            StaticCollection_Registration.Instance,
            TalkingActivator_Registration.Instance,
            Terminal_Registration.Instance,
            TextureSet_Registration.Instance,
            Weapon_Registration.Instance,
        });
        dict[typeof(IPlaceableObjectGetter)] = dict[typeof(IPlaceableObject)] with { Setter = false };
        dict[typeof(IIdleRelation)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            ActionRecord_Registration.Instance,
            IdleAnimation_Registration.Instance,
        });
        dict[typeof(IIdleRelationGetter)] = dict[typeof(IIdleRelation)] with { Setter = false };
        dict[typeof(IObjectId)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            Activator_Registration.Instance,
            Ammunition_Registration.Instance,
            Armor_Registration.Instance,
            Book_Registration.Instance,
            Container_Registration.Instance,
            Door_Registration.Instance,
            Faction_Registration.Instance,
            FormList_Registration.Instance,
            Furniture_Registration.Instance,
            Holotape_Registration.Instance,
            IdleMarker_Registration.Instance,
            Ingestible_Registration.Instance,
            Ingredient_Registration.Instance,
            Key_Registration.Instance,
            Light_Registration.Instance,
            MiscItem_Registration.Instance,
            MovableStatic_Registration.Instance,
            Npc_Registration.Instance,
            ObjectModification_Registration.Instance,
            Projectile_Registration.Instance,
            Spell_Registration.Instance,
            Static_Registration.Instance,
            TextureSet_Registration.Instance,
            Weapon_Registration.Instance,
        });
        dict[typeof(IObjectIdGetter)] = dict[typeof(IObjectId)] with { Setter = false };
        dict[typeof(IStaticObject)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            Activator_Registration.Instance,
            Ammunition_Registration.Instance,
            Book_Registration.Instance,
            Container_Registration.Instance,
            Door_Registration.Instance,
            Flora_Registration.Instance,
            Furniture_Registration.Instance,
            Ingestible_Registration.Instance,
            MiscItem_Registration.Instance,
            MovableStatic_Registration.Instance,
            Static_Registration.Instance,
            Terminal_Registration.Instance,
            Weapon_Registration.Instance,
        });
        dict[typeof(IStaticObjectGetter)] = dict[typeof(IStaticObject)] with { Setter = false };
        dict[typeof(IDamageTypeTarget)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            ActorValueInformation_Registration.Instance,
        });
        dict[typeof(IDamageTypeTargetGetter)] = dict[typeof(IDamageTypeTarget)] with { Setter = false };
        dict[typeof(IHarvestTarget)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            Ammunition_Registration.Instance,
            Armor_Registration.Instance,
            Book_Registration.Instance,
            ConstructibleObject_Registration.Instance,
            Holotape_Registration.Instance,
            Ingestible_Registration.Instance,
            Ingredient_Registration.Instance,
            Key_Registration.Instance,
            LeveledItem_Registration.Instance,
            LeveledItem_Registration.Instance,
            Light_Registration.Instance,
            MiscItem_Registration.Instance,
            Weapon_Registration.Instance,
        });
        dict[typeof(IHarvestTargetGetter)] = dict[typeof(IHarvestTarget)] with { Setter = false };
        dict[typeof(IItem)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            Armor_Registration.Instance,
            Book_Registration.Instance,
            Ingestible_Registration.Instance,
            Ingredient_Registration.Instance,
            Key_Registration.Instance,
            LeveledItem_Registration.Instance,
            Light_Registration.Instance,
            MiscItem_Registration.Instance,
        });
        dict[typeof(IItemGetter)] = dict[typeof(IItem)] with { Setter = false };
        dict[typeof(IOutfitTarget)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            Armor_Registration.Instance,
            LeveledItem_Registration.Instance,
        });
        dict[typeof(IOutfitTargetGetter)] = dict[typeof(IOutfitTarget)] with { Setter = false };
        dict[typeof(IConstructible)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            Armor_Registration.Instance,
            Book_Registration.Instance,
            Furniture_Registration.Instance,
            Ingestible_Registration.Instance,
            Ingredient_Registration.Instance,
            Key_Registration.Instance,
            Light_Registration.Instance,
            MiscItem_Registration.Instance,
        });
        dict[typeof(IConstructibleGetter)] = dict[typeof(IConstructible)] with { Setter = false };
        dict[typeof(IBindableEquipment)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            Armor_Registration.Instance,
        });
        dict[typeof(IBindableEquipmentGetter)] = dict[typeof(IBindableEquipment)] with { Setter = false };
        dict[typeof(IFurnitureAssociation)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            Armor_Registration.Instance,
            Hazard_Registration.Instance,
            Perk_Registration.Instance,
            Spell_Registration.Instance,
            Weapon_Registration.Instance,
        });
        dict[typeof(IFurnitureAssociationGetter)] = dict[typeof(IFurnitureAssociation)] with { Setter = false };
        dict[typeof(IOwner)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            Faction_Registration.Instance,
        });
        dict[typeof(IOwnerGetter)] = dict[typeof(IOwner)] with { Setter = false };
        dict[typeof(IRelatable)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            Faction_Registration.Instance,
            Race_Registration.Instance,
        });
        dict[typeof(IRelatableGetter)] = dict[typeof(IRelatable)] with { Setter = false };
        dict[typeof(IRegionTarget)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            Flora_Registration.Instance,
            LandscapeTexture_Registration.Instance,
            MovableStatic_Registration.Instance,
            Tree_Registration.Instance,
        });
        dict[typeof(IRegionTargetGetter)] = dict[typeof(IRegionTarget)] with { Setter = false };
        dict[typeof(ILockList)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            FormList_Registration.Instance,
            Npc_Registration.Instance,
        });
        dict[typeof(ILockListGetter)] = dict[typeof(ILockList)] with { Setter = false };
        dict[typeof(IPlacedTrapTarget)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            Hazard_Registration.Instance,
            Projectile_Registration.Instance,
        });
        dict[typeof(IPlacedTrapTargetGetter)] = dict[typeof(IPlacedTrapTarget)] with { Setter = false };
        dict[typeof(IKeywordLinkedReference)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            Keyword_Registration.Instance,
            APlacedTrap_Registration.Instance,
        });
        dict[typeof(IKeywordLinkedReferenceGetter)] = dict[typeof(IKeywordLinkedReference)] with { Setter = false };
        dict[typeof(INpcSpawn)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            LeveledNpc_Registration.Instance,
        });
        dict[typeof(INpcSpawnGetter)] = dict[typeof(INpcSpawn)] with { Setter = false };
        dict[typeof(ISpellRecord)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            LeveledSpell_Registration.Instance,
            Spell_Registration.Instance,
        });
        dict[typeof(ISpellRecordGetter)] = dict[typeof(ISpellRecord)] with { Setter = false };
        dict[typeof(IEmittance)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            Light_Registration.Instance,
            Region_Registration.Instance,
        });
        dict[typeof(IEmittanceGetter)] = dict[typeof(IEmittance)] with { Setter = false };
        dict[typeof(ILocationRecord)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            Location_Registration.Instance,
            LocationReferenceType_Registration.Instance,
        });
        dict[typeof(ILocationRecordGetter)] = dict[typeof(ILocationRecord)] with { Setter = false };
        dict[typeof(IEffectRecord)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            ObjectEffect_Registration.Instance,
            Spell_Registration.Instance,
        });
        dict[typeof(IEffectRecordGetter)] = dict[typeof(IEffectRecord)] with { Setter = false };
        dict[typeof(ILocationTargetable)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            PlacedNpc_Registration.Instance,
            PlacedObject_Registration.Instance,
            APlacedTrap_Registration.Instance,
        });
        dict[typeof(ILocationTargetableGetter)] = dict[typeof(ILocationTargetable)] with { Setter = false };
        dict[typeof(IPlaced)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            PlacedNpc_Registration.Instance,
            PlacedObject_Registration.Instance,
            APlacedTrap_Registration.Instance,
        });
        dict[typeof(IPlacedGetter)] = dict[typeof(IPlaced)] with { Setter = false };
        dict[typeof(IPlacedSimple)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            PlacedNpc_Registration.Instance,
            PlacedObject_Registration.Instance,
        });
        dict[typeof(IPlacedSimpleGetter)] = dict[typeof(IPlacedSimple)] with { Setter = false };
        dict[typeof(IPlacedThing)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            PlacedObject_Registration.Instance,
            APlacedTrap_Registration.Instance,
        });
        dict[typeof(IPlacedThingGetter)] = dict[typeof(IPlacedThing)] with { Setter = false };
        dict[typeof(ILinkedReference)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            APlacedTrap_Registration.Instance,
        });
        dict[typeof(ILinkedReferenceGetter)] = dict[typeof(ILinkedReference)] with { Setter = false };
        dict[typeof(ISound)] = new InterfaceMappingResult(true, new ILoquiRegistration[]
        {
            SoundDescriptor_Registration.Instance,
            SoundMarker_Registration.Instance,
        });
        dict[typeof(ISoundGetter)] = dict[typeof(ISound)] with { Setter = false };
        InterfaceToObjectTypes = dict;
    }
}
