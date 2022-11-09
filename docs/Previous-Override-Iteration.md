A lot of functionality like [Record Lookup](Record-Lookup) or [Winning Overrides](Winning-Overrides) deals with the "winning" version of a record, as in the record as it exists in the last Mod to override it on the Load Order.

LinkCache offers an easy way to dig deeper into the load order and access the non-winning versions of records from previous mods.

# ResolveAll
While you can call it on a LinkCache directly, typically the preferred way to tap into this functionality is off a [FormLink](https://github.com/Mutagen-Modding/Mutagen/wiki/ModKey%2C-FormKey%2C-FormLink#formlink):
```cs
IFormLinkGetter<INpcGetter> npcLink = ...;

// Will loop over every version of that Npc
// starting from the winning override, and ending with its originating definition
foreach (INpcGetter npcRecord in npcLink.ResolveAll(myLinkCache))
{
    Console.WriteLine($"The Npc's EditorID is {npcRecord.EditorID}");
}
```

With this pattern, you can loop over and interact with every override a record has from within a load order.

# ResolveAllContexts
If you look at the above code snippet, it's not as useful as it could be.  It will print all the EditorIDs, but if anything interesting happened you would not be able to tell what mod was responsible.

This is because `ResolveAll` enumerates over records directly, and records do not have knowledge of what mod contained them.  They know what mod defined the record in the first place (as that's part of its FormKey), but that might not be the mod that originated the record's state being interacted with.

`ResolveAllContexts` is an alternative that returns [ModContext](ModContexts) objects instead, which have a lot more information/tooling about where a record came from.

```cs
IFormLinkGetter<INpcGetter> npcLink = ...;

foreach (var npcRecordContext in npcLink.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(myLinkCache))
{
    Console.WriteLine($"The Npc's EditorID is {npcRecordContext.Record.EditorID} in mod {npcRecordContext.ModKey}");
}
```

This will now print more interesting information, as we can now tell what mod made what change.

However, you will notice that the call is much more complex, and requires you specify a lot more details.  You can read about why [here](https://github.com/Mutagen-Modding/Mutagen/wiki/ModContexts#complex-call-signature).

# Lazy Enumeration
This is discussed in more detail [here](Enumerable-Laziness), but it is important to only loop over what you need.  This allows the Link Cache to only parse as deep into the Load Order as it needs.

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