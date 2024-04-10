using Noggog;
using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using static Mutagen.Bethesda.Translations.Binary.UtilityTranslation;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Skyrim.Assets;
using Mutagen.Bethesda.Plugins.Utility;

namespace Mutagen.Bethesda.Skyrim;

public partial class SkyrimMod : AMod
{
    private uint GetDefaultInitialNextFormID(
        SkyrimRelease release,
        bool? forceUseLowerFormIDRanges) =>
        GetDefaultInitialNextFormID(release, this.ModHeader.Stats.Version, forceUseLowerFormIDRanges);

    public override uint MinimumCustomFormID(bool? forceUseLowerFormIDRanges = false) => 
        GetDefaultInitialNextFormID(this.SkyrimRelease, 
            this.ModHeader.Stats.Version, 
            forceUseLowerFormIDRanges);

    partial void CustomCtor()
    {
        this.ModHeader.FormVersion = GameConstants.Get(GameRelease).DefaultFormVersion!.Value;
    }

    private static readonly HashSet<GameRelease> _allowedLowerRangeReleases = new HashSet<GameRelease>()
    {
        GameRelease.SkyrimSE,
        GameRelease.SkyrimSEGog,
        GameRelease.EnderalSE,
    };

    public static uint GetDefaultInitialNextFormID(
        SkyrimRelease release,
        float headerVersion,
        bool? forceUseLowerFormIDRanges)
    {
        return HeaderVersionHelper.GetNextFormId(
            release: release.ToGameRelease(),
            allowedReleases: _allowedLowerRangeReleases,
            headerVersion: headerVersion,
            useLowerRangesVersion: 1.71f,
            forceUseLowerFormIDRanges: forceUseLowerFormIDRanges,
            higherFormIdRange: 0x800);
    }
}

internal partial class SkyrimModBinaryOverlay
{
    public uint MinimumCustomFormID(bool? forceUseLowerFormIDRanges = false) =>
        SkyrimMod.GetDefaultInitialNextFormID(
            this.SkyrimRelease,
            this.ModHeader.Stats.Version,
            forceUseLowerFormIDRanges);
}

partial class SkyrimModSetterCommon
{
    private static partial void RemapInferredAssetLinks(
        ISkyrimMod obj,
        IReadOnlyDictionary<IAssetLinkGetter, string> mapping,
        AssetLinkQuery queryCategories)
    {
        // Nothing to do here, we can't change the name of the mod
    }
}

partial class SkyrimModCommon
{
    public static partial IEnumerable<IAssetLinkGetter> GetInferredAssetLinks(ISkyrimModGetter obj, Type? assetType)
    {
        if (assetType != null && assetType != typeof(SkyrimTranslationAssetType)) yield break;
        if ((obj.ModHeader.Flags & SkyrimModHeader.HeaderFlag.Localized) == 0) yield break;

        var modName = obj.ModKey.Name;
        foreach (var language in SkyrimTranslationAssetType.Languages)
        {
            foreach (var translationExtension in SkyrimTranslationAssetType.Instance.FileExtensions)
            {
                yield return new AssetLink<SkyrimTranslationAssetType>($"{modName}_{language}.{translationExtension}");
            }
        }
    }
    
    public static void WriteCellsParallel(
        ISkyrimListGroupGetter<ICellBlockGetter> group,
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
        using (var stream = new MutagenWriter(groupByteStream, bundle with {}, dispose: false))
        {
            stream.Position += 8;
            SkyrimListGroupBinaryWriteTranslation.WriteEmbedded<ICellBlockGetter>(group, stream);
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
        ISkyrimGroupGetter<IWorldspaceGetter> group,
        int targetIndex,
        Stream[] streamDepositArray,
        WritingBundle bundle,
        ParallelWriteParameters parallelWriteParameters)
    {
        var cache = group.RecordCache;
        if (cache == null || cache.Count == 0) return;
        Stream[] streams = new Stream[cache.Count + 1];
        byte[] groupBytes = new byte[bundle.Constants.GroupConstants.HeaderLength];
        BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
        var groupByteStream = new MemoryStream(groupBytes);
        using (var stream = new MutagenWriter(groupByteStream, bundle with {}, dispose: false))
        {
            stream.Position += 8;
            SkyrimGroupBinaryWriteTranslation.WriteEmbedded<IWorldspaceGetter>(group, stream);
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
            worldGroupWriter.Write(Zeros.Slice(0, bundle.Constants.GroupConstants.LengthLength));
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

    public static void WriteDialogTopicsParallel(
        ISkyrimGroupGetter<IDialogTopicGetter> group,
        int targetIndex,
        Stream[] streamDepositArray,
        WritingBundle bundle,
        ParallelWriteParameters parallelWriteParameters)
    {
        WriteGroupParallel(group, targetIndex, streamDepositArray, bundle, parallelWriteParameters);
    }
}