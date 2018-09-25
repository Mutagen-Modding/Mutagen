using Loqui;
using Noggog;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class FormIDLink<T> : NotifyingItem<T>, IFormIDLink<T>, IEquatable<ILink<T>>
       where T : MajorRecord
    {
        public bool Linked => this.Item != null;
        public FormID? UnlinkedForm { get; private set; }
        public FormID FormID => LinkExt.GetFormID(this);

        public FormIDLink()
        {
        }

        public FormIDLink(FormID unlinkedForm)
        {
            this.UnlinkedForm = unlinkedForm;
        }

        private void UpdateUnlinkedForm(T change)
        {
            this.UnlinkedForm = change?.FormID ?? UnlinkedForm;
        }

        public override void Set(T value, NotifyingFireParameters cmds = null)
        {
            UpdateUnlinkedForm(value);
            base.Set(value, cmds);
        }

        public void SetIfSucceeded(TryGet<FormID> formID)
        {
            if (formID.Failed) return;
            this.Set(formID.Value);
        }

        public void Set(FormID id)
        {
            this.UnlinkedForm = id;
        }

        public void SetIfSucceededOrDefault(TryGet<FormID> formID)
        {
            if (formID.Failed)
            {
                this.Unset();
                return;
            }
            this.UnlinkedForm = formID.Value;
        }

        public static bool TryGetLink<M>(
            FormID? unlinkedForm,
            ModList<M> modList,
            M sourceMod,
            out T item)
            where M : IMod<M>
        {
            if (!unlinkedForm.HasValue)
            {
                item = default(T);
                return false;
            }
            M mod;
            if (modList != null && unlinkedForm.Value.ModID != ModID.Zero)
            {
                if (!sourceMod.MasterReferences.TryGet(unlinkedForm.Value.ModID.ID, out var masterRef)
                    || !ModKey.TryFactory(masterRef.Master, out var modKey)
                    || !modList.TryGetMod(modKey, out var modListing))
                {
                    item = default(T);
                    return false;
                }
                mod = modListing.Mod;
            }
            else
            {
                mod = sourceMod;
            }
            if (!mod.MajorRecords.TryGetValue(unlinkedForm.Value, out var rec))
            {
                item = default(T);
                return false;
            }
            if (rec is T t)
            {
                item = t;
                return true;
            }
            item = default(T);
            return false;
        }

        public virtual bool Link<M>(
            ModList<M> modList,
            M sourceMod,
            NotifyingFireParameters cmds = null)
            where M : IMod<M>
        {
            if (!TryGetLink(
                this.UnlinkedForm,
                modList,
                sourceMod,
                out var item))
            {
                this.Item = default(T);
                return false;
            }
            this.Set(item, cmds);
            return true;
        }

        public static bool operator ==(FormIDLink<T> lhs, IFormIDLink<T> rhs)
        {
            return lhs.FormID.Equals(rhs.FormID);
        }

        public static bool operator !=(FormIDLink<T> lhs, IFormIDLink<T> rhs)
        {
            return !lhs.FormID.Equals(rhs.FormID);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ILink<T> rhs)) return false;
            return this.Equals(rhs);
        }

        public bool Equals(ILink<T> other) => LinkExt.Equals(this, other);

        public override int GetHashCode() => LinkExt.HashCode(this);

        public override string ToString() => LinkExt.ToString(this);
    }
}
