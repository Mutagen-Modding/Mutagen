using System.IO.Abstractions;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

public class PlacedObjectRoomDataTests
{
    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void RoomDataIsWrittenExactlyWhenPresent(
        IFileSystem fileSystem,
        DirectoryPath existingDirectory,
        DirectoryPath existingRewriteDirectory,
        SkyrimMod mod,
        Cell cell,
        PlacedObject allZeroRoomData,
        PlacedObject noRoomData)
    {
        allZeroRoomData.Unknown = 0;
        cell.Temporary.Add(allZeroRoomData);
        cell.Temporary.Add(noRoomData);
        mod.Cells.Records.Add(new CellBlock
        {
            GroupType = GroupTypeEnum.InteriorCellBlock,
            SubBlocks =
            {
                new CellSubBlock
                {
                    GroupType = GroupTypeEnum.InteriorCellSubBlock,
                    Cells = { cell },
                },
            },
        });
        var path = new ModPath(mod.ModKey, Path.Combine(existingDirectory, mod.ModKey.FileName));
        mod.BeginWrite
            .ToPath(path)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();

        using var overlay = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
            .FromPath(path)
            .WithFileSystem(fileSystem)
            .Construct();
        ShouldHoldRoomDataExactlyWhereSet(overlay, allZeroRoomData, noRoomData);

        var rewritePath = new ModPath(mod.ModKey, Path.Combine(existingRewriteDirectory, mod.ModKey.FileName));
        overlay.BeginWrite
            .ToPath(rewritePath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();
        var rewritten = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
            .FromPath(rewritePath)
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        ShouldHoldRoomDataExactlyWhereSet(rewritten, allZeroRoomData, noRoomData);
    }

    private static void ShouldHoldRoomDataExactlyWhereSet(
        ISkyrimModGetter mod,
        IPlacedObjectGetter allZeroRoomData,
        IPlacedObjectGetter noRoomData)
    {
        var placed = mod.EnumerateMajorRecords<IPlacedObjectGetter>().ToDictionary(x => x.FormKey);
        placed[allZeroRoomData.FormKey].Unknown.ShouldBe((short)0);
        placed[noRoomData.FormKey].Unknown.ShouldBeNull();
    }
}
