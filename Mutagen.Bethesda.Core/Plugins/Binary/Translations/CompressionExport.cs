using System.Buffers.Binary;
using System.IO.Compression;
using System.Reactive.Disposables;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public readonly struct CompressionExport : IDisposable
{
    private readonly MemoryTributary _tributary = new();
    
    /// <summary>
    /// Writer being tracked
    /// </summary>
    public readonly MutagenWriter OriginalWriter;
    
    /// <summary>
    /// Writer being used for compression
    /// </summary>
    public MutagenWriter CompressionWriter { get; }

    private CompressionExport(
        MutagenWriter writer)
    {
        OriginalWriter = writer;
        CompressionWriter = new MutagenWriter(_tributary, writer.MetaData);
    }

    public static IDisposable Compression(
        bool isCompressed,
        MutagenWriter writer,
        out MutagenWriter writerToUse)
    {
        if (!isCompressed)
        {
            writerToUse = writer;
            return Disposable.Empty;
        }

        var ret = new CompressionExport(writer);
        writerToUse = ret.CompressionWriter;
        return ret;
    }
    
    public void Dispose()
    {
        Span<byte> b = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(b, (uint)_tributary.Length);
        OriginalWriter.BaseStream.Write(b);
        using var stream = new ZLibStream(OriginalWriter.BaseStream, CompressionMode.Compress);
        _tributary.Position = 0;
        _tributary.CopyTo(stream);
    }
}