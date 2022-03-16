using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda
{
    public static class GameEnvironmentMixIn
    {
        public static IGameEnvironmentState<IOblivionMod, IOblivionModGetter> Oblivion(
            this GameEnvironment env,
            LinkCachePreferences? linkCachePrefs = null)
        {
            return env.Construct<IOblivionMod, IOblivionModGetter>(GameRelease.Oblivion, linkCachePrefs);
        }

    }
}
