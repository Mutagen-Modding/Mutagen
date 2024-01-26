using System.Diagnostics.CodeAnalysis;
using Loqui;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public interface IGetterTypeMapper
{
    bool TryGetGetterType(Type t, [MaybeNullWhen(false)] out Type getter);
    bool TryGetGetterType(GameCategory category, Type t, [MaybeNullWhen(false)] out Type getter);
    bool TryGetGetterType(string fullName, [MaybeNullWhen(false)] out Type getter);
    bool TryGetGetterType(GameCategory category, string fullName, [MaybeNullWhen(false)] out Type getter);
}

public class GetterTypeMapper : IGetterTypeMapper
{
    private readonly IMetaInterfaceMapGetter _meta;
    
    public GetterTypeMapper(IMetaInterfaceMapGetter meta)
    {
        _meta = meta;
    }

    public bool TryGetGetterType(Type t, [MaybeNullWhen(false)] out Type getter)
    {
        if (LoquiRegistration.TryGetRegister(t, out var loquiRegis))
        {
            getter = loquiRegis.GetterType;
            return true;
        }

        if (_meta.TryGetRegistrationsForInterface(t, out var interfaceRegis))
        {
            getter = interfaceRegis.Types.Getter;
            return true;
        }

        getter = default;
        return false;
    }

    public bool TryGetGetterType(GameCategory category, Type t, [MaybeNullWhen(false)] out Type getter)
    {
        if (LoquiRegistration.TryGetRegister(t, out var loquiRegis))
        {
            getter = loquiRegis.GetterType;
            return loquiRegis.ProtocolKey.Namespace == Enums<GameCategory>.ToStringFast((int)category);
        }

        if (_meta.TryGetRegistrationsForInterface(category, t, out var interfaceRegis))
        {
            getter = interfaceRegis.Types.Getter;
            return true;
        }

        getter = default;
        return false;
    }

    public bool TryGetGetterType(string fullName, [MaybeNullWhen(false)] out Type getter)
    {
        if (LoquiRegistration.TryGetRegisterByFullName(fullName, out var regis))
        {
            getter = regis.GetterType;
            return true;
        }

        if (_meta.TryGetRegistrationsForInterface(fullName, out var interfaceRegis))
        {
            getter = interfaceRegis.Types.Getter;
            return true;
        }

        getter = default;
        return false;
    }

    public bool TryGetGetterType(GameCategory category, string fullName, [MaybeNullWhen(false)] out Type getter)
    {
        if (LoquiRegistration.TryGetRegisterByFullName(fullName, out var loquiRegis))
        {
            getter = loquiRegis.GetterType;
            return loquiRegis.ProtocolKey.Namespace == Enums<GameCategory>.ToStringFast((int)category);
        }

        if (_meta.TryGetRegistrationsForInterface(category, fullName, out var interfaceRegis))
        {
            getter = interfaceRegis.Types.Getter;
            return true;
        }

        getter = default;
        return false;
    }
}

public static class GetterTypeMapping
{
    private static readonly Lazy<GetterTypeMapper> _mapper = new(() =>
    {
        return new GetterTypeMapper(MetaInterfaceMapping.Instance);
    });

    public static IGetterTypeMapper Instance => _mapper.Value;

    internal static IGetterTypeMapper Warmup()
    {
        return _mapper.Value;
    }
}