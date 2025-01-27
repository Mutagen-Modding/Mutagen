using System.Buffers.Binary;
using DynamicData.Aggregation;
using Loqui.Internal;
using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Utility;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;

namespace Mutagen.Bethesda.Fallout4;

public partial class Fallout4Mod : AMod
{
    public override uint GetDefaultInitialNextFormID(bool? forceUseLowerFormIDRanges = false) =>
        GetDefaultInitialNextFormIDStatic(this.Fallout4Release, this.ModHeader.Stats.Version, forceUseLowerFormIDRanges);
    
    public override bool IsMaster
    {
        get => this.ModHeader.Flags.HasFlag(Fallout4ModHeader.HeaderFlag.Master);
        set => this.ModHeader.Flags = this.ModHeader.Flags.SetFlag(Fallout4ModHeader.HeaderFlag.Master, value);
    }
    
    public override bool CanBeSmallMaster => true;

    public override bool IsSmallMaster
    {
        get => this.ModHeader.Flags.HasFlag(Fallout4ModHeader.HeaderFlag.Small);
        set => this.ModHeader.Flags = this.ModHeader.Flags.SetFlag(Fallout4ModHeader.HeaderFlag.Small, value);
    }
    public override bool CanBeMediumMaster => false;
    public override bool IsMediumMaster
    {
        get => false;
        set => throw new ArgumentException("Tried to set half master flag on unsupported mod type");
    }

    public override bool ListsOverriddenForms => true;

    partial void CustomCtor()
    {
        this.ModHeader.FormVersion = GameConstants.Get(GameRelease).DefaultFormVersion!.Value;
    }
    
    internal static uint GetDefaultInitialNextFormIDStatic(Fallout4Release release, float headerVersion, bool? forceUseLowerFormIDRanges)
    {
        return HeaderVersionHelper.GetInitialFormId(
            release: release.ToGameRelease(),
            allowedReleases: null,
            headerVersion: headerVersion,
            forceUseLowerFormIDRanges: forceUseLowerFormIDRanges,
            constants: GameConstants.Get(release.ToGameRelease()));
    }

    public override IReadOnlyList<IFormLinkGetter<IMajorRecordGetter>>? OverriddenForms =>
        this.ModHeader.OverriddenForms;

    internal class Fallout4CreateBuilderInstantiator : IBinaryReadBuilderInstantiator<IFallout4Mod, IFallout4ModDisposableGetter, GroupMask>
    {
        public static readonly Fallout4CreateBuilderInstantiator Instance = new();
        
