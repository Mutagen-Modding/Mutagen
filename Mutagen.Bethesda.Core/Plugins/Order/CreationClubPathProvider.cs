using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using Noggog;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.Plugins.Order
{
    public interface ICreationClubPathProvider
    {
        /// <summary>
        /// Returns expected location of the creation club load order file
        /// </summary>
        /// <param name="category">Game to locate for</param>
        /// <param name="dataPath">Path to the data folder</param>
        /// <returns>Expected path to creation club load order file</returns>
        FilePath? GetListingsPath(GameCategory category, DirectoryPath dataPath);
        
        /// <summary>
        /// Attempts to locate the path to a creation club load order file, and ensure existence
        /// </summary>
        /// <param name="category">Game to locate for</param>
        /// <param name="dataPath">Path to the data folder</param>
        /// <param name="path">Path to load order file if it was located</param>
        /// <returns>True if file located</returns>
        bool TryLocateListingsPath(GameCategory category, DirectoryPath dataPath, [MaybeNullWhen(false)] out FilePath path);
        
        
        /// <summary>
        /// Attempts to locate the path to a creation club load order file, and ensure existence
        /// </summary>
        /// <param name="category">Game to locate for</param>
        /// <param name="dataPath">Path to the data folder</param>
        /// <returns>Path to load order file if it was located</returns>
        /// <exception cref="FileNotFoundException">If expected creation club file did not exist</exception>
        FilePath LocateListingsPath(GameCategory category, DirectoryPath dataPath);
    }

    public class CreationClubPathProvider : ICreationClubPathProvider
    {
        private readonly IFileSystem _fileSystem;

        public CreationClubPathProvider(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        
        /// <inheritdoc />
        public FilePath? GetListingsPath(GameCategory category, DirectoryPath dataPath)
        {
            switch (category)
            {
                case GameCategory.Oblivion:
                    return null;
                case GameCategory.Skyrim:
                case GameCategory.Fallout4:
                    return Path.Combine(Path.GetDirectoryName(dataPath.Path)!, $"{category}.ccc");
                default:
                    throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        public bool TryLocateListingsPath(GameCategory category, DirectoryPath dataPath, [MaybeNullWhen(false)] out FilePath path)
        {
            var tmpPath = GetListingsPath(category, dataPath);
            if (tmpPath == null) return false;
            path = tmpPath.Value;
            return _fileSystem.File.Exists(path);
        }

        /// <inheritdoc />
        public FilePath LocateListingsPath(GameCategory category, DirectoryPath dataPath)
        {
            if (TryLocateListingsPath(category, dataPath, out var path))
            {
                return path;
            }
            throw new FileNotFoundException($"Could not locate load order automatically.  Expected a file at: {path.Path}");
        }
    }
}