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
    .TransformLoadOrderListings(x => x.Where(x => !x.ModKey.Name.Contains("SkipMe")))
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
This lets you mix in a mod that you plan on exporting content with.  You can put multiple `WithOutputMod` calls in your builder chain.

!!! warn "Modifies Load Order"
    Using WithOutputMod adjusts the Load Order the Game Environment will give you, as described below

#### Load Order Trimming
When using WithOutputMod, be aware that this modifies the Load Order the environment will provide you to be trimmed so that it doesn't include the outgoing mod and anything after it.   The Load Order presented by the environment represents the existing mods to consider while building the output mod, and as such, excludes the outgoing mod itself, and anything after it.

This is to help avoid:

- "Collection modified while looping" errors
- Adding masters to the outgoing mod from mods after itself in the load order (not allowed)

#### Link Cache
The Link Cache made by the environment with an output mod will include the Load Order mods PLUS the outgoing mod(s) at the end.  Lookups against the Link Cache WILL resolve objects from your outgoing mod appropriately.   Outgoing mods are registered in the Link Cache in a non-caching fashion, so edits to your outgoing mod will reflect in subsequent lookups appropriately.

#### Further Customization
If you want to break away from the default behaviors described above, you can always make your own Load Order and Link Cache objects that contain different content the way you want them.   You can even opt to not use WithOutputMod entirely and have the Game Environment give you the whole Load Order as a baseline but customize it as you want from there.

### WithTargetDataFolder
Allows you to customize what game folder the environment will be constructed against.  Useful when dealing with [ad-hoc installations](Game-Locations.md#sources).

### WithLoadOrder
This is a `TransformLoadOrderListings` style call that simply discards any existing load order and injects an explicitly provided one.  Will respect the ordering alongside other `TransformLoadOrderListings` phase calls.
