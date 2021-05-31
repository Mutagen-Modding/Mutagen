using System;
using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Records
{
    public static class ModInstantiator
    {
        private static Dictionary<GameCategory, Func<ModPath, GameRelease, IModGetter>> _dict = new();

        static ModInstantiator()
        {
            foreach (var cat in EnumExt.GetValues<GameCategory>())
            {
                var regis = cat.ToModRegistration();
                _dict[cat] = ModInstantiatorReflection.GetOverlay(regis);
            }
        }

        public static IModGetter Importer(ModPath path, GameRelease release)
        {
            return _dict[release.ToCategory()](path, release);
        }
    }
}
