using Noggog;
using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins;
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
    public override uint GetDefaultInitialNextFormID(bool? forceUseLowerFormIDRanges = false) => 
        GetDefaultInitialNextFormIDStatic(this.SkyrimRelease, 
            this.ModHeader.Stats.Version, 
            forceUseLowerFormIDRanges);

    public override bool CanBeSmallMaster => true;

    public override bool ListsOverriddenForms => true;
    
    public override bool IsMaster
    {
        get => this.ModHeader.Flags.HasFlag(SkyrimModHeader.HeaderFlag.Master);
        set => this.ModHeader.Flags = this.ModHeader.Flags.SetFlag(SkyrimModHeader.HeaderFlag.Master, value);
    }

    public override bool IsSmallMaster
    {
        get => this.ModHeader.Flags.HasFlag(SkyrimModHeader.HeaderFlag.Small);
        set => this.ModHeader.Flags = this.ModHeader.Flags.SetFlag(SkyrimModHeader.HeaderFlag.Small, value);
    }
    public override bool CanBeMediumMaster => false;
    public override bool IsMediumMaster
    {
        get => false;
        set => throw new ArgumentException("Tried to set half master flag on unsupported mod type");
    }

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

    internal static uint GetDefaultInitialNextFormIDStatic(
        SkyrimRelease release,
        float headerVersion,
        bool? forceUseLowerFormIDRanges)
    {
        return HeaderVersionHelper.GetInitialFormId(
            release: release.ToGameRelease(),
            allowedReleases: _allowedLowerRangeReleases,
            headerVersion: headerVersion,
            forceUseLowerFormIDRanges: forceUseLowerFormIDRanges,
            constants: GameConstants.Get(release.ToGameRelease()));
    }

    public override IReadOnlyList<IFormLinkGetter<IMajorRecordGetter>>? OverriddenForms =>
        this.ModHeader.OverriddenForms;

    internal class SkyrimCreateBuilderInstantiator : IBinaryReadBuilderInstantiator<ISkyrimMod, ISkyrimModDisposableGetter, GroupMask>
    {
        public static readonly SkyrimCreateBuilderInstantiator Instance = new();
        
        public ISkyrimMod Mutable(BinaryReadMutableBuilder<ISkyrimMod, ISkyrimModDisposableGetter, GroupMask> builder)
        {
            if (builder._param._path != null)
            {
                return SkyrimMod.CreateFromBinary(
                    path: new ModPath(
                        builder._param.ModKey,
                        builder._param._path.Value),
                    release: builder._param.GameRelease.ToSkyrimRelease(),
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

                return SkyrimMod.CreateFromBinary(
                    stream: stream,
                    release: builder._param.GameRelease.ToSkyrimRelease(),
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

        public ISkyrimModDisposableGetter Readonly(BinaryReadBuilder<ISkyrimMod, ISkyrimModDisposableGetter, GroupMask> builder)
        {
            if (builder._param._path != null)
            {
                return SkyrimMod.CreateFromBinaryOverlay(
                    path: new ModPath(
                        builder._param.ModKey,
                        builder._param._path.Value),
                    release: builder._param.GameRelease.ToSkyrimRelease(),
                    param: builder._param.Params);
            }
            else if (builder._param._streamFactory != null)
            {
                var stream = builder._param._streamFactory();

                return SkyrimMod.CreateFromBinaryOverlay(
                    stream: stream,
                    release: builder._param.GameRelease.ToSkyrimRelease(),
                    modKey: builder._param.ModKey,
                    param: builder._param.Params);
            }
            else
            {
                throw new ArgumentException("Path or stream factory needs to be specified");
            }
        }
    }
    
    public static BinaryReadBuilderSourceStreamFactoryChoice<ISkyrimMod, ISkyrimModDisposableGetter, GroupMask> 
        Create(SkyrimRelease release) => new( 
        release.ToGameRelease(), 
        SkyrimCreateBuilderInstantiator.Instance,
        needsRecordTypeInfoCacheReader: false);
    
    internal class SkyrimWriteBuilderInstantiator : IBinaryWriteBuilderWriter<ISkyrimModGetter>
    {
        public static readonly SkyrimWriteBuilderInstantiator Instance = new();

        public async Task WriteAsync(ISkyrimModGetter mod, BinaryWriteBuilderParams<ISkyrimModGetter> param)
        {
            Write(mod, param);
        }

        public void Write(ISkyrimModGetter mod, BinaryWriteBuilderParams<ISkyrimModGetter> param)
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

    public BinaryModdedWriteBuilderLoadOrderChoice<ISkyrimModGetter> 
        BeginWrite => new(
        this, 
        SkyrimWriteBuilderInstantiator.Instance);

    IBinaryModdedWriteBuilderLoadOrderChoice IModGetter.BeginWrite => this.BeginWrite;

    public static BinaryWriteBuilderLoadOrderChoice<ISkyrimModGetter> WriteBuilder(SkyrimRelease release) => new(release.ToGameRelease(), SkyrimWriteBuilderInstantiator.Instance);
}

internal partial class SkyrimModBinaryOverlay
{
    public uint GetDefaultInitialNextFormID(bool? forceUseLowerFormIDRanges = false) =>
        SkyrimMod.GetDefaultInitialNextFormIDStatic(
            this.SkyrimRelease,
            this.ModHeader.Stats.Version,
            forceUseLowerFormIDRanges);
    
    public bool IsMaster => this.ModHeader.Flags.HasFlag(SkyrimModHeader.HeaderFlag.Master);
    public bool CanBeSmallMaster => true;
    public bool IsSmallMaster => this.ModHeader.Flags.HasFlag(SkyrimModHeader.HeaderFlag.Small);
    
    public bool CanBeMediumMaster => false;
    public bool IsMediumMaster => false;
    public bool ListsOverriddenForms => true;
    public IReadOnlyList<IFormLinkGetter<IMajorRecordGetter>>? OverriddenForms =>
        this.ModHeader.OverriddenForms;

    public IBinaryModdedWriteBuilderLoadOrderChoice 
        BeginWrite => new BinaryModdedWriteBuilderLoadOrderChoice<ISkyrimModGetter>(
        this, 
        SkyrimMod.SkyrimWriteBuilderInstantiator.Instance);
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
                worldspace);
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

    partial void GetCustomRecordCount(ISkyrimModGetter item, Action<uint> setter)
    {
        uint count = 0;
        // Tally Cell Group counts
        int cellSubGroupCount(ICellGetter cell)
        {
            int cellGroupCount = 0;
            if ((cell.Temporary?.Count ?? 0) > 0
                || cell.NavigationMeshes.Count > 0
                || cell.Landscape != null)
            {
                cellGroupCount++;
            }
            if ((cell.Persistent?.Count ?? 0) > 0)
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