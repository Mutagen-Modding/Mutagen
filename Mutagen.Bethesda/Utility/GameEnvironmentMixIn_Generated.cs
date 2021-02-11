using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Fallout4;

namespace Mutagen.Bethesda
{
    public static class GameEnvironmentMixIn
    {
        public static GameEnvironmentState<IOblivionMod, IOblivionModGetter> Oblivion(this GameEnvironment env)
        {
            return env.Construct<IOblivionMod, IOblivionModGetter>(GameRelease.Oblivion);
        }

        public static GameEnvironmentState<ISkyrimMod, ISkyrimModGetter> Skyrim(
            this GameEnvironment env,
            SkyrimRelease gameRelease)
        {
            return env.Construct<ISkyrimMod, ISkyrimModGetter>(gameRelease.ToGameRelease());
        }

        public static GameEnvironmentState<IFallout4Mod, IFallout4ModGetter> Fallout4(this GameEnvironment env)
        {
            return env.Construct<IFallout4Mod, IFallout4ModGetter>(GameRelease.Fallout4);
        }

    }
}
