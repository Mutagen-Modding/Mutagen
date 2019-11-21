using Noggog;
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
        Type TargetType { get; }
        bool TryResolveFormKey<M>(LinkingPackage<M> package, out FormKey formKey) where M : IModGetter;
        bool TryResolve<M>(LinkingPackage<M> package, out IMajorRecordCommonGetter formKey) where M : IModGetter;
    }

    public interface ILinkGetter<out TMajor> : ILinkGetter
        where TMajor : IMajorRecordCommonGetter
    {
        TMajor Resolve<M>(LinkingPackage<M> package) where M : IModGetter;
    }

    public interface ISetLinkGetter : ILinkGetter
    {
        bool HasBeenSet { get; }
    }

    public interface ISetLinkGetter<out Major> : ILinkGetter<Major>, ISetLinkGetter
        where Major : IMajorRecordCommonGetter
    {
    }

    public interface ISetLink<T> : ISetLinkGetter<T>
        where T : IMajorRecordCommonGetter
    {
        new bool HasBeenSet { get; set; }
    }

    public static class ILinkExt
    {
        public static void SetToFormKey<T, R>(this IFormIDSetLink<T> link, IFormIDSetLinkGetter<R> rhs)
            where R : IMajorRecordCommonGetter
            where T : R
        {
            if (rhs.HasBeenSet)
            {
                link.FormKey = rhs.FormKey;
            }
            else
            {
                link.Unset();
            }
        }
    }
}
