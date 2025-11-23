# Exceeding Master Limits

## Overview

Bethesda plugins have a hard limit of 255 masters per mod file. When a mod references records from more masters than this limit allows, it cannot be saved as a single file. Mutagen's multi-mod support provides automatic splitting of a single mod object into several mod files when exported, and merging functionality when reading to work around this limitation.

## File Naming Convention

When a mod is split, the files follow this naming pattern:

- `MyMod_1.esp`
- `MyMod_2.esp`
- `MyMod_3.esp`
- etc.

The files are numbered consecutively starting from `_1`. When reading, Mutagen expects no gaps in the sequence.

## Writing with Auto-Split

### Using the Builder (Recommended)

The easiest way to write a mod with automatic splitting is to use the `WithAutoSplit()` builder option:

```cs
SkyrimMod mod = ...;

await mod.BeginWrite
    .ToPath(outputPath)
    .WithLoadOrder(loadOrder)
    .WithAutoSplit()
    .WriteAsync();
```

When `WithAutoSplit()` is enabled:

1. Mutagen first attempts to write the mod normally
2. If a `TooManyMastersException` occurs, the mod is automatically split into multiple files
3. Records are distributed across files to keep each file under the master limit
4. Old split files (from previous writes with more splits) are automatically cleaned up

!!! warning
    Auto-split only works with file path writes (`ToPath` or `IntoFolder`). Using `ToStream` with `WithAutoSplit()` will throw a `NotSupportedException`.

## Reading Split Mods

### Using the Builder (Recommended)

The easiest way to read split mods is to use the `WithAutoSplitSupport()` builder option:

```cs
// Read as readonly overlay
using var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromPath(modPath)
    .WithAutoSplitSupport()
    .Construct();

// Read as mutable mod (deep copies the overlay)
var mutableMod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromPath(modPath)
    .WithAutoSplitSupport()
    .Mutable()
    .Construct();
```

When `WithAutoSplitSupport()` is enabled:

1. Mutagen checks if split files exist (e.g., `MyMod_1.esp`, `MyMod_2.esp`)
2. If found, automatically imports and merges them into a single unified view
3. If no split files exist, falls back to normal single-file import
4. For mutable imports, the merged overlay is deep-copied to a mutable mod

!!! warning
    Auto-split support only works with file path reads (`FromPath`). Using `FromStream` with `WithAutoSplitSupport()` will throw a `NotSupportedException`.

### Detecting Split Files

Use `MultiModFileAnalysis` to check if split files exist before reading:

```cs
using Mutagen.Bethesda.Plugins.Analysis;

// Check if split files exist
if (MultiModFileAnalysis.IsMultiModFile(folder, modKey))
{
    // Get the list of split file paths
    var splitFiles = MultiModFileAnalysis.GetSplitModFiles(folder, modKey);
    Console.WriteLine($"Found {splitFiles.Count} split files");
}
```

`IsMultiModFile` returns:

- `true` if valid split files exist (at least _1 and _2)
- `false` if no split files exist (single mod)
- Throws `SplitModException` if files are in an invalid state

### Using MultiModFileReader

The `MultiModFileReader` class provides a way to read split mod files for dependency injection scenarios:

```cs
using Mutagen.Bethesda.Plugins.Analysis.DI;

var reader = new MultiModFileReader();

// Read split files and get a merged overlay
using var mergedMod = reader.Read<ISkyrimModDisposableGetter>(
    folder,           // Directory containing the split files
    modKey,           // The base ModKey (without _1, _2 suffixes)
    GameRelease.SkyrimSE,
    loadOrder,        // Load order for master ordering
    BinaryReadParameters.Default);

// Use the merged mod as if it were a single file
foreach (var npc in mergedMod.Npcs)
{
    // Process records from all split files
}
```

The reader:

- Automatically detects split files matching the pattern `ModKey_1.ext`, `ModKey_2.ext`, etc.
- Validates that files are numbered consecutively
- Returns a read-only overlay that presents all split files as a single unified mod
- Throws `SplitModException` if no split files are found or only one file exists

