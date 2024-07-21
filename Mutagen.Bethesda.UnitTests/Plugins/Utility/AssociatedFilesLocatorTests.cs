using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.IO;
using Mutagen.Bethesda.Plugins.IO.DI;
using Mutagen.Bethesda.Plugins.Utility;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
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
            .Should().BeEmpty();
    }
    
    [Theory, MutagenAutoData]
    public void ModFile(
        ModPath existingModPath,
        AssociatedFilesLocator sut)
    {
        sut.GetAssociatedFiles(existingModPath)
            .Should().Equal(existingModPath);
        sut.GetAssociatedFiles(existingModPath, 
                AssociatedModFileCategory.Archives 
                | AssociatedModFileCategory.RawStrings)
            .Should().BeEmpty();
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
            .Should().Equal(
                existingModPath.AsEnumerable()
                    .Select(x => x.Path)
                    .And(stringsFiles));
        sut.GetAssociatedFiles(existingModPath, 
                AssociatedModFileCategory.Plugin 
                | AssociatedModFileCategory.Archives)
            .Should().Equal(existingModPath);
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
            .Should().Equal(
                existingModPath.AsEnumerable()
                    .Select(x => x.Path)
                    .And(files));
        sut.GetAssociatedFiles(existingModPath, 
                AssociatedModFileCategory.Plugin 
                | AssociatedModFileCategory.RawStrings)
            .Should().Equal(existingModPath);
    }
}