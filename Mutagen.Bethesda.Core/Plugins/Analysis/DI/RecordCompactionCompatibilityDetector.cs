using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Analysis.DI;

public interface IRecordCompactionCompatibilityDetector
{
    bool IsSmallMasterCompatible(IModGetter mod);
    bool CouldBeSmallMasterCompatible(IModGetter mod);
    bool IsMediumMasterCompatible(IModGetter mod);
    bool CouldBeMediumMasterCompatible(IModGetter mod);
    RangeUInt32? GetSmallMasterRange(IModGetter mod);
    RangeUInt32? GetMediumMasterRange(IModGetter mod);
    RangeUInt32 GetFullMasterRange(IModGetter mod, bool potential);
    RangeUInt32? GetAllowedRange(IModGetter mod, bool potential);
    void IterateAndThrowIfIncompatible(IModGetter mod, bool potential);
    void ThrowIfIncompatible(
        IModGetter mod, 
        RangeUInt32 range,
        IMajorRecordGetter rec);
}

public class RecordCompactionCompatibilityDetector : IRecordCompactionCompatibilityDetector
{
    public bool IsSmallMasterCompatible(IModGetter mod)
    {
        var range = GetSmallMasterRange(mod);
        if (range == null) return false;

        return IterateToCheckCompatibility(mod, range.Value);
    }
    
    public bool CouldBeSmallMasterCompatible(IModGetter mod)
    {
        var range = GetSmallMasterRange(mod);
        if (range == null) return false;

        return IterateToCheckCouldBeCompatible(mod, range.Value);
    }
    
    public bool IsMediumMasterCompatible(IModGetter mod)
    {
        var range = GetMediumMasterRange(mod);
        if (range == null) return false;

        return IterateToCheckCompatibility(mod, range.Value);
    }
    
    public bool CouldBeMediumMasterCompatible(IModGetter mod)
    {
        var range = GetMediumMasterRange(mod);
        if (range == null) return false;

        return IterateToCheckCouldBeCompatible(mod, range.Value);
    }

    public RangeUInt32? GetSmallMasterRange(IModGetter mod)
    {
        if (!mod.CanBeSmallMaster) return null;
        var lowerRange = mod.GetDefaultInitialNextFormID(forceUseLowerFormIDRanges: null);
        return new RangeUInt32(lowerRange, FormID.SmallIdMask);
    }

    public RangeUInt32? GetMediumMasterRange(IModGetter mod)
    {
        if (!mod.CanBeMediumMaster) return null;
        var lowerRange = mod.GetDefaultInitialNextFormID(forceUseLowerFormIDRanges: null);
        return new RangeUInt32(lowerRange, FormID.MediumIdMask);
    }

    public RangeUInt32 GetFullMasterRange(IModGetter mod, bool potential)
    {
        return GetFullMasterRange(mod, mod.MasterReferences.Count, potential);
    }

    internal RangeUInt32 GetFullMasterRange(IModGetter mod, int masterCount, bool potential)
    {
        var lowerRange = mod.GetDefaultInitialNextFormID(
            forceUseLowerFormIDRanges: potential || masterCount > 0 ? null : false);
        return new RangeUInt32(lowerRange, FormID.FullIdMask);
    }

    public RangeUInt32? GetAllowedRange(IModGetter mod, bool potential)
    {
        return GetAllowedRange(mod, mod.MasterReferences.Count, potential);
    }

    internal RangeUInt32? GetAllowedRange(IModGetter mod, int masterCount, bool potential)
    {
        if (mod.IsMediumMaster)
        {
            return GetMediumMasterRange(mod);
        }
        if (mod.IsSmallMaster)
        {
            return GetSmallMasterRange(mod);
        }

        return GetFullMasterRange(mod, masterCount, potential);
    }
    
    private bool IterateToCheckCompatibility(IModGetter modGetter, RangeUInt32 range)
    {
        ModKey patchModKey = modGetter.ModKey;
        
        foreach (var rec in modGetter.EnumerateMajorRecords())
        {
            if (!rec.FormKey.ModKey.Equals(patchModKey)) continue;
            if (!range.IsInRange(rec.FormKey.ID)) return false;
        }

        return true;
    }
    
    private bool IterateToCheckCouldBeCompatible(IModGetter modGetter, RangeUInt32 range)
    {
        var rangeSize = range.Difference;
        ModKey patchModKey = modGetter.ModKey;
        var numOriginating = modGetter
            .EnumerateMajorRecords()
            .Count(x => x.FormKey.ModKey.Equals(patchModKey));
        return rangeSize >= numOriginating;
    }
    
    public void IterateAndThrowIfIncompatible(IModGetter mod, bool potential)
    {
        RangeUInt32? formIdRange = GetAllowedRange(mod, potential);

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
                small: mod.IsSmallMaster,
                medium: mod.IsMediumMaster,
                range: range,
                outOfBounds: rec
            );
        }
    }
}