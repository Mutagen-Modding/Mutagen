using Noggog;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public interface IIsolatedAbstractInterfaceMapGetter : IInterfaceMapGetter
{
}

internal class IsolatedAbstractInterfaceMapper : InterfaceMapGetter, IIsolatedAbstractInterfaceMapGetter
{
    public static IsolatedAbstractInterfaceMapper AutomaticFactory()
    {
        var ret = new IsolatedAbstractInterfaceMapper();
        foreach (var category in EnumExt<GameCategory>.Values)
        {
            var t = Type.GetType(
                $"Mutagen.Bethesda.{category}.{category}IsolatedAbstractInterfaceMapping, Mutagen.Bethesda.{category}");
            if (t == null) continue;
            var obj = Activator.CreateInstance(t);
            var regis = obj as IInterfaceMapping;
            if (regis == null) continue;
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