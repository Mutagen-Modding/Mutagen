using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order;
using System.IO;
using System.Linq;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class HasModsMixInTests
    {
        [Fact]
        public void HasMod_Empty()
        {
            Enumerable.Empty<ModListing>()
                .HasMod(Utility.LightMasterModKey)
                .Should().BeFalse();
        }

        [Fact]
        public void HasMod_Typical()
        {
            var listings = new ModListing[]
            {
                new ModListing(Utility.LightMasterModKey, true),
                new ModListing(Utility.LightMasterModKey2, false),
                new ModListing(Utility.LightMasterModKey3, true),
            };
            listings
                .HasMod(Utility.LightMasterModKey)
                .Should().BeTrue();
            listings
                .HasMod(Utility.LightMasterModKey2)
                .Should().BeTrue();
            listings
                .HasMod(Utility.LightMasterModKey3)
                .Should().BeTrue();
            listings
                .HasMod(Utility.LightMasterModKey4)
                .Should().BeFalse();
        }

        [Fact]
        public void HasMod_Enabled()
        {
            var listings = new ModListing[]
            {
                new ModListing(Utility.LightMasterModKey, true),
                new ModListing(Utility.LightMasterModKey2, false),
                new ModListing(Utility.LightMasterModKey3, true),
            };
            listings
                .HasMod(Utility.LightMasterModKey, enabled: true)
                .Should().BeTrue();
            listings
                .HasMod(Utility.LightMasterModKey, enabled: false)
                .Should().BeFalse();
            listings
                .HasMod(Utility.LightMasterModKey2, enabled: false)
                .Should().BeTrue();
            listings
                .HasMod(Utility.LightMasterModKey2, enabled: true)
                .Should().BeFalse();
            listings
                .HasMod(Utility.LightMasterModKey3, enabled: true)
                .Should().BeTrue();
            listings
                .HasMod(Utility.LightMasterModKey3, enabled: false)
                .Should().BeFalse();
        }

        [Fact]
        public void EmptyListings()
        {
            Enumerable.Empty<ModListing>()
                .HasMods(Utility.LightMasterModKey, Utility.LightMasterModKey2)
                .Should().BeFalse();
        }

        [Fact]
        public void EmptyInput()
        {
            Enumerable.Empty<ModListing>()
                .HasMods()
                .Should().BeTrue();
        }

        [Fact]
        public void Single()
        {
            var listings = new ModListing[]
            {
                new ModListing(Utility.LightMasterModKey, true),
                new ModListing(Utility.LightMasterModKey2, false),
                new ModListing(Utility.LightMasterModKey3, true),
            };
            listings
                .HasMods(Utility.LightMasterModKey)
                .Should().BeTrue();
            listings
                .HasMods(Utility.LightMasterModKey2)
                .Should().BeTrue();
            listings
                .HasMods(Utility.LightMasterModKey3)
                .Should().BeTrue();
            listings
                .HasMods(Utility.LightMasterModKey4)
                .Should().BeFalse();
        }

        [Fact]
        public void Typical()
        {
            var listings = new ModListing[]
            {
                new ModListing(Utility.LightMasterModKey, true),
                new ModListing(Utility.LightMasterModKey2, false),
                new ModListing(Utility.LightMasterModKey3, true),
            };
            listings
                .HasMods(Utility.LightMasterModKey, Utility.LightMasterModKey2, Utility.LightMasterModKey3)
                .Should().BeTrue();
            listings
                .HasMods(Utility.LightMasterModKey, Utility.LightMasterModKey2)
                .Should().BeTrue();
            listings
                .HasMods(Utility.LightMasterModKey, Utility.LightMasterModKey2, Utility.LightMasterModKey4)
                .Should().BeFalse();
        }

        [Fact]
        public void Enabled_EmptyListings()
        {
            Enumerable.Empty<ModListing>()
                .HasMods(true, Utility.LightMasterModKey, Utility.LightMasterModKey2)
                .Should().BeFalse();
            Enumerable.Empty<ModListing>()
                .HasMods(false, Utility.LightMasterModKey, Utility.LightMasterModKey2)
                .Should().BeFalse();
        }

        [Fact]
        public void Enabled_EmptyInput()
        {
            Enumerable.Empty<ModListing>()
                .HasMods(true)
                .Should().BeTrue();
            Enumerable.Empty<ModListing>()
                .HasMods(false)
                .Should().BeTrue();
        }

        [Fact]
        public void Enabled_Single()
        {
            var listings = new ModListing[]
            {
                new ModListing(Utility.LightMasterModKey, true),
                new ModListing(Utility.LightMasterModKey2, false),
                new ModListing(Utility.LightMasterModKey3, true),
            };
            listings
                .HasMods(true, Utility.LightMasterModKey)
                .Should().BeTrue();
            listings
                .HasMods(false, Utility.LightMasterModKey)
                .Should().BeFalse();
            listings
                .HasMods(false, Utility.LightMasterModKey2)
                .Should().BeTrue();
            listings
                .HasMods(true, Utility.LightMasterModKey2)
                .Should().BeFalse();
            listings
                .HasMods(true, Utility.LightMasterModKey3)
                .Should().BeTrue();
            listings
                .HasMods(false, Utility.LightMasterModKey3)
                .Should().BeFalse();
            listings
                .HasMods(true, Utility.LightMasterModKey4)
                .Should().BeFalse();
            listings
                .HasMods(false, Utility.LightMasterModKey4)
                .Should().BeFalse();
        }

        [Fact]
        public void Enabled_Typical()
        {
            var listings = new ModListing[]
            {
                new ModListing(Utility.LightMasterModKey, true),
                new ModListing(Utility.LightMasterModKey2, false),
                new ModListing(Utility.LightMasterModKey3, true),
            };
            listings
                .HasMods(
                    true,
                    Utility.LightMasterModKey, Utility.LightMasterModKey3)
                .Should().BeTrue();
            listings
                .HasMods(false, Utility.LightMasterModKey2)
                .Should().BeTrue();
            listings
                .HasMods(
                    true,
                    Utility.LightMasterModKey, Utility.LightMasterModKey2, Utility.LightMasterModKey3)
                .Should().BeFalse();
            listings
                .HasMods(
                    true,
                    Utility.LightMasterModKey, Utility.LightMasterModKey2, Utility.LightMasterModKey4)
                .Should().BeFalse();
        }
    }
}
