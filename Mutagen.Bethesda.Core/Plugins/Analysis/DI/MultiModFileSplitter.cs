using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Analysis.DI;

public class MultiModFileSplitter : IMultiModFileSplitter
{
    internal class EquatableModKeySet : IEquatable<EquatableModKeySet>
    {
        private readonly ModKey[] _modKeys;
        private readonly int _hash;

        public EquatableModKeySet(IEnumerable<ModKey> modKeys)
        {
            _modKeys = modKeys.OrderBy(x => x.FileName.String).ToArray();
            _hash = GetHashCodeForModKeys();
        }
        
        private int GetHashCodeForModKeys()
        {
            HashCode hashCode = default;
            foreach (var modKey in _modKeys)
            {
                hashCode.Add(modKey);
            }
            return hashCode.ToHashCode();
        }

        public bool Equals(EquatableModKeySet? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _modKeys.SequenceEqual(other._modKeys);
        }
        
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EquatableModKeySet)obj);
        }
        
        public override int GetHashCode()
        {
            return _hash;
        }
    }
    
    /// <summary>
    /// Helper class to contain data for a cluster, which will eventually become a file
    /// </summary>
    internal class Cluster<TMod, TModGetter>
        where TMod : IMod, TModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
        where TModGetter : IModGetter
    {
        /// <summary>
        /// The masters for the current cluster. The output file must have these masters.
        /// </summary>
        public HashSet<ModKey> Masters = new();

        /// <summary>
        /// The records for the current cluster, these would be contained in the resulting file
        /// </summary>
        public List<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> Records = new();
    }

    private static HashSet<ModKey> GetAllMastersForRecord(
        IMajorRecordGetter record,
        ModKey except)
    {
        var result = new HashSet<ModKey>();

        result.Add(record.FormKey.ModKey);

        foreach (var formLink in record.EnumerateFormLinks(iterateNestedRecords: false))
        {
            result.Add(formLink.FormKey.ModKey);
        }

        result.Remove(except);

        return result;
    }

    /// <summary>
    /// Gets the masters needed for clustering a record, accounting for deep nested record structures.
    /// Uses iterateNestedRecords: false to get only the record's own FormLinks, excluding child
    /// records' FormLinks from the master count. For child records with parent contexts, this includes
    /// the parent's own (shallow) masters since the parent will be pulled into the same output file
    /// via GetOrAddAsOverride.
    /// </summary>
    private static HashSet<ModKey> GetMastersForClustering(
        IModContext<IMajorRecordGetter> rec,
        ModKey except)
    {
        var result = GetAllMastersForRecord(rec.Record, except);

        // Walk the parent chain (e.g., DialogResponse -> DialogTopic, PlacedObject -> Cell -> Worldspace)
        // including each parent's shallow masters since GetOrAddAsOverride will pull
        // parent records into the same output file.
        var parent = rec.Parent;
        while (parent?.Record is IMajorRecordGetter parentRecord)
        {
            var parentMasters = GetAllMastersForRecord(parentRecord, except);
            result.UnionWith(parentMasters);
            parent = parent.Parent;
        }

        return result;
    }

    /// <summary>
    /// Splits a given `inputMod` into n output Clusters, each containing at most `limit` masters.
    /// </summary>
    /// <param name="inputMod"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    private static List<Cluster<TMod, TModGetter>> GenerateClusters<TMod, TModGetter>(TMod inputMod, int limit)
        where TMod : IMod, TModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
        where TModGetter : IModGetter
    {
        var clusters = new List<Cluster<TMod, TModGetter>>();

        var clusterLookupCache = new Dictionary<EquatableModKeySet, Cluster<TMod, TModGetter>>();

        var linkCache = inputMod.ToUntypedImmutableLinkCache();
        foreach (var rec in inputMod.EnumerateMajorRecordContexts<IMajorRecord, IMajorRecordGetter>(linkCache))
        {
            var mastersHashSet = GetMastersForClustering(rec, inputMod.ModKey);

            // Check if single record exceeds master limit
            if (mastersHashSet.Count > limit)
            {
                throw new TooManyMastersException(
                    inputMod.ModKey,
                    mastersHashSet.ToArray());
            }

            var masters = new EquatableModKeySet(mastersHashSet);

            if (clusterLookupCache.ContainsKey(masters))
            {
                var cacheCluster = clusterLookupCache[masters];
                // found a cluster in the cache
                // and in this case, the current masterlist should be a subset of cacheCluster already
                cacheCluster.Records.Add(rec);
                continue;
            }

            Cluster<TMod, TModGetter>? existingCluster = null;
            
            foreach (Cluster<TMod, TModGetter> curCluster in clusters)
            {
                var missingMasters = mastersHashSet.Except(curCluster.Masters).ToArray();

                if (curCluster.Masters.Count + missingMasters.Count() <= limit)
                {
                    // found an existing cluster where the current record fits
                    curCluster.Masters.Add(missingMasters);
                    existingCluster = curCluster;
                    break;
                }
            }

            if (existingCluster == null)
            {
                // we didn't find any, create new
                var newCluster = new Cluster<TMod, TModGetter>
                {
                    Masters = mastersHashSet
                };
                newCluster.Records.Add(rec);

                clusters.Add(newCluster);
                clusterLookupCache.Add(masters, newCluster);
                continue;
            }

            existingCluster.Records.Add(rec);
            clusterLookupCache.Add(masters, existingCluster);
        }

        return clusters;
    }
        
    public IReadOnlyCollection<TMod> Split<TMod, TModGetter>(TMod inputMod, int masterLimit)
        where TMod : IMod, TModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
        where TModGetter : IModGetter
    {
        var result = new List<TMod>();
        var clusters = GenerateClusters<TMod, TModGetter>(inputMod, masterLimit);
        for (int i = 0; i < clusters.Count; i++)
        {
            var curCluster = clusters[i];
            string curFileName;
            if (i == 0)
            {
                // call the first output the same as the input, so Synthesis.esp stays Synthesis.esp
                curFileName = inputMod.ModKey.FileName;
            }
            else
            {
                // otherwise, suffix them with a number, making Synthesis_1.esp, Synthesis_2.esp, etc
                curFileName = $"{inputMod.ModKey.FileName.NameWithoutExtension}_{(i + 1)}{inputMod.ModKey.FileName.Extension}";
            }

            var newMod = ModFactory<TMod>.Activator(ModKey.FromFileName(curFileName), inputMod.GameRelease);

            foreach (var context in curCluster.Records)
            {
                if (context.Record.FormKey.ModKey == inputMod.ModKey)
                {
                    // this is a Form which has been created within inputMod -> copy it right over
                    context.DuplicateIntoAsNewRecord(newMod, new FormKey(newMod.ModKey, context.Record.FormKey.ID));
                }
                else
                {
                    // this is an override -> copy as override
                    context.GetOrAddAsOverride(newMod);
                }
            }

            result.Add(newMod);
        }

        return result;
    }
}