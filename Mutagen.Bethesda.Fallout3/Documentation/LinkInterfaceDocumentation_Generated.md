# Link Interfaces
Link Interfaces are used by FormLinks to point to several record types at once.  For example, a Container record might be able to contain Armors, Weapons, Ingredients, etc.

An interface would be defined such as 'IItem', which all Armor, Weapon, Ingredients would all implement.

A `FormLink<IItem>` could then point to all those record types by pointing to the interface instead.
## Interfaces to Concrete Classes
### IBoundItem
- Armor
- Weapon
### IExplodeSpawn
- AcousticSpace
### IPlaceableObject
- AcousticSpace
### IPlaced
- PlacedObject
### IReferenceableObject
- AcousticSpace
### IRelatable
- Faction
- Race
## Concrete Classes to Interfaces
### AcousticSpace
- IExplodeSpawn
- IPlaceableObject
- IReferenceableObject
### Armor
- IBoundItem
### Faction
- IRelatable
### PlacedObject
- IPlaced
### Race
- IRelatable
### Weapon
- IBoundItem
