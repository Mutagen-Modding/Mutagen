using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Mutagen.Bethesda.Plugins;
using Noggog;

namespace Mutagen.Bethesda.Archives
{
    public interface IGetApplicableArchivePaths
    {
        /// <summary>
        /// Enumerates all Archives for a given release that are within a given dataFolderPath.
        /// </summary>
        /// <param name="release">GameRelease to query for</param>
        /// <param name="dataFolderPath">Folder to query within</param>
        /// <returns></returns>
        IEnumerable<FilePath> Get(GameRelease release, DirectoryPath dataFolderPath);

        /// <summary>
        /// Enumerates all Archives for a given release that are within a given dataFolderPath.
        /// </summary>
        /// <param name="release">GameRelease to query for</param>
        /// <param name="dataFolderPath">Folder to query within</param>
        /// <param name="archiveOrdering">Archive ordering overload.  Empty enumerable means no ordering.</param>
        /// <returns></returns>
        IEnumerable<FilePath> Get(GameRelease release, DirectoryPath dataFolderPath, IEnumerable<FileName>? archiveOrdering);

        /// <summary>
        /// Enumerates all applicable Archives for a given release and ModKey that are within a given dataFolderPath.<br/>
        /// This call is intended to return Archives related to one specific mod.<br/>
        /// NOTE:  It is currently a bit experimental
        /// </summary>
        /// <param name="release">GameRelease to query for</param>
        /// <param name="dataFolderPath">Folder to query within</param>
        /// <param name="modKey">ModKey to query about</param>
        /// <returns></returns>
        IEnumerable<FilePath> Get(GameRelease release, DirectoryPath dataFolderPath, ModKey modKey);

        /// <summary>
        /// Enumerates all applicable Archives for a given release and ModKey that are within a given dataFolderPath.<br/>
        /// This call is intended to return Archives related to one specific mod.<br/>
        /// NOTE:  It is currently a bit experimental
        /// </summary>
        /// <param name="release">GameRelease to query for</param>
        /// <param name="dataFolderPath">Folder to query within</param>
        /// <param name="modKey">ModKey to query about</param>
        /// <param name="archiveOrdering">Archive ordering overload.  Empty enumerable means no ordering.</param>
        /// <returns></returns>
        IEnumerable<FilePath> Get(GameRelease release, DirectoryPath dataFolderPath, ModKey modKey, IEnumerable<FileName>? archiveOrdering);

        /// <summary>
        /// Enumerates all applicable Archives for a given release and ModKey that are within a given dataFolderPath.<br/>
        /// This call is intended to return Archives related to one specific mod.<br/>
        /// NOTE:  It is currently a bit experimental
        /// </summary>
        /// <param name="release">GameRelease to query for</param>
        /// <param name="dataFolderPath">Folder to query within</param>
        /// <param name="modKey">ModKey to query about</param>
        /// <param name="archiveOrdering">How to order the archive paths.  Null for no ordering</param>
        /// <returns>Full paths of Archives that apply to the given mod and exist</returns>
        IEnumerable<FilePath> Get(GameRelease release, DirectoryPath dataFolderPath, ModKey modKey, IComparer<FileName>? archiveOrdering);
    }

    public class GetApplicableArchivePaths : IGetApplicableArchivePaths
    {
        private readonly IFileSystem _fileSystem;
        private readonly IGetArchiveIniListings _iniListings;
        private readonly ICheckArchiveApplicability _applicability;

        public GetApplicableArchivePaths(
            IFileSystem fileSystem,
            IGetArchiveIniListings iniListings,
            ICheckArchiveApplicability applicability)
        {
            _fileSystem = fileSystem;
            _iniListings = iniListings;
            _applicability = applicability;
        }
        
        /// <inheritdoc />
        public IEnumerable<FilePath> Get(GameRelease release, DirectoryPath dataFolderPath)
        {
            return GetInternal(release, dataFolderPath, default(ModKey?), GetPriorityOrderComparer(release));
        }

        /// <inheritdoc />
        public IEnumerable<FilePath> Get(GameRelease release, DirectoryPath dataFolderPath, IEnumerable<FileName>? archiveOrdering)
        {
            return GetInternal(release, dataFolderPath, default(ModKey?), GetPriorityOrderComparer(release, archiveOrdering));
        }

        /// <inheritdoc />
        public IEnumerable<FilePath> Get(GameRelease release, DirectoryPath dataFolderPath, ModKey modKey)
        {
            return Get(release, dataFolderPath, modKey, GetPriorityOrderComparer(release));
        }

        /// <inheritdoc />
        public IEnumerable<FilePath> Get(GameRelease release, DirectoryPath dataFolderPath, ModKey modKey, IEnumerable<FileName>? archiveOrdering)
        {
            return Get(release, dataFolderPath, modKey, GetPriorityOrderComparer(release, archiveOrdering));
        }

        /// <inheritdoc />
        public IEnumerable<FilePath> Get(GameRelease release, DirectoryPath dataFolderPath, ModKey modKey, IComparer<FileName>? archiveOrdering)
        {
            return GetInternal(release, dataFolderPath, modKey, archiveOrdering);
        }
        
        private IEnumerable<FilePath> GetInternal(GameRelease release, DirectoryPath dataFolderPath, ModKey? modKey, IComparer<FileName>? archiveOrdering)
        {
            if (modKey.HasValue && modKey.Value.IsNull)
            {
                return Enumerable.Empty<FilePath>();
            }
            
            var ret = _fileSystem.Directory.EnumerateFilePaths(dataFolderPath, searchPattern: $"*{Archive.GetExtension(release)}");
            if (modKey != null)
            {
                var iniListedArchives = _iniListings.Get(release).ToHashSet();
                ret = ret
                    .Where(archive =>
                    {
                        if (iniListedArchives.Contains(archive.Name)) return true;
                        return _applicability.IsApplicable(release, modKey.Value, archive.Name);
                    });
            }
            if (archiveOrdering != null)
            {
                return ret.OrderBy(x => x.Name, archiveOrdering);
            }
            return ret;
        }

        private IComparer<FileName>? GetPriorityOrderComparer(GameRelease release, IEnumerable<FileName>? listedArchiveOrdering = null)
        {
            return GetPriorityOrderComparer(listedArchiveOrdering ?? _iniListings.Get(release));
        }

        private IComparer<FileName>? GetPriorityOrderComparer(IEnumerable<FileName> listedArchiveOrdering)
        {
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