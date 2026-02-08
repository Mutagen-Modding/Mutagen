using Noggog;
using Shouldly;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Meta;
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
        var mod = new OblivionMod(modKey,
            OblivionRelease.Oblivion);
        mod.ModHeader.Stats.Version.ShouldBe(1f);
        mod.ModHeader.Stats.NextFormID.ShouldEqual(0x800);
    }

    [Theory, MutagenAutoData]
    public void Starfield(ModKey modKey)
    {
        var mod = new StarfieldMod(modKey, StarfieldRelease.Starfield);
        mod.ModHeader.Stats.Version.ShouldBe(0.96f);
        mod.ModHeader.Stats.NextFormID.ShouldEqual(0x800);
    }

    [Theory, MutagenAutoData]
    public void SkyrimSE(ModKey modKey)
    {
        var mod = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);
        mod.ModHeader.Stats.Version.ShouldBe(1.71f);
        mod.ModHeader.Stats.NextFormID.ShouldEqual(0x800);
    }

    [Theory, MutagenAutoData]
    public void SkyrimLE(ModKey modKey)
    {
        var mod = new SkyrimMod(modKey, SkyrimRelease.SkyrimLE);
        mod.ModHeader.Stats.Version.ShouldBe(1.70f);
        mod.ModHeader.Stats.NextFormID.ShouldEqual(0x800);
    }
    
    [Theory, MutagenAutoData]
    public void ExplicitHeaderVersionOverride_SkyrimSE(ModKey modKey)
    {
        var mod = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE, headerVersion: 2.0f);
        mod.ModHeader.Stats.Version.ShouldBe(2.0f);
    }

    [Fact]
    public void ModStatsVersionDefaultMatchesGameConstants()
    {
        foreach (var category in Enums<GameCategory>.Values)
        {
            var modStatsType = Type.GetType(
                $"Mutagen.Bethesda.{category}.ModStats, Mutagen.Bethesda.{category}");
            modStatsType.ShouldNotBeNull($"Could not find ModStats type for {category}");

            var field = modStatsType.GetField("VersionDefault",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            field.ShouldNotBeNull($"ModStats for {category} missing VersionDefault field");
            var versionDefault = (float)field.GetValue(null)!;

            var releaseVersions = category.GetRelatedReleases()
                .Select(r => GameConstants.Get(r).DefaultModHeaderVersion)
                .Where(v => v.HasValue)
                .Select(v => v!.Value)
                .ToHashSet();

            releaseVersions.ShouldContain(versionDefault,
                $"{category}.ModStats.VersionDefault ({versionDefault}) "
                + $"not found among GameConstants values: {string.Join(", ", releaseVersions)}");
        }
    }
}