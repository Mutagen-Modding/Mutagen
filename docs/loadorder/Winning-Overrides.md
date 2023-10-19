# Winning Override Iteration
It is very common task to retrieve the "winning override" version of each record on a Load Order.  Winning Overrides are the versions of each record as they exist in the mod with the highest priority, and will thus be what's used by the game when running.

There are extension methods to streamline these operations.

!!! info "How To Override a Record"
    This page outlines how to see the winning overrides in an existing load order.  If you then want to modify them further with your own override, this topic is covered more in depth [here](../plugins/Create,-Duplicate,-and-Override.md#getoraddasoverride).

    [:octicons-arrow-right-24: Overriding Records](../plugins/Create,-Duplicate,-and-Override.md#getoraddasoverride)


## Winning Overrides
=== "By Load Order"
    ``` { .cs hl_lines=3 }
    LoadOrder<ModListing<ISkyrimModGetter>> loadOrder = ...;

    foreach (var npc in loadOrder.PriorityOrder.Npc().WinningOverrides())
    {
       Console.WriteLine($"Processed {npc.EditorID}");
    }
    ```

=== "Arbitrary Mods"
    ``` { .cs hl_lines=3 }
    IEnumerable<IModGetter> mods = ...;

    foreach (var npc in mods.WinningOverrides<INpcGetter>())
    {
       Console.WriteLine($"Processed {npc.EditorID}");
    }
    ```


## Winning Context Overrides
You can also iterate over Mod Context objects, which give more information and have more features that just looping the raw records.

[:octicons-arrow-right-24: Mod Contexts](../linkcache/ModContexts.md)

[:octicons-arrow-right-24: Link Caches](../linkcache/index.md)

=== "By Load Order"
    ``` { .cs hl_lines=4 }
    LoadOrder<ModListing<ISkyrimModGetter>> loadOrder = ...;
    ILinkCache linkCache = ...;

    foreach (var context in loadOrder.PriorityOrder.PlacedObjects().WinningContextOverrides(linkCache))
    {
       Console.WriteLine($"Processed {context.Record.EditorID}");
    }
    ```

=== "Arbitrary Mods"
    ``` { .cs hl_lines=4 }
    IEnumerable<ISkyrimModGetter> mods = ...;
    ILinkCache linkCache = ...;

    foreach (var context in mods.WinningOverrideContexts<ISkyrimMod, ISkyrimModGetter, IPlacedObject, IPlacedObjectGetter>(linkCache))
    {
       Console.WriteLine($"Processed {context.Record.EditorID}");
    }
    ```
