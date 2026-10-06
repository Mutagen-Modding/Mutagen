using System.Runtime.CompilerServices;
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
        foreach (var category in Enums<GameCategory>.Values)
        {
            IInterfaceMapping? regis;
            if (!GameRegistrations.TryGet(category, out var definition))
            {
                if (!RuntimeFeature.IsDynamicCodeSupported) continue;
                var t = Type.GetType(
                    $"Mutagen.Bethesda.{category}.{category}{nickname}Mapping, Mutagen.Bethesda.{category}");
                if (t == null) continue;
                var obj = Activator.CreateInstance(t);
                regis = obj as IInterfaceMapping;
                if (regis == null) continue;
            }
            else
            {
                regis = definition.LinkMapping();
            }
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