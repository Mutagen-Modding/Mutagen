# Translation Masks
Several functionalities such as Equality, DeepCopy, and a few others have support for a concept called Translation Masks.  These allow for customization of what members are involved in those operations.

## DeepCopy(In)

```cs
var mask = new Npc.TranslationMask(defaultOn: false)
{
    Height = true,
    Weight = true,
    Destructible = true,
};

npc.DeepCopyIn(npc2, mask);
```

The above would copy in the values of the `Height`, `Weight`, and `Destructible` subobject from `npc2` into `npc`.  All other members of `npc` would be left untouched.

[:octicons-arrow-right-24: Copy Functionality](plugins/Copy-Functionality.md)

## Equality
Providing a translation mask to an Equality call will control what members are compared when determining equality.

```cs
var mask = new Npc.TranslationMask(defaultOn: false)
{
    Height = true,
    Weight = true,
};

npc.Equals(npc2, mask);
```

The above would compare equality of the Height and Weight of the two Npcs.  All other members would not be considered.

[:octicons-arrow-right-24: Equality](plugins/Translation-Masks.md)

!!! bug "Work in Progress"
    Equality functionality exists, but has not been thoroughly tested.  Bugs within may exist.  Please report if you see any.

## Translation Mask Construction
The above examples are very simple mask constructions.  They can get more complex when there are subobjects and/or lists involved.

### defaultOn Parameter 
When creating a mask, you can either give it `defaultOn: true` where all the fields will be marked `true` by default, and then you can selectively mark fields false as desired.  Inversely, you can give it `defaultOn: false` where all the fields will be marked `false` by default, and you'll need to turn specific fields `true` as needed.

=== "Copy Only One Field"
    ```cs
    // Only copy Height
    var mask = new Npc.TranslationMask(defaultOn: false)
    {
        Height = true,
    };
    ```

=== "Copy All But One Field"
    ```cs
    // Copy everything BUT Height
    var mask = new Npc.TranslationMask(defaultOn: true)
    {
        Height = false,
    };
    ```

### Subobjects
When a record has subobjects, the subobject field is itself a Translation mask letting you set members within that subobject
```cs
// Copy destructible subobject, but not its Stages field
var mask = new Npc.TranslationMask(defaultOn: true)
{
    Destructible = new Destructible.TranslationMask(true)
    {
        Stages = false
    }
};
```

#### Implicit Bool Conversion Shorthand
When dealing with subobjects, the above API can get a bit verbose.  For the basic situations of inclusion/exclusion, Translation Masks are implicitly convertible from `bool`s.  So you can do something like this:

```cs
rec.DeepCopy(new Npc.TranslationMask(defaultOn: false)
{
    // Boolean converts to a translation mask for you
    Destructible = true
    // Equivalent to
    // Destructible = new Destructible.TranslationMask(true)
});
```

This would copy over everything but the `Destructible` member, but is much easier to write

### onOverall Parameter
This one has a lot more nuance.  It relates to behavior for a nullable subobject, and gives the user more control as to how those are handled.

It does not have any effect if it is the top level mask.   It only has an effect if it's a submask used within another mask.

To help demonstrate its usage, let's take Skyrim Npc as a case study.  It has a simplified interface of:
```cs
public interface INpc
{
    // A name
    string? Name { get; set; }
    // A nullable subobject
    Destructible? Destructible { get; set; }
    // Other fields
}
```

Consider this:
```cs
rec.DeepCopy(new Npc.TranslationMask(true)
{
    Name = false,
    // Implicit:
    // Destructible = null
});
```
This is an easy way to specify that you want all fields copied except the name.  Destructible's submask object will be left null, but in this case, that means we want to copy it.  (we shouldn't have to define Destructible's subobject in order to bring it over)

Okay, but what about the case when we want to omit Destructible, then?  There's some confusion when you try to do this with nullable subobjects.  Does a false mean:

- Destructible should not be considered at all during equality
- All fields on the destructible mask should be false.
Those are two slightly different things.

The bool `onOverall` gives us the extra control to specify exactly what we want.

```cs
// This will skip Destructible from being considered during the copy altogether
rec.DeepCopy(new Npc.TranslationMask(true)
{
    Name = false,
    Destructible = new Destructible.TranslationMask(onOverall: false)
});
```
So whatever value the `rec.Destructible` had before, it will still have, as it will be not touched or considered at all.

This is in contrast to this:
```cs
rec.DeepCopy(new Npc.TranslationMask(true)
{
    Name = false,
    Destructible = new Destructible.TranslationMask(
        defaultOn: false, 
        // optional, but shown for clarity
        onOverall: true)
    {
        Stages = true
    }
});
```
This setup has `onOverall` of true, and so the mask will be applied and considered during the copy.
`defaultOn` is false, and so by default it would only copy over the `Stages` member of Destructible, as well as respecting the nullability.  If the source Npc had a null Destructible, the the target will get a null Destructible.

In summary:
`onOverall = false` is what we'd set if we did not want it to have any effects, and wanted `Destructible` to not be considered whatsoever.  
`onOverall = true` (default), is what we'd set if we wanted the mask to be applied, in which case `defaultOn` and any field settings would take effect during the application.

## Best Practices
### Create Once, Use Many
Usually you do not need to create a translation mask per call.  Rather, you can make the desired mask once ahead of time and reuse it for each equality call.
