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
    public partial class OblivionMod : IMod<OblivionMod>, ILinkContainer
    {
        private static readonly object _subscribeObject = new object();
        public ISourceList<MasterReference> MasterReferences => this.TES4.MasterReferences;
        public ModKey ModKey { get; }

        void IMod<OblivionMod>.Write_Binary(string path, ModKey modKey)
        {
            this.Write_Binary(path, modKey, importMask: null);
        }

        public OblivionMod(ModKey modKey)
            : this()
        {
            this.ModKey = modKey;
        }

        partial void CustomCtor()
        {
            this.TES4.Header.NextObjectID = 0xD62; // first available ID on empty CS plugins

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
                this.TES4.Header.NextObjectID++);
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

        partial void Create_Xml_Folder_Worldspaces(DirectoryPath dir, string name, int index, ErrorMaskBuilder errorMask)
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
                            XElement elem = XElement.Load(path);
                            if (elem.Name != "Group")
                            {
                                throw new ArgumentException("XML file did not have \"Group\" top node.");
                            }
                            this.Worldspaces.FillPublic_Xml(
                                elem,
                                errorMask,
                                translationMask: GroupExt.XmlFolderTranslationCrystal);
                        }

                        int i = 0;
                        foreach (var subDir in dir.EnumerateDirectories(includeSelf: false, recursive: false)
                            .OrderBy(d => d.Name))
                        {
                            using (errorMask?.PushIndex(i++))
                            {
                                if (Worldspace.TryCreate_Xml_Folder(subDir, out var ws, errorMask))
                                {
                                    this.Worldspaces.Items.Set(ws);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    when (errorMask != null)
                    {
                        errorMask.ReportException(ex);
                    }
                }
            }
        }

        partial void Write_Xml_Folder_Worldspaces(DirectoryPath dir, string name, int index, ErrorMaskBuilder errorMask)
        {
            dir = new DirectoryPath(Path.Combine(dir.Path, nameof(this.Worldspaces)));
            using (errorMask?.PushIndex(index))
            {
                using (errorMask?.PushIndex((int)Group_FieldIndex.Items))
                {
                    XElement topNode = new XElement("Group");
                    this.Worldspaces.WriteToNode_Xml(
                        topNode,
                        errorMask,
                        translationMask: GroupExt.XmlFolderTranslationCrystal);
                    int counter = 0;
                    XElement items = new XElement("Items");
                    foreach (var item in this.Worldspaces.Items.Items)
                    {
                        using (errorMask.PushIndex(counter))
                        {
                            try
                            {
                                item.Write_Xml_Folder(
                                    node: items,
                                    name: name,
                                    counter: counter,
                                    dir: dir,
                                    errorMask: errorMask);
                            }
                            catch (Exception ex)
                            when (errorMask != null)
                            {
                                errorMask.ReportException(ex);
                            }
                        }
                        counter++;
                    }
                    if (items.HasElements)
                    {
                        topNode.Add(items);
                    }
                    if (topNode.HasElements)
                    {
                        dir.Create();
                        topNode.SaveIfChanged(Path.Combine(dir.Path, $"Group.xml"));
                    }
                }
            }
        }
    }
}
