# Importing
All Mods are generated with the ability to create themselves from their binary plugin format (esp/esm/esl).

## Read Only
By default, mods are readonly.  This comes with a lot of speed and memory usage upsides.  Only fields that are accessed get parsed, which saves a lot of time and work.

```cs
using var mod = OblivionMod.Create(OblivionRelease.Oblivion)
    .FromPath(pathToMod)
    .Construct();
// Code accessing the mod
```

!!! tip "Preferred"
    Use readonly when possible, up until you want to mutate records.

	[:octicons-arrow-right-24: Mutation Patterns](Create,-Duplicate,-and-Override.md#overriding-records)

### No Up Front Work
With readonly objects, a mod object is returned almost immediately for use after having done almost no parsing.  As the user accesses members of the `Mod`, the parsing is done on demand for only the members accessed.

### No Persistent References to Records or Memory
Readonly systems keep no internal references to any record objects.  This means that when a user is done working with something and discards it, the GC is allowed to immediately clean it up.

### Requires an Open Stream
Readonly objects keep reference to an open stream internally, so they can read when accessed.

!!! info "Disposable"
    Binary Overlay objects implement `IDisposable`.  Putting them in `using` statements to close when appropriate is good practice.

### Best Practices

!!! warning "Avoid Repeated Access"
    The Overlay pattern has the downside that repeated access of properties means repeated parsing of the same data.

[:octicons-arrow-right-24: Overlay Best Practices](../best-practices/Overlays-Single-Access.md)

## Mutable
A normal C# object can be created containing all of a mod's data.  This is generally be reserved for when constructing or modifying records for output.

```cs
using var mod = OblivionMod.Create(OblivionRelease.Oblivion)
    .FromPath(pathToMod)
    .Mutable()
    .Construct();
```

The only difference is we add `Mutable()`, which returns a mutable variant of the mod object, which is fulfilled in a completely different way. The entire mod will have been loaded into memory via classes created to store all the various fields and records.   You can then begin to interact with the mod object.

!!! warning "Heavier Load"
    This route spends more time and memory loading in everything up front, even the data you will not be interacting with.

	If you don't need to modify the mod, consider instead:

	[:octicons-arrow-right-24: Read Only Mod Importing](#read-only)

## Flexible Game Target
In more complex setups, often the game type is not known at compile time.

=== "Untyped"
    ```cs
    using var readOnlyInputMod = ModFactory.ImportGetter(pathToMod, release);
    var mutableMod = ModFactory.ImportSetter(pathToMod, release);
    ```

=== "Generic"
    ```cs
    var mod = ModFactory<TMod>.Importer(ModKey.FromFileName("MyMod.esp"), release);
    ```

    !!! success "Dispose Appropriately"
         Binary overlays are disposable, as they can keep streams open as they are accessed.  Make sure to utilize `using` statements to dispose of them appropriately.

## Builder Pattern
All mod importing in Mutagen uses a fluent builder pattern that allows you to customize the import process. The builder pattern starts with `Create()` and ends with `Construct()`, with various configuration methods in between.

### Basic Structure
```cs
var mod = OblivionMod.Create(OblivionRelease.Oblivion)  // Start builder
    .FromPath(pathToMod)                                 // Specify source
    .Construct();                                        // Execute import
```

The builder pattern is designed to guide you through the import process with a progressive API that requires certain decisions at specific points.

### Source Selection
The first required step is to specify where to load the mod from.

#### FromPath
Loads a mod from a file path. This is the most common approach.

```cs
var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromPath(new ModPath(modKey, pathToModFile))
    .Construct();
```

#### FromStream
Loads a mod from an open stream. The stream must remain open for the lifetime of the overlay.

```cs
using var stream = File.OpenRead(pathToModFile);
var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromStream(stream, modKey)
    .Construct();
```

!!! warning "Stream Lifetime"
    For readonly imports, the stream must remain open as data is read on-demand. For mutable imports, the stream can be closed after `Construct()` completes.

#### FromStreamFactory
Loads a mod using a stream factory that creates new streams on demand. Useful for readonly imports that need multiple concurrent streams.

```cs
var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromStreamFactory(() => File.OpenRead(pathToModFile), modKey)
    .Construct();
```

!!! info "Stream Factory Requirements"
    The factory must return a new stream each time it is called. Returning the same stream instance will cause errors.

### Data Folder

#### WithDataFolder
Specifies the data folder location for loading master files.

```cs
var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromPath(modPath)
    .WithLoadOrder(masterKey1, masterKey2)
    .WithDataFolder(dataFolderPath)
    .Construct();
```

#### WithDefaultDataFolder
Uses the game's installed data folder location automatically.

```cs
var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromPath(modPath)
    .WithDefaultDataFolder()
    .Construct();
```

### Split File Support

#### WithAutoSplitSupport
Enables automatic detection and merging of split mod files (e.g., `ModName_1.esp`, `ModName_2.esp`).

```cs
var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromPath(modPath)
    .WithAutoSplitSupport()
    .Construct();
```

!!! warning "Path Only"
    Auto-split support only works with `FromPath`. Using `FromStream` with this option will throw an exception.

[:octicons-arrow-right-24: More on Split Files](Exceeding-Master-Limits.md)

### Load Order Configuration
For most games and situations, **load order is not required** for reading mods. Load order information is only necessary in these specific cases:

- **Starfield** - Required for proper FormID resolution with separated masters [:octicons-arrow-right-24: See Starfield section](#starfield-and-separated-master-games)
- **ExtraData Usage** - Suggested if your code will interact with ExtraData [:octicons-arrow-right-24: ExtraData documentation](specific/ExtraData.md)

For all other situations, you can safely omit load order configuration.

#### WithLoadOrder (ModKeys)
Provides a list of ModKeys to load from the data folder.

```cs
var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromPath(modPath)
    .WithLoadOrder(masterKey1, masterKey2)
    .WithDataFolder(dataFolderPath)
    .Construct();
```

#### WithDefaultLoadOrder
Uses the game's default load order from the installed game location.

```cs
var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromPath(modPath)
    .WithDefaultLoadOrder()
    .Construct();
```

#### WithLoadOrderFromHeaderMasters
Reads the master list from the mod header and loads those specific mods.

```cs
var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromPath(modPath)
    .WithLoadOrderFromHeaderMasters()
    .WithDataFolder(dataFolderPath)
    .Construct();
```

#### WithLinkCache
Provides an existing LinkCache instead of constructing one from a load order.

```cs
var linkCache = mods.ToImmutableLinkCache();

var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromPath(modPath)
    .WithLinkCache(linkCache)
    .Construct();
```

!!! warning "Mutual Exclusivity"
    `WithLinkCache` and `WithLoadOrder` methods are mutually exclusive. Use one or the other, not both.

#### WithNoLoadOrder
Skips load order processing entirely. Use with caution.

```cs
var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromPath(modPath)
    .WithNoLoadOrder()
    .Construct();
```

!!! danger "Potential Corruption"
    For Starfield and other separated master games, using `WithNoLoadOrder` can cause data corruption if the mod references light or medium masters.

### String Configuration
Control how strings and translations are handled during import.

#### WithStringsFolder
Overrides the default strings folder location.

```cs
var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromPath(modPath)
    .WithStringsFolder(customStringsFolder)
    .Construct();
```

#### WithTargetLanguage
Sets the target language for TranslatedString fields.

```cs
var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromPath(modPath)
    .WithTargetLanguage(Language.French)
    .Construct();
```

#### WithEncoding
Overrides the encoding provider used for string parsing.

```cs
var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromPath(modPath)
    .WithEncoding(customEncodingProvider)
    .Construct();
```

### Performance Options

#### Parallel Processing
Enables or disables parallel processing during import. Enabled by default for mutable imports.

```cs
// Enable parallel processing
var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromPath(modPath)
    .Mutable()
    .Parallel(true)
    .Construct();

// Disable parallel processing
var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromPath(modPath)
    .Mutable()
    .SingleThread()
    .Construct();
```

### Mutable-Specific Options
These options are only available when using `.Mutable()`.

#### WithGroupMask
Limits which record types are imported, reducing memory usage and load time.

```cs
var mod = OblivionMod.Create(OblivionRelease.Oblivion)
    .FromPath(pathToMod)
    .Mutable()
    .WithGroupMask(new GroupMask()
    {
        Potions = true,
        NPCs = true,
    })
    .Construct();
```

!!! info "Generally Unused"
    Generally this feature is unused, as [Binary Overlays](#read-only) handle the situation of selectively accessing data much better.

#### WithErrorMask
Adds error tracking to help debug issues during import.

```cs
var errorMask = new ErrorMaskBuilder();
var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromPath(modPath)
    .Mutable()
    .WithErrorMask(errorMask)
    .Construct();

// Check for errors
if (errorMask.Overall != null)
{
    Console.WriteLine(errorMask.Overall.ToString());
}
```

### Error Handling

#### ThrowIfUnknownSubrecord
Controls whether unknown subrecords cause exceptions.

```cs
var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromPath(modPath)
    .ThrowIfUnknownSubrecord(true)
    .Construct();
```

### Advanced Options

#### WithFileSystem
Provides a custom file system implementation for testing or virtualization.

```cs
var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromPath(modPath)
    .WithFileSystem(customFileSystem)
    .Construct();
```

### Starfield and Separated Master Games
Starfield and other games with separated master load orders require special handling for light and medium masters.

#### WithKnownMasters
Provides master style information without requiring the files to be present in the data folder.

```cs
var knownMaster = new KeyedMasterStyle(masterKey, MasterStyle.Medium);

var mod = StarfieldMod.Create(StarfieldRelease.Starfield)
    .FromPath(modPath)
    .WithKnownMasters(knownMaster)
    .WithNoLoadOrder()
    .Construct();
```

This is useful when you have master information but the actual master files aren't available in the data folder.

#### Separated Load Order Requirements
For Starfield, you must provide either:

1. A load order with mod objects (contains master style info):
   ```cs
   var mod = StarfieldMod.Create(StarfieldRelease.Starfield)
       .FromPath(modPath)
       .WithLoadOrder(masterMod1, masterMod2)
       .Construct();
   ```

2. A load order with ModKeys and a data folder (to read master style from files):
   ```cs
   var mod = StarfieldMod.Create(StarfieldRelease.Starfield)
       .FromPath(modPath)
       .WithLoadOrder(masterKey1, masterKey2)
       .WithDataFolder(dataFolderPath)
       .Construct();
   ```

3. Known masters with no load order (for controlled scenarios):
   ```cs
   var mod = StarfieldMod.Create(StarfieldRelease.Starfield)
       .FromPath(modPath)
       .WithKnownMasters(knownMaster1, knownMaster2)
       .WithNoLoadOrder()
       .Construct();
   ```

!!! danger "Required for Starfield"
    Failing to provide proper master information for Starfield mods will result in corrupted FormID resolution when the mod references light or medium masters.

### Group Masks
Often, users are not interested in all records that a mod contains.  Group Masks are an optional parameter that allows you to specify which record types you are interested in importing.
```cs

using var mod = OblivionMod.Create(OblivionRelease.Oblivion)
    .FromPath(pathToMod)
    .Mutable()
    .WithGroupMask(new GroupMask()
    {
        Potions = true,
        NPCs = true,
    })
    .Construct();
```
This import call will only process and import Potions and NPCs.

!!! info "Generally Unused"
    Generally this feature is unused, as [Binary Overlays](#read-only) handle the situation of selectively accessing data much better.
