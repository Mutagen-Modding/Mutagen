using System.IO.Abstractions;
using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.IO;
using Mutagen.Bethesda.Plugins.IO.DI;
using Mutagen.Bethesda.Plugins.Utility;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Noggog.Testing.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Utility;

public class AssociatedFilesLocatorTests
{
    [Theory, MutagenAutoData]
    public void Empty(
        ModPath modPath,
        AssociatedFilesLocator sut)
    {
        sut.GetAssociatedFiles(modPath)
            .ShouldBeEmpty();
    }
    
    [Theory, MutagenAutoData]
    public void ModFile(
        ModPath existingModPath,
        AssociatedFilesLocator sut)
    {
        sut.GetAssociatedFiles(existingModPath)
            .ShouldEqualEnumerable(existingModPath);
        sut.GetAssociatedFiles(existingModPath, 
                AssociatedModFileCategory.Archives 
                | AssociatedModFileCategory.RawStrings)
            .ShouldBeEmpty();
    }
    
    [Theory, MutagenAutoData]
    public void StringsFiles(
        IFileSystem fileSystem,
        ModPath existingModPath,
        AssociatedFilesLocator sut)
    {
        var stringsFileNames = new List<string>()
        {
            $"{existingModPath.ModKey.Name}_English.strings",
            $"{existingModPath.ModKey.Name}_English.dlstrings",
            $"{existingModPath.ModKey.Name}_English.ilstrings",
            $"{existingModPath.ModKey.Name}_French.strings",
            $"{existingModPath.ModKey.Name}_French.dlstrings",
            $"{existingModPath.ModKey.Name}_French.ilstrings",
            $"{existingModPath.ModKey.Name}_en.strings",
            $"{existingModPath.ModKey.Name}_en.dlstrings",
            $"{existingModPath.ModKey.Name}_en.ilstrings",
            $"{existingModPath.ModKey.Name}_fr.strings",
            $"{existingModPath.ModKey.Name}_fr.dlstrings",
            $"{existingModPath.ModKey.Name}_fr.ilstrings",
        };
        var stringsFiles = stringsFileNames.Select(f =>
        {
            return new FilePath(Path.Combine(existingModPath.Path.Directory!, "Strings", f));
        }).ToArray();
        foreach (var f in stringsFiles)
        {
            fileSystem.Directory.CreateDirectory(f.Directory!);
            fileSystem.File.Create(f);
        }
        sut.GetAssociatedFiles(existingModPath)
            .ShouldBe(
                existingModPath.AsEnumerable()
                    .Select(x => x.Path)
                    .And(stringsFiles));
        sut.GetAssociatedFiles(existingModPath, 
                AssociatedModFileCategory.Plugin 
                | AssociatedModFileCategory.Archives)
            .ShouldEqualEnumerable(existingModPath);
    }
    
    [Theory, MutagenAutoData]
    public void BsaFiles(
        IFileSystem fileSystem,
        ModPath existingModPath,
        AssociatedFilesLocator sut)
    {
        var fileNames = new List<string>()
        {
            $"{existingModPath.ModKey.Name}.bsa",
            $"{existingModPath.ModKey.Name} - Something.bsa",
            $"{existingModPath.ModKey.Name}.ba2",
            $"{existingModPath.ModKey.Name} - Something.ba2",
        };
        var files = fileNames.Select(f =>
        {
            return new FilePath(Path.Combine(existingModPath.Path.Directory!, f));
        }).ToArray();
        foreach (var f in files)
        {
            fileSystem.Directory.CreateDirectory(f.Directory!);
            fileSystem.File.Create(f);
        }
        sut.GetAssociatedFiles(existingModPath)
            .ShouldBe(
                existingModPath.AsEnumerable()
                    .Select(x => x.Path)
                    .And(files));
        sut.GetAssociatedFiles(existingModPath,
                AssociatedModFileCategory.Plugin
                | AssociatedModFileCategory.RawStrings)
            .Select(x => (ModPath)x)
            .ShouldEqualEnumerable(existingModPath);
    }

    [Theory, MutagenAutoData]
    public void SplitPluginFiles(
        IFileSystem fileSystem,
        ModPath existingModPath,
        AssociatedFilesLocator sut)
    {
        var dir = existingModPath.Path.Directory!;
        var baseName = Path.GetFileNameWithoutExtension(existingModPath.Path);
        var extension = Path.GetExtension(existingModPath.Path);
        var splitFiles = new List<FilePath>
        {
            new(Path.Combine(dir, $"{baseName}_2{extension}")),
            new(Path.Combine(dir, $"{baseName}_3{extension}")),
        };
        foreach (var f in splitFiles)
        {
            fileSystem.File.Create(f);
        }
        sut.GetAssociatedFiles(existingModPath, AssociatedModFileCategory.SplitPlugins)
            .ShouldBe(splitFiles);
    }

    [Theory, MutagenAutoData]
    public void PluginFlagDoesNotReturnSplitFiles(
        IFileSystem fileSystem,
        ModPath existingModPath,
        AssociatedFilesLocator sut)
    {
        var dir = existingModPath.Path.Directory!;
        var baseName = Path.GetFileNameWithoutExtension(existingModPath.Path);
        var extension = Path.GetExtension(existingModPath.Path);
        var splitFile = Path.Combine(dir, $"{baseName}_2{extension}");
        fileSystem.File.Create(splitFile);
        sut.GetAssociatedFiles(existingModPath, AssociatedModFileCategory.Plugin)
            .ShouldEqualEnumerable(existingModPath);
    }

    [Theory, MutagenAutoData]
    public void NonNumericSuffixExcluded(
        IFileSystem fileSystem,
        ModPath existingModPath,
        AssociatedFilesLocator sut)
    {
        var dir = existingModPath.Path.Directory!;
        var baseName = Path.GetFileNameWithoutExtension(existingModPath.Path);
        var extension = Path.GetExtension(existingModPath.Path);
        fileSystem.File.Create(Path.Combine(dir, $"{baseName}_Patch{extension}"));
        fileSystem.File.Create(Path.Combine(dir, $"{baseName}_abc{extension}"));
        sut.GetAssociatedFiles(existingModPath, AssociatedModFileCategory.SplitPlugins)
            .ShouldBeEmpty();
    }
}