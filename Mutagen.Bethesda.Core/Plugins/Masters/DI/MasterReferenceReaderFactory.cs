using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Plugins.Masters.DI;

public interface IMasterReferenceReaderFactory
{
    IMasterReferenceReader FromPath(ModPath path);
    IMasterReferenceReader FromStream(Stream stream, ModKey modKey, bool disposeStream = true);
    IMasterReferenceReader FromStream(IMutagenReadStream stream);
}

public class MasterReferenceReaderFactory : IMasterReferenceReaderFactory
{
    private readonly IFileSystem _fileSystem;
    private readonly IGameReleaseContext _gameReleaseContext;

    public MasterReferenceReaderFactory(
        IFileSystem fileSystem,
        IGameReleaseContext gameReleaseContext)
    {
        _fileSystem = fileSystem;
        _gameReleaseContext = gameReleaseContext;
    }
        
    public IMasterReferenceReader FromPath(ModPath path)
    {
        return MasterReferenceCollection.FromPath(path, _gameReleaseContext.Release, fileSystem: _fileSystem);
    }

    public IMasterReferenceReader FromStream(Stream stream, ModKey modKey, bool disposeStream = true)
    {
        return MasterReferenceCollection.FromStream(stream, modKey, _gameReleaseContext.Release, disposeStream);
    }

    public IMasterReferenceReader FromStream(IMutagenReadStream stream)
    {
        return MasterReferenceCollection.FromStream(stream);
    }
}