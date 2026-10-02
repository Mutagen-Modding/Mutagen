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
            if (GameRegistrations.TryGet(category, out var definition))
            {
                AddMasks(definition.OverrideMasks());
                continue;
            }

            // Compatibility fallback for games without static registration; reflection is not trim/AOT safe.
            var t = Type.GetType(
                $"Mutagen.Bethesda.{category}.{category}OverrideMaskRegistration, Mutagen.Bethesda.{category}");
            if (t == null) continue;
            var obj = Activator.CreateInstance(t);
            var regis = obj as IOverrideMaskRegistration;
            if (regis == null) continue;
            AddMasks(regis);
        }
    }

    private static void AddMasks(IOverrideMaskRegistration registration)
    {
        foreach (var (record, mask) in registration.Masks)
        {
            AddAsOverrideMasks.Add(record.ClassType, mask);
            AddAsOverrideMasks.Add(record.GetterType, mask);
            AddAsOverrideMasks.Add(record.SetterType, mask);
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