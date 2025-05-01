using Noggog;
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

namespace Mutagen.Bethesda.Fallout3;

public partial class Fallout3Mod : AMod
{
    public override uint GetDefaultInitialNextFormID(bool? forceUseLowerFormIDRanges = false) =>
        GetDefaultInitialNextFormIDStatic(
            this.ModHeader.Stats.Version,
            forceUseLowerFormIDRanges);
    
    public override bool IsMaster
    {
        get => this.ModHeader.Flags.HasFlag(Fallout3ModHeader.HeaderFlag.Master);
        set => this.ModHeader.Flags = this.ModHeader.Flags.SetFlag(Fallout3ModHeader.HeaderFlag.Master, value);
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
            release: GameRelease.Fallout3,
            allowedReleases: null,
            headerVersion: headerVersion,
            forceUseLowerFormIDRanges: forceUseLowerFormIDRanges,
            constants: GameConstants.Get(GameRelease.Fallout3));
    }

    internal class Fallout3CreateBuilderInstantiator : IBinaryReadBuilderInstantiator<IFallout3Mod, IFallout3ModDisposableGetter, GroupMask>
    {
        public static readonly Fallout3CreateBuilderInstantiator Instance = new();
        
        public IFallout3Mod Mutable(BinaryReadMutableBuilder<IFallout3Mod, IFallout3ModDisposableGetter, GroupMask> builder)
        {
            if (builder._param._path != null)
            {
                return Fallout3Mod.CreateFromBinary(
                    path: new ModPath(
                        builder._param.ModKey,
                        builder._param._path.Value),
                    release: builder._param.GameRelease.ToFallout3Release(),
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

                return Fallout3Mod.CreateFromBinary(
                    stream: stream,
                    release: builder._param.GameRelease.ToFallout3Release(),
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

        public IFallout3ModDisposableGetter Readonly(BinaryReadBuilder<IFallout3Mod, IFallout3ModDisposableGetter, GroupMask> builder)
        {
            if (builder._param._path != null)
            {
                return Fallout3Mod.CreateFromBinaryOverlay(
                    path: new ModPath(
                        builder._param.ModKey,
                        builder._param._path.Value),
                    release: builder._param.GameRelease.ToFallout3Release(),
                    param: builder._param.Params);
            }
            else if (builder._param._streamFactory != null)
            {
                var stream = builder._param._streamFactory();

                return Fallout3Mod.CreateFromBinaryOverlay(
                    stream: stream,
                    release: builder._param.GameRelease.ToFallout3Release(),
                    modKey: builder._param.ModKey,
                    param: builder._param.Params);
            }
            else
            {
                throw new ArgumentException("Path or stream factory needs to be specified");
            }
        }
    }

    public static BinaryReadBuilderSourceStreamFactoryChoice<IFallout3Mod, IFallout3ModDisposableGetter, GroupMask> 
        Create(Fallout3Release release) => new(
        release.ToGameRelease(), 
        Fallout3CreateBuilderInstantiator.Instance,
        needsRecordTypeInfoCacheReader: true);

    internal class Fallout3WriteBuilderInstantiator : IBinaryWriteBuilderWriter<IFallout3ModGetter>
    {
        public static readonly Fallout3WriteBuilderInstantiator Instance = new();

        public async Task WriteAsync(IFallout3ModGetter mod, BinaryWriteBuilderParams<IFallout3ModGetter> param)
        {
            Write(mod, param);
        }

        public void Write(IFallout3ModGetter mod, BinaryWriteBuilderParams<IFallout3ModGetter> param)
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

    public BinaryModdedWriteBuilderTargetChoice<IFallout3ModGetter> 
        BeginWrite => new(
        this, 
        Fallout3WriteBuilderInstantiator.Instance);

    IBinaryModdedWriteBuilderTargetChoice IModGetter.BeginWrite => this.BeginWrite;

    public static BinaryWriteBuilderTargetChoice<IFallout3ModGetter> WriteBuilder() =>
        new(GameRelease.Fallout3, Fallout3WriteBuilderInstantiator.Instance);

    IMod IModGetter.DeepCopy() => this.DeepCopy();
}

public partial interface IFallout3ModGetter
{
    BinaryModdedWriteBuilderTargetChoice<IFallout3ModGetter> BeginWrite { get; }
}

partial class Fallout3ModSetterTranslationCommon
{
    partial void DeepCopyInCustom(IFallout3Mod item, IFallout3ModGetter rhs, ErrorMaskBuilder? errorMask,
        TranslationCrystal? copyMask, bool deepCopy)
    {
        if (!deepCopy) return;
        if (item is not AMod mod)
        {
            throw new NotImplementedException();
        }
        mod.SetModKey(rhs.ModKey);
    }

    public partial Fallout3Mod DeepCopyGetNew(IFallout3ModGetter item)
    {
        return new Fallout3Mod(item.ModKey, item.Fallout3Release);
    }
}

internal partial class Fallout3ModBinaryOverlay
{
    public uint GetDefaultInitialNextFormID(bool? forceUseLowerFormIDRanges = false) =>
        Fallout3Mod.GetDefaultInitialNextFormIDStatic(
            this.ModHeader.Stats.Version,
            forceUseLowerFormIDRanges);
    
    public bool IsMaster => this.ModHeader.Flags.HasFlag(Fallout3ModHeader.HeaderFlag.Master);
    public bool CanBeSmallMaster => false;
    public bool IsSmallMaster => false;
    public bool CanBeMediumMaster => false;
    public bool IsMediumMaster => false;
    public bool ListsOverriddenForms => false;
    public MasterStyle MasterStyle => this.GetMasterStyle();
    IMod IModGetter.DeepCopy() => this.DeepCopy();
    
    public IReadOnlyList<IFormLinkGetter<IMajorRecordGetter>>? OverriddenForms => null;

    IBinaryModdedWriteBuilderTargetChoice IModGetter.BeginWrite => 
        new BinaryModdedWriteBuilderTargetChoice<IFallout3ModGetter>(
            this, 
            Fallout3Mod.Fallout3WriteBuilderInstantiator.Instance);

    public BinaryModdedWriteBuilderTargetChoice<IFallout3ModGetter> BeginWrite => 
        new BinaryModdedWriteBuilderTargetChoice<IFallout3ModGetter>(
            this, 
            Fallout3Mod.Fallout3WriteBuilderInstantiator.Instance);
}

partial class Fallout3ModCommon
{
    public static void WriteCellsParallel(
        IFallout3ListGroupGetter<ICellBlockGetter> group,
        int targetIndex,
        Stream[] streamDepositArray,
        WritingBundle bundle,
        ParallelWriteParameters parallelWriteParameters)
    {
        if (group.Records.Count == 0) return;
        Stream[] streams = new Stream[group.Records.Count + 1];
        byte[] groupBytes = new byte[GameConstants.Fallout3.GroupConstants.HeaderLength];
        BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
        var groupByteStream = new MemoryStream(groupBytes);
        using (var stream = new MutagenWriter(groupByteStream, bundle with {}, dispose: false))
        {
            stream.Position += 8;
            Fallout3ListGroupBinaryWriteTranslation.WriteEmbedded<ICellBlockGetter>(group, stream);
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
        byte[] groupBytes = new byte[GameConstants.Fallout3.GroupConstants.HeaderLength];
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
        byte[] groupBytes = new byte[GameConstants.Fallout3.GroupConstants.HeaderLength];
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
    
    partial void GetCustomRecordCount(IFallout3ModGetter item, Action<uint> setter)
    {
        // ToDo
    }
}