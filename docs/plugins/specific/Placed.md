# Placed Objects

Cells contain "placed" records like objects, NPCs, traps, and other things that exist at a specific position in the game world. These records live inside a Cell's `Persistent` or `Temporary` lists and are one of the most commonly interacted with record types when writing patchers.

## The IPlaced Umbrella Interface

`IPlaced` is an **umbrella interface** that covers all the different types of things that can be placed inside a cell. A single cell can contain a mix of placed NPCs, placed objects, placed traps, and more all stored together in the same list typed as `IPlaced`.

The reason `IPlaced` is an interface rather than a base class is because the concrete types have very different record structures and inheritance hierarchies. A `PlacedNpc` and a `PlacedArrow` share some common placement data (position, enable parent, scripts, etc.), but are otherwise very different records. The interface unifies them under one type so cells can store them in a single collection.

### Concrete Types

The concrete types that implement `IPlaced` vary slightly by game, but in Skyrim for example:

| Type | Record Type | Description |
| --- | --- | --- |
| `PlacedObject` | REFR | A placed static, container, door, activator, etc. |
| `PlacedNpc` | ACHR | A placed NPC |
| `PlacedArrow` | PARW | A placed arrow projectile |
| `PlacedBarrier` | PBAR | A placed barrier |
| `PlacedBeam` | PBEA | A placed beam |
| `PlacedCone` | PCON | A placed cone projectile |
| `PlacedFlame` | PFLA | A placed flame projectile |
| `PlacedHazard` | PHZD | A placed hazard |
| `PlacedMissile` | PMIS | A placed missile |
| `PlacedTrap` | PGRE | A placed trap |

The trap-related types (`PlacedArrow`, `PlacedBarrier`, `PlacedBeam`, etc.) all inherit from a shared abstract class `APlacedTrap`.

### Sub-Interfaces

`IPlaced` also breaks down into two sub-interfaces that group the types differently:

- **`IPlacedSimple`** - `PlacedNpc`, `PlacedObject`
- **`IPlacedThing`** - `PlacedObject`, `APlacedTrap` (and its subclasses)

These can be useful if you only care about a subset of placed types.

## Switch Statements

Similar to [Abstract Subclassing](../Abstract-Subclassing.md), a cell's placed records are stored as the umbrella `IPlaced` type, so you'll use switch statements to handle specific kinds:

```cs
ICellGetter cell = ...;

foreach (IPlacedGetter placed in cell.Temporary)
{
    switch (placed)
    {
        case IPlacedObjectGetter placedObj:
            Console.WriteLine($"Object: {placedObj.FormKey} -> {placedObj.Base}");
            break;
        case IPlacedNpcGetter placedNpc:
            Console.WriteLine($"NPC: {placedNpc.FormKey} -> {placedNpc.Base}");
            break;
        case IAPlacedTrapGetter placedTrap:
            Console.WriteLine($"Trap: {placedTrap.FormKey}");
            break;
    }
}
```

## Filtering With LINQ

Since `Persistent` and `Temporary` are typed as `IReadOnlyList<IPlacedGetter>`, you can use LINQ's `.OfType<T>()` to iterate only a specific kind of placed record:

=== "Placed Objects Only"
    ```cs
    ICellGetter cell = ...;

    foreach (var placedObj in cell.Temporary
        .OfType<IPlacedObjectGetter>())
    {
        Console.WriteLine($"Object: {placedObj.Base}");
    }
    ```
=== "Placed NPCs Only"
    ```cs
    ICellGetter cell = ...;

    foreach (var placedNpc in cell.Persistent
        .OfType<IPlacedNpcGetter>())
    {
        Console.WriteLine($"NPC: {placedNpc.Base}");
    }
    ```
=== "All Traps"
    ```cs
    ICellGetter cell = ...;

    foreach (var trap in cell.Temporary
        .OfType<IAPlacedTrapGetter>())
    {
        Console.WriteLine($"Trap: {trap.FormKey}");
    }
    ```

## Winning Overrides

You can iterate winning placed records across an entire load order using the typed accessors:

```cs
ILoadOrder<ISkyrimMod, ISkyrimModGetter> loadOrder = ...;
ILinkCache linkCache = ...;

foreach (var placedContext in loadOrder.PriorityOrder.PlacedObject().WinningContextOverrides(linkCache))
{
    Console.WriteLine($"Placed Object: {placedContext.Record.FormKey}");
}
```

This works for `PlacedObject()`, `PlacedNpc()`, and all other placed types individually.

## Nesting and Mod Contexts

Placed records are **deeply nested** inside cells, which makes them prime candidates for [Mod Contexts](../../linkcache/ModContexts.md).

The full nesting hierarchy looks like:

```
Mod
└── Cells / Worldspaces
    └── Block
        └── SubBlock
            └── Cell
                ├── Persistent  (list of IPlaced)
                └── Temporary   (list of IPlaced)
```

Manually creating an override for a placed record would require you to recreate this entire hierarchy in your output mod. Mod Contexts handle all of this automatically with a single call:

=== "Override a Specific Placed Object"
    ```cs
    ILinkCache linkCache = ...;
    ISkyrimMod outgoingMod = ...;
    FormKey targetPlacedObject = ...;

    var context = linkCache.ResolveContext<IPlacedObject, IPlacedObjectGetter>(targetPlacedObject);
    var overridePlaced = context.GetOrAddAsOverride(outgoingMod);
    // The entire Cell -> SubBlock -> Block hierarchy is created for you
    ```
=== "Override All Placed NPCs"
    ```cs
    ILoadOrder<ISkyrimMod, ISkyrimModGetter> loadOrder = ...;
    ILinkCache linkCache = ...;
    ISkyrimMod outgoingMod = ...;

    foreach (var context in loadOrder.PriorityOrder.PlacedNpc().WinningContextOverrides(linkCache))
    {
        if (context.Record.Race.FormKey == someRaceFormKey)
        {
            var overrideNpc = context.GetOrAddAsOverride(outgoingMod);
            // Modify overrideNpc...
        }
    }
    ```

### Parent Detection

Mod Contexts also let you inspect a placed record's parent hierarchy, which is useful for determining whether something is in a worldspace or interior cell:

```cs
foreach (var placedContext in loadOrder.PriorityOrder.PlacedObject().WinningContextOverrides(linkCache))
{
    if (placedContext.TryGetParent<IWorldspaceGetter>(out var worldspace))
    {
        Console.WriteLine($"{placedContext.Record.FormKey} is in worldspace: {worldspace}");
    }
    else
    {
        Console.WriteLine($"{placedContext.Record.FormKey} is in an interior cell");
    }
}
```

## Disabling Placed Records

Mutagen provides an extension method for safely disabling placed records using the standard "undelete and disable" procedure:

```cs
IPlaced placed = ...;

placed.Disable();
```

This handles the details of setting the `InitiallyDisabled` flag, adjusting the Z position, and configuring the enable parent to keep the record properly disabled.
