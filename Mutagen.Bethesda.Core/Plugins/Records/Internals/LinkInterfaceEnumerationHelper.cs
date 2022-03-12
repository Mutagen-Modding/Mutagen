using System;
using System.Collections.Generic;
using System.Linq;
using Loqui;

namespace Mutagen.Bethesda.Plugins.Records.Internals;

public static class LinkInterfaceEnumerationHelper
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
}