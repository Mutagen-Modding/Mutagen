# Environment
When writing a program that is going to interact with Bethesda mods, often you would like to interact with the entirity of a game environment.  Mutagen comes with a convenience bootstrapper object that brings all the components of a game installations environment into one place:

- A LoadOrder object with the current load order 
- ReadOnly Mod objects ready for use on the load order object
- LinkCache relative to the load order
- Data folder path
- Load order file path (Plugins.txt)
- Creation Club load order file path (Skyrim.ccc)

!!! tip "Synthesis"
    If you're coding within a [Synthesis](https://github.com/Mutagen-Modding/Synthesis) Patcher, you should not make your own environment as described here.  Synthesis provides its own environment-like `IPatcherState` object in its Run function.
    
    [:octicons-arrow-right-24: Synthesis State Object](https://github.com/Mutagen-Modding/Synthesis/wiki/Coding-a-Patcher#synthesis-state-object)

## Simple Usage
```cs
using (var env = GameEnvironment.Typical.Skyrim(SkyrimRelease.SkyrimSE))
{
    // Now we can get straight to work
    Console.WriteLine($"Data folder was found to be at: {env.DataFolderPath}");
    Console.WriteLine($"Load Order has {env.LoadOrder.Count} mods");
    
    // Let's print the load order as an example
    Console.WriteLine($"Load Order:");
    foreach (var listing in env.LoadOrder.ListedOrder)
    {
        Console.WriteLine($"  {listing}");
    }
}
// Environment is now disposed, so all contained objects are no longer accurate or valid
```

## Advanced Usage
The above example just shows the basic one line environment definition to get the typical environment.  Mutagen by default will construct Game Environments relative the game installation registered by Steam.

If you have custom requirements or want to mix in output mods, etc, be sure to check out the [Environment Construction](Environment-Construction.md) documentation.

[:octicons-arrow-right-24: Advanced Game Environment Construction](Environment-Construction.md)

[:octicons-arrow-right-24: Game Locations](Game-Locations.md)
