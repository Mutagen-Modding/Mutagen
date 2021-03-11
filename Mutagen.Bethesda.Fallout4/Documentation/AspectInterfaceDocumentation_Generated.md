# Aspect Interfaces
Aspect Interfaces expose common aspects of records.  For example, `INamed` are implemented by all records that have a `Name`.

Functions can then be written that take in `INamed`, allowing any record that has a name to be passed in.
## Interfaces to Concrete Classes
### INamed
- ActionRecord
- Class
- Component
- Faction
- Keyword
### IObjectBounded
- Component
- TextureSet
### Keyword
- Keyword
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
### Keyword
- INamed
- Keyword
### TextureSet
- IObjectBounded
