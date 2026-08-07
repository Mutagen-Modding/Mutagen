using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Analysis.DI;

public class MultiModFileSplitter : IMultiModFileSplitter
{
    /// <summary>
    /// Record representing a record to export and what it references
    /// </summary>
    private class AnalyzedRecord<TMod, TModGetter>
        where TMod : IMod, TModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
        where TModGetter : IModGetter
    {
        public required IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> Context { get; init; }

        /// <summary>Whether this record is originally defined within the mod</summary>
        public required bool IsOriginatingRecord { get; init; }

        /// <summary>External mods this record references, parent records included.</summary>
        public required HashSet<ModKey> ExternalMasters { get; init; }

        /// <summary>The originating records this record references, parent records included.</summary>
        public required HashSet<FormKey> ReferencedOriginRecords { get; init; }
    }

    /// <summary>
    /// Record representing a mod fragment to export and what it references
    /// </summary>
    private class ModFragment<TMod, TModGetter>
        where TMod : IMod, TModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
        where TModGetter : IModGetter
    {
        public required int Index { get; init; }
        public required ModKey Key { get; init; }
        public HashSet<ModKey> Masters { get; } = new();
        public List<AnalyzedRecord<TMod, TModGetter>> Records { get; } = new();
    }

    /// <summary>
    /// Collects a records information like its external masters and referenced records, walking the parent chain
    /// </summary>
    private static AnalyzedRecord<TMod, TModGetter> AnalyzeContext<TMod, TModGetter>(
        IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> context,
        ModKey inputKey)
        where TMod : IMod, TModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
        where TModGetter : IModGetter
    {
        var externalMods = new HashSet<ModKey>();
        var referencedOriginatedRecords = new HashSet<FormKey>();

        void Accumulate(IMajorRecordGetter record)
        {
            if (record.FormKey.ModKey != inputKey)
            {
                externalMods.Add(record.FormKey.ModKey);
            }

            foreach (var formLink in record.EnumerateFormLinks(iterateNestedRecords: false))
            {
                if (formLink.FormKey.IsNull) continue;
                
                if (formLink.FormKey.ModKey == inputKey)
                {
                    referencedOriginatedRecords.Add(formLink.FormKey);
                }
                else
                {
                    externalMods.Add(formLink.FormKey.ModKey);
                }
            }
        }

        Accumulate(context.Record);

        // Walk the full parent chain
        var parent = context.Parent;
        while (parent != null)
        {
            if (parent.Record is IMajorRecordGetter parentRecord)
            {
                Accumulate(parentRecord);
            }
            parent = parent.Parent;
        }

        return new AnalyzedRecord<TMod, TModGetter>
        {
            Context = context,
            IsOriginatingRecord = context.Record.FormKey.ModKey == inputKey,
            ExternalMasters = externalMods,
            ReferencedOriginRecords = referencedOriginatedRecords,
        };
    }

    public IReadOnlyList<TMod> Split<TMod, TModGetter>(TMod inputMod, int masterLimit)
        where TMod : IMod, TModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
        where TModGetter : IModGetter
    {
        var inputKey = inputMod.ModKey;
        var linkCache = inputMod.ToUntypedImmutableLinkCache();

        var records = inputMod
            .EnumerateMajorRecordContexts<IMajorRecord, IMajorRecordGetter>(linkCache)
            .Select(context => AnalyzeContext(context, inputKey))
            .ToList();

        var createdRecordsByKey = records
            .Where(x => x.IsOriginatingRecord)
            .ToDictionary(x => x.Context.Record.FormKey, x => x);

        // Order created records so that a record is placed only after every created record it references
        var clusterSets = OrderCreatedClusters(createdRecordsByKey);

        var modFragments = new List<ModFragment<TMod, TModGetter>>();
        var originIndexByRecord = new Dictionary<FormKey, int>();

        // Place created clusters with dependencies first
        foreach (var clusterSet in clusterSets)
        {
            var externalMasters = new HashSet<ModKey>();
            var referencedOutside = new HashSet<FormKey>();
            foreach (var key in clusterSet)
            {
                var record = createdRecordsByKey[key];
                externalMasters.UnionWith(record.ExternalMasters);
                foreach (var referenced in record.ReferencedOriginRecords)
                {
                    if (!clusterSet.Contains(referenced))
                    {
                        referencedOutside.Add(referenced);
                    }
                }
            }

            var modFragment = ReserveModFragment(modFragments, inputMod, externalMasters, referencedOutside, masterLimit, originIndexByRecord);
            foreach (var key in clusterSet)
            {
                modFragment.Records.Add(createdRecordsByKey[key]);
                originIndexByRecord[key] = modFragment.Index;
            }
        }

        // Place overrides. Each must load no earlier than every created record it references.
        foreach (var record in records)
        {
            if (record.IsOriginatingRecord) continue;
            var modFragment = ReserveModFragment(modFragments, inputMod, record.ExternalMasters, record.ReferencedOriginRecords, masterLimit, originIndexByRecord);
            modFragment.Records.Add(record);
        }

        // Records originating in the first mod fragment keep the input key, so they need no remap
        var remap = createdRecordsByKey.Keys
            .Select(x => (Key: x, Index: originIndexByRecord.GetValueOrDefault(x, 0)))
            .Where(x => x.Index != 0)
            .ToDictionary(
                x => x.Key,
                x => new FormKey(KeyForIndex(inputKey, x.Index), x.Key.ID));

        return WriteModFragments<TMod, TModGetter>(inputMod, modFragments, remap);
    }

