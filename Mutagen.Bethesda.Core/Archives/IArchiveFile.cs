using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Archives
{
    public interface IArchiveFile
    {
        /// <summary>
        /// The path of the file inside the archive
        /// </summary>
        string Path { get; }

        /// <summary>
        /// The uncompressed file size
        /// </summary>
        uint Size { get; }

        /// <summary>
        /// Copies this entry to the given stream
        /// </summary>
        void CopyDataTo(Stream output);

        /// <summary>
        /// Copies this entry to the given stream
        /// </summary>
        ValueTask CopyDataToAsync(Stream output);
    }
}
