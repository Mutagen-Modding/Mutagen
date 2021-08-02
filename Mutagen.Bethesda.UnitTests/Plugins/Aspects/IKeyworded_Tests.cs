using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Aspects
{
    public class IKeyworded_Tests
    {
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
    }
}
