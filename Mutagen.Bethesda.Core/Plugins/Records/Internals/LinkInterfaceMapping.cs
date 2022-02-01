using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Mutagen.Bethesda.Plugins.Records.Internals;

public interface ILinkInterfaceMapGetter
{
    IReadOnlyDictionary<Type, Type[]> InterfaceToObjectTypes(GameCategory mode);
    bool TryGetByFullName(string name, [MaybeNullWhen(false)] out Type type);
}

public class LinkInterfaceMapper : ILinkInterfaceMapGetter
{
    public Dictionary<GameCategory, IReadOnlyDictionary<Type, Type[]>> Mappings = new();
    public Dictionary<string, Type> NameToInterfaceTypeMapping = new();
        
    private LinkInterfaceMapper()
    {
    }
        
    public IReadOnlyDictionary<Type, Type[]> InterfaceToObjectTypes(GameCategory mode)
    {
        if (Mappings.TryGetValue(mode, out var value))
        {
            return value;
        }

        return DictionaryExt.Empty<Type, Type[]>();
    }

    public bool TryGetByFullName(string name, [MaybeNullWhen(false)] out Type type)
    {
        return NameToInterfaceTypeMapping.TryGetValue(name, out type);
    }

    public void Register(ILinkInterfaceMapping mapping)
    {
        Mappings[mapping.GameCategory] = mapping.InterfaceToObjectTypes;
        foreach (var interf in mapping.InterfaceToObjectTypes.Keys)
        {
            NameToInterfaceTypeMapping[interf.FullName!] = interf;
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

    public static LinkInterfaceMapper InternalInstance => _mapper.Value;
}