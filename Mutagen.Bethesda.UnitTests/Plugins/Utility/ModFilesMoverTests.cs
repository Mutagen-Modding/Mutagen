using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.IO;
using Mutagen.Bethesda.Plugins.IO.DI;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Utility;

public class ModFilesMoverTests
{
    public class StructureUnderTest
    {
        public readonly ModFilesMover Sut;

        public StructureUnderTest(IFileSystem fileSystem)
        {
            Sut = new ModFilesMover(
                fileSystem,
                new AssociatedFilesLocator(
                    fileSystem));
        }
    }
    
    public class ExistingSetupFixture
    {
        public DirectoryPath ExistingDirectory { get; }
        public string Content { get; }
        private readonly IFileSystem _fileSystem;
        public IReadOnlyList<Func<DirectoryPath, string>> StringPathCreators { get; set; }
        public string[] StringPaths { get; set; }
        public IReadOnlyList<Func<DirectoryPath, string>> BsaPathCreators { get; set; }
        public string[] BsaPaths { get; set; }
        public ModPath ModPath { get; set; }

        public ExistingSetupFixture(
            IFileSystem fileSystem,
            DirectoryPath existingDirectory,
            string content)
        {
            ExistingDirectory = existingDirectory;
            Content = content;
            _fileSystem = fileSystem;
        }

        public void SetupFor(ModKey modKey)
        {
            ModPath = Path.Combine(ExistingDirectory, modKey.FileName);
            StringPathCreators = new List<Func<DirectoryPath, string>>()
            {
                d => Path.Combine(d, $"{modKey.Name}_english.strings"),
                d => Path.Combine(d, $"{modKey.Name}_french.strings"),
                d => Path.Combine(d, $"{modKey.Name}_english.dlstrings"),
                d => Path.Combine(d, $"{modKey.Name}_french.dlstrings"),
                d => Path.Combine(d, $"{modKey.Name}_english.ilstrings"),
                d => Path.Combine(d, $"{modKey.Name}_french.ilstrings"),
            };
            var stringsFolder = Path.Combine(ExistingDirectory, "Strings");
            StringPaths = StringPathCreators.Select(s =>
            {
                var f = s(stringsFolder);
                _fileSystem.Directory.CreateDirectory(stringsFolder);
                _fileSystem.File.WriteAllText(f, Content);
                return f;
            }).ToArray();
            BsaPathCreators = new List<Func<DirectoryPath, string>>()
            {
                d => Path.Combine(d, $"{modKey.Name}.bsa"),
                d => Path.Combine(d, $"{modKey.Name}.ba2"),
                d => Path.Combine(d, $"{modKey.Name} - Something.bsa"),
                d => Path.Combine(d, $"{modKey.Name} - Something.ba2"),
            };
            BsaPaths = BsaPathCreators.Select(s =>
            {
                var f = s(ExistingDirectory);
                _fileSystem.File.WriteAllText(f, Content);
                return f;
            }).ToArray();
            _fileSystem.File.WriteAllText(ModPath, Content);
        }

        public void AssertStrings(bool exist, string? content = null)
        {
            StringPaths.ForEach(s =>
            {
                if (exist)
                {
                    _fileSystem.File.ReadAllText(s).Should().Be(content ?? Content);
                }
                else
                {
                    _fileSystem.File.Exists(s).Should().BeFalse();
                }
            });
        }

        public void AssertBsas(bool exist, string? content = null)
        {
            BsaPaths.ForEach(s =>
            {
                if (exist)
                {
                    _fileSystem.File.ReadAllText(s).Should().Be(content ?? Content);
                }
                else
                {
                    _fileSystem.File.Exists(s).Should().BeFalse();
                }
            });
        }
    }
    
    public class Fixture
    {
        private readonly ExistingSetupFixture _existingSetup;
        public string Content => _existingSetup.Content;
        public ModPath ModPath => _existingSetup.ModPath;
        public ModKey ModKey => ModPath.ModKey;
        public IFileSystem FileSystem { get; }
        public DirectoryPath ExistingDir => _existingSetup.ExistingDirectory;
        public IReadOnlyList<Func<DirectoryPath, string>> StringPathCreators => _existingSetup.StringPathCreators;
        public string[] StringPaths => _existingSetup.StringPaths;
        public IReadOnlyList<Func<DirectoryPath, string>> BsaPathCreators => _existingSetup.BsaPathCreators;
        public string[] BsaPaths => _existingSetup.BsaPaths;

        public Fixture(
            IFileSystem fileSystem,
            ModKey modKey,
            ExistingSetupFixture existingSetup)
        {
            _existingSetup = existingSetup;
            FileSystem = fileSystem;
            existingSetup.SetupFor(modKey);
        }

