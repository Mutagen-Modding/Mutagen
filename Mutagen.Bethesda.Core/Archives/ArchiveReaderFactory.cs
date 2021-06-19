using System;
using System.IO.Abstractions;
using Mutagen.Bethesda.Archives.Ba2;
using Mutagen.Bethesda.Archives.Bsa;
using Noggog;

namespace Mutagen.Bethesda.Archives
{
    public interface IArchiveReaderFactory
    {
        /// <summary>
        /// Creates an Archive reader object from the given path, for the given Game Release.
        /// </summary>
        /// <param name="release">GameRelease the archive is for</param>
        /// <param name="path">Path to create archive reader from</param>
        /// <returns>Archive reader object</returns>
        IArchiveReader Create(GameRelease release, FilePath path);
    }

    public class ArchiveReaderFactory : IArchiveReaderFactory
    {
        private readonly IFileSystem _fileSystem;

        public ArchiveReaderFactory(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        
        /// <summary>
        /// Creates an Archive reader object from the given path, for the given Game Release.
        /// </summary>
        /// <param name="release">GameRelease the archive is for</param>
        /// <param name="path">Path to create archive reader from</param>
        /// <returns>Archive reader object</returns>
        public IArchiveReader Create(GameRelease release, FilePath path)
        {
            switch (release)
            {
                case GameRelease.Oblivion:
                case GameRelease.SkyrimLE:
                case GameRelease.SkyrimSE:
                case GameRelease.SkyrimVR:
                    return new BsaReader(path, _fileSystem);
                case GameRelease.Fallout4:
                    return new Ba2Reader(path, _fileSystem);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}