# Testing

Mutagen's tests are written with [xUnit](https://xunit.net/), [AutoFixture](https://github.com/AutoFixture/AutoFixture), [NSubstitute](https://nsubstitute.github.io/), and [Shouldly](https://docs.shouldly.org/).  

## AutoData Attributes
These attributes tie in AutoFixture mechanisms which handle a lot of the arrange phase lifting.

New tests should be a `[Theory]` with one of Mutagen's AutoData attributes, and take what they need as parameters.

| Attribute | Use When |
| --- | --- |
| `MutagenAutoData` | The test works with plain values, paths, and the filesystem.  Interfaces are filled with NSubstitute mocks. |
| `MutagenModAutoData` | As above, and the test needs mods and records. |
| `MutagenContainerAutoData` | The test is exercising one of Mutagen's services.  See [Testing Services](#testing-services). |

Each takes a `GameRelease` (SkyrimSE by default):

```cs
[Theory, MutagenModAutoData(GameRelease.Fallout4)]
```

The AutoFixture mechanisms let tests request parameters for testing.  Most of the generated values are random and should only be used for when you need an item for a test, but the exact value doesn't matter.  When the test relies on a specific value to flex an edge case, write it out.

### Primitives
Most everyday types can be requested out of the box.  This is plain AutoFixture behavior, and has nothing to do with Mutagen:

- `string`, `int`, `float`, `bool`, and the other built-in types
- Enums
- `Guid` and `DateTime`
- Arrays, lists, and other collections of any of the above

```cs
[Theory, MutagenAutoData]
public void Example(
    string name,
    int count,
    List<string> names)
{
}
```

### ModKeys
Ask for `ModKey` parameters rather than constructing them.  AutoFixture looks to the parameter's name to decide what kind of ModKey is made:

- Contains `master` → a master (`.esm`)
- Contains `light` → a light master (`.esl`)
- Anything else → a plugin (`.esp`)
- Also contains `existing` → an empty file for it is created in the data folder of the test's `IFileSystem`.  See [Filesystem](#filesystem).

```cs
[Theory, MutagenAutoData]
public void Example(
    IFileSystem fileSystem,
    IDataDirectoryProvider dataDirectory,
    ModKey masterModKey,
    ModKey lightModKey,
    ModKey otherModKey,
    ModKey existingModKey)
{
    masterModKey.Type.ShouldBe(ModType.Master);
    lightModKey.Type.ShouldBe(ModType.Light);
    otherModKey.Type.ShouldBe(ModType.Plugin);
    fileSystem.File.Exists(Path.Combine(dataDirectory.Path, existingModKey.FileName)).ShouldBeTrue();
}
```

`FormKey` parameters can be requested as well.

### Mods and Records
`MutagenModAutoData` can provide concrete mods and records.  A record parameter is added to the most recent mod parameter listed before it:

```cs
[Theory, MutagenModAutoData]
public void Example(
    SkyrimMod master,
    Npc masterNpc,
    SkyrimMod plugin,
    Npc pluginNpc)
{
    master.Npcs.ContainsKey(masterNpc.FormKey).ShouldBeTrue();
    plugin.Npcs.ContainsKey(pluginNpc.FormKey).ShouldBeTrue();
    pluginNpc.FormKey.ModKey.ShouldBe(plugin.ModKey);
}
```

Records come with only their FormKey set.  Pass `ConfigureMembers: true` to the AutoData attribute fill every property with generated values.

### Filesystem
All of the AutoData attributes provide a fake, in-memory `IFileSystem` by default.  Take it as a parameter and pass it into whatever reads or writes files.  

Paths can be requested too:

| Type | Provides |
| --- | --- |
| `FilePath` | A unique file path |
| `DirectoryPath` | A unique directory path |
| `ModPath` | A path to a mod file in the data folder, along with its `ModKey` |

By default, nothing exists at these paths.  If the parameter's name contains `existing`, such as `existingDirectory`, it is created in the test's `IFileSystem`.  Files are created empty.

The same `IFileSystem` instance is given to the test and to everything else in it, so these files and folders are visible to the code under test.

```cs
[Theory, MutagenAutoData]
public void Example(
    IFileSystem fileSystem,
    FilePath existingFile,
    DirectoryPath existingDirectory,
    ModPath existingModPath,
    FilePath otherFile)
{
    fileSystem.File.Exists(existingFile).ShouldBeTrue();
    fileSystem.Directory.Exists(existingDirectory).ShouldBeTrue();
    fileSystem.File.Exists(existingModPath).ShouldBeTrue();
    fileSystem.File.Exists(otherFile).ShouldBeFalse();
}
```

A full write and read round trip looks like this:

