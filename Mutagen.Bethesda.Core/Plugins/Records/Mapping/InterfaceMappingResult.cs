using Loqui;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public sealed record InterfaceMappingResult(bool Setter, IReadOnlyList<ILoquiRegistration> Registrations);