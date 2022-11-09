Records commonly have "pointers" to other records via [FormLink](ModKey%2C-FormKey%2C-FormLink#formlinks) members, which are an enhancement wrapper around a FormID.  To look up the record being referenced requires two things:
- **A relative context to look up against**.  Is the lookup relative to a single `Mod`, or a `LoadOrder`?
- **Work to be done** to iterate over all records in a `Mod` or `LoadOrder`, so that it can be determined if the record is present.

In Mutagen, these concepts are contained in an object called `LinkCache`.  These objects help give the user direct control over:
- What to link against
- How long to persist any cached information about lookups

## Creating a LinkCache
```cs
// This LinkCache will look inside the Mod for any records
ILinkCache<OblivionMod> modLinkCache = mod.CreateLinkCache();

// This LinkCache will look inside the LoadOrder for any records
ILinkCache<OblivionMod> loadOrderLinkCache = loadOrder.CreateLinkCache();
```

An important aspect of LinkCaches is the context they are attached to:
- **From a Mod:** It will look up records as they exist in that mod.  If a queried record does not explicitly exist in that mod in some form, it will not be located.
- **From a Load Order** It will look up records based on Load Order priority.  A queried record will return the version from the latest mod that contains it.  This is the version that will be loaded and seen in-game.

## Querying a LinkCache
Here's an example of looking up an NPC's Race.
```cs
ILinkCache<OblivionMod> linkCache = ...;
NPC npc = ...;
if (npc.Race.TryResolve(linkCache, out var race))
{
   System.Console.WriteLine($"{npc.EditorID} was of race {race.EditorID}");
}
else
{
   System.Console.WriteLine($"{npc.EditorID} had no race, or it wasn't found");
}
```

### Lookup Optimizations
#### Lazy Lookup and Caching
A `LinkCache` does no work until a record lookup is requested, at which point it will attempt to do the least amount of work possible to locate the record in the context it was created from.  LinkCache remembers any work done so if a second similar record is queried it will return immediately.

#### Short Circuiting on Type
One aspect of this that might not be immediately apparent is that a LinkCache only queries the appropriate Group.  A linking algorithm that is just based on FormIDs has to iterate over every single record, as that FormID could theoretically point to any record.  However, since [FormLink](ModKey%2C-FormKey%2C-FormLink#formlinks)s are associated with a record type as well, the LinkCache is able to short circuit and only search in the appropriate group.  This avoids a lot of unnecessary work.

#### Short Circuiting on Depth
`LinkCache`s relative to `LoadOrder`s also have shortcircuiting on type, but additionally also short circuit to only search as "deep" as needed.  For example, if a `LoadOrder` has 27 mods and a lookup finds its match after only processing the top 3 mods, then any further work will be skipped.

If later a record is queried contained in a mod deeper than 3 levels, it will then do the additional work to search deeper into the LoadOrder.

### Cache Lifetime Control
`LinkCache` objects cache information about past lookups, so the information can be returned immediately for subsequent lookups.  One aspect of this is that an internal `Dictionary<FormKey, IMajorRecord>` exists, where a `LinkCache` retains references to the `MajorRecord`s it found.  This can ruin the low memory footprint of the [Binary Overlay](Binary-Overlay-Concepts) pattern.  As such, it might be important for a user to have control over when/how caches are retained.

Because a `LinkCache` is just an object the user created, they can choose how long they want the cache to persist.  If a cache is becoming too big and is a strain on memory usage, a user can choose to make a new instance or `.Clear()` the cache.  A `LinkCache` can be made once for the lifetime of the program, or they can be created and released to GC frequently (at the expense of lost lookup work).  The user is in charge of choosing the patterns that suit their needs best.

### Modification Safety
One important thing to note about `LinkCache`s is that they are not "modification safe".  This means that if a `Mod` or `LoadOrder` is modified, then existing `LinkCache` objects looking at them are no longer reliable.  

New `LinkCache`s should be constructed after any modifications that:
- Add or Remove a `MajorRecord` in a mod
- If `Mod`s are added/removed/reordered on a `LoadOrder`

Changing values inside of a `MajorRecord` (Name/Health/etc) is safe to do and will not cause `LinkCache` corruption.
