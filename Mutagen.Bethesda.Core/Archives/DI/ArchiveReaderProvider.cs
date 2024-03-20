using System.IO.Abstractions;
using Mutagen.Bethesda.Archives.Ba2;
using Mutagen.Bethesda.Archives.Bsa;
using Mutagen.Bethesda.Environments.DI;
using Noggog;

namespace Mutagen.Bethesda.Archives.DI;

public interface IArchiveReaderProvider
{
    /// <summary>
    /// Creates an Archive reader object from the given path, for the given Game Release.
    /// </summary>
    /// <param name="path">Path to create archive reader from</param>
    /// <returns>Archive reader object</returns>
    IArchiveReader Create(FilePath path);
}

public sealed class ArchiveReaderProvider : IArchiveReaderProvider
{
    private readonly IFileSystem _fileSystem;
    private readonly IGameReleaseContext _gameRelease;

    public ArchiveReaderProvider(
        IFileSystem fileSystem,
        IGameReleaseContext gameRelease)
    {
        _fileSystem = fileSystem;
        _gameRelease = gameRelease;
    }
        
    /// <inheritdoc />
    public IArchiveReader Create(FilePath path)
    {
        switch (_gameRelease.Release)
        {
            case GameRelease.Oblivion:
            case GameRelease.SkyrimLE:
            case GameRelease.SkyrimSE:
            case GameRelease.SkyrimSEGog:
            case GameRelease.SkyrimVR:
            case GameRelease.EnderalLE:
            case GameRelease.EnderalSE:
                return new BsaReader(path, _fileSystem);
            case GameRelease.Starfield:
            case GameRelease.Fallout4:
            case GameRelease.Fallout4VR:
                return new Ba2Reader(path, _fileSystem);
            default:
                throw new NotImplementedException();
        }
    }
}