﻿using Noggog;
using System.Buffers.Binary;
using Loqui.Internal;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using static Mutagen.Bethesda.Translations.Binary.UtilityTranslation;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Plugins.Utility;

namespace Mutagen.Bethesda.Morrowind;

public partial class MorrowindMod : AMod
{
    public override uint GetDefaultInitialNextFormID(bool? forceUseLowerFormIDRanges = false) =>
        GetDefaultInitialNextFormIDStatic(
            this.ModHeader.Stats.Version,
            forceUseLowerFormIDRanges);
    
    public override bool IsMaster
    {
        get => this.ModHeader.Flags.HasFlag(MorrowindModHeader.HeaderFlag.Master);
        set => this.ModHeader.Flags = this.ModHeader.Flags.SetFlag(MorrowindModHeader.HeaderFlag.Master, value);
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
            release: GameRelease.Morrowind,
            allowedReleases: null,
            headerVersion: headerVersion,
            forceUseLowerFormIDRanges: forceUseLowerFormIDRanges,
            constants: GameConstants.Get(GameRelease.Morrowind));
    }

    internal class MorrowindCreateBuilderInstantiator : IBinaryReadBuilderInstantiator<IMorrowindMod, IMorrowindModDisposableGetter, GroupMask>
    {
        public static readonly MorrowindCreateBuilderInstantiator Instance = new();
        
        public IMorrowindMod Mutable(BinaryReadMutableBuilder<IMorrowindMod, IMorrowindModDisposableGetter, GroupMask> builder)
        {
            if (builder._param._path != null)
            {
                return MorrowindMod.CreateFromBinary(
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

                return MorrowindMod.CreateFromBinary(
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

        public IMorrowindModDisposableGetter Readonly(BinaryReadBuilder<IMorrowindMod, IMorrowindModDisposableGetter, GroupMask> builder)
        {
            if (builder._param._path != null)
            {
                return MorrowindMod.CreateFromBinaryOverlay(
                    path: new ModPath(
                        builder._param.ModKey,
                        builder._param._path.Value),
                    param: builder._param.Params);
            }
            else if (builder._param._streamFactory != null)
            {
                var stream = builder._param._streamFactory();

                return MorrowindMod.CreateFromBinaryOverlay(
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

    public static BinaryReadBuilderSourceStreamFactoryChoice<IMorrowindMod, IMorrowindModDisposableGetter, GroupMask> 
        Create => new(
        GameRelease.Morrowind, 
        MorrowindCreateBuilderInstantiator.Instance,
        needsRecordTypeInfoCacheReader: true);

    internal class MorrowindWriteBuilderInstantiator : IBinaryWriteBuilderWriter<IMorrowindModGetter>
    {
        public static readonly MorrowindWriteBuilderInstantiator Instance = new();

        public async Task WriteAsync(IMorrowindModGetter mod, BinaryWriteBuilderParams<IMorrowindModGetter> param)
        {
            Write(mod, param);
        }

        public void Write(IMorrowindModGetter mod, BinaryWriteBuilderParams<IMorrowindModGetter> param)
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

    public BinaryModdedWriteBuilderTargetChoice<IMorrowindModGetter> 
        BeginWrite => new(
        this, 
        MorrowindWriteBuilderInstantiator.Instance);

    IBinaryModdedWriteBuilderTargetChoice IModGetter.BeginWrite => this.BeginWrite;

    public static BinaryWriteBuilderTargetChoice<IMorrowindModGetter> WriteBuilder() =>
        new(GameRelease.Morrowind, MorrowindWriteBuilderInstantiator.Instance);

    IMod IModGetter.DeepCopy() => this.DeepCopy();
}

public partial interface IMorrowindModGetter
{
    BinaryModdedWriteBuilderTargetChoice<IMorrowindModGetter> BeginWrite { get; }
}

partial class MorrowindModSetterTranslationCommon
{
    partial void DeepCopyInCustom(IMorrowindMod item, IMorrowindModGetter rhs, ErrorMaskBuilder? errorMask,
        TranslationCrystal? copyMask, bool deepCopy)
    {
        if (!deepCopy) return;
        if (item is not AMod mod)
        {
            throw new NotImplementedException();
        }
        mod.SetModKey(rhs.ModKey);
    }

    public partial MorrowindMod DeepCopyGetNew(IMorrowindModGetter item)
    {
        return new MorrowindMod(item.ModKey);
    }
}

internal partial class MorrowindModBinaryOverlay
{
    public uint GetDefaultInitialNextFormID(bool? forceUseLowerFormIDRanges = false) =>
        MorrowindMod.GetDefaultInitialNextFormIDStatic(
            this.ModHeader.Stats.Version,
            forceUseLowerFormIDRanges);
    
    public bool IsMaster => this.ModHeader.Flags.HasFlag(MorrowindModHeader.HeaderFlag.Master);
    public bool CanBeSmallMaster => false;
    public bool IsSmallMaster => false;
    public bool CanBeMediumMaster => false;
    public bool IsMediumMaster => false;
    public bool ListsOverriddenForms => false;
    public MasterStyle MasterStyle => this.GetMasterStyle();
    IMod IModGetter.DeepCopy() => this.DeepCopy();
    
    public IReadOnlyList<IFormLinkGetter<IMajorRecordGetter>>? OverriddenForms => null;

    IBinaryModdedWriteBuilderTargetChoice IModGetter.BeginWrite => 
        new BinaryModdedWriteBuilderTargetChoice<IMorrowindModGetter>(
            this, 
            MorrowindMod.MorrowindWriteBuilderInstantiator.Instance);

    public BinaryModdedWriteBuilderTargetChoice<IMorrowindModGetter> BeginWrite => 
        new BinaryModdedWriteBuilderTargetChoice<IMorrowindModGetter>(
            this, 
            MorrowindMod.MorrowindWriteBuilderInstantiator.Instance);
}

partial class MorrowindModCommon
{
    public static void WriteCellsParallel(
        IMorrowindListGroupGetter<ICellBlockGetter> group,
        int targetIndex,
        Stream[] streamDepositArray,
        WritingBundle bundle,
        ParallelWriteParameters parallelWriteParameters)
    {
        if (group.Records.Count == 0) return;
        Stream[] streams = new Stream[group.Records.Count + 1];
        byte[] groupBytes = new byte[GameConstants.Morrowind.GroupConstants.HeaderLength];
        BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
        var groupByteStream = new MemoryStream(groupBytes);
        using (var stream = new MutagenWriter(groupByteStream, bundle with {}, dispose: false))
        {
            stream.Position += 8;
            MorrowindListGroupBinaryWriteTranslation.WriteEmbedded<ICellBlockGetter>(group, stream);
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
        byte[] groupBytes = new byte[GameConstants.Morrowind.GroupConstants.HeaderLength];
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
        byte[] groupBytes = new byte[GameConstants.Morrowind.GroupConstants.HeaderLength];
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

    // public static void WriteWorldspacesParallel(
    //     IMorrowindGroupGetter<IWorldspaceGetter> group,
    //     int targetIndex,
    //     Stream[] streamDepositArray,
    //     WritingBundle bundle,
    //     ParallelWriteParameters parallelWriteParameters)
    // {
    //     var cache = group.RecordCache;
    //     if (cache == null || cache.Count == 0) return;
    //     Stream[] streams = new Stream[cache.Count + 1];
    //     byte[] groupBytes = new byte[GameConstants.Morrowind.GroupConstants.HeaderLength];
    //     BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
    //     var groupByteStream = new MemoryStream(groupBytes);
    //     using (var stream = new MutagenWriter(groupByteStream, bundle with {}, dispose: false))
    //     {
    //         stream.Position += 8;
    //         MorrowindGroupBinaryWriteTranslation.WriteEmbedded<IWorldspaceGetter>(group, stream);
    //     }
    //     streams[0] = groupByteStream;
    //     Parallel.ForEach(group, parallelWriteParameters.ParallelOptions, (worldspace, worldspaceState, worldspaceCounter) =>
    //     {
    //         var worldTrib = new MemoryTributary();
    //         using (var writer = new MutagenWriter(worldTrib, bundle with {}, dispose: false))
    //         {
    //             using (HeaderExport.Header(
    //                        writer: writer,
    //                        record: RecordTypes.WRLD,
    //                        type: ObjectType.Record))
    //             {
    //                 WorldspaceBinaryWriteTranslation.WriteEmbedded(
    //                     item: worldspace,
    //                     writer: writer);
    //                 WorldspaceBinaryWriteTranslation.WriteRecordTypes(
    //                     item: worldspace,
    //                     writer: writer,
    //                     translationParams: null);
    //             }
    //         }
    //         var road = worldspace.Road;
    //         var topCell = worldspace.TopCell;
    //         var subCells = worldspace.SubCells;
    //         if (subCells?.Count == 0
    //             && road == null
    //             && topCell == null)
    //         {
    //             streams[worldspaceCounter + 1] = worldTrib;
    //             return;
    //         }
    //
    //         Stream[] subStreams = new Stream[(subCells?.Count ?? 0) + 1];
    //
    //         var worldGroupTrib = new MemoryTributary();
    //         var worldGroupWriter = new MutagenWriter(worldGroupTrib, bundle with {}, dispose: false);
    //         worldGroupWriter.Write(RecordTypes.GRUP.TypeInt);
    //         worldGroupWriter.Write(Zeros.Slice(0, GameConstants.Morrowind.GroupConstants.LengthLength));
    //         FormKeyBinaryTranslation.Instance.Write(
    //             worldGroupWriter,
    //             worldspace);
    //         worldGroupWriter.Write((int)GroupTypeEnum.WorldChildren);
    //         worldGroupWriter.Write(worldspace.SubCellsTimestamp);
    //         road?.WriteToBinary(worldGroupWriter);
    //         topCell?.WriteToBinary(worldGroupWriter);
    //         subStreams[0] = worldGroupTrib;
    //
    //         if (subCells != null)
    //         {
    //             Parallel.ForEach(subCells, parallelWriteParameters.ParallelOptions, (block, blockState, blockCounter) =>
    //             {
    //                 WriteBlocksParallel(
    //                     block,
    //                     (int)blockCounter + 1,
    //                     subStreams,
    //                     bundle,
    //                     parallelWriteParameters);
    //             });
    //         }
    //
    //         worldGroupWriter.Position = 4;
    //         worldGroupWriter.Write((uint)(subStreams.WhereNotNull().Select(s => s.Length).Sum()));
    //         streams[worldspaceCounter + 1] = new CompositeReadStream(worldTrib.AsEnumerable().And(subStreams), resetPositions: true);
    //     });
    //     PluginUtilityTranslation.CompileSetGroupLength(streams, groupBytes);
    //     streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
    // }

    // public static void WriteBlocksParallel(
    //     IWorldspaceBlockGetter block,
    //     int targetIndex,
    //     Stream[] streamDepositArray,
    //     WritingBundle bundle,
    //     ParallelWriteParameters parallelWriteParameters)
    // {
    //     var items = block.Items;
    //     Stream[] streams = new Stream[(items?.Count ?? 0)+ 1];
    //     byte[] groupBytes = new byte[GameConstants.Morrowind.GroupConstants.HeaderLength];
    //     BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
    //     var groupByteStream = new MemoryStream(groupBytes);
    //     using (var stream = new MutagenWriter(groupByteStream, bundle with {}, dispose: false))
    //     {
    //         stream.Position += 8;
    //         WorldspaceBlockBinaryWriteTranslation.WriteEmbedded(block, stream);
    //     }
    //     streams[0] = groupByteStream;
    //     if (items != null)
    //     {
    //         Parallel.ForEach(items, parallelWriteParameters.ParallelOptions, (subBlock, state, counter) =>
    //         {
    //             WriteSubBlocksParallel(
    //                 subBlock,
    //                 (int)counter + 1,
    //                 streams,
    //                 bundle,
    //                 parallelWriteParameters);
    //         });
    //     }
    //     PluginUtilityTranslation.CompileSetGroupLength(streams, groupBytes);
    //     streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
    // }

    // public static void WriteSubBlocksParallel(
    //     IWorldspaceSubBlockGetter subBlock,
    //     int targetIndex,
    //     Stream[] streamDepositArray,
    //     WritingBundle bundle,
    //     ParallelWriteParameters parallelWriteParameters)
    // {
    //     var items = subBlock.Items;
    //     Stream[] streams = new Stream[(items?.Count ?? 0) + 1];
    //     byte[] groupBytes = new byte[GameConstants.Morrowind.GroupConstants.HeaderLength];
    //     BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
    //     var groupByteStream = new MemoryStream(groupBytes);
    //     using (var stream = new MutagenWriter(groupByteStream, bundle with {}, dispose: false))
    //     {
    //         stream.Position += 8;
    //         WorldspaceSubBlockBinaryWriteTranslation.WriteEmbedded(subBlock, stream);
    //     }
    //     streams[0] = groupByteStream;
    //     if (items != null)
    //     {
    //         Parallel.ForEach(items, parallelWriteParameters.ParallelOptions, (cell, state, counter) =>
    //         {
    //             MemoryTributary trib = new MemoryTributary();
    //             cell.WriteToBinary(new MutagenWriter(trib, bundle with {}, dispose: false));
    //             streams[(int)counter + 1] = trib;
    //         });
    //     }
    //     PluginUtilityTranslation.CompileSetGroupLength(streams, groupBytes);
    //     streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
    // }

    // public static void WriteDialogTopicsParallel(
    //     IMorrowindGroupGetter<IDialogTopicGetter> group,
    //     int targetIndex,
    //     Stream[] streamDepositArray,
    //     WritingBundle bundle,
    //     ParallelWriteParameters parallelWriteParameters)
    // {
    //     WriteGroupParallel(group, targetIndex, streamDepositArray, bundle, parallelWriteParameters);
    // }

    partial void GetCustomRecordCount(IMorrowindModGetter item, Action<uint> setter)
    {
        uint count = 0;
        // Tally Cell Group counts
        // int cellSubGroupCount(ICellGetter cell)
        // {
        //     int cellGroupCount = 0;
        //     if ((cell.Temporary?.Count ?? 0) > 0
        //         || cell.PathGrid != null
        //         || cell.Landscape != null)
        //     {
        //         cellGroupCount++;
        //     }
        //     if ((cell.Persistent?.Count ?? 0) > 0)
        //     {
        //         cellGroupCount++;
        //     }
        //     if ((cell.VisibleWhenDistant?.Count ?? 0) > 0)
        //     {
        //         cellGroupCount++;
        //     }
        //     if (cellGroupCount > 0)
        //     {
        //         cellGroupCount++;
        //     }
        //     return cellGroupCount;
        // }
        // count += (uint)item.Cells.Records.Count; // Block Count
        // count += (uint)item.Cells.Records.Sum(block => block.SubBlocks?.Count ?? 0); // Sub Block Count
        // count += (uint)item.Cells.Records
        //     .SelectMany(block => block.SubBlocks)
        //     .SelectMany(subBlock => subBlock.Cells)
        //     .Select(cellSubGroupCount)
        //     .Sum();
        //
        // // Tally Worldspace Group Counts
        // count += (uint)item.Worldspaces.Sum(wrld => wrld.SubCells?.Count ?? 0); // Cell Blocks
        // count += (uint)item.Worldspaces
        //     .SelectMany(wrld => wrld.SubCells)
        //     .Sum(block => block.Items?.Count ?? 0); // Cell Sub Blocks
        // count += (uint)item.Worldspaces
        //     .SelectMany(wrld => wrld.SubCells)
        //     .SelectMany(block => block.Items)
        //     .SelectMany(subBlock => subBlock.Items)
        //     .Sum(cellSubGroupCount); // Cell sub groups
        //
        // // Tally Dialog Group Counts
        // count += (uint)item.DialogTopics.RecordCache.Count;
        
        // Set count
        setter(count);
    }
}