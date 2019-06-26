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
        bool Link<M>(
            ModList<M> modList,
            M sourceMod)
            where M : IMod;
#if DEBUG
        new bool AttemptedLink { get; set; }
#endif
    }

    public interface ILinkGetter<out T> : ILinkGetter
        where T : IMajorRecordInternalGetter
    {
        T Item { get; }
    }

    public interface ILink<T> : ILink, ILinkGetter<T>
        where T : IMajorRecordInternalGetter
    {
        new T Item { get; set; }
        void Set(FormKey form);
        void Set(ILink<T> link);
        void Set<R>(ILink<R> link) where R : T;
        void SetLink(ILink<T> value);
        void Unset();
    }

    public interface ISetLinkGetter<out T> : ILinkGetter<T>
        where T : IMajorRecordInternalGetter
    {
        bool HasBeenSet { get; }
    }

    public interface ISetLink<T> : ILink<T>, ISetLinkGetter<T>
        where T : IMajorRecordInternalGetter
    {
        new bool HasBeenSet { get; set; }
        void SetLink(ISetLink<T> rhs, ISetLink<T> def);
    }
}
