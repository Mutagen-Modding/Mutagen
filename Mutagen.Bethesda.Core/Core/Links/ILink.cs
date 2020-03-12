using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public interface ILinkGetter
    {
        Type TargetType { get; }
        bool TryGetModKey([MaybeNullWhen(false)] out ModKey modKey);
        bool TryResolveFormKey<M>(ILinkCache<M> package, out FormKey formKey) where M : IModGetter;
        bool TryResolveCommon<M>(ILinkCache<M> package, out IMajorRecordCommonGetter majorRecord) where M : IModGetter;
    }

    public interface ILinkGetter<out TMajor> : ILinkGetter
        where TMajor : IMajorRecordCommonGetter
    {
        ITryGetter<TMajor> TryResolve<TMod>(ILinkCache<TMod> package) where TMod : IModGetter;
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
        public static bool TryResolve<TMod, TMajor>(this ILinkGetter<TMajor> link, ILinkCache<TMod> package, out TMajor majorRecord)
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

        public static TMajor Resolve<TMod, TMajor>(this ILinkGetter<TMajor> link, ILinkCache<TMod> package)
            where TMod : IModGetter
            where TMajor : IMajorRecordCommonGetter
        {
            return link.TryResolve<TMod>(package).Value;
        }
    }
}
