<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
## Table Of Contents

- [Context](#context)
- [Mutability](#mutability)
  - [Immutable Link Caches](#immutable-link-caches)
  - [Mutable Link Caches](#mutable-link-caches)
- [Memory Usage](#memory-usage)
  - [Identifier Only Caches](#identifier-only-caches)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

The LinkCache is the record lookup engine.  It powers a lot of functionality, such as:
- Looking up records by [FormKey/FormLink](https://github.com/Mutagen-Modding/Mutagen/wiki/ModKey%2C-FormKey%2C-FormLink#resolves)
- Finding the [Winning Override](Winning-Overrides) in a [Load Order](Load-Order)
- [Iterating over all versions of a record](Previous-Override-Iteration) within a [Load Order](Load-Order)

# Context
Every LinkCache is created from a context:
- A single mod
- A [Load Order](Load-Order)
- Any arbitrary enumeration of mods

```cs
// Can attach to a single mod
ISkyrimModGetter aSingleMod = ...;
var linkCacheConnectedToMod = aSingleMod.ToImmutableLinkCache();

// Or a load order of mods
ILoadOrderGetter<ISkyrimModGetter> someLoadOrder = ...;
var linkCacheConnectedToLoadOrder = someLoadOrder.ToImmutableLinkCache();

// Or any random list of mods you provide
IEnumerable<ISkyrimModGetter> anyListOfMods = ...;
var linkCacheConnectedToThoseMods = anyListOfMods.ToImmutableLinkCache();
```

Each of the above link caches will look up and return records relative to their contexts.

# Mutability
## Immutable Link Caches
A LinkCache will look up records from a given context.  Being that this is a costly operation, it is preferable to cache information so that future lookups can happen faster.  However, one of the requirements for this optimization is that the presence of records in mods cannot be modified, or the link cache will potentially return faulty information.  

Important Note:  When using Immutable Link Caches, it is safe to modify content ON records (Name of an Npc, Items in a Container, etc).  It is NOT safe to add new records to a Mod, or remove from a Mod.

If you do not plan to add/remove records from the Mods, it is always recommended to use Immutable Link Caches, as they will be much more optimized.

## Mutable Link Caches
Sometimes it is desirable to have a mod on a Link Cache that you are allowed to modify.  [Synthesis](https://github.com/Mutagen-Modding/Synthesis), for example, needs to be able to modify the outgoing Patch Mod object.

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

# Memory Usage
When using Immutable Link Caches, references to records will be kept inside the cache.  This can lead to memory growth as records are queried.

Mutable components of Link Caches do not cache records, and so will not use memory (beyond the memory used by the mutable mod itself).

Since the LinkCache is just an object caching records relative to a context, you can easily release this memory by tossing your LinkCache away for the Garbage Collector to pick up once you're done with it, or want to make a new fresh cache.  (Perhaps a Clear() call will be added in the future, too)

## Identifier Only Caches
In some situations like the [FormKey Picker](FormKey-Picker), we only care about the FormKey and EditorID of records.  Caching the entire record is a waste of memory.

```cs
var linkCache = myLoadOrder.ToImmutableLinkCache(LinkCachePreferences.OnlyIdentifiers());
```

In this mode, the Link Cache will only store the FormKey/EditorID.  As such, any call that attempts to retrieve a record will throw an exception. Only certain calls that don't retrieve a whole record will be safe to use, namely `TryResolveIdentifier` or `AllIdentifiers`.