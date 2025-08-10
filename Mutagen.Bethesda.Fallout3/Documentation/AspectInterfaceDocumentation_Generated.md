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
- MagicEffect
### INamed
- AlternateTexture
- Class
- Eyes
- Faction
- Hair
- HeadPart
- MagicEffect
- Race
### IObjectBounded
- AcousticSpace
- Sound
- TextureSet
## Concrete Classes to Interfaces
### AcousticSpace
- IObjectBounded
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
### MagicEffect
- IModeled
- INamed
### MenuIcon
- IHasIcons
### Race
- INamed
### Sound
- IObjectBounded
### TextureSet
- IObjectBounded
