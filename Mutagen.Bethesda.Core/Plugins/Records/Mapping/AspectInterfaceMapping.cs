using System;
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
            var obj = Activator.CreateInstance(
                $"Mutagen.Bethesda.{category}",
                $"Mutagen.Bethesda.{category}.{category}AspectInterfaceMapping");
            ret.Register((obj?.Unwrap() as IInterfaceMapping)!);
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