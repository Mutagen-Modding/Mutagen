using System.Collections.Generic;
using System.Linq;
using Noggog;

namespace Mutagen.Bethesda.Archives.Ba2
{
    class Ba2FolderWrapper : IArchiveFolder
    {
        public IReadOnlyCollection<IArchiveFile> Files { get; }

        public DirectoryPath? Path { get; }

        public Ba2FolderWrapper(DirectoryPath path, IEnumerable<IArchiveFile> files)
        {
            Path = path;
            Files = files.ToList();
        }
    }
}
