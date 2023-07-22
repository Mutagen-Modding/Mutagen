# Winning Override Iteration
## Winning Overrides
It is very common task to retrieve the "winning override" version of each record.  These are the versions of each record as they exist in the mod with the highest priority, and will thus be what's used by the game when running.

There are extension methods to streamline this:
```csharp
LoadOrder<ModListing<ISkyrimModGetter>> loadOrder = ...;
foreach (var npc in loadOrder.PriorityOrder.Npc().WinningOverrides())
{
   // Code to process the winning override version each NPC that appears on the loadorder
   Console.WriteLine($"Processed {npc.EditorID}");
}
```

If you then want to take winning overrides and add them to your mod with some modifications, this topic is covered more in depth [here](../plugins/Create,-Duplicate,-and-Override.md#getoraddasoverride).

## Winning Context Overrides
The above loop will just give you each record in the game with it's "winning" content.  Sometimes more information is needed, though.

You can instead opt to iterate over [ModContexts](../linkcache/ModContexts.md) which is a wrapper object containing the record of interest PLUS other useful information and features.

```csharp
LoadOrder<ModListing<ISkyrimModGetter>> loadOrder = ...;
foreach (var npcContext in loadOrder.PriorityOrder.Npc().WinningContextOverrides())
{
   // Code to process the winning override version each NPC that appears on the loadorder
   Console.WriteLine($"Processed {npcContext.Record.EditorID} as defined in mod {npcContext.ModKey}");
}
```

You can read more about [ModContexts](../linkcache/ModContexts.md) to see all the features they offer.
