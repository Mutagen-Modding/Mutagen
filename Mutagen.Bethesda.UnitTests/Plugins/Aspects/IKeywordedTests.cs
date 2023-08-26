using System.Collections.Immutable;

using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Aspects;

public class IKeywordedTests
{
    #region Single Tests
    [Fact]
    public void HasKeyword_ByFormKey_Empty()
    {
        Npc npc = new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE);
        npc.HasKeyword(TestConstants.Form2).Should().BeFalse();
    }

    [Fact]
    public void HasKeyword_ByFormKey_NotFound()
    {
        Npc npc = new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE);
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(TestConstants.Form3);
        npc.HasKeyword(TestConstants.Form2).Should().BeFalse();
    }

    [Fact]
    public void HasKeyword_ByFormKey_Found()
    {
        Npc npc = new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE);
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(TestConstants.Form2);
        npc.HasKeyword(TestConstants.Form2).Should().BeTrue();
    }

    [Fact]
    public void HasKeyword_ByRecord_Empty()
    {
        Npc npc = new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE);
        Keyword keyword = new Keyword(TestConstants.Form4, SkyrimRelease.SkyrimSE);
        npc.HasKeyword(keyword).Should().BeFalse();
    }

    [Fact]
    public void HasKeyword_ByRecord_NotFound()
    {
        Npc npc = new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE);
        Keyword keyword = new Keyword(TestConstants.Form4, SkyrimRelease.SkyrimSE);
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(TestConstants.Form3);
        npc.HasKeyword(keyword).Should().BeFalse();
    }

    [Fact]
    public void HasKeyword_ByRecord_Found()
    {
        Npc npc = new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE);
        Keyword keyword = new Keyword(TestConstants.Form4, SkyrimRelease.SkyrimSE);
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(keyword);
        npc.HasKeyword(keyword).Should().BeTrue();
    }

    [Fact]
    public void HasKeyword_ByEditorID_Empty()
    {
        SkyrimMod mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var npc = mod.Npcs.AddNew();
        var cache = mod.ToImmutableLinkCache();
        npc.HasKeyword(TestConstants.Edid1, cache).Should().BeFalse();
    }

    [Fact]
    public void HasKeyword_ByEditorID_NotFound()
    {
        SkyrimMod mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var npc = mod.Npcs.AddNew();
        var cache = mod.ToImmutableLinkCache();
        Keyword keyword = mod.Keywords.AddNew();
        keyword.EditorID = TestConstants.Edid2;
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(keyword);
        npc.HasKeyword(TestConstants.Edid1, cache).Should().BeFalse();
    }

    [Fact]
    public void HasKeyword_ByEditorID_Found()
    {
        SkyrimMod mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var npc = mod.Npcs.AddNew();
        var cache = mod.ToImmutableLinkCache();
        Keyword keyword = mod.Keywords.AddNew();
        keyword.EditorID = TestConstants.Edid1;
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(keyword);
        npc.HasKeyword(TestConstants.Edid1, cache).Should().BeTrue();
    }

    [Fact]
    public void TryResolveKeyword_ByFormKey_Found()
    {
        SkyrimMod mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var npc = mod.Npcs.AddNew();
        var cache = mod.ToImmutableLinkCache();
        Keyword keyword = mod.Keywords.AddNew();
        keyword.EditorID = TestConstants.Edid1;
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(keyword);
        npc.TryResolveKeyword(keyword.FormKey, cache, out var kw).Should().BeTrue();
        kw.Should().Be(keyword);
    }

    [Fact]
    public void TryResolveKeyword_ByEditorID_Found()
    {
        SkyrimMod mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var npc = mod.Npcs.AddNew();
        var cache = mod.ToImmutableLinkCache();
        Keyword keyword = mod.Keywords.AddNew();
        keyword.EditorID = TestConstants.Edid1;
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(keyword);
        npc.TryResolveKeyword(TestConstants.Edid1, cache, out var kw).Should().BeTrue();
        kw.Should().Be(keyword);
    }
    #endregion
    
    #region List Tests
    [Fact]
    public void HasAnyKeyword_ByFormKeyList_Empty()
    {
        Npc npc = new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE);
        npc.HasAnyKeyword(ImmutableList.Create(TestConstants.Form2, TestConstants.Form3)).Should().BeFalse();
    }

    [Fact]
    public void HasAnyKeyword_ByFormKeyList_NotFound()
    {
        Npc npc = new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE);
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(TestConstants.Form3);
        npc.HasAnyKeyword(ImmutableList.Create(TestConstants.Form2, TestConstants.Form4)).Should().BeFalse();
    }

    [Fact]
    public void HasAnyKeyword_ByFormKeyList_Found()
    {
        Npc npc = new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE);
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(TestConstants.Form2);
        npc.HasAnyKeyword(ImmutableList.Create(TestConstants.Form3, TestConstants.Form2)).Should().BeTrue();
    }

    [Fact]
    public void HasAnyKeyword_ByRecordList_Empty()
    {
        Npc npc = new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE);
        Keyword keyword1 = new Keyword(TestConstants.Form3, SkyrimRelease.SkyrimSE);
        Keyword keyword2 = new Keyword(TestConstants.Form4, SkyrimRelease.SkyrimSE);
        npc.HasAnyKeyword(ImmutableList.Create(keyword1, keyword2)).Should().BeFalse();
    }

    [Fact]
    public void HasAnyKeyword_ByRecordList_NotFound()
    {
        Npc npc = new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE);
        Keyword keyword2 = new Keyword(TestConstants.Form2, SkyrimRelease.SkyrimSE);
        Keyword keyword4 = new Keyword(TestConstants.Form4, SkyrimRelease.SkyrimSE);
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(TestConstants.Form3);
        npc.HasAnyKeyword(ImmutableList.Create(keyword2, keyword4)).Should().BeFalse();
    }

    [Fact]
    public void HasAnyKeyword_ByRecordList_Found()
    {
        Npc npc = new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE);
        Keyword keyword3 = new Keyword(TestConstants.Form3, SkyrimRelease.SkyrimSE);
        Keyword keyword4 = new Keyword(TestConstants.Form4, SkyrimRelease.SkyrimSE);
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(keyword4);
        npc.HasAnyKeyword(ImmutableList.Create(keyword3, keyword4)).Should().BeTrue();
    }

    [Fact]
    public void HasAnyKeyword_ByEditorIDList_Empty()
    {
        SkyrimMod mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var npc = mod.Npcs.AddNew();
        var cache = mod.ToImmutableLinkCache();
        npc.HasAnyKeyword(ImmutableList.Create(TestConstants.Edid1), cache).Should().BeFalse();
    }

    [Fact]
    public void HasAnyKeyword_ByEditorIDList_NotFound()
    {
        SkyrimMod mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var npc = mod.Npcs.AddNew();
        var cache = mod.ToImmutableLinkCache();
        Keyword keyword = mod.Keywords.AddNew();
        keyword.EditorID = TestConstants.Edid2;
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(keyword);
        npc.HasAnyKeyword(ImmutableList.Create(TestConstants.Edid1), cache).Should().BeFalse();
    }

    [Fact]
    public void HasAnyKeyword_ByEditorIDList_Found()
    {
        SkyrimMod mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var npc = mod.Npcs.AddNew();
        var cache = mod.ToImmutableLinkCache();
        Keyword keyword = mod.Keywords.AddNew();
        keyword.EditorID = TestConstants.Edid1;
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(keyword);
        npc.HasAnyKeyword(ImmutableList.Create(TestConstants.Edid2, TestConstants.Edid1), cache).Should().BeTrue();
    }
    #endregion
}
