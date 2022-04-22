using Noggog.WPF;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Mutagen.Bethesda.WPF.Reflection.Fields;

public class EnumerableFormLinkSettingsNodeViewBase : NoggogUserControl<EnumerableFormLinkSettingsVM> { }

/// <summary>
/// Interaction logic for EnumerableFormLinkSettingsNodeView.xaml
/// </summary>
public partial class EnumerableFormLinkSettingsNodeView : EnumerableFormLinkSettingsNodeViewBase
{
    public EnumerableFormLinkSettingsNodeView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
        {
            this.WhenAnyValue(x => x.ViewModel!.Meta.DisplayName)
                .BindTo(this, x => x.SettingNameBlock.Text)
                .DisposeWith(disposable);
            this.WhenAnyValue(x => x.ViewModel!.FocusSettingCommand)
                .BindTo(this, x => x.SettingNameButton.Command)
                .DisposeWith(disposable);
            this.WhenAnyValue(x => x.ViewModel!.Values)
                .BindTo(this, x => x.FormPicker.FormKeys)
                .DisposeWith(disposable);
            this.WhenAnyValue(x => x.ViewModel!.LinkCache)
                .BindTo(this, x => x.FormPicker.LinkCache)
                .DisposeWith(disposable);
            this.WhenAnyValue(x => x.ViewModel!.ScopedTypes)
                .BindTo(this, x => x.FormPicker.ScopedTypes)
                .DisposeWith(disposable);
            this.WhenAnyValue(x => x.ViewModel!.IsFocused)
                .Select((focused) => focused ? double.NaN : 200d)
                .BindTo(this, x => x.FormPicker.Height)
                .DisposeWith(disposable);
        });
    }
}