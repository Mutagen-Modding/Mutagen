using System.Collections;
using Loqui;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Plugins.Records;

/// <summary>
/// Interface for blocks that have sub-blocks that can be merged.
/// </summary>
public interface IMergeableBlockWithSubBlocks<out TSubBlock> : IMergeableBlock
    where TSubBlock : ILoquiObject
{
    /// <summary>
    /// The sub-blocks contained in this block.
    /// </summary>
    IReadOnlyList<TSubBlock> SubBlocks { get; }
}

/// <summary>
/// Merged block that combines multiple blocks with the same block number from different mods.
/// </summary>
public class MergedBlock<TBlock, TSubBlock> : ILoquiObject, IBinaryItem, IFormLinkContainerGetter, IAssetLinkContainerGetter, IMajorRecordGetterEnumerable
    where TBlock : class, ILoquiObject, IBinaryItem, IFormLinkContainerGetter, IAssetLinkContainerGetter, IMajorRecordGetterEnumerable
    where TSubBlock : class, ILoquiObject, IFormLinkContainerGetter, IAssetLinkContainerGetter, IMajorRecordGetterEnumerable
{
    private readonly int _blockNumber;
    private readonly List<TBlock> _sourceBlocks;
    private readonly Func<TBlock, IReadOnlyList<TSubBlock>> _getSubBlocksFunc;
    private List<TSubBlock>? _mergedSubBlocks;
    private readonly object _mergeLock = new object();

    /// <summary>
    /// Creates a new MergedBlock.
    /// </summary>
    /// <param name="blockNumber">The block number for this merged block</param>
    /// <param name="sourceBlocks">Source blocks to merge</param>
    /// <param name="getSubBlocksFunc">Function to extract sub-blocks from a block</param>
    public MergedBlock(
        int blockNumber,
        List<TBlock> sourceBlocks,
        Func<TBlock, IReadOnlyList<TSubBlock>> getSubBlocksFunc)
    {
        _blockNumber = blockNumber;
        _sourceBlocks = sourceBlocks;
        _getSubBlocksFunc = getSubBlocksFunc;
    }

    public int BlockNumber => _blockNumber;

    public IReadOnlyList<TSubBlock> SubBlocks
    {
        get
        {
            if (_mergedSubBlocks != null) return _mergedSubBlocks;

            lock (_mergeLock)
            {
                if (_mergedSubBlocks != null) return _mergedSubBlocks;

                // Merge SubBlocks from all source blocks
                var allSubBlocks = new List<TSubBlock>();
                foreach (var block in _sourceBlocks)
                {
                    allSubBlocks.AddRange(_getSubBlocksFunc(block));
                }

                _mergedSubBlocks = allSubBlocks;
                return _mergedSubBlocks;
            }
        }
    }

    // ILoquiObject
    ILoquiRegistration ILoquiObject.Registration => null!;

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        sb.AppendLine($"Merged Block {BlockNumber} ({SubBlocks.Count} sub-blocks from {_sourceBlocks.Count} source blocks)");
    }

    // IAssetLinkContainerGetter
    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
    {
        foreach (var block in _sourceBlocks)
        {
            foreach (var link in block.EnumerateAssetLinks(queryCategories, linkCache, assetType))
            {
                yield return link;
            }
        }
    }

    // IBinaryItem - write all sub-blocks in the merged block
    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
    {
        foreach (var subBlock in SubBlocks)
        {
            if (subBlock is IBinaryItem binaryItem)
            {
                binaryItem.WriteToBinary(writer, translationParams);
            }
        }
    }

    object IBinaryItem.BinaryWriteTranslator => this;

    // IFormLinkContainerGetter
    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true)
    {
        foreach (var block in _sourceBlocks)
        {
            foreach (var link in block.EnumerateFormLinks(iterateNestedRecords))
            {
                yield return link;
            }
        }
    }

    // IMajorRecordGetterEnumerable
    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
    {
        foreach (var subBlock in SubBlocks)
        {
            if (subBlock is IMajorRecordGetterEnumerable enumerable)
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
        foreach (var subBlock in SubBlocks)
        {
            if (subBlock is IMajorRecordGetterEnumerable enumerable)
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
        foreach (var subBlock in SubBlocks)
        {
            if (subBlock is IMajorRecordGetterEnumerable enumerable)
            {
                foreach (var record in enumerable.EnumerateMajorRecords(type, throwIfUnknown))
                {
                    yield return record;
                }
            }
        }
    }
}
