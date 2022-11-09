The FormKey Picker helps users select record(s) by typing in:
- EditorIDs
- FormKeys
- FormIDs (Mod indices relative to current load order)

The picker can reference a [[LinkCache]] to do lookups to help display and locate records that actually exist in the user's active load order.

They can also be scoped to only allow or search for specific record types, if desired.

Make sure you've added the [Required Resources](Adding-Required-Resources), or the controls will not have any display.

The Mutagen [Test Display](https://github.com/Mutagen-Modding/Mutagen/tree/release/Mutagen.Bethesda.WPF.TestDisplay) app utilizes the FormKey pickers, and provides a good example of how to use them from within a WPF app.

# FormKey Picker
This is a picker to select a single FormKey.

![FormKey Picker](https://i.imgur.com/gtlg5Md.gif)

## View Side
```cs
<UserControl
    ...
    xmlns:plugins="clr-namespace:Mutagen.Bethesda.WPF.Plugins;assembly=Mutagen.Bethesda.WPF" >
    <plugins:FormKeyPicker 
        FormKey="{Binding MyFormKey}"
        LinkCache="{Binding MyLinkCache}"
        ScopedTypes="{Binding MyDesiredTypes}" />
</UserControl>
```

## ViewModel Side
```cs
// Some mechanics shown here are from `ReactiveUI`, or `Noggog.WPF`
public class MyViewModel : ViewModel
{
    [Reactive]
    public FormKey MyFormKey { get; set; }

    public ILinkCache MyLinkCache { get; }

    public IEnumerable<Type> MyDesiredTypes { get; }

    public MyViewModel()
    {
        // Create a GameEnvironment, to get a LinkCache connected to the current users' setup
        var env = GameEnvironment.Typical.Skyrim(
            SkyrimRelease.SkyrimSE, 
            // By passing in this preference, we only cache FormKey/EditorID info, keeping memory usage down
            LinkCachePreferences.OnlyIdentifiers());

        // When the ViewModel is destroyed, clean up the environment object.  Good practice
        env.DisposeWith(this);

        // Set the ViewModel's LinkCache to the one given by the environment
        LinkCache = env.LinkCache;

        // We only care about Armor records
        ScopedTypes = typeof(IArmorGetter).AsEnumerable();

        // User can now set the FormKey member, and the viewmodel can see the results
    }
}
```

# FormKey Multipicker
This is a picker to select any number of FormKeys.

![FormKey Multipicker](https://i.imgur.com/PlVXxu5.gif)

## View Side
```cs
<UserControl
    ...
    xmlns:plugins="clr-namespace:Mutagen.Bethesda.WPF.Plugins;assembly=Mutagen.Bethesda.WPF" >
    <plugins:FormKeyMultiPicker
        FormKeys="{Binding MyFormKeys}"
        LinkCache="{Binding MyLinkCache}"
        ScopedTypes="{Binding MyDesiredTypes}" />
</UserControl>
```

## ViewModel Side
```cs
// Some mechanics shown here are from `ReactiveUI`, or `Noggog.WPF`
public class MyViewModel : ViewModel
{
    // FormKeyItemViewModel provided by Mutagen.WPF
    public ObservableCollection<FormKeyItemViewModel> MyFormKeys { get; } = new();

    public ILinkCache MyLinkCache { get; }

    public IEnumerable<Type> MyDesiredTypes { get; }

    public MyViewModel()
    {
        // Create a GameEnvironment, to get a LinkCache connected to the current users' setup
        var env = GameEnvironment.Typical.Skyrim(
            SkyrimRelease.SkyrimSE, 
            // By passing in this preference, we only cache FormKey/EditorID info, keeping memory usage down
            LinkCachePreferences.OnlyIdentifiers());

        // When the ViewModel is destroyed, clean up the environment object.  Good practice
        env.DisposeWith(this);

        // Set the ViewModel's LinkCache to the one given by the environment
        LinkCache = env.LinkCache;

        // We only care about Armor records
        ScopedTypes = typeof(IArmorGetter).AsEnumerable();

        // User can now fill FormKeys collection, and the viewmodel can see the results
    }
}
```