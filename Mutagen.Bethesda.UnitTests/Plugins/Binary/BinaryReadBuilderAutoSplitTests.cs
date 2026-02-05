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
        var modPath = new ModPath(modKey, outputPath);
        MultiModFileAnalysis.IsMultiModFile(modPath, fileSystem).ShouldBeTrue();
    }

    [Theory, MutagenModAutoData]
    public void IsMultiModFile_ReturnsFalseWhenNoSplitFiles(
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        var modKey = new ModKey("NonExistent", ModType.Plugin);
        var modPath = new ModPath(modKey, Path.Combine(existingOutputDirectory.Path, modKey.FileName));

        MultiModFileAnalysis.IsMultiModFile(modPath, fileSystem).ShouldBeFalse();
    }

    [Theory, MutagenModAutoData]
    public void IsMultiModFile_ReturnsFalseWhenOnlyBaseFileExists(
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        var modKey = new ModKey("TestMod", ModType.Plugin);
        var baseFile = Path.Combine(existingOutputDirectory.Path, "TestMod.esp");
        var modPath = new ModPath(modKey, baseFile);

        // Create just the base file - no _2 means not a split set
        fileSystem.File.WriteAllText(baseFile, "dummy");

        // With new naming, a single base file without _2 is not considered a split set
        MultiModFileAnalysis.IsMultiModFile(modPath, fileSystem).ShouldBeFalse();
    }

    [Theory, MutagenModAutoData]
    public void IsMultiModFile_ReturnsTrueWhenBaseAndSplit2Exist(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        var modKey = mod.ModKey;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(modKey.FileName);
        var extension = Path.GetExtension(modKey.FileName);

        // With new naming: base file IS the first split file
        var baseFile = Path.Combine(existingOutputDirectory.Path, modKey.FileName);
        var splitFile2 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");

        fileSystem.File.WriteAllText(baseFile, "dummy");
        fileSystem.File.WriteAllText(splitFile2, "dummy");

        var modPath = new ModPath(modKey, baseFile);
        // This is now valid - base file + _2 = a valid split set
        MultiModFileAnalysis.IsMultiModFile(modPath, fileSystem).ShouldBeTrue();
    }

    [Theory, MutagenModAutoData]
    public void GetSplitModFiles_ReturnsEmptyWhenOnlyBaseFileExists(
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        var modKey = new ModKey("TestMod", ModType.Plugin);
        var baseFile = Path.Combine(existingOutputDirectory.Path, "TestMod.esp");
        var modPath = new ModPath(modKey, baseFile);

        fileSystem.File.WriteAllText(baseFile, "dummy");

        // With new naming, a single base file without _2 returns empty (not a split set)
        var result = MultiModFileAnalysis.GetSplitModFiles(modPath, fileSystem);
        result.ShouldBeEmpty();
    }

    [Theory, MutagenModAutoData]
    public void GetSplitModFiles_ReturnsFilesWhenBaseAndSplit2Exist(
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        var modKey = new ModKey("TestMod", ModType.Plugin);
        var baseFile = Path.Combine(existingOutputDirectory.Path, "TestMod.esp");
        var splitFile2 = Path.Combine(existingOutputDirectory.Path, "TestMod_2.esp");

        fileSystem.File.WriteAllText(baseFile, "dummy");
        fileSystem.File.WriteAllText(splitFile2, "dummy");

        var modPath = new ModPath(modKey, baseFile);
        // With new naming, base file + _2 is a valid split set
        var result = MultiModFileAnalysis.GetSplitModFiles(modPath, fileSystem);
        result.Count.ShouldBe(2);
        result[0].Path.ShouldBe(baseFile);
        result[1].Path.ShouldBe(splitFile2);
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
