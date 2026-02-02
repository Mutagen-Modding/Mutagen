using DynamicData.Binding;
using Noggog.WorkEngine;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Mutagen.Bethesda.Tests.GUI;

public class RunningTestsVM : ViewModel
{
    public ObservableCollectionExtended<GroupTestVM> Groups { get; } = new ObservableCollectionExtended<GroupTestVM>();

    [Reactive]
    public Exception? Error { get; set; }

    [Reactive]
    public PassthroughTestVM? SelectedPassthrough { get; set; }

    private readonly ObservableAsPropertyHelper<TestVM?> _SelectedTest;
    public TestVM? SelectedTest => _SelectedTest.Value;

    public RunningTestsVM()
    {
        _SelectedTest = this.WhenAnyValue(x => x.SelectedPassthrough!.SelectedTest)
            .ToRxAppGuiProperty(this, nameof(SelectedTest), default);
    }

    public async Task Run(MainVM mvm)
    {
        var dropoff = new WorkDropoff();
        using var consumer = new WorkConsumer(
            new NumWorkThreadsConstant(null),
            dropoff, dropoff);
        consumer.Start();
        Groups.AddRange(mvm.Groups
            .Where(g => g.Do)
            .Where(g => g.Passthroughs.Select(p => p.Do).Any())
            .Select(g => new GroupTestVM(this, g)));
        await Task.WhenAll(Groups
            .SelectMany(g => g.Passthroughs.Items)
            .Select(t => t.Run(dropoff)));
    }
}