# Big Cheat Sheet
## Overview
A massive list of code snippets without much contextual explaination.

### Target Game
Most code snippets will assume Skyrim SE, but code should work for any game, generally.

### Preparing the Examples
When `...` is seen, that generally means the example will not cover how that object might have been made.  `...` is not actually valid code to be copied and pasted.

### Missing Namespaces
If you're just copy pasting code, often it will not compile because some required namespaces are missing.  You can have the IDE import them by clicking on the red object in question and activating quick fixes (`Ctrl - .` in Visual Studio).

[:octicons-arrow-right-24: Namespaces](familiar/Namespaces.md)

## Construct an Environment

=== "Typical"
    ``` { .cs hl_lines="1" }
    using var env = GameEnvironment.Typical.Skyrim(SkyrimRelease.SkyrimSE);

    Console.WriteLine($"Data folder is: {state.DataFolderPath}");
    ```

[:octicons-arrow-right-24: Environments](environment/index.md)

## Retrieve a Mod From a Load Order
=== "Resolve"
    ``` cs
    var mod = myLoadOrder.ResolveMod("MyMod.esp");
    ```
=== "TryGet"
    ``` cs
    if (myLoadOrder.TryGetValue("MyMod.esp", out var mod))
    {
        // ..
    }
    ```

[:octicons-arrow-right-24: Mod Retrieval](loadorder/index.md#accessing-specific-listings)

## Basic Record Inspection and Override Example
``` cs
using var env = GameEnvironment.Typical.Skyrim(SkyrimRelease.SkyrimSE);

var outgoingPatch = new SkyrimMod(ModKey.FromFileName("MyPatch.esp"), SkyrimRelease.SkyrimSE);

foreach (var readonlyNpc in env.LoadOrder.PriorityOrder.Npc().WinningContextOverrides())
{
    // Readonly phase
    // Skip npc if health offset greater than 100
    if (readonlyNpc.Record.Configuration.HealthOffset < 100) continue;
    // Mutable phase
    var npc = readonlyNpc.GetOrAddAsOverride(outgoingPatch);
    // Set all lower health offsets to be at least 100
    npc.Configuration.HealthOffset = 100;
}

outgoingPatch.BeginWrite
    .ToPath(Path.Combine("SomePath", outgoingPatch.ModKey.FileName))
    .WithLoadOrder(env.LoadOrder)
    .Write();
```

[:octicons-arrow-right-24: Mod Retrieval](loadorder/index.md#accessing-specific-listings)

## Construct a ModKey
=== "FromFileName"
    ``` cs
    var modKey = ModKey.FromFileName("Skyrim.esm");
    ```
=== "TryFromFileName"
    ``` cs
    if (ModKey.TryFromFileName("Skyrim.esm", out var modKey))
    {
       // If conversion successful.
    }
    else
    {
       // An unsuccessful conversion.
       // Might occur if there was an extension typo, like "Skyrim.esz"
    }
    ```
=== "New"
    ``` cs
    var modKey = new ModKey("Skyrim", ModType.Plugin);
    ```

[:octicons-arrow-right-24: ModKeys](plugins/ModKey, FormKey, FormLink.md#modkey)

## Get List of Masters From A Mod
=== "MasterReferenceCollection"
    ``` { .cs hl_lines="1" }
    var masterCollection = MasterReferenceCollection.FromPath(pathToMod, GameRelease.SkyrimSE);

    Console.WriteLine($"The mod {masterCollection.CurrentMod} has masters:");
    foreach (var master in masterCollection.Masters)
    {
        Console.WriteLine($"  {master.Master.FileName}");
    }
    ```
=== "Mod Object"
    ``` { .cs hl_lines="1" }
    using var mod = SkyrimMod.CreateFromBinaryOverlay(pathToMod, SkyrimRelease.SkyrimSE);

    Console.WriteLine($"The mod {mod.ModKey} has masters:");
    foreach (var master in mod.ModHeader.MasterReferences)
    {
        Console.WriteLine($"  {master.Master.FileName}");
    }
    ```

## Look Up a Record
``` { .cs hl_lines=4 }
ILinkCache linkCache = ...;
var formLink = new FormLink<IFormListGetter>(FormKey.Factory("123456:Skyrim.esm"));

if (formLink.TryResolve(linkCache, out var foundRecord))
{
    // Use the specific record we were looking for
}
```
	
[:octicons-arrow-right-24: Link Caches](linkcache/index.md)

!!! tip "Avoid Hand Writing FormKeys"
    Using the [FormKey Generator](https://github.com/Mutagen-Modding/Mutagen.Bethesda.FormKeys) project is a good alternative to hand constructing FormLinks"

## Convert FormKey to FormID
=== MasterReferenceCollection
    ``` { .cs hl_lines="4" }
    FormKey formKey = ...;
    IMasterReferenceCollection masterCollection = ...;
    
    FormID formID = masterCollection.GetFormID(formKey);
    ```

[:octicons-arrow-right-24: FormKeys](plugins/ModKey, FormKey, FormLink.md#formkey)

## Convert FormKey to FormLink
``` { .cs hl_lines="3" }
FormKey formKey = ...;
// NOTE: Typically want to use the "getter" interfaces for FormLinks
var npcLink = formKey.ToLink<INpcGetter>();
```

[:octicons-arrow-right-24: FormLinks](plugins/ModKey, FormKey, FormLink.md#formlink)

## Check if FormLink is Pointing to a Null record
``` { .cs hl_lines="2" }
Npc npc = ...;
if (!npc.Race.IsNull)
{
   // ...
}
```

[:octicons-arrow-right-24: FormLink Nullability](best-practices/FormLink-Nullability.md/#checking-if-formlink-is-null)

## Convert FormLink to NullableFormLink
=== SetTo
    ``` { .cs hl_lines="4" }
    IFormLinkGetter<IEquipTypeGetter> link = ...;
    IFormLinkNullableGetter<IEquipTypeGetter> nullableLink = ...;
    
    nullableLink.SetTo(link);
    ```
=== AsNullable
    ``` { .cs hl_lines="3" }
    IFormLinkGetter<IEquipTypeGetter> link = ...;
	
    IFormLinkNullableGetter<IEquipTypeGetter> nullableLink = link.AsNullable();
    ```

[:octicons-arrow-right-24: FormLinks](plugins/ModKey, FormKey, FormLink.md#formlink)

## Convert MajorRecord to FormLink
``` cs
INpcGetter npcGetter = ...;
var npcLink = npcGetter.ToLink();
```

??? tip "Always use Getter interfaces"

    If the source major record type is not a getter interface it is recommended to take extra steps to keep the link targeting the getter interface

    ``` { .cs hl_lines="2 4" }
    INpc npcSetter = ...;
    var npcLink2 = npcSetter.ToLink<INpcGetter>();
    // or
    IFormLinkGetter<INpcGetter> npcLink3 = npcSetter.ToLink();
    ```
	
    [:octicons-arrow-right-24: Use Getter Interfaces in FormLinks](best-practices/FormLinks-Target-Getter-Interfaces.md)
	
## Iterate Winning Overrides
=== "Normal Record"
    ``` { .cs hl_lines=3 }
    ILoadOrder<ISkyrimMod, ISkyrimModGetter> loadOrder = ...;
    
    foreach (var keywordGetter in loadOrder.PriorityOrder.Keywords().WinningOverrides())
    {
       // Process each keyword record's winning override
    }
    ```

=== "Mod Context"
    ``` { .cs hl_lines=3 }
    ILoadOrder<ISkyrimMod, ISkyrimModGetter> loadOrder = ...;
    ILinkCache linkCache = ...;
    
    foreach (var cellContext in loadOrder.PriorityOrder.Cell().WinningContextOverrides(linkCache))
    {
       // Process each cell record's winning override
    }
    ```

[:octicons-arrow-right-24: Winning Overrides](loadorder/Winning-Overrides.md)

## Iterate Original Definitions
``` cs
foreach (var keywordGetter in loadOrder.ListedOrder.Keywords().WinningOverrides())
{
   // Process each keyword record's original definition
}
```

??? Reasoning
    By swapping to ListedOrder, the loop will now iterate over the original definitions of each record.  By viewing the load order "backwards", is sees the original mods as the winning override to return

## Override a Nested Record
=== "Specific Cell"
    ```cs
    FormKey someFormKey = ...;
    ILinkCache linkCache = ...;
    ISkyrimMod outgoingMod = ...;
	
    var cellContext = linkCache.ResolveContext<ICell, ICellGetter>(someFormKey);
    var overrideCell = cellContext.GetOrAddAsOverride(outgoingMod);
    ```
=== "All Cells"
    ```cs
    ILoadOrder<ISkyrimMod, ISkyrimModGetter> loadOrder = ...;
    FormKey someFormKey = ...;
    ILinkCache linkCache = ...;
    ISkyrimMod outgoingMod = ...;
	
    foreach (var cellContext in loadOrder.PriorityOrder.Cell().WinningContextOverrides(linkCache))
    {
        var overrideCell = cellContext.GetOrAddAsOverride(outgoingMod);
    }
    ```

[:octicons-arrow-right-24: Winning Overrides](linkcache/ModContexts.md)

## Check If A FormLink Points to a Specific Record
=== "FormKey Mapping Library"
    ``` cs
    INpcGetter npc = ...;
    if (Skyrim.Race.KhajiitRace.Equals(npc.Race))
    {
    }
    ```
    [:octicons-arrow-right-24: FormLink Mapping](https://github.com/Mutagen-Modding/Mutagen.Bethesda.FormKeys)
	
=== "By FormLink"
    ``` cs
    INpcGetter npc = ...;
    IFormLinkGetter<INpcGetter> formLink = ...;
    if (target.Equals(npc.Race))
    {
    }
    ```
    [:octicons-arrow-right-24: FormLinks](plugins/ModKey, FormKey, FormLink.md#formlink)
	
=== "By FormKey"
    ``` cs
    INpcGetter npc = ...;
    FormKey formKey = ...;
    if (formKey.Equals(npc.Race))
    {
    }
    ```
    [:octicons-arrow-right-24: FormKeys](plugins/ModKey, FormKey, FormLink.md#formkey)


## Duplicate a Record
Copy an existing record with a new FormKey
=== "Same Source Mod"
    ``` cs
    var dup = someRecord.Duplicate(newFormKey);
    ```
=== "From Target Mod"
    === "Next Available FormKey"
        ``` cs { .cs hl_lines="2" }
		Npc someRecord = ...;
        var dup = someMod.Npcs.DuplicateInAsNewRecord(someRecord);
        ```
    === "Specific FormKey"
        ``` { .cs hl_lines="2" }
		FormKey desiredFormKey = ...;
        var dup2 = someMod.Npcs.DuplicateInAsNewRecord(desiredFormKey);
        ```
    === "Persistence Support"
        ``` { .cs hl_lines="2" }
		string someUniqueEditorId = ...;
        var dup3 = someMod.Npcs.DuplicateInAsNewRecord(someUniqueEditorId);
        ```
		
		[:octicons-arrow-right-24: FormKey Persistence](plugins/FormKey-Allocation-and-Persistence.md)

[:octicons-arrow-right-24: Duplication](plugins/Create,-Duplicate,-and-Override.md#by-duplication)

## Detect if Given Plugin is the Winning Override for a Specific Record
There might be several ways to accomplish this, depending on the gritty situation, but here is one route:
``` cs
// Use the link cache to locate the winning record, with additional context
if (formLinkOfRecordOfInterest.TryResolveSimpleContext(someLinkCache, out var context))
{
    // The context's ModKey will be from the record that contained it
    if (context.ModKey == givenPlugin.ModKey)
    {
        // givenPlugin was the winningmost override for the record!
    }
    else
    {
        // Some other mod is the winningmost override for this record:  context.ModKey will have which one that is
    }
}
```

## Find all Major Record Types
``` cs
foreach (var recTypes in MajorRecordTypeEnumerator.GetMajorRecordTypesFor(GameCategory.Skyrim))
{
    Console.WriteLine($"Getter: {recTypes.GetterType}");
    Console.WriteLine($"Setter: {recTypes.SetterType}");
    Console.WriteLine($"Class: {recTypes.ClassType}");
}
```

## Find all Major Record Types for Top Level Groups
```cs
foreach (var recTypes in MajorRecordTypeEnumerator.GetTopLevelMajorRecordTypesFor(GameCategory.Skyrim))
{
    Console.WriteLine($"Getter: {recTypes.GetterType}");
    Console.WriteLine($"Setter: {recTypes.SetterType}");
    Console.WriteLine($"Class: {recTypes.ClassType}");
}
```

## Enrich Exceptions
```cs
var majorRecordContext = ...;
try
{
    // Access majorRecordContext and potentially throw
}
catch (Exception e)
{
    throw RecordException.Enrich(e, majorRecordContext);
}
```

[:octicons-arrow-right-24: Exception Enrichment](best-practices/Enrich-Exceptions.md)

## Detect if PlacedObject is inside Worldspace
```cs
var loadOrder = ...;
var linkCache = ...;

foreach (var placedObjectContext in loadOrder.PriorityOrder.PlacedObject().WinningContextOverrides(linkCache))
{
    Console.WriteLine($"Checking placed object: {placedObjectContext.Record}");
    if (placedObjectContext.TryGetParent<IWorldspaceGetter>(out var worldspace))
    {
        Console.WriteLine($"Was in worldspace: {worldspace}");
    }
}
```

[:octicons-arrow-right-24: Mod Context Parents](linkcache/ModContexts.md/#parent-concepts)

## Call Generic Function by Mod Type
```cs
public class MyClass
{
    public void DoSomeThings(IMod mod)
    {
        ModToGenericCallHelper.InvokeFromCategory(
            this,
            mod.GameRelease.ToCategory(),
            typeof(MyClass).GetMethod(nameof(DoSomeThingsGeneric), BindingFlags.NonPublic | BindingFlags.Instance)!,
            new object[] { mod });
    }

    private void DoSomeThingsGeneric<TMod, TModGetter>(TMod mod)
        where TModGetter : IModGetter
        where TMod : IMod, TModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
    {
        // Actual logic
    }
}
```

[:octicons-arrow-right-24: Common to Generic Crossover](plugins/other-utility.md#common-to-generic-crossover)