# Prefer Readonly Types
## Overview
Mutagen offers up records in several ways.  Consider dealing with an Npc, it would offer:

- `Npc` class.   A class with all the fields an Npc has
- `INpc` interface.   An interface with all the fields an Npc has.  The class implements this.
- `INpcGetter` interface.  An interface with all the fields an Npc has, but only gettable.  Cannot be modified.

In most example code, APIs, and projects you look at the code will mostly be dealing with `INpcGetter`, and you should too.

The best practice is to convert from readonly interfaces to mutable version as late as possible.  This allows the systems to avoid parsing the whole record when it's not applicable.  

To transition to a mutable object is typically done via [Override Mechanics](../plugins/Create,-Duplicate,-and-Override.md#overriding-records), which leans on copying the readonly object into a mutable one, which reads and parses all the fields during that process.

## Reasoning

Doing the bulk of your work on readonly objects has several major upsides.

### Readonly Increases Speed
A lot of Mutagen's speed comes from short circuiting unnecessary work.  A big way it does this is by exposing readonly records in a highly specialized fashion that are very lightweight and fast.   But one of their downsides is they are read only.


```cs
foreach (var readonlyNpc in mod.Npcs)
{
    // Readonly phase
	// Skip npc if health offset greater than 100
    if (readonlyNpc.Configuration.HealthOffset < 100) continue;

    // Mutable phase
    var npc = outgoingMod.Npcs.GetOrAddAsOverride(readonlyNpc);
    // Set all lower health offsets to be at least 100
    npc.Configuration.HealthOffset = 100;
}
```

For 99% of Npcs, we will just want to check if the HealthOffset is less than 100, and if so, skip.  Readonly mods are able to ONLY parse the data related to HealthOffset, and thus for 99% of the Npcs can skip 99% of the parsing work.

As soon as you want to modify something, you have to [first convert it to a settable version of the record](../plugins/Create,-Duplicate,-and-Override.md#overriding-records).  This means reading ALL the data within that record to fill out each field one by one.  This is often a waste of time, and so should be done as late as possible after all filtering and investigation code has run on readonly objects.

### Helps Avoid Malformed Mod Issues
If a mod has a single malformed record, this can cause parsing issues.  By using readonly mods, you will avoid interacting with this object entirely if it's not of interest to your program.   For example, if there's a malformed NavMesh object in a mod, but your program is only interested in Weapons, then you'll avoid the problem record entirely.

However, if your code was interested in that malformed NavMesh, then of course the core issue will still need to be dealt with, either by notifying the mod author, upgrading Mutagen code to handle it better, etc.

### Adds Clearer Intention to Modifications
#### A Fully Mutable Ecosystem Has Easy Pitfalls
Let's pretend for a moment that all records were mutable within the entire ecosystem.  This can easily lead to some very subtle bug prone situations.

```cs
// Retrieve an Npc from Skyrim.esm
// Making use of the Mutagen.Bethesda.FormKeys.Skyrim helper library
// Note that in this pretend example, the environment is providing setter interfaces
INpc orthorn = env.LoadOrder["Skyrim.esm"].Npcs[Skyrim.Npc.Orthorn];

// Modify speed to be x2
orthorn.Speed *= 2;

// Add to our outgoing patch
outgoingPatch.Npcs.Add(orthorn);
```

The above logic has some unexpected and probably undesirable side effects.  We not only overrode Orthorn to have more Speed in our outgoing patch, we also modified the original record as it existed within the `Skyrim.esm` mod object!   In fact, it's the same Npc class instance shared by both mods!  Changing the values in one changes the values in all, as both mods are pointing to the same object.  If future code is to iterate Skyrim's records, it would see that Orthorn is faster there, too, rather than the record having the original speed Skyrim.esm defined.  

Skyrim.esm should not be so easily modified.  We wanted to modify Orthorn as it was defined in our outgoing patch.  Skyrim.esm should be more or less immutable unless we take explicit intentional steps to do so.

#### Initially Immutable Environment Encourages Clearer Intentions
Take the same situation, but in the actual ecosystem that provides getter interfaces by default:
```cs
// Retrieve an Npc from Skyrim.esm
// Making use of the Mutagen.Bethesda.FormKeys.Skyrim helper library
// Note that the actual ecosystems provide only a Getter interface here
INpcGetter orthornGetter = env.LoadOrder["Skyrim.esm"].Npcs[Skyrim.Npc.Orthorn];

// We want to modify, and so we have to indicate which mod wants to contain that modification
var orthornSetter = outgoingPatch.Npcs.GetOrAddAsOverride(orthornGetter);

// Modify speed to be x2
orthornSetter.Speed *= 2;
```

This is better in a few ways:

- As part of the modification process, we are required to indicate which mod is going to "house" those modifications
- The object instance we are modifying only exists in our outgoing patch, rather than many mods
- The original Skyrim.esm definition is left intact.  In fact, it cannot possibly be modified as the entire mod object is readonly fundamentally.
