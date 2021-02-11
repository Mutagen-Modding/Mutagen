using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public static class GameEnvironmentMixIn
    {
        public static GameEnvironmentState<TModSetter, TModGetter> Construct<TModSetter, TModGetter>(this GameEnvironment env, GameRelease release)
            where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
            where TModGetter : class, IContextGetterMod<TModSetter, TModGetter>
        {
            if (!GameLocations.TryGetGameFolder(release, out var gameFolderPath))
            {
                throw new ArgumentException($"Could not find game folder automatically.");
            }

            return GameEnvironmentState<TModSetter, TModGetter>.Construct(release, gameFolderPath);
        }
    }
}
