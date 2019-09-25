using Loqui;
using Mutagen.Bethesda.Tests;
using Noggog;
using Noggog.Notifying;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class FormIDSetLink<T> : LoquiNotifyingObject, IFormIDSetLink<T>, IEquatable<ILink<T>>
       where T : class, IMajorRecordCommonGetter
    {
        public static readonly IFormIDSetLinkGetter<T> Empty = new FormIDSetLink<T>();

        private bool _HasBeenSet;
        public bool HasBeenSet
        {
            get => this._HasBeenSet;
            set => this.RaiseAndSetIfChanged(ref this._HasBeenSet, value, nameof(HasBeenSet));
        }
        private T _Item;
        public T Item
        {
            get => this._Item;
            set => this.Set(value, true);
        }
        public bool Linked => this.HasBeenSet && this.Item != null;
        public FormKey? UnlinkedForm { get; private set; }
        public FormKey FormKey => LinkExt.GetFormKey(this);
        Type ILinkGetter.TargetType => typeof(T);
        FormKey ILink.FormKey
        {
            get => this.FormKey;
            set => SetUnlinkedForm(value);
        }
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

        public void SetUnlinkedForm(FormKey form)
        {
            this.UnlinkedForm = form;
            this.Item = default;
        }

        public virtual void Set(T value, bool hasBeenSet)
        {
            UpdateUnlinkedForm(value);
            this.RaiseAndSetIfChanged(ref this._Item, value, ref this._HasBeenSet, hasBeenSet, nameof(Item), nameof(HasBeenSet));
        }

        public void SetLink(ILink<T> value)
        {
            this.Set(value.Item);
        }

        public void Set(T value)
        {
            this.Set(value, true);
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

        public void Unset()
        {
            this.Set(default(T), hasBeenSet: false);
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

        public void SetLink(ISetLink<T> rhs, ISetLink<T> def)
        {
            if (rhs.HasBeenSet)
            {
                this.UnlinkedForm = rhs.FormKey;
                this.RaiseAndSetIfChanged(ref this._Item, rhs.Item, ref this._HasBeenSet, true, nameof(Item), nameof(HasBeenSet));
            }
            else if (def.HasBeenSet)
            {
                this.UnlinkedForm = def.FormKey;
                this.RaiseAndSetIfChanged(ref this._Item, def.Item, ref this._HasBeenSet, true, nameof(Item), nameof(HasBeenSet));
            }
            else
            {
                this.Unset();
            }
        }

        public void Set(ILink<T> link)
        {
            if (link.Linked)
            {
                this.Set(link.Item);
            }
            else
            {
                this.UnlinkedForm = link.FormKey;
            }
        }

        public void Set<R>(ILink<R> link)
            where R : T
        {
            if (link.Linked)
            {
                this.Set(link.Item);
            }
            else
            {
                this.UnlinkedForm = link.FormKey;
            }
        }

        public virtual bool Link<M>(
            ModList<M> modList,
            M sourceMod)
            where M : IMod
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
            this.Set(item);
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
