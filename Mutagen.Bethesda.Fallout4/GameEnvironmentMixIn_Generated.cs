using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda
{
    public static class GameEnvironmentMixIn
    {
        public static IGameEnvironment<IFallout4Mod, IFallout4ModGetter> Fallout4(
            this GameEnvironment env,
            LinkCachePreferences? linkCachePrefs = null)
        {
            return env.Construct<IFallout4Mod, IFallout4ModGetter>(GameRelease.Fallout4, linkCachePrefs);
        }

    }
}
