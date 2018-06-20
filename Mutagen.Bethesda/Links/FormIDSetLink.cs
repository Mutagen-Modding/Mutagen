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
    public class FormIDSetLink<T> : NotifyingSetItem<T>, IFormIDSetLink<T>, IEquatable<ILink<T>>
       where T : MajorRecord
    {
        public bool Linked => this.HasBeenSet && this.Item != null;
        public FormID? UnlinkedForm { get; private set; }
        public FormID FormID => LinkExt.GetFormID(this);

        public FormIDSetLink()
        {
            this.Subscribe(UpdateUnlinkedForm);
        }

        public FormIDSetLink(FormID unlinkedForm)
            : base(markAsSet: true)
        {
            this.UnlinkedForm = unlinkedForm;
            this.HasBeenSet = true;
            this.Subscribe(UpdateUnlinkedForm);
        }

        private void UpdateUnlinkedForm(Change<T> change)
        {
            this.UnlinkedForm = change.New?.FormID ?? UnlinkedForm;
            this.HasBeenSet = this.UnlinkedForm.HasValue;
        }

        public void SetIfSucceeded(TryGet<FormID> formID)
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

        public virtual bool Link<M>(
            ModList<M> modList,
            M sourceMod,
            NotifyingFireParameters cmds = null)
            where M : IMod
        {
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
