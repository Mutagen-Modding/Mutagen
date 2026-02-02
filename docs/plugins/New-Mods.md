# New Mods
How to make a new mod object from scratch without a file

## Known Game
If you know the game you will be working with at compile time, this is the typical way to create a new mod:

``` cs
var newMod = new SkyrimMod(ModKey.FromFileName("MyMod.esp"), SkyrimRelease.SkyrimSE);
```

## Unknown Game
In more complex setups, often the game type is not known at compile time

=== "Untyped"
    ```cs
    var newMod = ModFactory.Activator(ModKey.FromFileName("MyMod.esp"), release);
    ```

=== "Generic"
    ```cs
    var newMod = ModFactory<TMod>.Activator(ModKey.FromFileName("MyMod.esp"), release);
    ```
