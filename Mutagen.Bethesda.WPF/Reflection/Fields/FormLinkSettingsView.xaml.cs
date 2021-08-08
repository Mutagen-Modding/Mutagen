using Noggog.WPF;
using ReactiveUI;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Mutagen.Bethesda.WPF.Reflection.Fields
{
    public class FormLinkSettingsViewBase : NoggogUserControl<FormLinkSettingsVM> { }

    /// <summary>
    /// Interaction logic for FormLinkSettingsView.xaml
    /// </summary>
    public partial class FormLinkSettingsView : FormLinkSettingsViewBase
    {
        public FormLinkSettingsView()
        {
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.WhenAnyValue(x => x.ViewModel!.Meta.DisplayName)
                    .BindTo(this, x => x.SettingsNameBox.Text)
                    .DisposeWith(disposable);
                this.Bind(this.ViewModel, x => x.Value, x => x.FormPicker.FormKey)
                    .DisposeWith(disposable);
                this.WhenAnyValue(x => x.ViewModel!.LinkCache)
                    .BindTo(this, x => x.FormPicker.LinkCache)
                    .DisposeWith(disposable);
                this.WhenAnyValue(x => x.ViewModel!.ScopedTypes)
                    .BindTo(this, x => x.FormPicker.ScopedTypes)
                    .DisposeWith(disposable);
                this.WhenAnyValue(x => x.ViewModel!.IsFocused)
                    .Select((focused) => focused ? double.NaN : 200d)
                    .BindTo(this, x => x.FormPicker.SearchBoxHeight)
                    .DisposeWith(disposable);
            });
        }
    }
}
