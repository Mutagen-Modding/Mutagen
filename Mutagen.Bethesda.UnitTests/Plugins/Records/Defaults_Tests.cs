using FluentAssertions;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records
{
    public class Defaults_Tests
    {
        [Fact]
        public void NewSkyrimSEForm44Header()
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
            var ammo = mod.Ammunitions.AddNew();
            ammo.FormVersion.Should().Be(44);
        }

        [Fact]
        public void NewSkyrimSEForm43Header()
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var ammo = mod.Ammunitions.AddNew();
            ammo.FormVersion.Should().Be(43);
        }
    }
}
