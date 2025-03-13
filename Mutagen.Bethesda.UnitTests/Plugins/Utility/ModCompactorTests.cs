using System.Reflection;
using Autofac;
using Shouldly;
using Mutagen.Bethesda.Autofac;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Utility.DI;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog.Testing.Extensions;
using Xunit;
using SkyrimNpc = Mutagen.Bethesda.Skyrim.Npc;
using StarfieldNpc = Mutagen.Bethesda.Starfield.Npc;

namespace Mutagen.Bethesda.UnitTests.Plugins.Utility;

public class ModCompactorTests
{
    public class Payload
    {
        public IModCompactor Sut { get; }

        public Payload()
        {
            var b = new ContainerBuilder();
            b.RegisterModule<MutagenModule>();
            Sut = b.Build().Resolve<IModCompactor>();
        }
    }
    
    [Theory, MutagenModAutoData]
    public void AlreadySmallMasterCompatible(
        SkyrimMod mod,
        SkyrimNpc n,
        SkyrimNpc n2,
        Payload sut)
    {
        mod.IsSmallMaster.ShouldBeFalse();
        mod.IsMediumMaster.ShouldBeFalse();
        sut.Sut.CompactToSmallMaster(mod);
        mod.IsSmallMaster.ShouldBeTrue();
        mod.IsMediumMaster.ShouldBeFalse();
        mod.Npcs.Records.Select(x => x.FormKey)
            .ShouldEqual(n.FormKey, n2.FormKey);
    }
    
    [Theory, MutagenModAutoData]
    public void CompactRecordToSmallMaster(
        SkyrimMod mod,
        SkyrimNpc n,
        Payload sut)
    {
        var n2 = mod.Npcs.AddReturn(
            new SkyrimNpc(FormKey.Factory($"010000:{mod.ModKey.FileName}"), SkyrimRelease.SkyrimSE));
        var n3 = mod.Npcs.AddReturn(
            new SkyrimNpc(FormKey.Factory($"000030:{mod.ModKey.FileName}"), SkyrimRelease.SkyrimSE));
        mod.IsSmallMaster.ShouldBeFalse();
        mod.IsMediumMaster.ShouldBeFalse();
        sut.Sut.CompactToSmallMaster(mod);
        mod.IsSmallMaster.ShouldBeTrue();
        mod.IsMediumMaster.ShouldBeFalse();
        mod.Npcs.Records.Select(x => x.FormKey)
            .ShouldEqual(n.FormKey, new FormKey(n.FormKey.ModKey, n.FormKey.ID + 1), n3.FormKey);
    }
    
    [Theory, MutagenModAutoData]
    public void FailToCompactRecordToSmallMaster(
        SkyrimMod mod,
        SkyrimNpc n,
        Payload sut)
    {
        for (int i = 0; i < 4096; i++)
        {
            mod.Npcs.AddNew();
        }
        mod.IsSmallMaster.ShouldBeFalse();
        mod.IsMediumMaster.ShouldBeFalse();
        Assert.Throws<TargetInvocationException>(() =>
        {
            sut.Sut.CompactToSmallMaster(mod);
        });
    }
    
    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void AlreadyMediumMasterCompatible(
        StarfieldMod mod,
        StarfieldNpc n,
        StarfieldNpc n2,
        Payload sut)
    {
        mod.IsSmallMaster.ShouldBeFalse();
        mod.IsMediumMaster.ShouldBeFalse();
        sut.Sut.CompactToMediumMaster(mod);
        mod.IsSmallMaster.ShouldBeFalse();
        mod.IsMediumMaster.ShouldBeTrue();
        mod.Npcs.Records.Select(x => x.FormKey)
            .ShouldEqual(n.FormKey, n2.FormKey);
    }
    
    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void CompactRecordToMediumMaster(
        StarfieldMod mod,
        StarfieldNpc n,
        Payload sut)
    {
        var n2 = mod.Npcs.AddReturn(
            new Starfield.Npc(FormKey.Factory($"080000:{mod.ModKey.FileName}"), StarfieldRelease.Starfield));
        var n3 = mod.Npcs.AddReturn(
            new Starfield.Npc(FormKey.Factory($"000030:{mod.ModKey.FileName}"), StarfieldRelease.Starfield));
        mod.IsSmallMaster.ShouldBeFalse();
        mod.IsMediumMaster.ShouldBeFalse();
        sut.Sut.CompactToMediumMaster(mod);
        mod.IsSmallMaster.ShouldBeFalse();
        mod.IsMediumMaster.ShouldBeTrue();
        mod.Npcs.Records.Select(x => x.FormKey)
            .ShouldEqual(n.FormKey, new FormKey(n.FormKey.ModKey, n.FormKey.ID + 1), n3.FormKey);
    }
    
    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void FailToCompactRecordToMediumMaster(
        StarfieldMod mod,
        StarfieldNpc n,
        Payload sut)
    {
        for (int i = 0; i < 65536; i++)
        {
            mod.Npcs.AddNew();
        }
        mod.IsSmallMaster.ShouldBeFalse();
        mod.IsMediumMaster.ShouldBeFalse();
        Assert.Throws<TargetInvocationException>(() =>
        {
            sut.Sut.CompactToMediumMaster(mod);
        });
    }
    
