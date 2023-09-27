# Environment Construction

!!! tip "Synthesis"
    If you're coding within a [Synthesis](https://github.com/Mutagen-Modding/Synthesis) Patcher, you should not make your own environment as described here.  Synthesis provides its own environment-like `IPatcherState` object in its Run function.
    
    [:octicons-arrow-right-24: Synthesis State Object](https://github.com/Mutagen-Modding/Synthesis/wiki/Coding-a-Patcher#synthesis-state-object)

## Known Game Category
The simplest way to construct an environment if you know the game you want to target is:
```cs
using (var env = GameEnvironment.Typical.Skyrim(SkyrimRelease.SkyrimSE))
{
    // Use ISkyrimModGetter environment
}
```

## Unknown Game Category
You can construct an environment when you don't know the target game:
```cs
using (var env = GameEnvironment.Typical.Construct(someGameRelease))
{
    // Use IModGetter environment
}
```

But this has the downside of not knowing the type of mod it will contain at compile time.  This means it will only expose `IModGetter` objects, which will be harder to interact with, depending on your goals

## Game Environment Builder
Lets you fluently tweak the environment that will be built to be customized to your needs:

- Override the target Data folder
- Wanting to omit a mod
- Wanting to customize load order
- Wanting to add an output mod, and integrate it with the link cache

These types of goals can be achieved with a builder:
```cs
var outgoing = new SkyrimMod(ModKey.FromFileName("MyMod.esp"), SkyrimRelease.SkyrimSE);
using var env = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter>(GameRelease.SkyrimSE)
    .TransformLoadOrder(x => x.Where(x => !x.ModKey.Name.Contains("SkipMe")))
    .WithTargetDataFolder(someCustomDataFolderPath)
    .WithOutputMod(outgoing)
    .Build();
```

### Transform Load Order
This gives you an Enumerable of mods, and lets you filter some out, or mix some in at your discretion.
#### TransformLoadOrderListings
This call gives you the listings as they appear in the load order files, before the mod objects get created.  Ideally any trimming is done here, rather than after the mods have been created.

Order between multiple `TransformLoadOrderListings` is respected.

#### TransformModListings
This call gives you the listings after they have been transformed by any `TransformLoadOrderListings` calls, and after mod objects have been created for each listing.  As such, this call can interact with the mod objects as part of its transform logic.

Order between multiple `TransformModListings` is respected, but will always come after any `TransformLoadOrderListings` calls.

### WithOutputMod
This lets you mix in a mod that you plan on exporting content with.  It will be added to the end of the LinkCache as a mutable mod that is safe to change.   You can put multiple `WithOutputMod` calls in your builder chain, and the order they appear will determine how they're placed on the Load Order and which ends up being the winning override.

### WithTargetDataFolder
Allows you to customize what game folder the environment will be constructed against.  Useful when dealing with [ad-hoc installations](Game-Locations.md#adhoc-installations).

### WithLoadOrder
This is a `TransformLoadOrderListings` style call that simply discards any existing load order and injects an explicitly provided one.  Will respect the ordering alongside other `TransformLoadOrderListings` phase calls.
