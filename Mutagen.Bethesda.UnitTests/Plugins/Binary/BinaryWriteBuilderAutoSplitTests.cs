using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using System.IO.Abstractions;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary;

public class BinaryWriteBuilderAutoSplitTests
{
    [Theory, MutagenModAutoData]
    public void WithAutoSplit_EnablesAutoSplitting(SkyrimMod mod, DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // Combine mod's ModKey and DirectoryPath to create a valid output path
        var outputPath = Path.Combine(existingOutputDirectory.Path, mod.ModKey.FileName);

        // Add a simple record so the mod isn't empty
        var misc = mod.MiscItems.AddNew();
        misc.EditorID = "TestItem";

        // Use the builder with auto-split enabled
        mod.BeginWrite
            .ToPath(outputPath, fileSystem)
            .WithNoLoadOrder()
            .WithAutoSplit()
            .Write();

        // File should be written
        fileSystem.File.Exists(outputPath).ShouldBeTrue();
    }

    [Theory, MutagenModAutoData]
    public void WithAutoSplit_SplitsModWithTooManyMasters(SkyrimMod mod, DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // Combine mod's ModKey and DirectoryPath to create a valid output path
        var outputPath = Path.Combine(existingOutputDirectory.Path, mod.ModKey.FileName);

        // Create a mod that would exceed master limits
        // Add form lists that reference many different mods
        var originalFormLists = new List<IFormListGetter>();
        for (int i = 0; i < 10; i++)
        {
            var formList = mod.FormLists.AddNew();
            // Set EditorID to FormKey for tracking after split
            formList.EditorID = formList.FormKey.ToString();

            // Add items from different master files (simulating many masters)
            for (uint j = 0; j < 30; j++)
            {
                var masterKey = new ModKey($"Master_{i}_{j}", ModType.Plugin);
                formList.Items.Add(new FormKey(masterKey, 0x800 + j));
            }

            originalFormLists.Add(formList);
        }

        // Write with auto-split
        mod.BeginWrite
            .ToPath(outputPath, fileSystem)
            .WithNoLoadOrder()
            .WithAutoSplit()
            .Write();

        // Verify split files were created
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(mod.ModKey.FileName);
        var extension = Path.GetExtension(mod.ModKey.FileName);
        var splitFile1Path = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_1{extension}");
        var splitFile2Path = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");

        fileSystem.File.Exists(splitFile1Path).ShouldBeTrue();
        fileSystem.File.Exists(splitFile2Path).ShouldBeTrue();

        // Re-import the split mods and verify content
        var splitMod1 = SkyrimMod.CreateFromBinaryOverlay(
            splitFile1Path,
            (SkyrimRelease)mod.GameRelease,
            new BinaryReadParameters() { FileSystem = fileSystem });
        var splitMod2 = SkyrimMod.CreateFromBinaryOverlay(
            splitFile2Path,
            (SkyrimRelease)mod.GameRelease,
            new BinaryReadParameters() { FileSystem = fileSystem });

        // Collect all FormLists from both split mods
        var reimportedFormLists = new List<IFormListGetter>();
        reimportedFormLists.AddRange(splitMod1.FormLists);
        reimportedFormLists.AddRange(splitMod2.FormLists);

        // Verify we have the same number of FormLists
        reimportedFormLists.Count.ShouldBe(originalFormLists.Count);

        // Verify each FormList by EditorID (which is set to FormKey)
        var expectedEdids = originalFormLists.Select(f => f.EditorID).ToHashSet();
        var reimportedEdids = reimportedFormLists.Select(f => f.EditorID).ToHashSet();

        reimportedEdids.ShouldBe(expectedEdids);

        // Verify the content of each FormList matches
        foreach (var originalFormList in originalFormLists)
        {
            var reimportedFormList = reimportedFormLists.FirstOrDefault(f => f.EditorID == originalFormList.EditorID);
            reimportedFormList.ShouldNotBeNull();

            // Verify the items are the same
            reimportedFormList.Items.Count.ShouldBe(originalFormList.Items.Count);
            for (int i = 0; i < originalFormList.Items.Count; i++)
            {
                reimportedFormList.Items[i].FormKey.ShouldBe(originalFormList.Items[i].FormKey);
            }
        }
    }

    [Theory, MutagenModAutoData]
    public void WithAutoSplit_ThrowsWhenSingleRecordExceedsMasterLimit(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        var outputPath = Path.Combine(existingOutputDirectory.Path, mod.ModKey.FileName);

        // Create a single FormList that references more than 255 masters
        // This cannot be split because the record itself exceeds the limit
        var formList = mod.FormLists.AddNew();
        formList.EditorID = "MassiveFormList";

        // Add items from 300 different master files
        for (uint i = 0; i < 300; i++)
        {
            var masterKey = new ModKey($"Master_{i}", ModType.Plugin);
            formList.Items.Add(new FormKey(masterKey, 0x800));
        }

        // Auto-split should still throw because a single record exceeds the limit
        Should.Throw<Mutagen.Bethesda.Plugins.Exceptions.TooManyMastersException>(() =>
        {
            mod.BeginWrite
                .ToPath(outputPath, fileSystem)
                .WithNoLoadOrder()
                .WithAutoSplit()
                .Write();
        });
    }

