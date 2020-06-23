using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// Interface for IBinaryReadStream enhanced with Mutagen specific functionality
    /// </summary>
    public interface IMutagenReadStream : IBinaryReadStream
    {
        /// <summary>
        /// All the extra meta bits for parsing
        /// </summary>
        ParsingBundle MetaData { get; }

        /// <summary>
        /// Convenience offset tracker variable for helping print meaningful position information
        /// relative to an original source file.  Only used if a stream gets reframed.
        /// </summary>
        long OffsetReference { get; }

        /// <summary>
        /// Reads an amount of bytes into an internal array and returns a new stream wrapping those bytes.
        /// OffsetReference is updated to be aligned to the original source starting position.
        /// This call will advance the source stream by the number of bytes.
        /// The returned stream will be ready to read and start at its Position 0.
        /// </summary>
        /// <param name="length">Number of bytes to read and reframe</param>
        /// <returns>A new stream wrapping an internal array, set to position 0.</returns>
        IMutagenReadStream ReadAndReframe(int length);
    }
}
