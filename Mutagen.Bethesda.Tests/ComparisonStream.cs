namespace Mutagen.Bethesda.Tests;

public class ComparisonStream : Stream
{
    Stream stream1;
    byte[] buf1;
    Stream stream2;
    byte[] buf2;
    long len;
    byte equal;
    byte notEqual;

    public ComparisonStream(
        Stream stream1,
        Stream stream2,
        int buffLen = 4096,
        byte equal = 0,
        byte notEqual = 1)
    {
        this.stream1 = stream1;
        this.stream2 = stream2;
        len = Math.Min(stream1.Length, stream2.Length);
        buf1 = new byte[buffLen];
        buf2 = new byte[buffLen];
        this.equal = equal;
        this.notEqual = notEqual;
    }

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length => len;
    public override long Position
    {
        get => stream1.Position;
        set
        {
            stream1.Position = value;
            stream2.Position = value;
        }
    }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var toRead = Math.Min(count, buf1.Length);
        var read1 = stream1.Read(buf1, offset, toRead);
        var read2 = stream2.Read(buf2, offset, toRead);
        if (read1 < read2)
        {
            stream2.Position -= read2 - read1;
            toRead = read1;
        }
        else if (read2 < read1)
        {
            stream1.Position -= read1 - read2;
            toRead = read2;
        }
        for (int i = 0; i < toRead; i++)
        {
            buffer[i + offset] = buf1[i] == buf2[i] ? equal : notEqual;
        }
        return toRead;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
}