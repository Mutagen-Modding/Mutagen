using System;
using System.IO;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda
{
    public class GameEnvironment
    {
        public static readonly GameEnvironment Typical = new GameEnvironment();

        private GameEnvironment()
        {
        }
    }

    /// <summary>
    /// A class housing commonly used utilities when interacting with a game environment
    /// </summary>
    public class GameEnvironmentState<TModSetter, TModGetter> : IDisposable
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter>
    {
        private readonly bool _dispose;

        public string GameFolderPath { get; }

        /// <summary>
        /// Load Order object containing all the mods present in the environment.
        /// </summary>
        public LoadOrder<IModListing<TModGetter>> LoadOrder { get; }

        /// <summary>
        /// Convenience Link Cache to use created from the provided Load Order object
        /// </summary>
        public ILinkCache<TModSetter, TModGetter> LinkCache { get; }

        public GameEnvironmentState(
            string gameFolderPath,
            LoadOrder<IModListing<TModGetter>> loadOrder,
            ILinkCache<TModSetter, TModGetter> linkCache,
            bool dispose = true)
        {
            GameFolderPath = gameFolderPath;
            LoadOrder = loadOrder;
            LinkCache = linkCache;
            _dispose = dispose;
        }

        public void Dispose()
        {
            if (!_dispose) return;
            LoadOrder.Dispose();
            LinkCache.Dispose();
        }

        public static GameEnvironmentState<TModSetter, TModGetter> Construct(
            GameRelease release,
            string gameFolderPath,
            LinkCachePreferences? linkCachePrefs = null)
        {
            var dataPath = Path.Combine(gameFolderPath, "Data");

            var loadOrder = Mutagen.Bethesda.Plugins.Order.LoadOrder.Import<TModGetter>(
                dataPath,
                Mutagen.Bethesda.Plugins.Order.LoadOrder.GetListings(release, dataPath),
                release);

            return new GameEnvironmentState<TModSetter, TModGetter>(
                gameFolderPath: gameFolderPath,
                loadOrder: loadOrder,
                linkCache: loadOrder.ToImmutableLinkCache<TModSetter, TModGetter>(linkCachePrefs),
                dispose: true);
        }
    }
}
