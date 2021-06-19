using System;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Installs;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda
{
    public static class GameEnvironmentMixIn
    {
        public static GameEnvironmentState<TModSetter, TModGetter> Construct<TModSetter, TModGetter>(
            this GameEnvironment env, 
            GameRelease release,
            LinkCachePreferences? linkCachePrefs = null)
            where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
            where TModGetter : class, IContextGetterMod<TModSetter, TModGetter>
        {
            if (!GameLocations.TryGetGameFolder(release, out var gameFolderPath))
            {
                throw new ArgumentException($"Could not find game folder automatically.");
            }

            return GameEnvironmentState<TModSetter, TModGetter>.Construct(release, gameFolderPath, linkCachePrefs);
        }
    }
}
