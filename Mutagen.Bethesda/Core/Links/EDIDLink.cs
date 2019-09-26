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
    public class EDIDLink<T> : FormIDLink<T>, IEDIDLink<T>
       where T : class, IMajorRecordCommonGetter
    {
        public new static readonly IEDIDLinkGetter<T> Empty = new EDIDLink<T>();

        public static readonly RecordType UNLINKED = new RecordType("\0\0\0\0");
        private IDisposable edidSub;
        public RecordType EDID { get; private set; } = UNLINKED;

        public EDIDLink()
            : base()
        {
        }

        public EDIDLink(RecordType unlinkedEDID)
            : this()
        {
            this.EDID = unlinkedEDID;
        }

        public EDIDLink(FormKey unlinkedForm)
            : base(unlinkedForm)
        {
        }

        public override void Set(T value)
        {
            HandleItemChange(new Change<T>(this.Item, value));
            base.Set(value);
        }

        public void Set(IEDIDLink<T> link)
        {
            if (link.Linked)
            {
                this.Set(link.Item);
            }
            else
            {
                this.EDID = link.EDID;
            }
        }

        private void HandleItemChange(Change<T> change)
        {
            this.EDID = EDIDLink<T>.UNLINKED;
            this.edidSub?.Dispose();
            this.edidSub = change.New?.WhenAny(x => x.EditorID)
                .Subscribe(UpdateUnlinked);
        }

        private void UpdateUnlinked(string change)
        {
            this.EDID = new RecordType(change);
        }

        public void SetIfSucceeded(TryGet<RecordType> item)
        {
            if (item.Failed) return;
            Set(item.Value);
        }

        public void Set(RecordType item)
        {
            this.EDID = item;
        }

        public void SetIfSuccessful(TryGet<string> item)
        {
            if (!item.Succeeded) return;
            Set(item.Value);
        }

        public void Set(string item)
        {
            this.EDID = new RecordType(item);
        }

        public void SetIfSucceededOrDefault(TryGet<RecordType> item)
        {
            if (item.Failed)
            {
                this.Unset();
                return;
            }
            this.EDID = item.Value;
        }

        public void SetIfSuccessfulOrDefault(TryGet<string> item)
        {
            if (!item.Succeeded)
            {
                this.Unset();
                return;
            }
            this.EDID = new RecordType(item.Value);
        }

        public static IEDIDLink<T> FactoryFromCache(RecordType edidRecordType, RecordType targetRecordType, BinaryWrapperFactoryPackage package)
        {
            var ret = new EDIDLink<T>(edidRecordType);
            TryLinkToCache(ret, package.EdidLinkCache[targetRecordType]);
            return ret;
        }

        public static bool TryLinkToCache(
            IEDIDLink<T> link,
            IReadOnlyDictionary<RecordType, object> cache)
        {
            if (link.EDID == UNLINKED) return false;
            if (cache.TryGetValue(link.EDID, out var rec))
            {
                link.Item = (T)rec;
                return true;
            }
            link.Item = default;
            return false;
        }

        private static bool TryLinkToMod(
            IEDIDLink<T> link,
            IModGetter mod)
        {
            if (link.EDID == UNLINKED) return false;
            // ToDo
            // Improve to not be a forloop
            var group = mod.GetGroupGetter<T>();
            foreach (var rec in group.Values)
            {
                if (link.EDID.Type.Equals(rec.EditorID))
                {
                    link.Item = rec;
                    return true;
                }
            }
            link.Item = default;
            return false;
        }

        public static bool TryLink<M>(
            IEDIDLink<T> link,
            LinkingPackage<M> package)
            where M : IMod
        {
#if DEBUG
            link.AttemptedLink = true;
#endif
            if (package.SourceMod != null && TryLinkToMod(link, package.SourceMod)) return true;
            if (package.ModList == null) return false;
            foreach (var listing in package.ModList)
            {
                if (!listing.Loaded) return false;
                if (TryLinkToMod(link, listing.Mod)) return true;
            }
            return false;
        }

        public override bool Link<M>(LinkingPackage<M> package)
        {
            if (this.UnlinkedForm.HasValue)
            {
                return base.Link(package);
            }
            return TryLink(this, package);
        }
    }
}
