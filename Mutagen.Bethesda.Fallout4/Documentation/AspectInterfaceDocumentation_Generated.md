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
### INamed
- ActionRecord
- Class
- Component
- Faction
- HeadPart
- Keyword
- Race
### IObjectBounded
- Component
- TextureSet
## Concrete Classes to Interfaces
### ActionRecord
- INamed
### Class
- INamed
### Component
- INamed
- IObjectBounded
### Faction
- INamed
### HeadPart
- IModeled
- INamed
### Keyword
- IKeywordCommon
- INamed
### Race
- IKeyworded
- INamed
### TextureSet
- IObjectBounded
