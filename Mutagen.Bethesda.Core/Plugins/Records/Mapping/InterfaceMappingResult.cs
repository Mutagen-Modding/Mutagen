using Loqui;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public sealed record InterfaceMappingTypes(
    Type Getter,
    Type? Setter);

public sealed record InterfaceMappingResult(
    bool Setter,
    IReadOnlyList<ILoquiRegistration> Registrations,
    InterfaceMappingTypes Types);