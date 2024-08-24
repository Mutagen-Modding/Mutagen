using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Masters;

internal static class MasterStyleConstruction
{
    public static MasterStyle ConstructFromFlags(int flags, GameConstants constants)
    {
        bool small = constants.SmallMasterFlag.HasValue && Enums.HasFlag(flags, constants.SmallMasterFlag.Value);
        bool medium = constants.MediumMasterFlag.HasValue && Enums.HasFlag(flags, constants.MediumMasterFlag.Value);
        
        if (small && medium)
        {
            throw new ModHeaderMalformedException(
                "Mod was both a light and medium master");
        }

        if (small) return MasterStyle.Small;
        if (medium) return MasterStyle.Medium;
        return MasterStyle.Full;
    }
}