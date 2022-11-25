namespace Mutagen.Bethesda.Installs;

public record RegistryGameSource : IGameSource
{
    public string RegistryPath { get; init; } = string.Empty;
    public string RegistryKey { get; init; } = string.Empty;
}