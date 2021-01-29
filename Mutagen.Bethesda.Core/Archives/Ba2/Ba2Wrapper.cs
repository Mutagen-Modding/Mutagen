using Compression.BSA;
using Mutagen.Bethesda.Bsa;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wabbajack.Common;

namespace Mutagen.Bethesda.Ba2
{
    class Ba2Wrapper : IArchiveReader
    {
        private readonly BA2Reader _reader;

        public Ba2Wrapper(string path)
        {
            _reader = BA2Reader.Load(new AbsolutePath(path)).Result;
        }

        public bool TryGetFolder(string path, [MaybeNullWhen(false)] out IArchiveFolder folder)
        {
            folder = new Ba2FolderWrapper(
                path,
                _reader.Files
                    .Where(f => f.Path.ToString().StartsWith(path, StringComparison.OrdinalIgnoreCase))
                    .Select(f => new BsaFileWrapper(f)));
            return folder.Files.Count > 0;
        }
    }
}
