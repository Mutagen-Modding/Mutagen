using System.Linq;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class ModContainedFormLinksTests
{
    [Fact]
    public void IteratesContainedFormLinks()
    {
        var mod = new SkyrimMod(ModKey.FromNameAndExtension("Test.esp"), SkyrimRelease.SkyrimSE);
        var npc = mod.Npcs.AddNew();
        var key = FormKey.Factory("123456:Skyrim.esm");
        npc.Class.SetTo(key);
        mod.ContainedFormLinks.Select(x => x.FormKey).Should().Contain(key);
    }
}