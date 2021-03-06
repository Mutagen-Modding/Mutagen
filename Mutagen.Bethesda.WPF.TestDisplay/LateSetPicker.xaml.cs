using ReactiveUI;
using Noggog.WPF;
using System.Reactive.Disposables;

namespace Mutagen.Bethesda.WPF.TestDisplay
{
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
                this.BindStrict(this.ViewModel, vm => vm.FormKey, view => view.FormKeyPicker.FormKey)
                    .DisposeWith(disposable);

                this.WhenAnyValue(x => x.ViewModel!.LinkCache)
                    .BindToStrict(this, x => x.FormKeyPicker.LinkCache)
                    .DisposeWith(disposable);

                this.WhenAnyValue(x => x.ViewModel!.ScopedTypes)
                    .BindToStrict(this, x => x.FormKeyPicker.ScopedTypes)
                    .DisposeWith(disposable);

                this.WhenAnyValue(x => x.ViewModel!.SetCommand)
                    .BindToStrict(this, x => x.SetButton.Command)
                    .DisposeWith(disposable);
            });
        }
    }
}
