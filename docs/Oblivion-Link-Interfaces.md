<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
## Table Of Contents

- [Link Interfaces](#link-interfaces)
  - [Interfaces to Concrete Classes](#interfaces-to-concrete-classes)
    - [IOwner](#iowner)
    - [IPlaced](#iplaced)
  - [Concrete Classes to Interfaces](#concrete-classes-to-interfaces)
    - [Faction](#faction)
    - [Landscape](#landscape)
    - [Npc](#npc)
    - [PlacedCreature](#placedcreature)
    - [PlacedNpc](#placednpc)
    - [PlacedObject](#placedobject)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

# Link Interfaces
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
