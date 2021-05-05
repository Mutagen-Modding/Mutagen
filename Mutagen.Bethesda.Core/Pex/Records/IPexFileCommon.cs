using System;
using System.IO;
using Noggog;

namespace Mutagen.Bethesda.Pex.Records
{
    public interface IPexFileCommonGetter
    {
        ReadOnlyMemorySlice<String?> UserFlags { get; }

        /// <summary>
        /// Exports to disk in Bethesda Pex binary format.
        /// </summary>
        /// <param name="path">Path to export to</param>
        void WriteToBinary(string path);

        /// <summary>
        /// Exports to disk in Bethesda Pex binary format.
        /// </summary>
        /// <param name="stream">Stream to export to</param>
        void WriteToBinary(Stream stream);
    }

    public interface IPexFileCommon : IPexFileCommonGetter
    {
        new String?[] UserFlags { get; }
    }
}
