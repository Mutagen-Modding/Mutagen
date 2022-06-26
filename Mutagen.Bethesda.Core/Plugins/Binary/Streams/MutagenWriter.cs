using Mutagen.Bethesda.Plugins.Meta;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Streams;

/// <summary>
/// A wrapper around IBinaryWriter with extra Mutagen-specific reference data
/// </summary>
public class MutagenWriter : IBinaryWriteStream, IDisposable
{
    private readonly bool dispose = true;
    private const byte Zero = 0;
        
    /// <summary>
    /// Wrapped writer
    /// </summary>
    public BinaryWriter Writer;
        
    /// <summary>
    /// Base stream that the writer wraps
    /// </summary>
    public Stream BaseStream { get; }

    /// <summary>
    /// All the extra meta bits for writing
    /// </summary>
    public WritingBundle MetaData { get; }

    /// <inheritdoc/>
    public long Position
    {
        get => BaseStream.Position;
        set => BaseStream.Position = value;
    }

    /// <inheritdoc/>
    public long Length
    {
        get => BaseStream.Length;
    }

    public bool IsLittleEndian => true;

    public MutagenWriter(
        FilePath path,
        GameConstants constants)
    {
        BaseStream = new FileStream(path.Path, FileMode.Create, FileAccess.Write);
        Writer = new BinaryWriter(BaseStream);
        MetaData = new WritingBundle(constants);
    }

    public MutagenWriter(
        FilePath path,
        WritingBundle meta)
    {
        BaseStream = new FileStream(path.Path, FileMode.Create, FileAccess.Write);
        Writer = new BinaryWriter(BaseStream);
        MetaData = meta;
    }

    public MutagenWriter(
        Stream stream,
        WritingBundle meta,
        bool dispose = true)
    {
        this.dispose = dispose;
        BaseStream = stream;
        Writer = new BinaryWriter(stream);
        MetaData = meta;
    }

    public MutagenWriter(
        Stream stream,
        GameConstants constants,
        bool dispose = true)
    {
        this.dispose = dispose;
        BaseStream = stream;
        Writer = new BinaryWriter(stream);
        MetaData = new WritingBundle(constants);
    }

    public MutagenWriter(
        BinaryWriter writer,
        GameConstants constants)
    {
        BaseStream = writer.BaseStream;
        Writer = writer;
        MetaData = new WritingBundle(constants);
    }

    /// <inheritdoc/>
    public void Write(bool b)
    {
        Writer.Write(b);
    }

    public void Write(bool b, int length)
    {
        switch (length)
        {
            case 1:
                Writer.Write((byte)(b ? 1 : 0));
                break;
            case 2:
                Writer.Write((short)(b ? 1 : 0));
                break;
            case 4:
                Writer.Write((int)(b ? 1 : 0));
                break;
            default:
                throw new NotImplementedException();
        }
    }

    /// <inheritdoc/>
    public void Write(bool? b)
    {
        if (!b.HasValue) return;
        Writer.Write(b.Value);
    }

    /// <inheritdoc/>
    public void Write(byte b)
    {
        Writer.Write(b);
    }

    /// <inheritdoc/>
    public void Write(byte? b)
    {
        if (!b.HasValue) return;
        Writer.Write(b.Value);
    }

    /// <inheritdoc/>
    public void Write(byte[]? b)
    {
        if (b == null) return;
        Writer.Write(b);
    }

    public void Write(ReadOnlyMemorySlice<byte> b)
    {
        Writer.Write(b.Span);
    }

    /// <inheritdoc/>
    public void Write(ReadOnlySpan<byte> b)
    {
        Writer.Write(b);
    }

    /// <inheritdoc/>
    public void Write(ushort b)
    {
        Writer.Write(b);
    }

    /// <inheritdoc/>
    public void Write(ushort? b)
    {
        if (!b.HasValue) return;
        Writer.Write(b.Value);
    }

    /// <inheritdoc/>
    public void Write(uint b)
    {
        Writer.Write(b);
    }

    /// <inheritdoc/>
    public void Write(uint? b)
    {
        if (!b.HasValue) return;
        Writer.Write(b.Value);
    }

    /// <inheritdoc/>
    public void Write(ulong b)
    {
        Writer.Write(b);
    }

    /// <inheritdoc/>
    public void Write(ulong? b)
    {
        if (!b.HasValue) return;
        Writer.Write(b.Value);
    }

    public void Write(sbyte s)
    {
        Writer.Write(s);
    }

    /// <inheritdoc/>
    public void Write(sbyte? s)
    {
        if (!s.HasValue) return;
        Writer.Write(s.Value);
    }

    /// <inheritdoc/>
    public void Write(short s)
    {
        Writer.Write(s);
    }

    /// <inheritdoc/>
    public void Write(short? s)
    {
        if (!s.HasValue) return;
        Writer.Write(s.Value);
    }

    /// <inheritdoc/>
    public void Write(int i)
    {
        Writer.Write(i);
    }

    /// <summary>
    /// Writes an int, limited to the given number of bytes
    /// </summary>
    /// <param name="i">Number to write</param>
    /// <param name="length">Number of bytes to write out</param>
    public void Write(int i, byte length)
    {
        switch (length)
        {
            case 1:
                Writer.Write(checked((byte)i));
                break;
            case 2:
                Writer.Write(checked((short)i));
                break;
            case 4:
                Writer.Write(i);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    /// <inheritdoc/>
    public void Write(int? i)
    {
        if (!i.HasValue) return;
        Writer.Write(i.Value);
    }

    /// <inheritdoc/>
    public void Write(long i)
    {
        Writer.Write(i);
    }

    /// <inheritdoc/>
    public void Write(long? i)
    {
        if (!i.HasValue) return;
        Writer.Write(i.Value);
    }

    /// <inheritdoc/>
    public void Write(float i)
    {
        Writer.Write(i);
    }

    /// <inheritdoc/>
    public void Write(float? i)
    {
        if (!i.HasValue) return;
        Writer.Write(i.Value);
    }

    /// <inheritdoc/>
    public void Write(double i)
    {
        Writer.Write(i);
    }

    /// <inheritdoc/>
    public void Write(double? i)
    {
        if (!i.HasValue) return;
        Writer.Write(i.Value);
    }

    /// <inheritdoc/>
    public void Write(char c)
    {
        Writer.Write(c);
    }

    /// <inheritdoc/>
    public void Write(char? c)
    {
        if (!c.HasValue) return;
        Writer.Write(c.Value);
    }

    /// <inheritdoc/>
    public void WriteZeros(uint num)
    {
        for (uint i = 0; i < num; i++)
        {
            Write(Zero);
        }
    }

    /// <summary>
    /// Disposes of Writer if applicable
    /// </summary>
    public void Dispose()
    {
        if (dispose)
        {
            Writer.Dispose();
        }
    }
}