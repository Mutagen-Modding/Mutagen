using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Analysis.DI;

public class RecordCompactionCompatibilityDetector
{
    private const uint LightMasterMax = 0xFFF;
    private const uint HalfMasterMax = 0xFFFF;
    
    public bool IsLightMasterCompatible(IModGetter mod)
    {
        var range = GetLightMasterRange(mod);
        if (range == null) return false;

        return IsCompatible(mod, range.Value);
    }
    
    public bool IsHalfMasterCompatible(IModGetter mod)
    {
        var range = GetHalfMasterRange(mod);
        if (range == null) return false;

        return IsCompatible(mod, range.Value);
    }

    public RangeUInt32? GetLightMasterRange(IModGetter mod)
    {
        if (!mod.CanBeLightMaster) return null;
        var lowerRange = mod.GetDefaultInitialNextFormID(forceUseLowerFormIDRanges: null);
        return new RangeUInt32(lowerRange, LightMasterMax);
    }

    public RangeUInt32? GetHalfMasterRange(IModGetter mod)
    {
        if (!mod.CanBeHalfMaster) return null;
        var lowerRange = mod.GetDefaultInitialNextFormID(forceUseLowerFormIDRanges: null);
        return new RangeUInt32(lowerRange, HalfMasterMax);
    }

    public RangeUInt32? GetRange(IModGetter mod)
    {
        if (mod.IsHalfMaster)
        {
            return GetHalfMasterRange(mod);
        }
        if (mod.IsLightMaster)
        {
            return GetLightMasterRange(mod);
        }

        return null;
    }
    
    private bool IsCompatible(IModGetter modGetter, RangeUInt32 range)
    {
        ModKey patchModKey = modGetter.ModKey;
        
        foreach (var rec in modGetter.EnumerateMajorRecords())
        {
            if (!rec.FormKey.ModKey.Equals(patchModKey)) continue;
            if (!range.IsInRange(rec.FormKey.ID)) return false;
        }

        return true;
    }
    
    public void ThrowIfIncompatible(IModGetter mod)
    {
        RangeUInt32? formIdRange = GetRange(mod);

        if (formIdRange == null) return;
        
        foreach (var rec in mod.EnumerateMajorRecords())
        {
            ThrowIfIncompatible(mod, formIdRange.Value, rec);
        }
    }
    
    public void ThrowIfIncompatible(
        IModGetter mod, 
        RangeUInt32 range,
        IMajorRecordGetter rec)
    {
        ModKey patchModKey = mod.ModKey;
        
        if (!rec.FormKey.ModKey.Equals(patchModKey)) return;
        if (!range.IsInRange(rec.FormKey.ID))
        {
            throw new FormIDCompactionOutOfBoundsException(
                light: mod.IsLightMaster,
                half: mod.IsHalfMaster,
                range: range,
                outOfBounds: rec
            );
        }
    }
}