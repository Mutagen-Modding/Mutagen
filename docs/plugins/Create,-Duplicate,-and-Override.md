---
order: 250
---
# Create, Duplicate, and Override
## Create New Records
New records can be constructed in a few ways.  Note that a record's FormKey is required during construction, and immutable.  If you want to change the FormKey of a record, a new one should be made.  Any desired fields can be brought over via [CopyIn](Copy-Functionality.md#deepcopyin) mechanics.
### By Constructor
Any standalone record can be made by using its constructor.
``` { .cs hl_lines="2" }
FormKey formKey = ...;
var potion = new Potion(formKey);
```
### From a Mod's Group
If you have an `IMod` object that you want the record to originate from, you can easily make one from the corresponding `Group`.
```cs
var potion = mod.Potions.AddNew();
```
!!! into "Claims Next Available FormKey"
    The new record will have the next available `FormKey` from that mod based on the metadata in its header, and automatically be added to the Group it originated from.  
    
### By Duplication
Duplicating a record creates a new record with a new FormKey while copying all the data from an existing source record. This differs from overriding, where the original FormKey is preserved. Duplication is useful when you want to create a new, distinct record based on an existing one.

#### Key Differences: Duplication vs Override
- **Duplication**: Assigns a new FormKey → Creates a new record
- **Override**: Preserves the original FormKey → Modifies an existing record

#### DuplicateInAsNewRecord
For simple, non-nested records, use the `DuplicateInAsNewRecord` convenience method:

=== "Claim Next FormID"
    ``` { .cs hl_lines=4 }
    INpcGetter sourceNpc = ...;
    ISkyrimMod myMod = ...;

    var copy = myMod.Npcs.DuplicateInAsNewRecord(sourceNpc);
    ```
=== "Specific FormKey"
    ``` { .cs hl_lines=5 }
    INpcGetter sourceNpc = ...;
    ISkyrimMod myMod = ...;
    FormKey formKey = ...;

    var copy = myMod.Npcs.DuplicateInAsNewRecord(sourceNpc, formKey);
    ```
=== "Synced"
    ``` { .cs hl_lines=5 }
    INpcGetter sourceNpc = ...;
    ISkyrimMod myMod = ...;

    var copy = myMod.Npcs.DuplicateInAsNewRecord(sourceNpc, "SomeUniqueEditorID");
    ```

    !!! Bug "Extra Requirements"
        If the supplied EditorID is not unique, it will throw an exception.

        [:octicons-arrow-right-24: FormID Allocation](FormKey-Allocation-and-Persistence.md#keep-editorids-unique)

#### Duplicate + Add Pattern
For complex nested records or when you need more control, use the `Duplicate()` method followed by `Add()`:

```csharp
INpcGetter sourceNpc = ...;
ISkyrimMod myMod = ...;

// Create a duplicate with a new FormKey
var npcCopy = sourceNpc.Duplicate(myMod.GetNextFormKey());

// Optionally modify the duplicate
npcCopy.Name = "Variant Name";

// Add it to your mod
myMod.Npcs.Add(npcCopy);
```

This pattern is necessary for nested records like Placed Objects or Cells where `DuplicateInAsNewRecord` is not available.

#### FormLink Behavior

!!! warning "FormLinks Are Not Automatically Updated"
    When you duplicate a record, any FormLinks within that record still point to their original targets. They are **not** automatically updated to point to the new duplicated record.

    You must manually update FormLinks if needed. See the [Remapping FormLinks](Remapping-FormLinks.md) page for details on how to update FormLinks to point to duplicated records.



## Overriding Records
It is very common to want to modify a record that is from another mod.  This just entails making a copy of the original record and adding it to your output mod.  The fact that the FormKey doesn't originate from your output mod implicitly means that it's an override.

### GetOrAddAsOverride
The quick any easy way to override a record is to utilize a Group.GetOrAddAsOverride call.  This will check if the record already exists as an override in your group, and return it if so.  Otherwise, it will copy the source record to a new object, and add it to your mod for you.
```csharp
INpcGetter sourceNpc = ...;
// Retrieve or copy-and-add the record to our mod
var overrideRecord = myMod.Npcs.GetOrAddAsOverride(sourceNpc);
// Modify the name to be different inside myMod
overrideRecord.Name = "My New Name";
```

!!! tip "Call After Filters to Avoid ITPOs"
    With this pattern, it is preferable to call `GetOrAddAsOverride` as late as possible, after all filtering has been done and you know you want to override and modify the record.  This avoids adding records to your mod that have no changes.

### Deep Copy Then Insert
This pattern is an alternative to `GetOrAddAsOverride`.  It is sometimes preferable if it's hard to delay the `GetOrAddAsOverride` call, and you want more control over when a record actually gets added to the outgoing mod.

```csharp
INpcGetter sourceNpc = ...;
// Make a new mutable object that is a copy of the record to override
var npcCopy = sourceNpc.DeepCopy();

// Modify the name
npcCopy.Name = "My New Name";

// Apply some late filter logic
if (!SomeFilter(npcCopy)) return;

// Add the record as an override after the filter
outputMod.Npcs.Add(npcCopy);
```

This strategy works well if you might change your mind and not add the copied record to the outgoing mod.  It lets you get a mutable version of the record without adding it to your outgoing mod until you are certain you want to include it.

### Nested Records
Some records like Placed Objects and Cells are often nested underneath other records.  This makes it harder to follow the above patterns.  For these you will want to make use of [ModContext](../linkcache/ModContexts.md) concepts.

[:octicons-arrow-right-24: Mod Contexts](../linkcache/ModContexts.md)