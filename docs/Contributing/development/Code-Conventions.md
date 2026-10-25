# Code Conventions

## Generated Code
Files ending in `_Generated.cs` are written by the code generator and should never be edited by hand.  Change the record's xml or the generator, then regenerate.  See [Code Generation](Code-Generation.md).

## Adding Files to Projects
The core and game projects (`Mutagen.Bethesda.Core`, `Mutagen.Bethesda.Skyrim`, and so on) list every source file explicitly in their `.csproj`, so that generated files can be nested under their xml definitions.  A new `.cs` file in one of these projects needs a matching entry:

```xml
<Compile Include="Plugins\Analysis\DI\MultiModFileReader.cs" />
```

If a new file seems to be ignored by the build, check that it is listed.

## Comments
Comment only when the code alone can't explain itself, such as a workaround for a quirk in the game's data or a non-obvious ordering requirement.  Don't restate what the code does, and don't add summary blocks to tests.

The reasoning behind a change belongs in the commit message or pull request.

## Prefer `ModPath`
When working with a path to a mod file, take a `ModPath` rather than a separate `DirectoryPath` and `ModKey`.  A `ModPath` carries both, so they can't drift out of sync.

```cs
// Preferred
public void Process(ModPath modPath)
{
    var modKey = modPath.ModKey;
    var filePath = modPath.Path;
}

// Avoid
public void Process(DirectoryPath folder, ModKey modKey)
{
    var filePath = Path.Combine(folder.Path, modKey.FileName);
}
```

## Avoid `dynamic`
Mutagen's interfaces and generic methods cover the cases where `dynamic` might be tempting.  Using them keeps type checking at compile time, rather than surfacing mistakes at runtime.

## Use Named Constants
Prefer existing constants over literal numbers, such as header lengths from `GameConstants`.  When no constant exists, give the value a well-named one.
