The ModKey Picker helps users select mod(s) by typing in their names.

The picker can reference certain objects to know what mods actually exist on a user's active load order:
- A [[Load Order]] object
- Any enumerable of type `ModKey` or `IModListingGetter`
- An `IObservable<IChangeSet<T>>` of type `ModKey` or `IModListingGetter`. (Reactive Extension concepts)

Make sure you've added the [Required Resources](Adding-Required-Resources), or the controls will not have any display.

The Mutagen [Test Display](https://github.com/Mutagen-Modding/Mutagen/tree/release/Mutagen.Bethesda.WPF.TestDisplay) app utilizes the ModKey pickers, and provides a good example of how to use them from within a WPF app.

# ModKey Picker
This is a picker to select a single ModKey.

![ModKey Picker](https://i.imgur.com/FYT1EDq.gif)

## View Side
```cs
<UserControl
    ...
    xmlns:plugins="clr-namespace:Mutagen.Bethesda.WPF.Plugins;assembly=Mutagen.Bethesda.WPF" >
    <plugins:ModKeyPicker 
        ModKey="{Binding MyModKey}"
        SearchableMods="{Binding MyAvailableMods}" />
</UserControl>
```

## ViewModel Side
```cs
// Some mechanics shown here are from `ReactiveUI`, or `Noggog.WPF`
public class MyViewModel : ViewModel
{
    [Reactive]
    public ModKey MyModKey { get; set; }

    public object MyAvailableMods { get; }

    public MyViewModel()
    {
        // Create a GameEnvironment, to get a LoadOrder connected to the current users' setup
        var env = GameEnvironment.Typical.Skyrim(
            SkyrimRelease.SkyrimSE, 
            // By passing in this preference, we only cache FormKey/EditorID info, keeping memory usage down
            LinkCachePreferences.OnlyIdentifiers());

        // When the ViewModel is destroyed, clean up the environment object.  Good practice
        env.DisposeWith(this);

        // Set the ViewModel's available mods to the LoadOrder given by the environment
        MyAvailableMods = env.LoadOrder;

        // User can now set the ModKey member, and the viewmodel can see the results
    }
}
```

# ModKey Multipicker
This is a picker to select any number of ModKeys.

![ModKey Multipicker](https://i.imgur.com/TNnR53A.gif)

## View Side
```cs
<UserControl
    ...
    xmlns:plugins="clr-namespace:Mutagen.Bethesda.WPF.Plugins;assembly=Mutagen.Bethesda.WPF" >
    <plugins:ModKeyMultiPicker
        ModKeys="{Binding MyModKeys}"
        SearchableMods="{Binding MyAvailableMods}" />
</UserControl>
```

## ViewModel Side
```cs
// Some mechanics shown here are from `ReactiveUI`, or `Noggog.WPF`
public class MyViewModel : ViewModel
{
    // ModKeyItemViewModel provided by Mutagen.WPF
    public ObservableCollection<ModKeyItemViewModel> MyModKeys { get; } = new();

    public object MyAvailableMods { get; }

    public MyViewModel()
    {
        // Create a GameEnvironment, to get a LoadOrder connected to the current users' setup
        var env = GameEnvironment.Typical.Skyrim(
            SkyrimRelease.SkyrimSE, 
            // By passing in this preference, we only cache FormKey/EditorID info, keeping memory usage down
            LinkCachePreferences.OnlyIdentifiers());

        // When the ViewModel is destroyed, clean up the environment object.  Good practice
        env.DisposeWith(this);

        // Set the ViewModel's available mods to the LoadOrder given by the environment
        MyAvailableMods = env.LinkCache;

        // User can now set the MyModKeys member, and the viewmodel can see the results
    }
}
```
