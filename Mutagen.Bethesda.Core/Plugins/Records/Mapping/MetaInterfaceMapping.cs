using System.Diagnostics.CodeAnalysis;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public interface IMetaInterfaceMapGetter
{
    bool TryGetRegistrationsForInterface(GameCategory mode, Type type,
        [MaybeNullWhen(false)] out InterfaceMappingResult registrations);
    bool TryGetRegistrationsForInterface(Type type,
        [MaybeNullWhen(false)] out InterfaceMappingResult registrations);
    bool TryGetRegistrationsForInterface(GameCategory mode, string fullName,
        [MaybeNullWhen(false)] out InterfaceMappingResult registrations);
    bool TryGetRegistrationsForInterface(string fullName,
        [MaybeNullWhen(false)] out InterfaceMappingResult registrations);
}

internal sealed class MetaInterfaceMapper : IMetaInterfaceMapGetter
{
    private readonly IAspectInterfaceMapGetter _aspectInterfaceMapGetter;
    private readonly ILinkInterfaceMapGetter _linkInterfaceMapGetter;
    private readonly IInheritingInterfaceMapGetter _inheritingInterfaceMapGetter;
    private readonly IIsolatedAbstractInterfaceMapGetter _abstractInterfaceMapGetter;
    private readonly Lazy<Dictionary<GameCategory, Dictionary<Type, InterfaceMappingResult>>> _categoryTypeDict;
    private readonly Lazy<Dictionary<Type, InterfaceMappingResult>> _typeDict;
    private readonly Lazy<Dictionary<GameCategory, Dictionary<string, InterfaceMappingResult>>> _categoryTypeDictByName;
    private readonly Lazy<Dictionary<string, InterfaceMappingResult>> _typeDictByName;
    
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
        _categoryTypeDict = new Lazy<Dictionary<GameCategory, Dictionary<Type, InterfaceMappingResult>>>(() =>
        {
            var ret = new Dictionary<GameCategory, Dictionary<Type, InterfaceMappingResult>>();
            foreach (var cat in Enums<GameCategory>.Values)
            {
                ret[cat] = _aspectInterfaceMapGetter.InterfaceToObjectTypes(cat)
                    .Concat(_linkInterfaceMapGetter.InterfaceToObjectTypes(cat))
                    .Concat(_inheritingInterfaceMapGetter.InterfaceToObjectTypes(cat))
                    .Concat(_abstractInterfaceMapGetter.InterfaceToObjectTypes(cat))
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            return ret;
        });
        _categoryTypeDictByName = new Lazy<Dictionary<GameCategory, Dictionary<string, InterfaceMappingResult>>>(() =>
        {
            var ret = new Dictionary<GameCategory, Dictionary<string, InterfaceMappingResult>>();
            foreach (var cat in _categoryTypeDict.Value)
            {
                var dict = new Dictionary<string, InterfaceMappingResult>();
                foreach (var kv in cat.Value)
                {
                    dict[kv.Key.FullName!] = kv.Value;
                }

                ret[cat.Key] = dict;
            }
            return ret;
        });
        _typeDict = new Lazy<Dictionary<Type, InterfaceMappingResult>>(() =>
        {
            var ret = new Dictionary<Type, InterfaceMappingResult>();
            foreach (var kv in _categoryTypeDict.Value
                         .SelectMany(x => x.Value))
            {
                if (!ret.TryGetValue(kv.Key, out var items))
                {
                    ret[kv.Key] = kv.Value with { Registrations = kv.Value.Registrations.ToList() };
                }
                else
                {
                    ret[kv.Key] = items with { Registrations = items.Registrations.Concat(kv.Value.Registrations).ToList() };
                }
            }
            return ret;
        });
        _typeDictByName = new Lazy<Dictionary<string, InterfaceMappingResult>>(() =>
        {
            var ret = new Dictionary<string, InterfaceMappingResult>();
            foreach (var cat in _typeDict.Value)
            {
                ret[cat.Key.FullName!] = cat.Value;
            }
            return ret;
        });
    }

    public bool TryGetRegistrationsForInterface(GameCategory mode, Type type, [MaybeNullWhen(false)] out InterfaceMappingResult registrations)
    {
        if (!_categoryTypeDict.Value.TryGetValue(mode, out var subDict))
        {
            registrations = default;
            return false;
        }
        return subDict.TryGetValue(type, out registrations);
    }

    public bool TryGetRegistrationsForInterface(Type type, [MaybeNullWhen(false)] out InterfaceMappingResult registrations)
    {
        return _typeDict.Value.TryGetValue(type, out registrations);
    }

    public bool TryGetRegistrationsForInterface(GameCategory mode, string fullName, [MaybeNullWhen(false)] out InterfaceMappingResult registrations)
    {
        if (!_categoryTypeDictByName.Value.TryGetValue(mode, out var subDict))
        {
            registrations = default;
            return false;
        }
        return subDict.TryGetValue(fullName, out registrations);
    }

    public bool TryGetRegistrationsForInterface(string fullName, [MaybeNullWhen(false)] out InterfaceMappingResult registrations)
    {
        return _typeDictByName.Value.TryGetValue(fullName, out registrations);
    }
}

public static class MetaInterfaceMapping
{
    private static readonly Lazy<MetaInterfaceMapper> _mapper = new(() =>
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