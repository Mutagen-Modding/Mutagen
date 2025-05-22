# Aspect Interfaces
Aspect Interfaces expose common aspects of records.  For example, `INamed` are implemented by all records that have a `Name`.

Functions can then be written that take in `INamed`, allowing any record that has a name to be passed in.
## Interfaces to Concrete Classes
### IHasIcons
- MenuIcon
### IModeled
- Hair
- HeadPart
### INamed
- AlternateTexture
- Class
- Eyes
- Faction
- Hair
- HeadPart
### IObjectBounded
- TextureSet
## Concrete Classes to Interfaces
### AlternateTexture
- INamed
### Class
- INamed
### Eyes
- INamed
### Faction
- INamed
### Hair
- IModeled
- INamed
### HeadPart
- IModeled
- INamed
### MenuIcon
- IHasIcons
### TextureSet
- IObjectBounded
