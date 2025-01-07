## Importing
All Mods are generated with the ability to create themselves from their binary plugin format (esp/esm/esl).

## Read Only
The Binary Overlay imports mods in a readonly, on-demand fashion.  Only fields that are accessed get parsed, which saves a lot of time and work.

```cs
using var mod = OblivionMod.CreateFromBinaryOverlay(pathToMod);
// Code accessing the mod
```

!!! tip "Preferred"
    Use Binary Overlays when possible, up until you want to mutate records.
	
	[:octicons-arrow-right-24: Mutation Patterns](../best-practices/Read-Only.md)

### No Up Front Work
With the overlay pattern, a mod object is returned almost immediately for use after having done almost no parsing.  As the user accesses members of the `Mod`, the parsing is done on demand for only the members accessed.

### No Persistent References to Records or Memory
Binary Overlays keep no internal references to any record objects.  This means that when a user is done working with something and discards it, the GC is allowed to immediately clean it up.

### Requires an Open Stream
Binary Overlays keep reference to an open stream internally, so they can read when accessed.

!!! info "Disposable"
    Binary Overlay objects implement `IDisposable`.  Putting them in `using` statements to close when appropriate is good practice.

### Best Practices

!!! warning "Avoid Repeated Access"
    The Overlay pattern has the downside that repeated access of properties means repeated parsing of the same data.  

[:octicons-arrow-right-24: Overlay Best Practices](../best-practices/Overlays-Single-Access.md)

## Mutable
A normal C# object can be created containing all of a mod's data.  This is generally be reserved for when constructing or modifying records for output.

```cs
string path = "..../Oblivion.esm";
var mod = OblivionMod.CreateFromBinary(path);
```
At this point, the entire mod will have been loaded into memory via classes created to store all the various fields and records.   You can then begin to interact with the mod object.

!!! warning "Heavier Load"
    This route spends more time and memory loading in everything up front, even the data you will not be interacting with.
	
	If you don't need to modify the mod, consider instead:
	
	[:octicons-arrow-right-24: Read Only Mod Importing](#read-only-mod-importing)
	

### Group Masks
Often, users are not interested in all records that a mod contains.  Group Masks are an optional parameter that allows you to specify which record types you are interested in importing.
```cs
string path = "..../Oblivion.esm";
var mod = OblivionMod.CreateFromBinary(
    path,
    importMask: new GroupMask()
    {
        Potions = true,
        NPCs = true,
    }); 
```
This import call will only process and import Potions and NPCs.

!!! info "Generally Unused"
    Generally this feature is unused, as [Binary Overlays](Importing-and-Construction.md#read-only-mod-importing) handle the situation of selectively accessing data much better.
	
## Flexible Game Target
In more complex setups, often the game type is not known at compile time.

=== "Untyped"
    ```cs
    using var readOnlyInputMod = ModInstantiator.ImportGetter(pathToMod, release);
    var mutableMod = ModInstantiator.ImportSetter(pathToMod, release);
    ```

=== "Generic"
    ```cs
    var mod = ModInstantiator<TMod>.Importer(ModKey.FromFileName("MyMod.esp"), release);
    ```
	
    !!! success "Dispose Appropriately"
         Binary overlays are disposable, as they can keep streams open as they are accessed.  Make sure to utilize `using` statements to dispose of them appropriately.


## Construction
How to make a new mod object from scratch without a file

### Known Game
If you know the game you will be working with at compile time, this is the typical way to create a new mod:

``` cs
var newMod = new SkyrimMod(ModKey.FromFileName("MyMod.esp"), SkyrimRelease.SkyrimSE);
```

### Unknown Game
In more complex setups, often the game type is not known at compile time 

=== "Untyped"
    ```cs
    var newMod = ModInstantiator.Activator(ModKey.FromFileName("MyMod.esp"), release);
    ```

=== "Generic"
    ```cs
    var newMod = ModInstantiator<TMod>.Activator(ModKey.FromFileName("MyMod.esp"), release);
    ```
