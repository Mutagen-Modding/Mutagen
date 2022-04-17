using System.Collections.Generic;

namespace Mutagen.Bethesda.Plugins.Implicit;

public record ImplicitRegistration(
    GameRelease GameRelease,
    ImplicitModKeyCollection BaseMasters,
    ImplicitModKeyCollection Listings,
    IReadOnlyCollection<FormKey> RecordFormKeys);