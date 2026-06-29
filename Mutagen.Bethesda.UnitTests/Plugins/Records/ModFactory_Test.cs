using Loqui;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using System.IO.Abstractions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

internal class NoReleaseModFactoryTest : AModFactoryTest<OblivionMod, IOblivionMod, IOblivionModGetter, OblivionModBinaryOverlay>
{
    public override ModPath ModPath => TestDataPathing.OblivionTestMod;
    public override GameRelease Release => GameRelease.Oblivion;
    public override ILoquiRegistration Registration => OblivionMod_Registration.Instance;
}

internal class ReleaseModFactoryTest : AModFactoryTest<SkyrimMod, ISkyrimMod, ISkyrimModGetter, SkyrimModBinaryOverlay>
{
    public override ModPath ModPath => TestDataPathing.SkyrimTestMod;
    public override GameRelease Release => GameRelease.SkyrimSE;
    public override ILoquiRegistration Registration => SkyrimMod_Registration.Instance;
}

internal class DisposableModFactoryTest : AModFactoryTest<SkyrimMod, ISkyrimMod, ISkyrimModDisposableGetter, SkyrimModBinaryOverlay>
{
    public override ModPath ModPath => TestDataPathing.SkyrimTestMod;
    public override GameRelease Release => GameRelease.SkyrimSE;
    public override ILoquiRegistration Registration => SkyrimMod_Registration.Instance;
}

public abstract class AModFactoryTest<TDirect, TSetter, TGetter, TOverlay>
    where TDirect : IMod
    where TSetter : IMod
    where TGetter : IModGetter
    where TOverlay : IModGetter
{
    public abstract ModPath ModPath { get; }
    public abstract GameRelease Release { get; }
    public abstract ILoquiRegistration Registration { get; }

    [Fact]
    public void Direct()
    {
        var ret = ModFactoryReflection.GetActivator<TDirect>(Registration)(ModPath, Release);
        Assert.IsType<TDirect>(ret);
        Assert.Equal(ModPath.ModKey, ret.ModKey);
    }

    [Fact]
    public void Setter()
    {
        var ret = ModFactoryReflection.GetActivator<TSetter>(Registration)(ModPath, Release);
        Assert.IsType<TDirect>(ret);
        Assert.Equal(ModPath.ModKey, ret.ModKey);
    }

    [Fact]
    public void Getter()
    {
        var ret = ModFactoryReflection.GetActivator<TGetter>(Registration)(ModPath, Release);
        Assert.IsType<TDirect>(ret);
        Assert.Equal(ModPath.ModKey, ret.ModKey);
    }

    [Fact]
    public void Import_Direct()
    {
        var ret = ModFactoryReflection.GetImporter<TDirect>(Registration)(
            ModPath,
            Release);
        Assert.IsType<TDirect>(ret);
        Assert.Equal(ModPath.ModKey, ret.ModKey);
    }

    [Fact]
    public void Import_Setter()
    {
        var ret = ModFactoryReflection.GetImporter<TSetter>(Registration)(
            ModPath,
            Release);
        Assert.IsType<TDirect>(ret);
        Assert.Equal(ModPath.ModKey, ret.ModKey);
    }

    [Fact]
    public void Import_Getter()
    {
        var ret = ModFactoryReflection.GetImporter<TGetter>(Registration)(
            ModPath,
            Release);
        Assert.IsType<TOverlay>(ret);
        Assert.Equal(ModPath.ModKey, ret.ModKey);
    }
}

