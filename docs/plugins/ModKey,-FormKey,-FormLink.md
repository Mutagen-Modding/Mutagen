This article covers three fundamental identifiers:

- **[ModKey](#modkey)** _(a unique identifier for a mod)_
- **[FormKey](#formkey)** _(FormID)_
- **[FormLink](#formlink)** _(Typing added to a FormKey)_

# ModKey
`ModKey`s represent a unique identifier for a mod, as an alternative to a raw string.

They contain:

- `string` of a mod's name (without '.esp' or '.esm')
- An `enum` of whether it is a master, plugin, or light master

## Construction
### Basic Constructor
```cs
var modKey = new ModKey("Oblivion", type: ModType.Master);
```

### Factory Construction
```cs
if (ModKey.TryFromFileName("Oblivion.esm", out var modKey))
{
   // If conversion successful.
}
else
{
   // An unsuccessful conversion.
   // Might occur if there was an extension typo, like "Oblivion.esz"
}

// Would throw an exception if string could not be converted
modKey = ModKey.FromFileName("Skyrim.esm");
```

# FormKey
A `FormKey` represents a unique identifier for a record. They are Mutagen's interpretation of a `FormID`.

They contain:

- A record's `uint` ID (without master indices)
- A `ModKey`

A `FormKey` might be printed to a string as such: `123456:Oblivion.esm`.  The numbers are the last 6 digits of a FormID (with no mod index), followed by the string name of the mod it originated from. 

## Why does Mutagen use FormKeys instead of FormIDs?
### FormKeys Avoid Master Mismatch Bugs
`FormID` concepts are a staple of Bethesda modding, but they can be the source of some confusion and bugs.  This is usually due to the fact that a `FormID`'s master index bits are relative to the mod/loadorder that contains them.  This means a `FormID` contained in one mod can point to a completely different record if used to query into a different mod.  A user has to be very careful and "correct" the master indices to match the mod or context that they want to query into.

Mutagen abstracts away some of the complexity of `FormID`s by exposing `FormKey` objects instead.  There are no Master Indices, but rather a string of the mod's name they originate from.  This means a `FormKey` can never be misinterpreted to point to the wrong record, no matter what context it is used in.

### FormKeys Remove the 255 Mod Limit
As there is no Mod index stored as a byte within a `FormKey`, there is no 255 master limit within Mutagen.  Mods can contain records from >255 number of masters, and `LoadOrders` can contain >255 mods.

However, **note that the limit is still enforced when exporting to Binary**;  When transposed to disk, there is no way around the 255 limit.

## Construction
### Basic Constructor
```cs
var modKey = new ModKey("Oblivion", type: ModType.Master);
var formKey = new FormKey(modKey, 0x123456);
```
### Try Factory
```cs
if (FormKey.TryFactory("123456:Oblivion.esm", out var formKey))
{
   // If conversion successful
}

// Would throw an exception if string could not be converted
formKey = FormKey.Factory("123456:Skyrim.esm");
```

# FormLink
A `FormLink` adds type safety to the concept of a `FormKey`

They contain:

- A `FormKey`
- A Major Record type `<T>`

As an example of the type safety they provide, consider an NPC's `Race` member, which is of type: `FormLink<IRace>`.  This link is not allowed to point to or link against any record that is not a `Race`.  It is hard to accidentally set a `FormLink<IRace>` link to point to an `IPotion` record, as attempting to do so would typically result in a compiler error.

## Resolves
Combine a FormLink and a [LinkCache](../linkcache/index.md) to look up a record.  (The LinkCache is associated with a specific context like a load order or a single mod)
```cs
var someItemLink = new FormLink<IItemGetter>(FormKey.Factory("123456:Skyrim.esm"));

if (someItemLink.TryResolve(myLinkCache, out var itemRecord))
{
   // The FormKey associated with the FormLink was found
   // and was of the type associated with the link (IItemRecord)
   Console.WriteLine($"Was able to find the item record object: {itemRecord}");
}
```

### Scoping a Resolve
One important aspect is that Resolutions query based on the type.  So if the type is `Npc`, then only Npcs are parsed/inspected.  Some FormLinks have types that are too broad, though.  A `FormLink<ISkyrimMajorRecordGetter>` will inspect every record in the game, as it has no knowledge of what type to look for.  This may or may not be desired.

For example, `IItemGetter` can be many different types of records (Ammunition, Armor, Books, etc).  If you know you only care about `Armor` records, you can further limit the scope:
```cs
if (someItemLink.TryResolve<IArmorGetter>(myLinkCache, out var armorRecord))
{
   // Found an Armor
}
```
This will block matching against anything that isn't an Armor, and also be much faster.

## SetTo
Generated records will have FormLinks that point to records.  You can change the record they point to by modifying the FormLink
```cs
// Some Npc
var npc = new Npc(...);

// Set to a new FormKey
npc.WornArmor.SetTo(FormKey.Factory("123456:Skyrim.esm");

// Or to an actual Armor record
var armor = new Armor(...);
npc.WornArmor.SetTo(armor);

// Things that don't make sense will be blocked by the compiler
var ingredient = new Ingredient(...);
npc.WornArmor.SetTo(ingredient); // Compiler error
```

## Construction
Most FormLinks come as part of generated record objects, but you can also create your own.
Note that usually FormLinks work best when they point to getter interfaces, rather than setters.
```cs
// A FormKey without a type
FormKey myArmorKey = FormKey.Factory("123456:Skyrim.esm");

// An actual Armor object
IArmorGetter armor = new Armor(myArmorKey);

// Time to make some FormLinks
// Can create by passing in the FormKey or record object
FormLink<IArmorGetter> myArmorLink = new FormLink<IArmorGetter>(myArmorKey);
myArmorLink = new FormLink<IArmorGetter>(armor);

// They also have implicit operators if the left hand side's type is known
myArmorLink = myArmorKey;
myArmorLink = armor;

// FormKeys and record objects have AsLink helpers, too, when the left hand side type isn't known
var myOtherLink = myArmorKey.AsLink<IArmorGetter>();
var myOtherLink2 = armor.AsLink<IArmorGetter>();
```
