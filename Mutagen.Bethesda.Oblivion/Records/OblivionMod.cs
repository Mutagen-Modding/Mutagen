using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using ReactiveUI;
using System.Reactive.Linq;
using Noggog;
using Noggog.Notifying;
using CSharpExt.Rx;
using DynamicData;
using System.Collections.ObjectModel;
using System.Threading;
using Loqui.Internal;
using Mutagen.Bethesda.Internals;
using System.IO;
using System.Xml.Linq;
using Loqui.Xml;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class OblivionMod : IMod, ILinkContainer
    {
        public ISourceList<MasterReference> MasterReferences => this.ModHeader.MasterReferences;
        public ModKey ModKey { get; }

        public OblivionMod(ModKey modKey)
            : this()
        {
            this.ModKey = modKey ?? throw new ArgumentNullException();
        }

        partial void CustomCtor()
        {
            this.ModHeader.Stats.NextObjectID = 0xD62; // first available ID on empty CS plugins

            Observable.Merge<IChangeSet<IMajorRecord>>(
                    this.Cells.Items.Connect()
                        .TransformMany(cellBlock => cellBlock.Items)
                        .TransformMany(cellSubBlock => cellSubBlock.Items)
                        .MergeMany(cell => GetCellRecords(cell)),
                    this.Worldspaces.Items.Connect()
                        .RemoveKey()
                        .MergeMany(worldSpace => GetWorldspaceRecords(worldSpace)),
                    this.DialogTopics.Items.Connect()
                        .RemoveKey()
                        .MergeMany(dialog => GetDialogRecords(dialog)))
                .AddKey(c => c.FormKey)
                .PopulateInto(this._majorRecords);
        }

        void IMod.Write_Binary(string path, ModKey modKey)
        {
            this.Write_Binary(path, modKey, importMask: null);
        }

        private IObservable<IChangeSet<IMajorRecord>> GetCellRecords(Cell cell)
        {
            if (cell == null) return Observable.Empty<IChangeSet<IMajorRecord>>();
            return Observable.Merge<IChangeSet<IMajorRecord>>(
                Observable
                    .Return<IMajorRecord>(cell)
                    .ToObservableChangeSet(),
                cell.WhenAny(x => x.PathGrid)
                    .Select<PathGrid, IMajorRecord>(l => l)
                    .ToObservableChangeSet_SingleItemNotNull(),
                cell.WhenAny(x => x.Landscape)
                    .Select<Landscape, IMajorRecord>(l => l)
                    .ToObservableChangeSet_SingleItemNotNull(),
                Observable.Merge(
                    cell.Persistent.Connect(),
                    cell.Temporary.Connect(),
                    cell.VisibleWhenDistant.Connect())
                    .Transform<IPlaced, IMajorRecord>(p => p));
        }

        private IObservable<IChangeSet<IMajorRecord>> GetWorldspaceRecords(Worldspace worldspace)
        {
            return Observable.Merge<IChangeSet<IMajorRecord>>(
                Observable
                    .Return<IMajorRecord>(worldspace)
                    .ToObservableChangeSet(),
                worldspace.WhenAny(x => x.Road)
                    .Select<Road, IMajorRecord>(l => l)
                    .ToObservableChangeSet_SingleItemNotNull(),
                worldspace.WhenAny(x => x.TopCell)
                    .Select(c => GetCellRecords(c))
                    .Switch(),
                worldspace.SubCells.Connect()
                    .TransformMany(block => block.Items)
                    .TransformMany(subBlock => subBlock.Items)
                    .MergeMany(c => GetCellRecords(c)));
        }

        private IObservable<IChangeSet<IMajorRecord>> GetDialogRecords(DialogTopic dialog)
        {
            return Observable.Merge<IChangeSet<IMajorRecord>>(
                Observable
                    .Return<IMajorRecord>(dialog)
                    .ToObservableChangeSet(),
                dialog.Items.Connect()
                    .Transform<DialogItem, IMajorRecord>(i => i));
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
                    || cell.PathGrid_IsSet
                    || cell.Landscape_IsSet)
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
            count += this.Cells.Items.Count; // Block Count
            count += this.Cells.Items.Sum(block => block.Items.Count); // Sub Block Count
            count += this.Cells.Items
                .SelectMany(block => block.Items)
                .SelectMany(subBlock => subBlock.Items)
                .Select(cellSubGroupCount)
                .Sum();

            // Tally Worldspace Group Counts
            count += this.Worldspaces.Sum(wrld => wrld.SubCells.Count); // Cell Blocks
            count += this.Worldspaces
                .SelectMany(wrld => wrld.SubCells.Items)
                .Sum(block => block.Items.Count); // Cell Sub Blocks
            count += this.Worldspaces
                .SelectMany(wrld => wrld.SubCells.Items)
                .SelectMany(block => block.Items)
                .SelectMany(subBlock => subBlock.Items)
                .Sum(cellSubGroupCount); // Cell sub groups

            // Tally Dialog Group Counts
            count += this.DialogTopics.Items.Count;
            setter(count);
        }

        async Task Create_Xml_Folder_Worldspaces(DirectoryPath dir, string name, int index, ErrorMaskBuilder errorMask)
        {
            dir = new DirectoryPath(Path.Combine(dir.Path, nameof(this.Worldspaces)));
            using (errorMask?.PushIndex(index))
            {
                using (errorMask?.PushIndex((int)Group_FieldIndex.Items))
                {
                    try
                    {
                        var path = Path.Combine(dir.Path, $"Group.xml");
                        if (File.Exists(path))
                        {
                            this.Worldspaces.FillPublic_Xml(
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
                                var get = await Worldspace.TryCreate_Xml_Folder(subDir.Dir, errorMask);
                                return get.Value;
                            }));
                        }
                        var worldspaces = await Task.WhenAll(tasks);
                        this.Worldspaces.Items.Set(worldspaces.Where(ws => ws != null));
                    }
                    catch (Exception ex)
                    when (errorMask != null)
                    {
                        errorMask.ReportException(ex);
                    }
                }
            }
        }

        async Task Write_Xml_Folder_Worldspaces(DirectoryPath dir, string name, int index, ErrorMaskBuilder errorMask)
        {
            if (!this.Worldspaces.Items.HasBeenSet) return;
            dir = new DirectoryPath(Path.Combine(dir.Path, nameof(this.Worldspaces)));
            using (errorMask?.PushIndex(index))
            {
                using (errorMask?.PushIndex((int)Group_FieldIndex.Items))
                {
                    dir.Create();
                    this.Worldspaces.Write_Xml(
                        Path.Combine(dir.Path, $"Group.xml"),
                        errorMask,
                        translationMask: GroupExt.XmlFolderTranslationCrystal);
                    int counter = 0;
                    List<Task> tasks = new List<Task>();
                    foreach (var item in this.Worldspaces.Items.Items)
                    {
                        //using (errorMask.PushIndex(counter))
                        //{
                        //    try
                        //    {
                        tasks.Add(
                            item.Write_Xml_Folder(
                                node: null,
                                name: name,
                                counter: counter++,
                                dir: dir,
                                errorMask: errorMask));
                        //    }
                        //    catch (Exception ex)
                        //    when (errorMask != null)
                        //    {
                        //        errorMask.ReportException(ex);
                        //    }
                        //}
                        //counter++;
                    }
                    await Task.WhenAll(tasks);
                }
            }
        }

        public static readonly Script_TranslationMask ScriptXmlFolderTranslationMask = new Script_TranslationMask(true)
        {
            Fields = new Loqui.MaskItem<bool, ScriptFields_TranslationMask>(
                true,
                new ScriptFields_TranslationMask(true)
                {
                    SourceCode = false
                })
        };
        public static readonly TranslationCrystal ScriptXmlFolderTranslationCrystal = ScriptXmlFolderTranslationMask.GetCrystal();

        async Task Create_Xml_Folder_Scripts(DirectoryPath dir, string name, int index, ErrorMaskBuilder errorMask)
        {
            dir = new DirectoryPath(Path.Combine(dir.Path, nameof(this.Scripts)));

            var path = Path.Combine(dir.Path, $"Group.xml");
            if (File.Exists(path))
            {
                this.Scripts.FillPublic_Xml(
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
                    var script = Script.Create_Xml(
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
                var scripts = await Task.WhenAll(tasks);
                this.Scripts.Items.Set(scripts);
            }
        }

        async Task Write_Xml_Folder_Scripts(DirectoryPath dir, string name, int index, ErrorMaskBuilder errorMask)
        {
            if (!this.Scripts.Items.HasBeenSet) return;
            dir = new DirectoryPath(Path.Combine(dir.Path, nameof(this.Scripts)));
            using (errorMask?.PushIndex(index))
            {
                using (errorMask?.PushIndex((int)Group_FieldIndex.Items))
                {
                    dir.Create();
                    this.Scripts.Write_Xml(
                        Path.Combine(dir.Path, "Group.xml"),
                        errorMask,
                        translationMask: GroupExt.XmlFolderTranslationCrystal);
                    int counter = 0;
                    List<Task> tasks = new List<Task>();
                    foreach (var item in this.Scripts.Items.Items)
                    {
                        DirectoryPath subDir = new DirectoryPath(Path.Combine(dir.Path, FolderTranslation.GetFileString(item, counter++)));

                        subDir.Create();
                        tasks.Add(Task.Run(() =>
                        {
                            item.Write_Xml(
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
                    await Task.WhenAll(tasks);
                }
            }
        }
    }
}
