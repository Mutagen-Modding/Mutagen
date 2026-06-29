# Keywords

Many record types in Bethesda games carry a list of Keywords, including Armor, Weapon, Npc, Ingestible, MagicEffect, Race, and many more. Mutagen provides convenience methods for checking keyword membership, adding/removing keywords, and more.

## HasKeyword

The most common operation is checking whether a record contains a specific keyword. Mutagen provides several overloads:

=== "By EditorID"
    ```cs
    INpcGetter npc = ...;
    ILinkCache linkCache = ...;

    if (npc.HasKeyword("ArmorHeavy", linkCache))
    {
        // NPC has the keyword
    }
    ```
=== "By FormLink"
    ```cs
    INpcGetter npc = ...;
    IFormLinkGetter<IKeywordGetter> armorHeavyKeyword = ...;

    if (npc.HasKeyword(armorHeavyKeyword))
    {
        // NPC has the keyword
    }
    ```
=== "By FormKey"
    ```cs
    INpcGetter npc = ...;
    FormKey keywordFormKey = ...;

    if (npc.HasKeyword(keywordFormKey))
    {
        // NPC has the keyword
    }
    ```
=== "By Record"
    ```cs
    INpcGetter npc = ...;
    IKeywordGetter keyword = ...;

    if (npc.HasKeyword(keyword))
    {
        // NPC has the keyword
    }
    ```

## HasAnyKeyword

When you need to check against multiple keywords at once:

=== "By EditorIDs"
    ```cs
    IWeaponGetter weapon = ...;
    ILinkCache linkCache = ...;

    if (weapon.HasAnyKeyword(new[] { "WeapTypeSword", "WeapTypeDagger" }, linkCache))
    {
        // Weapon matches at least one of the keywords
    }
    ```
=== "By FormLinks"
    ```cs
    IWeaponGetter weapon = ...;
    IEnumerable<IFormLinkGetter<IKeywordGetter>> swordKeywords = ...;

    if (weapon.HasAnyKeyword(swordKeywords))
    {
        // Weapon matches at least one of the keywords
    }
    ```
=== "By FormKeys"
    ```cs
    IWeaponGetter weapon = ...;
    IEnumerable<FormKey> keywordFormKeys = ...;

    if (weapon.HasAnyKeyword(keywordFormKeys))
    {
        // Weapon matches at least one of the keywords
    }
    ```

## TryResolveKeyword

If you need both the check and the resolved Keyword record, use `TryResolveKeyword` to do both in one call:

=== "By EditorID"
    ```cs
    IArmorGetter armor = ...;
    ILinkCache linkCache = ...;

    if (armor.TryResolveKeyword("ArmorHeavy", linkCache, out var keyword))
    {
        Console.WriteLine($"Found keyword: {keyword.FormKey}");
    }
    ```
=== "By FormLink"
    ```cs
    IArmorGetter armor = ...;
    IFormLinkGetter<IKeywordGetter> keywordLink = ...;
    ILinkCache linkCache = ...;

    if (armor.TryResolveKeyword(keywordLink, linkCache, out var keyword))
    {
        Console.WriteLine($"Found keyword: {keyword.EditorID}");
    }
    ```
=== "By FormKey"
    ```cs
    IArmorGetter armor = ...;
    FormKey keywordFormKey = ...;
    ILinkCache linkCache = ...;

    if (armor.TryResolveKeyword(keywordFormKey, linkCache, out var keyword))
    {
        Console.WriteLine($"Found keyword: {keyword.EditorID}");
    }
    ```

## Adding and Removing Keywords

The `Keywords` property on mutable records is a nullable `ExtendedList`:

=== "Add a Keyword"
    ```cs
    INpc npc = ...;
    IFormLinkGetter<IKeywordGetter> newKeyword = ...;

    // Initialize the list if null
    npc.Keywords ??= new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
    npc.Keywords.Add(newKeyword);
    ```
=== "Remove a Keyword"
    ```cs
    INpc npc = ...;
    FormKey keywordToRemove = ...;

    npc.Keywords?.RemoveAll(x => x.FormKey == keywordToRemove);
    ```

## The IKeyworded Interface

Under the hood, all of the above convenience methods are powered by the `IKeyworded` [aspect interface](../../game-specific/skyrim/Skyrim-Aspect-Interfaces.md). Any record type that has a `Keywords` field implements this interface, which means you can write generic code that works across all keyworded record types:

```cs
void PrintKeywords(IKeywordedGetter<IKeywordGetter> keyworded)
{
    if (keyworded.Keywords == null) return;

    foreach (var keywordLink in keyworded.Keywords)
    {
        Console.WriteLine($"  Keyword: {keywordLink.FormKey}");
    }
}
```
