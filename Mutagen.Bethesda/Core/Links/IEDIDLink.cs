using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public interface IEDIDLinkGetter : ILinkGetter
    {
        RecordType EDID { get; }
    }

    public interface IEDIDLinkGetter<out TMajor> : ILinkGetter<TMajor>, IEDIDLinkGetter
       where TMajor : IMajorRecordCommonGetter
    {
    }

    public interface IEDIDLink<T> : IEDIDLinkGetter<T>, IEDIDLinkGetter
       where T : IMajorRecordCommonGetter
    {
        new RecordType EDID { get; set; }
        void Unset();
    }

    public interface IEDIDSetLinkGetter<out TMajor> : IEDIDLinkGetter<TMajor>, ISetLinkGetter<TMajor>
       where TMajor : IMajorRecordCommonGetter
    {
    }

    public interface IEDIDSetLink<TMajor> : IEDIDLink<TMajor>, ISetLink<TMajor>, IEDIDSetLinkGetter<TMajor>
       where TMajor : IMajorRecordCommonGetter
    {
    }

    public static class IEDIDLinkExt
    {
        public static bool TryResolve<TMajor, TMod>(this IEDIDLinkGetter<TMajor> edidLink, LinkingPackage<TMod> package, out TMajor item)
            where TMajor : IMajorRecordCommonGetter
            where TMod : IMod
        {
            item = edidLink.Resolve(package);
            return item != null;
        }
    }
}
