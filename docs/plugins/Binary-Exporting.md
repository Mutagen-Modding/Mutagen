# Binary Exporting

## Exporting

Mod objects can be exported to an esp/esm on disk

### Builder
The preferred route for exporting is utilizing a "builder":
```cs
SkyrimMod mod = ...;

await mod.BeginWrite
    .ToPath(path)
    .WithDefaultLoadOrder()
    .WriteAsync();
```

The build acts as a "wizard", leading through the steps and options when writing.  The above will write the mod to the given path, looking up the load order in the default locations (plugins.txt) for the purposes of ordering the masters properly.

This builder comes with lots of dials and options to customize how you want to write a mod.

#### Load Order

`WithDefaultLoadOrder` leans on built in Mutagen systems to locate the load order.

You can also provide it yourself with `WithLoadOrder` by passing in a list of ModKeys or similar style objects.

Lastly, `WithLoadOrderFromHeaderMasters` lets you just use the mod objects existing header as the master ordering to refer to.  Being that the header isn't actually updated by Mutagen when modifying mod objects, this call is only useful in very specific situations.

#### Data Folder

The data folder can be overridden via `WithDataFolder`, where you can specify the path explicitly.

#### ModKeySync

These builder options specify the logic to use to keep a mod's ModKey in sync with its path.  By default the system will throw an exception if the output path's name does not match the ModKey of the mod object.

`NoModKeySync` will turn this off

`WithModKeySync` lets you specify explicitly what style you want, including `CorrectToPath`

#### FileSystem
`WithFileSystem` can be used to override the target filesystem.  By default, the normal OS filesystem will be used.

#### Masters List Content
By default, the exporting process will loop over all records in the mod to determine the masters that need to be included in the mod's header.  

`NoMastersListContentCheck` will turn this off, and write whatever is currently in the mod header.  Note that this does not affect the order of the mods, just the content.

#### Masters List ordering
By default, the load order will drive the order of the masters.  

`WithMastersListOrdering` lets you override and hand feed an alternative ordering.

#### Next Form ID
By default, all records will be iterated in order to find the next FormID available to be marked in the mod header.

`NoNextFormIDProcessing` can be called to disable this logic.

`WithForcedLowerFormIdRangeUsage` will override the behavior of whether lower range formIDs (0-0x800) should be used in the case of no records being present.

#### FormID Uniqueness
By default, writing will throw if duplicate FormIDs are encountered.

`NoFormIDUniquenessCheck` will disable this check

#### FormID Null Standardization
By default, all FormIDs that are null will be standardized to all zeros

`NoNullFormIDStandardization` turns this logic off.

#### FormID Compactness
Be default, writing will check the compaction flag (Small/Medium/Full) master style and confirm that the contents are in the allowed range.

`NoFormIDCompactnessCheck` will disable this check.

`WithFormIDCompactnessCheck` will let you explicitly specify.

#### Strings
`WithStringsWriter` lets you provide a custom StringsWriter

`WithTargetLanguage` lets you set the target language (for embedded setups)

`WithEmbeddedEncodings` lets you override the encodings used

#### Lower Range FormIDs

Some mods and releases in certain situations allow the usage of FormIDs all the way down to ID 0.

One requirement of this feature is that the mod has a single master.  By default, if lower IDs are used, but no master is present, then the first mod on the Load Order will be added as a placeholder.

`ThrowIfLowerRangeDisallowed` makes it throw in this situation instead.

`WithPlaceholderMasterIfLowerRangeDisallowed` lets you customize what will be used as the placeholder.

`NoCheckIfLowerRangeDisallowed` will disable the check entirely.

#### Parallel

Binary exporting has some parallelization potential.  By default, it will write as parallel as it can.

`WithParallelWriteParameters` lets you customize the behaviors.

`SingleThread` sets it to just use a single thread.

#### Extra Masters
Mutagen automatically determines what masters are necessary.

`WithExtraIncludedMasters` lets you add additional masters to include

`WithExplicitOverridingMasterList` lets you completely override the masters that are included.

`WithAllParentMasters` sets the logic to look at the parent mods and include their masters as well, recursively.   Requires DataFolder knowledge in order to find and read the parent mod content.

These options only concern themselves with master content.  The order of the masters is determined via other calls.

#### Overridden Forms List
Mutagen automatically populates a header's "overridden forms" list.

`WithOverriddenFormsOption` lets you disable/enable this logic.

### Legacy

There is a more direct call `WriteToBinary`
```cs
SkyrimMod mod = ...;

mod.WriteToBinary(desiredFilePath);
```
But this is not recommended, if the builder alternative is viable, as the customizations and requirements for a successful export are less easily accessed.

#### Typical Legacy Export
The recommended call for exporting a mod passes the load order to sort with as input:
```cs
SkyrimMod mod = ...;

mod.WriteToBinary(
    desiredFilePath,
    new BinaryWriteParameters()
    {
        MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrder)
    });
```

This extra information helps keep the masters in proper order, as the load order information is provided for the write call to use.  

## Compaction

