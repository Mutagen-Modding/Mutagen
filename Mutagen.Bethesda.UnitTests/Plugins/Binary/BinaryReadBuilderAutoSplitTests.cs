using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Analysis;
using Mutagen.Bethesda.Plugins.Analysis.DI;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using System.IO.Abstractions;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary;

public class BinaryReadBuilderAutoSplitTests
{
    [Theory, MutagenModAutoData]
    public void IsMultiModFile_ReturnsTrueForValidSplitFiles(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        // Create valid split files
        var modKey = mod.ModKey;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(modKey.FileName);
        var extension = Path.GetExtension(modKey.FileName);

        // Write split files with actual mod content
        for (int i = 0; i < 5; i++)
        {
            var formList = mod.FormLists.AddNew();
            formList.EditorID = $"FormList{i}";
            for (uint j = 0; j < 60; j++)
            {
                var masterKey = new ModKey($"Master_{i}_{j}", ModType.Plugin);
                formList.Items.Add(new FormKey(masterKey, 0x800 + j));
            }
        }

        // Write with auto-split to create split files
        var outputPath = Path.Combine(existingOutputDirectory.Path, modKey.FileName);
        var autoSplitWriter = new AutoSplitModWriter(new MultiModFileSplitter());
        autoSplitWriter.Write<ISkyrimMod, ISkyrimModGetter>(
            mod,
            outputPath,
            BinaryWriteParameters.Default with { FileSystem = fileSystem });

        // Verify IsMultiModFile returns true
        MultiModFileAnalysis.IsMultiModFile(existingOutputDirectory, modKey, fileSystem).ShouldBeTrue();
    }

    [Theory, MutagenModAutoData]
    public void IsMultiModFile_ReturnsFalseWhenNoSplitFiles(
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        var modKey = new ModKey("NonExistent", ModType.Plugin);

        MultiModFileAnalysis.IsMultiModFile(existingOutputDirectory, modKey, fileSystem).ShouldBeFalse();
    }

    [Theory, MutagenModAutoData]
    public void IsMultiModFile_ThrowsWhenOnlyFirstSplitExists(
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        var modKey = new ModKey("TestMod", ModType.Plugin);
        var splitFile1 = Path.Combine(existingOutputDirectory.Path, "TestMod_1.esp");

        // Create just one split file
        fileSystem.File.WriteAllText(splitFile1, "dummy");

        Should.Throw<SplitModException>(() =>
            MultiModFileAnalysis.IsMultiModFile(existingOutputDirectory, modKey, fileSystem))
            .Message.ShouldContain("only one split file");
    }

    [Theory, MutagenModAutoData]
    public void IsMultiModFile_ThrowsWhenOriginalAndSplitsCoexist(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        var modKey = mod.ModKey;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(modKey.FileName);
        var extension = Path.GetExtension(modKey.FileName);

        // Create split files
        var splitFile1 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_1{extension}");
        var splitFile2 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");
        var originalFile = Path.Combine(existingOutputDirectory.Path, modKey.FileName);

        fileSystem.File.WriteAllText(splitFile1, "dummy");
        fileSystem.File.WriteAllText(splitFile2, "dummy");
        fileSystem.File.WriteAllText(originalFile, "dummy");

        Should.Throw<SplitModException>(() =>
            MultiModFileAnalysis.IsMultiModFile(existingOutputDirectory, modKey, fileSystem))
            .Message.ShouldContain("both split files and original");
    }

    [Theory, MutagenModAutoData]
    public void GetSplitModFiles_ThrowsWhenOnlyFirstSplitExists(
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        var modKey = new ModKey("TestMod", ModType.Plugin);
        var splitFile1 = Path.Combine(existingOutputDirectory.Path, "TestMod_1.esp");

        fileSystem.File.WriteAllText(splitFile1, "dummy");

        Should.Throw<SplitModException>(() =>
            MultiModFileAnalysis.GetSplitModFiles(existingOutputDirectory, modKey, fileSystem))
            .Message.ShouldContain("only one split file");
    }

    [Theory, MutagenModAutoData]
    public void GetSplitModFiles_ThrowsWhenOriginalAndSplitsCoexist(
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        var modKey = new ModKey("TestMod", ModType.Plugin);
        var splitFile1 = Path.Combine(existingOutputDirectory.Path, "TestMod_1.esp");
        var splitFile2 = Path.Combine(existingOutputDirectory.Path, "TestMod_2.esp");
        var originalFile = Path.Combine(existingOutputDirectory.Path, "TestMod.esp");

        fileSystem.File.WriteAllText(splitFile1, "dummy");
        fileSystem.File.WriteAllText(splitFile2, "dummy");
        fileSystem.File.WriteAllText(originalFile, "dummy");

        Should.Throw<SplitModException>(() =>
            MultiModFileAnalysis.GetSplitModFiles(existingOutputDirectory, modKey, fileSystem))
            .Message.ShouldContain("both split files and original");
    }

