# Load Order
A Load Order represents an ordered list of items with ModKeys, where the items later in the list "win" and override the records from previous items.  It also acts as a dictionary style object, able to look up entries by ModKey.

## Entries
The `LoadOrder<T>` class is generic, and can contain many types of objects.  These are often objects that expose the information you would expect from a Bethesda load order.

!!! tip "Use Your Own"
    You can use `LoadOrder<T>` containers to hold other types of objects, as long as they implement `IModKeyed` and so have a `ModKey`

### Load Order Listing
These are entries that only have information that could be provided by a typical Bethesda load order file.

| Field | Type | Description |
| ----- | ----- | ----- |
| ModKey | ModKey | The ModKey associated with the entry |
| Enabled | bool | Whether the entry is to be considered enabled in the game |
| Ghosted | bool | Whether the entry is "ghosted" and has an extra suffix causes the game to ignore it |
| FileName | string | File name of the entry, with ghost suffix included, if applicable |

[:octicons-arrow-right-24: ModKey](../plugins/ModKey,%20FormKey,%20FormLink.md#modkey)

### Mod Listing
These objects expand upon Load Order Listings by providing disk information related to the mod file itself as it exists on disk.

Additional Fields:

| Field | Type | Description |
| ----- | ----- | ----- |
| ExistsOnDisk | bool | Whether the mod exists on disk |
| Mod | TMod (generic) | Mutagen mod object (null if missing from disk) |

These fields depend on the context in which the LoadOrder object was constructed, such as what Data folder it was relative to.

## Interacting with LoadOrder
`LoadOrder` as a container that is acts simultaneously like a dictionary and a list.  You will find the similar accessors of both in its API.

### Priority vs Listed Ordering
While `LoadOrder` is a "list" object, two properties are exposed for when you want to enumerate to help clarify behavior:

- **ListedOrder** - Enumerates items in the order they were listed
- **PriorityOrder** - Enumerates the items so that highest "winning" priority comes first

??? example "Example"
    ```csharp
    foreach (var mod in loadOrder.PriorityOrder)
    {
       // ...
    }
    ```

    Enumeration is exposed like this just for the extra clarification and to reduce confusion of what ordering things are being iterated.  This is more clearly iterating the highest priority mods first, and will probably have `Skyrim.esm` or whatever base game file last.

### Accessing Specific Listings
`LoadOrder` has a lot accessors for checking if certain mods are on the list, and retrieving them.
```cs
ModKey modKey = ...;
ILoadOrder<IModListing<ISkyrimModGetter>> loadOrder = ...;

if (loadOrder.TryGetValue(modKey, out var listing))
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
var obj = loadOrder.TryGetAtIndex(index);
if (obj != null)
{
   // Got the listing by index
}

// And Clear/Add
loadOrder.Clear();
loadOrder.Add(listing);
```

## Construction
### Via Environment
Typically you will not construct a Load Order object yourself.  Using a [Game Environment](../environment/index.md) will have a Load Order for you to use based on the game installation you point to.

[:octicons-arrow-right-24: Environments](../environment/index.md)

### Create and Fill
`LoadOrder` is just a normal object that you could declare and fill yourself.

``` { .cs hl_lines=3 }
TMyObject someObj = ...;

var loadOrder = new LoadOrder<TMyObject>();
loadOrder.Add(someObj);

```

### Import
If you don't want a whole environment, and just want some convenience for importing a Load Order, this exists as well.

=== "Simple"
    ```cs
    var loadOrder = LoadOrder.Import<ISkyrimModGetter>(GameRelease.SkyrimSE);
    ```
=== "Data Folder Overridden"
    ```cs
    var loadOrder = LoadOrder.Import<ISkyrimModGetter>(dataFolderPath, GameRelease.SkyrimSE);
    ```
=== "Explicit Listings"
    ```cs
    var listings = new List<LoadOrderListing>()
    {
       new LoadOrderListing(ModKey.FromFileName("Skyrim.esm"), enabled: true)
    };

    var loadOrder = LoadOrder.Import<ISkyrimModGetter>(listings, GameRelease.SkyrimSE);
    ```
!!! tip "Prefer Getter Generic"
    The choice of specifying Getter or Setter interfaces (`ISkyrimMod` vs `ISkyrimModGetter`) is important, as that will drive the style that the mods are imported with.  If the Getter is specified, then the more optimized [Binary Overlay](../plugins/Binary-Overlay.md) systems will be used.  If Setter is specified, then all the import work will need to be done up front into a mutable object.

## Writing a Load Order
A `LoadOrder` can also export its contents to a file.
```cs
ILoadOrderGetter<IModListingGetter> loadOrder = ...;

LoadOrder.Write(
   somePath, 
   GameRelease.SkyrimSE,
   loadOrder,
   removeImplicitMods: true);
```

## PluginListings and CreationClubListings
The Load Order API often abstracts away the complications that a Load Order is driven from a few sources:

- Implicit Listings (Mods that don't need to be listed, but are assumed)
- Normal Plugins File (Plugins.txt)
- Installed Creation Club Mods ([GameName].ccc)

Logic related to each concept lives in its own class:

- Implicits.Listings
- PluginListings
- CreationClubListings

In each you will be able to do tasks related to that specific load order source concept.
