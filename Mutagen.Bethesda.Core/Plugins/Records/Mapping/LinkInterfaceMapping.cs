using Noggog;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public interface ILinkInterfaceMapGetter : IInterfaceMapGetter
{
}

internal sealed class LinkInterfaceMapper : InterfaceMapGetter, ILinkInterfaceMapGetter
{
    public static LinkInterfaceMapper AutomaticFactory(string nickname)
    {
        var ret = new LinkInterfaceMapper();
        foreach (var category in EnumExt<GameCategory>.Values)
        {
            var t = Type.GetType(
                $"Mutagen.Bethesda.{category}.{category}{nickname}Mapping, Mutagen.Bethesda.{category}");
            if (t == null) continue;
            var obj = Activator.CreateInstance(t);
            var regis = obj as IInterfaceMapping;
            if (regis == null) continue;
            ret.Register(regis);
        }
        return ret;
    }
}

public static class LinkInterfaceMapping
{
    private static Lazy<LinkInterfaceMapper> _mapper = new(() =>
    {
        return LinkInterfaceMapper.AutomaticFactory("LinkInterface");
    });

    public static ILinkInterfaceMapGetter Instance => _mapper.Value;
}