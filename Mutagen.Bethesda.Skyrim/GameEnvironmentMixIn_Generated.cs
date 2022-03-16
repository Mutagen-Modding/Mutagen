using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda
{
    public static class GameEnvironmentMixIn
    {
        public static IGameEnvironmentState<ISkyrimMod, ISkyrimModGetter> Skyrim(
            this GameEnvironment env,
            SkyrimRelease gameRelease,
            LinkCachePreferences? linkCachePrefs = null)
        {
            return env.Construct<ISkyrimMod, ISkyrimModGetter>(gameRelease.ToGameRelease(), linkCachePrefs);
        }

    }
}
