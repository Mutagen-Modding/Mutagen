using Loqui;
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
    public class FormIDSetLink<T> : FormIDLink<T>, IFormIDSetLink<T>, IEquatable<IFormIDSetLinkGetter<T>>
       where T : class, IMajorRecordCommonGetter
    {
        public static readonly IFormIDSetLinkGetter<T> Empty = new FormIDSetLink<T>();

        public bool HasBeenSet { get; set; }

        public FormIDSetLink()
        {
        }

        public FormIDSetLink(FormKey formKey)
        {
            this.FormKey = formKey;
            this.HasBeenSet = true;
        }

        public override void Set(T value)
        {
            base.Set(value);
        }

        public void Set(T value, bool hasBeenSet)
        {
            this.Set(value?.FormKey ?? FormKey.NULL, hasBeenSet);
        }

        public void Set(FormKey value, bool hasBeenSet)
        {
            this.FormKey = value;
            this.HasBeenSet = hasBeenSet;
        }

        public override void Unset()
        {
            this.FormKey = FormKey.NULL;
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