    /// <summary>
    /// Tarjan strongly-connected-components over the created-record reference graph. Each component becomes a
    /// cluster of records that must share a mod fragment, returned in dependency-first order (a cluster is emitted
    /// only after everything it references). Edges: a created record to each created record it links to, plus
    /// bidirectional edges across created parent chains so a created parent and its created children always
    /// share a cluster.
    /// </summary>
    private static List<HashSet<FormKey>> OrderCreatedClusters<TMod, TModGetter>(
        Dictionary<FormKey, AnalyzedRecord<TMod, TModGetter>> createdRecordsByKey)
        where TMod : IMod, TModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
        where TModGetter : IModGetter
    {
        var adjacency = new Dictionary<FormKey, HashSet<FormKey>>();
        foreach (var key in createdRecordsByKey.Keys)
        {
            adjacency.GetOrAdd(key);
        }

        foreach (var (key, record) in createdRecordsByKey)
        {
            foreach (var referenced in record.ReferencedOriginRecords)
            {
                if (referenced.Equals(key)) continue;
                if (createdRecordsByKey.ContainsKey(referenced))
                {
                    adjacency.GetOrAdd(key).Add(referenced);
                }
            }

            var parent = record.Context.Parent;
            while (parent != null)
            {
                if (parent.Record is IMajorRecordGetter parentRecord
                    && createdRecordsByKey.ContainsKey(parentRecord.FormKey)
                    && !parentRecord.FormKey.Equals(key))
                {
                    adjacency.GetOrAdd(key).Add(parentRecord.FormKey);
                    adjacency.GetOrAdd(parentRecord.FormKey).Add(key);
                }
                parent = parent.Parent;
            }
        }

        var nextVisitOrder = 0;
        var visitOrderByKey = new Dictionary<FormKey, int>();
        var lowLinkByKey = new Dictionary<FormKey, int>();
        var onStack = new HashSet<FormKey>();
        var stack = new Stack<FormKey>();
        var clusters = new List<HashSet<FormKey>>();

        void StrongConnect(FormKey node)
        {
            visitOrderByKey[node] = nextVisitOrder;
            lowLinkByKey[node] = nextVisitOrder;
            nextVisitOrder++;
            stack.Push(node);
            onStack.Add(node);

            foreach (var neighbor in adjacency.GetOrAdd(node))
            {
                if (!visitOrderByKey.TryGetValue(neighbor, out var neighborVisitOrder))
                {
                    StrongConnect(neighbor);
                    lowLinkByKey[node] = Math.Min(lowLinkByKey[node], lowLinkByKey[neighbor]);
                }
                else if (onStack.Contains(neighbor))
                {
                    lowLinkByKey[node] = Math.Min(lowLinkByKey[node], neighborVisitOrder);
                }
            }

            if (lowLinkByKey[node] == visitOrderByKey[node])
            {
                var cluster = new HashSet<FormKey>();
                FormKey popped;
                do
                {
                    popped = stack.Pop();
                    onStack.Remove(popped);
                    cluster.Add(popped);
                }
                while (!popped.Equals(node));
                clusters.Add(cluster);
            }
        }

        foreach (var key in createdRecordsByKey.Keys)
        {
            if (!visitOrderByKey.ContainsKey(key))
            {
                StrongConnect(key);
            }
        }

        return clusters;
    }

