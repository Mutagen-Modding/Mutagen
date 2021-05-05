using System;
using System.Collections.Generic;
using System.IO;
using Mutagen.Bethesda.Pex.Records.Internals;
using Noggog;

namespace Mutagen.Bethesda.Pex.Records

{
    public static class PexFileInstantiator
    {
        private static Dictionary<GameCategory, Func<Stream, IPexFileCommon>> _dict = new();

        static PexFileInstantiator()
        {
            foreach (var cat in EnumExt.GetValues<GameCategory>())
            {
                var regis = cat.ToPexRegistration();
                if (regis == null) continue;
                _dict[cat] = PexFileInstantiatorReflection.GetImporter(regis);
            }
        }

        public static IPexFileCommon Importer(string path, GameRelease release)
        {
            using var stream = File.Open(path, FileMode.Open);
            return Importer(stream, release);
        }

        public static IPexFileCommon Importer(Stream stream, GameRelease release)
        {
            return _dict[release.ToCategory()](stream);
        }
    }
}
