# Aspect Interfaces
Aspect Interfaces expose common aspects of records.  For example, `INamed` are implemented by all records that have a `Name`.

Functions can then be written that take in `INamed`, allowing any record that has a name to be passed in.
## Interfaces to Concrete Classes
### IKeywordCommon
- Keyword
### IKeyworded
- Race
### IModeled
- Grass
- HeadPart
### INamed
- ActionRecord
- Class
- Component
- Faction
- HeadPart
- Keyword
- MaterialType
- Race
### IObjectBounded
- AcousticSpace
- Component
- Grass
- SoundMarker
- TextureSet
## Concrete Classes to Interfaces
### AcousticSpace
- IObjectBounded
### ActionRecord
- INamed
### Class
- INamed
### Component
- INamed
- IObjectBounded
### Faction
- INamed
### Grass
- IModeled
- IObjectBounded
### HeadPart
- IModeled
- INamed
### Keyword
- IKeywordCommon
- INamed
### MaterialType
- INamed
### Race
- IKeyworded
- INamed
### SoundMarker
- IObjectBounded
### TextureSet
- IObjectBounded
