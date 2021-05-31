using System.Collections.Generic;
using Noggog;

namespace Mutagen.Bethesda.Archives
{
    public interface IArchiveFolder
    {
        string? Path { get; }
        IReadOnlyCollection<IArchiveFile> Files { get; }
    }
}
