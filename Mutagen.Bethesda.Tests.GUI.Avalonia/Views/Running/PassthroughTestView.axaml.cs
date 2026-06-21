using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Input;

namespace Mutagen.Bethesda.Tests.GUI.Views;

public partial class PassthroughTestView : UserControl
{
    public static readonly FuncValueConverter<TimeSpan?, string?> MinutesConverter =
        new(ts => ts is { } t ? $"{t.TotalMinutes:n2}m" : null);

    public PassthroughTestView()
    {
        InitializeComponent();
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (DataContext is PassthroughTestVM { SelectCommand: ICommand cmd } && cmd.CanExecute(null))
        {
            cmd.Execute(null);
        }
    }
}
