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
            return dialog.Items.Connect()
                .Transform<DialogItem, IMajorRecord>(i => i);
        }

        public FormKey GetNextFormKey()
        {
            return new FormKey(
                this.ModKey,
                this.TES4.Header.NextObjectID++);
        }
    }
}
