using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using GameFinder;
using GameFinder.StoreHandlers.Steam;
using GameFinder.StoreHandlers.GOG;
using Microsoft.Win32;

namespace Mutagen.Bethesda.Installs
{
    /// <summary>
    /// A static class that locates game installations
    /// </summary>
    public static class GameLocations
    {
        private static readonly GameLocator Locator = new();
        
        /// <inheritdoc cref="IGameLocator" />
        public static IEnumerable<DirectoryPath> GetGameFolders(GameRelease release)
        {
            return Locator.GetGameFolders(release);
        }

        /// <inheritdoc cref="IGameLocator" />
        public static bool TryGetGameFolderFromRegistry(GameRelease release,
            [MaybeNullWhen(false)] out DirectoryPath path)
        {
            return Locator.TryGetGameFolderFromRegistry(release, out path);
        }
        
        /// <inheritdoc cref="IGameLocator" />
        public static bool TryGetGameFolder(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path)
        {
            return Locator.TryGetGameFolder(release, out path);
        }

        /// <inheritdoc cref="IGameLocator" />
        public static DirectoryPath GetGameFolder(GameRelease release)
        {
            return Locator.GetGameFolder(release);
        }

        /// <inheritdoc cref="IGameLocator" />
        public static bool TryGetDataFolder(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path)
        {
            return Locator.TryGetDataFolder(release, out path);
        }

        /// <inheritdoc cref="IGameLocator" />
        public static DirectoryPath GetDataFolder(GameRelease release)
        {
            return Locator.GetDataFolder(release);
        }
    }
}
