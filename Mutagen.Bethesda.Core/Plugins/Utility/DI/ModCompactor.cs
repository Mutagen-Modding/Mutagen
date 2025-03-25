﻿using System.Reflection;
using Mutagen.Bethesda.Plugins.Analysis.DI;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Utility.DI;

public class ModCompactionResults
{
    public MasterStyle ResultingStyle { get; init; }
    public required IReadOnlyDictionary<FormKey, FormKey> RemappedFormKeys { get; init; }
}

public interface IModCompactor
{
    ModCompactionResults CompactToSmallMaster(IMod mod);
    ModCompactionResults CompactToMediumMaster(IMod mod);
    ModCompactionResults CompactToFullMaster(IMod mod);
    ModCompactionResults CompactTo(IMod mod, MasterStyle style);
    ModCompactionResults CompactToWithFallback(IMod mod, MasterStyle style);
}

public class ModCompactor : IModCompactor
{
    private readonly IRecordCompactionCompatibilityDetector _compactionCompatibilityDetector;
    private readonly MethodInfo _methodInfo;
    
    public ModCompactor(IRecordCompactionCompatibilityDetector compactionCompatibilityDetector)
    {
        _compactionCompatibilityDetector = compactionCompatibilityDetector;
        _methodInfo = typeof(ModCompactor).GetMethod(nameof(DoCompactingGeneric), BindingFlags.NonPublic | BindingFlags.Instance)!;
    }
    
    public ModCompactionResults CompactToSmallMaster(IMod mod)
    {
        var range = _compactionCompatibilityDetector.GetSmallMasterRange(mod);
        if (!mod.CanBeSmallMaster || !range.HasValue) throw new ArgumentException("Cannot be small master");
        mod.IsSmallMaster = true;
        if (mod.CanBeMediumMaster)
        {
            mod.IsMediumMaster = false;
        }

        return new ModCompactionResults()
        {
            RemappedFormKeys = DoCompacting(mod, range.Value),
            ResultingStyle = MasterStyle.Small
        };
    }
    
    public ModCompactionResults CompactToMediumMaster(IMod mod)
    {
        var range = _compactionCompatibilityDetector.GetMediumMasterRange(mod);
        if (!mod.CanBeMediumMaster || !range.HasValue) throw new ArgumentException("Cannot be medium master");
        if (mod.CanBeSmallMaster)
        {
            mod.IsSmallMaster = false;
        }
        mod.IsMediumMaster = true;

        return new ModCompactionResults()
        {
            RemappedFormKeys = DoCompacting(mod, range.Value),
            ResultingStyle = MasterStyle.Medium
        };
    }
    
    public ModCompactionResults CompactToFullMaster(IMod mod)
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

        return new ModCompactionResults()
        {
            RemappedFormKeys = DoCompacting(mod, range),
            ResultingStyle = MasterStyle.Full
        };
    }

    public ModCompactionResults CompactTo(IMod mod, MasterStyle style)
    {
        switch (style)
        {
            case MasterStyle.Full:
                return CompactToFullMaster(mod);
            case MasterStyle.Small:
                return CompactToSmallMaster(mod);
            case MasterStyle.Medium:
                return CompactToMediumMaster(mod);
            default:
                throw new ArgumentOutOfRangeException(nameof(style), style, null);
        }
    }

    public ModCompactionResults CompactToWithFallback(IMod mod, MasterStyle style)
    {
        var targetStyle = style;
        if (style == MasterStyle.Small)
        {
            if (!mod.CanBeSmallMaster) throw new ArgumentException("Cannot be small master");

            try
            {
                return CompactToSmallMaster(mod);
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
                return CompactToMediumMaster(mod);
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
        
        return CompactToFullMaster(mod);
    }

    private IReadOnlyDictionary<FormKey, FormKey> DoCompacting(IMod mod, RangeUInt32 range)
    {
        return (IReadOnlyDictionary<FormKey, FormKey>)ModToGenericCallHelper.InvokeFromCategory(
            this,
            mod.GameRelease.ToCategory(),
            _methodInfo,
            new object[] { mod, range })!;
    }
    
    private IReadOnlyDictionary<FormKey, FormKey> DoCompactingGeneric<TMod, TModGetter>(TMod mod, RangeUInt32 range)
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

        if (outOfRange.Count == 0) return new Dictionary<FormKey, FormKey>();
        
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

        return remapped;
    }
}