        public void AssertStrings(bool exist, string? content = null)
        {
            _existingSetup.AssertStrings(exist, content);
        }

        public void AssertBsas(bool exist, string? content = null)
        {
            _existingSetup.AssertBsas(exist, content);
        }
    }
    
    [Theory, MutagenModAutoData]
    public void Empty(
        IFileSystem fileSystem,
        ModPath modPath,
        DirectoryPath existingDirectoryPath2,
        StructureUnderTest sut)
    {
        ModPath modPath2 = Path.Combine(existingDirectoryPath2, modPath.ModKey.FileName);
        sut.Sut.MoveModTo(modPath, existingDirectoryPath2);
        fileSystem.File.Exists(modPath).Should().BeFalse();
        fileSystem.File.Exists(modPath2).Should().BeFalse();
    }
    
    [Theory, MutagenModAutoData]
    public void NonLocalized(
        IFileSystem fileSystem,
        SkyrimMod mod,
        Npc npc,
        DirectoryPath existingDirectoryPath,
        DirectoryPath existingDirectoryPath2,
        StructureUnderTest sut)
    {
        mod.UsingLocalization = false;
        ModPath modPath = Path.Combine(existingDirectoryPath, mod.ModKey.FileName);
        mod.WriteToBinaryParallel(modPath, fileSystem: fileSystem);
        ModPath modPath2 = Path.Combine(existingDirectoryPath2, mod.ModKey.FileName);
        sut.Sut.MoveModTo(modPath, existingDirectoryPath2);
        fileSystem.File.Exists(modPath).Should().BeFalse();
        fileSystem.File.Exists(modPath2).Should().BeTrue();
    }

    [Theory, MutagenModAutoData]
    public void LocalizedStringsFiles(
        IFileSystem fileSystem,
        Fixture fixture,
        DirectoryPath existingDirectoryPath2,
        StructureUnderTest sut)
    {
        fileSystem.File.Exists(fixture.ModPath).Should().BeTrue();
        ModPath modPath2 = Path.Combine(existingDirectoryPath2, fixture.ModKey.FileName);
        var stringsFolder2 = Path.Combine(existingDirectoryPath2, "Strings");
        
        sut.Sut.MoveModTo(fixture.ModPath, modPath2.Path.Directory!.Value);
        
        fileSystem.File.Exists(fixture.ModPath).Should().BeFalse();
        fixture.AssertStrings(false);
        
        fileSystem.File.Exists(modPath2).Should().BeTrue();
        fixture.StringPathCreators.ForEach(s =>
        {
            var f = s(stringsFolder2);
            fileSystem.File.ReadAllText(f).Should().Be(fixture.Content);
        });
    }

    [Theory, MutagenModAutoData]
    public void BsaFiles(
        IFileSystem fileSystem,
        Fixture fixture,
        DirectoryPath existingDirectoryPath2,
        StructureUnderTest sut)
    {
        ModPath modPath2 = Path.Combine(existingDirectoryPath2, fixture.ModPath.ModKey.FileName);
        
        sut.Sut.MoveModTo(fixture.ModPath, modPath2.Path.Directory!.Value);
        
        fileSystem.File.Exists(fixture.ModPath).Should().BeFalse();
        fixture.AssertBsas(false);
        
        fileSystem.File.Exists(modPath2).Should().BeTrue();
        fixture.BsaPathCreators.ForEach(s =>
        {
            var f = s(existingDirectoryPath2);
            fileSystem.File.ReadAllText(f).Should().Be(fixture.Content);
        });
    }

    [Theory, MutagenModAutoData]
    public void EmptyOtherFilesSafe(
        IFileSystem fileSystem,
        ModPath modPath,
        ModKey otherModKey,
        DirectoryPath existingDirectoryPath2,
        StructureUnderTest sut)
    {
        var otherModPath = new ModPath(otherModKey, Path.Combine(modPath.Path.Directory!.Value, otherModKey.FileName));
        fileSystem.File.Create(otherModPath);

        FilePath otherStringsPath =
            Path.Combine(otherModPath.Path.Directory!, "Strings", $"{otherModKey.Name}_English.strings");
        fileSystem.Directory.CreateDirectory(otherStringsPath.Directory!);
        fileSystem.File.Create(otherStringsPath);

        var otherBsaPath =
            Path.Combine(otherModPath.Path.Directory!, $"{otherModKey.Name}.bsa");
        fileSystem.File.Create(otherBsaPath);

        var otherBsaPath2 =
            Path.Combine(otherModPath.Path.Directory!, $"{otherModKey.Name} - Something.bsa");
        fileSystem.File.Create(otherBsaPath2);
        
        ModPath modPath2 = Path.Combine(existingDirectoryPath2, modPath.ModKey.FileName);
        sut.Sut.MoveModTo(modPath, existingDirectoryPath2);
        fileSystem.File.Exists(modPath).Should().BeFalse();
        fileSystem.File.Exists(modPath2).Should().BeFalse();
        fileSystem.File.Exists(otherModPath).Should().BeTrue();
        fileSystem.File.Exists(otherStringsPath).Should().BeTrue();
        fileSystem.File.Exists(otherBsaPath).Should().BeTrue();
        fileSystem.File.Exists(otherBsaPath2).Should().BeTrue();
    }
    
