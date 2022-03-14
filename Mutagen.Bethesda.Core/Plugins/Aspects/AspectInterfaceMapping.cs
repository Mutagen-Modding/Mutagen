using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Aspects;

public interface IAspectInterfaceMapGetter
{
    IReadOnlyDictionary<Type, InterfaceMappingResult> InterfaceToObjectTypes(GameCategory mode);
    bool TryGetByFullName(string name, [MaybeNullWhen(false)] out Type type);
}

public class AspectInterfaceMapper : IAspectInterfaceMapGetter
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
        foreach (var interf in TypeExt.GetInheritingFromInterface<IAspectInterfaceMapping>(
                     loadAssemblies: true))
        {
            ret.Register((Activator.CreateInstance(interf) as IAspectInterfaceMapping)!);
        }

        return ret;
    }

    public static AspectInterfaceMapper EmptyFactory()
    {
        return new AspectInterfaceMapper();
    }
}

public static class AspectInterfaceMapping
{
    public static bool AutomaticRegistration = true;

    private static Lazy<AspectInterfaceMapper> _mapper = new(() =>
    {
        if (AutomaticRegistration)
        {
            return AspectInterfaceMapper.AutomaticFactory();
        }
        else
        {
            return AspectInterfaceMapper.EmptyFactory();
        }
    });

    public static IAspectInterfaceMapGetter Instance => _mapper.Value;

    public static AspectInterfaceMapper InternalInstance => _mapper.Value;
}