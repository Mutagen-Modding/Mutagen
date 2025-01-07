using Noggog;

namespace Mutagen.Bethesda.Archives.DI;

public interface IArchiveListingDetailsProvider
{
    bool Empty { get; }
    int PriorityIndexFor(FileName fileName);
    bool Contains(FileName fileName);
}