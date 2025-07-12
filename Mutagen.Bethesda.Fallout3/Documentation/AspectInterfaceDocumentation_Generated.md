# Aspect Interfaces
Aspect Interfaces expose common aspects of records.  For example, `INamed` are implemented by all records that have a `Name`.

Functions can then be written that take in `INamed`, allowing any record that has a name to be passed in.
## Interfaces to Concrete Classes
### IHasIcons
- BodyPartItem
- HeadPartItem
- MenuIcon
### IModeled
- BodyData
- BodyPartItem
- Hair
- HeadPart
- HeadPartItem
### INamed
- AlternateTexture
- Class
- Eyes
- Faction
- Hair
- HeadPart
- Race
### IObjectBounded
- Sound
- TextureSet
## Concrete Classes to Interfaces
### AlternateTexture
- INamed
### BodyData
- IModeled
### BodyPartItem
- IHasIcons
- IModeled
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
### HeadPartItem
- IHasIcons
- IModeled
### MenuIcon
- IHasIcons
### Race
- INamed
### Sound
- IObjectBounded
### TextureSet
- IObjectBounded
