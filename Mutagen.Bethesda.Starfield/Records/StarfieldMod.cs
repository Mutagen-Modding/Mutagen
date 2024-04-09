using System.Buffers.Binary;
using System.Diagnostics;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Utility;
using Mutagen.Bethesda.Starfield.Internals;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;

namespace Mutagen.Bethesda.Starfield;

public partial class StarfieldMod : AMod
{
    private uint GetDefaultInitialNextFormID(
        StarfieldRelease release,
        bool? forceUseLowerFormIDRanges) =>
        GetDefaultInitialNextFormID(release, this.ModHeader.Stats.Version, forceUseLowerFormIDRanges);

    public override uint MinimumCustomFormID(bool? forceUseLowerFormIDRanges = null) =>
        GetDefaultInitialNextFormID(
            this.StarfieldRelease,
            this.ModHeader.Stats.Version, 
            forceUseLowerFormIDRanges);

    partial void CustomCtor()
    {
        this.ModHeader.FormVersion = GameConstants.Get(GameRelease).DefaultFormVersion!.Value;
    }

    public static uint GetDefaultInitialNextFormID(
        StarfieldRelease release, 
        float headerVersion, 
        bool? forceUseLowerFormIDRanges)
    {
        return HeaderVersionHelper.GetNextFormId(
            release: release.ToGameRelease(),
            allowedReleases: null,
            headerVersion: headerVersion,
            useLowerRangesVersion: 0f,
            forceUseLowerFormIDRanges: forceUseLowerFormIDRanges,
            higherFormIdRange: 800);
    }
}

internal partial class StarfieldModBinaryOverlay
{
    public uint MinimumCustomFormID(bool? forceUseLowerFormIDRanges = null) => 
        StarfieldMod.GetDefaultInitialNextFormID(
            this.StarfieldRelease,
            this.ModHeader.Stats.Version,
            forceUseLowerFormIDRanges);
}

partial class StarfieldModSetterCommon
{
    private static partial void RemapInferredAssetLinks(
        IStarfieldMod obj,
        IReadOnlyDictionary<IAssetLinkGetter, string> mapping,
        AssetLinkQuery queryCategories)
    {
        // Nothing to do here, we can't change the name of the mod
    }

}

partial class StarfieldModCommon
{
    public static partial IEnumerable<IAssetLinkGetter> GetInferredAssetLinks(IStarfieldModGetter obj, Type? assetType)
    {
        yield break;
    }
    
    public static void WriteCellsParallel(
        IStarfieldListGroupGetter<ICellBlockGetter> group,
        int targetIndex,
        Stream[] streamDepositArray,
        WritingBundle bundle,
        ParallelWriteParameters parallelWriteParameters)
    {
        if (group.Records.Count == 0) return;
        Stream[] streams = new Stream[group.Records.Count + 1];
        byte[] groupBytes = new byte[bundle.Constants.GroupConstants.HeaderLength];
        BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
        var groupByteStream = new MemoryStream(groupBytes);
        using (var stream = new MutagenWriter(groupByteStream, bundle with { }, dispose: false))
        {
            stream.Position += 8;
            StarfieldListGroupBinaryWriteTranslation.WriteEmbedded<ICellBlockGetter>(group, stream);
        }

        streams[0] = groupByteStream;
        Parallel.ForEach(group.Records, parallelWriteParameters.ParallelOptions, (cellBlock, state, counter) =>
        {
            WriteBlocksParallel(
                cellBlock,
                (int)counter + 1,
                streams,
                bundle,
                parallelWriteParameters);
        });
        PluginUtilityTranslation.CompileSetGroupLength(streams, groupBytes);
        streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
    }
    
