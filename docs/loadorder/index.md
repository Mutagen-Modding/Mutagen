# Load Order
A Load Order represents a set of mods in a given order, where the mods loaded later "win" and override the records from previous mods.

## Getting a Load Order
Typically you will not construct a Load Order object yourself.  Most times, using a [Game Environment](../environment/index.md) is more desirable, and will have a Load Order for you to use.

If you want to construct a Load Order object more manually, this will be discussed later in the article.

## ModListings
A `ModListing` consists of:

- A `ModKey`
- Whether it's marked as enabled
- Whether "ghosted" and has an extra suffix causes the game to ignore it

A `ModListing` can also be generic, such as `ModListing<ISkyrimModGetter>`.  This will then also contain:

- A nullable Mod object, which is present if the Mod in question exists on disk in the Data Folder


## Interacting with LoadOrder
`LoadOrder` as a container has a lot of accessors like `TryGetValue(ModKey, out T item)` or `IndexOf(ModKey)` that make it act simultaneously like a dictionary and a list.

### Priority vs Listed Ordering
While `LoadOrder` is a "list" of `ModListing` object, two properties are exposed for when you want to enumerate to help clarify behavior:

- **ListedOrder** - Enumerates items in the order they were listed
- **PriorityOrder** - Enumerates the items so that highest priority comes first (reverse)

Enumeration is exposed like this just for the extra clarification and to reduce confusion of what ordering things are being iterated.  This is very clearly iterating the highest priority mods first, and will probably have `Skyrim.esm` or whatever base game file last:
```csharp
foreach (var mod in loadOrder.PriorityOrder)
{
   // ...
}
```

### Filtering Listings


### Accessing Specific Listings
`LoadOrder` has a lot accessors for checking if certain mods are on the list, and retrieving them.
```cs
var modKey = ModKey.FromFilePath("Skyrim.esm");
LoadOrder loadOrder = ...;

if (loadOrder.TryGetValue(modKey, out var listing)
{
   // Load order had that mod
}

// Also has typical contains and index access
if (loadOrder.ContainsKey(modKey))
{
    listing = loadOrder[modKey];
}

// There is also int index API
int index = loadOrder.IndexOf(modKey);
if (loadOrder.TryGetIndex(index, out listing)
{
   // Got the listing by index
}

// And Clear/Add
loadOrder.Clear();
loadOrder.Add(listing);
```

## Reading a Load Order
### Getting Listings
If you want to load the typical load order that the game itself will use:
```csharp
IEnumerable<IModListingGetter> loadOrder = LoadOrder.GetListings(GameRelease.SkyrimSE, pathToDataFolder);
```

This gives a simple enumerable of the `ModListing` in the order they will be loaded.

### Importing Mods
Usually you want more than just the list of `ModListings` in order; You want the associated mods as accessible objects to use.

`LoadOrder` is just a normal object that you could declare and fill yourself, but there are a few convenience methods to do this for you:

```csharp
LoadOrder<IModListing<ISkyrimModGetter>> loadOrder = LoadOrder.Import<ISkyrimModGetter>(
   dataFolder: pathToDataFolder,
   loadOrder: LoadOrder.GetListings(GameRelease.SkyrimSE, pathToDataFolder),
   gameRelease: GameRelease.SkyrimSE);
```

#### Specifying Getter vs Setter
The choice of specifying Getter or Setter interfaces (`ISkyrimMod` vs `ISkyrimModGetter`) is important, as that will drive the style that the mods are imported with.  If the Getter is specified, then the more optimized [Binary Overlay](../plugins/Binary-Overlay.md) systems will be used.  If Setter is specified, then all the import work will need to be done up front into a mutable object.

## Writing a Load Order
A `LoadOrder` can also export its contents to a file.
```cs
IEnumerable<ModListing> myListings = ...;
LoadOrder.Write(
   somePath, 
   GameRelease.SkyrimSE,
   myListings);
```

## PluginListings and CreationClubListings
The above API abstracts away the complications that a Load Order is driven from a few sources:

- Implicit Listings (Mods that don't need to be listed, but are assumed)
- Normal Plugins File (Plugins.txt)
- Installed Creation Club Mods ([GameName].ccc)

Logic related to each concept lives in its own class:

- Implicits.Listings
- PluginListings
- CreationClubListings

In each you will be able to do tasks related to that specific load order source concept.
