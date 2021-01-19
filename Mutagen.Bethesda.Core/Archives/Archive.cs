using Mutagen.Bethesda.Ba2;
using Mutagen.Bethesda.Bsa;
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
                    return new BsaWrapper(path);
                case GameRelease.Fallout4:
                    return new Ba2Wrapper(path);
                default:
                    throw new NotImplementedException();
            }
        }

        public static IEnumerable<string> GetApplicableArchivePaths(GameRelease release, string dataFolderPath, ModKey modKey)
        {
            return Directory.EnumerateFiles(dataFolderPath, $"{modKey.Name}*{GetArchiveExtension(release)}");
        }
    }
}
