# Previous Override Iteration
LinkCache offers an easy way to dig deeper past winning overrides into the load order and access the non-winning versions of records from previous mods.

Some important concepts to consider when Resolving:

[:octicons-arrow-right-24: Scoping Type](Scoping-Type.md)

[:octicons-arrow-right-24: Resolve Target](index.md#resolve-target)

## ResolveAll
This call will loop over all versions of a record.  If given a ResolveTarget, it will start looping at the end given.  For example, ResolveTarget.Winning will return the winning record first, and then loop all the way to the originating version last.

=== "By FormLink"
    ``` { .cs hl_lines=4 }
    IFormLinkGetter<INpcGetter> myLink = ...;
    ILinkCache myLinkCache = ...;

    foreach (var record in myLink.ResolveAll(myLinkCache))
    {
        Console.WriteLine($"EditorID is {record.EditorID}");
    }
    ```

=== "By LinkCache"
    ``` { .cs hl_lines=4 }
    IFormLinkGetter<INpcGetter> myLink = ...;
    ILinkCache myLinkCache = ...;

    foreach (var record in myLink.ResolveAll(myLinkCache))
    {
        Console.WriteLine($"EditorID is {record.EditorID}");
    }
    ```

!!! tip "Context Variants Preferred"
    This call only returns the records themselves.  The context variants will be able to inform you on where the record came from

## ResolveAllContexts
`ResolveAllContexts` is an alternative that returns [ModContext](ModContexts.md) objects instead, which have a lot more information/tooling about where a record came from.

```cs
IFormLinkGetter<INpcGetter> formLink = ...;

foreach (var npcRecordContext in npcLink.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(myLinkCache))
{
    Console.WriteLine($"The Npc's EditorID is {npcRecordContext.Record.EditorID} in mod {npcRecordContext.ModKey}");
}
```

Refer to the Mod Contexts documentation for more information about type requirements on the call

[:octicons-arrow-right-24: Mod Contexts](ModContexts.md)

## Best Practices
It is important to only loop over what you need.  This allows the Link Cache to only parse as deep into the Load Order as it needs.

[:octicons-arrow-right-24: Enumerable Laziness](../best-practices/Enumerable-Laziness.md)

```cs
// Break out of your loops if you're done
foreach (var npcRecord in npcLink.ResolveAll(myLinkCache))
{
    if (HasWhatINeed(npcRecord))
    {
        // Stop looping
        break;
    }
}

// Or only take what you're interested in
INpcGetter[] recordWithPreviousOverride = npcLink.ResolveAll(myLinkCache)
    // This limits the looping to two levels deep, at most
    .Take(2)
    // Solidifies the results into an array for reuse
    .ToArray();

// Got the winning record
var winningRecord = recordWithPreviousOverride[0];
if (recordWithPreviousOverride.Length > 1)
{
    // And it has a previous override, too
    var previousOverride = recordWithPreviousOverride[1];
}
```
