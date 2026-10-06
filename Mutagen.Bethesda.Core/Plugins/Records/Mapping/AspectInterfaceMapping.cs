using Noggog;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public interface IAspectInterfaceMapGetter : IInterfaceMapGetter
{
}

internal sealed class AspectInterfaceMapper : InterfaceMapGetter, IAspectInterfaceMapGetter
{
    public static AspectInterfaceMapper AutomaticFactory()
    {
        var ret = new AspectInterfaceMapper();
        foreach (var category in Enums<GameCategory>.Values)
        {
            IInterfaceMapping? regis;
            if (!GameRegistrations.TryGet(category, out var definition))
            {
                // Compatibility fallback for games without static registration; reflection is not trim/AOT safe.
                var t = Type.GetType(
                    $"Mutagen.Bethesda.{category}.{category}AspectInterfaceMapping, Mutagen.Bethesda.{category}");
                if (t == null) continue;
                var obj = Activator.CreateInstance(t);
                regis = obj as IInterfaceMapping;
                if (regis == null) continue;
            }
            else
            {
                regis = definition.AspectMapping();
            }
            ret.Register(regis);
        }
        return ret;
    }
}

public static class AspectInterfaceMapping
{
    public static bool AutomaticRegistration = true;

    private static Lazy<AspectInterfaceMapper> _mapper = new(() =>
    {
        return AspectInterfaceMapper.AutomaticFactory();
    });

    public static IAspectInterfaceMapGetter Instance => _mapper.Value;
}