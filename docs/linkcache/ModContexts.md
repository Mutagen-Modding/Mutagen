# Mod Contexts
Mod Contexts are an opt-in advanced alternative of most LinkCache functionality.  They provide extra contextual information needed to add additional useful features.

A `ModContext` contains:

- `Record` - The record itself
- `ModKey` - The `ModKey` that the associated record came from.  Not where it was originally defined and declared, but rather what mod contained the version of the record as it is. (usually the winning override mod)
- `Parent` - If dealing with a "nested" record like `PlacedObject`, this will contain a reference to the parent record (like a `Cell`).

## Retrieving a ModContext

Usually you will be provided a state from which ModContexts can be retrieved

[:octicons-arrow-right-24: Environments](../environment/index.md)

### By Looping WinningOverrides

=== "Top Level"
    ``` { .cs hl_lines=1 }
    foreach (var context in environment.LoadOrder.PriorityOrder.Npc().WinningContextOverrides())
    {
        Console.WriteLine($"Found npc {context.Record.FormKey} {context.Record.EditorID}, which was found in mod {context.ModKey}");
    }
    ```
=== "Nested"
    ``` { .cs hl_lines=1 }
    foreach (var context in environment.LoadOrder.PriorityOrder.PlacedObject().WinningContextOverrides(environment.LinkCache))
    {
        Console.WriteLine($"Found npc {context.Record.FormKey} {context.Record.EditorID}, which was found in mod {context.ModKey}");
    }
    ```

    !!! info "LinkCache Required"
        Nested records require a LinkCache in case they need to interact with other records


### By LinkCache Resolves

=== "TryGet"
    ``` { .cs hl_lines=3 }
    IFormLinkGetter<INpcGetter> myFormLink = ...;

    if (myFormLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(environment.LinkCache, out var context))
    {
        Console.WriteLine($"Found npc {context.Record.FormKey} {context.Record.EditorID}, which was found in mod {context.ModKey}");
    }
    ```

=== "Get"
    ``` { .cs hl_lines=3 }
    IFormLinkGetter<INpcGetter> myFormLink = ...;

    var context = myFormLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(environment.LinkCache);
    Console.WriteLine($"Found npc {context.Record.FormKey} {context.Record.EditorID}, which was found in mod {context.ModKey}");
    ```

!!! info "Complex Generics"
    This API call requires a lot of generic information.  More on that [here](#complex-call-signature).

#### Simple Contexts
If the generic types are too demanding and you only are interested in non-mutating operations, then you can just request "simple" contexts.

=== "TryGet"
    ``` { .cs hl_lines=3 }
    IFormLinkGetter<INpcGetter> myFormLink = ...;

    if (myFormLink.TryResolveSimpleContext(environment.LinkCache, out var context))
    {
        Console.WriteLine($"Found npc {context.Record.FormKey} {context.Record.EditorID}, which was found in mod {context.ModKey}");
    }
    ```

=== "Get"
    ``` { .cs hl_lines=3 }
    IFormLinkGetter<INpcGetter> myFormLink = ...;

    var context = myFormLink.ResolveSimpleContext(environment.LinkCache);
    Console.WriteLine($"Found npc {context.Record.FormKey} {context.Record.EditorID}, which was found in mod {context.ModKey}");
    ```

## Features
### Containing ModKey
A ModContext can give you the ModKey of the mod file that contained the record. 

``` { .cs }
var modContext = ...;

Console.WriteLine($"The mod that contained this record: {modContext.ModKey}");
Console.WriteLine($"The mod that originally defined the record: {modContext.Record.FormKey.ModKey}");
```

!!! warning "ModKey Interpretation Confusion"
    Be aware that the ModKey from a ModContext is the mod that contained the version of the record contained.  This is a separate concept than the ModKey that originated the record initially, which is still accessible through the FormKey

### Overridding Deep Records
Records like PlacedObjects are hard to override, as they are nested underneath both Worldspace and Cell records, which must also be overridden in the process.   ModContexts abstract this away so that the call is a consistent single call, no matter if it is nested or not.

``` { .cs }
var modContext = ...;
ISkyrimMod outgoingMod = ...;

var overrideRecord = modContext.GetOrAddAsOverride(outgoingMod);
```

### Parent Concepts
A ModContext has a reference to the parent record in nested scenarios, such as PlacedObject parent Cell

#### TryGetParent
=== "TryGet"
    ``` { .cs hl_lines=3 }
    var modContext = ...;

    if (modContext.TryGetParent<ICellGetter>(out var cell))
    {
        Console.WriteLine($"The parent record was a cell: {cell.FormKey}");
    }
    ```
=== "TryGetContext"
    ``` { .cs hl_lines=3 }
    var modContext = ...;

    if (modContext.TryGetParentContext<ICell, ICellGetter>(out var cell))
    {
        Console.WriteLine($"The parent record was a cell: {cell.FormKey}");
    }
    ```
=== "TryGetSimpleContext"
    ``` { .cs hl_lines=3 }
    var modContext = ...;

    if (modContext.TryGetParentSimpleContext<ICellGetter>(out var cell))
    {
        Console.WriteLine($"The parent record was a cell: {cell.FormKey}");
    }
    ```

!!! tip "Recursive"
    These calls are recursive, and do not need to be the first parent to succeed.  A PlacedObject context can retrieve the parent Worldspace in one call.

#### Parent Direct Access
``` { .cs hl_lines=3 }
var modContext = ...;

if (modContext.Parent.Record is ICellGetter cell)
{
    Console.WriteLine($"The parent record was a cell: {cell.FormKey}");
}
```

!!! info "Typeless"
    The type that the parent is expected to be is not contained, so you must query for the correct object type that you expect.   It also must be the exact type that the parent is in order to match.

## Complex Call Signature
Mod contexts require a lot of information about the types involved:

- Mod Type
- Record Type
- Setter and Getter variants

In some circumstances, these types can be inferred, but sometimes when this isn't the case, then all generics need to be provided.