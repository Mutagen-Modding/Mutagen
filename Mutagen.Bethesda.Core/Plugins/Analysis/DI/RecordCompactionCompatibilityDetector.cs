using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Analysis.DI;

public class RecordCompactionCompatibilityDetector
{
    private const uint LightMasterMin = 0x800;
    private const uint LightMasterMax = 0xFFF;
    
    public bool IsLightModCompatible(IModGetter modGetter)
    {
        if (!modGetter.CanBeLightMaster) return false;
        ModKey patchModKey = modGetter.ModKey;

        foreach (var rec in modGetter.EnumerateMajorRecords())
        {
            if (!rec.FormKey.ModKey.Equals(patchModKey)) continue;
            if (rec.FormKey.ID < LightMasterMin) return false;
            if (rec.FormKey.ID > LightMasterMax) return false;
        }

        return true;
    }
}