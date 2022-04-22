using Noggog.WPF;
using ReactiveUI;
using System.Reactive.Disposables;

namespace Mutagen.Bethesda.Tests.GUI.Views;

public class TestResultsViewBase : ReactiveUserControl<RunningTestsVM> { }

/// <summary>
/// Interaction logic for TestResultsView.xaml
/// </summary>
public partial class TestResultsView : TestResultsViewBase
{
    public TestResultsView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
        {
            this.WhenAnyValue(x => x.ViewModel!.Groups)
                .BindTo(this, x => x.PassthroughGroupsList.ItemsSource)
                .DisposeWith(disposable);
            this.WhenAnyFallback(x => x.ViewModel!.SelectedPassthrough!.TestsDisplay)
                .BindTo(this, x => x.SelectedTestsControl.ItemsSource)
                .DisposeWith(disposable);
            this.WhenAnyValue(x => x.ViewModel!.SelectedTest!.Output)
                .BindTo(this, x => x.TerminalControl.ItemsSource)
                .DisposeWith(disposable);
        });
    }
}