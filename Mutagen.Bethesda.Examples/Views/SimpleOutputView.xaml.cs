using Noggog.WPF;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mutagen.Bethesda.Examples
{
    /// <summary>
    /// Interaction logic for PrintContentView.xaml
    /// </summary>
    public partial class SimpleOutputView : ReactiveUserControl<SimpleOutputVM>
    {
        public SimpleOutputView()
        {
            InitializeComponent();
            this.WhenActivated(dispose =>
            {
                this.WhenAny(x => x.ViewModel.Name)
                    .BindToStrict(this, x => x.NameTextBlock.Text)
                    .DisposeWith(dispose);
                this.WhenAny(x => x.ViewModel.Description)
                    .BindToStrict(this, x => x.DescriptionTextBlock.Text)
                    .DisposeWith(dispose);
                this.WhenAny(x => x.ViewModel.ClearCommand)
                    .BindToStrict(this, x => x.ClearButton.Command)
                    .DisposeWith(dispose);
                this.WhenAny(x => x.ViewModel.RunCommand)
                    .BindToStrict(this, x => x.RunButton.Command)
                    .DisposeWith(dispose);
                this.WhenAny(x => x.ViewModel.OutputDisplay)
                    .BindToStrict(this, x => x.TerminalControl.ItemsSource)
                    .DisposeWith(dispose);
                this.WhenAny(x => x.ViewModel.OutputDisplay.Count)
                    .Select(x => x.ToString())
                    .BindToStrict(this, x => x.CountRun.Text)
                    .DisposeWith(dispose);
                this.WhenAny(x => x.ViewModel.LastTiming)
                    .BindToStrict(this, x => x.TimingRun.Text)
                    .DisposeWith(dispose);
            });
        }
    }
}
