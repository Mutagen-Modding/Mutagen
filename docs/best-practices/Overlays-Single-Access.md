
### Best Practices
As noted above, the Overlay pattern has the downside that repeated access of properties means repeated parsing of the same data.  While this may sound scary at first, it shouldn't pose much of a problem in most scenarios as parsing is very quick.  Most caching solutions that attempt to "fix" this quality will probably be slower and come with the additional downside of bloating memory usage.

However, this parse-per-access quality does encourage a slightly different coding pattern if you want to squeeze out the tiniest optimization.  Ideally you still want to access properties as little as possible when you can.  Here are some suggestions and tricks:

#### Example of Misuse
Here is an example of very slight misuse (only when using an Overlay):
```cs
if (npcOverlay.EditorID != null)
{
    System.Console.WriteLine($"NPC's EditorID was: {npcOverlay.EditorID}");
}
```

Why is this bad practice?  Because `npcOverlay.EditorID` was accessed twice, and so the byte to string parsing for EditorID was executed twice.  In reality, this isn't the biggest deal as it's a small amount of data, and the parsing is extremely quick.  But we're here to talk about optimization and best practices.

#### Save to Variable
One simple way to reduce access calls when you know you're going to be doing several in a row is to just save the single access to a variable:
```cs
var editorID = npcOverlay.EditorID;
if (editorID != null)
{
    System.Console.WriteLine($"NPC's EditorID was: {editorID}");
}
```
This is a simple way to remove the redundant access/parse.

#### Group Access Special Case
Accessing a Group on a binary overlay mod object is a special case that is even more expensive.  The mod objects are designed to open the file and read the contents from disk at the Group level.  Compared to normal records/fields which multiple accesses will just result in parsing the same memory twice, multiple accesses of the same group will result in multiple reads of the content off the disk.  This is much slower.  

As such, Groups should receive extra care to only access them once.  As a simple example using the tip describe above:
```cs
var overlayMod = ...;

// Retrieve the group once
var npcs = overlayMod.Npcs;

foreach (var thing in things)
{
   // Use the same variable many times
   if (npcs.TryGetValue(thing.FormKey, out var record))
      ...
}
```
#### Pattern Matching
It is a very common pattern that a member might not be set.  An alternate way of dealing with this is via Pattern Matching:
```cs
if (npcOverlay.EditorID is string editorID)
{
    System.Console.WriteLine($"NPC's EditorID was {editorID}");
}
```

This provides a nice if-exists-then-do pattern, where `EditorID` is only parsed once and its result exposed in a non-null `string` variable to use inside the if statement.

If you want a more `var`-like behavior, you can also consider this pattern
```
if (npcOverlay.EditorID is {} editorID)
{
    System.Console.WriteLine($"NPC's EditorID was {editorID}");
}
```

You no longer need to specify the type `string`, but it's an odd looking pattern.  C# might introduce a better looking one in the future.

#### Safe Navigation Operator
In C#, a call on a potentially null member can be short circuited easily by adding a Safe Navigation Operator (`?`).  This does a null check, and only calls the function or retrieves the data if it is not null.  Here's how it might be used:
```cs
System.Console.WriteLine($"NPC's EditorID was {npcOverlay.EditorID?.ToString()}");
```
`ToString()` is only called if EditorID is not null.  It has the upside of only accessing the property once before calling the function, and so is proper practice when calling functions on Overlay members that could be null.

#### Null Coalescing Operator
Another common operator related to potentially null items is the Null Coalescing operator (`??`).  This checks if an item is null, and if so, returns a second value.
```cs
System.Console.WriteLine($"NPC's EditorID was {npcOverlay.EditorID ?? "UNKNOWN"}");
```
This will access and return EditorID, unless it is null, at which point "UNKNOWN" will be returned and printed.  It also has the upside of only accessing the property once before calling the function, and so is proper practice when accessing Overlay members that could be null.




### Concrete Example
It might be useful to walk through a concrete example, and some of the mechanics going on under the hood.  Consider this code:
```cs
using (IOblivionModGetter mod = OblivionMod.CreateFromBinaryOverlay(pathToMod))
{
   foreach (var potion in mod.Potions.Records)
   {
      // Check if the record has an EditorID, and get it
      if (!potion.EditorID.TryGet(out var edid)) continue;
      // Print the result
      System.Console.WriteLine(edid);
   }
}
```
This code is intended to print each Potion's Editor ID to the console.

#### What work is actually done by this code?

- A file is opened
- An overlay class that implements `IOblivionModGetter` and has a reference to the file stream is instantiated.
- Quick skip-over parsing of the file is done to locate the locations of the Groups.
- User accesses the Potion Group.
- A Group overlay object is created, and the Potion Group's raw bytes are read from the file.
- Group overlay object marks all locations of Potion records in its data.
- User loops over all Potions in the Group.
- For each loop iteration, an `IPotionGetter` overlay object is created, pointing to the memory location for that record.
- The potion overlay object marks the location of all subrecords it contains.
- The user retrieves the EditorID (once), and checks if is set.
- The EDID subrecord is parsed from the byte array as a string and returned.
- The string is printed to the console.
- After loop is over, the stream is closed.

#### What are some things that were not done?

- No Groups besides Potion were parsed. Their top level locations were noted, but no contents were processed.
- No subrecords were parsed, except EditorID (EDID).
- No object had a reference to all the Potion records, so as to keep their contents in memory.  The Group object simply has a list of locations.  The user has the only reference to any Potion record at any given moment, and as soon as they were done with it was cleaned up.
- No extra code was written by the user to help indicate they were only interested in Potions or EditorIDs.  Writing code that accessed them was the implicit indication of interest itself.
