using Noggog;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public interface IInheritingInterfaceMapGetter : IInterfaceMapGetter
{
}

internal sealed class InheritingInterfaceMapper : InterfaceMapGetter, IInheritingInterfaceMapGetter
{
    public static InheritingInterfaceMapper AutomaticFactory()
    {
        var ret = new InheritingInterfaceMapper();
        foreach (var category in EnumExt<GameCategory>.Values)
        {
            var t = Type.GetType(
                $"Mutagen.Bethesda.{category}.{category}InheritingInterfaceMapping, Mutagen.Bethesda.{category}");
            if (t == null) continue;
            var obj = Activator.CreateInstance(t);
            var regis = obj as IInterfaceMapping;
            if (regis == null) continue;
            ret.Register(regis);
        }
        return ret;
    }
}

public static class InheritingInterfaceMapping
{
    public static bool AutomaticRegistration = true;

    private static Lazy<InheritingInterfaceMapper> _mapper = new(() =>
    {
        return InheritingInterfaceMapper.AutomaticFactory();
    });

    public static IInheritingInterfaceMapGetter Instance => _mapper.Value;
}