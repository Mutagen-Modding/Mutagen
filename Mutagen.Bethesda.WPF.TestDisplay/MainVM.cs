using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.WPF;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

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

        public MainVM()
        {
            var env = GameEnvironment.Typical.Skyrim(SkyrimRelease.SkyrimSE, LinkCachePreferences.OnlyIdentifiers())
                .DisposeWith(this);
            LinkCache = env.LinkCache;
            LoadOrder = env.LoadOrder;
            ScopedTypes = typeof(IArmorGetter).AsEnumerable();
        }
    }
}
