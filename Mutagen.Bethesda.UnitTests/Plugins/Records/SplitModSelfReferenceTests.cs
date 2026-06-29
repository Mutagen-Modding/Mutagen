using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Shouldly;
using System.IO.Abstractions;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

/// <summary>
/// Tests that multi-file split mod import handles self-references correctly.
///
/// When a mod is split into multiple files (e.g. MyMod.esp + MyMod_2.esp) because it exceeds
/// master limits, records in MyMod_2.esp may reference records in MyMod.esp. This causes
/// MyMod_2.esp to list MyMod.esp as a master. When re-importing with the base mod key,
/// this becomes a self-reference that should be handled gracefully rather than throwing.
/// </summary>
public class SplitModSelfReferenceTests
{
    private static readonly ModKey TestModKey = new("SplitTest", ModType.Plugin);
    private static readonly ModKey SplitModKey2 = new("SplitTest_2", ModType.Plugin);
    private static readonly ModKey SplitModKey3 = new("SplitTest_3", ModType.Plugin);

    [Theory, MutagenModAutoData]
    public void SplitFile_ReferencesBaseMod_DoesNotThrowSelfReference(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // Scenario: MyMod.esp has recordA. MyMod_2.esp has recordB that references recordA.
        // When written, MyMod_2.esp lists MyMod.esp as a master.
        // Importing both with the base mod key should not throw SelfReferenceException.
        var mod1 = new SkyrimMod(TestModKey, SkyrimRelease.SkyrimSE);
        var flst1 = new FormList(new FormKey(TestModKey, 0x800), SkyrimRelease.SkyrimSE);
        flst1.EditorID = "RecordA";
        mod1.FormLists.Add(flst1);

        var mod2 = new SkyrimMod(SplitModKey2, SkyrimRelease.SkyrimSE);
        var flst2 = new FormList(new FormKey(SplitModKey2, 0x900), SkyrimRelease.SkyrimSE);
        flst2.EditorID = "RecordB";
        flst2.Items.Add(flst1.ToLink());
        mod2.FormLists.Add(flst2);

        var file1 = WriteSplitFile(mod1, existingOutputDirectory, fileSystem, 1);
        var file2 = WriteSplitFile(mod2, existingOutputDirectory, fileSystem, 2);

        // Import using ImportMultiFileGetter with the base mod key for all files,
        // simulating what ImportGetterWithMultiFileDetection does
        using var result = ModFactory.ImportMultiFileGetter(
            TestModKey,
            new[] { new ModPath(TestModKey, file1), new ModPath(TestModKey, file2) },
            Array.Empty<ModKey>(),
            GameRelease.SkyrimSE,
            new BinaryReadParameters { FileSystem = fileSystem });

        result.ShouldNotBeNull();
        result.ModKey.ShouldBe(TestModKey);

        // The self-reference should be filtered out of the merged masters
        result.MasterReferences.ShouldNotContain(m => m.Master == TestModKey);
        result.MasterReferences.ShouldNotContain(m => m.Master == SplitModKey2);
    }

