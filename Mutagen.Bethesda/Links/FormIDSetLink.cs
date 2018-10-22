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
       where T : MajorRecord
    {
        public bool Linked => this.HasBeenSet && this.Item != null;
        public FormKey? UnlinkedForm { get; private set; }
        public FormKey FormKey => LinkExt.GetFormKey(this);
        Type ILink.TargetType => typeof(T);
#if DEBUG
        public bool AttemptedLink { get; set; }
#endif

        public FormIDSetLink()
        {
#if DEBUG
            FormIDLinkTesterHelper.Add(this);
#endif
        }

        public FormIDSetLink(FormKey unlinkedForm)
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
            this.UnlinkedForm = change?.FormKey ?? UnlinkedForm;
            this._HasBeenSet = this.UnlinkedForm.HasValue;
        }

        public override void Set(T value, bool hasBeenSet, NotifyingFireParameters cmds = null)
        {
            UpdateUnlinkedForm(value);
            base.Set(value, hasBeenSet, cmds);
        }

        public void SetIfSucceeded(TryGet<FormKey> formKey)
        {
            if (formKey.Failed)
            {
                return;
            }
            this.UnlinkedForm = formKey.Value;
            this.HasBeenSet = true;
        }

        public void SetIfSucceededOrDefault(TryGet<FormKey> formKey)
        {
            if (formKey.Failed)
            {
                this.Unset();
                return;
            }
            this.Set(formKey.Value);
        }

        public void Set(FormKey formKey)
        {
            this.UnlinkedForm = formKey;
            this.HasBeenSet = true;
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
            if (!this.HasBeenSet) return false;
            if (!FormIDLink<T>.TryGetLink(
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

        public static bool operator ==(FormIDSetLink<T> lhs, IFormIDLink<T> rhs)
        {
            return lhs.FormKey.Equals(rhs.FormKey);
        }

        public static bool operator !=(FormIDSetLink<T> lhs, IFormIDLink<T> rhs)
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
