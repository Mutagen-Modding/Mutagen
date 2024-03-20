using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda
{
    public static class GameEnvironmentMixIn
    {
        public static IGameEnvironment<IFallout4Mod, IFallout4ModGetter> Fallout4(
            this GameEnvironment env,
            Fallout4Release gameRelease,
            LinkCachePreferences? linkCachePrefs = null)
        {
            return env.Construct<IFallout4Mod, IFallout4ModGetter>(gameRelease.ToGameRelease(), linkCachePrefs);
        }

    }
}