    public static void WriteBlocksParallel(
        ICellBlockGetter block,
        int targetIndex,
        Stream[] streamDepositArray,
        WritingBundle bundle,
        ParallelWriteParameters parallelWriteParameters)
    {
        var subBlocks = block.SubBlocks;
        Stream[] streams = new Stream[(subBlocks?.Count ?? 0) + 1];
        byte[] groupBytes = new byte[bundle.Constants.GroupConstants.HeaderLength];
        BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
        var groupByteStream = new MemoryStream(groupBytes);
        using (var stream = new MutagenWriter(groupByteStream, bundle with {}, dispose: false))
        {
            stream.Position += 8;
            CellBlockBinaryWriteTranslation.WriteEmbedded(block, stream);
        }
        streams[0] = groupByteStream;
        if (subBlocks != null)
        {
            Parallel.ForEach(subBlocks, parallelWriteParameters.ParallelOptions, (cellSubBlock, state, counter) =>
            {
                WriteSubBlocksParallel(
                    cellSubBlock,
                    (int)counter + 1,
                    streams,
                    bundle,
                    parallelWriteParameters);
            });
        }
        PluginUtilityTranslation.CompileSetGroupLength(streams, groupBytes);
        streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
    }

    public static void WriteSubBlocksParallel(
        ICellSubBlockGetter subBlock,
        int targetIndex,
        Stream[] streamDepositArray,
        WritingBundle bundle,
        ParallelWriteParameters parallelWriteParameters)
    {
        var cells = subBlock.Cells;
        Stream[] streams = new Stream[(cells?.Count ?? 0) + 1];
        byte[] groupBytes = new byte[bundle.Constants.GroupConstants.HeaderLength];
        var groupByteStream = new MemoryStream(groupBytes);
        BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
        using (var stream = new MutagenWriter(
                   groupByteStream,
                   bundle with {},
                   dispose: false))
        {
            stream.Position += 8;
            CellSubBlockBinaryWriteTranslation.WriteEmbedded(subBlock, stream);
        }
        streams[0] = groupByteStream;
        if (cells != null)
        {
            Parallel.ForEach(cells, parallelWriteParameters.ParallelOptions, (cell, state, counter) =>
            {
                MemoryTributary trib = new MemoryTributary();
                cell.WriteToBinary(new MutagenWriter(
                    trib,
                    bundle with {},
                    dispose: false));
                streams[(int)counter + 1] = trib;
            });
        }
        PluginUtilityTranslation.CompileSetGroupLength(streams, groupBytes);
        streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
    }
    
    public static void WriteWorldspacesParallel(
        IStarfieldGroupGetter<IWorldspaceGetter> group,
        int targetIndex,
        Stream[] streamDepositArray,
        WritingBundle bundle,
        ParallelWriteParameters parallelWriteParameters)
    {
        var cache = group.RecordCache;
        if (cache.Count == 0) return;
        Stream[] streams = new Stream[cache.Count + 1];
        byte[] groupBytes = new byte[bundle.Constants.GroupConstants.HeaderLength];
        BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
        var groupByteStream = new MemoryStream(groupBytes);
        using (var stream = new MutagenWriter(groupByteStream, bundle with {}, dispose: false))
        {
            stream.Position += 8;
            StarfieldGroupBinaryWriteTranslation.WriteEmbedded<IWorldspaceGetter>(group, stream);
        }
        streams[0] = groupByteStream;
        Parallel.ForEach(group, parallelWriteParameters.ParallelOptions, (worldspace, worldspaceState, worldspaceCounter) =>
        {
            var worldTrib = new MemoryTributary();
            using (var writer = new MutagenWriter(worldTrib, bundle with {}, dispose: false))
            {
                using (HeaderExport.Header(
                           writer: writer,
                           record: RecordTypes.WRLD,
                           type: ObjectType.Record))
                {
                    WorldspaceBinaryWriteTranslation.WriteEmbedded(
                        item: worldspace,
                        writer: writer);
                    WorldspaceBinaryWriteTranslation.WriteRecordTypes(
                        item: worldspace,
                        writer: writer,
                        translationParams: null);
                }
            }
            var topCell = worldspace.TopCell;
            var subCells = worldspace.SubCells;
            if (subCells?.Count == 0
                && topCell == null)
            {
                streams[worldspaceCounter + 1] = worldTrib;
                return;
            }

            Stream[] subStreams = new Stream[(subCells?.Count ?? 0) + 1];

            var worldGroupTrib = new MemoryTributary();
            var worldGroupWriter = new MutagenWriter(worldGroupTrib, bundle with {}, dispose: false);
            worldGroupWriter.Write(RecordTypes.GRUP.TypeInt);
            worldGroupWriter.Write(UtilityTranslation.Zeros.Slice(0, bundle.Constants.GroupConstants.LengthLength));
            FormKeyBinaryTranslation.Instance.Write(
                worldGroupWriter,
                worldspace.FormKey);
            worldGroupWriter.Write((int)GroupTypeEnum.WorldChildren);
            worldGroupWriter.Write(worldspace.SubCellsTimestamp);
            worldGroupWriter.Write(worldspace.SubCellsUnknown);
            topCell?.WriteToBinary(worldGroupWriter);
            subStreams[0] = worldGroupTrib;

            if (subCells != null)
            {
                Parallel.ForEach(subCells, parallelWriteParameters.ParallelOptions, (block, blockState, blockCounter) =>
                {
                    WriteBlocksParallel(
                        block,
                        (int)blockCounter + 1,
                        subStreams,
                        bundle,
                        parallelWriteParameters);
                });
            }

            worldGroupWriter.Position = 4;
            worldGroupWriter.Write((uint)(subStreams.NotNull().Select(s => s.Length).Sum()));
            streams[worldspaceCounter + 1] = new CompositeReadStream(worldTrib.AsEnumerable().And(subStreams), resetPositions: true);
        });
        PluginUtilityTranslation.CompileSetGroupLength(streams, groupBytes);
        streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
    }

