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
    public partial class OblivionMod : AMod
    {
        public const uint DefaultInitialNextObjectID = 0xD62;
        private uint GetDefaultInitialNextObjectID() => DefaultInitialNextObjectID;

        partial void GetCustomRecordCount(Action<uint> setter)
        {
            uint count = 0;
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
            count += (uint)this.Cells.Records.Count; // Block Count
            count += (uint)this.Cells.Records.Sum(block => block.SubBlocks?.Count ?? 0); // Sub Block Count
            count += (uint)this.Cells.Records
                .SelectMany(block => block.SubBlocks)
                .SelectMany(subBlock => subBlock.Cells)
                .Select(cellSubGroupCount)
                .Sum();

            // Tally Worldspace Group Counts
            count += (uint)this.Worldspaces.Sum(wrld => wrld.SubCells?.Count ?? 0); // Cell Blocks
            count += (uint)this.Worldspaces
                .SelectMany(wrld => wrld.SubCells)
                .Sum(block => block.Items?.Count ?? 0); // Cell Sub Blocks
            count += (uint)this.Worldspaces
                .SelectMany(wrld => wrld.SubCells)
                .SelectMany(block => block.Items)
                .SelectMany(subBlock => subBlock.Items)
                .Sum(cellSubGroupCount); // Cell sub groups

            // Tally Dialog Group Counts
            count += (uint)this.DialogTopics.RecordCache.Count;
            setter(count);
        }
    }

    namespace Internals
    {
        public partial class OblivionModCommon
        {
            public static void WriteCellsParallel(
                IListGroupGetter<ICellBlockGetter> group,
                MasterReferenceReader masters,
                int targetIndex,
                Stream[] streamDepositArray)
            {
                if (group.Records.Count == 0) return;
                Stream[] streams = new Stream[group.Records.Count + 1];
                byte[] groupBytes = new byte[GameConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
                var groupByteStream = new MemoryStream(groupBytes);
                using (var stream = new MutagenWriter(groupByteStream, GameConstants.Oblivion, dispose: false))
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
                byte[] groupBytes = new byte[GameConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
                var groupByteStream = new MemoryStream(groupBytes);
                using (var stream = new MutagenWriter(groupByteStream, GameConstants.Oblivion, dispose: false))
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
                byte[] groupBytes = new byte[GameConstants.Oblivion.GroupConstants.HeaderLength];
                var groupByteStream = new MemoryStream(groupBytes);
                var bundle = new WritingBundle(GameConstants.Oblivion)
                {
                    MasterReferences = masters
                };
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
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

            public static void WriteWorldspacesParallel(
                IGroupGetter<IWorldspaceGetter> group,
                MasterReferenceReader masters,
                int targetIndex,
                Stream[] streamDepositArray)
            {
                var cache = group.RecordCache;
                if (cache == null || cache.Count == 0) return;
                Stream[] streams = new Stream[cache.Count + 1];
                var bundle = new WritingBundle(GameConstants.Oblivion)
                {
                    MasterReferences = masters
                };
                byte[] groupBytes = new byte[GameConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
                var groupByteStream = new MemoryStream(groupBytes);
                using (var stream = new MutagenWriter(groupByteStream, GameConstants.Oblivion, dispose: false))
                {
                    stream.Position += 8;
                    GroupBinaryWriteTranslation.WriteEmbedded<IWorldspaceGetter>(group, stream);
                }
                streams[0] = groupByteStream;
                Parallel.ForEach(group.Records, (worldspace, worldspaceState, worldspaceCounter) =>
                {
                    var worldTrib = new MemoryTributary();
                    using (var writer = new MutagenWriter(worldTrib, bundle, dispose: false))
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
                                recordTypeConverter: null);
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
                    var worldGroupWriter = new MutagenWriter(worldGroupTrib, bundle, dispose: false);
                    worldGroupWriter.Write(RecordTypes.GRUP.TypeInt);
                    worldGroupWriter.Write(UtilityTranslation.Zeros.Slice(0, GameConstants.Oblivion.GroupConstants.LengthLength));
                    FormKeyBinaryTranslation.Instance.Write(
                        worldGroupWriter,
                        worldspace.FormKey);
                    worldGroupWriter.Write((int)GroupTypeEnum.WorldChildren);
                    worldGroupWriter.Write(worldspace.SubCellsTimestamp);
                    road?.WriteToBinary(worldGroupWriter);
                    topCell?.WriteToBinary(worldGroupWriter);
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
                MasterReferenceReader masters,
                int targetIndex,
                Stream[] streamDepositArray)
            {
                var items = block.Items;
                Stream[] streams = new Stream[(items?.Count ?? 0)+ 1];
                byte[] groupBytes = new byte[GameConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
                var groupByteStream = new MemoryStream(groupBytes);
                using (var stream = new MutagenWriter(groupByteStream, GameConstants.Oblivion, dispose: false))
                {
                    stream.Position += 8;
                    WorldspaceBlockBinaryWriteTranslation.WriteEmbedded(block, stream);
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
                MasterReferenceReader masters,
                int targetIndex,
                Stream[] streamDepositArray)
            {
                var items = subBlock.Items;
                var bundle = new WritingBundle(GameConstants.Oblivion)
                {
                    MasterReferences = masters
                };
                Stream[] streams = new Stream[(items?.Count ?? 0) + 1];
                byte[] groupBytes = new byte[GameConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), RecordTypes.GRUP.TypeInt);
                var groupByteStream = new MemoryStream(groupBytes);
                using (var stream = new MutagenWriter(groupByteStream, bundle, dispose: false))
                {
                    stream.Position += 8;
                    WorldspaceSubBlockBinaryWriteTranslation.WriteEmbedded(subBlock, stream);
                }
                streams[0] = groupByteStream;
                if (items != null)
                {
                    Parallel.ForEach(items, (cell, state, counter) =>
                    {
                        MemoryTributary trib = new MemoryTributary();
                        cell.WriteToBinary(new MutagenWriter(trib, bundle, dispose: false));
                        streams[(int)counter + 1] = trib;
                    });
                }
                UtilityTranslation.CompileSetGroupLength(streams, groupBytes);
                streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
            }

            public static void WriteDialogTopicsParallel(
                IGroupGetter<IDialogTopicGetter> group,
                MasterReferenceReader masters,
                int targetIndex,
                Stream[] streamDepositArray)
            {
                WriteGroupParallel(group, masters, targetIndex, streamDepositArray);
            }
        }
    }
}
