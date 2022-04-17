using Mutagen.Bethesda.WPF.Reflection.Fields;
using Noggog.WPF;
using ReactiveUI;
using System.Reactive.Disposables;

namespace Mutagen.Bethesda.WPF.Reflection;

public class SettingDepthViewBase : NoggogUserControl<SettingsNodeVM> { }

/// <summary>
/// Interaction logic for SettingDepthView.xaml
/// </summary>
public partial class SettingDepthView : SettingDepthViewBase
{
    public SettingDepthView()
    {
        InitializeComponent();
        this.WhenActivated((disposable) =>
        {
            this.WhenAnyValue(x => x.ViewModel!.Parents.Value)
                .BindTo(this, x => x.ParentSettingList.ItemsSource)
                .DisposeWith(disposable);
        });
    }
}