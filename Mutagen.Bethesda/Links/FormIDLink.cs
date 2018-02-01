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
            this.Subscribe(UpdateUnlinkedForm);
        }

        public FormIDLink(FormID unlinkedForm)
        {
            this.UnlinkedForm = unlinkedForm;
            this.Subscribe(UpdateUnlinkedForm);
        }

        private void UpdateUnlinkedForm(Change<T> change)
        {
            this.UnlinkedForm = change.New?.FormID ?? UnlinkedForm;
        }

        public void SetIfSucceeded(TryGet<FormID> formID)
        {
            if (formID.Failed) return;
            this.UnlinkedForm = formID.Value;
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
