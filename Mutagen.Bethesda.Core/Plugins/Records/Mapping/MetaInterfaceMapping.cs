using System;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Aspects;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public interface IMetaInterfaceMapGetter
{
    bool TryGetRegistrationsForInterface(GameCategory mode, Type type,
        [MaybeNullWhen(false)] out InterfaceMappingResult registrations);
}

public class MetaInterfaceMapper : IMetaInterfaceMapGetter
{
    private readonly IAspectInterfaceMapGetter _aspectInterfaceMapGetter;
    private readonly ILinkInterfaceMapGetter _linkInterfaceMapGetter;

    public MetaInterfaceMapper(
        IAspectInterfaceMapGetter aspectInterfaceMapGetter,
        ILinkInterfaceMapGetter linkInterfaceMapGetter)
    {
        _aspectInterfaceMapGetter = aspectInterfaceMapGetter;
        _linkInterfaceMapGetter = linkInterfaceMapGetter;
    }

    public bool TryGetRegistrationsForInterface(GameCategory mode, Type type, [MaybeNullWhen(false)] out InterfaceMappingResult registrations)
    {
        if (_aspectInterfaceMapGetter.InterfaceToObjectTypes(mode).TryGetValue(type, out registrations)) return true;
        if (_linkInterfaceMapGetter.InterfaceToObjectTypes(mode).TryGetValue(type, out registrations)) return true;
        return false;
    }
}

public static class MetaInterfaceMapping
{
    private static Lazy<MetaInterfaceMapper> _mapper = new(() =>
    {
        return new MetaInterfaceMapper(
            AspectInterfaceMapping.Instance,
            LinkInterfaceMapping.Instance);
    });

    public static IMetaInterfaceMapGetter Instance => _mapper.Value;

    internal static IMetaInterfaceMapGetter Warmup()
    {
        return _mapper.Value;
    }
}