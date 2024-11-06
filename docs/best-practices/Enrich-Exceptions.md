# Exception Enrichment

Mutagen's code is written in a lightweight way that means its exceptions are not fully filled out with all the extra information that might be interesting.   Often, an exception will just print the very specific field that failed, but not include other important details such as:

- FormKey of the record it was from
- ModKey of the mod it was from

It is recommended that you wrap access code in a try/catch that enriches the exception with that extra information. 

## RecordException Enrichment

This enriches an exception relative to a major record's information

```cs
foreach (var npc in state.LoadOrder.PriorityOrder.Npc().WinningOverrides())
{
    try
    {
        var overrideNpc = state.PatchMod.Npcs.GetOrAddAsOverride(npc);
        overrideNpc.Height *= 1.3f;
    }
    catch (Exception e)
    {
        throw RecordException.Enrich(e, npc);
    }
}
```

This will make the exception include:

- EditorID
- RecordType (NPC_)
- FormKey

### ModKey Inclusion
The above code will -NOT- include `ModKey`.  The ModKey that the record override originated from cannot be inferred automatically and so must be passed in.   The above call has a `modKey` parameter that you can pass this information to if you have it.

More than likely, though, the best way to do this is to use [ModContexts](../linkcache/ModContexts.md), which contain the information about what Mod the record originated from.
```cs
foreach (var npcContext in state.LoadOrder.PriorityOrder.Npc().WinningContextOverrides())
{
    try
    {
        var overrideNpc = npcContext.GetOrAddAsOverride(state.PatchMod);
        overrideNpc.Height *= 1.3f;
    }
    catch (Exception e)
    {
        throw RecordException.Enrich(e, npcContext);
    }
}
```

## Subrecord Exception

This is an even more specialized version of RecordException that also includes the Subrecord type.  Typically this is just used by the Mutagen engine itself, and not applicable to user code.