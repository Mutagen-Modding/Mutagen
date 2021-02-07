using Compression.BSA;
using System.IO;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Ba2
{
    class Ba2FileWrapper : IArchiveFile
    {
        private readonly IFile _file;

        public Ba2FileWrapper(IFile file)
        {
            _file = file;
        }

        public string Path => _file.Path.ToString();

        public uint Size => _file.Size;

        public void CopyDataTo(Stream output)
        {
            _file.CopyDataTo(output).AsTask().Wait();
        }

        public async ValueTask CopyDataToAsync(Stream output)
        {
            await _file.CopyDataTo(output);
        }
    }
}
