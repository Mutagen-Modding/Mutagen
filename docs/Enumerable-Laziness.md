# Setup
One common pitfall related to Enumerable/LINQ usage.  Take this basic Synthesis patcher example:
```cs
var weaponsWithDamageMoreThanTen = 
    // Loop over all winning override weapons in the load order
    state.LoadOrder.PriorityOrder.Weapon().WinningOverrides()
    // Only want ones with damage above 10
    .Where(x =>
    {
        if (x.BasicStats == null) return false;
        return x.BasicStats.Damage > 10;
    });

// Print!
Console.WriteLine("Weapons with damage more than 10:");
foreach (var weaponGetter in weaponsWithDamageMoreThanTen)
{
    Console.WriteLine(weaponGetter.ToString());
}
```
This works as expected, printing lots of weapons to the console.

# Problem
## What's the problem?

The variable `weaponsWithDamageMoreThanTen` is a deferred Linq statement.  Sometimes can also be called a "naked" or "lazy" Linq statement.

## What does this mean?

This means `weaponsWithDamageMoreThanTen` does not contain all the weapons with damage more than 10.  Rather, it is instructions for HOW to iterate the load order to get weapons with damage more than 10.

It is a set of instructions, not the actual data itself.

## Why does this matter?
It matters because multiple usages of these instructions means multiple executions of the instructions to get the results.

Consider this:
```cs
// Print!  One usage
Console.WriteLine("Weapons with damage more than 10:");
foreach (var weaponGetter in weaponsWithDamageMoreThanTen)
{
    Console.WriteLine(weaponGetter.ToString());
}

// Do some work and add to the outgoing patch.  Two usage
foreach (var weaponGetter in weaponsWithDamageMoreThanTen)
{
    // Put in outgoing patch
    var weapon = state.PatchMod.Weapons.GetOrAddAsOverride(weaponGetter);
    // Cut the damage in half
    weapon.BasicStats!.Damage /= 2;
}
```

This will work and produce results.  However, it will take 2x the time as it could.

The print logic will execute the instructions provided by `weaponsWithDamageMoreThanTen`.  It will go to disk, read the Weapon GRUP data, parse the GRUP data into hundreds of Weapon records, iterate all those objects through your 10 damage filter, and then print to the console.

The 2nd loop will then run.  It will go to disk, read the Weapon GRUP data, parse the GRUP data into hundreds of Weapon records, iterate all those objects through your 10 damage filter, and then add half the damage and put it into the outgoing patch.

Notice this logic read from the disk and parsed the weapons TWICE.  This is because `weaponsWithDamageMoreThanTen` contains instructions for HOW to retrieve the weapons.  So each usage of those instructions DOES that work again.

This concept where each usage does the work again can get catastrophically slow depending on what you're doing.

# Solution
Linq is powerful and useful, but with power comes responsibility. 

## Small Metaphor
A naked Linq statement is like a set of instructions.

Which would be more efficient?

`Using instructions for how to make an airplane, and using them to construct a new airplane for every flight.`

Or 

`Using instructions for how to make an airplane once ahead of time, and reusing that airplane for many flights.`

Obviously making one airplane and reusing it is the better option.

Having instructions for how to make an airplane isn't a bad thing to have, but choosing when to execute those instructions is important.

## The Fix
The fix is just to execute those instructions ahead of time once, so that downstream usages don't rerun the logic each time.  This can be done very simply:
```cs
var weaponsWithDamageMoreThanTen = 
    // Loop over all winning override weapons in the load order
    state.LoadOrder.PriorityOrder.Weapon().WinningOverrides()
    // Only want ones with damage above 10
    .Where(x =>
    {
        if (x.BasicStats == null) return false;
        return x.BasicStats.Damage > 10;
    })
    // Tell the above instructions to run immediately
    // and put themselves into an array
    .ToArray();
```

That's it.  Now all the usages downstream will loop over the contents of the array, rather than rerunning the instructions for themselves.

This is the equivalent of using the airplane instructions once to make a plane, and now it can be reused by everyone downstream.

# Why is Linq Deferred/Lazy?
It is important to understand why Linq statements are lazy in the first place?  Why have this dangerous behavior at all?

The laziness of a Linq enumerable is actually very powerful, as it allows us to short circuit work that is not needed.

Consider this:
```cs
var npcs = state.LoadOrder.PriorityOrder.Npc().WinningOverrides();

foreach (var npc in npcs)
{
    if (npc.Name.Contains("Goblin"))
    {
        Console.WriteLine("Our game has a Goblin");
        break;
    }
}
```

This logic will parse Npc records on disk until it finds a Goblin.  It will then print and stop working.

The laziness of the Linq statement means that we only execute the instructions UNTIL we find a goblin.  If the first Npc record we saw was a Goblin, the program will have only parsed one Npc record.  This would be VERY fast, then.

Compare to this:
```cs
var npcs = state.LoadOrder.PriorityOrder.Npc().WinningOverrides()
    .ToArray();

foreach (var npc in npcs)
{
    if (npc.Name.Contains("Goblin"))
    {
        Console.WriteLine("Our game has a Goblin");
        break;
    }
}
```
We've added that ToArray() to turn the instructions into their results up front, storing them in an array.  In this case, this is NOT what we want.

Now, if the Goblin was the first Npc record we found, we will still parse ALL Npcs in the game:
1)  Set up instructions for how to find npcs
2)  Do instructions once, putting all npcs into an array
3)  Loop over array, stopping once we find a goblin.

Notice that the work was done ahead of time, so we couldn't short circuit the work early if we found what we wanted.  We will do ALL the work every time, even if we're tossing 99% of the results into the trash.

# Conclusion
The laziness of Linq statements is a powerful tool, but it is very dangerous if misused, and so must be understood.

If a Linq statement is going to be reused, consider calling a `ToArray` to do the work once ahead of time.

If the Linq statement only has one user, it's better to leave as a Deferred Linq statement, so that that one user can choose to stop processing early.

If you're unsure, it is usually better to use the `ToArray`.  Doing unnecessary work once is a better gamble than potentially doing the same work 1000x.