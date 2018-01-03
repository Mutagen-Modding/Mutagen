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
        public RecordType? UnlinkedEDID { get; private set; } = EDIDLink<T>.UNLINKED;

        public EDIDSetLink()
            : base()
        {
            this.Subscribe(UpdateUnlinked);
        }

        public EDIDSetLink(RecordType unlinkedEDID)
            : this()
        {
            this.UnlinkedEDID = unlinkedEDID;
            this.Subscribe(UpdateUnlinked);
        }

        public EDIDSetLink(RawFormID unlinkedForm)
            : base(unlinkedForm)
        {
            this.Subscribe(UpdateUnlinked);
        }

        private void UpdateUnlinked(Change<T> change)
        {
            var edid = change.New?.EditorID;
            if (edid != null)
            {
                this.UnlinkedEDID = null;
            }
            else
            {
                this.UnlinkedEDID = EDIDLink<T>.UNLINKED;
            }
        }

        public void SetIfSucceeded(TryGet<RecordType> item)
        {
            if (item.Failed) return;
            this.UnlinkedEDID = item.Value;
        }

        public void SetIfSuccessful(TryGet<string> item)
        {
            if (!item.Succeeded) return;
            this.UnlinkedEDID = new RecordType(item.Value);
        }
    }
}
