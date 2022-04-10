using System;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public interface IMetaInterfaceMapGetter
{
    bool TryGetRegistrationsForInterface(GameCategory mode, Type type,
        [MaybeNullWhen(false)] out InterfaceMappingResult registrations);
}

internal class MetaInterfaceMapper : IMetaInterfaceMapGetter
{
    private readonly IAspectInterfaceMapGetter _aspectInterfaceMapGetter;
    private readonly ILinkInterfaceMapGetter _linkInterfaceMapGetter;
    private readonly IInheritingInterfaceMapGetter _inheritingInterfaceMapGetter;
    private readonly IIsolatedAbstractInterfaceMapGetter _abstractInterfaceMapGetter;

    public MetaInterfaceMapper(
        IAspectInterfaceMapGetter aspectInterfaceMapGetter,
        ILinkInterfaceMapGetter linkInterfaceMapGetter,
        IInheritingInterfaceMapGetter inheritingInterfaceMapGetter,
        IIsolatedAbstractInterfaceMapGetter abstractInterfaceMapGetter)
    {
        _aspectInterfaceMapGetter = aspectInterfaceMapGetter;
        _linkInterfaceMapGetter = linkInterfaceMapGetter;
        _inheritingInterfaceMapGetter = inheritingInterfaceMapGetter;
        _abstractInterfaceMapGetter = abstractInterfaceMapGetter;
    }

    public bool TryGetRegistrationsForInterface(GameCategory mode, Type type, [MaybeNullWhen(false)] out InterfaceMappingResult registrations)
    {
        if (_aspectInterfaceMapGetter.InterfaceToObjectTypes(mode).TryGetValue(type, out registrations)) return true;
        if (_linkInterfaceMapGetter.InterfaceToObjectTypes(mode).TryGetValue(type, out registrations)) return true;
        if (_inheritingInterfaceMapGetter.InterfaceToObjectTypes(mode).TryGetValue(type, out registrations)) return true;
        if (_abstractInterfaceMapGetter.InterfaceToObjectTypes(mode).TryGetValue(type, out registrations)) return true;
        return false;
    }
}

public static class MetaInterfaceMapping
{
    private static Lazy<MetaInterfaceMapper> _mapper = new(() =>
    {
        return new MetaInterfaceMapper(
            AspectInterfaceMapping.Instance,
            LinkInterfaceMapping.Instance,
            InheritingInterfaceMapping.Instance,
            AbstractInterfaceMapping.Instance);
    });

    public static IMetaInterfaceMapGetter Instance => _mapper.Value;

    internal static IMetaInterfaceMapGetter Warmup()
    {
        return _mapper.Value;
    }
}