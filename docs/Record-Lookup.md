# TryResolve
`TryResolve` is the typical call for looking up records pointed to by a FormKey.  Similar to how Control-Clicking a FormID in xEdit will bring you to the record a FormID points to.  It takes a LinkCache as a parameter to the call, which will inspect the content it is attached to (whether it's a load order or a single mod) and try to locate the record that matches:
- The FormKey (FormID)
- The type specified

If found, the record returned will be from the mod latest in the load order which is the "winning" override.

For [Immutable caches](https://github.com/Mutagen-Modding/Mutagen/wiki/LinkCache#immutable-link-caches), the results will be cached, and subsequent lookups will be almost instant.

# FormLink Entry Point
While the LinkCache is the object doing the work, lookups are typically initiated from FormLink objects.  This is because they contain the typing information (Npc/Weapon/etc) already, so the call can be quite succinct:
```cs
ILinkCache myLinkCache = ...;
IFormLinkGetter<INpcGetter> myLink = ...;

if (myLink.TryResolve(myLinkCache, out var npc))
{
    Console.WriteLine($"Found the npc! {npc.EditorID}");
}
```

# LinkCache Entry Point
You can also initiate off of a LinkCache directly
```cs
FormKey myFormKey = ...;

if (myLinkCache.TryResolve(myFormKey, out var record))
{
    Console.WriteLine($"Found a record! {record.EditorID}");
}
```

But the code above has two problems:
- It will only be able to return `IMajorRecordGetter` or some other very umbrella type
- It will complain that an unoptimized call is used

This is because the above call has no information about the record type being looked up.  To help provide this information, you can instead:
```cs
FormKey myFormKey = ...;

if (myLinkCache.TryResolve<INpcGetter>(myFormKey, out var npc))
{
    Console.WriteLine($"Found an npc! {npc.EditorID}");
}
```

Now that the type is specified, it will run faster and be able to return a more appropriate type (INpcGetter) for you to use.  As mentioned earlier, though, it is preferable to just [use FormLinks everywhere](https://github.com/Mutagen-Modding/Mutagen/wiki/FormLinks-Instead-of-FormID-FormKey) and initiate off of those.