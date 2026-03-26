# Link Interfaces
Link Interfaces are used by FormLinks to point to several record types at once.  For example, a Container record might be able to contain Armors, Weapons, Ingredients, etc.

An interface would be defined such as 'IItem', which all Armor, Weapon, Ingredients would all implement.

A `FormLink<IItem>` could then point to all those record types by pointing to the interface instead.
## Interfaces to Concrete Classes
### IBindableEquipment
- Weapon
### IComplexLocation
- Cell
- Worldspace
### IConstructibleObjectTarget
- AObjectModification
- Weapon
### IExplodeSpawn
- Weapon
### IFurnitureAssociation
- Weapon
### IHarvestTarget
- Weapon
### IItem
- Weapon
### IKeywordLinkedReference
- PlacedNpc
### IObjectId
- AObjectModification
- Weapon
### IPlaceableObject
- AObjectModification
- Debris
- Weapon
### IPlaced
- PlacedNpc
- PlacedObject
### IPlacedSimple
- PlacedNpc
- PlacedObject
### IPlacedThing
- PlacedObject
### IPreCutMapEntryReference
- Landscape
### IReferenceableObject
- AObjectModification
- Debris
- Weapon
### IStaticTarget
- Weapon
## Concrete Classes to Interfaces
### AObjectModification
- IConstructibleObjectTarget
- IObjectId
- IPlaceableObject
- IReferenceableObject
### Cell
- IComplexLocation
### Debris
- IPlaceableObject
- IReferenceableObject
### Landscape
- IPreCutMapEntryReference
### PlacedNpc
- IKeywordLinkedReference
- IPlaced
- IPlacedSimple
### PlacedObject
- IPlaced
- IPlacedSimple
- IPlacedThing
### Weapon
- IBindableEquipment
- IConstructibleObjectTarget
- IExplodeSpawn
- IFurnitureAssociation
- IHarvestTarget
- IItem
- IObjectId
- IPlaceableObject
- IReferenceableObject
- IStaticTarget
### Worldspace
- IComplexLocation
