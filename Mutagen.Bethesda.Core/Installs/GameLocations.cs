using Noggog;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Installs.DI;

namespace Mutagen.Bethesda.Installs;

/// <summary>
/// A static class that locates game installations
/// </summary>
public static class GameLocations
{
    private static readonly GameLocator Locator = new();
    private static IGameDirectoryLookup GameDirLookup => Locator;
    private static IDataDirectoryLookup DataDirLookup => Locator;
    private static IGameInstallProvider GameInstallProvider => Locator; 
        
    /// <inheritdoc cref="GameLocator" />
    public static IEnumerable<DirectoryPath> GetGameFolders(GameRelease release)
    {
        return GameInstallProvider.GetAll(release);
    }

    /// <inheritdoc cref="GameLocator" />
    [Obsolete("Will be removed soon")]
    public static bool TryGetGameFolderFromRegistry(GameRelease release,
        [MaybeNullWhen(false)] out DirectoryPath path)
    {
        return Locator.TryGetGameDirectoryFromRegistry(release, out path);
    }
    
    /// <inheritdoc cref="GameLocator" />
    [Obsolete("Use variant with GameInstallMode instead")]
    public static bool TryGetGameFolder(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path)
    {
        return GameDirLookup.TryGet(release, out path);
    }
    
    /// <inheritdoc cref="GameLocator" />
    public static bool TryGetGameFolder(GameRelease release, GameInstallMode installMode, [MaybeNullWhen(false)] out DirectoryPath path)
    {
        return GameDirLookup.TryGet(release, installMode, out path);
    }

    /// <inheritdoc cref="GameLocator" />
    [Obsolete("Use variant with GameInstallMode instead")]
    public static DirectoryPath GetGameFolder(GameRelease release)
    {
        return GameDirLookup.Get(release);
    }

    /// <inheritdoc cref="GameLocator" />
    public static DirectoryPath GetGameFolder(GameRelease release, GameInstallMode installMode)
    {
        return GameDirLookup.Get(release, installMode);
    }

    /// <inheritdoc cref="GameLocator" />
    [Obsolete("Use variant with GameInstallMode instead")]
    public static bool TryGetDataFolder(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path)
    {
        return DataDirLookup.TryGet(release, out path);
    }

    /// <inheritdoc cref="GameLocator" />
    public static bool TryGetDataFolder(GameRelease release, GameInstallMode installMode, [MaybeNullWhen(false)] out DirectoryPath path)
    {
        return DataDirLookup.TryGet(release, installMode, out path);
    }

    /// <inheritdoc cref="GameLocator" />
    [Obsolete("Use variant with GameInstallMode instead")]
    public static DirectoryPath GetDataFolder(GameRelease release)
    {
        return DataDirLookup.Get(release);
    }

    /// <inheritdoc cref="GameLocator" />
    public static DirectoryPath GetDataFolder(GameRelease release, GameInstallMode installMode)
    {
        return DataDirLookup.Get(release, installMode);
    }
}