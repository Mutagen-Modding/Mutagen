# ExtraData (Container Item Ownership)

ExtraData is a subrecord found on Container entries that stores ownership and condition information for items. The ownership data indicates who owns an item and under what conditions it can be taken.

The complexity comes from the fact that the binary format doesn't explicitly specify the owner type - it must be determined by looking up the FormKey in a LinkCache to see what record type it points to. This creates special handling requirements when reading mods without a complete load order.

## Owner Types

ExtraData uses abstract subclassing to represent different owner types:

- **`NpcOwner`** - When the owner is an NPC record
    - `Npc` - FormLink to the owning NPC
    - `Global` - Optional FormLink to a Global variable
- **`FactionOwner`** - When the owner is a Faction record
    - `Faction` - FormLink to the owning Faction
    - `RequiredRank` - Minimum faction rank required
- **`UntypedOwner`** - Fallback when the owner type cannot be determined
    - `OwnerData` - FormLink to any major record (the owner)
    - `VariableData` - FormLink to any major record (additional data)

## Construction

When creating ExtraData, you should know the owner type and use the appropriate subclass:

=== "NPC Owner"
    ```cs
    var container = mod.Containers.AddNew("MyContainer");
    var entry = new ContainerEntry
    {
        Item = new ContainerItem
        {
            Item = myItem.AsLink(),
            Count = 5
        },
        Data = new ExtraData
        {
            Owner = new NpcOwner
            {
                Npc = myNpc.AsLink(),
                Global = FormLink<IGlobalGetter>.Null
            },
            ItemCondition = 100f
        }
    };
    container.Items.Add(entry);
    ```

=== "Faction Owner"
    ```cs
    var container = mod.Containers.AddNew("MyContainer");
    var entry = new ContainerEntry
    {
        Item = new ContainerItem
        {
            Item = myItem.AsLink(),
            Count = 5
        },
        Data = new ExtraData
        {
            Owner = new FactionOwner
            {
                Faction = myFaction.AsLink(),
                RequiredRank = 2
            },
            ItemCondition = 100f
        }
    };
    container.Items.Add(entry);
    ```

## Reading

ExtraData presents a unique challenge: the binary format doesn't specify the owner type, so Mutagen must look up the FormKey in other mod files to determine whether it points to an NPC or Faction record. This means **ExtraData is one of the only records that requires a load order for full functionality**.

Mutagen tries its best to determine correct owner type (`NpcOwner` or `FactionOwner`). However, it can only do so when the owner record is within the same mod as the ExtraData record being inspected.  Otherwise it requires a load order at time of creation to locate it.  When no load order is available, Mutagen falls back to `UntypedOwner` to preserve the data.

Reading ExtraData uses a switch statement to handle all three possible owner types:

```cs
using var env = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter>(GameRelease.SkyrimSE)
    .WithLoadOrder(loadOrderFilePath)
    .Build();

foreach (var container in env.LoadOrder.PriorityOrder.Container().WinningOverrides())
{
    foreach (var entry in container.Items)
    {
        if (entry.Data?.Owner == null) continue;

        switch (entry.Data.Owner)
        {
            case NpcOwner npcOwner:
                Console.WriteLine($"NPC Owner: {npcOwner.Npc.FormKey}");
                if (!npcOwner.Global.IsNull)
                {
                    Console.WriteLine($"  Global condition: {npcOwner.Global.FormKey}");
                }
                break;

            case FactionOwner factionOwner:
                Console.WriteLine($"Faction Owner: {factionOwner.Faction.FormKey}");
                Console.WriteLine($"  Required rank: {factionOwner.RequiredRank}");
                break;

            case UntypedOwner untypedOwner:
                Console.WriteLine($"Unknown owner type: {untypedOwner.OwnerData.FormKey}");
                break;
        }
    }
}
```

!!! info "UntypedOwner Without Load Order"
    If you read a mod without providing a load order, and the owner references a record from another mod, you'll receive an `UntypedOwner` instead of the specific type. The FormKey data is still available in `OwnerData` and `VariableData` fields, but you won't know whether it's an NPC or Faction without looking it up in a LinkCache.
