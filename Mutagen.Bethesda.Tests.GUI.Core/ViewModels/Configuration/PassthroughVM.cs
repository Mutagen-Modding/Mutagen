using Noggog.UI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace Mutagen.Bethesda.Tests.GUI;

public class PassthroughVM : ViewModel
{
    public PathPickerVM Path { get; }

    [Reactive]
    public bool Do { get; set; } = true;

    public PassthroughGroupVM Parent { get; }

    private readonly ObservableAsPropertyHelper<bool> _Doing;
    public bool Doing => _Doing.Value;

    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

    public PassthroughVM(PassthroughGroupVM group)
    {
        Parent = group;
        Path = new PathPickerVM(group.Parent.SchedulerProvider, group.Parent.PathPickerDialogProvider)
        {
            ExistCheckOption = PathPickerVM.CheckOptions.On,
            PathType = PathPickerVM.PathTypeOptions.File,
        };
        this.WhenAnyValue(
                x => x.Do,
                x => x.Parent.Do,
                (c, p) => c && p)
            .ToRxAppGuiProperty(this, nameof(Doing), out _Doing);
        DeleteCommand = ReactiveCommand.Create(() =>
        {
            group.Passthroughs.Remove(this);
        });
    }

    public PassthroughVM(PassthroughGroupVM group, Target target)
        : this(group)
    {
        Do = target.Do;
        Path.TargetPath = target.Path;
    }
}