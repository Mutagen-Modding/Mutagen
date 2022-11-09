# Complication
`FormLinks` are `FormKeys` with typing information mixed in as to which record type they should associate with.  As such, they require you specify the typing you want to target.  Assuming you just wanted to target Npcs, there are still a few options:
- `Npc` -> The direct class
- `INpc` -> The setter interface
- `INpcGetter` -> The readonly interface

# Best Practice
The correct usage (99% of the time) is to always use the `INpcGetter` readonly interface.

`IFormLinkGetter<INpcGetter>`

Or more rarely

`FormKey<INpcGetter>`

The above still has the generic targeting the getter interface `INpcGetter`, which is the important part

# Why

Consider a [LinkCache resolve](https://github.com/Mutagen-Modding/Mutagen/wiki/Record-Lookup).  A FormLink and a LinkCache are combined to look up a record with a specific FormKey and Type.  

Using a non-getter interface is less ideal, because it limits the scope that the LinkCache can match against.  This might result in a failure to match where it might find the Npc with the target FormKey, but not be able to satisfy the more restrictive type.
```cs
IFormLinkGetter<INpc> myTargetNpc = ...;
if (myTargetNpc.TryResolve(myLinkCache, out var npc))
{
   // Found a INpc!
}
```
The `TryResolve` call wants to return an `INpc` type to you.  But if all it can find is a [readonly `INpcGetter`](https://github.com/Mutagen-Modding/Mutagen/wiki/Getters-Everywhere%3F), it cannot pretend that it's settable, and so fails to match.  This is the result of you asking the system to find an Npc that is settable, when the ones that exist are only getters.

You can solve this issue by modifying the TryResolve scope:
```cs
IFormLinkGetter<INpc> myTargetNpc = ...;
if (myTargetNpc.TryResolve<INpcGetter>(myLinkCache, out var npc))
{
   // Found a INpcGetter!
}
```
But it's preferable to have just had the FormLink type target getters in the first place.

There are very limited and intentional scenarios where having a FormLink target a mutable type is desirable, but for the most part it's a pitfall trap for new users.