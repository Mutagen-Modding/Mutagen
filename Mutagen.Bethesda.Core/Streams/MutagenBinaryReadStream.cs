using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// A class that wraps a stream with Mutagen-specific binary reading functionality
    /// </summary>
    public class MutagenBinaryReadStream : BinaryReadStream, IMutagenReadStream
    {
        private readonly string? _path;

        /// <summary>
        /// Convenience offset tracker variable for helping print meaningful position information
        /// relative to an original source file.  Only used if a stream gets reframed.
        /// </summary>
        public long OffsetReference { get; }
        
        /// <summary>
        /// Game constants meta object to reference for header length measurements
        /// </summary>
        public GameConstants MetaData { get; }
        
        /// <summary>
        /// Optional MasterReferenceReader to reference while reading
        /// </summary>
        public MasterReferenceReader? MasterReferences { get; set; }

        /// <summary>
        /// Constructor that opens a read stream to a path
        /// </summary>
        /// <param name="path">Path to read from</param>
        /// <param name="metaData">Game constants meta object to reference for header length measurements</param>
        /// <param name="masterReferences">Optional MasterReferenceReader to reference while reading</param>
        /// <param name="bufferSize">Size of internal buffer</param>
        /// <param name="offsetReference">Optional offset reference position to use</param>
        public MutagenBinaryReadStream(
            string path, 
            GameConstants metaData, 
            MasterReferenceReader? masterReferences = null,
            int bufferSize = 4096, 
            long offsetReference = 0)
            : base(path, bufferSize)
        {
            this._path = path;
            this.MetaData = metaData;
            this.MasterReferences = masterReferences;
            this.OffsetReference = offsetReference;
        }

        /// <summary>
        /// Constructor that wraps an existing stream
        /// </summary>
        /// <param name="stream">Stream to wrap and read from</param>
        /// <param name="metaData">Game constants meta object to reference for header length measurements</param>
        /// <param name="masterReferences">Optional MasterReferenceReader to reference while reading</param>
        /// <param name="bufferSize">Size of internal buffer</param>
        /// <param name="offsetReference">Optional offset reference position to use</param>
        public MutagenBinaryReadStream(
            Stream stream, 
            GameConstants metaData,
            MasterReferenceReader? masterReferences = null,
            int bufferSize = 4096, 
            bool dispose = true, 
            long offsetReference = 0)
            : base(stream, bufferSize, dispose)
        {
            this.MetaData = metaData;
            this.MasterReferences = masterReferences;
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
            var offset = this.OffsetReference + this.Position;
            return new MutagenMemoryReadStream(this.ReadBytes(length), this.MetaData, this.MasterReferences, offsetReference: offset);
        }

        public override string ToString()
        {
            return $"{(_path == null ? " " : $"{_path}: ")}{this._stream.Position}-{this._stream.Length} ({this._stream.Remaining()})";
        }
    }
}
