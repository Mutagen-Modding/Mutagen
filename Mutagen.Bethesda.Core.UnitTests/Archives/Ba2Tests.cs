using Mutagen.Bethesda.Archives.Ba2;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Shouldly;
using System.IO.Abstractions;
using System.Text;
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

    [Theory, MutagenAutoData]
    public void TryGetFolder_FindsFolder(
        IFileSystem fileSystem,
        DirectoryPath existingDirectory)
    {
        var layout = WriteGeneralBa2(fileSystem, existingDirectory,
            ("textures/a.dds", [1, 2]),
            ("strings/test_en.strings", [3, 4, 5]),
            ("meshes/b.nif", [6]));
        var archive = new Ba2Reader(layout.ArchivePath, fileSystem);

        archive.TryGetFolder("STRINGS", out var folder).ShouldBeTrue();
        folder.Path.ShouldBe("STRINGS");
        var file = folder.Files.ShouldHaveSingleItem();
        file.Path.ShouldBe("strings/test_en.strings");
        file.GetBytes().ShouldBe(new byte[] { 3, 4, 5 });
    }

    [Theory, MutagenAutoData]
    public void TryGetFolder_MissingFolder(
        IFileSystem fileSystem,
        DirectoryPath existingDirectory)
    {
        var layout = WriteGeneralBa2(fileSystem, existingDirectory,
            ("textures/a.dds", [1, 2]),
            ("meshes/b.nif", [6]));
        var archive = new Ba2Reader(layout.ArchivePath, fileSystem);

        archive.TryGetFolder("strings", out _).ShouldBeFalse();
        archive.Files.Select(f => f.Path).ShouldBe(["textures/a.dds", "meshes/b.nif"]);
        archive.TryGetFolder("meshes", out var folder).ShouldBeTrue();
        folder.Files.ShouldHaveSingleItem().GetBytes().ShouldBe(new byte[] { 6 });
    }

    [Theory, MutagenAutoData]
    public void TryGetFolder_MissingFolder_DoesNotReadFileRecords(
        IFileSystem fileSystem,
        DirectoryPath existingDirectory)
    {
        var layout = WriteGeneralBa2(fileSystem, existingDirectory,
            ("textures/a.dds", [1, 2]),
            ("meshes/b.nif", [6]));
        var streams = new List<ReadTrackingStream>();
        var archive = new Ba2Reader(() =>
        {
            var stream = new ReadTrackingStream(fileSystem.File.ReadAllBytes(layout.ArchivePath));
            streams.Add(stream);
            return stream;
        });

        archive.TryGetFolder("strings", out _).ShouldBeFalse();

        streams.SelectMany(s => s.Reads)
            .Where(r => r.Start < layout.FileRecordsEnd && r.Start + r.Length > layout.FileRecordsStart)
            .ShouldBeEmpty();
    }

    [Theory, MutagenAutoData]
    public void TryGetFolder_RepeatedMisses_ScanNameTableOnce(
        IFileSystem fileSystem,
        DirectoryPath existingDirectory)
    {
        const int misses = 5;
        var layout = WriteGeneralBa2(fileSystem, existingDirectory,
            ("textures/a.dds", [1, 2]),
            ("meshes/b.nif", [6]));
        var streams = new List<ReadTrackingStream>();
        var archive = new Ba2Reader(() =>
        {
            var stream = new ReadTrackingStream(fileSystem.File.ReadAllBytes(layout.ArchivePath));
            streams.Add(stream);
            return stream;
        });

        for (var i = 0; i < misses; i++)
        {
            archive.TryGetFolder($"strings{i}", out _).ShouldBeFalse();
        }
        archive.TryGetFolder("meshes", out var folder).ShouldBeTrue();
        folder.Files.ShouldHaveSingleItem().Path.ShouldBe("meshes/b.nif");

        bool ReadsNameTable(ReadTrackingStream s) =>
            s.Reads.Any(r => r.Length > 0 && r.Start + r.Length > layout.NameTableStart);
        bool ReadsFileRecords(ReadTrackingStream s) =>
            s.Reads.Any(r => r.Start < layout.FileRecordsEnd && r.Start + r.Length > layout.FileRecordsStart);
        streams.Count(s => ReadsNameTable(s) && !ReadsFileRecords(s)).ShouldBeLessThanOrEqualTo(1);
        streams.Count(ReadsNameTable).ShouldBeLessThanOrEqualTo(2);
    }

    private record Ba2Layout(FilePath ArchivePath, long FileRecordsStart, long FileRecordsEnd, long NameTableStart);

    private static Ba2Layout WriteGeneralBa2(
        IFileSystem fileSystem,
        DirectoryPath directory,
        params (string Path, byte[] Data)[] files)
    {
        const uint version = 1;
        const int headerLength = 24;
        const int recordLength = 36;
        const int extensionLength = 4;
        const uint noHash = 0;
        const uint noFlags = 0;
        const uint uncompressedSize = 0;
        const uint align = 0xBAADF00D;
        var fileRecordsEnd = headerLength + recordLength * files.Length;
        var nameTableOffset = fileRecordsEnd + files.Sum(f => f.Data.Length);

        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        writer.Write("BTDX"u8);
        writer.Write(version);
        writer.Write("GNRL"u8);
        writer.Write((uint)files.Length);
        writer.Write((ulong)nameTableOffset);
        long dataOffset = fileRecordsEnd;
        foreach (var file in files)
        {
            writer.Write(noHash);
            writer.Write(Encoding.ASCII.GetBytes(Path.GetExtension(file.Path).TrimStart('.').PadRight(extensionLength, (char)0)[..extensionLength]));
            writer.Write(noHash);
            writer.Write(noFlags);
            writer.Write((ulong)dataOffset);
            writer.Write(uncompressedSize);
            writer.Write((uint)file.Data.Length);
            writer.Write(align);
            dataOffset += file.Data.Length;
        }
        foreach (var file in files)
        {
            writer.Write(file.Data);
        }
        foreach (var file in files)
        {
            var name = Encoding.UTF8.GetBytes(file.Path);
            writer.Write((short)name.Length);
            writer.Write(name);
        }
        writer.Flush();

        var archivePath = new FilePath(Path.Combine(directory, "Test.ba2"));
        fileSystem.File.WriteAllBytes(archivePath, stream.ToArray());
        return new Ba2Layout(archivePath, headerLength, fileRecordsEnd, nameTableOffset);
    }
}

internal sealed class ReadTrackingStream(byte[] data) : MemoryStream(data)
{
    public List<(long Start, int Length)> Reads { get; } = new();

    public override int Read(byte[] buffer, int offset, int count)
    {
        var start = Position;
        var read = base.Read(buffer, offset, count);
        Reads.Add((start, read));
        return read;
    }

    public override int Read(Span<byte> buffer)
    {
        var start = Position;
        var read = base.Read(buffer);
        Reads.Add((start, read));
        return read;
    }

    public override int ReadByte()
    {
        var start = Position;
        var read = base.ReadByte();
        Reads.Add((start, read < 0 ? 0 : 1));
        return read;
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
