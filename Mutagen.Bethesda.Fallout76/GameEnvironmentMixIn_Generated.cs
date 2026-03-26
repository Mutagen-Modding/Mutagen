using Mutagen.Bethesda.Fallout76;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda
{
    public static class GameEnvironmentMixIn
    {
        public static IGameEnvironment<IFallout76Mod, IFallout76ModGetter> Fallout76(
            this GameEnvironment env,
            Fallout76Release gameRelease,
            LinkCachePreferences? linkCachePrefs = null)
        {
            return env.Construct<IFallout76Mod, IFallout76ModGetter>(gameRelease.ToGameRelease(), linkCachePrefs);
        }

    }
}
