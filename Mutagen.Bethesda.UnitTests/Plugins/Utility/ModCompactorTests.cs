using System.Reflection;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Utility;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;
using Npc = Mutagen.Bethesda.Skyrim.Npc;

namespace Mutagen.Bethesda.UnitTests.Plugins.Utility;

public class ModCompactorTests
{
    [Theory, MutagenModAutoData]
    public void AlreadySmallMasterCompatible(
        SkyrimMod mod,
        Npc n,
        Npc n2,
        ModCompactor modCompactor)
    {
        mod.IsSmallMaster.Should().BeFalse();
        modCompactor.CompactToSmallMaster(mod);
        mod.IsSmallMaster.Should().BeTrue();
        mod.Npcs.Records.Select(x => x.FormKey)
            .Should().Equal(n.FormKey, n2.FormKey);
    }
    
    [Theory, MutagenModAutoData]
    public void CompactRecordToSmallMaster(
        SkyrimMod mod,
        Npc n,
        ModCompactor modCompactor)
    {
        var n2 = mod.Npcs.AddReturn(
            new Npc(FormKey.Factory($"010000:{mod.ModKey.FileName}"), SkyrimRelease.SkyrimSE));
        var n3 = mod.Npcs.AddReturn(
            new Npc(FormKey.Factory($"000030:{mod.ModKey.FileName}"), SkyrimRelease.SkyrimSE));
        mod.IsSmallMaster.Should().BeFalse();
        modCompactor.CompactToSmallMaster(mod);
        mod.IsSmallMaster.Should().BeTrue();
        mod.Npcs.Records.Select(x => x.FormKey)
            .Should().Equal(n.FormKey, new FormKey(n.FormKey.ModKey, n.FormKey.ID + 1), n3.FormKey);
    }
    
    [Theory, MutagenModAutoData]
    public void FailToCompactRecordToSmallMaster(
        SkyrimMod mod,
        Npc n,
        ModCompactor modCompactor)
    {
        for (int i = 0; i < 4096; i++)
        {
            mod.Npcs.AddNew();
        }
        mod.IsSmallMaster.Should().BeFalse();
        Assert.Throws<TargetInvocationException>(() =>
        {
            modCompactor.CompactToSmallMaster(mod);
        });
    }
    
    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void AlreadyMediumMasterCompatible(
        ModKey modKey,
        ModCompactor modCompactor)
    {
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        var n = mod.Npcs.AddNew();
        var n2 = mod.Npcs.AddNew();
        mod.IsMediumMaster.Should().BeFalse();
        modCompactor.CompactToMediumMaster(mod);
        mod.IsMediumMaster.Should().BeTrue();
        mod.Npcs.Records.Select(x => x.FormKey)
            .Should().Equal(n.FormKey, n2.FormKey);
    }
    
    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void CompactRecordToMediumMaster(
        ModKey modKey,
        ModCompactor modCompactor)
    {
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        var n = mod.Npcs.AddNew();
        var n2 = mod.Npcs.AddReturn(
            new Starfield.Npc(FormKey.Factory($"080000:{mod.ModKey.FileName}"), StarfieldRelease.Starfield));
        var n3 = mod.Npcs.AddReturn(
            new Starfield.Npc(FormKey.Factory($"000030:{mod.ModKey.FileName}"), StarfieldRelease.Starfield));
        mod.IsMediumMaster.Should().BeFalse();
        modCompactor.CompactToMediumMaster(mod);
        mod.IsMediumMaster.Should().BeTrue();
        mod.Npcs.Records.Select(x => x.FormKey)
            .Should().Equal(n.FormKey, new FormKey(n.FormKey.ModKey, n.FormKey.ID + 1), n3.FormKey);
    }
    
    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void FailToCompactRecordToMediumMaster(
        ModKey modKey,
        ModCompactor modCompactor)
    {
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        var n = mod.Npcs.AddNew();
        for (int i = 0; i < 65536; i++)
        {
            mod.Npcs.AddNew();
        }
        mod.IsMediumMaster.Should().BeFalse();
        Assert.Throws<TargetInvocationException>(() =>
        {
            modCompactor.CompactToMediumMaster(mod);
        });
    }
}