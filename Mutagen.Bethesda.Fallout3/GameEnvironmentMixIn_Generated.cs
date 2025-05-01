using Mutagen.Bethesda.Fallout3;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda
{
    public static class GameEnvironmentMixIn
    {
        public static IGameEnvironment<IFallout3Mod, IFallout3ModGetter> Fallout3(
            this GameEnvironment env,
            Fallout3Release gameRelease,
            LinkCachePreferences? linkCachePrefs = null)
        {
            return env.Construct<IFallout3Mod, IFallout3ModGetter>(gameRelease.ToGameRelease(), linkCachePrefs);
        }

    }
}
