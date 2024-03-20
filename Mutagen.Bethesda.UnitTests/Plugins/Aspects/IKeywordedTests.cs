using System.Collections.Immutable;

using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.Testing.AutoData;

using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Aspects;

public class IKeywordedTests
{
    #region Single Tests
    [Theory, MutagenModAutoData]
    public void HasKeyword_ByFormKey_Empty(Npc npc, FormKey form)
    {
        npc.HasKeyword(form).Should().BeFalse();
    }

    [Theory, MutagenModAutoData]
    public void HasKeyword_ByFormKey_NotFound(Npc npc, FormKey form1, FormKey form2)
    {
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(form1);
        npc.HasKeyword(form2).Should().BeFalse();
    }

    [Theory, MutagenModAutoData]
    public void HasKeyword_ByFormKey_Found(Npc npc, FormKey form)
    {
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(form);
        npc.HasKeyword(form).Should().BeTrue();
    }

    [Theory, MutagenModAutoData]
    public void HasKeyword_ByRecord_Empty(Npc npc, Keyword keyword)
    {
        npc.HasKeyword(keyword).Should().BeFalse();
    }

    [Theory, MutagenModAutoData]
    public void HasKeyword_ByRecord_NotFound(Npc npc, Keyword keyword1, Keyword keyword2)
    {
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(keyword1);
        npc.HasKeyword(keyword2).Should().BeFalse();
    }

    [Theory, MutagenModAutoData]
    public void HasKeyword_ByRecord_Found(Npc npc, Keyword keyword)
    {
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(keyword);
        npc.HasKeyword(keyword).Should().BeTrue();
    }
    
    [Theory, MutagenModAutoData]
    public void HasKeyword_ByEditorID_Empty(SkyrimMod mod, Npc npc)
    {
        var cache = mod.ToImmutableLinkCache();
        npc.HasKeyword(TestConstants.Edid1, cache).Should().BeFalse();
    }

    [Theory, MutagenModAutoData]
    public void HasKeyword_ByEditorID_NotFound(SkyrimMod mod, Npc npc, Keyword keyword)
    {
        var cache = mod.ToImmutableLinkCache();
        keyword.EditorID = TestConstants.Edid2;
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(keyword);
        npc.HasKeyword(TestConstants.Edid1, cache).Should().BeFalse();
    }

    [Theory, MutagenModAutoData]
    public void HasKeyword_ByEditorID_Found(SkyrimMod mod, Npc npc, Keyword keyword)
    {
        var cache = mod.ToImmutableLinkCache();
        keyword.EditorID = TestConstants.Edid1;
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(keyword);
        npc.HasKeyword(keyword.EditorID, cache).Should().BeTrue();
    }

    [Theory, MutagenModAutoData]
    public void TryResolveKeyword_ByFormKey_Found(SkyrimMod mod, Npc npc, Keyword keyword)
    {
        var cache = mod.ToImmutableLinkCache();
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(keyword);
        npc.TryResolveKeyword(keyword.FormKey, cache, out var kw).Should().BeTrue();
        kw.Should().Be(keyword);
    }

    [Theory, MutagenModAutoData]
    public void TryResolveKeyword_ByEditorID_Found(SkyrimMod mod, Npc npc, Keyword keyword)
    {
        var cache = mod.ToImmutableLinkCache();
        keyword.EditorID = TestConstants.Edid1;
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(keyword);
        npc.TryResolveKeyword(keyword.EditorID, cache, out var kw).Should().BeTrue();
        kw.Should().Be(keyword);
    }
    #endregion
    
    #region List Tests
    [Theory, MutagenModAutoData]
    public void HasAnyKeyword_ByFormKeyList_Empty(Npc npc, FormKey form1, FormKey form2)
    {
        npc.HasAnyKeyword(ImmutableList.Create(form1, form2)).Should().BeFalse();
    }

    [Theory, MutagenModAutoData]
    public void HasAnyKeyword_ByFormKeyList_NotFound(Npc npc, Keyword keyword, FormKey form1, FormKey form2)
    {
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(keyword.ToLink());
        npc.HasAnyKeyword(ImmutableList.Create(form1, form2)).Should().BeFalse();
    }

    [Theory, MutagenModAutoData]
    public void HasAnyKeyword_ByFormKeyList_Found(Npc npc, Keyword keyword, FormKey form)
    {
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(keyword.ToLink());
        npc.HasAnyKeyword(ImmutableList.Create(form, keyword.ToLink().FormKey)).Should().BeTrue();
    }

    [Theory, MutagenModAutoData]
    public void HasAnyKeyword_ByRecordList_Empty(Npc npc, Keyword keyword1, Keyword keyword2)
    {
        npc.HasAnyKeyword(ImmutableList.Create(keyword1, keyword2)).Should().BeFalse();
    }

    [Theory, MutagenModAutoData]
    public void HasAnyKeyword_ByRecordList_NotFound(Npc npc, Keyword keyword1, Keyword keyword2, Keyword keyword3)
    {
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(keyword1);
        npc.HasAnyKeyword(ImmutableList.Create(keyword2, keyword3)).Should().BeFalse();
    }

    [Theory, MutagenModAutoData]
    public void HasAnyKeyword_ByRecordList_Found(Npc npc, Keyword keyword1, Keyword keyword2)
    {
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(keyword2);
        npc.HasAnyKeyword(ImmutableList.Create(keyword1, keyword2)).Should().BeTrue();
    }

    [Theory, MutagenModAutoData]
    public void HasAnyKeyword_ByEditorIDList_Empty(SkyrimMod mod, Npc npc)
    {
        var cache = mod.ToImmutableLinkCache();
        npc.HasAnyKeyword(ImmutableList.Create(TestConstants.Edid1), cache).Should().BeFalse();
    }

    [Theory, MutagenModAutoData]
    public void HasAnyKeyword_ByEditorIDList_NotFound(SkyrimMod mod, Npc npc, Keyword keyword)
    {
        var cache = mod.ToImmutableLinkCache();
        keyword.EditorID = TestConstants.Edid2;
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(keyword);
        npc.HasAnyKeyword(ImmutableList.Create(TestConstants.Edid1), cache).Should().BeFalse();
    }

    [Theory, MutagenModAutoData]
    public void HasAnyKeyword_ByEditorIDList_Found(SkyrimMod mod, Npc npc, Keyword keyword)
    {
        var cache = mod.ToImmutableLinkCache();
        keyword.EditorID = TestConstants.Edid1;
        npc.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
        npc.Keywords.Add(keyword);
        npc.HasAnyKeyword(ImmutableList.Create(TestConstants.Edid2, keyword.EditorID), cache).Should().BeTrue();
    }
    #endregion
}
