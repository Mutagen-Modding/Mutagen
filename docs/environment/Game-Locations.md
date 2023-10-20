# Game Locations
!!! tip "Use Environments When Possible"
    Game location concepts are somewhat unnecessary to interact with, since usually the preferred entry point is via [Environments](index.md)

    [:octicons-arrow-right-24: Environments](index.md)


## Sources
Currently, Mutagen locates games via a few sources:

- Looks in the registry
- Looks in Steam systems (via [GameFinder](https://github.com/erri120/GameFinder))

GameLocation API will NOT locate ad-hoc game folders that exist randomly on your system.  It must be registered in one of the two above listed sources in order to be located automatically

!!! warning "Adhoc Installations Will Not Be Located"
    Installations like [Wabbajack](https://github.com/wabbajack-tools/wabbajack) might have a game folder that is "off the grid".  In these situations, your tools need to offer some way for the user to define where their target data folder is, as the system will not be able to locate these unregistered folders automatically.

    [:octicons-arrow-right-24: Wabbajack](https://github.com/wabbajack-tools/wabbajack)


## Game Folder vs Data Folder
The Game Location API uses "Game" and "Data" folder naming to represent two similar paths:

- `Game` folder is the one containing the game's exe itself.  This is not where mods go, typically.
- `Data` folder is usually within the `Game` folder: `%Game%/Data`.  This is where mods reside, and as such is the typical folder of interest.

## API
### Get__Folder
You can query for a Game or Data folder
=== "Get"
    ```cs
    var dataFolder = GameLocations.Get[Game/Data]Folder(GameRelease.SkyrimSE);
    ```

=== "TryGet"
    ```cs
    if (GameLocations.TryGet[Game/Data]Folder(GameRelease.SkyrimSE, out var dataFolder))
    {
    }
    ```

### GetGameFolders
There is an enumerable option to get all the Game folders from the various sources
```cs
foreach (var location in GameLocations.GetGameFolders(GameRelease.SkyrimSE))
{
}
```

### TryGetGameFolderFromRegistry
You can directly query the Registry, rather than looking at game installation paths.
```cs
if (GameLocations.TryGetGameFolderFromRegistry(GameRelease.SkyrimSE, out var gameFolder))
{
}
```