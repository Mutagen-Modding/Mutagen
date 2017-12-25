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
    public class FormIDLink<T> : NotifyingSetItem<T>, ILink<T>, IEquatable<FormIDLink<T>>
       where T : MajorRecord
    {
        public bool Linked => this.HasBeenSet && this.Item != null;
        public RawFormID? UnlinkedForm { get; private set; }
        public RawFormID FormID
        {
            get
            {
                RawFormID? ret = this.Item?.FormID ?? UnlinkedForm;
                return ret ?? RawFormID.NULL;
            }
        }

        public FormIDLink()
        {
            this.Subscribe(UpdateUnlinkedForm);
        }

        public FormIDLink(RawFormID unlinkedForm)
        {
            this.UnlinkedForm = unlinkedForm;
            this.Subscribe(UpdateUnlinkedForm);
        }

        public void Link<Mod>(
            ModList<Mod> modList,
            INotifyingListGetter<MasterReference> masterList)
            where Mod : IMod
        {
            if (!masterList.InRange(this.FormID.ModID.ID)) return;
            var master = masterList[this.FormID.ModID.ID];
            if (!modList.TryGetMod(this.UnlinkedForm.Value.ModID, out var result)) return;
            if (!result.TryGetRecord<T>(this.UnlinkedForm.Value.ID, out var record)) return;
            this.Item = record;
        }

        private void UpdateUnlinkedForm(Change<T> change)
        {
            this.UnlinkedForm = change.New?.FormID ?? UnlinkedForm;
        }

        public void SetIfSucceeded(TryGet<RawFormID> formID)
        {
            if (formID.Failed) return;
            this.UnlinkedForm = formID.Value;
        }

        public static bool operator ==(FormIDLink<T> lhs, FormIDLink<T> rhs)
        {
            return lhs.FormID.Equals(rhs.FormID);
        }

        public static bool operator !=(FormIDLink<T> lhs, FormIDLink<T> rhs)
        {
            return !lhs.FormID.Equals(rhs.FormID);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FormIDLink<T> rhs)) return false;
            return this.Equals(rhs);
        }

        public bool Equals(FormIDLink<T> other)
        {
            return this.FormID.Equals(other.FormID);
        }

        public override int GetHashCode()
        {
            return this.FormID.GetHashCode();
        }

        public override string ToString()
        {
            return $"{(this.Linked ? this.Item.EditorID : "UNLINKED")} [{typeof(T).Name}] ({this.FormID})";
        }

        public void ToString(FileGeneration fg, string name)
        {
            fg.AppendLine($"{name} [{typeof(T).Name}] ({this.FormID})");
        }
    }
}
