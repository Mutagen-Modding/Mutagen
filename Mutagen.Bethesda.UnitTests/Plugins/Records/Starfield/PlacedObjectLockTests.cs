using System.IO.Abstractions;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Starfield;

public class PlacedObjectLockTests
{
    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void LockEndsAfterItsFlagsExactlyWhenBroken(
        IFileSystem fileSystem,
        DirectoryPath existingDirectory,
        DirectoryPath existingRewriteDirectory,
        StarfieldMod mod,
        Cell cell,
        PlacedObject shortLock,
        PlacedObject fullLock,
        LockLevel level)
    {
        shortLock.Lock = new LockData { Level = level, Versioning = LockData.VersioningBreaks.Break0 };
        fullLock.Lock = new LockData { Level = level };
        cell.Temporary.Add(shortLock);
        cell.Temporary.Add(fullLock);
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

        using var overlay = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(path)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Construct();
        ShouldHoldTheLocks(overlay, shortLock, fullLock);

        var rewritePath = new ModPath(mod.ModKey, Path.Combine(existingRewriteDirectory, mod.ModKey.FileName));
        overlay.BeginWrite
            .ToPath(rewritePath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();
        var rewritten = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(rewritePath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        ShouldHoldTheLocks(rewritten, shortLock, fullLock);
    }

    private static void ShouldHoldTheLocks(IStarfieldModGetter mod, params IPlacedObjectGetter[] written)
    {
        var placed = mod.EnumerateMajorRecords<IPlacedObjectGetter>().ToDictionary(x => x.FormKey);
        foreach (var original in written)
        {
            var reread = placed[original.FormKey].Lock.ShouldNotBeNull();
            reread.Level.ShouldBe(original.Lock!.Level);
            reread.Versioning.ShouldBe(original.Lock!.Versioning);
        }
    }
}
