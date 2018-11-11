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

            this.Cells.Items.Connect()
                .TransformMany(cellBlock => cellBlock.Items)
                .TransformMany(cellSubBlock => cellSubBlock.Items)
                .Transform<Cell, IMajorRecord>(c => c)
                .AddKey(c => c.FormKey)
                .PopulateInto(this._majorRecords);
            //this.SubscribeToWorldspaces();
            //this.SubscribeToDialogs();
        }

        public FormKey GetNextFormKey()
        {
            return new FormKey(
                this.ModKey,
                this.TES4.Header.NextObjectID++);
        }

        //#region Cell Subscription

        //protected void SubscribeToCellSubBlock(CellSubBlock subBlock)
        //{
        //    subBlock.Items.Subscribe_Enumerable_Single(_subscribeObject, (change) =>
        //    {
        //        switch (change.AddRem)
        //        {
        //            case AddRemove.Add:
        //                SubscribeToCell(change.Item);
        //                break;
        //            case AddRemove.Remove:
        //                UnsubscribeFromCell(change.Item);
        //                break;
        //            default:
        //                break;
        //        }
        //        Mutagen.Bethesda.Utility.ModifyButThrow(_majorRecords, change);
        //    });
        //}

        //protected void SubscribeToCell(Cell cell)
        //{
        //    cell.Persistent.Subscribe_Enumerable_Single(_subscribeObject, (r) => Mutagen.Bethesda.Utility.ModifyButThrow(_majorRecords, r));
        //    cell.Temporary.Subscribe_Enumerable_Single(_subscribeObject, (r) => Mutagen.Bethesda.Utility.ModifyButThrow(_majorRecords, r));
        //    cell.VisibleWhenDistant.Subscribe_Enumerable_Single(_subscribeObject, (r) => Mutagen.Bethesda.Utility.ModifyButThrow(_majorRecords, r));
        //    cell.Landscape_Property.Subscribe(_subscribeObject, (r) =>
        //    {
        //        if (r.Old != null)
        //        {
        //            _majorRecords.Remove(r.Old.FormKey);
        //        }
        //        if (r.New != null)
        //        {
        //            if (_majorRecords.ContainsKey(r.New.FormKey))
        //            {
        //                throw new ArgumentException($"Cannot add a landscape {r.New.FormKey} that exists elsewhere in the same mod.");
        //            }
        //            _majorRecords[r.New.FormKey] = r.New;
        //        }
        //    });

        //    // ToDo
        //    // Unsubscribe mechanics, add back in remove
        //    cell.WhenAny(x => x.PathGrid).Subscribe((r) =>
        //    {
        //        //if (r.Old != null)
        //        //{
        //        //    _majorRecords.Remove(r.Old.FormKey);
        //        //}
        //        if (r != null)
        //        {
        //            if (_majorRecords.ContainsKey(r.FormKey))
        //            {
        //                throw new ArgumentException($"Cannot add a pathgrid {r.FormKey} that exists elsewhere in the same mod.");
        //            }
        //            _majorRecords[r.FormKey] = r;
        //        }
        //    });
        //}

        //protected void UnsubscribeFromCellBlock(CellBlock block)
        //{
        //    block.Items.Unsubscribe(_subscribeObject);
        //    foreach (var subBlock in block.Items)
        //    {
        //        subBlock.Items.Unsubscribe(_subscribeObject);
        //        foreach (var cell in subBlock.Items)
        //        {
        //            _majorRecords.Remove(cell.FormKey);
        //        }
        //    }
        //}

        //protected void UnsubscribeFromCell(Cell cell)
        //{
        //    cell.Persistent.Unsubscribe(_subscribeObject);
        //    cell.Temporary.Unsubscribe(_subscribeObject);
        //    cell.VisibleWhenDistant.Unsubscribe(_subscribeObject);
        //    _majorRecords.Remove(cell.PathGrid.FormID);
        //    // ToDo
        //    // Unsubscribe mechanics for pathgrid
        //}
        //#endregion

        //#region Worldspace Subscription
        //protected void SubscribeToWorldspaces()
        //{
        //    //_Worldspaces_Object.Items.Subscribe_Enumerable_Single((change) =>
        //    //{
        //    //    switch (change.AddRem)
        //    //    {
        //    //        case AddRemove.Add:
        //    //            SubscribeToWorldspace(change.Item.Value);
        //    //            break;
        //    //        case AddRemove.Remove:
        //    //            UnsubscribeFromWorldspace(change.Item.Value);
        //    //            break;
        //    //        default:
        //    //            throw new NotImplementedException();
        //    //    }
        //    //});
        //}

        //protected void SubscribeToWorldspace(Worldspace worldspace)
        //{
        //    // ToDo
        //    // Unsubscribe mechanics, add back in remove
        //    worldspace.WhenAny(x => x.Road).Subscribe((r) =>
        //    {
        //        //if (r.Old != null)
        //        //{
        //        //    _majorRecords.Remove(r.Old.FormID);
        //        //}
        //        if (r != null)
        //        {
        //            if (_majorRecords.ContainsKey(r.FormKey))
        //            {
        //                throw new ArgumentException("Cannot add a road that exists elsewhere in the same mod.");
        //            }
        //            _majorRecords[r.FormKey] = r;
        //        }
        //    });
        //    // ToDo
        //    // Unsubscribe mechanics, add back in remove
        //    worldspace.WhenAny(x => x.TopCell).Subscribe((r) =>
        //    {
        //        //if (r.Old != null)
        //        //{
        //        //    _majorRecords.Remove(r.Old.FormID);
        //        //    UnsubscribeFromCell(r.Old);
        //        //}
        //        if (r != null)
        //        {
        //            SubscribeToCell(r);
        //            _majorRecords[r.FormKey] = r;
        //        }
        //    });
        //    worldspace.SubCells.Subscribe_Enumerable_Single(_subscribeObject, (change) =>
        //    {
        //        switch (change.AddRem)
        //        {
        //            case AddRemove.Add:
        //                SubscribeToWorldspaceBlock(change.Item);
        //                break;
        //            case AddRemove.Remove:
        //                change.Item.Items.Unsubscribe(_subscribeObject);
        //                break;
        //            default:
        //                break;
        //        }
        //    });
        //}

        //protected void SubscribeToWorldspaceBlock(WorldspaceBlock block)
        //{
        //    block.Items.Subscribe_Enumerable_Single(_subscribeObject, (change) =>
        //    {
        //        switch (change.AddRem)
        //        {
        //            case AddRemove.Add:
        //                SubscribeToWorldspaceSubBlock(change.Item);
        //                break;
        //            case AddRemove.Remove:
        //                change.Item.Items.Unsubscribe(_subscribeObject);
        //                break;
        //            default:
        //                break;
        //        }
        //    });
        //}

        //protected void SubscribeToWorldspaceSubBlock(WorldspaceSubBlock block)
        //{
        //    block.Items.Subscribe_Enumerable_Single(_subscribeObject, (change) =>
        //    {
        //        switch (change.AddRem)
        //        {
        //            case AddRemove.Add:
        //                SubscribeToCell(change.Item);
        //                break;
        //            case AddRemove.Remove:
        //                UnsubscribeFromCell(change.Item);
        //                break;
        //            default:
        //                break;
        //        }
        //        Utility.ModifyButThrow(_majorRecords, change);
        //    });
        //}

        //protected void UnsubscribeFromWorldspace(Worldspace worldspace)
        //{
        //    worldspace.SubCells.Unsubscribe(_subscribeObject);
        //    // ToDo
        //    // Unsubscribe mechanics
        //    //worldspace.TopCell_Property.Unsubscribe(_subscribeObject);
        //    //worldspace.Road_Property.Unsubscribe(_subscribeObject);
        //}
        //#endregion

        //#region Dialog Subscription
        //protected void SubscribeToDialogs()
        //{
        //    //_DialogTopics_Object.Items.Subscribe_Enumerable_Single((change) =>
        //    //{
        //    //    switch (change.AddRem)
        //    //    {
        //    //        case AddRemove.Add:
        //    //            change.Item.Value.Items.Subscribe_Enumerable_Single(
        //    //                _subscribeObject, 
        //    //                (r) => Utility.ModifyButThrow(_majorRecords, r));
        //    //            break;
        //    //        case AddRemove.Remove:
        //    //            foreach (var item in change.Item.Value.Items)
        //    //            {
        //    //                _majorRecords.Remove(item.FormID);
        //    //            }
        //    //            change.Item.Value.Items.Unsubscribe(_subscribeObject);
        //    //            break;
        //    //        default:
        //    //            throw new NotImplementedException();
        //    //    }
        //    //});
        //}

        //public FormKey GetNextAvailableFormKey()
        //{
        //    return new FormKey(
        //        this.ModKey,
        //        this.TES4.Header.NextObjectID++);
        //}
        //#endregion
    }
}
