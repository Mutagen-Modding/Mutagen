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
    public class FormIDSetLink<T> : NotifyingSetItem<T>, IFormIDSetLink<T>, IEquatable<ILink<T>>
       where T : IMajorRecord
    {
        public bool Linked => this.HasBeenSet && this.Item != null;
        public FormID? UnlinkedForm { get; private set; }
        public FormID FormID => LinkExt.GetFormID(this);
#if DEBUG
        public bool AttemptedLink { get; set; }
#endif

        public FormIDSetLink()
        {
#if DEBUG
            FormIDLinkTesterHelper.Add(this);
#endif
        }

        public FormIDSetLink(FormID unlinkedForm)
            : base(markAsSet: true)
        {
            this.UnlinkedForm = unlinkedForm;
            this._HasBeenSet = true;
#if DEBUG
            FormIDLinkTesterHelper.Add(this);
#endif
        }

        private void UpdateUnlinkedForm(T change)
        {
            this.UnlinkedForm = change?.FormID ?? UnlinkedForm;
            this._HasBeenSet = this.UnlinkedForm.HasValue;
        }

        public override void Set(T value, bool hasBeenSet, NotifyingFireParameters cmds = null)
        {
            UpdateUnlinkedForm(value);
            base.Set(value, hasBeenSet, cmds);
        }

        public void SetIfSucceeded(TryGet<FormID> formID)
        {
            if (formID.Failed)
            {
                return;
            }
            this.UnlinkedForm = formID.Value;
            this.HasBeenSet = true;
        }

        public void SetIfSucceededOrDefault(TryGet<FormID> formID)
        {
            if (formID.Failed)
            {
                this.Unset();
                return;
            }
            this.Set(formID.Value);
        }

        public void Set(FormID formID)
        {
            this.UnlinkedForm = formID;
            this.HasBeenSet = true;
        }

        public void Set(ILink<T> link, NotifyingFireParameters cmds = null)
        {
            if (link.Linked)
            {
                this.Set(link.Item, cmds);
            }
            else
            {
                this.UnlinkedForm = link.FormID;
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
                this.UnlinkedForm = link.FormID;
            }
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
            if (!FormIDLink<T>.TryGetLink(
                this.UnlinkedForm,
                modList,
                sourceMod,
                out var item))
            {
                this.Unset(cmds.ToUnsetParams());
                return false;
            }
            this.Set(item, cmds);
            return true;
        }

        public static bool operator ==(FormIDSetLink<T> lhs, IFormIDLink<T> rhs)
        {
            return lhs.FormID.Equals(rhs.FormID);
        }

        public static bool operator !=(FormIDSetLink<T> lhs, IFormIDLink<T> rhs)
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
