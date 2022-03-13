using System;
using System.Collections.Generic;
using System.Linq;
using Loqui;
using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda.Plugins.Records.Internals;

public static class InterfaceEnumerationHelper
{
    public static IEnumerable<IMajorRecordGetter> EnumerateMajorRecordsFor<T>(
        GameCategory category,
        T obj, 
        Type linkInterface,
        bool setter)
        where T : IMajorRecordGetterEnumerable, ILoquiObjectGetter
    {
        if (setter && !obj.Registration.SetterType.IsAssignableFrom(obj.GetType())) return Enumerable.Empty<IMajorRecordGetter>();
        var mapping = LinkInterfaceMapping.Instance.InterfaceToObjectTypes(category);
        if (!mapping.TryGetValue(linkInterface, out var inheritingTypes)) return Enumerable.Empty<IMajorRecordGetter>();
        return inheritingTypes.SelectMany(t => obj.EnumerateMajorRecords(setter ? t.SetterType : t.GetterType));
    }
    
    public static IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>> EnumerateMajorRecordContextsFor<TObj, TMod, TModGetter, TMajor, TMajorGetter>(
        GameCategory category,
        TObj obj, 
        Type linkInterface,
        ILinkCache linkCache)
        where TObj : IMajorRecordContextEnumerable<TMod, TModGetter>, ILoquiObjectGetter
        where TModGetter : IModGetter
        where TMod : TModGetter, IMod
        where TMajor : class, IMajorRecord, TMajorGetter
        where TMajorGetter : class, IMajorRecordGetter
    {
        var mapping = LinkInterfaceMapping.Instance.InterfaceToObjectTypes(category);
        if (!mapping.TryGetValue(linkInterface, out var inheritingTypes)) return Enumerable.Empty<IModContext<TMod, TModGetter, TMajor, TMajorGetter>>();
        return inheritingTypes.SelectMany(t => obj.EnumerateMajorRecordContexts(linkCache, t.GetterType))
            .Select(m => m.AsType<TMod, TModGetter, IMajorRecord, IMajorRecordGetter, TMajor, TMajorGetter>());
    }
}