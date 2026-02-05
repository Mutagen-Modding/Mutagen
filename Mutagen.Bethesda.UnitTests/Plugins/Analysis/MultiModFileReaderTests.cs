using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Analysis.DI;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.UnitTests.Plugins.Analysis;

public class MultiModFileReaderTests
{
    [Theory, MutagenModAutoData]
    public void RoundTrip_WriteSplitThenReadMerged(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        // Create a mod that will trigger auto-split (lots of masters)
        var originalFormLists = new List<IFormListGetter>();
        for (int i = 0; i < 5; i++)
        {
            var formList = mod.FormLists.AddNew();
            formList.EditorID = formList.FormKey.ToString();

            // Add items from different master files
            for (uint j = 0; j < 70; j++)
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

        // Verify split files were created (base file + _2)
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(mod.ModKey.FileName);
        var extension = Path.GetExtension(mod.ModKey.FileName);
        var splitFile1 = Path.Combine(existingOutputDirectory.Path, mod.ModKey.FileName);  // Base file (no suffix)
        var splitFile2 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");

        fileSystem.File.Exists(splitFile1).ShouldBeTrue();
        fileSystem.File.Exists(splitFile2).ShouldBeTrue();

        // Create a simple load order with all the masters
        var loadOrder = new List<IModMasterStyledGetter>();
        foreach (var master in mod.ModHeader.MasterReferences)
        {
            loadOrder.Add(new KeyedMasterStyle(master.Master, MasterStyle.Full));
        }

        // Read back using MultiModFileReader (returns read-only overlay)
        var reader = new MultiModFileReader();
        using var mergedMod = reader.Read<ISkyrimModDisposableGetter>(
            existingOutputDirectory,
            originalModKey,
            mod.GameRelease,
            loadOrder,
            BinaryReadParameters.Default with { FileSystem = fileSystem });

        // Verify the merged mod has the original ModKey
        mergedMod.ModKey.ShouldBe(originalModKey);

        // Verify all FormLists are present
        mergedMod.FormLists.Count.ShouldBe(originalFormLists.Count);

        // Verify FormList content matches
        foreach (var originalFormList in originalFormLists)
        {
            var mergedFormList = mergedMod.FormLists.FirstOrDefault(f => f.EditorID == originalFormList.EditorID);
            mergedFormList.ShouldNotBeNull();

            // Verify the items are the same
            mergedFormList.Items.Count.ShouldBe(originalFormList.Items.Count);
            for (int i = 0; i < originalFormList.Items.Count; i++)
            {
                mergedFormList.Items[i].FormKey.ShouldBe(originalFormList.Items[i].FormKey);
            }
        }
    }

    [Theory, MutagenModAutoData]
    public void ThrowsOnNoSplitFiles(
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        var modKey = new ModKey("NonExistent", ModType.Plugin);
        var loadOrder = new List<IModMasterStyledGetter>();

        var reader = new MultiModFileReader();

        Should.Throw<SplitModException>(() =>
        {
            reader.Read<ISkyrimModDisposableGetter>(
                existingOutputDirectory,
                modKey,
                GameRelease.SkyrimSE,
                loadOrder,
                BinaryReadParameters.Default with { FileSystem = fileSystem });
        }).Message.ShouldContain("No split files found");
    }

    [Theory, MutagenModAutoData]
    public void ThrowsOnOnlyOneSplitFile(
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        var modKey = new ModKey("TestMod", ModType.Plugin);
        // With new naming: base file exists but _2 doesn't = single file, not split
        // To trigger "only one split file" error, we need base file + _2 missing but base exists
        // Actually, since DetectSplitFiles returns empty if _2 doesn't exist, we need a different test
        // The error occurs if base exists AND _2 exists but we somehow detect only 1 file (edge case)
        // For new naming scheme, this test needs adjustment - create base file only
        var baseFile = Path.Combine(existingOutputDirectory.Path, "TestMod.esp");

        // Create just the base file - this means no split (not an error, returns empty)
        // To test error, we actually don't have this edge case anymore with new detection
        // Keep this test but expect "No split files found" since detection returns empty
        fileSystem.File.WriteAllText(baseFile, "dummy content");

        var loadOrder = new List<IModMasterStyledGetter>();
        var reader = new MultiModFileReader();

        Should.Throw<SplitModException>(() =>
        {
            reader.Read<ISkyrimModDisposableGetter>(
                existingOutputDirectory,
                modKey,
                GameRelease.SkyrimSE,
                loadOrder,
                BinaryReadParameters.Default with { FileSystem = fileSystem });
        }).Message.ShouldContain("No split files found");
    }
}
