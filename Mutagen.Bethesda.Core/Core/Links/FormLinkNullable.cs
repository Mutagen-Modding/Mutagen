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
    public class FormLinkNullable<TMajor> : IFormLinkNullable<TMajor>, IEquatable<IFormLinkGetter<TMajor>>, IEquatable<IFormLinkNullableGetter<TMajor>>
       where TMajor : class, IMajorRecordCommonGetter
    {
        public static readonly IFormLinkNullableGetter<TMajor> Empty = new FormLinkNullable<TMajor>();

        public FormKey? FormKey { get; set; } = null;
        Type ILinkGetter.TargetType => typeof(TMajor);

        public FormLinkNullable()
        {
        }

        public FormLinkNullable(FormKey formKey)
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

        public static bool operator ==(FormLinkNullable<TMajor> lhs, IFormLink<TMajor> rhs)
        {
            return lhs.FormKey?.Equals(rhs.FormKey) ?? false;
        }

        public static bool operator !=(FormLinkNullable<TMajor> lhs, IFormLink<TMajor> rhs)
        {
            return !lhs.FormKey?.Equals(rhs.FormKey) ?? true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ILinkGetter<TMajor> rhs)) return false;
            return this.Equals(rhs);
        }

        public bool Equals(IFormLinkGetter<TMajor> other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKey, other.FormKey);

        public bool Equals(IFormLinkNullableGetter<TMajor> other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKey, other.FormKey);

        public override int GetHashCode() => this.FormKey?.GetHashCode() ?? 0;

        public override string ToString() => this.FormKey?.ToString() ?? string.Empty;

        public bool TryResolve<M>(ILinkCache<M> package, [MaybeNullWhen(false)] out TMajor major)
            where M : IModGetter
        {
            if (this.FormKey == null
                || this.FormKey.Equals(FormKey.Null))
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
            if (this.FormKey == null)
            {
                formKey = default!;
                return false;
            }
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
    }
}
