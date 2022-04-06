using System;
using System.Collections.Generic;
using Loqui;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Internals;

internal interface IOverrideMaskRegistration
{
    IEnumerable<(ILoquiRegistration, object)> Masks { get; }
}
    
internal static class OverrideMaskRegistrations
{
    private static readonly Dictionary<Type, object> AddAsOverrideMasks = new();

    static OverrideMaskRegistrations()
    {
        foreach (var type in typeof(IOverrideMaskRegistration).GetInheritingFromInterface(filter: x => x.FullName?.Contains("Mutagen") ?? false))
        {
            var regis = (IOverrideMaskRegistration)Activator.CreateInstance(type)!;
            foreach (var maskMap in regis.Masks)
            {
                AddAsOverrideMasks.Add(maskMap.Item1.ClassType, maskMap.Item2);
                AddAsOverrideMasks.Add(maskMap.Item1.GetterType, maskMap.Item2);
                AddAsOverrideMasks.Add(maskMap.Item1.SetterType, maskMap.Item2);
            }
        }
    }

    public static object? Get<TMajor>()
        where TMajor : IMajorRecordGetter
    {
        return AddAsOverrideMasks.GetValueOrDefault(typeof(TMajor));
    }

    public static void Warmup()
    {
        // Nothing to do
    }
}