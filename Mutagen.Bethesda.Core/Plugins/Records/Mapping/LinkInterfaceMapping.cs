using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public interface ILinkInterfaceMapGetter
{
    IReadOnlyDictionary<Type, InterfaceMappingResult> InterfaceToObjectTypes(GameCategory mode);
    bool TryGetByFullName(string name, [MaybeNullWhen(false)] out Type type);
}

internal class LinkInterfaceMapper : ILinkInterfaceMapGetter
{
    private readonly HashSet<Type> _registeredMappings = new();
    private readonly Dictionary<GameCategory, IReadOnlyDictionary<Type, InterfaceMappingResult>> _mappings = new();
    private readonly Dictionary<string, Type> _nameToInterfaceTypeMapping = new();
        
    public IReadOnlyDictionary<Type, InterfaceMappingResult> InterfaceToObjectTypes(GameCategory mode)
    {
        if (_mappings.TryGetValue(mode, out var value))
        {
            return value;
        }

        return DictionaryExt.Empty<Type, InterfaceMappingResult>();
    }

    public bool TryGetByFullName(string name, [MaybeNullWhen(false)] out Type type)
    {
        return _nameToInterfaceTypeMapping.TryGetValue(name, out type);
    }

    public void Register(ILinkInterfaceMapping mapping)
    {
        if (!_registeredMappings.Add(mapping.GetType())) return;
        _mappings[mapping.GameCategory] = mapping.InterfaceToObjectTypes;
        foreach (var interf in mapping.InterfaceToObjectTypes.Keys)
        {
            _nameToInterfaceTypeMapping[interf.FullName!] = interf;
        }
    }

    public static LinkInterfaceMapper AutomaticFactory()
    {
        var ret = new LinkInterfaceMapper();
        foreach (var interf in TypeExt.GetInheritingFromInterface<ILinkInterfaceMapping>(
                     loadAssemblies: true))
        {
            ret.Register((Activator.CreateInstance(interf) as ILinkInterfaceMapping)!);
        }

        return ret;
    }

    public static LinkInterfaceMapper EmptyFactory()
    {
        return new LinkInterfaceMapper();
    }
}

public static class LinkInterfaceMapping
{
    public static bool AutomaticRegistration = true;

    private static Lazy<LinkInterfaceMapper> _mapper = new(() =>
    {
        if (AutomaticRegistration)
        {
            return LinkInterfaceMapper.AutomaticFactory();
        }
        else
        {
            return LinkInterfaceMapper.EmptyFactory();
        }
    });

    public static ILinkInterfaceMapGetter Instance => _mapper.Value;

    internal static LinkInterfaceMapper InternalInstance => _mapper.Value;
}