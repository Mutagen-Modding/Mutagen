namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public interface ILinkInterfaceMapGetter : IInterfaceMapGetter
{
}

internal sealed class LinkInterfaceMapper : InterfaceMapGetter, ILinkInterfaceMapGetter
{
    public static LinkInterfaceMapper AutomaticFactory()
    {
        var ret = new LinkInterfaceMapper();
        foreach (var module in GameRuntime.Modules)
        {
            ret.Register(module.CreateLinkInterfaceMapping());
        }
        return ret;
    }
}

public static class LinkInterfaceMapping
{
    private static Lazy<LinkInterfaceMapper> _mapper = new(() =>
    {
        return LinkInterfaceMapper.AutomaticFactory();
    });

    public static ILinkInterfaceMapGetter Instance => _mapper.Value;
}