public class ModFactoryMultiFileTests
{
    [Theory, MutagenModAutoData]
    public void ImportMultiFileGetter_SkyrimMod_CreatesMultiFileOverlay(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        // Use the mod's existing ModKey
        var modKey = mod.ModKey;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(modKey.FileName);
        var extension = Path.GetExtension(modKey.FileName);
        var splitFile1 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_1{extension}");
        var splitFile2 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");

        // Write split files (allow ModKey to be corrected to match the file path)
        mod.WriteToBinary(splitFile1, BinaryWriteParameters.Default with { FileSystem = fileSystem, ModKey = ModKeyOption.CorrectToPath });
        mod.WriteToBinary(splitFile2, BinaryWriteParameters.Default with { FileSystem = fileSystem, ModKey = ModKeyOption.CorrectToPath });

        // Import using ModFactory
        var result = ModFactory<ISkyrimModDisposableGetter>.ImportMultiFileGetter(
            modKey,
            new[] { (ModPath)splitFile1, (ModPath)splitFile2 },
            Array.Empty<ModKey>(),
            GameRelease.SkyrimSE,
            BinaryReadParameters.Default with { FileSystem = fileSystem });

        Assert.NotNull(result);
        Assert.Equal(modKey, result.ModKey);

        // Verify it's a multi-file overlay (not just a regular mod)
        var typeName = result.GetType().Name;
        Assert.Contains("MultiModOverlay", typeName);
    }

    [Theory, MutagenModAutoData]
    public void ImportMultiFileGetter_NonGeneric_SkyrimMod_CreatesMultiFileOverlay(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        // Use the mod's existing ModKey
        var modKey = mod.ModKey;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(modKey.FileName);
        var extension = Path.GetExtension(modKey.FileName);
        var splitFile1 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_1{extension}");
        var splitFile2 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");

        // Write split files (allow ModKey to be corrected to match the file path)
        mod.WriteToBinary(splitFile1, BinaryWriteParameters.Default with { FileSystem = fileSystem, ModKey = ModKeyOption.CorrectToPath });
        mod.WriteToBinary(splitFile2, BinaryWriteParameters.Default with { FileSystem = fileSystem, ModKey = ModKeyOption.CorrectToPath });

        // Import using non-generic ModFactory
        var result = ModFactory.ImportMultiFileGetter(
            modKey,
            new[] { (ModPath)splitFile1, (ModPath)splitFile2 },
            Array.Empty<ModKey>(),
            GameRelease.SkyrimSE,
            BinaryReadParameters.Default with { FileSystem = fileSystem });

        Assert.NotNull(result);
        Assert.Equal(modKey, result.ModKey);

        // Verify it's a multi-file overlay
        var typeName = result.GetType().Name;
        Assert.Contains("MultiModOverlay", typeName);
    }

    [Theory, MutagenModAutoData]
    public void ImportGetterWithMultiFileDetection_SingleFile_ImportsSingleFile(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        // Use the mod's existing ModKey
        var modKey = mod.ModKey;
        var modPath = Path.Combine(existingOutputDirectory.Path, modKey.FileName);

        // Write single file
        mod.WriteToBinary(modPath, BinaryWriteParameters.Default with { FileSystem = fileSystem });

        // Import using ImportGetterWithMultiFileDetection
        var result = ModFactory.ImportGetterWithMultiFileDetection(
            (ModPath)modPath,
            Array.Empty<ModKey>(),
            GameRelease.SkyrimSE,
            BinaryReadParameters.Default with { FileSystem = fileSystem });

        Assert.NotNull(result);
        Assert.Equal(modKey, result.ModKey);

        // Verify it's NOT a multi-file overlay (should be single file overlay)
        var typeName = result.GetType().Name;
        Assert.DoesNotContain("MultiModOverlay", typeName);
    }

    [Theory, MutagenModAutoData]
    public void ImportGetterWithMultiFileDetection_SplitFiles_ImportsMultiFile(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        // Use the mod's existing ModKey
        var modKey = mod.ModKey;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(modKey.FileName);
        var extension = Path.GetExtension(modKey.FileName);
        var basePath = Path.Combine(existingOutputDirectory.Path, modKey.FileName);
        // First file is the base path (no suffix), subsequent files have _2, _3, etc.
        var splitFile1 = basePath;  // First file uses base path
        var splitFile2 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");

        // Write split files (allow ModKey to be corrected to match the file path)
        mod.WriteToBinary(splitFile1, BinaryWriteParameters.Default with { FileSystem = fileSystem });
        mod.WriteToBinary(splitFile2, BinaryWriteParameters.Default with { FileSystem = fileSystem, ModKey = ModKeyOption.CorrectToPath });

        // Import using ImportGetterWithMultiFileDetection with base path
        var result = ModFactory.ImportGetterWithMultiFileDetection(
            (ModPath)basePath,
            Array.Empty<ModKey>(),
            GameRelease.SkyrimSE,
            BinaryReadParameters.Default with { FileSystem = fileSystem });

        Assert.NotNull(result);
        Assert.Equal(modKey, result.ModKey);

        // Verify it's a multi-file overlay
        var typeName = result.GetType().Name;
        Assert.Contains("MultiModOverlay", typeName);
    }

