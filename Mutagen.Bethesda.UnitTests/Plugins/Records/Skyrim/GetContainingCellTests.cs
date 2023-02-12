using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Records.Tooling;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

public class GetContainingCellTests
{
    [Fact]
    public void TestTamriel()
    {
        var mod = new SkyrimMod(new ModKey("Test", ModType.Master), SkyrimRelease.SkyrimSE);
        var worldspace = mod.Worldspaces.AddNew();
        worldspace.SubCells.Add(new WorldspaceBlock
        {
            GroupType = GroupTypeEnum.ExteriorCellBlock,
            Items = new ExtendedList<WorldspaceSubBlock>
            {
                new()
                {
                    BlockNumberX = 0,
                    BlockNumberY = 0,
                    GroupType = GroupTypeEnum.ExteriorCellSubBlock,
                    Items = new ExtendedList<Cell>
                    {
                        // MossMotherCavernExterior01
                        // https://modmapper.com/?cell=-14%2C-13
                        // -14,-13
                        new(mod, "MossMotherCavernExterior01")
                        {
                            Grid = new CellGrid
                            {
                                Point = new P2Int(-14, -13)
                            },
                            // These are temporary placed references and they are used to verify that temporary objects correcty use the parent cell regardless of placement
                            Temporary = new ExtendedList<IPlaced>
                            {
                                new PlacedNpc(mod, "temp_MossMotherCavernExterior01"),
                                new PlacedObject(mod, "temp_MossMotherCavernExterior01")
                            }
                        },
                        // DawnstarExterior04
                        // https://modmapper.com/?cell=7%2C26
                        // 7,26
                        new(mod, "DawnstarExterior04")
                        {
                            Grid = new CellGrid
                            {
                                Point = new P2Int(7, 26)
                            }
                        }
                    }
                }
            }
        });
        worldspace.TopCell = new Cell(mod, "persistentCell")
        {
            Persistent = new ExtendedList<IPlaced>
            {
                // Valdr
                // https://en.uesp.net/wiki/Skyrim:Valdr
                new PlacedNpc(mod, "411B9_MossMotherCavernExterior01")
                {
                    Placement = new Placement
                    {
                        Position = new P3Float(-56662.046875f, -49624.226562f, 1308.730225f)
                    },
                    MajorFlags = PlacedNpc.MajorFlag.Persistent
                },
                // Captain Wayfinder
                // https://en.uesp.net/wiki/Skyrim:Captain_Wayfinder
                new PlacedNpc(mod, "A17AB_DawnstarExterior04")
                {
                    Placement = new Placement
                    {
                        Position = new P3Float(29279.250000f, 106758.640625f, -13949.279297f)
                    },
                    MajorFlags = PlacedNpc.MajorFlag.Persistent
                },
                // Ralof
                // https://en.uesp.net/wiki/Skyrim:Ralof
                // Helgen was not added as a cell, so his location cannot be found
                new PlacedNpc(mod, "2BF9E_NULL")
                {
                    Placement = new Placement
                    {
                        Position = new P3Float(19840.773438f, -79532.500000f, 8450.620117f)
                    },
                    MajorFlags = PlacedNpc.MajorFlag.Persistent
                }
            },
            MajorFlags = Cell.MajorFlag.Persistent
        };
        LoadOrder<ModListing<ISkyrimModGetter>> loadOrder = new()
        {
            new ModListing<ISkyrimModGetter>(mod)
        };
        var linkCache = loadOrder.ToMutableLinkCache();
        var allCellContexts = loadOrder.PriorityOrder.Cell().WinningContextOverrides(linkCache);
        var allPlacedContexts = loadOrder.PriorityOrder.PlacedNpc().WinningContextOverrides(linkCache);
        WorldspaceCellLocationCache worldspaceCellLocationCache = new(allCellContexts);
        foreach (var placedContext in allPlacedContexts)
        {
            var editorID = placedContext.Record.EditorID!;
            var expectedCellEditorID = editorID.Split("_")[1];
            if (placedContext.TryGetContainingCell(worldspaceCellLocationCache, out var containingCell))
                expectedCellEditorID.Should().Be(containingCell.Record.EditorID);
            else
                expectedCellEditorID.Should().Be("NULL");
        }
    }
}