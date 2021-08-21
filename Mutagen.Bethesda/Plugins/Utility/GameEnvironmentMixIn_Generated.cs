using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Fallout4;
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

        public static IGameEnvironmentState<ISkyrimMod, ISkyrimModGetter> Skyrim(
            this GameEnvironment env,
            SkyrimRelease gameRelease,
            LinkCachePreferences? linkCachePrefs = null)
        {
            return env.Construct<ISkyrimMod, ISkyrimModGetter>(gameRelease.ToGameRelease(), linkCachePrefs);
        }

        public static IGameEnvironmentState<IFallout4Mod, IFallout4ModGetter> Fallout4(
            this GameEnvironment env,
            LinkCachePreferences? linkCachePrefs = null)
        {
            return env.Construct<IFallout4Mod, IFallout4ModGetter>(GameRelease.Fallout4, linkCachePrefs);
        }

    }
}