    [Theory, MutagenModAutoData]
    public void ImportGetterWithMultiFileDetection_Generic_SplitFiles_ImportsMultiFile(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        // Use the mod's existing ModKey
        var modKey = mod.ModKey;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(modKey.FileName);
        var extension = Path.GetExtension(modKey.FileName);
        var basePath = Path.Combine(existingOutputDirectory.Path, modKey.FileName);
        // First file is the base path (no suffix), subsequent files have _2, _3, etc.
        var splitFile1 = basePath;  // First file uses base path
        var splitFile2 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");

        // Write split files
        mod.WriteToBinary(splitFile1, BinaryWriteParameters.Default with { FileSystem = fileSystem });
        mod.WriteToBinary(splitFile2, BinaryWriteParameters.Default with { FileSystem = fileSystem, ModKey = ModKeyOption.CorrectToPath });

        // Import using generic ModFactory
        var result = ModFactory<ISkyrimModDisposableGetter>.ImportGetterWithMultiFileDetection(
            (ModPath)basePath,
            Array.Empty<ModKey>(),
            GameRelease.SkyrimSE,
            BinaryReadParameters.Default with { FileSystem = fileSystem });

        Assert.NotNull(result);
        Assert.Equal(modKey, result.ModKey);

        // Verify it's a multi-file overlay
        var typeName = result.GetType().Name;
        Assert.Contains("MultiModOverlay", typeName);
    }

    [Theory, MutagenModAutoData]
    public void ImportGetterWithMultiFileDetection_Generic_SingleFile_ImportsSingleFile(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        // Use the mod's existing ModKey
        var modKey = mod.ModKey;
        var modPath = Path.Combine(existingOutputDirectory.Path, modKey.FileName);

        // Write single file
        mod.WriteToBinary(modPath, BinaryWriteParameters.Default with { FileSystem = fileSystem });

        // Import using generic ModFactory
        var result = ModFactory<ISkyrimModDisposableGetter>.ImportGetterWithMultiFileDetection(
            (ModPath)modPath,
            Array.Empty<ModKey>(),
            GameRelease.SkyrimSE,
            BinaryReadParameters.Default with { FileSystem = fileSystem });

        Assert.NotNull(result);
        Assert.Equal(modKey, result.ModKey);

        // Verify it's NOT a multi-file overlay
        var typeName = result.GetType().Name;
        Assert.DoesNotContain("MultiModOverlay", typeName);
    }

    [Theory, MutagenModAutoData]
    public void ImportSetterWithMultiFileDetection_SingleFile_ImportsMutableMod(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        // Use the mod's existing ModKey
        var modKey = mod.ModKey;
        var modPath = Path.Combine(existingOutputDirectory.Path, modKey.FileName);

        // Write single file
        mod.WriteToBinary(modPath, BinaryWriteParameters.Default with { FileSystem = fileSystem });

        // Import using ImportSetterWithMultiFileDetection
        var result = ModFactory.ImportSetterWithMultiFileDetection(
            (ModPath)modPath,
            Array.Empty<ModKey>(),
            GameRelease.SkyrimSE,
            BinaryReadParameters.Default with { FileSystem = fileSystem });

        Assert.NotNull(result);
        Assert.Equal(modKey, result.ModKey);

        // Verify it's mutable (implements IMod)
        Assert.IsAssignableFrom<IMod>(result);
    }

