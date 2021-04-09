using IniParser;
using Mutagen.Bethesda.Ba2;
using Mutagen.Bethesda.Bsa;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public interface IArchiveReader
    {
        bool TryGetFolder(string path, [MaybeNullWhen(false)] out IArchiveFolder folder);
        IEnumerable<IArchiveFile> Files { get; }
    }

    public static class Archive
    {
        public static string GetArchiveExtension(GameRelease release)
        {
            switch (release.ToCategory())
            {
                case GameCategory.Oblivion:
                case GameCategory.Skyrim:
                    return ".bsa";
                case GameCategory.Fallout4:
                    return ".ba2";
                default:
                    throw new NotImplementedException();
            }
        }

        public static IArchiveReader CreateReader(GameRelease release, string path)
        {
            switch (release)
            {
                case GameRelease.Oblivion:
                case GameRelease.SkyrimLE:
                case GameRelease.SkyrimSE:
                case GameRelease.SkyrimVR:
                    return new BsaReader(path);
                case GameRelease.Fallout4:
                    return new Ba2Reader(path);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Enumerates all applicable BSAs for a given release and ModKey that are within a given dataFolderPath.
        /// Orders the results to a BSA load order driven by the ini
        /// </summary>
        /// <param name="release">GameRelease to query for</param>
        /// <param name="dataFolderPath">Folder to query within</param>
        /// <param name="modKey">ModKey to query about</param>
        /// <returns></returns>
        public static IEnumerable<string> GetApplicableArchivePaths(GameRelease release, string dataFolderPath, ModKey modKey)
        {
            return GetApplicableArchivePaths(release, dataFolderPath, modKey, GetPriorityOrderComparer(release, null));
        }

        /// <summary>
        /// Enumerates all applicable BSAs for a given release and ModKey that are within a given dataFolderPath.
        /// </summary>
        /// <param name="release">GameRelease to query for</param>
        /// <param name="dataFolderPath">Folder to query within</param>
        /// <param name="modKey">ModKey to query about</param>
        /// <param name="bsaOrdering">BSA ordering overload.  Empty enumerable means no ordering.</param>
        /// <returns></returns>
        public static IEnumerable<string> GetApplicableArchivePaths(GameRelease release, string dataFolderPath, ModKey modKey, IEnumerable<string> bsaOrdering)
        {
            return GetApplicableArchivePaths(release, dataFolderPath, modKey, GetPriorityOrderComparer(release, bsaOrdering));
        }

        /// <summary>
        /// Enumerates all applicable BSAs for a given release and ModKey that are within a given dataFolderPath.
        /// Orders the results to a BSA load order driven by the ini
        /// </summary>
        /// <param name="release">GameRelease to query for</param>
        /// <param name="dataFolderPath">Folder to query within</param>
        /// <param name="modKey">ModKey to query about</param>
        /// <param name="bsaOrdering">How to order the bsa archive paths.  Null for no ordering</param>
        /// <returns></returns>
        public static IEnumerable<string> GetApplicableArchivePaths(GameRelease release, string dataFolderPath, ModKey modKey, IComparer<string>? bsaOrdering)
        {
            var ret = Directory.EnumerateFiles(dataFolderPath, $"*{GetArchiveExtension(release)}");
            if (bsaOrdering != null)
            {
                var ret2 = ret.OrderBy(x => x, bsaOrdering);
                return ret2;
            }
            return ret;
        }

        public static IEnumerable<string> GetTypicalOrder(GameRelease release)
        {
            var iniPath = Ini.GetTypicalPath(release);
            var parser = new FileIniDataParser();
            var data = parser.ReadFile(iniPath);
            var basePath = data["Archive"];
            var str1 = basePath["sResourceArchiveList"]?.Split(", ");
            var str2 = basePath["sResourceArchiveList2"]?.Split(", ");
            var ret = str1.EmptyIfNull().And(str2.EmptyIfNull()).ToList();
            return ret;
        }

        public static IComparer<string>? GetPriorityOrderComparer(GameRelease release, IEnumerable<string>? bsaOrdering = null)
        {
            bsaOrdering ??= GetTypicalOrder(release);
            var bsaOrderingList = bsaOrdering.ToList();
            if (bsaOrderingList.Count == 0) return null;
            bsaOrderingList.Reverse();
            return Comparer<string>.Create((a, b) =>
            {
                var indexA = bsaOrderingList.IndexOf(Path.GetFileName(a), (x, y) => string.Equals(x, y, StringComparison.OrdinalIgnoreCase));
                var indexB = bsaOrderingList.IndexOf(Path.GetFileName(b), (x, y) => string.Equals(x, y, StringComparison.OrdinalIgnoreCase));
                return indexA - indexB;
            });
        }
    }
}
