using Noggog;
using Noggog.Notifying;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Binary;

namespace Mutagen.Bethesda
{
    public class EDIDLink<TMajor> : IEDIDLink<TMajor>, IEquatable<IEDIDLinkGetter<TMajor>>
       where TMajor : class, IMajorRecordCommonGetter
    {
        public static readonly IEDIDLinkGetter<TMajor> Empty = new EDIDLink<TMajor>();

        public static readonly RecordType UNLINKED = new RecordType("\0\0\0\0");
        public virtual RecordType EDID { get; set; } = UNLINKED;
        Type ILinkGetter.TargetType => typeof(TMajor);

        public EDIDLink()
            : base()
        {
        }

        public EDIDLink(RecordType unlinkedEDID)
            : this()
        {
            this.EDID = unlinkedEDID;
        }

        public void Set(RecordType type)
        {
            this.EDID = type;
        }

        public void Set(TMajor value)
        {
            if (value?.EditorID == null)
            {
                this.EDID = UNLINKED;
            }
            this.Set(new RecordType(value.EditorID));
        }

        public bool Equals(IEDIDLinkGetter<TMajor> other) => this.EDID.Equals(other.EDID);

        public override int GetHashCode() => this.EDID.GetHashCode();

        public override string ToString() => this.EDID.ToString();

        private bool TryLinkToMod(
            IModGetter mod,
            out TMajor item)
        {
            if (this.EDID == UNLINKED)
            {
                item = default;
                return false;
            }
            // ToDo
            // Improve to not be a forloop
            var group = mod.GetGroupGetter<TMajor>();
            foreach (var rec in group.Items)
            {
                if (this.EDID.Type.Equals(rec.EditorID))
                {
                    item = rec;
                    return true;
                }
            }
            item = default;
            return false;
        }

        public bool TryResolve<M>(ILinkingPackage<M> package, out TMajor major)
            where M : IModGetter
        {
            if (this.EDID == UNLINKED)
            {
                major = default;
                return false;
            }
            if (package.SourceMod != null && TryLinkToMod(package.SourceMod, out var item))
            {
                major = item;
                return true;
            }
            if (package.ModList == null)
            {
                major = default;
                return false;
            }
            foreach (var listing in package.ModList)
            {
                if (!listing.Loaded)
                {
                    major = default;
                    return false;
                }
                if (TryLinkToMod(listing.Mod, out item))
                {
                    major = item;
                    return true;
                }
            }
            major = default;
            return false;
        }

        public bool TryResolveFormKey<M>(ILinkingPackage<M> package, out FormKey formKey)
            where M : IModGetter
        {
            if (TryResolve(package, out var rec))
            {
                formKey = rec.FormKey;
                return true;
            }
            formKey = default;
            return false;
        }

        bool ILinkGetter.TryResolveCommon<M>(ILinkingPackage<M> package, out IMajorRecordCommonGetter formKey)
        {
            if (TryResolve(package, out TMajor rec))
            {
                formKey = rec;
                return true;
            }
            formKey = default;
            return false;
        }

        public ITryGetter<TMajor> TryResolve<TMod>(ILinkingPackage<TMod> package) 
            where TMod : IModGetter
        {
            if (TryResolve(package, out TMajor rec))
            {
                return TryGet<TMajor>.Succeed(rec);
            }
            return TryGet<TMajor>.Failure;
        }

        public virtual void Unset()
        {
            this.EDID = UNLINKED;
        }
    }
}
