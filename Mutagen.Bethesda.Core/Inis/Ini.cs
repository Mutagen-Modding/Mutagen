using Mutagen.Bethesda.Inis.DI;
using Mutagen.Bethesda.Installs.DI;
using Noggog;

namespace Mutagen.Bethesda.Inis;

public static class Ini
{
    private static readonly IniPathLookup Lookup = new(
        GameLocatorLookupCache.Instance);
    
    public static FilePath GetTypicalPath(GameRelease release)
    {
        return Lookup.Get(release);
    }
    
    public static FilePath? TryGetTypicalPath(GameRelease release)
    {
        return Lookup.TryGet(release);
    }
}