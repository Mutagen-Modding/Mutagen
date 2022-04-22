using DynamicData.Binding;
using Noggog;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;

namespace Mutagen.Bethesda.Tests.GUI;

public class PassthroughGroupVM : ViewModel
{
    public static IReadOnlyList<GameRelease> GameReleases { get; } = EnumExt<GameRelease>.Values.ToList();
    public ObservableCollectionExtended<PassthroughVM> Passthroughs { get; } = new ObservableCollectionExtended<PassthroughVM>();

    [Reactive]
    public bool Do { get; set; }

    [Reactive]
    public GameRelease GameRelease { get; set; }

    [Reactive]
    public string NicknameSuffix { get; set; } = string.Empty;

    public ReactiveCommand<Unit, Unit> AddPassthroughCommand { get; }

    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

    public MainVM Parent { get; }

    public PassthroughGroupVM(MainVM vm)
    {
        Parent = vm;
        AddPassthroughCommand = ReactiveCommand.Create(() =>
        {
            Passthroughs.Add(new PassthroughVM(this));
        });
        DeleteCommand = ReactiveCommand.Create(() =>
        {
            vm.Groups.Remove(this);
        });
    }

    public PassthroughGroupVM(MainVM vm, TargetGroup group)
        : this(vm)
    {
        Do = group.Do;
        GameRelease = group.GameRelease;
        NicknameSuffix = group.NicknameSuffix;
        Passthroughs.AddRange(group.Targets.Select(t => new PassthroughVM(this, t)));
    }
}