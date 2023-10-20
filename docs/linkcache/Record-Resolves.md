# Record Resolve
Record resolves are the main feature that Link Caches provide:  looking up a record relative to some mods.

Some important concepts to consider when Resolving:

[:octicons-arrow-right-24: Scoping Type](Scoping-Type.md)

[:octicons-arrow-right-24: Resolve Target](index.md#resolve-target)

## TryResolve
`TryResolve` is the typical call for looking up records pointed to by a FormKey.  Similar to how Control-Clicking a FormID in xEdit will bring you to the record a FormID points to.  It takes a LinkCache as a parameter to the call, which will inspect the content it is attached to (whether it's a load order or a single mod) and try to locate the record that matches:

- The FormKey (FormID)
- The type specified

If found, the record returned will be from the mod latest in the load order which is the "winning" override.

!!! tip "Subsequent Resolves"
    For [Immutable caches](index.md#immutable-link-caches), the results will be cached, and subsequent resolves will be almost instant

=== "FormLink Entry Point"
    ``` { .cs hl_lines=4 }
    ILinkCache myLinkCache = ...;
    IFormLinkGetter<INpcGetter> myLink = ...;

    if (myLink.TryResolve(myLinkCache, out var npc))
    {
        Console.WriteLine($"Found the npc! {npc.EditorID}");
    }
    ```

=== "LinkCache Entry Point"
    ``` { .cs hl_lines=4 }
    ILinkCache myLinkCache = ...;
    IFormLinkGetter<INpcGetter> myLink = ...;

    if (myLinkCache.TryResolve(myLink, out var record))
    {
        Console.WriteLine($"Found a record! {record.EditorID}");
    }
    ```

=== "By EditorID"
    ``` { .cs hl_lines=3 } 
    ILinkCache myLinkCache = ...;
    string myEditorID = ...;

    if (myLinkCache.TryResolve(myEditorId, out var record))
    {
        Console.WriteLine($"Found a record! {record.EditorID}");
    }
    ```

=== "By FormKey"
    ``` { .cs hl_lines=3 } 
    ILinkCache myLinkCache = ...;
    myFormKey formKey = ...;

    if (myLinkCache.TryResolve(myFormKey, out var record))
    {
        Console.WriteLine($"Found a record! {record.EditorID}");
    }
    ```

## TryResolveIdentifier
Sometimes you are not interested in getting the whole record.   The `TryResolveIdentifier` option lets you provide a FormKey to look up the associated EditorID, and vice versa.

``` { .cs hl_lines=3 } 
ILinkCache myLinkCache = ...;
    string myEditorID = ...;

if (myLinkCache.TryResolveIdentifier(myEditorId, out var associatedFormKey))
{
    Console.WriteLine($"Found a record! {associatedFormKey}");
}
```

This is often useful to use with lightweight Link Caches, which only deal in identifiers.

[:octicons-arrow-right-24: Identifier Only Caches](index.md#identifier-only-caches)

### Throwing Variants
All API described above has non-Try variants that will throw an exception if the resolve is unsuccessful.  These can be useful if you know you want to short circuit and fail if a record is missing.