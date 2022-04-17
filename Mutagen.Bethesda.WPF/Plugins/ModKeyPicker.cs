using Mutagen.Bethesda.Plugins;
using ReactiveUI;
using System.Windows;

namespace Mutagen.Bethesda.WPF.Plugins;

public class ModKeyPicker : AModKeyPicker
{
    public double MaxSearchBoxHeight
    {
        get => (double)GetValue(MaxSearchBoxHeightProperty);
        set => SetValue(MaxSearchBoxHeightProperty, value);
    }
    public static readonly DependencyProperty MaxSearchBoxHeightProperty = DependencyProperty.Register(nameof(MaxSearchBoxHeight), typeof(double), typeof(ModKeyPicker),
        new FrameworkPropertyMetadata(1000d));

    public double SearchBoxHeight
    {
        get => (double)GetValue(SearchBoxHeightProperty);
        set => SetValue(SearchBoxHeightProperty, value);
    }
    public static readonly DependencyProperty SearchBoxHeightProperty = DependencyProperty.Register(nameof(SearchBoxHeight), typeof(double), typeof(ModKeyPicker),
        new FrameworkPropertyMetadata(double.NaN));

    public ModKeyPicker()
    {
        PickerClickCommand = ReactiveCommand.Create((object o) =>
        {
            switch (o)
            {
                case ModKey modKey:
                    ModKey = modKey;
                    InSearchMode = false;
                    break;
                default:
                    break;
            }
        });
    }
}