using Compression.BSA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Bsa
{
    class BsaFileWrapper : IArchiveFile
    {
        private readonly IFile _file;

        public BsaFileWrapper(IFile file)
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
