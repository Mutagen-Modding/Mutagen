using System;
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
            var obj = Activator.CreateInstance(
                $"Mutagen.Bethesda.{category}",
                $"Mutagen.Bethesda.{category}.{category}IsolatedAbstractInterfaceMapping");
            ret.Register((obj?.Unwrap() as IInterfaceMapping)!);
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