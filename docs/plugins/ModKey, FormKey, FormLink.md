---
label: ModKey, FormKey, FormLink
order: 1000
---

This article covers three fundamental identifiers:

- **[ModKey](#modkey)** _(a unique identifier for a mod)_
- **[FormKey](#formkey)** _(FormID)_
- **[FormLink](#formlink)** _(Typing added to a FormKey)_

## ModKey
`ModKey`s represent a unique identifier for a mod, as an alternative to a raw string.

| Field   | Type  | Description | Skyrim.esm Example |
| ---    | --- | --- | --- |
| Name | string | Mod's name (without '.esp' or '.esm) | "Skyrim" |
| Type | enum | Whether it is a master, plugin, or light master | ModType.Master |

### Construction
=== "TryFromFileName"
    ```cs
    if (ModKey.TryFromFileName("Skyrim.esm", out var modKey))
    {
       // If conversion successful.
    }
    else
    {
       // An unsuccessful conversion.
       // Might occur if there was an extension typo, like "Skyrim.esz"
    }
    ```
=== "FromFileName"
    ```cs
    var modKey = ModKey.FromFileName("Skyrim.esm");
    ```
    !!! warning "This would throw an exception if string could not be converted"
=== "New"
    ```cs
    var modKey = new ModKey("Skyrim", ModType.Plugin);
    ```

## FormKey
A `FormKey` represents a unique identifier for a record. They are Mutagen's interpretation of a `FormID`.

| Field   | Type  | Description |  Skyrim Iron Sword example |
| ---    | --- | --- | --- |
| ModKey | ModKey | The ModKey associated with the FormKey | "Skyrim.esm" |
| ID | uint | A record's FormID (without master indices) | 0x012EB7 |

!!! warning "Mod indices are ignored in the uint, as this information is stored in the ModKey"

!!! info "String Representation"
    An example FormKey string might be `123456:Skyrim.esm`

    The numbers are the last 6 digits of a FormID (with no mod index), followed by the string name of the mod that originally defined it

### Construction
=== "TryFactory"
    ```cs
    if (FormKey.TryFactory("123456:Oblivion.esm", out var formKey))
    {
       // If conversion successful
    }
    ```
=== "Factory"
    ```cs
    formKey = FormKey.Factory("123456:Skyrim.esm");
    ```
    !!! warning "This would throw an exception if string could not be converted"
=== "Constructor"
    ```cs
    var modKey = ModKey.FromFileName("Skyrim.esm");
    var formKey = new FormKey(modKey, 0x123456);
    ```

### Why FormKeys instead of FormIDs?

#### Avoid Master Mismatch Bugs
`FormID` concepts are a staple of Bethesda modding, but they can be the source of some confusion and bugs.  

!!! warning
    A `FormID`'s master index bits are relative to the mod/loadorder that contains them.  A `FormID` contained in one mod can point to a completely different record if used to query into a different mod.  A user has to "realign" the master indices of a FormID to match the mod or context that they want to query into.

Mutagen abstracts away some of the complexity of `FormID`s by exposing `FormKey` objects instead.  There are no Master Indices, but rather a string of the mod's name they originate from.  This means a `FormKey` can never be misinterpreted to point to the wrong record, no matter what context it is used in.

#### Remove the 255 Mod Limit
As there is no Mod index stored as a byte within a `FormKey`, there is no 255 master limit within Mutagen when a program is running.  Mods can contain records from >255 number of masters, and `LoadOrders` can contain >255 mods.

!!! warning "Limit Still Exists on Disk"
    The limit is still enforced when exporting to Binary.  When transposed to disk, there is no way around the 255 limit.

## FormLink
A `FormLink<T>` is generic, where T is a Major Record type.

| Field   | Type  | Description |   Skyrim Iron Sword example |
| ---    | --- | --- | --- |
| FormKey | FormKey | The FormKey associated with the FormLink | "012EB7:Skyrim.esm" |
| Type | Type | A Major Record type | IWeaponGetter |

A `FormLink` adds type safety to the concept of a `FormKey`.  Consider an NPC's `Race` member, which is of type: `FormLink<IRaceGetter>`.  This link is not allowed to point to or link against any record that is not a `Race`.  It is hard to accidentally set a `FormLink<IRaceGetter>` link to point to an `IPotionGetter` record, as attempting to do so would typically result in a compiler error.

!!! tip "Best Practices"
    It is recommended to use "Getter" interfaces when specifying generic types

    [:octicons-arrow-right-24: Prefer Getters](../best-practices/FormLinks-Target-Getter-Interfaces.md)



### Resolves

FormLinks can be combined with [LinkCaches](../linkcache/index.md) to easily look up a record. 

``` { .cs hl_lines=3 }
IFormLinkGetter<IItemGetter> someItemLink = ...;

if (someItemLink.TryResolve(myLinkCache, out var itemRecord))
{
   // The FormKey associated with the FormLink was found
   // and was of the type associated with the link (IItemRecord)
   Console.WriteLine($"Was able to find the item record object: {itemRecord}");
}
```

[:octicons-arrow-right-24: Record Lookups](../linkcache/Record-Resolves.md)

### SetTo
Generated records will have FormLinks that point to records.  You can change the record they point to by modifying the FormLink
=== "FormKey"
    ``` { cs hl_lines="4" }
    Npc npc = ...;
    FormKey formKey = ...;
	
    npc.WornArmor.SetTo(formKey);
	```
=== "Record"
    ``` { cs hl_lines="4" }
    Npc npc = ...;
    Armor armor = ...;
	
    npc.WornArmor.SetTo(armor);
	```

!!! info "Things that don't make sense will be blocked by the compiler"
    ``` { cs hl_lines="5" }
    Npc npc = ...;
    Ingredient ingredient = ...;
	
	// An ingredient cannot be a worn armor
    npc.WornArmor.SetTo(ingredient); // Compiler error
	```

### Construction
Most FormLinks come as part of generated record objects, but you can also create your own.
Note that usually FormLinks work best when they point to getter interfaces, rather than setters.

=== "Constructor"
    ``` { cs hl_lines="5 8" }
    FormKey formKey = ...;
    IArmorGetter armor = ...;
	
    // With FormKey
    var link1 = new FormLink<IArmorGetter>(formKey);
	
    // With Record
    var link2 = new FormLink<IArmorGetter>(armor);
	```
=== "Implicit Operator"
    ``` { cs hl_lines="6 9" }
    FormKey formKey = ...;
    IArmorGetter armor = ...;
	FormLink<IArmorGetter> myLink;
	
    // Implictly assign with FormKey
	link = formKey;
	
    // Implictly assign with Record
	link = armor;
	```
=== "Helper Functions"
    ``` { cs hl_lines="5 8" }
    FormKey formKey = ...;
    IArmorGetter armor = ...;
	
	// By FormKey
    var myOtherLink = myArmorKey.AsLink<IArmorGetter>();
	
	// By Record
    var myOtherLink2 = armor.AsLink();
	```

!!! tip "Best Practices"
    It is recommended to use "Getter" interfaces when specifying scoping types

    [:octicons-arrow-right-24: Prefer Getters](../best-practices/FormLinks-Target-Getter-Interfaces.md)
