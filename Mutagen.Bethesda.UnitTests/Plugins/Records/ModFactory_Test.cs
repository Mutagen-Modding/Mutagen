using Loqui;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Masters;
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
            Array.Empty<IModMasterStyledGetter>(),
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
            Array.Empty<IModMasterStyledGetter>(),
            GameRelease.SkyrimSE,
            BinaryReadParameters.Default with { FileSystem = fileSystem });

        Assert.NotNull(result);
        Assert.Equal(modKey, result.ModKey);

        // Verify it's a multi-file overlay
        var typeName = result.GetType().Name;
        Assert.Contains("MultiModOverlay", typeName);
    }
}
