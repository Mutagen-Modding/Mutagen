using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Records
{
    public static class ModInstantiator
    {
        private static Dictionary<GameCategory, ModInstantiator<IModGetter>.ImporterDelegate> _dict = new();

        static ModInstantiator()
        {
            foreach (var cat in EnumExt.GetValues<GameCategory>())
            {
                var regis = cat.ToModRegistration();
                _dict[cat] = ModInstantiatorReflection.GetOverlay<IModGetter>(regis);
            }
        }

        public static IModGetter Importer(ModPath path, GameRelease release, IFileSystem? fileSystem = null)
        {
            return _dict[release.ToCategory()](path, release, fileSystem);
        }
    }
}
