using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Plugins.Order.Implementations;
using Mutagen.Bethesda.WPF.Reflection;
using Noggog;
using Noggog.WPF;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using Mutagen.Bethesda.Environments;

namespace Mutagen.Bethesda.WPF.TestDisplay;

public class MainVM : ViewModel
{
    [Reactive]
    public FormKey FormKey { get; set; }

    public ILinkCache LinkCache { get; }

    public object LoadOrder { get; }

    public IEnumerable<Type> ScopedTypes { get; }

    public ObservableCollection<FormKey> FormKeys { get; } = new();

    public ObservableCollection<ModKey> ModKeys { get; } = new();

    public LateSetPickerVM LateSetPickerVM { get; }

    public ReflectionSettingsVM Reflection { get; }

    public FileSyncedLoadOrderVM LoadOrderVM { get; }

    public MainVM(FileSyncedLoadOrderVM loadOrderVm)
    {
        var gameRelease = SkyrimRelease.SkyrimSE;
        var env = GameEnvironment.Typical.Skyrim(gameRelease, LinkCachePreferences.OnlyIdentifiers())
            .DisposeWith(this);
        LinkCache = env.LinkCache;
        LoadOrder = env.LoadOrder;
        ScopedTypes = typeof(IArmorGetter).AsEnumerable();
        LateSetPickerVM = new LateSetPickerVM(this);
        Reflection = new ReflectionSettingsVM(
            ReflectionSettingsParameters.CreateFrom(
                new TestSettings(),
                env.LoadOrder.ListedOrder,
                env.LinkCache));
        LoadOrderVM = loadOrderVm;
    }
}