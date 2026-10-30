using System.IO.Abstractions;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class CellCombinedReferenceIndexTests
{
    [Theory, MutagenModAutoData(GameRelease.Fallout4)]
    public void CombinedReferenceIndexIsWrittenExactlyWhenPresent(
        IFileSystem fileSystem,
        DirectoryPath existingDirectory,
        DirectoryPath existingRewriteDirectory,
        Fallout4Mod mod,
        Cell emptyIndex,
        Cell noIndex)
    {
        emptyIndex.CombinedMeshes = new ExtendedList<uint>();
        emptyIndex.CombinedMeshReferences = new ExtendedList<CellCombinedMeshReference>();
        mod.Cells.Records.Add(new CellBlock
        {
            GroupType = GroupTypeEnum.InteriorCellBlock,
            SubBlocks =
            {
                new CellSubBlock
                {
                    GroupType = GroupTypeEnum.InteriorCellSubBlock,
                    Cells = { emptyIndex, noIndex },
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
        ShouldHoldTheIndexExactlyWhereSet(overlay, emptyIndex, noIndex);

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
        ShouldHoldTheIndexExactlyWhereSet(rewritten, emptyIndex, noIndex);
    }

    private static void ShouldHoldTheIndexExactlyWhereSet(
        IFallout4ModGetter mod,
        ICellGetter emptyIndex,
        ICellGetter noIndex)
    {
        var cells = mod.EnumerateMajorRecords<ICellGetter>().ToDictionary(x => x.FormKey);
        cells[emptyIndex.FormKey].CombinedMeshes.ShouldNotBeNull().ShouldBeEmpty();
        cells[emptyIndex.FormKey].CombinedMeshReferences.ShouldNotBeNull().ShouldBeEmpty();
        cells[noIndex.FormKey].CombinedMeshes.ShouldBeNull();
        cells[noIndex.FormKey].CombinedMeshReferences.ShouldBeNull();
    }
}
