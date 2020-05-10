using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// A class that wraps an array or span directly with Mutagen-specific binary reading functionality
    /// </summary>
    public class MutagenMemoryReadStream : BinaryMemoryReadStream, IMutagenReadStream
    {
        /// <inheritdoc/>
        public long OffsetReference { get; }

        /// <inheritdoc/>
        public GameConstants MetaData { get; }

        /// <inheritdoc/>
        public MasterReferenceReader? MasterReferences { get; set; }

        /// <inheritdoc/>
        public RecordInfoCache? RecordInfoCache { get; set; }

        /// <summary>
        /// Constructor that wraps an array
        /// </summary>
        /// <param name="data">Array to wrap and read from</param>
        /// <param name="metaData">Game constants meta object to reference for header length measurements</param>
        /// <param name="masterReferences">Optional MasterReferenceReader to reference while reading</param>
        /// <param name="offsetReference">Optional offset reference position to use</param>
        public MutagenMemoryReadStream(
            byte[] data, 
            GameConstants metaData,
            MasterReferenceReader? masterReferences = null,
            long offsetReference = 0)
            : base(data)
        {
            this.MetaData = metaData;
            this.MasterReferences = masterReferences;
            this.OffsetReference = offsetReference;
        }

        /// <summary>
        /// Constructor that wraps a memory slice
        /// </summary>
        /// <param name="data">Span to wrap and read from</param>
        /// <param name="metaData">Game constants meta object to reference for header length measurements</param>
        /// <param name="masterReferences">Optional MasterReferenceReader to reference while reading</param>
        /// <param name="infoCache">Optional RecordInfoCache to reference while readingg</param>
        /// <param name="offsetReference">Optional offset reference position to use</param>
        public MutagenMemoryReadStream(
            ReadOnlyMemorySlice<byte> data, 
            GameConstants metaData,
            MasterReferenceReader? masterReferences = null,
            RecordInfoCache? infoCache = null,
            long offsetReference = 0)
            : base(data)
        {
            this.MetaData = metaData;
            this.MasterReferences = masterReferences;
            this.RecordInfoCache = infoCache;
            this.OffsetReference = offsetReference;
        }

        /// <summary>
        /// Reads an amount of bytes into an internal array and returns a new stream wrapping those bytes.
        /// OffsetReference is updated to be aligned to the original source starting position.
        /// This call will advance the source stream by the number of bytes.
        /// The returned stream will be ready to read and start at its Position 0.
        /// </summary>
        /// <param name="length">Number of bytes to read and reframe</param>
        /// <returns>A new stream wrapping an internal array, set to position 0.</returns>
        public IMutagenReadStream ReadAndReframe(int length)
        {
            return new MutagenMemoryReadStream(
                this.Data, 
                this.MetaData, 
                this.MasterReferences, 
                offsetReference: this.OffsetReference + this.Position,
                infoCache: this.RecordInfoCache);
        }
    }
}
