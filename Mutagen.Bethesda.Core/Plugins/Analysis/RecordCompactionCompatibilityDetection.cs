using Mutagen.Bethesda.Plugins.Analysis.DI;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Analysis;

public static class RecordCompactionCompatibilityDetection
{
    private static RecordCompactionCompatibilityDetector _detector = new();

    public static bool IsSmallMasterCompatible(IModGetter mod)
    {
        return _detector.IsSmallMasterCompatible(mod);
    }

    public static bool CouldBeSmallMasterCompatible(IModGetter mod)
    {
        return _detector.CouldBeSmallMasterCompatible(mod);
    }
    
    public static bool IsMediumMasterCompatible(IModGetter mod)
    {
        return _detector.IsMediumMasterCompatible(mod);
    }

    public static bool CouldBeMediumMasterCompatible(IModGetter mod)
    {
        return _detector.CouldBeMediumMasterCompatible(mod);
    }

    public static RangeUInt32? GetSmallMasterRange(IModGetter mod)
    {
        return _detector.GetSmallMasterRange(mod);
    }

    public static RangeUInt32? GetMediumMasterRange(IModGetter mod)
    {
        return _detector.GetMediumMasterRange(mod);
    }

    public static RangeUInt32 GetFullMasterRange(IModGetter mod, bool potential)
    {
        return _detector.GetFullMasterRange(mod, potential);
    }

    public static RangeUInt32? GetAllowedRange(IModGetter mod, bool potential)
    {
        return _detector.GetAllowedRange(mod, potential);
    }

    public static void IterateAndThrowIfIncompatible(IModGetter mod, bool potential)
    {
        _detector.IterateAndThrowIfIncompatible(mod, potential);
    }
}
