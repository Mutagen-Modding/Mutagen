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
            .ModExists(TestConstants.LightMasterModKey)
            .Should().BeFalse();
    }

    [Fact]
    public void HasMod_Typical()
    {
        var listings = new ModListing[]
        {
            new ModListing(TestConstants.LightMasterModKey, true, existsOnDisk: true),
            new ModListing(TestConstants.LightMasterModKey2, false, existsOnDisk: true),
            new ModListing(TestConstants.LightMasterModKey3, true, existsOnDisk: true),
        };
        listings
            .ModExists(TestConstants.LightMasterModKey)
            .Should().BeTrue();
        listings
            .ModExists(TestConstants.LightMasterModKey2)
            .Should().BeTrue();
        listings
            .ModExists(TestConstants.LightMasterModKey3)
            .Should().BeTrue();
        listings
            .ModExists(TestConstants.LightMasterModKey4)
            .Should().BeFalse();
    }

    [Fact]
    public void HasMod_Enabled()
    {
        var listings = new ModListing[]
        {
            new ModListing(TestConstants.LightMasterModKey, true, existsOnDisk: true),
            new ModListing(TestConstants.LightMasterModKey2, false, existsOnDisk: true),
            new ModListing(TestConstants.LightMasterModKey3, true, existsOnDisk: true),
        };
        listings
            .ModExists(TestConstants.LightMasterModKey, enabled: true)
            .Should().BeTrue();
        listings
            .ModExists(TestConstants.LightMasterModKey, enabled: false)
            .Should().BeFalse();
        listings
            .ModExists(TestConstants.LightMasterModKey2, enabled: false)
            .Should().BeTrue();
        listings
            .ModExists(TestConstants.LightMasterModKey2, enabled: true)
            .Should().BeFalse();
        listings
            .ModExists(TestConstants.LightMasterModKey3, enabled: true)
            .Should().BeTrue();
        listings
            .ModExists(TestConstants.LightMasterModKey3, enabled: false)
            .Should().BeFalse();
    }

    [Fact]
    public void EmptyListings()
    {
        Enumerable.Empty<ModListing>()
            .ModsExist(TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2)
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
            new ModListing(TestConstants.LightMasterModKey, true, existsOnDisk: true),
            new ModListing(TestConstants.LightMasterModKey2, false, existsOnDisk: true),
            new ModListing(TestConstants.LightMasterModKey3, true, existsOnDisk: true),
        };
        listings
            .ModsExist(TestConstants.LightMasterModKey)
            .Should().BeTrue();
        listings
            .ModsExist(TestConstants.LightMasterModKey2)
            .Should().BeTrue();
        listings
            .ModsExist(TestConstants.LightMasterModKey3)
            .Should().BeTrue();
        listings
            .ModsExist(TestConstants.LightMasterModKey4)
            .Should().BeFalse();
    }

    [Fact]
    public void Typical()
    {
        var listings = new ModListing[]
        {
            new ModListing(TestConstants.LightMasterModKey, true, existsOnDisk: true),
            new ModListing(TestConstants.LightMasterModKey2, false, existsOnDisk: true),
            new ModListing(TestConstants.LightMasterModKey3, true, existsOnDisk: true),
        };
        listings
            .ModsExist(TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2, TestConstants.LightMasterModKey3)
            .Should().BeTrue();
        listings
            .ModsExist(TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2)
            .Should().BeTrue();
        listings
            .ModsExist(TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2, TestConstants.LightMasterModKey4)
            .Should().BeFalse();
    }

    [Fact]
    public void Enabled_EmptyListings()
    {
        Enumerable.Empty<ModListing>()
            .ModsExist(true, TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2)
            .Should().BeFalse();
        Enumerable.Empty<ModListing>()
            .ModsExist(false, TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2)
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
            new ModListing(TestConstants.LightMasterModKey, true, existsOnDisk: true),
            new ModListing(TestConstants.LightMasterModKey2, false, existsOnDisk: true),
            new ModListing(TestConstants.LightMasterModKey3, true, existsOnDisk: true),
        };
        listings
            .ModsExist(true, TestConstants.LightMasterModKey)
            .Should().BeTrue();
        listings
            .ModsExist(false, TestConstants.LightMasterModKey)
            .Should().BeFalse();
        listings
            .ModsExist(false, TestConstants.LightMasterModKey2)
            .Should().BeTrue();
        listings
            .ModsExist(true, TestConstants.LightMasterModKey2)
            .Should().BeFalse();
        listings
            .ModsExist(true, TestConstants.LightMasterModKey3)
            .Should().BeTrue();
        listings
            .ModsExist(false, TestConstants.LightMasterModKey3)
            .Should().BeFalse();
        listings
            .ModsExist(true, TestConstants.LightMasterModKey4)
            .Should().BeFalse();
        listings
            .ModsExist(false, TestConstants.LightMasterModKey4)
            .Should().BeFalse();
    }

    [Fact]
    public void Exists_Single()
    {
        var listings = new ModListing[]
        {
            new ModListing(TestConstants.LightMasterModKey, true, existsOnDisk: true),
            new ModListing(TestConstants.LightMasterModKey2, false, existsOnDisk: false),
            new ModListing(TestConstants.LightMasterModKey3, true, existsOnDisk: true),
        };
        listings
            .ModsExist(TestConstants.LightMasterModKey)
            .Should().BeTrue();
        listings
            .ModsExist(TestConstants.LightMasterModKey2)
            .Should().BeFalse();
        listings
            .ModsExist(TestConstants.LightMasterModKey3)
            .Should().BeTrue();
        listings
            .ModsExist(TestConstants.LightMasterModKey4)
            .Should().BeFalse();
    }

    [Fact]
    public void Enabled_Typical()
    {
        var listings = new ModListing[]
        {
            new ModListing(TestConstants.LightMasterModKey, true, existsOnDisk: true),
            new ModListing(TestConstants.LightMasterModKey2, false, existsOnDisk: true),
            new ModListing(TestConstants.LightMasterModKey3, true, existsOnDisk: true),
        };
        listings
            .ModsExist(
                true,
                TestConstants.LightMasterModKey, TestConstants.LightMasterModKey3)
            .Should().BeTrue();
        listings
            .ModsExist(false, TestConstants.LightMasterModKey2)
            .Should().BeTrue();
        listings
            .ModsExist(
                true,
                TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2, TestConstants.LightMasterModKey3)
            .Should().BeFalse();
        listings
            .ModsExist(
                true,
                TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2, TestConstants.LightMasterModKey4)
            .Should().BeFalse();
    }
}