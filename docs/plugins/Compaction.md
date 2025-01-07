# Compaction
Bethesda mod files come in a few compaction styles:

- `Full`  This is the normal mod, allowing for IDs in the range of 0xFFFFFF
- `Medium`  This is a mod that can only have IDs in the range 0xFFFF.
- `Small`  (Sometimes called Light).  Can only have IDs in the range 0xFFF

Compacting mods intelligently allows the game to load more mods overall.  These docs will not go into the gritty specifics of that.

## Compacting

`ModCompaction` is a static class that offers the ability to compact a mod to the given acceptable range for Small/Medium/Full masters.

This will reassign records out of range and give them new FormIDs.

```cs
IMod mod = ...;

ModCompaction.CompactToSmallMaster(mod);
```

This will throw if it is unable to compact for any reason.

```cs
IMod mod = ...;

ModCompaction.CompactToSmallMaster(mod, MasterStyle.Small);
```

This alternative will try to compact to `Small`, but will fall back to `Medium` and then `Full` if it is unable to do so.

## Compatibility Detection

`RecordCompactionCompatibilityDetection` provides functionality to test a mod to see if it could be compacted.

Some examples:
- `IsSmallMasterCompatible`
- `CouldBeSmallMasterCompatible`
- `GetSmallMasterRange`
- `IterateAndThrowIfIncompatible` 

Some of these calls have `bool potential` parameters. 

This corresponds to `Full` styles, where [Lower Range FormIDs](Exporting.md/#lower-range-formids) are only viable if there is a master present.  By marking `potential` as true, the functions will test viability assuming that there will be masters present.