    [Theory, MutagenModAutoData]
    public void ImportSetterWithMultiFileDetection_SplitFiles_ImportsMutableMod(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        // Use the mod's existing ModKey
        var modKey = mod.ModKey;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(modKey.FileName);
        var extension = Path.GetExtension(modKey.FileName);
        var basePath = Path.Combine(existingOutputDirectory.Path, modKey.FileName);
        // First file is the base path (no suffix), subsequent files have _2, _3, etc.
        var splitFile1 = basePath;  // First file uses base path
        var splitFile2 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");

        // Write split files
        mod.WriteToBinary(splitFile1, BinaryWriteParameters.Default with { FileSystem = fileSystem });
        mod.WriteToBinary(splitFile2, BinaryWriteParameters.Default with { FileSystem = fileSystem, ModKey = ModKeyOption.CorrectToPath });

        // Import using ImportSetterWithMultiFileDetection with base path
        var result = ModFactory.ImportSetterWithMultiFileDetection(
            (ModPath)basePath,
            Array.Empty<ModKey>(),
            GameRelease.SkyrimSE,
            BinaryReadParameters.Default with { FileSystem = fileSystem });

        Assert.NotNull(result);
        Assert.Equal(modKey, result.ModKey);

        // Verify it's mutable (implements IMod)
        Assert.IsAssignableFrom<IMod>(result);
    }

    [Theory, MutagenModAutoData]
    public void ImportSetterWithMultiFileDetection_Generic_SplitFiles_ImportsMutableMod(
        SkyrimMod mod,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        // Use the mod's existing ModKey
        var modKey = mod.ModKey;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(modKey.FileName);
        var extension = Path.GetExtension(modKey.FileName);
        var basePath = Path.Combine(existingOutputDirectory.Path, modKey.FileName);
        // First file is the base path (no suffix), subsequent files have _2, _3, etc.
        var splitFile1 = basePath;  // First file uses base path
        var splitFile2 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");

        // Write split files
        mod.WriteToBinary(splitFile1, BinaryWriteParameters.Default with { FileSystem = fileSystem });
        mod.WriteToBinary(splitFile2, BinaryWriteParameters.Default with { FileSystem = fileSystem, ModKey = ModKeyOption.CorrectToPath });

        // Import using generic ModFactory
        var result = ModFactory<ISkyrimMod>.ImportSetterWithMultiFileDetection(
            (ModPath)basePath,
            Array.Empty<ModKey>(),
            GameRelease.SkyrimSE,
            BinaryReadParameters.Default with { FileSystem = fileSystem });

        Assert.NotNull(result);
        Assert.Equal(modKey, result.ModKey);

        // Verify it's mutable (implements IMod)
        Assert.IsAssignableFrom<IMod>(result);
    }

    [Theory, MutagenAutoData]
    public void ImportMultiFileGetter_SplitFileReferencesBaseMod_DoesNotThrowSelfReference(
        ModKey modKey,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        // Scenario: Mod.esp is split into Mod.esp and Mod_2.esp
        // Mod_2.esp references a record in Mod.esp, creating a master dependency
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(modKey.FileName);
        var extension = Path.GetExtension(modKey.FileName);
        var mod2Key = new ModKey($"{fileNameWithoutExtension}_2", modKey.Type);

        // Create first split file with a record
        var mod1 = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);
        var flst1 = new FormList(new FormKey(modKey, 0x800), SkyrimRelease.SkyrimSE);
        flst1.EditorID = "TestFormList1";
        mod1.FormLists.Add(flst1);

        // Create second split file that references mod1's FormList, creating a cross-mod master
        var mod2 = new SkyrimMod(mod2Key, SkyrimRelease.SkyrimSE);
        var flst2 = new FormList(new FormKey(mod2Key, 0x900), SkyrimRelease.SkyrimSE);
        flst2.EditorID = "TestFormList2";
        flst2.Items.Add(flst1.ToLink());
        mod2.FormLists.Add(flst2);

        var splitFile1 = Path.Combine(existingOutputDirectory.Path, modKey.FileName);
        var splitFile2 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");

        // Write split files
        mod1.WriteToBinary(splitFile1, BinaryWriteParameters.Default with { FileSystem = fileSystem });
        mod2.WriteToBinary(splitFile2, BinaryWriteParameters.Default with { FileSystem = fileSystem });

