using Mutagen.Bethesda.Inis.DI;
using Noggog;

namespace Mutagen.Bethesda.Inis;

// Private until API can be made more mature
internal class Ini
{
    private static readonly IniPathLookup Lookup = new();
    
    public static FilePath GetTypicalPath(GameRelease release, GameInstallMode installMode)
    {
        return Lookup.Get(release, installMode);
    }
}