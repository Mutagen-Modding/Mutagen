using ReactiveUI;
using Noggog.WPF;
using System.Reactive.Disposables;

namespace Mutagen.Bethesda.WPF.TestDisplay
{
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
                this.BindStrict(this.ViewModel, vm => vm.FormKey, view => view.FormKeyPicker.FormKey)
                    .DisposeWith(disposable);

                this.WhenAnyValue(x => x.ViewModel!.LinkCache)
                    .BindToStrict(this, x => x.FormKeyPicker.LinkCache)
                    .DisposeWith(disposable);

                this.WhenAnyValue(x => x.ViewModel!.ScopedTypes)
                    .BindToStrict(this, x => x.FormKeyPicker.ScopedTypes)
                    .DisposeWith(disposable);

                this.WhenAnyValue(x => x.ViewModel!.LinkCache)
                    .BindToStrict(this, x => x.FormKeyMultiPicker.LinkCache)
                    .DisposeWith(disposable);

                this.WhenAnyValue(x => x.ViewModel!.ScopedTypes)
                    .BindToStrict(this, x => x.FormKeyMultiPicker.ScopedTypes)
                    .DisposeWith(disposable);

                this.WhenAnyValue(x => x.ViewModel!.FormKeys)
                    .BindToStrict(this, x => x.FormKeyMultiPicker.FormKeys)
                    .DisposeWith(disposable);

                this.WhenAnyValue(x => x.ViewModel!.LoadOrder)
                    .BindToStrict(this, x => x.ModKeyPicker.SearchableMods)
                    .DisposeWith(disposable);

                this.WhenAnyValue(x => x.ViewModel!.ModKeys)
                    .BindToStrict(this, x => x.ModKeyMultiPicker.ModKeys)
                    .DisposeWith(disposable);

                this.WhenAnyValue(x => x.ViewModel!.LoadOrder)
                    .BindToStrict(this, x => x.ModKeyMultiPicker.SearchableMods)
                    .DisposeWith(disposable);

                this.WhenAnyValue(x => x.ViewModel!.LateSetPickerVM)
                    .BindToStrict(this, x => x.LateSetPicker.DataContext)
                    .DisposeWith(disposable);
            });
        }
    }
}
