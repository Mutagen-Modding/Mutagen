namespace Mutagen.Bethesda.Installs.DI;

public class ProtonPrefixProvider : IProtonPrefixProvider
{
    public string? TryGetProtonLocalAppData(GameRelease release)
    {
        return GameLocator.Instance.TryGetProtonLocalAppData(release);
    }

    public string? TryGetProtonMyDocuments(GameRelease release)
    {
        return GameLocator.Instance.TryGetProtonMyDocuments(release);
    }
}
