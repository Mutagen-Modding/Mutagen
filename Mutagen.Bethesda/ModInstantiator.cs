using Mutagen.Bethesda.Plugins.Records.Internals;

namespace Mutagen.Bethesda.Plugins.Records
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
