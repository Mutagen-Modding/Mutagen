## Associated Files Locator

Mod files will often have several associated files in addition to a plugin (esm/esp) file:

- Strings Files
- Archive Files

This utility functionality locates all files that are associated with a given mod
=== "Static"
    ``` cs
    foreach (var file in PluginUtilityIO.GetAssociatedFiles(pathToMod))
	{
	
	}
    ```
=== "Dependency Injection"
    ``` cs
	IAssociatedFilesLocator filesLocator = ...;
	
    foreach (var file in filesLocator.GetAssociatedFiles(pathToMod))
	{
	
	}
    ```
	
An optional `AssociatedModFileCategory` parameter lets you specify which types to look for, in case you want to only look for a few:

- Plugin
- RawStrings
- Archives

Code to only look for the plugin and raw strings (excluding archives):
=== "Static"
    ``` cs
    foreach (var file in PluginUtilityIO.GetAssociatedFiles(
	   pathToMod, 
	   AssociatedModFileCategory.Plugin | AssociatedModFileCategory.RawStrings))
	{
	
	}
    ```
=== "Dependency Injection"
    ``` cs
	IAssociatedFilesLocator filesLocator = ...;
	
    foreach (var file in filesLocator.GetAssociatedFiles(
	   pathToMod, 
	   AssociatedModFileCategory.Plugin | AssociatedModFileCategory.RawStrings))
	{
	
	}
    ```
	
## Mod Files Mover

When moving a mod file to overwrite an old mod file, this can typically work with a simple line.
```cs
File.Move(originalPath, newPath, overwrite: true);
```

But for mods, this only works if it's a simple mod with a single file.  With mods that have localization or archive files have several associated files, this gets more complex.

This call will move a mod along with all its associated files to the new location.  Additionally, it will clean up old files no longer used by the overwritten version of the mod.  

For example, if you are overwriting a non-localized version of a mod onto a localized version, then the raw strings from the old setup will be removed.

=== "Static"
    ``` cs
    foreach (var file in PluginUtilityIO.GetAssociatedFiles(
	   pathToMod, 
	   AssociatedModFileCategory.Plugin | AssociatedModFileCategory.RawStrings,
	   overwrite: true))
	{
	
	}
    ```
=== "Dependency Injection"
    ``` cs
	IAssociatedFilesLocator filesLocator = ...;
	
    foreach (var file in filesLocator.GetAssociatedFiles(
	   pathToMod, 
	   AssociatedModFileCategory.Plugin | AssociatedModFileCategory.RawStrings,
	   overwrite: true))
	{
	
	}
    ```

!!! bug "Deleted Files"
    This function deletes files, so be sure you're calling it appropriately and have any backups desired