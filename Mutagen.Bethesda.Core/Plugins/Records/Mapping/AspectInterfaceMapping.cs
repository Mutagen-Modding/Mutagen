using Noggog;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public interface IAspectInterfaceMapGetter : IInterfaceMapGetter
{
}

internal class AspectInterfaceMapper : InterfaceMapGetter, IAspectInterfaceMapGetter
{
    public static AspectInterfaceMapper AutomaticFactory()
    {
        var ret = new AspectInterfaceMapper();
        foreach (var category in EnumExt<GameCategory>.Values)
        {
            var t = Type.GetType(
                $"Mutagen.Bethesda.{category}.{category}AspectInterfaceMapping, Mutagen.Bethesda.{category}");
            if (t == null) continue;
            var obj = Activator.CreateInstance(t);
            var regis = obj as IInterfaceMapping;
            if (regis == null) continue;
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