using Loqui;
using Mutagen.Bethesda.Plugins.Records;

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
        foreach (var module in GameRuntime.Modules)
        {
            foreach (var maskMap in module.CreateOverrideMaskRegistration().Masks)
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

    public static object? Get(Type type)
    {
        return AddAsOverrideMasks.GetValueOrDefault(type);
    }

    public static void Warmup()
    {
        // Nothing to do
    }
}