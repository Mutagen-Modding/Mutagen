<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
## Table Of Contents

- [Use Environments When Possible](#use-environments-when-possible)
- [Game Locations](#game-locations)
  - [Game Folder vs Data Folder](#game-folder-vs-data-folder)
- [Get__Folder](#get__folder)
- [Sources](#sources)
- [AdHoc Installations](#adhoc-installations)
- [GetGameFolders](#getgamefolders)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

# Use Environments When Possible
Game location concepts are somewhat unnecessary to interact with, since usually the preferred entry point is via [Environments](Environment)

This section will go over it anyway, as some more complex programs might want direct access to the logic.

# Game Locations
## Game Folder vs Data Folder
The `Game` folder is the one containing the game's exe itself.  This is not where mods go, typically.

The `Data` folder is usually within the `Game` folder: `%Game%/Data`.  This is where mods reside, and as such is the typical folder of interest.

# Get__Folder
You can query for a Game or Data folder easily:
```cs
var dataFolder = GameLocations.GetDataFolder(GameRelease.SkyrimSE);
```
Also via a Try pattern
```cs
if (GameLocations.TryGetDataFolder(GameRelease.SkyrimSE, out var dataFolder))
{
}
```

# Sources
Currently, Mutagen locates games via a few sources:
- Looks in the registry
- Looks in Steam systems (via [GameFinder](https://github.com/erri120/GameFinder))

You can directly query the Registry, if you wish
```cs
if (GameLocations.TryGetGameFolderFromRegistry(GameRelease.SkyrimSE, out var gameFolder))
{
}
```

# AdHoc Installations
Note that this API will NOT locate ad-hoc game folders that exist randomly on your system.  It must be registered in one of the two above listed sources in order to be located.   Mutagen will not crawl the drives of your computer looking for installations.

As such, installations like [Wabbajack](https://github.com/wabbajack-tools/wabbajack) might have a game folder that is "off the grid".  In these situations, your tools need to offer some way for the user to define where their target data folder is, as the system will not be able to locate these unregistered folders automatically.

# GetGameFolders
There is an enumerable option to get all the Game folders from the above listed sources.  Currently that will only return at max two.
```cs
foreach (var location in GameLocations.GetGameFolders(GameRelease.SkyrimSE))
{
}
```