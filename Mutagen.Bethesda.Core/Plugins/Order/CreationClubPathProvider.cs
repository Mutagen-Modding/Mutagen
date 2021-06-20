using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Noggog;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.Plugins.Order
{
    public interface ICreationClubPathProvider
    {
        /// <summary>
        /// Attempts to locate the path to a creation club load order file, and ensure existence
        /// </summary>
        /// <param name="category">Game to locate for</param>
        /// <param name="dataPath">Path to the data folder</param>
        /// <returns>Path to load order file if it was located</returns>
        FilePath? GetListingsPath(GameCategory category, DirectoryPath dataPath);
        
        /// <summary>
        /// Attempts to locate the path to a creation club load order file, and ensure existence
        /// </summary>
        /// <param name="category">Game to locate for</param>
        /// <param name="dataPath">Path to the data folder</param>
        /// <param name="path">Path to load order file if it was located</param>
        /// <returns>True if file located</returns>
        bool TryGetListingsPath(GameCategory category, DirectoryPath dataPath, [MaybeNullWhen(false)] out FilePath path);
    }

    public class CreationClubPathProvider : ICreationClubPathProvider
    {
        private readonly IFileSystem _fileSystem;

        public CreationClubPathProvider(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        
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

        public bool TryGetListingsPath(GameCategory category, DirectoryPath dataPath, [MaybeNullWhen(false)] out FilePath path)
        {
            var tmpPath = GetListingsPath(category, dataPath);
            if (tmpPath == null) return false;
            path = tmpPath.Value;
            return _fileSystem.File.Exists(path);
        }
    }
}