    [Theory, MutagenModAutoData]
    public void OtherFilesSafe(
        IFileSystem fileSystem,
        Fixture Fixture,
        ModKey otherModKey,
        DirectoryPath existingDirectoryPath2,
        StructureUnderTest sut)
    {
        var otherModPath = new ModPath(otherModKey, Path.Combine(Fixture.ModPath.Path.Directory!.Value, otherModKey.FileName));
        fileSystem.File.Create(otherModPath);

        FilePath otherStringsPath =
            Path.Combine(otherModPath.Path.Directory!, "Strings", $"{otherModKey.Name}_English.strings");
        fileSystem.Directory.CreateDirectory(otherStringsPath.Directory!);
        fileSystem.File.Create(otherStringsPath);

        var otherBsaPath =
            Path.Combine(otherModPath.Path.Directory!, $"{otherModKey.Name}.bsa");
        fileSystem.File.Create(otherBsaPath);

        var otherBsaPath2 =
            Path.Combine(otherModPath.Path.Directory!, $"{otherModKey.Name} - Something.bsa");
        fileSystem.File.Create(otherBsaPath2);
        
        ModPath modPath2 = Path.Combine(existingDirectoryPath2, Fixture.ModPath.ModKey.FileName);
        sut.Sut.MoveModTo(Fixture.ModPath, existingDirectoryPath2);
        fileSystem.File.Exists(Fixture.ModPath).Should().BeFalse();
        fileSystem.File.Exists(modPath2).Should().BeTrue();
        fileSystem.File.Exists(otherModPath).Should().BeTrue();
        fileSystem.File.Exists(otherStringsPath).Should().BeTrue();
        fileSystem.File.Exists(otherBsaPath).Should().BeTrue();
        fileSystem.File.Exists(otherBsaPath2).Should().BeTrue();
    }
    
    [Theory, MutagenModAutoData]
    public void Overwrite(
        IFileSystem fileSystem,
        Fixture Fixture,
        DirectoryPath existingDirectoryPath2,
        string modContent2,
        string stringContent2,
        string bsaContent2,
        StructureUnderTest sut)
    {
        var modPath2 = Path.Combine(existingDirectoryPath2, Fixture.ModPath.ModKey.FileName);
        fileSystem.File.WriteAllText(modPath2, modContent2);
        fileSystem.Directory.CreateDirectory(Path.Combine(existingDirectoryPath2, "Strings"));
        var stringsPath2 = Path.Combine(existingDirectoryPath2, "Strings", $"{Fixture.ModPath.ModKey.Name}_english.strings");
        fileSystem.File.WriteAllText(
            stringsPath2,
            stringContent2);
        var bsaPath2 = Path.Combine(existingDirectoryPath2, $"{Fixture.ModPath.ModKey.Name}.bsa");
        fileSystem.File.WriteAllText(
            bsaPath2,
            bsaContent2);
        
        sut.Sut.MoveModTo(Fixture.ModPath, existingDirectoryPath2, overwrite: true);
        fileSystem.File.Exists(Fixture.ModPath).Should().BeFalse();
        Fixture.StringPaths.ForEach(f =>
        {
            fileSystem.File.Exists(f).Should().BeFalse();
        });
        Fixture.BsaPaths.ForEach(f =>
        {
            fileSystem.File.Exists(f).Should().BeFalse();
        });
        fileSystem.File.ReadAllText(modPath2).Should().Be(Fixture.Content);
        fileSystem.File.ReadAllText(stringsPath2).Should().Be(Fixture.Content);
        fileSystem.File.ReadAllText(bsaPath2).Should().Be(Fixture.Content);
    }
    
