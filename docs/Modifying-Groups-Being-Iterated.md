# Modifying Groups Being Iterated
## Looping and Modifying the Same Record Types
Consider this:
```cs
foreach (var lvln in state.LoadOrder.PriorityOrder.LeveledNpc().WinningOverrides())
{
    var lvlnCopy = lvln.DeepCopy();
    lvlnCopy.EditorId += "Copy";
    // Some modifications
    state.PatchMod.LeveledNpcs.Set(lvlnCopy);
}
```
This code is dangerous, as it modifies the same record types it's inspecting.

## Modifying a Collection Being Enumerated
Consider this simpler code:
```cs
var list = new List<int>();
list.Add(3);
list.Add(7);
foreach (var item in list)
{
    // Exception on next loop
    list.Add(item * 2);
}
```
This code will throw an exception.  

It is because this loop is modifying the `list` it is looping.
The first loop iteration it will see `item = 3`, and add `6` to the list.

It will then try to loop to the next item, but C# will throw an exception 
`System.InvalidOperationException: Collection was modified; enumeration operation may not execute.`

In C#, you are not allowed to modify a collection as it's being looped.

## Avoiding the Exceptions
There are two routes to avoiding the `Collection was modified` exception
### Create a Temporary Collection
This can be as simple as:
```cs
foreach (var item in list
   // Now we're safe
   .ToArray())
{
    list.Add(item * 2);
}
```
The reason this works is that the `ToArray` call eagerly copies all the items from `list` to a new array.  The `foreach` loop then loops over that array.  Then, when you add an item to the list, you're not actually modifying the same collection you are looping, and so it succeeds.

### Stop looping after modification
Sometimes for certain purposes, you want to stop the logic after modifying once.
```cs
foreach (var item in list)
{
    list.Add(item * 2);
    break;
}
```
This self modification is allowed, because it is not the `list.Add` that is the problem, but rather the foreach loop trying to go to the next item right after.  So by breaking out of the loop, this code is safe.

## Applying it to Mutagen
Taking these same concepts back to Mutagen, if you're adding/removing/replacing records from a mod/group that you're looping, you can get the same exception.

If you're modifying the same record types you are looping, then you can follow the same patterns described above.  Just add a `ToArray` to keep yourself from modifying the collections you are looping:
```cs
foreach (var lvln in state.LoadOrder.PriorityOrder.LeveledNpc().WinningOverrides().ToArray())
{
    var lvlnCopy = lvln.DeepCopy();
    lvlnCopy.EditorId += "Copy";
    // Some modifications
    state.PatchMod.LeveledNpcs.Set(lvlnCopy);
}
```

Note that this only matters when you are Adding/Removing/Replacing a record FROM/TO a group in a mod.

If you are just modifying fields on the records (Setting the name, for example), this is not dangerous.  In this situation, you are not actually modifying the collection itself, you're modifying a field on a record.
