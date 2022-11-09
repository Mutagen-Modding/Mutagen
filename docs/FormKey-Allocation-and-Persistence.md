It is common that tooling that is generating new records when creating plugins wants to keep their FormKeys consistent across several runs.  The same records should get the same FormKeys.

There are some challenges with fulfilling this:
- How is a record detected to be the "same" as one from a previous run?
- Where/How do you persist the mapping information between runs?

# Mapping Records Via EditorID
One of the challenges is finding a way to map records from a previous run with records from the current run, so that their FormKeys can be synced.  Mutagen does this by requiring the developer provide a unique EditorID when they want syncing to occur.

Mutagen offers a few ways to hook into its allocation API.
```cs
// No EditorID is provided at time of creation, so no syncing features invoked
var unsyncedNpc = mod.Npcs.AddNew();

// EditorID is provided, so FormKey syncing features are active for this record
var syncedNpc = mod.Npcs.AddNew("SomeUniqueEdid");

// Some more routes
var syncedNpc2 = new Npc(mod, "SomeThirdEdid");
var syncedNpc3 = new Npc(mod.GetNextFormKey("SomeOtherEdid"));
```

For the records where the EditorID was provided at the time of creation, syncing functionality will be applied if enabled (more on this later).

# Keep EditorIDs Unique
With this pattern, the burden is on the developer to ensure that all records created in this fashion are supplied unique EditorIDs.  If new record is made with an EditorID that has already been used in the current "run", then an exception will be thrown.

It is recommended to name the EditorIDs off the aspects that drove the record to be made in the first place:

`MyMod_GlassArmorNoviceShockEnchantment`

`MyMod_GlassArmorMasterShockEnchantment`

Since each is named with a specific "goal" in mind, it will be less likely to collide.

# Persistence and Allocation
The other half that needs to be considered is where the mapping information is stored, and how that data gets imported/used to fulfill the allocation requests described above.

## Setting a Mod's Allocator
Every mod can have its FormKey allocator set, which is the logic that hands out new FormKeys to records.  This where a FormKey syncing allocator can be injected with whatever behavior we wish.

```cs
var mod = new SkyrimMod(...);
var allocator = new TextFileFormKeyAllocator(mod, pathToFile);
mod.SetAllocator(allocator);
```
This would set the mod to sync its FormKeys to a text file at the given path.

## Text File Allocators
These alloctors save their data into a text file with the following format:
```
TheEditorIdToSyncAgainst
123456
[...Repeat...]
```

One thing of note is that it saves just the FormID, without the mod indices.  The ModKey to be associated with is not persisted in the file itself.

### TextFileFormKeyAllocator
This a simplistic 1:1 allocator from a single mod to a single file.  As such, the ModKey of the mod is combined with the FormID retrieved from the file to get the actual FormKey for use.

```cs
var mod = new SkyrimMod(...);
var allocator = new TextFileFormKeyAllocator(mod, pathToFile);
mod.SetAllocator(allocator);
```

### TextFileSharedFormKeyAllocator
This is a more advanced allocator for when several separate sources need to coordinate together to avoid FormKey collisions.  A prime example is a Synthesis patcher run, where several separate programs will run, and all need to avoid allocating FormKeys that another has used.

```cs
var mod = new SkyrimMod(...);
var allocator = new TextFileSharedFormKeyAllocator(mod, pathToFolder, "MyProgramName");
mod.SetAllocator(allocator);
```

This will save to a folder, instead, with a file within under "MyProgramName" that has this specific programs sync information.  However, the system will examine other files within that folder so that those FormKeys can be avoided when allocating fresh new FormKeys.

### Sqlite
There is also the beginnings of a Sqlite backed persistence system within `Mutagen.Bethesda.Sqlite`.  It needs to be optimized before it will be a viable choice.

## Saving Allocation State
Allocators are created separately from a mod, even if they are assigned to a mod and tightly associated with it.  As such, allocators are themselves in charge of persisting their state once allocations have been made.  Typically this is done via disposal mechanics:

```cs
var mod = new SkyrimMod(...);
// This API pattern will dispose the allocator when its variable goes out of scope
using var allocator = new TextFileFormKeyAllocator(mod, pathToFile);
mod.SetAllocator(allocator);

// Or
var mod2 = new SkyrimMod(...);
using (var allocator2 = new TextFileFormKeyAllocator(mod, pathToFile))
{
    mod2.SetAllocator(allocator2);
    // Do work on mod
} // Allocator exports results to disk here

// Or
var mod3 = new SkyrimMod(...);
var allocator3 = new TextFileFormKeyAllocator(mod, pathToFile)
mod3.SetAllocator(allocator3);

// Do work on mod
// Manually dispose or persist allocator when done
allocator3.Commit(); // Or Dispose
```
