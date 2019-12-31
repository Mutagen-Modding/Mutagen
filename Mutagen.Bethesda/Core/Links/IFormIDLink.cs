using Loqui;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public interface IFormIDLinkGetter : ILinkGetter
    {
        FormKey FormKey { get; }
    }

    public interface IFormIDLinkGetter<out TMajor> : ILinkGetter<TMajor>, IFormIDLinkGetter
       where TMajor : IMajorRecordCommonGetter
    {
    }

    public interface IFormIDLink : IFormIDLinkGetter
    {
        new FormKey FormKey { get; set; }
        void Unset();
    }

    public interface IFormIDLink<TMajor> : IFormIDLinkGetter<TMajor>, IFormIDLink
       where TMajor : IMajorRecordCommonGetter
    {
    }

    public interface IFormIDSetLinkGetter<out TMajor> : IFormIDLinkGetter<TMajor>, ISetLinkGetter<TMajor>
       where TMajor : IMajorRecordCommonGetter
    {
    }

    public interface IFormIDSetLink<TMajor> : IFormIDLink<TMajor>, ISetLink<TMajor>, IFormIDSetLinkGetter<TMajor>
       where TMajor : IMajorRecordCommonGetter
    {
    }

    public static class IFormLinkExt
    {
        public static bool TryResolve<TMajor, TMod>(this IFormIDLinkGetter<TMajor> formIDLink, ILinkingPackage<TMod> package, out TMajor item)
            where TMajor : IMajorRecordCommonGetter
            where TMod : IMod
        {
            item = formIDLink.Resolve(package);
            return item != null;
        }
    }
}
