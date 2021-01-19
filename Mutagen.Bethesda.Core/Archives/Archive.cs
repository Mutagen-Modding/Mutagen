using Mutagen.Bethesda.Bsa;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
