# Aspect Interfaces
Aspect Interfaces expose common aspects of records.  For example, `INamed` are implemented by all records that have a `Name`.

Functions can then be written that take in `INamed`, allowing any record that has a name to be passed in.
## Interfaces to Concrete Classes
### IKeywordCommon
- Keyword
### IKeyworded
- Race
### IModeled
- HeadPart
- Weapon
### INamed
- BipedObjectData
- Class
- DamageType
- FaceMorphItem
- Faction
- HeadPart
- Keyword
- MorphGroup
- Race
### IObjectBounded
- AOPFRecord
- TextureSet
### IPositionRotation
- Transform
## Concrete Classes to Interfaces
### AOPFRecord
- IObjectBounded
### BipedObjectData
- INamed
### Class
- INamed
### DamageType
- INamed
### FaceMorphItem
- INamed
### Faction
- INamed
### HeadPart
- IModeled
- INamed
### Keyword
- IKeywordCommon
- INamed
### MorphGroup
- INamed
### Race
- IKeyworded
- INamed
### TextureSet
- IObjectBounded
### Transform
- IPositionRotation
### Weapon
- IModeled
