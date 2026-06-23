using Loqui;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Fallout4;

internal static class CellBlockConsolidator
{
    public static void MergeInto<T>(IExtendedList<T> existing, IEnumerable<T> newBlocks)
        where T : class, ICellBlock, IBinaryItem
    {
        foreach (var newBlock in newBlocks)
        {
            T? match = null;
            foreach (var item in existing)
            {
                if (item.BlockNumber == newBlock.BlockNumber)
                {
                    match = item;
                    break;
                }
            }
            if (match is null)
            {
                existing.Add(newBlock);
            }
            else
            {
                match.GroupType = newBlock.GroupType;
                match.LastModified = newBlock.LastModified;
                match.Unknown = newBlock.Unknown;
                MergeSubBlocks(match.SubBlocks, newBlock.SubBlocks);
            }
        }
    }

    private static void MergeSubBlocks(IExtendedList<CellSubBlock> existing, IEnumerable<CellSubBlock> newSubs)
    {
        foreach (var newSub in newSubs)
        {
            CellSubBlock? match = null;
            foreach (var item in existing)
            {
                if (item.BlockNumber == newSub.BlockNumber)
                {
                    match = item;
                    break;
                }
            }
            if (match is null)
            {
                existing.Add(newSub);
            }
            else
            {
                match.GroupType = newSub.GroupType;
                match.LastModified = newSub.LastModified;
                match.Unknown = newSub.Unknown;
                match.Cells.AddRange(newSub.Cells);
            }
        }
    }

    public static IReadOnlyList<ICellBlockGetter> ConsolidateBlocks(IReadOnlyList<IFallout4ListGroupGetter<ICellBlockGetter>> sources)
    {
        if (sources.Count == 0) return Array.Empty<ICellBlockGetter>();
        if (sources.Count == 1) return sources[0].Records;

        var byBlockNumber = new Dictionary<int, List<ICellBlockGetter>>();
        var order = new List<int>();
        foreach (var source in sources)
        {
            foreach (var block in source.Records)
            {
                if (!byBlockNumber.TryGetValue(block.BlockNumber, out var list))
                {
                    list = new List<ICellBlockGetter>();
                    byBlockNumber[block.BlockNumber] = list;
                    order.Add(block.BlockNumber);
                }
                list.Add(block);
            }
        }

        var result = new List<ICellBlockGetter>(order.Count);
        foreach (var blockNumber in order)
        {
            var blocks = byBlockNumber[blockNumber];
            if (blocks.Count == 1)
            {
                result.Add(blocks[0]);
            }
            else
            {
                result.Add(new ConsolidatedCellBlock(blockNumber, blocks));
            }
        }
        return result;
    }
}

internal sealed class ConsolidatedCellBlock : ICellBlockGetter
{
    private readonly int _blockNumber;
    private readonly List<ICellBlockGetter> _sources;
    private IReadOnlyList<ICellSubBlockGetter>? _subBlocks;

    public ConsolidatedCellBlock(int blockNumber, List<ICellBlockGetter> sources)
    {
        _blockNumber = blockNumber;
        _sources = sources;
    }

    public int BlockNumber => _blockNumber;
    public GroupTypeEnum GroupType => _sources[^1].GroupType;
    public int LastModified => _sources[^1].LastModified;
    public int Unknown => _sources[^1].Unknown;

    public IReadOnlyList<ICellSubBlockGetter> SubBlocks
    {
        get
        {
            if (_subBlocks != null) return _subBlocks;
            var bySubBlockNumber = new Dictionary<int, List<ICellSubBlockGetter>>();
            var order = new List<int>();
            foreach (var block in _sources)
            {
                foreach (var sb in block.SubBlocks)
                {
                    if (!bySubBlockNumber.TryGetValue(sb.BlockNumber, out var list))
                    {
                        list = new List<ICellSubBlockGetter>();
                        bySubBlockNumber[sb.BlockNumber] = list;
                        order.Add(sb.BlockNumber);
                    }
                    list.Add(sb);
                }
            }
            var result = new List<ICellSubBlockGetter>(order.Count);
            foreach (var n in order)
            {
                var subs = bySubBlockNumber[n];
                result.Add(subs.Count == 1 ? subs[0] : new ConsolidatedCellSubBlock(n, subs));
            }
            _subBlocks = result;
            return _subBlocks;
        }
    }

