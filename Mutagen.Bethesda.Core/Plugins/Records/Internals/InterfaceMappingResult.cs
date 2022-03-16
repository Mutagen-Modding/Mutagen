using System.Collections.Generic;
using Loqui;

namespace Mutagen.Bethesda.Plugins.Records.Internals;

public record InterfaceMappingResult(bool Setter, IReadOnlyList<ILoquiRegistration> Registrations);