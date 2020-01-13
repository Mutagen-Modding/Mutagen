using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System;
using Noggog.WPF;

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
                this.WhenAny(x => x.ViewModel.ModFilePath)
                    .BindTo(this, x => x.ModFilePicker.PickerVM)
                    .DisposeWith(disposable);
                this.WhenAny(x => x.ViewModel.Examples)
                    .BindTo(this, x => x.ExamplesListBox.ItemsSource)
                    .DisposeWith(disposable);
                this.Bind(this.ViewModel, x => x.SelectedExample, x => x.ExamplesListBox.SelectedValue)
                    .DisposeWith(disposable);
                this.WhenAny(x => x.ViewModel.SelectedExample)
                    .BindTo(this, x => x.SelectedExampleContent.Content)
                    .DisposeWith(disposable);
            });
        }
    }
}
