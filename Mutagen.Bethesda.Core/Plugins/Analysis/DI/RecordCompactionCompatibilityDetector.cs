using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Analysis.DI;

public class RecordCompactionCompatibilityDetector
{
    private const uint LightMasterMax = 0xFFF;
    private const uint HalfMasterMax = 0xFFFF;
    
    public bool IsLightMasterCompatible(IModGetter modGetter)
    {
        if (!modGetter.CanBeLightMaster) return false;
        var lowerRange = modGetter.GetDefaultInitialNextFormID(forceUseLowerFormIDRanges: null);

        return IsCompatible(modGetter, lowerRange, LightMasterMax);
    }
    
    public bool IsHalfMasterCompatible(IModGetter modGetter)
    {
        if (!modGetter.CanBeHalfMaster) return false;
        var lowerRange = modGetter.GetDefaultInitialNextFormID(forceUseLowerFormIDRanges: null);

        return IsCompatible(modGetter, lowerRange, HalfMasterMax);
    }
    
    private bool IsCompatible(IModGetter modGetter, uint lowerRange, uint max)
    {
        ModKey patchModKey = modGetter.ModKey;
        
        foreach (var rec in modGetter.EnumerateMajorRecords())
        {
            if (!rec.FormKey.ModKey.Equals(patchModKey)) continue;
            if (rec.FormKey.ID < lowerRange) return false;
            if (rec.FormKey.ID > max) return false;
        }

        return true;
    }
}