```cs
[Theory, MutagenModAutoData]
public void RoundTrip(
    IFileSystem fileSystem,
    SkyrimMod mod,
    Npc npc,
    string name,
    DirectoryPath existingDirectory)
{
    npc.Name = name;
    var path = new ModPath(mod.ModKey, Path.Combine(existingDirectory, mod.ModKey.FileName));
    mod.BeginWrite
        .ToPath(path)
        .WithNoLoadOrder()
        .WithFileSystem(fileSystem)
        .Write();

    using var reimport = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
        .FromPath(path)
        .WithFileSystem(fileSystem)
        .Construct();
    reimport.Npcs.ShouldHaveSingleItem().Name!.String.ShouldBe(name);
}
```

This keeps tests fast, lets them run in parallel without colliding, and leaves nothing behind on disk.

## Testing Services
Use `MutagenContainerAutoData` when testing one of Mutagen's services, such as a class from a `DI` namespace.  Its dependencies are filled with the real implementations registered in `MutagenModule`, so the test runs the same wiring that users get, and also catches registration mistakes.

The test sets up the environment, and the services read from it.  The pieces of the environment are single instances, so the test and the code under test see the same ones:

- `IFileSystem`
- `IDataDirectoryProvider` and `IGameDirectoryProvider`
- The fakes in `Mutagen.Bethesda.Testing.Fakes`, such as `ManualLoadOrderProvider` and `ManualArchiveIniListings`

```cs
[Theory, MutagenContainerAutoData]
public void Example(
    IFileSystem fileSystem,
    ManualLoadOrderProvider loadOrder,
    ManualArchiveIniListings iniListings,
    IDataDirectoryProvider dataDirectory,
    GetApplicableArchivePaths sut)
{
    loadOrder.SetTo("Skyrim.esm", "MyMod.esp");
    iniListings.SetTo("Explicit.bsa");
    fileSystem.File.WriteAllText(Path.Combine(dataDirectory.Path, "MyMod.bsa"), string.Empty);
    fileSystem.File.WriteAllText(Path.Combine(dataDirectory.Path, "Explicit.bsa"), string.Empty);
    sut.Get().ShouldBe([
        Path.Combine(dataDirectory.Path, "Explicit.bsa"),
        Path.Combine(dataDirectory.Path, "MyMod.bsa"),
    ]);
}
```

The system under test can be requested as either its class or its interface.  Mods and records can be requested the same way as with `MutagenModAutoData`.

If the system under test reads its inputs in its constructor, request it as `Lazy<T>`, set up the environment, and then access `.Value`.

Avoid `[Frozen]` in these tests.  It only reaches the constructor of a class AutoFixture builds itself, and not anything the container builds behind it, so it's easy to believe a mock is being used when it isn't.

If a test needs a dependency to look or behave a certain way that the existing fakes can't express, extend the fake or add a new one to `Mutagen.Bethesda.Testing.Fakes`.  This includes failure cases: rather than switching to a mock to make a dependency throw, give its fake a way to throw.  A fake behaves like the real thing everywhere the test didn't touch, while a mock quietly returns defaults for anything that wasn't set up.

## When to Use Mocks
Use mocks rarely.  Call counts and other interaction checks are less valuable than real side effects, like the files written or the load order produced, and they break when the implementation changes even if its behavior doesn't.

When a mock is the right fit, use `MutagenAutoData`, which fills every dependency with an NSubstitute mock.  Mark a parameter `[Frozen]`, before the system under test, so the system under test receives that same instance:

```cs
[Theory, MutagenAutoData]
public void QueriesOnceInitially(
    [Frozen] IPluginListingsProvider listings,
    PluginLiveLoadOrderProvider sut)
{
    sut.Get(out _).AsObservableList();
    listings.Received(1).Get();
}
```

The filesystem, its `MockFileSystemWatcher`, and `IDataDirectoryProvider` are always shared, with or without `[Frozen]`.

## Assertions
Use Shouldly (`x.ShouldBe(y)`, `x.ShouldBeTrue()`, `list.ShouldHaveSingleItem()`) rather than xUnit's `Assert`.  Its failure messages include the expression that failed, which makes failures easier to read.

Every test should assert something.  A call whose result is discarded, like `fileSystem.File.Exists(path);`, verifies nothing.

## Building Test Data
Prefer creating records through the normal API and writing them out, over hand-assembling bytes.

When a test does need raw bytes, such as a malformed record that the API can't produce, take sizes and offsets from `GameConstants` rather than writing numbers inline:

```cs
var header = new byte[GameConstants.Fallout4.MajorConstants.HeaderLength];
```

## Bug Fixes
Every bug fix should come with a test that fails without the fix.  If a bug applies to several games, cover each one, typically by passing a different `GameRelease` to the AutoData attribute.
