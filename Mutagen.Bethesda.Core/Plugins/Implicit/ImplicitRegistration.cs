namespace Mutagen.Bethesda.Plugins.Implicit;

public sealed record ImplicitRegistration(
    GameRelease GameRelease,
    ImplicitModKeyCollection BaseMasters,
    ImplicitModKeyCollection Listings,
    IReadOnlyCollection<FormKey> RecordFormKeys);