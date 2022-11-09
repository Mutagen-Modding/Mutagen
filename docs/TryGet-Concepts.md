There are many concepts within Mutagen that are optional, nullable, or may not link up at runtime.

It is good practice to code in a way that is able to handle both situations:
- The field is not null.  The lookup found its target.  Etc
- The field is null.  The lookup failed to find its target.  Etc

To facilitate this, most API comes with `Try` alternatives.

# Direct Access
Let's first take a look at the non-Try route.

This pattern assumes your lookup will succeed.  This is only safe if you checked that it existed earlier.

## Mutagen Example
An example:
```cs
INpcGetter npc = ...;

IRaceGetter race = npc.Race.Resolve(someLinkCache);
```

This will work in 98% of scenarios, up until some user has an odd Npc that doesn't list a Race.  Maybe it was a test Npc that isn't used anymore, so it's not a big deal, except for the fact that your code will now throw an exception.  It cannot `Resolve` the race and give you an object back, so it throws.

## Generic C# Example
This is equivalent to using the Dictionary indexer directly
```cs
Dictionary<int, string> dict = new();
dict[23] = "Hello";

// I will throw
var str = dict[45];
```

# TryGet Patterns Instead
Instead, a better pattern might be:

## Mutagen Example
```cs
INpcGetter npc = ...;

if (npc.Race.TryResolve(someLinkCache, out var race))
{
    System.Console.WriteLine($"Race was {race}");
}
else
{
    // Did not find it.  Return? Skip?
}
```

Often, if in a loop, the cleanest pattern is:
```cs
foreach (INpcGetter npc in someCollection)
{
   // Look it up, otherwise go to next npc
   if (!npc.Race.TryResolve(someLinkCache, out var race)) continue;

   System.Console.WriteLine($"Race was {race}");
}
```

## Generic C# Example
For the basic C# dictionary example, this would be the equivalent of:
```cs
Dictionary<int, string> dict = new();
dict[23] = "Hello";

if (dict.TryGetValue(45, out var str))
{
   // Found it
}
```
# Summary
It is almost always preferable to use the `Try` alternative when available.   It will force you to consider both when it finds what it was looking for, as well as the case when it does not.

Straight "`Try`-less" calls should only be used when you've previously checked that the value exists.  Then you know the call is safe, and so it's proper to not need to `Try`.  This is rarely used, as the `Try` pattern both checks that it exists and gets the value in one swoop, so a followup retrieval is usually not needed.