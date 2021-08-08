using ReactiveUI;
using Noggog.WPF;
using System.Reactive.Disposables;
using System.Windows;
using System.Reactive.Linq;

namespace Mutagen.Bethesda.WPF.Plugins.Order
{
    public class LoadOrderViewBase : NoggogUserControl<ILoadOrderVM> { }

    /// <summary>
    /// Interaction logic for LoadOrderView.xaml
    /// </summary>
    public partial class LoadOrderView : LoadOrderViewBase
    {
        public LoadOrderView()
        {
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.WhenAnyValue(x => x.ViewModel!.LoadOrder)
                    .BindTo(this, x => x.ItemsControl.ItemsSource)
                    .DisposeWith(disposable);

                //this.WhenAnyValue(x => x.MainGrid.IsMouseOver)
                //    .Select(isOver => isOver ? Visibility.Visible : Visibility.Collapsed)
                //    .BindTo(this, x => x.BottomBorder.Visibility)
                //    .DisposeWith(disposable);
            });
        }
    }
}
