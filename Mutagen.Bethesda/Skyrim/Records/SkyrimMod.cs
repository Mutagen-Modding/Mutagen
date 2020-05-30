using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System.IO;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class SkyrimMod : AMod
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IList<MasterReference> IMod.MasterReferences => this.ModHeader.MasterReferences;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IMasterReferenceGetter> IModGetter.MasterReferences => this.ModHeader.MasterReferences;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        uint IMod.NextObjectID
        {
            get => this.ModHeader.Stats.NextObjectID;
            set => this.ModHeader.Stats.NextObjectID = value;
        }
    }

    namespace Internals
    {
        public partial class SkyrimModBinaryWriteTranslation
        {
            public static void WriteModHeader(
                ISkyrimModGetter mod,
                MutagenWriter writer,
                ModKey modKey)
            {
                var modHeader = mod.ModHeader.DeepCopy() as ModHeader;
                modHeader.Flags = modHeader.Flags.SetFlag(ModHeader.HeaderFlag.Master, modKey.Master);
                modHeader.MasterReferences.SetTo(writer.MetaData.MasterReferences!.Masters.Select(m => m.DeepCopy()));
                modHeader.WriteToBinary(writer);
            }
        }

        public partial class SkyrimModCommon
        {
            public static void WriteCellsParallel(
                IListGroupGetter<ICellBlockGetter> group,
                MasterReferenceReader masters,
                int targetIndex,
                Stream[] streamDepositArray)
            {
                if (group.Records.Count == 0) return;
                Stream[] streams = new Stream[group.Records.Count + 1];
                byte[] groupBytes = new byte[GameConstants.Skyrim.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                var groupByteStream = new MemoryStream(groupBytes);
                using (var stream = new MutagenWriter(groupByteStream, GameConstants.Skyrim, dispose: false))
                {
                    stream.Position += 8;
                    ListGroupBinaryWriteTranslation.WriteEmbedded<ICellBlockGetter>(group, stream);
                }
                streams[0] = groupByteStream;
                Parallel.ForEach(group.Records, (cellBlock, state, counter) =>
                {
                    WriteBlocksParallel(
                        cellBlock,
                        masters,
                        (int)counter + 1,
                        streams);
                });
                UtilityTranslation.CompileSetGroupLength(streams, groupBytes);
                streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
            }

            public static void WriteBlocksParallel(
                ICellBlockGetter block,
                MasterReferenceReader masters,
                int targetIndex,
                Stream[] streamDepositArray)
            {
                var subBlocks = block.SubBlocks;
                Stream[] streams = new Stream[(subBlocks?.Count ?? 0) + 1];
                byte[] groupBytes = new byte[GameConstants.Skyrim.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                var groupByteStream = new MemoryStream(groupBytes);
                using (var stream = new MutagenWriter(groupByteStream, GameConstants.Skyrim, dispose: false))
                {
                    stream.Position += 8;
                    CellBlockBinaryWriteTranslation.WriteEmbedded(block, stream);
                }
                streams[0] = groupByteStream;
                if (subBlocks != null)
                {
                    Parallel.ForEach(subBlocks, (cellSubBlock, state, counter) =>
                    {
                        WriteSubBlocksParallel(
                            cellSubBlock,
                            masters,
                            (int)counter + 1,
                            streams);
                    });
                }
                UtilityTranslation.CompileSetGroupLength(streams, groupBytes);
                streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
            }

            public static void WriteSubBlocksParallel(
                ICellSubBlockGetter subBlock,
                MasterReferenceReader masters,
                int targetIndex,
                Stream[] streamDepositArray)
            {
                var cells = subBlock.Cells;
                Stream[] streams = new Stream[(cells?.Count ?? 0) + 1];
                byte[] groupBytes = new byte[GameConstants.Skyrim.GroupConstants.HeaderLength];
                var groupByteStream = new MemoryStream(groupBytes);
                var bundle = new WritingBundle(GameConstants.Skyrim)
                {
                    MasterReferences = masters
                };
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                using (var stream = new MutagenWriter(groupByteStream, bundle, dispose: false))
                {
                    stream.Position += 8;
                    CellSubBlockBinaryWriteTranslation.WriteEmbedded(subBlock, stream);
                }
                streams[0] = groupByteStream;
                if (cells != null)
                {
                    Parallel.ForEach(cells, (cell, state, counter) =>
                    {
                        MemoryTributary trib = new MemoryTributary();
                        cell.WriteToBinary(new MutagenWriter(trib, bundle, dispose: false));
                        streams[(int)counter + 1] = trib;
                    });
                }
                UtilityTranslation.CompileSetGroupLength(streams, groupBytes);
                streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
            }
        }
    }
}
