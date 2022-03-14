using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
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
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void FormLink_Direct_TryResolve_NoLink(LinkCachePreferences.RetentionType cacheType)
        {
            var formLink = new FormLink<INpcGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void FormLink_Direct_TryResolve_Linked(LinkCachePreferences.RetentionType cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var (style, package) = GetLinkCache(mod, cacheType);
            FormLink<INpc> formLink = new FormLink<INpc>(npc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(formLink.TryResolve(package, out var linkedRec));
                Assert.Same(npc, linkedRec);
            });
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void FormLink_Direct_TryResolve_DeepRecord_NoLink(LinkCachePreferences.RetentionType cacheType)
        {
            var formLink = new FormLink<IPlacedNpcGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void FormLink_Direct_TryResolve_DeepRecord_Linked(LinkCachePreferences.RetentionType cacheType)
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
                Assert.True(placedFormLink.TryResolve(package, out var linkedPlacedNpc));
                Assert.Same(placedNpc, linkedPlacedNpc);
            });
            var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(cellFormLink.TryResolve(package, out var linkedCell));
                Assert.Same(cell, linkedCell);
            });
            var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(worldspaceFormLink.TryResolve(package, out var linkedWorldspace));
                Assert.Same(worldspace, linkedWorldspace);
            });
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void FormLink_Direct_Resolve_NoLink(LinkCachePreferences.RetentionType cacheType)
        {
            var formLink = new FormLink<INpcGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            Assert.Null(formLink.TryResolve(package));
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void FormLink_Direct_Resolve_Linked(LinkCachePreferences.RetentionType cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var (style, package) = GetLinkCache(mod, cacheType);
            FormLink<INpc> formLink = new FormLink<INpc>(npc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(npc, formLink.TryResolve(package));
            });
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void FormLink_Direct_Resolve_DeepRecord_NoLink(LinkCachePreferences.RetentionType cacheType)
        {
            var formLink = new FormLink<IPlacedNpcGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            Assert.Null(formLink.TryResolve(package));
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void FormLink_Direct_Resolve_DeepRecord_Linked(LinkCachePreferences.RetentionType cacheType)
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
                Assert.Same(placedNpc, placedFormLink.TryResolve(package));
            });
            var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(cell, cellFormLink.TryResolve(package));
            });
            var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(worldspace, worldspaceFormLink.TryResolve(package));
            });
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void FormLink_Direct_TryResolve_MarkerInterface(LinkCachePreferences.RetentionType cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var spell = mod.Spells.AddNew();
            var (style, package) = GetLinkCache(mod, cacheType);
            var formLink = new FormLink<IEffectRecordGetter>(spell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(formLink.TryResolve(package, out var linkedRec));
                Assert.Same(spell, linkedRec);
            });
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void FormLink_Direct_TryResolve_MarkerInterface_NoLink(LinkCachePreferences.RetentionType cacheType)
        {
            var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void FormLink_Direct_TryResolve_MarkerInterface_DeepRecord_NoLink(LinkCachePreferences.RetentionType cacheType)
        {
            var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            Assert.False(formLink.TryResolve(package, out var _));
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void FormLink_Direct_TryResolve_MarkerInterface_DeepRecord_Linked(LinkCachePreferences.RetentionType cacheType)
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
                Assert.True(placedFormLink.TryResolve(package, out var linkedPlacedNpc));
                Assert.Same(placedNpc, linkedPlacedNpc);
            });
            var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(cellFormLink.TryResolve(package, out var linkedCell));
                Assert.Same(cell, linkedCell);
            });
            var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.True(worldspaceFormLink.TryResolve(package, out var linkedWorldspace));
                Assert.Same(worldspace, linkedWorldspace);
            });
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void FormLink_Direct_Resolve_MarkerInterface(LinkCachePreferences.RetentionType cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var spell = mod.Spells.AddNew();
            var (style, package) = GetLinkCache(mod, cacheType);
            var formLink = new FormLink<IEffectRecordGetter>(spell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(spell, formLink.TryResolve(package));
            });
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void FormLink_Direct_Resolve_MarkerInterface_NoLink(LinkCachePreferences.RetentionType cacheType)
        {
            var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            Assert.Null(formLink.TryResolve(package));
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void FormLink_Direct_Resolve_MarkerInterface_DeepRecord_NoLink(LinkCachePreferences.RetentionType cacheType)
        {
            var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            Assert.Null(formLink.TryResolve(package));
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void FormLink_Direct_Resolve_MarkerInterface_DeepRecord_Linked(LinkCachePreferences.RetentionType cacheType)
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
                Assert.Same(placedNpc, placedFormLink.TryResolve(package));
            });
            var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(cell, cellFormLink.TryResolve(package));
            });
            var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                Assert.Same(worldspace, worldspaceFormLink.TryResolve(package));
            });
        }
    }
}