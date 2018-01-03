using Noggog;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class EDIDLink<T> : FormIDLink<T>, IEDIDLink<T>
       where T : MajorRecord
    {
        public static readonly RecordType UNLINKED = new RecordType("\0\0\0\0");
        public RecordType EDID { get; private set; } = UNLINKED;

        public EDIDLink()
            : base()
        {
            this.Subscribe(HandleItemChange, fireInitial: false);
        }

        public EDIDLink(RecordType unlinkedEDID)
            : this()
        {
            this.EDID = unlinkedEDID;
            this.Subscribe(HandleItemChange, fireInitial: false);
        }

        public EDIDLink(RawFormID unlinkedForm)
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
        }

        public void SetIfSuccessful(TryGet<string> item)
        {
            if (!item.Succeeded) return;
            this.EDID = new RecordType(item.Value);
        }
    }
}
