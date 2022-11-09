A lot of Record data is exposed via flags and enums.  All of known enum types and their options are defined explicitly for strongly typed use.
### Normal Enum
Certain fields have a certain subset of valid options.  These are exposed as enums, where only one choice can be made.

For example, Oblivion's `Creature` has an enum that looks like this:
```cs
public enum CreatureTypeEnum
{
    Creature = 0,
    Daedra = 1,
    Undead = 2,
    Humanoid = 3,
    Horse = 4,
    Giant = 5,
}
```

As a user, you can set the desired value by referencing this enum
```cs
creature.CreatureType = CreatureTypeEnum.Undead;
```

You can also read the enum and do things like switch:
```cs
switch (creature.CreatureType)
{
    case CreatureType.Creature:
        break;
    case CreatureType.Daedra:
        break;
    case CreatureType.Undead:
        break;
    case CreatureType.Humanoid:
        break;
    case CreatureType.Horse:
        break;
    case CreatureType.Giant:
        break;
}
```

### Flags Enum
Certain fields are allowed to have several values, and so make use of C#'s [Flags] enum systems.

For example, Oblivion `NPC`'s flags look like this:
```cs
[Flags]
public enum NPCFlag
{
    Female = 0x000001,
    Essential = 0x000002,
    Respawn = 0x000008,
    AutoCalcStats = 0x000010,
    ...
}
```

This enum can then be used to specify several flags on an NPC:
```cs
// Mark the NPC an essential female
npc.Flags = NPCFlag.Female | NPCFlag.Essential;

// And then add that we also want it to auto calc stats
npc.Flags |= NPCFlag.AutoCalcStats;
```

You can read up on C# flag enums for more API tricks.