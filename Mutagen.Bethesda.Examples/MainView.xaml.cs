using ReactiveUI;
using Noggog.WPF;
using System.Reactive.Disposables;

namespace Mutagen.Bethesda.Examples
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : ReactiveUserControl<MainVM>
    {
        public MainView()
        {
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.OneWayBindStrict(this.ViewModel, x => x.ModFilePath, x => x.ModFilePicker.PickerVM)
                    .DisposeWith(disposable);
            });
        }
    }
}
