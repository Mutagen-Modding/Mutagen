---
order: 275
---
# Binary Format Complexity Abstraction
Bethesda's binary format contains a lot implementation complexities that are unrelated to the actual content of the records.  A lot of times the exposure of these details are a source of confusion, and don't add much upside in the way of flexibility or power into the hands of the user.  Mutagen attempts to abstract these complexities away so that the end user is dealing with the distilled record content more directly, rather than wading through the gritty specifics that only matter in the context of their binary format on disk.

## FormKeys and FormLinks
This topic was covered in detail in the [ModKey, FormKey, FormLink](plugins/ModKey,-FormKey,-FormLink.md#formkeys) section, and so will not be covered here.

## Record Types
Most seasoned modders are familiar with the 4 character record headers. `EDID` is `Editor ID`.  `FULL` is `Name`.  `MODL` is `Model`.  

These concepts are not exposed in Mutagen's generated classes.   Members are exposed their readable name: `npc.EditorID`, `npc.Name`, and `npc.Model`

### Type Safety
By not using a string based lookup, there is no possibility of typos or misqueries, no need to remember the more obscure header strings, and no possibility of type mishandling.  Because everything is exposed via normal members, any typo or attempt to access a non-existent member will be caught by the compiler, instead of a failed string query at runtime.

### Alternate Headers
Some common Record Types have alternate versions to denote different fields.  A common example of this is the `Model` Record Type `MODL`, which on Armor WorldModel records is `MOD2` for males, and `MOD4`.   Mutagen deals with these alternative record types under the hood, exposing the various fields as it would normally.

## List Mechanics
### Item Storage
There are a lot of varying ways that lists are stored in the binary format:

- Repeated subrecords, with their Record Header as the delimiter.  Unknown amount.
  _(SPLO records in Oblivion)_

- Extra prepended subrecord /w the count of items in the list.  This can then be followed by repeated subrecords with Record Headers, or headerless raw data of known lengths.
  _(Keyword lists in Skyrim)_

- A Record Header for the list itself, followed immediately by a uint count, followed by undelimited content of known length. _(Skyrim Model's Alternate Textures)_
- Same as above, but with an extra set of bytes to denote the contained structure length

No matter what pattern is used for a specific set of records, they are all exposed via straight up lists in the API:
```cs
// Oblivion spells
IList<FormLink<Spell>> Spells { get; set; }

// A keyword list
IList<FormLink<Keyword>> Keywords { get; set; }

// A model's alternate textures
IList<AlternateTexture> AlternateTextures { get; set; }
```

### Count Subrecords
Some lists have a preceding extra record that contains the count of a given list.  For example `KSIZ` is the counter type for Keywords, which has a number for how many `KWDA` records are expected to follow containing keyword formIDs.   Mutagen handles these counter records internally, so you can just modify any list to contain the desired content, and the correct count will be written automatically.

## Markers
Some subrecords have specialized "marker" subrecords that precede them.  Sometimes they just mark the location of a section of subrecords, while other times they affect the context of the following subrecords.

A good specific example can be found in Skyrim's `Race`'s Body Data subrecords.  A `Race` has a body data struct for males, and a body data struct for females.  These records are first "marked" by an empty `NAM0` record.  They are then further "marked" by empty `MNAM` or `FNAM` subrecords, to indicate if the following data is for males or females.  The binary might follow this pattern:
```
NAM0: No content, just a marker
MNAM: No content, just a marker
INDX: Body Data content for males
MODL: Body Data content for males
FNAM: No content, just a marker
INDX: Body Data content for females
MODL: Body Data content for females
```

Mutagen abstracts the concepts of markers away from the user completely.  No concepts of markers exist in the API exposed.  A user can just directly access the data they're interested in, and if a marker-related field is set for exporting, the marker systems will be handled automatically during export internally.

## PathGrid Point Zipping
Path grid information in records such as Oblivion's `Road` are stored in two separate subrecord lists that need to be considered together in order to get the complete structure.  The points themselves are stored in the list `PGRP`, while the information about the connections between points is stored in `PGRR`.  If you want to know what points have what connections, you need to offset your queries into the `PGRR` subrecord properly.  Each Point exposes the number of connections it has.  Then, to find the connections for Point #7, you have to offset your query into `PGRR` by 12 * the number of connections Points #1-6 had.  You then have to extract the number of connection floats appropriate for Point #7, based on the number of connections it says it has.

Mutagen abstracts this "zipping" work for you and offers a simple `IList<RoadPoint> Points;` member on `Road`.  Each `RoadPoint` has a `IList<P3Float> Connections;` with all the connections related to that point.  All the complexity of the dual subrecords is hidden from the user, and they can just manipulate these lists directly in a more straightforward and familiar fashion.

## Multiple Flag Consolidation
Some records have so many flags that they overflow above the typical 4 bytes.  Bethesda will then put a second set of flags elsewhere in the record with the overflow flag values.  One example of this happening is in Skyrim's Race record.  Mutagen merges the overflow into one Flag field exposing all of the values in one place.

## GenderedItem
Bethesda records have a lot of fields that come in a Male/Female pair.  While not too much of a problem by itself, most of the complexity comes in the variety of ways that these pairs can be organized.  Sometimes the M/F data is held in dedicated `MNAM`/`FNAM` records.  Sometimes those records are prepended by a marker, as mentioned above.  Sometimes instead both are found in a single subrecord, with the data just back to back.

Mutagen standardizes all the various cases, and exposes the male/female alternatives using a `GenderedItem` struct in the public API:
```
public class GenderedItem<T>
{
    public T Male { get; set; }
    public T Female { get; set; }
}

...

// Using Oblivion's Race's Height as an example
race.Height.Male = 1.5f;
float femaleHeight = race.Height.Female;
```

## Pseudo Enum Dictionaries
Skyrim's Race record has a hacky binary implementation for its `Biped Object Names` field.

This field is supposed to expose the names associated with the different values of this enum:
```
[Flags]
public enum BipedObjectFlag : uint
{
    Head = 0x0000_0001,
    Hair = 0x0000_0002,
    Body = 0x0000_0004,
    ...
    Ears = 0x0000_2000,
    DecapitateHead = 0x0010_0000,
    Decapitate = 0x0020_0000,
    FX01 = 0x8000_0000,
}
```

With this field, a user can specify different string names for the Head slot, the Hair slot, etc.  Oddly, though, this is all stored as a list, where the 3rd item would be associated with the `Body`, for example.  Additionally, to get out to the later enum values, empty list items must be inserted for each unknown flag index.

```
// Example binary structure
NAME - "Top Thing"  // Head index
NAME - "Inedible Spaghetti" // Hair index
NAME - "Gut Housing" // Body index
...
NAME - "Future Wings" // Ear index
NAME - empty // Empty index for 0x0000_4000
NAME - empty // Empty index for 0x0000_8000
...
NAME - empty // Empty index for 0x0008_0000
NAME - "Decapitate Head" // Decapitate Head index
```

Notice that the user needs to coordinate/know that the third index is the body index.  They also need to make sure to insert/count the proper empty indices in order to "reach" the later flags.  If any item is inserted/removed, all the surrounding items are now misaligned, etc.

Mutagen wraps these List concepts internally, and exposes `Dictionary<BipedObjectFlag, string>` in its public API instead.  The user can get/set the dictionary in a more natural way, and the error prone list implementation will be handled for them under the hood.
