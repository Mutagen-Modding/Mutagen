using ReactiveUI;
using Noggog.WPF;
using System.Reactive.Disposables;

namespace Mutagen.Bethesda.WPF.TestDisplay;

public class MainViewBase : NoggogUserControl<MainVM> { }

/// <summary>
/// Interaction logic for MainView.xaml
/// </summary>
public partial class MainView : MainViewBase
{
    public MainView()
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

            this.WhenAnyValue(x => x.ViewModel!.LinkCache)
                .BindTo(this, x => x.FormKeyMultiPicker.LinkCache)
                .DisposeWith(disposable);

            this.WhenAnyValue(x => x.ViewModel!.ScopedTypes)
                .BindTo(this, x => x.FormKeyMultiPicker.ScopedTypes)
                .DisposeWith(disposable);

            this.WhenAnyValue(x => x.ViewModel!.FormKeys)
                .BindTo(this, x => x.FormKeyMultiPicker.FormKeys)
                .DisposeWith(disposable);

            this.WhenAnyValue(x => x.ViewModel!.LoadOrder)
                .BindTo(this, x => x.ModKeyPicker.SearchableMods)
                .DisposeWith(disposable);

            this.WhenAnyValue(x => x.ViewModel!.ModKeys)
                .BindTo(this, x => x.ModKeyMultiPicker.ModKeys)
                .DisposeWith(disposable);

            this.WhenAnyValue(x => x.ViewModel!.LoadOrder)
                .BindTo(this, x => x.ModKeyMultiPicker.SearchableMods)
                .DisposeWith(disposable);

            this.WhenAnyValue(x => x.ViewModel!.LateSetPickerVM)
                .BindTo(this, x => x.LateSetPicker.DataContext)
                .DisposeWith(disposable);
        });
    }
}