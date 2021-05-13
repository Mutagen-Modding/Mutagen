using System.Collections.Generic;
using Noggog;

namespace Mutagen.Bethesda.Archives
{
    public interface IArchiveFolder
    {
        DirectoryPath? Path { get; }
        IReadOnlyCollection<IArchiveFile> Files { get; }
    }
}
