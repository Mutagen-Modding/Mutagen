using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public interface ILinkGetter
    {
        bool Linked { get; }
        FormKey FormKey { get; }
        Type TargetType { get; }
#if DEBUG
        bool AttemptedLink { get; }
#endif
    }

    public interface ILink : ILinkGetter
    {
        new FormKey FormKey { get; set; }
        bool Link<M>(LinkingPackage<M> package) where M : IMod;
#if DEBUG
        new bool AttemptedLink { get; set; }
#endif
    }

    public interface ILinkGetter<out T> : ILinkGetter
        where T : IMajorRecordCommonGetter
    {
        T Item { get; }
    }

    public interface ILink<T> : ILink, ILinkGetter<T>
        where T : IMajorRecordCommonGetter
    {
        new T Item { get; set; }
        void Set(FormKey form);
        void Set(ILinkGetter<T> link);
        void Set<R>(ILinkGetter<R> link) where R : T;
        void SetLink(ILinkGetter<T> value);
        void Unset();
    }

    public interface ISetLinkGetter<out T> : ILinkGetter<T>
        where T : IMajorRecordCommonGetter
    {
        bool HasBeenSet { get; }
    }

    public interface ISetLink<T> : ILink<T>, ISetLinkGetter<T>
        where T : IMajorRecordCommonGetter
    {
        new bool HasBeenSet { get; set; }
        void SetLink(ISetLinkGetter<T> rhs, ISetLinkGetter<T> def);
    }
}
