using Mutagen.Bethesda.Plugins.Order.DI;
using Noggog;

namespace Mutagen.Bethesda.Archives.DI;

public class CachedArchiveListingDetailsProvider : IArchiveListingDetailsProvider
{
    private readonly ILoadOrderListingsProvider _listingsProvider;
    private readonly IGetArchiveIniListings _getArchiveIniListings;
    private readonly Lazy<Payload> _payload;

    private class Payload
    {
        public required IReadOnlyList<FileName> Listed { get; init; }
        public required IReadOnlyList<FileName> Priority { get; init; }
        public required IReadOnlySet<FileName> Set { get; init; }
    }
    
    public CachedArchiveListingDetailsProvider(
        ILoadOrderListingsProvider listingsProvider,
        IGetArchiveIniListings getArchiveIniListings,
        IArchiveNameFromModKeyProvider archiveNameFromModKeyProvider)
    {
        _listingsProvider = listingsProvider;
        _getArchiveIniListings = getArchiveIniListings;
        _payload = new Lazy<Payload>(() =>
        {
            var listed = new List<FileName>();
            listed.AddRange(_getArchiveIniListings.TryGet().EmptyIfNull());
            listed.AddRange(_listingsProvider.Get()
                .Where(x => x.Enabled)
                .Select(x => x.ModKey)
                .Select(archiveNameFromModKeyProvider.Get));
            return new Payload()
            {
                Listed = listed,
                Priority = ((IEnumerable<FileName>)listed).Reverse().ToList(),
                Set = listed.ToHashSet(),
            };
        });
    }

    public bool Empty => _payload.Value.Listed.Count == 0;
    
    public int PriorityIndexFor(FileName fileName)
    {
        return _payload.Value.Priority.IndexOf(fileName);
    }
    
    public bool Contains(FileName fileName)
    {
        return _payload.Value.Set.Contains(fileName);
    }
}