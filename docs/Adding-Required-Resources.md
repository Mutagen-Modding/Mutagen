`Mutagen.WPF` is built with style-less controls.  This means the controls are just logic, and you can add your own look exactly how you want it.

Typically though a default look is provided, and `Mutagen.WPF` does this as well.  It is built on top of [MahApps](https://mahapps.com/), and other reusable libraries.  

# Global Import
To import these resources/looks, the easiest way is to modify the `App.xaml` file that comes with every WPF project:
```cs
<Application ...>
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <!--  MahApps.Metro resource dictionaries.  -->
                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml" />
                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml" />
                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Controls.Buttons.xaml" />

                <!--  Purple Accent and AppTheme setting  -->
                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Themes/Dark.Purple.xaml" />

                <!--  Noggog.WPF Theming  -->
                <ResourceDictionary Source="pack://application:,,,/Noggog.WPF;component/Everything.xaml" />

                <!--  Mutagen Specific Theming  -->
                <ResourceDictionary Source="pack://application:,,,/Mutagen.Bethesda.WPF;component/Everything.xaml" />
                
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

# Non-Global Import
You can also add the above ResourceDictionary to specific controls in your WPF app, if you don't want to import/apply them globally to everything.

# Work In Progress
These patterns may be adjusted over time, as they don't allow for the easiest customization of color theming and such.  As better patterns for exposing more control are discovered, these suggestions might change.
