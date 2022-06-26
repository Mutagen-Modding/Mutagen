using System.Collections;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins.Order;
using Noggog.WPF;
using ReactiveUI.Fody.Helpers;

namespace Mutagen.Bethesda.WPF.Plugins.Order;

public interface ILoadOrderVM
{
    bool ShowDisabled { get; }
    bool ShowGhosted { get; }
    IEnumerable LoadOrder { get; }
}

public abstract class ALoadOrderVM<TEntryVM> : ViewModel, ILoadOrderVM
    where TEntryVM : IModListingGetter
{
    [Reactive]
    public bool ShowDisabled { get; set; }

    [Reactive]
    public bool ShowGhosted { get; set; }

    public abstract IObservableCollection<TEntryVM> LoadOrder { get; }

    IEnumerable ILoadOrderVM.LoadOrder => LoadOrder;
}