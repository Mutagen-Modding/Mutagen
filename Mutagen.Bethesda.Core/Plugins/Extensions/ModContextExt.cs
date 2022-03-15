using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Cache.Internals;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda
{
    public static class ModContextExt
    {
        public static bool IsUnderneath<T>(this IModContext context)
        {
            return TryGetParent<T>(context, out _);
        }

        public static bool TryGetParent<T>(this IModContext context, [MaybeNullWhen(false)] out T item)
        {
            var targetContext = context.Parent;
            while (targetContext != null)
            {
                if (targetContext.Record is T t)
                {
                    item = t;
                    return true;
                }
                targetContext = targetContext.Parent;
            }
            item = default;
            return false;
        }

        public static IModContext<TMod, TModGetter, RMajorSetter, RMajorGetter> AsType<TMod, TModGetter, TMajor, TMajorGetter, RMajorSetter, RMajorGetter>(this IModContext<TMod, TModGetter, TMajor, TMajorGetter> context)
            where TModGetter : IModGetter
            where TMod : TModGetter, IMod
            where TMajor : IMajorRecordQueryable, TMajorGetter
            where TMajorGetter : IMajorRecordQueryableGetter
            where RMajorSetter : TMajor, RMajorGetter
            where RMajorGetter : TMajorGetter
        {
            return new ModContextCaster<TMod, TModGetter, TMajor, TMajorGetter, RMajorSetter, RMajorGetter>(context);
        }

        public static IModContext<RMajorGetter> AsType<TMajorGetter, RMajorGetter>(this IModContext<TMajorGetter> context)
            where TMajorGetter : IMajorRecordQueryableGetter
            where RMajorGetter : TMajorGetter
        {
            return new SimpleModContextCaster<TMajorGetter, RMajorGetter>(context);
        }
    }
}
