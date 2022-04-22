using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Allocators;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.AutoData;
using Noggog.Testing.IO;
using Xunit;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.UnitTests.Plugins.Allocators;

public class TextFileSharedFormKeyAllocatorTests : ISharedFormKeyAllocatorTests<TextFileSharedFormKeyAllocator>
{
    private const string DefaultName = "default";

    protected override TextFileSharedFormKeyAllocator CreateAllocator(IFileSystem fileSystem, IMod mod, string path) => new(mod, path, DefaultName, preload: true, fileSystem: fileSystem);

    protected override TextFileSharedFormKeyAllocator CreateNamedAllocator(IFileSystem fileSystem, IMod mod, string path, string patcherName) => new(mod, path, patcherName, preload: true, fileSystem: fileSystem);

    public static readonly string SomeFolder = $"{PathingUtil.DrivePrefix}SomeFolder";
        
    protected override string ConstructTypicalPath(IFileSystem fileSystem)
    {
        fileSystem.Directory.CreateDirectory(SomeFolder);
        fileSystem.File.WriteAllText(Path.Combine(SomeFolder, TextFileSharedFormKeyAllocator.MarkerFileName), null);
        return SomeFolder;
    }

    [Fact]
    public void StaticExport()
    {
        var fileSystem = new MockFileSystem();
        var someFile = $"{PathingUtil.DrivePrefix}SomeFile";
        TextFileSharedFormKeyAllocator.WriteToFile(
            someFile,
            new (string, FormKey)[]
            {
                (TestConstants.Edid1, TestConstants.Form1),
                (TestConstants.Edid2, TestConstants.Form2),
            },
            fileSystem);
        var lines = fileSystem.File.ReadAllLines(someFile + ".txt");
        Assert.Equal(
            new string[]
            {
                TestConstants.Edid1,
                TestConstants.Form1.ID.ToString(),
                TestConstants.Edid2,
                TestConstants.Form2.ID.ToString(),
            },
            lines);
    }

    private IFileSystem GetTypicalFileSystem()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.Directory.CreateDirectory(SomeFolder);
        return fileSystem;
    }

    [Theory, MutagenAutoData(Release: GameRelease.Oblivion)]
    public void TypicalImport(IMod mod)
    {
        var fileSystem = GetTypicalFileSystem();
        fileSystem.File.WriteAllLines(
            Path.Combine(SomeFolder, Patcher1 + ".txt"),
            new string[]
            {
                TestConstants.Edid1,
                TestConstants.Form1.ID.ToString(),
                TestConstants.Edid2,
                TestConstants.Form2.ID.ToString(),
            });
        fileSystem.File.WriteAllText(Path.Combine(SomeFolder, TextFileSharedFormKeyAllocator.MarkerFileName), null);
        var allocator = new TextFileSharedFormKeyAllocator(mod, SomeFolder, Patcher1, preload: true, fileSystem: fileSystem);
        var formID = allocator.GetNextFormKey(TestConstants.Edid1);
        Assert.Equal(TestConstants.Form1.ID, formID.ID);
        Assert.Equal(mod.ModKey, formID.ModKey);
        formID = allocator.GetNextFormKey(TestConstants.Edid2);
        Assert.Equal(TestConstants.Form2.ID, formID.ID);
        Assert.Equal(mod.ModKey, formID.ModKey);
    }

    [Theory, MutagenAutoData(Release: GameRelease.Oblivion)]
    public void FailedImportTruncatedFile(IMod mod)
    {
        var fileSystem = GetTypicalFileSystem();
        fileSystem.File.WriteAllLines(
            Path.Combine(SomeFolder, Patcher1 + ".txt"),
            new string[]
            {
                TestConstants.Edid1,
                TestConstants.Form1.ID.ToString(),
                TestConstants.Edid2,
                //TestConstants.Form2.ID.ToString(),
            });
        Assert.ThrowsAny<Exception>(() => new TextFileSharedFormKeyAllocator(mod, SomeFolder, DefaultName, preload: true, fileSystem: fileSystem));
    }

    [Theory, MutagenAutoData(Release: GameRelease.Oblivion)]
    public void FailedImportDuplicateFormKey(IMod mod)
    {
        var fileSystem = GetTypicalFileSystem();
        fileSystem.File.WriteAllText(Path.Combine(SomeFolder, TextFileSharedFormKeyAllocator.MarkerFileName), null);
        fileSystem.File.WriteAllLines(
            Path.Combine(SomeFolder, Patcher1 + ".txt"),
            new string[]
            {
                TestConstants.Edid1,
                TestConstants.Form1.ID.ToString(),
                TestConstants.Edid2,
                TestConstants.Form1.ID.ToString(),
            });
        Assert.Throws<ArgumentException>(() => new TextFileSharedFormKeyAllocator(mod, SomeFolder, DefaultName, preload: true, fileSystem: fileSystem));
    }

    [Theory, MutagenAutoData(Release: GameRelease.Oblivion)]
    public void FailedImportDuplicateEditorId(IMod mod)
    {
        var fileSystem = GetTypicalFileSystem();
        fileSystem.File.WriteAllText(Path.Combine(SomeFolder, TextFileSharedFormKeyAllocator.MarkerFileName), null);
        fileSystem.File.WriteAllLines(
            Path.Combine(SomeFolder, Patcher1 + ".txt"),
            new string[]
            {
                TestConstants.Edid1,
                TestConstants.Form1.ID.ToString(),
                TestConstants.Edid1,
                TestConstants.Form2.ID.ToString(),
            });
        Assert.Throws<ArgumentException>(() => new TextFileSharedFormKeyAllocator(mod, SomeFolder, Patcher1, preload: true, fileSystem: fileSystem));
    }

    [Theory, MutagenAutoData(Release: GameRelease.Oblivion)]
    public void TypicalReimport(IMod mod)
    {
        var fileSystem = GetTypicalFileSystem();
        TextFileSharedFormKeyAllocator.WriteToFile(
            Path.Combine(SomeFolder, Patcher1),
            new (string, FormKey)[]
            {
                (TestConstants.Edid1, TestConstants.Form1),
                (TestConstants.Edid2, TestConstants.Form2),
            },
            fileSystem: fileSystem);
        fileSystem.File.WriteAllText(Path.Combine(SomeFolder, TextFileSharedFormKeyAllocator.MarkerFileName), null);
        using var allocator = new TextFileSharedFormKeyAllocator(mod, SomeFolder, Patcher1, fileSystem: fileSystem);
        var formID = allocator.GetNextFormKey();
        formID = allocator.GetNextFormKey(TestConstants.Edid1);
        Assert.Equal(formID.ID, TestConstants.Form1.ID);
        Assert.Equal(formID.ModKey, mod.ModKey);
        formID = allocator.GetNextFormKey(TestConstants.Edid2);
        Assert.Equal(formID.ID, TestConstants.Form2.ID);
        Assert.Equal(formID.ModKey, mod.ModKey);
    }
}