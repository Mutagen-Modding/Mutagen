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
        bool TryResolveFormKey<M>(ILinkingPackage<M> package, out FormKey formKey) where M : IModGetter;
        bool TryResolveCommon<M>(ILinkingPackage<M> package, out IMajorRecordCommonGetter majorRecord) where M : IModGetter;
    }

    public interface ILinkGetter<out TMajor> : ILinkGetter
        where TMajor : IMajorRecordCommonGetter
    {
        ITryGetter<TMajor> TryResolve<TMod>(ILinkingPackage<TMod> package) where TMod : IModGetter;
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

        public static bool TryResolve<TMod, TMajor>(this ILinkGetter<TMajor> link, ILinkingPackage<TMod> package, out TMajor majorRecord)
            where TMod : IModGetter
            where TMajor : IMajorRecordCommonGetter
        {
            var ret = link.TryResolve<TMod>(package);
            if (ret.Succeeded)
            {
                majorRecord = ret.Value;
                return true;
            }
            majorRecord = default;
            return false;
        }

        public static TMajor Resolve<TMod, TMajor>(this ILinkGetter<TMajor> link, ILinkingPackage<TMod> package)
            where TMod : IModGetter
            where TMajor : IMajorRecordCommonGetter
        {
            return link.TryResolve<TMod>(package).Value;
        }
    }
}
