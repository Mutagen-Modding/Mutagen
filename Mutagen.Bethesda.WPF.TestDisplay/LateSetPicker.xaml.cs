using ReactiveUI;
using Noggog.WPF;
using System.Reactive.Disposables;

namespace Mutagen.Bethesda.WPF.TestDisplay;

public class LateSetPickerBase : NoggogUserControl<LateSetPickerVM> { }

/// <summary>
/// Interaction logic for LateSetPicker.xaml
/// </summary>
public partial class LateSetPicker : LateSetPickerBase
{
    public LateSetPicker()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
        {
            this.Bind(this.ViewModel, vm => vm.FormKey, view => view.FormKeyPicker.FormKey)
                .DisposeWith(disposable);

            this.WhenAnyValue(x => x.ViewModel!.LinkCache)
                .BindTo(this, x => x.FormKeyPicker.LinkCache)
                .DisposeWith(disposable);

            this.WhenAnyValue(x => x.ViewModel!.ScopedTypes)
                .BindTo(this, x => x.FormKeyPicker.ScopedTypes)
                .DisposeWith(disposable);

            this.WhenAnyValue(x => x.ViewModel!.SetCommand)
                .BindTo(this, x => x.SetButton.Command)
                .DisposeWith(disposable);
        });
    }
}