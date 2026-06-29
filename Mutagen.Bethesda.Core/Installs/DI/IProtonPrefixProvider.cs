namespace Mutagen.Bethesda.Installs.DI;

public interface IProtonPrefixProvider
{
    string? TryGetProtonLocalAppData(GameRelease release);
    string? TryGetProtonMyDocuments(GameRelease release);
}
