# Link Interfaces
Link Interfaces are used by FormLinks to point to several record types at once.  For example, a Container record might be able to contain Armors, Weapons, Ingredients, etc.

An interface would be defined such as 'IItem', which all Armor, Weapon, Ingredients would all implement.

A `FormLink<IItem>` could then point to all those record types by pointing to the interface instead.
## Interfaces to Concrete Classes
### IDamageTypeTarget
- ActorValueInformation
- ASpell
### IIdleRelation
- ActionRecord
### IKeywordLinkedReference
- Keyword
### ILocationRecord
- LocationReferenceType
### ILocationTargetable
- Door
### IObjectId
- Door
- Faction
- TextureSet
### IOwner
- Faction
### IRelatable
- Faction
- Race
## Concrete Classes to Interfaces
### ActionRecord
- IIdleRelation
### ActorValueInformation
- IDamageTypeTarget
### ASpell
- IDamageTypeTarget
### Door
- ILocationTargetable
- IObjectId
### Faction
- IObjectId
- IOwner
- IRelatable
### Keyword
- IKeywordLinkedReference
### LocationReferenceType
- ILocationRecord
### Race
- IRelatable
### TextureSet
- IObjectId
