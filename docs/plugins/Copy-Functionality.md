# Copy Functionality
Mutagen provides functionality for copying in data to an already existing object.

## DeepCopy
**This call will create a new object, and copy all fields.**  This will be done in a "deep" fashion, where duplicate objects will be made for each subobject and their fields copied over as well.  No references to the original object or any subobjects from the original will exist in the object copied to.

This separates the new object from the original object so that modifying either has no effect on the other.

```cs
IPotionGetter potionSource = ...;
Potion potionCopy = (Potion)potionSource.DeepCopy();
```

Some things to note:

- FormKey will match the original source
- Changes to either object will not affect the other
- The new record will not be a part of any Mod or Group unless added explicity.
- The cast is required currently, but will hopefully be unnecessary eventually.

## DeepCopyIn
**This call will copy in all fields from a second object to an already existing object**

```cs
IPotionGetter potionSource = ...;
Potion potionCopy = mod.Potions.AddNew();
potionCopy.DeepCopyIn(potionSource);
```

Some things to note:

- `FormKey`s are immutable, and will never be changed even with a copy in.  If you want a second record with the original's `FormKey`, use DeepCopy instead.
- Changes to either object will not affect the other

## Translation Masks
As with many other translational tasks in Mutagen, Copy functionality comes with the option to provide Masks to control what will get copied.

```cs
var onlyScriptsMask = new Potion.TranslationMask(defaultOn: false)
{
    Script = true
};

potion.DeepCopyIn(otherPotion, onlyScriptsMask);
```
This code will replace `potion`'s script data with the values from `otherPotion`.  All other subrecords will remain untouched.

## CopyInFrom[Binary]
In addition to being able to copy in fields from another object via DeepCopyIn, you can also CopyIn from other serialization sources such as from a Binary file on the disk.

This mainly only applies to Mod objects, rather than individual Major Records.  It is also less granular than other translation sources, and only lets you mask per Group type.

```cs
OblivionMod mod = ...;
mod.CopyInFromBinary(
    path: "C:/SomePath/SomeMod.esp",
    importMask: new GroupMask(false)
    {
        NPCs = true
    });
```

This code will replace the `mod` object's NPC Group's contents with the NPC contents from the file on disk.
