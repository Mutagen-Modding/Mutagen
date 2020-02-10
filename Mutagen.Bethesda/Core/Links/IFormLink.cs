using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public interface IFormLinkGetter : ILinkGetter
    {
        FormKey FormKey { get; }
    }

    public interface IFormLinkGetter<out TMajor> : ILinkGetter<TMajor>, IFormLinkGetter
       where TMajor : IMajorRecordCommonGetter
    {
    }

    public interface IFormLink : IFormLinkGetter
    {
        new FormKey FormKey { get; set; }
        void Unset();
    }

    public interface IFormLink<TMajor> : IFormLinkGetter<TMajor>, IFormLink
       where TMajor : IMajorRecordCommonGetter
    {
    }

    public interface IFormSetLinkGetter<out TMajor> : IFormLinkGetter<TMajor>, ISetLinkGetter<TMajor>
       where TMajor : IMajorRecordCommonGetter
    {
    }

    public interface IFormSetLink<TMajor> : IFormLink<TMajor>, ISetLink<TMajor>, IFormSetLinkGetter<TMajor>
       where TMajor : IMajorRecordCommonGetter
    {
    }

    public static class IFormLinkExt
    {
        public static bool TryResolve<TMajor, TMod>(this IFormLinkGetter<TMajor> formIDLink, ILinkCache<TMod> package, out TMajor item)
            where TMajor : IMajorRecordCommonGetter
            where TMod : IMod
        {
            item = formIDLink.Resolve(package);
            return item != null;
        }
    }
}
