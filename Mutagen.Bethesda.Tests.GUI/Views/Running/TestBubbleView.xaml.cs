using Noggog;
using Noggog.WPF;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

namespace Mutagen.Bethesda.Tests.GUI.Views;

public class TestBubbleViewBase : ReactiveUserControl<TestVM> { }

/// <summary>
/// Interaction logic for TestBubbleView.xaml
/// </summary>
public partial class TestBubbleView : TestBubbleViewBase
{
    public TestBubbleView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
        {
            this.WhenAnyFallback(x => x.ViewModel!.Test)
                .Select(t =>
                {
                    if (t.FilePath == null) return t.Name;
                    return $"{t.Name} {t.FilePath.Value.Name}";
                })
                .BindTo(this, x => x.Name.Text)
                .DisposeWith(disposable);
            this.TopBorder.Events().MouseUp
                .Unit()
                .InvokeCommand(this, x => x.ViewModel!.SelectCommand)
                .DisposeWith(disposable);
        });
    }
}