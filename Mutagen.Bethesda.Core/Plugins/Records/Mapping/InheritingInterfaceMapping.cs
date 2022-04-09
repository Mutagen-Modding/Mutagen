using System;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public interface IInheritingInterfaceMapGetter : IInterfaceMapGetter
{
}

internal class InheritingInterfaceMapper : InterfaceMapGetter, IInheritingInterfaceMapGetter
{
    public static InheritingInterfaceMapper AutomaticFactory()
    {
        var ret = new InheritingInterfaceMapper();
        foreach (var category in EnumExt<GameCategory>.Values)
        {
            var obj = Activator.CreateInstance(
                $"Mutagen.Bethesda.{category}",
                $"Mutagen.Bethesda.{category}.{category}InheritingInterfaceMapping");
            ret.Register((obj?.Unwrap() as IInterfaceMapping)!);
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