using Noggog.WPF;
using ReactiveUI;
using System.Reactive.Disposables;

namespace Mutagen.Bethesda.WPF.Reflection.Fields;

public class StaticEnumDictionaryViewBase : NoggogUserControl<EnumDictionarySettingsVM> { }

/// <summary>
/// Interaction logic for StaticEnumDictionaryView.xaml
/// </summary>
public partial class StaticEnumDictionaryView : StaticEnumDictionaryViewBase
{
    public StaticEnumDictionaryView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
        {
            this.WhenAnyValue(x => x.ViewModel!.Meta.DisplayName)
                .BindTo(this, x => x.SettingNameBlock.Text)
                .DisposeWith(disposable);
            this.WhenAnyValue(x => x.ViewModel!.Items)
                .BindTo(this, x => x.TabControl.ItemsSource)
                .DisposeWith(disposable);
            this.Bind(this.ViewModel, vm => vm.Selected, v => v.TabControl.SelectedItem)
                .DisposeWith(disposable);
            this.WhenAnyValue(x => x.ViewModel!.FocusSettingCommand)
                .BindTo(this, x => x.SettingNameButton.Command)
                .DisposeWith(disposable);
        });
    }
}