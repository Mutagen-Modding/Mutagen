using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
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
    /// Enumerates all Archives
    /// </summary>
    /// <param name="archiveOrdering">Archive ordering overload.  Empty enumerable means no ordering.</param>
    /// <param name="returnEmptyIfMissing">If ini file is missing, return empty instead of throwing an exception</param>
    /// <returns></returns>
    IEnumerable<FilePath> Get(IEnumerable<FileName>? archiveOrdering, bool returnEmptyIfMissing = true);

    /// <summary>
    /// Enumerates all Archives
    /// </summary>
    /// <param name="modOrdering">Archive ordering overload based on a mod order.  Empty enumerable means no ordering.</param>
    /// <param name="returnEmptyIfMissing">If ini file is missing, return empty instead of throwing an exception</param>
    /// <returns></returns>
    IEnumerable<FilePath> Get(IEnumerable<ModKey>? modOrdering, bool returnEmptyIfMissing = true);

    /// <summary>
    /// Enumerates all applicable Archives for a given ModKey<br/>
    /// This call is intended to return Archives related to one specific mod.<br/>
    /// NOTE:  It is currently a bit experimental
    /// </summary>
    /// <param name="modKey">ModKey to query about</param>
    /// <param name="returnEmptyIfMissing">If ini file is missing, return empty instead of throwing an exception</param>
    /// <returns></returns>
    IEnumerable<FilePath> Get(ModKey modKey, bool returnEmptyIfMissing = true);

    /// <summary>
    /// Enumerates all applicable Archives for a given ModKey<br/>
    /// This call is intended to return Archives related to one specific mod.<br/>
    /// NOTE:  It is currently a bit experimental
    /// </summary>
    /// <param name="modKey">ModKey to query about</param>
    /// <param name="archiveOrdering">Archive ordering overload.  Empty enumerable means no ordering.</param>
    /// <param name="returnEmptyIfMissing">If ini file is missing, return empty instead of throwing an exception</param>
    /// <returns></returns>
    IEnumerable<FilePath> Get(ModKey modKey, IEnumerable<FileName>? archiveOrdering, bool returnEmptyIfMissing = true);

    /// <summary>
    /// Enumerates all applicable Archives for a given ModKey<br/>
    /// This call is intended to return Archives related to one specific mod.<br/>
    /// NOTE:  It is currently a bit experimental
    /// </summary>
    /// <param name="modKey">ModKey to query about</param>
    /// <param name="archiveOrdering">How to order the archive paths.  Null for no ordering</param>
    /// <param name="returnEmptyIfMissing">If ini file is missing, return empty instead of throwing an exception</param>
    /// <returns>Full paths of Archives that apply to the given mod and exist</returns>
    IEnumerable<FilePath> Get(ModKey modKey, IComparer<FileName>? archiveOrdering, bool returnEmptyIfMissing = true);
}

public sealed class GetApplicableArchivePaths : IGetApplicableArchivePaths
{
    private readonly IFileSystem _fileSystem;
    private readonly IGetArchiveIniListings _iniListings;
    private readonly ICheckArchiveApplicability _applicability;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IArchiveExtensionProvider _archiveExtension;

    public GetApplicableArchivePaths(
        IFileSystem fileSystem,
        IGetArchiveIniListings iniListings,
        ICheckArchiveApplicability applicability,
        IDataDirectoryProvider dataDirectoryProvider,
        IArchiveExtensionProvider archiveExtension)
    {
        _fileSystem = fileSystem;
        _iniListings = iniListings;
        _applicability = applicability;
        _dataDirectoryProvider = dataDirectoryProvider;
        _archiveExtension = archiveExtension;
    }
        
    /// <inheritdoc />
    public IEnumerable<FilePath> Get(bool returnEmptyIfMissing = true)
    {
        return GetInternal(default(ModKey?), GetPriorityOrderComparer(null, emptyIfMissing: returnEmptyIfMissing), returnEmptyIfMissing);
    }

    /// <inheritdoc />
    public IEnumerable<FilePath> Get(IEnumerable<FileName>? archiveOrdering, bool returnEmptyIfMissing = true)
    {
        return GetInternal(default(ModKey?), GetPriorityOrderComparer(archiveOrdering, emptyIfMissing: returnEmptyIfMissing), returnEmptyIfMissing);
    }

    public IEnumerable<FilePath> Get(IEnumerable<ModKey>? modOrdering, bool returnEmptyIfMissing = true)
    {
        return GetInternal(default, GetPriorityModOrderComparer(modOrdering), returnEmptyIfMissing);
    }

    /// <inheritdoc />
    public IEnumerable<FilePath> Get(ModKey modKey, bool returnEmptyIfMissing = true)
    {
        return Get(modKey, GetPriorityOrderComparer(null, emptyIfMissing: returnEmptyIfMissing), returnEmptyIfMissing);
    }

    /// <inheritdoc />
    public IEnumerable<FilePath> Get(ModKey modKey, IEnumerable<FileName>? archiveOrdering, bool returnEmptyIfMissing = true)
    {
        return Get(modKey, GetPriorityOrderComparer(archiveOrdering, emptyIfMissing: returnEmptyIfMissing), returnEmptyIfMissing);
    }

    /// <inheritdoc />
    public IEnumerable<FilePath> Get(ModKey modKey, IComparer<FileName>? archiveOrdering, bool returnEmptyIfMissing = true)
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


    private IComparer<FileName>? GetPriorityOrderComparer(IEnumerable<FileName>? listedArchiveOrdering, bool emptyIfMissing)
    {
        if (emptyIfMissing)
        {
            listedArchiveOrdering ??= _iniListings.TryGet() ?? Enumerable.Empty<FileName>();
        }
        else
        {
            listedArchiveOrdering ??= _iniListings.Get();
        }
        var archiveOrderingList = listedArchiveOrdering.ToList();
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
    
    private IComparer<FileName>? GetPriorityModOrderComparer(IEnumerable<ModKey>? listedModOrdering)
    {
        if (listedModOrdering is null) return null;

        var iniListedArchives = _iniListings.Get().ToList();
        var modOrderingList = listedModOrdering.ToList();
        return Comparer<FileName>.Create((a, b) =>
        {
            var iniIndexOfA = iniListedArchives.IndexOf(a);
            var iniIndexOfB = iniListedArchives.IndexOf(b);

            // Handle if any of the archives are listed in the ini file
            if (iniIndexOfA != -1 || iniIndexOfB != -1) {
                if (iniIndexOfA == -1) return 1;
                if (iniIndexOfB == -1) return -1;
                return iniIndexOfA - iniIndexOfB;
            }

            // If neither are listed in the ini file, use the mod ordering
            var modIndexA = modOrderingList.FindIndex(modKey => _applicability.IsApplicable(modKey, a));
            var modIndexB = modOrderingList.FindIndex(modKey => _applicability.IsApplicable(modKey, b));
            if (modIndexA == -1 && modIndexB == -1) return 0;
            if (modIndexA == -1) return 1;
            if (modIndexB == -1) return -1;
            return modIndexA - modIndexB;
        });
    }
}