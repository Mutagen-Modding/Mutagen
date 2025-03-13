﻿using System.IO.Abstractions;
using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
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
                    _fileSystem.File.ReadAllText(s).ShouldBe(content ?? Content);
                }
                else
                {
                    _fileSystem.File.Exists(s).ShouldBeFalse();
                }
            });
        }

        public void AssertBsas(bool exist, string? content = null)
        {
            BsaPaths.ForEach(s =>
            {
                if (exist)
                {
                    _fileSystem.File.ReadAllText(s).ShouldBe(content ?? Content);
                }
                else
                {
                    _fileSystem.File.Exists(s).ShouldBeFalse();
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

    #region Move
    
    [Theory, MutagenModAutoData]
    public void EmptyMove(
        IFileSystem fileSystem,
        ModPath modPath,
        DirectoryPath existingDirectoryPath2,
        StructureUnderTest sut)
    {
        ModPath modPath2 = Path.Combine(existingDirectoryPath2, modPath.ModKey.FileName);
        sut.Sut.MoveModTo(modPath, existingDirectoryPath2);
        fileSystem.File.Exists(modPath).ShouldBeFalse();
        fileSystem.File.Exists(modPath2).ShouldBeFalse();
    }
    
    [Theory, MutagenModAutoData]
    public void MoveNonLocalized(
        IFileSystem fileSystem,
        SkyrimMod mod,
        Npc npc,
        DirectoryPath existingDirectoryPath,
        DirectoryPath existingDirectoryPath2,
        StructureUnderTest sut)
    {
        mod.UsingLocalization = false;
        ModPath modPath = Path.Combine(existingDirectoryPath, mod.ModKey.FileName);
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();
        ModPath modPath2 = Path.Combine(existingDirectoryPath2, mod.ModKey.FileName);
        sut.Sut.MoveModTo(modPath, existingDirectoryPath2);
        fileSystem.File.Exists(modPath).ShouldBeFalse();
        fileSystem.File.Exists(modPath2).ShouldBeTrue();
    }

    [Theory, MutagenModAutoData]
    public void MoveLocalizedStringsFiles(
        IFileSystem fileSystem,
        Fixture fixture,
        DirectoryPath existingDirectoryPath2,
        StructureUnderTest sut)
    {
        fileSystem.File.Exists(fixture.ModPath).ShouldBeTrue();
        ModPath modPath2 = Path.Combine(existingDirectoryPath2, fixture.ModKey.FileName);
        var stringsFolder2 = Path.Combine(existingDirectoryPath2, "Strings");
        
        sut.Sut.MoveModTo(fixture.ModPath, modPath2.Path.Directory!.Value);
        
        fileSystem.File.Exists(fixture.ModPath).ShouldBeFalse();
        fixture.AssertStrings(false);
        
        fileSystem.File.Exists(modPath2).ShouldBeTrue();
        fixture.StringPathCreators.ForEach(s =>
        {
            var f = s(stringsFolder2);
            fileSystem.File.ReadAllText(f).ShouldBe(fixture.Content);
        });
    }

    [Theory, MutagenModAutoData]
    public void MoveBsaFiles(
        IFileSystem fileSystem,
        Fixture fixture,
        DirectoryPath existingDirectoryPath2,
        StructureUnderTest sut)
    {
        ModPath modPath2 = Path.Combine(existingDirectoryPath2, fixture.ModPath.ModKey.FileName);
        
        sut.Sut.MoveModTo(fixture.ModPath, modPath2.Path.Directory!.Value);
        
        fileSystem.File.Exists(fixture.ModPath).ShouldBeFalse();
        fixture.AssertBsas(false);
        
        fileSystem.File.Exists(modPath2).ShouldBeTrue();
        fixture.BsaPathCreators.ForEach(s =>
        {
            var f = s(existingDirectoryPath2);
            fileSystem.File.ReadAllText(f).ShouldBe(fixture.Content);
        });
    }

    [Theory, MutagenModAutoData]
    public void MoveEmptyOtherFilesSafe(
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
        fileSystem.File.Exists(modPath).ShouldBeFalse();
        fileSystem.File.Exists(modPath2).ShouldBeFalse();
        fileSystem.File.Exists(otherModPath).ShouldBeTrue();
        fileSystem.File.Exists(otherStringsPath).ShouldBeTrue();
        fileSystem.File.Exists(otherBsaPath).ShouldBeTrue();
        fileSystem.File.Exists(otherBsaPath2).ShouldBeTrue();
    }
    
    [Theory, MutagenModAutoData]
    public void MoveOtherFilesSafe(
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
        fileSystem.File.Exists(Fixture.ModPath).ShouldBeFalse();
        fileSystem.File.Exists(modPath2).ShouldBeTrue();
        fileSystem.File.Exists(otherModPath).ShouldBeTrue();
        fileSystem.File.Exists(otherStringsPath).ShouldBeTrue();
        fileSystem.File.Exists(otherBsaPath).ShouldBeTrue();
        fileSystem.File.Exists(otherBsaPath2).ShouldBeTrue();
    }
    
    [Theory, MutagenModAutoData]
    public void MoveOverwrite(
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
        fileSystem.File.Exists(Fixture.ModPath).ShouldBeFalse();
        Fixture.StringPaths.ForEach(f =>
        {
            fileSystem.File.Exists(f).ShouldBeFalse();
        });
        Fixture.BsaPaths.ForEach(f =>
        {
            fileSystem.File.Exists(f).ShouldBeFalse();
        });
        fileSystem.File.ReadAllText(modPath2).ShouldBe(Fixture.Content);
        fileSystem.File.ReadAllText(stringsPath2).ShouldBe(Fixture.Content);
        fileSystem.File.ReadAllText(bsaPath2).ShouldBe(Fixture.Content);
    }
    
    [Theory, MutagenModAutoData]
    public void MoveOverwriteBlocked(
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
    public void MoveDontMoveStrings(
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
        fileSystem.File.Exists(fixture.ModPath).ShouldBe(false);
        fixture.AssertStrings(true);
        fixture.AssertBsas(false);
        fileSystem.File.ReadAllText(fullSetup2.ModPath).ShouldBe(fixture.Content);
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
        fileSystem.File.Exists(fixture.ModPath).ShouldBe(false);
        fixture.AssertStrings(false);
        fixture.AssertBsas(true);
        fileSystem.File.ReadAllText(fullSetup2.ModPath).ShouldBe(fixture.Content);
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
        fileSystem.File.Exists(fixture.ModPath).ShouldBe(true);
        fixture.AssertStrings(false);
        fixture.AssertBsas(false);
        fileSystem.File.ReadAllText(fullSetup2.ModPath).ShouldBe(fullSetup2.Content);
        fullSetup2.AssertStrings(true, fixture.Content);
        fullSetup2.AssertBsas(true, fixture.Content);
    }

    #endregion

    #region Copy
    
    [Theory, MutagenModAutoData]
    public void EmptyCopy(
        IFileSystem fileSystem,
        ModPath modPath,
        DirectoryPath existingDirectoryPath2,
        StructureUnderTest sut)
    {
        ModPath modPath2 = Path.Combine(existingDirectoryPath2, modPath.ModKey.FileName);
        sut.Sut.CopyModTo(modPath, existingDirectoryPath2);
        fileSystem.File.Exists(modPath).ShouldBeFalse();
        fileSystem.File.Exists(modPath2).ShouldBeFalse();
    }
    
    [Theory, MutagenModAutoData]
    public void CopyNonLocalized(
        IFileSystem fileSystem,
        SkyrimMod mod,
        Npc npc,
        DirectoryPath existingDirectoryPath,
        DirectoryPath existingDirectoryPath2,
        StructureUnderTest sut)
    {
        mod.UsingLocalization = false;
        ModPath modPath = Path.Combine(existingDirectoryPath, mod.ModKey.FileName);
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();
        ModPath modPath2 = Path.Combine(existingDirectoryPath2, mod.ModKey.FileName);
        sut.Sut.CopyModTo(modPath, existingDirectoryPath2);
        fileSystem.File.Exists(modPath).ShouldBeTrue();
        fileSystem.File.Exists(modPath2).ShouldBeTrue();
    }

    [Theory, MutagenModAutoData]
    public void CopyLocalizedStringsFiles(
        IFileSystem fileSystem,
        Fixture fixture,
        DirectoryPath existingDirectoryPath2,
        StructureUnderTest sut)
    {
        fileSystem.File.Exists(fixture.ModPath).ShouldBeTrue();
        ModPath modPath2 = Path.Combine(existingDirectoryPath2, fixture.ModKey.FileName);
        var stringsFolder2 = Path.Combine(existingDirectoryPath2, "Strings");
        
        sut.Sut.CopyModTo(fixture.ModPath, modPath2.Path.Directory!.Value);
        
        fileSystem.File.Exists(fixture.ModPath).ShouldBeTrue();
        fixture.AssertStrings(true);
        
        fileSystem.File.Exists(modPath2).ShouldBeTrue();
        fixture.StringPathCreators.ForEach(s =>
        {
            var f = s(stringsFolder2);
            fileSystem.File.ReadAllText(f).ShouldBe(fixture.Content);
        });
    }

    [Theory, MutagenModAutoData]
    public void CopyBsaFiles(
        IFileSystem fileSystem,
        Fixture fixture,
        DirectoryPath existingDirectoryPath2,
        StructureUnderTest sut)
    {
        ModPath modPath2 = Path.Combine(existingDirectoryPath2, fixture.ModPath.ModKey.FileName);
        
        sut.Sut.CopyModTo(fixture.ModPath, modPath2.Path.Directory!.Value);
        
        fileSystem.File.Exists(fixture.ModPath).ShouldBeTrue();
        fixture.AssertBsas(true);
        
        fileSystem.File.Exists(modPath2).ShouldBeTrue();
        fixture.BsaPathCreators.ForEach(s =>
        {
            var f = s(existingDirectoryPath2);
            fileSystem.File.ReadAllText(f).ShouldBe(fixture.Content);
        });
    }

    [Theory, MutagenModAutoData]
    public void CopyEmptyOtherFilesSafe(
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
        sut.Sut.CopyModTo(modPath, existingDirectoryPath2);
        fileSystem.File.Exists(modPath).ShouldBeFalse();
        fileSystem.File.Exists(modPath2).ShouldBeFalse();
        fileSystem.File.Exists(otherModPath).ShouldBeTrue();
        fileSystem.File.Exists(otherStringsPath).ShouldBeTrue();
        fileSystem.File.Exists(otherBsaPath).ShouldBeTrue();
        fileSystem.File.Exists(otherBsaPath2).ShouldBeTrue();
    }
    
    [Theory, MutagenModAutoData]
    public void CopyOtherFilesSafe(
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
        sut.Sut.CopyModTo(Fixture.ModPath, existingDirectoryPath2);
        fileSystem.File.Exists(Fixture.ModPath).ShouldBeTrue();
        fileSystem.File.Exists(modPath2).ShouldBeTrue();
        fileSystem.File.Exists(otherModPath).ShouldBeTrue();
        fileSystem.File.Exists(otherStringsPath).ShouldBeTrue();
        fileSystem.File.Exists(otherBsaPath).ShouldBeTrue();
        fileSystem.File.Exists(otherBsaPath2).ShouldBeTrue();
    }
    
    [Theory, MutagenModAutoData]
    public void CopyOverwrite(
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
        
        sut.Sut.CopyModTo(Fixture.ModPath, existingDirectoryPath2, overwrite: true);
        fileSystem.File.Exists(Fixture.ModPath).ShouldBeTrue();
        Fixture.StringPaths.ForEach(f =>
        {
            fileSystem.File.Exists(f).ShouldBeTrue();
        });
        Fixture.BsaPaths.ForEach(f =>
        {
            fileSystem.File.Exists(f).ShouldBeTrue();
        });
        fileSystem.File.ReadAllText(modPath2).ShouldBe(Fixture.Content);
        fileSystem.File.ReadAllText(stringsPath2).ShouldBe(Fixture.Content);
        fileSystem.File.ReadAllText(bsaPath2).ShouldBe(Fixture.Content);
    }
    
    [Theory, MutagenModAutoData]
    public void CopyOverwriteBlocked(
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
            sut.Sut.CopyModTo(modPath, existingDirectoryPath2, overwrite: false);
        });
        fileSystem.File.Delete(modPath2);
        
        fileSystem.Directory.CreateDirectory(Path.Combine(existingDirectoryPath2, "Strings"));
        var stringsPath2 = Path.Combine(existingDirectoryPath2, "Strings", $"{modPath.ModKey.Name}_english.strings");
        fileSystem.File.WriteAllText(
            stringsPath2,
            content);
        Assert.Throws<IOException>(() =>
        {
            sut.Sut.CopyModTo(modPath, existingDirectoryPath2, overwrite: false);
        });
        fileSystem.File.Delete(stringsPath2);
        
        var bsaPath2 = Path.Combine(existingDirectoryPath2, $"{modPath.ModKey.Name}.bsa");
        fileSystem.File.WriteAllText(
            bsaPath2,
            content);
        Assert.Throws<IOException>(() =>
        {
            sut.Sut.CopyModTo(modPath, existingDirectoryPath2, overwrite: false);
        });
    }
    
    [Theory, MutagenModAutoData]
    public void CopyDontCopyStrings(
        IFileSystem fileSystem,
        Fixture fixture,
        ExistingSetupFixture fullSetup2,
        StructureUnderTest sut)
    {
        fullSetup2.SetupFor(fixture.ModKey);
        sut.Sut.CopyModTo(fixture.ModPath, fullSetup2.ExistingDirectory,
            overwrite: true,
            categories: AssociatedModFileCategory.Archives
                        | AssociatedModFileCategory.Plugin);
        fileSystem.File.Exists(fixture.ModPath).ShouldBe(true);
        fixture.AssertStrings(true);
        fixture.AssertBsas(true);
        fileSystem.File.ReadAllText(fullSetup2.ModPath).ShouldBe(fixture.Content);
        fullSetup2.AssertStrings(true);
        fullSetup2.AssertBsas(true, fixture.Content);
    }
    
    [Theory, MutagenModAutoData]
    public void DontCopyBsas(
        IFileSystem fileSystem,
        Fixture fixture,
        ExistingSetupFixture fullSetup2,
        StructureUnderTest sut)
    {
        fullSetup2.SetupFor(fixture.ModKey);
        sut.Sut.CopyModTo(fixture.ModPath, fullSetup2.ExistingDirectory,
            overwrite: true,
            categories: AssociatedModFileCategory.RawStrings
                        | AssociatedModFileCategory.Plugin);
        fileSystem.File.Exists(fixture.ModPath).ShouldBe(true);
        fixture.AssertStrings(true);
        fixture.AssertBsas(true);
        fileSystem.File.ReadAllText(fullSetup2.ModPath).ShouldBe(fixture.Content);
        fullSetup2.AssertStrings(true, fixture.Content);
        fullSetup2.AssertBsas(true);
    }
    
    [Theory, MutagenModAutoData]
    public void DontCopyPlugin(
        IFileSystem fileSystem,
        Fixture fixture,
        ExistingSetupFixture fullSetup2,
        StructureUnderTest sut)
    {
        fullSetup2.SetupFor(fixture.ModKey);
        sut.Sut.CopyModTo(fixture.ModPath, fullSetup2.ExistingDirectory,
            overwrite: true,
            categories: AssociatedModFileCategory.RawStrings
                        | AssociatedModFileCategory.Archives);
        fileSystem.File.Exists(fixture.ModPath).ShouldBe(true);
        fixture.AssertStrings(true);
        fixture.AssertBsas(true);
        fileSystem.File.ReadAllText(fullSetup2.ModPath).ShouldBe(fullSetup2.Content);
        fullSetup2.AssertStrings(true, fixture.Content);
        fullSetup2.AssertBsas(true, fixture.Content);
    }

    #endregion
}