    [Theory, MutagenModAutoData]
    public void SplitFile_ThreeWayCrossReference_DoesNotThrowSelfReference(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // Scenario: Three split files with cross-references:
        // MyMod.esp has recordA
        // MyMod_2.esp has recordB referencing recordA (masters MyMod.esp)
        // MyMod_3.esp has recordC referencing recordA and recordB (masters MyMod.esp, MyMod_2.esp)
        var mod1 = new SkyrimMod(TestModKey, SkyrimRelease.SkyrimSE);
        var flst1 = new FormList(new FormKey(TestModKey, 0x800), SkyrimRelease.SkyrimSE);
        flst1.EditorID = "RecordA";
        mod1.FormLists.Add(flst1);

        var mod2 = new SkyrimMod(SplitModKey2, SkyrimRelease.SkyrimSE);
        var flst2 = new FormList(new FormKey(SplitModKey2, 0x900), SkyrimRelease.SkyrimSE);
        flst2.EditorID = "RecordB";
        flst2.Items.Add(flst1.ToLink());
        mod2.FormLists.Add(flst2);

        var mod3 = new SkyrimMod(SplitModKey3, SkyrimRelease.SkyrimSE);
        var flst3 = new FormList(new FormKey(SplitModKey3, 0xA00), SkyrimRelease.SkyrimSE);
        flst3.EditorID = "RecordC";
        flst3.Items.Add(flst1.ToLink());
        flst3.Items.Add(flst2.ToLink());
        mod3.FormLists.Add(flst3);

        var file1 = WriteSplitFile(mod1, existingOutputDirectory, fileSystem, 1);
        var file2 = WriteSplitFile(mod2, existingOutputDirectory, fileSystem, 2);
        var file3 = WriteSplitFile(mod3, existingOutputDirectory, fileSystem, 3);

        using var result = ModFactory.ImportMultiFileGetter(
            TestModKey,
            new[] { new ModPath(TestModKey, file1), new ModPath(TestModKey, file2), new ModPath(TestModKey, file3) },
            Array.Empty<ModKey>(),
            GameRelease.SkyrimSE,
            new BinaryReadParameters { FileSystem = fileSystem });

        result.ShouldNotBeNull();
        result.ModKey.ShouldBe(TestModKey);

        // All split-file cross-references should be filtered out
        result.MasterReferences.ShouldNotContain(m => m.Master == TestModKey);
        result.MasterReferences.ShouldNotContain(m => m.Master == SplitModKey2);
        result.MasterReferences.ShouldNotContain(m => m.Master == SplitModKey3);
    }

    [Theory, MutagenModAutoData]
    public void ImportGetterWithMultiFileDetection_CrossReferencingSplitFiles_DoesNotThrow(
        DirectoryPath existingOutputDirectory, IFileSystem fileSystem)
    {
        // End-to-end test through the auto-detection path, which is the actual
        // code path that triggers the bug by forcing the base mod key on all split files.
        var mod1 = new SkyrimMod(TestModKey, SkyrimRelease.SkyrimSE);
        var flst1 = new FormList(new FormKey(TestModKey, 0x800), SkyrimRelease.SkyrimSE);
        flst1.EditorID = "RecordA";
        mod1.FormLists.Add(flst1);

        var mod2 = new SkyrimMod(SplitModKey2, SkyrimRelease.SkyrimSE);
        var flst2 = new FormList(new FormKey(SplitModKey2, 0x900), SkyrimRelease.SkyrimSE);
        flst2.EditorID = "RecordB";
        flst2.Items.Add(flst1.ToLink());
        mod2.FormLists.Add(flst2);

        var file1 = WriteSplitFile(mod1, existingOutputDirectory, fileSystem, 1);
        var file2 = WriteSplitFile(mod2, existingOutputDirectory, fileSystem, 2);

        // Go through the full auto-detection path
        using var result = ModFactory.ImportGetterWithMultiFileDetection(
            new ModPath(TestModKey, file1),
            Array.Empty<ModKey>(),
            GameRelease.SkyrimSE,
            new BinaryReadParameters { FileSystem = fileSystem });

        result.ShouldNotBeNull();
        result.ModKey.ShouldBe(TestModKey);
        result.MasterReferences.ShouldNotContain(m => m.Master == TestModKey);
        result.MasterReferences.ShouldNotContain(m => m.Master == SplitModKey2);
    }

    #region Helpers

    private static string WriteSplitFile(
        IMod mod, DirectoryPath outputDir, IFileSystem fileSystem, int index)
    {
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(TestModKey.FileName);
        var extension = Path.GetExtension(TestModKey.FileName);

        var path = index == 1
            ? Path.Combine(outputDir.Path, TestModKey.FileName)
            : Path.Combine(outputDir.Path, $"{fileNameWithoutExtension}_{index}{extension}");

        mod.WriteToBinary(path, new BinaryWriteParameters
        {
            FileSystem = fileSystem,
            ModKey = ModKeyOption.NoCheck
        });

        return path;
    }

    #endregion
}
