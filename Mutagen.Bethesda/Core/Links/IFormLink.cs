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
    }

    public interface IFormLink<TMajor> : IFormLinkGetter<TMajor>, IFormLink
       where TMajor : IMajorRecordCommonGetter
    {
    }

    public interface IFormLinkNullableGetter : ILinkGetter
    {
        FormKey? FormKey { get; }
    }

    public interface IFormLinkNullableGetter<out TMajor> : ILinkGetter<TMajor>, IFormLinkNullableGetter
       where TMajor : IMajorRecordCommonGetter
    {
    }

    public interface IFormLinkNullable : IFormLinkNullableGetter
    {
        new FormKey? FormKey { get; set; }
    }

    public interface IFormLinkNullable<TMajor> : IFormLinkNullableGetter<TMajor>, IFormLinkNullable
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
