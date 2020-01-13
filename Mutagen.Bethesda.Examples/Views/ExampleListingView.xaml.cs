using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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
    /// Interaction logic for ExampleListingView.xaml
    /// </summary>
    public partial class ExampleListingView : ReactiveUserControl<ExampleVM>
    {
        public ExampleListingView()
        {
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.WhenAny(x => x.ViewModel.Name)
                    .BindTo(this, x => x.TextBlock.Text)
                    .DisposeWith(disposable);
                this.WhenAny(x => x.ViewModel.Description)
                    .BindTo(this, x => x.TextBlock.ToolTip)
                    .DisposeWith(disposable);
            });
        }
    }
}
