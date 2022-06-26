using Noggog.WPF;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Mutagen.Bethesda.WPF.Reflection.Fields;

public class EnumerableModKeySettingsNodeViewBase : NoggogUserControl<EnumerableModKeySettingsVM> { }

/// <summary>
/// Interaction logic for EnumerableModKeySettingsNodeView.xaml
/// </summary>
public partial class EnumerableModKeySettingsNodeView : EnumerableModKeySettingsNodeViewBase
{
    public EnumerableModKeySettingsNodeView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
        {
            this.WhenAnyValue(x => x.ViewModel!.Meta.DisplayName)
                .BindTo(this, x => x.SettingsNameBlock.Text)
                .DisposeWith(disposable);
            this.WhenAnyValue(x => x.ViewModel!.Values)
                .BindTo(this, x => x.RequiredModsPicker.ModKeys)
                .DisposeWith(disposable);
            this.WhenAnyValue(x => x.ViewModel!.DetectedLoadOrder)
                .BindTo(this, x => x.RequiredModsPicker.SearchableMods)
                .DisposeWith(disposable);
            this.WhenAnyValue(x => x.ViewModel!.IsFocused)
                .Select((focused) => focused ? double.NaN : 200d)
                .BindTo(this, x => x.RequiredModsPicker.Height)
                .DisposeWith(disposable);
        });
    }
}