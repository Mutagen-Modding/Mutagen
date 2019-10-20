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
    public class FormIDLink<T> : IFormIDLink<T>, IEquatable<ILink<T>>
       where T : class, IMajorRecordCommonGetter
    {
        public static readonly IFormIDLinkGetter<T> Empty = new FormIDLink<T>();

        private T _Item;
        public T Item
        {
            get => this._Item;
            set => this.Set(value);
        }
        public bool Linked => this.Item != null;
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

        public FormIDLink()
        {
#if DEBUG
            FormIDLinkTesterHelper.Add(this);
#endif
        }

        public FormIDLink(FormKey formKey)
        {
            this.UnlinkedForm = formKey;
#if DEBUG
            FormIDLinkTesterHelper.Add(this);
#endif
        }

        private void UpdateUnlinkedForm(T change)
        {
            this.UnlinkedForm = change?.FormKey ?? UnlinkedForm;
        }

        public void SetUnlinkedForm(FormKey form)
        {
            this.UnlinkedForm = form;
            this.Item = default;
        }

        public virtual void Set(T value)
        {
            UpdateUnlinkedForm(value);
            this._Item = value;
        }

        public void SetIfSucceeded(TryGet<FormKey> formKey)
        {
            if (formKey.Failed) return;
            this.Set(formKey.Value);
        }

        public void SetLink(ILinkGetter<T> value)
        {
            this.Set(value.Item);
        }

        public void Set(FormKey id)
        {
            this.UnlinkedForm = id;
        }

        public void SetIfSucceededOrDefault(TryGet<FormKey> formKey)
        {
            if (formKey.Failed)
            {
                this.Unset();            }
            else
            {
                this.UnlinkedForm = formKey.Value;
            }
        }

        public void Set(ILinkGetter<T> link)
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

        public void Set<R>(ILinkGetter<R> link)
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

        public static bool TryGetLink<M>(
            FormKey? unlinkedForm,
            LinkingPackage<M> package,
            out T item)
            where M : IMod
        {
            if (!unlinkedForm.HasValue
                || unlinkedForm.Equals(FormKey.NULL))
            {
                item = default(T);
                return false;
            }
            if (!package.TryGetMajorRecords(unlinkedForm.Value.ModKey, out var majorRecs))
            {
                item = default;
                return false;
            }
            if (!majorRecs.TryGetValue(unlinkedForm.Value, out var rec))
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

        public virtual bool Link<M>(LinkingPackage<M> package)
            where M : IMod
        {
#if DEBUG
            this.AttemptedLink = true;
#endif
            if (!TryGetLink(
                this.UnlinkedForm,
                package,
                out var item))
            {
                this.Item = default;
                return false;
            }
            this.Set(item);
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

        public virtual void Unset()
        {
            this.Set(default(T));
        }
    }
}