    ILoquiRegistration ILoquiObject.Registration => CellBlock_Registration.Instance;

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        sb.AppendLine($"Consolidated Cell Block {_blockNumber} ({_sources.Count} sources)");
    }

    object ICellBlockGetter.CommonInstance() => CellBlockCommon.Instance;
    object? ICellBlockGetter.CommonSetterInstance() => null;
    object ICellBlockGetter.CommonSetterTranslationInstance() => CellBlockSetterTranslationCommon.Instance;

    object IBinaryItem.BinaryWriteTranslator => CellBlockBinaryWriteTranslation.Instance;

    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
    {
        CellBlockBinaryWriteTranslation.Instance.Write(writer, this, translationParams);
    }

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true) => CellBlockCommon.Instance.EnumerateFormLinks(this, iterateNestedRecords);
    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories, IAssetLinkCache? linkCache, Type? assetType) => CellBlockCommon.Instance.EnumerateAssetLinks(this, queryCategories, linkCache, assetType);

    IEnumerable<IMajorRecordGetter> IMajorRecordGetterEnumerable.EnumerateMajorRecords() => this.EnumerateMajorRecords();
    IEnumerable<T> IMajorRecordGetterEnumerable.EnumerateMajorRecords<T>(bool throwIfUnknown) => this.EnumerateMajorRecords<T>(throwIfUnknown: throwIfUnknown);
    IEnumerable<IMajorRecordGetter> IMajorRecordGetterEnumerable.EnumerateMajorRecords(Type type, bool throwIfUnknown) => this.EnumerateMajorRecords(type: type, throwIfUnknown: throwIfUnknown);
}

internal sealed class ConsolidatedCellSubBlock : ICellSubBlockGetter
{
    private readonly int _blockNumber;
    private readonly List<ICellSubBlockGetter> _sources;
    private IReadOnlyList<ICellGetter>? _cells;

    public ConsolidatedCellSubBlock(int blockNumber, List<ICellSubBlockGetter> sources)
    {
        _blockNumber = blockNumber;
        _sources = sources;
    }

    public int BlockNumber => _blockNumber;
    public GroupTypeEnum GroupType => _sources[^1].GroupType;
    public int LastModified => _sources[^1].LastModified;
    public int Unknown => _sources[^1].Unknown;

    public IReadOnlyList<ICellGetter> Cells
    {
        get
        {
            if (_cells != null) return _cells;
            var all = new List<ICellGetter>();
            foreach (var sb in _sources)
            {
                all.AddRange(sb.Cells);
            }
            _cells = all;
            return _cells;
        }
    }

    ILoquiRegistration ILoquiObject.Registration => CellSubBlock_Registration.Instance;

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        sb.AppendLine($"Consolidated Cell SubBlock {_blockNumber} ({_sources.Count} sources)");
    }

    object ICellSubBlockGetter.CommonInstance() => CellSubBlockCommon.Instance;
    object? ICellSubBlockGetter.CommonSetterInstance() => null;
    object ICellSubBlockGetter.CommonSetterTranslationInstance() => CellSubBlockSetterTranslationCommon.Instance;

    object IBinaryItem.BinaryWriteTranslator => CellSubBlockBinaryWriteTranslation.Instance;

    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
    {
        CellSubBlockBinaryWriteTranslation.Instance.Write(writer, this, translationParams);
    }

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks(bool iterateNestedRecords = true) => CellSubBlockCommon.Instance.EnumerateFormLinks(this, iterateNestedRecords);
    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories, IAssetLinkCache? linkCache, Type? assetType) => CellSubBlockCommon.Instance.EnumerateAssetLinks(this, queryCategories, linkCache, assetType);

    IEnumerable<IMajorRecordGetter> IMajorRecordGetterEnumerable.EnumerateMajorRecords() => this.EnumerateMajorRecords();
    IEnumerable<T> IMajorRecordGetterEnumerable.EnumerateMajorRecords<T>(bool throwIfUnknown) => this.EnumerateMajorRecords<T>(throwIfUnknown: throwIfUnknown);
    IEnumerable<IMajorRecordGetter> IMajorRecordGetterEnumerable.EnumerateMajorRecords(Type type, bool throwIfUnknown) => this.EnumerateMajorRecords(type: type, throwIfUnknown: throwIfUnknown);
}
