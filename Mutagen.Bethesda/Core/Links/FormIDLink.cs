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
    public class FormIDLink<TMajor> : IFormIDLink<TMajor>, IEquatable<IFormIDLinkGetter<TMajor>>
       where TMajor : class, IMajorRecordCommonGetter
    {
        public static readonly IFormIDLinkGetter<TMajor> Empty = new FormIDLink<TMajor>();

        public virtual FormKey FormKey { get; set; } = FormKey.NULL;
        Type ILinkGetter.TargetType => typeof(TMajor);

        public FormIDLink()
        {
        }

        public FormIDLink(FormKey formKey)
        {
            this.FormKey = formKey;
        }

        public virtual void Set(FormKey formKey)
        {
            this.Set(formKey);
        }

        public virtual void Set(TMajor value)
        {
            this.FormKey = value?.FormKey ?? FormKey.NULL;
        }

        public virtual void Unset()
        {
            this.FormKey = FormKey.NULL;
        }

        public static bool operator ==(FormIDLink<TMajor> lhs, IFormIDLink<TMajor> rhs)
        {
            return lhs.FormKey.Equals(rhs.FormKey);
        }

        public static bool operator !=(FormIDLink<TMajor> lhs, IFormIDLink<TMajor> rhs)
        {
            return !lhs.FormKey.Equals(rhs.FormKey);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ILinkGetter<TMajor> rhs)) return false;
            return this.Equals(rhs);
        }

        public bool Equals(IFormIDLinkGetter<TMajor> other) => this.FormKey.Equals(other.FormKey);

        public override int GetHashCode() => this.FormKey.GetHashCode();

        public override string ToString() => this.FormKey.ToString();

        public bool TryResolve<M>(LinkingPackage<M> package, out TMajor major)
            where M : IModGetter
        {
            if (this.FormKey.Equals(FormKey.NULL))
            {
                major = default;
                return false;
            }
            if (package.TryGetMajorRecord<TMajor>(this.FormKey, out var majorRec))
            {
                major = majorRec;
                return true;
            }
            major = default;
            return false;
        }

        public bool TryResolveFormKey<M>(LinkingPackage<M> package, out FormKey formKey)
            where M : IModGetter
        {
            formKey = this.FormKey;
            return true;
        }

        bool ILinkGetter.TryResolve<M>(LinkingPackage<M> package, out IMajorRecordCommonGetter formKey)
        {
            if (TryResolve(package, out TMajor rec))
            {
                formKey = rec;
                return true;
            }
            formKey = default;
            return false;
        }

        TMajor ILinkGetter<TMajor>.Resolve<M>(LinkingPackage<M> package)
        {
            if (this.TryResolve(package, out var major))
            {
                return major;
            }
            return default;
        }
    }
}
