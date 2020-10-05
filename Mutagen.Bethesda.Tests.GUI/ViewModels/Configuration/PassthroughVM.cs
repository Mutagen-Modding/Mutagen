using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests.GUI
{
    public class PassthroughVM : ViewModel
    {
        public PathPickerVM Path { get; } = new PathPickerVM()
        {
            ExistCheckOption = PathPickerVM.CheckOptions.On,
            PathType = PathPickerVM.PathTypeOptions.File,
        };

        [Reactive]
        public bool Do { get; set; } = true;

        public PassthroughGroupVM Parent { get; }

        private readonly ObservableAsPropertyHelper<bool> _Doing;
        public bool Doing => _Doing.Value;

        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

        public PassthroughVM(PassthroughGroupVM group)
        {
            Parent = group;
            this.WhenAnyValue(
                    x => x.Do,
                    x => x.Parent.Do,
                    (c, p) => c && p)
                .ToGuiProperty(this, nameof(Doing), out _Doing);
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
}