    [Theory, MutagenModAutoData]
    public void AlreadyFullMasterCompatible(
        SkyrimMod mod,
        SkyrimNpc n,
        Payload sut)
    {
        var n2 = mod.Npcs.AddNew(FormKey.Factory($"000810:{mod.ModKey.FileName}"));
        mod.IsSmallMaster.ShouldBeFalse();
        mod.IsMediumMaster.ShouldBeFalse();
        sut.Sut.CompactToFullMaster(mod);
        mod.IsSmallMaster.ShouldBeFalse();
        mod.IsMediumMaster.ShouldBeFalse();
        mod.Npcs.Records.Select(x => x.FormKey)
            .ShouldEqual(n.FormKey, n2.FormKey);
    }
    
    [Theory, MutagenModAutoData]
    public void CompactRecordToFullMasterWithoutLowRange(
        SkyrimMod mod,
        SkyrimNpc n,
        Payload sut)
    {
        var n2 = mod.Npcs.AddNew(FormKey.Factory($"000010:{mod.ModKey.FileName}"));
        mod.IsSmallMaster.ShouldBeFalse();
        mod.IsMediumMaster.ShouldBeFalse();
        sut.Sut.CompactToFullMaster(mod);
        mod.IsSmallMaster.ShouldBeFalse();
        mod.IsMediumMaster.ShouldBeFalse();
        mod.Npcs.Records.Select(x => x.FormKey)
            .ShouldEqual(n.FormKey, new FormKey(n.FormKey.ModKey, n.FormKey.ID + 1));
    }
    
    [Theory, MutagenModAutoData]
    public void CompactToWithFallbackSmall(
        SkyrimMod mod,
        SkyrimNpc n,
        Payload sut)
    {
        var n2 = mod.Npcs.AddNew(FormKey.Factory($"010000:{mod.ModKey.FileName}"));
        var n3 = mod.Npcs.AddReturn(
            new SkyrimNpc(FormKey.Factory($"000030:{mod.ModKey.FileName}"), SkyrimRelease.SkyrimSE));
        mod.IsSmallMaster.ShouldBeFalse();
        mod.IsMediumMaster.ShouldBeFalse();
        sut.Sut.CompactToWithFallback(mod, MasterStyle.Small);
        mod.IsSmallMaster.ShouldBeTrue();
        mod.IsMediumMaster.ShouldBeFalse();
        mod.Npcs.Records.Select(x => x.FormKey)
            .ShouldEqual(n.FormKey, new FormKey(n.FormKey.ModKey, n.FormKey.ID + 1), n3.FormKey);
    }
    
    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void CompactToWithFallbackSmallFallbackToMedium(
        StarfieldMod mod,
        StarfieldNpc n,
        Payload sut)
    {
        var n2 = mod.Npcs.AddNew(FormKey.Factory($"010000:{mod.ModKey.FileName}"));
        var n3 = mod.Npcs.AddReturn(
            new StarfieldNpc(FormKey.Factory($"000030:{mod.ModKey.FileName}"), StarfieldRelease.Starfield));
        for (int i = 0; i < 4096; i++)
        {
            mod.Npcs.AddNew();
        }
        mod.IsSmallMaster.ShouldBeFalse();
        mod.IsMediumMaster.ShouldBeFalse();
        sut.Sut.CompactToWithFallback(mod, MasterStyle.Small);
        mod.IsSmallMaster.ShouldBeFalse();
        mod.IsMediumMaster.ShouldBeTrue();
        mod.Npcs.Records.Count().ShouldBe(4099);
    }
    
    [Theory, MutagenModAutoData]
    public void CompactToWithFallbackSmallFallbackToFull(
        SkyrimMod mod,
        SkyrimNpc n,
        Payload sut)
    {
        var n2 = mod.Npcs.AddNew(FormKey.Factory($"010000:{mod.ModKey.FileName}"));
        var n3 = mod.Npcs.AddReturn(
            new SkyrimNpc(FormKey.Factory($"000030:{mod.ModKey.FileName}"), SkyrimRelease.SkyrimSE));
        for (int i = 0; i < 4096; i++)
        {
            mod.Npcs.AddNew();
        }
        mod.IsSmallMaster.ShouldBeFalse();
        mod.IsMediumMaster.ShouldBeFalse();
        sut.Sut.CompactToWithFallback(mod, MasterStyle.Small);
        mod.IsSmallMaster.ShouldBeFalse();
        mod.IsMediumMaster.ShouldBeFalse();
        mod.Npcs.Records.Count().ShouldBe(4099);
    }
}