### Using ModFactory

`ModFactory` is useful when you need to import split mods with generic type parameters. This is common in library code or utilities that work across multiple game types:

```cs
using Mutagen.Bethesda.Plugins.Records;

public TModGetter ImportSplitMod<TModGetter>(
    ModKey modKey,
    IEnumerable<ModPath> splitFiles,
    IEnumerable<IModMasterStyledGetter> loadOrder,
    GameRelease release)
    where TModGetter : class, IModDisposeGetter
{
    return ModFactory<TModGetter>.ImportMultiFileGetter(
        modKey,
        splitFiles,
        loadOrder,
        release);
}
```

### Using ModImporter (DI)

For dependency injection scenarios:

```cs
using Mutagen.Bethesda.Plugins.Records.DI;

var modImporter = new ModImporter(fileSystem, gameReleaseContext);

var mergedMod = modImporter.ImportMultiFile<ISkyrimModDisposableGetter>(
    modKey,
    splitFilePaths,
    loadOrder,
    BinaryReadParameters.Default);
```

## Important Considerations

### Read-Only Result

The merged multi-mod overlay is **read-only** (implements `IModGetter` only). If you need to modify the data, you must copy records to a new mutable mod.

### Resource Disposal

The merged overlay holds references to the underlying split files. Always dispose properly:

```cs
using var mergedMod = reader.Read<ISkyrimModDisposableGetter>(...);
// Use the mod
// Disposal happens automatically at end of using block
```

### Load Order Requirements

Writing with auto-split requires load order information to properly order masters in the split files. Ensure you provide an accurate load order that includes all masters referenced by the mod.

### Round-Trip Support

Mods written with auto-split can be read back using `WithAutoSplitSupport()`, preserving all records and their data:

```cs
// Write with auto-split
await mod.BeginWrite
    .ToPath(outputPath)
    .WithLoadOrder(loadOrder)
    .WithAutoSplit()
    .WriteAsync();

// Read back as merged overlay
using var roundTripped = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
    .FromPath(outputPath)
    .WithAutoSplitSupport()
    .Construct();

// roundTripped contains all records from the original mod
```

## Limitations

### Single Record Exceeding Master Limit

Auto-split works by distributing records across multiple files so each file stays under the 255 master limit. However, this cannot help when a **single record** itself references more than 255 masters.

For example, a `FormList` that contains references to items from 300 different master files cannot be split - the record itself requires all those masters to be present. In this case, `TooManyMastersException` will be thrown even with `WithAutoSplit()` enabled.

```cs
// This will throw TooManyMastersException
var formList = mod.FormLists.AddNew();
for (uint i = 0; i < 300; i++)
{
    var masterKey = new ModKey($"Master_{i}", ModType.Plugin);
    formList.Items.Add(new FormKey(masterKey, 0x800));
}

// Auto-split cannot help here - the single FormList needs all 300 masters
await mod.BeginWrite
    .ToPath(outputPath)
    .WithLoadOrder(loadOrder)
    .WithAutoSplit()  // Still throws TooManyMastersException
    .WriteAsync();
```

## Error Handling

### SplitModException

Thrown when:

- No split files are found for the specified ModKey (when using `MultiModFileReader`)
- Only one split file exists (expected at least 2)
- Both split files and original unsplit mod exist in the same directory

```cs
try
{
    // Check for split files - throws if invalid state
    if (MultiModFileAnalysis.IsMultiModFile(folder, modKey))
    {
        var files = MultiModFileAnalysis.GetSplitModFiles(folder, modKey);
    }
}
catch (SplitModException ex)
{
    // Handle invalid split file state
    Console.WriteLine(ex.Message);
}
```

### TooManyMastersException

When writing without `WithAutoSplit()`, this exception is thrown if the mod exceeds the master limit. Enable auto-split to handle this automatically.
