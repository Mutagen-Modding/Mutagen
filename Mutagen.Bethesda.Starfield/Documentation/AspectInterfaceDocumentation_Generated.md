# Aspect Interfaces
Aspect Interfaces expose common aspects of records.  For example, `INamed` are implemented by all records that have a `Name`.

Functions can then be written that take in `INamed`, allowing any record that has a name to be passed in.
## Interfaces to Concrete Classes
### IKeywordCommon
- Keyword
### IModeled
- HeadPart
- Weapon
### INamed
- ActionRecord
- Class
- DamageType
- Faction
- HeadPart
- Keyword
### IObjectBounded
- TextureSet
### IPositionRotation
- Transform
## Concrete Classes to Interfaces
### ActionRecord
- INamed
### Class
- INamed
### DamageType
- INamed
### Faction
- INamed
### HeadPart
- IModeled
- INamed
### Keyword
- IKeywordCommon
- INamed
### TextureSet
- IObjectBounded
### Transform
- IPositionRotation
### Weapon
- IModeled
