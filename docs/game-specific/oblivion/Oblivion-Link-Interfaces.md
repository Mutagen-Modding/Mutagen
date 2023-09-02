# Oblivion Link Interfaces
## Link Interfaces
Link Interfaces are used by FormLinks to point to several record types at once.  For example, a Container record might be able to contain Armors, Weapons, Ingredients, etc.

An interface would be defined such as 'IItem', which all Armor, Weapon, Ingredients would all implement.

A `FormLink<IItem>` could then point to all those record types by pointing to the interface instead.
## Interfaces to Concrete Classes
### IOwner

- Faction
- Npc
### IPlaced

- Landscape
- PlacedCreature
- PlacedNpc
- PlacedObject
## Concrete Classes to Interfaces
### Faction

- IOwner
### Landscape

- IPlaced
### Npc

- IOwner
### PlacedCreature

- IPlaced
### PlacedNpc

- IPlaced
### PlacedObject

- IPlaced
