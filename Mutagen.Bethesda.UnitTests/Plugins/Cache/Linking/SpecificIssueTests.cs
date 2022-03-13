using System.Linq;
using FluentAssertions;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;
using Noggog;
using Xunit;

#nullable disable
#pragma warning disable CS0618 // Type or member is obsolete

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking
{
    public partial class ALinkingTests
    {
        [Theory]
        [MemberData(nameof(ContextTestSources))]
        public void PlacedInCellQuerySucceedsIfMajorRecordType(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
        {
            var prototype = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
            var placed = new PlacedObject(prototype);
            prototype.Cells.Records.Add(new CellBlock()
            {
                SubBlocks = new ExtendedList<CellSubBlock>()
                {
                    new CellSubBlock()
                    {
                        Cells = new ExtendedList<Cell>()
                        {
                            new Cell(prototype)
                            {
                                Temporary = new ExtendedList<IPlaced>()
                                {
                                    placed
                                }
                            }
                        }
                    }
                }
            });
            using var disp = ConvertMod(prototype, out var mod);
            var (style, package) = GetLinkCache(mod, cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                contextRetriever.TryResolveContext<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(placed.AsLink(), package, out var rec)
                .Should().BeTrue();
                rec.Record.Should().Be(placed);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                package.TryResolve<ISkyrimMajorRecordGetter>(placed.FormKey, out var rec2)
                .Should().BeTrue();
                rec2.Should().Be(placed);
            });
        }

        [Theory]
        [MemberData(nameof(ContextTestSources))]
        public void PlacedInWorldspaceQuerySucceedsIfMajorRecordType(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
        {
            var prototype = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
            var placed = new PlacedObject(prototype);
            prototype.Worldspaces.Add(new Worldspace(prototype)
            {
                SubCells = new ExtendedList<WorldspaceBlock>()
                {
                    new WorldspaceBlock()
                    {
                        Items = new ExtendedList<WorldspaceSubBlock>()
                        {
                            new WorldspaceSubBlock()
                            {
                                Items = new ExtendedList<Cell>()
                                {
                                    new Cell(prototype)
                                    {
                                        Temporary = new ExtendedList<IPlaced>()
                                        {
                                            placed
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });
            using var disp = ConvertMod(prototype, out var mod);
            var (style, package) = GetLinkCache(mod, cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                package.TryResolve<ISkyrimMajorRecordGetter>(placed.FormKey, out var rec2)
                    .Should().BeTrue();
                rec2.Should().Be(placed);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                contextRetriever.TryResolveContext<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(placed.AsLink(), package, out var rec)
                    .Should().BeTrue();
                rec.Record.Should().Be(placed);
            });
        }

        [Theory]
        [MemberData(nameof(ContextTestSources))]
        public void LocationTargetableInWorldspaceQuerySucceeds(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
        {
            var prototype = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
            var placed = new PlacedNpc(prototype);
            prototype.Worldspaces.Add(new Worldspace(prototype)
            {
                TopCell = 
                    new Cell(prototype)
                    {
                        Temporary = new ExtendedList<IPlaced>()
                        {
                            placed
                        }
                    }
            });
            using var disp = ConvertMod(prototype, out var mod);
            var (style, package) = GetLinkCache(mod, cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                package.TryResolve<ILocationTargetableGetter>(placed.FormKey, out var rec2)
                    .Should().BeTrue();
                rec2.Should().Be(placed);
            });
            WrapPotentialThrow(cacheType, style, () =>
            {
                contextRetriever.TryResolveContext<ILocationTargetable, ILocationTargetableGetter>(placed.AsLink(), package, out var rec)
                    .Should().BeTrue();
                rec.Record.Should().Be(placed);
            });
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void PlacedInWorldspaceOnlyOverridesPlaced(LinkCachePreferences.RetentionType cacheType)
        {
            var prototype = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
            var placed = new PlacedObject(prototype);
            var outgoing = new SkyrimMod(TestConstants.PluginModKey4, SkyrimRelease.SkyrimSE);
            prototype.Worldspaces.Add(new Worldspace(prototype)
            {
                SubCells = new ExtendedList<WorldspaceBlock>()
                {
                    new WorldspaceBlock()
                    {
                        Items = new ExtendedList<WorldspaceSubBlock>()
                        {
                            new WorldspaceSubBlock()
                            {
                                Items = new ExtendedList<Cell>()
                                {
                                    new Cell(prototype)
                                    {
                                        Landscape = new Landscape(prototype),
                                        Temporary = new ExtendedList<IPlaced>()
                                        {
                                            placed
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });
            using var disp = ConvertMod(prototype, out var mod);
            var (style, package) = GetLinkCache(mod, cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                package.TryResolveContext<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(placed.FormKey, out var rec)
                    .Should().BeTrue();
                rec.Record.Should().Be(placed);
                rec.GetOrAddAsOverride(outgoing);
                outgoing.Worldspaces.First().SubCells.First().Items.First().Items.First().Landscape.Should().BeNull();
            });
        }
    }
}