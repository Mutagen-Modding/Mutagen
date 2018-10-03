using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using ReactiveUI;
using Noggog;
using Noggog.Notifying;
using CSharpExt.Rx;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class OblivionMod : IMod, ILinkContainer
    {
        private static readonly object _subscribeObject = new object();
        public IObservableSetList<MasterReference> MasterReferences => this.TES4.MasterReferences;

        partial void CustomCtor()
        {
            this.SubscribeToCells();
            this.SubscribeToWorldspaces();
            this.SubscribeToDialogs();
        }

        #region Cell Subscription
        protected void SubscribeToCells()
        {
            //_Cells_Object.Items.Subscribe_Enumerable_Single((change) =>
            //{
            //    switch (change.AddRem)
            //    {
            //        case AddRemove.Add:
            //            SubscribeToCellBlock(change.Item);
            //            break;
            //        case AddRemove.Remove:
            //            UnsubscribeFromCellBlock(change.Item);
            //            break;
            //        default:
            //            throw new NotImplementedException();
            //    }
            //});
        }

        protected void SubscribeToCellBlock(CellBlock block)
        {
            block.Items.Subscribe_Enumerable_Single(_subscribeObject, (change) =>
            {
                switch (change.AddRem)
                {
                    case AddRemove.Add:
                        SubscribeToCellSubBlock(change.Item);
                        break;
                    case AddRemove.Remove:
                        change.Item.Items.Unsubscribe(_subscribeObject);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            });
        }

        protected void SubscribeToCellSubBlock(CellSubBlock subBlock)
        {
            subBlock.Items.Subscribe_Enumerable_Single(_subscribeObject, (change) =>
            {
                switch (change.AddRem)
                {
                    case AddRemove.Add:
                        SubscribeToCell(change.Item);
                        break;
                    case AddRemove.Remove:
                        UnsubscribeFromCell(change.Item);
                        break;
                    default:
                        break;
                }
                Utility.ModifyButThrow(_majorRecords, change);
            });
        }

        protected void SubscribeToCell(Cell cell)
        {
            cell.Persistent.Subscribe_Enumerable_Single(_subscribeObject, (r) => Utility.ModifyButThrow(_majorRecords, r));
            cell.Temporary.Subscribe_Enumerable_Single(_subscribeObject, (r) => Utility.ModifyButThrow(_majorRecords, r));
            cell.VisibleWhenDistant.Subscribe_Enumerable_Single(_subscribeObject, (r) => Utility.ModifyButThrow(_majorRecords, r));

            // ToDo
            // Unsubscribe mechanics, add back in remove
            cell.WhenAny(x => x.PathGrid).Subscribe((r) =>
            {
                //if (r.Old != null)
                //{
                //    _majorRecords.Remove(r.Old.FormID);
                //}
                if (r != null)
                {
                    if (_majorRecords.ContainsKey(r.FormID))
                    {
                        throw new ArgumentException($"Cannot add a pathgrid {r.FormID} that exists elsewhere in the same mod.");
                    }
                    _majorRecords[r.FormID] = r;
                }
            });
        }

        protected void UnsubscribeFromCellBlock(CellBlock block)
        {
            block.Items.Unsubscribe(_subscribeObject);
            foreach (var subBlock in block.Items)
            {
                subBlock.Items.Unsubscribe(_subscribeObject);
                foreach (var cell in subBlock.Items)
                {
                    _majorRecords.Remove(cell.FormID);
                }
            }
        }

        protected void UnsubscribeFromCell(Cell cell)
        {
            cell.Persistent.Unsubscribe(_subscribeObject);
            cell.Temporary.Unsubscribe(_subscribeObject);
            cell.VisibleWhenDistant.Unsubscribe(_subscribeObject);
            _majorRecords.Remove(cell.PathGrid.FormID);
            // ToDo
            // Unsubscribe mechanics for pathgrid
        }
        #endregion

        #region Worldspace Subscription
        protected void SubscribeToWorldspaces()
        {
            //_Worldspaces_Object.Items.Subscribe_Enumerable_Single((change) =>
            //{
            //    switch (change.AddRem)
            //    {
            //        case AddRemove.Add:
            //            SubscribeToWorldspace(change.Item.Value);
            //            break;
            //        case AddRemove.Remove:
            //            UnsubscribeFromWorldspace(change.Item.Value);
            //            break;
            //        default:
            //            throw new NotImplementedException();
            //    }
            //});
        }

        protected void SubscribeToWorldspace(Worldspace worldspace)
        {
            // ToDo
            // Unsubscribe mechanics, add back in remove
            worldspace.WhenAny(x => x.Road).Subscribe((r) =>
            {
                //if (r.Old != null)
                //{
                //    _majorRecords.Remove(r.Old.FormID);
                //}
                if (r != null)
                {
                    if (_majorRecords.ContainsKey(r.FormID))
                    {
                        throw new ArgumentException("Cannot add a road that exists elsewhere in the same mod.");
                    }
                    _majorRecords[r.FormID] = r;
                }
            });
            // ToDo
            // Unsubscribe mechanics, add back in remove
            worldspace.WhenAny(x => x.TopCell).Subscribe((r) =>
            {
                //if (r.Old != null)
                //{
                //    _majorRecords.Remove(r.Old.FormID);
                //    UnsubscribeFromCell(r.Old);
                //}
                if (r != null)
                {
                    SubscribeToCell(r);
                    _majorRecords[r.FormID] = r;
                }
            });
            worldspace.SubCells.Subscribe_Enumerable_Single(_subscribeObject, (change) =>
            {
                switch (change.AddRem)
                {
                    case AddRemove.Add:
                        SubscribeToWorldspaceBlock(change.Item);
                        break;
                    case AddRemove.Remove:
                        change.Item.Items.Unsubscribe(_subscribeObject);
                        break;
                    default:
                        break;
                }
            });
        }

        protected void SubscribeToWorldspaceBlock(WorldspaceBlock block)
        {
            block.Items.Subscribe_Enumerable_Single(_subscribeObject, (change) =>
            {
                switch (change.AddRem)
                {
                    case AddRemove.Add:
                        SubscribeToWorldspaceSubBlock(change.Item);
                        break;
                    case AddRemove.Remove:
                        change.Item.Items.Unsubscribe(_subscribeObject);
                        break;
                    default:
                        break;
                }
            });
        }

        protected void SubscribeToWorldspaceSubBlock(WorldspaceSubBlock block)
        {
            block.Items.Subscribe_Enumerable_Single(_subscribeObject, (change) =>
            {
                switch (change.AddRem)
                {
                    case AddRemove.Add:
                        SubscribeToCell(change.Item);
                        break;
                    case AddRemove.Remove:
                        UnsubscribeFromCell(change.Item);
                        break;
                    default:
                        break;
                }
                Utility.ModifyButThrow(_majorRecords, change);
            });
        }

        protected void UnsubscribeFromWorldspace(Worldspace worldspace)
        {
            worldspace.SubCells.Unsubscribe(_subscribeObject);
            // ToDo
            // Unsubscribe mechanics
            //worldspace.TopCell_Property.Unsubscribe(_subscribeObject);
            //worldspace.Road_Property.Unsubscribe(_subscribeObject);
        }
        #endregion

        #region Dialog Subscription
        protected void SubscribeToDialogs()
        {
            //_DialogTopics_Object.Items.Subscribe_Enumerable_Single((change) =>
            //{
            //    switch (change.AddRem)
            //    {
            //        case AddRemove.Add:
            //            change.Item.Value.Items.Subscribe_Enumerable_Single(
            //                _subscribeObject, 
            //                (r) => Utility.ModifyButThrow(_majorRecords, r));
            //            break;
            //        case AddRemove.Remove:
            //            foreach (var item in change.Item.Value.Items)
            //            {
            //                _majorRecords.Remove(item.FormID);
            //            }
            //            change.Item.Value.Items.Unsubscribe(_subscribeObject);
            //            break;
            //        default:
            //            throw new NotImplementedException();
            //    }
            //});
        }
        #endregion
    }
}
