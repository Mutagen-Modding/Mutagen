﻿using System.Reflection;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Utility;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;
using SkyrimNpc = Mutagen.Bethesda.Skyrim.Npc;
using StarfieldNpc = Mutagen.Bethesda.Starfield.Npc;

namespace Mutagen.Bethesda.UnitTests.Plugins.Utility;

public class ModCompactorTests
{
    [Theory, MutagenModAutoData]
    public void AlreadySmallMasterCompatible(
        SkyrimMod mod,
        SkyrimNpc n,
        SkyrimNpc n2,
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
        SkyrimNpc n,
        ModCompactor modCompactor)
    {
        var n2 = mod.Npcs.AddReturn(
            new SkyrimNpc(FormKey.Factory($"010000:{mod.ModKey.FileName}"), SkyrimRelease.SkyrimSE));
        var n3 = mod.Npcs.AddReturn(
            new SkyrimNpc(FormKey.Factory($"000030:{mod.ModKey.FileName}"), SkyrimRelease.SkyrimSE));
        mod.IsSmallMaster.Should().BeFalse();
        modCompactor.CompactToSmallMaster(mod);
        mod.IsSmallMaster.Should().BeTrue();
        mod.Npcs.Records.Select(x => x.FormKey)
            .Should().Equal(n.FormKey, new FormKey(n.FormKey.ModKey, n.FormKey.ID + 1), n3.FormKey);
    }
    
    [Theory, MutagenModAutoData]
    public void FailToCompactRecordToSmallMaster(
        SkyrimMod mod,
        SkyrimNpc n,
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
        StarfieldMod mod,
        StarfieldNpc n,
        StarfieldNpc n2,
        ModCompactor modCompactor)
    {
        mod.IsMediumMaster.Should().BeFalse();
        modCompactor.CompactToMediumMaster(mod);
        mod.IsMediumMaster.Should().BeTrue();
        mod.Npcs.Records.Select(x => x.FormKey)
            .Should().Equal(n.FormKey, n2.FormKey);
    }
    
    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void CompactRecordToMediumMaster(
        StarfieldMod mod,
        StarfieldNpc n,
        ModCompactor modCompactor)
    {
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
        StarfieldMod mod,
        StarfieldNpc n,
        ModCompactor modCompactor)
    {
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
    
    [Theory, MutagenModAutoData]
    public void AlreadyFullMasterCompatible(
        SkyrimMod mod,
        SkyrimNpc n,
        ModCompactor modCompactor)
    {
        var n2 = mod.Npcs.AddNew(FormKey.Factory($"000810:{mod.ModKey.FileName}"));
        mod.IsSmallMaster.Should().BeFalse();
        mod.IsMediumMaster.Should().BeFalse();
        modCompactor.CompactToFullMaster(mod);
        mod.IsSmallMaster.Should().BeFalse();
        mod.IsMediumMaster.Should().BeFalse();
        mod.Npcs.Records.Select(x => x.FormKey)
            .Should().Equal(n.FormKey, n2.FormKey);
    }
    
    [Theory, MutagenModAutoData]
    public void CompactRecordToFullMasterWithoutLowRange(
        SkyrimMod mod,
        SkyrimNpc n,
        ModCompactor modCompactor)
    {
        var n2 = mod.Npcs.AddNew(FormKey.Factory($"000010:{mod.ModKey.FileName}"));
        mod.IsSmallMaster.Should().BeFalse();
        mod.IsMediumMaster.Should().BeFalse();
        modCompactor.CompactToFullMaster(mod);
        mod.IsSmallMaster.Should().BeFalse();
        mod.IsMediumMaster.Should().BeFalse();
        mod.Npcs.Records.Select(x => x.FormKey)
            .Should().Equal(n.FormKey, new FormKey(n.FormKey.ModKey, n.FormKey.ID + 1));
    }
}