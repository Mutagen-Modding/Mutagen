using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;
using Xunit;

#nullable disable
#pragma warning disable CS0618 // Type or member is obsolete

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking;

public partial class ALinkingTests
{
    LoadOrder<ISkyrimModGetter> TypicalLoadOrder()
    {
        return new LoadOrder<ISkyrimModGetter>()
        {
            new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE),
            new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
        };
    }

    [Fact]
    public void FormLink_LoadOrder_TryResolve_NoLink()
    {
        var package = TypicalLoadOrder().ToImmutableLinkCache();
        FormLink<INpc> formLink = new FormLink<INpc>(UnusedFormKey);
        Assert.False(formLink.TryResolve(package, out var _));
    }

    [Theory]
    [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
    [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
    public void FormLink_LoadOrder_TryResolve_Linked(LinkCachePreferences.RetentionType cacheType)
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var loadOrder = new LoadOrder<ISkyrimModGetter>()
        {
            mod,
            new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
        };
        var (style, package) = GetLinkCache(loadOrder, cacheType);
        FormLink<INpc> formLink = new FormLink<INpc>(npc.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.True(formLink.TryResolve(package, out var linkedRec));
            Assert.Same(npc, linkedRec);
        });
    }

    [Fact]
    public void FormLink_LoadOrder_TryResolve_DeepRecord_NoLink()
    {
        var package = TypicalLoadOrder().ToImmutableLinkCache();
        FormLink<IPlacedNpc> formLink = new FormLink<IPlacedNpc>(UnusedFormKey);
        Assert.False(formLink.TryResolve(package, out var _));
    }

    [Theory]
    [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
    [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
    public void FormLink_LoadOrder_TryResolve_DeepRecord_Linked(LinkCachePreferences.RetentionType cacheType)
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
        var loadOrder = new LoadOrder<ISkyrimModGetter>()
        {
            mod,
            new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
        };
        var (style, package) = GetLinkCache(loadOrder, cacheType);
        FormLink<IPlacedNpc> placedFormLink = new FormLink<IPlacedNpc>(placedNpc.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.True(placedFormLink.TryResolve(package, out var linkedPlacedNpc));
            Assert.Same(placedNpc, linkedPlacedNpc);
        });
        FormLink<ICell> cellFormLink = new FormLink<ICell>(cell.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.True(cellFormLink.TryResolve(package, out var linkedCell));
            Assert.Same(cell, linkedCell);
        });
        FormLink<IWorldspace> worldspaceFormLink = new FormLink<IWorldspace>(worldspace.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.True(worldspaceFormLink.TryResolve(package, out var linkedWorldspace));
            Assert.Same(worldspace, linkedWorldspace);
        });
    }

    [Fact]
    public void FormLink_LoadOrder_Resolve_NoLink()
    {
        var package = TypicalLoadOrder().ToImmutableLinkCache();
        FormLink<INpc> formLink = new FormLink<INpc>(UnusedFormKey);
        Assert.Null(formLink.TryResolve(package));
    }

    [Theory]
    [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
    [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
    public void FormLink_LoadOrder_Resolve_Linked(LinkCachePreferences.RetentionType cacheType)
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var loadOrder = new LoadOrder<ISkyrimModGetter>()
        {
            mod,
            new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
        };
        var (style, package) = GetLinkCache(loadOrder, cacheType);
        FormLink<INpc> formLink = new FormLink<INpc>(npc.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.Same(npc, formLink.TryResolve(package));
        });
    }

    [Fact]
    public void FormLink_LoadOrder_Resolve_DeepRecord_NoLink()
    {
        FormLink<IPlacedNpc> formLink = new FormLink<IPlacedNpc>(UnusedFormKey);
        var package = TypicalLoadOrder().ToImmutableLinkCache();
        Assert.Null(formLink.TryResolve(package));
    }

    [Theory]
    [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
    [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
    public void FormLink_LoadOrder_Resolve_DeepRecord_Linked(LinkCachePreferences.RetentionType cacheType)
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
        var loadOrder = new LoadOrder<ISkyrimModGetter>()
        {
            mod,
            new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
        };
        var (style, package) = GetLinkCache(loadOrder, cacheType);
        FormLink<IPlacedNpc> placedFormLink = new FormLink<IPlacedNpc>(placedNpc.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.Same(placedNpc, placedFormLink.TryResolve(package));
        });
        FormLink<ICell> cellFormLink = new FormLink<ICell>(cell.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.Same(cell, cellFormLink.TryResolve(package));
        });
        FormLink<IWorldspace> worldspaceFormLink = new FormLink<IWorldspace>(worldspace.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.Same(worldspace, worldspaceFormLink.TryResolve(package));
        });
    }

    [Theory]
    [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
    [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
    public void FormLink_LoadOrder_TryResolve_MarkerInterface(LinkCachePreferences.RetentionType cacheType)
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var spell = mod.Spells.AddNew();
        var loadOrder = new LoadOrder<ISkyrimModGetter>()
        {
            mod,
            new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
        };
        var (style, package) = GetLinkCache(loadOrder, cacheType);
        FormLink<IEffectRecord> formLink = new FormLink<IEffectRecord>(spell.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.True(formLink.TryResolve(package, out var linkedRec));
            Assert.Same(spell, linkedRec);
        });
    }

    [Fact]
    public void FormLink_LoadOrder_TryResolve_MarkerInterface_NoLink()
    {
        FormLink<IEffectRecord> formLink = new FormLink<IEffectRecord>(UnusedFormKey);
        var package = TypicalLoadOrder().ToImmutableLinkCache();
        Assert.False(formLink.TryResolve(package, out var _));
    }

    [Fact]
    public void FormLink_LoadOrder_TryResolve_MarkerInterface_DeepRecord_NoLink()
    {
        FormLink<IPlaced> formLink = new FormLink<IPlaced>(UnusedFormKey);
        var package = TypicalLoadOrder().ToImmutableLinkCache();
        Assert.False(formLink.TryResolve(package, out var _));
    }

    [Theory]
    [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
    [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
    public void FormLink_LoadOrder_TryResolve_MarkerInterface_DeepRecord_Linked(LinkCachePreferences.RetentionType cacheType)
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
        var loadOrder = new LoadOrder<ISkyrimModGetter>()
        {
            mod,
            new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
        };
        var (style, package) = GetLinkCache(loadOrder, cacheType);
        FormLink<IPlaced> placedFormLink = new FormLink<IPlaced>(placedNpc.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.True(placedFormLink.TryResolve(package, out var linkedPlacedNpc));
            Assert.Same(placedNpc, linkedPlacedNpc);
        });
        FormLink<ICell> cellFormLink = new FormLink<ICell>(cell.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.True(cellFormLink.TryResolve(package, out var linkedCell));
            Assert.Same(cell, linkedCell);
        });
        FormLink<IWorldspace> worldspaceFormLink = new FormLink<IWorldspace>(worldspace.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.True(worldspaceFormLink.TryResolve(package, out var linkedWorldspace));
            Assert.Same(worldspace, linkedWorldspace);
        });
    }

    [Theory]
    [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
    [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
    public void FormLink_LoadOrder_Resolve_MarkerInterface(LinkCachePreferences.RetentionType cacheType)
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var spell = mod.Spells.AddNew();
        var loadOrder = new LoadOrder<ISkyrimModGetter>()
        {
            mod,
            new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
        };
        var (style, package) = GetLinkCache(loadOrder, cacheType);
        FormLink<IEffectRecord> formLink = new FormLink<IEffectRecord>(spell.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.Same(spell, formLink.TryResolve(package));
        });
    }

    [Fact]
    public void FormLink_LoadOrder_Resolve_MarkerInterface_NoLink()
    {
        FormLink<IEffectRecord> formLink = new FormLink<IEffectRecord>(UnusedFormKey);
        var package = TypicalLoadOrder().ToImmutableLinkCache();
        Assert.Null(formLink.TryResolve(package));
    }

    [Fact]
    public void FormLink_LoadOrder_Resolve_MarkerInterface_DeepRecord_NoLink()
    {
        FormLink<IPlaced> formLink = new FormLink<IPlaced>(UnusedFormKey);
        var package = TypicalLoadOrder().ToImmutableLinkCache();
        Assert.Null(formLink.TryResolve(package));
    }

    [Theory]
    [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
    [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
    public void FormLink_LoadOrder_Resolve_MarkerInterface_DeepRecord_Linked(LinkCachePreferences.RetentionType cacheType)
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
        var loadOrder = new LoadOrder<ISkyrimModGetter>()
        {
            mod,
            new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
        };
        var (style, package) = GetLinkCache(loadOrder, cacheType);
        FormLink<IPlaced> placedFormLink = new FormLink<IPlaced>(placedNpc.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.Same(placedNpc, placedFormLink.Resolve(package));
        });
        FormLink<ICell> cellFormLink = new FormLink<ICell>(cell.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.Same(cell, cellFormLink.Resolve(package));
        });
        FormLink<IWorldspace> worldspaceFormLink = new FormLink<IWorldspace>(worldspace.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.Same(worldspace, worldspaceFormLink.Resolve(package));
        });
    }
}