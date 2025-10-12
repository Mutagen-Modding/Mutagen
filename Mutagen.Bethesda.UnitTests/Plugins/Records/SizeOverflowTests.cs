using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Noggog;
using Noggog.Testing.IO;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public abstract class SizeOverflowTests
{
    protected abstract IWorldspaceGetter Get(IFileSystem fileSystem, ModPath path);
        
    [Fact]
    public void WorldspaceOffsetOverflowReadTest()
    {
        var worldspace = Get(IFileSystemExt.DefaultFilesystem, TestDataPathing.SizeOverflow);
        worldspace.OffsetData.HasValue.ShouldBeTrue();
        worldspace.OffsetData!.Value.Length.ShouldBe(0x3B);
    }
        
    [Fact]
    public void WorldspaceMaxHeightOverflowReadTest()
    {
        var worldspace = Get(IFileSystemExt.DefaultFilesystem, TestDataPathing.SubObjectSizeOverflow);
        worldspace.MaxHeight.ShouldNotBeNull();
        worldspace.MaxHeight!.Min.ShouldBe(new P2Int16(-96, -96));
        worldspace.MaxHeight!.Max.ShouldBe(new P2Int16(96, 97));
        worldspace.MaxHeight!.CellData.Length.ShouldBe(0x33);
    }
        
    [Fact]
    public void WorldspaceOffsetOverflowWriteTest()
    {
        var path = $"{PathingUtil.DrivePrefix}WritePath.esp";
        var mockFileSystem = new MockFileSystem();
        var worldspace = new Worldspace(FormKey.Null, SkyrimRelease.SkyrimSE);
        worldspace.OffsetData = new byte[ushort.MaxValue + 1];
        using (var writer = new MutagenWriter(mockFileSystem.File.OpenWrite(path), GameConstants.SkyrimSE))
        {
            writer.MetaData.MasterReferences = new MasterReferenceCollection(ModKey.Null, []);
            writer.MetaData.SeparatedMasterPackage = SeparatedMasterPackage.NotSeparate(writer.MetaData.MasterReferences);
            worldspace.WriteToBinary(writer);
        }

        var worldspaceRead = Get(mockFileSystem, path);
        worldspaceRead.OffsetData.ShouldNotBeNull();
        worldspaceRead.OffsetData!.Value.Length.ShouldBe(ushort.MaxValue + 1);
    }
        
    [Fact]
    public void WorldspaceMaxHeightOverflowWriteTest()
    {
        var path = $"{PathingUtil.DrivePrefix}WritePath.esp";
        var mockFileSystem = new MockFileSystem();
        var worldspace = new Worldspace(FormKey.Null, SkyrimRelease.SkyrimSE);
        worldspace.MaxHeight = new WorldspaceMaxHeight()
        {
            CellData = new byte[ushort.MaxValue + 1]
        };
        using (var writer = new MutagenWriter(mockFileSystem.File.OpenWrite(path), GameConstants.SkyrimSE))
        {
            writer.MetaData.MasterReferences = new MasterReferenceCollection(ModKey.Null, []);
            writer.MetaData.SeparatedMasterPackage = SeparatedMasterPackage.NotSeparate(writer.MetaData.MasterReferences);
            worldspace.WriteToBinary(writer);
        }

        var worldspaceRead = Get(mockFileSystem, path);
        worldspaceRead.MaxHeight.ShouldNotBeNull();
        worldspaceRead.MaxHeight!.CellData.Length.ShouldBe(ushort.MaxValue + 1);
    }
}
    
public class SizeOverflowTestsDirect : SizeOverflowTests
{
    protected override IWorldspaceGetter Get(IFileSystem fileSystem, ModPath path)
    {
        var masters = SeparatedMasterPackage.NotSeparate(
            new MasterReferenceCollection(
                "Skyrim.esm"));
        using var reader = new MutagenBinaryReadStream(fileSystem.File.OpenRead(path),
            new ParsingMeta(GameConstants.SkyrimSE, 
                Constants.Skyrim,
                masters));
        return Worldspace.CreateFromBinary(new MutagenFrame(reader));
    }
}
    
public class SizeOverflowTestsOverlay : SizeOverflowTests
{
    protected override IWorldspaceGetter Get(IFileSystem fileSystem, ModPath path)
    {
        var bytes = fileSystem.File.ReadAllBytes(path);
        return WorldspaceBinaryOverlay.WorldspaceFactory(
            bytes,
            new BinaryOverlayFactoryPackage(
                new ParsingMeta(
                    GameConstants.SkyrimSE,
                    path.ModKey,
                    null!)));
    }
}