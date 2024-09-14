using Noggog;
using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using static Mutagen.Bethesda.Translations.Binary.UtilityTranslation;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Plugins.Utility;

namespace Mutagen.Bethesda.Oblivion;

public partial class OblivionMod : AMod
{
    public override uint GetDefaultInitialNextFormID(bool? forceUseLowerFormIDRanges = false) =>
        GetDefaultInitialNextFormIDStatic(
            this.ModHeader.Stats.Version,
            forceUseLowerFormIDRanges);
    
    public override bool IsMaster
    {
        get => this.ModHeader.Flags.HasFlag(OblivionModHeader.HeaderFlag.Master);
        set => this.ModHeader.Flags = this.ModHeader.Flags.SetFlag(OblivionModHeader.HeaderFlag.Master, value);
    }
    
    public override bool CanBeSmallMaster => false;
    public override bool IsSmallMaster
    {
        get => false;
        set => throw new ArgumentException("Tried to set light master flag on unsupported mod type");
    }
    public override bool CanBeMediumMaster => false;
    public override bool IsMediumMaster
    {
        get => false;
        set => throw new ArgumentException("Tried to set half master flag on unsupported mod type");
    }

    public override bool ListsOverriddenForms => false;
    
    public override IReadOnlyList<IFormLinkGetter<IMajorRecordGetter>>? OverriddenForms => null;

    internal static uint GetDefaultInitialNextFormIDStatic(float headerVersion,
        bool? forceUseLowerFormIDRanges)
    {
        return HeaderVersionHelper.GetInitialFormId(
            release: GameRelease.Oblivion,
            allowedReleases: null,
            headerVersion: headerVersion,
            forceUseLowerFormIDRanges: forceUseLowerFormIDRanges,
            constants: GameConstants.Get(GameRelease.Oblivion));
    }

    internal class OblivionCreateBuilderInstantiator : IBinaryReadBuilderInstantiator<IOblivionMod, IOblivionModDisposableGetter, GroupMask>
    {
        public static readonly OblivionCreateBuilderInstantiator Instance = new();
        
        public IOblivionMod Mutable(BinaryReadMutableBuilder<IOblivionMod, IOblivionModDisposableGetter, GroupMask> builder)
        {
            if (builder._param._path != null)
            {
                return OblivionMod.CreateFromBinary(
                    path: new ModPath(
                        builder._param.ModKey,
                        builder._param._path.Value),
                    errorMask: builder._param.ErrorMaskBuilder,
                    param: builder._param.Params,
                    importMask: builder._param.GroupMask);
            }
            else if (builder._param._streamFactory != null)
            {
                var stream = builder._param._streamFactory();
                var recordCache =  new RecordTypeInfoCacheReader(() =>
                {
                    var stream1 = builder._param._streamFactory();
                    if (stream1 == stream)
                    {
                        throw new ArgumentException("Stream factory provided returned the same stream twice");
                    }
                    var meta = ParsingMeta.Factory(builder._param.Params, builder._param.GameRelease, builder._param.ModKey, stream1);
                    return new MutagenBinaryReadStream(stream, meta);
                });

                return OblivionMod.CreateFromBinary(
                    stream: stream,
                    modKey: builder._param.ModKey,
                    infoCache: recordCache,
                    param: builder._param.Params,
                    importMask: builder._param.GroupMask);
            }
            else
            {
                throw new ArgumentException("Path or stream factory needs to be specified");
            }
        }

        public IOblivionModDisposableGetter Readonly(BinaryReadBuilder<IOblivionMod, IOblivionModDisposableGetter, GroupMask> builder)
        {
            if (builder._param._path != null)
            {
                return OblivionMod.CreateFromBinaryOverlay(
                    path: new ModPath(
                        builder._param.ModKey,
                        builder._param._path.Value),
                    param: builder._param.Params);
            }
            else if (builder._param._streamFactory != null)
            {
                var stream = builder._param._streamFactory();

                return OblivionMod.CreateFromBinaryOverlay(
                    stream: stream,
                    modKey: builder._param.ModKey,
                    param: builder._param.Params);
            }
            else
            {
                throw new ArgumentException("Path or stream factory needs to be specified");
            }
        }
    }

    public static BinaryReadBuilderSourceStreamFactoryChoice<IOblivionMod, IOblivionModDisposableGetter, GroupMask> 
        Create => new(
        GameRelease.Oblivion, 
        OblivionCreateBuilderInstantiator.Instance,
        needsRecordTypeInfoCacheReader: true);

