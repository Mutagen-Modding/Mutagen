using Loqui;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Cache.Internals;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

internal static class InterfaceEnumerationHelper
{
    public static bool TryEnumerateLinkRecordsFor<T>(
        GameCategory category,
        T obj, 
        Type linkInterface,
        out IEnumerable<IMajorRecordGetter> interfaces)
        where T : IMajorRecordGetterEnumerable, ILoquiObjectGetter
    {
        var mapping = LinkInterfaceMapping.Instance.InterfaceToObjectTypes(category);
        if (!mapping.TryGetValue(linkInterface, out var inheritingTypes))
        {
            interfaces = Enumerable.Empty<IMajorRecordGetter>();
            return false;
        }

        if (inheritingTypes.Setter && !obj.Registration.SetterType.IsAssignableFrom(obj.GetType()))
        {
            interfaces = Enumerable.Empty<IMajorRecordGetter>();
            return true;
        }
        
        interfaces = inheritingTypes.Registrations.SelectMany(t => obj.EnumerateMajorRecords(inheritingTypes.Setter ? t.SetterType : t.GetterType));
        return true;
    }
    
    public static bool TryEnumerateAspectRecordsFor<T>(
        GameCategory category,
        T obj, 
        Type linkInterface,
        out IEnumerable<IMajorRecordGetter> interfaces)
        where T : IMajorRecordGetterEnumerable, ILoquiObjectGetter
    {
        var mapping = AspectInterfaceMapping.Instance.InterfaceToObjectTypes(category);
        if (!mapping.TryGetValue(linkInterface, out var inheritingTypes))
        {
            interfaces = Enumerable.Empty<IMajorRecordGetter>();
            return false;
        }

        if (inheritingTypes.Setter && !obj.Registration.SetterType.IsAssignableFrom(obj.GetType()))
        {
            interfaces = Enumerable.Empty<IMajorRecordGetter>();
            return true;
        }
        
        interfaces = inheritingTypes.Registrations.SelectMany(t => obj.EnumerateMajorRecords(inheritingTypes.Setter ? t.SetterType : t.GetterType, throwIfUnknown: false));
        return true;
    }
    
    public static bool TryEnumerateInterfaceRecordsFor<T>(
        GameCategory category,
        T obj, 
        Type linkInterface,
        out IEnumerable<IMajorRecordGetter> interfaces)
        where T : IMajorRecordGetterEnumerable, ILoquiObjectGetter
    {
        if (!MetaInterfaceMapping.Instance.TryGetRegistrationsForInterface(category, linkInterface, out var inheritingTypes))
        {
            interfaces = Enumerable.Empty<IMajorRecordGetter>();
            return false;
        }

        if (inheritingTypes.Setter && !obj.Registration.SetterType.IsAssignableFrom(obj.GetType()))
        {
            interfaces = Enumerable.Empty<IMajorRecordGetter>();
            return true;
        }
        
        interfaces = inheritingTypes.Registrations.SelectMany(t => obj.EnumerateMajorRecords(inheritingTypes.Setter ? t.SetterType : t.GetterType, throwIfUnknown: false));
        return true;
    }
    
    public static bool TryEnumerateLinkContextsFor<T, TMod, TModGetter>(
        GameCategory category,
        T obj, 
        Type linkInterface,
        ILinkCache linkCache,
        out IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> interfaces)
        where T : IMajorRecordContextEnumerable<TMod, TModGetter>, ILoquiObjectGetter
        where TModGetter : IModGetter
        where TMod : TModGetter, IMod
    {
        var mapping = LinkInterfaceMapping.Instance.InterfaceToObjectTypes(category);
        if (!mapping.TryGetValue(linkInterface, out var inheritingTypes))
        {
            interfaces = Enumerable.Empty<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>>();
            return false;
        }
    
        if (inheritingTypes.Setter && !obj.Registration.SetterType.IsAssignableFrom(obj.GetType()))
        {
            interfaces = Enumerable.Empty<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>>();
            return true;
        }
        
        interfaces = inheritingTypes.Registrations
            .SelectMany(t => obj.EnumerateMajorRecordContexts(linkCache, inheritingTypes.Setter ? t.SetterType : t.GetterType));
        return true;
    }
    
