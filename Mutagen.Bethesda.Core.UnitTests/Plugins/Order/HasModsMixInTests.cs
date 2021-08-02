using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order;
using System.IO;
using System.Linq;
using Mutagen.Bethesda.Core.UnitTests;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class HasModsMixInTests
    {
        [Fact]
        public void HasMod_Empty()
        {
            Enumerable.Empty<ModListing>()
                .HasMod(TestConstants.LightMasterModKey)
                .Should().BeFalse();
        }

        [Fact]
        public void HasMod_Typical()
        {
            var listings = new ModListing[]
            {
                new ModListing(TestConstants.LightMasterModKey, true),
                new ModListing(TestConstants.LightMasterModKey2, false),
                new ModListing(TestConstants.LightMasterModKey3, true),
            };
            listings
                .HasMod(TestConstants.LightMasterModKey)
                .Should().BeTrue();
            listings
                .HasMod(TestConstants.LightMasterModKey2)
                .Should().BeTrue();
            listings
                .HasMod(TestConstants.LightMasterModKey3)
                .Should().BeTrue();
            listings
                .HasMod(TestConstants.LightMasterModKey4)
                .Should().BeFalse();
        }

        [Fact]
        public void HasMod_Enabled()
        {
            var listings = new ModListing[]
            {
                new ModListing(TestConstants.LightMasterModKey, true),
                new ModListing(TestConstants.LightMasterModKey2, false),
                new ModListing(TestConstants.LightMasterModKey3, true),
            };
            listings
                .HasMod(TestConstants.LightMasterModKey, enabled: true)
                .Should().BeTrue();
            listings
                .HasMod(TestConstants.LightMasterModKey, enabled: false)
                .Should().BeFalse();
            listings
                .HasMod(TestConstants.LightMasterModKey2, enabled: false)
                .Should().BeTrue();
            listings
                .HasMod(TestConstants.LightMasterModKey2, enabled: true)
                .Should().BeFalse();
            listings
                .HasMod(TestConstants.LightMasterModKey3, enabled: true)
                .Should().BeTrue();
            listings
                .HasMod(TestConstants.LightMasterModKey3, enabled: false)
                .Should().BeFalse();
        }

        [Fact]
        public void EmptyListings()
        {
            Enumerable.Empty<ModListing>()
                .HasMods(TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2)
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
                new ModListing(TestConstants.LightMasterModKey, true),
                new ModListing(TestConstants.LightMasterModKey2, false),
                new ModListing(TestConstants.LightMasterModKey3, true),
            };
            listings
                .HasMods(TestConstants.LightMasterModKey)
                .Should().BeTrue();
            listings
                .HasMods(TestConstants.LightMasterModKey2)
                .Should().BeTrue();
            listings
                .HasMods(TestConstants.LightMasterModKey3)
                .Should().BeTrue();
            listings
                .HasMods(TestConstants.LightMasterModKey4)
                .Should().BeFalse();
        }

        [Fact]
        public void Typical()
        {
            var listings = new ModListing[]
            {
                new ModListing(TestConstants.LightMasterModKey, true),
                new ModListing(TestConstants.LightMasterModKey2, false),
                new ModListing(TestConstants.LightMasterModKey3, true),
            };
            listings
                .HasMods(TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2, TestConstants.LightMasterModKey3)
                .Should().BeTrue();
            listings
                .HasMods(TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2)
                .Should().BeTrue();
            listings
                .HasMods(TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2, TestConstants.LightMasterModKey4)
                .Should().BeFalse();
        }

        [Fact]
        public void Enabled_EmptyListings()
        {
            Enumerable.Empty<ModListing>()
                .HasMods(true, TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2)
                .Should().BeFalse();
            Enumerable.Empty<ModListing>()
                .HasMods(false, TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2)
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
                new ModListing(TestConstants.LightMasterModKey, true),
                new ModListing(TestConstants.LightMasterModKey2, false),
                new ModListing(TestConstants.LightMasterModKey3, true),
            };
            listings
                .HasMods(true, TestConstants.LightMasterModKey)
                .Should().BeTrue();
            listings
                .HasMods(false, TestConstants.LightMasterModKey)
                .Should().BeFalse();
            listings
                .HasMods(false, TestConstants.LightMasterModKey2)
                .Should().BeTrue();
            listings
                .HasMods(true, TestConstants.LightMasterModKey2)
                .Should().BeFalse();
            listings
                .HasMods(true, TestConstants.LightMasterModKey3)
                .Should().BeTrue();
            listings
                .HasMods(false, TestConstants.LightMasterModKey3)
                .Should().BeFalse();
            listings
                .HasMods(true, TestConstants.LightMasterModKey4)
                .Should().BeFalse();
            listings
                .HasMods(false, TestConstants.LightMasterModKey4)
                .Should().BeFalse();
        }

        [Fact]
        public void Enabled_Typical()
        {
            var listings = new ModListing[]
            {
                new ModListing(TestConstants.LightMasterModKey, true),
                new ModListing(TestConstants.LightMasterModKey2, false),
                new ModListing(TestConstants.LightMasterModKey3, true),
            };
            listings
                .HasMods(
                    true,
                    TestConstants.LightMasterModKey, TestConstants.LightMasterModKey3)
                .Should().BeTrue();
            listings
                .HasMods(false, TestConstants.LightMasterModKey2)
                .Should().BeTrue();
            listings
                .HasMods(
                    true,
                    TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2, TestConstants.LightMasterModKey3)
                .Should().BeFalse();
            listings
                .HasMods(
                    true,
                    TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2, TestConstants.LightMasterModKey4)
                .Should().BeFalse();
        }
    }
}
