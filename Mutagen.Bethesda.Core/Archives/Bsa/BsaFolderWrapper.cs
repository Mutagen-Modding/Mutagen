using Compression.BSA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Bsa
{
    class BsaFolderWrapper : IArchiveFolder
    {
        private readonly IFolder _folder;

        public BsaFolderWrapper(IFolder folder)
        {
            _folder = folder;
        }

        public IReadOnlyCollection<IArchiveFile> Files => _folder.Files.Select(x => new BsaFileWrapper(x)).ToList();

        public string? Path => _folder.Name;
    }
}
