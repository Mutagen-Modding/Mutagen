using Shouldly;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class DuplicateInAsNewRecordTests
{
    [Fact]
    public void Typical()
    {
        const string Name = "TEST";

        var npc = new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE)
        {
            Name = Name
        };
        var mod = new SkyrimMod(TestConstants.PluginModKey4, SkyrimRelease.SkyrimSE);
        var npc2 = mod.Npcs.DuplicateInAsNewRecord(npc);
        npc2.FormKey.ModKey.ShouldBe(mod.ModKey);
        npc2.Name!.String.ShouldBe(Name);
    }

    [Fact]
    public void Abstract()
    {
        const int Data = 15;

        Global glob = new GlobalInt(TestConstants.Form1, SkyrimRelease.SkyrimSE)
        {
            Data = Data
        };
        var mod = new SkyrimMod(TestConstants.PluginModKey4, SkyrimRelease.SkyrimSE);
        var glob2 = (GlobalInt)mod.Globals.DuplicateInAsNewRecord(glob);
        glob2.FormKey.ModKey.ShouldBe(mod.ModKey);
        glob2.Data.ShouldBe(Data);
    }

    [Fact]
    public void AbstractPassDecendant()
    {
        const int Data = 15;

        var glob = new GlobalInt(TestConstants.Form1, SkyrimRelease.SkyrimSE)
        {
            Data = Data
        };
        var mod = new SkyrimMod(TestConstants.PluginModKey4, SkyrimRelease.SkyrimSE);
        var glob2 = (GlobalInt)mod.Globals.DuplicateInAsNewRecord<Global, IGlobalIntGetter, IGlobalGetter>(glob);
        glob2.FormKey.ModKey.ShouldBe(mod.ModKey);
        glob2.Data.ShouldBe(Data);
    }
}