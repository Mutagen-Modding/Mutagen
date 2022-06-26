using Loqui;

namespace Mutagen.Bethesda.Plugins.Records.Mapping;

public record InterfaceMappingResult(bool Setter, IReadOnlyList<ILoquiRegistration> Registrations);