using Shouldly;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Noggog.Testing.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class DefaultsTests
{
    [Fact]
    public void NewSkyrimSEForm44Header()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var ammo = mod.Ammunitions.AddNew();
        ammo.FormVersion.ShouldEqual(44);
    }

    [Fact]
    public void NewSkyrimSEForm43Header()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var ammo = mod.Ammunitions.AddNew();
        ammo.FormVersion.ShouldEqual(43);
    }
}