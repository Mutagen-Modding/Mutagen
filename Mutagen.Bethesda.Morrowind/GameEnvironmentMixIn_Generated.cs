using Mutagen.Bethesda.Morrowind;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda
{
    public static class GameEnvironmentMixIn
    {
        public static IGameEnvironment<IMorrowindMod, IMorrowindModGetter> Morrowind(
            this GameEnvironment env,
            LinkCachePreferences? linkCachePrefs = null)
        {
            return env.Construct<IMorrowindMod, IMorrowindModGetter>(GameRelease.Morrowind, linkCachePrefs);
        }

    }
}
