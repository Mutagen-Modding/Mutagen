using Loqui;
using Mutagen.Bethesda.Tests;
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
       where T : IMajorRecord
    {
        public bool Linked => this.Item != null;
        public FormKey? UnlinkedForm { get; private set; }
        public FormKey FormKey => LinkExt.GetFormKey(this);
        Type ILink.TargetType => typeof(T);
#if DEBUG
        public bool AttemptedLink { get; set; }
#endif

        public FormIDLink()
        {
#if DEBUG
            FormIDLinkTesterHelper.Add(this);
#endif
        }

        public FormIDLink(FormKey unlinkedForm)
        {
            this.UnlinkedForm = unlinkedForm;
#if DEBUG
            FormIDLinkTesterHelper.Add(this);
#endif
        }

        private void UpdateUnlinkedForm(T change)
        {
            this.UnlinkedForm = change?.FormKey ?? UnlinkedForm;
        }

        public override void Set(T value, NotifyingFireParameters cmds = null)
        {
            UpdateUnlinkedForm(value);
            base.Set(value, cmds);
        }

        public void SetIfSucceeded(TryGet<FormKey> formKey)
        {
            if (formKey.Failed) return;
            this.Set(formKey.Value);
        }

        public void Set(FormKey id)
        {
            this.UnlinkedForm = id;
        }

        public void SetIfSucceededOrDefault(TryGet<FormKey> formKey)
        {
            if (formKey.Failed)
            {
                this.Unset();
                return;
            }
            this.UnlinkedForm = formKey.Value;
        }

        public void Set(ILink<T> link, NotifyingFireParameters cmds = null)
        {
            if (link.Linked)
            {
                this.Set(link.Item, cmds);
            }
            else
            {
                this.UnlinkedForm = link.FormKey;
            }
        }

        public void Set<R>(ILink<R> link, NotifyingFireParameters cmds = null)
            where R : T
        {
            if (link.Linked)
            {
                this.Set(link.Item, cmds);
            }
            else
            {
                this.UnlinkedForm = link.FormKey;
            }
        }

        public static bool TryGetLink<M>(
            FormKey? unlinkedForm,
            ModList<M> modList,
            M sourceMod,
            out T item)
            where M : IMod<M>
        {
            if (!unlinkedForm.HasValue
                || unlinkedForm.Equals(FormKey.NULL))
            {
                item = default(T);
                return false;
            }
            M mod;
            if (sourceMod.ModKey == unlinkedForm.Value.ModKey)
            {
                mod = sourceMod;
            }
            else if (modList != null)
            {
                if (!modList.TryGetMod(unlinkedForm.Value.ModKey, out var modListing))
                {
                    item = default(T);
                    return false;
                }
                mod = modListing.Mod;
            }
            else
            {
                item = default;
                return false;
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
#if DEBUG
            this.AttemptedLink = true;
#endif
            if (!TryGetLink(
                this.UnlinkedForm,
                modList,
                sourceMod,
                out var item))
            {
                this.Item = default;
                return false;
            }
            this.Set(item, cmds);
            return true;
        }

        public static bool operator ==(FormIDLink<T> lhs, IFormIDLink<T> rhs)
        {
            return lhs.FormKey.Equals(rhs.FormKey);
        }

        public static bool operator !=(FormIDLink<T> lhs, IFormIDLink<T> rhs)
        {
            return !lhs.FormKey.Equals(rhs.FormKey);
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
