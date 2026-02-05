using DynamicData;
using DynamicData.Binding;
using Noggog.WPF;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using Noggog;

namespace Mutagen.Bethesda.Tests.GUI;

public class TestVM : ViewModel
{
    public Test Test { get; }
    public PassthroughTestVM Passthrough { get; }
    public TestVM? Parent { get; }

    private IObservableCollection<string> _output;
    public IObservableCollection<string> Output => _output;

    private readonly ObservableAsPropertyHelper<TestState> _State;
    public TestState State => _State.Value;

    public ReactiveCommand<Unit, Unit> SelectCommand { get; }

    private readonly ObservableAsPropertyHelper<bool> _IsSelected;
    public bool IsSelected => _IsSelected.Value;

    public TestVM(PassthroughTestVM passthrough, Test t, TestVM? parent)
    {
        Test = t;
        Passthrough = passthrough;
        Parent = parent;
        Test.Output
            .SelectMany(x => x.Split(Environment.NewLine))
            .ToObservableChangeSet()
            .ObserveOnRxAppGui()
            .Bind(out _output)
            .Subscribe()
            .DisposeWith(this);
        _State = Test.StateSignal
            .Catch(Observable.Return(TestState.Error))
            .ToRxAppGuiProperty(this, nameof(State));
        SelectCommand = ReactiveCommand.Create(() =>
        {
            Passthrough.SelectedTest = this;
        });
        _IsSelected = passthrough.WhenAnyValue(x => x.SelectedTest)
            .Select(x => x == this)
            .ToRxAppGuiProperty(this, nameof(IsSelected));
    }
}