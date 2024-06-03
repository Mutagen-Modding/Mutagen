using FluentAssertions;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Analysis.DI;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;
using Npc = Mutagen.Bethesda.Skyrim.Npc;

namespace Mutagen.Bethesda.UnitTests.Plugins.Analysis;

public class RecordCompactionCompatibilityDetectorTests
{
    [Theory, MutagenModAutoData(GameRelease.Oblivion)]
    public void OldGame(
        OblivionMod mod,
        RecordCompactionCompatibilityDetector sut)
    {
        sut.IsLightModCompatible(mod)
            .Should().BeFalse();
    }
    
    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void LightMasterEmpty(
        SkyrimMod mod,
        RecordCompactionCompatibilityDetector sut)
    {
        sut.IsLightModCompatible(mod)
            .Should().BeTrue();
    }
    
    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void LightMasterLowerFormIDCompatible(
        SkyrimMod mod,
        Npc n,
        RecordCompactionCompatibilityDetector sut)
    {
        sut.IsLightModCompatible(mod)
            .Should().BeTrue();
    }
    
    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void LightMasterLowerFormIDIncompatible(
        SkyrimMod mod,
        Npc n,
        RecordCompactionCompatibilityDetector sut)
    {
        mod.ModHeader.Version = 55;
        sut.IsLightModCompatible(mod)
            .Should().BeFalse();
    }
    
    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void LightMasterNormalFormID(
        SkyrimMod mod,
        RecordCompactionCompatibilityDetector sut)
    {
        mod.Npcs.Add(new Npc(new FormKey(mod.ModKey, 0x801), SkyrimRelease.SkyrimSE));
        sut.IsLightModCompatible(mod)
            .Should().BeTrue();
    }
    
    [Theory, MutagenModAutoData(GameRelease.SkyrimSE)]
    public void LightMasterNotCompatible(
        SkyrimMod mod,
        RecordCompactionCompatibilityDetector sut)
    {
        mod.Npcs.Add(new Npc(new FormKey(mod.ModKey, 0xFFFF), SkyrimRelease.SkyrimSE));
        sut.IsLightModCompatible(mod)
            .Should().BeFalse();
    }
}