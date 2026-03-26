using Mutagen.Bethesda.FalloutNV;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda
{
    public static class GameEnvironmentMixIn
    {
        public static IGameEnvironment<IFalloutNVMod, IFalloutNVModGetter> FalloutNV(
            this GameEnvironment env,
            FalloutNVRelease gameRelease,
            LinkCachePreferences? linkCachePrefs = null)
        {
            return env.Construct<IFalloutNVMod, IFalloutNVModGetter>(gameRelease.ToGameRelease(), linkCachePrefs);
        }

    }
}
