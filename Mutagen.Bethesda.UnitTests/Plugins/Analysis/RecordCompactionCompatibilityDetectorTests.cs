using Shouldly;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Analysis.DI;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;
using Npc = Mutagen.Bethesda.Skyrim.Npc;
using StarfieldNpc = Mutagen.Bethesda.Starfield.Npc;

namespace Mutagen.Bethesda.UnitTests.Plugins.Analysis;

public class RecordCompactionCompatibilityDetectorTests
{
    [Theory, MutagenModAutoData(GameRelease.Oblivion)]
    public void OldGame(
        OblivionMod mod,
        RecordCompactionCompatibilityDetector sut)
    {
        sut.IsSmallMasterCompatible(mod)
            .ShouldBeFalse();
        sut.IsMediumMasterCompatible(mod)
            .ShouldBeFalse();
        sut.CouldBeSmallMasterCompatible(mod)
            .ShouldBeFalse();
        sut.CouldBeMediumMasterCompatible(mod)
            .ShouldBeFalse();
    }
    
    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void LightMasterEmpty(
        SkyrimMod mod,
        RecordCompactionCompatibilityDetector sut)
    {
        sut.IsSmallMasterCompatible(mod)
            .ShouldBeTrue();
        sut.CouldBeSmallMasterCompatible(mod)
            .ShouldBeTrue();
    }
    
    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void MediumMasterEmpty(
        StarfieldMod mod,
        RecordCompactionCompatibilityDetector sut)
    {
        sut.IsMediumMasterCompatible(mod)
            .ShouldBeTrue();
        sut.CouldBeMediumMasterCompatible(mod)
            .ShouldBeTrue();
    }
    
    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void LightMasterLowerFormIDCompatible(
        SkyrimMod mod,
        Npc n,
        RecordCompactionCompatibilityDetector sut)
    {
        sut.IsSmallMasterCompatible(mod)
            .ShouldBeTrue();
        sut.CouldBeSmallMasterCompatible(mod)
            .ShouldBeTrue();
    }
    
    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void LightMasterLowerFormIDIncompatible(
        SkyrimMod mod,
        RecordCompactionCompatibilityDetector sut)
    {
        Npc n = new Npc(new FormKey(mod.ModKey, 0x500), SkyrimRelease.SkyrimSE);
        mod.Npcs.Add(n);
        mod.ModHeader.Stats.Version = 1.60f;
        sut.IsSmallMasterCompatible(mod)
            .ShouldBeFalse();
        sut.CouldBeSmallMasterCompatible(mod)
            .ShouldBeTrue();
    }
    
    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void LightMasterNormalFormID(
        SkyrimMod mod,
        RecordCompactionCompatibilityDetector sut)
    {
        mod.Npcs.Add(new Npc(new FormKey(mod.ModKey, 0x801), SkyrimRelease.SkyrimSE));
        sut.IsSmallMasterCompatible(mod)
            .ShouldBeTrue();
        sut.CouldBeSmallMasterCompatible(mod)
            .ShouldBeTrue();
    }
    
    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void LightMasterCouldNotBeSmallMasterCompatible(
        SkyrimMod mod,
        RecordCompactionCompatibilityDetector sut)
    {
        for (int i = 0; i < 4096; i++)
        {
            mod.Npcs.AddNew();
        }
        sut.CouldBeSmallMasterCompatible(mod)
            .ShouldBeFalse();
    }
    
    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void MediumMasterNormalFormID(
        StarfieldMod mod,
        RecordCompactionCompatibilityDetector sut)
    {
        mod.Npcs.Add(new StarfieldNpc(new FormKey(mod.ModKey, 0x801), StarfieldRelease.Starfield));
        sut.IsMediumMasterCompatible(mod)
            .ShouldBeTrue();
        sut.CouldBeSmallMasterCompatible(mod)
            .ShouldBeTrue();
    }
    
    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void LightMasterNotCompatible(
        SkyrimMod mod,
        RecordCompactionCompatibilityDetector sut)
    {
        mod.Npcs.Add(new Npc(new FormKey(mod.ModKey, 0xFFFF), SkyrimRelease.SkyrimSE));
        sut.IsSmallMasterCompatible(mod)
            .ShouldBeFalse();
        sut.CouldBeSmallMasterCompatible(mod)
            .ShouldBeTrue();
    }
    
    [Theory, MutagenModAutoData(GameRelease.Starfield)]
    public void MediumMasterNotCompatible(
        StarfieldMod mod,
        RecordCompactionCompatibilityDetector sut)
    {
        mod.Npcs.Add(new StarfieldNpc(new FormKey(mod.ModKey, 0xFFFFF), StarfieldRelease.Starfield));
        sut.IsMediumMasterCompatible(mod)
            .ShouldBeFalse();
        sut.CouldBeSmallMasterCompatible(mod)
            .ShouldBeTrue();
    }
    
    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void MediumMasterCouldNotBeMediumMasterCompatible(
        SkyrimMod mod,
        RecordCompactionCompatibilityDetector sut)
    {
        for (int i = 0; i < 65536; i++)
        {
            mod.Npcs.AddNew();
        }
        sut.CouldBeMediumMasterCompatible(mod)
            .ShouldBeFalse();
    }
}