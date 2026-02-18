using System.Collections;
using Loqui;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Plugins.Records;

/// <summary>
/// Interface for blocks that can be merged by a numeric key (like BlockNumber).
/// </summary>
public interface IMergeableBlock : ILoquiObject, IBinaryItem, IFormLinkContainerGetter, IAssetLinkContainerGetter, IMajorRecordGetterEnumerable
{
    /// <summary>
    /// The numeric key used to merge blocks (e.g., BlockNumber).
    /// </summary>
    int BlockNumber { get; }
}

/// <summary>
/// Merged list group that combines multiple list groups (like Cells) into a single unified view.
/// Blocks from different mods are merged by a numeric key (like BlockNumber).
/// </summary>
public class MergedListGroup<TBlock, TListGroup> : ILoquiObject, IListGroupGetter<TBlock>, IBinaryItem, IFormLinkContainerGetter, IAssetLinkContainerGetter, IMajorRecordGetterEnumerable
    where TBlock : class, ILoquiObject, IBinaryItem, IFormLinkContainerGetter, IAssetLinkContainerGetter, IMajorRecordGetterEnumerable
    where TListGroup : IEnumerable<TBlock>
{
    private readonly IEnumerable<TListGroup> _sourceGroups;
    private readonly Func<TBlock, int> _getBlockNumberFunc;
    private readonly Func<int, List<TBlock>, TBlock> _mergeBlocksFunc;
    private List<TBlock>? _cache;
    private readonly object _cacheLock = new object();

    /// <summary>
    /// Creates a new MergedListGroup.
    /// </summary>
    /// <param name="sourceGroups">Source list groups to merge</param>
    /// <param name="getBlockNumberFunc">Function to extract the block number from a block</param>
    /// <param name="mergeBlocksFunc">Function to merge multiple blocks with the same block number.
    /// Takes (blockNumber, blocksToMerge) and returns a merged block.</param>
    public MergedListGroup(
        IEnumerable<TListGroup> sourceGroups,
        Func<TBlock, int> getBlockNumberFunc,
        Func<int, List<TBlock>, TBlock> mergeBlocksFunc)
    {
        _sourceGroups = sourceGroups;
        _getBlockNumberFunc = getBlockNumberFunc;
        _mergeBlocksFunc = mergeBlocksFunc;
    }

    private List<TBlock> Cache
    {
        get
        {
            if (_cache != null) return _cache;

            lock (_cacheLock)
            {
                if (_cache != null) return _cache;

                // Merge blocks by block number
                var blocksByNumber = new Dictionary<int, List<TBlock>>();

                foreach (var group in _sourceGroups)
                {
                    foreach (var block in group)
                    {
                        var blockNumber = _getBlockNumberFunc(block);
                        if (!blocksByNumber.ContainsKey(blockNumber))
                        {
                            blocksByNumber[blockNumber] = new List<TBlock>();
                        }
                        blocksByNumber[blockNumber].Add(block);
                    }
                }

                // Create merged blocks
                var result = new List<TBlock>();
                foreach (var blockNumber in blocksByNumber.Keys.OrderBy(k => k))
                {
                    var blocksForNumber = blocksByNumber[blockNumber];
                    if (blocksForNumber.Count == 1)
                    {
                        result.Add(blocksForNumber[0]);
                    }
                    else
                    {
                        // Multiple blocks with same number - merge them
                        result.Add(_mergeBlocksFunc(blockNumber, blocksForNumber));
                    }
                }

                _cache = result;
                return _cache;
            }
        }
    }

    public IEnumerator<TBlock> GetEnumerator() => Cache.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => Cache.Count;

    public TBlock this[int index] => Cache[index];

    public IReadOnlyList<TBlock> Records => Cache;
    IReadOnlyList<ILoquiObject> IListGroupGetter.Records => Cache;
    IEnumerable<TBlock> IGroupCommonGetter<TBlock>.Records => Cache;
    IEnumerable<ILoquiObject> IGroupCommonGetter.Records => Cache;

    IEnumerable<TBlock> IListGroupGetter<TBlock>.GetEnumerator() => Cache;

    // ILoquiObject
    ILoquiRegistration ILoquiObject.Registration => null!;

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        sb.AppendLine($"Merged List Group ({Count} blocks):");
        foreach (var block in Cache.Take(5))
        {
            sb.AppendLine($"  - Block {_getBlockNumberFunc(block)}");
        }
        if (Count > 5)
        {
            sb.AppendLine($"  ... and {Count - 5} more");
        }
    }

    // IAssetLinkContainerGetter
    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
    {
        foreach (var block in Cache)
        {
            foreach (var link in block.EnumerateAssetLinks(queryCategories, linkCache, assetType))
            {
                yield return link;
            }
        }
    }

    // IBinaryItem - write all blocks in the merged list
    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
    {
        foreach (var block in Cache)
        {
            block.WriteToBinary(writer, translationParams);
        }
    }

    object IBinaryItem.BinaryWriteTranslator => this;

    // IFormLinkContainerGetter
    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true)
    {
        foreach (var block in Cache)
        {
            foreach (var link in block.EnumerateFormLinks(iterateNestedRecords))
            {
                yield return link;
            }
        }
    }

    // IGroupCommonGetter
    public ILoquiRegistration ContainedRecordRegistration
    {
        get
        {
            var firstGroup = _sourceGroups.FirstOrDefault();
            if (firstGroup is IGroupCommonGetter groupCommon)
            {
                return groupCommon.ContainedRecordRegistration;
            }
            throw new NotImplementedException("Source groups do not implement IGroupCommonGetter");
        }
    }
    public Type ContainedRecordType => typeof(TBlock);

    // IMajorRecordGetterEnumerable
    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
    {
        foreach (var block in Cache)
        {
            if (block is IMajorRecordGetterEnumerable enumerable)
            {
                foreach (var record in enumerable.EnumerateMajorRecords())
                {
                    yield return record;
                }
            }
        }
    }

    public IEnumerable<T> EnumerateMajorRecords<T>(bool throwIfUnknown = true)
        where T : class, IMajorRecordQueryableGetter
    {
        foreach (var block in Cache)
        {
            if (block is IMajorRecordGetterEnumerable enumerable)
            {
                foreach (var record in enumerable.EnumerateMajorRecords<T>(throwIfUnknown))
                {
                    yield return record;
                }
            }
        }
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
    {
        foreach (var block in Cache)
        {
            if (block is IMajorRecordGetterEnumerable enumerable)
            {
                foreach (var record in enumerable.EnumerateMajorRecords(type, throwIfUnknown))
                {
                    yield return record;
                }
            }
        }
    }
}
