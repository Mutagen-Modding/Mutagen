# Link Interfaces
Link Interfaces are used by FormLinks to point to several record types at once.  For example, a Container record might be able to contain Armors, Weapons, Ingredients, etc.

An interface would be defined such as 'IItem', which all Armor, Weapon, Ingredients would all implement.

A `FormLink<IItem>` could then point to all those record types by pointing to the interface instead.
## Interfaces to Concrete Classes
### IAliasVoiceType
- Faction
### IExplodeSpawn
- TextureSet
### IIdleRelation
- ActionRecord
### IKeywordLinkedReference
- Keyword
### ILocationRecord
- LocationReferenceType
### IObjectId
- Faction
- TextureSet
### IOwner
- Faction
### IPlaceableObject
- TextureSet
### IPlaced
- PlacedObject
### IPlacedSimple
- PlacedObject
### IPlacedThing
- PlacedObject
### IReferenceableObject
- TextureSet
### IRelatable
- Faction
- Race
### ISpellRecord
- Spell
### IVoiceTypeOrList
- FormList
- VoiceType
## Concrete Classes to Interfaces
### ActionRecord
- IIdleRelation
### Faction
- IAliasVoiceType
- IObjectId
- IOwner
- IRelatable
### FormList
- IVoiceTypeOrList
### Keyword
- IKeywordLinkedReference
### LocationReferenceType
- ILocationRecord
### PlacedObject
- IPlaced
- IPlacedSimple
- IPlacedThing
### Race
- IRelatable
### Spell
- ISpellRecord
### TextureSet
- IExplodeSpawn
- IObjectId
- IPlaceableObject
- IReferenceableObject
### VoiceType
- IVoiceTypeOrList
