using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class FormIDSetLink<T> : FormIDLink<T>, IFormIDSetLink<T>, IEquatable<IFormIDSetLinkGetter<T>>
       where T : class, IMajorRecordCommonGetter
    {
        public new static readonly IFormIDSetLinkGetter<T> Empty = new FormIDSetLink<T>();

        public bool HasBeenSet { get; set; }
        public override FormKey FormKey
        { 
            get => base.FormKey;
            set => this.Set(value, true);
        }

        public FormIDSetLink()
        {
        }

        public FormIDSetLink(FormKey formKey)
        {
            base.FormKey = formKey;
            this.HasBeenSet = true;
        }

        public override void Set(T value)
        {
            base.Set(value);
        }

        public void Set(T value, bool hasBeenSet)
        {
            this.Set(value?.FormKey ?? FormKey.Null, hasBeenSet);
        }

        public void Set(FormKey value, bool hasBeenSet)
        {
            base.FormKey = value;
            this.HasBeenSet = hasBeenSet;
        }

        public override void Unset()
        {
            base.FormKey = FormKey.Null;
            this.HasBeenSet = false;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is IFormIDSetLinkGetter<T> rhs)) return false;
            return this.Equals(rhs);
        }

        public bool Equals(IFormIDSetLinkGetter<T> other)
        {
            if (this.HasBeenSet != other.HasBeenSet) return false;
            if (this.FormKey != other.FormKey) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return this.FormKey.GetHashCode()
                .CombineHashCode(this.HasBeenSet.GetHashCode());
        }
    }
}
