using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Mutagen.Bethesda.Tests;

namespace Mutagen.Bethesda.Tests.GUI.Views;

public partial class TestBubbleView : UserControl
{
    public static readonly FuncValueConverter<Test?, string?> DisplayNameConverter =
        new(t => t is null ? null : t.FilePath is null ? t.Name : $"{t.Name} {t.FilePath.Value.Name}");

    public TestBubbleView()
    {
        InitializeComponent();
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (DataContext is TestVM { SelectCommand: ICommand cmd } && cmd.CanExecute(null))
        {
            cmd.Execute(null);
        }
    }
}
