using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order.DI;
using Noggog;

namespace Mutagen.Bethesda.Archives.DI;

public interface IGetApplicableArchivePaths
{
    /// <summary>
    /// Enumerates all Archives
    /// </summary>
    /// <param name="returnEmptyIfMissing">If ini file is missing, return empty instead of throwing an exception</param>
    IEnumerable<FilePath> Get(bool returnEmptyIfMissing = true);

    /// <summary>
    /// Enumerates all applicable Archives for a given ModKey<br/>
    /// This call is intended to return Archives related to one specific mod.<br/>
    /// NOTE:  It is currently a bit experimental
    /// </summary>
    /// <param name="modKey">ModKey to query about</param>
    /// <param name="returnEmptyIfMissing">If ini file is missing, return empty instead of throwing an exception</param>
    /// <returns></returns>
    IEnumerable<FilePath> Get(ModKey modKey, bool returnEmptyIfMissing = true);
}

public sealed class GetApplicableArchivePaths : IGetApplicableArchivePaths
{
    private readonly IFileSystem _fileSystem;
    private readonly IGetArchiveIniListings _iniListings;
    private readonly ICheckArchiveApplicability _applicability;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IArchiveExtensionProvider _archiveExtension;
    private readonly ILoadOrderListingsProvider _loadOrderListingsProvider;

    public GetApplicableArchivePaths(
        IFileSystem fileSystem,
        IGetArchiveIniListings iniListings,
        ICheckArchiveApplicability applicability,
        IDataDirectoryProvider dataDirectoryProvider,
        IArchiveExtensionProvider archiveExtension,
        ILoadOrderListingsProvider loadOrderListingsProvider)
    {
        _fileSystem = fileSystem;
        _iniListings = iniListings;
        _applicability = applicability;
        _dataDirectoryProvider = dataDirectoryProvider;
        _archiveExtension = archiveExtension;
        _loadOrderListingsProvider = loadOrderListingsProvider;
    }
        
    /// <inheritdoc />
    public IEnumerable<FilePath> Get(bool returnEmptyIfMissing = true)
    {
        return GetInternal(default(ModKey?), GetPriorityOrderComparer(emptyIfMissing: returnEmptyIfMissing), returnEmptyIfMissing);
    }

    /// <inheritdoc />
    public IEnumerable<FilePath> Get(ModKey modKey, bool returnEmptyIfMissing = true)
    {
        return Get(modKey, GetPriorityOrderComparer(emptyIfMissing: returnEmptyIfMissing), returnEmptyIfMissing);
    }

    private IEnumerable<FilePath> Get(ModKey modKey, IComparer<FileName>? archiveOrdering, bool returnEmptyIfMissing = true)
    {
        return GetInternal(modKey, archiveOrdering, returnEmptyIfMissing);
    }
        
    private IEnumerable<FilePath> GetInternal(ModKey? modKey, IComparer<FileName>? archiveOrdering, bool returnEmptyIfMissing = true)
    {
        if (modKey.HasValue && modKey.Value.IsNull)
        {
            return Enumerable.Empty<FilePath>();
        }

        if (returnEmptyIfMissing && !_fileSystem.Directory.Exists(_dataDirectoryProvider.Path))
        {
            return Enumerable.Empty<FilePath>();
        }
            
        var ret = _fileSystem.Directory.EnumerateFilePaths(_dataDirectoryProvider.Path, searchPattern: $"*{_archiveExtension.Get()}");
        if (modKey != null)
        {
            IReadOnlyCollection<FileName> iniListedArchives;
            if (returnEmptyIfMissing)
            {
                iniListedArchives = (IReadOnlyCollection<FileName>?)_iniListings.TryGet()?.ToHashSet() ?? Array.Empty<FileName>();
            }
            else
            {
                iniListedArchives = _iniListings.Get().ToHashSet();
            }
            ret = ret
                .Where(archive =>
                {
                    if (iniListedArchives.Contains(archive.Name)) return true;
                    return _applicability.IsApplicable(modKey.Value, archive.Name);
                });
        }
        if (archiveOrdering != null)
        {
            return ret.OrderBy(x => x.Name, archiveOrdering);
        }
        return ret;
    }


    private IComparer<FileName>? GetPriorityOrderComparer(bool emptyIfMissing)
    {
        IReadOnlyList<FileName> archiveOrderingList;
        if (emptyIfMissing)
        {
            archiveOrderingList = _iniListings.TryGet()?.ToList() ?? [];
        }
        else
        {
            archiveOrderingList = _iniListings.Get().ToList();
        }
        if (archiveOrderingList.Count == 0) return null;
        archiveOrderingList.Reverse();
        return Comparer<FileName>.Create((a, b) =>
        {
            var indexA = archiveOrderingList.IndexOf(a);
            var indexB = archiveOrderingList.IndexOf(b);
            if (indexA == -1 && indexB == -1) return 0;
            if (indexA == -1) return 1;
            if (indexB == -1) return -1;
            return indexA - indexB;
        });
    }
}