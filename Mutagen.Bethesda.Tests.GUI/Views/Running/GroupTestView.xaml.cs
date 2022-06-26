using Noggog.WPF;
using ReactiveUI;
using System.Reactive.Disposables;

namespace Mutagen.Bethesda.Tests.GUI.Views;

public class GroupTestViewBase : ReactiveUserControl<GroupTestVM> { }

/// <summary>
/// Interaction logic for GroupTestView.xaml
/// </summary>
public partial class GroupTestView : GroupTestViewBase
{
    public GroupTestView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
        {
            this.WhenAnyFallback(x => x.ViewModel!.Name)
                .BindTo(this, x => x.Name.Text)
                .DisposeWith(disposable);
            this.WhenAnyFallback(x => x.ViewModel!.PassthroughDisplay)
                .BindTo(this, x => x.PassthroughsControl.ItemsSource)
                .DisposeWith(disposable);
        });
    }
}