    [Theory, MutagenModAutoData]
    public void WithAutoSplitSupport_ReadsSplitFilesAsReadonly(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        // Create records and write with auto-split
        var originalFormLists = new List<IFormListGetter>();
        for (int i = 0; i < 5; i++)
        {
            var formList = mod.FormLists.AddNew();
            formList.EditorID = formList.FormKey.ToString();
            for (uint j = 0; j < 60; j++)
            {
                var masterKey = new ModKey($"Master_{i}_{j}", ModType.Plugin);
                formList.Items.Add(new FormKey(masterKey, 0x800 + j));
            }
            originalFormLists.Add(formList);
        }

        var originalModKey = mod.ModKey;
        var outputPath = Path.Combine(existingOutputDirectory.Path, mod.ModKey.FileName);

        // Write with auto-split
        var autoSplitWriter = new AutoSplitModWriter(new MultiModFileSplitter());
        autoSplitWriter.Write<ISkyrimMod, ISkyrimModGetter>(
            mod,
            outputPath,
            BinaryWriteParameters.Default with { FileSystem = fileSystem });

        // Read back using builder with auto-split support
        using var readMod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
            .FromPath(new ModPath(originalModKey, outputPath))
            .WithFileSystem(fileSystem)
            .WithAutoSplitSupport()
            .Construct();

        // Verify ModKey
        readMod.ModKey.ShouldBe(originalModKey);

        // Verify all FormLists are present
        readMod.FormLists.Count.ShouldBe(originalFormLists.Count);

        // Verify content
        foreach (var originalFormList in originalFormLists)
        {
            var readFormList = readMod.FormLists.FirstOrDefault(f => f.EditorID == originalFormList.EditorID);
            readFormList.ShouldNotBeNull();
            readFormList.Items.Count.ShouldBe(originalFormList.Items.Count);
        }
    }

    [Theory, MutagenModAutoData]
    public void WithAutoSplitSupport_ReadsSplitFilesAsMutable(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        // Create records and write with auto-split
        var originalFormLists = new List<IFormListGetter>();
        for (int i = 0; i < 5; i++)
        {
            var formList = mod.FormLists.AddNew();
            formList.EditorID = formList.FormKey.ToString();
            for (uint j = 0; j < 60; j++)
            {
                var masterKey = new ModKey($"Master_{i}_{j}", ModType.Plugin);
                formList.Items.Add(new FormKey(masterKey, 0x800 + j));
            }
            originalFormLists.Add(formList);
        }

        var originalModKey = mod.ModKey;
        var outputPath = Path.Combine(existingOutputDirectory.Path, mod.ModKey.FileName);

        // Write with auto-split
        var autoSplitWriter = new AutoSplitModWriter(new MultiModFileSplitter());
        autoSplitWriter.Write<ISkyrimMod, ISkyrimModGetter>(
            mod,
            outputPath,
            BinaryWriteParameters.Default with { FileSystem = fileSystem });

        // Read back using builder with auto-split support as mutable
        var readMod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
            .FromPath(new ModPath(originalModKey, outputPath))
            .WithFileSystem(fileSystem)
            .WithAutoSplitSupport()
            .Mutable()
            .Construct();

        // Verify ModKey
        readMod.ModKey.ShouldBe(originalModKey);

        // Verify it's mutable by adding a record
        var newMisc = readMod.MiscItems.AddNew();
        newMisc.EditorID = "NewItem";

        // Verify all original FormLists are present
        readMod.FormLists.Count.ShouldBe(originalFormLists.Count);
    }

    [Theory, MutagenModAutoData]
    public void WithAutoSplitSupport_FallsBackToNormalImportWhenNoSplitFiles(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        // Write a normal (non-split) mod
        var misc = mod.MiscItems.AddNew();
        misc.EditorID = "TestItem";

        var outputPath = Path.Combine(existingOutputDirectory.Path, mod.ModKey.FileName);
        mod.WriteToBinary(outputPath, new BinaryWriteParameters { FileSystem = fileSystem });

        // Read with auto-split support - should fall back to normal import
        using var readMod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
            .FromPath(new ModPath(mod.ModKey, outputPath))
            .WithFileSystem(fileSystem)
            .WithAutoSplitSupport()
            .Construct();

        readMod.ModKey.ShouldBe(mod.ModKey);
        readMod.MiscItems.Count.ShouldBe(1);
        readMod.MiscItems.First().EditorID.ShouldBe("TestItem");
    }
}
