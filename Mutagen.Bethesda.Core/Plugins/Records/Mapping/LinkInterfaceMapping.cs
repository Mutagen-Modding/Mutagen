using System;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public interface ILinkInterfaceMapGetter : IInterfaceMapGetter
{
}

internal class LinkInterfaceMapper : InterfaceMapGetter, ILinkInterfaceMapGetter
{
    public static LinkInterfaceMapper AutomaticFactory(string nickname)
    {
        var ret = new LinkInterfaceMapper();
        foreach (var category in EnumExt<GameCategory>.Values)
        {
            var obj = Activator.CreateInstance(
                $"Mutagen.Bethesda.{category}",
                $"Mutagen.Bethesda.{category}.{category}{nickname}Mapping");
            ret.Register((obj?.Unwrap() as IInterfaceMapping)!);
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