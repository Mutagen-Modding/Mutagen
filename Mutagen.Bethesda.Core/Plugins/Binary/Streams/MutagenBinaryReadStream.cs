using Noggog;
using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Binary.Streams;

/// <summary>
/// A class that wraps a stream with Mutagen-specific binary reading functionality
/// </summary>
public sealed class MutagenBinaryReadStream : BinaryReadStream, IMutagenReadStream
{
    private readonly FilePath _path;

    /// <inheritdoc/>
    public long OffsetReference { get; }

    /// <inheritdoc/>
    public ParsingMeta MetaData { get; }

    /// <summary>
    /// Constructor that opens a read stream to a path
    /// </summary>
    /// <param name="path">Path to read from</param>
    /// <param name="metaData">Bundle of all related metadata for parsing</param>
    /// <param name="bufferSize">Size of internal buffer</param>
    /// <param name="offsetReference">Optional offset reference position to use</param>
    /// <param name="fileSystem">FileSystem to read from</param>
    public MutagenBinaryReadStream(
        FilePath path,
        ParsingMeta metaData,
        int bufferSize = 4096,
        long offsetReference = 0,
        IFileSystem? fileSystem = null)
        : base(fileSystem.GetOrDefault().File.OpenRead(path.Path), bufferSize)
    {
        _path = path;
        MetaData = metaData;
        OffsetReference = offsetReference;
    }

    /// <summary>
    /// Constructor that opens a read stream to a path
    /// </summary>
    /// <param name="path">Path to read from</param>
    /// <param name="release">Game Release the stream is for</param>
    /// <param name="loadOrder">Load Order for reference.  Required if Game has separated load order systems</param>
    /// <param name="bufferSize">Size of internal buffer</param>
    /// <param name="offsetReference">Optional offset reference position to use</param>
    /// <param name="fileSystem">FileSystem to read from</param>
    public MutagenBinaryReadStream(
        ModPath path,
        GameRelease release,
        ILoadOrderGetter<IModFlagsGetter>? loadOrder,
        int bufferSize = 4096,
        long offsetReference = 0,
        IFileSystem? fileSystem = null)
        : base(fileSystem.GetOrDefault().File.OpenRead(path), bufferSize)
    {
        MetaData = new ParsingMeta(
            release, 
            path.ModKey,
            SeparatedMasterPackage.Factory(release, path, loadOrder, fileSystem));
        OffsetReference = offsetReference;
    }

    /// <summary>
    /// Constructor that wraps an existing stream
    /// </summary>
    /// <param name="stream">Stream to wrap and read from</param>
    /// <param name="metaData">Bundle of all related metadata for parsing</param>
    /// <param name="bufferSize">Size of internal buffer</param>
    /// <param name="dispose">Whether to dispose the source stream</param>
    /// <param name="offsetReference">Optional offset reference position to use</param>
    public MutagenBinaryReadStream(
        Stream stream,
        ParsingMeta metaData,
        int bufferSize = 4096,
        bool dispose = true,
        long offsetReference = 0)
        : base(stream, bufferSize, dispose)
    {
        MetaData = metaData;
        OffsetReference = offsetReference;
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
        var offset = OffsetReference + Position;
        return new MutagenMemoryReadStream(
            ReadMemory(length, readSafe: true),
            MetaData, 
            offsetReference: offset);
    }

    public override string ToString()
    {
        return $"{_path}{_stream.Position}-{_stream.Length} ({_stream.Remaining()})";
    }
}