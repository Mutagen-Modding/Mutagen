using FluentAssertions;
using Mutagen.Bethesda.Skyrim;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class Defaults_Tests
    {
        [Fact]
        public void NewSkyrimSEForm44Header()
        {
            var mod = new SkyrimMod(Utility.ModKey, SkyrimRelease.SkyrimSE);
            var ammo = mod.Ammunitions.AddNew();
            ammo.FormVersion.Should().Be(44);
        }

        [Fact]
        public void NewSkyrimSEForm43Header()
        {
            var mod = new SkyrimMod(Utility.ModKey, SkyrimRelease.SkyrimLE);
            var ammo = mod.Ammunitions.AddNew();
            ammo.FormVersion.Should().Be(43);
        }
    }
}
