Mod Contexts are an opt-in advanced feature of most LinkCache functionality.  They act as storage for contextual information and the wiring and logic needed to perform certain actions in a context aware manner.

`ModContext`s contain:
- `Record` - The record itself
- `ModKey` - The `ModKey` that the associated record came from.  Not where it was originally defined and declared, but rather what mod contained the version of the record as it is. (usually the winning override mod)
- `Parent` - If dealing with a "nested" record like `PlacedObject`, this will contain a reference to the parent record (like a `Cell`).

ModContexts offer additional information as described above, but they also come with many useful features, which will be described in more detail below.

# Retrieving a ModContext
## By Looping WinningOverrides

## By LinkCache Lookups
A more typical [Record Lookup](Record-Lookup) call returns the record object directly:
```cs
IFormLinkGetter<INpcGetter> myFormLink = ...;

if (myFormLink.TryResolve(myLinkCache, out var record))
{
    Console.WriteLine($"Found npc {record.FormKey} {record.EditorID}, but I don't know what mod contained it");
}
```

This API cannot inform you about what mod contained the record returned.  It just gives you the winning override record, if found.

You can do a slightly different call to instead get a ModContext object, which contains the record object, but also a lot more information:
```cs
IFormLinkGetter<INpcGetter> myFormLink = ...;

if (myFormLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(myLinkCache, out var context))
{
    Console.WriteLine($"Found npc {context.Record.FormKey} {record.Record.EditorID}, which was found in mod {context.ModKey}");
}
```

Note:  The API call is much more verbose.  More on that [here](https://github.com/Mutagen-Modding/Mutagen/wiki/ModContexts#complex-call-signature).

# Parent Concepts

# Deep Record Insertion and Duplication

# Complex Call Signature
