namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public interface IInheritingInterfaceMapGetter : IInterfaceMapGetter
{
}

internal sealed class InheritingInterfaceMapper : InterfaceMapGetter, IInheritingInterfaceMapGetter
{
    public static InheritingInterfaceMapper AutomaticFactory()
    {
        var ret = new InheritingInterfaceMapper();
        foreach (var module in GameRuntime.Modules)
        {
            ret.Register(module.CreateInheritingInterfaceMapping());
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