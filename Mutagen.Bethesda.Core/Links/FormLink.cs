using Loqui;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class FormLink<TMajor> : IFormLink<TMajor>, IEquatable<IFormLinkGetter<TMajor>>
       where TMajor : class, IMajorRecordCommonGetter
    {
        public static readonly IFormLinkGetter<TMajor> Empty = new FormLink<TMajor>();

        public virtual FormKey FormKey { get; set; } = FormKey.Null;
        Type ILinkGetter.TargetType => typeof(TMajor);

        public FormLink()
        {
        }

        public FormLink(FormKey formKey)
        {
            this.FormKey = formKey;
        }

        public virtual void Set(FormKey formKey)
        {
            this.Set(formKey);
        }

        public virtual void Set(TMajor value)
        {
            this.FormKey = value?.FormKey ?? FormKey.Null;
        }

        public virtual void Unset()
        {
            this.FormKey = FormKey.Null;
        }

        public static bool operator ==(FormLink<TMajor> lhs, IFormLink<TMajor> rhs)
        {
            return lhs.FormKey.Equals(rhs.FormKey);
        }

        public static bool operator !=(FormLink<TMajor> lhs, IFormLink<TMajor> rhs)
        {
            return !lhs.FormKey.Equals(rhs.FormKey);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ILinkGetter<TMajor> rhs)) return false;
            return this.Equals(rhs);
        }

        public bool Equals(IFormLinkGetter<TMajor> other) => this.FormKey.Equals(other.FormKey);

        public override int GetHashCode() => this.FormKey.GetHashCode();

        public override string ToString() => this.FormKey.ToString();

        public bool TryResolve<M>(ILinkCache<M> package, [MaybeNullWhen(false)] out TMajor major)
            where M : IModGetter
        {
            if (this.FormKey.Equals(FormKey.Null))
            {
                major = default!;
                return false;
            }
            if (package.TryLookup<TMajor>(this.FormKey, out var majorRec))
            {
                major = majorRec;
                return true;
            }
            major = default!;
            return false;
        }

        public bool TryResolveFormKey<M>(ILinkCache<M> package, [MaybeNullWhen(false)] out FormKey formKey)
            where M : IModGetter
        {
            formKey = this.FormKey;
            return true;
        }

        bool ILinkGetter.TryResolveCommon<M>(ILinkCache<M> package, [MaybeNullWhen(false)] out IMajorRecordCommonGetter formKey)
        {
            if (TryResolve(package, out var rec))
            {
                formKey = rec;
                return true;
            }
            formKey = default!;
            return false;
        }

        public ITryGetter<TMajor> TryResolve<TMod>(ILinkCache<TMod> package)
            where TMod : IModGetter
        {
            if (TryResolve(package, out var rec))
            {
                return TryGet<TMajor>.Succeed(rec);
            }
            return TryGet<TMajor>.Failure;
        }

        public bool TryGetModKey([MaybeNullWhen(false)] out ModKey modKey)
        {
            modKey = this.FormKey.ModKey;
            return true;

        }
    }
}