    [Theory, MutagenModAutoData]
    public void WithAutoSplit_LowerFormKeyPlaceholderEdgeCase(
        ModKey modKey,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        var mod = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE, forceUseLowerFormIDRanges: true);
        var outputPath = Path.Combine(existingOutputDirectory.Path, mod.ModKey.FileName);

        // Create FormLists that reference masters to exceed the master limit
        // Create one FormList per master (256 masters) to efficiently trigger splitting
        for (int i = 0; i < 256; i++)
        {
            var formList = mod.FormLists.AddNew();
            formList.EditorID = $"FormList{i:D3}";

            // Each FormList references a unique master
            var masterKey = new ModKey($"Master{i:D3}.esp", ModType.Plugin);
            formList.Items.Add(new FormKey(masterKey, 0x800));
        }

        // Capture the original FormKeys before splitting
        var originalFormKeys = mod.FormLists.Select(f => f.FormKey).ToHashSet();

        // Verify that records were actually created in the mod (not external)
        foreach (var formKey in originalFormKeys)
        {
            formKey.ModKey.ShouldBe(mod.ModKey, "All FormLists should belong to the mod being split");
        }

        // Write with auto-split
        mod.BeginWrite
            .ToPath(outputPath, fileSystem)
            .WithNoLoadOrder()
            .WithForcedLowerFormIdRangeUsage(true)
            .WithAutoSplit()
            .Write();

        // Verify split files were created
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(mod.ModKey.FileName);
        var extension = Path.GetExtension(mod.ModKey.FileName);
        var splitFile1Path = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_1{extension}");
        var splitFile2Path = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");

        fileSystem.File.Exists(splitFile1Path).ShouldBeTrue();
        fileSystem.File.Exists(splitFile2Path).ShouldBeTrue();

        // Re-import both split mods
        var splitMod1 = SkyrimMod.CreateFromBinaryOverlay(
            splitFile1Path,
            (SkyrimRelease)mod.GameRelease,
            new BinaryReadParameters() { FileSystem = fileSystem });
        var splitMod2 = SkyrimMod.CreateFromBinaryOverlay(
            splitFile2Path,
            (SkyrimRelease)mod.GameRelease,
            new BinaryReadParameters() { FileSystem = fileSystem });

        // Collect all FormKeys from both split mods
        var allFormKeys = new HashSet<FormKey>();
        var splitMod1FormKeys = new List<FormKey>();
        var splitMod2FormKeys = new List<FormKey>();

        foreach (var formList in splitMod1.FormLists)
        {
            splitMod1FormKeys.Add(formList.FormKey);
            // Verify no collision when adding
            allFormKeys.Add(formList.FormKey).ShouldBeTrue(
                $"FormKey collision detected: {formList.FormKey} appears in both split files");
        }
        foreach (var formList in splitMod2.FormLists)
        {
            splitMod2FormKeys.Add(formList.FormKey);
            // Verify no collision when adding
            allFormKeys.Add(formList.FormKey).ShouldBeTrue(
                $"FormKey collision detected: {formList.FormKey} appears in both split files");
        }

        // Debug: Print out the FormKeys to see what's happening
        System.Console.WriteLine($"Split file 1 FormKeys: {string.Join(", ", splitMod1FormKeys)}");
        System.Console.WriteLine($"Split file 2 FormKeys: {string.Join(", ", splitMod2FormKeys)}");

        // Verify all FormLists are accounted for
        allFormKeys.Count.ShouldBe(originalFormKeys.Count,
            "Total number of FormLists across split files should match original");

        // VERIFY CORRECT BEHAVIOR: Split files should have non-overlapping FormID ranges
        // Extract just the FormID (not the full FormKey) from each file
        var splitMod1FormIDs = splitMod1FormKeys.Select(fk => fk.ID).ToHashSet();
        var splitMod2FormIDs = splitMod2FormKeys.Select(fk => fk.ID).ToHashSet();

        // Find overlapping FormIDs
        var overlappingFormIDs = splitMod1FormIDs.Intersect(splitMod2FormIDs).ToList();

        // Debug output to help diagnose issues
        System.Console.WriteLine($"Split file 1 has {splitMod1FormIDs.Count} unique FormIDs");
        System.Console.WriteLine($"Split file 2 has {splitMod2FormIDs.Count} unique FormIDs");
        if (overlappingFormIDs.Any())
        {
            System.Console.WriteLine($"ERROR: {overlappingFormIDs.Count} FormIDs overlap between split files!");
            System.Console.WriteLine($"Overlapping FormIDs: {string.Join(", ", overlappingFormIDs.Select(id => $"0x{id:X6}"))}");
        }

        // ASSERTION: Split files MUST have non-overlapping FormID ranges
        // This prevents issues when:
        // 1. Merging split files back together
        // 2. Loading both files in a load order simultaneously
        // 3. Extending one of the split files with more records
        overlappingFormIDs.ShouldBeEmpty(
            "Split files must have non-overlapping FormID ranges. " +
            $"Found {overlappingFormIDs.Count} overlapping FormIDs: {string.Join(", ", overlappingFormIDs.Select(id => $"0x{id:X6}"))}");
    }
}
