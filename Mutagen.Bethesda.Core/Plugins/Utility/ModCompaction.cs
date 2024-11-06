using Mutagen.Bethesda.Plugins.Analysis.DI;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Utility.DI;

namespace Mutagen.Bethesda.Plugins.Utility;

public static class ModCompaction
{
    private static readonly ModCompactor _compactor = new(
        new RecordCompactionCompatibilityDetector());
    
    public static void CompactToSmallMaster(IMod mod)
    {
        _compactor.CompactToSmallMaster(mod);
    }
    
    public static void CompactToMediumMaster(IMod mod)
    {
        _compactor.CompactToMediumMaster(mod);
    }
    
    public static void CompactToFullMaster(IMod mod)
    {
        _compactor.CompactToFullMaster(mod);
    }

    public static void CompactTo(IMod mod, MasterStyle style)
    {
        _compactor.CompactTo(mod, style);
    }

    public static void CompactToWithFallback(IMod mod, MasterStyle style)
    {
        _compactor.CompactToWithFallback(mod, style);
    }
}
