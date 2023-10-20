Occasionally records will be classes that are abstract   

This article will go over the concepts of Abstract Subclassing through the lens of looking at a specific example found in Skyrim Npc.   Other records have similar but different concepts.  

Consider Skyrim's [NpcConfiguration records](https://github.com/Mutagen-Modding/Mutagen/blob/e1619008d45fb87da7b6a58acfd90553e319545c/Mutagen.Bethesda.Skyrim/Records/Major%20Records/Npc.xml#L72):

```cs
public interface INpcConfigurationGetter
{
    // ...
    short MagickaOffset { get; }
    short StaminaOffset { get; }
    // The field below is an abstract type
    IANpcLevelGetter Level { get; }
    // ...
}
```

## Why is it Needed?

Why is the `Level` field an odd abstract `ANpcLevel` object?  

The subclassing helps encapsulate some complexity while remaining consistent and type safe.

Consider that `NPC_`'s `Level` field is an integer normally.  But if you turn on the `PC Level Mult` flag, it suddenly acts as a float.   So how can Mutagen expose this in a type safe manner if the field can just change its type depending on a switch somewhere else?

Mutagen exposes this by using subclassing.  `ANpcLevel` has two implementing classes:

| Implementation Class   | Type  |
| ---    | --- |
| NpcLevel | integer |
| PcLevelMult | float |

These two alternatives allow the same field to contain different types.

## Setting an Abstract Subclass Member
You will notice Mutagen does not expose the `Pc Level Mult` flag.   Instead, you simultaneously control both the "mode" that the `Level` is in, as well as `Level`'s value by choosing the appropriate subclass.  

=== "Pc Level Mult Flag OFF"
    ```cs
    INpc n = ...

    // Setting to straight level, with the Pc level Mult flag "off"
    n.Configuration.Level = new NpcLevel()
    {
        Level = 10
    };
    ```
=== "Pc Level Mult Flag ON"
    ```cs
    INpc n = ...

    // Setting to Pc Level Mult flag "on", now with float capabilities
    n.Configuration.Level = new PcLevelMult()
    {
        Level = 2.1234f
    };
    ```

Now, it's very clear when `Level` is an integer, and when it is a float.  The flag's value and `Level`'s type are "bundled" as one choice, depending on which subclass you make.

## Reading an Abstract Subclass Member
Reading needs to respect/consider these subclasses in the same way.  One easy way to do this is using a C# type switch:
```cs
INpcGetter n = ...
switch (n.Configuration.Level)
{
    case INpcLevelGetter straightLevel:
        System.Console.WriteLine($"Npc level was {straightLevel.Level}");
        break;
    case IPcLevelMultGetter mult:
        System.Console.WriteLine($"Npc level multiplier was {mult.Level}");
        break;
    default:
        System.Console.WriteLine("Huh?");
        break;
}
```

!!! bug "Do Not Cast"
     It is not good practice to hard cast to the desired type
     ```cs
     INpcGetter n = ...

     // Bad code:
     int level = ((INpcLevelGetter)n.Configuration.Level).Level;
     ```
     This will break if you are processing an Npc with the `PC Level Mult` flag on, as the subobject won't be of type `INpcLevelGetter`.

## Summary
Abstract Subclassing is used when a concept is complex enough to warrant the need for extra control.  It can help with:

- Exposing one field as many types
- Bundling complex configurations into one atomic decision, so that there is no potential for invalid configurations.

In the above example, you will never accidentally deal with a `Level` that is of type `float` unless it is in `Pc Level Mult` mode, and vice versa.  That is not the biggest deal, but in many other situations the concepts/differences are more extreme.

## Other Records with Abstract Subclassing
Skyrim Npc is not the only record type that uses these concepts.  There are many other records that have the need for data structure to change depending on context, and they will use Abstract Subclassing to help expose that.

Some other examples include:
[Perk Effects](https://github.com/Mutagen-Modding/Mutagen/blob/e1619008d45fb87da7b6a58acfd90553e319545c/Mutagen.Bethesda.Skyrim/Records/Major%20Records/Perk.xml#L23)

These show a more extreme example where the fields that a Perk Effect contains vary widely depending on the Perk type.  The subclassing helps only expose the applicable fields for a given effect type.

[Magic Effects](https://github.com/Mutagen-Modding/Mutagen/blob/e1619008d45fb87da7b6a58acfd90553e319545c/Mutagen.Bethesda.Skyrim/Records/Major%20Records/MagicEffect.xml#L68)

An effect can reference many different types of records, where some effect types can point to records of type ABC, while another effect type can only point to records of type XYZ.  The subclassing again helps expose only the correct typing depending on the effect type you're dealing with.

## Documentation
Each subclassing situation is different and is trying to solve a different complexity specific to that record.  As things mature, documentation outlining each specific structure will probably be written.

[:octicons-arrow-right-24: Specific Records](specific/index.md)

Additionally, you can investigate the subclassing alternatives yourself without documents:

- Utilize Intellisense, and follow references in the IDE to see the classes and what they contain.
- Of importance:  The interfaces of these abstract classes contain comments of what options are available:
```cs
/// <summary>
/// Implemented by: [NpcLevel, PcLevelMult]
/// </summary>
public partial interface IANpcLevel
{
   // ...
}
```
This helps narrow down which types it can be so you know what to switch on and handle.

- Also, you can sometimes refer to the xmls that define the records, like the ones linked above.
