using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Plugins;
using Mutagen.Bethesda.WPF.Plugins.Order;
using Mutagen.Bethesda.WPF.Plugins.Order.Implementations;
using Mutagen.Bethesda.WPF.Reflection;
using Noggog;
using Noggog.WPF;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Mutagen.Bethesda.WPF.TestDisplay
{
    public class MainVM : ViewModel
    {
        [Reactive]
        public FormKey FormKey { get; set; }

        public ILinkCache LinkCache { get; }

        public object LoadOrder { get; }

        public IEnumerable<Type> ScopedTypes { get; }

        public ObservableCollection<FormKeyItemViewModel> FormKeys { get; } = new ObservableCollection<FormKeyItemViewModel>();

        public ObservableCollection<ModKeyItemViewModel> ModKeys { get; } = new ObservableCollection<ModKeyItemViewModel>();

        public LateSetPickerVM LateSetPickerVM { get; }

        public ReflectionSettingsVM Reflection { get; }

        public FileSyncedLoadOrderVM LoadOrderVM { get; }

    public MainVM()
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
        LoadOrderVM = new FileSyncedLoadOrderVM(env.LoadOrderFilePath)
        {
            DataFolderPath = env.DataFolderPath.Path,
            CreationClubFilePath = env.CreationKitLoadOrderFilePath?.Path ?? string.Empty,
            GameRelease = gameRelease.ToGameRelease(),
        };
    }
    }
}
