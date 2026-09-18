using StrongInject;

namespace Mutagen.Bethesda.Installs.DI;

internal class InstallsModule
{
    [Instance(Options.AsImplementedInterfaces)] public static GameLocatorLookupCache LookupCache = GameLocatorLookupCache.Instance;
}