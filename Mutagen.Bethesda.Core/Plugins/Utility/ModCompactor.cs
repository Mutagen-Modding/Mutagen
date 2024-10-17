using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Mutagen.Bethesda.Plugins.Analysis.DI;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Utility;

public class ModCompactor
{
    private readonly RecordCompactionCompatibilityDetector _compactionCompatibilityDetector;
    
    public ModCompactor(RecordCompactionCompatibilityDetector compactionCompatibilityDetector)
    {
        _compactionCompatibilityDetector = compactionCompatibilityDetector;
    }
    
    public void CompactToSmallMaster(IMod mod)
    {
        var range = _compactionCompatibilityDetector.GetSmallMasterRange(mod);
        if (!mod.CanBeSmallMaster || !range.HasValue) throw new ArgumentException("Cannot be small master");
        mod.IsSmallMaster = true;
        DoCompacting(mod, range.Value);
    }
    
    public void CompactToMediumMaster(IMod mod)
    {
        var range = _compactionCompatibilityDetector.GetMediumMasterRange(mod);
        if (!mod.CanBeMediumMaster || !range.HasValue) throw new ArgumentException("Cannot be medium master");
        mod.IsMediumMaster = true;
        DoCompacting(mod, range.Value);
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