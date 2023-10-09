using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class HasModsMixInTests
{
    [Fact]
    public void HasMod_Empty()
    {
        Enumerable.Empty<ModListing>()
            .ModExists(TestConstants.LightModKey)
            .Should().BeFalse();
    }

    [Fact]
    public void HasMod_Typical()
    {
        var listings = new ModListing[]
        {
            new ModListing(TestConstants.LightModKey, true, existsOnDisk: true),
            new ModListing(TestConstants.LightModKey2, false, existsOnDisk: true),
            new ModListing(TestConstants.LightModKey3, true, existsOnDisk: true),
        };
        listings
            .ModExists(TestConstants.LightModKey)
            .Should().BeTrue();
        listings
            .ModExists(TestConstants.LightModKey2)
            .Should().BeTrue();
        listings
            .ModExists(TestConstants.LightModKey3)
            .Should().BeTrue();
        listings
            .ModExists(TestConstants.LightModKey4)
            .Should().BeFalse();
    }

    [Fact]
    public void HasMod_Enabled()
    {
        var listings = new ModListing[]
        {
            new ModListing(TestConstants.LightModKey, true, existsOnDisk: true),
            new ModListing(TestConstants.LightModKey2, false, existsOnDisk: true),
            new ModListing(TestConstants.LightModKey3, true, existsOnDisk: true),
        };
        listings
            .ModExists(TestConstants.LightModKey, enabled: true)
            .Should().BeTrue();
        listings
            .ModExists(TestConstants.LightModKey, enabled: false)
            .Should().BeFalse();
        listings
            .ModExists(TestConstants.LightModKey2, enabled: false)
            .Should().BeTrue();
        listings
            .ModExists(TestConstants.LightModKey2, enabled: true)
            .Should().BeFalse();
        listings
            .ModExists(TestConstants.LightModKey3, enabled: true)
            .Should().BeTrue();
        listings
            .ModExists(TestConstants.LightModKey3, enabled: false)
            .Should().BeFalse();
    }

    [Fact]
    public void EmptyListings()
    {
        Enumerable.Empty<ModListing>()
            .ModsExist(TestConstants.LightModKey, TestConstants.LightModKey2)
            .Should().BeFalse();
    }

    [Fact]
    public void EmptyInput()
    {
        Enumerable.Empty<ModListing>()
            .ModsExist()
            .Should().BeTrue();
    }

    [Fact]
    public void Single()
    {
        var listings = new ModListing[]
        {
            new ModListing(TestConstants.LightModKey, true, existsOnDisk: true),
            new ModListing(TestConstants.LightModKey2, false, existsOnDisk: true),
            new ModListing(TestConstants.LightModKey3, true, existsOnDisk: true),
        };
        listings
            .ModsExist(TestConstants.LightModKey)
            .Should().BeTrue();
        listings
            .ModsExist(TestConstants.LightModKey2)
            .Should().BeTrue();
        listings
            .ModsExist(TestConstants.LightModKey3)
            .Should().BeTrue();
        listings
            .ModsExist(TestConstants.LightModKey4)
            .Should().BeFalse();
    }

    [Fact]
    public void Typical()
    {
        var listings = new ModListing[]
        {
            new ModListing(TestConstants.LightModKey, true, existsOnDisk: true),
            new ModListing(TestConstants.LightModKey2, false, existsOnDisk: true),
            new ModListing(TestConstants.LightModKey3, true, existsOnDisk: true),
        };
        listings
            .ModsExist(TestConstants.LightModKey, TestConstants.LightModKey2, TestConstants.LightModKey3)
            .Should().BeTrue();
        listings
            .ModsExist(TestConstants.LightModKey, TestConstants.LightModKey2)
            .Should().BeTrue();
        listings
            .ModsExist(TestConstants.LightModKey, TestConstants.LightModKey2, TestConstants.LightModKey4)
            .Should().BeFalse();
    }

    [Fact]
    public void Enabled_EmptyListings()
    {
        Enumerable.Empty<ModListing>()
            .ModsExist(true, TestConstants.LightModKey, TestConstants.LightModKey2)
            .Should().BeFalse();
        Enumerable.Empty<ModListing>()
            .ModsExist(false, TestConstants.LightModKey, TestConstants.LightModKey2)
            .Should().BeFalse();
    }

    [Fact]
    public void Enabled_EmptyInput()
    {
        Enumerable.Empty<ModListing>()
            .ModsExist(true)
            .Should().BeTrue();
        Enumerable.Empty<ModListing>()
            .ModsExist(false)
            .Should().BeTrue();
    }

    [Fact]
    public void Enabled_Single()
    {
        var listings = new ModListing[]
        {
            new ModListing(TestConstants.LightModKey, true, existsOnDisk: true),
            new ModListing(TestConstants.LightModKey2, false, existsOnDisk: true),
            new ModListing(TestConstants.LightModKey3, true, existsOnDisk: true),
        };
        listings
            .ModsExist(true, TestConstants.LightModKey)
            .Should().BeTrue();
        listings
            .ModsExist(false, TestConstants.LightModKey)
            .Should().BeFalse();
        listings
            .ModsExist(false, TestConstants.LightModKey2)
            .Should().BeTrue();
        listings
            .ModsExist(true, TestConstants.LightModKey2)
            .Should().BeFalse();
        listings
            .ModsExist(true, TestConstants.LightModKey3)
            .Should().BeTrue();
        listings
            .ModsExist(false, TestConstants.LightModKey3)
            .Should().BeFalse();
        listings
            .ModsExist(true, TestConstants.LightModKey4)
            .Should().BeFalse();
        listings
            .ModsExist(false, TestConstants.LightModKey4)
            .Should().BeFalse();
    }

    [Fact]
    public void Exists_Single()
    {
        var listings = new ModListing[]
        {
            new ModListing(TestConstants.LightModKey, true, existsOnDisk: true),
            new ModListing(TestConstants.LightModKey2, false, existsOnDisk: false),
            new ModListing(TestConstants.LightModKey3, true, existsOnDisk: true),
        };
        listings
            .ModsExist(TestConstants.LightModKey)
            .Should().BeTrue();
        listings
            .ModsExist(TestConstants.LightModKey2)
            .Should().BeFalse();
        listings
            .ModsExist(TestConstants.LightModKey3)
            .Should().BeTrue();
        listings
            .ModsExist(TestConstants.LightModKey4)
            .Should().BeFalse();
    }

    [Fact]
    public void Enabled_Typical()
    {
        var listings = new ModListing[]
        {
            new ModListing(TestConstants.LightModKey, true, existsOnDisk: true),
            new ModListing(TestConstants.LightModKey2, false, existsOnDisk: true),
            new ModListing(TestConstants.LightModKey3, true, existsOnDisk: true),
        };
        listings
            .ModsExist(
                true,
                TestConstants.LightModKey, TestConstants.LightModKey3)
            .Should().BeTrue();
        listings
            .ModsExist(false, TestConstants.LightModKey2)
            .Should().BeTrue();
        listings
            .ModsExist(
                true,
                TestConstants.LightModKey, TestConstants.LightModKey2, TestConstants.LightModKey3)
            .Should().BeFalse();
        listings
            .ModsExist(
                true,
                TestConstants.LightModKey, TestConstants.LightModKey2, TestConstants.LightModKey4)
            .Should().BeFalse();
    }
}