    public static void WriteBlocksParallel(
        IWorldspaceBlockGetter block,
        int targetIndex,
        Stream[] streamDepositArray,
        WritingBundle bundle,
        ParallelWriteParameters parallelWriteParameters)
    {
        var items = block.Items;
        Stream[] streams = new Stream[(items?.Count ?? 0) + 1];
        byte[] groupBytes = new byte[bundle.Constants.GroupConstants.HeaderLength];
        BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
        var groupByteStream = new MemoryStream(groupBytes);
        using (var stream = new MutagenWriter(groupByteStream, bundle with {}, dispose: false))
        {
            stream.Position += 8;
            WorldspaceBlockBinaryWriteTranslation.WriteEmbedded(block, stream);
        }
        streams[0] = groupByteStream;
        if (items != null)
        {
            Parallel.ForEach(items, parallelWriteParameters.ParallelOptions, (subBlock, state, counter) =>
            {
                WriteSubBlocksParallel(
                    subBlock,
                    (int)counter + 1,
                    streams,
                    bundle,
                    parallelWriteParameters);
            });
        }
        PluginUtilityTranslation.CompileSetGroupLength(streams, groupBytes);
        streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
    }

    public static void WriteSubBlocksParallel(
        IWorldspaceSubBlockGetter subBlock,
        int targetIndex,
        Stream[] streamDepositArray,
        WritingBundle bundle,
        ParallelWriteParameters parallelWriteParameters)
    {
        var items = subBlock.Items;
        Stream[] streams = new Stream[(items?.Count ?? 0) + 1];
        byte[] groupBytes = new byte[bundle.Constants.GroupConstants.HeaderLength];
        BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
        var groupByteStream = new MemoryStream(groupBytes);
        using (var stream = new MutagenWriter(
                   groupByteStream,
                   bundle with {},
                   dispose: false))
        {
            stream.Position += 8;
            WorldspaceSubBlockBinaryWriteTranslation.WriteEmbedded(subBlock, stream);
        }
        streams[0] = groupByteStream;
        if (items != null)
        {
            Parallel.ForEach(items, parallelWriteParameters.ParallelOptions, (cell, state, counter) =>
            {
                MemoryTributary trib = new MemoryTributary();
                cell.WriteToBinary(new MutagenWriter(
                    trib,
                    bundle with {},
                    dispose: false));
                streams[(int)counter + 1] = trib;
            });
        }
        PluginUtilityTranslation.CompileSetGroupLength(streams, groupBytes);
        streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
    }
    
    public static void WriteQuestsParallel(
        IStarfieldGroupGetter<IQuestGetter> group,
        int targetIndex,
        Stream[] streamDepositArray,
        WritingBundle bundle,
        ParallelWriteParameters parallelWriteParameters)
    {
        WriteGroupParallel(group, targetIndex, streamDepositArray, bundle, parallelWriteParameters);
    }
}