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

        public static readonly Script.TranslationMask ScriptXmlFolderTranslationMask = new Script.TranslationMask(true)
        {
            Fields = new Loqui.MaskItem<bool, ScriptFields.TranslationMask?>(
                true,
                new ScriptFields.TranslationMask(true)
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
