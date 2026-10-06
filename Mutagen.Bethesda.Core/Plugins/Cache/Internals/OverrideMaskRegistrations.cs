using System.Runtime.CompilerServices;
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
        foreach (var category in Enums<GameCategory>.Values)
        {
            IOverrideMaskRegistration? regis;
            if (!GameRegistrations.TryGet(category, out var definition))
            {
                if (!RuntimeFeature.IsDynamicCodeSupported) continue;
                var t = Type.GetType(
                    $"Mutagen.Bethesda.{category}.{category}OverrideMaskRegistration, Mutagen.Bethesda.{category}");
                if (t == null) continue;
                var obj = Activator.CreateInstance(t);
                regis = obj as IOverrideMaskRegistration;
                if (regis == null) continue;
            }
            else
            {
                regis = definition.OverrideMasks();
            }
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

    public static object? Get(Type type)
    {
        return AddAsOverrideMasks.GetValueOrDefault(type);
    }

    public static void Warmup()
    {
        // Nothing to do
    }
}