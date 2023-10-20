# Link Cache
The LinkCache is the record lookup engine.  It is made relative to any number of mods, and does complex lookups to find records.


It powers a lot of functionality, such as:

- Looking up records by [FormKey/FormLink](Record-Resolves.md)
- Finding the [Winning Override](../loadorder/Winning-Overrides.md) in a [Load Order](../loadorder/index.md)
- [Iterating over all versions of a record](Previous-Override-Iteration.md) within a [Load Order](../loadorder/index.md)

## Construction
Every LinkCache is created relative to some mods.  It will look up and return records relative to these mods.

=== "Single"
    ``` { .cs hl_lines=2 }
    ISkyrimModGetter aSingleMod = ...;
    var linkCacheConnectedToMod = aSingleMod.ToImmutableLinkCache();
    ```
=== "Many"
    ``` { .cs hl_lines=2 }
    IEnumerable<ISkyrimModGetter> anyListOfMods = ...;
    var linkCacheConnectedToThoseMods = anyListOfMods.ToImmutableLinkCache();
    ```
=== "Load Order"
    ``` { .cs hl_lines=2 }
    ILoadOrderGetter<ISkyrimModGetter> someLoadOrder = ...;
    var linkCacheConnectedToLoadOrder = someLoadOrder.ToImmutableLinkCache();
    ```

    [:octicons-arrow-right-24: Load Order](../loadorder/index.md)

## Mutability
It can often be a costly operation to look up records.   It is preferable to cache information so that future lookups can happen faster.   Depending on whether you want to mutate the mods a Link Cache looks to, you will need to choose the correct type of Link Cache with the correct caching patterns.

### Immutable Link Caches
If you do not plan to add/remove records from the Mods, it is always recommended to use Immutable Link Caches, as they will be much more optimized.  

!!! bug "No Adds or Removes"
    It is not safe to add or remove records from backing mods of Immutable Link Caches.  This can corrupt a cache.

!!! tip "Record Content Modification is Safe"
    It is safe to modify content ON records (Name of an Npc, Items in a Container, etc)


### Mutable Link Caches
Sometimes it is desirable to have a mod on a Link Cache that you are allowed to modify.  A common use case is having an outgoing mod file that will eventually be exported.  This mod would both like to be able to be modified while also being able to resolve its content from the Link Cache.

In these scenarios, we can create a Mutable Link Cache.  This is a combination of an Immutable Link Cache for most of the mods in a load order, PLUS a mutable component for the final mods at the end that we want to modify.  As such there are a few things to consider:

- Most of the load order still gets the speed optimizations of being immutable
- We only pay the speed price when dealing with the one (few) mutable mods at the end.
- Because of this structure, the mutable mods MUST be at the end.

```cs
// Immutable, readonly load order
ILoadOrderGetter<ISkyrimModGetter> readOnlyLoadOrder = ...;

// A mutable mod we want to be able to change
SkyrimMod mod = new SkyrimMod(ModKey.FromFileName("MyMod.esp"), SkyrimRelease.SkyrimSE);

var linkCache = readOnlyLoadOrder.ToMutableLinkCache(mutableMods: mod);

// No problems here
var npc = mod.Npcs.AddNew();
```

The result will be a mostly immutable Link Cache, with a mutable component at the end for `mod`.  It it safe to add/remove records from `mod`, as the Link Cache will react and continue to return accurate results even after the changes.

!!! warning "Slower"
    Mutable link caches are slower, so always use Immutable variants when possible

## Resolve Target
|  Target | Direction  |  Intention |
| ----- | ----- | ----- |
| Winning | Later mods first | Locates the version of a record that the game will utilize |
| Origin | Earlier mods first | Locates the initial version of the record as it was originally defined |

By default all interactions with a Link Cache have the Resolve Target of "Winning".

## Memory Usage
When using Immutable Link Caches, references to records will be kept inside the cache.  This can lead to memory growth as records are queried.

Mutable components of Link Caches do not cache records, and so will not use memory (beyond the memory used by the mutable mod itself).

Since the LinkCache is just an object caching records relative to a context, you can easily release this memory by tossing your LinkCache away for the Garbage Collector to pick up once you're done with it, or want to make a new fresh cache.  (Perhaps a Clear() call will be added in the future, too)

### Identifier Only Caches
In some situations like the [FormKey Picker](../wpf/FormKey-Picker.md), we only care about the FormKey and EditorID of records.  Caching the entire record is a waste of memory.

```cs
var linkCache = myLoadOrder.ToImmutableLinkCache(LinkCachePreferences.OnlyIdentifiers());
```

In this mode, the Link Cache will only store the FormKey/EditorID.  As such, any call that attempts to retrieve a record will throw an exception. Only certain calls that don't retrieve a whole record will be safe to use, namely `TryResolveIdentifier` or `AllIdentifiers`.