    public static bool TryEnumerateAspectContextsFor<T, TMod, TModGetter>(
        GameCategory category,
        T obj, 
        Type linkInterface,
        ILinkCache linkCache,
        Func<T, IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>>> getEnumer,
        out IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> interfaces)
        where T : IMajorRecordContextEnumerable<TMod, TModGetter>, ILoquiObjectGetter
        where TModGetter : IModGetter
        where TMod : TModGetter, IMod
    {
        var mapping = AspectInterfaceMapping.Instance.InterfaceToObjectTypes(category);
        if (!mapping.TryGetValue(linkInterface, out var inheritingTypes))
        {
            interfaces = Enumerable.Empty<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>>();
            return false;
        }
    
        if (inheritingTypes.Setter && !obj.Registration.SetterType.IsAssignableFrom(obj.GetType()))
        {
            interfaces = Enumerable.Empty<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>>();
            return true;
        }
        
        interfaces = inheritingTypes.Registrations
            .SelectMany(t => obj.EnumerateMajorRecordContexts(linkCache, inheritingTypes.Setter ? t.SetterType : t.GetterType, throwIfUnknown: false));
        return true;
    }
    
    public delegate IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> ContextGetter<TMod, TModGetter>(
            ILinkCache linkCache,
            Type type,
            bool throwIfUnknown)
        where TModGetter : IModGetter
        where TMod : TModGetter, IMod;

    public static bool TryEnumerateInterfaceContextsFor<T, TMod, TModGetter>(
        GameCategory category,
        T obj, 
        Type linkInterface,
        ILinkCache linkCache,
        ContextGetter<TMod, TModGetter> getter,
        out IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> interfaces)
        where T : ILoquiObjectGetter
        where TModGetter : IModGetter
        where TMod : TModGetter, IMod
    {
        if (!MetaInterfaceMapping.Instance.TryGetRegistrationsForInterface(category, linkInterface, out var inheritingTypes))
        {
            interfaces = Enumerable.Empty<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>>();
            return false;
        }
    
        if (inheritingTypes.Setter && !obj.Registration.SetterType.IsAssignableFrom(obj.GetType()))
        {
            interfaces = Enumerable.Empty<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>>();
            return true;
        }
        
        interfaces = inheritingTypes.Registrations
            .SelectMany(t => getter(linkCache, inheritingTypes.Setter ? t.SetterType : t.GetterType, throwIfUnknown: false));
        return true;
    }

    public static bool TryEnumerateInterfaceContextsFor<T, TMod, TModGetter>(
        GameCategory category,
        T obj, 
        Type linkInterface,
        ILinkCache linkCache,
        out IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> interfaces)
        where T : IMajorRecordContextEnumerable<TMod, TModGetter>, ILoquiObjectGetter
        where TModGetter : IModGetter
        where TMod : TModGetter, IMod
    {
        return TryEnumerateInterfaceContextsFor<T, TMod, TModGetter>(category, obj, linkInterface, linkCache,
            obj.EnumerateMajorRecordContexts,
            out interfaces);
    }

    public static IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> EnumerateGroupContexts<TMod, TModGetter, TMajor, TMajorTarget>(
            IGroupGetter<TMajorTarget> srcGroup,
            Type type,
            ModKey modKey,
            Func<TMod, IGroup<TMajor>> group,
            Func<TModGetter, IGroupGetter<TMajorTarget>> groupGetter)
        where TModGetter : IModGetter
        where TMod : TModGetter, IMod
        where TMajor : class, IMajorRecordInternal, TMajorTarget
        where TMajorTarget : class, IMajorRecordGetter
    {
        if (!srcGroup.ContainedRecordType.InheritsFrom(type)) yield break;

        foreach (var item in srcGroup.Records)
        {
            yield return new GroupModContext<TMod, TModGetter, TMajor, TMajorTarget>(
                modKey: modKey,
                record: item,
                group: group,
                groupGetter: groupGetter);
        }
    }
}