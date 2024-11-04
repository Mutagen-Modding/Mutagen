using System.Reflection;
using Mutagen.Bethesda.Plugins.Analysis.DI;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Utility.DI;

public interface IModCompactor
{
    void CompactToSmallMaster(IMod mod);
    void CompactToMediumMaster(IMod mod);
    void CompactToFullMaster(IMod mod);
    void CompactTo(IMod mod, MasterStyle style);
    void CompactToWithFallback(IMod mod, MasterStyle style);
}

public class ModCompactor : IModCompactor
{
    private readonly IRecordCompactionCompatibilityDetector _compactionCompatibilityDetector;
    
    public ModCompactor(IRecordCompactionCompatibilityDetector compactionCompatibilityDetector)
    {
        _compactionCompatibilityDetector = compactionCompatibilityDetector;
    }
    
    public void CompactToSmallMaster(IMod mod)
    {
        var range = _compactionCompatibilityDetector.GetSmallMasterRange(mod);
        if (!mod.CanBeSmallMaster || !range.HasValue) throw new ArgumentException("Cannot be small master");
        mod.IsSmallMaster = true;
        if (mod.CanBeMediumMaster)
        {
            mod.IsMediumMaster = false;
        }
        DoCompacting(mod, range.Value);
    }
    
    public void CompactToMediumMaster(IMod mod)
    {
        var range = _compactionCompatibilityDetector.GetMediumMasterRange(mod);
        if (!mod.CanBeMediumMaster || !range.HasValue) throw new ArgumentException("Cannot be medium master");
        if (mod.CanBeSmallMaster)
        {
            mod.IsSmallMaster = false;
        }
        mod.IsMediumMaster = true;
        DoCompacting(mod, range.Value);
    }
    
    public void CompactToFullMaster(IMod mod)
    {
        var range = _compactionCompatibilityDetector.GetFullMasterRange(mod, potential: false);
        if (mod.CanBeSmallMaster)
        {
            mod.IsSmallMaster = false;
        }

        if (mod.CanBeMediumMaster)
        {
            mod.IsMediumMaster = false;
        }
        DoCompacting(mod, range);
    }

    public void CompactTo(IMod mod, MasterStyle style)
    {
        switch (style)
        {
            case MasterStyle.Full:
                CompactToFullMaster(mod);
                break;
            case MasterStyle.Small:
                CompactToSmallMaster(mod);
                break;
            case MasterStyle.Medium:
                CompactToMediumMaster(mod);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(style), style, null);
        }
    }

    public void CompactToWithFallback(IMod mod, MasterStyle style)
    {
        var targetStyle = style;
        if (style == MasterStyle.Small)
        {
            if (!mod.CanBeSmallMaster) throw new ArgumentException("Cannot be small master");

            try
            {
                CompactToSmallMaster(mod);
                return;
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException is not FormIDCompactionOutOfBoundsException)
                {
                    throw;
                }
            }
            catch (FormIDCompactionOutOfBoundsException e)
            {
            }
            targetStyle = MasterStyle.Medium;
        }
        if (style == MasterStyle.Medium || (targetStyle == MasterStyle.Medium && mod.CanBeMediumMaster))
        {
            try
            {
                CompactToMediumMaster(mod);
                return;
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException is not FormIDCompactionOutOfBoundsException)
                {
                    throw;
                }
            }
            catch (FormIDCompactionOutOfBoundsException e)
            {
            }
        }
        
        CompactToFullMaster(mod);
    }

    private static void DoCompacting(IMod mod, RangeUInt32 range)
    {
        ModToGenericCallHelper.InvokeFromCategory(
            mod,
            typeof(ModCompactor).GetMethod(nameof(DoCompactingGeneric), BindingFlags.NonPublic | BindingFlags.Static)!,
            new object[] { mod, range });
    }
    
    private static void DoCompactingGeneric<TMod, TModGetter>(TMod mod, RangeUInt32 range)
        where TModGetter : IModGetter
        where TMod : IMod, TModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
    {
        var linkCache = mod.ToUntypedImmutableLinkCache();
        var originatingRecords = mod.EnumerateMajorRecordContexts<IMajorRecord, IMajorRecordGetter>(linkCache)
            .Where(x => x.Record.FormKey.ModKey == mod.ModKey)
            .ToDictionary(x => x.Record.FormKey, x => x);

        var outOfRange = originatingRecords.Values
            .Where(x => !range.IsInRange(x.Record.FormKey.ID))
            .ToList();
        
        if (outOfRange.Count == 0) return;
        
        if (originatingRecords.Count > range.Difference)
        {
            throw new FormIDCompactionOutOfBoundsException(
                small: mod.IsSmallMaster,
                Medium: mod.IsMediumMaster,
                range: range
            );
        }

        var remapped = new Dictionary<FormKey, FormKey>();
        var nextId = Math.Max(
            mod.GetDefaultInitialNextFormID(),
            range.Min);
        foreach (var rec in outOfRange)
        {
            FormKey nextFormKey;
            
            do
            {
                if (nextId > range.Max)
                {
                    throw new FormIDCompactionOutOfBoundsException(
                        small: mod.IsSmallMaster,
                        Medium: mod.IsMediumMaster,
                        range: range
                    );
                }
                nextFormKey = new FormKey(mod.ModKey, nextId++);
            } while (originatingRecords.ContainsKey(nextFormKey));

            mod.Remove(rec.Record);
            rec.DuplicateIntoAsNewRecord(mod, nextFormKey);
            
            remapped.Add(rec.Record.FormKey, nextFormKey);
        }
    }
}