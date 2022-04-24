using System.Diagnostics.CodeAnalysis;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public interface IInterfaceMapGetter
{
    IReadOnlyDictionary<Type, InterfaceMappingResult> InterfaceToObjectTypes(GameCategory mode);
    bool TryGetByFullName(string name, [MaybeNullWhen(false)] out Type type);
}

internal class InterfaceMapGetter : IInterfaceMapGetter
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

    public void Register(IInterfaceMapping mapping)
    {
        if (!_registeredMappings.Add(mapping.GetType())) return;
        _mappings[mapping.GameCategory] = mapping.InterfaceToObjectTypes;
        foreach (var interf in mapping.InterfaceToObjectTypes.Keys)
        {
            _nameToInterfaceTypeMapping[interf.FullName!] = interf;
        }
    }
}