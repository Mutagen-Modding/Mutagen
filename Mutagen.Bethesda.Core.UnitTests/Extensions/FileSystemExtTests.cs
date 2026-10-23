using System.IO.Abstractions;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Extensions;

public class FileSystemExtTests
{
    private static readonly IFileSystem FileSystem = new FileSystem();

    [Fact]
    public void AnswersTheExactPathWhenItExists()
    {
        using var dir = TestPathing.GetTempFolder(nameof(FileSystemExtTests));
        var onDisk = Path.Combine(dir.Dir.Path, "MyMod.esp");
        File.WriteAllText(onDisk, string.Empty);
        FileSystem.ResolveIgnoringCase(dir.Dir.Path, "MyMod.esp").ShouldBe(onDisk);
    }

    [Fact]
    public void FindsFileWhoseNameDiffersInCase()
    {
        using var dir = TestPathing.GetTempFolder(nameof(FileSystemExtTests));
        File.WriteAllText(Path.Combine(dir.Dir.Path, "mymod.esp"), "plugin");
        File.ReadAllText(FileSystem.ResolveIgnoringCase(dir.Dir.Path, "MyMod.esp")).ShouldBe("plugin");
    }

    [Fact]
    public void FindsFileThroughFoldersWhoseNamesDifferInCase()
    {
        using var dir = TestPathing.GetTempFolder(nameof(FileSystemExtTests));
        var folder = Directory.CreateDirectory(Path.Combine(dir.Dir.Path, "textures", "armor")).FullName;
        File.WriteAllText(Path.Combine(folder, "foo.dds"), "texture");
        File.ReadAllText(FileSystem.ResolveIgnoringCase(dir.Dir.Path, Path.Combine("Textures", "Armor", "Foo.dds"))).ShouldBe("texture");
    }

    [Fact]
    public void AnswersTheCombinedPathWhenNothingMatches()
    {
        using var dir = TestPathing.GetTempFolder(nameof(FileSystemExtTests));
        var missingRoot = Path.Combine(dir.Dir.Path, "Data");
        FileSystem.ResolveIgnoringCase(missingRoot, Path.Combine("Textures", "Foo.dds"))
            .ShouldBe(Path.Combine(missingRoot, "Textures", "Foo.dds"));
    }

    [Fact]
    public void ThrowsWhenTwoNamesDifferOnlyInCaseAndNeitherIsExact()
    {
        if (!OperatingSystem.IsLinux()) return;
        using var dir = TestPathing.GetTempFolder(nameof(FileSystemExtTests));
        File.WriteAllText(Path.Combine(dir.Dir.Path, "mymod.esp"), string.Empty);
        File.WriteAllText(Path.Combine(dir.Dir.Path, "MYMOD.esp"), string.Empty);
        var ex = Should.Throw<IOException>(() => FileSystem.ResolveIgnoringCase(dir.Dir.Path, "MyMod.esp"));
        ex.Message.ShouldContain("mymod.esp");
        ex.Message.ShouldContain("MYMOD.esp");
    }

    [Fact]
    public void PrefersTheExactNameOverOneDifferingInCase()
    {
        if (!OperatingSystem.IsLinux()) return;
        using var dir = TestPathing.GetTempFolder(nameof(FileSystemExtTests));
        var exact = Directory.CreateDirectory(Path.Combine(dir.Dir.Path, "Strings")).FullName;
        Directory.CreateDirectory(Path.Combine(dir.Dir.Path, "strings"));
        File.WriteAllText(Path.Combine(exact, "mymod_english.strings"), "exact");
        File.ReadAllText(FileSystem.ResolveIgnoringCase(dir.Dir.Path, Path.Combine("Strings", "MyMod_English.STRINGS"))).ShouldBe("exact");
    }
}
