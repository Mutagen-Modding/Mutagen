using FluentAssertions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Oblivion.Records;

public class EnumerateContextsTests
{
    [Theory]
    [MutagenModAutoData(GameRelease.Oblivion)]
    public void CellEnumeration(
        OblivionMod mod,
        Cell cell1,
        Cell cell2,
        Cell cell3,
        Worldspace worldspace)
    {
        mod.Cells
            .AddReturn(new CellBlock())
            .SubBlocks
            .AddReturn(new CellSubBlock())
            .Cells.Add(cell1);
        worldspace.TopCell = cell2;
        worldspace.SubCells
            .AddReturn(new WorldspaceBlock())
            .Items
            .AddReturn(new WorldspaceSubBlock())
            .Items
            .Add(cell3);
        var linkCache = mod.ToImmutableLinkCache();
        mod.EnumerateMajorRecordContexts()
            .Select(x => x.Record.FormKey)
            .Should().Equal(cell1.FormKey, worldspace.FormKey, cell2.FormKey, cell3.FormKey);
        mod.EnumerateMajorRecordContexts<IMajorRecord, IMajorRecordGetter>(linkCache)
            .Select(x => x.Record.FormKey)
            .Should().Equal(cell1.FormKey, worldspace.FormKey, cell2.FormKey, cell3.FormKey);
        mod.EnumerateMajorRecordContexts(linkCache, typeof(Cell))
            .Select(x => x.Record.FormKey)
            .Should().Equal(cell1.FormKey, cell2.FormKey, cell3.FormKey);
        mod.EnumerateMajorRecordContexts<ICell, ICellGetter>(linkCache)
            .Select(x => x.Record.FormKey)
            .Should().Equal(cell1.FormKey, cell2.FormKey, cell3.FormKey);
    }
}