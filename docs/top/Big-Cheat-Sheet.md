# Big Cheat Sheet
## Overview
A massive list of code snippets without much contextual explaination.

### Related Reading
Relevant wikis for more detailed reading will be linked.

### Target Game
Most code snippets will assume Skyrim SE, but code should work for any game, generally.

### Preparing the Examples
When `...` is seen, that generally means the example will not cover how that object might have been made.  `...` is not actually valid code to be copied and pasted.

### Missing Namespaces
If you're just copy pasting code, often it will not compile because some required namespaces are missing.  You can have the IDE import them by clicking on the red object in question and activating quick fixes (`Ctrl - .` in Visual Studio).

[More Reading](https://github.com/Mutagen-Modding/Mutagen/wiki/Namespaces)

## Construct an Environment
```cs
using var env = GameEnvironment.Typical.Skyrim(SkyrimRelease.SkyrimSE);

Console.WriteLine($"Data folder is: {state.DataFolderPath}");
Console.WriteLine($"Load order is:");
foreach (var listing in state.LoadOrder.ListedOrder)
{
    Console.WriteLine($"  {listing}");
}
```

[Read About Environments](https://github.com/Mutagen-Modding/Mutagen/wiki/Environment)

## Retrieve a Mod From a Load Order
```cs
var mod = myLoadOrder["MyMod.esp"];
```
or TryGet alternative
```cs
if (myLoadOrder.TryGetValue("MyMod.esp", out var mod))
{
    // ..
}
```

[Read About Mod Retrieval](https://github.com/Mutagen-Modding/Mutagen/wiki/Load-Order#accessing-specific-listings)

## Construct a ModKey
```cs
var modKey = ModKey.FromFileName("Skyrim.esm");
modKey = new ModKey("Skyrim", ModType.Plugin);
```
[Read About ModKeys](https://github.com/Mutagen-Modding/Mutagen/wiki/ModKey%2C-FormKey%2C-FormLink#modkey)

## Get List of Masters From A Mod
### Via MasterReferenceCollection
```cs
var masterCollection = MasterReferenceCollection.FromPath(pathToMod, GameRelease.SkyrimSE);

Console.WriteLine($"The mod {masterCollection.CurrentMod} has masters:");
foreach (var master in masterCollection.Masters)
{
    Console.WriteLine($"  {master.Master.FileName}");
}
```
### Via Mod Object
```cs
using var mod = SkyrimMod.CreateFromBinaryOverlay(pathToMod, SkyrimRelease.SkyrimSE);

Console.WriteLine($"The mod {mod.ModKey} has masters:");
foreach (var master in mod.ModHeader.MasterReferences)
{
    Console.WriteLine($"  {master.Master.FileName}");
}
```

## Get Access to Record Data
If you don't know what records you want to process, and just want to process everything the load order has, then this is typical:
```
foreach (var formList in loadOrder.PriorityOrder.FormList.WinningOverrides())
{
    // each form list that exists, do something
}
```
If you know ahead of time the ones you want to look for and process, then it's more typical to do this:

```
if (state.LinkCache.TryResolve<IFormListGetter>(FormKey.Factory("123456:Skyrim.esm"), out var foundRecord))
{
    // Found the specific record we were looking for
}
```
Using the [FormKey Generator](https://github.com/Mutagen-Modding/Mutagen.Bethesda.FormKeys) project output is better than hand writing, in this case.

## Convert FormKey to FormID
### Via MasterReferenceCollection
```cs
FormKey formKey = ...;
IMasterReferenceCollection masterCollection = ...;

FormID formID = masterCollection.GetFormID(formKey);
```

[Read About FormKeys](https://github.com/Mutagen-Modding/Mutagen/wiki/ModKey%2C-FormKey%2C-FormLink#formkey)

## Convert FormKey to FormLink
```cs
FormKey formKey = ...;
// NOTE: Typically want to use the "getter" interfaces for FormLinks
var npcLink = formKey.ToLink<INpcGetter>();
```

[Read About FormKeys](https://github.com/Mutagen-Modding/Mutagen/wiki/ModKey%2C-FormKey%2C-FormLink#formkey)

## Convert MajorRecord to FormLink
```cs
INpcGetter npcGetter = ...;
var npcLink = npcGetter.ToLink();

// If the source major record type is not a getter interface
// it is recommended to take extra steps to keep the link targeting the getter interface
INpc npcSetter = ...;
var npcLink2 = npcSetter.ToLink<INpcGetter>();
// or
IFormLinkGetter<INpcGetter> npcLink3 = npcSetter.ToLink();
```

## Iterate Winning Overrides
```cs
// Load order is typically retrieved via Synthesis state or Environment systems
foreach (var keywordGetter in loadOrder.PriorityOrder.Keywords().WinningOverrides())
{
   // Process each keyword record's winning override
}
```

## Iterate Records' Original Definitions
```cs
// By swapping to ListedOrder, the loop will now iterate over the original definitions of each record
// By viewing the load order "backwards", is sees the original mods as the winning override to return
foreach (var keywordGetter in loadOrder.ListedOrder.Keywords().WinningOverrides())
{
   // Process each keyword record's original definition
}
```

## Check If A FormLink Points to a Specific Record
### By FormKey
```cs
INpcGetter npc = ...;
var target = FormKey.Factory("013745:Skyrim.esm"); // Khajiit Race
if (target.FormKey.Equals(npc.Race))
{
}
```

[Read About FormKeys](https://github.com/Mutagen-Modding/Mutagen/wiki/ModKey%2C-FormKey%2C-FormLink#formkey)

### By FormLink
```cs
INpcGetter npc = ...;
var targetFormLink = FormKey
   .Factory("013745:Skyrim.esm") // Khajiit Race
   .AsLink<IRaceGetter>(); // Convert to a FormLink for more type safety
if (target.Equals(npc.Race))
{
}
```

[Read About FormLinks](https://github.com/Mutagen-Modding/Mutagen/wiki/ModKey%2C-FormKey%2C-FormLink#formlink)

### Using FormKey Mapping Library
```cs
INpcGetter npc = ...;
if (Skyrim.Race.KhajiitRace.Equals(npc.Race))
{
}
```

[Read About FormLink Mapping](https://github.com/Mutagen-Modding/Mutagen.Bethesda.FormKeys)

## Duplicate a Record
_Copy an existing record with a new FormKey_
```cs
var dup = someRecord.Duplicate(newFormKey);
```
```cs
// Assign the next available FormKey the mod has
var dup = someMod.Npcs.DuplicateInAsNewRecord();
// Assign a specific FormKey
var dup2 = someMod.Npcs.DuplicateInAsNewRecord(newFormKey);
// Hook into the FormID persistence system
var dup3 = someMod.Npcs.DuplicateInAsNewRecord(someEditorId);
```

[Read more about duplication](https://github.com/Mutagen-Modding/Mutagen/wiki/Create%2C-Duplicate%2C-and-Override#by-duplication)
[Read more about FormKey Persistence](https://github.com/Mutagen-Modding/Mutagen/wiki/FormKey-Allocation-and-Persistence)
