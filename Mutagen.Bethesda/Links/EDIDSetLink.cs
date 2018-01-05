using Noggog;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class EDIDSetLink<T> : FormIDSetLink<T>, IEDIDSetLink<T>
       where T : MajorRecord
    {
        public RecordType EDID { get; private set; } = EDIDLink<T>.UNLINKED;

        public EDIDSetLink()
            : base()
        {
            this.Subscribe(HandleItemChange, fireInitial: false);
        }

        public EDIDSetLink(RecordType unlinkedEDID)
            : this()
        {
            this.EDID = unlinkedEDID;
            this.Subscribe(HandleItemChange, fireInitial: false);
        }

        public EDIDSetLink(RawFormID unlinkedForm)
            : base(unlinkedForm)
        {
            this.Subscribe(HandleItemChange, fireInitial: false);
        }

        private void HandleItemChange(Change<T> change)
        {
            this.EDID = EDIDLink<T>.UNLINKED;
            change.Old?.EditorID_Property.Unsubscribe(this);
            change.New?.EditorID_Property.Subscribe(this, UpdateUnlinked);
        }

        private void UpdateUnlinked(Change<string> change)
        {
            this.EDID = new RecordType(change.New);
        }

        public void SetIfSucceeded(TryGet<RecordType> item)
        {
            if (item.Failed) return;
            this.EDID = item.Value;
            this.HasBeenSet = true;
        }

        public void SetIfSuccessful(TryGet<string> item)
        {
            if (!item.Succeeded) return;
            this.EDID = new RecordType(item.Value);
            this.HasBeenSet = true;
        }
    }
}
