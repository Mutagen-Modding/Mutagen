using FluentAssertions;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Strings.DI;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Archives;

public class BsaTests
{
    public static FilePath TestBsa = new(Path.Combine("..", "..", "..", "Archives", "test.bsa"));
    public static string SomeFolder = Path.Combine("derp", "some_FoldeR");

    [Fact]
    public void TryGetFolder_CaseInsensitive()
    {
        var archive = Archive.CreateReader(GameRelease.SkyrimSE, TestBsa);
        archive.TryGetFolder(SomeFolder, out var folder)
            .Should().BeTrue();
        if (folder == null) throw new NullReferenceException();
        folder.Files.Should().HaveCount(1);
        var expected = Path.Combine(SomeFolder, "someotherfile.txt");
        folder.Files.First().Path.Should().Be(expected.ToLower());
    }
        
    [Fact]
    public void AsBytes()
    {
        var archive = Archive.CreateReader(GameRelease.SkyrimSE, TestBsa);
        archive.TryGetFolder(SomeFolder, out var folder)
            .Should().BeTrue();
        if (folder == null) throw new NullReferenceException();
        var file = folder.Files.First();
        BinaryStringUtility.ProcessWholeToZString(file.GetBytes(), MutagenEncodingProvider._1252)
            .Should().Be("Found me");
    }
        
    [Fact]
    public void GetSpan()
    {
        var archive = Archive.CreateReader(GameRelease.SkyrimSE, TestBsa);
        archive.TryGetFolder(SomeFolder, out var folder)
            .Should().BeTrue();
        if (folder == null) throw new NullReferenceException();
        var file = folder.Files.First();
        BinaryStringUtility.ProcessWholeToZString(file.GetSpan(), MutagenEncodingProvider._1252)
            .Should().Be("Found me");
    }
        
    [Fact]
    public void GetMemorySlice()
    {
        var archive = Archive.CreateReader(GameRelease.SkyrimSE, TestBsa);
        archive.TryGetFolder(SomeFolder, out var folder)
            .Should().BeTrue();
        if (folder == null) throw new NullReferenceException();
        var file = folder.Files.First();
        BinaryStringUtility.ProcessWholeToZString(file.GetMemorySlice(), MutagenEncodingProvider._1252)
            .Should().Be("Found me");
    }
        
    [Fact]
    public void AsStream()
    {
        var archive = Archive.CreateReader(GameRelease.SkyrimSE, TestBsa);
        archive.TryGetFolder(SomeFolder, out var folder)
            .Should().BeTrue();
        if (folder == null) throw new NullReferenceException();
        var file = folder.Files.First();
        var stream = file.AsStream();
        byte[] b = new byte[stream.Length];
        stream.Remaining().Should().Be(8);
        stream.Read(b);
        stream.Remaining().Should().Be(0);
        BinaryStringUtility.ProcessWholeToZString(b, MutagenEncodingProvider._1252)
            .Should().Be("Found me");
    }
}