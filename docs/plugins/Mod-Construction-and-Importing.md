## Construction

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
	
### Unknown Game
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