    internal class OblivionWriteBuilderInstantiator : IBinaryWriteBuilderWriter<IOblivionModGetter>
    {
        public static readonly OblivionWriteBuilderInstantiator Instance = new();

        public async Task WriteAsync(IOblivionModGetter mod, BinaryWriteBuilderParams<IOblivionModGetter> param)
        {
            Write(mod, param);
        }

        public void Write(IOblivionModGetter mod, BinaryWriteBuilderParams<IOblivionModGetter> param)
        {
            if (param._path != null)
            {
                mod.WriteToBinary(param._path.Value, param._param);
            }
            else if (param._stream != null)
            {
                mod.WriteToBinary(param._stream, param._param);
            }
            else
            {
                throw new ArgumentException("Path or stream factory needs to be specified");
            }
        }
    }

    public BinaryModdedWriteBuilderLoadOrderChoice<IOblivionModGetter> 
        BeginWrite => new(
        this, 
        OblivionWriteBuilderInstantiator.Instance);

    IBinaryModdedWriteBuilderLoadOrderChoice IModGetter.BeginWrite => this.BeginWrite;

    public static BinaryWriteBuilderLoadOrderChoice<IOblivionModGetter> WriteBuilder() => new(GameRelease.Oblivion, OblivionWriteBuilderInstantiator.Instance);
}

internal partial class OblivionModBinaryOverlay
{
    public uint GetDefaultInitialNextFormID(bool? forceUseLowerFormIDRanges = false) =>
        OblivionMod.GetDefaultInitialNextFormIDStatic(
            this.ModHeader.Stats.Version,
            forceUseLowerFormIDRanges);
    
    public bool IsMaster => this.ModHeader.Flags.HasFlag(OblivionModHeader.HeaderFlag.Master);
    public bool CanBeSmallMaster => false;
    public bool IsSmallMaster => false;
    public bool CanBeMediumMaster => false;
    public bool IsMediumMaster => false;
    public bool ListsOverriddenForms => false;
    public MasterStyle MasterStyle => this.GetMasterStyle();
    
    public IReadOnlyList<IFormLinkGetter<IMajorRecordGetter>>? OverriddenForms => null;

    public IBinaryModdedWriteBuilderLoadOrderChoice 
        BeginWrite => new BinaryModdedWriteBuilderLoadOrderChoice<IOblivionModGetter>(
        this, 
        OblivionMod.OblivionWriteBuilderInstantiator.Instance);
}

