using System;
using System.Reactive.Disposables;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Cache.Implementations;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Implementations
{
    public class MutableDirectTests : ALinkingTests
    {
        public override bool ReadOnly => false;

        public override IDisposable ConvertMod(SkyrimMod mod, out ISkyrimModGetter getter)
        {
            getter = mod;
            return Disposable.Empty;
        }

        protected override (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(ISkyrimModGetter modGetter, LinkCachePreferences prefs)
        {
            return (LinkCacheStyle.OnlyDirect, new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(modGetter));
        }

        protected override (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCachePreferences prefs)
        {
            return (LinkCacheStyle.HasCaching, new MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter>(loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>(prefs)));
        }
    }
}