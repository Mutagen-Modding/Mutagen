using System.IO.Abstractions;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class PlacedObjectLockTests
{
    [Theory, MutagenModAutoData(GameRelease.Fallout4)]
    public void LockEndsAfterItsFlagsExactlyWhenBroken(
        IFileSystem fileSystem,
        DirectoryPath existingDirectory,
        DirectoryPath existingRewriteDirectory,
        Fallout4Mod mod,
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

        using var overlay = Fallout4Mod.Create(Fallout4Release.Fallout4)
            .FromPath(path)
            .WithFileSystem(fileSystem)
            .Construct();
        ShouldHoldTheLocks(overlay, shortLock, fullLock);

        var rewritePath = new ModPath(mod.ModKey, Path.Combine(existingRewriteDirectory, mod.ModKey.FileName));
        overlay.BeginWrite
            .ToPath(rewritePath)
            .WithNoLoadOrder()
            .WithFileSystem(fileSystem)
            .Write();
        var rewritten = Fallout4Mod.Create(Fallout4Release.Fallout4)
            .FromPath(rewritePath)
            .WithFileSystem(fileSystem)
            .Mutable()
            .Construct();
        ShouldHoldTheLocks(rewritten, shortLock, fullLock);
    }

    private static void ShouldHoldTheLocks(IFallout4ModGetter mod, params IPlacedObjectGetter[] written)
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
