using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public static class ModInstantiator
    {
        public static IModGetter Importer(ModPath path, GameRelease release)
        {
            var regis = release.ToCategory().ToModRegistration();
            return ModInstantiatorReflection.GetOverlay(regis)(path, release);
        }
    }
}
