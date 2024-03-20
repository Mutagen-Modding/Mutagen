# Prefer Readonly Types
## Overview
Mutagen offers up records in several ways.  Consider dealing with an Npc, it would offer:

- `Npc` class.   A class with all the fields an Npc has
- `INpc` interface.   An interface with all the fields an Npc has.  The class implements this.
- `INpcGetter` interface.  An interface with all the fields an Npc has, but only gettable.  Cannot be modified.

In most example code, APIs, and projects you look at the code will mostly be dealing with `INpcGetter`, and you should too.

## Late Mutation
The best practice is to convert from readonly interfaces to mutable version as late as possible.  This allows the systems to avoid parsing the whole record when it's not applicable.

### Readonly Increases Speed
A lot of Mutagen's speed comes from short circuiting unnecessary work.  A big way it does this is by exposing records via [Binary Overlays](../plugins/Binary-Importing.md).  These are record objects that are very lightweight and fast.   But one of their downsides is they are read only.

As soon as you want to modify something, you have to first convert it to a settable version of the record.  This means creating a more "normal" settable `Npc` class, and reading ALL the data within that record to fill out each field one by one.  This is often a waste of time.

Take a look at our original example, if the Npc in question has a Level higher than 5, then all that work and time of reading the other fields is wasted.  Once we find out the level is higher than 5, we no longer care about it anymore, and would prefer to have not parsed any of the other data.  This is just one small example where it is preferable to remain in the parse-on-demand readonly mode as long as possible.

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