    /// <summary>
    /// Reserves the mod fragment for a set of external masters and referenced formkeys: the first mod fragment that can
    /// hold it without exceeding the master limit and without referencing a later mod fragment, else a new mod
    /// fragment. The unit's masters are registered on the chosen mod fragment. 
    /// </summary>
    private static ModFragment<TMod, TModGetter> ReserveModFragment<TMod, TModGetter>(
        List<ModFragment<TMod, TModGetter>> modFragments,
        TMod inputMod,
        HashSet<ModKey> externals,
        HashSet<FormKey> referencedCreated,
        int masterLimit,
        Dictionary<FormKey, int> originIndexByRecord)
        where TMod : IMod, TModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
        where TModGetter : IModGetter
    {
        var requiredMin = 0;
        foreach (var created in referencedCreated)
        {
            requiredMin = Math.Max(requiredMin, originIndexByRecord.GetValueOrDefault(created, 0));
        }

        HashSet<ModKey> MastersForIndex(int index)
        {
            var set = new HashSet<ModKey>(externals);
            foreach (var created in referencedCreated)
            {
                var createdOriginIndex = originIndexByRecord.GetValueOrDefault(created, 0);
                if (createdOriginIndex < index)
                {
                    set.Add(KeyForIndex(inputMod.ModKey, createdOriginIndex));
                }
            }
            return set;
        }

        for (var i = requiredMin; i < modFragments.Count; i++)
        {
            var modFragment = modFragments[i];
            var needed = MastersForIndex(i);
            var missing = needed.Count(m => !modFragment.Masters.Contains(m));
            if (modFragment.Masters.Count + missing <= masterLimit)
            {
                modFragment.Masters.UnionWith(needed);
                return modFragment;
            }
        }

        var newIndex = modFragments.Count;
        var newModFragment = new ModFragment<TMod, TModGetter>
        {
            Index = newIndex,
            Key = KeyForIndex(inputMod.ModKey, newIndex),
        };
        modFragments.Add(newModFragment);

        var newNeeded = MastersForIndex(newIndex);
        if (newNeeded.Count > masterLimit)
        {
            throw new TooManyMastersException(inputMod.ModKey, newNeeded.ToArray());
        }
        newModFragment.Masters.UnionWith(newNeeded);
        return newModFragment;
    }

    private static ModKey KeyForIndex(ModKey inputKey, int index)
    {
        if (index == 0) return inputKey;
        var fileName = inputKey.FileName;
        return ModKey.FromFileName($"{fileName.NameWithoutExtension}_{index + 1}{fileName.Extension}");
    }

    private static IReadOnlyList<TMod> WriteModFragments<TMod, TModGetter>(
        TMod inputMod,
        List<ModFragment<TMod, TModGetter>> modFragments,
        Dictionary<FormKey, FormKey> remap)
        where TMod : IMod, TModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
        where TModGetter : IModGetter
    {
        var release = inputMod.GameRelease;
        var result = new List<TMod>(modFragments.Count);
        foreach (var modFragment in modFragments)
        {
            // Header version has to go in via the activator, as it drives the initial NextFormID
            var newMod = ModFactory<TMod>.Activator(
                modFragment.Key,
                release,
                headerVersion: inputMod.ModHeader.HeaderVersion);
            newMod.ModHeader.RawFlags = inputMod.ModHeader.RawFlags;
            newMod.ModHeader.Author = inputMod.ModHeader.Author;
            newMod.ModHeader.Description = inputMod.ModHeader.Description;

            foreach (var record in modFragment.Records)
            {
                if (record.IsOriginatingRecord)
                {
                    record.Context.DuplicateIntoAsNewRecord(
                        newMod,
                        new FormKey(newMod.ModKey, record.Context.Record.FormKey.ID));
                }
                else
                {
                    record.Context.GetOrAddAsOverride(newMod);
                }
            }

            if (remap.Count > 0)
            {
                newMod.RemapLinks(remap);
            }

            result.Add(newMod);
        }

        return result;
    }
}
