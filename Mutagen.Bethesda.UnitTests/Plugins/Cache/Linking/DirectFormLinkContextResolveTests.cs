﻿using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;
using Xunit;

#nullable disable
#pragma warning disable CS0618 // Type or member is obsolete

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking;

public partial class ALinkingTests
{
    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_TryResolveContext_NoLink(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var formLink = new FormLink<INpcGetter>(UnusedFormKey);
        var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.False(contextRetriever.TryResolveContext<INpc, INpcGetter>(formLink, package, out var _));
        });
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_TryResolveContext_Linked(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var (style, package) = GetLinkCache(mod, cacheType);
        var formLink = new FormLink<INpcGetter>(npc.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.True(contextRetriever.TryResolveContext<INpc, INpcGetter>(formLink, package, out var linkedRec));
            linkedRec.Record.ShouldBeSameAs(npc);
            linkedRec.ModKey.ShouldBe(TestConstants.PluginModKey);
            linkedRec.Parent.ShouldBeNull();
        });
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_TryResolveContext_DeepRecord_NoLink(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var formLink = new FormLink<IPlacedNpcGetter>(UnusedFormKey);
        var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.False(contextRetriever.TryResolveContext<IPlacedNpc, IPlacedNpcGetter>(formLink, package, out var _));
        });
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_TryResolveContext_DeepRecord_Linked(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
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
            Assert.True(contextRetriever.TryResolveContext<IPlacedNpc, IPlacedNpcGetter>(placedFormLink, package, out var linkedPlacedNpc));
            linkedPlacedNpc.Record.ShouldBeSameAs(placedNpc);
            linkedPlacedNpc.ModKey.ShouldBe(TestConstants.PluginModKey);
            linkedPlacedNpc.Parent.Record.ShouldBe(cell);
            var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
            Assert.True(contextRetriever.TryResolveContext<ICell, ICellGetter>(cellFormLink, package, out var linkedCell));
            linkedCell.Record.ShouldBeSameAs(cell);
            linkedCell.ModKey.ShouldBe(TestConstants.PluginModKey);
            linkedCell.Parent.Record.ShouldBe(subBlock);
            var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
            Assert.True(contextRetriever.TryResolveContext<IWorldspace, IWorldspaceGetter>(worldspaceFormLink, package, out var linkedWorldspace));
            linkedWorldspace.Record.ShouldBeSameAs(worldspace);
            linkedWorldspace.ModKey.ShouldBe(TestConstants.PluginModKey);
            linkedWorldspace.Parent.ShouldBeNull();
        });
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_ResolveContext_NoLink(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var formLink = new FormLink<INpcGetter>(UnusedFormKey);
        var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.Null(contextRetriever.ResolveContext<INpc, INpcGetter>(formLink, package));
        });
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_ResolveContext_Linked(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var (style, package) = GetLinkCache(mod, cacheType);
        var formLink = new FormLink<INpcGetter>(npc.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            var resolvedNpc = contextRetriever.ResolveContext<INpc, INpcGetter>(formLink, package);
            resolvedNpc.Record.ShouldBeSameAs(npc);
            resolvedNpc.ModKey.ShouldBe(TestConstants.PluginModKey);
            resolvedNpc.Parent.ShouldBeNull();
        });
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_ResolveContext_DeepRecord_NoLink(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var formLink = new FormLink<IPlacedNpcGetter>(UnusedFormKey);
        var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.Null(contextRetriever.ResolveContext<IPlacedNpc, IPlacedNpcGetter>(formLink, package));
        });
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_ResolveContext_DeepRecord_Linked(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
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
            var linkedPlacedNpc = contextRetriever.ResolveContext<IPlacedNpc, IPlacedNpcGetter>(placedFormLink, package);
            linkedPlacedNpc.Record.ShouldBeSameAs(placedNpc);
            linkedPlacedNpc.ModKey.ShouldBe(TestConstants.PluginModKey);
            linkedPlacedNpc.Parent.Record.ShouldBeSameAs(cell);
            var cellFormLink = new FormLink<ICell>(cell.FormKey);
            var linkedCell = contextRetriever.ResolveContext<ICell, ICellGetter>(cellFormLink, package);
            linkedCell.Record.ShouldBeSameAs(cell);
            linkedCell.ModKey.ShouldBe(TestConstants.PluginModKey);
            linkedCell.Parent.Record.ShouldBeSameAs(subBlock);
            var worldspaceFormLink = new FormLink<IWorldspace>(worldspace.FormKey);
            var linkedWorldspace = contextRetriever.ResolveContext<IWorldspace, IWorldspaceGetter>(worldspaceFormLink, package);
            linkedWorldspace.Record.ShouldBeSameAs(worldspace);
            linkedWorldspace.ModKey.ShouldBe(TestConstants.PluginModKey);
            linkedWorldspace.Parent.ShouldBeNull();
        });
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_TryResolveContext_MarkerInterface(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var spell = mod.Spells.AddNew();
        var (style, package) = GetLinkCache(mod, cacheType);
        var formLink = new FormLink<IEffectRecordGetter>(spell.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.True(contextRetriever.TryResolveContext<IEffectRecord, IEffectRecordGetter>(formLink, package, out var linkedRec));
            linkedRec.Record.ShouldBeSameAs(spell);
            linkedRec.ModKey.ShouldBe(TestConstants.PluginModKey);
            linkedRec.Parent.ShouldBeNull();
        });
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_TryResolveContext_MarkerInterface_NoLink(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
        var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.False(contextRetriever.TryResolveContext<IEffectRecord, IEffectRecordGetter>(formLink, package, out var _));
        });
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_TryResolveContext_MarkerInterface_DeepRecord_NoLink(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
        var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
        WrapPotentialThrow(cacheType, style, () =>
        {
            Assert.False(contextRetriever.TryResolveContext<IPlaced, IPlacedGetter>(formLink, package, out var _));
        });
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_TryResolveContext_MarkerInterface_DeepRecord_Linked(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
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
            Assert.True(contextRetriever.TryResolveContext<IPlaced, IPlacedGetter>(placedFormLink, package, out var linkedPlacedNpc));
            linkedPlacedNpc.Record.ShouldBeSameAs(placedNpc);
            linkedPlacedNpc.ModKey.ShouldBe(TestConstants.PluginModKey);
            linkedPlacedNpc.Parent.Record.ShouldBeSameAs(cell);
            var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
            Assert.True(contextRetriever.TryResolveContext<ICell, ICellGetter>(cellFormLink, package, out var linkedCell));
            linkedCell.Record.ShouldBeSameAs(cell);
            linkedCell.ModKey.ShouldBe(TestConstants.PluginModKey);
            linkedCell.Parent.Record.ShouldBeSameAs(subBlock);
            var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
            Assert.True(contextRetriever.TryResolveContext<IWorldspace, IWorldspaceGetter>(worldspaceFormLink, package, out var linkedWorldspace));
            linkedWorldspace.Record.ShouldBeSameAs(worldspace);
            linkedWorldspace.ModKey.ShouldBe(TestConstants.PluginModKey);
            linkedWorldspace.Parent.ShouldBeNull();
        });
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_ResolveContext_MarkerInterface(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var spell = mod.Spells.AddNew();
        var (style, package) = GetLinkCache(mod, cacheType);
        var formLink = new FormLink<IEffectRecordGetter>(spell.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            var resolved = contextRetriever.ResolveContext<IEffectRecord, IEffectRecordGetter>(formLink, package);
            resolved.Record.ShouldBeSameAs(spell);
            resolved.ModKey.ShouldBe(TestConstants.PluginModKey);
            resolved.Parent.ShouldBeNull();
        });
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_ResolveContext_MarkerInterface_NoLink(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
        var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
        WrapPotentialThrow(cacheType, style, () =>
        {
            var resolved = contextRetriever.ResolveContext<IEffectRecord, IEffectRecordGetter>(formLink, package);
            Assert.Null(resolved);
        });
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_ResolveContext_MarkerInterface_DeepRecord_NoLink(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
        var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
        WrapPotentialThrow(cacheType, style, () =>
        {
            var resolved = contextRetriever.ResolveContext<IPlaced, IPlacedGetter>(formLink, package);
            Assert.Null(resolved);
        });
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_ResolveContext_MarkerInterface_DeepRecord_Linked(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
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
            var resolvedPlaced = contextRetriever.ResolveContext<IPlaced, IPlacedGetter>(placedFormLink, package);
            resolvedPlaced.Record.ShouldBeSameAs(placedNpc);
            resolvedPlaced.ModKey.ShouldBe(TestConstants.PluginModKey);
            resolvedPlaced.Parent.Record.ShouldBeSameAs(cell);
            var cellFormLink = new FormLink<ICellGetter>(cell.FormKey);
            var resolvedCell = contextRetriever.ResolveContext<ICell, ICellGetter>(cellFormLink, package);
            resolvedCell.Record.ShouldBeSameAs(cell);
            resolvedCell.ModKey.ShouldBe(TestConstants.PluginModKey);
            resolvedCell.Parent.Record.ShouldBeSameAs(subBlock);
            var worldspaceFormLink = new FormLink<IWorldspaceGetter>(worldspace.FormKey);
            var resolvedWorldspace = contextRetriever.ResolveContext<IWorldspace, IWorldspaceGetter>(worldspaceFormLink, package);
            resolvedWorldspace.Record.ShouldBeSameAs(worldspace);
            resolvedWorldspace.ModKey.ShouldBe(TestConstants.PluginModKey);
            resolvedWorldspace.Parent.ShouldBeNull();
        });
    }
}