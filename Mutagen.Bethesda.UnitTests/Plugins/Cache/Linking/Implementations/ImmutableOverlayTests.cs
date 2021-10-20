using System;
using System.Reactive.Disposables;
using Mutagen.Bethesda.Core.UnitTests;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Cache.Implementations;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;
using Noggog.Utility;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Implementations
{
    public class ImmutableOverlayTests : ALinkingTests
    {
        public override bool ReadOnly => true;

        public override IDisposable ConvertMod(SkyrimMod mod, out ISkyrimModGetter getter)
        {
            return ConvertModToOverlay(mod, out getter);
        }

        public static IDisposable ConvertModToOverlay(SkyrimMod mod, out ISkyrimModGetter getter)
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
            return (LinkCacheStyle.HasCaching, new ImmutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(modGetter, prefs));
        }

        protected override (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCachePreferences prefs)
        {
            return (LinkCacheStyle.HasCaching, loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>(prefs));
        }
    }
}