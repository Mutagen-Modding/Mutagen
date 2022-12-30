<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
## Table Of Contents

- [Typical Single Game Usage](#typical-single-game-usage)
- [GameEnvironmentState](#gameenvironmentstate)
- [Advanced Usage](#advanced-usage)
- [Synthesis Usage](#synthesis-usage)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

When writing a program that is going to interact with Bethesda mods, there are several things you typically want to interact with.  Mutagen comes with a convenience bootstrapper object that constructs them all for a typical installation and exposes them all in one place:

# Typical Single Game Usage
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

# GameEnvironmentState
The environment object that is given to you has lots of useful contextual items:
- A LoadOrder object with the current load order 
- ReadOnly Mod objects ready for use on the load order object, when they are found to exist
- LinkCache relative to the load order
- Data folder path
- Load order file path (Plugins.txt)
- Creation Club load order file path (Skyrim.ccc)

# Advanced Usage
The above example just shows the basic one line environment definition to get the typical environment.  Mutagen by default will construct Game Environments relative the game installation registered by Steam, as [described here](https://github.com/Mutagen-Modding/Mutagen/wiki/Game-Locations#sources). 

If you have custom requirements or want to mix in output mods, etc, be sure to check out the [Environment Construction](https://github.com/Mutagen-Modding/Mutagen/wiki/Environment-Construction) documentation.

# Synthesis Usage
If you're coding within a [Synthesis Patcher](https://github.com/Mutagen-Modding/Synthesis), you should not make your own environment as described here.  Synthesis provides its own environment-like `IPatcherState` object in its Run function.  [Read More](https://github.com/Mutagen-Modding/Synthesis/wiki/Coding-a-Patcher#synthesis-state-object)