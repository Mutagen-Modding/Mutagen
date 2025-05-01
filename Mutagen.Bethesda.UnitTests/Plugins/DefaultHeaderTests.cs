using Shouldly;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog.Testing.Extensions;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class DefaultHeaderTests
{
    [Theory, MutagenAutoData]
    public void Fallout4(ModKey modKey)
    {
        var mod = new Fallout4Mod(modKey, Fallout4Release.Fallout4);
        mod.ModHeader.Stats.Version.ShouldBe(1f);
        mod.ModHeader.Stats.NextFormID.ShouldEqual(0x800);
    }

    [Theory, MutagenAutoData]
    public void Oblivion(ModKey modKey)
    {
        var mod = new OblivionMod(modKey);
        mod.ModHeader.Stats.Version.ShouldBe(1f);
        mod.ModHeader.Stats.NextFormID.ShouldEqual(0xD62);
    }

    [Theory, MutagenAutoData]
    public void Starfield(ModKey modKey)
    {
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.ModHeader.Stats.Version.ShouldBe(0.96f);
        mod.ModHeader.Stats.NextFormID.ShouldEqual(0x800);
    }

    [Theory, MutagenAutoData]
    public void Skyrim(ModKey modKey)
    {
        var mod = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);
        mod.ModHeader.Stats.Version.ShouldBe(1.71f);
        mod.ModHeader.Stats.NextFormID.ShouldEqual(0x800);
    }
}