        public IFallout4Mod Mutable(BinaryReadMutableBuilder<IFallout4Mod, IFallout4ModDisposableGetter, GroupMask> builder)
        {
            if (builder._param._path != null)
            {
                return Fallout4Mod.CreateFromBinary(
                    path: new ModPath(
                        builder._param.ModKey,
                        builder._param._path.Value),
                    release: builder._param.GameRelease.ToFallout4Release(),
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

                return Fallout4Mod.CreateFromBinary(
                    stream: stream,
                    release: builder._param.GameRelease.ToFallout4Release(),
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

        public IFallout4ModDisposableGetter Readonly(BinaryReadBuilder<IFallout4Mod, IFallout4ModDisposableGetter, GroupMask> builder)
        {
            if (builder._param._path != null)
            {
                return Fallout4Mod.CreateFromBinaryOverlay(
                    path: new ModPath(
                        builder._param.ModKey,
                        builder._param._path.Value),
                    release: builder._param.GameRelease.ToFallout4Release(),
                    param: builder._param.Params);
            }
            else if (builder._param._streamFactory != null)
            {
                var stream = builder._param._streamFactory();

                return Fallout4Mod.CreateFromBinaryOverlay(
                    stream: stream,
                    release: builder._param.GameRelease.ToFallout4Release(),
                    modKey: builder._param.ModKey,
                    param: builder._param.Params);
            }
            else
            {
                throw new ArgumentException("Path or stream factory needs to be specified");
            }
        }
    }
    
    public static BinaryReadBuilderSourceStreamFactoryChoice<IFallout4Mod, IFallout4ModDisposableGetter, GroupMask> 
        Create(Fallout4Release release) => new( 
        release.ToGameRelease(), 
        Fallout4CreateBuilderInstantiator.Instance,
        needsRecordTypeInfoCacheReader: false);
    
    internal class Fallout4WriteBuilderInstantiator : IBinaryWriteBuilderWriter<IFallout4ModGetter>
    {
        public static readonly Fallout4WriteBuilderInstantiator Instance = new();

        public async Task WriteAsync(IFallout4ModGetter mod, BinaryWriteBuilderParams<IFallout4ModGetter> param)
        {
            Write(mod, param);
        }

        public void Write(IFallout4ModGetter mod, BinaryWriteBuilderParams<IFallout4ModGetter> param)
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

    public BinaryModdedWriteBuilderTargetChoice<IFallout4ModGetter> 
        BeginWrite => new(
        this, 
        Fallout4WriteBuilderInstantiator.Instance);

    IBinaryModdedWriteBuilderTargetChoice IModGetter.BeginWrite => this.BeginWrite;

    public static BinaryWriteBuilderTargetChoice<IFallout4ModGetter> WriteBuilder(Fallout4Release release) =>
        new(release.ToGameRelease(), Fallout4WriteBuilderInstantiator.Instance);
    
    IMod IModGetter.DeepCopy() => this.DeepCopy();
}

public partial interface IFallout4ModGetter
{
    BinaryModdedWriteBuilderTargetChoice<IFallout4ModGetter> BeginWrite { get; }
}

partial class Fallout4ModSetterTranslationCommon
{
    partial void DeepCopyInCustom(IFallout4Mod item, IFallout4ModGetter rhs, ErrorMaskBuilder? errorMask,
        TranslationCrystal? copyMask, bool deepCopy)
    {
        if (!deepCopy) return;
        if (item is not AMod mod)
        {
            throw new NotImplementedException();
        }
        mod.SetModKey(rhs.ModKey);
    }

    public partial Fallout4Mod DeepCopyGetNew(IFallout4ModGetter item)
    {
        return new Fallout4Mod(item.ModKey, item.Fallout4Release);
    }
}

internal partial class Fallout4ModBinaryOverlay
{
    public uint GetDefaultInitialNextFormID(bool? forceUseLowerFormIDRanges = false) =>
        Fallout4Mod.GetDefaultInitialNextFormIDStatic(
            this.Fallout4Release,
            this.ModHeader.Stats.Version,
            forceUseLowerFormIDRanges);
    
    public bool IsMaster => this.ModHeader.Flags.HasFlag(Fallout4ModHeader.HeaderFlag.Master);
    public bool CanBeSmallMaster => true;
    public bool IsSmallMaster => this.ModHeader.Flags.HasFlag(Fallout4ModHeader.HeaderFlag.Small);

    public bool CanBeMediumMaster => false;
    public bool IsMediumMaster => false;
    public bool ListsOverriddenForms => true;
    public MasterStyle MasterStyle => this.GetMasterStyle();
    
    public IReadOnlyList<IFormLinkGetter<IMajorRecordGetter>>? OverriddenForms =>
        this.ModHeader.OverriddenForms;


    IBinaryModdedWriteBuilderTargetChoice IModGetter.BeginWrite => 
        new BinaryModdedWriteBuilderTargetChoice<IFallout4ModGetter>(
            this, 
            Fallout4Mod.Fallout4WriteBuilderInstantiator.Instance);

    public BinaryModdedWriteBuilderTargetChoice<IFallout4ModGetter> BeginWrite => 
        new BinaryModdedWriteBuilderTargetChoice<IFallout4ModGetter>(
            this, 
            Fallout4Mod.Fallout4WriteBuilderInstantiator.Instance);
    
    IMod IModGetter.DeepCopy() => this.DeepCopy();
}

partial class Fallout4ModCommon
{
    public static void WriteCellsParallel(
        IFallout4ListGroupGetter<ICellBlockGetter> group,
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
            Fallout4ListGroupBinaryWriteTranslation.WriteEmbedded<ICellBlockGetter>(group, stream);
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
        IFallout4GroupGetter<IWorldspaceGetter> group,
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
            Fallout4GroupBinaryWriteTranslation.WriteEmbedded<IWorldspaceGetter>(group, stream);
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
            worldGroupWriter.Write((uint)(subStreams.WhereNotNull().Select(s => s.Length).Sum()));
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
        IFallout4GroupGetter<IQuestGetter> group,
        int targetIndex,
        Stream[] streamDepositArray,
        WritingBundle bundle,
        ParallelWriteParameters parallelWriteParameters)
    {
        WriteGroupParallel(group, targetIndex, streamDepositArray, bundle, parallelWriteParameters);
    }

    partial void GetCustomRecordCount(IFallout4ModGetter item, Action<uint> setter)
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
        var quests = item.Quests.ToArray();
        count += (uint)quests.Length;
        count += (uint)quests.SelectMany(x => x.DialogTopics)
            .Select(x => x.Responses.Count > 0 ? 1 : 0)
            .Sum();
        
        // Set count
        setter(count);
    }
}
