using Noggog;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Installs.DI;

namespace Mutagen.Bethesda.Installs;

/// <summary>
/// A static class that locates game installations
/// </summary>
public static class GameLocations
{
    private static readonly GameLocator Locator = new();
        
    /// <inheritdoc cref="GameLocator" />
    public static IEnumerable<DirectoryPath> GetGameFolders(GameRelease release)
    {
        return Locator.GetGameDirectories(release);
    }

    /// <inheritdoc cref="GameLocator" />
    public static bool TryGetGameFolderFromRegistry(GameRelease release,
        [MaybeNullWhen(false)] out DirectoryPath path)
    {
        return Locator.TryGetGameDirectoryFromRegistry(release, out path);
    }
        
    /// <inheritdoc cref="GameLocator" />
    public static bool TryGetGameFolder(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path)
    {
        return Locator.TryGetGameDirectory(release, out path);
    }

    /// <inheritdoc cref="GameLocator" />
    public static DirectoryPath GetGameFolder(GameRelease release)
    {
        return Locator.GetGameDirectory(release);
    }

    /// <inheritdoc cref="GameLocator" />
    public static bool TryGetDataFolder(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path)
    {
        return Locator.TryGetDataDirectory(release, out path);
    }

    /// <inheritdoc cref="GameLocator" />
    public static DirectoryPath GetDataFolder(GameRelease release)
    {
        return Locator.GetDataDirectory(release);
    }
}