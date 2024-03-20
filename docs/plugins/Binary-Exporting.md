## Naive Export
A basic mod write call is pretty straightforward:
```cs
SkyrimMod mod = ...;

mod.WriteToBinary(desiredFilePath);
```

!!! warning "Master Export Preferences Not Provided"
    This is often not desired default behavior, depending on the master content.

## Typical Export
The recommended call for exporting a mod passes the load order to sort with as input:
```cs
SkyrimMod mod = ...;

mod.WriteToBinary(
    desiredFilePath,
    new BinaryWriteParameters()
    {
        MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrder)
    });
```

This extra information helps keep the masters in proper order, as the load order information is provided for the write call to use

## No Master Changes
Sometimes you have adjusted the masters yourself, or perhaps are just re-exporting a mod with no changes, and know the masters list is proper.

In this case, you can override the default behavior to leave the masters alone:
```cs
SkyrimMod mod = ...;

mod.WriteToBinary(
    desiredFilePath,
    new BinaryWriteParameters()
    {
        MastersListOrdering = new MastersListOrderingEnumOption()
        {
            Option = MastersListOrderingOption.NoCheck
        },
        MastersListContent = MastersListContentOption.NoCheck
    };
```
This instructs it to neither replace the content, nor reorder the masters.
