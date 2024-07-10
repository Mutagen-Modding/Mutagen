using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Masters;

public enum MasterStyle
{
    Full,
    Light,
    Medium
}

internal static class MasterStyleConstruction
{
    public static MasterStyle ConstructFromFlags(int flags, GameConstants constants)
    {
        bool light = constants.LightMasterFlag.HasValue && Enums.HasFlag(flags, constants.LightMasterFlag.Value);
        bool medium = constants.MediumMasterFlag.HasValue && Enums.HasFlag(flags, constants.MediumMasterFlag.Value);
        
        if (light && medium)
        {
            throw new ModHeaderMalformedException(
                Enumerable.Empty<ModKey>(),
                "Mod was both a light and medium master");
        }

        if (light) return MasterStyle.Light;
        if (medium) return MasterStyle.Medium;
        return MasterStyle.Full;
    }
}