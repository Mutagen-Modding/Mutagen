using Noggog.WPF;
using ReactiveUI;
using System.Reactive.Disposables;

namespace Mutagen.Bethesda.WPF.Reflection.Fields
{
    public class EnumSettingsNodeViewBase : NoggogUserControl<EnumSettingsVM> { }

    /// <summary>
    /// Interaction logic for EnumSettingsNodeView.xaml
    /// </summary>
    public partial class EnumSettingsNodeView : EnumSettingsNodeViewBase
    {
        public EnumSettingsNodeView()
        {
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.WhenAnyValue(x => x.ViewModel!.EnumNames)
                    .BindTo(this, view => view.Combobox.ItemsSource)
                    .DisposeWith(disposable);
                this.Bind(ViewModel, vm => vm.Value, view => view.Combobox.SelectedValue)
                    .DisposeWith(disposable);
            });
        }
    }
}
