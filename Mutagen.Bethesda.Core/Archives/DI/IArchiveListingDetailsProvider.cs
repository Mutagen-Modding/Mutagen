using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order.DI;
using Noggog;

namespace Mutagen.Bethesda.Archives.DI;

public interface IArchiveListingDetailsProvider
{
    bool Contains(FileName fileName);
    bool IsIni(FileName fileName);
    IComparer<FileName> GetComparerFor(ModKey? modKey);
}

public class CachedArchiveListingDetailsProvider : IArchiveListingDetailsProvider
{
    private readonly IGetArchiveIniListings _getArchiveIniListings;
    private readonly Lazy<Payload> _payload;

    private class Payload
    {
        public required IReadOnlyDictionary<FileName, int> ListedPriority { get; init; }
        public required IReadOnlySet<FileName> IniSet { get; init; }
        public required IReadOnlyDictionary<FileName, int> IniPriority { get; init; }
        public required IReadOnlySet<FileName> AllSet { get; init; }
    }
    
    public CachedArchiveListingDetailsProvider(
        ILoadOrderListingsProvider listingsProvider,
        IGetArchiveIniListings getArchiveIniListings,
        IArchiveNameFromModKeyProvider archiveNameFromModKeyProvider)
    {
        _getArchiveIniListings = getArchiveIniListings;
        _payload = new Lazy<Payload>(() =>
        {
            var ini = new List<FileName>(_getArchiveIniListings.TryGet().EmptyIfNull());
            var listed = new List<FileName>(
                listingsProvider.Get()
                    .Where(x => x.Enabled)
                    .Select(x => x.ModKey)
                    .Select(archiveNameFromModKeyProvider.Get));
            return new Payload()
            {
                IniSet = ini.ToHashSet(),
                IniPriority = Priority(ini),
                ListedPriority = Priority(listed),
                AllSet = listed.And(ini).ToHashSet(),
            };
        });
    }

    private IReadOnlyDictionary<FileName, int> Priority(IEnumerable<FileName> e)
    {
        return e
            .Distinct()
            .Reverse()
            .WithIndex()
            .ToDictionary(x => x.Item, x => x.Index);
    }

    public bool Contains(FileName fileName)
    {
        var strippedFileName = BsaWithoutSuffix(fileName, out _);
        return _payload.Value.AllSet.Contains(strippedFileName);
    }
    
    public bool IsIni(FileName fileName)
    {
        return _payload.Value.IniSet.Contains(fileName);
    }
    
    public IComparer<FileName> GetComparerFor(ModKey? modKey)
    {
        return new Comparer(modKey, this);
    }
    
    private FileName BsaWithoutSuffix(FileName fileName, out string? suffix)
    {
        var lastIndexOfDelim = fileName.String.LastIndexOf(" - ", StringComparison.OrdinalIgnoreCase);
        if (lastIndexOfDelim == -1)
        {
            suffix = null;
            return fileName;
        }
        suffix = fileName.String.Substring(lastIndexOfDelim + 3);
        return new FileName(fileName.String.Substring(0, lastIndexOfDelim));
    }
    

    private class Comparer : IComparer<FileName>
    {
        private readonly ModKey? _modKey;
        private readonly CachedArchiveListingDetailsProvider _d;
        
        public Comparer(
            ModKey? modKey,
            CachedArchiveListingDetailsProvider detailsProvider)
        {
            _modKey = modKey;
            _d = detailsProvider;
        }
        
        private int FindListedIndex(FileName fileName, out string? suffix)
        {
            if (_d._payload.Value.ListedPriority.TryGetValue(fileName, out var index))
            {
                suffix = null;
                return index;
            }
        
            var strippedFileName = _d.BsaWithoutSuffix(fileName, out suffix);
            if (_d._payload.Value.ListedPriority.TryGetValue(strippedFileName, out index))
            {
                return index;
            }

            if (_modKey.HasValue && _modKey.Value.FileName.NameWithoutExtension.Equals(strippedFileName.NameWithoutExtension, StringComparison.OrdinalIgnoreCase))
            {
                return int.MaxValue;
            }
            
            return _d._payload.Value.ListedPriority[strippedFileName];
        }
        
        public int Compare(FileName x, FileName y)
        {
            var payload = _d._payload.Value;
            var iniX = payload.IniSet.Contains(x);
            var iniY = payload.IniSet.Contains(y);
            if (iniX && iniY)
            {
                return payload.IniPriority[y].CompareTo(payload.IniPriority[x]);
            }
            if (iniX || iniY)
            {
                if (iniX) return -1;
                if (iniY) return 1;
                throw new NotImplementedException();
            }
            var listedX = FindListedIndex(x, out var suffixX);
            var listedY = FindListedIndex(y, out var suffixY);
            if (listedY != listedX)
            {
                return listedY.CompareTo(listedX);
            }

            if (suffixX == suffixY) throw new NotImplementedException();
            if (suffixX == null) return -1;
            if (suffixY == null) return 1;
            return String.Compare(suffixY, suffixX, StringComparison.InvariantCulture);
        }
    }
}