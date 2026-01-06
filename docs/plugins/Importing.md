# Importing
All Mods are generated with the ability to create themselves from their binary plugin format (esp/esm/esl).

## Read Only
By default, mods are readonly.  This comes with a lot of speed and memory usage upsides.  Only fields that are accessed get parsed, which saves a lot of time and work.

```cs
using var mod = OblivionMod.Create(OblivionRelease.Oblivion)
    .FromPath(pathToMod)
    .Construct();
// Code accessing the mod
```

!!! tip "Preferred"
    Use readonly when possible, up until you want to mutate records.

	[:octicons-arrow-right-24: Mutation Patterns](Create,-Duplicate,-and-Override.md#overriding-records)

### No Up Front Work
With readonly objects, a mod object is returned almost immediately for use after having done almost no parsing.  As the user accesses members of the `Mod`, the parsing is done on demand for only the members accessed.

### No Persistent References to Records or Memory
Readonly systems keep no internal references to any record objects.  This means that when a user is done working with something and discards it, the GC is allowed to immediately clean it up.

### Requires an Open Stream
Readonly objects keep reference to an open stream internally, so they can read when accessed.

!!! info "Disposable"
    Binary Overlay objects implement `IDisposable`.  Putting them in `using` statements to close when appropriate is good practice.

### Best Practices

!!! warning "Avoid Repeated Access"
    The Overlay pattern has the downside that repeated access of properties means repeated parsing of the same data.

[:octicons-arrow-right-24: Overlay Best Practices](../best-practices/Overlays-Single-Access.md)

## Mutable
A normal C# object can be created containing all of a mod's data.  This is generally be reserved for when constructing or modifying records for output.

```cs
using var mod = OblivionMod.Create(OblivionRelease.Oblivion)
    .FromPath(pathToMod)
    .Mutable()
    .Construct();
```

The only difference is we add `Mutable()`, which returns a mutable variant of the mod object, which is fulfilled in a completely different way. The entire mod will have been loaded into memory via classes created to store all the various fields and records.   You can then begin to interact with the mod object.

!!! warning "Heavier Load"
    This route spends more time and memory loading in everything up front, even the data you will not be interacting with.

	If you don't need to modify the mod, consider instead:

	[:octicons-arrow-right-24: Read Only Mod Importing](#read-only)


### Group Masks
Often, users are not interested in all records that a mod contains.  Group Masks are an optional parameter that allows you to specify which record types you are interested in importing.
```cs

using var mod = OblivionMod.Create(OblivionRelease.Oblivion)
    .FromPath(pathToMod)
    .Mutable()
    .WithGroupMask(new GroupMask()
    {
        Potions = true,
        NPCs = true,
    })
    .Construct();
```
This import call will only process and import Potions and NPCs.

!!! info "Generally Unused"
    Generally this feature is unused, as [Binary Overlays](#read-only) handle the situation of selectively accessing data much better.

## Flexible Game Target
In more complex setups, often the game type is not known at compile time.

=== "Untyped"
    ```cs
    using var readOnlyInputMod = ModFactory.ImportGetter(pathToMod, release);
    var mutableMod = ModFactory.ImportSetter(pathToMod, release);
    ```

=== "Generic"
    ```cs
    var mod = ModFactory<TMod>.Importer(ModKey.FromFileName("MyMod.esp"), release);
    ```

    !!! success "Dispose Appropriately"
         Binary overlays are disposable, as they can keep streams open as they are accessed.  Make sure to utilize `using` statements to dispose of them appropriately.
