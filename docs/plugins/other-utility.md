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
	
## Common to Generic Crossover
Often you'll be dealing with code in a generic setup where you aren't sure what game you'll be targeting.  Often one route to deal with this is to define generics:
```cs
public void MyFunction<TMod, TModGetter>(TMod mod)
    where TModGetter : IModGetter
    where TMod : IMod, TModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
{
    // Do something
}
```

This is often a great route to allow the caller to know what type they want to interact with, and let them decide.
However, at some point higher up in your code, adding generics becomes burdensome, or the API cannot offer generics for one reason or the other.

What happens when we have a call that is not generic, but wants to call one that is?
```cs
public class MyClass
{
    public void DoSomeThings(IMod mod)
    {
        // What do we put here?
        DoSomeThingsGeneric<???, ???>(mod);
    }

    private void DoSomeThingsGeneric<TMod, TModGetter>(TMod mod)
        where TModGetter : IModGetter
        where TMod : IMod, TModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
    {
        // Actual logic
    }
}
```

The answer is that we would use C# reflection to make the first function call the 2nd.  However, this is fairly gritty code that is easy to get wrong.

Mutagen offers `ModToGenericCallHelper` to help with this.  Take the above example again:
```cs
public class MyClass
{
    public void DoSomeThings(IMod mod)
    {
	    // Mutagen helper class
        ModToGenericCallHelper.InvokeFromCategory(
		    // Pass in the object that has the generic function we want to call
            this,
			// What category is this related to?  The mod knows its own category, here.
            mod.GameRelease.ToCategory(),
			// This is a bit of C# reflection to locate the method we want to call
			// In our case, it's in a class called MyClass, the function is called DoSomeThingsGeneric,
			// which is private and non-static
            typeof(MyClass).GetMethod(nameof(DoSomeThingsGeneric), BindingFlags.NonPublic | BindingFlags.Instance)!,
			// What parameter to pass to DoSomeThingsGeneric.  Might have more depending on the call you designed
            new object[] { mod });
    }

    private void DoSomeThingsGeneric<TMod, TModGetter>(TMod mod)
        where TModGetter : IModGetter
        where TMod : IMod, TModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
    {
        // Actual logic
    }
}
```

!!! tip "Optimization"
    To optimize the above call a bit more, we could do the GetMethod call once ahead of time (in MyClass' constructor) and store the MethodInfo for re-use every time DoSomeThings got called, as it does not change per call
	
This helper abstracts away the complexityof locating what types `<TMod, TModGetter>` are, and wiring those up.