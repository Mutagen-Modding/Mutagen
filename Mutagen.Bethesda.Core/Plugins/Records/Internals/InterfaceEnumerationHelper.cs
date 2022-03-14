using System;
using System.Collections.Generic;
using System.Linq;
using Loqui;

namespace Mutagen.Bethesda.Plugins.Records.Internals;

public static class InterfaceEnumerationHelper
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
        
        interfaces = inheritingTypes.Registrations.SelectMany(t => obj.EnumerateMajorRecords(inheritingTypes.Setter ? t.SetterType : t.GetterType));
        return true;
    }
    
    public static bool TryEnumerateInterfaceRecordsFor<T>(
        GameCategory category,
        T obj, 
        Type linkInterface,
        out IEnumerable<IMajorRecordGetter> interfaces)
        where T : IMajorRecordGetterEnumerable, ILoquiObjectGetter
    {
        return TryEnumerateLinkRecordsFor(category, obj, linkInterface, out interfaces)
            || TryEnumerateAspectRecordsFor(category, obj, linkInterface, out interfaces);
    }
}