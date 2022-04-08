using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Aspects;

public interface IAspectInterfaceMapGetter
{
    IReadOnlyDictionary<Type, InterfaceMappingResult> InterfaceToObjectTypes(GameCategory mode);
    bool TryGetByFullName(string name, [MaybeNullWhen(false)] out Type type);
}

internal class AspectInterfaceMapper : IAspectInterfaceMapGetter
{
    public Dictionary<GameCategory, IReadOnlyDictionary<Type, InterfaceMappingResult>> Mappings = new();
    public Dictionary<string, Type> NameToInterfaceTypeMapping = new();
        
    public IReadOnlyDictionary<Type, InterfaceMappingResult> InterfaceToObjectTypes(GameCategory mode)
    {
        if (Mappings.TryGetValue(mode, out var value))
        {
            return value;
        }

        return DictionaryExt.Empty<Type, InterfaceMappingResult>();
    }

    public bool TryGetByFullName(string name, [MaybeNullWhen(false)] out Type type)
    {
        return NameToInterfaceTypeMapping.TryGetValue(name, out type);
    }

    public void Register(IAspectInterfaceMapping mapping)
    {
        Mappings[mapping.GameCategory] = mapping.InterfaceToObjectTypes;
        foreach (var interf in mapping.InterfaceToObjectTypes.Keys)
        {
            NameToInterfaceTypeMapping[interf.FullName!] = interf;
        }
    }

    public static AspectInterfaceMapper AutomaticFactory()
    {
        var ret = new AspectInterfaceMapper();
        foreach (var category in EnumExt<GameCategory>.Values)
        {
            var obj = Activator.CreateInstance(
                $"Mutagen.Bethesda.{category}",
                $"Mutagen.Bethesda.{category}.{category}AspectInterfaceMapping");
            ret.Register((obj?.Unwrap() as IAspectInterfaceMapping)!);
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

    internal static AspectInterfaceMapper InternalInstance => _mapper.Value;
}