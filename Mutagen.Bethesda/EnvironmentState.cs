using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// A class housing commonly used utilities when interacting with a game environment
    /// </summary>
    public class EnvironmentState<TModSetter, TModGetter> : IDisposable
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
        ILinkCache<TModSetter, TModGetter> LinkCache { get; }

        public EnvironmentState(
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

        public static EnvironmentState<TModSetter, TModGetter> ConstructTypical(GameRelease release)
        {
            // Confirm target game release matches
            var regis = release.ToCategory().ToModRegistration();
            if (!typeof(TModSetter).IsAssignableFrom(regis.SetterType))
            {
                throw new ArgumentException($"Target mod type {typeof(TModSetter)} was not of the expected type {regis.SetterType}");
            }
            if (!typeof(TModGetter).IsAssignableFrom(regis.GetterType))
            {
                throw new ArgumentException($"Target mod type {typeof(TModGetter)} was not of the expected type {regis.GetterType}");
            }

            if (!GameLocations.TryGetGameFolder(release, out var gameFolderPath))
            {
                throw new ArgumentException($"Could not find game folder automatically.");
            }

            var dataPath = Path.Combine(gameFolderPath, "Data");

            var loadOrder = Mutagen.Bethesda.LoadOrder.Import<TModGetter>(
                dataPath,
                Mutagen.Bethesda.LoadOrder.GetListings(release, dataPath),
                release);

            return new EnvironmentState<TModSetter, TModGetter>(
                gameFolderPath: gameFolderPath,
                loadOrder: loadOrder,
                linkCache: loadOrder.ToImmutableLinkCache<TModSetter, TModGetter>(),
                dispose: true);
        }

        public void Dispose()
        {
            if (!_dispose) return;
            LoadOrder.Dispose();
            LinkCache.Dispose();
        }
    }
}
