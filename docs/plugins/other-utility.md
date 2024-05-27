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
	