partial class OblivionModCommon
{
    public static void WriteCellsParallel(
        IOblivionListGroupGetter<ICellBlockGetter> group,
        int targetIndex,
        Stream[] streamDepositArray,
        WritingBundle bundle,
        ParallelWriteParameters parallelWriteParameters)
    {
        if (group.Records.Count == 0) return;
        Stream[] streams = new Stream[group.Records.Count + 1];
        byte[] groupBytes = new byte[GameConstants.Oblivion.GroupConstants.HeaderLength];
        BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
        var groupByteStream = new MemoryStream(groupBytes);
        using (var stream = new MutagenWriter(groupByteStream, bundle with {}, dispose: false))
        {
            stream.Position += 8;
            OblivionListGroupBinaryWriteTranslation.WriteEmbedded<ICellBlockGetter>(group, stream);
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
        byte[] groupBytes = new byte[GameConstants.Oblivion.GroupConstants.HeaderLength];
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
        byte[] groupBytes = new byte[GameConstants.Oblivion.GroupConstants.HeaderLength];
        var groupByteStream = new MemoryStream(groupBytes);
        BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
        using (var stream = new MutagenWriter(groupByteStream, bundle with {}, dispose: false))
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
                cell.WriteToBinary(new MutagenWriter(trib, bundle with {}, dispose: false));
                streams[(int)counter + 1] = trib;
            });
        }
        PluginUtilityTranslation.CompileSetGroupLength(streams, groupBytes);
        streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
    }

    public static void WriteWorldspacesParallel(
        IOblivionGroupGetter<IWorldspaceGetter> group,
        int targetIndex,
        Stream[] streamDepositArray,
        WritingBundle bundle,
        ParallelWriteParameters parallelWriteParameters)
    {
        var cache = group.RecordCache;
        if (cache == null || cache.Count == 0) return;
        Stream[] streams = new Stream[cache.Count + 1];
        byte[] groupBytes = new byte[GameConstants.Oblivion.GroupConstants.HeaderLength];
        BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
        var groupByteStream = new MemoryStream(groupBytes);
        using (var stream = new MutagenWriter(groupByteStream, bundle with {}, dispose: false))
        {
            stream.Position += 8;
            OblivionGroupBinaryWriteTranslation.WriteEmbedded<IWorldspaceGetter>(group, stream);
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
            var road = worldspace.Road;
            var topCell = worldspace.TopCell;
            var subCells = worldspace.SubCells;
            if (subCells?.Count == 0
                && road == null
                && topCell == null)
            {
                streams[worldspaceCounter + 1] = worldTrib;
                return;
            }

            Stream[] subStreams = new Stream[(subCells?.Count ?? 0) + 1];

            var worldGroupTrib = new MemoryTributary();
            var worldGroupWriter = new MutagenWriter(worldGroupTrib, bundle with {}, dispose: false);
            worldGroupWriter.Write(RecordTypes.GRUP.TypeInt);
            worldGroupWriter.Write(Zeros.Slice(0, GameConstants.Oblivion.GroupConstants.LengthLength));
            FormKeyBinaryTranslation.Instance.Write(
                worldGroupWriter,
                worldspace);
            worldGroupWriter.Write((int)GroupTypeEnum.WorldChildren);
            worldGroupWriter.Write(worldspace.SubCellsTimestamp);
            road?.WriteToBinary(worldGroupWriter);
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
        Stream[] streams = new Stream[(items?.Count ?? 0)+ 1];
        byte[] groupBytes = new byte[GameConstants.Oblivion.GroupConstants.HeaderLength];
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
        byte[] groupBytes = new byte[GameConstants.Oblivion.GroupConstants.HeaderLength];
        BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
        var groupByteStream = new MemoryStream(groupBytes);
        using (var stream = new MutagenWriter(groupByteStream, bundle with {}, dispose: false))
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
                cell.WriteToBinary(new MutagenWriter(trib, bundle with {}, dispose: false));
                streams[(int)counter + 1] = trib;
            });
        }
        PluginUtilityTranslation.CompileSetGroupLength(streams, groupBytes);
        streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
    }

    public static void WriteDialogTopicsParallel(
        IOblivionGroupGetter<IDialogTopicGetter> group,
        int targetIndex,
        Stream[] streamDepositArray,
        WritingBundle bundle,
        ParallelWriteParameters parallelWriteParameters)
    {
        WriteGroupParallel(group, targetIndex, streamDepositArray, bundle, parallelWriteParameters);
    }

    partial void GetCustomRecordCount(IOblivionModGetter item, Action<uint> setter)
    {
        uint count = 0;
        // Tally Cell Group counts
        int cellSubGroupCount(ICellGetter cell)
        {
            int cellGroupCount = 0;
            if ((cell.Temporary?.Count ?? 0) > 0
                || cell.PathGrid != null
                || cell.Landscape != null)
            {
                cellGroupCount++;
            }
            if ((cell.Persistent?.Count ?? 0) > 0)
            {
                cellGroupCount++;
            }
            if ((cell.VisibleWhenDistant?.Count ?? 0) > 0)
            {
                cellGroupCount++;
            }
            if (cellGroupCount > 0)
            {
                cellGroupCount++;
            }
            return cellGroupCount;
        }
        count += (uint)item.Cells.Records.Count; // Block Count
        count += (uint)item.Cells.Records.Sum(block => block.SubBlocks?.Count ?? 0); // Sub Block Count
        count += (uint)item.Cells.Records
            .SelectMany(block => block.SubBlocks)
            .SelectMany(subBlock => subBlock.Cells)
            .Select(cellSubGroupCount)
            .Sum();

        // Tally Worldspace Group Counts
        count += (uint)item.Worldspaces.Sum(wrld => wrld.SubCells?.Count ?? 0); // Cell Blocks
        count += (uint)item.Worldspaces
            .SelectMany(wrld => wrld.SubCells)
            .Sum(block => block.Items?.Count ?? 0); // Cell Sub Blocks
        count += (uint)item.Worldspaces
            .SelectMany(wrld => wrld.SubCells)
            .SelectMany(block => block.Items)
            .SelectMany(subBlock => subBlock.Items)
            .Sum(cellSubGroupCount); // Cell sub groups

        // Tally Dialog Group Counts
        count += (uint)item.DialogTopics.RecordCache.Count;
        
        // Set count
        setter(count);
    }
}