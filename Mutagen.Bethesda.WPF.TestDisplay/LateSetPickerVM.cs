using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mutagen.Bethesda.WPF.TestDisplay;

public class LateSetPickerVM : ViewModel
{
    public ICommand SetCommand { get; }

    [Reactive]
    public FormKey FormKey { get; set; }

    [Reactive]
    public ILinkCache? LinkCache { get; set; }

    [Reactive]
    public IEnumerable<Type>? ScopedTypes { get; set; }

    public LateSetPickerVM(MainVM mainVM)
    {
        SetCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            LinkCache = null;
            ScopedTypes = null;
            FormKey = FormKey.Null;

            await Task.Delay(2000);

            FormKey = FormKey.Factory("0136D4:Skyrim.esm");

            await Task.Delay(2000);

            LinkCache = mainVM.LinkCache;
            ScopedTypes = mainVM.ScopedTypes;
        });
    }
}