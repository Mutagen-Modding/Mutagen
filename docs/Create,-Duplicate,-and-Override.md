## Create New Records
New records can be constructed in a few ways.  Note that a record's FormKey is required during construction, and immutable.  If you want to change the FormKey of a record, a new one should be made.  Any desired fields can be brought over via [CopyIn](Copy-Functionality#deepcopyin) mechanics.
### By Constructor
Any standalone record can be made by using its constructor.
```cs
// Verbose example, showing all the components
var modKey = ModKey.Factory("Oblivion", master: true);
var formKey = new FormKey(modKey, 0x123456);
var potion = new Potion(formKey);
```
### From a Mod's Group
If you have an `IMod` object that you want the record to originate from, you can easily make one from the corresponding `Group`.
```cs
var potion = mod.Potions.AddNew();
```
The new record will have the next available `FormKey` from that mod based on the metadata in its header, and automatically be added to the Group it originated from.  Note this is not applicable to Binary Overlays, as they are getter only interfaces, so this concept is not applicable.  This is instead meant for new Mod objects that have been created for the purpose of modification and eventual export.

### By Duplication
Duplicating a record is the equivalent of creating a fresh record, and then copying in data to match a different record.  This can be done via [CopyIn](Copy-Functionality#deepcopyin) API, but there are some convenience methods for this as well

```csharp
INpcGetter sourceNpc = ...;
// Add a new record with a new FormKey to our mod, with the given record's data
var copy = myMod.Npcs.DuplicateInAsNewRecord(sourceNpc);
// Modify the name of our new separate record
copy.Name = "My Name";
```

## Overriding Records
It is very common to want to modify a record that is from another mod.  This just entails making a copy of the original record and adding it to your output mod.  The fact that the FormKey doesn't originate from your output mod implicitly means that it's an override.

### GetOrAddAsOverride
The quick any easy way to override a record is to utilize a Group.GetOrAddAsOverride call.  This will check if the record already exists as an override in your group, and return it if so.  Otherwise, it will copy the source record to a new object, and add it to your mod for you.
```csharp
INpcGetter sourceNpc = ...;
// Retrieve or copy-and-add the record to our mod
var override = myMod.Npcs.GetOrAddAsOverride(sourceNpc);
// Modify the name to be different inside myMod
override.Name = "My New Name";
```

NOTE:  With this pattern, it is preferable to call `GetOrAddAsOverride` as late as possible, after all filtering has been done and you know you want to override and modify the record.  This avoids adding records to your mod that have no changes.

### Deep Copy Then Insert
This pattern is an alternative to `GetOrAddAsOverride`.  It is sometimes preferable if it's hard to delay the `GetOrAddAsOverride` call, and you want more control over when a record actually gets added to the outgoing mod.

```csharp
INpcGetter sourceNpc = ...;
// Make a new mutable object that is a copy of the record to override
var npcCopy = sourceNpc.DeepCopy();
// Modify the name
npcCopy.Name = "My New Name";
// Add the record to the mod we want to contain it
// Note that the copy has the same FormKey as the original, so it's considered an "override" when added to
// a new mod that will be loaded after it
outputMod.Npcs.Add(npcCopy);
```

This strategy works well if you might change your mind and not add the copied record to the outgoing mod.  It lets you get a mutable version of the record without adding it to your outgoing mod until you are certain you want to include it.

### Nested Records
Some records like Placed Objects and Cells are often nested underneath other records.  This makes it harder to follow the above patterns.  For these you will want to make use of the [[ModContexts]] concepts.
