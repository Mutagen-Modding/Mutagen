using Compression.BSA;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wabbajack.Common;

namespace Mutagen.Bethesda.Bsa
{
    class BsaWrapper : IArchiveReader
    {
        private readonly BSAReader _reader;

        public BsaWrapper(string path)
        {
            _reader = BSAReader.Load(new AbsolutePath(path, skipValidation: true));
        }

        public bool TryGetFolder(string path, [MaybeNullWhen(false)] out IArchiveFolder folder)
        {
            if (_reader.TryGetFolder(path, out var internalFolder))
            {
                folder = new BsaFolderWrapper(internalFolder);
                return true;
            }
            folder = default;
            return false;
        }
    }
}
