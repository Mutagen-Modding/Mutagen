using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using System.Reactive.Linq;
using Noggog;
using Loqui.Internal;
using Mutagen.Bethesda.Internals;
using System.IO;
using System.Xml.Linq;
using System.Buffers.Binary;
using System.Diagnostics;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class OblivionMod
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IList<MasterReference> IMod.MasterReferences => this.ModHeader.MasterReferences;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IMasterReferenceGetter> IModGetter.MasterReferences => this.ModHeader.MasterReferences;

        public ModKey ModKey { get; } = ModKey.Null;

        public OblivionMod(ModKey modKey)
            : this()
        {
            this.ModKey = modKey;
        }

        partial void CustomCtor()
        {
            this.ModHeader.Stats.NextObjectID = 0xD62; // first available ID on empty CS plugins
        }

        public FormKey GetNextFormKey()
        {
            return new FormKey(
                this.ModKey,
                this.ModHeader.Stats.NextObjectID++);
        }

        partial void GetCustomRecordCount(Action<int> setter)
        {
            int count = 0;
            // Tally Cell Group counts
            int cellSubGroupCount(Cell cell)
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
            count += this.Cells.Records.Count; // Block Count
            count += this.Cells.Records.Sum(block => block.SubBlocks?.Count ?? 0); // Sub Block Count
            count += this.Cells.Records
                .SelectMany(block => block.SubBlocks)
                .SelectMany(subBlock => subBlock.Cells)
                .Select(cellSubGroupCount)
                .Sum();

            // Tally Worldspace Group Counts
            count += this.Worldspaces.Sum(wrld => wrld.SubCells?.Count ?? 0); // Cell Blocks
            count += this.Worldspaces
                .SelectMany(wrld => wrld.SubCells)
                .Sum(block => block.Items?.Count ?? 0); // Cell Sub Blocks
            count += this.Worldspaces
                .SelectMany(wrld => wrld.SubCells)
                .SelectMany(block => block.Items)
                .SelectMany(subBlock => subBlock.Items)
                .Sum(cellSubGroupCount); // Cell sub groups

            // Tally Dialog Group Counts
            count += this.DialogTopics.RecordCache.Count;
            setter(count);
        }
    }

    namespace Internals
    {
        public partial class OblivionModCommon
        {
            public static void WriteCellsParallel(
                IListGroupGetter<ICellBlockGetter> group,
                MasterReferences masters,
                int targetIndex,
                Stream[] streamDepositArray)
            {
                if (group.Records.Count == 0) return;
                Stream[] streams = new Stream[group.Records.Count + 1];
                byte[] groupBytes = new byte[GameConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                var groupByteStream = new MemoryStream(groupBytes);
                using (var stream = new MutagenWriter(groupByteStream, GameConstants.Oblivion, dispose: false))
                {
                    stream.Position += 8;
                    ListGroupBinaryWriteTranslation.Write_Embedded<ICellBlockGetter>(group, stream, default!);
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
                MasterReferences masters,
                int targetIndex,
                Stream[] streamDepositArray)
            {
                var subBlocks = block.SubBlocks;
                Stream[] streams = new Stream[(subBlocks?.Count ?? 0) + 1];
                byte[] groupBytes = new byte[GameConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                var groupByteStream = new MemoryStream(groupBytes);
                using (var stream = new MutagenWriter(groupByteStream, GameConstants.Oblivion, dispose: false))
                {
                    stream.Position += 8;
                    CellBlockBinaryWriteTranslation.Write_Embedded(block, stream, default!);
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
                MasterReferences masters,
                int targetIndex,
                Stream[] streamDepositArray)
            {
                var cells = subBlock.Cells;
                Stream[] streams = new Stream[(cells?.Count ?? 0) + 1];
                byte[] groupBytes = new byte[GameConstants.Oblivion.GroupConstants.HeaderLength];
                var groupByteStream = new MemoryStream(groupBytes);
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                using (var stream = new MutagenWriter(groupByteStream, GameConstants.Oblivion, dispose: false))
                {
                    stream.Position += 8;
                    CellSubBlockBinaryWriteTranslation.Write_Embedded(subBlock, stream, default!);
                }
                streams[0] = groupByteStream;
                if (cells != null)
                {
                    Parallel.ForEach(cells, (cell, state, counter) =>
                    {
                        MemoryTributary trib = new MemoryTributary();
                        cell.WriteToBinary(new MutagenWriter(trib, GameConstants.Oblivion, dispose: false), masters);
                        streams[(int)counter + 1] = trib;
                    });
                }
                UtilityTranslation.CompileSetGroupLength(streams, groupBytes);
                streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
            }

            public static void WriteWorldspacesParallel(
                IGroupGetter<IWorldspaceGetter> group,
                MasterReferences masters,
                int targetIndex,
                Stream[] streamDepositArray)
            {
                var cache = group.RecordCache;
                if (cache == null || cache.Count == 0) return;
                Stream[] streams = new Stream[cache.Count + 1];
                byte[] groupBytes = new byte[GameConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                var groupByteStream = new MemoryStream(groupBytes);
                using (var stream = new MutagenWriter(groupByteStream, GameConstants.Oblivion, dispose: false))
                {
                    stream.Position += 8;
                    GroupBinaryWriteTranslation.Write_Embedded<IWorldspaceGetter>(group, stream, default!);
                }
                streams[0] = groupByteStream;
                Parallel.ForEach(group.Records, (worldspace, worldspaceState, worldspaceCounter) =>
                {
                    var worldTrib = new MemoryTributary();
                    using (var writer = new MutagenWriter(worldTrib, GameConstants.Oblivion, dispose: false))
                    {
                        using (HeaderExport.ExportHeader(
                            writer: writer,
                            record: Worldspace_Registration.WRLD_HEADER,
                            type: ObjectType.Record))
                        {
                            WorldspaceBinaryWriteTranslation.Write_Embedded(
                                item: worldspace,
                                writer: writer,
                                masterReferences: masters);
                            WorldspaceBinaryWriteTranslation.Write_RecordTypes(
                                item: worldspace,
                                writer: writer,
                                recordTypeConverter: null,
                                masterReferences: masters);
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
                    var worldGroupWriter = new MutagenWriter(worldGroupTrib, GameConstants.Oblivion, dispose: false);
                    worldGroupWriter.Write(Group_Registration.GRUP_HEADER.TypeInt);
                    worldGroupWriter.Write(UtilityTranslation.Zeros.Slice(0, GameConstants.Oblivion.GroupConstants.LengthLength));
                    FormKeyBinaryTranslation.Instance.Write(
                        worldGroupWriter,
                        worldspace.FormKey,
                        masters);
                    worldGroupWriter.Write((int)GroupTypeEnum.WorldChildren);
                    worldGroupWriter.Write(worldspace.SubCellsTimestamp);
                    if (road != null)
                    {
                        road.WriteToBinary(
                            worldGroupWriter,
                            masterReferences: masters);
                    }
                    if (topCell != null)
                    {
                        topCell.WriteToBinary(
                            worldGroupWriter,
                            masterReferences: masters);
                    }
                    subStreams[0] = worldGroupTrib;

                    if (subCells != null)
                    {
                        Parallel.ForEach(subCells, (block, blockState, blockCounter) =>
                        {
                            WriteBlocksParallel(
                                block,
                                masters,
                                (int)blockCounter + 1,
                                subStreams);
                        });
                    }

                    worldGroupWriter.Position = 4;
                    worldGroupWriter.Write((uint)(subStreams.NotNull().Select(s => s.Length).Sum()));
                    streams[worldspaceCounter + 1] = new CompositeReadStream(worldTrib.And(subStreams), resetPositions: true);
                });
                UtilityTranslation.CompileSetGroupLength(streams, groupBytes);
                streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
            }

            public static void WriteBlocksParallel(
                IWorldspaceBlockGetter block,
                MasterReferences masters,
                int targetIndex,
                Stream[] streamDepositArray)
            {
                var items = block.Items;
                Stream[] streams = new Stream[(items?.Count ?? 0)+ 1];
                byte[] groupBytes = new byte[GameConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                var groupByteStream = new MemoryStream(groupBytes);
                using (var stream = new MutagenWriter(groupByteStream, GameConstants.Oblivion, dispose: false))
                {
                    stream.Position += 8;
                    WorldspaceBlockBinaryWriteTranslation.Write_Embedded(block, stream, default!);
                }
                streams[0] = groupByteStream;
                if (items != null)
                {
                    Parallel.ForEach(items, (subBlock, state, counter) =>
                    {
                        WriteSubBlocksParallel(
                            subBlock,
                            masters,
                            (int)counter + 1,
                            streams);
                    });
                }
                UtilityTranslation.CompileSetGroupLength(streams, groupBytes);
                streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
            }

            public static void WriteSubBlocksParallel(
                IWorldspaceSubBlockGetter subBlock,
                MasterReferences masters,
                int targetIndex,
                Stream[] streamDepositArray)
            {
                var items = subBlock.Items;
                Stream[] streams = new Stream[(items?.Count ?? 0) + 1];
                byte[] groupBytes = new byte[GameConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                var groupByteStream = new MemoryStream(groupBytes);
                using (var stream = new MutagenWriter(groupByteStream, GameConstants.Oblivion, dispose: false))
                {
                    stream.Position += 8;
                    WorldspaceSubBlockBinaryWriteTranslation.Write_Embedded(subBlock, stream, default!);
                }
                streams[0] = groupByteStream;
                if (items != null)
                {
                    Parallel.ForEach(items, (cell, state, counter) =>
                    {
                        MemoryTributary trib = new MemoryTributary();
                        cell.WriteToBinary(new MutagenWriter(trib, GameConstants.Oblivion, dispose: false), masters);
                        streams[(int)counter + 1] = trib;
                    });
                }
                UtilityTranslation.CompileSetGroupLength(streams, groupBytes);
                streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
            }

            public static void WriteDialogTopicsParallel(
                IGroupGetter<IDialogTopicGetter> group,
                MasterReferences masters,
                int targetIndex,
                Stream[] streamDepositArray)
            {
                WriteGroupParallel(group, masters, targetIndex, streamDepositArray);
            }
        }
    }
}
