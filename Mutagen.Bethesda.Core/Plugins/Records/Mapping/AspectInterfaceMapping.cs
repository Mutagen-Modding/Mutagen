namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public interface IAspectInterfaceMapGetter : IInterfaceMapGetter
{
}

internal sealed class AspectInterfaceMapper : InterfaceMapGetter, IAspectInterfaceMapGetter
{
    public static AspectInterfaceMapper AutomaticFactory()
    {
        var ret = new AspectInterfaceMapper();
        foreach (var module in GameRuntime.Modules)
        {
            ret.Register(module.CreateAspectInterfaceMapping());
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