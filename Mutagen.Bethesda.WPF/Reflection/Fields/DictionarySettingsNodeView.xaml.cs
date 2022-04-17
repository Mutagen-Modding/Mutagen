using Noggog.WPF;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

namespace Mutagen.Bethesda.WPF.Reflection.Fields;

public class DictionarySettingsNodeViewBase : NoggogUserControl<DictionarySettingsVM> { }

/// <summary>
/// Interaction logic for DictionarySettingsNodeView.xaml
/// </summary>
public partial class DictionarySettingsNodeView : DictionarySettingsNodeViewBase
{
    public DictionarySettingsNodeView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
        {
            this.WhenAnyValue(x => x.ViewModel!.FocusSettingCommand)
                .BindTo(this, x => x.SettingNameButton.Command)
                .DisposeWith(disposable);
            this.WhenAnyValue(x => x.ViewModel!.Meta.DisplayName)
                .BindTo(this, x => x.SettingsNameBlock.Text)
                .DisposeWith(disposable);
            this.WhenAnyValue(x => x.ViewModel!.Items)
                .BindTo(this, x => x.TabControl.ItemsSource)
                .DisposeWith(disposable);
            this.Bind(this.ViewModel, vm => vm.Selected, v => v.TabControl.SelectedItem)
                .DisposeWith(disposable);

            this.WhenAnyValue(x => x.ViewModel!.AddCommand)
                .BindTo(this, x => x.AddButton.Command)
                .DisposeWith(disposable);
            this.WhenAnyValue(x => x.ViewModel!.DeleteCommand)
                .BindTo(this, x => x.DeleteButton.Command)
                .DisposeWith(disposable);
            this.WhenAnyValue(x => x.ViewModel!.ConfirmCommand)
                .BindTo(this, x => x.ConfirmButton.Command)
                .DisposeWith(disposable);
            this.WhenAnyValue(x => x.ViewModel!.MidDelete)
                .Select(x => x ? Visibility.Visible : Visibility.Collapsed)
                .BindTo(this, x => x.ConfirmButton.Visibility)
                .DisposeWith(disposable);

            this.Bind(this.ViewModel, vm => vm.AddPaneText, v => v.AddNewPaneBox.Text)
                .DisposeWith(disposable);
        });
    }
}