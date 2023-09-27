# Scoping Type
The supplied type to calls on a Link Cache are important as they allow the systems to reduce the records that needs to be searched.  This improves both speed and memory usage.   

For example, if you know you only want to look up an `Npc`, then scope your calls to the type `Npc` so that only `Npc` records are parsed. 

To scope a call, simply provide a type that is the most specific for your needs, which can be done in a variety of ways.

=== "Scoped Implicitly By FormLink"
    ```cs
    ILinkCache myLinkCache = ...;
    FormLink<INpcGetter> myFormLink = ...;

    if (myLinkCache.TryResolve(myFormLink, out var targetNpc))
    {
        // Found the Npc
    }
    ```


=== "Scoped By Generic"
    ```cs
    ILinkCache myLinkCache = ...;
    FormKey myFormKey = ...;

    if (myLinkCache.TryResolve<INpcGetter>(myFormKey, out var targetNpc))
    {
        // Found the Npc
    }
    ```
=== "Scoped By Parameter"
    ```cs
    ILinkCache myLinkCache = ...;
    FormKey myFormKey = ...;

    if (myLinkCache.TryResolve(myFormKey, typeof(INpcGetter), out var targetNpc))
    {
        // Found the Npc
    }
    ```

    !!! tip "Use Generic When Possible"
        This query will run optimally thanks to the passed in type, but the returned vartiable `targetNpc` will only be `ISkyrimMajorRecordGetter`, rather than `INpcGetter`, making it hard to use without further casting.  Using the generic alternative will improve this.
=== "Unscoped"
    ```cs
    ILinkCache myLinkCache = ...;
    FormKey myFormKey = ...;

    if (myLinkCache.TryResolve(myFormKey, out var targetNpc))
    {
        // Found the Npc
    }
    ```

    !!! warning "Not Recommended"
        This type of untyped query is allowed, but should only be used as a last resort
        
## Situations to Scope
### Link Interfaces
Link Interfaces are "umbrella" interfaces that can point to many record types.

[:octicons-arrow-right-24: Link Interfaces](../plugins/Interfaces.md#link-interfaces)

For example, `IItemGetter` can be:

- Ammunition
- Armor
- Books
- etc

If you know you only care about `Armor` records, you can adjust your call to tighten to that scope.

### FormLinks Pointing to All Major Records

Some FormLinks do not have a known target type, and will be presented as "any" major record:  `IFormLinkGetter<ISkyrimMajorRecordGetter>`

These calls will be just as slow as providing no type at all.  If you have any idea of the record type you are looking for, you should provide it in these situations

### Other

Depending on your coding situation, you might be fed a "naked" FormKey that has no type associated.  When using these as inputs for Link Cache API, provide a scope type if there is one for your coding situation.