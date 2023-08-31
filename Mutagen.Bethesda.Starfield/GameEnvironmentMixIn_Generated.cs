using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda
{
    public static class GameEnvironmentMixIn
    {
        public static IGameEnvironment<IStarfieldMod, IStarfieldModGetter> Starfield(
            this GameEnvironment env,
            LinkCachePreferences? linkCachePrefs = null)
        {
            return env.Construct<IStarfieldMod, IStarfieldModGetter>(GameRelease.Starfield, linkCachePrefs);
        }

    }
}
