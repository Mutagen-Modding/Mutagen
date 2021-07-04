using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Noggog;

namespace Mutagen.Bethesda.Archives.DI
{
    public interface IGetApplicableArchivePaths
    {
        /// <summary>
        /// Enumerates all Archives
        /// </summary>
        IEnumerable<FilePath> Get();

        /// <summary>
        /// Enumerates all Archives
        /// </summary>
        /// <param name="archiveOrdering">Archive ordering overload.  Empty enumerable means no ordering.</param>
        /// <returns></returns>
        IEnumerable<FilePath> Get(IEnumerable<FileName>? archiveOrdering);

        /// <summary>
        /// Enumerates all applicable Archives for a given ModKey<br/>
        /// This call is intended to return Archives related to one specific mod.<br/>
        /// NOTE:  It is currently a bit experimental
        /// </summary>
        /// <param name="modKey">ModKey to query about</param>
        /// <returns></returns>
        IEnumerable<FilePath> Get(ModKey modKey);

        /// <summary>
        /// Enumerates all applicable Archives for a given ModKey<br/>
        /// This call is intended to return Archives related to one specific mod.<br/>
        /// NOTE:  It is currently a bit experimental
        /// </summary>
        /// <param name="modKey">ModKey to query about</param>
        /// <param name="archiveOrdering">Archive ordering overload.  Empty enumerable means no ordering.</param>
        /// <returns></returns>
        IEnumerable<FilePath> Get(ModKey modKey, IEnumerable<FileName>? archiveOrdering);

        /// <summary>
        /// Enumerates all applicable Archives for a given ModKey<br/>
        /// This call is intended to return Archives related to one specific mod.<br/>
        /// NOTE:  It is currently a bit experimental
        /// </summary>
        /// <param name="modKey">ModKey to query about</param>
        /// <param name="archiveOrdering">How to order the archive paths.  Null for no ordering</param>
        /// <returns>Full paths of Archives that apply to the given mod and exist</returns>
        IEnumerable<FilePath> Get(ModKey modKey, IComparer<FileName>? archiveOrdering);
    }

    public class GetApplicableArchivePaths : IGetApplicableArchivePaths
    {
        private readonly IFileSystem _fileSystem;
        private readonly IGetArchiveIniListings _iniListings;
        private readonly ICheckArchiveApplicability _applicability;
        private readonly IGameReleaseContext _gameReleaseContext;
        private readonly IDataDirectoryProvider _dataDirectoryProvider;

        public GetApplicableArchivePaths(
            IFileSystem fileSystem,
            IGetArchiveIniListings iniListings,
            ICheckArchiveApplicability applicability,
            IGameReleaseContext gameReleaseContext,
            IDataDirectoryProvider dataDirectoryProvider)
        {
            _fileSystem = fileSystem;
            _iniListings = iniListings;
            _applicability = applicability;
            _gameReleaseContext = gameReleaseContext;
            _dataDirectoryProvider = dataDirectoryProvider;
        }
        
        /// <inheritdoc />
        public IEnumerable<FilePath> Get()
        {
            return GetInternal(default(ModKey?), GetPriorityOrderComparer(null));
        }

        /// <inheritdoc />
        public IEnumerable<FilePath> Get(IEnumerable<FileName>? archiveOrdering)
        {
            return GetInternal(default(ModKey?), GetPriorityOrderComparer(archiveOrdering));
        }

        /// <inheritdoc />
        public IEnumerable<FilePath> Get(ModKey modKey)
        {
            return Get(modKey, GetPriorityOrderComparer(null));
        }

        /// <inheritdoc />
        public IEnumerable<FilePath> Get(ModKey modKey, IEnumerable<FileName>? archiveOrdering)
        {
            return Get(modKey, GetPriorityOrderComparer(archiveOrdering));
        }

        /// <inheritdoc />
        public IEnumerable<FilePath> Get(ModKey modKey, IComparer<FileName>? archiveOrdering)
        {
            return GetInternal(modKey, archiveOrdering);
        }
        
        private IEnumerable<FilePath> GetInternal(ModKey? modKey, IComparer<FileName>? archiveOrdering)
        {
            if (modKey.HasValue && modKey.Value.IsNull)
            {
                return Enumerable.Empty<FilePath>();
            }
            
            var ret = _fileSystem.Directory.EnumerateFilePaths(_dataDirectoryProvider.Path, searchPattern: $"*{Archive.GetExtension(_gameReleaseContext.Release)}");
            if (modKey != null)
            {
                var iniListedArchives = _iniListings.Get().ToHashSet();
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

        private IComparer<FileName>? GetPriorityOrderComparer(IEnumerable<FileName>? listedArchiveOrdering)
        {
            listedArchiveOrdering ??= _iniListings.Get();
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
    }
}