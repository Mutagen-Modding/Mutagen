using System;
using System.Reactive.Disposables;
using Mutagen.Bethesda.Core.UnitTests;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;
using Noggog.Utility;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Implementations
{
    public class MutableOverlayTests : ALinkingTests
    {
        public override bool ReadOnly => true;

        public override IDisposable ConvertMod(SkyrimMod mod, out ISkyrimModGetter getter)
        {
            var tempFile = new TempFile(extraDirectoryPaths: TestPathing.TempFolderPath);
            var path = new ModPath(mod.ModKey, tempFile.File.Path);
            mod.WriteToBinaryParallel(
                path,
                new BinaryWriteParameters()
                {
                    ModKey = ModKeyOption.NoCheck,
                });
            var overlay = SkyrimMod.CreateFromBinaryOverlay(path, SkyrimRelease.SkyrimLE);
            getter = overlay;
            return Disposable.Create(() =>
            {
                overlay.Dispose();
                tempFile.Dispose();
            });
        }

        protected override (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(ISkyrimModGetter modGetter, LinkCachePreferences prefs)
        {
            return (LinkCacheStyle.OnlyDirect, new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(modGetter));
        }

        protected override (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCachePreferences prefs)
        {
            return (LinkCacheStyle.OnlyDirect, new MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter>(loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>()));
        }
    }
}