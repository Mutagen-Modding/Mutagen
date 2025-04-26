using DynamicData;
using DynamicData.Binding;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Noggog;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Tests.GUI;

public class PassthroughTestVM : ViewModel
{
    public GroupTestVM Group { get; }
    public PassthroughVM Settings { get; }

    public SourceList<TestVM> Tests = new();
    private readonly IObservableCollection<TestVM> _testsDisplay;
    public IObservableCollection<TestVM> TestsDisplay => _testsDisplay;

    private readonly ObservableAsPropertyHelper<string> _Name;
    public string Name => _Name.Value;

    public ReactiveCommand<Unit, Unit> SelectCommand { get; }

    private readonly ObservableAsPropertyHelper<bool> _IsSelected;
    public bool IsSelected => _IsSelected.Value;

    [Reactive]
    public TestVM? SelectedTest { get; set; }

    private readonly ObservableAsPropertyHelper<TestState> _State;
    public TestState State => _State.Value;

    [Reactive]
    public TimeSpan? TimeSpent { get; private set; }
    
    public ICommand OpenFileLocationCommand { get; }

    public PassthroughTestVM(GroupTestVM group, PassthroughVM p)
    {
        Group = group;
        Settings = p;
        _Name = this.WhenAnyValue(x => x.Settings.Path.TargetPath)
            .Select(path =>
            {
                try
                {
                    return Path.GetFileName(path);
                }
                catch (Exception)
                {
                    return path;
                }
            })
            .ToGuiProperty(this, nameof(Name), string.Empty);
        Tests.Connect()
            .Bind(out _testsDisplay)
            .Subscribe()
            .DisposeWith(this);

        SelectCommand = ReactiveCommand.Create(() =>
        {
            Group.Parent.SelectedPassthrough = this;
        });
        _IsSelected = Group.Parent.WhenAnyValue(x => x.SelectedPassthrough)
            .Select(x => x == this)
            .ToGuiProperty(this, nameof(IsSelected));
        _State = Tests.Connect()
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
        OpenFileLocationCommand = ReactiveCommand.Create(
            () =>
            {
                try
                {
                    Process.Start(new ProcessStartInfo(PassthroughTest.GetTestFolderPath(Name))
                    {
                        UseShellExecute = true,
                    });
                }
                catch (Exception)
                {
                }
            });
    }

    public async Task Run()
    {
        var dropoff = new WorkDropoff();
        using var consumer = new WorkConsumer(
            new NumWorkThreadsConstant(null),
            dropoff, dropoff);
        List<Test> tests = new List<Test>();
        var passthroughSettings = Settings.Parent.Parent.GetPassthroughSettings();
        var passthrough = PassthroughTest.Factory(new PassthroughTestParams()
        {
            WorkDropoff = dropoff,
            NicknameSuffix = Settings.Parent.NicknameSuffix,
            PassthroughSettings = passthroughSettings,
            Target = new Target()
            {
                Do = true,
                Path = Settings.Path.TargetPath
            },
            GameRelease = Settings.Parent.GameRelease,
        });
        if (passthroughSettings.HasAnyToRun)
        {
            tests.Add(AddTest(passthrough.BinaryPassthroughTest()));
        }
        if (Settings.Parent.Parent.TestEquals)
        {
            tests.Add(AddTest(passthrough.TestEquality()));
        }
        consumer.Start();
        Stopwatch sw = new Stopwatch();
        sw.Start();
        await Task.WhenAll(tests.Select(t => dropoff.EnqueueAndWait(t.Start)));
        sw.Stop();
        TimeSpent = sw.Elapsed;
    }

    private Test AddTest(Test test, TestVM? parent = null)
    {
        var vm = new TestVM(this, test, parent);
        Tests.Add(vm);
        foreach (var child in test.Children)
        {
            AddTest(child, vm);
        }
        return test;
    }
}