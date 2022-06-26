using Noggog;

namespace Mutagen.Bethesda.Archives;

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
    /// Retrieves the data and returns it as a new array of bytes
    /// </summary>
    /// <returns>New array of bytes containing data</returns>
    byte[] GetBytes();

    /// <summary>
    /// Retrieves the data and returns it as a readonly span that may be part of a larger memory block
    /// </summary>
    /// <returns>span containing data that may be part of a larger memory block</returns>
    ReadOnlySpan<byte> GetSpan();

    /// <summary>
    /// Retrieves the data and returns it as a readonly span that may be part of a larger memory block
    /// </summary>
    /// <returns>span containing data that may be part of a larger memory block</returns>
    ReadOnlyMemorySlice<byte> GetMemorySlice();

    /// <summary>
    /// Retrieves the data as a Stream
    /// </summary>
    /// <returns>File data as a Stream</returns>
    Stream AsStream();
}