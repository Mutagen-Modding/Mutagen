using Mutagen.Bethesda.Inis.DI;
using Mutagen.Bethesda.Installs.DI;
using Noggog;

namespace Mutagen.Bethesda.Inis;

// Private until API can be made more mature
internal class Ini
{
    private static readonly IniPathLookup Lookup = new(
        GameLocator.Instance);
    
    public static FilePath GetTypicalPath(GameRelease release)
    {
        return Lookup.Get(release);
    }
}