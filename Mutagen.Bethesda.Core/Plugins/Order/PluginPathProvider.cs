using System;
using System.IO;
using System.IO.Abstractions;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order
{
    public interface IPluginPathProvider
    {
        /// <summary>
        /// Returns expected location of the plugin load order file
        /// </summary>
        /// <param name="release">Game to locate for</param>
        /// <returns>Expected path to load order file</returns>
        FilePath GetListingsPath(GameRelease release);
        
        /// <summary>
        /// Attempts to locate the path to a game's load order file, and ensure existence
        /// </summary>
        /// <param name="release">Game to locate for</param>
        /// <returns>Path to load order file if it was located</returns>
        /// <exception cref="FileNotFoundException">If expected plugin file did not exist</exception>
        FilePath LocateListingsPath(GameRelease release);
        
        /// <summary>
        /// Attempts to locate the path to a game's load order file, and ensure existence
        /// </summary>
        /// <param name="release">Game to locate for</param>
        /// <param name="path">Path to load order file if it was located</param>
        /// <returns>True if file located</returns>
        bool TryLocateListingsPath(GameRelease release, out FilePath path);
    }

    public class PluginPathProvider : IPluginPathProvider
    {
        private readonly IFileSystem _fileSystem;

        public PluginPathProvider(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        
        private string GetRelativePluginsPath(GameRelease release)
        {
            return release switch
            {
                GameRelease.Oblivion => "Oblivion/Plugins.txt",
                GameRelease.SkyrimLE => "Skyrim/Plugins.txt",
                GameRelease.SkyrimSE => "Skyrim Special Edition/Plugins.txt",
                GameRelease.SkyrimVR => "Skyrim VR/Plugins.txt",
                GameRelease.Fallout4 => "Fallout4/Plugins.txt",
                _ => throw new NotImplementedException()
            };
        }

        /// <inheritdoc />
        public FilePath GetListingsPath(GameRelease release)
        {
            string pluginPath = GetRelativePluginsPath(release);
            return Path.Combine(
                Environment.GetEnvironmentVariable("LocalAppData")!,
                pluginPath);
        }

        /// <inheritdoc />
        public bool TryLocateListingsPath(GameRelease release, out FilePath path)
        {
            path = new FilePath(GetListingsPath(release));
            return _fileSystem.File.Exists(path);
        }

        /// <inheritdoc />
        public FilePath LocateListingsPath(GameRelease game)
        {
            if (TryLocateListingsPath(game, out var path))
            {
                return path;
            }
            throw new FileNotFoundException($"Could not locate load order automatically.  Expected a file at: {path.Path}");
        }
    }
}