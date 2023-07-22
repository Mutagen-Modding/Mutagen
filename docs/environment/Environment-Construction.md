# Environment Construction
## Single Game Category Construction
As mentioned in the [overview section](Environment), the typical way to construct an environment if you know the game you want to target is:
```cs
using (var env = GameEnvironment.Typical.Skyrim(SkyrimRelease.SkyrimSE))
{
    // Use environment
}
```

### Synthesis Usage
If you're coding within a [Synthesis Patcher](https://github.com/Mutagen-Modding/Synthesis), you should not make your own environment as described here.  Synthesis provides its own environment-like `IPatcherState` object in its Run function.  [Read More](https://github.com/Mutagen-Modding/Synthesis/wiki/Coding-a-Patcher#synthesis-state-object)

## Unknown Game Construction
You can construct an environment when you don't know the target game:
```cs
using (var env = GameEnvironment.Typical.Construct(someGameRelease))
{
    // Use environment
}
```

But this has the downside of not knowing the type of mod it will contain at compile time.  This means it will only expose `IModGetter` objects, which will be harder to interact with, depending on your goals

## Game Environment Builder
Lets you fluently tweak the environment that will be built to be customized to your needs.

### Problem
There are a lot of times when the [Single Game Category Construction](#single-game-category-construction) game environment does not suit your needs.  Consider:
- Wanting to omit a mod
- Wanting to add an output mod, and integrate it with the link cache

To do this with the simple bootstapper, you'd have to construct your own LoadOrder/LinkCache objects using the ones it gave you, but with your desired modifications.

### Builder Pattern
Instead, you can make use of the `GameEnvironmentBuilder` system:
```cs
var outgoing = new SkyrimMod(ModKey.FromFileName("MyMod.esp"), SkyrimRelease.SkyrimSE);
using var env = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter>(GameRelease.SkyrimSE)
    .TransformLoadOrder(x => x.Where(x => !x.ModKey.Name.Contains("SkipMe")))
    .WithTargetDataFolder(someCustomDataFolderPath)
    .WithOutputMod(outgoing)
    .Build();
```
It uses a fluent API that lets you mix in the modifications you want to make to the environment as it's being built.  In this case the environment given to you will not include any mods that contain "SkipMe", will target a custom data folder specified, and will contain the `outgoing` mod at the end of its LinkCache.

#### Transform Load Order
This gives you an Enumerable of mods, and lets you filter some out, or mix some in at your discretion.
##### TransformLoadOrderListings
This call gives you the listings as they appear in the load order files, before the mod objects get created.  Ideally any trimming is done here, rather than after the mods have been created.

Order between multiple `TransformLoadOrderListings` is respected.

##### TransformModListings
This call gives you the listings after they have been transformed by any `TransformLoadOrderListings` calls, and after mod objects have been created for each listing.  As such, this call can interact with the mod objects as part of its transform logic.

Order between multiple `TransformModListings` is respected, but will always come after any `TransformLoadOrderListings` calls.

#### WithOutputMod
This lets you mix in a mod that you plan on exporting content with.  It will be added to the end of the LinkCache as a mutable mod that is safe to change.   You can put multiple `WithOutputMod` calls in your builder chain, and the order they appear will determine how they're placed on the Load Order and which ends up being the winning override.

#### WithTargetDataFolder
Allows you to customize what game folder the environment will be constructed against.  Useful when dealing with [ad-hoc installations](Game-Locations.md#adhoc-installations).

#### WithLoadOrder
This is a `TransformLoadOrderListings` style call that simply discards any existing load order and injects an explicitly provided one.  Will respect the ordering alongside other `TransformLoadOrderListings` phase calls.
