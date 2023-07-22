# Game Constants
As Bethesda games are released, headers are modified slightly.  They still have a lot in common, but certain things move or the total length changes, or something else that will misalign any common parsing code.

`GameConstants` is a class containing all the various alignment information specific to a game.  Things like:
- ModHeaderLength
- HeaderIncludedInLength
- LengthLength (amount of bytes the 'length' section is)
- Etc

`GameConstants` also has static instances of itself for each Bethesda game, so that users can look up the specific header details for a specific game.  There is also a `GameCategory` enum that can be used to query to get the constants:
```cs
var oblivionHeaderData = GameConstants.Oblivion;
// An alternate way
oblivionHeaderData = GameConstants.Get(GameCategory.Oblivion);
var headerLength = oblivionHeaderData.MajorConstants.HeaderLength;
```

The usefulness of this "registry" is that code can be written once, while referring to a header meta object to account for differences in the games.  Give the code a different meta object to refer to for the appropriate game, and all the alignment works out.