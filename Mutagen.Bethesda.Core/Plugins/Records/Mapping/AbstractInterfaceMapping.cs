namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public interface IIsolatedAbstractInterfaceMapGetter : IInterfaceMapGetter
{
}

internal sealed class IsolatedAbstractInterfaceMapper : InterfaceMapGetter, IIsolatedAbstractInterfaceMapGetter
{
    public static IsolatedAbstractInterfaceMapper AutomaticFactory()
    {
        var ret = new IsolatedAbstractInterfaceMapper();
        foreach (var module in GameRuntime.Modules)
        {
            ret.Register(module.CreateIsolatedAbstractInterfaceMapping());
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