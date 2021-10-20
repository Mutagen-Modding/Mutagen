using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;
using Xunit;

#nullable disable
#pragma warning disable CS0618 // Type or member is obsolete

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking
{
    public partial class ALinkingTests
    {
        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolveContext_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<INpcGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(formLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(package, out var _));
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolveContext_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var (style, package) = GetLinkCache(mod, cacheType);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(formLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(package, out var linkedRec));
                linkedRec.Record.Should().BeSameAs(npc);
                linkedRec.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedRec.Parent.Should().BeNull();
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolveContext_DeepRecord_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IPlacedNpcGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.False(formLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter>(package, out var _));
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolveContext_DeepRecord_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var (style, package) = GetLinkCache(mod, cacheType);
            var placedFormLink = new FormLink<IPlacedNpcGetter>(placedNpc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(placedFormLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter>(package, out var linkedPlacedNpc));
                linkedPlacedNpc.Record.Should().BeSameAs(placedNpc);
                linkedPlacedNpc.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedPlacedNpc.Parent.Record.Should().Be(cell);
                var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
                Assert.True(cellFormLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(package, out var linkedCell));
                linkedCell.Record.Should().BeSameAs(cell);
                linkedCell.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedCell.Parent.Record.Should().Be(subBlock);
                var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
                Assert.True(worldspaceFormLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter>(package, out var linkedWorldspace));
                linkedWorldspace.Record.Should().BeSameAs(worldspace);
                linkedWorldspace.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedWorldspace.Parent.Should().BeNull();
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveContext_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<INpcGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Null(formLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(package));
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveContext_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var (style, package) = GetLinkCache(mod, cacheType);
            var formLink = new FormLink<INpcGetter>(npc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var resolvedNpc = formLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(package);
                resolvedNpc.Record.Should().BeSameAs(npc);
                resolvedNpc.ModKey.Should().Be(TestConstants.PluginModKey);
                resolvedNpc.Parent.Should().BeNull();
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveContext_DeepRecord_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IPlacedNpcGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Null(formLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter>(package));
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveContext_DeepRecord_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var (style, package) = GetLinkCache(mod, cacheType);
            var placedFormLink = new FormLink<IPlacedNpc>(placedNpc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var linkedPlacedNpc = placedFormLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter>(package);
                linkedPlacedNpc.Record.Should().BeSameAs(placedNpc);
                linkedPlacedNpc.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedPlacedNpc.Parent.Record.Should().BeSameAs(cell);
                var cellFormLink = new FormLink<ICell>(cell.FormKey);
                var linkedCell = cellFormLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(package);
                linkedCell.Record.Should().BeSameAs(cell);
                linkedCell.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedCell.Parent.Record.Should().BeSameAs(subBlock);
                var worldspaceFormLink = new FormLink<IWorldspace>(worldspace.FormKey);
                var linkedWorldspace = worldspaceFormLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter>(package);
                linkedWorldspace.Record.Should().BeSameAs(worldspace);
                linkedWorldspace.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedWorldspace.Parent.Should().BeNull();
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolveContext_MarkerInterface(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var spell = mod.Spells.AddNew();
            var (style, package) = GetLinkCache(mod, cacheType);
            var formLink = new FormLink<IEffectRecordGetter>(spell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(formLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, IEffectRecord, IEffectRecordGetter>(package, out var linkedRec));
                linkedRec.Record.Should().BeSameAs(spell);
                linkedRec.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedRec.Parent.Should().BeNull();
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolveContext_MarkerInterface_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolveContext_MarkerInterface_DeepRecord_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_TryResolveContext_MarkerInterface_DeepRecord_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var (style, package) = GetLinkCache(mod, cacheType);
            var placedFormLink = new FormLink<IPlacedGetter>(placedNpc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(placedFormLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, IPlaced, IPlacedGetter>(package, out var linkedPlacedNpc));
                linkedPlacedNpc.Record.Should().BeSameAs(placedNpc);
                linkedPlacedNpc.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedPlacedNpc.Parent.Record.Should().BeSameAs(cell);
                var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
                Assert.True(cellFormLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(package, out var linkedCell));
                linkedCell.Record.Should().BeSameAs(cell);
                linkedCell.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedCell.Parent.Record.Should().BeSameAs(subBlock);
                var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
                Assert.True(worldspaceFormLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter>(package, out var linkedWorldspace));
                linkedWorldspace.Record.Should().BeSameAs(worldspace);
                linkedWorldspace.ModKey.Should().Be(TestConstants.PluginModKey);
                linkedWorldspace.Parent.Should().BeNull();
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveContext_MarkerInterface(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var spell = mod.Spells.AddNew();
            var (style, package) = GetLinkCache(mod, cacheType);
            var formLink = new FormLink<IEffectRecordGetter>(spell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var resolved = formLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, IEffectRecord, IEffectRecordGetter>(package);
                resolved.Record.Should().BeSameAs(spell);
                resolved.ModKey.Should().Be(TestConstants.PluginModKey);
                resolved.Parent.Should().BeNull();
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveContext_MarkerInterface_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var resolved = formLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, IEffectRecord, IEffectRecordGetter>(package);
                Assert.Null(resolved);
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveContext_MarkerInterface_DeepRecord_NoLink(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var resolved = formLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, IPlaced, IPlacedGetter>(package);
                Assert.Null(resolved);
            });
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveContext_MarkerInterface_DeepRecord_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var worldspace = mod.Worldspaces.AddNew();
            var subBlock = new WorldspaceSubBlock();
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            subBlock.Items.Add(cell);
            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimLE);
            cell.Temporary.Add(placedNpc);
            var block = new WorldspaceBlock();
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var (style, package) = GetLinkCache(mod, cacheType);
            var placedFormLink = new FormLink<IPlacedGetter>(placedNpc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var resolvedPlaced = placedFormLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, IPlaced, IPlacedGetter>(package);
                resolvedPlaced.Record.Should().BeSameAs(placedNpc);
                resolvedPlaced.ModKey.Should().Be(TestConstants.PluginModKey);
                resolvedPlaced.Parent.Record.Should().BeSameAs(cell);
                var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
                var resolvedCell = cellFormLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(package);
                resolvedCell.Record.Should().BeSameAs(cell);
                resolvedCell.ModKey.Should().Be(TestConstants.PluginModKey);
                resolvedCell.Parent.Record.Should().BeSameAs(subBlock);
                var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
                var resolvedWorldspace = worldspaceFormLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, IWorldspace, IWorldspaceGetter>(package);
                resolvedWorldspace.Record.Should().BeSameAs(worldspace);
                resolvedWorldspace.ModKey.Should().Be(TestConstants.PluginModKey);
                resolvedWorldspace.Parent.Should().BeNull();
            });
        }
    }
}