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
    IEnumerable<FilePath> Get();

    /// <summary>
    /// Enumerates all applicable Archives for a given ModKey<br/>
    /// This call is intended to return Archives related to one specific mod.<br/>
    /// NOTE:  It is currently a bit experimental
    /// </summary>
    /// <param name="modKey">ModKey to query about</param>
    /// <returns></returns>
    IEnumerable<FilePath> Get(ModKey modKey);
}

public sealed class GetApplicableArchivePaths : IGetApplicableArchivePaths
{
    private readonly IFileSystem _fileSystem;
    private readonly ICheckArchiveApplicability _applicability;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IArchiveExtensionProvider _archiveExtension;
    private readonly IArchiveListingDetailsProvider _archiveListingDetailsProvider;

    public GetApplicableArchivePaths(
        IFileSystem fileSystem,
        ICheckArchiveApplicability applicability,
        IDataDirectoryProvider dataDirectoryProvider,
        IArchiveExtensionProvider archiveExtension,
        IArchiveListingDetailsProvider ArchiveListingDetailsProvider)
    {
        _fileSystem = fileSystem;
        _applicability = applicability;
        _dataDirectoryProvider = dataDirectoryProvider;
        _archiveExtension = archiveExtension;
        _archiveListingDetailsProvider = ArchiveListingDetailsProvider;
    }
        
    /// <inheritdoc />
    public IEnumerable<FilePath> Get()
    {
        return GetInternal(default(ModKey?), GetPriorityOrderComparer());
    }

    /// <inheritdoc />
    public IEnumerable<FilePath> Get(ModKey modKey)
    {
        return Get(modKey, GetPriorityOrderComparer());
    }

    private IEnumerable<FilePath> Get(ModKey modKey, IComparer<FileName>? archiveOrdering)
    {
        return GetInternal(modKey, archiveOrdering);
    }
        
    private IEnumerable<FilePath> GetInternal(ModKey? modKey, IComparer<FileName>? archiveOrdering)
    {
        if (modKey.HasValue && modKey.Value.IsNull)
        {
            return [];
        }

        if (!_fileSystem.Directory.Exists(_dataDirectoryProvider.Path))
        {
            return [];
        }
            
        var ret = _fileSystem.Directory.EnumerateFilePaths(_dataDirectoryProvider.Path, searchPattern: $"*{_archiveExtension.Get()}");
        if (modKey != null)
        {
            ret = ret
                .Where(archive =>
                {
                    if (_archiveListingDetailsProvider.Contains(archive.Name)) return true;
                    return _applicability.IsApplicable(modKey.Value, archive.Name);
                });
        }
        if (archiveOrdering != null)
        {
            return ret.OrderBy(x => x.Name, archiveOrdering);
        }
        return ret;
    }


    private IComparer<FileName>? GetPriorityOrderComparer()
    {
        if (_archiveListingDetailsProvider.Empty) return null;
        return Comparer<FileName>.Create((a, b) =>
        {
            var indexA = _archiveListingDetailsProvider.PriorityIndexFor(a);
            var indexB = _archiveListingDetailsProvider.PriorityIndexFor(b);
            if (indexA == -1 && indexB == -1) return 0;
            if (indexA == -1) return 1;
            if (indexB == -1) return -1;
            return indexA - indexB;
        });
    }
}