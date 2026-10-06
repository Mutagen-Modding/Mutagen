using Noggog;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public interface IIsolatedAbstractInterfaceMapGetter : IInterfaceMapGetter
{
}

internal sealed class IsolatedAbstractInterfaceMapper : InterfaceMapGetter, IIsolatedAbstractInterfaceMapGetter
{
    public static IsolatedAbstractInterfaceMapper AutomaticFactory()
    {
        var ret = new IsolatedAbstractInterfaceMapper();
        foreach (var category in Enums<GameCategory>.Values)
        {
            IInterfaceMapping? regis;
            if (!GameRegistrations.TryGet(category, out var definition))
            {
                // Compatibility fallback for games without static registration; reflection is not trim/AOT safe.
                var t = Type.GetType(
                    $"Mutagen.Bethesda.{category}.{category}IsolatedAbstractInterfaceMapping, Mutagen.Bethesda.{category}");
                if (t == null) continue;
                var obj = Activator.CreateInstance(t);
                regis = obj as IInterfaceMapping;
                if (regis == null) continue;
            }
            else
            {
                regis = definition.AbstractMapping();
            }
            ret.Register(regis);
        }
        return ret;
    }
}

public static class AbstractInterfaceMapping
{
    public static bool AutomaticRegistration = true;

    private static Lazy<IsolatedAbstractInterfaceMapper> _mapper = new(() =>
    {
        return IsolatedAbstractInterfaceMapper.AutomaticFactory();
    });

    public static IIsolatedAbstractInterfaceMapGetter Instance => _mapper.Value;
}