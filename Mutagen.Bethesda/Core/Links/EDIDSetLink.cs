using Noggog;
using Noggog.Notifying;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class EDIDSetLink<T> : EDIDLink<T>, IEDIDSetLink<T>
       where T : class, IMajorRecordCommonGetter
    {
        public new static readonly IEDIDSetLinkGetter<T> Empty = new EDIDSetLink<T>();

        public bool HasBeenSet { get; set; }

        public EDIDSetLink()
            : base()
        {
        }

        public EDIDSetLink(RecordType unlinkedEDID)
            : this()
        {
            this.EDID = unlinkedEDID;
        }

        public void Set(RecordType value, bool hasBeenSet)
        {
            this.HasBeenSet = hasBeenSet;
            this.EDID = value;
        }

        public void Set(T value, bool hasBeenSet)
        {
            if (value?.EditorID == null)
            {
                this.EDID = UNLINKED;
                this.HasBeenSet = hasBeenSet;
            }
            this.Set(new RecordType(value.EditorID), hasBeenSet);
        }

        public override void Unset()
        {
            this.HasBeenSet = false;
            this.EDID = UNLINKED;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is IEDIDSetLink<T> rhs)) return false;
            return this.Equals(rhs);
        }

        public bool Equals(IEDIDSetLink<T> other)
        {
            if (this.HasBeenSet != other.HasBeenSet) return false;
            if (this.EDID != other.EDID) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return this.EDID.GetHashCode()
                .CombineHashCode(this.HasBeenSet.GetHashCode());
        }
    }
}
