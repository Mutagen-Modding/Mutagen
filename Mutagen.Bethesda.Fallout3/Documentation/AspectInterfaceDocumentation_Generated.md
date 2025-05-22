# Aspect Interfaces
Aspect Interfaces expose common aspects of records.  For example, `INamed` are implemented by all records that have a `Name`.

Functions can then be written that take in `INamed`, allowing any record that has a name to be passed in.
## Interfaces to Concrete Classes
### IHasIcons
- MenuIcon
### INamed
- Class
### IObjectBounded
- TextureSet
## Concrete Classes to Interfaces
### Class
- INamed
### MenuIcon
- IHasIcons
### TextureSet
- IObjectBounded
