using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda;

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

    public static IModContext<TMod, TModGetter, TRhsMajorSetter, TRhsMajorGetter> AsType<TMod, TModGetter, TMajor, TMajorGetter, TRhsMajorSetter, TRhsMajorGetter>(this IModContext<TMod, TModGetter, TMajor, TMajorGetter> context)
        where TModGetter : IModGetter
        where TMod : TModGetter, IMod
        where TMajor : IMajorRecordQueryable, TMajorGetter
        where TMajorGetter : IMajorRecordQueryableGetter
        where TRhsMajorSetter : TMajor, TRhsMajorGetter
        where TRhsMajorGetter : TMajorGetter
    {
        return new ModContextCaster<TMod, TModGetter, TMajor, TMajorGetter, TRhsMajorSetter, TRhsMajorGetter>(context);
    }

    public static IModContext<TRhsMajorGetter> AsType<TMajorGetter, TRhsMajorGetter>(this IModContext<TMajorGetter> context)
        where TMajorGetter : IMajorRecordQueryableGetter
        where TRhsMajorGetter : TMajorGetter
    {
        return new SimpleModContextCaster<TMajorGetter, TRhsMajorGetter>(context);
    }
}