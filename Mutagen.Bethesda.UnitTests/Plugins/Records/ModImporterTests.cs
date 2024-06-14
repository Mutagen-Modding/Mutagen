using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records.DI;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class ModImporterTests
{
    [Theory, MutagenModAutoData]
    public void GenericClassGetter(
        IFileSystem fileSystem,
        SkyrimMod mod,
        Npc npc,
        DirectoryPath existingDir,
        ModImporter<ISkyrimModGetter> sut)
    {
        var path = Path.Combine(existingDir, mod.ModKey.FileName);
        mod.WriteToBinary(path, new BinaryWriteParameters()
        {
            FileSystem = fileSystem
        });
        var import = sut.Import(path);
        import.Should().BeOfType<SkyrimModBinaryOverlay>();
    }
    
    [Theory, MutagenModAutoData]
    public void GenericClassSetter(
        IFileSystem fileSystem,
        SkyrimMod mod,
        Npc npc,
        DirectoryPath existingDir,
        ModImporter<ISkyrimMod> sut)
    {
        var path = Path.Combine(existingDir, mod.ModKey.FileName);
        mod.WriteToBinary(path, new BinaryWriteParameters()
        {
            FileSystem = fileSystem
        });
        var import = sut.Import(path);
        import.Should().BeOfType<SkyrimMod>();
    }
    
    [Theory, MutagenModAutoData]
    public void GenericClassDirect(
        IFileSystem fileSystem,
        SkyrimMod mod,
        Npc npc,
        DirectoryPath existingDir,
        ModImporter<SkyrimMod> sut)
    {
        var path = Path.Combine(existingDir, mod.ModKey.FileName);
        mod.WriteToBinary(path, new BinaryWriteParameters()
        {
            FileSystem = fileSystem
        });
        var import = sut.Import(path);
        import.Should().BeOfType<SkyrimMod>();
    }
    
    [Theory, MutagenModAutoData]
    public void GenericFunctionGetter(
        IFileSystem fileSystem,
        SkyrimMod mod,
        Npc npc,
        DirectoryPath existingDir,
        ModImporter sut)
    {
        var path = Path.Combine(existingDir, mod.ModKey.FileName);
        mod.WriteToBinary(path, new BinaryWriteParameters()
        {
            FileSystem = fileSystem
        });
        var import = sut.Import<ISkyrimModGetter>(path);
        import.Should().BeOfType<SkyrimModBinaryOverlay>();
    }
    
    [Theory, MutagenModAutoData]
    public void GenericFunctionSetter(
        IFileSystem fileSystem,
        SkyrimMod mod,
        Npc npc,
        DirectoryPath existingDir,
        ModImporter sut)
    {
        var path = Path.Combine(existingDir, mod.ModKey.FileName);
        mod.WriteToBinary(path, new BinaryWriteParameters()
        {
            FileSystem = fileSystem
        });
        var import = sut.Import<ISkyrimMod>(path);
        import.Should().BeOfType<SkyrimMod>();
    }
    
    [Theory, MutagenModAutoData]
    public void GenericFunctionDirect(
        IFileSystem fileSystem,
        SkyrimMod mod,
        Npc npc,
        DirectoryPath existingDir,
        ModImporter sut)
    {
        var path = Path.Combine(existingDir, mod.ModKey.FileName);
        mod.WriteToBinary(path, new BinaryWriteParameters()
        {
            FileSystem = fileSystem
        });
        var import = sut.Import<SkyrimMod>(path);
        import.Should().BeOfType<SkyrimMod>();
    }
    
    [Theory, MutagenModAutoData]
    public void UntypedImport(
        IFileSystem fileSystem,
        SkyrimMod mod,
        Npc npc,
        DirectoryPath existingDir,
        ModImporter sut)
    {
        var path = Path.Combine(existingDir, mod.ModKey.FileName);
        mod.WriteToBinary(path, new BinaryWriteParameters()
        {
            FileSystem = fileSystem
        });
        var import = sut.Import(path);
        import.Should().BeOfType<SkyrimModBinaryOverlay>();
    }
}