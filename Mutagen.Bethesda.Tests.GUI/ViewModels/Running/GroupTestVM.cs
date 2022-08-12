using System.Diagnostics;
using DynamicData;
using DynamicData.Binding;
using Noggog.WPF;
using ReactiveUI;
using System.Reactive.Linq;
using Noggog;

namespace Mutagen.Bethesda.Tests.GUI;

public class GroupTestVM : ViewModel
{
    public RunningTestsVM Parent { get; }

    public SourceList<PassthroughTestVM> Passthroughs = new();
    private readonly ObservableCollectionExtended<PassthroughTestVM> _passthroughDisplay = new();
    public IObservableCollection<PassthroughTestVM> PassthroughDisplay => _passthroughDisplay;

    private readonly ObservableAsPropertyHelper<string> _name;
    public string Name => _name.Value;

    public PassthroughGroupVM Settings { get; }

    private readonly ObservableAsPropertyHelper<TestState> _state;
    public TestState State => _state.Value;

    public GroupTestVM(RunningTestsVM parent, PassthroughGroupVM group)
    {
        Parent = parent;
        Settings = group;
        Passthroughs.AddRange(group.Passthroughs
            .Where(p => p.Do)
            .Select(p => new PassthroughTestVM(this, p)));
        Passthroughs.Connect()
            .ObserveOnGui()
            .Bind(_passthroughDisplay)
            .Subscribe()
            .DisposeWith(this);
        _name = this.WhenAnyValue(x => x.Settings.GameRelease)
            .Select(g => g.ToString())
            .ToGuiProperty(this, nameof(Name), string.Empty);
        _state = Passthroughs.Connect()
            .TransformMany(x => x.Tests)
            .AutoRefresh(x => x.State)
            .Transform(p => p.State, transformOnRefresh: true)
            .QueryWhenChanged(states =>
            {
                bool notComplete = false;
                foreach (var state in states)
                {
                    if (state == TestState.Error) return TestState.Error;
                    if (state != TestState.Complete)
                    {
                        notComplete = true;
                    }
                }
                return notComplete ? TestState.Running : TestState.Complete;
            })
            .ToGuiProperty(this, nameof(State));
    }
}