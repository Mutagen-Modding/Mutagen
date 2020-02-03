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

        public ModKey ModKey { get; }

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
                if (cell.Temporary.Count > 0
                    || cell.PathGrid != null
                    || cell.Landscape != null)
                {
                    cellGroupCount++;
                }
                if (cell.Persistent.Count > 0)
                {
                    cellGroupCount++;
                }
                if (cell.VisibleWhenDistant.Count > 0)
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
            count += this.Cells.Records.Sum(block => block.SubBlocks.Count); // Sub Block Count
            count += this.Cells.Records
                .SelectMany(block => block.SubBlocks)
                .SelectMany(subBlock => subBlock.Cells)
                .Select(cellSubGroupCount)
                .Sum();

            // Tally Worldspace Group Counts
            count += this.Worldspaces.Sum(wrld => wrld.SubCells.Count); // Cell Blocks
            count += this.Worldspaces
                .SelectMany(wrld => wrld.SubCells)
                .Sum(block => block.Items.Count); // Cell Sub Blocks
            count += this.Worldspaces
                .SelectMany(wrld => wrld.SubCells)
                .SelectMany(block => block.Items)
                .SelectMany(subBlock => subBlock.Items)
                .Sum(cellSubGroupCount); // Cell sub groups

            // Tally Dialog Group Counts
            count += this.DialogTopics.RecordCache.Count;
            setter(count);
        }

        async Task CreateFromXmlFolderWorldspaces(DirectoryPath dir, string name, int index, ErrorMaskBuilder? errorMask)
        {
            dir = new DirectoryPath(Path.Combine(dir.Path, nameof(this.Worldspaces)));
            using (errorMask?.PushIndex(index))
            {
                using (errorMask?.PushIndex((int)Group_FieldIndex.RecordCache))
                {
                    try
                    {
                        var path = Path.Combine(dir.Path, $"Group.xml");
                        if (File.Exists(path))
                        {
                            GroupXmlCreateTranslation<Worldspace>.FillPublicXml(
                                this.Worldspaces,
                                XElement.Load(path),
                                errorMask,
                                translationMask: GroupExt.XmlFolderTranslationCrystal);
                        }

                        List<Task<Worldspace>> tasks = new List<Task<Worldspace>>();
                        foreach (var subDir in dir.EnumerateDirectories(includeSelf: false, recursive: false)
                            .SelectWhere(d =>
                            {
                                if (Mutagen.Bethesda.FolderTranslation.TryGetItemIndex(d.Name, out var i))
                                {
                                    return TryGet<(int Index, DirectoryPath Dir)>.Succeed((i, d));
                                }
                                return TryGet<(int Index, DirectoryPath Dir)>.Failure;
                            })
                            .OrderBy(d => d.Index))
                        {
                            tasks.Add(Task.Run(async () =>
                            {
                                var get = await Worldspace.TryCreateXmFolder(subDir.Dir, errorMask).ConfigureAwait(false);
                                return get.Value;
                            }));
                        }
                        var worldspaces = await Task.WhenAll(tasks).ConfigureAwait(false);
                        this.Worldspaces.RecordCache.Set(worldspaces.Where(ws => ws != null));
                    }
                    catch (Exception ex)
                    when (errorMask != null)
                    {
                        errorMask.ReportException(ex);
                    }
                }
            }
        }

        async Task WriteToXmlFolderWorldspaces(DirectoryPath dir, string name, int index, ErrorMaskBuilder? errorMask)
        {
            if (this.Worldspaces.RecordCache.Count == 0) return;
            dir = new DirectoryPath(Path.Combine(dir.Path, nameof(this.Worldspaces)));
            using (errorMask?.PushIndex(index))
            {
                using (errorMask?.PushIndex((int)Group_FieldIndex.RecordCache))
                {
                    dir.Create();
                    this.Worldspaces.WriteToXml(
                        Path.Combine(dir.Path, $"Group.xml"),
                        errorMask,
                        translationMask: GroupExt.XmlFolderTranslationCrystal);
                    int counter = 0;
                    List<Task> tasks = new List<Task>();
                    foreach (var item in this.Worldspaces.Records)
                    {
                        tasks.Add(
                            item.WriteToXmlFolder(
                                node: null!,
                                name: name,
                                counter: counter++,
                                dir: dir,
                                errorMask: errorMask));
                    }
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }
            }
        }

        public static readonly Script_TranslationMask ScriptXmlFolderTranslationMask = new Script_TranslationMask(true)
        {
            Fields = new Loqui.MaskItem<bool, ScriptFields_TranslationMask?>(
                true,
                new ScriptFields_TranslationMask(true)
                {
                    SourceCode = false
                })
        };
        public static readonly TranslationCrystal ScriptXmlFolderTranslationCrystal = ScriptXmlFolderTranslationMask.GetCrystal();

        async Task CreateFromXmlFolderScripts(DirectoryPath dir, string name, int index, ErrorMaskBuilder? errorMask)
        {
            dir = new DirectoryPath(Path.Combine(dir.Path, nameof(this.Scripts)));

            var path = Path.Combine(dir.Path, $"Group.xml");
            if (File.Exists(path))
            {
                GroupXmlCreateTranslation<Script>.FillPublicXml(
                    this.Scripts,
                    XElement.Load(path),
                    errorMask,
                    translationMask: GroupExt.XmlFolderTranslationCrystal);
            }

            List<Task<Script>> tasks = new List<Task<Script>>();
            foreach (var subDir in dir.EnumerateDirectories(includeSelf: false, recursive: false)
                .SelectWhere(d =>
                {
                    if (Mutagen.Bethesda.FolderTranslation.TryGetItemIndex(d.Name, out var i))
                    {
                        return TryGet<(int Index, DirectoryPath Dir)>.Succeed((i, d));
                    }
                    return TryGet<(int Index, DirectoryPath Dir)>.Failure;
                })
                .OrderBy(d => d.Index))
            {
                var scriptXml = new FilePath(Path.Combine(subDir.Dir.Path, "Script.xml"));
                if (!scriptXml.Exists) continue;
                tasks.Add(Task.Run(() =>
                {
                    var script = Script.CreateFromXml(
                        scriptXml.Path,
                        errorMask: errorMask,
                        translationMask: ScriptXmlFolderTranslationMask);
                    var scriptText = new FilePath(Path.Combine(subDir.Dir.Path, "SourceCode.txt"));
                    if (scriptText.Exists)
                    {
                        script.Fields.SourceCode = File.ReadAllText(scriptText.Path);
                    }
                    return script;
                }));
                var scripts = await Task.WhenAll(tasks).ConfigureAwait(false);
                this.Scripts.RecordCache.Set(scripts);
            }
        }

        async Task WriteToXmlFolderScripts(DirectoryPath dir, string name, int index, ErrorMaskBuilder? errorMask)
        {
            if (this.Scripts.RecordCache.Count == 0) return;
            dir = new DirectoryPath(Path.Combine(dir.Path, nameof(this.Scripts)));
            using (errorMask?.PushIndex(index))
            {
                using (errorMask?.PushIndex((int)Group_FieldIndex.RecordCache))
                {
                    dir.Create();
                    this.Scripts.WriteToXml(
                        Path.Combine(dir.Path, "Group.xml"),
                        errorMask,
                        translationMask: GroupExt.XmlFolderTranslationCrystal);
                    int counter = 0;
                    List<Task> tasks = new List<Task>();
                    foreach (var item in this.Scripts.Records)
                    {
                        DirectoryPath subDir = new DirectoryPath(Path.Combine(dir.Path, FolderTranslation.GetFileString(item, counter++)));

                        subDir.Create();
                        tasks.Add(Task.Run(() =>
                        {
                            item.WriteToXml(
                                Path.Combine(subDir.Path, $"Script.xml"),
                                errorMask: errorMask,
                                translationMask: ScriptXmlFolderTranslationCrystal);
                            var sourceCodePath = Path.Combine(subDir.Path, "SourceCode.txt");
                            using (var textOut = new System.IO.StreamWriter(File.Create(sourceCodePath)))
                            {
                                textOut.Write(item.Fields.SourceCode);
                            }
                            File.SetLastAccessTime(sourceCodePath, DateTime.Now);
                        }));
                    }
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }
            }
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
                byte[] groupBytes = new byte[MetaDataConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                var groupByteStream = new MemoryStream(groupBytes);
                using (var stream = new MutagenWriter(groupByteStream, MetaDataConstants.Oblivion, dispose: false))
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
                if (block.SubBlocks.Count == 0) return;
                Stream[] streams = new Stream[block.SubBlocks.Count + 1];
                byte[] groupBytes = new byte[MetaDataConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                var groupByteStream = new MemoryStream(groupBytes);
                using (var stream = new MutagenWriter(groupByteStream, MetaDataConstants.Oblivion, dispose: false))
                {
                    stream.Position += 8;
                    CellBlockBinaryWriteTranslation.Write_Embedded(block, stream, default!);
                }
                streams[0] = groupByteStream;
                Parallel.ForEach(block.SubBlocks, (cellSubBlock, state, counter) =>
                {
                    WriteSubBlocksParallel(
                        cellSubBlock,
                        masters,
                        (int)counter + 1,
                        streams);
                });
                UtilityTranslation.CompileSetGroupLength(streams, groupBytes);
                streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
            }

            public static void WriteSubBlocksParallel(
                ICellSubBlockGetter subBlock,
                MasterReferences masters,
                int targetIndex,
                Stream[] streamDepositArray)
            {
                if (subBlock.Cells.Count == 0) return;
                Stream[] streams = new Stream[subBlock.Cells.Count + 1];
                byte[] groupBytes = new byte[MetaDataConstants.Oblivion.GroupConstants.HeaderLength];
                var groupByteStream = new MemoryStream(groupBytes);
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                using (var stream = new MutagenWriter(groupByteStream, MetaDataConstants.Oblivion, dispose: false))
                {
                    stream.Position += 8;
                    CellSubBlockBinaryWriteTranslation.Write_Embedded(subBlock, stream, default!);
                }
                streams[0] = groupByteStream;
                Parallel.ForEach(subBlock.Cells, (cell, state, counter) =>
                {
                    MemoryTributary trib = new MemoryTributary();
                    cell.WriteToBinary(new MutagenWriter(trib, MetaDataConstants.Oblivion, dispose: false), masters);
                    streams[(int)counter + 1] = trib;
                });
                UtilityTranslation.CompileSetGroupLength(streams, groupBytes);
                streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
            }

            public static void WriteWorldspacesParallel(
                IGroupGetter<IWorldspaceGetter> group,
                MasterReferences masters,
                int targetIndex,
                Stream[] streamDepositArray)
            {
                if (group.RecordCache.Count == 0) return;
                Stream[] streams = new Stream[group.RecordCache.Count + 1];
                byte[] groupBytes = new byte[MetaDataConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                var groupByteStream = new MemoryStream(groupBytes);
                using (var stream = new MutagenWriter(groupByteStream, MetaDataConstants.Oblivion, dispose: false))
                {
                    stream.Position += 8;
                    GroupBinaryWriteTranslation.Write_Embedded<IWorldspaceGetter>(group, stream, default!);
                }
                streams[0] = groupByteStream;
                Parallel.ForEach(group.Records, (worldspace, worldspaceState, worldspaceCounter) =>
                {
                    var worldTrib = new MemoryTributary();
                    using (var writer = new MutagenWriter(worldTrib, MetaDataConstants.Oblivion, dispose: false))
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
                    if (worldspace.SubCells.Count == 0
                        && road == null
                        && topCell == null)
                    {
                        streams[worldspaceCounter + 1] = worldTrib;
                        return;
                    }

                    Stream[] subStreams = new Stream[worldspace.SubCells.Count + 1];

                    var worldGroupTrib = new MemoryTributary();
                    var worldGroupWriter = new MutagenWriter(worldGroupTrib, MetaDataConstants.Oblivion, dispose: false);
                    worldGroupWriter.Write(Group_Registration.GRUP_HEADER.TypeInt);
                    worldGroupWriter.Write(UtilityTranslation.Zeros.Slice(0, MetaDataConstants.Oblivion.GroupConstants.LengthLength));
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

                    Parallel.ForEach(worldspace.SubCells, (block, blockState, blockCounter) =>
                    {
                        WriteBlocksAsync(
                            block,
                            masters,
                            (int)blockCounter + 1,
                            subStreams);
                    });

                    worldGroupWriter.Position = 4;
                    worldGroupWriter.Write((uint)(subStreams.NotNull().Select(s => s.Length).Sum()));
                    streams[worldspaceCounter + 1] = new CompositeReadStream(worldTrib.And(subStreams), resetPositions: true);
                });
                UtilityTranslation.CompileSetGroupLength(streams, groupBytes);
                streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
            }

            public static void WriteBlocksAsync(
                IWorldspaceBlockGetter block,
                MasterReferences masters,
                int targetIndex,
                Stream[] streamDepositArray)
            {
                Stream[] streams = new Stream[block.Items.Count + 1];
                byte[] groupBytes = new byte[MetaDataConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                var groupByteStream = new MemoryStream(groupBytes);
                using (var stream = new MutagenWriter(groupByteStream, MetaDataConstants.Oblivion, dispose: false))
                {
                    stream.Position += 8;
                    WorldspaceBlockBinaryWriteTranslation.Write_Embedded(block, stream, default!);
                }
                streams[0] = groupByteStream;
                Parallel.ForEach(block.Items, (subBlock, state, counter) =>
                {
                    WriteSubBlocksParallel(
                        subBlock,
                        masters,
                        (int)counter + 1,
                        streams);
                });
                UtilityTranslation.CompileSetGroupLength(streams, groupBytes);
                streamDepositArray[targetIndex] = new CompositeReadStream(streams, resetPositions: true);
            }

            public static void WriteSubBlocksParallel(
                IWorldspaceSubBlockGetter subBlock,
                MasterReferences masters,
                int targetIndex,
                Stream[] streamDepositArray)
            {
                Stream[] streams = new Stream[subBlock.Items.Count + 1];
                byte[] groupBytes = new byte[MetaDataConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                var groupByteStream = new MemoryStream(groupBytes);
                using (var stream = new MutagenWriter(groupByteStream, MetaDataConstants.Oblivion, dispose: false))
                {
                    stream.Position += 8;
                    WorldspaceSubBlockBinaryWriteTranslation.Write_Embedded(subBlock, stream, default!);
                }
                streams[0] = groupByteStream;
                Parallel.ForEach(subBlock.Items, (cell, state, counter) =>
                {
                    MemoryTributary trib = new MemoryTributary();
                    cell.WriteToBinary(new MutagenWriter(trib, MetaDataConstants.Oblivion, dispose: false), masters);
                    streams[(int)counter + 1] = trib;
                });
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

            public static async Task<IEnumerable<Stream>> WriteCellsAsync(
                IListGroupGetter<ICellBlockGetter> group,
                MasterReferences masters)
            {
                if (group.Records.Count == 0)
                {
                    return EnumerableExt<Stream>.Empty;
                }
                List<Task<Stream>> streams = new List<Task<Stream>>();
                byte[] groupBytes = new byte[MetaDataConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                using (var stream = new MutagenWriter(new MemoryStream(groupBytes), MetaDataConstants.Oblivion))
                {
                    stream.Position += 8;
                    ListGroupBinaryWriteTranslation.Write_Embedded<ICellBlockGetter>(group, stream, default!);
                }
                streams.Add(Task.FromResult<Stream>(new MemoryStream(groupBytes)));
                foreach (var cellBlock in group.Records)
                {
                    streams.Add(Task.Run<Stream>(async () =>
                    {
                        MemoryTributary trib = new MemoryTributary();
                        await UtilityTranslation.CompileStreamsInto(
                            WriteBlocksAsync(
                                cellBlock,
                                masters),
                            trib).ConfigureAwait(false);
                        return trib;
                    }));
                }
                return await UtilityTranslation.CompileSetGroupLength(streams, groupBytes).ConfigureAwait(false);
            }

            public static async Task<IEnumerable<Stream>> WriteBlocksAsync(
                ICellBlockGetter block,
                MasterReferences masters)
            {
                if (block.SubBlocks.Count == 0)
                {
                    return EnumerableExt<Stream>.Empty;
                }
                List<Task<Stream>> streams = new List<Task<Stream>>();
                byte[] groupBytes = new byte[MetaDataConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                using (var stream = new MutagenWriter(new MemoryStream(groupBytes), MetaDataConstants.Oblivion))
                {
                    stream.Position += 8;
                    CellBlockBinaryWriteTranslation.Write_Embedded(block, stream, default!);
                }
                streams.Add(Task.FromResult<Stream>(new MemoryStream(groupBytes)));
                foreach (var subBlock in block.SubBlocks)
                {
                    streams.Add(Task.Run<Stream>(async () =>
                    {
                        MemoryTributary trib = new MemoryTributary();
                        await UtilityTranslation.CompileStreamsInto(
                            WriteSubBlocksAsync(
                                subBlock,
                                masters),
                            trib).ConfigureAwait(false);
                        return trib;
                    }));
                }
                return await UtilityTranslation.CompileSetGroupLength(streams, groupBytes).ConfigureAwait(false);
            }

            public static async Task<IEnumerable<Stream>> WriteSubBlocksAsync(
                ICellSubBlockGetter subBlock,
                MasterReferences masters)
            {
                if (subBlock.Cells.Count == 0)
                {
                    return EnumerableExt<Stream>.Empty;
                }
                List<Task<Stream>> streams = new List<Task<Stream>>();
                byte[] groupBytes = new byte[MetaDataConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                using (var stream = new MutagenWriter(new MemoryStream(groupBytes), MetaDataConstants.Oblivion))
                {
                    stream.Position += 8;
                    CellSubBlockBinaryWriteTranslation.Write_Embedded(subBlock, stream, default!);
                }
                streams.Add(Task.FromResult<Stream>(new MemoryStream(groupBytes)));
                foreach (var cell in subBlock.Cells)
                {
                    streams.Add(Task.Run<Stream>(() =>
                    {
                        MemoryTributary trib = new MemoryTributary();
                        cell.WriteToBinary(new MutagenWriter(trib, MetaDataConstants.Oblivion), masters);
                        return trib;
                    }));
                }
                return await UtilityTranslation.CompileSetGroupLength(streams, groupBytes).ConfigureAwait(false);
            }

            public static async Task<IEnumerable<Stream>> WriteWorldspacesAsync(
                IGroupGetter<IWorldspaceGetter> group,
                MasterReferences masters)
            {
                if (group.RecordCache.Count == 0)
                {
                    return EnumerableExt<Stream>.Empty;
                }
                List<Task<Stream>> streams = new List<Task<Stream>>();
                byte[] groupBytes = new byte[MetaDataConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                using (var stream = new MutagenWriter(new MemoryStream(groupBytes), MetaDataConstants.Oblivion))
                {
                    stream.Position += 8;
                    GroupBinaryWriteTranslation.Write_Embedded<IWorldspaceGetter>(group, stream, default!);
                }
                streams.Add(Task.FromResult<Stream>(new MemoryStream(groupBytes)));
                foreach (var worldspace in group.Records)
                {
                    var worldTrib = new MemoryTributary();
                    using (var writer = new MutagenWriter(worldTrib, MetaDataConstants.Oblivion))
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
                    streams.Add(Task.FromResult<Stream>(worldTrib));
                    var road = worldspace.Road;
                    var topCell = worldspace.TopCell;
                    if (worldspace.SubCells.Count == 0
                        && road == null
                        && topCell == null) continue;
                    streams.Add(Task.Run<Stream>(async () =>
                    {
                        var worldGroupTrib = new MemoryTributary();
                        var worldGroupWriter = new MutagenWriter(worldGroupTrib, MetaDataConstants.Oblivion);
                        worldGroupWriter.Write(Group_Registration.GRUP_HEADER.TypeInt);
                        worldGroupWriter.Write(UtilityTranslation.Zeros.Slice(0, MetaDataConstants.Oblivion.GroupConstants.LengthLength));
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
                        List<Task<Stream>> subGroupStreams = new List<Task<Stream>>();
                        foreach (var block in worldspace.SubCells)
                        {
                            subGroupStreams.Add(Task.Run<Stream>(async () =>
                            {
                                MemoryTributary trib = new MemoryTributary();
                                await UtilityTranslation.CompileStreamsInto(
                                    WriteBlocksAsync(
                                        block,
                                        masters),
                                    trib).ConfigureAwait(false);
                                return trib;
                            }));
                        }
                        await UtilityTranslation.CompileStreamsInto(subGroupStreams, worldGroupTrib).ConfigureAwait(false);
                        worldGroupWriter.Position = 4;
                        worldGroupWriter.Write((uint)worldGroupTrib.Length);
                        return worldGroupTrib;
                    }));
                }
                return await UtilityTranslation.CompileSetGroupLength(streams, groupBytes).ConfigureAwait(false);
            }

            public static async Task<IEnumerable<Stream>> WriteBlocksAsync(
                IWorldspaceBlockGetter block,
                MasterReferences masters)
            {
                List<Task<Stream>> streams = new List<Task<Stream>>();
                byte[] groupBytes = new byte[MetaDataConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                using (var stream = new MutagenWriter(new MemoryStream(groupBytes), MetaDataConstants.Oblivion))
                {
                    stream.Position += 8;
                    WorldspaceBlockBinaryWriteTranslation.Write_Embedded(block, stream, default!);
                }
                streams.Add(Task.FromResult<Stream>(new MemoryStream(groupBytes)));
                foreach (var subBlock in block.Items)
                {
                    streams.Add(Task.Run<Stream>(async () =>
                    {
                        MemoryTributary trib = new MemoryTributary();
                        await UtilityTranslation.CompileStreamsInto(
                            WriteSubBlocksAsync(
                                subBlock,
                                masters),
                            trib).ConfigureAwait(false);
                        return trib;
                    }));
                }
                return await UtilityTranslation.CompileSetGroupLength(streams, groupBytes).ConfigureAwait(false);
            }

            public static async Task<IEnumerable<Stream>> WriteSubBlocksAsync(
                IWorldspaceSubBlockGetter subBlock,
                MasterReferences masters)
            {
                List<Task<Stream>> streams = new List<Task<Stream>>();
                byte[] groupBytes = new byte[MetaDataConstants.Oblivion.GroupConstants.HeaderLength];
                BinaryPrimitives.WriteInt32LittleEndian(groupBytes.AsSpan(), Group_Registration.GRUP_HEADER.TypeInt);
                using (var stream = new MutagenWriter(new MemoryStream(groupBytes), MetaDataConstants.Oblivion))
                {
                    stream.Position += 8;
                    WorldspaceSubBlockBinaryWriteTranslation.Write_Embedded(subBlock, stream, default!);
                }
                streams.Add(Task.FromResult<Stream>(new MemoryStream(groupBytes)));
                foreach (var cell in subBlock.Items)
                {
                    streams.Add(Task.Run<Stream>(() =>
                    {
                        MemoryTributary trib = new MemoryTributary();
                        cell.WriteToBinary(new MutagenWriter(trib, MetaDataConstants.Oblivion), masters);
                        return trib;
                    }));
                }
                return await UtilityTranslation.CompileSetGroupLength(streams, groupBytes).ConfigureAwait(false);
            }

            public static Task<IEnumerable<Stream>> WriteDialogTopicsAsync(
                IGroupGetter<IDialogTopicGetter> group,
                MasterReferences masters)
            {
                return WriteGroupAsync(group, masters);
            }
        }
    }
}
