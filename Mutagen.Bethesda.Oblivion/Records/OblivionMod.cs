using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using Noggog.Notifying;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class OblivionMod : IMod, ILinkContainer
    {
        public INotifyingListGetter<MasterReference> MasterReferences => this.TES4.MasterReferences;

        public static IReadOnlyCollection<RecordType> NonTypeGroups { get; } = new HashSet<RecordType>(
            new RecordType[]
            {
                new RecordType("WRLD"),
                new RecordType("DIAL"),
            });

        partial void CustomCtor()
        {
            this.SubscribeToCells();
        }

        protected void SubscribeToCells()
        {
            _Cells_Object.Items.Subscribe_Enumerable_Single((change) =>
            {
                switch (change.AddRem)
                {
                    case AddRemove.Add:
                        SubscribeToCellBlock(change.Item);
                        break;
                    case AddRemove.Remove:
                        UnsubscribeFromCellBlock(change.Item);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            });
        }

        protected void SubscribeToCellBlock(CellBlock block)
        {
            block.Items.Subscribe_Enumerable_Single(this, (change) =>
            {
                switch (change.AddRem)
                {
                    case AddRemove.Add:
                        SubscribeToCellSubBlock(change.Item);
                        break;
                    case AddRemove.Remove:
                        break;
                    default:
                        throw new NotImplementedException();
                }
            });
        }

        protected void SubscribeToCellSubBlock(CellSubBlock subBlock)
        {
            subBlock.Items.Subscribe_Enumerable_Single(this, (change) =>
            {
                if (change.AddRem == AddRemove.Add
                    && _majorRecords.ContainsKey(change.Item.FormID))
                {
                    throw new ArgumentException("Cannot add a cell that exists elsewhere in the same mod.");
                }
                _majorRecords.Modify(change.Item.FormID, change.Item, change.AddRem);
            });
        }

        protected void UnsubscribeFromCellBlock(CellBlock block)
        {
            block.Items.Unsubscribe(this);
            foreach (var subBlock in block.Items)
            {
                subBlock.Items.Unsubscribe(this);
                foreach (var cell in subBlock.Items)
                {
                    _majorRecords.Remove(cell.FormID);
                }
            }
        }
    }
}
