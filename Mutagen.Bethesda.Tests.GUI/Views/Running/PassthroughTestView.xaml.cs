using Noggog;
using Noggog.WPF;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

namespace Mutagen.Bethesda.Tests.GUI.Views
{
    public class PassthroughTestViewBase : ReactiveUserControl<PassthroughTestVM> { }

    /// <summary>
    /// Interaction logic for PassthroughTestView.xaml
    /// </summary>
    public partial class PassthroughTestView : PassthroughTestViewBase
    {
        public PassthroughTestView()
        {
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.WhenAnyFallback(x => x.ViewModel!.Name)
                    .BindTo(this, x => x.Name.Text)
                    .DisposeWith(disposable);
                this.TopBorder.Events().MouseUp
                    .Unit()
                    .InvokeCommand(this, x => x.ViewModel!.SelectCommand)
                    .DisposeWith(disposable);
                this.WhenAnyFallback(x => x.ViewModel!.TimeSpent)
                    .Select(x =>
                    {
                        if (x == null) return null;
                        return $"{x.Value.TotalMinutes:n2}m";
                    })
                    .BindTo(this, x => x.TimeSpent.Text)
                    .DisposeWith(disposable);
            });
        }
    }
}
