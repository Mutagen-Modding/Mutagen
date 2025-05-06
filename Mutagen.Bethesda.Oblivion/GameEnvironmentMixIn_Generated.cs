using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda
{
    public static class GameEnvironmentMixIn
    {
        public static IGameEnvironment<IOblivionMod, IOblivionModGetter> Oblivion(
            this GameEnvironment env,
            OblivionRelease gameRelease,
            LinkCachePreferences? linkCachePrefs = null)
        {
            return env.Construct<IOblivionMod, IOblivionModGetter>(gameRelease.ToGameRelease(), linkCachePrefs);
        }

    }
}
