using System.IO;
using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Plugins.Masters
{
    public interface IMasterReferenceReaderFactory
    {
        IMasterReferenceReader FromPath(ModPath path, GameRelease release);
        IMasterReferenceReader FromStream(Stream stream, ModKey modKey, GameRelease release, bool disposeStream = true);
        IMasterReferenceReader FromStream(IMutagenReadStream stream);
    }

    public class MasterReferenceReaderFactory : IMasterReferenceReaderFactory
    {
        private readonly IFileSystem _fileSystem;

        public MasterReferenceReaderFactory(
            IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        
        public IMasterReferenceReader FromPath(ModPath path, GameRelease release)
        {
            return MasterReferenceReader.FromPath(path, release, fileSystem: _fileSystem);
        }

        public IMasterReferenceReader FromStream(Stream stream, ModKey modKey, GameRelease release, bool disposeStream = true)
        {
            return MasterReferenceReader.FromStream(stream, modKey, release, disposeStream);
        }

        public IMasterReferenceReader FromStream(IMutagenReadStream stream)
        {
            return MasterReferenceReader.FromStream(stream);
        }
    }
}