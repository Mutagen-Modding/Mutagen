# Typical Mod Construction and Importing
If you're only dealing with one game, then code like this is typical:
```cs
public void SomeFunction()
{
    var newMod = new SkyrimMod(ModKey.FromFileName("MyMod.esp"), SkyrimRelease.SkyrimSE);

    var mutableInputMod = SkyrimMod.CreateFromBinary(inputPath, SkyrimRelease.SkyrimSE);

    using var readOnlyInputMod = SkyrimMod.CreateFromBinaryOverlay(inputPath, SkyrimRelease.SkyrimSE);
}
```

# Generic Variants
If you're intending to work on many game types in generic code, then the above code won't work well.  Instead, it looks like:
```cs
public void SomeFunction<TMod>(GameRelease release, ModPath inputPath)
    where TMod : IModGetter
{
    var newMod = ModInstantiator<TMod>.Activator(ModKey.FromFileName("MyMod.esp"), release);

    var importedMod = ModInstantiator<TMod>.Importer(inputPath, release);
}
```
In this way, this function does not need to know whether it's a SkyrimMod, or a Fallout4Mod.  This of course, comes with the downside of only being able to interact with the object as much as the TMod constraint allows.  You would not be able to access the Water group, for example, as it's uncertain if the mod in question has those.

## Getter vs Setter
If the constraint `TMod` is only a Getter interface, then `ModInstantiator<TMod>.Importer` will return a binary overlay underlying object.  If it's a setter constraint, then it must be backed by a fully imported mutable object.