    [Theory, MutagenModAutoData]
    public void OverwriteBlocked(
        IFileSystem fileSystem,
        ModPath modPath,
        string content,
        DirectoryPath existingDirectoryPath2,
        StructureUnderTest sut)
    {
        fileSystem.File.WriteAllText(modPath, content);
        fileSystem.Directory.CreateDirectory(Path.Combine(modPath.Path.Directory!, "Strings"));
        var stringsPath = Path.Combine(modPath.Path.Directory!, "Strings", $"{modPath.ModKey.Name}_english.strings");
        fileSystem.File.WriteAllText(
            stringsPath,
            content);
        var bsaPath = Path.Combine(modPath.Path.Directory!, $"{modPath.ModKey.Name}.bsa");
        fileSystem.File.WriteAllText(
            bsaPath,
            content);

        var modPath2 = Path.Combine(existingDirectoryPath2, modPath.ModKey.FileName);
        fileSystem.File.WriteAllText(modPath2, content);

        Assert.Throws<IOException>(() =>
        {
            sut.Sut.MoveModTo(modPath, existingDirectoryPath2, overwrite: false);
        });
        fileSystem.File.Delete(modPath2);
        
        fileSystem.Directory.CreateDirectory(Path.Combine(existingDirectoryPath2, "Strings"));
        var stringsPath2 = Path.Combine(existingDirectoryPath2, "Strings", $"{modPath.ModKey.Name}_english.strings");
        fileSystem.File.WriteAllText(
            stringsPath2,
            content);
        Assert.Throws<IOException>(() =>
        {
            sut.Sut.MoveModTo(modPath, existingDirectoryPath2, overwrite: false);
        });
        fileSystem.File.Delete(stringsPath2);
        
        var bsaPath2 = Path.Combine(existingDirectoryPath2, $"{modPath.ModKey.Name}.bsa");
        fileSystem.File.WriteAllText(
            bsaPath2,
            content);
        Assert.Throws<IOException>(() =>
        {
            sut.Sut.MoveModTo(modPath, existingDirectoryPath2, overwrite: false);
        });
    }
    
    [Theory, MutagenModAutoData]
    public void DontMoveStrings(
        IFileSystem fileSystem,
        Fixture fixture,
        ExistingSetupFixture fullSetup2,
        StructureUnderTest sut)
    {
        fullSetup2.SetupFor(fixture.ModKey);
        sut.Sut.MoveModTo(fixture.ModPath, fullSetup2.ExistingDirectory,
            overwrite: true,
            categories: AssociatedModFileCategory.Archives
                        | AssociatedModFileCategory.Plugin);
        fileSystem.File.Exists(fixture.ModPath).Should().Be(false);
        fixture.AssertStrings(true);
        fixture.AssertBsas(false);
        fileSystem.File.ReadAllText(fullSetup2.ModPath).Should().Be(fixture.Content);
        fullSetup2.AssertStrings(true);
        fullSetup2.AssertBsas(true, fixture.Content);
    }
    
    [Theory, MutagenModAutoData]
    public void DontMoveBsas(
        IFileSystem fileSystem,
        Fixture fixture,
        ExistingSetupFixture fullSetup2,
        StructureUnderTest sut)
    {
        fullSetup2.SetupFor(fixture.ModKey);
        sut.Sut.MoveModTo(fixture.ModPath, fullSetup2.ExistingDirectory,
            overwrite: true,
            categories: AssociatedModFileCategory.RawStrings
                        | AssociatedModFileCategory.Plugin);
        fileSystem.File.Exists(fixture.ModPath).Should().Be(false);
        fixture.AssertStrings(false);
        fixture.AssertBsas(true);
        fileSystem.File.ReadAllText(fullSetup2.ModPath).Should().Be(fixture.Content);
        fullSetup2.AssertStrings(true, fixture.Content);
        fullSetup2.AssertBsas(true);
    }
    
    [Theory, MutagenModAutoData]
    public void DontMovePlugin(
        IFileSystem fileSystem,
        Fixture fixture,
        ExistingSetupFixture fullSetup2,
        StructureUnderTest sut)
    {
        fullSetup2.SetupFor(fixture.ModKey);
        sut.Sut.MoveModTo(fixture.ModPath, fullSetup2.ExistingDirectory,
            overwrite: true,
            categories: AssociatedModFileCategory.RawStrings
                        | AssociatedModFileCategory.Archives);
        fileSystem.File.Exists(fixture.ModPath).Should().Be(true);
        fixture.AssertStrings(false);
        fixture.AssertBsas(false);
        fileSystem.File.ReadAllText(fullSetup2.ModPath).Should().Be(fullSetup2.Content);
        fullSetup2.AssertStrings(true, fixture.Content);
        fullSetup2.AssertBsas(true, fixture.Content);
    }
}