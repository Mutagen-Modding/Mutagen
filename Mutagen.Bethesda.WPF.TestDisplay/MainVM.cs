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

        public IEnumerable<Type> ScopedTypes { get; }

        public ObservableCollection<FormKey> FormKeys { get; } = new ObservableCollection<FormKey>();

        public MainVM()
        {
            var env = GameEnvironment.Typical.Skyrim(SkyrimRelease.SkyrimSE)
                .DisposeWith(this);
            LinkCache = env.LinkCache;
            ScopedTypes = typeof(IArmorGetter).AsEnumerable();
        }
    }
}
