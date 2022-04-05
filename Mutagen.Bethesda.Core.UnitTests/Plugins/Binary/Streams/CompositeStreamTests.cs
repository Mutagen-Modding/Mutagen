using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;
using Xunit;
using Stream = System.IO.Stream;
using MemoryStream = System.IO.MemoryStream;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Streams;

public class CompositeStreamTests
{
    const int TotalLength = 32;
    IEnumerable<Stream> TypicalStreams()
    {
        yield return new MemoryStream(Enumerable.Range(0, 10).Select(i => (byte)i).ToArray());
        yield return new MemoryStream(Enumerable.Range(10, 15).Select(i => (byte)i).ToArray());
        yield return new MemoryStream(Enumerable.Range(25, 7).Select(i => (byte)i).ToArray());
    }

    #region Reads
    [Fact]
    public void EmptyRead()
    {
        var compositeStream = new CompositeReadStream(EnumerableExt<Stream>.Empty);
        Assert.Equal(0, compositeStream.Remaining());
        Assert.Equal(0, compositeStream.Length);
        Assert.Equal(0, compositeStream.Position);
        Assert.Equal(0, compositeStream.Read(new byte[0], 0, 10));
    }

    [Fact]
    public void TypicalFirstRead()
    {
        int toRead = 5;
        byte[] buf = new byte[toRead];
        var compositeStream = new CompositeReadStream(TypicalStreams());
        Assert.Equal(TotalLength, compositeStream.Remaining());
        Assert.Equal(TotalLength, compositeStream.Length);
        Assert.Equal(0, compositeStream.Position);
        Assert.Equal(toRead, compositeStream.Read(buf, 0, toRead));
        Assert.Equal(toRead, compositeStream.Position);
        Assert.Equal(TotalLength - toRead, compositeStream.Remaining());
        Assert.Equal(TotalLength, compositeStream.Length);
    }

    [Fact]
    public void TypicalOverread()
    {
        int toRead = 13;
        byte[] buf = new byte[toRead];
        var compositeStream = new CompositeReadStream(TypicalStreams());
        Assert.Equal(TotalLength, compositeStream.Remaining());
        Assert.Equal(TotalLength, compositeStream.Length);
        Assert.Equal(0, compositeStream.Position);
        Assert.Equal(toRead, compositeStream.Read(buf, 0, toRead));
        Assert.Equal(toRead, compositeStream.Position);
        Assert.Equal(TotalLength - toRead, compositeStream.Remaining());
        Assert.Equal(TotalLength, compositeStream.Length);
    }

    [Fact]
    public void CopyIntoStream()
    {
        byte[] buf = new byte[TotalLength];
        var memStream = new MemoryStream(buf);
        var compositeStream = new CompositeReadStream(TypicalStreams());
        compositeStream.CopyTo(memStream);
        Assert.Equal(TotalLength, compositeStream.Position);
        Assert.Equal(0, compositeStream.Remaining());
        Assert.Equal(TotalLength, compositeStream.Length);
        Assert.Equal(TotalLength, memStream.Position);
        Assert.Equal(0, memStream.Remaining());
        Assert.Equal(TotalLength, memStream.Length);
        for (int i = 0; i < buf.Length; i++)
        {
            Assert.Equal(i, buf[i]);
        }
    }
    #endregion
}