using Mutagen.Bethesda.Archives.Ba2;
using Noggog;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Archives;

/// <summary>Verifies BA2 file and texture payload reads.</summary>
public class Ba2Tests
{
    /// <summary>A BA2 fixture containing one uncompressed file with bytes 1 through 8.</summary>
    public static readonly FilePath TestBa2 = new(Path.Combine("..", "..", "..", "Archives", "test.ba2"));

    /// <summary>A BC1 texture fixture with an uncompressed mip and a zlib-compressed mip.</summary>
    public static readonly FilePath TestTextureBa2 = new(Path.Combine("..", "..", "..", "Archives", "test-textures.ba2"));

    /// <summary>Reads a general file completely even when the source returns short reads.</summary>
    [Fact]
    public void AsBytes_ShortReads()
    {
        var data = File.ReadAllBytes(TestBa2.Path);
        var archive = new Ba2Reader(() => new ShortReadStream(data));
        byte[] expected = [1, 2, 3, 4, 5, 6, 7, 8];

        var result = archive.Files.Single().GetBytes();

        result.ShouldBe(expected);
    }

    /// <summary>Reads both compressed and uncompressed texture chunks completely.</summary>
    [Fact]
    public void AsBytes_TextureShortReads()
    {
        var data = File.ReadAllBytes(TestTextureBa2.Path);
        var archive = new Ba2Reader(() => new ShortReadStream(data));
        byte[] expected = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16];

        var result = archive.Files.Single().GetBytes();

        result.TakeLast(expected.Length).ShouldBe(expected);
    }
}

/// <summary>A seekable archive test stream that returns at most two bytes on each read.</summary>
internal sealed class ShortReadStream(byte[] data) : MemoryStream(data)
{
    /// <summary>Returns at most two bytes through the array overload.</summary>
    public override int Read(byte[] buffer, int offset, int count)
    {
        return base.Read(buffer, offset, Math.Min(count, 2));
    }

    /// <summary>Returns at most two bytes through the span overload.</summary>
    public override int Read(Span<byte> buffer)
    {
        return base.Read(buffer[..Math.Min(buffer.Length, 2)]);
    }
}