        // This should NOT throw SelfReferenceException
        var result = ModFactory.ImportMultiFileGetter(
            modKey,
            new[] { (ModPath)splitFile1, (ModPath)splitFile2 },
            Array.Empty<ModKey>(),
            GameRelease.SkyrimSE,
            BinaryReadParameters.Default with { FileSystem = fileSystem });

        Assert.NotNull(result);
        Assert.Equal(modKey, result.ModKey);

        // Verify the self-reference was filtered out of masters
        Assert.DoesNotContain(result.MasterReferences, m => m.Master == modKey);
    }

    [Theory, MutagenAutoData]
    public void ImportMultiFileGetter_SplitFilesCrossReference_FiltersAllSplitModKeys(
        ModKey modKey,
        DirectoryPath existingOutputDirectory,
        IFileSystem fileSystem)
    {
        // Scenario: Mod.esp is split into Mod.esp, Mod_2.esp, and Mod_3.esp
        // Mod_2.esp references a record in Mod.esp
        // Mod_3.esp references records in both Mod.esp and Mod_2.esp
        // All cross-references between split files should be filtered out
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(modKey.FileName);
        var extension = Path.GetExtension(modKey.FileName);
        var mod2Key = new ModKey($"{fileNameWithoutExtension}_2", modKey.Type);
        var mod3Key = new ModKey($"{fileNameWithoutExtension}_3", modKey.Type);

        // Create first split file with a record
        var mod1 = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);
        var flst1 = new FormList(new FormKey(modKey, 0x800), SkyrimRelease.SkyrimSE);
        flst1.EditorID = "TestFormList1";
        mod1.FormLists.Add(flst1);

        // Create second split file referencing mod1's record
        var mod2 = new SkyrimMod(mod2Key, SkyrimRelease.SkyrimSE);
        var flst2 = new FormList(new FormKey(mod2Key, 0x900), SkyrimRelease.SkyrimSE);
        flst2.EditorID = "TestFormList2";
        flst2.Items.Add(flst1.ToLink());
        mod2.FormLists.Add(flst2);

        // Create third split file referencing records in both mod1 and mod2
        var mod3 = new SkyrimMod(mod3Key, SkyrimRelease.SkyrimSE);
        var flst3 = new FormList(new FormKey(mod3Key, 0xA00), SkyrimRelease.SkyrimSE);
        flst3.EditorID = "TestFormList3";
        flst3.Items.Add(flst1.ToLink());
        flst3.Items.Add(flst2.ToLink());
        mod3.FormLists.Add(flst3);

        var splitFile1 = Path.Combine(existingOutputDirectory.Path, modKey.FileName);
        var splitFile2 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_2{extension}");
        var splitFile3 = Path.Combine(existingOutputDirectory.Path, $"{fileNameWithoutExtension}_3{extension}");

        // Write split files
        mod1.WriteToBinary(splitFile1, BinaryWriteParameters.Default with { FileSystem = fileSystem });
        mod2.WriteToBinary(splitFile2, BinaryWriteParameters.Default with { FileSystem = fileSystem });
        mod3.WriteToBinary(splitFile3, BinaryWriteParameters.Default with { FileSystem = fileSystem });

        // This should NOT throw
        var result = ModFactory.ImportMultiFileGetter(
            modKey,
            new[] { (ModPath)splitFile1, (ModPath)splitFile2, (ModPath)splitFile3 },
            Array.Empty<ModKey>(),
            GameRelease.SkyrimSE,
            BinaryReadParameters.Default with { FileSystem = fileSystem });

        Assert.NotNull(result);
        Assert.Equal(modKey, result.ModKey);

        // Verify all split file cross-references were filtered out
        Assert.DoesNotContain(result.MasterReferences, m => m.Master == modKey);
        Assert.DoesNotContain(result.MasterReferences, m => m.Master == mod2Key);
        Assert.DoesNotContain(result.MasterReferences, m => m.Master == mod3Key);
    }
}
