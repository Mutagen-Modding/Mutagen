## Construction

### Known Game
If you know the game you will be working with at compile time, this is the typical way to create a new mod:

``` cs
var newMod = new SkyrimMod(ModKey.FromFileName("MyMod.esp"), SkyrimRelease.SkyrimSE);
```

### Generic
In more complex setups, often the game type is a generic that is not known at compile time 

```cs
var newMod = ModInstantiator<TMod>.Activator(ModKey.FromFileName("MyMod.esp"), release);
```

## Importing
It is usually preferable to import as readonly initially.  As such, importing a mod as readonly is usually preferable.

[:octicons-arrow-right-24: Read Only Best Practices](../best-practices/Read-Only.md)

### Known Game
If you know the game you will be working with at compile time, this is the typical way to import a mod:

=== "Read Only"
    ```cs
    using var readOnlyInputMod = SkyrimMod.CreateFromBinaryOverlay(inputPath, SkyrimRelease.SkyrimSE);
    ```

    !!! success "Dispose Appropriately"
         Binary overlays are disposable, as they can keep streams open as they are accessed.  Make sure to utilize `using` statements to dispose of them appropriately.

=== "Mutable"
    ```cs
    var mutableInputMod = SkyrimMod.CreateFromBinary(inputPath, SkyrimRelease.SkyrimSE);
    ```    

### Generic
In more complex setups, often the game type is a generic that is not known at compile time.   When in this mode functionality does not need to know whether it's a SkyrimMod, or a Fallout4Mod if that's more useful.  

This of course, comes with the downside of only being able to interact with the object as much as the TMod constraint allows.  You would not be able to access the Water group, for example, as it's uncertain if the mod in question has those.

```cs
public void SomeFunction<TMod>(GameRelease release, ModPath inputPath)
    where TMod : IModGetter
{
    using var importedMod = ModInstantiator<TMod>.Importer(inputPath, release);
    // Limited but reusable generic functionality
}
```

There is no explicit specification of mutable vs readonly in the function's definition.  It's up to the caller to decide if `TMod` will be a settable type or not, and an overlay or mutable class will be made under the hood as needed.

!!! success "Dispose Appropriately"
    Since the compile time does not know if it will be an overlay, it always returns a disposable object.  Make sure to utilize `using` statements.
